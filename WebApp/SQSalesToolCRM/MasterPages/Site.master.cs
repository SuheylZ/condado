using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DBG = System.Diagnostics.Debug;
using SalesTool.DataAccess;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using SalesTool.Schema;
using System.Data;
using System.Threading;
using SalesTool.DataAccess.Models;

public partial class SiteMasterPage : System.Web.UI.MasterPage, IReportMenu
{
    public GlobalAppSettings settings;
    public SiteMasterPage()
    {
        settings = HttpContext.Current.ApplicationSettings();
    }
    public override void Dispose()
    {
        base.Dispose();
        settings = null;
    }

    #region IReportMenu
    public void Add(string name, string url)
    {
        Telerik.Web.UI.RadMenuItem item = new Telerik.Web.UI.RadMenuItem(name, url);
        tlMenu.Items[1].Items[0].Items.Add(item);
    }

    public int ReportCount
    {
        get
        {
            return tlMenu.Items[1].Items[0].Items.Count;
        }

    }

    public void Clear()
    {
        tlMenu.Items[1].Items[0].Items.Clear();
    }

    #endregion

    #region events

    protected override void OnPreRender(EventArgs e)
    {
        base.OnPreRender(e);
    }

    protected void Evt_SearchRequested(object sender, EventArgs e)
    {
        int i = default(int);  //SZ [Apr 4, 2013] Do no processing except calling the appropiate page
        int.TryParse(ddlSearchOptions.SelectedValue, out i);

        string text = txtSearch.Text.Trim();
        if (text != string.Empty)
        {
            //SZ [Apr 4, redirect to the leads page for search
            string sURL = string.Format("{0}?{1}={2}&{3}={4}",
                Konstants.K_VIEW_LEADS_PAGE,
                Konstants.K_SEARCH_BY, i.ToString(),
                Konstants.K_SEARCH_FOR, text);
            RedirectTo(sURL);
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        //if (Application[Konstants.APPLICATION_VERSION] == null)
        //   Application[Konstants.APPLICATION_VERSION] = ApplicationSettings.Version;
        if (PhoneBar1.FindControl("divQueue") != null)
            PhoneBar1.FindControl("divQueue").Visible = settings.IsPhoneBarGALEnabled;
        if(PhoneBar1.FindControl("hotKeyDiv1") != null && PhoneBar1.FindControl("hotKeyDiv2") != null)
        {
            PhoneBar1.FindControl("hotKeyDiv1").Visible = settings.IsHotKeysVisible;
            PhoneBar1.FindControl("hotKeyDiv2").Visible = settings.IsHotKeysVisible;
        }
        if (!settings.IsPhoneBarEnabled)
            PhoneBar1.Visible = false;
        if (!IsPostBack)
        {
            //if (ApplicationSettings.IsTermLife)
            tlMenu.Items[3].Items[8].NavigateUrl = string.Format("{0}?agentid={1}", tlMenu.Items[3].Items[8].NavigateUrl, (Page as SalesBasePage).CurrentUser.Key.ToString());
            if (settings.IsTermLife)
            {
                foreach (var item in ddlSearchOptions.Items)
                {
                    ((ListItem)item).Selected = false;
                }
                ddlSearchOptions.Items.Insert(0, new ListItem("ARC Reference", "6") { Selected = true });
            }
        }
        //IH- KGalHeight & KGalWidth set "Get A Lead" page popup window size.
        //hdngalHeight.Value = ApplicationSettings.KGalHeight;
        //hdngalWidth.Value = ApplicationSettings.KGalWidth; 

        hdngalHeight.Value = settings.KGalHeight;
        hdngalWidth.Value = settings.KGalWidth;

        var nstatus = (HtmlGenericControl)HeadLoginView.FindControl("loginStatusSpan");
        //if (nstatus != null && ApplicationSettings.IsSSOMode) nstatus.Visible = false;
        if (nstatus != null && settings.IsSSOMode) nstatus.Visible = false;

        if (!IsPostBack)
        {
            pnlOuterForFade.Visible = false;
            if (!homeCss.Href.Contains("HomeTable.css"))
                homeCss.Href = "~/Styles/GeneralTable.css";
            Application[Konstants.K_SCRIPT_MANAGER] = RadScriptManager1;
        }

        if (newPageUrl.Value != "")
        {
            if (newPageUrl.Value == "~/")
            {
                Evt_LoggingOut(null, null);

                Session.Clear();
                //Session.Abandon();
                System.Web.Security.FormsAuthentication.SignOut();
                System.Web.Security.FormsAuthentication.RedirectToLoginPage();
            }

            RedirectTo(newPageUrl.Value);
        }

        //ctlMenu.ApplyPermissions();
        SalesSecurity sec = (Page as SalesBasePage).AppSecurity;
        var engine = (Page as SalesDataPage).Engine;
        var user = (Page as SalesBasePage).CurrentUser;
        if (user != null)
        {
            Session[Konstants.K_USERID] = user.Key;
        }

        if (!IsPostBack)
        {
            //YA[Jan 31, 2014]            
            AcdButtonInitialize(false);

            DBG.Assert((Page as SalesBasePage) != null);

            if (sec.IsAdministrationOff)
                tlMenu.Items[3].Visible = false;
            else
                AlterAdministrationMenu(sec);
            AlterQuickLinksMenu(engine, user);
            if (user.Security.Account.PriorityView == (int)Konstants.AccountPriorityView.Off)
                tlMenu.Items[0].Items[1].Visible = false;
            //SZ [May 21, 2013] Added for reports
            AddReportsMenu(engine, user);
            //YA[May 21, 2013] Custom Report Menu Item Security Setting
            //tlMenu.Items[1].Items[2].Visible = (Page as SalesBasePage).CurrentUser.Security.Report.CustomReportDesigner;

            // Attiq - April-04-2014
            // Changed the index from 2 to 1 because Canned Reports Menu Item is removed from Reports Menu.
            tlMenu.Items[1].Items[1].Visible = (Page as SalesBasePage).CurrentUser.Security.Report.CustomReportDesigner;

            // wm
            EventCalendarHelper eventCalendarHelper = new EventCalendarHelper((Page as SalesBasePage));
            eventCalendarHelper.SetRemindersForNextHoursByUserID((Page as SalesBasePage).CurrentUser.Key);
            eventCalendarHelper.SetNewMostRecentToHappenEventDateTime(AccountId);

            //eventCalendarHelper.StoreLeadLastCalendarDetails(AccountId, (Page as SalesBasePage).CurrentUser.Key);
        }
        //YA[July 31, 2013] View Reconciliation Report(Duplicate Flagged Records) link
        if (sec.CanViewDuplicates)
        {
            var records = engine.DuplicateRecordActions.GetReconciliationReport();
            int countRecord = records.Count();
            if (countRecord > 0)
            {
                lblReconciliationReportTotal.Text = countRecord.ToString();
                divReconciliationReport.Visible = true;
            }
            else
            {
                divReconciliationReport.Visible = false;
            }
        }

        //////[QN, 29/4/2013] this code reset the session timer of application
        //hdnIdleTimeOut.Value = Convert.ToString(ApplicationSettings.IdleTimeOut * 1000);
        hdnIdleTimeOut.Value = Convert.ToString(settings.IdleTimeOut * 1000);
    }

    public ASP.usercontrols_statuslabel_ascx Message
    {
        get { return ctlMessage; }
    }
    public object EventCalendarList
    {
        get { return EventCalendarList2; }
    }
    private bool AcdButtonInitialize(bool IsButtonClicked = true)
    {

        //YA[Jan 31, 2014]        
        var user = (Page as SalesBasePage).CurrentUser;
        //btnACD.Visible =( ApplicationSettings.IsPhoneSystemInContact);
        //MH:02 May 2014 IsPhoneSystemInContact to IsPhoneSystemAcdButtonVisible
        btnACD.Visible = (settings.IsPhoneSystemAcdButtonVisible);
        //if (ApplicationSettings.IsPhoneSystemInContact)
        if (settings.IsPhoneSystemAcdButtonVisible)
        {
            if (GetACDCap(user.Key))
            {
                timerACDUpdate.Enabled = false;
                pnlOuterForFade.Visible = false;
                btnACD.Text = "ACD OFF";
                btnACD.CssClass = "buttonstyleACDOFF";
                if (IsButtonClicked) ShowAlert("ACD hard cap has been reached.", "Alert");
                return false;
            }
            else
            {
                btnACD.Enabled = !(GetACDCap(user.Key));
                if (GetACDOnOff(user.Key))
                {
                    btnACD.Text = "ACD ON";
                    btnACD.CssClass = "buttonstyle";
                }
                else
                {
                    btnACD.Text = "ACD OFF";
                    btnACD.CssClass = "buttonstyleACDOFF";
                }
                if (GetACDStatus(user.Key))
                    btnACD.CssClass = "buttonstyleACDPurple";
            }
        }
        return true;
    }

    // WM - [29.07.2013]
    protected long AccountId
    {
        get
        {
            long accountId = default(long);
            long.TryParse((Session[Konstants.K_ACCOUNT_ID] ?? "").ToString(), out accountId);
            return accountId;
        }
    }

    protected void Page_PreRender(object sender, EventArgs e)
    {
        //string script = "<script type=\"text/javascript\">theForm.submit();</script>";
        //Page.ClientScript.RegisterStartupScript(Page.GetType(), "run_Again", script);
        //HiddenField hdnApplicationTimeOut = (HiddenField)Page.FindControl("hdnIdleTimeOut");
        //hdnIdleTimeOut.Value = Convert.ToString(ApplicationSettings.IdleTimeOut * 1000);
        //string clientWidth = widthHidden.Value;
        //Response.Write(string.Format("Width: {0}, Height: {1}", clientWidth, clientHeight));
    }

    void RedirectTo(string Url)
    {
        Response.Redirect(Url, false);
        HttpContext.Current.ApplicationInstance.CompleteRequest();
    }

    protected void Evt_LoggingOut(object sender, System.Web.UI.WebControls.LoginCancelEventArgs e)
    {
        using (SalesTool.Logging.Logging log = SalesTool.Logging.Logging.Instance)
        {
            Guid key = Guid.Empty;
            object o = Session[Konstants.K_USERID];
            if (o != null)
                Guid.TryParse(o.ToString(), out key);
            log.Write(SalesTool.Logging.AuditEvent.Logout, "The user has logged out", key);
            Session.Remove(Konstants.K_USERID);
        }
        Session.Abandon();
    }

    /// <summary>
    /// [QN, 29/4/2013] this code reset the session timer of application
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnTimerContinue_Click(object sender, EventArgs e)
    {
        //hdnIdleTimeOut.Value = Convert.ToString(ApplicationSettings.IdleTimeOut * 60000);
        hdnIdleTimeOut.Value = Convert.ToString(settings.IdleTimeOut * 60000);
    }
    protected void winSessionTimer_Load(object sender, EventArgs e)
    {
    }

    #endregion

    #region Methods

    void AlterAdministrationMenu(SalesSecurity security)
    {
        tlMenu.Items[3].Items[0].Visible = security.CanManageUsers;
        tlMenu.Items[3].Items[1].Visible = security.CanManageRoles;
        tlMenu.Items[3].Items[2].Visible = security.CanManageSkillGroups;
        tlMenu.Items[3].Items[3].Visible = security.CanManageCampaigns;
        tlMenu.Items[3].Items[4].Visible = security.CanManageOutboundRouting;
        tlMenu.Items[3].Items[5].Visible = security.CanManageEmailTemplates;
        tlMenu.Items[3].Items[6].Visible = security.CanManagePosts;
        tlMenu.Items[3].Items[7].Visible = security.CanManageQuickLinks;
        tlMenu.Items[3].Items[8].Visible = security.CanManageGetALead;
        tlMenu.Items[3].Items[9].Visible = security.CanManageAlerts;
        tlMenu.Items[3].Items[10].Visible = security.CanManagePrioritizationRules;
        tlMenu.Items[3].Items[11].Visible = security.CanManageReassignment;
        tlMenu.Items[3].Items[12].Visible = security.CanManageDuplicateRules;
        tlMenu.Items[3].Items[13].Visible = security.CanViewDuplicates;
        tlMenu.Items[3].Items[14].Visible = security.CanManageDashboard;
    }
    void AlterQuickLinksMenu(SalesTool.DataAccess.DBEngine engine, SalesTool.DataAccess.Models.User user)
    {
        DBG.Assert(user != null);
        //YA [Dec 17, 2012] Dynamic Quick Links Sub Menu
        //if (Session["IsQuickLinkAdded"] ==null)
        //{
        tlMenu.Items[2].Items.Clear();
        //DBEngine engine = (Page as SalesDataPage).Engine;


        //_engine = new SalesTool.DataAccess.DBEngine();
        //int countIndex = 0;
        //var result = engine.QuickLinksActions.GetAll().Where(
        //        x => x.QuickLinkSkills.Count()>0 && !(x.IsDeleted??false) && 

        //            x.QuickLinkSkills.Any(
        //            y => y.SkillGroups.IsDeleted == false && 
        //            y.SkillGroups.Users.Count() > 0 && 
        //            y.SkillGroups.Users.Any(
        //                z => z.Key == currentUser.Key
        //                )
        //            )
        //          ).ToList();

        //SZ [Apr 11, 2013] optimized version of making menu 
        string[] targets = { "_blank", "_parent", "_search", "_self", "_top" };
        SalesTool.DataAccess.Models.FilteredQuickLinks[] arr = engine.QuickLinksActions.GetQuickLinksFor(user.Key).ToArray();
        for (int i = 0; i < arr.Length; i++)
        {
            Telerik.Web.UI.RadMenuItem itm = new Telerik.Web.UI.RadMenuItem { Text = arr[i].Name, NavigateUrl = arr[i].Url, Enabled = arr[i].IsEnabled ?? false };
            itm.Attributes.Add("HasAlert", (arr[i].IsAlert ?? default(bool)).ToString());
            itm.Attributes.Add("Message", arr[i].Message);
            itm.Attributes.Add("TargetManual", targets[arr[i].Target ?? 0]);
            tlMenu.Items[2].Items.Add(itm);

            //tlMenu.Items[2].Items[countIndex].Enabled = U.IsEnabled ?? false;
            //tlMenu.Items[2].Items[countIndex].Target = targetString;
            //tlMenu.Items[2].Items[countIndex].Attributes[] = 
            //tlMenu.Items[2].Items[countIndex].Attributes[;
            //tlMenu.Items[2].Items[countIndex].Attributes["TargetManual"] = targetString;
            //countIndex++;
        }
    }

    //SZ [Apr 11, 2013] String array is used to optimize the menu generation.
    // Array is faster then calling a function
    //private string CheckMenuNavigationTarget(byte target)
    //{
    //    string Ans = "_blank";
    //    switch (target)
    //    {
    //        case 1: Ans="_parent";break;
    //        case 2: Ans = "_search";break;
    //        case 3: Ans = "_self";break;
    //        case 4: Ans = "_top";break;
    //    }
    //    return Ans;
    //}

    public System.Web.UI.WebControls.Button buttonYes
    {
        get { return this.btnYes; }
    }

    public bool DirtyFlag
    {
        get { return dirtyFlag.Value == "1"; }
        set { dirtyFlag.Value = (value ? "1" : "0"); }
    }
    public Telerik.Web.UI.RadWindowManager WindowManager
    {
        get
        {
            return tlWindowManager;
        }
    }

    public void ShowAlert(string message, string Title = "Sales Tool")
    {
        tlWindowManager.RadAlert(message, 400, 100, Title, "", "");
    }
    public void ShowAlert(Exception ex, string Title = "Sales Tool")
    {
        tlWindowManager.RadAlert(ex.Message, 400, 100, Title, "");
    }

    //YA[12 June 2014]
    public int GetWindowWidth()
    {
        int nValue = 0;
        int.TryParse(hdnfieldWidth.Value, out nValue);
        return nValue;
    }
    //YA[12 June 2014]
    public int GetWindowHeight()
    {
        int nValue = 0;
        int.TryParse(hdnfieldHeight.Value, out nValue);
        return nValue;
    }


    //SZ [May 21, 2013] This fucntion aLTERS and adds reports menu
    void AddReportsMenu(SalesTool.DataAccess.DBEngine Engine, SalesTool.DataAccess.Models.User user)
    {
        var Menu = this as IReportMenu;
        System.Collections.Generic.IEnumerable<SalesTool.DataAccess.Models.ReportUsers> reports = Engine.CustomReportsAction.GetReportUsersByUserKey(user.Key).ToList();
        //Imran H:[12.08.12] UnCommented the code after comparing the files with TFS 2252,2253 with 2316
        //[6:46:12 PM] John Dobrotka: Waheed commented some code and messed some stuff up
        //[6:46:36 PM] John Dobrotka: My Reports Menu under the Report menu is not populating

        if (reports.Count() > 0)
        {
            Menu.Clear();
            tlMenu.Items[1].Items[0].Text = tlMenu.Items[1].Items[0].Text + " >";
            List<int> reportsList = reports.Select(s => s.ReportID).ToList();
            List<Report> list = Engine.CustomReportsAction.GetAll().Where(p => reportsList.Contains(p.ReportID)).ToList();
            foreach (var report in list)
            {
                string title = report.ReportTitle;
                Menu.Add(title,
                    string.Format(Konstants.K_DISPLAY_REPORT_PAGE, report.ReportID.ToString())
                    );
            }
            //foreach (SalesTool.DataAccess.Models.ReportUsers report in reports.ToList())
            //{
            //    SalesTool.DataAccess.Models.Report nReport = Engine.CustomReportsAction.Get(report.ReportID);
            //    string title = nReport.ReportTitle;
            //    Menu.Add(title,
            //        string.Format(Konstants.K_DISPLAY_REPORT_PAGE, report.ReportID.ToString())
            //        );
            //}
        }

    }

    //SZ [Aug 27, 2013] It colors the bar and displays the text on the leftmost side.
    public void ColorTheBar(string text, string color)
    {
        loginbar.Style["background"] = color;
        loginbarTop.Style["background"] = color;
        txtCallStatus.Text = text;
    }
    #endregion
    public static string GetPageName()
    {
        string sPagePath = System.Web.HttpContext.Current.Request.Url.AbsolutePath;
        System.IO.FileInfo oFileInfo = new System.IO.FileInfo(sPagePath);
        string sPageName = oFileInfo.Name;
        return sPageName;
    }

    public string PageName { get { return GetPageName(); } }
    protected void btnACD_Click(object sender, EventArgs e)
    {
        var user = (Page as SalesBasePage).CurrentUser;
        if (GetACDStatus(user.Key)) return;
        if (AcdButtonInitialize())
        {
            SetACDStatus("1", user.Key.ToString());
            //InContactSwapSkills.Execute(user, btnACD.Text == "ACD ON" ? false : true);     
            timerACDUpdate.Enabled = true;
            pnlOuterForFade.Visible = true;

        }
    }
    private void SetACDStatus(string status, string userKey)
    {
        string query = @"Update users set usr_acd_status_flag = " + status + " where usr_key = '" + userKey + "'";
        SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, query);
    }

    private bool GetACDStatus(Guid userKey)
    {
        string query = @"select usr_acd_status_flag from users where usr_key = '" + userKey.ToString() + "'";
        object status = SalesTool.Common.SqlHelper.ExecuteScalar(ApplicationSettings.ADOConnectionString, CommandType.Text, query);
        bool result = true;
        bool.TryParse(status.ToString(), out result);
        return result;
    }

    private bool GetACDOnOff(Guid userKey)
    {
        string query = @"select usr_acd_flag from users where usr_key = '" + userKey.ToString() + "'";
        object status = SalesTool.Common.SqlHelper.ExecuteScalar(ApplicationSettings.ADOConnectionString, CommandType.Text, query);
        bool result = true;
        bool.TryParse(status.ToString(), out result);
        return result;
    }

    private bool GetACDCap(Guid userKey)
    {
        string query = @"select usr_acd_cap_flag from users where usr_key = '" + userKey.ToString() + "'";
        object status = SalesTool.Common.SqlHelper.ExecuteScalar(ApplicationSettings.ADOConnectionString, CommandType.Text, query);
        bool result = true;
        bool.TryParse(status.ToString(), out result);
        return result;
    }
    protected void timerACDUpdate_Tick(object sender, EventArgs e)
    {
        var user = (Page as SalesBasePage).CurrentUser;
        if (!GetACDStatus(user.Key))
        {
            timerACDUpdate.Enabled = false;
            pnlOuterForFade.Visible = false;
        }
        AcdButtonInitialize();
    }
}
