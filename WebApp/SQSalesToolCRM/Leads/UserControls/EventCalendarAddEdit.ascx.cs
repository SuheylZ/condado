using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using System.Data;

public partial class Leads_UserControls_EventCalendarAddEdit : AccountsBaseControl, ICalenderNotifier
{
    //private void FillUserDropdown()
    //{

    //}

    //private void SetSelectedValue(DropDownList ddl, int? val)
    //{
    //    this.SetSelectedValue(ddl, (decimal?)val);
    //}
    //private void SetSelectedValue(DropDownList ddl, long? val)
    //{
    //    this.SetSelectedValue(ddl, (decimal?)val);
    //}
    //private void SetSelectedValue(DropDownList ddl, decimal? val)
    //{
    //    string v = val.HasValue ? val.Value.ToString() : "";

    //    this.SetSelectedValue(ddl, v);
    //}
    //private void SetSelectedValue<T>(DropDownList ddl, Nullable<T> val) where T:struct
    //{
    //    string sVal = (val?? default(T)).ToString();
    //    if (ddl.Items.Count>0)
    //    {
    //        if (ddl.Items.FindByValue(sVal)!= null)
    //            ddl.SelectedValue=sVal;
    //        else
    //            ddl.SelectedIndex = 0;
    //    }
    //}
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
    //    //if (masterPage != null)
    //    //{
    //    //    masterPage.buttonYes.Click += new EventHandler(CancelOnForm_Click);
    //    //}

    //    //[QN 4/4/2013] "Popup" and "Dismiss Upon Action" ...
    //    //...checkboxes needs to be checked by default.
    //    //Buttons Add New, Save and Cancel button must ...
    //    //...be hidden when this control is accessed on Apply action wizard
    //    if (this.IsOnActionWizard)
    //    {
    //        btnAddNew.Visible = false;
    //        btnCancelChanges.Visible = false;
    //        btnSaveAndCloseOnForm.Visible = false;
    //        btnSaveOnForm.Visible = false;
    //    }
    //   //comment ended

    //    //if (!Page.IsPostBack)
    //    //{
    //    //   // FillUserDropdown();

    //    //    //this.PagingNavigationBar.Visible = this.DisplayPagingBar;
    //    //    //this.grdEventCalendar.Visible = !this.HideEventsList;

    //    //    //InitForAccount();
    //    //}
    //}

    //////protected void Page_Load(object sender, EventArgs e)
    //////{
    //////    if (!Page.IsPostBack)
    //////    {
    //////        this.PagingNavigationBar.Visible = this.DisplayPagingBar;
    //////    }


    //////    if (!Page.IsPostBack)
    //////    {
    //////        BindgrdEventCalendar();
    //////    }
    //////}


    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
    //interface ILeadControlSave, in the accounts base page
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        bool bAns = false;



    //        return bAns;
    //    }
    //}
    //protected override void InnerInit()
    //{
    //    Refresh();
    //}
    //protected override void InnerLoad()
    //{
    //}
    //void SetCampaignTitle()
    //{
    //    var lead = Engine.LeadsActions.Get(Convert.ToInt64(Session[Konstants.K_LEAD_ID] ?? 0));
    //    if (lead != null)
    //    {
    //        var campaign = Engine.ManageCampaignActions.Get(lead.CampaignId ?? 0);

    //        if (campaign != null)
    //        {
    //            lbCampaign.Text = campaign.Title;
    //        }
    //    }
    //}

    bool HideEventsList
    {
        get
        {
            return hdnHideEventsList.Value == "1";
        }
        set
        {
            hdnHideEventsList.Value = value ? "1" : "0";
        }
    }

    //TM [16 june, 2014] Added the property to show or Hide the Events Grid
    public bool IsEventsGridvisible
    {
        get
        {
            return divEventsandButtons.Visible;
        }
        set
        {
            divEventsandButtons.Visible = value;
        }
    }

    //bool isPostBackCall = false;
    bool ShowCurrentUserEvents
    {
        get
        {
            return hdnShowCurrentUserEvents.Value == "1";
        }
        set
        {
            hdnShowCurrentUserEvents.Value = value ? "1" : "0";
        }
    }
    long EventID
    {
        get
        {
            return Helper.SafeConvert<long>(hdnFieldEditForm.Value);
        }
        set
        {
            hdnFieldEditForm.Value = value.ToString();
        }
    }
    string NewAddedEventIDs
    {
        get
        {
            return hdnNewAddedEventIDs.Value.TrimStart(',');
        }
        set
        {
            hdnNewAddedEventIDs.Value = value;
        }
    }
    bool IsEditMode
    {
        get
        {
            return hdnFieldIsEditMode.Value == "1";
        }
        set
        {
            hdnFieldIsEditMode.Value = value ? "1" : "0";
        }
    }
    bool IsOnActionWizard
    {
        get
        {
            return hdnIsOnActionWizard.Value == "1";
        }
        set
        {
            hdnIsOnActionWizard.Value = value ? "1" : "0";
        }
    }
    //IH[28.08.13] The is method is called form the event calander sechudler.
    public void IsCalenderEventCall()
    {
        InnerLoad(true);
    }



    void ClearFields(bool isNotSave)
    {
        //var entity = new EventCalendar();
        //[QN &  WM 4/4/2013] "Popup" and "Dismiss Upon Action" ...
        //...checkboxes needs to be checked by default.
        //entity.DismissUponStatusChange = System.Configuration.ConfigurationManager.AppSettings["DefaultCalendarDismiss"] == "1";
        //entity.AlertPopup = true;
        // this.MaptoForm(entity); 
        // SZ [Sep 6, 2013] What a rotten logic!!!
        // sending an empty record n letting the control decide what to do rather than properly initializing fields
        // This kind of crappy code written by an infamous teamember "WM" is prone to errors and issues always.
        // proper initializtion is given below.
        if (isNotSave)
        {
            lblMessageGrid.Text = string.Empty;
        }
        txtTitle.Text = string.Empty;
        txtDescription.Text = string.Empty;
        lbCampaign.Text = Engine.LeadsActions.GetCampaignTitle(Helper.SafeConvert<long>((Session[Konstants.K_LEAD_ID] ?? 0).ToString()));

        EventID = 0;
        IsEditMode = false;

        ddlUser.DataSource = Engine.UserActions.GetAll().OrderBy(x => x.LastName).Select(T => new { Key = T.Key, FullName = T.LastName + ", " + T.FirstName });
        ddlUser.DataBind();
        ddlUser.SelectedIndex = ddlUser.Items.IndexOf(ddlUser.Items.FindByValue(SalesPage.CurrentUser.Key.ToString()));

        hdnSelectedUsr.Value = SalesPage.CurrentUser.Key.ToString();

        rdoTimeFromNow.Checked = true;
        rdoSpecificDateTimeFromNow.Checked = false;

        ddlTimeFromNow.SelectedIndex = 0;
        rdpSpcificDateFromNow.SelectedDate = null;
        rtpSpcificTimeFromNow.SelectedTime = null;
        chkAlertMePopup.Checked = true;
        chkAlertMeEmail.Checked = false;
        chkAlertMeText.Checked = false;
        ddlAlertTimeBefore.SelectedIndex = 0;
        chkCreateOutlookCalendarReminder.Checked = false;
        //chkDismissUponStatusChange.Checked = ApplicationSettings.DefaultCalendarDismiss;//System.Configuration.ConfigurationManager.AppSettings["DefaultCalendarDismiss"] == "1";
        chkDismissUponStatusChange.Checked = Engine.ApplicationSettings.DefaultCalendarDismiss;//System.Configuration.ConfigurationManager.AppSettings["DefaultCalendarDismiss"] == "1";
        rdoEventTypeTask.Checked = false;
        rdoEventTypeAppointment.Checked = true;
    }

    protected override void InnerInit()
    {
        ClearFields(true);//Refresh();
        BindgrdEventCalendar();
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        //IH 29.08.12: 
        //FillUserDropdown();
        if (ddlUser.Items.Count == 0)
        {
            ddlUser.DataSource =
                Engine.UserActions.GetAll()
                      .OrderBy(x => x.LastName)
                      .Select(T => new { Key = T.Key, FullName = T.LastName + ", " + T.FirstName });
            ddlUser.DataBind();
            ddlUser.SelectedIndex = ddlUser.Items.IndexOf(ddlUser.Items.FindByValue(SalesPage.CurrentUser.Key.ToString()));
            if (hdnSelectedUsr.Value != string.Empty)
            {
                var selectedUsr = new Guid(hdnSelectedUsr.Value);
                if (selectedUsr != SalesPage.CurrentUser.Key)
                    ddlUser.SelectedIndex = ddlUser.Items.IndexOf(ddlUser.Items.FindByValue(selectedUsr.ToString()));
            }

        }
        if (bFirstTime)
        {
            this.PagingNavigationBar.Visible = this.DisplayPagingBar;
            //this.grdEventCalendar.Visible = !this.HideEventsList;
            //InitForAccount();
        }

        if (this.IsOnActionWizard)
        {
            btnAddNew.Visible = false;
            btnCancelChanges.Visible = false;
            btnSaveAndCloseOnForm.Visible = false;
            btnSaveOnForm.Visible = false;
        }
        if (!IsPostBack)
        {
            Int32? result = Engine.EventCalendarActions.GetEventTimeZone(this.AccountID, SalesPage.CurrentUser.Key);
            if (result != null)
            {
                ddlTimeZone.SelectedValue = result.ToString();
                ddlTimeZone.DataBind();
            }
        }
        //ddlUser.DataSource = Engine.UserActions.GetAll().OrderBy(x => x.LastName).Select(T => new { Key = T.Key, FullName = T.LastName + ", " + T.FirstName }).ToList();
        //ddlUser.DataBind();
        //ddlUser.SelectedIndex = ddlUser.Items.IndexOf(ddlUser.Items.FindByValue(SalesPage.CurrentUser.Key.ToString()));
    }
    public override bool IsValidated
    {
        get
        {
            bool bAns = false;
            return bAns;
        }
    }
    protected override void InnerSave()
    {
        SaveForm();
    }


    public void DeleteNewAddedEvents()
    {
        if (NewAddedEventIDs.Trim() == "")
        {
            return;
        }

        var Ids = NewAddedEventIDs.Split(',');

        foreach (var id in Ids)
        {
            Engine.EventCalendarActions.Delete(Helper.SafeConvert<long>(id));
        }

        NewAddedEventIDs = "";
    }
    public void DismissUponActionChange()
    {
        if (base.SalesPage == null || base.SalesPage.CurrentUser == null)
        {
            return;
        }
        bool hasEdited = Engine.EventCalendarActions.DismissUponActionChange(this.AccountID, NewAddedEventIDs);
        if (hasEdited) StoreLeadLastCalendarDetails(Helper.SafeConvert<long>((Session[Konstants.K_LEAD_ID] ?? 0).ToString()));
        NewAddedEventIDs = "";
    }

    //public void InitForAccount()
    //{
    //    ClearFields();
    //    BindgrdEventCalendar();
    //}

    void MaptoForm(long id = 0)
    {
        var entity = Engine.EventCalendarActions.Get(id);

        this.MaptoForm(entity);
    }
    void MaptoForm(EventCalendar entity)
    {
        lblMessageGrid.Text = "";
        this.EventID = entity.ID;
        this.IsEditMode = entity.ID != 0;

        lbCampaign.Text = Engine.LeadsActions.GetCampaignTitle(Helper.SafeConvert<long>((Session[Konstants.K_LEAD_ID] ?? 0).ToString()));

        ddlUser.SelectedIndex = ddlUser.Items.IndexOf(ddlUser.Items.FindByValue(entity.UserID.ToString()));
        txtTitle.Text = entity.Title;
        txtDescription.Text = entity.Description;

        rdoTimeFromNow.Checked = entity.IsTimeFromNow;
        rdoSpecificDateTimeFromNow.Checked = entity.IsSpecificDateTimeFromNow;

        if (entity.IsTimeFromNow)
        {
            ddlTimeFromNow.SelectedIndex = entity.TimeFromNow;

            rdpSpcificDateFromNow.SelectedDate = null;
            rtpSpcificTimeFromNow.SelectedTime = null;
        }
        else if (entity.IsSpecificDateTimeFromNow)
        {
            ddlTimeFromNow.SelectedIndex = 0;
            if (entity.TimeZoneId != null)
            {
                List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = Engine.EventCalendarActions.GetTimeZones().ToList();
                //TM [22 09 2014] get Current users Time zone ID and convert time to users timezone.

                int userTimeZoneID = (int)Engine.UserActions.Get(this.SalesPage.CurrentUser.Key, false).TimeZoneID;

                int galOstTime = lstGalTimezones.Find(tz => tz.Id == userTimeZoneID).IncrementOst.Value;
                int galDstTime = lstGalTimezones.Find(tz => tz.Id == userTimeZoneID).IncrementDst.Value;
                
                DateTime dt = Helper.ConvertTimeFromUtc(entity.SpecificDateTimeFromNow, userTimeZoneID, galOstTime, galDstTime);
                rdpSpcificDateFromNow.SelectedDate = dt.Date;
                rtpSpcificTimeFromNow.SelectedTime = dt.TimeOfDay;
                ddlTimeZone.SelectedValue = userTimeZoneID.ToString();
            }
            else
            {
                rdpSpcificDateFromNow.SelectedDate = entity.SpecificDateTimeFromNow.Date;
                rtpSpcificTimeFromNow.SelectedTime = entity.SpecificDateTimeFromNow.TimeOfDay;
                //ddlTimeZone.SelectedValue = entity.TimeZoneId.Value.ToString();
            }
        }

        chkAlertMePopup.Checked = entity.AlertPopup;
        chkAlertMeEmail.Checked = entity.AlertEmail;
        chkAlertMeText.Checked = entity.AlertTextSMS;
        ddlAlertTimeBefore.SelectedIndex = entity.AlertTimeBefore;
        chkCreateOutlookCalendarReminder.Checked = entity.CreateOutlookReminder;
        chkDismissUponStatusChange.Checked = entity.DismissUponStatusChange;
        rdoEventTypeTask.Checked = entity.EventType == 0;
        rdoEventTypeAppointment.Checked = entity.EventType == 1;
    } 
    
    void MaptoForm(vw_AccountEventCalendar entity)
    {
        lblMessageGrid.Text = "";
        this.EventID = entity.ID;
        this.IsEditMode = entity.ID != 0;

        lbCampaign.Text = Engine.LeadsActions.GetCampaignTitle(Helper.SafeConvert<long>((Session[Konstants.K_LEAD_ID] ?? 0).ToString()));

        ddlUser.SelectedIndex = ddlUser.Items.IndexOf(ddlUser.Items.FindByValue(entity.UserID.ToString()));
        txtTitle.Text = entity.Title;
        txtDescription.Text = entity.Description;

        rdoTimeFromNow.Checked = entity.IsTimeFromNow;
        rdoSpecificDateTimeFromNow.Checked = entity.IsSpecificDateTimeFromNow;

        if (entity.IsTimeFromNow)
        {
            ddlTimeFromNow.SelectedIndex = entity.TimeFromNow;

            rdpSpcificDateFromNow.SelectedDate = null;
            rtpSpcificTimeFromNow.SelectedTime = null;
        }
        else if (entity.IsSpecificDateTimeFromNow)
        {
            ddlTimeFromNow.SelectedIndex = 0;
            if (entity.TimeZoneId != null)
            {
                List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = Engine.EventCalendarActions.GetTimeZones().ToList();
                //TM [22 09 2014] get Current users Time zone ID and convert time to users timezone.

                int userTimeZoneID = (int)Engine.UserActions.Get(this.SalesPage.CurrentUser.Key, false).TimeZoneID;

                int galOstTime = lstGalTimezones.Find(tz => tz.Id == userTimeZoneID).IncrementOst.Value;
                int galDstTime = lstGalTimezones.Find(tz => tz.Id == userTimeZoneID).IncrementDst.Value;

                DateTime dt = Helper.ConvertTimeFromUtc(entity.SpecificDateTimeFromNow, userTimeZoneID, galOstTime, galDstTime);
                rdpSpcificDateFromNow.SelectedDate = dt.Date;
                rtpSpcificTimeFromNow.SelectedTime = dt.TimeOfDay;
                ddlTimeZone.SelectedValue = userTimeZoneID.ToString();
            }
            else
            {
                rdpSpcificDateFromNow.SelectedDate = entity.SpecificDateTimeFromNow.Date;
                rtpSpcificTimeFromNow.SelectedTime = entity.SpecificDateTimeFromNow.TimeOfDay;
                //ddlTimeZone.SelectedValue = entity.TimeZoneId.Value.ToString();
            }
        }

        chkAlertMePopup.Checked = entity.AlertPopup;
        chkAlertMeEmail.Checked = entity.AlertEmail;
        chkAlertMeText.Checked = entity.AlertTextSMS;
        ddlAlertTimeBefore.SelectedIndex = entity.AlertTimeBefore;
        chkCreateOutlookCalendarReminder.Checked = entity.CreateOutlookReminder;
        chkDismissUponStatusChange.Checked = entity.DismissUponStatusChange;
        rdoEventTypeTask.Checked = entity.EventType == 0;
        rdoEventTypeAppointment.Checked = entity.EventType == 1;
    }
    void MaptoEntity(EventCalendar entity)
    {
        var currentUserName = this.SalesPage.CurrentUser.FullName;

        try
        {

            if (this.EventID == 0)
            {
                entity.AccountID = this.AccountID;
                entity.EventStatus = 0;// Pending
                entity.Added.By1 = currentUserName;
            }
            else
            {
                entity.Changed.By1 = currentUserName;
            }

            entity.UserID = new Guid(ddlUser.SelectedValue);

            entity.Title = txtTitle.Text;
            entity.Description = txtDescription.Text;

            entity.IsTimeFromNow = rdoTimeFromNow.Checked;
            entity.IsSpecificDateTimeFromNow = rdoSpecificDateTimeFromNow.Checked;

            if (rdoTimeFromNow.Checked)
            {
                entity.TimeFromNow = ddlTimeFromNow.SelectedIndex;
                entity.SpecificDateTimeFromNow = Helper.AddTimeSpantoDate(DateTime.Now, ddlTimeFromNow.Text);
            }
            else //if (rdoSpecificDateTimeFromNow.Checked)
            {
                entity.TimeFromNow = 0;
                List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = Engine.EventCalendarActions.GetTimeZones().ToList();

                //int galOstTime = lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementOst.Value;
                //int galDstTime = lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementDst.Value;

                int galOstTime = 0;
                int galDstTime = 0;

                if (lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementOst.HasValue)
                    galOstTime = lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementOst.Value;
                if (lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementDst.HasValue)
                    galDstTime = lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementDst.Value;


                // SZ [Aug 15, 2014] Code for date time handling: this IS NOT NEEDED if database is setup correctly with default values for eventcalender table.
                // SQS database has defined default values for datetime, while sqah defaults were simply missing. this happens all the time when database is updated. 
                // As a safeguard, I put in the following code so even if the table does not have default values, it still works.
                // This is absolute nonsense that everytime some database update makes 
                // some existing thing goes missing and the dev team has to put in the code to handle that missing part which used to be there for almost a year! 
                DateTime dt;
                if (rdpSpcificDateFromNow.SelectedDate.HasValue)
                    dt = rdpSpcificDateFromNow.SelectedDate.Value;
                else
                    dt = DateTime.Today;
                TimeSpan ts;
                if (rtpSpcificTimeFromNow.SelectedTime.HasValue)
                    ts = rtpSpcificTimeFromNow.SelectedTime.Value;
                else
                    ts = TimeSpan.Zero;

                entity.SpecificDateTimeFromNow = Helper.ConvertTimeToUtc(dt, ts, Convert.ToInt32(ddlTimeZone.SelectedValue), galOstTime, galDstTime);
                //entity.SpecificDateTimeFromNow = Helper.ConvertTimeToUtc(rdpSpcificDateFromNow.SelectedDate, rtpSpcificTimeFromNow.SelectedTime, Convert.ToInt32(ddlTimeZone.SelectedValue), galOstTime, galDstTime);
                entity.TimeZoneId = Convert.ToInt32(ddlTimeZone.SelectedValue);
            }

            entity.AlertPopup = chkAlertMePopup.Checked;
            entity.AlertEmail = chkAlertMeEmail.Checked;
            entity.AlertTextSMS = chkAlertMeText.Checked;
            entity.AlertTimeBefore = ddlAlertTimeBefore.SelectedIndex;
            entity.CreateOutlookReminder = chkCreateOutlookCalendarReminder.Checked;
            entity.DismissUponStatusChange = chkDismissUponStatusChange.Checked;
            entity.EventType = rdoEventTypeTask.Checked ? 0 : 1;
        }
        catch (Exception ex)
        {
            Engine.AccountHistory.Log(AccountID, ex.ToString(), SalesPage.CurrentUser.Key);
        }
    }
    
    void MaptoEntity(vw_AccountEventCalendar entity)
    {
        var currentUserName = this.SalesPage.CurrentUser.FullName;

        try
        {
            
            if (this.EventID == 0)
            {
                entity.AccountID = this.AccountID;
                entity.EventStatus = 0;// Pending
                entity.Added.By1 = currentUserName;
            }
            else
            {
                entity.Changed.By1 = currentUserName;
            }

            entity.UserID = new Guid(ddlUser.SelectedValue);

            entity.Title = txtTitle.Text;
            entity.Description = txtDescription.Text;

            entity.IsTimeFromNow = rdoTimeFromNow.Checked;
            entity.IsSpecificDateTimeFromNow = rdoSpecificDateTimeFromNow.Checked;

            if (rdoTimeFromNow.Checked)
            {
                entity.TimeFromNow = ddlTimeFromNow.SelectedIndex;
                entity.SpecificDateTimeFromNow = Helper.AddTimeSpantoDate(DateTime.Now, ddlTimeFromNow.Text);
            }
            else //if (rdoSpecificDateTimeFromNow.Checked)
            {
                entity.TimeFromNow = 0;
                List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = Engine.EventCalendarActions.GetTimeZones().ToList();
                
                //int galOstTime = lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementOst.Value;
                //int galDstTime = lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementDst.Value;

                int galOstTime = 0;
                int galDstTime = 0;
                
                if(lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementOst.HasValue)
                    galOstTime = lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementOst.Value;
                if(lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementDst.HasValue)
                    galDstTime=lstGalTimezones.Find(r => r.Id == Convert.ToInt32(ddlTimeZone.SelectedValue)).IncrementDst.Value;
                

                // SZ [Aug 15, 2014] Code for date time handling: this IS NOT NEEDED if database is setup correctly with default values for eventcalender table.
                // SQS database has defined default values for datetime, while sqah defaults were simply missing. this happens all the time when database is updated. 
                // As a safeguard, I put in the following code so even if the table does not have default values, it still works.
                // This is absolute nonsense that everytime some database update makes 
                // some existing thing goes missing and the dev team has to put in the code to handle that missing part which used to be there for almost a year! 
                DateTime dt;
                if (rdpSpcificDateFromNow.SelectedDate.HasValue)
                    dt = rdpSpcificDateFromNow.SelectedDate.Value;
                else
                    dt = DateTime.Today;
                TimeSpan ts;
                if (rtpSpcificTimeFromNow.SelectedTime.HasValue)
                    ts = rtpSpcificTimeFromNow.SelectedTime.Value;
                else
                    ts = TimeSpan.Zero;

                entity.SpecificDateTimeFromNow = Helper.ConvertTimeToUtc(dt, ts, Convert.ToInt32(ddlTimeZone.SelectedValue), galOstTime, galDstTime);
                //entity.SpecificDateTimeFromNow = Helper.ConvertTimeToUtc(rdpSpcificDateFromNow.SelectedDate, rtpSpcificTimeFromNow.SelectedTime, Convert.ToInt32(ddlTimeZone.SelectedValue), galOstTime, galDstTime);
                entity.TimeZoneId = Convert.ToInt32(ddlTimeZone.SelectedValue);
            }

            entity.AlertPopup = chkAlertMePopup.Checked;
            entity.AlertEmail = chkAlertMeEmail.Checked;
            entity.AlertTextSMS = chkAlertMeText.Checked;
            entity.AlertTimeBefore = ddlAlertTimeBefore.SelectedIndex;
            entity.CreateOutlookReminder = chkCreateOutlookCalendarReminder.Checked;
            entity.DismissUponStatusChange = chkDismissUponStatusChange.Checked;
            entity.EventType = rdoEventTypeTask.Checked ? 0 : 1;
        }
        catch (Exception ex)
        {
            Engine.AccountHistory.Log(AccountID, ex.ToString(), SalesPage.CurrentUser.Key);
        }
    }
    void SaveForm(bool closeForm = false)
    {
        Page.Validate("EventCalendarVldGroup");
        if (Page.IsValid)
        {
            try
            {
                bool isEditModeBeforeSave = IsEditMode;
                if (!IsEditMode)
                {
                    var entity = new EventCalendar();
                    MaptoEntity(entity);
                    Engine.EventCalendarActions.Add(entity);
                    StoreLeadLastCalendarDetails(Helper.SafeConvert<long>((Session[Konstants.K_LEAD_ID] ?? 0).ToString()));
                    this.EventID = entity.ID;
                    NewAddedEventIDs += "," + entity.ID.ToString();
                    this.IsEditMode = true;
                }
                else
                {
                    var entity = Engine.EventCalendarActions.Get(this.EventID);
                    if (entity != null)
                    {
                        MaptoEntity(entity);
                        Engine.EventCalendarActions.Change(entity);
                        StoreLeadLastCalendarDetails(Helper.SafeConvert<long>((Session[Konstants.K_LEAD_ID] ?? 0).ToString()));
                    }
                }

                EventCalendarHelper eventCalendarHelper = new EventCalendarHelper((Page as SalesBasePage));
                eventCalendarHelper.SetNewMostRecentToHappenEventDateTime(this.AccountID);
                eventCalendarHelper.SetReminderAfterAddNewEditOrSnooze(this.EventID, isEditModeBeforeSave ? "Edit" : "AddNew");

                //SZ [Sep 6, 2103] notify the observers ab out the chnages
                NotifyChanges();

                if (!closeForm)
                {
                    BindgrdEventCalendar();
                    lblMessageGrid.Text = Messages.RecordSavedSuccess;

                    //if (!isEditModeBeforeSave)
                    //{
                    //    lblMessageForm.Text = Messages.RecordSavedSuccess;
                    //}
                    //else
                    //{
                    //    lblMessageForm.Text = Messages.RecordSavedSuccess;
                    //}
                }
                else
                    lblMessageGrid.Text = "";
            }
            catch (Exception ex)
            {
                lblMessageGrid.Text = "Error: " + ex.ToString();//.Message;
            }
        }
    }
  
    private void StoreLeadLastCalendarDetails(long leadKey)
    {
        Lead L = Engine.LeadsActions.Get(leadKey);
        if (L.Account == null) return;//New Account
        DateTime dt = DateTime.Now;
        Guid assignedUser = L.Account.AssignedUserKey.HasValue ? L.Account.AssignedUserKey.Value : Guid.Empty;
        var U = this.SalesPage.CurrentUser;
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

    public bool ShowOptionButtons
    {
        get
        {
            return hdnShowOptionButtons.Value == "1";
        }
        set
        {
            hdnShowOptionButtons.Value = value ? "1" : "0";
        }
    }
    public bool DisplayPagingBar
    {
        get
        {
            return hdnDisplayPagingBar.Value == "1";
        }
        set
        {
            hdnDisplayPagingBar.Value = value ? "1" : "0";
        }
    }

    protected void grdEventCalendar_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        lblMessageGrid.Text = "";
        if (e.CommandName == "EditX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string dataKeyValue = grdEventCalendar.DataKeys[row.RowIndex].Value.ToString();

            this.MaptoForm(Helper.SafeConvert<long>(dataKeyValue));
        }
        else if (e.CommandName == "CompleteX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string dataKeyValue = grdEventCalendar.DataKeys[row.RowIndex].Value.ToString();

            Engine.EventCalendarActions.Complete(Helper.SafeConvert<long>(dataKeyValue));
            EventCalendarHelper eventCalendarHelper = new EventCalendarHelper((Page as SalesBasePage));
            eventCalendarHelper.SetNewMostRecentToHappenEventDateTime(this.AccountID);

            lblMessageGrid.Text = "Status changed successfully.";
            BindgrdEventCalendar();
            NotifyChanges();
        }

        else if (e.CommandName == "DeleteX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string dataKeyValue = grdEventCalendar.DataKeys[row.RowIndex].Value.ToString();

            long key = Helper.SafeConvert<long>(dataKeyValue);

            if (key == EventID)
            {
                lblAlertMessage.Text = "You are deleting the event that you are editting.<br/>Are you sure you want to delete it?";
                dlgAlertBox.VisibleOnPageLoad = true;
                dlgAlertBox.Visible = true;
                return;
            }
            
            DeleteCalendarEvent(key);
        }
    }

    private void DeleteCalendarEvent(long key)
    {
        Engine.EventCalendarActions.Delete(key);
        EventCalendarHelper eventCalendarHelper = new EventCalendarHelper((Page as SalesBasePage));
        eventCalendarHelper.SetNewMostRecentToHappenEventDateTime(this.AccountID);

        lblMessageGrid.Text = Messages.RecordDeletedSuccess;
        BindgrdEventCalendar();
        NotifyChanges();
    }
    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        int size = e.PageSize;
        size = size > 100 ? 100 : size;
        grdEventCalendar.PageSize = size;
        BindgrdEventCalendar();
    }
    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        grdEventCalendar.PageIndex = e.PageNumber;
        BindgrdEventCalendar();
    }
    protected void grdEventCalendar_Sorting(object sender, GridViewSortEventArgs e)
    {
        try
        {
            if (SortColumn == e.SortExpression)
            {
                SortAscending = !SortAscending;
            }
            else
            {
                SortColumn = e.SortExpression;
                SortAscending = true;
            }

            BindgrdEventCalendar();
        }
        catch (Exception ex)
        {
            lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }
    protected void grdEventCalendar_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            grdEventCalendar.PageIndex = e.NewPageIndex;
            BindgrdEventCalendar();
        }
        catch (Exception ex)
        {
            lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }
    protected void AddNew_Click(object sender, EventArgs e)
    {
        ClearFields(true);
    }
    protected void Save_Click(object sender, EventArgs e)
    {
        if (ddlTimeZone.SelectedValue == "")
        {
            //get sp call
        }
        else
        {
            //selected hours in
        }
        lblMessageGrid.Text = "";
        SaveForm();
        ClearFields(false);
    }
    protected void SaveAndClose_Click(object sender, EventArgs e)
    {
        SaveForm(true);
    }
    protected void CancelChanges_Click(object sender, EventArgs e)
    {
        //if (this.EventID > 0)
        //    this.MaptoForm(this.EventID);
        //else
        ClearFields(true);
    }


    string SortColumn
    {
        get
        {
            return hdSortColumn.Value.Trim();
        }
        set
        {
            hdSortColumn.Value = value.Trim();
        }
    }
    bool SortAscending
    {
        get
        {
            bool bAsc = false;
            bool.TryParse(hdSortDirection.Value, out bAsc);
            return bAsc;
        }
        set
        {
            hdSortDirection.Value = value.ToString();
        }
    }


    protected string GetEventStatus(int eventStatus)
    {
        // 0 - Pending
        // 1 - Past Due
        // 2 - Completed
        // 3 - Dismissed

        var status = "Pending";
        switch (eventStatus)
        {
            case 1:
                status = "Past Due";
                break;
            case 2:
                status = "Completed";
                break;
            case 3:
                status = "Dismissed";
                break;
        }

        return status;
    }
    protected bool IsCompleted(int eventStatus)
    {
        return eventStatus == 2;
    }

    void BindgrdEventCalendar()
    {
        //TM [17 June 2014] removed the check to avoid the broken and empty grid of events
        //if (this.HideEventsList)
        //{
        //    return;
        //}

        try
        {
            //var result = Engine.EventCalendarActions.GetByAccountID(this.AccountID).AsEnumerable()
            //    .Select(T => new
            //    {
            //        ID = T.ID,
            //        Date = T.SpecificDateTimeFromNow,
            //        Title = T.Title,
            //        UserName = T.User == null ? "" : T.User.FirstName + " " + T.User.LastName,
            //        UserId = T.User == null ? new Guid() : T.User.Key,
            //        Status = this.GetEventStatus(T.EventStatus),
            //        IsCompleted = this.IsCompleted(T.EventStatus),

            //    }).AsQueryable();

            //if (ShowCurrentUserEvents)
            //{
            //    result = result.Where(x => x.UserId == this.SalesPage.CurrentUser.Key);
            //}

            //WM - [24.07.2013]
            // EventStatus==0, shows only pending events
            var result = Engine.EventCalendarActions.GetAll().AsQueryable()
                .Select(T => new
                {
                    ID = T.ID,
                    AccountID = T.AccountID,
                    UserId = T.UserID,
                    UserName = T.User == null ? "" : T.User.FirstName + " " + T.User.LastName,
                    
                    // SZ: Aug 26, 2014. Task 76, by Troy Assigned  
                    //Date = T.SpecificDateTimeFromNow,  
                    
                    Date =  T.SpecificDateTimeFromNow,//T.Added.On1.HasValue? T.Added.On1.Value : DateTime.MinValue,
                    Title = T.Title,
                    Status = (T.SpecificDateTimeFromNow < DateTime.Now && T.EventStatus == 0) ? 1 : T.EventStatus,//this.GetEventStatus(1) : this.GetEventStatus(T.EventStatus),
                    IsTimeFromNow = T.IsTimeFromNow,
                    TimeZoneId = T.TimeZoneId
                }).AsQueryable();

            if (ShowCurrentUserEvents)
            {
                result = result.Where(x => x.UserId == this.SalesPage.CurrentUser.Key);
            }
            else
            {
                result = result.Where(x => x.AccountID == this.AccountID);
            }
            if (result.Count() == 0)
            {
                DataSet ds = new DataSet();
                DataTable dt = new DataTable();
                dt.Clear();
                //dt.Columns.Add("Date");
                //dt.Columns.Add("Title");
                //dt.Columns.Add("UserName");
                //dt.Columns.Add("Status");
                //dt.Columns.Add("Id");                  
                dt.Columns.Add("Title", typeof(string));
                dt.Columns.Add("UserName", typeof(string));
                dt.Columns.Add("Status", typeof(int));
                dt.Columns.Add("Id", typeof(int));
                dt.Columns.Add("Date", typeof(DateTime));
                //////////////////////////
                DataRow _ravi = dt.NewRow();
                _ravi["Date"] = "01/01/2012";
                _ravi["Title"] = "";
                _ravi["UserName"] = "";
                _ravi["Status"] = 1;
                _ravi["Id"] = 1;
                dt.Rows.Add(_ravi);
                ds.Tables.Add(dt);
                grdEventCalendar.DataSource = ds.Tables[0];
                grdEventCalendar.DataBind();
                grdEventCalendar.Rows[0].Visible = false;
                lblNoRecords.Visible = true;
                //Date
                //Title
                //UserName
                //Status
            }
            else
            {
                List<CalendarHelper> lstHelper = new List<CalendarHelper>();
                CalendarHelper obj = new CalendarHelper();
                List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = Engine.EventCalendarActions.GetTimeZones().ToList();

                //TM [22 09 2014] get Current users Time zone ID and convert time to users timezone.
                int userTimeZoneID = (int)Engine.UserActions.Get(this.SalesPage.CurrentUser.Key, false).TimeZoneID;
                int galOstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementOst.Value;
                int galDstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementDst.Value;

                foreach (var v in result.AsEnumerable().ToList())
                {
                    //if (v.TimeZoneId != null && lstGalTimezones.Find(r => r.Id == v.TimeZoneId) != null)
                    //{
                        
                    //    obj = new CalendarHelper() { ID = v.ID, Date = v.Date, Status = v.Status, Title = v.Title, UserName = v.UserName };
                    //}
                    //else
                    //    obj = new CalendarHelper() { ID = v.ID, Date = v.Date, Status = v.Status, Title = v.Title, UserName = v.UserName };

                    DateTime dt = new DateTime();
                    dt = Helper.ConvertTimeFromUtc(v.Date, userTimeZoneID, galOstTime, galDstTime);

                    obj = new CalendarHelper() { ID = v.ID, Date = dt, Status = v.Status, Title = v.Title, UserName = v.UserName };
                    lstHelper.Add(obj);
                }

                grdEventCalendar.DataSource = PagingNavigationBar.ApplyPaging(lstHelper);
                grdEventCalendar.DataBind();

                lblNoRecords.Visible = false;
            }
        }
        catch (Exception ex)
        {
            lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }


    List<ICalenderNotification> _eventObservers = new List<ICalenderNotification>();
    public void Register(ICalenderNotification observer)
    {
        if (observer != null)
            _eventObservers.Add(observer);
    }
    void NotifyChanges()
    {
        foreach (ICalenderNotification item in _eventObservers)
            item.EventsUpdated();
    }
    protected void btnCloseEventCalendar_Click(object sender, EventArgs e)
    {
        CloseRadWindow();
    }
    private void CloseRadWindow()
    {
        String strPath = string.Format("Leads.aspx?accountid={0}&IsParentPopupClose=true", AccountID);
        lblCloseRadWindow.Text = "<script type='text/javascript'>SetPageAndClose(" + (char)(39) + strPath + (char)(39) + ");</script>";
    }

    protected void btnOk_Click(object sender, EventArgs e)
    {
        DeleteCalendarEvent(this.EventID);
        ClearFields(false);
        //lblMessageGrid.Text = Messages.RecordDeletedSuccess;
        dlgAlertBox.Visible = false;
        dlgAlertBox.VisibleOnPageLoad = false;
    }
    protected void btnCancelDelete_Click(object sender, EventArgs e)
    {
        dlgAlertBox.Visible = false;
        dlgAlertBox.VisibleOnPageLoad = false;
    }

}

class CalendarHelper
{
    public string Title { get; set; }
    public string UserName { get; set; }
    public int Status { get; set; }
    public Int64 ID { get; set; }
    public DateTime Date { get; set; }
}