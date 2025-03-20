using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SignalR_Engine
{
    public class ACDHub : Hub
    {
        public ACDConnectionMappings acdConnectionMappings = new ACDConnectionMappings();

        public void send(string name, string message)
        {
            Clients.All.broadcastMessageACD(name, message + Context.ConnectionId);
        }

        public static void UpdateACDCounts()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ACDHub>();
            context.Clients.All.updateDialerCounts("Message from #UpdateACDCounts SignalR DialerHub BroadCast");
            //Logger.log(" ACD, UpdateACDCounts Broadcast");
        }

        public static void UpdateACDCounts(List<string> lstUsers)
        {
            List<string> lstConnectionIDs = ACDConnectionMappings.GetconnectionsList(lstUsers);
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<ACDHub>();
            if (lstConnectionIDs != null && lstConnectionIDs.Any())
            {
                context.Clients.Clients(lstConnectionIDs).updateDialerCounts("Message from #UpdateACDCounts SignalR DialerHub List");
                //Logger.log(" ACD, Dialer Hub UpdateACDCounts Called");
            }
            else
            {
                //context.Clients.All.updateDialerCounts("Message from #UpdateACDCounts SignalR DialerHub Broadcast");
                UpdateACDCounts();
            }

        }

        public void RegisterNewUser(string userKey)
        {
            try
            {
                SignalR_ACD_Bindings user = acdConnectionMappings.AddUser(userKey, Context.ConnectionId);
                if (user != null)
                {
                    Clients.Caller.KeepToken(user.Token);
                }
                else
                {
                    Clients.All.LogServerError(DateTime.Now.ToString() + " RegisterNewUser" + " Function returned Null");
                    Logger.log(" ACD, RegisterNewUser " + " Function returned Null");
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " RegisterNewUser" + e.ToString());
                Logger.log(" ACD, RegisterNewUser \n" + e.ToString());
            }
        }

        public void UnregisterUser(string Token)
        {
            try
            {
                acdConnectionMappings.RemoveUser(Token);
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " UnregisterUser" + e.ToString());
                Logger.log(" ACD, UnregisterUser " + e.ToString());
            }
        }

        public void UpdateUser(string Token)
        {
            try
            {
                Logger.log(" ACD, Updating USER");
                bool success = false;
                if (!string.IsNullOrEmpty(Token))
                {
                    success = acdConnectionMappings.UpdateUser(Token, Context.ConnectionId);
                }
                if (!success)
                {
                    Clients.Caller.SendRegisterationRequest();
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " UpdateUser " + e.ToString());
                Logger.log(" ACD,  UpdateUser " + e.ToString());
            }
        }

        public void UpdateUserWithKey(string Token, string Key)
        {
            try
            {
                Logger.log(" ACD, UpdateUserWithKey");
                if (!string.IsNullOrEmpty(Token))
                {
                    acdConnectionMappings.UpdateUser(Token, Context.ConnectionId, Key);
                }
            }
            catch (Exception e)
            {
                Clients.All.LogServerError(" " + DateTime.Now.ToString() + " UpdateUserWithSkill " + e.ToString());
                Logger.log(" ACD,  UpdateUserWithKey " + e.ToString());
            }
        }

        public override Task OnConnected()
        {
            try
            {
                acdConnectionMappings.MarkConnectedTime(Context.ConnectionId);
                Clients.Caller.SendToken();
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnConnected, Exception: " + e.ToString());
                Logger.log(" ACD,  OnConnected " + e.ToString());
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool reason)
        {
            try
            {
                acdConnectionMappings.MarkDisconnectedTime(Context.ConnectionId);
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnDisconnected, Exception: " + e.ToString());
                Logger.log(" ACD,  OnDisconnected " + e.ToString());
            }
            return base.OnDisconnected(reason);
        }
    }
}