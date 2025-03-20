using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;

namespace SignalR_Engine
{
    public class DialerHub : Hub
    {
        public DialerConnectionMappings dialerConnectionMappings = new DialerConnectionMappings();

        public void send(string name, string message)
        {
            Clients.All.broadcastMessage1212(name, message + Context.ConnectionId);
        }

        public static void UpdateDialerCounts()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DialerHub>();
            context.Clients.All.updateDialerCounts("Message from #UpdateDialerCounts SignalR DialerHub BroadCast");
            //Logger.log(" GWB, Dialer Hub UpdateDialerCounts Called");
        }

        public static void UpdateDialerCounts(List<string> lstConnectionIDs)
        {
            if (lstConnectionIDs != null && lstConnectionIDs.Any())
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DialerHub>();
                context.Clients.Clients(lstConnectionIDs).updateDialerCounts("Message from #UpdateDialerCounts SignalR DialerHub List");
                //Logger.log(" GWB, Dialer Hub UpdateDialerCounts Called");
            }
            else
            {
                UpdateDialerCounts();
            }
        }

        //public static void UpdateDialerSettings(List<string> lstConnectionIDs = null)
        //{
        //    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<DialerHub>();
        //    if (lstConnectionIDs != null && lstConnectionIDs.Any())
        //    {
        //        context.Clients.Clients(lstConnectionIDs).updateSettings("Message from #UpdateDialerSettings SignalR DialerHub List");
        //        //Logger.log(" GWB, Dialer Hub UpdateDialerCounts Called");
        //    }
        //    else
        //    {
        //        context.Clients.All.updateSettings("Message from #UpdateDialerSettings SignalR DialerHub List");
        //    }
        //}

        public void RegisterNewUser(string userKey)
        {
            try
            {
                SignalR_GWB_Bindings user = dialerConnectionMappings.AddUser(userKey, Context.ConnectionId);
                if (user != null)
                {
                    Clients.Caller.KeepToken(user.Token);
                }
                else
                {
                    Clients.All.LogServerError(DateTime.Now.ToString() + " RegisterNewUser Function returned Null");
                    Logger.log(" GWB, RegisterNewUser Function returned Null");
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " RegisterNewUser" + e.ToString());
                Logger.log(" GWB, RegisterNewUser \n" + e.ToString());
            }
        }

        public void UnregisterUser(string Token)
        {
            try
            {
                dialerConnectionMappings.RemoveUser(Token);
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " UnregisterUser" + e.ToString());
                Logger.log(" GWB, UnregisterUser \n" + e.ToString());
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
                    success = dialerConnectionMappings.UpdateUser(Token, Context.ConnectionId);
                }
                if (!success)
                {
                    Clients.Caller.SendRegisterationRequest();
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " UpdateUser " + e.ToString());
                Logger.log(" GWB, UpdateUser \n" + e.ToString());
            }
        }

        public void UpdateUserWithKey(string Token, string Key)
        {
            try
            {
                //Logger.log("Update user with skill");
                if (!string.IsNullOrEmpty(Token))
                {
                    dialerConnectionMappings.UpdateUser(Token, Context.ConnectionId, Key);
                }
            }
            catch (Exception e)
            {
                Clients.All.LogServerError(DateTime.Now.ToString() + " UpdateUserWithSkill" + e.ToString());
                Logger.log(" GWB, UpdateUserWithKey \n" + e.ToString());
            }
        }

        public override Task OnConnected()
        {
            try
            {
                dialerConnectionMappings.MarkConnectedTime(Context.ConnectionId);
                //Logger.log(" GWB, New connection");
                Clients.Caller.SendToken();
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnConnected, Exception: " + e.ToString());
                Logger.log(" GWB, OnConnected \n " + e.ToString());
            }
            return base.OnConnected();
        }

        public override Task OnDisconnected(bool reason)
        {
            try
            {
                //Logger.log("Client disconnected");
                dialerConnectionMappings.MarkDisconnectedTime(Context.ConnectionId);
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnDisconnected, Exception: " + e.ToString());
                Logger.log(" GWB, OnDisconnected \n" + e.ToString());
            }
            return base.OnDisconnected(reason);
        }


    }
}