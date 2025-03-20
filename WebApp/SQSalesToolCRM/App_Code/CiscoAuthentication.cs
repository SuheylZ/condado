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
using System.IO;
using System.Net;
using System.Xml;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json.Linq;

public class CiscoAuthenticaiton : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{
    #region Variables
    private HttpContext ctx;
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
    private SalesTool.DataAccess.GlobalAppSettings _settings = null;
    public SalesTool.DataAccess.GlobalAppSettings settings
    {
        get
        {
            if (_settings == null)
                _settings = SalesTool.DataAccess.DBEngine.ApplicationSettingsInstance;
            return _settings;
        }
    }
    #endregion
    #region ProcessRequest
    public void ProcessRequest(HttpContext context)
    {
        #region Variables
        HttpRequest Request = context.Request;
        HttpResponse Response = context.Response;
        ctx = context;
        //Log("something");
        HttpWebRequest request;
        StringBuilder sb = new StringBuilder();
        string jsonText = string.Empty;
        string method = string.Empty;
        string _URL = string.Empty;
        #endregion
        if (!string.IsNullOrEmpty(Request.QueryString["method"]))
            method = Request.QueryString["method"];
        switch (method)
        {
            #region Refresh User
            case "RefreshUser":
                GetUserRefresh(Request, Response);
                break;
            #endregion
            #region GetCredentials
            case "GetCredentials":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    string userID = Convert.ToString(context.Session[Konstants.K_USERID]);
                    MODELS.User _user = Engine.UserActions.Get(new Guid(userID));
                    User user = new User();
                    user.UserName = _user.Cisco_AgentId;
                    user.Password = _user.Cisco_AgentPassword;
                    user.Extention1 = _user.Cisco_AgentExtension1;
                    user.Extention2 = _user.Cisco_AgentExtension2;
                    user.Url = _user.Cisco_DomainAddress;
                    user.UserID = _user.Key.ToString();
                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    sb = new StringBuilder();
                    jc.Serialize(user, sb);
                    Engine.Dispose();
                    Response.Write(sb.ToString());
                    Response.ContentType = "application/json";
                }
                break;
            #endregion
            #region Authenticate
            case "Authenticate":
                try
                {
                    _URL = Request.Form[0];
                    request = (HttpWebRequest)WebRequest.Create(settings.CiscoAPIUrl + settings.CiscoWebAPIPath + _URL);
                    request.Method = "PUT";
                    request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                    request.Headers.Add("contenttype", "application/xml");
                    request.ContentType = "Application/XML";
                    using (var s = request.GetRequestStream())
                    using (var sw = new StreamWriter(s))
                    {
                        //sw.Write("<User><state>LOGOUT</state></User>");
                        sw.Write("<User><state>LOGIN</state><extension>" + Request.Form[1] + "</extension></User>");
                    }
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString() == "Accepted")
                        {
                            string cookieValue = response.Headers.GetValues("Set-Cookie")[1];
                            JavaScriptSerializer jc = new JavaScriptSerializer();

                            jc.Serialize(new CiscoCookie() { Cookie = cookieValue }, sb);
                            var encoding = ASCIIEncoding.ASCII;
                            using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                            {
                                string responseText = reader.ReadToEnd();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    sb.Append(ex.Message);
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    Response.StatusDescription = ex.Message;
                }
                Response.Write(sb.ToString());
                Response.ContentType = "json";
                break;
            #endregion
            #region Get User
            case "GetUser":
                GetUser(Request, Response);
                break;
            #endregion
            #region ChangeState
            case "ChangeState":
                _URL = Request.Form[0];
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                string stateName = Request.Form[3];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<User><state>" + stateName + "</state></User>");
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        JavaScriptSerializer jc = new JavaScriptSerializer();
                        jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        Log("Agent State Changed");
                    }
                }
                Response.Write(sb.ToString());
                Response.ContentType = "json";
                break;
            #endregion
            #region OutBoundCalls
            case "OutBoundCall":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "POST";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                string phoneNumber = Request.Form[4];
                string extension = Request.Form[3];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<Dialog><requestedAction>MAKE_CALL</requestedAction><fromAddress>" + extension + "</fromAddress><toAddress>" + phoneNumber + "</toAddress></Dialog>");
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        GetUserRefresh(Request, Response);
                        //JavaScriptSerializer jc = new JavaScriptSerializer();
                        //jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        //jsonText = GetDialog(Request, Response);
                        Log("OutBound Call to " + phoneNumber);
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
            #region HoldContact
            case "HoldContact":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);

                extension = Request.Form[3];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>HOLD</requestedAction></Dialog>");
                }//918163827026
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        GetUserRefresh(Request, Response);
                        Log("Hold Contact ");
                        //JavaScriptSerializer jc = new JavaScriptSerializer();
                        //jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        //jsonText = GetDialog(Request, Response);
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
            #region ResumeContact
            case "ResumeContact":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);

                extension = Request.Form[3];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>RETRIEVE</requestedAction></Dialog>");
                }//918163827026
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        GetUserRefresh(Request, Response);
                        Log("Resume Contact");
                        //JavaScriptSerializer jc = new JavaScriptSerializer();
                        //jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        //jsonText = GetDialog(Request, Response);
                    }
                }
                //Response.Write(jsonText);
                //Response.ContentType = "json";
                break;
            #endregion
            #region HangUpContact
            case "HangUpContact":
                try
                {
                    _URL = Request.Form[0];
                    request = null;
                    request = (HttpWebRequest)WebRequest.Create(_URL);
                    request.Method = "PUT";
                    request.Headers.Add("Authorization", "Basic " + Request.Form[2]);

                    extension = Request.Form[3];
                    request.Headers.Add("contenttype", "application/xml");
                    request.ContentType = "Application/XML";
                    using (var s = request.GetRequestStream())
                    using (var sw = new StreamWriter(s))
                    {
                        sw.Write("<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>DROP</requestedAction></Dialog>");
                    }//918163827026
                    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                    {
                        if (response.StatusCode.ToString() == "Accepted")
                        {
                            GetUserRefresh(Request, Response);
                            //JavaScriptSerializer jc = new JavaScriptSerializer();
                            //jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                            //jsonText = GetDialog(Request, Response);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Response.StatusCode = (int)HttpStatusCode.NotFound;
                    Response.StatusDescription = ex.Message;
                }
                //Response.Write(jsonText);
                //Response.ContentType = "json";
                break;
            #endregion
            #region AnswerCall
            case "AnswerCall":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);

                extension = Request.Form[3];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<Dialog><targetMediaAddress>" + extension + "</targetMediaAddress><requestedAction>ANSWER</requestedAction></Dialog>");
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        JavaScriptSerializer jc = new JavaScriptSerializer();
                        jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        jsonText = GetDialog(Request, Response);
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
            #region Transfer Call
            case "TransferCall":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                string toAddress = Request.Form[4];
                string targetMediaAddress = Request.Form[3];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<Dialog><targetMediaAddress>" + targetMediaAddress + "</targetMediaAddress><requestedAction>TRANSFER</requestedAction></Dialog>");
                }//918163827026
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        JavaScriptSerializer jc = new JavaScriptSerializer();
                        jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        jsonText = GetDialog(Request, Response);
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
            #region Conference Call
            case "ConferenceCall":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                targetMediaAddress = Request.Form[3];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<Dialog> <targetMediaAddress>" + targetMediaAddress + "</targetMediaAddress><requestedAction>CONFERENCE</requestedAction></Dialog>");
                }//918163827026
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        JavaScriptSerializer jc = new JavaScriptSerializer();
                        jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        jsonText = GetDialog(Request, Response);
                        Log("Conference Call ");
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
            #region ConsultCall
            case "ConsultCall":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                targetMediaAddress = Request.Form[3];
                phoneNumber = Request.Form[4];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {

                    sw.Write("<Dialog> <targetMediaAddress>" + targetMediaAddress + "</targetMediaAddress><toAddress>" + phoneNumber + "</toAddress><requestedAction>CONSULT_CALL</requestedAction></Dialog>");


                }//918163827026
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        JavaScriptSerializer jc = new JavaScriptSerializer();
                        jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        jsonText = GetDialog(Request, Response);
                        Log("Consult Call");
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
            #region Company Details
            case "CompanyDetails":
                if (context.Session[Konstants.K_USERID] != null)
                {
                    List<MODELS.ArcCompanyDirectory> lst = Engine.PhoneBarActions.getCompanyDetails();
                    sb = new StringBuilder();
                    JavaScriptSerializer jc = new JavaScriptSerializer();
                    jc.Serialize(lst, sb);
                    Engine.Dispose();
                }
                Response.Write(sb.ToString());
                Response.ContentType = "json";
                break;
            #endregion
            #region PhoneBook
            case "GetPhoneBookData":
                try
                {
                    string accountId = Request.Form[0];
                    var objArc = Engine.ArcActions.Get();
                    Int64 actId = Convert.ToInt64(accountId);
                    var query = (from v in objArc
                                 select new { v.AccountKey, v.ArcRefreanceKey }).
                     Where(x => (x.AccountKey == actId)).ToList();
                    foreach (var item in query)
                    {
                        jsonText = GetPhoneBook(item.ArcRefreanceKey);
                        dynamic d = JObject.Parse(jsonText);
                        if (d == null || d.CaseResults == null || d.CaseResults[0] == null || d.CaseResults[0].Status == null || d.CaseResults[0].Status.Value == null || d.CaseResults[0].Status.Value != "Accepted")
                        {
                            Response.StatusDescription = "This reference number " + item.ArcRefreanceKey + " is not working";
                            Response.StatusCode = (int)HttpStatusCode.PreconditionFailed;
                        }
                        else
                        {
                            Response.StatusCode = (int)HttpStatusCode.OK;
                            break;
                        }
                    }
                }
                catch (Exception ex) { Response.StatusDescription = ex.Message; Response.StatusCode = 401; }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
            #region SEND DTMF
            case "SendDTMF":
                _URL = Request.Form[0];
                request = null;
                request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "PUT";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                targetMediaAddress = Request.Form[3];
                phoneNumber = Request.Form[4];
                request.Headers.Add("contenttype", "application/xml");
                request.ContentType = "Application/XML";
                using (var s = request.GetRequestStream())
                using (var sw = new StreamWriter(s))
                {
                    sw.Write("<Dialog><requestedAction>SEND_DTMF</requestedAction><targetMediaAddress>" + targetMediaAddress + "</targetMediaAddress><actionParams><ActionParam><name>dtmfString</name><value>" + phoneNumber + "</value></ActionParam></actionParams></Dialog>");

                    //sw.Write("<Dialog><targetMediaAddress>" + targetMediaAddress + "<actionParams><ActionParam><name>dtmfString</name><value>" + phoneNumber + "</value></ActionParam></actionParams><requestedAction>SEND_DTMF</requestedAction></Dialog>");
                }
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "Accepted")
                    {
                        JavaScriptSerializer jc = new JavaScriptSerializer();
                        jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
                        jsonText = GetDialog(Request, Response);
                        // Log("Consult Call");
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
                break;
            #endregion
        }
    }
    #endregion
    #region Methods

    public void Log(string action)
    {
        //File.AppendAllText(ctx.Server.MapPath("log") + @"\ciscolog.txt", System.Environment.NewLine + action + " " + DateTime.Now.AddHours(-10).ToString("dd/MM/yyyy hh:mm:ss tt"));
    }
    public string GetPhoneBook(string arcReferenceKey)
    {
        StringBuilder sb = new StringBuilder();
        string jsonText = "{'Login': {'UserId': '" + settings.CiscoPhoneBarArcServiceUserName + "','Password': '" + settings.CiscoPhoneBarArcServiceUserPassword + "','id': " + settings.CiscoPhoneBarArcServiceId + " },'Cases': [ {'Reference': '" + arcReferenceKey + "' }]}";
        string _URL = settings.CiscoPhoneBarArcServiceURL;
        HttpWebRequest request = null;
        request = (HttpWebRequest)WebRequest.Create(_URL);
        request.Method = "POST";
        request.Headers.Add("contenttype", "application/json");
        request.ContentType = "application/json";
        using (var s = request.GetRequestStream())
        using (var sw = new StreamWriter(s))
        {
            sw.Write(jsonText);
        }
        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            JavaScriptSerializer jc = new JavaScriptSerializer();
            jc.Serialize(new ResponseResult() { Result = "Accepted" }, sb);
            XmlDocument xmlDoc = new XmlDocument();
            Stream stream = response.GetResponseStream();
            jsonText = new StreamReader(stream).ReadToEnd();
        }
        return jsonText;
    }

    public void GetUser(HttpRequest Request, HttpResponse Response)
    {
        string _URL = Request.Form[0];
        string jsonText = string.Empty;
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
        request.Method = "GET";
        request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
        request.Headers.Add("contenttype", "application/xml");

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            if (response.StatusCode.ToString() == "OK")
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                jsonText = JsonConvert.SerializeXmlNode(xmlDoc);
            }
        }
        Response.Write(jsonText);
        Response.ContentType = "json";
    }

    public void GetUserRefresh(HttpRequest Request, HttpResponse Response)
    {
        string jsonText = string.Empty;
        try
        {
            if (Request.Form.Count > 2)
            {
                string _URL = Request.Form["data[refreshURL]"];
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
                request.Method = "GET";
                request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
                request.Headers.Add("contenttype", "application/xml");

                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode.ToString() == "OK")
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.Load(response.GetResponseStream());
                        jsonText = "[" + JsonConvert.SerializeXmlNode(xmlDoc);
                        jsonText += "," + GetDialog(Request, Response) + "]";
                        var data = JsonConvert.DeserializeObject(jsonText);
                        jsonText = JsonConvert.SerializeObject(data);
                    }
                }
                Response.Write(jsonText);
                Response.ContentType = "json";
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.StatusDescription = ex.Message;
        }
    }

    public string GetDialog(HttpRequest Request, HttpResponse Response)
    {
        string jsonText = string.Empty;
        try
        {
            string _URL = Request.Form["data[refreshURL]"] + "/Dialogs";

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_URL);
            request.Method = "GET";
            request.Headers.Add("Authorization", "Basic " + Request.Form[2]);
            request.Headers.Add("contenttype", "application/xml");

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                //if (response.StatusCode.ToString() == "OK")
                //{
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(response.GetResponseStream());
                jsonText = JsonConvert.SerializeXmlNode(xmlDoc);

                //}
            }
        }
        catch (Exception ex)
        {
            Response.StatusCode = (int)HttpStatusCode.NotFound;
            Response.StatusDescription = ex.Message;
        }
        return jsonText;
    }
    #endregion
    public bool IsReusable
    {
        get
        {
            return false;
        }
    }


    #region Helper Classes
    public class CiscoCookie
    {
        public string Cookie { get; set; }
    }

    public class User
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Extention1 { get; set; }
        public string Extention2 { get; set; }
        public string Url { get; set; }
        public string UserID { get; set; }
    }

    public class CallData
    {
        public string DialogId { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public string CallType { get; set; }
    }
    public class ResponseResult
    {
        public string Result { get; set; }
    }
    #endregion
}