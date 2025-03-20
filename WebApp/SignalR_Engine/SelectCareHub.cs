using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;
using NLog;
using System.Data.SqlClient;



namespace SignalR_Engine
{
    public class SelectCareHub : Hub
    {
        
        #region arcHubCode


        #region Data
        const string K_NOTIFY = "notify";
        const string K_APP_STORAGE_KEY = "ARCHUBPATH";

        [Serializable()]
        internal class BridgeConnection
        {
            internal string BridgeId = string.Empty;
            internal string WebAppId = string.Empty;
        }

        private static Dictionary<Guid, BridgeConnection> _bridgeMap = new Dictionary<Guid, BridgeConnection>(64);

        public static void RegisterInDatabase(string url)
        {
            // update application_storage for K_APP_STORAGE_KEY
            SignalR_Engine.SignalREntities ent = new SignalREntities();
            ent.Database.Connection.Open();
            ent.Database.ExecuteSqlCommand("UPDATE [application_storage] SET [tvalue]=@value WHERE [Key]=@key",
                new SqlParameter[]{ 
                    new SqlParameter("@key", K_APP_STORAGE_KEY),
                    new SqlParameter("@value", url)}
                 );
        }
        #endregion

        #region ArcHub
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
                    Logger.Error(ex.ToString());
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
        #endregion

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

        NLog.Logger Logger { get { return NLog.LogManager.GetLogger(Konstants.LOGGER_NAME); } }
        #endregion


        #endregion
        
        public ConnectionMappings connectionMappings = new ConnectionMappings();

        static NLog.Logger Writer { get { return NLog.LogManager.GetLogger(Konstants.LOGGER_NAME); } } 
        public void send(string name, string message)
        {
            Clients.All.broadcastMessage(name, message);
        }

        public static void UpdatePersonalQueueCounts(string SkillID)
        {
            try
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SelectCareHub>();

                if (!string.IsNullOrEmpty(SkillID))
                {
                    List<string> lstConnectionIDs = ConnectionMappings.GetConnectionIDsWithSkill(SkillID);
                    if (lstConnectionIDs != null && lstConnectionIDs.Any())
                    {
                        context.Clients.Clients(lstConnectionIDs).UpdateQueueCounts("Message from #UpdateCounts SignalR SelectCareHub to connections: " + lstConnectionIDs.Count);
                        return;
                    }
                }
                context.Clients.All.UpdateQueueCounts("Message from #UpdateCounts SignalR SelectCareHub BroadCast");
            }
            catch (Exception e)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SelectCareHub>();
                context.Clients.All.LogServerError(DateTime.Now.ToString() + " UpdatePersonalQueueCounts with skillID: " + SkillID + " exception: "+  e.ToString());
                
                string msg = " UpdateCounts " + e.ToString();
                //NativeLog.log(msg);
                Writer.Error(msg);
            }
        }
        
        public static void UpdateGWBCounts(List<string> lstUsers)
        {
            try
            {
                List<string> lstConnectionIDs = ConnectionMappings.GetconnectionsList(lstUsers);
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SelectCareHub>();
                if (lstConnectionIDs != null && lstConnectionIDs.Any())
                {
                    context.Clients.Clients(lstConnectionIDs).updateDialerCounts("Message from #UpdateDialerCounts SignalR DialerHub List");
                    //Logger.log(" GWB, Dialer Hub UpdateDialerCounts Called");
                }
                else
                {
                    context.Clients.All.updateDialerCounts("Message from #UpdateDialerCounts SignalR DialerHub BroadCast");
                }
            }
            catch (Exception e)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SelectCareHub>();
                context.Clients.All.LogServerError(DateTime.Now.ToString() + " UpdateGWBCounts, with users list : " + lstUsers.ToString() + " exception: \n" + e.ToString());
                string msg = " UpdateGWBCounts " + e.ToString();
                //NativeLog.log(msg);
                Writer.Error(msg);
            }
        }

        public static void UpdateACDCounts(List<string> lstUsers)
        {
            try
            {
                List<string> lstConnectionIDs = ConnectionMappings.GetconnectionsList(lstUsers);
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SelectCareHub>();

                if (lstConnectionIDs != null && lstConnectionIDs.Any())
                {
                    context.Clients.Clients(lstConnectionIDs).updateAcdCounts("Message from #UpdateACDCounts SignalR DialerHub List");
                    //NativeLog.log(" ACD, Dialer Hub UpdateACDCounts Called");
                    Writer.Trace(" ACD, Dialer Hub UpdateACDCounts Called");
                }
                else
                {
                    context.Clients.All.updateAcdCounts("Message from #UpdateACDCounts SignalR acdHub BroadCast");
                }
            }
            catch (Exception e)
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SelectCareHub>();
                context.Clients.All.LogServerError(DateTime.Now.ToString() + " UpdateACDCounts with users list : " + lstUsers.ToString() + " exception: " + e.ToString());
                //NativeLog.log(" UpdateACDCounts " + e.ToString());
                Writer.Trace(" UpdateACDCounts " + e.ToString());
            }
        }

        public void RegisterNewUser(string userKey)
        {
            try
            {
                SignalR_Bindings user = connectionMappings.AddUser(userKey, Context.ConnectionId);
                if (user != null)
                {
                    Clients.Caller.KeepToken(user.Token);
                }
                else
                {
                    Clients.Caller.LogServerError(DateTime.Now.ToString() + " RegisterNewUser" + " Function returned Null");
                    //NativeLog.log("RegisterNewUser Function returned Null user key = "+ userKey);
                    Writer.Trace("RegisterNewUser Function returned Null user key = " + userKey);

                    Clients.Caller.KeepToken("");
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " RegisterNewUser" + e.ToString());
                //NativeLog.log(" RegisterNewUser \n" + e.ToString());
                Writer.Error(" RegisterNewUser \n" + e.ToString());
            }
        }

        public void UpdateUser(string Token)
        {
            try
            {
                //Logger.log("Updating USER");
                bool success = false;
                if (!string.IsNullOrEmpty(Token))
                {
                    success = connectionMappings.UpdateUser(Token, Context.ConnectionId) != null ? true : false; ;
                }
                if (!success)
                {
                    Clients.Caller.SendRegisterationRequest();
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " UpdateUser " + e.ToString());
                //NativeLog.log(" UpdateUser " + e.ToString());
                Writer.Error(" UpdateUser " + e.ToString());
            }
        }

        public void UpdateUserWithKey(string Token, string Key)
        {
            try
            {
                SignalR_Bindings user = new SignalR_Bindings(); ;
                user = connectionMappings.UpdateUser(Token, Context.ConnectionId, Key);
                if (user != null)
                {
                    Clients.Caller.KeepToken(user.Token);
                }
                else
                {
                    Clients.Caller.LogServerError(DateTime.Now.ToString() + " UpdateUserWithKey.  Function returned Null");
                    //NativeLog.log(" UpdateUserWithKey Function returned Null user key = " + Key);
                    Clients.Caller.KeepToken("");
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " UpdateUserWithKey \n" + e.ToString());
                //NativeLog.log(" UpdateUserWithKey " + e.ToString());
                Writer.Error(" UpdateUserWithKey " + e.ToString());
            }
        }

        public void keepAlive()
        {
            Clients.Caller.KeepAlive(DateTime.Now.ToString() + ": Keep Alive response from SignalR. Connection ID : " + Context.ConnectionId.ToString());
        }

        public override Task OnConnected()
        {
            try
            {
                //NativeLog.log("New connection OnConnected");
                Writer.Trace("New connection OnConnected");
                //Clients.Caller.UpdateQueueCounts("Message Update Personal Queue from SignalR HUB UPON new connection");
                //Clients.Caller.updateDialerCounts("Message Update Dialer Counts from SignalR HUB UPON new connection");
                Clients.Caller.updateAcdCounts("Message Update ACD Counts from SignalR HUB UPON new connection");
                Clients.Caller.SendToken();
                Clients.Caller.KeepAlive(DateTime.Now.ToString() + ": Keep Alive response from SignalR");
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnConnected" + e.ToString());
                //NativeLog.log(" OnConnected " + e.ToString());
                Writer.Error(" OnConnected " + e.ToString());
            }
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            try
            {
                //NativeLog.log("reconnection");
                Writer.Trace("Reconnection");
                //Clients.Caller.UpdateQueueCounts("Message Update Personal Queue from SignalR HUB UPON new connection");
                //Clients.Caller.updateDialerCounts("Message Update Dialer Counts from SignalR HUB UPON new connection");
                Clients.Caller.updateAcdCounts("Message Update ACD Counts from SignalR HUB UPON new connection");
                Clients.Caller.SendToken();
                Clients.Caller.KeepAlive(DateTime.Now.ToString() + ": Keep Alive response from Select care SignalR");
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnConnected" + e.ToString());
                //NativeLog.log(" OnReconnected " + e.ToString());
                Writer.Error(" OnReconnected " + e.ToString());
            }
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool reason)
        {
            try
            {
                //Logger.log("Client disconnected");
                connectionMappings.MarkDisconnectedTime(Context.ConnectionId);
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnDisconnected" + e.ToString());
                //NativeLog.log(" OnDisconnected " + e.ToString());
                Writer.Error(" OnDisconnected " + e.ToString());
            }
            return base.OnDisconnected(reason);
        }
    }
}