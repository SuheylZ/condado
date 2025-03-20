using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SignalR_Engine
{
    public class CentralConnectionManager
    {


        //ACDConnectionMappings acd = new ACDConnectionMappings();
        //DialerConnectionMappings gwb = new DialerConnectionMappings();
        //ConnectionMappings personal = new ConnectionMappings();

        public static void UpdateConnectionID(string token, string connectionID)
        {
            try
            {
                SignalREntities db = new SignalREntities();
                SignalR_ACD_Bindings acdBinding = (from a in db.SignalR_ACD_Bindings
                                                   where a.Token == token
                                                   select a).FirstOrDefault();
                if (acdBinding != null)
                {
                    acdBinding.ConnectionID = connectionID;
                    db.SaveChanges();
                }

                SignalR_GWB_Bindings gwbBinding = (from a in db.SignalR_GWB_Bindings
                                                   where a.Token == token
                                                   select a).FirstOrDefault();
                if (gwbBinding != null)
                {
                    gwbBinding.ConnectionID = connectionID;
                    db.SaveChanges();
                }

                SignalR_Bindings personalBinding = (from a in db.SignalR_Bindings
                                                    where a.Token == token
                                                    select a).FirstOrDefault();
                if (personalBinding != null)
                {
                    personalBinding.ConnectionID = connectionID;
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                
                Logger.log(e.ToString());
            }
        }

    }
}