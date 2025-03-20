<%@ WebHandler Language="C#" Class="PhoneQueueAcd" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Collections.Generic;
using MODELS = SalesTool.DataAccess.Models;
using System.Linq;
using System.Web.Script.Serialization;
using System.IO;
using System.Globalization;
using SalesTool.DataAccess;
using System.Text;

public class PhoneQueueAcd : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{

    #region Constants

    public string JsonMessage = string.Empty;
    const string Status = "status";
    const string TimeStamp = "timestamp";
    const string Skill = "skill";
    const string ContactId = "contactid";
    const string PhoneNumber = "phonenumber";
    #endregion
    JsonMessage jsonMessage = new JsonMessage();
    JavaScriptSerializer jc = new JavaScriptSerializer();
    StringBuilder sb = new StringBuilder();
    HttpContext context = null;
    private SalesTool.DataAccess.DBEngine _engine = null;
    public SalesTool.DataAccess.DBEngine Engine
    {
        get
        {
            if (_engine == null)
            {
                _engine = new SalesTool.DataAccess.DBEngine();
                _engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
                _engine.Init(ApplicationSettings.ADOConnectionString);
            }
            return _engine;
        }
    }
    
    //TM [13 09 2014] Mutex object for ACDtoDial critical section 
    private static object objMutexACD = new object();    
    
    public void ProcessRequest(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        string method = string.Empty;
        if (!string.IsNullOrEmpty(Request.QueryString["method"]))
            method = Request.QueryString["method"];
        this.context = context;
        switch (method)
        {
            case "GetAll":
                GetAll(Request, Response);
                break;
            case "GetPendingCalls":
                GetPendingCalls(Request, Response);
                break;
            case "GetByStatus":
                GetByStatus(Request, Response);
                break;
            case "GetCount":
                GetCount(Request, Response);
                break;
            case "SetStatus":
                SetStatus(Request, Response);
                break;
            case "AddEdit":
                AddEdit(Request, Response);
                break;
            case "GetMissedRejected":
                GetMissedRejected(Request, Response);
                break;
            case "GetUserData":
                {
                    UserDatatemp userData = Getuserdata(context);
                    System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    jc.Serialize(userData, sb);
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            case "DialAcd":
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    if (context.Session[Konstants.K_USERID] != null)
                    {
                        //TM [13 09 2014] Added the Mutex lock for the critical section
                        // uncomment the commented code for debugging and diagnosing
                        MODELS.ACDToDial_Result result = null;
                        //Logger.Logfile("ACD BEFORE ENTERING Mutex code, user ID " + context.Session[Konstants.K_USERID].ToString()); 
                        lock (objMutexACD)
                        {
                            System.Threading.Thread.Sleep(2000);
                            //Logger.Logfile("ACD ENTERED Mutex code, user ID " + context.Session[Konstants.K_USERID].ToString().ToString());
                            //System.Threading.Thread.Sleep(10000);
                            result = Engine.PhoneBarActions.ACDToDial(Guid.Parse(context.Session[Konstants.K_USERID].ToString())).FirstOrDefault();
                        }
                        //Logger.Logfile("ACD EXITTED MUTEX code, user ID " + context.Session[Konstants.K_USERID].ToString());
                        
                        if (result != null )
                        {                            
                            System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                            jc.Serialize(result, sb);
                            NotifySignalRACD(result.qia_key);
                        }
                    }
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            case "UpdateStatsCallTaken":
                {
                    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    if (context.Session[Konstants.K_USERID] != null)
                    {
                        try
                        {
                            Guid agentId = Guid.Parse(context.Session[Konstants.K_USERID].ToString());
                            Int64 queueId = (Request.QueryString["queueid"] != null && Request.QueryString["queueid"] != string.Empty) ? Convert.ToInt64(Request.QueryString["queueid"]) : 0;
                            Engine.PhoneBarActions.UpdateStatsCallTaken(agentId, queueId);
                        }
                        catch (Exception ex)
                        {
                            Response.StatusDescription = ex.Message;
                            Response.StatusCode = 401;
                        }
                    }
                    System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                    jc.Serialize(string.Empty, sb);
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            default:
                break;
        }
    }
    #region Helper Methods

    //Method commented out because max daily cap is not required here

    ///// <summary>
    ///// Core dictionality to load data from sp
    ///// </summary>
    ///// <typeparam name="T"></typeparam>
    ///// <param name="spName"></param>
    ///// <param name="timeout"></param>
    ///// <author>MH</author>
    ///// <returns></returns>
    //public T LoadData<T>(string spName, HttpContext context1)
    //{        
    //    var parm =
    //      new System.Data.SqlClient.SqlParameterFluent().Add("agentid", Guid.Parse(context1.Session[Konstants.K_USERID].ToString()))
    //          .ToObjectArray();
    //    ApplicationStorageHelper helper = new SalesTool.DataAccess.ApplicationStorageHelper("ApplicationServices");
    //    return helper.Context.Database.SqlQuery<T>("exec GetACDGalMaxDailyCount", parm).FirstOrDefault();
    //}
    
    public UserDatatemp Getuserdata(HttpContext context)
    {
        UserDatatemp userData = new UserDatatemp();

        userData.agentKey = Convert.ToString(context.Session[Konstants.K_USERID]);
        userData.maxDailyLead = 0; //Get from DB
        //userData.maxDailyLead = (int)LoadData<int>("GetWebGalMaxDailyCount", context);
        
        userData.SignalRurl =
            string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SignalRurl"]) ?
            "" : System.Configuration.ConfigurationManager.AppSettings["SignalRurl"];
        return userData;
    }

    private void GetMissedRejected(HttpRequest Request, HttpResponse Response)
    {
        var statusResult = Engine.PhoneBarActions.GetACDInboundQueuePersonal();
        string qryStatus = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;
        string qryInboundSkillId = Request.QueryString["inboundSkillId"] != null ? Request.QueryString["inboundSkillId"] : string.Empty;

        DateTime dt = DateTime.Now;
        var query = (from v in statusResult
                     select v).
                     Where(x => (x.Status == "missed" || x.Status == "reject"))
                     .Where(x => (x.AddDate.Value.Year == dt.Year && x.AddDate.Value.Day == dt.Day && x.AddDate.Value.Month == dt.Month) ||
                          (x.ModifiedDate.Value.Year == dt.Year && x.ModifiedDate.Value.Day == dt.Day && x.ModifiedDate.Value.Month == dt.Month)).
                          Where(x => x.Skill == qryInboundSkillId)
                 .OrderByDescending(x => x.AddDate.Value).ThenByDescending(x => x.ModifiedDate.Value).ToList();
        jc.Serialize(query, sb);
        Engine.Dispose();
        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
    }
    private void GetAll(HttpRequest Request, HttpResponse Response)
    {
        var result = Engine.PhoneBarActions.GetACDInboundQueuePersonal();
        jc.Serialize(result, sb);
        Engine.Dispose();
        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
    }
    private void GetPendingCalls(HttpRequest Request, HttpResponse Response)
    {
        if (context.Session[Konstants.K_USERID] != null)
        {
            var result = Engine.PhoneBarActions.GetQueueAcdStatistics(Guid.Parse(context.Session[Konstants.K_USERID].ToString()));
            if (result != null && result.FirstOrDefault() != null)
            {
                var t = result.FirstOrDefault();

                ACDResponseClass acdResponse = new ACDResponseClass();
                acdResponse.AcdCallTaken = t.AcdCallTaken == null ? 0 : (int)t.AcdCallTaken;
                acdResponse.AcdCount = t.AcdCount == null ? 0 : (int)t.AcdCount;
                acdResponse.Reason = t.Reason;
                acdResponse.IsEnabled = t.IsEnabled == null ? false : (bool)t.IsEnabled;
                    
                jc.Serialize(acdResponse, sb);
            }
            Engine.Dispose();
        }
        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
    }
    private void GetByStatus(HttpRequest Request, HttpResponse Response)
    {
        if (context.Session[Konstants.K_USERID] != null)
        {
            var statusResult = Engine.PhoneBarActions.GetACDInboundQueuePersonal();
            string qryInboundSkillId = Request.QueryString["inboundSkillId"] != null ? Request.QueryString["inboundSkillId"] : string.Empty;

            string qryStatus = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;
            Guid userKey = Guid.Empty;
            if (context != null)
                userKey = new Guid(context.Session[Konstants.K_USERID].ToString());
            var T = Engine.UserActions.Get(userKey);
            IEnumerable<MODELS.State> states = null;
            if (T != null)
                states = T.States;
            var tt = states.ToList();
            //if(states != null)
            //var query = (from v in statusResult
            //             where (v.Status == qryStatus && v.Skill == qryInboundSkillId)
            //             select v
            //         ).Where(r => states.ToList().Find(delegate(MODELS.State o) { return o.Id == r.StateKey.Value.ToString(); }).Id == v).OrderByDescending(x => x.AddDate.Value).ThenByDescending(x => x.ModifiedDate.Value).ToList();
            //            jc.Serialize(query, sb);
            Engine.Dispose();
            Response.Write(sb.ToString());
            Response.ContentType = "application/json";
        }
    }
    public void GetCount(HttpRequest Request, HttpResponse Response)
    {
        var resultCount = Engine.PhoneBarActions.GetACDInboundQueuePersonal();
        int count = resultCount.Count();
        var MaxDate = (from d in resultCount select d.AddDate).Max();
        CountHelper objCountHelper = new CountHelper() { Count = count, LastDateTime = MaxDate == null ? "null" : MaxDate.Value.ToString() };

        jc.Serialize(objCountHelper, sb);
        Engine.Dispose();
        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
    }
    private void SetStatus(HttpRequest Request, HttpResponse Response)
    {
        const string Id = "id";
        string status = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;
        string id = Request.QueryString[Id] != null ? Request.QueryString[Id] : string.Empty;
        string inboundSkillId = Engine.PhoneBarActions.SaveAcd(id, status);

        string qryStatus = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;

        var statusResult = Engine.PhoneBarActions.GetACDInboundQueuePersonal();
        var query = (from v in statusResult
                     where v.Status == "pending" && v.Skill == inboundSkillId
                     select v).ToList();
        var MaxDate = (from d in statusResult select d.AddDate).Max();
        CountHelper objCountHelper = new CountHelper() { Count = query.Count(), LastDateTime = MaxDate == null ? "null" : MaxDate.Value.ToString() };

        jc.Serialize(objCountHelper, sb);
        Engine.Dispose();

        Response.Write(sb.ToString());
        Response.ContentType = "application/json";

        List<string> lstuserKeys = new List<string>();
        //lstuserKeys = get list of relevant users from DataBase

        //Engine.model
        
        
        //##################################### NEED TO PASS PARAM
        //UpdateCounts by invoking SignalR        
        //NotifySignalRACD();

        //Update counts using SignalR
        long QueueID = 0;
        if (long.TryParse(id, out QueueID))
        {
            NotifySignalRACD(QueueID);
        }
    }

    private void AddEdit(HttpRequest Request, HttpResponse Response)
    {
        long queueID = 0;        
        string timeStamp = Request.QueryString[TimeStamp] != null ? Request.QueryString[TimeStamp] : string.Empty;
        string skill = Request.QueryString[Skill] != null ? Request.QueryString[Skill] : string.Empty;
        string status = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;
        string contactid = Request.QueryString[ContactId] != null ? Request.QueryString[ContactId] : string.Empty;
        string phoneNumber = Request.QueryString[PhoneNumber] != null ? Request.QueryString[PhoneNumber] : string.Empty;
        string campaignId = Request.QueryString["campaignid"] != null ? Request.QueryString["campaignid"] : "0";
        DateTime dResult = DateTime.Now;
        bool checkForError = true;

        if (!DateTime.TryParse(timeStamp, out dResult))
        {
            if (!DateTime.TryParse(timeStamp, null, DateTimeStyles.RoundtripKind, out dResult))
            {
                jsonMessage.result = "failed";
                jsonMessage.description = "TimeStamp Date format incorrect";

                jc.Serialize(jsonMessage, sb);
                checkForError = false;
            }
        }
        CheckForEmptyString(Request);
        if (!string.IsNullOrEmpty(jsonMessage.result) && checkForError)
        {
            jc.Serialize(jsonMessage, sb);
        }
        else if (checkForError)
        {
            int reslt = Engine.PhoneBarActions.SaveAcd(timeStamp, skill, status, contactid, phoneNumber, campaignId, out queueID);
            if (reslt == 0)
                jsonMessage = new JsonMessage() { result = "success", description = "contactid " + contactid + " updated'}" };
            else
                jsonMessage = new JsonMessage() { result = "success", description = "contactid " + contactid + " inserted'}" };
            jc.Serialize(jsonMessage, sb);
            
            //SignalR web service reference
            //SignalRWebservices.SignalRWebServices srws = new SignalRWebservices.SignalRWebServices();
            //srws.InvokeHubUpdateCounts(skill);

            //TM 20 06 2014 Service call without webreference
        }
        //string result = NotifySignalR(skill);
        //Response.Write(sb.ToString() + " " + result);
        //Response.ContentType = "application/json";

        //List<string> lstuserKeys = new List<string>();
        //lstuserKeys = get list of relevant users from DataBase

        //Update ACD statistics table
        //Engine.PhoneBarActions.
        
        //UpdateCounts by invoking SignalR
        NotifySignalRACD(queueID);

        Engine.Dispose();
    }

    private string NotifySignalRACD(long queueID)
    {
        List<string> lstuserGuidStrings = new List<string>();
        try
        {   //TM [20 08 2014] Notify relevant users immediately
            //Engine.MODELS.ObjectResult<GetAgentListForACDUpdate_Result> ;

            var varLstOfGuids = Engine.PhoneBarActions.GetACDAgentList(queueID);

            
            if (varLstOfGuids != null)
            {
                foreach (var g in varLstOfGuids)
                {
                    if (g != null && g.agent_id != null)
                    {
                        lstuserGuidStrings.Add(g.agent_id.ToString());
                    }
                }
            }

            if (!lstuserGuidStrings.Any())
            {
                return "Empty list of users, no notification sent";
            }

            string url = ConfigurationManager.AppSettings["SignalRACDWebServiceURL"].ToString();
            Uri address = new Uri(url);
            
            //Create the web request
            System.Net.HttpWebRequest extrequest = System.Net.WebRequest.Create(address) as System.Net.HttpWebRequest;
            
            // Set type to POST 
            extrequest.Method = "POST";
            extrequest.ContentType = "application/x-www-form-urlencoded";

            var jsonSerialiser = new JavaScriptSerializer();
            var json = jsonSerialiser.Serialize(lstuserGuidStrings);

            // Create the data we want to send 
            StringBuilder data = new StringBuilder();
            data.Append("serializedUsersList=" + json);
            // Create a byte array of the data we want to send 
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

            // Set the content length in the request headers 
            extrequest.ContentLength = byteData.Length;

            // Write data 
            using (Stream postStream = extrequest.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }
            
            // Get response 
            //extrequest.GetResponse();
            using (System.Net.HttpWebResponse response1 = extrequest.GetResponse() as System.Net.HttpWebResponse)
            {
                //response1.Close();
            }
        }
        catch (Exception e)
        {
            return e.Message;
        }
        return "Notification sent to list of users; Count: " + lstuserGuidStrings.Count().ToString();
    }
    #endregion
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    public void CheckForEmptyString(HttpRequest Request)
    {
        if (!string.IsNullOrEmpty(Request.QueryString[PhoneNumber]) && !string.IsNullOrEmpty(Request.QueryString[TimeStamp]) && !string.IsNullOrEmpty(Request.QueryString[Status]) && !string.IsNullOrEmpty(Request.QueryString[ContactId]))
            return;
        jsonMessage.result = "failed";
        if (string.IsNullOrEmpty(Request.QueryString[TimeStamp]))
            jsonMessage.description += TimeStamp + " missing,";
        //if (string.IsNullOrEmpty(Request.QueryString[Skill]))
        //    jsonMessage.description += Skill + " missing,";
        if (string.IsNullOrEmpty(Request.QueryString[Status]))
            jsonMessage.description += Status + " missing,";
        if (string.IsNullOrEmpty(Request.QueryString[ContactId]))
            jsonMessage.description += ContactId + " missing,";
        if (string.IsNullOrEmpty(Request.QueryString[PhoneNumber]))
            jsonMessage.description += PhoneNumber + " missing,";
        jsonMessage.description = jsonMessage.description.TrimEnd(',');
    }

}

/// <summary>
/// Class that contains properties for user data, It is a temporary class, may be modified/removed in newer version
/// <author>TM</author>
/// </summary>
public class UserDatatemp
{
    public string agentKey { get; set; }
    public string SignalRurl { get; set; }
    //public int DisableSeconds { get; set; }
    public int maxDailyLead { get; set; }
}

public class ACDResponseClass
{
    public int AcdCallTaken;
    public int AcdCount;
    public string Reason;
    public bool IsEnabled;
}