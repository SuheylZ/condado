using System;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.AspNet.SignalR.Hubs;


namespace SignalR_Engine
{
    public partial class SelectCareHub : Hub
    {
        public ConnectionMappings connectionMappings = new ConnectionMappings();

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
                context.Clients.All.LogServerError(DateTime.Now.ToString() + " UpdatePersonalQueueCounts with skillID: " + SkillID + " exception: " + e.ToString());
                Logger1.log(" UpdateCounts " + e.ToString());
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
                    //Logger1.log(" GWB, Dialer Hub UpdateDialerCounts Called");
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
                Logger1.log(" UpdateGWBCounts " + e.ToString());
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
                    Logger1.log(" ACD, Dialer Hub UpdateACDCounts Called");
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
                Logger1.log(" UpdateACDCounts " + e.ToString());
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
                    Logger1.log("RegisterNewUser Function returned Null user key = " + userKey);
                    Clients.Caller.KeepToken("");
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " RegisterNewUser" + e.ToString());
                Logger1.log(" RegisterNewUser \n" + e.ToString());
            }
        }

        public void UpdateUser(string Token)
        {
            try
            {
                //Logger1.log("Updating USER");
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
                Logger1.log(" UpdateUser " + e.ToString());
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
                    Logger1.log(" UpdateUserWithKey Function returned Null user key = " + Key);
                    Clients.Caller.KeepToken("");
                }
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " UpdateUserWithKey \n" + e.ToString());
                Logger1.log(" UpdateUserWithKey " + e.ToString());
            }
        }

        public void keepAlive()
        {
            Clients.Caller.KeepAlive(DateTime.Now.ToString() + ": Keep Alive response from SignalR. Connection ID : " + Context.ConnectionId.ToString());
            //Clients.All.KeepAlive(DateTime.Now.ToString() + ": Keep Alive response from SignalR BroadCast");
        }

        public override Task OnConnected()
        {
            try
            {
                Logger1.log("New connection OnConnected");
                //already getting updates on page load
                //Clients.Caller.UpdateQueueCounts("Message Update Personal Queue from SignalR HUB UPON new connection");
                //Clients.Caller.updateDialerCounts("Message Update Dialer Counts from SignalR HUB UPON new connection");
                Clients.Caller.updateAcdCounts("Message Update ACD Counts from SignalR HUB UPON new connection");
                Clients.Caller.SendToken();
                Clients.Caller.KeepAlive(DateTime.Now.ToString() + ": Keep Alive response from SignalR");
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnConnected" + e.ToString());
                Logger1.log(" OnConnected " + e.ToString());
            }
            return base.OnConnected();
        }

        public override Task OnReconnected()
        {
            try
            {
                Logger1.log("re connection");
                //already getting updates on page load
                //Clients.Caller.UpdateQueueCounts("Message Update Personal Queue from SignalR HUB UPON new connection");
                //Clients.Caller.updateDialerCounts("Message Update Dialer Counts from SignalR HUB UPON new connection");
                Clients.Caller.updateAcdCounts("Message Update ACD Counts from SignalR HUB UPON new connection");
                Clients.Caller.SendToken();
                Clients.Caller.KeepAlive(DateTime.Now.ToString() + ": Keep Alive response from Select care SignalR");
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnConnected" + e.ToString());
                Logger1.log(" OnConnected " + e.ToString());
            }
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool reason)
        {
            try
            {
                //Logger1.log("Client disconnected");
                connectionMappings.MarkDisconnectedTime(Context.ConnectionId);
            }
            catch (Exception e)
            {
                Clients.Caller.LogServerError(DateTime.Now.ToString() + " OnDisconnected" + e.ToString());
                Logger1.log(" OnDisconnected " + e.ToString());
            }
            return base.OnDisconnected(reason);
        }
    }
}