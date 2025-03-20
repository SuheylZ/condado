using System;
using System.Linq;
using System.Net.Mime;
using System.Web;
using System.Web.SessionState;
using System.Web.Security;
using System.Security.Principal;
using System.Web.Optimization;
using SalesTool.DataAccess;

public class Global : HttpApplication
{
    public void Application_Start(object sender, EventArgs e)
    {
        var bundles = BundleTable.Bundles;
        //bundles.UseCdn = true;   //enable CDN support

        //var mainViewModel = "~/PhoneBars/BarInContact/ViewModels/mainViewModel.js";
        //var agentViewModel = "~/PhoneBars/BarInContact/ViewModels/agentViewModel.js";
        //var callsViewModel = "~/PhoneBars/BarInContact/ViewModels/callsViewModel.js";
        //var chatViewModel = "~/PhoneBars/BarInContact/ViewModels/chatViewModel.js";
        //var addressViewModel = "~/PhoneBars/BarInContact/ViewModels/addressViewModel.js";
        //var historyViewModel = "~/PhoneBars/BarInContact/ViewModels/historyViewModel.js";
        //var logViewModel = "~/PhoneBars/BarInContact/ViewModels/logViewModel.js";
        //var connectViewModel = "~/PhoneBars/BarInContact/ViewModels/connectViewModel.js";
        //var hotKeyViewModel = "~/PhoneBars/BarInContact/ViewModels/hotKeyViewModel.js";
        //var signalRViewModel = "~/PhoneBars/GAL/signalRViewModel.js";
        //var acdSignalRViewModel ="~/PhoneBars/GAL/ACDSignalRViewModel.js";
        //var icAgentAPI = "~/PhoneBars/BarInContact/Scripts/icFramework/icAgentAPI.js";
        //var galAgentAPI = "~/PhoneBars/GAL/galAgentAPI.js";
        var condadoReference = "~/PhoneBars/BarInContact/Scripts/condadoReference.js";
        var cookieJS = "~/Scripts/jquery.cookie.js";
        var selectableItemHelper = "~/PhoneBars/BarInContact/ViewModels/Helpers/selectableItemHelper.js";
        var browserHelper = "~/PhoneBars/BarInContact/ViewModels/Helpers/browserHelper.js";
        //var ciscoHelper = "~/PhoneBars/BarCisco/ViewModels/csMainViewModel.js";
        //var ciscoConnect = "~/PhoneBars/BarCisco/ViewModels/csConnectViewModel.js";
        //var queuePersonalViewModel = "~/PhoneBars/GAL/queuePersonalViewModel.js";
        //var queueACDViewModel = "~/PhoneBars/GAL/queueAcdViewModel.js";
        //var displayPhoneBar = "~/PhoneBars/Scripts/displayPhoneBar.js";
        bundles.Add(new ScriptBundle("~/bundles/viewModels")
        //.Include(icAgentAPI)
        
        .Include(cookieJS).Include(condadoReference)
        //.Include(galAgentAPI).Include(mainViewModel).Include(agentViewModel)
        //.Include(callsViewModel).Include(chatViewModel).Include(addressViewModel)
        //.Include(historyViewModel).Include(logViewModel).Include(connectViewModel)
        //.Include(hotKeyViewModel).Include(signalRViewModel)
        //.Include(queuePersonalViewModel)
        //.Include(queueACDViewModel)
        //.Include(acdSignalRViewModel)
        .Include(selectableItemHelper).Include(browserHelper));
       //.Include(ciscoHelper).Include(ciscoConnect)
        //.Include(displayPhoneBar));

        BundleTable.EnableOptimizations = true;
        Application[Konstants.APPLICATION_VERSION] = ApplicationSettings.Version;
    }

   
    public override void Init()
    {
        SessionStateModule session = Modules["Session"] as SessionStateModule;
        if (session != null)
        {
            session.Start += new EventHandler(Evt_Session_Start);
            session.End += new EventHandler(Evt_Session_End);
        }

        base.Init();
        // SZ [Jan 27, 2014]
        
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {
        if (this.Context != null && this.Context.Response != null)
            this.Context.Response.AppendHeader("Access-Control-Allow-Origin", "*");
        //this.Context.Response.AddHeader("Access-Control-Allow-Headers", "accept,origin,authorization,content-type");
        // this.Context.Response.AppendHeader("Access-Control-Allow-Origin", @"http://localhost:17356");
        //    // Fires upon attempting to authenticate the use

        //    {
        //        string cookieName = FormsAuthentication.FormsCookieName;
        //        HttpCookie authCookie = Context.Request.Cookies[cookieName];

        //        if ((authCookie == null))
        //        {
        //            //There is no authentication cookie.
        //            return;
        //        }

        //        FormsAuthenticationTicket authTicket = null;

        //        try
        //        {
        //            authTicket = FormsAuthentication.Decrypt(authCookie.Value);
        //        }
        //        catch (Exception ex)
        //        {
        //            //Write the exception to the Event Log.
        //            return;
        //        }

        //        if ((authTicket == null))
        //        {
        //            //Cookie failed to decrypt.
        //            return;
        //        }

        //        //When the ticket was created, the UserData property was assigned a
        //        //pipe-delimited string of group names.
        //        string[] groups = authTicket.UserData.Split(new char[] { '|' });

        //        //Create an Identity.
        //        GenericIdentity id = new GenericIdentity(authTicket.Name, "LdapAuthentication");

        //        //This principal flows throughout the request.
        //        GenericPrincipal principal = new GenericPrincipal(id, groups);

        //        Context.User = principal;
        //    }
    }

    private void Evt_Session_Start(object sender, EventArgs e)
    {
        // SZ [Mar 25, 2013] this has been added so that first request can easily be identified.
        // SZ [Mar 27, 2013] it was written and commented out bcoz if the user has stored credentials in cookie on browser, 
        // the line below would ignore that and take them to the home page anyway. not consistent behaviour with 
        // modern internet applications
        //Session[Konstants.K_FIRST_TIME] = true.ToString();
        Application.ReloadSettings();
    }
    private void Evt_Session_End(object sender, EventArgs e)
    {
        try
        {
            Guid key = Guid.Empty;
            object o = Session[Konstants.K_USERID];
            if (o != null)
                Guid.TryParse(o.ToString(), out key);
            using (SalesTool.Logging.Logging log = SalesTool.Logging.Logging.Instance)
                log.Write(SalesTool.Logging.AuditEvent.SessionTerminate, "Session has been terminated by ASP.NET", key);
        }
        catch { }
    }
    //public void Application_BeginRequest(object src, EventArgs e)
    //{
    //    Context.Items["loadstarttime"] = DateTime.Now;
    //}

    //public void Application_EndRequest(object src, EventArgs e)
    //{
    //    DateTime end = (DateTime)Context.Items["loadstarttime"];
    //    TimeSpan loadtime = DateTime.Now - end;
    //    //Response.Write("<div><h3>This page took " + loadtime.TotalSeconds.ToString() + " secs to load</h3></div>");
    //}

}
