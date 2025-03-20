/*
 * Filename:  ArcClient
 * Author:    MH
 * Purpose: This class is responsible of Querying Data, Creating valid request for Arc APi, sending and handling 
 * of request to Api.
 *          
      Initial Implementation:  18:12:2013
      
 */

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Arc;
using SalesTool.DataAccess.Models;
using SelectCare.ARC.Client.Config;
using SelectCare.ArcApi;

namespace SelectCare.ARC.Client
{
    public class ArcClient:IDisposable
    {
        #region Properties

        private List<StateModel> States
        {
            get
            {
                return _states ?? (
                                      //_states = Engine.ManageStates.GetAll().ToList()); 
                                      _states = Engine.ArcActions.GetAllStates());
            }
        }

        private List<CampaignModel> Campaigns
        {
            get
            {
                return _campaigns ?? (
                    //_campaigns = Engine.ManageCampaignActions.GetAll().ToList()); 
                                        _campaigns = Engine.ArcActions.GetAllCampaign());
            }
        }

        public int RequestSize
        {
            get
            {
                if (_requestSize == -1)
                    _requestSize = ApplicationSettings.RequestSize;
                return _requestSize;
            }
        }

        public RequestLogin Login
        {
            get
            {
                if (_login == null)
                {
                    _login = new RequestLogin()
                        {
                            //UserId = ApplicationSettings.UserId,
                            //Password = ApplicationSettings.Password,
                            UserId = Engine.ApplicationSettings.ARC_API_USER_ID,
                            Password = Engine.ApplicationSettings.ARC_API_USER_PASSWORD,
                        };
                }
                return _login;
            }
        }

        public string BaseAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_baseAddress))
                    _baseAddress = Engine.ApplicationSettings.ARC_API_BASE_ADDRESS;
                    //_baseAddress = ApplicationSettings.BaseUrl;
                return _baseAddress;
            }
        }

        public string ConnectionString { get; set; }
        private RequestLogin _login;

        private int _requestSize = -1;
        protected DBEngine Engine = null;

        private List<StateModel> _states = null;
        private List<CampaignModel> _campaigns = null;

        private string _baseAddress;

        #endregion

        #region Constructors.

        /// <summary>
        /// Initialize Database Engine
        /// </summary>
        /// <param name="engine"></param>
        public ArcClient(DBEngine engine)
        {

            Engine = engine;
        }

        /// <summary>
        /// Initialize Database engine with given connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public ArcClient(string connectionString)
        {
            Engine = new DBEngine();
            ConnectionString = connectionString;
            Engine.InitLeadsContext(connectionString);
        }

        #endregion

        #region Invoking Methods

        /// <summary>
        /// This function will retrieve Agent Changed data from database by page size (given as property) creates request and
        /// sent to the arc web api.
        /// For Data Access <see cref="ArcActions.GetChangedAgentArcCases"/>
        /// For request creation <see cref="CreateChangeAgentRequest"/>
        /// For posting request <see cref="ArcClientHelper.PostChangeAgent"/>
        /// <summary>
        /// Sends all Arc cases references under Account in AccountHistory with comments UserAssigned.
        /// Account's user initial, and modification date,
        /// </summary>
        /// <author>MH.</author>
        /// </summary>
        public void InvokeChangeAgent()
        {
            try
            {
                RequestLogger.WriteNotification("ChangeAgent job initiated");
                int count = Engine.ArcActions.GetChangedAgentArcCases().Count();
                if (count > 0)
                {
                    RequestLogger.WriteLog(string.Format("Creating ChangeAgent request for {0} Agents", count));
                    string fileName = DateTime.Now.ToFileTime().ToString();
                    var pages = count / RequestSize + 1;
                    for (int i = 1; i <= pages; i++)
                    {
                        var result =
                            Engine.ArcActions.GetChangedAgentArcCases()
                                   .OrderBy(p => p.AccountHistoryId)
                                   .Page(i, RequestSize)
                                   .ToList();
                        if (result.Any())
                        {

                            RequestLogger.WriteLog(string.Format("Creating ChangeAgent request for Page {0}...", i));
                            ArcRequest request = CreateChangeAgentRequest(result);
                            if (ApplicationSettings.Debug)
                            {
                                string requestFileName = fileName + "_" + i + ".xml";
                                RequestLogger.WriteRequest(request, "ChangeAgent_" + requestFileName);
                            }
                            var task = ArcClientHelper.PostChangeAgent(BaseAddress, request).Result; //block call
                          

                            var response = HandleLeadResponseEx(task,
                                                              "ChangeAgent_" + fileName + "_" + i + "_response.xml");

                            string ids = GetCommaSeparatedReferences(response);
                            if (!string.IsNullOrEmpty(ids))
                            {
                                //TODO very tricky. because Account history id is not known.
                                //NOTE:
                                int affected =
                                    InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryChangeAgentDeliveryStatus", ids);
                            }

                        }

                    }
                }
                else
                {
                    RequestLogger.WriteNotification("No data found");
                }
                RequestLogger.WriteNotification("ChangeAgent method completed.");
            }
            catch (Exception ex)
            {
                RequestLogger.WriteError("Method Name: ChangeAgent");
                GetMessageRecursively(ex);
            }


        }


        /// <summary>
        /// This function will retrieve Action Data from AccountHistory based on entry type 1
        /// For Data Access <see cref="ArcActions.GetArcActions"/>
        /// For Request Creation <see cref="CreateAddActionRequestByGroupByRef"/>
        /// For Posting Request <see cref="ArcClientHelper.PostAddAction"/>
        /// SP proj_Arc_UpdateAccountHistoryDeliveryStatusByKey
        /// </summary>
        public void InvokeCreateAction()
        {

            try
            {
                RequestLogger.WriteNotification("CreateAction job initiated");
                //int count = _engine.ArcActions.GetArcActions().Count();
                int count = Engine.ArcActions.GetArcActions().GroupBy(p => p.Reference).Count();
                if (count > 0)
                {
                    RequestLogger.WriteLog(string.Format("Creating AddAction request for {0} actions", count));
                    var pages = count / RequestSize + 1;
                    string fileName = DateTime.Now.ToFileTime().ToString();
                    for (int i = 1; i <= pages; i++)
                    {
                        var groupByRef =
                            Engine.ArcActions.GetArcActions()
                                   .GroupBy(p => p.Reference)
                                   .OrderBy(p => p.Key)
                                   .Page(i, RequestSize)
                                   .ToList();
                        if (groupByRef.Any())
                        {
                            string requestFileName = fileName + "_" + i;
                            RequestLogger.WriteLog(string.Format("Creating AddAction request for Page #[ {0} ] ", i));
                            ArcRequest request = CreateAddActionRequestByGroupByRef(groupByRef);

                            if (ApplicationSettings.Debug)
                            {
                                RequestLogger.WriteRequest(request, "CreateAction_" + requestFileName + ".xml");
                            }
                            var task = ArcClientHelper.PostAddAction(BaseAddress, request).Result; //block call
                            //var response = HandleLeadActionResponse(task, "CreateAction_" + fileName + "_response.xml");
                            var response = HandleLeadResponseEx(task, "CreateAction_" + requestFileName + "_response.xml");

                            string ids = GetCommaSeparatedIdsFromActions(response);
                            if (!string.IsNullOrEmpty(ids))
                            {
                                int affected = InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryDeliveryStatusByKey",
                                                                    ids);

                            }
                            // Lead reference sub status will play role as well

                        }
                    }
                }
                else
                {
                    RequestLogger.WriteNotification("No data found");
                }
                RequestLogger.WriteNotification("CreateAction method completed...");
            }
            catch (Exception ex)
            {
                RequestLogger.WriteError("Method Name: CreateAction");
                GetMessageRecursively(ex);
            }
        }



        /// <summary>
        ///  This function will retrieve StopLetter data from database by page size (given as property)
        ///  creates request and send to the arc web api.
        /// Data retrieving query<see cref="ArcActions.GetArcCasesForStopLetters"/>
        /// Request Generation<see cref="CreateLeadPostStopLetterRequest"/>
        /// Posting Data <see cref="ArcClientHelper.PostAddAction"/>
        /// For Delivery proj_Arc_UpdateStopLetterDeliveryStatus
        /// </summary>
        public void InvokeLeadStopLetterRequest()
        {
            RequestLogger.WriteLog("StopLetter job initiated...");
            try
            {
                int count = Engine.ArcActions.GetArcCasesForStopLetters().Count();
                if (count > 0)
                {
                    RequestLogger.WriteLog(string.Format("Creating StopLetters request for [ {0} ]", count));
                    var pages = count / RequestSize + 1;
                    string fileName = DateTime.Now.ToFileTime().ToString();
                    for (int i = 1; i <= pages; i++)
                    {
                        var result = Engine.ArcActions.GetArcCasesForStopLetters().OrderBy(p => p.Key).Page(i, RequestSize).ToList();
                        if (result.Any())
                        {
                            string requestFileName = fileName + "_" + i;
                            RequestLogger.WriteLog(string.Format("Creating StopLetters request of page [ {0} ]", i));
                            ArcRequest request = CreateLeadPostStopLetterRequest(result);
                            if (ApplicationSettings.Debug)
                            {
                                RequestLogger.WriteRequest(request, "StopLetters_" + requestFileName + ".xml");
                            }
                            var task = ArcClientHelper.PostStopLetter(BaseAddress, request).Result; //thread blocking call
                            //var response = HandleLeadResponse(task, "StopLetters_" + requestFileName + "_response.xml");
                            var response = HandleLeadResponseEx(task, "StopLetters_" + requestFileName + "_response.xml");
                            if (response != null && response.LeadResults.Any())
                            {
                                string ids = GetCommaSeparatedReferences(response);
                                if (!string.IsNullOrEmpty(ids))
                                {
                                    RequestLogger.WriteDeliveredIds(ids);
                                    int affected = InvokeStoreProcedure("proj_Arc_UpdateStopLetterDeliveryStatus", ids);

                                }
                            }
                        }
                    }

                }
                else
                {
                    RequestLogger.WriteNotification("No data found");
                }
                RequestLogger.WriteNotification("StopLetter method completed...");

            }
            catch (Exception ex)
            {
                RequestLogger.WriteError("Method Name: StopLetter");
                GetMessageRecursively(ex);
            }
        }



        /// <summary>
        /// This function will retrieve Log Letter data from database by page size (given as property)
        /// For Data retrieving <see cref="ArcActions.GetArcLetterLogs"/>
        /// For Request Generation <see cref="CreateLogLetterRequest"/>
        /// For Posting Request <see cref="ArcClientHelper.PostLetterLog"/>
        /// For Persistence EmailQueu devlivery status 
        /// Sp:proj_Arc_UpdateLetterLogDeliveryStatus
        /// creates request and send to the arc web api.
        /// </summary>
        public void InvokeLeadLogLetterRequest()
        {
            RequestLogger.WriteLog("LogLetters job initiated...");
            try
            {
                int count = Engine.ArcActions.GetArcLetterLogs().GroupBy(p => p.Reference).Count();
                if (count > 0)
                {
                    RequestLogger.WriteLog(string.Format("Creating LogLetters request for [ {0} ]", count));
                    var pages = count / RequestSize + 1;
                    string fileName = DateTime.Now.ToFileTime().ToString();
                    for (int i = 1; i <= pages; i++)
                    {
                        var result =
                            Engine.ArcActions.GetArcLetterLogs()
                                   .GroupBy(p => p.Reference)
                                   .OrderBy(p => p.Key)
                                   .Page(i, RequestSize)
                                   .ToList();
                        if (result.Any())
                        {
                            string requestFileName = fileName + "_" + i;
                            RequestLogger.WriteLog(string.Format("Creating LogLetters request of page [ {0} ]", i));
                            ArcRequest request = CreateLogLetterRequest(result);
                            if (ApplicationSettings.Debug)
                            {
                                RequestLogger.WriteRequest(request, "LogLetters_" + requestFileName + ".xml");
                            }
                            var task = ArcClientHelper.PostLetterLog(BaseAddress, request).Result;
                            //thread blocking call
                            //var response = HandleLeadResponse(task, "LogLetters_" + requestFileName + "_response.xml");
                            var response = HandleLeadResponseEx(task, "LogLetters_" + requestFileName + "_response.xml");
                            string ids = GetCommaSeparatedIdsFromMessage(response);
                            if (!string.IsNullOrEmpty(ids))
                            {
                                RequestLogger.WriteDeliveredIds(ids);
                                int affected = InvokeStoreProcedure("proj_Arc_UpdateLetterLogDeliveryStatus", ids);
                                RequestLogger.WriteLog(string.Format("{0} Successfully delivered to Arc System",
                                                                     affected));
                            }
                        }
                    }
                }
                else
                {
                    RequestLogger.WriteNotification("No data found");
                }
                RequestLogger.WriteNotification("LogLetters method completed...");
            }
            catch (Exception ex)
            {
                RequestLogger.WriteError("Method Name: LogLetters");
                GetMessageRecursively(ex);
            }
        }




        /// <summary>
        /// This function will retrieve Log Letter data from database by page size (given as property)
        /// For Data retrieving 
        /// </summary>
        public void InvokeLeadUpdateConsentRequest()
        {
            RequestLogger.WriteLog("UpdateConsent job initiated...");
            try
            {
                int count = Engine.ArcActions.GetArcConsentUpdate().Count();
                if (count > 0)
                {
                    RequestLogger.WriteLog(string.Format("Creating UpdateConsent request for [ {0} ]", count));
                    var pages = count / RequestSize + 1;
                    string fileName = DateTime.Now.ToFileTime().ToString();
                    for (int i = 1; i <= pages; i++)
                    {
                        var result =
                            Engine.ArcActions.GetArcConsentUpdate().OrderBy(p => p.Key).Page(i, RequestSize).ToList();
                        if (result.Any())
                        {
                            string requestFileName = fileName + "_" + i;
                            RequestLogger.WriteLog(string.Format("Creating UpdateConsent request of page [ {0} ]", i));
                            ArcRequest request = CreateLeadUpdateRequest(result);
                            if (ApplicationSettings.Debug)
                            {
                                RequestLogger.WriteRequest(request, "UpdateConsent_" + requestFileName + ".xml");
                            }
                            var task = ArcClientHelper.PostUpdate(BaseAddress, request).Result; //thread blocking call
                            //var response = HandleLeadConsentResponse(task,
                            //                                         "UpdateConsent_" + requestFileName +
                            //                                         "_response.xml");
                            var response = HandleLeadResponseEx(task,
                                                                     "UpdateConsent_" + requestFileName +
                                                                     "_response.xml");
                            string ids = GetCommaSeparatedReferences(response);
                            if (!string.IsNullOrEmpty(ids))
                            {
                                RequestLogger.WriteDeliveredIds(ids);
                                int updated = InvokeStoreProcedure("proj_Arc_UpdateConsentDeliveryStatus", ids);
                            }
                            //TODO: presist 
                        }
                    }
                }
                else
                {
                    RequestLogger.WriteNotification("No data found");
                }
                RequestLogger.WriteNotification("UpdateConsent method completed...");
            }
            catch (Exception ex)
            {
                RequestLogger.WriteError("Method Name: UpdateConsent");
                GetMessageRecursively(ex);
            }
        }

        // Changed on John request
        // [2:19:27 AM] John Dobrotka: Muzammil, i sent you the base Query for CreateOps
        ///// <summary>
        ///// This function will retrieve data for opportunities from database by page size (given as property)
        ///// For Data Access <see cref="ArcActions.GetNonDeliveredArcOpportunities"/>
        ///// For Request Generation <see cref="CreateLeadOpportunities"/>
        ///// For Request Posting <see cref="ArcClientHelper.PostCreateOp"/>
        ///// For Persistence AccountHistory delivered status
        ///// proj_Arc_UpdateAccountHistoryDeliveryStatusByKey
        ///// </summary>
        //public void InvokeLeadOpportunitiesRequest()
        //{
        //    RequestLogger.WriteLog("CreatedOp task initiated...");
        //    try
        //    {
        //        int count = Engine.ArcActions.GetNonDeliveredArcOpportunities().Count();
        //        if (count > 0)
        //            RequestLogger.WriteLog(string.Format("Creating CreatedOp request for [ {0} ]", count));
        //        var pages = count / RequestSize + 1;
        //        string fileName = DateTime.Now.ToFileTime().ToString();
        //        for (int i = 1; i <= pages; i++)
        //        {
        //            var result = Engine.ArcActions.GetNonDeliveredArcOpportunities().OrderBy(p => p.AccountHistoryId).Page(i, RequestSize).ToList();
        //            if (result.Any())
        //            {
        //                string requestFileName = fileName + "_" + i;
        //                RequestLogger.WriteLog(string.Format("Creating CreatedOp request of page [ {0} ]", i));
        //                OpRequest request = CreateLeadOpportunities(result);
        //                if (ApplicationSettings.Debug)
        //                {
        //                    RequestLogger.WriteRequest(request, "CreatedOp_" + requestFileName + ".xml");
        //                }
        //                var task = ArcClientHelper.PostCreateOp(BaseAddress, request).Result; //thread blocking call
        //                var response = HandleOpResponse(task, "CreatedOp_" + requestFileName + "_response.xml");
        //                string ids = GetCommaSeparatedIds(response);
        //                if (!string.IsNullOrEmpty(ids))
        //                {
        //                    RequestLogger.WriteDeliveredIds(ids);
        //                    int affected = InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryDeliveryStatusByKey", ids);
        //                    RequestLogger.WriteLog(string.Format("{0} Successfully delivered to Arc System", affected));
        //                }
        //            }
        //        }
        //        RequestLogger.WriteNotification("CreatedOp method completed...");
        //    }
        //    catch (Exception ex)
        //    {
        //        RequestLogger.WriteError("Method Name: CreatedOp");
        //        GetMessageRecursively(ex);
        //    }
        //}

        //public void InvokeCreateOrUpdateLeadRequest()
        //{
        //    RequestLogger.WriteLog("CreateOrUpdateLeadRequest job initiated...");
        //    //TODO: no filtration critra exists 
        //    try
        //    {
        //        int count = Engine.ArcActions.GetAll().Count();
        //        if (count > 0)
        //            RequestLogger.WriteLog(string.Format("Creating CreateOrUpdateLeadRequest request for [ {0} ]", count));
        //        var pages = count / 1 + 1;
        //        string fileName = DateTime.Now.ToFileTime().ToString();
        //        for (int i = 1; i <= pages; i++)
        //        {
        //            var result = Engine.ArcActions.GetAll().OrderBy(p => p.Key).Page(i, 1).ToList();
        //            if (result.Any())
        //            {
        //                string requestFileName = fileName + "_" + i;
        //                RequestLogger.WriteLog(string.Format("Creating CreateOrUpdateLeadRequest request of page [ {0} ]", i));
        //                ArcRequest request = CreateCreateOrUpdateLeadRequest(result);
        //                if (ApplicationSettings.Debug)
        //                {
        //                    RequestLogger.WriteRequest(request, "CreateOrUpdateLeadRequest_" + requestFileName + ".xml");
        //                }
        //                var task = ArcClientHelper.PostInsertUpdateLeadRequest(BaseAddress, request).Result; //thread blocking call
        //                HandleResponse(task, "CreateOrUpdateLeadRequest_" + requestFileName + "_response.xml");

        //                //TODO: presist 
        //            }
        //        }
        //        RequestLogger.WriteNotification("UpdateConsent method completed...");
        //    }
        //    catch (Exception ex)
        //    {
        //        RequestLogger.WriteError("Method Name: UpdateConsent");
        //        GetMessageRecursively(ex);
        //    }
        //}

        public void InvokeLeadOpportunitiesRequest(DateTime startDate,DateTime endDate)
        {
            RequestLogger.WriteLog("CreatedOp task initiated...");
            try
            {
                var result = Engine.ArcActions.GetCreateOpData(startDate, endDate);
                int count = result.Count;
                if(count>0)
                    RequestLogger.WriteLog(string.Format("Creating CreatedOp request for [ {0} ]", count));
                else
                {
                    RequestLogger.WriteNotification("No data found");
                }
                var pages = count / RequestSize + 1;
                string fileName = DateTime.Now.ToFileTime().ToString();
                for (int i = 1; i <= pages; i++)
                {
                    var subSet=result.OrderBy(p => p.ID).Page(i, RequestSize).ToList();
                    if (subSet.Any())
                    {
                        string requestFileName = fileName + "_" + i;
                        RequestLogger.WriteLog(string.Format("Creating CreatedOp request of page [ {0} ]", i));
                        OpRequest request = CreateLeadOpportunities(subSet);
                        if (ApplicationSettings.Debug)
                        {
                            RequestLogger.WriteRequest(request, "CreatedOp_" + requestFileName + ".xml");
                        }
                        var task = ArcClientHelper.PostCreateOp(BaseAddress, request).Result; //thread blocking call
                        var response = HandleOpResponse(task, "CreatedOp_" + requestFileName + "_response.xml");
                        string ids = GetCommaSeparatedIds(response);
                        if (!string.IsNullOrEmpty(ids))
                        {
                            RequestLogger.WriteDeliveredIds(ids);
                            int affected = InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryDeliveryStatusByKey", ids);
                            RequestLogger.WriteLog(string.Format("{0} Successfully delivered to Arc System", affected));
                        }
                    }
                }
            }
                
            catch (Exception ex)
            {
                RequestLogger.WriteError("Method Name: CreatedOp");
                GetMessageRecursively(ex);
            }
            
        }

       
        #endregion

        #region Handle Response
        /// <summary>
        /// Handle the Lead Response result.
        /// Writes Response to file if Debug flag in setting is on <see cref="ApplicationSettings.Debug"/>
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <param name="fileName">File name to writes as resonse</param>
        /// <returns></returns>
        private ArcResponse HandleLeadResponseEx(HttpResponseMessage responseMessage, string fileName = "")
        {
            if (responseMessage.IsSuccessStatusCode)
            {

                try
                {
                    var result = responseMessage.Content.ReadAsAsync<ArcResponse>().Result;
                    if (ApplicationSettings.Debug && !string.IsNullOrEmpty(fileName))
                    {
                        RequestLogger.WriteResponse(result, fileName);
                    }
                    PrintLeadResult(result);
                    return result;
                }
                catch (Exception e)
                {
                    HandleInvalidResponseAsStringContents(responseMessage, fileName, e);
                }
            }else
            {
                Console.WriteLine("Ops error found {0} ,{1}", responseMessage.StatusCode, responseMessage.ReasonPhrase);
                
            }
            return null;
        }

        private static void HandleInvalidResponseAsStringContents(HttpResponseMessage responseMessage, string fileName,
                                                                  Exception e)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Something bad happened while parsing response...");
            string response = responseMessage.Content.ReadAsStringAsync().Result;
            builder.AppendLine(response);
            RequestLogger.WriteError(builder.ToString(),fileName);
            builder.AppendLine("Got error while parsing response...");
            builder.AppendLine(e.Message);
            RequestLogger.WriteResponse(builder.ToString(), fileName);
        }

        /// <summary>
        /// Print Lead results
        /// Print's lead and sub status results and reason
        /// </summary>
        /// <param name="result"></param>
        public static void PrintLeadResult(ArcResponse result)
        {
            Console.WriteLine("==========================  Response  ============================");
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine("{0} {1} {2} {3}", "Accepted", "AccountId", "Reference", "Reason");
            Console.BackgroundColor = ConsoleColor.Black;
            foreach (var leadResult in result.LeadResults)
            {
                Console.Write("{0,6}{1,10}{2,11}", leadResult.Accepted, leadResult.AccountID, leadResult.Reference);
                PrintReason(leadResult.Reason, 51);
                Console.WriteLine();

                if (leadResult.SubResults != null && leadResult.SubResults.Any())
                {
                    Console.BackgroundColor = ConsoleColor.Yellow;
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.WriteLine("{0} {1,9} {2,18}", "Accepted", "Id", "Reason");
                    Console.BackgroundColor = ConsoleColor.Black;
                    foreach (SubResult subResult in leadResult.SubResults)
                    {
                        Console.Write("{0,6}{1,18}", subResult.Accepted, subResult.ID);
                        PrintLeadSubReason(subResult.Reason, 51);
                        Console.WriteLine();
                    }
                }
                // Generate line.
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Prints Reason to console window in red color
        /// </summary>
        /// <param name="reason">message</param>
        /// <param name="width">length of column</param>
        private static void PrintReason(string reason, int width)
        {
            string output = "";
            if (!string.IsNullOrEmpty(reason))
            {
                output = Regex.Replace(reason, @"\r\n?|\n", " ");
                output = Regex.Replace(output, @"\s+", " ");
                if (!string.IsNullOrEmpty(output))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    List<string> list = Split(output, width).ToList();
                    if (list.Any())
                    {
                        Console.Write(" {0,51}", list.FirstOrDefault());
                        list.RemoveAt(0);
                        foreach (string s in list)
                        {
                            if (!string.IsNullOrEmpty(s))
                                Console.WriteLine("{0,27} {1,51}", "", s.Trim());
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }

            //string outputString = "";
            //if (!String.IsNullOrEmpty(outputString))
            //{
            //    outputString = reason.Trim().Replace("\n", "");
            //    while (outputString.Contains("  "))
            //    {
            //        reason = reason.Replace("  ", " ");
            //    }
            //    Console.ForegroundColor = ConsoleColor.Red;
            //    List<string> list = Split(reason, width).ToList();
            //    Console.Write(" {0,51}", list.FirstOrDefault());
            //    foreach (string s in list.Skip(1).ToList())
            //    {
            //        if (!string.IsNullOrEmpty(s))
            //            Console.WriteLine("{0,27} {1,51}", "", s.Trim());
            //    }
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
        }

        /// <summary>
        /// Print's substatus reason to console window in red color
        /// </summary>
        /// <param name="reason"></param>
        /// <param name="width"></param>
        private static void PrintLeadSubReason(string reason, int width)
        {
            string output = "";
            if (!string.IsNullOrEmpty(reason))
            {
                output = Regex.Replace(reason, @"\r\n?|\n", " ");
                output = Regex.Replace(output, @"\s+", " ");
                if (!string.IsNullOrEmpty(output))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    List<string> list = Split(output, width).ToList();
                    if (list.Any())
                    {
                        Console.Write(" {0,51}", list.FirstOrDefault());
                        list.RemoveAt(0);
                        foreach (string s in list)
                        {
                            if (!string.IsNullOrEmpty(s))
                                Console.WriteLine("{0,27} {1,51}", "", s.Trim());
                        }
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                }
            }
            //if (!String.IsNullOrEmpty(reason))
            //{
            //    reason = reason.Trim().Replace("\n", "");
            //    while (reason.Contains("  "))
            //    {
            //        reason = reason.Replace("  ", " ");
            //    }
            //    Console.ForegroundColor = ConsoleColor.DarkCyan;
            //    Console.Write(" {0,54}", Split(reason, width).FirstOrDefault());
            //    foreach (string s in Split(reason, width).Skip(1))
            //    {
            //        if (!string.IsNullOrEmpty(s))
            //            Console.WriteLine(" {0,22} {1,54}", "   ", s.Trim());
            //    }
            //    Console.ForegroundColor = ConsoleColor.White;
            //}
        }

        /// <summary>
        /// Splits the string in chunks to show  data in column.
        /// </summary>
        /// <param name="input">string</param>
        /// <param name="length">with of column</param>
        /// <returns></returns>
        public static IEnumerable<string> Split(string input, int length)
        {
            int index = 0;
            while (index < input.Length)
            {
                if (index + length < input.Length)
                    yield return input.Substring(index, length);
                else
                    yield return input.Substring(index);

                index += length;
            }
        }

        private ArcResponse HandleLeadConsentResponse(HttpResponseMessage task, string fileName = "")
        {

            if (task.IsSuccessStatusCode)
            {
                var result = task.Content.ReadAsAsync<ArcResponse>().Result;
                if (ApplicationSettings.Debug && !string.IsNullOrEmpty(fileName))
                {

                    RequestLogger.WriteResponse(result, fileName);
                }
                Console.WriteLine("Response  ===========================================");
                Console.WriteLine("{0,5} {1,5} {2,5}", "Accepted", "Reference", "Reason");
                foreach (var leadResult in result.LeadResults)
                {
                    Console.WriteLine("{0,4} {1,13} {2,11}", leadResult.Accepted, leadResult.Reference, leadResult.Reason);
                }
                return result;
            }
            else
            {
                Console.WriteLine("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
            }
            return null;
        }
        /// <summary>
        /// Handle Response as a string
        /// </summary>
        /// <param name="task"></param>
        /// <param name="fileName"></param>
        private void HandleResponse(HttpResponseMessage task, string fileName = "")
        {

            if (task.IsSuccessStatusCode)
            {
                var result = task.Content.ReadAsStringAsync();

                Console.WriteLine("========================  Response  =================================");
                Console.WriteLine(result.Result);
            }
            else
            {
                Console.WriteLine("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
            }

        }
        /// <summary>
        /// Handle Create op request response.
        /// </summary>
        /// <param name="task"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private OpResponse HandleOpResponse(HttpResponseMessage task, string fileName = "")
        {

            if (task.IsSuccessStatusCode)
            {
                try
                {
                    var result = task.Content.ReadAsAsync<OpResponse>().Result;
                    if (ApplicationSettings.Debug && !string.IsNullOrEmpty(fileName))
                    {

                        RequestLogger.WriteResponse(result, fileName);
                    }
                    Console.WriteLine("===================== Response =========================");
                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                    Console.WriteLine("{0} {1,7} {2,10}", "Accepted", "ID", "Reason");
                    Console.BackgroundColor = ConsoleColor.Black;
                    foreach (var leadResult in result.OpResults)
                    {
                        Console.Write("{0,6} {1,9}", leadResult.Accepted, leadResult.ID);
                        PrintReason(leadResult.Reason, 51);
                        Console.WriteLine();
                    }
                    return result;
                }
                catch (Exception e)
                {
                    HandleInvalidResponseAsStringContents(task,fileName,e);
                }
            }
            Console.WriteLine("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
            return null;
        }

        #endregion

        #region Request Creation process

        /// <summary>
        /// 3.1
        /// Creates a arc request will list of AccountHistory with credential given in Login Property
        /// Required Status Node
        /// Account.AssignedUser's Arc Id will becomes UserInitial. and last changes from account modification date.
        /// <see cref="ArcDataConversion.ConvertToLeadChangeAgentRequest"/>
        /// <seealso cref="ArcClientHelper.PostChangeAgent"/>
        /// <remarks>Request should contain Status node</remarks>
        /// </summary>
        /// <returns>ArcRequest with multiple leads</returns>
        /// <author>MH</author>
        public ArcRequest CreateChangeAgentRequest(List<AccountHistory> accountHistory)
        {
            if (accountHistory.Any())
            {
                var request = new ArcRequest { Login = Login, Leads = new List<ArcLead>() };
                foreach (var account in accountHistory)
                {
                    var lead = account.ConvertToLeadChangeAgentRequest();
                    if (lead != null && lead.Any())
                        request.Leads.AddRange(lead);
                }
                return request;
            }
            return null;
        }
        /// <summary>
        /// 3.1
        /// Creates a arc request will list of AccountHistory with credential given in Login Property
        /// Required Status Node
        /// Account.AssignedUser's Arc Id will becomes UserInitial. and last changes from account modification date.
        /// <see cref="ArcDataConversion.ConvertToLeadChangeAgentRequest"/>
        /// <seealso cref="ArcClientHelper.PostChangeAgent"/>
        /// <remarks>Request should contain Status node</remarks>
        /// </summary>
        /// <returns>ArcRequest with multiple leads</returns>
        /// <author>MH</author>
        public ArcRequest CreateChangeAgentRequest(List<vw_ArcChangeAgent> accountHistory)
        {
            if (accountHistory.Any())
            {
                var request = new ArcRequest { Login = Login, Leads = new List<ArcLead>() };
                foreach (var account in accountHistory)
                {
                    var lead = account.ConvertToLeadChangeAgentRequest();
                    if (lead != null && lead.Any())
                        request.Leads.AddRange(lead);
                }
                return request;
            }
            return null;
        }

        /// <summary>
        /// Create Action Request Group by reference 
        /// </summary>
        /// <param name="groupByRef"></param>
        /// <returns></returns>
        private ArcRequest CreateAddActionRequestByGroupByRef(List<IGrouping<string, vw_ArcActions>> groupByRef)
        {
            if (groupByRef.Any())
            {
                ArcRequest request = new ArcRequest() { Login = Login, Leads = new List<ArcLead>() };
                foreach (var refGrouping in groupByRef)
                {
                    var lead = new ArcLead()
                        {
                            Timestamp = DateTime.UtcNow.ApplyUtcFormat(),
                            Reference = refGrouping.Key,
                            Actions = new List<LeadAction>()
                        };
                    foreach (var vwArcActions in refGrouping)
                    {
                        var leadAction = new LeadAction()
                            {
                                Code = vwArcActions.Code,
                                Description = vwArcActions.Description,
                                Timestamp = vwArcActions.TimeSpan.HasValue ? vwArcActions.TimeSpan.Value.ToUniversalTime().ToString(ArcDataConversion.UtcDateFormat) : DateTime.UtcNow.ToUniversalTime().ToString(ArcDataConversion.UtcDateFormat),
                                ID = vwArcActions.ActionId,
                                Notes = vwArcActions.Notes,
                                PerformedBy = vwArcActions.RefferedBy
                            };
                        lead.Actions.Add(leadAction);
                    }
                    request.Leads.Add(lead);
                }
                return request;
            }
            return null;
        }


        /// <summary>
        /// Creates a arc request will list of cases with credential given in Login Property
        /// For Request generation <see cref="ArcDataConversion.ConvertToLeadStopLetterRequest"/>
        /// For Posting request <seealso cref="ArcClientHelper.PostStopLetter"/>
        /// <remarks>Each request will contain Email Communication node</remarks>
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public ArcRequest CreateLeadPostStopLetterRequest(List<ArcCases> cases)
        {
            if (cases.Any())
            {
                var request = new ArcRequest { Login = Login, Leads = new List<ArcLead>() };
                foreach (var arcCases in cases)
                {
                    //Timestamp is arcModification date
                    //LastModification in EmailCommunication is individual modification date 
                    var lead = arcCases.ConvertToLeadStopLetterRequest();
                    if (lead != null) request.Leads.Add(lead);
                }
                return request;
            }
            return null;
        }

        /// <summary>
        /// 3.4
        /// <see cref="ArcClientHelper.PostLetterLog"/>
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public ArcRequest CreateLogLetterRequest(List<vw_ArcLetterLog> result)
        {
            if (result.Any())
            {
                ArcRequest request = new ArcRequest();
                request.Login = Login;
                request.Leads = new List<ArcLead>();
                foreach (var letterLog in result)
                {
                    var message = new ArcMessage();
                    //message.Code = letterLog.Code;
                    message.Code = letterLog.Name;
                    message.Date = letterLog.Date.HasValue
                                       ? letterLog.Date.Value.ToUniversalTime().ToString(ArcDataConversion.UtcDateFormat)
                                       : DateTime.UtcNow.ToString(ArcDataConversion.UtcDateFormat);
                    message.ID = letterLog.Id;
                    message.Name = letterLog.Name;
                    if (letterLog.Status == 2)
                        message.Status = "Success";
                    else if (letterLog.Status == 3)
                        message.Status = "Failed";
                    else
                        message.Status = "Unknown";
                    var lead = new ArcLead()
                        {
                            EmailCommunications = new EmailCommunications()
                                {
                                    Messages = new List<ArcMessage>() { message }
                                }
                        };
                    request.Leads.Add(lead);
                }
                return request;
            }
            return null;
        }
        /// <summary>
        ///  Creates Group do vw_ArcLetterLog into ArcRequest.
        /// </summary>
        /// <param name="groupByRef"></param>
        /// <returns></returns>
        private ArcRequest CreateLogLetterRequest(List<IGrouping<string, vw_ArcLetterLog>> groupByRef)
        {
            if (groupByRef.Any())
            {
                ArcRequest request = new ArcRequest();
                request.Login = Login;
                request.Leads = new List<ArcLead>();
                foreach (IGrouping<string, vw_ArcLetterLog> letterLogs in groupByRef)
                {
                    var lead = new ArcLead()
                    {
                        Reference = letterLogs.Key,
                        Timestamp = DateTime.Now.ToUniversalTime().ApplyUtcFormat(),
                        EmailCommunications = new EmailCommunications()
                        {
                            Messages = new List<ArcMessage>()
                        }
                    };
                    foreach (vw_ArcLetterLog log in letterLogs)
                    {
                        var message = log.ConvertToArcMessage();
                        //var message = new ArcMessage();
                        //message.Code = log.Code;
                        //message.Date = log.Date.HasValue
                        //                   ? log.Date.Value.ToUniversalTime().ToString(ArcDataConversion.UtcDateFormat)
                        //                   : DateTime.UtcNow.ToString(ArcDataConversion.UtcDateFormat);
                        //message.ID = log.Id;
                        //message.Name = log.Name;

                        //if (log.Status == 2)
                        //    message.Status = "Success";
                        //else if (log.Status == 0)
                        //    message.Status = "Failed";
                        //else
                        //    message.Status = "Queued";
                        lead.EmailCommunications.Messages.Add(message);
                    }
                    request.Leads.Add(lead);
                }
                return request;
            }
            return null;
        }

        /// <summary>
        /// Creates a arc request will list of cases with credential given in Login Property
        /// <see cref="ArcDataConversion.ConvertToLeadUpdateRequest"/>
        /// <seealso cref="ArcClientHelper.PostUpdate"/>
        ///<summary>
        /// This is the request that contains one or more Leads. 
        /// Each lead provided should contain an Individual with a DayPhone or EveningPhone with a Consent defined.
        /// </summary>
        /// </summary>
        /// <param name="cases"></param>
        /// <returns></returns>
        /// <author>MH</author>
        /// <remarks>Actually applied for TCPA update</remarks>
        public ArcRequest CreateLeadUpdateRequest(List<ArcCases> cases)
        {

            if (cases.Any())
            {
                ArcRequest request = new ArcRequest { Login = Login, Leads = new List<ArcLead>() };
                foreach (var arcCases in cases)
                {
                    var lead = arcCases.ConvertToLeadUpdateRequest(States, Campaigns);
                    request.Leads.Add(lead);
                }
                return request;
            }
            return null;
        }

        /// <summary>
        /// Creates OpRequest from list of Account History.
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private OpRequest CreateLeadOpportunities(List<AccountHistory> result)
        {
            if (result != null && result.Any())
            {
                OpRequest request = new OpRequest();
                request.Login = Login;
                request.Opportunities = new List<Opportunity>();
                foreach (var history in result)
                {
                    string agentInitial = "";
                    if (history.UserL != null) agentInitial = history.UserL.ArcId;
                    var opertunity = new Opportunity()
                        {
                            Timestamp =
                                history.Datetime.HasValue
                                    ? history.Datetime.Value.ToUniversalTime()
                                             .ToString(ArcDataConversion.UtcDateFormat)
                                    : DateTime.UtcNow.ToString(ArcDataConversion.UtcDateFormat)
                            ,
                            AgentInitials = agentInitial,
                            ID = history.AccountHistoryId.ToString("#"),
                            DNIS = history.ArcDINS??"0000",
                            TalkTime = history.ArcTalkTime??0

                        };
                    request.Opportunities.Add(opertunity);
                }
                return request;
            }
            return null;
        }
        private OpRequest CreateLeadOpportunities(List<Arc_GetCreateOp_Result> result)
        {

            if (result != null && result.Any())
            {
                OpRequest request = new OpRequest();
                request.Login = Login;
                request.Opportunities = new List<Opportunity>();
                foreach (var opResult in result)
                {
                    var opportunity = new Opportunity()
                    {
                        Timestamp =
                            opResult.Timestamp.HasValue
                                ? opResult.Timestamp.Value.ToUniversalTime()
                                         .ToString(ArcDataConversion.UtcDateFormat)
                                : DateTime.UtcNow.ToString(ArcDataConversion.UtcDateFormat)
                        ,
                        AgentInitials = opResult.ARC_ID,
                        ID = opResult.ID.ToString(),
                        DNIS = opResult.DNIS ?? "0000",
                        TalkTime = opResult.TalkTime??0

                    };
                    request.Opportunities.Add(opportunity);
                }
                return request;
            }
            return null;
        }

        #endregion

        //private ArcRequest CreateCreateOrUpdateLeadRequest(List<ArcCases> result)
        //{
        //    if (result != null && result.Any())
        //    {
        //        ArcRequest request = new ArcRequest() { Login = Login, Leads = new List<ArcLead>() }
        //            ;
        //        foreach (ArcCases cases in result)
        //        {
        //            ArcLead arcLead = cases.ConvertToCreateOrUpdateLeadRequest(States, Campaigns);
        //            if (arcLead != null)
        //                request.Leads.Add(arcLead);

        //        }

        //        return request;
        //    }
        //    return null;
        //}

        #region Helper methods

        /// <summary>
        /// Gets the comma separated ids from success response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string GetCommaSeparatedIds(ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.Where(p => p.Accepted == "Y").Select(p => p.AccountID).ToList();

                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets the comma separated ids from success response.
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        public string GetCommaSeparatedIds(OpResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.OpResults.Where(p => p.Accepted == "Y").Select(p => p.ID).ToList();

                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets comma separated string from successful Response results
        /// </summary>
        /// <param name="response">Arc Response</param>
        /// <returns></returns>
        private string GetCommaSeparatedReferences(ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.Where(p => p.Accepted == "Y").Select(p => p.Reference).ToList();

                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets the comma separated string from Successful response string
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private string GetCommaSeparatedIdsFromActions(ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.SelectMany(p => p.SubResults.Where(s => s.Accepted == "Y"))
                         .Select(m => m.ID.Replace("Action:", ""))
                         .ToList();
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Gets the comma separated string from Successful response string
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        private string GetCommaSeparatedIdsFromMessage(ArcResponse response)
        {
            string output = "";
            if (response != null)
            {
                List<string> ids = response.LeadResults.SelectMany(p => p.SubResults.Where(s => s.Accepted == "Y"))
                         .Select(m => m.ID.Replace("Message:", ""))
                         .ToList();
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(output))
                        output += "," + id;
                    else
                        output = id;
                }
            }
            return output;
        }
        /// <summary>
        /// Calls store procedure with string parameter
        /// </summary>
        /// <param name="storeProcedureName"></param>
        /// <param name="commaSeparatedIds"></param>
        public int InvokeStoreProcedure(string storeProcedureName, string commaSeparatedIds)
        {
            SqlConnection connection = null;
            int query = 0;
            try
            {
                ConnectionStringHelper connectionHelper = new ConnectionStringHelper();
                connection = new SqlConnection(connectionHelper.ConnectionString);
                SqlCommand cmd = new SqlCommand(storeProcedureName, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                var para = new SqlParameter("Ids", commaSeparatedIds);
                cmd.Parameters.Add(para);
                connection.Open();
                query = cmd.ExecuteNonQuery();
                RequestLogger.WriteLog(string.Format("[{0}] Successfully delivered to arc system ", query));
                connection.Close();
            }
            catch (Exception ex)
            {
                if (connection != null) connection.Close();
                GetMessageRecursively(ex);
            }
            return query;
        }

        /// <summary>
        /// Gets exception detail 
        /// </summary>
        /// <param name="ex"></param>
        public static void GetMessageRecursively(Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            var exception = ex.GetBaseException();
            Console.WriteLine(exception.Message);
            if (exception.InnerException != null)
                GetMessageRecursively(exception.InnerException);
            Console.ForegroundColor = ConsoleColor.White;

        }

        #endregion

        private bool _isDisposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
                return;
            if (disposing)
            {
                // free managed code
                Engine.Dispose();
            }
            // free unmanaged code.
            _isDisposed = true;
        }
    }
}