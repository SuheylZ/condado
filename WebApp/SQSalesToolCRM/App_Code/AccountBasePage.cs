using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using System.Net;
using System.Text;
using SalesTool.DataAccess.Models;



/// <summary>
/// SZ [mar 26, 2013] This class the the base class for lead related pages. 
/// it is made for the code cleanup in the BasePageClass. In future this class can be used for 
/// further funct5ionality in teh leads page
/// </summary>
public class AccountBasePage :
    SalesBasePage, IIndividual, ITCPAProvider
{

    //SZ [May 9, 2103] Cache helper function
    //protected void Add2Cache(string name, object value)
    //{
    //    if (Cache[name] != null)
    //        Cache.Remove(name);
    //    Cache.Add(name, value, null, Konstants.K_Caching_Duration, TimeSpan.Zero, System.Web.Caching.CacheItemPriority.Normal, null);
    //}

    List<IIndividualNotification> _clients = new List<IIndividualNotification>();

    public SalesTool.DataAccess.Models.Account CurrentAccount
    {
        get
        {
            SalesTool.DataAccess.Models.Account A = null;
            if (Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] != null)
                A = Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] as SalesTool.DataAccess.Models.Account;
            else
            {
                A = Engine.AccountActions.Get(AccountID);
                Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] = A;
            }
            return A;
        }
        set
        {
            Session[CurrentUser.Key.ToString() + Session.SessionID.ToString()] = value;
        }
    }

    List<ViewIndividuals> indvs = new List<ViewIndividuals>();
    public IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals> Individuals
    {
        get
        {
            long? parentId = Request.ReadQueryStringAs<long?>(Konstants.K_PARENT_ACCOUNT_ID);
            if (parentId.HasValue)
            {
                if (indvs.Count == 0)
                    indvs = Engine.IndividualsActions.GetByAccountID(parentId.Value, (this as SalesBasePage).CurrentUser.Key).ToList();
                return indvs;
            }
            else
            {
                string key = AccountCacheKey;
                if (Cache[key] == null)
                    UpdateIndividuals(false);
                //return Cache[key] as IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals>;
                //MH: 15 May, My thought somehow Cache[key] is still returning null that raise 
                //System.Linq.Enumerable.where(IEnumerable'1 source, Func'2 predicate) Value cannot be null, paramerter name: Source exception
                return (Cache[key] as IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals>) ?? Enumerable.Empty<ViewIndividuals>(); ;
            }
        }
    }

    string AccountCacheKey { get { return string.Format("{0}_{1}", AccountID.ToString(), Session.SessionID); } }



    public void Notify(IIndividualNotification client)
    {
        if (client != null)
            _clients.Add(client);
    }
    public void UpdateIndividuals(bool bFireEvent = true)
    {
        string key = AccountCacheKey;

        if (Cache[key] != null)
            Cache.Remove(key);

        IEnumerable<SalesTool.DataAccess.Models.ViewIndividuals> data = Engine.IndividualsActions.GetByAccountID(AccountID, (this as SalesBasePage).CurrentUser.Key)/*.OrderBy(x=>x.FirstName)*/.ToList();
        Cache.Add(key, data, null,
            Konstants.K_Caching_Duration,
            System.Web.Caching.Cache.NoSlidingExpiration,
            System.Web.Caching.CacheItemPriority.Normal,
            null);

        if (_clients.Count > 0 && bFireEvent)
            foreach (var C in _clients)
                C.IndividualChanged(this as IIndividual);
    }


    protected void AccountLog(long accountId, string message, int ruleId = 0, string contactid = "")
    {
        if (ruleId < 1)
            Engine.AccountHistory.Log(accountId, message, CurrentUser.Key, 0, contactid);
        else
            Engine.AccountHistory.Log(accountId, message, CurrentUser.Key, ruleId, contactid);

    }

    protected void AccountLogAssignment(long accountId,string prefix, string message, int ruleId = 0, string contactid = "")
    {
        if (ruleId < 1)
            Engine.AccountHistory.LogAssignment(accountId,prefix, message, CurrentUser.Key, 0, contactid);
        else
            Engine.AccountHistory.LogAssignment(accountId,prefix, message, CurrentUser.Key, ruleId, contactid);

    }

    protected long AccountID
    {
        get
        {
            long accountId = default(long);
            long.TryParse((Session[Konstants.K_ACCOUNT_ID] ?? "").ToString(), out accountId);
            return accountId;
        }
        set
        {
            Session[Konstants.K_ACCOUNT_ID] = value;
            long accountParentId = default(long);
            if (value > 1)
            {
                CurrentAccount = Engine.AccountActions.Get(value);
                AccountParentID = CurrentAccount.AccountParent.HasValue ? CurrentAccount.AccountParent.Value : accountParentId;
            }
            else
            {
                CurrentAccount = null;
                AccountParentID = accountParentId;
            }
            UpdateIndividuals(false);
        }
    }
    protected long AccountParentID
    {
        get
        {
            long accountParentId = default(long);
            long.TryParse((Session[Konstants.K_PARENT_ACCOUNT_ID] ?? "").ToString(), out accountParentId);
            return accountParentId;
        }
        set
        {
            Session[Konstants.K_PARENT_ACCOUNT_ID] = value;
        }
    }
    protected bool HasParentAccountID
    {
        get
        {
            return Request.ReadQueryStringAs<long?>(Konstants.K_PARENT_ACCOUNT_ID).HasValue;
        }
    }

    protected bool IsNewAccount
    {
        get
        {
            return AccountID < 1;
        }
    }

    protected override void RegisterScript()
    {
        const string scriptId = "_acceref456789__script";
        string script = string.Format(
           @"
            function writeAccountLog(msg) {{
                var params = {{id: {1}, userId: ""{2}"", message: msg}};
                try 
                {{
                    $.post(""{0}"", params, function (arg) {{ }});
                }}
                catch (ex) {{
                    //alert(ex);
                }}
            }}",
           Request.Url.ToString().Replace(Request.RawUrl.ToString(), "") + "/Services/SelectCare.asmx/WriteAccountLog",
           AccountID.ToString(),
           CurrentUser.Key.ToString());


        if (!Page.ClientScript.IsClientScriptBlockRegistered(scriptId))
            Page.ClientScript.RegisterClientScriptBlock(this.GetType(), scriptId, script, true);

    }

    const string K_TCPA_DEBUG_HEADER = "DEBUG: (to turn debugging off, see the phoneValidator section in web.config)\n";
    const string K_TCPA_TEXTBOX = "__Alert_Invoker_23423__";
    const string K_TCPA_INVOKER = "__TCPA_Invoker_78346dgsaasdf__";

    Dictionary<int, ITCPAClient> _clientsTCPA = new Dictionary<int, ITCPAClient>();

    int TCPAActiveClient
    {
        get
        {

            int i = 0;
            int.TryParse((Session[K_TCPA_INVOKER] ?? "").ToString(), out i);
            return i;
        }
        set { Session[K_TCPA_INVOKER] = value.ToString(); }
    }

    SalesTool.Web.Configuration.PhoneValidatorSection PhoneValidationSettings
    {
        get
        {
            return System.Configuration.ConfigurationManager.GetSection(
                SalesTool.Web.Configuration.PhoneValidatorSection.SectionName
                ) as SalesTool.Web.Configuration.PhoneValidatorSection;
        }
    }

    WebRequest CreateRequest(string phone)
    {
        WebRequest Ans = System.Net.WebRequest.Create(PhoneValidationSettings.Uri);
        Ans.Method = "POST";
        Ans.ContentType = "text/xml";

        string path = Server.MapPath("~//App_Data//PhoneTypeRequest.xml");
        using (System.IO.StreamReader x = new System.IO.StreamReader(path))
        {
            StringBuilder sb = new StringBuilder(256);
            sb.Append(x.ReadToEnd());
            x.Close();
            sb.Replace("USERID", PhoneValidationSettings.User);
            sb.Replace("PASSWORD", PhoneValidationSettings.Password);
            //sb.Replace("TYPE", ApplicationSettings.IsAutoHome ? "AutoHome" : "Senior");
            sb.Replace("TYPE", Engine.ApplicationSettings.IsAutoHome ? "AutoHome" : "Senior");
            sb.Replace("PHONE", phone);
            sb.Replace("TIMEOUT", PhoneValidationSettings.Timeout.ToString());
            sb.Replace("PROTOCOL", PhoneValidationSettings.Protocol.ToString());

            if (PhoneValidationSettings.IsDebugged)
            {
                String str = Server.HtmlEncode(sb.ToString());
                str = str.Replace(PhoneValidationSettings.Password, "********");
                AccountLog(AccountID, string.Format("{0}Next Action: Request Created, Writting to request stream\n uri {1}\n Type={2}\n Request:\n{3}",
                    K_TCPA_DEBUG_HEADER,
                    PhoneValidationSettings.Uri,
                    //ApplicationSettings.IsAutoHome ? "AutoHome" : "Senior",
                    Engine.ApplicationSettings.IsAutoHome ? "AutoHome" : "Senior",
                   str));
            }
            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(Ans.GetRequestStream()))
            {
                writer.WriteLine(sb.ToString());
                writer.Close();
            }
        }

        return Ans;
    }
    PhoneType ParseResponse(string response)
    {
        PhoneType Ans = PhoneType.Unknown;

        try
        {
            if (PhoneValidationSettings.IsDebugged)
                AccountLog(AccountID, string.Format("{0}Next Action: Response recieved, starting parsing the response {1}", K_TCPA_DEBUG_HEADER, Server.HtmlEncode(response)));

            //<error>
            //    <code>perm</code>
            //    <message>source ip address (63.141.235.223) disallowed</message>
            //</error>

            //<root><transaction><output><message><input>data</input><code>AP04</code><message>Phoneiscellularormobile</message>
            //              </message></output><status>
            //                <result>data</result><count>5</count><uuid>03F4A424-90FD-4C2C-91CE-10FFAAFE7057</uuid><latency>1.125123</latency>
            //            </status></transaction></root>

            var doc = XElement.Parse(response);
            string name = doc.Name.ToString().ToLower().Trim();
            if (name == "error")
            {
                //SZ [Sep 29, 2013] Error xml recieved, log error irrespective of loging anabled or not
                var Error = new
                {
                    Code = (string)doc.Element("code").Value.ToString(),
                    Message = (string)doc.Element("message").Value.ToString()
                };
                AccountLog(AccountID, string.Format("{0}Error recieved from the remote server:\n[{1}] : {2}", "PHONE VALIDATION FAILED: ", Error.Code, Error.Message));
            }
            else if (name == "root")
            {
                var res = new
                {
                    Input = doc.Element("transaction").HasElements ?
                        (doc.Element("transaction").Element("output").HasElements ? doc.Element("transaction").Element("output").Element("message").Element("input").Value : "")
                        : "",
                    Code = doc.Element("transaction").HasElements ?
                        (doc.Element("transaction").Element("output").HasElements ? doc.Element("transaction").Element("output").Element("message").Element("code").Value : "")
                        : "",
                    Message = doc.Element("transaction").HasElements ?
                        (doc.Element("transaction").Element("output").HasElements ? doc.Element("transaction").Element("output").Element("message").Element("message").Value : "")
                        : "",
                    Result = doc.Element("transaction").HasElements ?
                        (doc.Element("transaction").Element("status").HasElements ? doc.Element("transaction").Element("status").Element("result").Value : "")
                        : "",
                    Count = Convert.ToInt32(doc.Element("transaction").HasElements ?
                        (doc.Element("transaction").Element("status").HasElements ? doc.Element("transaction").Element("status").Element("count").Value : "0")
                        : "0"),
                    UUID = new Guid(doc.Element("transaction").HasElements ?
                        (doc.Element("transaction").Element("status").HasElements ? doc.Element("transaction").Element("status").Element("uuid").Value : Guid.Empty.ToString())
                        : Guid.Empty.ToString()),
                    Latency = Convert.ToSingle(doc.Element("transaction").HasElements ?
                        (doc.Element("transaction").Element("status").HasElements ? doc.Element("transaction").Element("status").Element("latency").Value : "0.0")
                        : "0.0")
                };
                Ans = string.Compare(res.Code, "AP04", true) == 0 ? PhoneType.Cell :
                    string.Compare(res.Code, "AP09", true) == 0 ? PhoneType.Landline :
                    string.Compare(res.Code, "FP01", true) == 0 ? PhoneType.Unknown : PhoneType.Unknown;
            }
            else
            {
                if (PhoneValidationSettings.IsDebugged)
                    AccountLog(AccountID, string.Format("{0}Unknown response recieved. Failed to parse:\n{1}", K_TCPA_DEBUG_HEADER, response));
            }
        }
        catch (Exception ex)
        {
            if (PhoneValidationSettings.IsDebugged)
                AccountLog(AccountID, string.Format("{0} Error Occured:\n{1}", K_TCPA_DEBUG_HEADER, ex.Message));
        }

        if (PhoneValidationSettings.IsDebugged)
            AccountLog(AccountID, string.Format("{0} Determined phone as :\n{1}", K_TCPA_DEBUG_HEADER, Ans.ToString()));


        return Ans;
    }
    bool ActionAllowed(PhoneActionOn action, PhoneType type)
    {
        // PhoneActionOn->  		0  1  2  3  4  5  6
        // PhoneType   
        // 0						1  0  0  0  0  1  1                
        // 1						0  1  0  0  1  1  0					
        // 2                        0  0  1  0  1  0  1
        // 3                        0  0  0  1  0  0  0
        bool[,] truthTable = new bool[4, 7] {
            {true, false, false, false, false, true, true},
            {false, true, false, false, true, true, false},
            {false, false, true, false, true, false, true},
            {false, false, false, true, false, false, false}
        };

        return truthTable[(int)type, (int)action];
    }

    //SZ [Sep21, 2013] This is the place where service would be called.
    PhoneType Proxy_PhoneCheck(long phone)
    {
        PhoneType Ans = PhoneType.Unknown;
        try
        {
            // Call the service here
            System.Net.WebRequest req = CreateRequest(phone.ToString());

            if (PhoneValidationSettings.IsDebugged)
                AccountLog(AccountID, string.Format("{0}Next Action: Sending request to validate {1} ... ",
                    K_TCPA_DEBUG_HEADER,
                    phone.ToString()));


            if (PhoneValidationSettings.IsDebugged)
                AccountLog(AccountID, string.Format("{0}Next Action: Recieve the response for {1}",
                    K_TCPA_DEBUG_HEADER,
                    phone.ToString()));

            //ServicePointManager.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => { return true; };

            string response = string.Empty;
            System.Net.WebResponse rsp = req.GetResponse();
            using (System.IO.StreamReader reader = new System.IO.StreamReader(rsp.GetResponseStream()))
            {
                response = reader.ReadToEnd();
                reader.Close();
            }
            Ans = ParseResponse(response);
        }
        catch (Exception ex)
        {
            AccountLog(AccountID, string.Format("{0}Error: problem when verifying phone {1}.Exception:  {2}", K_TCPA_DEBUG_HEADER, phone.ToString(), ex.ToString()));
        }
        return Ans;
    }


    protected void DefaultAlertBoxHandler(TCPAConsentType choice)
    {
        try
        {
            // HasParentId is used to prevent quick save as saving is being done in popup
            if (IsNewAccount && !HasParentAccountID) // Sz [Oct 4, 2013] if the account is new, create it now.
                QuickAccountSave();
            if (!HasParentAccountID)
                Engine.AccountActions.SetConsent(AccountID, choice, CurrentUser.FullName);
            ITCPAClient activeClient = _clientsTCPA[TCPAActiveClient];
            if (activeClient != null)
            {
                TCPAActiveClient = 0;
                activeClient.ProcessConsent((Session[K_TCPA_TEXTBOX] ?? "").ToString(), choice);
            }
        }
        catch (Exception ex)
        {
            AccountLog(AccountID, ex.Message);
        }
    }

    public bool IsTCPAEnabled { get { return !(string.IsNullOrWhiteSpace(PhoneValidationSettings.Uri) && string.IsNullOrWhiteSpace(PhoneValidationSettings.User)); } }
    public int Register(ITCPAClient obj)
    {
        int key = 0;
        if (IsTCPAEnabled)
        {
            key = _clientsTCPA.Count + 1;
            _clientsTCPA.Add(key, obj);
        }
        return key;
    }
    public virtual void InvokeTCPA(object sender, int clientId)
    {
        string controlId = (sender as WebControl).ID;
        string text = (sender as ITextControl).Text;
        long phone = Helper.SafeConvert<long>(text);

        if (IsTCPAEnabled && phone > 0) // SZ [Sep 21, 2013] should not execute for empty numbers
        {
            TCPAActiveClient = clientId; // Store it for future reference
            Session[K_TCPA_TEXTBOX] = controlId; // Sz [Oct 3, 2014] Save the ControlId for using in the Alert Response
            TCPAConsentType consent = Engine.AccountActions.GetConsent(AccountID);
            if (consent != TCPAConsentType.Yes)
            {
                PhoneType type = Proxy_PhoneCheck(phone); // Call the service to check if the phone number is cell number
                if (ActionAllowed(PhoneValidationSettings.ActionOn, type)) // SZ [Oct 3, 2013] Alert Display on on any phone except landline
                    ShowAlertBox();
            }
        }
        if (!IsNewAccount)
            PendingSave();
    }
    /// <summary>
    /// Reads url query string as LeadsPageQueryModel object
    /// </summary>
    /// <author>MH:29 April 2014</author>
    public SalesTool.LeadsPageQueryModel LeadsQueryModel { get { return Request.ReadQueryStringAs<SalesTool.LeadsPageQueryModel>(); } }

    /// <summary>
    /// Reads url query string as ScreenPopQueryModel object
    /// </summary>
    /// <author>MH:29 April 2014</author>
    public SalesTool.ScreenPopQueryModel ScreenPopQueryModel { get { return Request.ReadQueryStringAs<SalesTool.ScreenPopQueryModel>(); } }

    public virtual void QuickAccountSave() { }
    public virtual void ShowAlertBox() { }
    public virtual void PendingSave() { }
    public override void Dispose()
    {
        base.Dispose();
        _clients = null;
        _clientsTCPA = null;
        indvs = null;
    }
}

namespace SalesTool
{
    public class LeadsPageQueryModel
    {
        [SalesTool.Web.UrlKeyValue(Konstants.K_ACCOUNT_ID, IsIgnoreCase = true)]
        public long? AccountId { get; set; }

        [SalesTool.Web.UrlKeyValue("campaignid", IsIgnoreCase = false)]
        public long? CampaignKey { get; set; }

        [SalesTool.Web.UrlKeyValue(Konstants.K_PARENT_ACCOUNT_ID, IsIgnoreCase = true)]
        public long? ParentAccountId { get; set; }

        [SalesTool.Web.UrlKeyValue("isurlprocessed", IsIgnoreCase = true)]
        public bool? IsUrlProcessed { get; set; }

        [SalesTool.Web.UrlKeyValue("avoidreassignment", IsIgnoreCase = true)]
        public bool? AvoidReAssignment { get; set; }

        [SalesTool.Web.UrlKeyValue("roleid", IsIgnoreCase = true)]
        public long? RoleId { get; set; }

        [SalesTool.Web.UrlKeyValue("IsParentPopupClose", IsIgnoreCase = true)]
        public bool? IsParentPopupClose { get; set; }
    }

    public class ScreenPopQueryModel
    {
        [SalesTool.Web.UrlKeyValue(Konstants.K_ACCOUNT_ID, IsIgnoreCase = true)]
        public long? AccountId { get; set; }

        [SalesTool.Web.UrlKeyValue("campaignid", IsIgnoreCase = false)]
        public long? CampaignKey { get; set; }

        [SalesTool.Web.UrlKeyValue(Konstants.K_PARENT_ACCOUNT_ID, IsIgnoreCase = true)]
        public long? ParentAccountId { get; set; }

        [SalesTool.Web.UrlKeyValue("phone", IsIgnoreCase = true)]
        public string Phone { get; set; }

        [SalesTool.Web.UrlKeyValue("statusid", IsIgnoreCase = true)]
        public long? StatusId { get; set; }

        [SalesTool.Web.UrlKeyValue("type", IsIgnoreCase = true)]
        public string Type { get; set; }
    }

}
