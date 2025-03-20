/*The purpose of the class is to improve the performace of the GAL_Engine
 * Designed By Muzamil H;
 * Date 19 Aug 2014
 * futher improvments are required. 1/5 completed 
 * devleopment Stoped on client's request.
 */

using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNet.SignalR;

namespace SignalR_Engine.GAL_Engine
{
    public class GAL_AcdEngine
    {
        private static IHubContext hubContext;
        public static void StartACDEngine()
        {
            hubContext = GlobalHost.ConnectionManager.GetHubContext<SelectCareHub>();
            AgentChangeTracker<gal_login_acd_statistics> acdTracker = new AgentChangeTracker<gal_login_acd_statistics>();
            while (true)
            {
                var stop = Stopwatch.StartNew();
                StartAcdWork(acdTracker);
                stop.Stop();
                var time = stop.Elapsed.ToString();
                System.Threading.Thread.Sleep(SleepTIme);
            }

        }

        public static int SleepTIme
        {
            get
            {
                var t = System.Configuration.ConfigurationManager.AppSettings["SleepTime"];
                int time = 1000;
                if (!string.IsNullOrEmpty(t))
                {
                    int.TryParse(t, out time);
                }
                return time;
            }
        }

        private static void StartAcdWork(AgentChangeTracker<gal_login_acd_statistics> tracker)
        {
            using (var GE = new SignalREntities())
            {
                
                GE.GetACDStatistics();
                var q = GE.gal_login_acd_statistics.AsNoTracking().ToList();
                tracker.ApplyData(q);
            }
            // count will show those which has changes.
            if (tracker.Count > 0)
            {
                // var agents = tracker.GetAgnetsToNotify();
                // because application is not receiving object but only single then its suitable to brodcast. all connected
                // to reduce overhead by hitting database also signalr does't pull out connection instantly so.
                hubContext.Clients.All.updateDialerCounts("Message from #UpdateACDCounts SignalR DialerHub List");
                //SignalRConnection.NotifySignalR(agents, SignalRMethod.AcdGal);
            }

        }
        //private static List<string> GetConnectionIds 

        //public static void StartWebEngine()
        //{

        //    AgentChangeTracker<LeadBasicDisplayAllAgents_Result> webTracker = new AgentChangeTracker<LeadBasicDisplayAllAgents_Result>();
        //    while (true)
        //    {
        //        var stop = Stopwatch.StartNew();
        //        StartwebWork(webTracker);
        //        stop.Stop();
        //        var time = stop.Elapsed.ToString();
        //        System.Threading.Thread.Sleep(SleepTIme);
        //    }

        //}
        //private static void StartwebWork(AgentChangeTracker<LeadBasicDisplayAllAgents_Result> tracker)
        //{
        //    using (var GE = new GALEngineEntities())
        //    {
        //        var q = GE.LeadBasicDisplayForAllAgents().ToList();
        //        tracker.ApplyData(q);
        //    }
        //    if (tracker.Count > 0)
        //    {
        //        var agents = tracker.GetAgnetsToNotify();
        //        SignalRConnection.NotifySignalR(agents, SignalRMethod.WebGal);
        //    }

        //}
    }
}