using SalesTool.DataAccess.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Leads_EventReminder : System.Web.UI.Page
{
    EventCalendarHelper eventCalendarHelper = null;

    Guid UserKey
    {
        get
        {
            Guid nKey = Guid.Empty;
            Guid.TryParse(hdnUserKey.Value, out nKey);
            return nKey;
        }
        set
        {
            hdnUserKey.Value = value.ToString();
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (eventCalendarHelper == null)
        {
            eventCalendarHelper = new EventCalendarHelper(Page);
        }
        
        if (!IsPostBack)
        {

            this.EventID = Helper.SafeConvert<long>(Request.QueryString["EventId"]);

            var evt = eventCalendarHelper.Engine.EventCalendarActions.Get(this.EventID);
            
            if (evt == null || evt.Dismissed || evt.Completed)
            {
                eventCalendarHelper.CloseWindow();

                return;
            }

            this.AccountId = evt.AccountID;
            UserKey = new Guid(Session[Konstants.K_USERID].ToString());

            lbEventTitle.Text = (evt.EventType == 0 ? "Task" : "Appointment") + " Calendar Event";
            //lbDateTime.Text = evt.SpecificDateTimeFromNow.ToString();
            lbTitle.Text = evt.Title;
            txtDesc.InnerHtml = evt.Description;


            //TM [01 10 2014] get Current users Time zone ID and convert time to users timezone.
            List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = eventCalendarHelper.Engine.EventCalendarActions.GetTimeZones().ToList();
            int userTimeZoneID = (int)eventCalendarHelper.Engine.UserActions.Get(UserKey, false).TimeZoneID;
            int galOstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementOst.Value;
            int galDstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementDst.Value;

            DateTime dt = new DateTime();
            dt = Helper.ConvertTimeFromUtc(evt.SpecificDateTimeFromNow, userTimeZoneID, galOstTime, galDstTime);
            lbDateTime.Text = dt.ToString();

            eventCalendarHelper.Engine.EventCalendarActions.OpenClose(this.EventID, true);
            if (Session[Konstants.K_USERID] != null)
            {
                
                Account A = eventCalendarHelper.Engine.AccountActions.Get(this.AccountId);
                if (A != null)
                {
                    Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
                    if(L != null)
                        eventCalendarHelper.StoreLeadLastCalendarDetails(L.Key, UserKey);
                }
            }
        }
    }

     
    public long AccountId
    {
        get
        {
            return Helper.SafeConvert<long>(hdnAccountId.Value);
        }
        set
        {
            hdnAccountId.Value = value.ToString();
        }
    }

    public long EventID
    {
        get
        {
            return Helper.SafeConvert<long>(hdnEventId.Value);
        }
        set
        {
            hdnEventId.Value = value.ToString();
        }
    }

    protected void Snooze_Click(object sender, EventArgs e)
    {
        DateTime dateTime = Helper.AddTimeSpantoDate(DateTime.Now, ddlTimeFromNow.Text);

        eventCalendarHelper.Engine.EventCalendarActions.Snooze(this.EventID, ddlTimeFromNow.SelectedIndex, dateTime);
        eventCalendarHelper.SetNewMostRecentToHappenEventDateTime(this.AccountId);
        eventCalendarHelper.SetReminderAfterAddNewEditOrSnooze(this.EventID, "Snooze");

        Account A = eventCalendarHelper.Engine.AccountActions.Get(this.AccountId);
        if (A != null)
        {
            Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
            if(L != null)
                eventCalendarHelper.StoreLeadLastCalendarDetails(L.Key, UserKey);
        }

        eventCalendarHelper.CloseWindow();
    }

    protected void Complete_Click(object sender, EventArgs e)
    {
        eventCalendarHelper.Engine.EventCalendarActions.Complete(this.EventID);
        eventCalendarHelper.SetNewMostRecentToHappenEventDateTime(this.AccountId);

        Account A = eventCalendarHelper.Engine.AccountActions.Get(this.AccountId);
        if (A != null)
        {
            Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
            if(L != null)
                eventCalendarHelper.StoreLeadLastCalendarDetails(L.Key, UserKey);
        }

        eventCalendarHelper.CloseWindow();
    }

    protected void Dismiss_Click(object sender, EventArgs e)
    {
        eventCalendarHelper.Engine.EventCalendarActions.Dismiss(this.EventID);
        eventCalendarHelper.SetNewMostRecentToHappenEventDateTime(this.AccountId);

        Account A = eventCalendarHelper.Engine.AccountActions.Get(this.AccountId);
        if (A != null)
        {
            Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
            if(L != null)
                eventCalendarHelper.StoreLeadLastCalendarDetails(L.Key, UserKey);
        }

        eventCalendarHelper.CloseWindow();
    }

    protected void OpenAccount_Click(object sender, EventArgs e)
    {
        eventCalendarHelper.Engine.EventCalendarActions.OpenClose(this.EventID, false);

        Account A = eventCalendarHelper.Engine.AccountActions.Get(this.AccountId);
        if (A != null)
        {
            Lead L = A.PrimaryLeadKey.HasValue ? A.Leads.FirstOrDefault(x => x.Key == A.PrimaryLeadKey) : A.Leads.FirstOrDefault();
            if(L != null)
                eventCalendarHelper.StoreLeadLastCalendarDetails(L.Key, UserKey);
        }

        eventCalendarHelper.CloseWindow();

        Session[Konstants.K_ACCOUNT_ID] = this.AccountId;

        var account = eventCalendarHelper.Engine.AccountActions.Get(this.AccountId);

        if (account != null)
        {
            Session[Konstants.K_LEAD_ID] = account.PrimaryLeadKey;
        }

        Type type = this.GetType().BaseType;
        string key = "OpenAccount";

        if (!Page.ClientScript.IsClientScriptBlockRegistered(type, key))
        {
            Page.ClientScript.RegisterClientScriptBlock(type, key, "window.opener.location.href='" + eventCalendarHelper.GetBaseUrl("~/Leads/Leads.aspx") + "'", true);
        }
    }
}