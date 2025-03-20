using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using NL = NLog.Logger;


namespace SignalR_Engine
{

        /*
         * This is the Arc Hub and orchestraizes the calls from selectcare to arc system with olebridge working as an intermediate.
         * 
         * WHAT HUB PROVIDES:
         * hub mantains the pairs and takes no part in activity. hub provides only three(3) functions
         * 
         * - isConnected() : to check if the communication is okay
         * - register(type, userkey) : registers the client to mantain the pair
         * - sendMessage(command, arguments) : main fucntion to send message to the destination. must send the agreed commands or commands are ignored
         * 
         * WHAT NEEDS TO BE IMPLEMENTED BY THE CLIENTS:
         * 
         * - messageReceived(string command, string arg) 
         *      Command handler. All commands are sent in the text format and arguments are sent in string format.
         *      the clients must interpret the commands and execute them. commands are the fixed contract between web and ole bridge. any unknown 
         *      command must be ignored and should not cause any error. errors should be sent only in form of notifications.
         * 
         * - ping() : it returns a long value that indicates the ticks
         *      
         * HOW A CLIENT REGISTERS ITSELF
         * before the messaging can begin, the hub needs to maintain a pair. therefore a client must register itself by using the following
         * register(type, userkey):  registers the client for the pair. 
         *                           type: "bridge"  if the client is an ole bridge  
         *                                 "crm" if the client is the web application
         *                           userKey: the guid of the user/agent
         * 
         * COMMANDS FOR OLE BRDIGE (lower case only)
         * notify : logging comment. argument contains the log message. 
         * newcase: creates the new case. argument is the account id in long integer.
         * opencase: opens the existing case. argument is an alphanumeric id.
         * 
         * 
         * COMMANDS FOR THE SELECT CARE 
         * notify: logging comment. argument contains the log message.
        */

        //[HubName("arcHub")]
    public partial class SelectCareHub : Hub
        {
            #region Data
            const string K_NOTIFY = "notify";

            [Serializable()]
            internal class BridgeConnection
            {
                internal string BridgeId = string.Empty;
                internal string WebAppId = string.Empty;
            }

            private static Dictionary<Guid, BridgeConnection> _bridgeMap = new Dictionary<Guid, BridgeConnection>(64);
            #endregion


            [HubMethodName("register")]
            public void Register(string type, string gid)
            {

                Logger.Debug("Connection Request Received: {0} {1}", type, gid);
                bool bIsBridge = type.Trim().ToLower().ToArray()[0] == 'b';
                Guid userKey = new Guid(gid);
                var Data = _bridgeMap.Where(x => x.Key == userKey).FirstOrDefault();
                string oldId = string.Empty;
                if (Data.Value != null)
                {
                    if (bIsBridge)
                    {
                        oldId = Data.Value.BridgeId;
                        Data.Value.BridgeId = Context.ConnectionId;
                    }
                    else
                    {
                        oldId = Data.Value.WebAppId;
                        Data.Value.WebAppId = Context.ConnectionId;
                    }
                }
                else
                {
                    if (bIsBridge)
                        _bridgeMap[userKey] = new BridgeConnection { BridgeId = Context.ConnectionId, WebAppId = string.Empty };
                    else
                        _bridgeMap[userKey] = new BridgeConnection { BridgeId = string.Empty, WebAppId = Context.ConnectionId };
                }

                if (!string.IsNullOrEmpty(oldId) && oldId != Context.ConnectionId)
                    SendCommand(K_NOTIFY, oldId, "You are now disconnected. Your value has been replaced by a newer connection.");
                if (oldId == Context.ConnectionId)
                    SendCommand(K_NOTIFY, "you are already connected.");
                else
                    SendCommand(K_NOTIFY, "you are now connected.");
            }

            [HubMethodName("isConnected")]
            public async Task<bool> IsConnected()
            {
                bool bAns = false;
                string sourceId = Context.ConnectionId;
                string destid = string.Empty;

                var Data = _bridgeMap.Where(x => x.Value.BridgeId == sourceId || x.Value.WebAppId == sourceId).FirstOrDefault();
                destid = (Data.Value.BridgeId == sourceId) ? Data.Value.WebAppId : Data.Value.BridgeId;

                if (!string.IsNullOrEmpty(destid))
                {
                    long start = DateTime.Now.Ticks, end = 0L, diff = 0L;
                    try
                    {
                        var x = await Clients.Client(destid).ping();
                        if (x != null)
                            end = (long)x;
                        diff = end - start;
                        bAns = true;
                    }
                    catch (Exception ex)
                    {
                        // Handle the cases or slow speed
                    }
                }
                return bAns;
            }

            [HubMethodName("sendMessage")]
            public void SendMessage(string command, string args = "")
            {
                string id = GetDestinationId();
                var t = Task.Factory.StartNew(() => SendCommand(command, id, args));
            }

            #region Internals
            void SendCommand(string command, string args = "")
            {
                string id = Context.ConnectionId;
                SendCommand(command, id, args);
            }
            void SendCommand(string command, string id, string arg = "")
            {
                Logger.Debug("Sending Message: {0} - {1} - {2}", command, id, arg);
                if (id != string.Empty)
                    Clients.Client(id).messageReceived(command, arg);
            }
            string GetDestinationId()
            {
                KeyValuePair<Guid, BridgeConnection> data;
                string destid = GetDestinationId(out data);
                return destid;
            }
            string GetDestinationId(out KeyValuePair<Guid, BridgeConnection> Data)
            {
                string id = Context.ConnectionId;
                Data = _bridgeMap.Where(x => x.Value.WebAppId == id || x.Value.BridgeId == id).FirstOrDefault();
                string destid = Data.Value.BridgeId == id ? Data.Value.WebAppId : Data.Value.BridgeId;
                return destid;
            }

            NL Logger { get { return NLog.LogManager.GetLogger("ArcBriklfasdkfhsdjk"); } }
            #endregion

        }
 }
