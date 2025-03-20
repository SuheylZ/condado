using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Script.Serialization;


using System.Reflection;
using SalesTool.Service.Interfaces;
//using SalesTool.DataAccess.Models;
//using Schema = SalesTool.Schema;

using System.Xml;
using System.Xml.Linq;
//using SalesTool.Schema;
using System.Data;
using System.Text.RegularExpressions;
using System.IO;
using GALEngine_Library;


namespace GalEngine
{
    public class Monitor :
        SalesTool.Service.Interfaces.IClient,
        SalesTool.Service.Interfaces.IClientInformation,
        SalesTool.Service.Interfaces.IClientTask
    {
        #region IClientTask
        public void Init(IServiceDataAccess db, IServiceLog log, string path)
        {

        }
        public void Execute(IServiceDataAccess da, IServiceLog log, string path)
        {
            StartMonitoring();
        }

         public void Dispose() { }
        #endregion

         #region IClientInformation
        public string Name
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;
            }
        }
        public string Version
        {
            get
            {
                return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }
        public string Description
        {
            get
            {
                string Ans = string.Empty;
                Assembly currentAssem = System.Reflection.Assembly.GetExecutingAssembly();
                object[] attribs = currentAssem.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), true);
                if (attribs.Length > 0)
                    Ans = ((AssemblyDescriptionAttribute)attribs[0]).Description;
                return Ans;
            }
        }
        #endregion

        
        #region IClient
        public IClientTask Task { get { return this; } }
        public IClientInformation Information { get { return this; } }
        #endregion

        public void StartMonitoring()
        {
            try
            {
                Logger.log(" --- New Service Execution Started ---");
                //Logger.log("---------------------- Monitoring Started ------------------------------");                
                GALEngineEntities GE = new GALEngineEntities();
                
                List<ACDData> lstACDData = new List<ACDData>();
                List<ACDData> lstACDDataFromDB = new List<ACDData>();
                List<queueAcdStatistics> lstDataFromDB = new List<queueAcdStatistics>();

                List<ACDData> lstACDAddList = new List<ACDData>();
                List<ACDData> lstACDRemovelist = new List<ACDData>();
                List<ACDData> lstACDUpdatedList = new List<ACDData>();


                List<WebData> lstWebData = new List<WebData>();
                List<WebData> lstWebDataFromDB = new List<WebData>();
                List<LeadBasicDisplayForAllAgents_Result> lstWebResult = new List<LeadBasicDisplayForAllAgents_Result>();

                List<WebData> lstWebAddList = new List<WebData>();
                List<WebData> lstwebRemovelist = new List<WebData>();
                List<WebData> lstWebUpdatedList = new List<WebData>();

                List<string> lstAgentIDsToNotify = new List<string>();

                int timetoSleep = 0;
                string msToSleep = ConfigurationManager.AppSettings["MilisecondsToSleep"].ToString();
                int.TryParse(msToSleep, out timetoSleep);
                if (timetoSleep == 0)
                {
                    timetoSleep = 100;
                }

                int maxLoopCount = 0, loopCounter = 0;
                string strLoopCount = ConfigurationManager.AppSettings["NumberOfLoops"].ToString();
                int.TryParse(strLoopCount, out maxLoopCount);
                if (maxLoopCount == 0)
                {
                    maxLoopCount = 100;
                }

                bool isAcdEnabled, isGwbEnabled;
                isAcdEnabled = DataAccess.getAcdGalEnabled();
                isGwbEnabled = DataAccess.getWebGalEnabled();

                var jSerialiser = new JavaScriptSerializer();

                string serializedAcdList = "", serializedGwbList = "";


                if (isAcdEnabled)
                {
                    serializedAcdList = DataAccess.LoadACDListAsJson();
                    if (!string.IsNullOrEmpty(serializedAcdList))
                    {
                        lstACDData = jSerialiser.Deserialize<List<ACDData>>(serializedAcdList);
                    }
                }
                if (isGwbEnabled)
                {
                    serializedGwbList = DataAccess.LoadGWBListAsJson();
                    if (!string.IsNullOrEmpty(serializedGwbList))
                    {
                        lstWebData = jSerialiser.Deserialize<List<WebData>>(serializedGwbList);
                    }
                }                
                while ((isAcdEnabled || isGwbEnabled)&& loopCounter <= maxLoopCount)
                {
                    Logger.log(" --- While Loop Started ---");
                    loopCounter++;
                    //Logger.log("While Loop itteration at " + DateTime.Now.ToString());
                    lstAgentIDsToNotify = new List<string>();
                    if (isAcdEnabled)
                    {
                        lstACDData.ForEach(delegate(ACDData o) { o.MarkSelected = false; });
                        GE.GetACDStatistics();
                        lstDataFromDB = (from dbdata in GE.queue_acd_statistics.AsNoTracking()
                                         select dbdata
                                            ).ToList();
                        lstACDDataFromDB.Clear();
                        //Logger.log("Before foreach (queueAcdStatistics dbdata in lstDataFromDB)");
                        foreach (queueAcdStatistics dbdata in lstDataFromDB)
                        {
                            ACDData acd = new ACDData();
                            acd.AcdAvailableCount = (int)dbdata.AcdCount;
                            acd.AcdTakenCount = (int)dbdata.AcdCallTaken;
                            acd.AcdEnabled = (bool)dbdata.IsEnabled;
                            acd.AgentID = dbdata.AgentID;
                            acd.Reason = dbdata.Reason;
                            acd.MarkSelected = false;
                            lstACDDataFromDB.Add(acd);

                        }
                        //Logger.log("After foreach (queueAcdStatistics dbdata in lstDataFromDB)");
                        //Remove any deleted agents from list
                        lstACDRemovelist = lstACDData.Where(c => !lstACDDataFromDB.Any(d => d.AgentID == c.AgentID)).ToList();
                        lstACDData.RemoveAll(r => lstACDRemovelist.Contains(r));

                        //Add any new agents added
                        lstACDAddList = lstACDDataFromDB.Where(c => !lstACDData.Any(d => d.AgentID == c.AgentID)).ToList();
                        lstACDAddList.ForEach(delegate(ACDData o) { o.MarkSelected = true; });
                        lstACDData.AddRange(lstACDAddList);

                        //check what agents have updates to get notified
                        //Logger.log("Before foreach (ACDData AcdDB in lstACDDataFromDB)");
                        foreach (ACDData AcdDB in lstACDDataFromDB)
                        {
                            ACDData AcdPrev = lstACDData.Where(l => l.AgentID == AcdDB.AgentID).FirstOrDefault();
                            if (AcdPrev != null)
                            {
                                AcdPrev.AcdAvailableCount = AcdDB.AcdAvailableCount;
                                AcdPrev.AcdTakenCount = AcdDB.AcdTakenCount;
                                AcdPrev.AcdEnabled = AcdDB.AcdEnabled;
                            }
                        }
                        //Logger.log("After foreach (ACDData AcdDB in lstACDDataFromDB)");
                        lstACDUpdatedList = lstACDData.Where(a => a.MarkSelected == true).ToList();
                        lstAgentIDsToNotify = lstACDUpdatedList.Select(a => a.AgentID.ToString()).ToList();

                        if (lstAgentIDsToNotify.Any())
                        {
                            SignalRConnection.NotifySignalR(lstAgentIDsToNotify, SignalRMethod.AcdGal);
                        }
                    }

                    lstAgentIDsToNotify.Clear();

                    if (isGwbEnabled)
                    {
                        lstWebData.ForEach(delegate(WebData o) { o.MarkSelected = false; });
                        GE.GetACDStatistics();
                        lstWebResult = GE.LeadBasicDisplayForAllAgents().ToList();
                        lstWebDataFromDB.Clear();
                        //Logger.log("Before foreach (LeadBasicDisplayForAllAgents_Result dbdata in lstWebResult)");
                        foreach (LeadBasicDisplayForAllAgents_Result dbdata in lstWebResult)
                        {
                            WebData web = new WebData();
                            web.WebAvailableCount = (int)dbdata.total_assignable_leads;
                            web.WebDialedCount = (int)dbdata.total_assigned_leads;
                            web.WebEnabled = (bool)dbdata.IsEnabled;
                            web.Reason = dbdata.Reason;
                            web.AgentID = dbdata.agent_l360_username;
                            web.MarkSelected = false;
                            lstWebDataFromDB.Add(web);
                        }
                        //Logger.log("After foreach (LeadBasicDisplayForAllAgents_Result dbdata in lstWebResult)");
                        //Remove any deleted agents from list
                        lstwebRemovelist = lstWebData.Where(c => !lstWebDataFromDB.Any(d => d.AgentID == c.AgentID)).ToList();
                        lstWebData.RemoveAll(r => lstwebRemovelist.Contains(r));

                        //Add any new agents added
                        lstWebAddList = lstWebDataFromDB.Where(c => !lstWebData.Any(d => d.AgentID == c.AgentID)).ToList();
                        lstWebAddList.ForEach(delegate(WebData o) { o.MarkSelected = true; });
                        lstWebData.AddRange(lstWebAddList);

                        //check what agents have updates to get notified
                        //Logger.log("Before foreach (WebData WebDB in lstWebDataFromDB)");
                        foreach (WebData WebDB in lstWebDataFromDB)
                        {
                            WebData WebPrev = lstWebData.Where(l => l.AgentID == WebDB.AgentID).FirstOrDefault();
                            if (WebPrev != null)
                            {
                                WebPrev.WebAvailableCount = WebDB.WebAvailableCount;
                                WebPrev.WebDialedCount = WebDB.WebDialedCount;
                                WebPrev.WebEnabled = WebDB.WebEnabled;
                            }
                        }
                        //Logger.log("After foreach (WebData WebDB in lstWebDataFromDB)");
                        lstWebUpdatedList = lstWebData.Where(a => a.MarkSelected == true).ToList();
                        lstAgentIDsToNotify = lstWebUpdatedList.Select(a => a.AgentID.ToString()).ToList();

                        if (lstAgentIDsToNotify.Any())
                        {
                            SignalRConnection.NotifySignalR(lstAgentIDsToNotify, SignalRMethod.WebGal);
                        }
                    }
                    Logger.log(" --- While Loop End --- sleep for ms: " + timetoSleep);
                    System.Threading.Thread.Sleep(timetoSleep);
                }

                if (isAcdEnabled)
                {
                    serializedAcdList = jSerialiser.Serialize(lstACDData);
                    DataAccess.SaveACDListAsJson(serializedAcdList);
                }

                if (isGwbEnabled)
                {
                    serializedAcdList = jSerialiser.Serialize(lstWebData);
                    DataAccess.SaveGWBListAsJson(serializedGwbList);
                }
            }
            catch (Exception ex)
            {
                Logger.log( System.DateTime.Now.ToString() +  ex.ToString());
            }
        }
    }
}