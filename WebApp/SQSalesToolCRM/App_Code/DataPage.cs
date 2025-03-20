// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      C:\Projects\Live\SQ Sales Tool\SQSalesToolCRM\
// 
// Description: The datapage class provides the following functionality
//              1. Data Access through Engine property
//              2. ViewState storage in the session so pages are smaller in size with no view state                
//              3. Access to Script manager through ScriptManager property
//              4. Path to temporary folder.
//          
//              You should derive from this class if you require database access but do not want the security 
//              or any other features in the salesbase page
//              
// Created By:   SZ
// Created On:   01/07/2012
// 
// --------------------------------------------------------------------------
// 

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;

public class SalesDataPage : System.Web.UI.Page
{
    private SalesTool.DataAccess.DBEngine _engine = null;

    const string ViewStateFieldName = "__VIEWSTATEKEY";
    const string RecentViewStateQueue = "RecentViewStateQueue";
    const int RecentViewStateQueueMaxLength = int.MaxValue;
    
    //protected override object LoadPageStateFromPersistenceMedium()
    //{
    //    // The cache key for this viewstate is stored where the viewstate normally is, so grab it
    //    string viewStateKey = Request.Form[ViewStateFieldName] as string;
    //    if (viewStateKey == null) return null;

    //    // Grab the viewstate data from the cache using the key to look it up
    //    string viewStateData = Cache[viewStateKey] as string;
    //    if (viewStateData == null) return null;

    //    // Deserialise it
    //    return new LosFormatter().Deserialize(viewStateData);
    //}
    //protected override void SavePageStateToPersistenceMedium(object viewState)
    //{
    //    // Serialise the viewstate information
    //    StringBuilder _viewState = new StringBuilder();
    //    StringWriter _writer = new StringWriter(_viewState);
    //    new LosFormatter().Serialize(_writer, viewState);

    //    // Give this viewstate a random key
    //    string viewStateKey = Guid.NewGuid().ToString();

    //    // Store the viewstate in the cache
    //    Cache.Add(viewStateKey, _viewState.ToString(), null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(Session.Timeout), System.Web.Caching.CacheItemPriority.Normal, null);

    //    // Store the viewstate's cache key in the viewstate hidden field, so on postback we can grab it from the cache
    //    ClientScript.RegisterHiddenField(ViewStateFieldName, viewStateKey);

    //    // Some tidying up: keep track of the X most recent viewstates for this user, and remove old ones
    //    var recent = Session[RecentViewStateQueue] as Queue<string>;
    //    if (recent == null) Session[RecentViewStateQueue] = recent = new Queue<string>();
    //    recent.Enqueue(viewStateKey); // Add this new one so it'll get removed later
    //    while (recent.Count > RecentViewStateQueueMaxLength) // If we've got lots in the queue, remove the old ones
    //        Cache.Remove(recent.Dequeue());
    //}

    //protected override PageStatePersister PageStatePersister
    //{
    //    get
    //    {
    //        return new SessionPageStatePersister(this);
    //    }
    //}

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);

        //Response.ExpiresAbsolute = DateTime.Now.AddDays(-1d);
        //Response.Expires = -1500;
        //Response.CacheControl = "no-cache";
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);
        //Page.Title = Konstants.K_APPLICATION_TITLE;
        
        // Attiq - April-10-2014.
        SetPageTitle();

        //Response.AppendHeader("Cache-Control", "no-cache, no-store, must-revalidate"); // HTTP 1.1.
        //Response.AppendHeader("Pragma", "no-cache"); // HTTP 1.0.
        //Response.AppendHeader("Expires", "0"); // Proxies.
    }
    
    // Attiq - April-10-2014.
    // Function to set Page Titles throughout the application.
    protected void SetPageTitle()
    {
        StringBuilder pagetitle = null;
        
        if (Engine.ApplicationSettings.IsSenior)
        {
            pagetitle = new StringBuilder(Konstants.K_APPLICATION_TITLE_SQS);
        }
        else if (Engine.ApplicationSettings.IsAutoHome)
        {
            pagetitle = new StringBuilder(Konstants.K_APPLICATION_TITLE_SQAH);
        }
        else if (Engine.ApplicationSettings.IsTermLife)
        {
            pagetitle = new StringBuilder(Konstants.K_APPLICATION_TITLE_SQL);
        }
        
        string path = GetCurrentPageName().ToLower();

        if (path.Contains(Konstants.K_DEFAULT_PAGE))
            pagetitle = pagetitle.Append(Konstants.K_DEFAULT_TITLE);
        else if (path.Contains(Konstants.K_NORMALVIEW_PAGE.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_NORMALVIEW_TITLE);
        //-------
        else if (path.Contains(Konstants.K_MANAGESKILL.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGESKILL_TITLE);

        else if (path.Contains(Konstants.K_MANAGECAMPAIGN.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGECAMPAIGN_TITLE);

        else if (path.Contains(Konstants.K_MANAGEACTIONS.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEACTIONS_TITLE);

        else if (path.Contains(Konstants.K_MANAGEEMAILS.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEEMAILS_TITLE);

        else if (path.Contains(Konstants.K_MANAGEPOSTS.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEPOSTS_TITLE);

        else if (path.Contains(Konstants.K_MANAGEQUICKLINKS.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEQUICKLINKS_TITLE);

        else if (path.Contains(Konstants.K_MANAGEAGENT.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEAGENT_TITLE);

        else if (path.Contains(Konstants.K_MANAGEALERTS.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEALERTS_TITLE);

        else if (path.Contains(Konstants.K_MANAGEPRIORITIZATION.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEPRIORITIZATION_TITLE);

        else if (path.Contains(Konstants.K_MANAGEREASSIGNMENTS.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEREASSIGNMENTS_TITLE);

        else if (path.Contains(Konstants.K_MANAGEDUPLICATES.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEDUPLICATES_TITLE);

        else if (path.Contains(Konstants.K_MANAGEDUPLICATES2.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEDUPLICATES2_TITLE);

        else if (path.Contains(Konstants.K_MANAGEDASHBOARD.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEDASHBOARD_TITLE);
            //----------
        else if (path.Contains(Konstants.K_PRIORITIZEDVIEW_PAGE))
            pagetitle = pagetitle.Append(Konstants.K_PRIORITIZEDVIEW_TITLE);
        else if (path.Contains(Konstants.K_CALENDAR_PAGE.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_CALENDAR_TITLE);
        else if (path.Contains(Konstants.K_CUSTOMREPORTS_PAGE.ToLower()))
            pagetitle = pagetitle.Append(Konstants.K_CUSTOMREPORTS_TITLE);
        else if (path.Contains(Konstants.K_REPORTDISPLAY_PAGE))
        {
            //string reportid = Request.QueryString["reportid"] == "1" ? "Accounts" : Request.QueryString["reportid"] == "2" ? ;
            pagetitle = pagetitle.Append(Konstants.K_REPORTDISPLAY_TITLE);
        }
        else if (path.Contains(Konstants.K_NEWACCOUNT_PAGE))
        {
            string accountid = Request.QueryString["accountid"];

            if (accountid == "-1")
                pagetitle = pagetitle.Append(Konstants.K_NEWACCOUNT_TITLE);
            else
            {
                pagetitle = pagetitle.Append(Konstants.K_EDITACCOUNT_TITLE + accountid);
            }
        }
        else if (path.Contains(Konstants.K_MANAGEUSERS_PAGE))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEUSERS_TITLE);
        else if (path.Contains(Konstants.K_OUTBOUNDROUTING_PAGE))
            pagetitle = pagetitle.Append(Konstants.K_OUTBOUNDROUTING_TITLE);
        else if (path.Contains(Konstants.K_ADMINMYREOPRTS_PAGE))
            pagetitle = pagetitle.Append(Konstants.K_ADMINMYREOPRTS_TITLE);
        else if (path.Contains(Konstants.K_MANAGEROLES_PAGE))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEROLES_TITLE);

        else if (path.Contains(Konstants.K_MANAGEROLES_PAGE))
            pagetitle = pagetitle.Append(Konstants.K_MANAGEROLES_TITLE);
        else
        {
            pagetitle = pagetitle.Remove(0, pagetitle.Length); // new StringBuilder("SQ Sales Tool");
            pagetitle = pagetitle.Append("SQ Sales Tool");
        }

        Page.Title = pagetitle.ToString();
    }

    // Attiq - April-10-2014.
    public string GetCurrentPageName()
    {
        string virtualPath = Page.MapPath(Page.AppRelativeVirtualPath);
        string[] arrPath = virtualPath.Split('\\');
        return arrPath[arrPath.Count() - 1];
    }

    protected ScriptManager CurrentScriptManager { get { return (FindControl("ajaxManager") as ScriptManager); } }

    public override void Dispose()
    {
        if (_engine != null)
        {
            if (client != null)
            {
                client.Dispose();
                client = null;
            }
            _engine.Dispose();
            _engine = null;
           
        }
        base.Dispose();
    }

    private ArcApiClient client;
    public SalesTool.DataAccess.DBEngine Engine
    {
        get
        {
            if (_engine == null)
            {
                _engine = new SalesTool.DataAccess.DBEngine();
                _engine.SetSettings(HttpContext.Current.Application.ApplicationSettings());
                //HM:31 March 2014
                _engine.HookArcGlobalListeners = _engine.ApplicationSettings.IsTermLife;
                //_engine.Init(ApplicationSettings.ADOConnectionString);
                _engine.Init(ApplicationSettings.ADOConnectionString);
                //if (ApplicationSettings.IsTermLife)
                if (_engine.ApplicationSettings.IsTermLife&& _engine.ApplicationSettings.IsEnabledRealTimeArcApiCalls)
                {
                    client = new ArcApiClient(_engine);
                }
                //_engine.Init(ApplicationSettings.AdminEF,ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
            }
            return _engine;
        }
    }
    //YA [Jan 22, 2013] Changed the property to public to use it in usercontrols.
    public string TemporaryFolder
    {
        get
        {
            if (Application[Konstants.K_TEMPORARY_FOLDER] == null)
                Application[Konstants.K_TEMPORARY_FOLDER] = HttpRuntime.CodegenDir;
            return Application[Konstants.K_TEMPORARY_FOLDER].ToString();
        }
    }
    
    
    //internal protected class CookieHelper
    //{
    //    HttpContext _ctx = null;
    //    internal CookieHelper(HttpContext ctx)
    //    {
    //        _ctx = ctx;
    //    }

    //    // SZ [Nov 29, 2012] added to pass value in response.redirect
    //    // once teh value is retrieved, it is removed
    //    internal string this[string name]
    //    {
    //        get
    //        {
    //            HttpCookie cookie = _ctx.Request.Cookies[name];
    //            string sAns = string.Empty;
    //            if (cookie != null)
    //            {
    //                sAns = cookie.Value;
    //                //_ctx.Response.Cookies.Remove(cookie.Name);
    //                //send a cookie with an expiration date in the past so the browser deletes the other one
    //                //you don't want the cookie appearing multiple times on your server
    //                //cookie.Value = null;
    //                //cookie.Expires = DateTime.Now.AddDays(-1);
    //                //Response.Cookies.Add(cookie);
    //            }
    //            return sAns;
    //        }
    //        set
    //        {
    //            HttpCookie cookie = new HttpCookie(name, value);
    //            _ctx.Response.Cookies.Add(cookie);
    //        }
    //    }
    //}
    //protected CookieHelper Cookie = new CookieHelper(HttpContext.Current);


    public IEnumerable<SalesTool.DataAccess.Models.State> USStates
    {
        get
        {
            const string K_STATES = "USA_STATES";
            if (Cache[K_STATES] == null)
                Cache[K_STATES] = Engine.Constants.States.ToList();
            return Cache[K_STATES] as IEnumerable<SalesTool.DataAccess.Models.State>;
        }
    }

    //SZ [May 9, 2103] Cache helper function
    public void Add2Cache(string key, object value)
    {
        if (Cache[key] != null)
            Cache.Remove(key);

        Cache.Add(key, value, null,
            Konstants.K_Caching_Duration,
            System.Web.Caching.Cache.NoSlidingExpiration,
            System.Web.Caching.CacheItemPriority.Normal,
            null);
    }

    public string PageName { 
    get { return this.GetPageName(); }
    }
}
  