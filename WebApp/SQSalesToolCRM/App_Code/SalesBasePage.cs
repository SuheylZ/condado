// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      C:\Projects\Live\SQ Sales Tool\SQSalesToolCRM\
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   SZ
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;

using SalesTool.Logging;

/// <summary>
/// Summary description for SalesBasePage
/// </summary>
public class SalesBasePage : SalesDataPage
{

    private SalesTool.DataAccess.Models.User _user = null;
    private SalesSecurity _security = null;
    private SalesTool.Schema.MiniUser _appUser = null;



    //wm
    #region Start Waheed

    public string GetBaseUrl(string relativePath)
    {
        return this.ResolveUrl(relativePath);
    }

    // end wm
    #endregion

    #region Methods

    protected override void OnInit(EventArgs e)
    {
        // base.OnInit(e);

        if (!User.Identity.IsAuthenticated)
        {
            //if (System.Configuration.ConfigurationManager.AppSettings[Konstants.K_DEBUG_CREDENTIALS] != null)
            //    DEBUG_Login();
            //else
            //{
            //SZ [Mar 15, 2013] Requested by teh client to save the url
            if (!Request.Url.ToString().ToLower().Contains("default"))
                RequestedURL = Request.Url.ToString();
            this.IsFirstTime = true;

            // SZ [Apr 12, 2013] performance hit but it is requried so that the problems do not occur
            Response.Redirect(Konstants.K_LOGINPAGE, true);
            //}
        }
        else
        {
            // SZ [Dec 26, 2013] Added to retrieve the current user 
            InnerRegisterScripts(this);

            // SZ [Mar 26, 2013] This has been added to route the user to the home page that 
            // has been assigned in the user administration area
            if (IsFirstTime)
            {
                string url = string.Empty;
                IsFirstTime = false;
                // SZ [Mar 15, 2013] if user requested soem page before login, now that he has logged in, take him to that page
                if (RequestedURL != string.Empty)
                {
                    url = RequestedURL;
                    RequestedURL = string.Empty;
                }
                else
                {
                    switch (CurrentUser.LoginLandingPage)
                    {
                        case 1: url = Konstants.K_HOMEPAGE; break;
                        case 2: url = Konstants.K_VIEW_LEADS_PAGE; break;
                        case 3: url = Konstants.K_PRIORITIZEDLEADSPAGE; break;
                        default: url = Konstants.K_HOMEPAGE; break;
                    }
                }
                // SZ [Mar 15, 2013] let it go without authentication. requested page chain will check for security and stuff.
                Redirect(url);
            }



            // SZ [Dec 10, 2012] it has been added to capture login event
            // as session would not contain value initially and after that
            // it contains the value until it gets expired
            if (!IsAllowed)
            {
                Redirect(Konstants.K_HOMEPAGE);
            }
            else
            {
                base.OnInit(e);
                Page_Initialize(this, e);
            }
        }


    }

    protected virtual void RegisterScript()
    {

    }
    private void InnerRegisterScripts(SalesBasePage page)
    {
        //YA[jan 06, 2013] Getting error with addition of the hidden Control. Error: The Controls collection cannot be modified because the control contains code blocks (i.e. <% ... %>).
        //This function was created so that logged In user key could be accessible universally.

        //const string Id = "_hdnUserKey56789";
        //const string scriptId = "_hdnUserKey56789__script";
        //string script = string.Format("function getUserKey(){{ return $('#{0}').val();}}", Id);
        //System.Web.UI.HtmlControls.HtmlInputHidden hdn = new System.Web.UI.HtmlControls.HtmlInputHidden
        //{
        //    ID = Id,
        //    Value = CurrentUser.Key.ToString()
        //};
        //page.Controls.AddAt(0,hdn);

        ////page.Controls.AddAt(0, new LiteralControl());
        //if(!page.ClientScript.IsClientScriptBlockRegistered(scriptId))
        //    page.ClientScript.RegisterClientScriptBlock(page.GetType(), scriptId, script, true);

        RegisterScript();
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);


        if (IsPostBack)
            Page_PostBack(this, e);
    }

    public override void Dispose()
    {
        // SZ [Dec 3, 2012] The session value for permissions is cleared since 
        // we use the latest permissions on every request
        Session.Remove(Konstants.K_PERMISSIONS);
        base.Dispose();
        _user = null;
        _security = null;
        _appUser = null;

    }

    //SZ [Apr 11, 2013] added to prevent an exception and exception stack.
    // performs faster than regular Response.Redirect as it does nto throw exception
    public void Redirect(string page)
    {
        Response.Redirect(page, false);
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }


    private void DEBUG_Login()
    {
        string[] tmp = System.Configuration.ConfigurationManager.AppSettings[Konstants.K_DEBUG_CREDENTIALS].Split(new char[] { '|' });
        if (Membership.ValidateUser(tmp[0], tmp[1]))
            FormsAuthentication.SetAuthCookie(tmp[0], true);
    }
    #endregion

    #region Properties

    private string RequestedURL
    {
        get
        {
            string sAns = string.Empty;
            if (Session[Konstants.K_REQUESTED_URL] != null)
                sAns = Session[Konstants.K_REQUESTED_URL].ToString();
            return sAns;
        }
        set
        {
            if (value == string.Empty)
                Session.Remove(Konstants.K_REQUESTED_URL);
            else
                Session[Konstants.K_REQUESTED_URL] = value;
        }
    }

    // SZ [mar 26, 2013] This property has been added to identify the first encounter after the login. 
    // The init instance gets called on every page request, to make sure that not every requested is redirected to the home page
    public bool IsFirstTime
    {
        get
        {
            bool bRet = false;
            bool.TryParse((Session[Konstants.K_FIRST_TIME] ?? "").ToString(), out bRet);
            return bRet;
        }
        set
        {
            Session[Konstants.K_FIRST_TIME] = value;
        }
    }


    public SalesSecurity AppSecurity
    {
        get
        {
            if (_security == null)
            {
                if (CurrentUser.HasPermissions)
                    _security = new SalesSecurity(CurrentUser.UserPermissions.FirstOrDefault());
                else
                    throw new System.Security.SecurityException(ErrorMessages.NOPermissions);
            }
            return _security;
        }
    }
    protected bool IsAllowed
    {
        get
        {
            string page = Request.Path.ToLower();

            if (page.Contains("managealerts.aspx"))
                return AppSecurity.CanManageAlerts;
            if (page.Contains("manageusers.aspx"))
                return AppSecurity.CanManageUsers;
            else if (page.Contains("managecampaign.aspx"))
                return AppSecurity.CanManageCampaigns;
            else if (page.Contains("manageemails.aspx"))
                return AppSecurity.CanManageEmailTemplates;
            else if (page.Contains("managegetlead.aspx"))
                return AppSecurity.CanManageGetALead;
            else if (page.Contains("outboundrouting.aspx"))
                return AppSecurity.CanManageOutboundRouting;
            else if (page.Contains("manageposts.aspx"))
                return AppSecurity.CanManagePosts;
            else if (page.Contains("manageprioritization.aspx"))
                return AppSecurity.CanManagePrioritizationRules;
            else if (page.Contains("managequicklinks.aspx"))
                return AppSecurity.CanManageQuickLinks;
            else if (page.Contains("manageretention.aspx"))
                return AppSecurity.CanManageRetention;
            else if (page.Contains("manageroles.aspx"))
                return AppSecurity.CanManageRoles;
            else if (page.Contains("manageskillgroups.aspx"))
                return AppSecurity.CanManageSkillGroups;
            else if (page.Contains("manageretentions.aspx"))
                return AppSecurity.CanManageRetention;
            else if (page.Contains("manageduplicates.aspx"))
                return AppSecurity.CanManageDuplicateRules;
            else if (page.Contains("viewduplicates.aspx"))
                return AppSecurity.CanViewDuplicates;
            else if (page.Contains("managedashboard.aspx"))
                return AppSecurity.CanManageDashboard;
            else if (page.Contains("managereassignments.aspx"))
                return AppSecurity.CanManageReassignment;
            return true;
        }
    }
    //private bool IsAuthenticatedUser{ 
    //    get{    
    //        return (User.Identity.IsAuthenticated);    
    //    }  
    //}

    public SalesTool.DataAccess.Models.User CurrentUser
    {
        get
        {
            if (_user == null)
            {
                // SZ [Apr 11, 2013] optimized version
                Guid key = Guid.Empty;
                if (Session[Konstants.K_USERID] == null)
                {
                    // This is the first time so initialize
                    var aspuser = Membership.GetUser();
                    if (aspuser == null)
                    { // SZ [may 30, 2013] this is a serious error. somebody accessing db without user logged in. immediately redirect
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        Response.Redirect(Konstants.K_LOGINPAGE);
                    }

                    key = aspuser == null ? Guid.Empty : new Guid(aspuser.ProviderUserKey.ToString());
                    Session[Konstants.K_USERID] = key.ToString();

                    // this is also after the login so audit the event
                    using (Logging log = Logging.Instance)
                        log.Write(AuditEvent.Login, "User has logged in successfully", key);
                }
                else
                    // Session contains the key, turn it into Guid
                    key = new Guid(Session[Konstants.K_USERID].ToString());
                _user = Engine.UserActions.Get(key);
               
            }
            return _user;

        }
    }

    //private SalesTool.DataAccess.Models.User activeUser
    //{
    //    get
    //    {
    //        const string kCurrentUser = "appCurrentUser";
    //        var app = HttpContext.Current.IfNotNull(p => p.Session);
    //        if (app != null)
    //        {
    //            var usr = app[kCurrentUser] as SalesTool.DataAccess.Models.User;
    //            if (usr != null)
    //                return usr;
    //        }
    //        return null;
    //    }
    //    set
    //    {
    //        const string kCurrentUser = "appCurrentUser";
    //        var app = HttpContext.Current.IfNotNull(p => p.Session);
    //        if (app != null)
    //        {
    //            app[kCurrentUser] = value;
    //        }
    //    }
    //}
    //SZ [Jan 15, 2013] Added the lightweight and faster version of getting user
    public SalesTool.Schema.MiniUser CurrentUser2
    {
        get
        {
            if (_appUser == null)
            {
                Guid key = Guid.Empty;
                if (Session[Konstants.K_USERID] == null)
                {
                    Guid.TryParse(Membership.GetUser().ProviderUserKey.ToString(), out key);
                    Session[Konstants.K_USERID] = key.ToString();
                }
                Guid.TryParse(Session[Konstants.K_USERID].ToString(), out key);
                _appUser = SalesTool.Schema.MiniUser.Get(key);
            }
            return _appUser;
        }
    }

    #endregion

    #region Events
    protected virtual void Page_Initialize(object sender, EventArgs args) { }
    protected virtual void Page_PostBack(object sender, EventArgs args) { }
    #endregion
}

