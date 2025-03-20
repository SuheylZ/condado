<%@ WebHandler Language="C#" Class="RouteToOwner" %>

using System;
using System.Web;
using System.Web.Script.Serialization;
using System.Text;
using MODELS = SalesTool.DataAccess.Models;

public class RouteToOwner : IHttpHandler
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
            case "Phone":
                if (Request.QueryString["SystemPhone"] != null)
                {
                    string phone = Request.QueryString["SystemPhone"];
                    string result = Engine.UserActions.GetUserPhoneSystemUserNameByPhoneNumber(phone);
                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    StringBuilder sb = new StringBuilder();
                    AgentHelper agent = new AgentHelper() { AgentID = result };
                    jc.Serialize(agent, sb);
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

    public class AgentHelper
    {
        public string AgentID { get; set; }
    }
}