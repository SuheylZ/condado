using System;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess.Models;
using DBG = System.Diagnostics.Debug;
using SalesTool.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Telerik.Web.UI;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
using System.Xml;
using System.Xml.Linq;
using System.Linq.Dynamic;



public partial class Leads_Leads : AccountBasePage,
    IIndividualNotification, ITCPAClient
{
    const string K_CAMPAIGN_PARAM = "campaignid";
    const string K_PHONE_PARAM = "phone";
    const string K_STATUS_PARAM = "statusid";
    const string K_ACCOUNT_PARAM = "accountid";
    const string K_CONTACT_ID = "contactid";
    const string K_PARENT_ACCOUNT_PARAM = "parentaccountid";
    const string K_TYPE_PARAM = "type";
    const string K_URL_PROCESSED = "isurlprocessed";
    const string K_PARENT_POPUP_CLOSE = "IsParentPopupClose";
    const string K_LOGIN_BAR_COLOUR = "__myloginbar_colour__";
    private const string K_LEAD_SOURCECODE = "source";
    const string K_PRIORITY_VIEW = "ruleid";
    Boolean IsActionButtonClicked = false;

    //YA[July 31, 2013] Key = Calculated time interval , Value = alert object id
    SortedList<int, string> timerAlertSortedList = new SortedList<int, string>();




    string IntervalsTimerAlert
    {
        get
        {
            return hdnFieldTimerIntervals.Value;
        }
        set
        {
            hdnFieldTimerIntervals.Value = value.ToString();
        }
    }

    DateTime GALBaseTime
    {
        get
        {
            DateTime lAns = DateTime.Now;
            DateTime.TryParse(hdnFieldGALBaseTime.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldGALBaseTime.Value = value.ToString();
        }
    }

    /// <summary>
    /// Current Outpulse ID
    /// </summary>
    string CurrentOutpulseIDDayPhone
    {
        get
        {
            return hdnCurrentOutPulseIDDayPhone.Value;
        }
        set
        {
            hdnCurrentOutPulseIDDayPhone.Value = value.ToString();
        }
    }
    string CurrentArcReferenceID
    {
        get
        {
            return hdnCurrentArcReferenceID.Value;
        }
        set
        {
            hdnCurrentArcReferenceID.Value = value.ToString();
        }
    }
    string ArcHubPath
    {
        get
        {
            return hdnArcHubPath.Value;
        }
        set
        {
            hdnArcHubPath.Value = value.ToString();
        }
    }
    bool UseArcNewImplementation
    {
        get
        {
            bool lAns = false;
            bool.TryParse(hdnUseArcNewImplementation.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnUseArcNewImplementation.Value = value.ToString();
        }
    }
    string CurrentOutpulseIDEveningPhone
    {
        get
        {
            return hdnCurrentOutPulseIDEveningPhone.Value;
        }
        set
        {
            hdnCurrentOutPulseIDEveningPhone.Value = value.ToString();
        }
    }
    //YA[27 Feb 2014]
    string CurrentOutpulseIDCellPhone
    {
        get
        {
            return hdnCurrentOutPulseIDCellPhone.Value;
        }
        set
        {
            hdnCurrentOutPulseIDCellPhone.Value = value.ToString();
        }
    }

    int CampaignID
    {
        get
        {
            int lAns = 0;
            int.TryParse(hdnFieldCampaignID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldCampaignID.Value = value.ToString();
        }
    }
    int PrimaryStatusID
    {
        get
        {
            int lAns = 0;
            int.TryParse(hdnFieldPrimaryStatus.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldPrimaryStatus.Value = value.ToString();
        }
    }
    int TimerAlertID
    {
        get
        {
            int lAns = 0;
            int.TryParse(hdnFieldTimerAlertID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldTimerAlertID.Value = value.ToString();
        }
    }

    private bool ShowTimerAlert
    {
        get
        {
            bool iVal = false;
            bool.TryParse(hdnShowTimerAlert.Value, out iVal);
            return iVal;
        }
        set
        {
            hdnShowTimerAlert.Value = value.ToString();
        }
    }

    public void IndividualChanged(IIndividual handle)
    {
        UpdateQuickFields(CurrentAccount);
    }
    
    /// <summary>
    /// Timer control tick event used in timer alert implementation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void timerAlert_Tick(object sender, EventArgs e)
    {
        SetTimerInterval();
    }
    //protected void Page_PreInit(object sender, EventArgs e)
    //{
    //    //if (Request.Browser.Browser == "IE" && Request.Browser.Version == "7.0")
    //    //{
    //    //    Page.Theme = "Default_IE7";
    //    //}
    //    //else
    //    //    Page.Theme = "Default";

    //}
    protected override void Page_Initialize(object sender, EventArgs args)
    {
        BindEvents();

        if (!IsSimpleUrl && !IsPostBack)
        {
            IsActionProcessed = false;
            if (IsAccountRequest)
            {
                long accountId = Helper.SafeConvert<long>(Request.QueryString[Konstants.K_ACCOUNT_ID]);

                if (accountId > 0 && Engine.AccountActions.Exists(accountId))
                {
                    // IH [25.09.13] here !IsAccountSoftDeleted
                    if (IsAccountSoftDeleted(accountId))
                        return;

                    AccountID = accountId;
                    //YA[July 31, 2013] Timer Alert Implementation
                    if (IsUrlProcessed)
                    {
                        bool UrlProcessed = false;
                        bool.TryParse(Request.QueryString[K_URL_PROCESSED], out UrlProcessed);
                        if (UrlProcessed)
                        {
                            ShowTimerAlert = true;
                            ShowAlertByTime();
                        }
                    }
                }
                else if (IsNewAccountRequest)
                {
                    AccountID = 0; LeadID = 0;
                }
                else
                {
                    // SZ [Sep 20, 2013] Now this is an error that non existent accoutn is requested. 
                    // Leads page should be disabled or not displayed at all.
                    AccountID = -2; LeadID = -2;
                }
            }
            else
            {
                // SZ [Sep 20, 2013] here if !IsAccountRequest
                // SZ [Oct 8, 2013] clear the accountID and LeadID
                ClearSessionNCache();
                string page = string.Format("{0}?accountid={1}&" + K_URL_PROCESSED + "={2}", Konstants.K_LEADS_PAGE,
                    ProcessUrl(),
                    ShowTimerAlert);
                // IH [25.09.13] here !IsAccountSoftDeleted, the isPhoneNoInUrl= true incase of phone search
                IsAccountSoftDeleted(AccountID, true);
                Response.Redirect(page);

            }

        }

        if (!IsPostBack)
        {
            InnerInitialize();
            if (AccountID > 0)
                InnerLoad(AccountID);
            else if (!IsNewAccountRequest)
                DenyAllFunctionality();
            //YA[18 March 2014] Click from Add New Individual for new account, load the default values according to parentaccountID as default values
            else if (IsNewAccountRequest && ParentAccountID > 0 && Engine.ApplicationSettings.IsMultipleAccountsAllowed)
            {
                LoadDefaultValues(ParentAccountID);
                PageAccountId = -1;
                LeadID = 0;
                AccountID = 0;
            }
            else if (IsNewAccountRequest && AccountID == 0)
            {
                ddUsers.SelectedValue = CurrentUser.Key.ToString();
                ddOU.SelectedValue = CurrentUser.Key.ToString();
            }
        }

        //MH:18 April disaple manage event button at new account.
        btnAddEvent1.Enabled = !(IsNewAccount || IsNewAccountRequest);
        //btnApplyAction.Enabled = !(IsNewAccount || IsNewAccountRequest);
        //if (IsNewAccount || IsNewAccountRequest)
        //{
        //    btnApplyAction.ToolTip = "Record is not saved yet!";
        //}
        //SZ [Sep 20, 2013] Code moved to InnerLoad()
        //SZ [Sep 20, 2013] Code removed, see history in TFS 

        //TM [05 09 2014] Added on Task request
        if (Engine.ApplicationSettings.IsTermLife)
        {
            btnEmailSender.Visible = false;
        }
    }

    //YA[19 March 2014] Added to load the default values for new account based on parent account data.
    private void LoadDefaultValues(long accID = 0)
    {
        Account A = Engine.AccountActions.Get(accID);
        EnableControlsBasedOnPermissions(); //SZ [Sep 13, 2013] Moved from Page_Initialize() to here as this is appropiate place

        if (A == null)
            Redirect(Konstants.K_VIEW_LEADS_PAGE);

        // SZ [Dec 19, 2013] since the account may contain a deleted user hence we need to rebind the user dropdowns
        // like this => (All users not deleted)+ (any assigned user of this account even if deleted) 
        // The commented out code was added by Muzamil H and is not longer needed as BindUserDropdowns() takes care of --Unassigned--(-1) 
        BindUserDropdowns(A.Key);

        if (A.AssignedUserKey.HasValue)
        {
            ddUsers.SelectedValue = A.AssignedUserKey.Value.ToString();
            //ddOU.SelectedValue = A.AssignedUserKey.Value.ToString();
        }

        if (A.OriginalUserKey.HasValue)
            ddOU.SelectedValue = A.OriginalUserKey.Value.ToString();
        if (A.AssignedCsrKey.HasValue)
            ddCSR.SelectedValue = A.AssignedCsrKey.Value.ToString();

        if (A.TransferUserKey.HasValue)
            ddTA.SelectedValue = A.TransferUserKey.Value.ToString();

        if (A.OnBoardUser.HasValue)
            ddOP.SelectedValue = A.OnBoardUser.Value.ToString();
        if (A.AlternateProductUser.HasValue)
            //MH:May 19, 2014 Fix: invalid selected value ddAltProduct
            ddAltProduct.SetSelectedValue(A.AlternateProductUser.Value.ToString());

        //YA[May 02, 2013] Changed the above sited code in the simple form
        lblDateCreatedText.Text = A.AddedOn.HasValue ? A.AddedOn.Value.ToString("MM/dd/yyyy hh:mm") : "";
        lblModifiedDateText.Text = A.ChangedOn.HasValue ? A.ChangedOn.Value.ToString("MM/dd/yyyy hh:mm") : "";
        //wm
        txtExternalAgent.Text = A.ExternalAgent ?? string.Empty;
        txtNotes.Text = A.Notes;
        txtLifeInfo.Text = A.LifeInfo;
        Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) :
           A.Leads.FirstOrDefault();
        if (L != null)
        {
            //LeadID = lea.Key;

            if (L.CampaignId.HasValue)
            {
                ddCampaigns.SelectedValue = L.CampaignId.ToString();
                CampaignID = L.CampaignId.Value;
                if (!IsPostBack)
                {
                    lblCampaignName.Text = ddCampaigns.SelectedItem != null ? ddCampaigns.SelectedItem.Text : "";
                    if (!IsParentPopupClose)
                    {
                        ctlPagerCampaignAlerts.Initialize(true);
                        int statusID = L.StatusId.HasValue ? L.StatusId.Value : 0;
                        PrimaryStatusID = statusID;
                        if (ShowCampaignAlerts(CampaignID.ToString(), PrimaryStatusID))
                            dlgCampaignAlert.VisibleOnPageLoad = true;
                    }
                }
            }
            if (L.StatusId.HasValue)
            {
                if (ddlStatus.Items.FindByValue(L.StatusId.ToString()) != null)
                    ddlStatus.SelectedValue = L.StatusId.ToString();
            }

            if (ddlStatus.Items.Count > 0)
            {
                int statusID = L.StatusId ?? 0;
                // MH [Nov 1, 2013] Function to handle the apply action button if no actions 
                BindAvailableActions(statusID);
                //MH:19 Sep 2014: Commented on Troy's request.
                //Helper.SafeAssignSelectedValueToDropDown(ddActions, Convert.ToString(L.ActionId));

                //if (lea.ActionId.HasValue)
                //    ddActions.SelectedValue = lea.ActionId.ToString();
                if (statusID > 0)
                {
                    //MH: 03 May 2014   Exception message: 'ddlSubStatus1' has a SelectedValue which is invalid because it does not exist in the list of items.
                    ddlSubStatus1.SafeBind(Engine.StatusActions.GetSubStatuses(statusID, false), L.SubStatusId.ToString());
                    //ddlSubStatus1.SelectedValue = null;
                    //ddlSubStatus1.DataSource = Engine.StatusActions.GetSubStatuses(statusID, false);
                    //ddlSubStatus1.DataBind();
                }
                if (L.SubStatusId != null)
                {
                    ddlSubStatus1.SetSelectedValue(L.SubStatusId.ToString());
                    //ddlSubStatus1.SelectedValue = lea.SubStatusId.ToString();   
                }
            }
            lblLastCallDate.Text = L.LastCallDate.HasValue ? L.LastCallDate.Value.ToString("MM/dd/yyyy hh:mm") : "";

            //WM - 11.07.2013
            lblLastActionDate.Text = L.LastActionDate.HasValue ? L.LastActionDate.Value.ToString("MM/dd/yyyy hh:mm") : "";

            //IH-09.10.12
            if (L.Account.AssignedUserKey != null)
                lblFirstAssignedUser.Text = Engine.UserActions.GetAll(true).Where(x => x.Key == L.Account.AssignedUserKey.Value).FirstOrDefault().FullName;

        }
        DialPhone();
        EnableTabs();
    }
    private void ClearSessionNCache()
    {
        AccountID = 0;
        LeadID = 0;
        ShouldRedirectLeads = false;
    }

    void DenyAllFunctionality()
    {
        // SZ [Sep 20, 2013] There is no account no leads just an error, it will cause more bugs if user is allowed to perform functions on non existent data
        // To understand the senario, see the code in the Page_Initialize()
        // so anyone seeing this function, write the code here to disable the whole leads page. You can call this fucntion 
        // from exception handlers where you think user should not be allowed to continue any further.
    }

    /// <summary>
    ///  IH [25.09.13] checked here accountd  IsAccountSoftDeleted,then it will not dispalyed the detail
    /// </summary>
    /// <param name="accountId"></param>
    /// <param name="isPhoneNoInUrl"></param>
    /// <returns></returns>
    bool IsAccountSoftDeleted(long accountId, bool isPhoneNoInUrl = false)
    {
        if (Engine.LeadsActions.IsLeadAccountSoftDeleted(accountId))
        {

            AccountID = 0; LeadID = 0;
            if (isPhoneNoInUrl)
                Response.Redirect(string.Format("{0}?accountid=-1", Konstants.K_LEADS_PAGE));
            else
                rdWAccountExist.VisibleOnPageLoad = true;
            return true;
        }
        return false;
    }


    /// <summary>
    /// Timer alert implementation 
    /// </summary>
    private void ShowAlertByTime()
    {
        SqlParameter[] arrSqlParam = new SqlParameter[] { new SqlParameter("userid", CurrentUser.Key.ToString()) };
        Object nObj = SalesTool.Common.SqlHelper.ExecuteScalar(ApplicationSettings.ADOConnectionString, CommandType.StoredProcedure, "proj_CheckTimerAlertGAL", arrSqlParam);
        if (nObj != null)
        {
            DateTime nGALDateTime = DateTime.Now;
            if (DateTime.TryParse(nObj.ToString(), out nGALDateTime))
                GALBaseTime = nGALDateTime;
        }
        else
            return;

        Lead L = Engine.AccountActions.GetPrimaryLead(AccountID);
        IQueryable<Alert> T = null;
        if (L != null)
        {
            string campaignID = L.CampaignId.HasValue ? L.CampaignId.Value.ToString() : "";
            int primaryStatusID = L.StatusId.HasValue ? L.StatusId.Value : 0;
            T = Engine.ManageAlertsActions.GetAllTimerAlertsByConditions(campaignID, primaryStatusID).OrderBy(x => x.TimeLapse);
        }
        else
            T = Engine.ManageAlertsActions.GetAllEnabledTimerAlerts().OrderBy(x => x.TimeLapse);

        timerAlertSortedList = new SortedList<int, string>();
        foreach (Alert item in T)
        {
            int _timeLapse = 0;
            if (int.TryParse(item.TimeLapse, out _timeLapse))
            {
                int nType = 0;
                if (int.TryParse(item.Notes, out nType))
                {
                    Konstants.TimeLapseType nTimelapseType = (Konstants.TimeLapseType)nType;
                    switch (nTimelapseType)
                    {
                        case Konstants.TimeLapseType.Seconds:
                            _timeLapse += _timeLapse * 1000;
                            break;
                        case Konstants.TimeLapseType.Minutes:
                            _timeLapse += _timeLapse * 60 * 1000;
                            break;
                        case Konstants.TimeLapseType.Hours:
                            _timeLapse += _timeLapse * 60 * 60 * 1000;
                            break;
                        default:
                            break;
                    }
                }
                AddTimerAlert(item, _timeLapse);
            }
        }
        Session["TimerAlertSortedList"] = timerAlertSortedList;
        SetTimerInterval();
    }

    /// <summary>
    /// Sorted list need unique values, Check the value key if already exists
    /// than add 1 millisecond and then try again.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="_timeLapse"></param>
    private void AddTimerAlert(Alert item, int _timeLapse)
    {
        if (!timerAlertSortedList.ContainsKey(_timeLapse))
            timerAlertSortedList.Add(_timeLapse, item.Id.ToString());
        else
        {
            _timeLapse += 1;
            AddTimerAlert(item, _timeLapse);
        }
    }

    /// <summary>
    /// Set Timer interval, 
    /// </summary>
    private void SetTimerInterval()
    {
        int nResult = 0;
        if (Session["TimerAlertSortedList"] != null)
        {
            timerAlertSortedList = (SortedList<int, string>)Session["TimerAlertSortedList"];
            bool hasIntervalSet = false;
            List<string> nKeyToRemove = new List<string>();
            foreach (var item in timerAlertSortedList)
            {
                int alertID = 0;
                if (int.TryParse(item.Value, out alertID))
                {
                    TimerAlertID = alertID;
                }
                if (GALBaseTime.AddMilliseconds(item.Key).CompareTo(DateTime.Now) <= 0)
                {
                    if (TimerAlertID > 0)
                    {
                        Alert nAlert = Engine.ManageAlertsActions.Get(TimerAlertID);
                        RadWindowControl uc = Page.LoadControl("..\\UserControls\\RadWindowControl.ascx") as RadWindowControl;
                        uc.SetLabel(nAlert.Name, nAlert.Message, nAlert.AlertType.Name, "");
                        divTimerAlert.Controls.Add(uc);
                        nKeyToRemove.Add(item.Value);
                    }
                }
                else if (!hasIntervalSet)
                {
                    TimeSpan timeDiff = GALBaseTime.AddMilliseconds(item.Key) - DateTime.Now;
                    timerAlert.Interval = (int)timeDiff.TotalMilliseconds;
                    hasIntervalSet = true;
                    nResult = item.Key;
                }
            }
            foreach (string keyValue in nKeyToRemove)
            {
                timerAlertSortedList.RemoveAt(timerAlertSortedList.IndexOfValue(keyValue));
            }
            Session["TimerAlertSortedList"] = timerAlertSortedList;
        }
        timerAlert.Enabled = nResult > 0;
    }

    void EnableControlsBasedOnPermissions()
    {
        var userPermission = CurrentUser.UserPermissions.FirstOrDefault();
        if (userPermission.Permissions.Account != null)
        {
            var accountPermission = userPermission.Permissions.Account;
            ddUsers.Enabled = accountPermission.Reassign;
            ddCSR.Enabled = accountPermission.ReassignCSR;
            ddTA.Enabled = accountPermission.ReassignTA;
            ddOP.Enabled = accountPermission.ReassignOB;
            ddAltProduct.Enabled = accountPermission.ReassignAP;

            ddlStatus.Enabled = accountPermission.ReassignedStatus;
            ddlSubStatus1.Enabled = accountPermission.ReassignedStatus;
            //WM - 10.06.2013
            txtExternalAgent.Enabled = accountPermission.CanEditExternalAgent;

            //[QN, 11-04-2013] 062: Read Only Campaign/Source Drop Down List on Leads.aspx when
            // CampaignOverride is checked. It do not hold for a new account. 
            if (AccountID > 0)
                ddCampaigns.Enabled = accountPermission.CampaignOverride;
        }
    }

    // SZ[Apr 18, 2013] this updates the quick fields for the primary and secondary users.

    void UpdateQuickFields(Account A)
    {

        ClearQuickFields();
        BindIndividuaDropdown();

        // long l = Helper.SafeConvert<long>(ddlIndividual.SelectedValue);
        long l = SelectedIndividualId;
        SalesTool.DataAccess.Models.Individual person = new Individual()
            {
                //MH:03 June 2014
                IsActive = true,
                IsDeleted = false,
                AddedBy = CurrentUser.FullName,
                AddedOn = DateTime.Now
            };
        if (l > 0)
            if (Engine.ApplicationSettings.IsTermLife)
                person = Engine.AccountActions.Get(SelectedIndividualAccountId.ConvertOrDefault<long>()).PrimaryIndividual;
            else
                person = Engine.IndividualsActions.Get(l);//A.Individuals.Where(x => x.Key == l).FirstOrDefault();

        SetIndividualDetails(/*IndividualType.Primary,*/ person);
        //HasSpouce = (person != null);

        //if (HasSpouce)
        //    SetIndividualDetails(IndividualType.Secondary, person);
        //ShowSpouceControls(HasSpouce);
    }

    //void UpdateQuickFields(long accID)
    //{
    //    ClearQuickFields();



    //    SalesTool.DataAccess.Models.Individual person = Engine.AccountActions.GetIndividual(accID, IndividualType.Primary);
    //    SetIndividualDetails(/*IndividualType.Primary,*/ person);

    //    //person = Engine.AccountActions.GetIndividual(accID, IndividualType.Secondary);
    //    //HasSpouce = (person != null);

    //    //if (HasSpouce)
    //    //    SetIndividualDetails(IndividualType.Secondary, person);
    //    //ShowSpouceControls(HasSpouce);
    //}

    void ClearQuickFields()
    {
        txtFName.Text = string.Empty;
        txtLName.Text = string.Empty;
        txtMiddleName.Text = string.Empty;
        txtDayTimePhNo.Text = string.Empty;
        txtEvePhNo.Text = string.Empty;
        txtCellPhNo.Text = string.Empty;
        txtZipCodePrimary.Text = string.Empty;
        txtEmailIndv.Text = string.Empty;
        chkEmailOptOutPrimary.Checked = false;
        txtAddress1Primary.Text = string.Empty;
        txtAddress2Primary.Text = string.Empty;
        txtCityPrimary.Text = string.Empty;
        ddlStatePrimary.SelectedIndex = -1;
        //YA[27 Feb 2014]
        ddlApplicationState.SelectedIndex = -1;

        diDOB.SelectedDate = null;
        diDOB2.SelectedDate = null; //kamran
        ddlGenderP.SelectedIndex = -1;

        // SZ [May 19, 2014] XXXXXX
        //txtSpouseFName.Text = string.Empty;
        //tbSpouseLastName.Text = string.Empty;
        //rdisecondDob.SelectedDate = null;
        //ddlGenderS.SelectedIndex = -1;
        //txtEmailSpouse.Text = string.Empty;
        //chkEmailOptOutSpuse.Checked = false;
        //txtAddress1Secondary.Text = string.Empty;
        //txtAddress2Secondary.Text = string.Empty;
        //txtCitySecondary.Text = string.Empty;
        //ddlStateSecondary.SelectedIndex = -1;
        //txtZipCodeSecondary.Text = string.Empty;
        //ddlConsent.SelectedIndex = 0;
    }

    long ProcessUrl()
    {
        ShowTimerAlert = true;
        string phone = "";
        int campaignId = 0, statusId = 0;
        long accountId = 0;
        TRType type = TRType.Unknown;
        CallType calltype = CallType.Unknown;
        string contactId = "";
        ExtractParameters(ref phone, ref campaignId, ref statusId, ref accountId, ref type, ref calltype, ref contactId);
        RectifyPhoneNumber(ref phone);

        if (phone != string.Empty)
        {

            List<long> accIds = new List<long>();
            using (SalesTool.Schema.ProxySearch search = new SalesTool.Schema.ProxySearch(ApplicationSettings.ADOConnectionString))
                accIds = search.SearchByPhone(phone);
            Lead L = new Lead();
            switch (accIds.Count)
            {
                case 0:
                    //MH:14- may 
                    //Engine.MarkArcUserAssignmentDeliveryAs = false;
                    // SZ [Apr 4, 2013] Create new account with campaignId & statusId
                    ClearSessionNCache();
                    accountId = InnerSave(phone, campaignId, statusId, type);
                    //SZ [Sep 12, 2013] made after the client discussion
                    AccountLog(accountId, string.Format("A new account has been created by screen pop. Url is {0}", Request.ToString()), 0, contactId);
                    GALEnhancements(phone, accountId, contactId);

                    break;
                case 1:
                    accountId = accIds[0];
                    Account A = Engine.AccountActions.Get(accIds[0]);
                    AccountLog(accountId, string.Format("Account has been found through screen pop. Url is {0}", Request.Url.ToString()), 0, contactId);
                    if (campaignId == 0)
                    {
                        L = Engine.AccountActions.GetPrimaryLead(A.Key);
                        LeadID = L.Key;
                        // SZ [Nov 13, 2013] commented out as per clients request. this shoudl be handled only when type is given
                        //if (A.TransferUserKey == null)
                        //{
                        //    A.TransferUserKey = CurrentUser.Key;
                        //    Engine.AccountActions.Update(A);
                        //}
                    }
                    else
                    {
                        L = Engine.AccountActions.GetPrimaryLead(A.Key);
                        if (L.CampaignId != campaignId)
                        {
                            // SZ [may 15, 2013] this has been added to avoid crash if a non existent campaign lead is saved.
                            campaignId = Engine.ManageCampaignActions.Exists(campaignId) ? campaignId : Engine.ApplicationSettings.DefaultCampaignId;
                            Lead Ln = L.Duplicate();
                            Ln.CampaignId = campaignId;
                            Ln.StatusId = (L.StatusId ?? 0) <= 0 ? Engine.ApplicationSettings.DefaultStatusId : L.StatusId;
                            Ln.AddedBy = CurrentUser.FullName;
                            Ln.AddedOn = DateTime.Now;
                            LeadID = Engine.LeadsActions.Add(Ln).Key;
                            //A.PrimaryLeadKey = Ln.Key;
                            //Engine.AccountActions.Update(A);
                        }
                    }
                    if (A.AssignedUserKey == null)
                    //YA[03 May 2014] Commented out this line of code and added else statement as per john equirement that if assigned user is null
                    //then assigned it with current user, otherwise ask for takeownership.
                    //  || A.AssignedUserKey != CurrentUser.Key)//MH:25 March 2014
                    {
                        A.AssignedUserKey = CurrentUser.Key;
                        //MH:25 March 2014
                        AccountLog(A.Key, "User assigned");
                        AccountLogAssignment(A.Key, "User assigned", " from Screen pop before takeownership");
                        Engine.AccountActions.Update(A, CurrentUser.FullName);
                    }
                    else
                        TakeOwnership(A);
                    if (A.OriginalUserKey == null)
                    {
                        A.OriginalUserKey = CurrentUser.Key;
                        //SR 24 4 2014
                        AccountLog(A.Key, "Original User assigned");
                        AccountLogAssignment(A.Key, "Original User assigned", " from Screen pop when it was null");
                        Engine.AccountActions.Update(A, CurrentUser.FullName);
                    }
                    AccountID = A.Key;
                    GALEnhancements(phone, accountId, contactId);
                    if (!string.IsNullOrEmpty(contactId) && LeadID > 0)
                    {
                        var lead = Engine.LeadsActions.Get(LeadID);
                        if (lead != null)
                        {
                            DateTime currentDateTime = DateTime.Now;
                            lead.LastCallDate = currentDateTime;
                            Engine.LeadsActions.Update(lead);
                            lblLastCallDate.Text = currentDateTime.ToString("MM/dd/yyyy hh:mm");
                        }
                    }
                    break;
                default:
                    if (accIds.Count > 1) GALEnhancements(phone, accIds[0]);
                    Response.Redirect(Konstants.K_VIEW_LEADS_PAGE + "?by=3&for=" + phone);
                    break;
            }
        }
        // SZ [Aug 27, 2013] added for the bar colour
        Session[K_LOGIN_BAR_COLOUR] = string.Format("{0}:{1}", accountId, (int)calltype);
        return accountId;
    }

    private void GALEnhancements(string phone, long accountId, string contactid = "")
    {
        //YA[September 24, 2013] Added as a GAL Enahancements, 
        //For ScreenPop added Call history.
        Engine.AccountHistory.AddCall(accountId, phone, CurrentUser.Key, contactid);
        //SZ [Apr 18, 2014] Added for updating the fields of call contact
        UpdateLeadCallInformation(accountId, CurrentUser.Key, false);

        BindActionHistoryGrid(accountId);
        if (LeadID > 0)
        {
            var lead = Engine.LeadsActions.Get(LeadID);

            if (lead != null)
            {
                DateTime currentDateTime = DateTime.Now;
                lead.LastCallDate = currentDateTime;

                Engine.LeadsActions.Update(lead);
                lblLastCallDate.Text = currentDateTime.ToString("MM/dd/yyyy hh:mm");
            }
        }

        //If agent_first_call is null, write the call log timestamp to the agent_first_call field
        SqlParameter[] arrSqlParam = new SqlParameter[] { new SqlParameter("userid", CurrentUser.Key.ToString()) };
        Object nObj = SalesTool.Common.SqlHelper.ExecuteScalar(ApplicationSettings.ADOConnectionString, CommandType.StoredProcedure, "proj_CheckAgentFirstCall", arrSqlParam);
        if (nObj == null || nObj.ToString() == "")
        {
            SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.StoredProcedure, "proj_UpdateAgentFirstCall", arrSqlParam);
        }
    }

    //SZ [Apr 3, 2013] this functions tries to identify issues and corrects them when possible
    bool RectifyPhoneNumber(ref string phone) //, ref int campaignId, ref int statusId, ref int accountId, ref TRType type)
    {
        bool bAns = false;
        //SZ [Apr 15, 2013] these lines are removed. this shoudl be checked later
        //campaignId = Engine.ManageCampaignActions.Exists(campaignId) ? campaignId : ApplicationSettings.DefaultCampaignId; // Helper.GetIntFromConfig(K_DEFAULT_CAMPAIGNID);
        //statusId = Engine.StatusActions.Exists(statusId) ? statusId : ApplicationSettings.DefaultStatusId; // Helper.GetIntFromConfig(K_DEFAULT_STATUSID);

        // SZ [Apr 11, 2013] This is necessary. Now as the search engine is used, partial phone numbers cannot be used. 
        // because search engine will try to find a match even with partial numbers and treat them as phone number phrases rather than as complete phone numbers.
        phone = !(phone.Length < 10 || phone.Length > 12) ? phone.Trim() : string.Empty;
        foreach (char c in phone.ToCharArray())
            if (!char.IsDigit(c))
            {
                phone = "";
                break;
            }
        bAns = phone != string.Empty;
        return bAns;
    }

    //SZ [Mar 15, 2013] Extracts the paramters from the request 
    void ExtractParameters(ref string phone, ref int campaignId, ref int statusId, ref long accountId, ref TRType trparam, ref CallType calltype, ref string contactId)
    {
        const string K_TYPE_BASIC = "basic";
        const string K_TYPE_XFER = "xfer";

        const string K_TYPE_CSR = "csr";
        const string K_TYPE_AP = "ap";
        const string K_TYPE_OB = "ob";

        const string K_TYPE_CALL_TYPE = "calltype";

        const string K_CALL_INBOUND = "inbound";
        const string K_CALL_OUTBOUND = "outbound";
        const string K_CALL_MANUAL = "manual";

        phone = Request.QueryString[K_PHONE_PARAM] ?? "";
        campaignId = Convert.ToInt32(Request.QueryString[K_CAMPAIGN_PARAM] ?? "0");
        statusId = Convert.ToInt32(Request.QueryString[K_STATUS_PARAM] ?? "0");
        accountId = Convert.ToInt64(Request.QueryString[K_ACCOUNT_PARAM] ?? "0");
        string sType = (Request.QueryString[K_TYPE_PARAM] ?? "").ToLower();
        string scall = (Request.QueryString[K_TYPE_CALL_TYPE] ?? "").ToLower();
        trparam = sType == K_TYPE_BASIC ? TRType.Basic : sType == K_TYPE_XFER ? TRType.Xfer : sType == K_TYPE_CSR ? TRType.CSR : sType == K_TYPE_AP ? TRType.AP : sType == K_TYPE_OB ? TRType.OB : TRType.Unknown;
        calltype = (scall == K_CALL_INBOUND) ? CallType.Inbound : (scall == K_CALL_OUTBOUND) ? CallType.Outbound : (scall == K_CALL_MANUAL) ? CallType.Manual : CallType.Unknown;

        contactId = Request.QueryString[K_CONTACT_ID] ?? "";
        Session[K_CONTACT_ID] = contactId;

    }

    //SZ [Mar 15, 2013] this is added for url disection
    bool IsSimpleUrl
    {
        get
        {
            return Request.QueryString.Count == 0;
        }
    }

    bool IsAccountRequest
    {
        get
        {
            return Request.QueryString[Konstants.K_ACCOUNT_ID] != null;
        }
    }
    int ActiveRuleId
    {
        get
        {
            int Ans = 0;
            Ans = !string.IsNullOrEmpty(Request.QueryString[K_PRIORITY_VIEW]) ?
                Helper.SafeConvert<int>(Request.QueryString[K_PRIORITY_VIEW]) :
                0;
            return Ans;
        }
    }

    bool IsNewAccountRequest
    {
        get
        {
            int i = default(int);
            int.TryParse(Request.QueryString[Konstants.K_ACCOUNT_ID] ?? "", out i);
            return i == -1;
        }
    }

    bool DoAvoidReassignment
    {
        get
        {
            bool i = false;
            bool.TryParse(Request.QueryString[Konstants.K_AVOID_REASSIGNMENT] ?? "", out i);
            return i;
        }
    }

    long ParentAccountID
    {
        get
        {
            long i = default(int);
            long.TryParse(Request.QueryString[Konstants.K_PARENT_ACCOUNT_ID] ?? "0", out i);
            return i;
        }
    }

    bool IsUrlProcessed
    {
        get
        {
            return Request.QueryString[K_URL_PROCESSED] != null;
        }
    }

    bool IsParentPopupClose
    {
        get
        {
            bool i = false;
            bool.TryParse(Request.QueryString[K_PARENT_POPUP_CLOSE] ?? "", out i);
            return i;
            //return Request.QueryString[K_PARENT_POPUP_CLOSE] != null;
        }
    }

    bool IsTAUser
    {
        get
        {
            //return (Engine.UserActions.GetTA().Where(x => x.Key == CurrentUser.Key).Count() == 1);
            return (Engine.UserActions.GetTA().Count(x => x.Key == CurrentUser.Key) == 1);
        }
    }

    bool IsCSRUser
    {
        get
        {
            // return (Engine.UserActions.GetCSR().Where(x => x.Key == CurrentUser.Key).Count() == 1);
            return (Engine.UserActions.GetCSR().Count(x => x.Key == CurrentUser.Key) == 1);
        }
    }

    void InnerInitialize()
    {
        if (IsNewAccount || IsNewAccountRequest)
        {
            btnEmailSender.Visible = false;
        }
        //YA[18 Dec, 2013]
        if (Engine.ApplicationSettings.HasButtonQuote)
        {
            string url = Engine.ApplicationSettings.ButtonQuoteURL + "?accountid=" + AccountID;//"http://aqe.condadogroup.com/ApplicantProfile.aspx?accountid=" + AccountID;
            string s = "window.open('" + url + "', '_blank');return false;";
            btnQuote.OnClientClick = s;
            btnQuote.Visible = true;
        }
        //YA[29 Jan, 2014]
        vldReqStatePrimary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        vldReqGender.Enabled = Engine.ApplicationSettings.IsTermLife;
        vldReqdiDOB.Enabled = Engine.ApplicationSettings.IsTermLife;
        //vldReqdiDOB2.Enabled = Engine.ApplicationSettings.IsTermLife;

        //SZ [may 20, 2013] moved from page_ionitialize to here.
        ClearFields();
        //[QN 6/4/2013]
        //this code populate iframe in the Get A Lead area of Leads.aspx.
        //getALeadFrame.Attributes.Add("src", "http://sqsst.condadogroup.com/gal/default.aspx?agentid=" + CurrentUser.Key);
        //accountIden.Value = Convert.ToString(AccountID);    
        //ClearInformation();

        //getALeadFrame.Attributes.Add("src", this.GetBaseUrl("~/gal/default.aspx?agentid=" + CurrentUser.Key));

        //HasSpouce = false;
        //ShowSpouceControls(false);

        //SZ [May 1, 2013] Removed the function due to performance reasons, code is shifted back to InnerInitialzie()
        //YA[April 17, 2013] Separated a function for filling the dropdowns as InnerInitialize function getting too long...
        //FillDropDownLists();
        ddlStatus.DataSource = Engine.StatusActions.All;
        ddlStatus.DataBind();

        //SZ [Apr 18, 2013] have been added to show states for the spouce. it is not a reported bug so far
        ddlStatePrimary.DataSource = USStates;
        ddlStatePrimary.DataBind();
        ddlStatePrimary.Items.Insert(0, new ListItem("--Select State--", "-1"));
        ddlStatePrimary.SelectedIndex = 0;

        //YA[27 Feb 2014]
        ddlApplicationState.DataSource = USStates;
        ddlApplicationState.DataBind();
        ddlApplicationState.Items.Insert(0, new ListItem("--Select State--", "-1"));
        ddlApplicationState.SelectedIndex = 0;

        //ddlStateSecondary.DataSource = USStates;
        //ddlStateSecondary.DataBind();

        if (ddlStatus.Items.Count > 0)
        {
            //MH. Disable Apply Action button if no action found
            int statusId = Convert.ToInt32(ddlStatus.SelectedValue);
            BindAvailableActions(statusId);
            //var actionSource = Engine.StatusActions.GetActionTemplates(Convert.ToInt32(ddlStatus.SelectedValue), false);
            //if (actionSource.Any())
            //{
            //    ddActions.DataSource = actionSource;
            //    ddActions.DataBind();
            //    btnApplyAction.Enabled = true;
            //}
            //else{
            //    btnApplyAction.Enabled = false;
            //}
        }

        var compaigns = Engine.ManageCampaignActions.GetAllEvenSoftDeleted().OrderBy(x => x.Title);//.Where(x => x.IsDeleted == false);

        SalesTool.Schema.LeadsDirect leadsDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
        //accountid=600425

        ddCampaigns.DataSource = leadsDirect.GetAllCampaigns(AccountID);//Engine.ManageCampaignActions.GetAllEvenSoftDeleted().OrderBy(x => x.Title).Where(x=> x.IsDeleted == false);
        ddCampaigns.DataValueField = "Key"; // Alias Name from Sproc
        ddCampaigns.DataTextField = "Title";   // Alias name from sproc
        ddCampaigns.DataBind();
        ddCampaigns.SelectedIndex = 0;
        Session["campIden"] = ddCampaigns.SelectedValue;

        //var leads = Engine.AccountActions.GetLeads(

        BindUserDropdowns();

        bool bVisible = Request.QueryString["AccountId"] != null && Request.QueryString["AccountId"] == "-1";
        ddlIndividual.Visible = !bVisible;
        lblIndividual.Visible = !bVisible;

        //ClearControls();
        //BindEvents();
        //EnableTabs(false);
        //YA[April 1, 2013] Used the InsuranceType setting placed in web.config

        //ArrangeTabsByApplicationMode(InsuranceType == (short)InsuranceTypeEnum.Senior);
        ArrangeTabsByApplicationMode();

        // SZ [Mar 25, 2013] added the fix for Next Account not clearign teh action history
        // Grid has to be bound on every postback. if not, it shows empty
        //BindActionHistoryGrid(AccountID);

        //Sz [Apr 18, 2013] This is the fix for session lost issue. 
        // Do not remove the following code
        PageAccountId = 0;
        BindActionHistoryGrid(AccountID);

        if (CurrentUser.Security.Account.NextAccountSettings == (int)Konstants.NextAccountSettings.Off)
        {
            btnNextLead.Visible = false;
        }
        //YA[26 Feb 2014]
        liApplicationState.Visible = Engine.ApplicationSettings.IsTermLife;
        liMiddleInitial.Visible = Engine.ApplicationSettings.IsTermLife;
        BindIndividuaDropdown();

        //Code Added by kamran... in sqah and sqs, both hide spouse information//troy asked to make this change.
        primaryIndividualLegend.InnerText = "Individual Information";
        // spouseSection.Visible = false;
        //pnlLead.Width = 918; //Previous = 839
        //grdAccountHistory.Width = 893;//Previous = 805

        //if (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout)
        //{
        //    primaryIndividualLegend.InnerText = "Individual Information";
        //    spouseSection.Visible = false;
        //    pnlLead.Width = 839;
        //    grdAccountHistory.Width = 805;
        //}
        //else
        //{
        //    primaryIndividualLegend.InnerText = "Primary Individual Information";
        //    spouseSection.Visible = true;
        //    pnlLead.Width = 548;
        //    grdAccountHistory.Width = 520;
        //}

    }
    public void BindIndividuaDropdown()
    {
        // SZ [may 14, 2014] why the code below? which can be written in an elegant way
        //if (Request.QueryString["AccountId"] != null)
        //{
        //    if (Request.QueryString["AccountId"] == "-1")
        //    {
        //        ddlIndividual.Visible = false;
        //        lblIndividual.Visible = false;
        //    }
        //}
        //else
        //{
        //    ddlIndividual.Visible = true;
        //    lblIndividual.Visible = true;
        //}

        //bool bVisible = Request.QueryString["AccountId"] != null && Request.QueryString["AccountId"] == "-1";
        //ddlIndividual.Visible = !bVisible;
        //lblIndividual.Visible = !bVisible;

        //if (AccountID > 0)
        //{
        // SZ [may 14, 2014] Repetition of lines at 1040 - 1052
        //ddlIndividual.Visible = true;
        //lblIndividual.Visible = true;



        //SZ [May 20, 2014] below is the extremely convoluted and cyptic code. Commented out lines serve the same purpose 
        // as newly added code.
        //DISABLED : 
        //var indColl = ((IIndividual)Page).Individuals;
        //var indCollection = indColl.Where(x => x.AccountId == AccountID).FirstOrDefault();

        // MH:22 May 2014; dev has not noticed that ddlIndividual dropdown was bound with AccountId previously that was 
        //leading to major issue causing new individual entry on save button.
        string currentIndv = ddlIndividual.SelectedValue;
        if (!Engine.ApplicationSettings.IsTermLife)
            ddlIndividual.DataValueField = "Id";
        else
        {
            ddlIndividual.DataValueField = "WithAccountAssociation";
            ddlIndividual.DataTextField = "FullNameWithAccountID";
        }
        //if (indCollection != null)
        //{
        
        var indvCol=(this as IIndividual).Individuals.OrderBy(p => p.FullName).ToList();
        if (!indvCol.Any())
        {
            ddlIndividual.ClearSelection();
        }
        ddlIndividual.DataSource = indvCol;
        ddlIndividual.DataBind();
        
        //DISABLED : 
        //ddlIndividual.SelectedValue = indCollection.AccountId.ToString();
        if (Engine.ApplicationSettings.IsTermLife)
        {
            //tmp = AccountID.ToString();
            //MH:22 May 2014; in life mode show account's first individuals, note in sql mode ddlIndividual dropdown is 
            // bound to comman separated account,indv key
            string s = AccountID.ToString();
            ListItem item = ddlIndividual.Items.Cast<ListItem>().FirstOrDefault(p => p.Value.Contains(s));
            if (item != null)
            {
                ddlIndividual.SelectedValue = item.Value;
            }
        }
        else
        {
            //MH:22 May 2014 by default show current accounts primary individual, otherwise show edited
            ddlIndividual.SetSelectedValue(string.IsNullOrEmpty(currentIndv)
                ? (CurrentAccount != null) ? CurrentAccount.PrimaryIndividualId.ToString() : ""
                                               : currentIndv);
        }

        //}
        //}
        //else
        //{
        //    ddlIndividual.Visible = false;
        //    lblIndividual.Visible = false;
        //}
    }
    protected void Page_PreRender(object sender, EventArgs e)
    {
        //YA[7 March, 2014]
        if (IsIndividualEditMode)
        {
            IndvInfo1.Edit(EditIndividualID);
        }
        if (IsLeadMarketingEditMode)
        {
            tlkLeadsTabs.Tabs[6].Selected = true;
            tlkLeadsTabs.Tabs[6].PageView.Visible = true;
            tlkLeadsPages.SelectedIndex = 6;
            LeadsMarketing1.EditRecord(EditLeadMarketingID);
        }
        // SZ [May 20, 2014] this line below causes severe issues in Senior and Autohome mode. correct code is given below
        //if (Request.QueryString["accountId"] != null) //Kamran
        //MH:22 May 2014
        //if (Engine.ApplicationSettings.IsTermLife && Request.QueryString["accountId"] != null) //Kamran
        //    ddlIndividual.SetSelectedValue(AccountID.ToString());
        //{
        //    if (ddlIndividual.Items.Count > 0)
        //        ddlIndividual.SelectedValue = Request.QueryString["accountId"].ToString();
        //}
        //YA[09 May 2014] Troy is having problem with this popup as he need to call during this process, will check later on this code for what purpose it was written.
        //if (Engine.ApplicationSettings.IsMultipleAccountsAllowed && CheckNewForSplit)
        //    IndvInfo1.ShowEditWindow();
    }

    bool CheckNewForSplit
    {
        get
        {
            var actId = Request.ReadQueryStringAs<long?>(Konstants.K_ACCOUNT_ID);
            var parentActId = Request.ReadQueryStringAs<long?>(Konstants.K_PARENT_ACCOUNT_ID);
            var avoidreassignment = Request.ReadQueryStringAs<bool>(Konstants.K_AVOID_REASSIGNMENT);
            return actId.HasValue && actId.Value < 0 && parentActId.HasValue && !avoidreassignment;
        }
    }


    private void BindUserDropdowns(long accId = 0)
    {
        var unassigned = new ListItem("--Unassigned--", "-1");
        //var leads = Engine.AccountActions.GetLeads(
        var assignedUsers = Engine.UserActions.GetAssigned(accId > 0, accId).OrderBy(x => x.FirstName).ToList();
        ddUsers.Items.Clear();
        ddUsers.DataSource = assignedUsers;
        ddUsers.DataBind();
        ddUsers.ClearSelection();
        ddUsers.Items.Insert(0, unassigned);
        //SR[4.11.2014]
        unassigned = new ListItem("--Unassigned--", "-1");
        ddOU.Items.Clear();
        ddOU.DataSource = assignedUsers;
        ddOU.DataBind();
        ddOU.ClearSelection();
        ddOU.Items.Insert(0, unassigned);

        //[MH:08- Jan -2014] dropdown multiple selection error fix
        unassigned = new ListItem("--Unassigned--", "-1");
        //IH 01-11-13 - sort ddCSR by FirstName asc
        ddCSR.Items.Clear();
        ddCSR.DataSource = Engine.UserActions.GetCSR(accId > 0, accId).OrderBy(x => x.FirstName).ToList();
        ddCSR.DataBind();
        ddCSR.ClearSelection();
        ddCSR.Items.Insert(0, unassigned);

        //[MH:08- Jan -2014] dropdown multiple selection error fix
        unassigned = new ListItem("--Unassigned--", "-1");
        //IH 01-11-13 - sort ddTA by FirstName asc
        ddTA.Items.Clear();
        ddTA.DataSource = Engine.UserActions.GetTA(accId > 0, accId).OrderBy(x => x.FirstName).ToList();
        ddTA.DataBind();
        ddTA.ClearSelection();
        ddTA.Items.Insert(0, unassigned);

        //SZ [MAR 14, 2014] as per new requirements
        //ddExAgent.DataSource = Engine.UserActions.GetExternalAgents(accId).OrderBy(x => x.FirstName).ToList();
        //ddExAgent.DataBind();

        //SZ [Mar 25, 2014] for addign null items
        Action<DropDownList, IQueryable> AddItems = (d, s) => { d.DataSource = s; d.DataBind(); d.ClearSelection(); d.Items.Insert(0, new ListItem("--Unassigned--", "-1")); };
        AddItems(ddAltProduct, Engine.UserActions.GetAltProductUsers(accId).OrderBy(x => x.FirstName));
        AddItems(ddOP, Engine.UserActions.GetAltOPUsers(accId).OrderBy(x => x.FirstName));
    }

    private void NewArcCall(long accId, long leadId)
    {
        SalesTool.DataAccess.Models.Lead L = Engine.LeadsActions.Get(leadId);

        try
        {
            Arc.ArcServerClass Service = new Arc.ArcServerClass();
            Service.NewCase(L.SourceCode, "", L.Account.PrimaryIndividual.FirstName, "", L.Account.PrimaryIndividual.LastName, "", L.Account.PrimaryIndividual.Gender, USStates.Where(x => x.Id == L.Account.PrimaryIndividual.StateID).FirstOrDefault().FullName, L.Account.PrimaryIndividual.Birthday.Value.ToShortDateString());
            Master.ShowAlert(Messages.ArcCallSuccess);
        }
        catch (Exception ex)
        {
            Engine.AccountHistory.Log(accId, ex.ToString(), CurrentUser.Key);
            Master.ShowAlert(ex);
        }
    }

    //    <object id="myComComponent" name="myComComponent" classid="clsid:4794D615-BE51-4a1e-B1BA-453F6E9337C4"></object>



    //<script language="javascript" type="text/javascript">
    //           function myButton_click() {
    //               var returnCode = myComComponent.MyFirstComCommand(myArgs.value);
    //               var msg = "myComComponent.MyFirstComCommand returned " + returnCode;
    //               appendText(msg);
    //               appendText("\r\n");
    //           }

    //           function setText(txt) {
    //               myTextBox.value = txt;
    //           }

    //           function MyComComponent_onload() {
    //               setText("");
    //               myComComponent.MyFirstComCommand("Hi");
    //           }

    //           function MyComComponent_onunload() {
    //               myComComponent.Dispose();
    //           }
    //       </script>

    void ClearFields()
    {
        ClearQuickFields();

        lblAccountID.Text = "--";
        lblDateCreatedText.Text = "--";
        lblModifiedDateText.Text = "--";
        lblLastCallDate.Text = "--";

        //WM - 11.07.2013
        lblLastActionDate.Text = "--";

        //IH-09.10.12
        lblFirstAssignedUser.Text = string.Empty;

        //Sz [mar 25, 2013] Clear primary Individial and Secondry controls
        txtFName.Text = txtLName.Text = txtMiddleName.Text = txtDayTimePhNo.Text = txtEvePhNo.Text = txtCellPhNo.Text = txtZipCodePrimary.Text = string.Empty;
        diDOB.SelectedDate = null;
        diDOB2.SelectedDate = null;
        ddlGenderP.SelectedIndex = 0;
        //txtSpouseFName.Text = tbSpouseLastName.Text = string.Empty;

        //rdisecondDob.SelectedDate = null;
        //ddlGenderS.SelectedIndex = 0;
        txtNotes.Text = "";
        txtLifeInfo.Text = "";
        txtExternalAgent.Text = "";
        txtEmailIndv.Text = "";
        //txtEmailSpouse.Text = "";
        chkEmailOptOutPrimary.Checked = false;
        txtAddress1Primary.Text = "";
        txtAddress2Primary.Text = "";
        txtCityPrimary.Text = "";
        //txtAddress1Secondary.Text = "";
        //txtCitySecondary.Text = "";
        tbNotes.Text = string.Empty;

        //chkEmailOptOutSpuse.Checked = false;
        //txtAddress2Secondary.Text = "";
        //txtZipCodeSecondary.Text = "";
        //if (ddlStateSecondary.Items.Count > 0) ddlStateSecondary.SelectedIndex = 0;
        //if (ddlStatePrimary.Items.Count > 0) ddlStatePrimary.SelectedIndex = 0;
        PopulateOnboardData(null);

    }

    public object EventCalendarList2
    {
        get { return Master.EventCalendarList; }
    }

    void BindEvents()
    {


        //IH [09.10.10] - here in call log entry will not be added in db  if on lnkDayPhone click event.
        //ref: skype chat :[12:35:20 AM] John Dobrotka: i don't want an entry in call log when any of those click to dial links are clicked
        lnkDayPhone.ServerClick += (obj, args) => AddCall(txtDayTimePhNo.Text, false, CurrentUser.PhoneCompanyName == "inContact" ? true : false, CurrentUser.PhoneCompanyName == "Cisco" ? true : false, CurrentOutpulseIDDayPhone);
        //IH [09.10.10]- here in call log entry will not be added in db if on lnkDayPhone click event.
        //ref: skype chat :[12:35:20 AM] John Dobrotka: i don't want an entry in call log when any of those click to dial links are clicked
        lnkEvePhNo.ServerClick += (obj, args) => AddCall(txtEvePhNo.Text, false, CurrentUser.PhoneCompanyName == "inContact" ? true : false, CurrentUser.PhoneCompanyName == "Cisco" ? true : false, CurrentOutpulseIDEveningPhone);

        //YA[27 Feb 2014]
        lnkCellPhone.ServerClick += (obj, args) => AddCall(txtCellPhNo.Text, false, CurrentUser.PhoneCompanyName == "inContact" ? true : false, CurrentUser.PhoneCompanyName == "Cisco" ? true : false, CurrentOutpulseIDCellPhone);

        rbtnAction.CheckedChanged += (obj, args) => BindActionHistoryGrid(AccountID);
        rbtnCalls.CheckedChanged += (obj, args) => BindActionHistoryGrid(AccountID);
        rbtnLog.CheckedChanged += (obj, args) => BindActionHistoryGrid(AccountID);
        rbtnPolicyStatus.CheckedChanged += (obj, args) => BindActionHistoryGrid(AccountID);
        rbtnAll.CheckedChanged += (obj, args) => BindActionHistoryGrid(AccountID);
        //ddlSpouse.SelectedIndexChanged += (sender, args) => ShowSpouceControls(HasSpouce);
        btnNextLead.Click += (sender, args) => ShowNextAccount();
        DisplayNextAccountReportBtn();
        btnNextLeadfromReport.Click += (sender, args) => GetNextReportLead();
        //IndvInfo1.IndividualUpdated += (o, a) => { UpdateIndividuals(AccountID); };
        //IndvInfo1.SaveSplitAccount += IndvInfo1SaveSplitAccount;
        (this as IIndividual).Notify(this);

        btnSave.Click += (o, a) => InnerSave();
        btnEmailSender.Click += (o, a) => ShowEmailSender();
        IffLog = (l, p, d, t) =>
        {
            string Ans = string.Empty, a = "{0} assigned", b = "{0} unassigned", x = "x";
            string[,] TT = new string[2, 2] { { string.Empty, a }, { b, x } };
            Func<string, int>[] S2I = new Func<string, int>[2];
            S2I[0] = (arg) => string.IsNullOrEmpty(arg) ? 0 : 1;
            S2I[1] = (arg) => (arg == "-1") ? 0 : 1;
            Ans = TT[S2I[0](p), S2I[1](d)];
            Ans = (Ans == x && p != d) ? a : (Ans == x && p == d) ? string.Empty : Ans;
            if (!string.IsNullOrEmpty(Ans)) AccountLog(l, string.Format(Ans, t));
        };

        EventCalendarAddEdit1.Register(EventCalendarList2 as ICalenderNotification);
        ddlIndividual.SelectedIndexChanged += (o, a) => DisplayIndividualData();
        //SZ [Sep21, 2013] Added for the consent dialog handler
        if (this is ITCPAProvider)
        {
            ITCPAProvider _provider = (this as ITCPAProvider);
            _tcpaId = _provider.Register(this as ITCPAClient);

            if (_tcpaId != 0)
            {
                txtDayTimePhNo.TextChanged += (o, a) => _provider.InvokeTCPA(o, _tcpaId);
                txtEvePhNo.TextChanged += (o, a) => _provider.InvokeTCPA(o, _tcpaId);
                //YA[27 Feb 2014]
                txtCellPhNo.TextChanged += (o, a) => _provider.InvokeTCPA(o, _tcpaId);

                dlgAlert.AlertClosed += (o, a) => DefaultAlertBoxHandler(a.Value);
            }
            else
            {
                txtDayTimePhNo.AutoPostBack = false;
                txtEvePhNo.AutoPostBack = false;
                txtCellPhNo.AutoPostBack = false;
            }
        }
        IndividualBox1.OnClose += (o, a) => CloseIndividualBox(a.Id);

        if (Engine.ApplicationSettings.IsSenior)
            PolicyInfo1.OnNewIndividual += (o, a) => AddIndividual();
        else
            AutoHomePolicy1.OnAddIndividual += (o, a) => AddIndividual();

        //RadWindowManager1.Windows.Add((IndividualBox1 as IDialogBox).GetWindow());
        //RadWindowManager1.Windows.Add((dlgAlert as IDialogBox).GetWindow());

        //SZ [Dec 10, 2013] added for the new arc fucntionality
        if (Engine.ApplicationSettings.IsTermLife)
        {
            btnNewArcCall.Visible = true;
            //btnNewArcCall.OnClientClick = string.Format(" <span id="MainContent_lblAccountID">902075</span>alert({0}); GetIndividualInfo({0});return false;", AccountID);
            //Click += (o, a) => NewArcCall(AccountID, LeadID);
        }

        ddActions.SelectedIndexChanged += (o, a) => { IsActionProcessed = false; ManageActionChange(); };
        tmrDisableAction.Tick += (o, a) =>
        {
            //var X = Engine.LocalActions.Get(Helper.SafeConvert<int>(ddActions.SelectedValue));
            ManageActionChange(); //DisableAction(X);
        };
        if (CurrentUser.UserPermissions.FirstOrDefault().Permissions.Administration.CanManageOriginalUser)
            ddOU.Enabled = true;

        mapdpInfo1.FreshPolicyStatus += OnFreshPolicyStatus;
        dentInfo1.FreshPolicyStatus += OnFreshPolicyStatus;
        PolicyInfo1.FreshPolicyStatus += OnFreshPolicyStatus;
        AutoHomePolicy1.FreshPolicyStatus += OnFreshPolicyStatus;

        //ManageActionChange();
        //try
        //{
        //    { //SZ [Apr 28, 2014] Added by the client in the meeting that CallAttempt should be calculated on every postback.
        //        var X = GetSelectedAction();

        //        DisableNextAction(X);
        //        if (X != null && (X.IsCallAttemptRequired ?? false))
        //            CallAttemptRequired();
        //    }
        //}
        //catch { }
    }
    const string K_HAS_ACTION_WORKED = "K_HAS_ACTION_WORKED_lksdh";
    bool IsActionProcessed
    {
        get
        {
            bool bAns = false;
            try
            {
                string tmp = ViewState[K_HAS_ACTION_WORKED].ConvertOrDefault<string>();
                bAns = Helper.SafeConvert<bool>(tmp);
            }
            catch
            { }
            return bAns;
        }
        set
        {
            ViewState[K_HAS_ACTION_WORKED] = value;
        }
    }
    private void DisableAction(SalesTool.DataAccess.Models.Action act, SalesTool.DataAccess.Models.Lead lea)
    {
        //Pre conditions
        DBG.Assert(act != null, "No action could be retrieved from the database. Check the caller function");
        DBG.Assert(lea != null, "there is no lead sent, check the caller function");

        int iDuration = Engine.ApplicationSettings.ActionDisabledSeconds > 0 ? Engine.ApplicationSettings.ActionDisabledSeconds : 10;
        bool bCondition = (act.HasDisableAction ?? false) && lea.LastActionDate.HasValue && (DateTime.Now.Subtract((lea.LastActionDate.Value)).TotalSeconds < iDuration);

        //YA[27 May 2014] This portion of code is already covered in the upper line of code.
        //int seconds = (int)DateTime.Now.Subtract(lea.LastActionDate ?? DateTime.MinValue).TotalSeconds;
        //bCondition = bCondition && (seconds < Engine.ApplicationSettings.ActionDisabledSeconds);

        btnApplyAction.Enabled = txtNotes.Enabled = !bCondition;
        tmrDisableAction.Enabled = bCondition;
        if (bCondition)
            tmrDisableAction.Interval = (
                Engine.ApplicationSettings.ActionDisabledSeconds > 0 ?
                Engine.ApplicationSettings.ActionDisabledSeconds : 20) * 1000;

        //if (X.HasDisableAction ?? false)
        //{

        //}
        //if (X.IsCallAttemptRequired ?? false)
        //{
        //    btnApplyAction.Enabled = false;
        //    tmrDisableAction.Enabled = false;
        //}

    }
    public override void PendingSave()
    {
        //btnSave.CssClass = "dirtybuttonstyle";
    }
    void CallAttemptRequired(SalesTool.DataAccess.Models.Action act, SalesTool.DataAccess.Models.Lead lea)
    {
        DBG.Assert(act != null, "No action could be retrieved from the database. Check the caller function");
        DBG.Assert(lea != null, "there is no lead sent, check the caller function");

        //bool bCondition = lea.LastCallAttemptDate != null || lea.LastCallContactDate != null;
        //DateTime dt = lea.LastCallAttemptDate!=null? lea.LastCallAttemptDate.Value: bCondition? lea.LastCallContactDate.Value:DateTime.MinValue;
        //var diff = DateTime.Now.Subtract(dt).TotalSeconds;
        //bCondition &= act.IsCallAttemptRequired ?? false && (diff < (double)Engine.ApplicationSettings.CallAttemptRequiredSeconds);
        //btnApplyAction.Enabled = bCondition;
        //tmrDisableAction.Enabled = bCondition;
        //tmrDisableAction.Interval = Engine.ApplicationSettings.CallAttemptRequiredSeconds>0?
        //   Engine.ApplicationSettings.CallAttemptRequiredSeconds * 1000 :
        //   5000;
        bool bCallAttempt = act.IsCallAttemptRequired ?? false;
        bool bApplyActionBtn = btnApplyAction.Enabled;
        if (bCallAttempt && bApplyActionBtn)
        {
            DateTime dx = DateTime.Now;
            DateTime dt;
            if (lea.LastCallAttemptDate == null && lea.LastCallContactDate == null)
                dt = dx;
            else if (lea.LastCallAttemptDate != null && lea.LastCallContactDate == null)
                dt = lea.LastCallAttemptDate.Value;
            else if (lea.LastCallAttemptDate == null && lea.LastCallContactDate != null)
                dt = lea.LastCallContactDate.Value;
            else
                dt = lea.LastCallAttemptDate > lea.LastCallContactDate ? lea.LastCallAttemptDate.Value : lea.LastCallContactDate.Value;

            var diff = dx.Subtract(dt).TotalSeconds;
            if ((diff < (double)Engine.ApplicationSettings.CallAttemptRequiredSeconds) && (diff > 0))
            {
                int tick = (int)Math.Abs(Engine.ApplicationSettings.CallAttemptRequiredSeconds - diff) * 1000;
                btnApplyAction.Enabled = txtNotes.Enabled = true;
                tmrDisableAction.Enabled = true;
                tmrDisableAction.Interval = tick > 0 ? tick : 5000;
            }
            else
                btnApplyAction.Enabled = txtNotes.Enabled = false;
        }

    }

    private void ManageActionChange()
    {
        var X = GetSelectedAction();
        var L = Engine.LeadsActions.GetPrimaryLeadByAccountId(AccountID);

        if (X != null && L != null)
        {
            DisableAction(X, L);
            CallAttemptRequired(X, L);
        }

        //if (X != null && (X.IsCallAttemptRequired ?? false))
        //    CallAttemptRequired();
        //{
        //    int diff = -1;
        //    var lea = Engine.LeadsActions.Get(LeadID);


        //    DateTime? dtCallAttempt = lea.LastCallAttemptDate;
        //    DateTime? dtCallContact = lea.LastCallDate;

        //    if (dtCallAttempt != null)
        //        diff = DateTime.Now.Subtract(dtCallContact.Value).Seconds;
        //    else if (dtCallContact != null)
        //        diff = DateTime.Now.Subtract(dtCallAttempt.Value).Seconds;

        //    if (diff < Engine.ApplicationSettings.CallAttemptRequiredSeconds)
        //        btnApplyAction.Enabled = true;
        //    else
        //        btnApplyAction.Enabled = false;
        //}
    }

    //private void DisableNextAction(SalesTool.DataAccess.Models.Action X)
    //{
    //    DateTime? dt = Engine.LeadsActions.GetPrimaryLeadByAccountId(AccountID).LastActionDate;

    //    if (X != null && (X.HasDisableAction ?? false))
    //    {
    //        if (dt == null) // SZ [Apr 30, 2014] if no last action date, enable button
    //            btnApplyAction.Enabled = true;
    //        else
    //        {
    //            btnApplyAction.Enabled = false;
    //            tmrDisableAction.Enabled = true;
    //            tmrDisableAction.Interval = Engine.ApplicationSettings.ActionDisabledSeconds > 0 ?
    //                Engine.ApplicationSettings.ActionDisabledSeconds * 1000 :
    //                5000;
    //        }
    //    }
    //    else
    //    {
    //        btnApplyAction.Enabled = true;
    //        tmrDisableAction.Enabled = false;
    //    }
    //}

    private SalesTool.DataAccess.Models.Action GetSelectedAction()
    {
        return Engine.LocalActions.Get(Helper.SafeConvert<int>(ddActions.SelectedValue));
    }

    private void AddIndividual()
    {
        IndividualBox1.Show(null);
    }

    private void CloseIndividualBox(long personId)
    {
        //if(ApplicationSettings.IsAutoHome)
        //    AutoHomePolicy1.
        //else
        //    PolicyInfo1 
    }



    void BindActionHistoryGrid(long accID = -1)
    {
        ActionHistoryType type =
            rbtnAction.Checked ? ActionHistoryType.Actions :
            rbtnCalls.Checked ? ActionHistoryType.Calls :
            rbtnPolicyStatus.Checked ? ActionHistoryType.PolicyStatus :
            rbtnLog.Checked ? ActionHistoryType.Log :
            ActionHistoryType.All;

        grdAccountHistory.Columns[4].Visible = !rbtnCalls.Checked;
        grdAccountHistory.Columns[5].Visible = !rbtnCalls.Checked;

        grdAccountHistory.Columns[6].Visible = rbtnCalls.Checked;
        grdAccountHistory.Columns[7].Visible = rbtnCalls.Checked;
        grdAccountHistory.Columns[8].Visible = rbtnCalls.Checked || rbtnAction.Checked;

        List<AccountHistoryEntry> lstAcctHistoryEntry = new List<AccountHistoryEntry>();
        var result = Engine.AccountHistory.GetHistory(accID, type).ToList();

        if (type == ActionHistoryType.Actions && Session[K_CONTACT_ID] != null)
        {
            result.ForEach(delegate(AccountHistoryEntry e) { lstAcctHistoryEntry.Add(new AccountHistoryEntry() { ContactId = Session[K_CONTACT_ID].ToString(), AccountId = e.AccountId, AddedOn = e.AddedOn, Comment = e.Comment, DNIS = e.DNIS, FullName = e.FullName, Key = e.Key, TalkTime = e.TalkTime, UserKey = e.UserKey, UserTypes = e.UserTypes, EntityKey = e.EntityKey, Entry = e.Entry, EntryType = e.EntryType, PVTitle = e.PVTitle }); });
            grdAccountHistory.DataSource = lstAcctHistoryEntry;
            grdAccountHistory.DataBind();
        }
        else
        {
            grdAccountHistory.DataSource = result;
            grdAccountHistory.DataBind();
        }
    }

    void InnerLoad(long accID = 0, bool IsSavedClick = false)
    {
        //CurrentAccount = Engine.AccountActions.Get(AccountID);
        bool hasArcCase = Engine.AccountActions.HasArcCase(accID);
        if (Engine.ApplicationSettings.IsTermLife)
        {
            btnNewArcCall.Visible = !hasArcCase;
            btnOpenCaseCall.Visible = hasArcCase;
        }
        else
        {
            btnNewArcCall.Visible = false;
            btnOpenCaseCall.Visible = false;
        }
        var latestArcCases = Engine.ArcActions.GetLatestArcCase(accID).FirstOrDefault();
        if (latestArcCases != null)
        {
            CurrentArcReferenceID = latestArcCases.ArcRefreanceKey;
        }
        UseArcNewImplementation = Engine.ApplicationSettings.USE_ARC_NEW_IMPLEMENTATION;
        
        ArcHubPath = Engine.ApplicationSettings.ARC_HUB_PATH;
        
        Account A = Engine.AccountActions.Get(accID);
        CurrentAccount = A;
        //Sz [Apr 18, 2013] This is the fix for session lost issue. 
        // Do not remove the following code
        btnEmailSender.Visible = Engine.ManageEmailTemplateActions.GetAllIQueryable().Where(x => x.Enabled ?? false).Count() > 0;

        EnableControlsBasedOnPermissions(); //SZ [Sep 13, 2013] Moved from Page_Initialize() to here as this is appropiate place
        PageAccountId = accID == 0 ? -1 : accID; //SZ [Apr 18, 2013] This is the fix for session lost
        BindActionHistoryGrid(accID);

        //PageAccountId = accID;

        // Attiq-April-09-2014
        // if Lead opening from Viweleads page Initiated from Screen Pop.
        // then set the current User as Account Assign User.

        if (Request.QueryString["by"] == "3" && A.AssignedUserKey == null)
        {
            A.AssignedUserKey = CurrentUser.Key;
            AccountLog(A.Key, "User assigned");
            AccountLogAssignment(A.Key, "User assigned", " from Inner safe when querystring has by=3 and AssignedUserKey is null");
            Engine.AccountActions.Update(A, CurrentUser.FullName);
        }

        if (A == null)
        {
            Redirect(Konstants.K_VIEW_LEADS_PAGE);
            return;
        }

        // SZ [Dec 19, 2013] since the account may contain a deleted user hence we need to rebind the user dropdowns
        // like this => (All users not deleted)+ (any assigned user of this account even if deleted) 
        // The commented out code was added by Muzamil H and is not longer needed as BindUserDropdowns() takes care of --Unassigned--(-1) 
        BindUserDropdowns(A.Key);
        //Helper.ddUsers.SelectedValue = A.AssignedUserKey.HasValue? A.AssignedUserKey.Value.ToString(): "-1";
        //YA[29 Jan, 2014]
        Individual IPrimary = Engine.IndividualsActions.Get(A.PrimaryIndividualId ?? 0);

        if (!IsIndividualAddNewAccount && IPrimary != null)
            ReadOnlyFields();

        if (A.AssignedUserKey.HasValue)
        {
            ddUsers.SelectedValue = A.AssignedUserKey.Value.ToString();
            // ddOU.SelectedValue = A.AssignedUserKey.Value.ToString();
        }

        if (A.OriginalUserKey.HasValue)
            ddOU.SelectedValue = A.OriginalUserKey.Value.ToString();

        if (A.AssignedCsrKey.HasValue)
            ddCSR.SelectedValue = A.AssignedCsrKey.Value.ToString();

        if (A.TransferUserKey.HasValue)
            ddTA.SelectedValue = A.TransferUserKey.Value.ToString();

        //SZ [Mar 14, 2014]
        if (A.AlternateProductUser.HasValue)
            //MH:May 19, 2014 Fix: invalid selected value ddAltProduct
            ddAltProduct.SetSelectedValue(A.AlternateProductUser.Value.ToString());

        if (A.OnBoardUser.HasValue)
            ddOP.SetSelectedValue(A.OnBoardUser.Value.ToString());

        //else 
        //    ddUsers.SelectedValue ="-1";

        //else
        //{
        //ListItem itemExist1 = ddUsers.Items.FindByText("--Unassigned--");
        //if (itemExist1 == null)
        //{
        //    ddUsers.Items.Insert(0, new ListItem("--Unassigned--", "-1"));
        //    ddUsers.SelectedValue = "-1";
        //}
        //else
        //{
        //ddCSR.SelectedValue = "-1";
        //}
        //}

        //if (A.AssignedCsrKey != null)
        //{
        //    ddCSR.SelectedValue = Convert.ToString(A.AssignedCsrKey);
        //}
        //else
        //{
        //    ListItem itemExist1 = ddCSR.Items.FindByText("--Unassigned--");
        //    if (itemExist1 == null)
        //    {
        //        ddCSR.Items.Insert(0, new ListItem("--Unassigned--", "-1"));
        //        ddCSR.SelectedValue = "-1";
        //    }
        //    else
        //    {
        //        ddCSR.SelectedValue = "-1";
        //    }
        //}

        //QN [April 1st, 2013]
        //This code is to popluate the Transfer Agent/User...
        //... dropdownlist from database.

        //MH.[01 Nov 2013] functionality is centralized  

        //Helper.SafeAssignSelectedValueToDropDown(ddTA, Convert.ToString(A.TransferUserKey));

        //if (A.TransferUserKey != null)
        //{
        //   Helper.SafeAssignSelectedValueToDropDown(ddTA, Convert.ToString(A.TransferUserKey));
        // //   ddTA.SelectedValue = Convert.ToString(A.TransferUserKey);
        //}
        //else
        //{
        //    ListItem itemExist1 = ddTA.Items.FindByText("--Unassigned--");
        //    if (itemExist1 == null)
        //    {
        //        ddTA.Items.Insert(0, new ListItem("--Unassigned--", "-1"));
        //        ddTA.SelectedValue = "-1";
        //    }
        //    else
        //    {
        //        ddTA.SelectedValue = "-1";
        //    }
        //}

        //[QN, 15-04-2013] Updated due to below request of client. 
        //[4/13/2013 2:34:37 AM] All dates need to be in MM/DD/YYYY format at all times going forward
        //John Dobrotka: we need to update these on the leads.aspx header
        //lblDateCreatedText.Text = DateTime.Parse(Convert.ToString(A.AddedOn.HasValue ? A.AddedOn.Value.ToShortDateString() : "")).ToString("MM/dd/yyyy") + " " + DateTime.Parse(Convert.ToString(A.AddedOn.HasValue ? A.AddedOn.Value.ToShortTimeString() : "")).ToString("hh:mm");
        //lblModifiedDateText.Text = DateTime.Parse(Convert.ToString(A.ChangedOn.HasValue ? A.ChangedOn.Value.ToLongDateString() : "")).ToString("MM/dd/yyyy") + " " + DateTime.Parse(Convert.ToString(A.Chang.edOn.HasValue ? A.ChangedOn.Value.ToShortTimeString() : "")).ToString("hh:mm");

        //YA[May 02, 2013] Changed the above sited code in the simple form
        lblDateCreatedText.Text = A.AddedOn.HasValue ? A.AddedOn.Value.ToString("MM/dd/yyyy hh:mm") : "";
        lblModifiedDateText.Text = A.ChangedOn.HasValue ? A.ChangedOn.Value.ToString("MM/dd/yyyy hh:mm") : "";

        //wm
        txtExternalAgent.Text = A.ExternalAgent ?? string.Empty;
        //YA[13 March, 2014] In new layout mode where related accounts are there
        //Notes field will be only save in parent account.
        if (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout && A.AccountParent != null)
        {
            Account parentAccount = Engine.AccountActions.Get(A.AccountParent.HasValue ? A.AccountParent.Value : -1);
            if (parentAccount != null) txtNotes.Text = parentAccount.Notes;
            else txtNotes.Text = A.Notes;
        }
        else
            txtNotes.Text = A.Notes;
        txtLifeInfo.Text = A.LifeInfo;

        //orderid = (orderid >= Engine.AccountActions.GetLeads(accID).Count()) ? 0 : orderid;
        //LeadIndex = orderid;

        lblAccountID.Text = accID > 0 ? accID.ToString() : "NIL";
        accountIden.Value = lblAccountID.Text;

        //Lead lea = Engine.AccountActions.GetLeadAt(accID, orderid);

        //YA[April 17, 2013] Commented this call, as account object is already there at the start this function call.
        //var account = Engine.AccountActions.Get(this.AccountID);

        //Lead lea = A.PrimaryLeadKey.HasValue ? A.Leads.Where(x => x.Key == A.PrimaryLeadKey).FirstOrDefault() :
        //    A.Leads.FirstOrDefault(); //Engine.AccountActions.GetLeadByPrimary(A.PrimaryLeadKey ?? -1).FirstOrDefault();
        Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) :
           A.Leads.FirstOrDefault();
        if (L != null)
        {
            LeadID = L.Key;
            if (L.CampaignId.HasValue)
            {
                ddCampaigns.SelectedValue = L.CampaignId.ToString();
                CampaignID = L.CampaignId.Value;
                if (!IsPostBack)
                {
                    lblCampaignName.Text = ddCampaigns.SelectedItem != null ? ddCampaigns.SelectedItem.Text : "";
                    if (!IsParentPopupClose)
                    {
                        ctlPagerCampaignAlerts.Initialize(true);
                        int statusID = L.StatusId.HasValue ? L.StatusId.Value : 0;
                        PrimaryStatusID = statusID;
                        if (ShowCampaignAlerts(CampaignID.ToString(), PrimaryStatusID))
                            dlgCampaignAlert.VisibleOnPageLoad = true;
                    }
                }
            }
            if (L.StatusId.HasValue)
            {
                if (ddlStatus.Items.FindByValue(L.StatusId.ToString()) != null)
                    ddlStatus.SelectedValue = L.StatusId.ToString();
            }

            if (ddlStatus.Items.Count > 0)
            {
                int statusID = L.StatusId ?? 0;
                // MH [Nov 1, 2013] Function to handle the apply action button if no actions 
                BindAvailableActions(statusID);

                //MH:19 Sep 2014: Commented on Troy's request. "when you edit a record the dropdown list should show in blank position."
                //Helper.SafeAssignSelectedValueToDropDown(ddActions, Convert.ToString(L.ActionId));

                //if (lea.ActionId.HasValue)
                //    ddActions.SelectedValue = lea.ActionId.ToString();
                if (statusID > 0)
                {
                    //MH: 03 May 2014   Exception message: 'ddlSubStatus1' has a SelectedValue which is invalid because it does not exist in the list of items.
                    ddlSubStatus1.SafeBind(Engine.StatusActions.GetSubStatuses(statusID, false));
                    //ddlSubStatus1.SelectedValue = null;
                    //ddlSubStatus1.DataSource = Engine.StatusActions.GetSubStatuses(statusID, false);
                    //ddlSubStatus1.DataBind();
                }
                if (L.SubStatusId != null)
                    ddlSubStatus1.SetSelectedValue(L.SubStatusId.ToString());
            }

            lblLastCallDate.Text = L.LastCallDate.HasValue ? L.LastCallDate.Value.ToString("MM/dd/yyyy hh:mm") : "";

            //WM - 11.07.2013
            lblLastActionDate.Text = L.LastActionDate.HasValue ? L.LastActionDate.Value.ToString("MM/dd/yyyy hh:mm") : "";

            //IH-09.10.12
            if (L.Account.AssignedUserKey != null)
                lblFirstAssignedUser.Text = Engine.UserActions.GetAll(true).Where(x => x.Key == L.Account.AssignedUserKey.Value).FirstOrDefault().FullName;

            UpdateQuickFields(A);

            //SZ [Apr 28, 2014] Added by the client in the meeting that CallAttempt should be calculated on every postback.
            ManageActionChange();
        }
        DialPhone();
        BindActionHistoryGrid(accID);
        BindIndividuaDropdown();
        EnableTabs();
        //YA[April 5, 2013] Checks for Reassign user popup for taking the Owership

        if (CurrentUser.Security.Account.LeadAccess == 0) // 0 is readonly
        {
            EnableEditing(false);
        }
        else if (!IsSavedClick && !DoAvoidReassignment && !IsParentPopupClose)
            TakeOwnership(A);

        UIColorTheBar();

        //TM [08 09 2014] Added on Task request
        if (Engine.ApplicationSettings.IsTermLife)
        {
            btnEmailSender.Visible = false;
        }
    }

    private void ReadOnlyFields()
    {
        //Helper.ddUsers.SelectedValue = A.AssignedUserKey.HasValue? A.AssignedUserKey.Value.ToString(): "-1";
        //YA[29 Jan, 2014]
        txtEmailIndv.Enabled = !Engine.ApplicationSettings.IsTermLife;
        txtAddress1Primary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        txtAddress2Primary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        //txtAddress1Secondary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        //txtAddress2Secondary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        txtCityPrimary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        //txtCitySecondary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        //txtEmailSpouse.Enabled = !Engine.ApplicationSettings.IsTermLife;
        txtZipCodePrimary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        //txtZipCodeSecondary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        ddlStatePrimary.Enabled = !Engine.ApplicationSettings.IsTermLife;
        //ddlStateSecondary.Enabled = !Engine.ApplicationSettings.IsTermLife;
    }

    private void BindAvailableActions(int statusID)
    {
        // MH [Nov 1, 2013] Disable Apply Button if no action 
        ddActions.Items.Clear();
        var actionSource = Engine.StatusActions.GetActionTemplates(statusID, false).Select(s => new NameIntValueLookup() { Value = s.Id, Name = s.Title }).ToList();
        if (actionSource.Any())
        {
            actionSource.Insert(0, new NameIntValueLookup() { Name = "", Value = null });
            //MH:03 May 2014   Exception message: 'ddActions' has a SelectedValue which is invalid because it does not exist in the list of items.
            ddActions.SafeBind(actionSource);
            //ddActions.SelectedValue = null;
            //ddActions.DataSource = actionSource;
            //ddActions.DataBind();

            //YA[27 May 2014] To remove the bug of apply action DisableNextAction feature.
            //btnApplyAction.Enabled = true;
            ddActions.Enabled = true;
        }
        else
        {
            ddActions.Enabled = false;
            btnApplyAction.Enabled = txtNotes.Enabled = false;
        }
    }

    private bool ShowCampaignAlerts(string campaignID, int statusID = 0)
    {
        IQueryable<Alert> T = null;
        bool result = false;
        //T = Engine.ManageAlertsActions.GetAllCampaignAlertsByCampaignId(campaignID);
        T = Engine.ManageAlertsActions.GetAllCampaignAlertsByConditions(campaignID, statusID);

        if (T != null)
        {
            var U = T.Select(x => new { x.Id, x.Name, x.Message, x.DetailMessage, AlertType = x.AlertType != null ? x.AlertType.Name : "" }).ToList();
            ctlPagerCampaignAlerts.ApplyPagingWithRecordCount(U.Count);

            frmViewCampaignAlert.DataSource = ctlPagerCampaignAlerts.ApplyPaging(U.AsEnumerable());
            frmViewCampaignAlert.DataBind();

            ctlPagerCampaignAlerts.SetRecordLabel("Alert");
            Label nType = (Label)frmViewCampaignAlert.FindControl("lblAlertType");
            if (nType != null)
                dlgCampaignAlert.Title = nType.Text;
            result = U.Count > 0;
        }
        return result;
    }

    private void TakeOwnership(Account A)
    {
        var user = this.CurrentUser;

        if (user != null)
        {
            var userPermission = user.UserPermissions.FirstOrDefault();

            if (userPermission.Permissions.Account != null)
            {
                var accountPermission = userPermission.Permissions.Account;
                if (accountPermission.ChangeOwnerShip)
                {
                    if (A.AssignedUserKey == null)
                    {
                        dlgReassignUser.VisibleOnPageLoad = OwnershipContinueShowOnership;
                        lblMessageReassignUsers.Text = "This account is not currently assigned, would you like to take ownership of this account?";
                    }
                    else if (!CurrentUser.Key.Equals(A.AssignedUserKey))
                    {
                        dlgReassignUser.VisibleOnPageLoad = OwnershipContinueShowOnership;
                        lblMessageReassignUsers.Text = "This account is assigned to " + A.User.FirstName + " " + A.User.LastName + ", would you like to take ownership of this account?";
                    }
                }
            }
        }
    }

    //YA[April 5, 2013] Event for taking the ownership
    protected void btnTakerOwnership_Click(object sender, EventArgs e)
    {
        //Engine.MarkArcUserAssignmentDeliveryAs = Engine.ArcActions.IsUserAssignedBefore(AccountID);
        //Take Ownership of the selected account
        var account = Engine.AccountActions.Get(AccountID);
        if (account.AssignedUserKey != CurrentUser.Key)
        {
            account.AssignedUserKey = CurrentUser.Key;
            //[MH:06-Jan-2014]
            // log agent assignment
            AccountLog(account.Key, "User assigned");
            AccountLogAssignment(account.Key, "User assigned", " from takeownership ");
        }


        //SR 4/11/2014 Add Original User to Account.
        if (account.OriginalUserKey == null)
        {
            account.OriginalUserKey = CurrentUser.Key;
            AccountLogAssignment(account.Key, "Orignal User assigned", " from takeownership click when it was null");
        }

        Engine.AccountActions.Update(account, CurrentUser.FullName);
        dlgReassignUser.VisibleOnPageLoad = false;
        //MH:02 June 2014 to avoid post back warring on page refresh redirect is added
        //InnerLoad(AccountID);
        Response.Redirect(Request.Url.ToString());
    }

    //YA[April 5, 2013] Event for ignoring the ownership take
    protected void btnContinue_Click(object sender, EventArgs e)
    {
        //MH:02 June 2014 to avoid post back warring on page refresh redirect is added
        // dlgReassignUser.VisibleOnPageLoad = false;
        OwnershipContinueShowOnership = false;
        Response.Redirect(Request.Url.ToString());
    }
    //MH: 02 June 2014
    public bool OwnershipContinueShowOnership
    {
        get
        {
            bool status = true;
            if (Session["OwnershipContinue"] != null)
            {
                status = Session["OwnershipContinue"].ConvertOrDefault<bool>();
                Session.Remove("OwnershipContinue");
            }
            return status;
        }
        set { Session["OwnershipContinue"] = value; }
    }
    /// <summary>
    /// YA[July 24, 2013] Closes the campaign alert dialog 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCloseCampaignAlert_Click(object sender, EventArgs e)
    {
        dlgCampaignAlert.VisibleOnPageLoad = false;
    }

    protected void EvtCampaignAlert_PageNumberChanged(object sender, EventArgs e)
    {
        ShowCampaignAlerts(CampaignID.ToString(), PrimaryStatusID);
    }

    protected void btnReadMore_Click(object sender, EventArgs e)
    {
        Label ndetailMsg = (Label)frmViewCampaignAlert.FindControl("lblInnerDetailMessage");
        if (ndetailMsg != null)
            ndetailMsg.Visible = !ndetailMsg.Visible;
        Label ndetail = (Label)frmViewCampaignAlert.FindControl("lblDetail");
        if (ndetail != null)
            ndetail.Visible = !ndetail.Visible;
        //btnShowMoreDetail
        Button nShowDetail = (Button)frmViewCampaignAlert.FindControl("btnShowMoreDetail");
        if (nShowDetail != null)
            nShowDetail.Text = ndetail != null && ndetail.Visible ? "Hide Detail" : "Read More";
    }

    protected void OnFreshPolicyStatus(object sender, EventArgs e)
    {
        BindActionHistoryGrid(AccountID);
    }

    // WM - 31 May, 2013
    public void ClickedToDial(object sender, DialClickedEventArgs e)
    {
        if (!string.IsNullOrEmpty(e.Message)) Master.ShowAlert(e.Message, "inContact Dial");
        this.AddCall(e.PhoneNumber);
    }

    public void AddCall(string phoneNumber, bool isAllowCallLogEntry = true, bool callInContact = false, bool callCisco = false, string outPulseID = "")
    {
        //IH 10.10.13- parmater isAllowCallLogEntry set value false from lnkEvePhNo and lnkDayPhone click event

        // SZ [Sep 21, 2013] Performed Code refactoring in this fuction
        if ((phoneNumber.Trim().Length < 10) || (AccountID <= 0))
            return;
        //YA[29 April 2014] Any click to dial will log an entry as Call Attempt   
        Engine.AccountHistory.LogCall(AccountID, phoneNumber, CurrentUser.Key);
        //SZ [Apr 18, 2014] Added for updating the fields of call attempt
        //var L = Engine.LeadsActions.Get(LeadID);
        //var act = Engine.LocalActions.Get(Helper.SafeConvert<int>(ddActions.SelectedValue));
        UpdateLeadCallInformation(AccountID, CurrentUser.Key);

        //YA[27 May 2014] Need to go through both the function DisableNextAction and CallAttemptRequired.
        ManageActionChange();
        //CallAttemptRequired(act, L);

        if (callInContact)
        {
            //InContactCall(phoneNumber, outPulseID);
            var sys = new PhoneSystem(Engine, CurrentUser);
            bool status = sys.InContactCall(AccountID, phoneNumber, outPulseID, Master.ShowAlert);
        }
        else if (callCisco)
        {
            var sys = new PhoneSystem(Engine, CurrentUser);
            bool status = sys.CiscoCall(AccountID, phoneNumber, outPulseID, Master.ShowAlert);
        }
        else
        {
            //SZ [Apr 18, 2014] Added for updating the fields of call contact
            UpdateLeadCallInformation(AccountID, CurrentUser.Key, false);
            BindActionHistoryGrid(AccountID);
        }

        //IH 09.10.13  commited the  AccountHistory.AddCall code because 
        //ref: skype chat :[12:35:20 AM] John Dobrotka: i don't want an entry in call log when any of those click to dial links are clicked
        if (isAllowCallLogEntry)
        {
            Engine.AccountHistory.AddCall(AccountID, phoneNumber, CurrentUser.Key);
        }
        if (LeadID > 0)
        {
            var lead = Engine.LeadsActions.Get(LeadID);

            if (lead != null)
            {
                DateTime currentDateTime = DateTime.Now;
                lead.LastCallDate = currentDateTime;

                Engine.LeadsActions.Update(lead);
                lblLastCallDate.Text = currentDateTime.ToString("MM/dd/yyyy hh:mm");
            }
        }
    }


    private void UpdateLeadCallInformation(long accId, Guid user, bool bIsCallAttempt = true)
    {
        Account X = Engine.AccountActions.Get(accId);
        Lead L = X.PrimaryLead;
        Action<Lead, Guid> dolog = null;

        if (bIsCallAttempt)
            dolog = (l, key) =>
            {
                l.LastCallAttemptDate = DateTime.Now;
                if (X.AssignedUserKey == key)
                    l.LastCallAttemptAssignedUserDate = DateTime.Now;
                if (X.AssignedCsrKey == key)
                    l.LastCallAttemptCSRUserDate = DateTime.Now;
                if (X.TransferUserKey == key)
                    l.LastCallAttemptTAUserDate = DateTime.Now;
                if (X.AlternateProductUser == key)
                    l.LastCallAttemptAPUserDate = DateTime.Now;
                if (X.OnBoardUser == key)
                    l.LastCallAttemptOBUserDate = DateTime.Now;
            };
        else
            dolog = (l, key) =>
            {
                l.LastCallContactDate = DateTime.Now;
                if (X.AssignedUserKey == key)
                    l.LastCallContactAssignedUserDate = DateTime.Now;
                if (X.AssignedCsrKey == key)
                    l.LastCallContactCSRUserDate = DateTime.Now;
                if (X.TransferUserKey == key)
                    l.LastCallContactTAUserDate = DateTime.Now;
                if (X.AlternateProductUser == key)
                    l.LastCallContactAPUserDate = DateTime.Now;
                if (X.OnBoardUser == key)
                    l.LastCallContactOBUserDate = DateTime.Now;
            };

        try
        {
            dolog(L, user);
            Engine.LeadsActions.Update(L);
        }
        catch (Exception ex)
        {
            ctlMessage.SetStatus(ex);
        }
    }

    private ASP.usercontrols_statuslabel_ascx ctlMessage
    {
        get { return Master.Message; }
    }

    Action<long, string, string, string> IffLog = null;
    public override void Validate()
    {
        base.Validate();
        //if(!IsValid)
        //    throw new Exception("Validation failed.");
        // MH:12 March 2014
        // Phone number is not a required field in SQL
        if (!Engine.ApplicationSettings.IsTermLife)
        {
            bool bResult = true;
            if (IsNewAccount)
                bResult &= string.IsNullOrWhiteSpace(txtDayTimePhNo.Text) && string.IsNullOrWhiteSpace(txtEvePhNo.Text);
            else
            {
                Individual I = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Primary);
                if (I != null)
                    // SZ [May 23, 2014] added the change cell phone is not taken into account 
                    //bResult &= string.IsNullOrWhiteSpace(txtDayTimePhNo.Text) && string.IsNullOrWhiteSpace(txtEvePhNo.Text) && string.IsNullOrWhiteSpace(I.InboundPhone);
                    bResult &= string.IsNullOrWhiteSpace(txtDayTimePhNo.Text) && string.IsNullOrWhiteSpace(txtEvePhNo.Text) && string.IsNullOrWhiteSpace(I.InboundPhone) && string.IsNullOrWhiteSpace(txtCellPhNo.Text);
            }

            if (bResult)
                throw new Exception("No phone number is specified. Please specify atleast one contact number.");
        }
    }

    // Attiq - April-08-2014. 
    // Added isCreatingNewAccount flag to send the current user as assigned user to the newly creating account.
    private bool isCreatingNewAccount = false;
    /// <summary>
    /// Select Id id from individual dropdwon in QuickSave
    /// </summary
    public long SelectedIndividualId
    {
        get
        {
            if (Engine.ApplicationSettings.IsTermLife)
            {
                string indvKey = null;
                if (!string.IsNullOrEmpty(ddlIndividual.SelectedValue))
                {
                    var splitResult = ddlIndividual.SelectedValue.Split(',');
                    if (splitResult.Count() > 1)
                        indvKey = splitResult[1];
                }
                return indvKey.ConvertOrDefault<long>();
            }
            if (!string.IsNullOrEmpty(ddlIndividual.SelectedValue))
                return ddlIndividual.SelectedValue.ConvertOrDefault<long>();
            return -1;
        }
    }
    /// <summary>
    /// Get Selected AccountId
    /// in case of sql it will return accountId portion after spliting comman
    /// other wise it will return from querystring
    /// </summary>
    public long? SelectedIndividualAccountId
    {
        get
        {
            if (Engine.ApplicationSettings.IsTermLife)
            {
                string indvKey = null;
                if (!string.IsNullOrEmpty(ddlIndividual.SelectedValue))
                {
                    indvKey = ddlIndividual.SelectedValue.Split(',')[0];
                }
                return indvKey.ConvertOrDefault<long?>();
            }
            return Request.ReadQueryStringAs<long?>(Konstants.K_ACCOUNT_ID);
        }
    }
    /// <returns>Errors Message if it has</returns>
    string InnerSave(bool bQuickPass = false, bool IsInApplyActionWorkFlow = false)
    {
        string error = "";
        try
        {
            if (!bQuickPass)
                Validate();
            bool HasNewAccountRequest = IsNewAccount;
            //MH:14 May 2014, if account is new then change agent will raise real-time updates to arc api
            //Engine.MarkArcUserAssignmentDeliveryAs = !(IsNewAccount || IsNewAccountRequest);

            Account A = IsNewAccount ? Engine.AccountActions.Add(new Account { AddedBy = CurrentUser.FullName, AddedOn = DateTime.Now }) : Engine.AccountActions.Get(AccountID);
            if ((CurrentUser.UserPermissions.FirstOrDefault().Permissions.Administration.CanManageOriginalUser ||
                A.OriginalUserKey == null) &&
                Convert.ToString(A.OriginalUserKey) != ddOU.SelectedValue)
            {
                var usr = (ddOU.SelectedValue != "-1") ? new Guid(ddOU.SelectedValue) : (Guid?)null;
                if (usr != null)
                {
                    IffLog(A.Key, Convert.ToString(A.OriginalUserKey), ddOU.SelectedValue, "Original User");
                    A.OriginalUserKey = usr;
                    AccountLogAssignment(A.Key, "Orignal User assigned", " from InnerSave ");
                }
            }

            if (ParentAccountID > 0)
                A.AccountParent = ParentAccountID;
            //SZ [Apr 18, 2013] this is the fix for session lost after save it no longer is a new account
            PageAccountId = A.Key;
            accountIden.Value = AccountID.ToString();

            if (IsNewAccount)
            {
                AccountLog(A.Key, string.Format("An account has been created. Url is {0}", Request.Url.ToString()));
                isCreatingNewAccount = true; // Attiq - April-08-2014.  
            }
            else
            {
                A.ChangedBy = CurrentUser.FullName;
                A.ChangedOn = DateTime.Now;
                AccountLog(A.Key, string.Format("An existing account is being saved. Url is {0}", Request.Url.ToString()));
            }

            lblAccountID.Text = (AccountID = A.Key).ToString();
            AssignUsers2Account(ref A);

            //YA[13 March, 2014] In new layout mode where related accounts are there
            //Notes field will be only save in parent account.
            if (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout && A.AccountParent != null)
            {
                Account parentAccount = Engine.AccountActions.Get(A.AccountParent.HasValue ? A.AccountParent.Value : -1);
                if (parentAccount != null) { parentAccount.Notes = txtNotes.Text; Engine.AccountActions.Update(parentAccount, CurrentUser.FullName); }
                else A.Notes = txtNotes.Text;
            }
            else
                A.Notes = txtNotes.Text;


            //WM - 10.06.2013
            A.ExternalAgent = txtExternalAgent.Text;
            A.LifeInfo = txtLifeInfo.Text;


            Individual p = null;
            //MH:22 May 2014; save what is in Quick field
            p = A.Individuals.FirstOrDefault(x => x.Key == SelectedIndividualId);
            GetIndividualDetails(ref p);
            if ((p.AccountId ?? 0) <= 0)
            {
                A.Individuals.Add(p);
                A.PrimaryIndividual = p;
            }



            //Lead lea = IsNewLead ? new Lead() : Engine.LeadsActions.Get(LeadID);
            Lead L;
            if (IsNewLead)
            {
                L = new Lead { AddedBy = CurrentUser.FullName, AddedOn = DateTime.Now };
                if (Engine.ApplicationSettings.IsTermLife && (IsNewAccount || IsNewAccountRequest) && L.SourceCode == null)
                    L.SourceCode = CurrentUser != null ? CurrentUser.ArcId : "";
            }
            else
            {
                L = Engine.LeadsActions.Get(LeadID);
                L.ChangedBy = CurrentUser.FullName;
                L.ChangedOn = DateTime.Now;
            }

            //SZ [Feb 21, 2014] fixed the issue of input string error
            //lea.CampaignId = Convert.ToInt32(ddCampaigns.SelectedValue);
            //lea.ActionId = Convert.ToInt32(ddActions.SelectedValue);
            //lea.StatusId = Convert.ToInt32(ddlStatus.SelectedValue);
            L.CampaignId = Helper.NullConvert<int>(ddCampaigns.SelectedValue);
            L.StatusId = Helper.NullConvert<int>(ddlStatus.SelectedValue);
            L.SubStatusId = Helper.NullConvert<int>(ddlSubStatus1.SelectedValue);

            // SZ [Mar 25, 2014] added according to the new requirements.
            // for details see the comments in StoreLastActionDetails() 
            //{
            //    int? aid = L.ActionId;
            //    int? uid = Helper.NullConvert<int>(ddActions.SelectedValue);
            //    if (aid != uid)
            //}
            //L.ActionId = Helper.NullConvert<int>(ddActions.SelectedValue);


            //int substatusID = default(int);
            //if (int.TryParse(ddlSubStatus1.SelectedValue, out substatusID))
            //    lea.SubStatusId = substatusID;

            if (IsNewLead)
            {
                L.AccountId = AccountID;
                LeadID = Engine.LeadsActions.Add(L).Key;
                A.PrimaryLeadKey = LeadID;
                //Engine.AccountActions.Update(A);
            }
            //else
            //    Engine.LeadsActions.Update(L);
            Engine.AccountActions.Update(A, CurrentUser.FullName);

            //bool bNew = false;
            // SZ [May 20, 2014] this is tricky part. 

            // 2 cases  (DD has the value, DD is blank


            //if (l=<0)
            //{



            //}
            //else
            //{
            //    Individual p = 
            //    GetIndividualDetails(ref p);
            //}

            //Individual IPrimary = Engine.IndividualsActions.Get(A.PrimaryIndividualId ?? 0);
            //if (IPrimary == null)
            //{
            //    bNew = true;
            //    IPrimary = new Individual();
            //    IPrimary.AccountId = AccountID;
            //}
            //GetIndividualDetails(/*IndividualType.Primary, */ref IPrimary);

            //if (bNew)
            //{
            //    IPrimary = Engine.IndividualsActions.Add(IPrimary);
            //    Engine.AccountActions.SetIndividual(A.Key, IndividualType.Primary, IPrimary.Key);
            //}
            //else
            //    Engine.IndividualsActions.Change(IPrimary, CurrentUser.FullName);

            //if (HasSpouce)
            //{
            //    bNew = false;
            //    Individual ISecondary = Engine.AccountActions.GetIndividual(A.Key, SalesTool.DataAccess.IndividualType.Secondary);
            //    if (ISecondary == null)
            //    {
            //        bNew = true;
            //        ISecondary = new Individual();
            //        ISecondary.AccountId = AccountID;
            //    }
            //    GetIndividualDetails(IndividualType.Secondary, ref ISecondary);
            //    if (bNew)
            //    {
            //        ISecondary = Engine.IndividualsActions.Add(ISecondary);
            //        Engine.AccountActions.SetIndividual(A.Key, IndividualType.Secondary, ISecondary.Key);
            //    }
            //    else
            //        Engine.IndividualsActions.Change(ISecondary, CurrentUser.FullName);
            //}
            //else
            //{
            //    if (A.SecondaryIndividualId.HasValue)
            //        Engine.AccountActions.SetIndividual(A.Key, IndividualType.Secondary, 0);
            //}

            if (this is IIndividual)
                (this as IIndividual).UpdateIndividuals();
            IsIndividualAddNewAccount = false;
            if (!IndvInfo1.IsEditingRecord)
                IndvInfo1.Refresh();
            //MH:19 Sep 2014: it is related to apply action not for all changes where  inner save is called. moved to apply action specifically.
            //StoreLastActionDetails(ref L, CurrentUser, Helper.NullConvert<int>(ddActions.SelectedValue));

            ctlMessage.SetStatus("Information Saved Successfully");

            //Sz [Apr 19, 2013] fixed the issue of saving child controls
            SaveChildControls();
            //YA[19 March 2014] Refresh Lead and Marketing Tab
            if (ApplicationSettings.IsTermLife) LeadsMarketing1.ShowGrid(true);
            //YA[June 21, 2013] Duplicate Management Implementation
            if (HasNewAccountRequest && (ApplicationSettings.CanUseDuplicateManagementFeature == Konstants.UseDuplicateManagementFeature.Both || ApplicationSettings.CanUseDuplicateManagementFeature == Konstants.UseDuplicateManagementFeature.User))
                CheckDuplicateLead.Execute(LeadID);
            if (!IsInApplyActionWorkFlow)
            {
                //YA[April 2, 2013] For setting the Email Queue records with Queued status
                if (Engine.ApplicationSettings.CanRunEmailUpdater)
                    EmailQueueUpdater.Execute(A.Key, L.ActionId, L.StatusId, L.SubStatusId);
                if (Engine.ApplicationSettings.CanRunPostQueueUpdater)
                    PostQueueUpdater.Execute(A.Key, L.ActionId, L.StatusId, L.SubStatusId);
            }

            if (!bQuickPass)
                RedirectIfNewAccount(A.Key);

            DialPhone();
            BindActionHistoryGrid(A.Key);
            //YA[29 Jan, 2014]
            ReadOnlyFields();
        }
        catch (Exception ex)
        {
            ctlMessage.SetStatus(ex);
            error = ctlMessage.ErrorMessage;
        }
        //SZ [Apr 22, 2013] This call updates the individuals grid
        // OKAY this confusing case. READ IT CAREFULLY:
        // if the individual control edits a record which is different than quick fields (the Primary and Secondry controls) then no problem
        // but if it starts editing the same record as in quick fields then conflict arrises. The control has more validation checks then the
        // quick fields and hence it should take precedence over the quick fields. uncomment the following line if functionality other than this is requested
        //IndvInfo1.Refresh();
        // btnSave.CssClass = "buttonstyle";
        return error;
    }

    private void StoreLastActionDetails(ref Lead L, User U, int? actionId)
    {
        if (L.Account == null) return;//New Account
        DateTime dt = DateTime.Now;
        Guid assignedUser = L.Account.AssignedUserKey.HasValue ? L.Account.AssignedUserKey.Value : Guid.Empty;
        // SZ [May 9, 2014] Client has asked to chnage the functionality. Instead of storing the user names, store the actions performed by them.
        //  [Wednesday, May 07, 2014 10:39 PM] John Dobrotka: Suheyl, one critical change that client pointed out to me
        //  lea_last_action_csr_usr, lea_last_action_ta_usr, lea_last_action_ob_usr, lea_last_action_ap_usr is supposed to be the action key, not the user
        //  the user would be stored in the action history— John Dobrotka, Wednesday, May 07, 2014 10:39 PM


        if (assignedUser == U.Key)
        {
            L.LastActionByAssigned = actionId;
            L.LastActionAssignedOn = dt;
        }
        if (U.IsAlternateProductType ?? false)
        {
            L.LastActionByAP = actionId;
            L.LastActionAPOn = dt;
        }
        if (U.IsOnboardType ?? false)
        {
            L.LastActionByOB = actionId;
            L.LastActionOBOn = dt;
        }
        if (U.IsTransferAgent ?? false)
        {
            L.LastActionByTA = actionId;
            L.LastActionTAOn = dt;
        }
        if (U.DoesCSRWork ?? false)
        {
            L.LastActionByCSR = actionId;
            L.LastActionCSROn = dt;
        }
        L.ActionId = actionId;

        //YA[27 May 2014] Last Date should only update when apply action work flow complete.
        //L.LastActionDate = dt;

        //find user type and store dates.
        //if (U.isalternateproducttype ?? false)
        //{
        //    l.lastactionapby = currentuser.fullname;
        //    l.lastactionapon = datetime.now;
        //}
        //if (currentuser.isonboardtype ?? false)
        //{
        //    l.lastactionobby = currentuser.fullname;
        //    l.lastactionobon = datetime.now;
        //}
        //if (currentuser.istransferagent ?? false)
        //{
        //    l.lastactiontaby = currentuser.fullname;
        //    l.lastactiontaon = datetime.now;
        //}
        //if (currentuser.doescsrwork ?? false)
        //{
        //    l.lastactioncsrby = currentuser.fullname;
        //    l.lastactioncsron = datetime.now;
        //}
    }

    //SZ [Mar 19, 2014] Combined all the user assignment to a single & elegant function rather then the dispersed code
    private void AssignUsers2Account(ref Account A)
    {
        IffLog(A.Key, Convert.ToString(A.AssignedCsrKey), ddCSR.SelectedValue, "CSR");

        // Attiq - April-08-2014- Set the Account Assigned User to be Current User if user reached this page from Add New Account
        if (isCreatingNewAccount && A.AssignedUserKey == null)
        {
            IffLog(A.Key, Convert.ToString(CurrentUser.Key), "", "User");
            if (A.AssignedUserKey != CurrentUser.Key)
            {
                A.AssignedUserKey = CurrentUser.Key;
                AccountLogAssignment(A.Key, "User assigned", "from AssignUsers2Account method when CreatingNewAccount");
            }
        }
        else
        {
            IffLog(A.Key, Convert.ToString(A.AssignedUserKey), ddUsers.SelectedValue, "User");
            var agent = (ddUsers.SelectedValue != "-1") ? new Guid(ddUsers.SelectedValue) : (Guid?)null;
            if (A.AssignedUserKey != agent)
            {
                AccountLogAssignment(A.Key, "User assigned", "from AssignUsers2Account method");
                A.AssignedUserKey = agent;
            }

        }

        IffLog(A.Key, Convert.ToString(A.TransferUserKey), ddTA.SelectedValue, "TA");
        IffLog(A.Key, Convert.ToString(A.OnBoardUser), ddOP.SelectedValue, "OB");
        IffLog(A.Key, Convert.ToString(A.AlternateProductUser), ddAltProduct.SelectedValue, "AP");

        A.AssignedCsrKey = (ddCSR.SelectedValue != "-1") ? new Guid(ddCSR.SelectedValue) : (Guid?)null;
        A.TransferUserKey = (ddTA.SelectedValue != "-1") ? new Guid(ddTA.SelectedValue) : (Guid?)null;
        A.OnBoardUser = (ddOP.SelectedValue != "-1") ? new Guid(ddOP.SelectedValue) : (Guid?)null;
        A.AlternateProductUser = (ddAltProduct.SelectedValue != "-1") ? new Guid(ddAltProduct.SelectedValue) : (Guid?)null;

        isCreatingNewAccount = false; // Attiq - April-08-2014. 
    }

    /* 
     * SZ [Mar 20, 2013] 
     * WARNING.....
     * 
     * DO NOT CALL THIS FUNCTION DIRECTLY IN ANY CASE. THIS MUST ONLY BE CALLED FROM ProcessURL()
     * if you are adding code. do not use session stuff like AccountID, LeadID.
     * 
     */
    long InnerSave(string phone, int campaignId, int? statusId, TRType type)
    {
        Account A = Engine.AccountActions.Add(new Account()
            {
                AddedBy = CurrentUser.FullName,
                AddedOn = DateTime.Now,
                //MH:21 April 2014
                AssignedUserKey = (CurrentUser != null) ? CurrentUser.Key : default(Guid?),
                //MH:02 Jun
                OriginalUserKey = (CurrentUser != null) ? CurrentUser.Key : default(Guid?)
            });
        //MH:21 April 2014
        if (A.AssignedUserKey != null)
        {
            AccountLog(A.Key, "User assigned");
            AccountLogAssignment(AccountID, "User assigned", " from InnerSave screen pop");
        }
        if (A.OriginalUserKey != null)
        {
            AccountLog(A.Key, "Original User assigned");
            AccountLogAssignment(AccountID, "Original User assigned", " from InnerSave screen pop");
        }

        //long accId = A.Key;
        campaignId = (campaignId == 0) ? Engine.ApplicationSettings.DefaultCampaignId : campaignId;
        statusId = (statusId == 0) ? Engine.ApplicationSettings.DefaultStatusId : statusId;

        Lead L = new Lead
        {
            AccountId = A.Key, // SZ [Oct 8, 2013] removed this line as the account is created in InnerSave so no need to check (AccountID == 0 ? accId : AccountID)  
            CampaignId = campaignId,
            StatusId = statusId,
            ActionId = null
        };
        //MH:28 Jan 2014
        if (Engine.ApplicationSettings.IsTermLife)
        {
            var code = Request.QueryString[K_LEAD_SOURCECODE];
            if (!string.IsNullOrEmpty(code))
                L.SourceCode = code;
            else if (!string.IsNullOrEmpty(Engine.ApplicationSettings.SourceCode))
                L.SourceCode = Engine.ApplicationSettings.SourceCode;
            else
            {
                L.SourceCode = "ADVS";
            }
        }
        //SZ [Apr 30, 2014] Requirement Changed: Client, during the call mentioned that fields will be stored and it does not matter if the current usr is csr user of the accoutn or not.
        //YA[April 12, 2013] If Current User is CSR user than it will not assign anything
        //if (!IsCSRUser)
        //{
        //SZ [Apr 3, 2013] this line has been added client has added new requirements if Xfer & currentuser in Tr list then assign or else lease null
        bool bLog = false;
        switch (type)
        {
            case TRType.Xfer:
                if (IsTAUser) { A.TransferUserKey = CurrentUser.Key; bLog = true; }
                //WM - [30.07.2013]
                else if (A.AssignedUserKey != CurrentUser.Key) { A.AssignedUserKey = CurrentUser.Key; bLog = true; }
                break;
            case TRType.Basic:
                if (A.AssignedUserKey != CurrentUser.Key) { A.AssignedUserKey = CurrentUser.Key; bLog = true; }
                break;
            case TRType.CSR:
                if (A.AssignedCsrKey != CurrentUser.Key) { A.AssignedCsrKey = CurrentUser.Key; bLog = true; }
                break;
            case TRType.AP:
                if (A.AlternateProductUser != CurrentUser.Key) { A.AlternateProductUser = CurrentUser.Key; bLog = true; }
                break;
            case TRType.OB:
                if (A.OnBoardUser != CurrentUser.Key) { A.OnBoardUser = CurrentUser.Key; bLog = true; }
                break;
        }
        if (bLog)
        {
            AccountLog(AccountID, "A user has been assigned");
            AccountLogAssignment(AccountID, "User assigned", " from InnerSave screen pop");
        }

        //}

        A.PrimaryLeadKey = Engine.LeadsActions.Add(L).Key;
        //AccountID = A.Key;
        //LeadID = lea.Key;

        Individual IPrimary = IsTCPAEnabled ? //SZ [Mer 20, 2013] TCPA Task#4 Added the TCPA switch & phone is saved in the inbound field else regular stuff
            Engine.IndividualsActions.Add(new Individual
            {
                AccountId = A.Key,
                InboundPhone = Helper.SafeConvert<long>(phone).ToString()
            }) :
            Engine.IndividualsActions.Add(new Individual
            {
                AccountId = A.Key,
                DayPhone = Helper.SafeConvert<long>(phone)
            });
        Engine.AccountActions.SetIndividual(A.Key, IndividualType.Primary, IPrimary.Key, CurrentUser.FullName);
        //WM - [30.07.2013] - this will be assigned conditionally in the above switch statement
        //A.AssignedUserKey = CurrentUser.Key;
        Engine.AccountActions.Update(A, CurrentUser.FullName);

        //AccountLog(A.Key, "Account has been created");        
        //YA[April 2, 2013] For setting the Email Queue records with Queued status
        if (Engine.ApplicationSettings.CanRunEmailUpdater)
        {
            EmailQueueUpdater.Execute(A.Key, L.ActionId, L.StatusId, L.SubStatusId);
        }
        if (Engine.ApplicationSettings.CanRunPostQueueUpdater)
        {
            PostQueueUpdater.Execute(A.Key, L.ActionId, L.StatusId, L.SubStatusId);
        }
        return A.Key;
    }

    private void CreateNewAccount(long parentAccountID)
    {
        Account nAccount = Engine.AccountActions.Add(new Account(), CurrentUser.FullName);
        Account oldAccount = Engine.AccountActions.Get(AccountID, true);

        //long parentAccountID = parentAccountID;
        //if(oldAccount.AccountParent.HasValue)
        //    parentAccountID = Engine.AccountActions.Get(oldAccount.AccountParent.Value, false).Key;

        nAccount.AssignedCsrKey = oldAccount.AssignedCsrKey;
        nAccount.AssignedUserKey = oldAccount.AssignedUserKey;
        nAccount.TransferUserKey = oldAccount.TransferUserKey;
        nAccount.Notes = oldAccount.Notes;
        nAccount.ExternalAgent = oldAccount.ExternalAgent;
        nAccount.LifeInfo = oldAccount.LifeInfo;
        nAccount.AddedBy = CurrentUser.FullName;


        Engine.AccountActions.Update(nAccount);

        Lead oldLead = Engine.LeadsActions.Get(oldAccount.PrimaryLeadKey ?? -1);
        Lead nlead = oldLead.Duplicate();

        nlead.AccountId = nAccount.Key;
        nlead.AddedBy = CurrentUser.Added.By;
        nlead.AddedOn = DateTime.Now;

        var U = Engine.LeadsActions.Add(nlead);
        nAccount.PrimaryLeadKey = U == null ? default(long) : U.Key;
        nAccount.AccountParent = parentAccountID;
        Engine.AccountActions.Update(nAccount);

        //Individual IPrimary = new Individual();
        //IPrimary.AccountId = nAccount.Key;
        //IPrimary = Engine.IndividualsActions.Add(IPrimary);
        //Engine.AccountActions.SetIndividual(nAccount.Key, IndividualType.Primary, IPrimary.Key);
        Session[Konstants.K_LEAD_INDIVIDUAL_ADD_NEW_ACCOUNT] = "true";

        AccountLog(nAccount.Key, string.Format("An account has been created. Url is {0}", Request.ToString()));

        //Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + nAccount.Key.ToString());
    }

    // SZ [May 19, 2014] XXXXX
    //void ShowSpouceControls(bool bShow)
    //{
    //    txtSpouseFName.Visible = bShow;
    //    tbSpouseLastName.Visible = bShow;
    //    rdisecondDob.Visible = bShow;
    //    lblSpouseFirstName.Visible = bShow;
    //    txtSpouseFName.Visible = bShow;
    //    lblSpouseLastName.Visible = bShow;
    //    tbSpouseLastName.Visible = bShow;
    //    lblSpouseDateOfBirth.Visible = bShow;
    //    rdisecondDob.Visible = bShow;
    //    valSpouceFirst.Visible = bShow;
    //    valSpouceLast.Visible = bShow;
    //    lblGenderS.Visible = bShow;
    //    ddlGenderS.Visible = bShow;

    //     SZ [May 19, 2014] XXXXXX
    //    lblEmailSpouse.Visible = bShow;
    //    txtEmailSpouse.Visible = bShow;

    //    lblEmailOptOutSpouse.Visible = bShow;
    //    chkEmailOptOutSpuse.Visible = bShow;

    //    lblAddress1Secondary.Visible = bShow;
    //    txtAddress1Secondary.Visible = bShow;
    //    lblAddress2Secondary.Visible = bShow;
    //    txtAddress2Secondary.Visible = bShow;
    //    lblCitySecondary.Visible = bShow;
    //    txtCitySecondary.Visible = bShow;
    //    lblStateSecondary.Visible = bShow;
    //    ddlStateSecondary.Visible = bShow;
    //    lblZipCodeSecondary.Visible = bShow;
    //    txtZipCodeSecondary.Visible = bShow;
    //}

    enum leadsTabs
    {
        Individuals = 0,
        ArcCases = 1,
        ArcHistory = 2,
        Homes = 3,
        Drivers = 4,
        Vechiles = 5,
        Policies = 6,
        Quotes = 7,
        CarriorIssueTracking = 8,
        MedicalSupplement = 9,
        ApplicationTracking = 10,
        MaAndPDF = 11,
        DentalVision = 12,
        LeadsAndMarketing = 13,
        Attachments = 14,
        Notes = 15,
        LifeInformation = 16
    }
    void ArrangeTabsByApplicationMode()
    {
        //Medical suppliment is removed from sqah
        //int[] tabHome = { 0, 3, 4, 5, 6, 7, 8, 13, 14, 15, 16 };
        //int[] tabSenior = { 0, 8, 9, 10, 11, 12, 13, 14, 15 }; //IH 07.10.13  -- removed 5-Quote from tabSenior Array
        //int[] tabTermlife = { 0, 1, 2, 9, 13, 14, 15 }; //SZ [Nov 12, 2013] added for the term life
        //MH: 28 April 2014 above code is replaced so to avoid un-intensional tabs showing after index changes
        //SQL - Individual/ArcCases/ArcHistory/Leads/Attachments/Notes
        //SQS - Individual/IssueTrackings/MedSupp/AppTracking/MAPDP/DentalVision/Leads/Attachments/Notes
        //SQAH - Indv/Home/Driver/Vehicle/Policy/Quote/Issue/Lead/Attach/Notes/LifeInfo
        int[] tabHome = new[] { 
            (int)leadsTabs.Individuals,             (int)leadsTabs.Homes,                   (int)leadsTabs.Drivers, 
            (int)leadsTabs.Vechiles,                (int)leadsTabs.Policies,                (int)leadsTabs.Quotes,
            (int)leadsTabs.CarriorIssueTracking,    (int)leadsTabs.LeadsAndMarketing,       (int)leadsTabs.Attachments,
            (int)leadsTabs.Notes,                   (int)leadsTabs.LifeInformation };

        int[] tabSenior = new[] { 
            (int)leadsTabs.Individuals,             (int)leadsTabs.CarriorIssueTracking,   (int)leadsTabs.LeadsAndMarketing,
            (int)leadsTabs.MedicalSupplement,       (int)leadsTabs.ApplicationTracking,    (int)leadsTabs.MaAndPDF,
            (int)leadsTabs.DentalVision,            (int)leadsTabs.Attachments,            (int)leadsTabs.Notes};

        int[] tabTermlife =
            {
                (int)leadsTabs.Individuals,        (int)leadsTabs.ArcCases,                (int)leadsTabs.ArcHistory,
                (int)leadsTabs.LeadsAndMarketing,  /*(int)leadsTabs.DentalVision,*/        (int)leadsTabs.Attachments,
                (int)leadsTabs.Notes,             
            };
        int imode = Engine.ApplicationSettings.InsuranceType;
        var active = imode == 0 ? tabSenior : imode == 1 ? tabHome : tabTermlife;

        foreach (RadTab tab in tlkLeadsTabs.Tabs)
        {
            tab.Visible = false;
            tab.PageView.Visible = false;
        }

        foreach (int i in active)
        {
            tlkLeadsTabs.Tabs[i].Visible = true;
            tlkLeadsTabs.Tabs[i].PageView.Visible = false;
        }

        tlkLeadsTabs.Tabs[active[0]].Selected = true;
        tlkLeadsTabs.Tabs[active[0]].PageView.Visible = true;

        var tabs = tlkLeadsTabs.Tabs.Where(x => !x.Visible);
        for (int i = 0; i < tabs.ToArray().Length; i++)
        {
            //tlkLeadsPages.PageViews.Remove(tabs.ToArray()[i].PageView);
            tlkLeadsTabs.Tabs.Remove(tabs.ToArray()[i]);
        }

        ////[QasimN 3/4/2013]
        //// index 6 to 13 are for Senior tabs
        //for (int i = 6; i <= 13; i++)
        //{
        //    //the below if condition let the Carrier issue tracking, Leads & Marketing...
        //    //... and Attachment tabs to be displayed on 
        //    if (i == 6 || i == 7 || i == 13)
        //    {
        //        tlkLeadsTabs.Tabs[i].PageView.Visible = false;
        //        continue;
        //    }
        //    tlkLeadsTabs.Tabs[i].Visible = senior;
        //    tlkLeadsTabs.Tabs[i].PageView.Visible = false;
        //}
        ////index 1 to 5 are Auto/Home tabs
        //for (int i = 1; i <= 5; i++)
        //{
        //    tlkLeadsTabs.Tabs[i].Visible = !senior;
        //    tlkLeadsTabs.Tabs[i].PageView.Visible = false;
        //}
        //tlkLeadsTabs.Tabs[12].Visible = false;
        //tlkLeadsTabs.Tabs[0].PageView.Visible = true;
        //tlkLeadsTabs.Tabs[0].Selected = true;
        //tlkLeadsPages.SelectedIndex = 0;        
    }

    void EnableTabs(bool bEnable = true)
    {
        foreach (RadTab t in tlkLeadsTabs.Tabs)
            t.Enabled = bEnable;


    }

    void ShowNextAccount()
    {
        long accountId = 0;
        if (Session["NextAccountId"] == null)
        {
            SalesTool.Schema.LeadsDirect leadsDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
            accountId = leadsDirect.NextPriorityAccount(AccountID, CurrentUser.Key);
        }
        else
        {
            accountId = Convert.ToInt64(Session["NextAccountId"] ?? 0);
            Session.Remove("NextAccountId");
        }
        if (AccountID == accountId)
        {
            RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "1", "Sys.Application.add_load(function(){{enableUI();}}, 0);", true);
            return;
        }

        if (accountId <= 0)
        {
            Response.Redirect(Konstants.K_PRIORITIZEDLEADSPAGE);
        }
        else
        {
            AccountID = accountId;
            //Redirect("Leads.aspx");
            Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + AccountID.ToString());
        }
    }

    long LeadID
    {
        get
        {
            long iLead = 0;
            long.TryParse((Session[Konstants.K_LEAD_ID] ?? "").ToString(), out iLead);
            return iLead;
        }
        set
        {
            Session[Konstants.K_LEAD_ID] = value;
        }
    }

    long EditIndividualID
    {
        get
        {
            long iResult = 0;
            long.TryParse((Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_KEY] ?? "").ToString(), out iResult);
            return iResult;
        }
        set
        {
            Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_KEY] = value;
        }
    }
    //YA[13 March 2014]
    bool IsLeadMarketingEditMode
    {
        get
        {
            bool iResult = false;
            bool.TryParse((Session[Konstants.K_LEAD_LEADMARKETING_EDIT_MODE] ?? "").ToString(), out iResult);
            return iResult;
        }
        set
        {
            Session[Konstants.K_LEAD_LEADMARKETING_EDIT_MODE] = value;
        }
    }
    long EditLeadMarketingID
    {
        get
        {
            long iResult = 0;
            long.TryParse((Session[Konstants.K_LEAD_LEADMARKETING_EDIT_KEY] ?? "").ToString(), out iResult);
            return iResult;
        }
        set
        {
            Session[Konstants.K_LEAD_LEADMARKETING_EDIT_KEY] = value;
        }
    }

    bool IsIndividualEditMode
    {
        get
        {
            bool iResult = false;
            bool.TryParse((Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_MODE] ?? "").ToString(), out iResult);
            return iResult;
        }
        set
        {
            Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_MODE] = value;
        }
    }
    bool IsIndividualAddNewAccount
    {
        get
        {
            bool iResult = false;
            bool.TryParse((Session[Konstants.K_LEAD_INDIVIDUAL_ADD_NEW_ACCOUNT] ?? "").ToString(), out iResult);
            return iResult;
        }
        set
        {
            Session[Konstants.K_LEAD_INDIVIDUAL_ADD_NEW_ACCOUNT] = value;
        }
    }

    bool IsNewLead
    {
        get
        {
            return LeadID < 1;
        }
    }

    // SZ [May 19, 2014] XXXXXX
    //bool HasSpouce
    //{
    //    get
    //    {
    //        // SZ [May 19, 2014] XXXXXX
    //        //return ddlSpouse.SelectedValue == "Y";

    //        return false;
    //    }
    //    set
    //    {
    //        //ddlSpouse.SelectedValue = value ? "Y" : "N";
    //    }
    //}

    void DialPhone()
    {
        string alertMessage = "javascript:alert('Invalid phone number.')";

        //SZ [May 1, 2013] below does not require any call to DB. it is resolved from the cache
        if (txtDayTimePhNo.Text.Trim().Length != 10)
        {
            lnkDayPhone.Attributes.Add("onclick", alertMessage);
        }
        else
        {
            Decimal dayTimePhoneNo = 0;
            Decimal.TryParse(txtDayTimePhNo.Text, out dayTimePhoneNo);

            // string outpulseId = Individuals.Where(x => x.DayPhone == Helper.SafeConvert<decimal>(txtDayTimePhNo.Text)).Select(x => x.OutpulseId).FirstOrDefault();
            string outpulseId = Individuals.Where(x => x.DayPhone == dayTimePhoneNo).Select(x => x.OutpulseId).FirstOrDefault();
            CurrentOutpulseIDDayPhone = outpulseId ?? "";
            if (Engine.ApplicationSettings.IsPhoneSystemFive9)
            {
                lnkDayPhone.Attributes.Add("onclick", Helper.GetPhoneWindowScript(txtDayTimePhNo.Text, outpulseId));
            }
            else
                lnkDayPhone.Attributes.Add("onclick", "");
        }

        if (txtEvePhNo.Text.Trim().Length != 10)
        {
            lnkEvePhNo.Attributes.Add("onclick", alertMessage);
        }
        else
        {
            Decimal eveningTimePhoneNo = 0;
            Decimal.TryParse(txtEvePhNo.Text, out eveningTimePhoneNo);
            //string outpulseId = Individuals.Where(x => x.EveningPhone == Helper.SafeConvert<decimal>(txtEvePhNo.Text)).Select(x => x.OutpulseId).FirstOrDefault();
            string outpulseId = Individuals.Where(x => x.EveningPhone == eveningTimePhoneNo).Select(x => x.OutpulseId).FirstOrDefault();
            CurrentOutpulseIDEveningPhone = outpulseId ?? "";
            if (Engine.ApplicationSettings.IsPhoneSystemFive9)
            {
                lnkEvePhNo.Attributes.Add("onclick", Helper.GetPhoneWindowScript(txtEvePhNo.Text, outpulseId));
            }
            else
                lnkEvePhNo.Attributes.Add("onclick", "");
        }

        //YA[27 Feb 2014]
        if (txtCellPhNo.Text.Trim().Length != 10)
        {
            lnkCellPhone.Attributes.Add("onclick", alertMessage);
        }
        else
        {
            Decimal cellPhoneNo = 0;
            Decimal.TryParse(txtCellPhNo.Text, out cellPhoneNo);
            string outpulseId = Individuals.Where(x => x.CellPhone == cellPhoneNo).Select(x => x.OutpulseId).FirstOrDefault();
            CurrentOutpulseIDCellPhone = outpulseId ?? "";
            if (Engine.ApplicationSettings.IsPhoneSystemFive9)
            {
                lnkCellPhone.Attributes.Add("onclick", Helper.GetPhoneWindowScript(txtCellPhNo.Text, outpulseId));
            }
            else
                lnkCellPhone.Attributes.Add("onclick", "");
        }
    }

    // SZ [Apr 2, 2014] Added the code required for the applyaction functionality:
    //    Disable Action – 20 Hours
    //1. If action is selected, Apply Action button is disabled for the time specified in 
    //ActionDisableSeconds since last action date 
    //iv. Automatic Next Account – 20 Hours 
    //1. When action is applied, the “Next Account” button functionality will be invoked
    // if(DisableAction) => ActionButton(false) Ajax Timer(G:ActionDisableSeconds) -> ActionButton(true) 

    //v.Stay in PV – 10 Hours
    // 1. Record is not removed from PV when this action is applied
    // if(Action.StayInPV) -> Applied : Redirect to Normal(false)

    //YA[Dec 16, 2013]
    private void InContactCall(string phoneNumber = "", string OutPulseID = "")
    {
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);

        inContactAuthorizationResponse authToken;
        JoinSessionResponse sessionResponse;

        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(OutPulseID))
        {
            exceptionMessage = "Outpulse ID Not Found.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
            return;
        }
        else if (string.IsNullOrEmpty(CurrentUser.PhoneSystemUsername) && string.IsNullOrEmpty(CurrentUser.PhoneSystemPassword))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
            return;
        }
        inContactFunctions funcs = new inContactFunctions();
        authToken = funcs.inContactAuthorization(Engine.ApplicationSettings.PhoneSystemAPIGrantType, Engine.ApplicationSettings.PhoneSystemAPIScope, CurrentUser.PhoneSystemUsername, CurrentUser.PhoneSystemPassword, Engine.ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        if (authToken == null)
        {
            exceptionMessage = "Unable to authenticate with Softphone.";
            Master.ShowAlert(exceptionMessage, "inContact Dial");
        }
        else
        {
            sessionResponse = funcs.inContactJoinSession(authToken, ref exceptionMessage);
            if (sessionResponse != null)
            {
                exceptionMessage = funcs.inContactDialNumber(authToken, sessionResponse, phoneNumber.Replace("-", ""), OutPulseID);
                if (!string.IsNullOrEmpty(exceptionMessage)) Master.ShowAlert(exceptionMessage, "inContact Dial Error");
            }
            else
            {
                Master.ShowAlert(exceptionMessage, "inContact Dial");
            }
        }
    }
    //public void DialPhone(object sender, EventArgs e)
    //{
    //    LinkButton lbtn = sender as LinkButton;
    //    if (lbtn == null)
    //        return;
    //    if (ApplicationSettings.IsPhoneSystemInContact)
    //    {
    //        InContactCall(lbtn.Text);
    //    }
    //}
    /// <summary>
    /// Imran H [25.09.13] here the popup will be displayed if account is soft deleted. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_Add_Clicked(object sender, EventArgs e)
    {
        string page = string.Format("{0}?accountid=-1", Konstants.K_LEADS_PAGE);
        Response.Redirect(page);

    }
    /// <summary>
    /// Imran H [25.09.13] here the popup will be displayed if account is soft deleted. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_ViewLead_Clicked(object sender, EventArgs e)
    {

        Redirect(Konstants.K_VIEW_LEADS_PAGE);

    }
    /// <summary>
    /// Imran H [25.09.13] here the popup will be displayed if account is soft deleted. 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PrioritizationView_Clicked(object sender, EventArgs e)
    {

        Redirect(Konstants.K_PRIORITIZEDLEADSPAGE);

    }



    protected void btnYes_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
            InnerSave();
    }

    protected void mybtn1_Click(object sender, EventArgs e)
    {
        Session["campIden"] = Convert.ToString(ddCampaigns.SelectedValue);
    }

    protected void ddCampaigns_SelectedIndexChanged(object sender, EventArgs e)
    {
        Session["campIden"] = Convert.ToString(ddCampaigns.SelectedValue);
    }

    protected void btnApplyAction_Click(object sender, EventArgs e)
    {
        //if (!Page.IsValid) return;
        bool hasShownApplyActionWorkFlowPopup = false;
        lblErrorMsg.Visible = false;
        IsActionButtonClicked = true;
        string applyActionSelectedValue = ddActions.SelectedValue;
        if (string.IsNullOrEmpty(applyActionSelectedValue))
        {
            ctlMessage.SetStatus("Action can not empty please select action.");
            return;
        }
        //MH: 09 March 2014 this comment validation is added before calling InnerSave function to prevent 
        //updates of the lead's data;
        SalesTool.DataAccess.Models.Action Ac = Engine.LocalActions.Get(Convert.ToInt32(applyActionSelectedValue));
        if (Ac.HasComment && string.IsNullOrEmpty(tbNotes.Text))
        {
            btnApplyAction.CausesValidation = true;
            lblErrorMsg.Visible = true;
            return;
        }

        string errorMessage = InnerSave(false, true);
        IsActionProcessed = false;  // SZ [May 16, 2014] Reset action.

        //YA[20 March 2014] In Add New account when data entered and apply action is pressed
        //Data saved but Individuals Quick Fields data not appear. Call to fix this issue.
        InnerLoad(AccountID, true);
        if (!string.IsNullOrEmpty(errorMessage))
            ctlMessage.SetStatus(errorMessage);
        if (AccountID > 0)
        {
            //Sz [Mar 26, 2014] Added for the new requirements. if th page is displayed from PV
            if (ActiveRuleId > 0)
            {
                AccountLog(AccountID, "An action has been initiated under the rule", ActiveRuleId);
                //d. When record is access from PV and Apply Action takes place on that record, store the rule value in a new 
                //field of account_history table called ach_pv_key – 25 Hours


            }
            Account A = Engine.AccountActions.Get(AccountID);
            if (ddActions.Items.Count > 0)
            {
                if (!Ac.HasAttempt || Ac.HasCalender)
                {
                    String actioncomment = "";
                    if (Ac.HasComment)
                    {
                        btnApplyAction.CausesValidation = true;

                        if (tbNotes.Text != "")
                        {
                            actioncomment = tbNotes.Text;
                        }
                        else
                        {
                            lblErrorMsg.Visible = true;
                            return;
                        }
                    }
                    else
                    {
                        // WM - 11.07.2013
                        //actioncomment = "Action '" + Ac.Title;
                        actioncomment = tbNotes.Text;
                    }
                    SalesTool.DataAccess.Models.Status s = Engine.StatusActions.GetActionChangedStatus(Convert.ToInt32(applyActionSelectedValue), Convert.ToInt32(ddlStatus.SelectedValue));
                    int statusIded = (s == null) ? Convert.ToInt32(ddlStatus.SelectedValue) : s.Id;

                    bool hasEdited = Engine.EventCalendarActions.DismissUponActionChange(this.AccountID, "");
                    Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
                    if (hasEdited && L != null) StoreLeadLastCalendarDetails(L.Key);
                    ShowApplyActionWinWin(actioncomment, statusIded, applyActionSelectedValue);
                    hasShownApplyActionWorkFlowPopup = true;
                }
                else
                {
                    if (LeadID > 0)
                    {
                        Lead L = Engine.LeadsActions.Get(LeadID);
                        Int64 LeadCurrentStatusId = Convert.ToInt64(L.StatusId ?? 0);
                        Int64 LeadCurrentSubstatusId = Convert.ToInt64(L.SubStatusId ?? 0);

                        if (ddActions.Items.Count > 0)
                        {
                            L.ActionId = Convert.ToInt32(applyActionSelectedValue);

                            StoreLastActionDetails(ref L, CurrentUser, Helper.NullConvert<int>(applyActionSelectedValue));
                            ShowEmailSender(true, L.ActionId.Value);
                            //MH:12 June 2014, is StayIn PV then don't update last action date.
                            if (!Ac.ShouldStayInPrioritizedView.ConvertOrDefault<bool>())
                            {
                                L.LastActionDate = DateTime.Now;
                                //WM - 11.07.2013
                                lblLastActionDate.Text = DateTime.Now.ToString("MM/dd/yyyy hh:mm");
                            }
                            else
                            {
                                lblLastActionDate.Text = L.LastActionDate.HasValue ? L.LastActionDate.Value.ToString("MM/dd/yyyy hh:mm") : string.Empty;
                            }
                            if (ddlStatus.Items.Count > 0)
                            {
                                SalesTool.DataAccess.Models.Status s = Engine.StatusActions.GetActionChangedStatus(Convert.ToInt32(applyActionSelectedValue), Convert.ToInt32(ddlStatus.SelectedValue));

                                L.StatusId = (s == null) ? Convert.ToInt32(ddlStatus.SelectedValue) : s.Id;
                                //lea.StatusId = Convert.ToInt32(ddlStatus.SelectedValue);

                                int statusId = (L.StatusId == Convert.ToInt32(ddlStatus.SelectedValue)) ? Convert.ToInt32(ddlStatus.SelectedValue) : s.Id;

                                var S = Engine.StatusActions.Get(Convert.ToInt32(statusId));

                                // SZ[Apr 19, 2013] altered the line below as it is a BAD BAD use of language
                                //int currentSubSt = (Convert.ToInt32(lea.SubStatusId) == null) ? 0 : Convert.ToInt32(lea.SubStatusId);
                                int currentSubSt = L.SubStatusId ?? 0;
                                currentSubSt = (L.StatusId != Convert.ToInt32(ddlStatus.SelectedValue)) ? 0 : Convert.ToInt32(L.SubStatusId);

                                //int currentSubSt = (Convert.ToInt32(lea.SubStatusId) == null) ? 0 : Convert.ToInt32(lea.SubStatusId);
                                //bool firstTimeSubStatus = false;
                                //if (currentSubSt == 0)
                                //{
                                //    currentSubSt = (lea.StatusId != Convert.ToInt32(ddlStatus.SelectedValue)) ? 0 : (ddlSubStatus1.Items.Count > 0 ? Convert.ToInt32(ddlSubStatus1.SelectedValue) : 0);
                                //    if (currentSubSt > 0)
                                //        firstTimeSubStatus = true;
                                //}

                                //currentSubSt = GetNextSubStatus(Convert.ToInt32(ddlStatus.SelectedValue), currentSubSt);

                                if (Convert.ToBoolean(S.Progress))
                                {
                                    SalesTool.Schema.LeadsDirect leadsDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);

                                    //[QN, 04-16-2013] Code to fetch next substatus has been moved to sql sp.
                                    //in order to improve the performance. GetNextSubstatus

                                    SalesTool.DataAccess.Models.Status SS = Engine.StatusActions.NextSubStatus(Convert.ToInt32(statusId), currentSubSt);
                                    int subStatusID = SS != null ? SS.Id : currentSubSt == 0 ? -1 : currentSubSt;
                                    L.SubStatusId = subStatusID;
                                    //if (!firstTimeSubStatus)
                                    //{
                                    //    Int32 nextSubStatusId = Convert.ToInt32(leadsDirect.GetNextSubstatus(Convert.ToInt64(statusId), Convert.ToInt64(currentSubSt)));
                                    //    int subStatusID = nextSubStatusId != null ? nextSubStatusId : -1;
                                    //    if (subStatusID > 0)
                                    //        lea.SubStatusId = subStatusID;
                                    //}
                                    //else
                                    //    lea.SubStatusId = currentSubSt;
                                }
                            }
                        }
                        if (Ac.HasComment)
                        {
                            btnApplyAction.CausesValidation = true;

                            if (tbNotes.Text != "")
                            {
                                StoreLastActionDetails(ref L, CurrentUser, Helper.NullConvert<int>(applyActionSelectedValue));
                                Engine.LeadsActions.Update(L);


                                //[MH: 07 March 2014
                                //IsMultipleAccountsAllowed is true when is in SQL Mode and New_Lead_Layout flag enabled.
                                if (Ac.HasReleatedActsUpdate && Engine.ApplicationSettings.IsMultipleAccountsAllowed)
                                {
                                    Engine.AccountHistory.ActionChangedRelatedAccounts(A.Key, Ac.Title, Server.HtmlEncode(tbNotes.Text), CurrentUser.Key, Convert.ToInt32(L.ActionId), LeadCurrentStatusId, LeadCurrentSubstatusId, L.StatusId, L.SubStatusId, ActiveRuleId);
                                }
                                else
                                {
                                    ///[QN, 14/05/2013] New parameters has been added in this function 
                                    ///... for a change request mentioned in mantis item 148.(http://bugs.condadogroup.com/view.php?id=148)
                                    ///... details is mentioned on the definition of this function. 
                                    /// Engine.AccountHistory.ManageActionChange(A.Key, Ac.Title, tbNotes.Text, CurrentUser.Key, Convert.ToInt64(lea.ActionId), LeadCurrentStatusId, LeadCurrentSubstatusId, Convert.ToInt64(lea.StatusId), Convert.ToInt64(lea.SubStatusId));
                                    Engine.AccountHistory.ActionChanged(A.Key, Ac.Title, Server.HtmlEncode(tbNotes.Text), CurrentUser.Key, Convert.ToInt64(L.ActionId), LeadCurrentStatusId, LeadCurrentSubstatusId, Convert.ToInt64(L.StatusId), Convert.ToInt64(L.SubStatusId), ActiveRuleId);
                                }
                                //SZ [Apr 28, 2014] Added as client requested on call. Leads dates need to be updated.
                                //UpdateLeadsHistoryOnApplyAction();
                                
                                bool hasEdited = Engine.EventCalendarActions.DismissUponActionChange(this.AccountID, "");
                                Lead lea = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
                                if (hasEdited && L != null) StoreLeadLastCalendarDetails(lea.Key);
                                //AddAccountHistory(entry, A, tbNotes.Text);
                            }
                            else
                            {
                                lblErrorMsg.Visible = true;
                                return;
                            }
                        }
                        else
                        {
                            StoreLastActionDetails(ref L, CurrentUser, Helper.NullConvert<int>(applyActionSelectedValue));
                            Engine.LeadsActions.Update(L);
                            //IsMultipleAccountsAllowed is true when is in SQL Mode and New_Lead_Layout flag enabled.
                            if (Ac.HasReleatedActsUpdate && Engine.ApplicationSettings.IsMultipleAccountsAllowed)
                            {
                                Engine.AccountHistory.ActionChangedRelatedAccounts(A.Key, Ac.Title, Server.HtmlEncode(tbNotes.Text), CurrentUser.Key, Convert.ToInt32(L.ActionId), LeadCurrentStatusId, LeadCurrentSubstatusId, L.StatusId, L.SubStatusId, ActiveRuleId);
                            }
                            else
                            {
                                //Engine.AccountHistory.ManageActionChange(A.Key, Ac.Title, tbNotes.Text, CurrentUser.Key);
                                ///[QN, 14/05/2013] New parameters has been added in this function 
                                ///... for a change request mentioned in mantis item 148.(http://bugs.condadogroup.com/view.php?id=148)
                                ///... details is mentioned on the definition of this function. 
                                //Engine.AccountHistory.ManageActionChange(A.Key, Ac.Title, tbNotes.Text, CurrentUser.Key, Convert.ToInt64(lea.ActionId), LeadCurrentStatusId, LeadCurrentSubstatusId, Convert.ToInt64(lea.StatusId), Convert.ToInt64(lea.SubStatusId));
                                ///
                                Engine.AccountHistory.ActionChanged(A.Key, Ac.Title, Server.HtmlEncode(tbNotes.Text), CurrentUser.Key, Convert.ToInt64(L.ActionId), LeadCurrentStatusId, LeadCurrentSubstatusId, Convert.ToInt64(L.StatusId), Convert.ToInt64(L.SubStatusId), ActiveRuleId);

                            }
                            //SZ [Apr 28, 2014] Added as client requested on call. Leads dates need to be updated.
                            //UpdateLeadsHistoryOnApplyAction();

                            bool hasEdited = Engine.EventCalendarActions.DismissUponActionChange(this.AccountID, "");
                            Lead lea = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
                            if (hasEdited && lea != null) StoreLeadLastCalendarDetails(lea.Key);
                            //AddAccountHistory(entry, A, "Action '" + Ac.Title + "' has been applied by " + CurrentUser.FirstName + " " + CurrentUser.LastName);
                        }
                        ddlStatus.DataSource = Engine.StatusActions.All;
                        ddlStatus.DataBind();
                        if (L.StatusId > 0)
                        {
                            ddlStatus.SelectedValue = Convert.ToString(L.StatusId);

                            if (ddlStatus.Items.Count > 0)
                            {
                                //ddActions.DataSource = Engine.StatusActions.GetActionTemplates(Convert.ToInt32(ddlStatus.SelectedValue), false);
                                //ddActions.DataBind();
                                //btnApplyAction.Enabled = true;
                                int statusId = Convert.ToInt32(ddlStatus.SelectedValue);
                                BindAvailableActions(statusId);
                            }
                            else
                            {
                                btnApplyAction.Enabled = txtNotes.Enabled = false;
                            }
                        }
                        //MH: 03 May 2014   Exception message: 'ddlSubStatus1' has a SelectedValue which is invalid because it does not exist in the list of items.
                        ddlSubStatus1.SafeBind(Engine.StatusActions.GetSubStatuses(Convert.ToInt32(ddlStatus.SelectedValue), false),
                            L.SubStatusId.ToString());
                        //ddlSubStatus1.Items.Clear();
                        //ddlSubStatus1.SelectedValue = null;
                        //ddlSubStatus1.DataSource = Engine.StatusActions.GetSubStatuses(Convert.ToInt32(ddlStatus.SelectedValue), false);
                        //ddlSubStatus1.DataBind();
                        //if (lea.SubStatusId != null)
                        //{
                        //    if (ddlSubStatus1.Items.FindByValue(lea.SubStatusId.ToString()) != null)
                        //    {
                        //        ddlSubStatus1.SelectedValue = lea.SubStatusId.ToString();
                        //    }
                        //}
                        //YA[April 2, 2013] For setting the Email Queue records with Queued status
                        if (Engine.ApplicationSettings.CanRunEmailUpdater)
                        {
                            EmailQueueUpdater.Execute(AccountID, L.ActionId, L.StatusId, L.SubStatusId);
                        }
                        if (Engine.ApplicationSettings.CanRunPostQueueUpdater)
                        {
                            PostQueueUpdater.Execute(AccountID, L.ActionId, L.StatusId, L.SubStatusId);
                        }
                    }

                    //Client's Requirement:
                    //Please take a look at the next account button.  
                    //When an user is setup with the "Next" setting and not the "top" 
                    //setting it is always supposed to go to the next record
                    //when an actio is applied that updates the status and takes that 
                    //account out of the prioritized list, the next record is the top of 
                    //the list again, but we need it to go to the next record

                    //[QN, 07/11/2013]
                    //based on above requirement we need to find next account before deleting it. 
                    Session.Remove("NextAccountId");
                    SalesTool.Schema.LeadsDirect ldDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
                    long acntId = ldDirect.NextPriorityAccount(AccountID, CurrentUser.Key);
                    //if (acntId > 0)
                    //    Session["NextAccountId"] = acntId;
                    if (acntId > 0)
                    {
                        //YA[22 Jan, 2013] Next Account Issue, Double clicking the button to go to the next account, 
                        //To resolve it place this condition.
                        if (AccountID == acntId)
                        {
                            Session["NextAccountId"] = null;
                        }
                        else
                            Session["NextAccountId"] = acntId;
                    }

                    if (!(Ac.ShouldStayInPrioritizedView ?? false))
                        Engine.ListPrioritizationAccount.DeleteByAccountID(AccountID);

                    Engine.ListRetentionAccount.DeleteByAccountID(AccountID);
                    BindActionHistoryGrid(AccountID);
                    tbNotes.Text = string.Empty;

                    if (Ac.ShouldAutomaticNextAccount ?? false)
                        ShowNextAccount();
                }

                //MH:18 April Refresh Appointment bar.
                // EventCalendarList2.BindgrdEventCalendar();
            }

            //YA[27 May 2014] Call to fix the issue that calculation should be applied after going through the apply action process.
            if (hasShownApplyActionWorkFlowPopup == false)
                ManageActionChange();
        }
    }

    private void StoreLeadLastCalendarDetails(long leadKey)
    {
        //Helper.SafeConvert<long>((Session[Konstants.K_LEAD_ID] ?? 0).ToString())
        Lead L = Engine.LeadsActions.Get(leadKey);
        if (L.Account == null) return;//New Account
        DateTime dt = DateTime.Now;
        Guid assignedUser = L.Account.AssignedUserKey.HasValue ? L.Account.AssignedUserKey.Value : Guid.Empty;
        var U = this.CurrentUser;
        if (assignedUser == U.Key)
        {
            L.LastCalendarChangeAssignedUserDate = dt;
        }
        if (U.IsAlternateProductType ?? false)
        {
            L.LastCalendarChangeAPUserDate = dt;
        }
        if (U.IsOnboardType ?? false)
        {
            L.LastCalendarChangeOBUserDate = dt;
        }
        if (U.IsTransferAgent ?? false)
        {
            L.LastCalendarChangeTAUserDate = dt;
        }
        if (U.DoesCSRWork ?? false)
        {
            L.LastCalendarChangeCSRUserDate = dt;
        }
        Engine.LeadsActions.Update(L);

    }


    private void UpdateLeadsHistoryOnApplyAction()
    {
        try
        {
            Lead L = Engine.AccountActions.Get(AccountID).PrimaryLead;
            StoreLastActionDetails(ref L, CurrentUser, Helper.NullConvert<int>(ddActions.SelectedValue));
            Engine.LeadsActions.Update(L);
        }
        catch (Exception ex)
        {
            ctlMessage.SetStatus(ex);
        }
    }

    protected void tbNotes_TextChanged(object sender, EventArgs e)
    {
        lblErrorMsg.Visible = false;
    }

    protected void ddlStatus_SelectedIndexChanged(object sender, EventArgs e)
    {
        // SZ [Mar 20, 2013] made changes after the sub status 1 has been added
        int statusId = default(int);
        if (int.TryParse(ddlStatus.SelectedValue, out statusId))
        {
            //MH. Disable button if no action 
            BindAvailableActions(statusId);
            //var actionSource=Engine.StatusActions.GetActionTemplates(statusId, false);
            //if (actionSource.Any())
            //{
            //    ddActions.DataSource = actionSource;
            //    ddActions.DataBind();
            //    btnApplyAction.Enabled = true;
            //}
            //else
            //{
            //    btnApplyAction.Enabled = false;
            //}

            //MH: 03 May 2014   Exception message: 'ddlSubStatus1' has a SelectedValue which is invalid because it does not exist in the list of items.
            ddlSubStatus1.SafeBind(Engine.StatusActions.GetSubStatuses(statusId, false), null);
            //ddlSubStatus1.SelectedValue = null;
            //ddlSubStatus1.DataSource = Engine.StatusActions.GetSubStatuses(statusId, false);
            //ddlSubStatus1.DataBind();
        }
    }

    protected void btnMore_Click1(object sender, EventArgs e)
    {
        if (ddCampaigns.Items.Count > 0)
        {
            int campaignID = 0;
            if (int.TryParse(ddCampaigns.SelectedValue, out campaignID))
            {
                string description = Engine.ManageCampaignActions.DescriptionOf(campaignID);
                lblCampaignDescription.Text = description.Length > 0 ? description : "No description found";
                dlgCampaignDescription.VisibleOnPageLoad = true;
            }
            //ShowCampaignWin(Convert.ToInt32(ddCampaigns.SelectedValue));
        }
    }

    protected void btnCloseCampaignDescription_Click(object sender, EventArgs e)
    {
        dlgCampaignDescription.VisibleOnPageLoad = false;
    }

    //YA[April 16, 2013] Added Rad tab Click to show the selected tab page view, initially only first tab pageview will be visible.
    protected void tlkLeadsTabs_TabClick(object sender, RadTabStripEventArgs e)
    {
        if (!e.Tab.PageView.Visible)
        {
            e.Tab.PageView.Visible = true;
            e.Tab.PageView.Controls[1].Visible = true;
        }
    }

    protected void btnAddEvent1_Click(object sender, EventArgs e)
    {
        if (AccountID > 0)
        {
            //EventCalendarAddEdit1.Initialize(); // ClearFields();
            //dlgEventCalendar.VisibleOnPageLoad = true;
            //YA[02 Feb, 2014]
            ShowEventCalendarWin();
        }
    }

    protected void btnCloseEventCalendar_Click(object sender, EventArgs e)
    {
        dlgEventCalendar.VisibleOnPageLoad = false;
    }

    public void ShowApplyActionWinWin(String ActionComment, int StatusId, string actionSelectedValue)
    {
        //YA[03 June 2014] Temporary fix for apply action workflow refresh issue
        tmrDisableAction.Enabled = false;

        string substatusId = "0";
        string realStatusId = "0";
        if (ddlSubStatus1.Items.Count > 0)
        {
            substatusId = ddlSubStatus1.SelectedValue ?? "0";
        }

        if (ddlStatus.Items.Count > 0)
        {
            realStatusId = ddlStatus.SelectedValue ?? "0";
        }

        var rdWin = new RadWindow();

        rdWin.Skin = "WebBlue";
        rdWin.Title = "Apply Action";
        rdWin.OnClientClose = "OnClientClose";
        rdWin.ID = "applyActionWin";
        rdWin.InitialBehaviors = Telerik.Web.UI.WindowBehaviors.None;
        rdWin.Behaviors = Telerik.Web.UI.WindowBehaviors.Move;
        rdWin.AutoSize = false;
        int winHeight = Master.GetWindowHeight();
        //if (winHeight > 500)
        //{
        //    rdWin.Height = winHeight-50;
        //    rdWin.MaxHeight = winHeight;
        //}
        //else
        {
            rdWin.Height = 900;//new Unit("100%"); //SR 26.3.2014
            rdWin.MaxHeight = 900;
        }
        rdWin.MinHeight = 300;
        rdWin.Width = 980;
        //rdWin.Top = 10;
        //rdWin.Left = 100;
        rdWin.VisibleStatusbar = false;
        rdWin.Modal = true;
        //SR 26.3.2014 rdWin.CssClass = "WindowRestore";
        //rdWin.CssClass = "WindowRestore";
        rdWin.NavigateUrl = "ApplyAction.aspx?statusId=" + Convert.ToString(StatusId) + "&LeadID=" + LeadID + "&AccountID=" + AccountID + "&ActionID=" + Convert.ToString(actionSelectedValue) + "&actioncomment=" + Server.UrlEncode(ActionComment) + "&substatusId=" + substatusId + "&realstatusId=" + realStatusId + "&ruleid=" + ActiveRuleId;
        rdWin.VisibleOnPageLoad = true;
        rdWin.CenterIfModal = true;
        rdWin.CssClass = "RadWindowTop";
        RadWindowManager2.Windows.Add(rdWin);
    }

    public void ShowCampaignWin(int StatusId)
    {
        var rdWin = new RadWindow();
        rdWin.Skin = "WebBlue";
        rdWin.Title = "Campaign Description";
        rdWin.OnClientClose = "OnClientClose";
        rdWin.ID = "campaignDescriptionWin";
        rdWin.InitialBehaviors = Telerik.Web.UI.WindowBehaviors.None;
        rdWin.Behaviors = Telerik.Web.UI.WindowBehaviors.Move;
        rdWin.Height = 425;
        rdWin.Width = 500;
        rdWin.VisibleStatusbar = false;
        rdWin.Modal = true;
        rdWin.NavigateUrl = "CampaignDescription.aspx?campIden=" + Convert.ToString(StatusId);
        rdWin.VisibleOnPageLoad = true;
        RadWindowManager2.Windows.Add(rdWin);
    }

    public void ShowEventCalendarWin()
    {
        var rdWin = new RadWindow();
        rdWin.Skin = "WebBlue";
        rdWin.Title = "Event Calendar";
        rdWin.OnClientClose = "OnClientClose";
        rdWin.ID = "eventCalendarWin";
        rdWin.InitialBehaviors = Telerik.Web.UI.WindowBehaviors.None;
        rdWin.Behaviors = Telerik.Web.UI.WindowBehaviors.Move;
        rdWin.Height = 650;
        rdWin.Width = 750;
        rdWin.VisibleStatusbar = false;
        rdWin.Modal = true;
        rdWin.NavigateUrl = "EventCalendar.aspx";
        rdWin.VisibleOnPageLoad = true;
        RadWindowManager2.Windows.Add(rdWin);
    }

    #region Leads PageFix

    //SZ [Apr 18, 2013] This is the fix for the session loss and varioables behaving eradically
    enum PageState
    {
        Unknown = 0,
        NewAccount = 1,
        Editing = 2,
        SessionLost = 4
    };

    long PageAccountId
    {
        get
        {
            long id = default(long);
            long.TryParse(hdnAccountId.Value.ToString(), out id);
            return id;
        }
        set
        {
            hdnAccountId.Value = value.ToString();
        }
    }

    PageState LeadsPageState
    {
        get
        {
            PageState Ans = PageState.Unknown;

            if (PageAccountId == AccountID)
                Ans = PageState.Editing;
            else if (PageAccountId == -1 && AccountID == 0)
                Ans = PageState.NewAccount;
            else if (PageAccountId != -1 && AccountID == 0)
                Ans = PageState.SessionLost;
            else if (PageAccountId != AccountID)
                Ans = PageState.SessionLost; // SZ [Apr 18, 2013] This is not exactly a session lost. 

            return Ans;
        }
    }

    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        if (LeadsPageState == PageState.SessionLost)
        {
            string url = Konstants.K_HOMEPAGE;

            switch (CurrentUser.LoginLandingPage)
            {
                case 1:
                    url = Konstants.K_HOMEPAGE;
                    break;
                case 2:
                    url = Konstants.K_VIEW_LEADS_PAGE;
                    break;
                case 3:
                    url = Konstants.K_PRIORITIZEDLEADSPAGE;
                    break;
                default:
                    url = Konstants.K_HOMEPAGE;
                    break;
            }
            Response.Redirect(url);
        }
    }

    void RedirectIfNewAccount(long accID)
    {
        int accountId = Convert.ToInt32(Request.QueryString[K_ACCOUNT_PARAM] ?? "0");

        if (!IsActionButtonClicked)
        {
            if (ParentAccountID > 0 && accountId == -1)
                Response.Redirect(string.Format("{0}?accountid={1}&{2}=true", Konstants.K_LEADS_PAGE, accID, Konstants.K_AVOID_REASSIGNMENT), false);
            else if (accountId == -1)
                Response.Redirect(string.Format("{0}?accountid={1}", Konstants.K_LEADS_PAGE, accID), false);

        }
    }

    #endregion

    //SZ [Apr 19, 2013] The fix for saving the control's data when the save button is hit
    void SaveChildControls()
    {
        StringBuilder sb = new StringBuilder();
        foreach (Telerik.Web.UI.RadTab T in tlkLeadsTabs.Tabs)
        {
            if (T.Visible && T.PageView.Controls[1] is AccountsBaseControl)
            {
                var X = T.PageView.Controls[1] as AccountsBaseControl;
                if (X != null && X.IsEditingRecord)
                {
                    if (X.IsValidated)
                        X.Save();
                    else
                        ctlMessage.SetStatus(new Exception(string.Format("Validation failed for {0} tab. ", T.Text)));
                }
            }
        }
        ctlMessage.SetStatus(sb.ToString());
    }

    List<AccountsBaseControl> GetChildControls()
    {
        List<AccountsBaseControl> lstControls = new List<AccountsBaseControl>();
        foreach (Telerik.Web.UI.RadTab T in tlkLeadsTabs.Tabs)
        {
            if (T.PageView.Controls[1] is AccountsBaseControl)
                lstControls.Add(T.PageView.Controls[1] as AccountsBaseControl);
        }
        return lstControls;
    }


    void GetIndividualDetails(/*SalesTool.DataAccess.IndividualType type, */ref Individual I)
    {
        //if (type == IndividualType.Primary)
        //{
        if (I == null)
            I = new Individual()
                {
                    //MH:03 June 2014
                    IsActive = true,
                    IsDeleted = false,
                    AddedBy = CurrentUser.FullName,
                    AddedOn = DateTime.Now
                };

        I.FirstName = txtFName.Text;
        I.LastName = txtLName.Text;
        I.MiddleName = txtMiddleName.Text;
        if (txtDayTimePhNo.Text != string.Empty)
            I.DayPhone = Helper.SafeConvert<long>(txtDayTimePhNo.Text); // Helper.ConvertToNullLong(Helper.ConvertMaskToPlainText(txtDayTimePhNo.Text));
        else
            I.DayPhone = null;

        if (txtEvePhNo.Text != string.Empty)
            I.EveningPhone = Helper.SafeConvert<long>(txtEvePhNo.Text);// Helper.ConvertToNullLong(Helper.ConvertMaskToPlainText(txtEvePhNo.Text));
        else
            I.EveningPhone = null;

        //YA[27 Feb, 2014]
        if (txtCellPhNo.Text != string.Empty)
            I.CellPhone = Helper.SafeConvert<long>(txtCellPhNo.Text);
        else
            I.CellPhone = null;
        //SC is not sending phone to ARC in case of add new record in SC
        if (I.EntityState == EntityState.Detached || I.EntityState == EntityState.Added)
        {
            I.IsChanged = I.DayPhone.HasValue || I.EveningPhone.HasValue || I.CellPhone.HasValue;
        }
        I.Birthday = diDOB.SelectedDate;
        I.indv_ap_date = diDOB2.SelectedDate;
        I.Gender = ddlGenderP.SelectedValue;
        I.Zipcode = txtZipCodePrimary.Text;
        I.Email = txtEmailIndv.Text;
        I.IndividualEmailOptOut = chkEmailOptOutPrimary.Checked;
        I.Address1 = txtAddress1Primary.Text;
        I.Address2 = txtAddress2Primary.Text;
        I.City = txtCityPrimary.Text;
        if (ddlStatePrimary.SelectedValue != string.Empty && ddlStatePrimary.SelectedValue != "-1")
            I.StateID = byte.Parse(ddlStatePrimary.SelectedValue);

        if (CurrentUser.IsOnboardType == true)
        {
            I.OnBoardAnnuity = chk_indv_ob_annuity.Checked;
            I.OnBoardInspection = chk_indv_ob_inspection.Checked;
            I.OnBoardAutoHomeLife = chk_indv_ob_auto_home_life.Checked;
            I.OnBoardApplicationeSign = chk_indv_ob_app_esign.Checked;
            I.OnBoardDental = chk_indv_ob_dental.Checked;
            I.OnBoardBilling = chk_indv_ob_billing.Checked;
            I.OnBoardCsPrep = chk_indv_ob_cs_prep.Checked;
            I.OnBoardAutoHome = chk_indv_ob_auto_home.Checked;
        }
        //YA[28 Feb 2014]
        if (ddlApplicationState.SelectedValue != string.Empty && ddlApplicationState.SelectedValue != "-1")
            I.ApplicationState = byte.Parse(ddlApplicationState.SelectedValue);
        //}
        //else
        //{
        //I.FirstName = txtSpouseFName.Text;
        //I.LastName = tbSpouseLastName.Text;
        //I.Gender = ddlGenderS.SelectedValue;
        //I.Email = txtEmailSpouse.Text;
        //I.IndividualEmailOptOut = chkEmailOptOutSpuse.Checked;
        //I.Address1 = txtAddress1Secondary.Text;
        //I.Address2 = txtAddress2Secondary.Text;
        //I.City = txtCitySecondary.Text;
        //I.Zipcode = txtZipCodeSecondary.Text;

        //if (rdisecondDob.SelectedDate.HasValue)
        //    I.Birthday = rdisecondDob.SelectedDate;
        //if (ddlStateSecondary.SelectedValue != string.Empty && ddlStateSecondary.SelectedValue != "-1")
        //    I.StateID = byte.Parse(ddlStateSecondary.SelectedValue);
        //}
    }
    public void PopulateOnboardData(Individual individual)
    {

        chk_indv_ob_dental.Checked = individual.IfNotNull(p => p.OnBoardDental);
        chk_indv_ob_auto_home_life.Checked = individual.IfNotNull(p => p.OnBoardAutoHomeLife);
        chk_indv_ob_annuity.Checked = individual.IfNotNull(p => p.OnBoardAnnuity);
        chk_indv_ob_billing.Checked = individual.IfNotNull(p => p.OnBoardBilling);
        chk_indv_ob_inspection.Checked = individual.IfNotNull(p => p.OnBoardInspection);
        chk_indv_ob_app_esign.Checked = individual.IfNotNull(p => p.OnBoardApplicationeSign);
        chk_indv_ob_cs_prep.Checked = individual.IfNotNull(p => p.OnBoardCsPrep);
        chk_indv_ob_auto_home.Checked = individual.IfNotNull(p => p.OnBoardAutoHome);

        chk_indv_ob_cs_prep.Enabled = chk_indv_ob_auto_home.Enabled = chk_indv_ob_billing.Enabled = chk_indv_ob_dental.Enabled = chk_indv_ob_auto_home_life.Enabled = chk_indv_ob_annuity.Enabled = chk_indv_ob_inspection.Enabled = chk_indv_ob_app_esign.Enabled = false;
        field_indv_ob_cs_prep.Visible = field_indv_ob_auto_home.Visible = field_indv_ob_billing.Visible = field_indv_ob_annuity.Visible = field_indv_ob_dental.Visible = field_indv_ob_auto_home_life.Visible = field_indv_ob_inspection.Visible = field_indv_ob_app_esign.Visible = false;


        if (Engine.ApplicationSettings.IsSenior)
        {
            if (CurrentUser.IsOnboardType == true)
            {
                chk_indv_ob_dental.Enabled = chk_indv_ob_auto_home_life.Enabled = chk_indv_ob_annuity.Enabled = true;
            }
            field_indv_ob_annuity.Visible = field_indv_ob_dental.Visible = field_indv_ob_auto_home_life.Visible = true;

        }
        else if (Engine.ApplicationSettings.IsAutoHome)
        {
            if (CurrentUser.IsOnboardType == true)
            {
                chk_indv_ob_billing.Enabled = chk_indv_ob_inspection.Enabled = chk_indv_ob_app_esign.Enabled = true;
            }
            field_indv_ob_billing.Visible = field_indv_ob_inspection.Visible = field_indv_ob_app_esign.Visible = true;
        }
        else if (Engine.ApplicationSettings.IsTermLife)
        {
            if (CurrentUser.IsOnboardType == true)
            {
                chk_indv_ob_dental.Enabled = chk_indv_ob_cs_prep.Enabled = chk_indv_ob_auto_home.Enabled = true;
            }
            field_indv_ob_dental.Visible = field_indv_ob_cs_prep.Visible = field_indv_ob_auto_home.Visible = true;
        }
    }
    void SetIndividualDetails(/*SalesTool.DataAccess.IndividualType type,*/ Individual I)
    {
        ClearQuickFields();
        if (I != null)
        {
            //if (type == IndividualType.Primary)
            //{
            //IndividualID = I.Key;
            txtFName.Text = I.FirstName;
            txtLName.Text = I.LastName;
            txtMiddleName.Text = I.MiddleName;
            txtDayTimePhNo.Text = I.DayPhone.HasValue ? I.DayPhone.ToString() : "";
            txtEvePhNo.Text = I.EveningPhone.HasValue ? I.EveningPhone.ToString() : "";
            //YA[27 Feb 2014]
            txtCellPhNo.Text = I.CellPhone.HasValue ? I.CellPhone.ToString() : "";

            txtZipCodePrimary.Text = I.Zipcode;
            txtEmailIndv.Text = I.Email;
            chkEmailOptOutPrimary.Checked = I.IndividualEmailOptOut;
            txtAddress1Primary.Text = I.Address1;
            txtAddress2Primary.Text = I.Address2;
            txtCityPrimary.Text = I.City;

            if (I.StateID.HasValue)
                if (ddlStatePrimary.Items.FindByValue(I.StateID.ToString()) != null)
                    ddlStatePrimary.SelectedValue = I.StateID.ToString();
            //YA[28 Feb 2014]
            if (I.ApplicationState.HasValue)
                if (ddlApplicationState.Items.FindByValue(I.ApplicationState.ToString()) != null)
                    ddlApplicationState.SelectedValue = I.ApplicationState.ToString();
            if (I.Birthday.HasValue)
                diDOB.SelectedDate = I.Birthday.Value;
            if (I.indv_ap_date.HasValue)
                diDOB2.SelectedDate = I.indv_ap_date.Value;
            if (I.Gender != string.Empty)
                ddlGenderP.SelectedValue = I.Gender;

            switch (I.HasConsent)
            {
                case TCPAConsentType.Blank: ddlConsent.SelectedValue = "0"; break;
                case TCPAConsentType.No: ddlConsent.SelectedValue = "2"; break;
                case TCPAConsentType.Yes: ddlConsent.SelectedValue = "1"; break;
                case TCPAConsentType.Undefined: ddlConsent.SelectedValue = "3"; break;
            }

            PopulateOnboardData(I);

            //ddlConsent.SelectedValue = ((int)I.HasConsent).ToString();
            //}
            //else if (type == IndividualType.Secondary)
            //{
            //txtSpouseFName.Text = I.FirstName;
            //tbSpouseLastName.Text = I.LastName;
            //if (I.Birthday.HasValue)
            //    rdisecondDob.SelectedDate = I.Birthday.Value;
            ////if (I.Gender != string.Empty)
            ////    ddlGenderS.SelectedValue = I.Gender;

            //txtEmailSpouse.Text = I.Email;
            //chkEmailOptOutSpuse.Checked = I.IndividualEmailOptOut;
            //txtAddress1Secondary.Text = I.Address1;
            //txtAddress2Secondary.Text = I.Address2;
            //txtCitySecondary.Text = I.City;
            //if (I.StateID.HasValue)
            //{
            //    if (ddlStateSecondary.Items.FindByValue(I.StateID.ToString()) != null)
            //        ddlStateSecondary.SelectedValue = I.StateID.ToString();
            //}
            //txtZipCodeSecondary.Text = I.Zipcode;
            //}
        }
    }

    //SZ [Aug 27, 2013] added on the request by the client
    // Check if the top bar is to be coloured and what type of call is it. 
    //AccountId is required. internals are handled by SalesTool.Direct
    void UIColorTheBar()
    {
        CallType cType = CallType.Unknown;
        if (Session[K_LOGIN_BAR_COLOUR] != null)
        {
            string[] sAry = (Session[K_LOGIN_BAR_COLOUR].ToString() ?? ":").Split(':');
            if (sAry.Length == 2)
            {
                long account = default(long), calltype = default(long);
                long.TryParse(sAry[0], out account);
                long.TryParse(sAry[1], out calltype);
                if (account == AccountID)
                    cType = (CallType)calltype;
            }
        }
        else
        {
            var x = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
            cType = (CallType)x.GetCallType(CurrentUser.Key);
        }

        switch (cType)
        {
            case CallType.Inbound:
                Master.ColorTheBar("Inbound Call", "green");
                break;
            case CallType.Outbound:
                Master.ColorTheBar("Outbound Call", "red");
                break;
            case CallType.Manual:
                Master.ColorTheBar("Manual Call", "red");
                break;
        }
    }

    /// <summary>
    /// This function get the next lead from report data, and reload the leads page.
    /// </summary>
    private void GetNextReportLead()
    {
        btnNextLeadfromReport.Visible = true;

        if (Session["ReportData"] != null)
        {
            DataTable dtReportData = (DataTable)Session["ReportData"];
            DataRow nextRecord = null;
            //get accountid
            //var idx = 0;
            DataRow[] drResult = dtReportData.Select("ID=" + AccountID);
            foreach (var row in drResult)
            {
                var idx = dtReportData.Rows.IndexOf(row);
                if (idx + 1 < dtReportData.Rows.Count)
                {
                    nextRecord = dtReportData.Rows[idx + 1];
                }
            }

            if (nextRecord != null)
                Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + nextRecord["ID"]);
            else
                Response.Redirect("../Reports/ReportDisplay.aspx?reportid=" + GetUsersReports(CurrentUser.Key));
            // DataRow nextRecord = dtReportData.Rows[idx + 1];
        }
        else
            Response.Redirect("../Reports/ReportDisplay.aspx?reportid=" + GetUsersReports(CurrentUser.Key));
    }

    private void DisplayNextAccountReportBtn()
    {
        if (Request.UrlReferrer != null)
        {
            if ((Request.UrlReferrer.ToString().Contains("ReportDisplay.aspx".ToLower()) || Request.UrlReferrer.ToString().Contains("Leads/Leads.aspx") || Request.UrlReferrer.ToString().Contains("Leads/ApplyAction.aspx")) && Session["ReportId"] != null)
                btnNextLeadfromReport.Visible = true;
            else
            {
                if (Session["ReportId"] != null)
                {
                    Session.Remove("ReportId");
                    Session.Remove("ReportData");
                }
            }
        }
        else
        {
            if (Session["ReportId"] != null)
                btnNextLeadfromReport.Visible = true;
        }
    }

    /// <summary>
    /// this function returns the user reportid from memory or 
    /// from database for current login user. 
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    private Int64 GetUsersReports(Guid user)
    {
        Int64 reportId = Session["ReportId"] != null ? Convert.ToInt64(Session["ReportId"]) : 0;

        if (reportId < 1)
        {
            IEnumerable<ReportUsers> reports = Engine.CustomReportsAction.GetReportUsersByUserKey(user);
            if (reports.Any())
            {
                foreach (ReportUsers report in reports)
                {
                    reportId = report.ReportID;
                    break;
                }

            }
        }

        return reportId;
    }

    void EnableEditing(bool bEnable)
    {
        Helper.EnableControls(pnlPrimary, bEnable);
        //Helper.EnableControls(pnlSpouce, bEnable);
        Helper.EnableControls(pnlLead, bEnable);
        //Helper.EnableControls(divAgents, bEnable);
        //Helper.EnableControls(pnlControlsTop, bEnable);
        EnableLeadHeaderControls(bEnable);
        txtNotes.Enabled = txtLifeInfo.Enabled = bEnable;
        var list = GetChildControls();
        foreach (var C in list)
            C.ReadOnly = !bEnable;

        // Even if the controls are enabled TCPA Consent field would remain disabled.
        ddlConsent.Enabled = false;
    }

    private void EnableLeadHeaderControls(bool bEnable)
    {
        ddCampaigns.Enabled = bEnable;
        ddlStatus.Enabled = bEnable;


        ddlSubStatus1.Enabled = bEnable;
        ddUsers.Enabled = bEnable;
        ddCSR.Enabled = bEnable;
        ddTA.Enabled = bEnable;
        txtExternalAgent.Enabled = bEnable;
    }

    const string K_SHOULD_REDIRECT = "__Leads_Should_Redirect_QuickSave__";
    bool ShouldRedirectLeads
    {
        get { return Convert.ToBoolean(Session[K_SHOULD_REDIRECT] ?? default(bool)); }
        set { Session[K_SHOULD_REDIRECT] = value; }
    }

    int _tcpaId = 0;
    public void ProcessConsent(string controlId, TCPAConsentType choice)
    {
        switch (choice)
        {
            case TCPAConsentType.Blank: ddlConsent.SelectedValue = "0"; break;
            case TCPAConsentType.No: ddlConsent.SelectedValue = "2"; break;
            case TCPAConsentType.Yes: ddlConsent.SelectedValue = "1"; break;
            case TCPAConsentType.Undefined: ddlConsent.SelectedValue = "3"; break;
        }

        if (choice == TCPAConsentType.No && !Engine.ApplicationSettings.IsTermLife)
        {
            // Sz [Oct 3, 2013] Clear the field that caused the trigger
            System.Web.UI.Control ctl = string.Compare(controlId, "txtDayTimePhNo", true) == 0 ? txtDayTimePhNo :
                string.Compare(controlId, "txtEvePhNo", true) == 0 ? txtEvePhNo :
                string.Compare(controlId, "txtCellPhNo", true) == 0 ? txtCellPhNo : null;
            if (ctl != null && ctl is ITextControl)
                (ctl as ITextControl).Text = string.Empty;
        }

        if (ShouldRedirectLeads)
        {
            RedirectIfNewAccount(AccountID);
            ShouldRedirectLeads = false;
        }
    }

    public override void QuickAccountSave()
    {
        InnerSave(true);
        ShouldRedirectLeads = true;
    }
    //public override void QuickIndividualSave()
    //{
    //    Account A = Engine.AccountActions.Get(AccountID);
    //    if(A!=null)
    //    {
    //        Individual p = null;
    //        //MH:22 May 2014; save what is in Quick field
    //        p = A.Individuals.FirstOrDefault(x => x.Key == SelectedIndividualId);
    //        if (p != null)
    //        {
    //            GetIndividualDetails(ref p);
    //            Engine.AccountActions.Update(A);
    //        }
    //    }
    //}
    public override void ShowAlertBox()
    {
        //var dlg = (IndividualBox1 as IDialogBox).GetWindow();
        //dlg.Modal = false;
        dlgAlert.Show();
    }

    private void ShowEmailSender(bool IsApplyAction = false, int actionID = 0)
    {
        if (IsApplyAction)
        {
            var nEmailTemplates = Engine.LocalActions.GetManualEmailTemplates(actionID);
            if (nEmailTemplates.Count() == 1)
            {
                radWindowManualEmail.VisibleOnPageLoad = true;
                ctlEmailSender.Initialize();
                ctlEmailSender.HasCustomTemplate = false;
                ctlEmailSender.IsGeneralMode = false;
                int templateID = nEmailTemplates.FirstOrDefault().Id;
                ctlEmailSender.ChangeHeight(400);
                ctlEmailSender.LoadEmailTemplateData(templateID);

                //ctlEmailSender.BindCustomEmailTemplates(nEmailTemplates.OrderBy(ApplicationSettings.EmailOrderClause));


                //ctlEmailSender.DisableForm();

            }
            else if (nEmailTemplates.Count() > 1)
            {
                //ctlEmailSender.HasCustomTemplate = true;
                //ctlEmailSender.IsGeneralMode = false;
                //ctlEmailSender.BindCustomEmailTemplates(nEmailTemplates);
                radWindowManualEmail.VisibleOnPageLoad = true;
                ctlEmailSender.Initialize();
                ctlEmailSender.HasCustomTemplate = false;
                ctlEmailSender.IsGeneralMode = false;

                var T = nEmailTemplates.OrderBy(Engine.ApplicationSettings.EmailOrderClause).FirstOrDefault();
                int templateID = T.Id;
                ctlEmailSender.ChangeHeight(400);
                ctlEmailSender.LoadEmailTemplateData(templateID);

                //ctlEmailSender.BindCustomEmailTemplates(nEmailTemplates.OrderBy(ApplicationSettings.EmailOrderClause));


                //ctlEmailSender.DisableForm();

            }
        }
        else
        {
            int statusID = default(int);
            int.TryParse(ddlStatus.SelectedValue, out statusID);

            int substatusID = default(int);
            int.TryParse(ddlSubStatus1.SelectedValue, out substatusID);

            //

            radWindowManualEmail.VisibleOnPageLoad = true;
            ctlEmailSender.Initialize();
            ctlEmailSender.HasCustomTemplate = true;
            ctlEmailSender.IsGeneralMode = true;
            var T = Engine.StatusActions.GetManualEmailTemplatesForStatus(statusID, substatusID);
            ctlEmailSender.BindCustomEmailTemplates(T);
            ctlEmailSender.ChangeHeight(500);
            //ctlEmailSender.DisableForm();
        }
    }
    protected void btnCloseManualEmail_Click(object sender, EventArgs e)
    {
        radWindowManualEmail.VisibleOnPageLoad = false;
    }
    protected void btnQueueEmail_Click(object sender, EventArgs e)
    {
        if (ctlEmailSender.QueueEmail())
            radWindowManualEmail.VisibleOnPageLoad = false;
    }

    #region dynamic data tab loading process
    /*MH:
     * Concent behind this code block is to reduce the server side load by ploting tab control with requred tabs based on application
     * type to funtional this code 1.comments tabs and multiviews views collection except [(Notes and life)only view] and wire up tlkLeadPages with tlkLeadsPages_OnInit
     * event and wrap custom control with fieldset with class condado.
     * comment ArrangeTabsByApplicationMode funtionality 
     */
    /// <summary>
    /// Dynamically loads controls to tabs based on application type
    /// </summary>
    public void SetTabs()
    {

        if (Engine.ApplicationSettings.IsAutoHome)
        {
            BuildAutoHomeTabs();
        }
        else if (Engine.ApplicationSettings.IsSenior)
        {
            BuildSeniorTabs();
        }
        else if (Engine.ApplicationSettings.IsTermLife)
        {
            BuildTermLifeTabs();
        }
    }

    /// <summary>
    /// Load TermLife tabs to tabstrip and its associated controls to multipage view
    /// </summary>
    private void BuildTermLifeTabs()
    {

        //Leads and Marketing
        dynamic leadsAndMarketing = AddPageView("~/Leads/UserControls/LeadsMarketing.ascx", "pgLeadsMarketing");
        if (leadsAndMarketing != null)
            leadsAndMarketing.Initialize();

        // Attachments
        dynamic attachment = AddPageView("~/Leads/UserControls/AccountAttachments.ascx", "pgAttachments");
        if (attachment != null)
            attachment.Initialize();

        // Individuals
        dynamic indvInfo1 =
            AddPageView("~/Leads/UserControls/IndividualsInformation.ascx", "pgIndividuals", true);


        if (indvInfo1 != null)
            indvInfo1.Initialize();

        AddTabToStrip("Individuals", "pgIndividuals", true);
        AddTabToStrip("Leads &amp; Marketing", "pgLeadsMarketing");
        AddTabToStrip("Attachments", "pgAttachments");
        AddTabToStrip("Notes", "pgNotes");

        AddTabToStrip("Arc Cases", "pgArcCases");
    }

    /// <summary>
    /// Loads Senior application controls to tabstring and its assoicated control to multipageview
    /// </summary>
    private void BuildSeniorTabs()
    {

        // Carrior Issue Tracking
        dynamic issues = AddPageView("~/Leads/UserControls/carrierIssuesInformation.ascx", "pgCarrierIssues");
        if (issues != null)
            issues.Initialize();

        //Lead and Marketing
        dynamic leadsAndMarketing = AddPageView("~/Leads/UserControls/LeadsMarketing.ascx", "pgLeadsMarketing");
        if (leadsAndMarketing != null)
            leadsAndMarketing.Initialize();

        // Medical Supplement
        dynamic policyInfo1 = AddPageView("~/Leads/UserControls/MedicalSupplement.ascx", "pgMedicareSupplement");
        if (policyInfo1 != null)
            policyInfo1.Initialize();
        // Application Tracking

        dynamic appTracking = AddPageView("~/Leads/UserControls/applicationInformation.ascx", "pgApplicationTracking");
        if (appTracking != null)
            appTracking.Initialize();

        //MA & PDP
        dynamic mapdpInfo = AddPageView("~/Leads/UserControls/mapdpInformation.ascx", "pgMAPDP");
        if (mapdpInfo != null)
            mapdpInfo.Initialize();

        // Dental & Vision 
        dynamic dentalVisionInformation = AddPageView("~/Leads/UserControls/dentalVisionInformation.ascx",
                                                      "pgDentalVision");
        if (dentalVisionInformation != null)
            dentalVisionInformation.Initialize();

        // Attachments
        dynamic attachment = AddPageView("~/Leads/UserControls/AccountAttachments.ascx", "pgAttachments");
        if (attachment != null)
            attachment.Initialize();

        // Individuals
        dynamic indvInfo1 =
            AddPageView("~/Leads/UserControls/IndividualsInformation.ascx", "pgIndividuals", true);

        if (indvInfo1 != null)
            indvInfo1.Initialize();

        // Construction of Tabs
        AddTabToStrip("Individuals", "pgIndividuals", true);
        AddTabToStrip("Carrier Issue Tracking", "pgCarrierIssues");
        AddTabToStrip("Leads &amp; Marketing", "pgLeadsMarketing");
        AddTabToStrip("Medicare Supplement", "pgMedicareSupplement");
        AddTabToStrip("Application Tracking", "pgApplicationTracking");

        AddTabToStrip("MA &amp; PDP", "pgMAPDP");
        AddTabToStrip("Dental &amp; Vision", "pgDentalVision");
        AddTabToStrip("Attachments", "pgAttachments");
        AddTabToStrip("Notes", "pgNotes");

    }

    /// <summary>
    /// Loads AutoHome applications controls to tabstrip and its associated control to multipageview
    /// </summary>
    private void BuildAutoHomeTabs()
    {


        // Homes
        dynamic homeInfo = AddPageView("~/Leads/UserControls/HomeInformation.ascx", "pgHomeInfo");
        if (homeInfo != null)
            homeInfo.Initialize();

        // Drivers
        dynamic driverInfo = AddPageView("~/Leads/UserControls/driverInformation.ascx", "pgDrivers");
        if (driverInfo != null)
            driverInfo.Initialize();

        //Vehicles
        dynamic vehicleInfo = AddPageView("~/Leads/UserControls/VehicleInfo.ascx", "pgVehicleInfo");
        if (vehicleInfo != null)
            vehicleInfo.Initialize();

        //Policies
        dynamic autoHomePolicy1 = AddPageView("~/Leads/UserControls/AutoHomePolicy.ascx", "pgPolicies");
        if (autoHomePolicy1 != null)
            autoHomePolicy1.Initialize();
        //Quotes
        dynamic autoHomeQuote = AddPageView("~/Leads/UserControls/AutoHomeQuote.ascx", "pgQuotes");
        if (autoHomeQuote != null)
            autoHomeQuote.Initialize();

        //Carrier Issue Tracking
        dynamic issues =
            AddPageView("~/Leads/UserControls/carrierIssuesInformation.ascx", "pgCarrierIssues");
        if (issues != null)
            issues.Initialize();

        //Leads and Marketing
        dynamic leadsAndMarketing = AddPageView("~/Leads/UserControls/LeadsMarketing.ascx", "pgLeadsMarketing");
        if (leadsAndMarketing != null)
            leadsAndMarketing.Initialize();

        //Attachments
        dynamic attachment = AddPageView("~/Leads/UserControls/AccountAttachments.ascx", "pgAttachments");
        if (attachment != null)
            attachment.Initialize();

        // Individual
        dynamic indvInfo1 =
            AddPageView("~/Leads/UserControls/IndividualsInformation.ascx", "pgIndividuals", true);
        if (indvInfo1 != null)
            indvInfo1.Initialize();

        // constructions of tabs
        AddTabToStrip("Individuals", "pgIndividuals", true);
        AddTabToStrip("Homes", "pgHomeInfo");
        AddTabToStrip("Drivers", "pgDrivers");
        AddTabToStrip("Vehicles", "pgVehicleInfo");
        AddTabToStrip("Policies", "pgPolicies");
        AddTabToStrip("Quotes", "pgQuotes");
        AddTabToStrip("Carrier Issue Tracking", "pgCarrierIssues");
        AddTabToStrip("Leads &amp; Marketing", "pgLeadsMarketing");

        AddTabToStrip("Attachments", "pgAttachments");
        AddTabToStrip("Notes", "pgNotes");
        AddTabToStrip("Life Information", "pgLifeInfo");


    }

    /// <summary>
    /// Add tabs to RadStrip(tlkLeadTabs) 
    /// </summary>
    /// <param name="header">Header of the tab</param>
    /// <param name="pageViewId">Associated RadPageView Id</param>
    /// <param name="selected">selected or not</param>

    private void AddTabToStrip(string header, string pageViewId, bool selected = false)
    {
        RadTab tab = new RadTab(header) { PageViewID = pageViewId, SkinID = "blue", Selected = selected };
        tlkLeadsTabs.Tabs.Add(tab);
    }

    /// <summary>
    /// Add control to RadMultiPage and return custom control as object
    /// </summary>
    /// <param name="path">Location of the control</param>
    /// <returns>Object</returns>
    public object AddPageView(string path, string id, bool selected = false)
    {

        RadPageView pg = new RadPageView();
        pg.ID = id;
        pg.Selected = selected;
        var control = Page.LoadControl(path);
        pg.Controls.Add(control);
        tlkLeadsPages.PageViews.AddAt(0, pg);
        return control;
    }

    /// <summary>
    /// On initialize Event Handler for LeadPages(MultiPageView)
    /// </summary>
    /// <param name="sender">RadMultiPage</param>
    /// <param name="e">EventArgs</param>
    protected void tlkLeadsPages_OnInit(object sender, EventArgs e)
    {
        SetTabs();
    }

    #endregion

    protected void btnQuote_Click(object sender, EventArgs e)
    {
        string url = Engine.ApplicationSettings.ButtonQuoteURL + "?accountid=" + AccountID;
        //"http://aqe.condadogroup.com/ApplicantProfile.aspx?accountid=" + AccountID;

        string s = "window.open('" + url + "', '_blank');return false;";
        //ClientScript.RegisterStartupScript(this.GetType(), "script", s, true);
        RadScriptManager.RegisterStartupScript(Page, Page.GetType(), "1", s, true);
        //btnQuote.OnClientClick = s;
    }
    //const string K_INDIVIDUAL_ID = "account_individual_id
    //long IndividualID
    //{
    //    get
    //    {
    //        long lAns = ViewState[K_INDIVIDUAL_ID].ConvertOrDefault<long>(); 
    //        return lAns;
    //    }
    //    set
    //    {
    //        ViewState[K_INDIVIDUAL_ID] = value.ToString();
    //    }
    //}


    private void DisplayIndividualData()
    {
        //var id = Helper.SafeConvert<long>(ddlIndividual.SelectedValue);
        //MH:22 May 2014
        if (Engine.ApplicationSettings.IsTermLife && SelectedIndividualAccountId != AccountID)
            Response.Redirect(string.Format("Leads.aspx?by=&accountid={0}", SelectedIndividualAccountId));
        else
        {
            //MH:22 May 2014
            SetIndividualDetails(Engine.IndividualsActions.Get(SelectedIndividualId));
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
