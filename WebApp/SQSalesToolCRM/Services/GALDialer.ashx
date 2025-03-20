<%@ WebHandler Language="C#" Class="GALDialer" %>

using System;
using System.Web;
using System.Linq;
using System.Collections.Generic;
using System.Windows.Forms.VisualStyles;
using SalesTool.DataAccess;
using System.Web.Security;

public class GALDialer : IHttpHandler, System.Web.SessionState.IRequiresSessionState
{


    CiscoMethods funcs = new CiscoMethods();
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

    private SalesTool.DataAccess.Models.User _user = null;
    public SalesTool.DataAccess.Models.User CurrentUser
    {
        get { return _user; }
        set { _user = value;}
    }

    public GALDialer(Guid agentId)
    {
        _helper = new SalesTool.DataAccess.ApplicationStorageHelper("ApplicationServices");
        AgentId = agentId;
    }

    public GALDialer()
    {
        _helper = new SalesTool.DataAccess.ApplicationStorageHelper("ApplicationServices");
    }

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            //AgentId = new Guid(Convert.ToString(context.Session[Konstants.K_USERID]));
            Guid tempGuid = new Guid();
            if (context.Session[Konstants.K_USERID] != null && _user == null)
            {
                Guid key = Guid.Empty;
                key = new Guid(context.Session[Konstants.K_USERID].ToString());
                _user = Engine.UserActions.Get(key);
                AgentId = _user.Key;
            }
            HttpRequest Request = context.Request;
            HttpResponse Response = context.Response;
            string method = string.Empty;
            if (!string.IsNullOrEmpty(Request.QueryString["method"]))
                method = Request.QueryString["method"];

            switch (method)
            {
                case "DialLead":

                    //if (!(context.Session[Konstants.K_USERID] != null && Guid.TryParse(context.Session[Konstants.K_USERID].ToString().Trim(), out tempGuid)))
                    //{
                    //    //HttpResponse ResponseTemp = context.Response;
                    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //    System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                    //    jc.Serialize(string.Empty, sb);
                    //    Response.Write(sb.ToString());
                    //    Response.ContentType = "application/json";
                    //    //Response.End();
                    //    //return;
                    //}
                    //else
                    {
                        if (Guid.TryParse(context.Session[Konstants.K_USERID].ToString(), out tempGuid))
                        {
                            AgentId = tempGuid;
                        }
                        
                        if (context.Session[Konstants.K_USERID] == null || !Guid.TryParse(context.Session[Konstants.K_USERID].ToString(), out tempGuid))
                        {
                            break; 
                        }
                        long accountID = 0;
                        string dialLeadRes = DialLead(out accountID);
                        //string dialLeadRes = DialLead();
                        System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        jc.Serialize(dialLeadRes, sb);
                        Response.Write(sb.ToString());
                        Response.ContentType = "application/json";
                        
                        //Commented out until Go ahead and further testing
                        //NotifyListofAgents(accountID);                        
                    }
                    break;

                case "GetCounts":
                    //if (context.Session[Konstants.K_USERID] == null || !Guid.TryParse(context.Session[Konstants.K_USERID].ToString(), out tempGuid))
                    //{
                    //    //HttpResponse ResponseTemp = context.Response;
                    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //    System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                    //    jc.Serialize(string.Empty, sb);
                    //    Response.Write(sb.ToString());
                    //    Response.ContentType = "application/json";
                    //    //Response.End();
                    //    //return;
                    //}
                    //else
                    {
                        GAL_LeadBasicDisplay GALBasicDisplay = GetLeadBasicDisplay();
                        System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        jc.Serialize(GALBasicDisplay, sb);
                        Response.Write(sb.ToString());
                        Response.ContentType = "application/json";    
                    }                    
                    break;

                case "GetUserData":
                    //if (context.Session[Konstants.K_USERID] == null || !Guid.TryParse(context.Session[Konstants.K_USERID].ToString(), out tempGuid))
                    //{
                    //    //HttpResponse ResponseTemp = context.Response;
                    //    System.Text.StringBuilder sb = new System.Text.StringBuilder();
                    //    System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                    //    jc.Serialize(string.Empty, sb);
                    //    Response.Write(sb.ToString());
                    //    Response.ContentType = "application/json";
                    //    //Response.End();
                    //    //return;
                    //}
                    //else
                    {                        
                        UserData userData = Getuserdata(context);
                        System.Web.Script.Serialization.JavaScriptSerializer jc = new System.Web.Script.Serialization.JavaScriptSerializer();
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        jc.Serialize(userData, sb);
                        Response.Write(sb.ToString());
                        Response.ContentType = "application/json";
                    }
                    break;
                case "NewLead":
                    long AccountID = 0;
                    if (!long.TryParse(Request.QueryString["accountID"], out AccountID))
                    {
                        Logger.Logfile("GALDialler NEWLEAD Invalid Account Id passed. account ID: " + AccountID);
                        break;
                    }

                    NotifyListofAgents(AccountID);
                    break;

                default:
                    break;
            }
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private static object mutexLeadBasicDisplayAllAgents = new object();
    public void NotifyListofAgents(long AccountID)
    {
        List<Guid> lstGuid = new List<Guid>();
        //Its already being called from within LeadBasicDisplayAllAgents
        //Loader.Database.ExecuteSqlCommand("exec spGalUpdate");
        lock (mutexLeadBasicDisplayAllAgents)
        {
            Loader.Database.ExecuteSqlCommand("exec LeadBasicDisplayAllAgents");
            //Call SP to get the list
            lstGuid = LoadUserIDs<List<Guid>>(AccountID);
        }
        
        List<string> lstUserIDGuidStrings = new List<string>();
        if (lstGuid != null)
        {
            foreach (var g in lstGuid)
            {
                if (g != null)
                {
                    lstUserIDGuidStrings.Add(g.ToString());
                }
            }
        }

        //if there is no userID in the list then return without notifying
        if (!lstUserIDGuidStrings.Any())
        {
            Logger.Logfile("WEB GAL List of Agents procedure retuned no user ID");
            return;
        }

        var jsonSerialiser = new System.Web.Script.Serialization.JavaScriptSerializer();
        var json = jsonSerialiser.Serialize(lstUserIDGuidStrings);

        NotifyDialerSignalR(json);
    }


    public UserData Getuserdata(HttpContext context)
    {
        UserData userData = new UserData();

        userData.agentKey = Convert.ToString(context.Session[Konstants.K_USERID]);

        userData.DisableSeconds = Settings.DialerDisableDialSeconds;
        //userData.maxDailyLead = 0; //Get from DB
        //userData.maxDailyLead = (int)LoadData<int>("GetWebGalMaxDailyCount");
        userData.SignalRurl =
            string.IsNullOrEmpty(System.Configuration.ConfigurationManager.AppSettings["SignalRurl"]) ?
            "" : System.Configuration.ConfigurationManager.AppSettings["SignalRurl"];
        return userData;
    }

    // Would be removed when alternates are ready. planned to be part of GAL Engine with DB polling.
    private static string NotifyDialerSignalR(string serializedListUserIDs)
    {
        try
        {
            {
                string url = System.Configuration.ConfigurationManager.AppSettings["SignalRDialerWebServiceURL"].ToString();
                Uri address = new Uri(url);

                //Create the web request
                System.Net.HttpWebRequest extrequest = System.Net.WebRequest.Create(address) as System.Net.HttpWebRequest;

                // Set type to POST 
                extrequest.Method = "POST";
                extrequest.ContentType = "application/x-www-form-urlencoded";

                // Create the data we want to send 
                System.Text.StringBuilder data = new System.Text.StringBuilder();
                data.Append("serializedUsersList=" + serializedListUserIDs);

                // Create a byte array of the data we want to send 
                byte[] byteData = System.Text.UTF8Encoding.UTF8.GetBytes(data.ToString());

                // Set the content length in the request headers 
                extrequest.ContentLength = byteData.Length;

                // Write data 
                using (System.IO.Stream postStream = extrequest.GetRequestStream())
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

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

    private bool _isDisposed;
    private readonly ApplicationStorageHelper _helper;
    private GAL_DialerSetting _setting;

    private Guid AgentId
    {
        //get { return Guid.Parse("6D690286-A449-4C09-A694-1940DF3B249F"); }
        get;
        set;
    }

    private System.Data.Entity.DbContext Loader
    {
        get { return _helper.Context; }
    }

    /// <summary>
    /// GAL settings
    /// </summary>
    public GAL_DialerSetting Settings
    {
        get
        {
            if (_setting == null)
            {
                _setting = _helper.Load<GAL_DialerSetting>();
            }
            return _setting;
        }
    }

    /// <summary>
    /// Gets the display data for particular agent.
    /// </summary>
    /// <author>MH</author>
    /// <returns></returns>
    public GAL_AgentDisplay GetGALDisplay()
    {
        var res = LoadData<GAL_AgentDisplay>("GALDisplay");
        return res;
    }

    /// <summary>
    /// Gets the Lead basic data for particular agent
    /// </summary>
    /// <author>MH</author>
    /// <returns></returns>
    public GAL_LeadBasicDisplay GetLeadBasicDisplay()
    {
        var res = LoadData<GAL_LeadBasicDisplay>("LeadBasicDisplay");
        return res;
    }

    private static object objMutex = new object();

    /// <summary>
    /// Gets the Lead to dial data for particular agent.
    /// Locks the SP execution to aloow one execution at a time
    /// uncomment the commented code for debugging and diagnosing
    /// </summary>
    /// <author>MH</author>
    /// <modified>TM 12 09 2014</modified>
    /// <returns></returns>
    public GAL_LeadToDial GetLeadToDial()
    {
        //TM [12 09 2014] Applied Lock and made the 
        GAL_LeadToDial data;

        //Logger.Logfile("Gal Dialer BEFORE ENTERING Mutex code, user ID " + AgentId.ToString());
        lock (objMutex)
        {
            //System.Threading.Thread.Sleep(500);
            //Logger.Logfile("Gal Dialer ENTERED Mutex code, user ID " + AgentId.ToString());
            data = new GAL_LeadToDial();
            data = LoadData<GAL_LeadToDial>("LeadToDial", TimeSpan.FromMinutes(1));
            //System.Threading.Thread.Sleep(2000);
        }
        //Logger.Logfile("Gal Dialer EXITTED MUTEX code, user ID " + AgentId.ToString());
        return data;
    }

    /// <summary>
    /// Gets InContact Cridendentials from users table
    /// </summary>
    /// <author>MH</author>
    /// <returns></returns>
    public InContactCridentials GetInContactCridentials()
    {
        var parm =
         new System.Data.SqlClient.SqlParameterFluent().Add("agentid", AgentId)
             .ToObjectArray();
        string query =
            "select usr_phone_system_password, usr_phone_system_username from users where usr_key =@agentid";
        var data = Loader.Database.SqlQuery<InContactCridentials>(query, parm).FirstOrDefault();
        return data;
    }

    /// <summary>
    /// Core dictionality to load data from sp
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spName"></param>
    /// <param name="timeout"></param>
    /// <author>MH</author>
    /// <returns></returns>
    public T LoadData<T>(string spName, TimeSpan? timeout = null)
    {
        if (timeout != null)
        {
            Loader.IncreaseObjectTimeout((int)timeout.Value.TotalSeconds);
        }
        var parm =
          new System.Data.SqlClient.SqlParameterFluent().Add("agentid", AgentId)
              .ToObjectArray();
        return Loader.ExecuteStoreProcedure<T>(spName, parm).FirstOrDefault();
    }

    /// <summary>
    /// method to execute
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spName"></param>
    /// <param name="timeout"></param>
    /// <author>TM</author>
    /// <returns></returns>
    public List<Guid> LoadUserIDs<T>(long AccountID, TimeSpan? timeout = null)
    {
        if (timeout != null)
        {
            Loader.IncreaseObjectTimeout((int)timeout.Value.TotalSeconds);
        }
        var parm =
          new System.Data.SqlClient.SqlParameterFluent().Add("AccountKey", AccountID)
              .ToObjectArray();

        return Loader.ExecuteStoreProcedure<Guid>("GetWebGALAgentListByAccountID", parm).ToList();
    }


    /// <summary>
    /// Performs Dial lead function.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="spName"></param>
    /// <author>MH</author>
    /// <returns></returns>
    public string DialLead(out long acctID)
    {
        string dialScript = string.Empty;

        acctID = 0;
        var leadToDial = GetLeadToDial();
        string exceptionMessage = string.Empty;
        if (leadToDial != null)
        {

            if (leadToDial.lead_l360_id.HasValue /* && AssignedLeadCheck(leadToDial.lead_l360_id.Value)*/)
            {
                if (AssignedLeadCheck(leadToDial.lead_l360_id.Value))
                {

                    acctID = leadToDial.lead_l360_id ?? 0;
                    string inContactDialCampaign = leadToDial.campaignid;
                    if (string.IsNullOrEmpty(inContactDialCampaign))
                        inContactDialCampaign = Settings.Five9DefaultCampaign;
                    if (CurrentUser.PhoneCompanyName == "inContact")
                    {
                        var cridentials = GetInContactCridentials();
                        if (cridentials != null)
                        {
                            var status = InContactCall(leadToDial.dialer_digits, inContactDialCampaign, cridentials, null);
                            if (status)
                            {
                                dialScript = GetScRedirectScript(leadToDial.dialer_digits, leadToDial.lea_cmp_id.ToString(), leadToDial.lea_status.ToString());
                            }
                        }
                    }
                    else if (CurrentUser.PhoneCompanyName == "Cisco")
                    {

                        string authCode = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(CurrentUser.Cisco_AgentId + ":" + CurrentUser.Cisco_AgentPassword));
                        funcs.CiscoAuthorization(CurrentUser.Cisco_AgentId, CurrentUser.Cisco_AgentExtension1, authCode, ref exceptionMessage);
                        if (exceptionMessage.Equals(string.Empty))
                        {
                            var status = CiscoCall(leadToDial.dialer_digits, inContactDialCampaign, null);
                            if (status)
                            {
                                dialScript = GetScRedirectScript(leadToDial.dialer_digits, leadToDial.lea_cmp_id.ToString(), leadToDial.lea_status.ToString());
                            }
                        }
                    }
                    else
                    {
                        dialScript = "http://localhost:9998/makeCall?number=" + leadToDial.dialer_digits + "&campaignId=" + inContactDialCampaign + "&checkDnc=true\",\"myW\",\"height=25,width=25,scrollbars=no,resizable=no,statusbar=no,menubar=no,toolbar=no,dependent=yes";
                    }
                }
            }
        }
        return dialScript;
    }

    /// <summary>
    /// Gets auto-redirect script for  on successful dial
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="inContactDialCampaign"></param>
    /// <param name="statusId"></param>
    /// <author>MH</author>
    /// <returns></returns>
    private string GetScRedirectScript(string phoneNumber, string inContactDialCampaign, string statusId)
    {
        string script = null;
        string para = null;
        para = string.Format("phone={0}&campaignid={1}&statusid={2}&type={3}&source={4}", phoneNumber, inContactDialCampaign, statusId, Settings.GAL_SCREEN_POP_REDIRECT_TYPE, Settings.default_source_code);

        HttpRequest Request = HttpContext.Current.Request;
        string baseUrl = Request.Url.Scheme + "://" + Request.Url.Authority + Request.ApplicationPath.TrimEnd('/') + "/";
        script = string.Format(baseUrl + "Leads/Leads.aspx?{0}", para);
        return script;
    }

    /// <summary>
    /// Checks if account is assigned to the particular individual
    /// </summary>
    /// <param name="accountId"></param>
    /// <author>MH</author>
    /// <returns></returns>
    public bool AssignedLeadCheck(long accountId)
    {
        var parm = new System.Data.SqlClient.SqlParameterFluent().Add("agentid", AgentId).Add("actid", accountId).ToObjectArray();
        const string query = "select case when act_assigned_usr =@agentid then 1 else 0 end from accounts where act_key =@actid";
        int intResult = Loader.Database.SqlQuery<int>(query, parm).FirstOrDefault();
        bool result = Convert.ToBoolean(intResult);
        return result;
    }

    /// <summary>
    /// Removed agent assignemnt from gal
    /// </summary>
    /// <author>MH</author>
    /// <param name="accountid"></param>
    public void RemoveAssignment(int accountid)
    {
        var parm = new System.Data.SqlClient.SqlParameterFluent().Add("agentid", AgentId).Add("actid", accountid).ToObjectArray();
        const string query = "DELETE FROM dbo.gal_assignments WHERE gas_act_key =@actid AND gas_usr_key =@agentid";
        var result = Loader.Database.ExecuteSqlCommand(query, parm);
    }

    /// <summary>
    /// Performs InContactCalling functionlity
    /// </summary>
    /// <param name="phoneNumber"></param>
    /// <param name="outPulseId"></param>
    /// <param name="cridentials"></param>
    /// <param name="action"></param>
    /// <author>MH:</author>
    /// <returns></returns>
    private bool InContactCall(string phoneNumber, string outPulseId, InContactCridentials cridentials, Action<string, string> action)
    {
        bool status = false;
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);

        inContactAuthorizationResponse authToken;
        JoinSessionResponse sessionResponse;

        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(outPulseId))
        {
            exceptionMessage = "Outpulse ID Not Found.";
            if (action != null) action("Ourpulse", exceptionMessage);
            return status;
        }
        else if (string.IsNullOrEmpty(cridentials.usr_phone_system_username) && string.IsNullOrEmpty(cridentials.usr_phone_system_password))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
            if (action != null) action("Credentials", exceptionMessage);
            return status;
        }
        inContactFunctions funcs = new inContactFunctions();
        authToken = funcs.inContactAuthorization(Settings.PHONE_SYSTEM_API_GRANT_TYPE, Settings.PHONE_SYSTEM_API__SCOPE, cridentials.usr_phone_system_username, cridentials.usr_phone_system_password, Settings.PHONE_SYSTEM_API_KEY, ref exceptionMessage);
        if (authToken == null)
        {
            exceptionMessage = "Unable to authenticate with Softphone.";
            if (action != null) action("Authentication", exceptionMessage);
            return status;
        }
        else
        {
            sessionResponse = funcs.inContactJoinSession(authToken, ref exceptionMessage);
            if (sessionResponse != null)
            {
                exceptionMessage = funcs.inContactDialNumber(authToken, sessionResponse, phoneNumber.Replace("-", ""), outPulseId);
                if (!string.IsNullOrEmpty(exceptionMessage))
                {
                    status = false;
                }
                else
                {
                    status = true;
                }
                return status;
            }
            else
            {
                //action(exceptionMessage, "inContact Dial");
                return status;
            }
        }
    }

    //cisco dial number
    private bool CiscoCall(string phoneNumber, string outPulseId, Action<string, string> action)
    {
        bool status = false;
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);

        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(outPulseId))
        {
            exceptionMessage = "Outpulse ID Not Found.";
            if (action != null) action("Ourpulse", exceptionMessage);
            return status;
        }
        else
        {
            string authCode = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(CurrentUser.Cisco_AgentId + ":" + CurrentUser.Cisco_AgentPassword));
            funcs.CiscoDialNumber(CurrentUser.Cisco_AgentId, authCode, phoneNumber, CurrentUser.Cisco_AgentExtension1, ref exceptionMessage);

            if (!string.IsNullOrEmpty(exceptionMessage))
            {
                status = false;
            }
            else
            {
                status = true;
            }
            return status;
        }
    }
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
            _setting = null;
            _helper.Dispose();
            // free managed resources
        }
        _isDisposed = true;
    }

    #endregion
}

/// <summary>
/// Class thats contains keys for GAL settings.
/// <author>MH</author>
/// </summary>
public class GAL_DialerSetting
{
    public string PHONE_SYSTEM_API_KEY { get; set; }

    public string PHONE_SYSTEM_API_GRANT_TYPE { get; set; }

    public string PHONE_SYSTEM_API__SCOPE { get; set; }

    public string PHONE_SYSTEM_TYPE { get; set; }

    public string default_source_code { get; set; }

    public string GAL_SCREEN_POP_REDIRECT_TYPE { get; set; }

    public int DialerDisableDialSeconds { get; set; }

    public bool IsPhoneSystemInContact
    {
        get { return PHONE_SYSTEM_TYPE == "inContact"; }
    }

    public bool IsPhoneSystemFive9
    {
        get { return PHONE_SYSTEM_TYPE == "Five9"; }
    }

    public string Five9DefaultCampaign { get; set; }
}

/// <summary>
/// Class that contains properties for user data
/// <author>TM</author>
/// </summary>
public class UserData
{
    public string agentKey { get; set; }
    public string SignalRurl { get; set; }
    public int DisableSeconds { get; set; }
    public int maxDailyLead { get; set; }
}

/// <summary>
/// Class that contains properties for GAL Dispaly data
/// <author>MH</author>
/// </summary>
public class GAL_AgentDisplay
{
    public bool? agent_call_flag { get; set; }
    public DateTime? agent_call_start { get; set; }
    public int? agent_call_start_m { get; set; }
    public int? agent_call_start_d { get; set; }
    public int? agent_call_start_y { get; set; }
    public int? agent_call_start_h { get; set; }
    public int? agent_call_start_mm { get; set; }
    public int? agent_call_start_s { get; set; }
    public string agent_call_campaign { get; set; }
    public string agent_call_type { get; set; }
    public int? campaign_call_timer { get; set; }
}

/// <summary>
/// Class that contains properties for GAL Lead basic data
/// <author>MH</author>
/// </summary>
public class GAL_LeadBasicDisplay
{
    public Guid? agent_l360_username { get; set; }
    public int? avg_max { get; set; }
    public int? total_assigned_leads { get; set; }
    public int? agent_max { get; set; }
    public int? total_available_leads { get; set; }
    public int? total_assignable_leads { get; set; }
    public DateTime? oldest_available { get; set; }
    public DateTime? newest_available { get; set; }
    public DateTime? last_refresh { get; set; }
    public bool? IsEnabled { get; set; }
    public string Reason { get; set; }
}

/// <summary>
/// Class that contains properties for GAL Leads to dail
/// <author>MH</author>
/// </summary>
public class GAL_LeadToDial
{
    public long? lead_l360_id { get; set; }
    public string lead_l360_firstname { get; set; }
    public string lead_l360_lastname { get; set; }
    public string dialer_digits { get; set; }
    public string campaignid { get; set; }
    public int? lea_status { get; set; }
    public int? lea_cmp_id { get; set; }
}

/// <summary>
/// Class that contains properties for InContact cridentials 
/// <author>Mh</author>
/// </summary>
public class InContactCridentials
{
    public string usr_phone_system_password { get; set; }

    public string usr_phone_system_username { get; set; }
}