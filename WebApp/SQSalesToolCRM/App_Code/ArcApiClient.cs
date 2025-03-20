/*
 * Filename:  ArcApiClient
 * Author:    MH
 * Purpose: This class is responsible for sending real time updates to arc service api, this implementation is plugged in
 * SalesDataPage class , its operates by listening events in DAL and it creates request based on current data and sends to 
 * arc service.
 * and send to the arc service api.
 *          
      Initial Implementation:  18:12:2013
      
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Arc;
using SalesTool.DataAccess.Models;
using SelectCare.ArcApi;


/// <summary>
/// Summary description for ArcApiClient
/// </summary>
public class ArcApiClient : System.IDisposable
{
    public string BaseUrl { get; set; }

    private RequestLogin Login { get; set; }
    private DBEngine Engine;
    //private ArcSettings settings;
    private logger _logger = new logger();
    private bool _isDisposed;
    private string K_ARCAPI = "Arc Api";

    public ArcApiClient(DBEngine _engine)
    {
        //if (ApplicationSettings.IsTermLife)
        if (_engine.ApplicationSettings.IsTermLife && _engine.ApplicationSettings.IsEnabledRealTimeArcApiCalls)
        {
            Engine = _engine;
            _logger.CanLog = ApplicationSetting.IsEnabledRealTimeArcApiLogging;
            //settings = new ArcSettings(ApplicationSettings.ADOConnectionString);
            //settings = new ArcSettings(ApplicationSettings.ADOConnectionString);
            //BaseUrl = settings.BaseUrl;
            BaseUrl = Engine.ApplicationSettings.ARC_API_BASE_ADDRESS;
            if (!string.IsNullOrEmpty(BaseUrl))
            {

                // Register for Change Agent and Add Action events log types
                if (ApplicationSetting.IsEnabledChangeAgentRealTimeApiCall || ApplicationSetting.IsEnabledAddActionRealTimeApiCall)
                {
                    Engine.AccountHistoryAdded += AccountHistoryAdded;

                    if (Engine.ApplicationSettings.IsEnabledAddActionRealTimeApiCall)
                        Engine.MultipleActionApplied += MultipleActionApplied;
                }

                // Register for Consent update listener
                if (Engine.ApplicationSettings.IsEnabledConsentUpdateRealTimeArcApiCall)
                    Engine.ConsentUpdated += ConsentUpdated;

                // Register for stop letter changes.
                if (Engine.ApplicationSettings.IsEnabledStopLettersRealTimeArcApiCall)
                    Engine.StopLetterChanged += StopLetterChanged;

                if (Engine.ApplicationSettings.IsEnabledLetterLogRealTimeArcApiCall)
                    Engine.EmailQueueAdded += EmailQueueAdded;

                Login = new RequestLogin()
                    {
                        //Password = settings.UserPassword,
                        //UserId = settings.UserId
                        UserId = Engine.ApplicationSettings.ARC_API_USER_ID,
                        Password = Engine.ApplicationSettings.ARC_API_USER_PASSWORD
                    };
            }
        }
    }

    private GlobalAppSettings ApplicationSetting
    {
        get { return Engine.ApplicationSettings; }
    }
    private ArcGlobalSettings ArcApplicationSetting
    {
        get { return Engine.ArcApplicationSettings; }
    }
    public string CurrentUniversalTime
    {
        get { return DateTime.Now.ToUniversalTime().ApplyUtcFormat(); }
    }
    private void MultipleActionApplied(object sender, ItemEventArgs<List<long>> args)
    {
        var groupByRef = Engine.ArcActions.GetArcActions(args.Item).GroupBy(p => p.Reference).ToList();
        if (groupByRef.Any())
        {
            ArcRequest request = CreateAddActionRequestByGroupByRef(groupByRef);
            if (request != null)
            {
                Task<HttpResponseMessage> task = ArcClientHelper.PostAddAction(BaseUrl, request);
                task.ContinueWith(p =>
                {
                    if (!p.IsFaulted)
                    {
                        try
                        {
                            p.Result.Content.ReadAsAsync<ArcResponse>().ContinueWith(r =>
                            {
                                if (!r.IsFaulted)
                                {
                                    string ids = r.Result.GetCommaSeparatedIdsFromActions();
                                    if (!string.IsNullOrEmpty(ids))
                                    {
                                        int affected = InvokeStoreProcedure("proj_Arc_UpdateAccountHistoryDeliveryStatusByKey",
                                                                    ids);
                                    }
                                    _logger.WriteLog("AddAction_Real-time", typeof(ArcRequest), request, typeof(ArcResponse), r.Result);
                                }
                                else
                                {
                                    if (r.Exception != null)
                                        HandleException(r.Exception);
                                }
                            });
                        }
                        catch (Exception ee)
                        {
                            //Console.WriteLine(e);
                            var error = HandleInvalidResponse(p, ee);
                            _logger.WriteLog("AddAction_Real-time", typeof(ArcRequest), request, typeof(string), error);
                        }
                    }
                    else
                    {
                        if (p.Exception != null)
                            HandleException(p.Exception);
                    }

                });
            }
        }
    }

    private void AccountHistoryAdded(object sender, AccountHistoryEventArgs e)
    {
        var history = e.History;
        if (history != null)
        {
            if (history.EntryType != null)
            {
                var type = (ActionHistoryType)Enum.ToObject(typeof(ActionHistoryType), history.EntryType);
                // Change Agent for data access <see cref="DBEngine.ArcActions.GetChangedAgentArcCases"/>
                if (type == ActionHistoryType.Log && history.Comment == "User assigned")
                {
                    if (Engine.ApplicationSettings.IsEnabledChangeAgentRealTimeApiCall)
                        ChangeAgent(history);
                }
                if (type == ActionHistoryType.Actions)
                {
                    if (Engine.ApplicationSettings.IsEnabledAddActionRealTimeApiCall)
                        AddAction(history);
                }
            }

        }
    }
    /// <summary>
    /// Excutes when it gets notification from DAL about the UserAssigned with log type
    /// will create ChangeAgent request based on AccountHistory and send to arc system 
    /// For Request Creation look at excetion method<see cref="history.ConvertToLeadChangeAgentRequest"/>
    /// For Posting <see cref="ArcClientHelper.PostChangeAgent"/>
    /// </summary>
    /// <param name="history"></param>
    private void ChangeAgent(AccountHistory history)
    {

        var leads = history.ConvertToLeadChangeAgentRequest();
        if (leads != null && leads.Any())
        {
            var request = new ArcRequest { Login = Login };
            request.Leads.AddRange(leads);
            Task<HttpResponseMessage> task = ArcClientHelper.PostChangeAgent(BaseUrl, request);
            task.ContinueWith(p =>
                {
                    if (!p.IsFaulted)
                    {
                        try
                        {
                            p.Result.Content.ReadAsAsync<ArcResponse>().ContinueWith(r =>
                                {
                                    if (!r.IsFaulted)
                                    {
                                        string ids = r.Result.GetCommaSeparatedIds();
                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            int affected =
                                                InvokeStoreProcedure(
                                                    "proj_Arc_UpdateAccountHistoryChangeAgentDeliveryStatus",
                                                    ids);
                                            _logger.WriteLog("ChangeAgent-Real-time", typeof(ArcRequest), request, typeof(ArcResponse), r.Result);
                                        }

                                    }
                                    else
                                    {
                                        if (r.Exception != null)
                                            HandleException(r.Exception);
                                    }
                                });
                        }
                        catch (Exception e)
                        {

                            //   Console.WriteLine(e);
                            var error = HandleInvalidResponse(p, e);
                            _logger.WriteLog("ChangeAgent-Real-time", typeof(ArcRequest), request, typeof(string), error);

                        }
                    }
                    else
                    {
                        if (p.Exception != null)
                        {
                            HandleException(p.Exception);
                        }
                    }
                });
        }

    }

    private static string HandleInvalidResponse(Task<System.Net.Http.HttpResponseMessage> p, Exception e)
    {
        StringBuilder builder = new StringBuilder();
        builder.AppendLine("Something bad happened while parsing arc-api response. Sever sent this...");
        string res = p.Result.Content.ReadAsStringAsync().Result;
        builder.AppendLine(res);
        builder.AppendLine("Error occurred while parsing ...");
        builder.AppendLine(e.Message);
        string error = builder.ToString();
        return error;
    }

    /// <summary>
    /// Executed when action is added in AccountHistory in DAL
    /// Gets the arcAction record based on history id
    /// For data access <see cref="ArcActions.GetArcActions"/>
    /// for Request creation <see cref="ConvertToLeadActionSingle"/>
    /// </summary>
    /// <param name="history"></param>
    private void AddAction(AccountHistory history)
    {
        try
        {
            long historyId = history.AccountHistoryId;
            var actions = Engine.ArcActions.GetArcActions().FirstOrDefault(p => p.ActionId == historyId);
            if (actions != null)
            {
                var request = ConvertToLeadActionSingle(actions);

                if (request != null)
                {
                    Task<HttpResponseMessage> task = ArcClientHelper.PostAddAction(BaseUrl, request);
                    task.ContinueWith(p =>
                        {
                            if (!p.IsFaulted)
                            {
                                try
                                {
                                    p.Result.Content.ReadAsAsync<ArcResponse>().ContinueWith(r =>
                                        {
                                            if (!r.IsFaulted)
                                            {
                                                string ids = r.Result.GetCommaSeparatedIdsFromActions();
                                                if (!string.IsNullOrEmpty(ids))
                                                {
                                                    history.IsDeliveredToArc = true;
                                                    Engine.AccountHistory.Save(history);
                                                }
                                                _logger.WriteLog("AddAction_Real-time", typeof(ArcRequest), request, typeof(ArcResponse), r.Result);
                                            }
                                            else
                                            {
                                                if (r.Exception != null)
                                                    HandleException(r.Exception);
                                            }
                                        });
                                }
                                catch (Exception e)
                                {
                                    //Console.WriteLine(e);
                                    var error = HandleInvalidResponse(p, e);
                                    _logger.WriteLog("AddAction_Real-time", typeof(ArcRequest), request, typeof(string), error);
                                }
                            }
                            else
                            {
                                if (p.Exception != null)
                                    HandleException(p.Exception);
                            }

                        });
                }
            }
        }
        catch (AggregateException exception)
        {
            //  HandleException(exception);
        }
    }

    /// <summary>
    /// This event handler is executed when the individual.EmailOpOut property changes or at when individual is saved
    /// and has changes which is not delivered to arc system yet.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void StopLetterChanged(object sender, ItemEventArgs<long> e)
    {
        try
        {
            long id = e.Item;
            var cases = Engine.ArcActions.GetArcCasesForStopLetters().FirstOrDefault(p => p.ArcIndividualKey == id);
            if (cases != null)
            {
                var lead = cases.ConvertToLeadStopLetterRequest();
                if (lead != null)
                {
                    var request = new ArcRequest { Login = Login, Leads = new List<ArcLead>() };
                    request.Leads.Add(lead);
                    Task<HttpResponseMessage> task = ArcClientHelper.PostStopLetter(BaseUrl, request);
                    task.ContinueWith(p =>
                        {
                            if (!p.IsFaulted)
                            {
                                try
                                {
                                    p.Result.Content.ReadAsAsync<ArcResponse>().ContinueWith(r =>
                                        {
                                            if (!r.IsFaulted)
                                            {
                                                bool isSuccess = r.Result.LeadResults.Any(pp => pp.Accepted == "Y");
                                                if (isSuccess)
                                                {
                                                    var individual = Engine.IndividualsActions.Get(id);
                                                    if (individual != null)
                                                    {
                                                        individual.IndividualEmailOptOutQueuedChange = false;
                                                        Engine.IndividualsActions.Change(individual, K_ARCAPI);
                                                    }
                                                }
                                                _logger.WriteLog("StopLetter_Real-time", typeof(ArcRequest), request, typeof(ArcResponse), r.Result);
                                            }
                                            else
                                            {
                                                if (r.Exception != null) HandleException(r.Exception);
                                            }
                                        });
                                }
                                catch (Exception rException)
                                {
                                    var error = HandleInvalidResponse(p, rException);
                                    _logger.WriteLog("StopLetter_Real-time", typeof(ArcRequest), request, typeof(string), error);
                                }
                            }
                            else
                            {
                                if (p.Exception != null)
                                    HandleException(p.Exception);
                            }
                        });
                }
            }
        }
        catch (Exception ex)
        {
            string msg = GetExceptionMessage(ex);
            System.Diagnostics.Debug.Write(msg);
        }
    }

    /// <summary>
    /// this handler is executed when EmailQueue is added in DAL
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void EmailQueueAdded(object sender, ItemEventArgs<long> e)
    {
        long Id = e.Item;
        var letterLog = Engine.ArcActions.GetArcLetterLogs().FirstOrDefault(p => p.Id == Id);
        if (letterLog != null)
        {
            var request = ConvertToLeadLogLettersSingle(letterLog);

            if (request != null)
            {
                Task<HttpResponseMessage> task = ArcClientHelper.PostLetterLog(BaseUrl, request);
                task.ContinueWith(p =>
                    {
                        if (!p.IsFaulted)
                        {
                            try
                            {
                                p.Result.Content.ReadAsAsync<ArcResponse>().ContinueWith(r =>
                                    {
                                        if (!r.IsFaulted)
                                        {
                                            if (r.Result.LeadResults.Any(pr => pr.SubResults.Any(pri => pri.Accepted == "Y")))
                                            {
                                                var queue = Engine.EmailQueueActions.Get(Id);
                                                if (queue != null)
                                                {
                                                    queue.IsDeliveredToArc = true;
                                                    Engine.EmailQueueActions.Change(queue);
                                                }
                                            }
                                            _logger.WriteLog("EmailQueue_Real-time", typeof(ArcRequest), request, typeof(ArcResponse), r.Result);
                                        }
                                        else
                                        {
                                            if (r.Exception != null)
                                                HandleException(r.Exception);
                                        }
                                    });
                            }
                            catch (Exception iException)
                            {
                                //Console.WriteLine(iException);
                                var error = HandleInvalidResponse(p, iException);
                                _logger.WriteLog("EmailQueue_Real-time", typeof(ArcRequest), request, typeof(string), error);
                            }
                        }
                        else
                        {
                            if (p.Exception != null)
                            {
                                HandleException(p.Exception);
                            }
                        }
                    });
            }
        }
    }
    /// <summary>
    /// To update consent information real-time to arc service api.
    /// for Data Access <see cref="ArcActions.GetArcConsentUpdate"/>
    /// For request generation <see cref="CreateLeadUpdateRequest"/>
    /// For Request Posting <see cref="ArcClientHelper.PostUpdate"/>
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e">this args are received when AccountAction.SetConsent executed in DAL agaist accountId</param>
    void ConsentUpdated(object sender, ItemEventArgs<long> e)
    {
        long accountId = e.Item;
        var list = Engine.ArcActions.GetArcConsentUpdate().Where(p => p.AccountKey == accountId).ToList();
        var request = CreateLeadUpdateRequest(list);
        if (request != null)
        {
            Task<HttpResponseMessage> task = ArcClientHelper.PostUpdate(BaseUrl, request);
            task.ContinueWith(p =>
                {
                    if (!p.IsFaulted)
                    {
                        try
                        {
                            p.Result.Content.ReadAsAsync<ArcResponse>().ContinueWith(r =>
                                {
                                    if (!r.IsFaulted)
                                    {
                                        string ids = r.Result.GetCommaSeparatedReferences();
                                        if (!string.IsNullOrEmpty(ids))
                                        {
                                            int updated = InvokeStoreProcedure("proj_Arc_UpdateConsentDeliveryStatus", ids);
                                        }

                                        _logger.WriteLog("ConsentUpdated_Real-time", typeof(ArcRequest), request, typeof(ArcResponse), r.Result);
                                    }
                                    else
                                    {
                                        if (r.Exception != null)
                                            HandleException(p.Exception);
                                    }
                                });
                        }

                        catch (Exception iException)
                        {
                            var error = HandleInvalidResponse(p, iException);
                            _logger.WriteLog("ConsentUpdated_Real-time", typeof(ArcRequest), request, typeof(string), error);
                        }
                    }
                    else
                    {
                        if (p.Exception != null)
                            HandleException(p.Exception);
                    }
                });
        }
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
                    Reference = refGrouping.Key,
                    Actions = new List<LeadAction>()
                };
                foreach (var vwArcActions in refGrouping)
                {
                    var leadAction = new LeadAction()
                    {
                        //Code = vwArcActions.Code,//ach_action_key
                        Code = vwArcActions.ArcAccountId ?? -1,
                        Description = vwArcActions.Description,//act_title
                        Timestamp = vwArcActions.TimeSpan.HasValue ? vwArcActions.TimeSpan.Value.ToUniversalTime().ToString(ArcDataConversion.UtcDateFormat) : CurrentUniversalTime,
                        ID = vwArcActions.ActionId,//ach_key
                        //ID = vwArcActions.ArcAccountId ?? -1,//MH:31 march 2014 [10:06 PM] objection
                        Notes = vwArcActions.Notes,//ach_comment
                        PerformedBy = vwArcActions.RefferedBy//usr_arc_id
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
    /// Converts a single instance of ArcAction view to ArcRequest with login information from Database Application_Store Table
    /// </summary>
    /// <param name="action">ArcAction View</param>
    /// <returns>Request to send</returns>
    public ArcRequest ConvertToLeadActionSingle(vw_ArcActions action)
    {
        var request = new ArcRequest() { Login = Login, Leads = new List<ArcLead>() };
        var lead = new ArcLead()
        {
            Timestamp = CurrentUniversalTime,
            Reference = action.Reference,
            Actions = new List<LeadAction>()
        };

        var leadAction = new LeadAction()
                           {
                               //Code = action.Code,//ach_action_key
                               Code = action.ArcAccountId ?? -1,
                               Description = action.Description,//act_title
                               Timestamp = action.TimeSpan.HasValue ? action.TimeSpan.Value.ToUniversalTime().ToString(ArcDataConversion.UtcDateFormat) : CurrentUniversalTime,
                               ID = action.ActionId,//ach_key 31 March 2014
                               //ID = action.ArcAccountId??-1,//ach_key 31 March 2014
                               Notes = action.Notes,//ach_comments
                               PerformedBy = action.RefferedBy//usr_arc_Id
                           };
        lead.Actions.Add(leadAction);
        request.Leads.Add(lead);
        return request;
    }
    /// <summary>
    /// Converts a single instance of ArcLetterLog view to ArcRequest 
    /// </summary>
    /// <param name="letterLog"></param>
    /// <returns></returns>
    public ArcRequest ConvertToLeadLogLettersSingle(vw_ArcLetterLog letterLog)
    {
        var request = new ArcRequest() { Login = Login, Leads = new List<ArcLead>() };
        var lead = new ArcLead()
        {
            Reference = letterLog.Reference,
            Timestamp = CurrentUniversalTime,
            EmailCommunications = new EmailCommunications()
            {
                Messages = new List<ArcMessage>() { }
            }

        };
        var message = letterLog.ConvertToArcMessage();

        if (message != null) lead.EmailCommunications.Messages.Add(message);
        request.Leads.Add(lead);
        return request;
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
            //var States = Engine.ManageStates.GetAll().ToList();
            //var Campaigns = Engine.ManageCampaignActions.GetAll().ToList();

            var States = Engine.ArcActions.GetAllStates();
            var Campaigns = Engine.ArcActions.GetAllCampaign();

            ArcRequest request = new ArcRequest { Login = Login, Leads = new List<ArcLead>() };
            foreach (var arcCases in cases)
            {
                var lead = arcCases.ConvertToLeadUpdateRequest(States, Campaigns);
                if (lead != null) request.Leads.Add(lead);
            }
            return request;
        }
        return null;
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

            //connection = new SqlConnection(ApplicationSettings.ADOConnectionString);
            connection = new SqlConnection(ApplicationSettings.ADOConnectionString);
            SqlCommand cmd = new SqlCommand(storeProcedureName, connection);
            cmd.CommandType = CommandType.StoredProcedure;
            var para = new SqlParameter("Ids", commaSeparatedIds);
            cmd.Parameters.Add(para);
            connection.Open();
            query = cmd.ExecuteNonQuery();
            connection.Close();
        }
        catch (Exception ex)
        {
            if (connection != null) connection.Close();
            // GetMessageRecursively(ex);
        }
        return query;
    }

    /// <summary>
    /// Gets exception detail 
    /// </summary>
    /// <param name="ex"></param>
    public static string GetExceptionMessage(Exception ex)
    {
        var builder = new StringBuilder();
        var exception = ex.GetBaseException();
        builder.AppendLine(exception.Message);
        while (exception.InnerException != null)
        {
            builder.AppendLine(exception.InnerException.GetBaseException().Message);
            exception = exception.InnerException;
        }
        return builder.ToString();
    }
    /// <summary>
    /// Handles Aggregated Exception raised by HttpClient and log into event log or file log based on configuration 
    /// </summary>
    /// <param name="exception"></param>
    private void HandleException(AggregateException exception)
    {
        //TODO: Add a logger 
        if (exception != null)
        {
            var logger = NLog.LogManager.GetLogger("Arc-Api Real-time");
            foreach (var ex in exception.InnerExceptions)
            {
                string msg = GetExceptionMessage(ex);
                System.Diagnostics.Debug.WriteLine(msg);
                logger.Error(msg);
            }
        }
    }


    // Change Agent.
    #region Disposable pattren

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool isDisposing)
    {
        if (_isDisposed) return;
        if (isDisposing)
        {
            // free managed resources
            Engine.AccountHistoryAdded -= AccountHistoryAdded;
            Engine.MultipleActionApplied -= MultipleActionApplied;
            // Register for Consent update listener
            Engine.ConsentUpdated -= ConsentUpdated;
            // Register for stop letter changes.
            Engine.StopLetterChanged -= StopLetterChanged;

            Engine.EmailQueueAdded -= EmailQueueAdded;
            Engine = null;
        }
        _logger = null;
        //settings = null;
        Login = null;
        BaseUrl = null;
        _isDisposed = true;
    }

    #endregion
}

internal class logger
{
    public bool CanLog { get; set; }

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
}