using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Arc;
using SalesTool.DataAccess.Models;
using SelectCare.ARC.Client.Config;
using SelectCare.ArcApi;

namespace SelectCare.ARC.Client
{
    public class ArcClientEx:IDisposable
    {
        private logger _logger;
        #region Properties

        private List<StateModel> States
        {
            get
            {
                return _states ?? (
                    //Uses cross context 
                    //_states = Engine.ManageStates.GetAll().ToList()
                    _states=Engine.ArcActions.GetAllStates()
                    );
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
                    _requestSize = Engine.ApplicationSettings.ARC_API_REQUEST_SIZE;
                    //_requestSize = ApplicationSettings.RequestSize;
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
                        Password = Engine.ApplicationSettings.ARC_API_USER_PASSWORD
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
        private bool disposed = false;

        #endregion

        #region Constructors.

        /// <summary>
        /// Initialize Database Engine
        /// </summary>
        /// <param name="engine"></param>
        public ArcClientEx(DBEngine engine)
        {
            _logger = new logger();
            Engine = engine;
        }

        /// <summary>
        /// Initialize Database engine with given connection string
        /// </summary>
        /// <param name="connectionString"></param>
        public ArcClientEx(string connectionString)
        {
            _logger = new logger();
            Engine = new DBEngine();
            ConnectionString = connectionString;
            //Engine.Init(connectionString);
            Engine.InitLeadsContext(connectionString);
        }

        #endregion


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
            string loggerName = "ChangeAgent";
            try
            {
                WriteNotification("ChangeAgent job initiated");
                int count = Engine.ArcActions.GetChangedAgentArcCases().Count();
                if (count > 0)
                {
                    //RequestLogger.WriteLog(string.Format("Creating ChangeAgent request for {0} Agents", count));
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

                            //RequestLogger.WriteLog(string.Format("Creating ChangeAgent request for Page {0}...", i));
                            ArcRequest request = CreateChangeAgentRequest(result);
                            var task = ArcClientHelper.PostChangeAgent(BaseAddress, request).Result; //block call
                            if (task.IsSuccessStatusCode)
                            {
                                try
                                {
                                    // cast response message 
                                    var responseResult = task.Content.ReadAsAsync<ArcResponse>().Result;
                                    // print response message to console window
                                    PrintLeadResult(responseResult);
                                    string ids = GetCommaSeparatedReferences(responseResult);
                                    
                                    // write request and response to same file
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(ArcResponse), responseResult);
                                   
                                    if (!string.IsNullOrEmpty(ids))
                                    {
                                        //TODO very tricky. because Account history id is not known.
                                        //NOTE:
                                        int affected =
                                            InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryChangeAgentDeliveryStatus", ids);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("Exception block");
                                    string error = HandleInvalidResponseAsStringContents(task, ex);
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), error);
                                }
                            }
                            else
                            {
                                string s = string.Format("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
                                WriteError(s);
                                _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), s);
                            }


                        }

                    }
                }
                else
                {
                    WriteNotification("No data found");
                }
                WriteNotification("ChangeAgent method completed.");
            }
            catch (Exception ex)
            {
                WriteError("Method Name: ChangeAgent");
                GetMessageRecursively(ex);
                _logger.WriteError("Method Name: ChangeAgent", ex);
            }


        }

        public string CurrentUniversalTime
        {
            get {return DateTime.Now.ToUniversalTime().ApplyUtcFormat(); }
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
            string loggerName = "AddAction";
            try
            {
                WriteNotification("CreateAction job initiated");
                int count = Engine.ArcActions.GetArcActions().GroupBy(p => p.Reference).Count();
                if (count > 0)
                {
                    WriteNotification(string.Format("Creating AddAction request for {0} actions", count));
                    var pages = count / RequestSize + 1;
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
                            //Creating request.
                            ArcRequest request = CreateAddActionRequestByGroupByRef(groupByRef);
                            // floting to endpoint
                            var task = ArcClientHelper.PostAddAction(BaseAddress, request).Result; //block call

                            if (task.IsSuccessStatusCode)
                            {
                                try
                                {
                                    // cast response message 
                                    var responseResult = task.Content.ReadAsAsync<ArcResponse>().Result;
                                    // print response message to console window
                                    PrintLeadResult(responseResult);
                                    string ids = GetCommaSeparatedIdsFromActions(responseResult);

                                    // write request and response to same file
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(ArcResponse), responseResult);

                                    if (!string.IsNullOrEmpty(ids))
                                    {
                                        int affected = InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryDeliveryStatusByKey",
                                                                            ids);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string error = HandleInvalidResponseAsStringContents(task, ex);
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), error);
                                }
                            }
                            else
                            {
                                string s = string.Format("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
                                WriteError(s);
                                _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), s);
                            }
                        }
                    }
                }
                else
                {
                    WriteNotification("No data found");
                }
                WriteNotification("CreateAction method completed...");
            }
            catch (Exception ex)
            {
                WriteError("Method Name: CreateAction");
                GetMessageRecursively(ex);
                _logger.WriteError("Method Name: CreateAction", ex);
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
            string loggerName = "StopLetters";
            WriteNotification("StopLetter job initiated...");
            try
            {
                int count = Engine.ArcActions.GetArcCasesForStopLetters().Count();
                if (count > 0)
                {
                    var pages = count / RequestSize + 1;

                    for (int i = 1; i <= pages; i++)
                    {
                        var result = Engine.ArcActions.GetArcCasesForStopLetters().OrderBy(p => p.Key).Page(i, RequestSize).ToList();
                        if (result.Any())
                        {

                            ArcRequest request = CreateLeadPostStopLetterRequest(result);

                            var task = ArcClientHelper.PostStopLetter(BaseAddress, request).Result; //thread blocking call
                            if (task.IsSuccessStatusCode)
                            {
                                try
                                {
                                    var response = task.Content.ReadAsAsync<ArcResponse>().Result;
                                    PrintLeadResult(response);
                                    string ids = null;
                                    if (response != null && response.LeadResults.Any())
                                    {
                                        ids = GetCommaSeparatedReferences(response);
                                    }
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(ArcResponse), response);
                                    if (!string.IsNullOrEmpty(ids))
                                    {
                                        //RequestLogger.WriteDeliveredIds(ids);
                                        int affected = InvokeStoreProcedure("proj_Arc_UpdateStopLetterDeliveryStatus", ids);

                                    }

                                }
                                catch (Exception e)
                                {
                                    string error = HandleInvalidResponseAsStringContents(task, e);
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), error);
                                }

                            }
                            else
                            {
                                string s = string.Format("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
                                WriteError(s);
                                _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), s);
                            }

                            //var response = HandleLeadResponseEx(task, "StopLetters_" + requestFileName + "_response.xml");

                        }
                    }

                }
                else
                {
                    WriteNotification("No data found");
                }
                WriteNotification("StopLetter method completed...");

            }
            catch (Exception ex)
            {
                WriteError("Method Name: StopLetter");
                GetMessageRecursively(ex);
                _logger.WriteError("Method Name: StopLetter", ex);
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
            string loggerName = "LogLetters";
            WriteNotification("LogLetters job initiated...");
            try
            {
                int count = Engine.ArcActions.GetArcLetterLogs().GroupBy(p => p.Reference).Count();
                if (count > 0)
                {
                    WriteNotification(string.Format("Creating LogLetters request for [ {0} ]", count));
                    var pages = count / RequestSize + 1;
                    
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
                            ArcRequest request = CreateLogLetterRequest(result);
                    
                            var task = ArcClientHelper.PostLetterLog(BaseAddress, request).Result;
                            if (task.IsSuccessStatusCode)
                            {
                                try
                                {
                                   var response= task.Content.ReadAsAsync<ArcResponse>().Result;
                                   PrintLeadResult(response);
                                   string ids = GetCommaSeparatedIdsFromMessage(response);

                                   _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(ArcResponse), response);
                                   if (!string.IsNullOrEmpty(ids))
                                   {
                                       //RequestLogger.WriteDeliveredIds(ids);
                                       int affected = InvokeStoreProcedure("proj_Arc_UpdateLetterLogDeliveryStatus", ids);
                                       WriteNotification(string.Format("{0} Successfully delivered to Arc System",
                                                                            affected));
                                   }
                                }
                                catch (Exception e)
                                {
                                    string error = HandleInvalidResponseAsStringContents(task, e);
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), error);
                                }
                            }
                            else
                            {
                                string s = string.Format("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
                                WriteError(s);
                                _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), s);
                            }
                            
                        }
                    }
                }
                else
                {
                    WriteNotification("No data found");
                }
                WriteNotification("LogLetters method completed...");
            }
            catch (Exception ex)
            {
                WriteError("Method Name: LogLetters");
                GetMessageRecursively(ex);
                _logger.WriteError("Method Name: LogLetters",ex);
            }
        }

        /// <summary>
        /// This function will retrieve Log Letter data from database by page size (given as property)
        /// For Data retrieving 
        /// </summary>
        public void InvokeLeadUpdateConsentRequest()
        {
            string loggerName = "UpdateConsent";
            WriteNotification("UpdateConsent job initiated...");
            try
            {
                int count = Engine.ArcActions.GetArcConsentUpdate().Count();
                if (count > 0)
                {
                    WriteNotification(string.Format("Creating UpdateConsent request for [ {0} ]", count));
                    var pages = count / RequestSize + 1;
                    
                    for (int i = 1; i <= pages; i++)
                    {
                        var result =
                            Engine.ArcActions.GetArcConsentUpdate().OrderBy(p => p.Key).Page(i, RequestSize).ToList();
                        if (result.Any())
                        {
                            
                            ArcRequest request = CreateLeadUpdateRequest(result);
                          
                            var task = ArcClientHelper.PostUpdate(BaseAddress, request).Result; //thread blocking call
                          
                            if (task.IsSuccessStatusCode)
                            {
                                try
                                {
                                    var response= task.Content.ReadAsAsync<ArcResponse>().Result;
                                    PrintLeadResult(response);
                                    string ids = GetCommaSeparatedReferences(response);

                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(ArcResponse), response);
                                    if (!string.IsNullOrEmpty(ids))
                                    {
                                        //RequestLogger.WriteDeliveredIds(ids);
                                        int updated = InvokeStoreProcedure("proj_Arc_UpdateConsentDeliveryStatus", ids);
                                    }
                                }
                                catch (Exception e)
                                {
                                    string error = HandleInvalidResponseAsStringContents(task, e);
                                    _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), error);
                                }
                            }
                            else
                            {
                                string s = string.Format("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
                                WriteError(s);
                                _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), s);
                            }
                           
                        }
                    }
                }
                else
                {
                    WriteNotification("No data found");
                }
                WriteNotification("UpdateConsent method completed...");
            }
            catch (Exception ex)
            {
                WriteError("Method Name: UpdateConsent");
                GetMessageRecursively(ex);
                _logger.WriteError("Method Name: UpdateConsent",ex);
            }
        }

        public void InvokeLeadOpportunitiesRequest(DateTime startDate, DateTime endDate)
        {
            string loggerName = "CreatedOp";
            WriteNotification("CreatedOp task initiated...");
            try
            {
                var result = Engine.ArcActions.GetCreateOpData(startDate, endDate);
                int count = result.Count;
                if (count > 0)
                {
                    var pages = count/RequestSize + 1;
                    for (int i = 1; i <= pages; i++)
                    {
                        var subSet = result.OrderBy(p => p.ID).Page(i, RequestSize).ToList();
                        if (subSet.Any())
                        {
                            
                            OpRequest request = CreateLeadOpportunities(subSet);
                            
                            var task = ArcClientHelper.PostCreateOp(BaseAddress, request).Result; //thread blocking call
                            if (task.IsSuccessStatusCode)
                            {
                                try
                                {
                                    var response = task.Content.ReadAsAsync<OpResponse>().Result;

                                    Console.WriteLine("===================== Response =========================");
                                    Console.BackgroundColor = ConsoleColor.DarkBlue;
                                    Console.WriteLine("{0} {1,7} {2,10}", "Accepted", "ID", "Reason");
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    foreach (var leadResult in response.OpResults)
                                    {
                                        Console.Write("{0,6} {1,9}", leadResult.Accepted, leadResult.ID);
                                        PrintReason(leadResult.Reason, 51);
                                        Console.WriteLine();
                                    }

                                    string ids = GetCommaSeparatedIds(response);
                                    _logger.WriteLog(loggerName, typeof(OpRequest),request,typeof(OpResponse),response);
                                    if (!string.IsNullOrEmpty(ids))
                                    {
                                        int affected = InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryDeliveryStatusByKey", ids);
                                        WriteNotification(string.Format("{0} Successfully delivered to Arc System", affected));
                                    }
                                    
                                }
                                catch (Exception e)
                                {
                                    HandleInvalidResponseAsStringContents(task, e);
                                }
                            }
                            else
                            {
                                string s = string.Format("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
                                WriteError(s);
                                _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), s);
                            }
                          
                            
                        }
                    }
                }
                else
                {
                    WriteNotification("No data found...");
                }
            }

            catch (Exception ex)
            {
                WriteError("Method Name: CreatedOp");
                GetMessageRecursively(ex);
                _logger.WriteError("Method Name: CreatedOp",ex);
            }

        }

        /// <summary>
        /// This function will get Individual's infomation that are changes in SelectCare and needs to update in ARC system by using ArcApis
        /// For Data retriving <see cref="ArcActions.GetArcCasesForContactChanged"/>
        /// For Request Generation <see cref="CreateContactUpdateRequest"/>
        /// For Posting Request <see cref="ArcClientHelper.PostContactUpdate"/>
        /// For Persistence see Store Procedure  proj_Arc_UpdateIndividualContactStatus
        /// </summary>
        public void InvokeLeadUpdateContactInformation()
        {
            const string loggerName = "UpdateContact";
            WriteNotification("UpdateContact task initiated...");
            try
            {
                int count = Engine.ArcActions.GetArcCasesForContactChanged().Count();
                if (count > 0)
                {
                    WriteNotification(string.Format("Creating UpdateContact request for [ {0} ]", count));
                    var pages = count/RequestSize + 1;

                    for (int i = 1; i <= pages; i++)
                    {
                        var result = Engine.ArcActions.GetArcCasesForContactChanged().Page(i, RequestSize).ToList();
                       if(result.Any())
                       {
                           ArcRequest request = CreateContactUpdateRequest(result);
                           var task = ArcClientHelper.PostContactUpdate(BaseAddress, request).Result;
                           if (task.IsSuccessStatusCode)
                           {
                               try
                               {
                                   var response = task.Content.ReadAsAsync<ArcResponse>().Result;
                                   PrintLeadResult(response);
                                   string ids = GetCommaSeparatedReferences(response);

                                   _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(ArcResponse), response);
                                   if (!string.IsNullOrEmpty(ids))
                                   {
                                       //RequestLogger.WriteDeliveredIds(ids);
                                       int affected = InvokeStoreProcedure("proj_Arc_UpdateIndividualContactStatus", ids);
                                       WriteNotification(string.Format("{0} Successfully delivered to Arc System",
                                                                            affected));
                                   }
                               }
                               catch (Exception e)
                               {
                                   string error = HandleInvalidResponseAsStringContents(task, e);
                                   _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), error);
                               }
                           }
                           else
                           {
                               string s = string.Format("Ops error found {0} ,{1}", task.StatusCode, task.ReasonPhrase);
                               WriteError(s);
                               _logger.WriteLog(loggerName, typeof(ArcRequest), request, typeof(string), s);
                           }
                       }
                    }

                }
                else
                {
                    WriteNotification("No data found");
                }
                WriteNotification("UpdateContact method completed...");
            }
            catch (Exception ex)
            {
                WriteError("Method Name: LogLetters");
                GetMessageRecursively(ex);
                _logger.WriteError("Method Name: LogLetters", ex);
            }
           
        }

        public static void WriteError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void WriteNotification(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        #region Handle Response


        private static string HandleInvalidResponseAsStringContents(HttpResponseMessage responseMessage, Exception e)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendLine("Something bad happened while parsing response...");
            string response = responseMessage.Content.ReadAsStringAsync().Result;
            builder.AppendLine(response);
            builder.AppendLine("Got error while parsing response...");
            builder.AppendLine(e.Message);
            return builder.ToString();
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
                        Timestamp = CurrentUniversalTime,
                        Reference = refGrouping.Key,//arc_ref
                        Actions = new List<LeadAction>()
                    };
                    foreach (var vwArcActions in refGrouping)
                    {
                        var leadAction = new LeadAction()
                        {
                            //Code = vwArcActions.Code, //ach_action_key
                            Code = vwArcActions.ArcAccountId??-1,// Mapping to Action.ArcArcAccountId 22 April 2014
                            Description = vwArcActions.Description,//act_title
                            Timestamp = vwArcActions.TimeSpan.HasValue ? vwArcActions.TimeSpan.Value.ToUniversalTime().ApplyUtcFormat() : CurrentUniversalTime,
                            ID = vwArcActions.ActionId,//ach_key
                            //ID=vwArcActions.ArcAccountId??-1,//MH:31 march 2014 [10:06 PM] objection
                            Notes = vwArcActions.Notes,//ach_comment
                            PerformedBy = vwArcActions.RefferedBy //usr_arc_id
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
                                       : CurrentUniversalTime;
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
                        Timestamp = CurrentUniversalTime,
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
                                         .ApplyUtcFormat()
                                :CurrentUniversalTime
                        ,
                        AgentInitials = agentInitial,
                        ID = history.AccountHistoryId.ToString("#"),
                        DNIS = history.ArcDINS ?? "0000",
                        TalkTime = history.ArcTalkTime ?? 0

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
                                ? opResult.Timestamp.Value.ToUniversalTime().ApplyUtcFormat()
                                : CurrentUniversalTime
                        ,
                        AgentInitials = opResult.ARC_ID,
                        ID = opResult.ID.ToString(),
                        DNIS = opResult.DNIS ?? "0000",
                        TalkTime = opResult.TalkTime ?? 0

                    };
                    request.Opportunities.Add(opportunity);
                }
                return request;
            }
            return null;
        }

        /// <summary>
        /// This method will convert list of ArcCases to leads request with data of Individual Contact information only
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private ArcRequest CreateContactUpdateRequest(List<ArcCases> result)
        {
            if (result != null && result.Any())
            {
                ArcRequest request = new ArcRequest() { Login = Login, Leads = new List<ArcLead>() }
                    ;
                foreach (ArcCases cases in result)
                {
                    ArcLead arcLead = cases.ConvertToContactUpdateLeadRequest(States, Campaigns);
                    if (arcLead != null)
                        request.Leads.Add(arcLead);

                }

                return request;
            }
            return null;
        }
        #endregion

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
            if (response != null&& response.LeadResults.Any())
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
                WriteNotification(string.Format("[{0}] Successfully delivered to arc system ", query));
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

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
                return;
            if (disposing)
            {
                // free managed resources
                Engine.Dispose();
            }
            // free un-managed code
            disposed = true;
        }
    }

    internal class logger
    {
        public bool CanLog { get { return true; } }

        public void WriteLog(string loggerName, Type requestType, object request, Type responseType, object response)
        {
            if (request != null)
            {
                if (CanLog)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Request", loggerName);
                    builder.AppendLine();
                    builder.AppendLine("========================================================================");
                    var requestXmlString = GetObjectAsXmlString(requestType, request);
                    builder.AppendLine(requestXmlString);
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Response", loggerName);
                    builder.AppendLine();
                    builder.AppendLine("========================================================================");
                    var responseXmlString = GetObjectAsXmlString(responseType, response);
                    builder.AppendLine(responseXmlString);
                    builder.AppendLine();
                    builder.AppendLine("####################################################################");
                    builder.AppendLine();
                    builder.AppendLine();
                    var logger = NLog.LogManager.GetLogger(loggerName);
                    string record = builder.ToString();
                    logger.Info(record);
                }
            }
        }
        private static string GetObjectAsXmlString(Type type, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            serializer.Serialize(writer, obj);
            return builder.ToString();
        }

        public void WriteError(string message, Exception exception)
        {
            var logger = NLog.LogManager.GetLogger("Errors");
            logger.Error(message + "\n" + exception);
        }

        public void WriteReport(string loggerName, int count, string ids)
        {

        }

        public void WriteLog(string loggerName, Type requestType, object request, Type responseType, object response, int count, string ids)
        {
            if (request != null)
            {
                if (CanLog)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Request", loggerName);
                    builder.AppendLine();
                    builder.AppendLine("========================================================================");
                    var requestXmlString = GetObjectAsXmlString(requestType, request);
                    builder.AppendLine(requestXmlString);
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Response", loggerName);
                    builder.AppendLine();
                    builder.AppendLine("========================================================================");
                    var responseXmlString = GetObjectAsXmlString(responseType, response);
                    builder.AppendLine(responseXmlString);
                    builder.AppendLine();
                    builder.AppendLine("========================================================================");
                    builder.AppendFormat("\t\t\t\t\t{0} Report");
                    builder.AppendLine("========================================================================");
                    builder.AppendLine();
                    builder.AppendLine(string.Format("Records Sent                 {0}", count));
                    builder.AppendLine(string.Format("Successfully delivered       {0}", string.IsNullOrEmpty(ids)?0: ids.Split(',').Count()));
                    builder.AppendLine();
                    builder.AppendLine("####################################################################");
                    builder.AppendLine();
                    builder.AppendLine();
                    var logger = NLog.LogManager.GetLogger(loggerName);
                    string record = builder.ToString();
                    logger.Info(record);
                }
            }
        }
    }
}
