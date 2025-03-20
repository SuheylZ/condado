<%@ WebHandler Language="C#" Class="PhoneQueuePersonal" %>

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

public class PhoneQueuePersonal : IHttpHandler
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

    public void ProcessRequest(HttpContext context)
    {
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        string method = string.Empty;
        if (!string.IsNullOrEmpty(Request.QueryString["method"]))
            method = Request.QueryString["method"];

        switch (method)
        {
            case "GetAll":
                GetAll(Request, Response);
                break;
            case "GetCountByStatus":
                GetCountByStatus(Request, Response);
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
            default:
                break;
        }
    }
    #region Helper Methods
    private void GetMissedRejected(HttpRequest Request, HttpResponse Response)
    {
        var statusResult = Engine.PhoneBarActions.GetInboundQueuePersonal();
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
        var result = Engine.PhoneBarActions.GetInboundQueuePersonal();
        jc.Serialize(result, sb);
        Engine.Dispose();
        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
    }
    private void GetCountByStatus(HttpRequest Request, HttpResponse Response)
    {
        var statusResult = Engine.PhoneBarActions.GetInboundQueuePersonal();
        string qryStatus = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;
        string qryInboundSkillId = Request.QueryString["inboundSkillId"] != null ? Request.QueryString["inboundSkillId"] : string.Empty;
        var query = (from v in statusResult
                     where v.Status == qryStatus && v.Skill == qryInboundSkillId
                     select v).ToList();
        var MaxDate = (from d in statusResult select d.AddDate).Max();
        CountHelper objCountHelper = new CountHelper() { Count = query.Count(), LastDateTime = MaxDate == null ? "null" : MaxDate.Value.ToString() };
        jc.Serialize(objCountHelper, sb);
        Engine.Dispose();
        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
    }
    private void GetByStatus(HttpRequest Request, HttpResponse Response)
    {
        var statusResult = Engine.PhoneBarActions.GetInboundQueuePersonal();
        string qryInboundSkillId = Request.QueryString["inboundSkillId"] != null ? Request.QueryString["inboundSkillId"] : string.Empty;
        string qryStatus = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;
        var query = (from v in statusResult
                     where (v.Status == qryStatus && v.Skill == qryInboundSkillId)
                     select v
                 ).OrderByDescending(x => x.AddDate.Value).ThenByDescending(x => x.ModifiedDate.Value).ToList();
        jc.Serialize(query, sb);
        Engine.Dispose();
        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
    }
    public void GetCount(HttpRequest Request, HttpResponse Response)
    {
        var resultCount = Engine.PhoneBarActions.GetInboundQueuePersonal();
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
        string inboundSkillId = Engine.PhoneBarActions.Save(id, status);

        string qryStatus = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;

        var statusResult = Engine.PhoneBarActions.GetInboundQueuePersonal();
        var query = (from v in statusResult
                     where v.Status == "pending" && v.Skill == inboundSkillId
                     select v).ToList();
        var MaxDate = (from d in statusResult select d.AddDate).Max();
        CountHelper objCountHelper = new CountHelper() { Count = query.Count(), LastDateTime = MaxDate == null ? "null" : MaxDate.Value.ToString() };

        jc.Serialize(objCountHelper, sb);
        Engine.Dispose();

        Response.Write(sb.ToString());
        Response.ContentType = "application/json";
        //UpdateCounts by invoking SignalR
        NotifySignalR(inboundSkillId);
    }

    private void AddEdit(HttpRequest Request, HttpResponse Response)
    {
        string timeStamp = Request.QueryString[TimeStamp] != null ? Request.QueryString[TimeStamp] : string.Empty;
        string skill = Request.QueryString[Skill] != null ? Request.QueryString[Skill] : string.Empty;
        string status = Request.QueryString[Status] != null ? Request.QueryString[Status] : string.Empty;
        string contactid = Request.QueryString[ContactId] != null ? Request.QueryString[ContactId] : string.Empty;
        string phoneNumber = Request.QueryString[PhoneNumber] != null ? Request.QueryString[PhoneNumber] : string.Empty;
        
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
            int reslt = Engine.PhoneBarActions.Save(timeStamp, skill, status, contactid, phoneNumber);
            if (reslt == 0)
                jsonMessage = new JsonMessage() { result = "success", description = "contactid " + contactid + " updated'}" };
            else
                jsonMessage = new JsonMessage() { result = "success", description = "contactid " + contactid + " inserted'}" };
            jc.Serialize(jsonMessage, sb);
            Engine.Dispose();
            //SignalR web service reference
            //SignalRWebservices.SignalRWebServices srws = new SignalRWebservices.SignalRWebServices();
            //srws.InvokeHubUpdateCounts(skill);

            //TM 20 06 2014 Service call without webreference
        }
        string result = NotifySignalR(skill);
        //Response.Write(sb.ToString() + " " + result);
        //Response.ContentType = "application/json";
    }

    private static string NotifySignalR(string skill)
    {
        try
        {
            {
                string url = ConfigurationManager.AppSettings["SignalRWebServiceURL"].ToString();
                Uri address = new Uri(url);
                //Create the web request
                System.Net.HttpWebRequest extrequest = System.Net.WebRequest.Create(address) as System.Net.HttpWebRequest;

                // Set type to POST 
                extrequest.Method = "POST";
                extrequest.ContentType = "application/x-www-form-urlencoded";

                // Create the data we want to send 
                StringBuilder data = new StringBuilder();
                data.Append("SkillID=" + skill);
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
        }
        catch (Exception e)
        {
            return e.Message;
        }
        return string.Empty;
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
        if (!string.IsNullOrEmpty(Request.QueryString[PhoneNumber]) && !string.IsNullOrEmpty(Request.QueryString[TimeStamp]) && !string.IsNullOrEmpty(Request.QueryString[Skill]) && !string.IsNullOrEmpty(Request.QueryString[Status]) && !string.IsNullOrEmpty(Request.QueryString[ContactId]))
            return;
        jsonMessage.result = "failed";
        if (string.IsNullOrEmpty(Request.QueryString[TimeStamp]))
            jsonMessage.description += TimeStamp + " missing,";
        if (string.IsNullOrEmpty(Request.QueryString[Skill]))
            jsonMessage.description += Skill + " missing,";
        if (string.IsNullOrEmpty(Request.QueryString[Status]))
            jsonMessage.description += Status + " missing,";
        if (string.IsNullOrEmpty(Request.QueryString[ContactId]))
            jsonMessage.description += ContactId + " missing,";
        if (string.IsNullOrEmpty(Request.QueryString[PhoneNumber]))
            jsonMessage.description += PhoneNumber + " missing,";
        jsonMessage.description = jsonMessage.description.TrimEnd(',');
    }
}
