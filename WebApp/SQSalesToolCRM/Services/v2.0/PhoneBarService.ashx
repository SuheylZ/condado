<%@ WebHandler Language="C#" Class="PhoneBarService" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Collections.Generic;
using MODELS = SalesTool.DataAccess.Models;
using System.Web.Script.Serialization;
using System.Text;

public class PhoneBarService : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
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
            case "Authenticate":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    string userID = Convert.ToString(context.Session[Konstants.K_USERID]);
                    MODELS.User _user = Engine.UserActions.Get(new Guid(userID));
                    User user = new User();
                    user.UserName = _user.PhoneSystemUsername;
                    user.Password = _user.PhoneSystemPassword;
                    user.StationType = _user.UserPhoneSystemStationType;
                    user.StationId = _user.UserPhoneSystemStationID;
                    user.InboundSkillId = _user.usr_phone_system_inbound_skillId;
                    user.PhoneSystemId = _user.PhoneSystemId;
                    //TM [26 june 2014] Added userID to the response to use in signalR
                    user.UserID = _user.Key.ToString();
                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    StringBuilder sb = new StringBuilder();
                    jc.Serialize(user, sb);
                    Engine.Dispose();
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            case "ApplicationStorageValues":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    SalesTool.DataAccess.GlobalAppSettings settings;
                    settings = Engine.ApplicationSettings;// SalesTool.DataAccess.DBEngine.ApplicationSettingsInstance;
                   
                    ServiceSettings phoneBarSettings = new ServiceSettings()
                     {
                         PhoneSystemAPIClientID = settings.PhoneSystemAPIClientID,
                         PhoneSystemAPIClientScope = settings.PhoneSystemAPIScope,
                         PhoneSystemAPISecret = settings.PhoneSystemAPISecret,
                         PhoneSystemAPIServiceURI = settings.PhoneSystemAPIServiceURI,
                         PhoneSystemScreenPop = settings.PhoneSystemScreenPop,
                         SignalRurl = string.IsNullOrEmpty(ConfigurationManager.AppSettings["SignalRurl"]) ? "" : ConfigurationManager.AppSettings["SignalRurl"],
                     };

                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    StringBuilder sb1 = new StringBuilder();
                    jc.Serialize(phoneBarSettings, sb1);
                    Engine.Dispose();
                    Response.Write(sb1.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            case "AnisAndDnis":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    if (Request.QueryString["DNIS"] != null && Request.QueryString["ANIS"] != null)
                    {
                        string dnis = Request.QueryString["DNIS"];
                        string anis = Request.QueryString["ANIS"];
                        string result = Engine.UserActions.GetScreenPop(anis, dnis);
                        string contactId = Request.QueryString["ContactID"];
                        AnisDnisLink objLink = new AnisDnisLink() { Link = result + "&contactId=" + contactId };

                        JavaScriptSerializer jc = new JavaScriptSerializer();
                        StringBuilder sb = new StringBuilder();
                        jc.Serialize(objLink, sb);
                        Engine.Dispose();
                        Response.Write(sb.ToString());
                        Response.ContentType = "application/json";
                    }
                }
                break;
            case "AgentInformation":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    string userID = Convert.ToString(context.Session[Konstants.K_USERID]);
                    MODELS.User _user = Engine.UserActions.Get(new Guid(userID));
                    User user = new User();
                    user.StationType = _user.UserPhoneSystemStationType;
                    user.StationId = _user.UserPhoneSystemStationID;
                    user.FullName = _user.FullName;
                    user.WorkPhone = _user.WorkPhone;

                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    StringBuilder sb = new StringBuilder();
                    jc.Serialize(user, sb);
                    Engine.Dispose();
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            case "ResetPassword":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    string userID = Convert.ToString(context.Session[Konstants.K_USERID]);

                    MODELS.User _user = Engine.UserActions.Get(new Guid(userID));
                    if (context.Request.Form["password"] != null)
                    {
                        _user.PhoneSystemPassword = context.Request.Form["password"];
                        _user.UserPhoneSystemStationID = context.Request.Form["phoneNumber"];
                        _user.PhoneSystemUsername = context.Request.Form["userName"];

                        if (context.Request.Form["chkPhoneNumber"].ToLower() == "phonenumber" || context.Request.Form["chkPhoneNumber"].ToLower() == "true")
                            _user.UserPhoneSystemStationType = "1";
                        else
                            _user.UserPhoneSystemStationType = "0";

                        Engine.UserActions.Update(_user);
                        Engine.Dispose();
                    }
                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    StringBuilder sb = new StringBuilder();
                    jc.Serialize("Success", sb);
                    Response.ContentType = "application/json";
                    Response.Write(sb.ToString());
                }
                break;
            case "HotKeyValues":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    SalesTool.DataAccess.GlobalAppSettings settings;
                    settings = Engine.ApplicationSettings;//SalesTool.DataAccess.DBEngine.ApplicationSettingsInstance;

                    HotKeyHelper objHotKeys = new HotKeyHelper()
                    {
                        HotKey1ID = settings.PhoneSystemHotKey1ID,
                        HotKey1Label = settings.PhoneSystemHotKey1Label,
                        HotKey1Type = settings.PhoneSystemHotKey1Type,

                        HotKey2ID = settings.PhoneSystemHotKey2ID,
                        HotKey2Label = settings.PhoneSystemHotKey2Label,
                        HotKey2Type = settings.PhoneSystemHotKey2Type,

                        HotKey3ID = settings.PhoneSystemHotKey3ID,
                        HotKey3Label = settings.PhoneSystemHotKey3Label,
                        HotKey3Type = settings.PhoneSystemHotKey3Type,

                        HotKey4ID = settings.PhoneSystemHotKey4ID,
                        HotKey4Label = settings.PhoneSystemHotKey4Label,
                        HotKey4Type = settings.PhoneSystemHotKey4Type,

                        HotKey5ID = settings.PhoneSystemHotKey5ID,
                        HotKey5Label = settings.PhoneSystemHotKey5Label,
                        HotKey5Type = settings.PhoneSystemHotKey5Type,

                        HotKey6ID = settings.PhoneSystemHotKey6ID,
                        HotKey6Label = settings.PhoneSystemHotKey6Label,
                        HotKey6Type = settings.PhoneSystemHotKey6Type,
                    };

                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    StringBuilder sb = new StringBuilder();
                    jc.Serialize(objHotKeys, sb);
                    Engine.Dispose();
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            default:
                break;
        }
    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }
    public class ServiceSettings
    {
        public string PhoneSystemAPISecret { get; set; }
        public string PhoneSystemAPIClientID { get; set; }
        public string PhoneSystemAPIClientScope { get; set; }
        public string PhoneSystemAPIServiceURI { get; set; }
        public string SignalRurl { get; set; }
        public string PhoneSystemScreenPop { get; set; }
        public string AssemblyVersion { get; set; }
    }
    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string StationId { get; set; }
        public string StationType { get; set; }
        public string FullName { get; set; }
        public string WorkPhone { get; set; }
        public string InboundSkillId { get; set; }
        public string PhoneSystemId { get; set; }
        //TM [26 June 2014] Adding user key for signalR
        public string UserID { get; set; }
    }
    public class HotKeyHelper
    {
        public string HotKey1Label { get; set; }
        public string HotKey1Type { get; set; }
        public string HotKey1ID { get; set; }
        public string HotKey2Label { get; set; }
        public string HotKey2Type { get; set; }
        public string HotKey2ID { get; set; }
        public string HotKey3Label { get; set; }
        public string HotKey3Type { get; set; }
        public string HotKey3ID { get; set; }

        public string HotKey4Label { get; set; }
        public string HotKey4Type { get; set; }
        public string HotKey4ID { get; set; }
        public string HotKey5Label { get; set; }
        public string HotKey5Type { get; set; }
        public string HotKey5ID { get; set; }
        public string HotKey6Label { get; set; }
        public string HotKey6Type { get; set; }
        public string HotKey6ID { get; set; }
    }
    public class AnisDnisLink
    {
        public string Link { get; set; }
    }
}