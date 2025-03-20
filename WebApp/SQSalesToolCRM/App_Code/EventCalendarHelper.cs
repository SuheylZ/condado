using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using SQL = System.Data.Objects.SqlClient.SqlFunctions;
/// <summary>
/// Summary description for EventCalendarHelper
/// </summary>
public class EventCalendarHelper
{
    private System.Web.UI.Page _page;
    public System.Web.UI.Page Page { get { return _page; } }

    private SalesTool.DataAccess.DBEngine _engine = null;
    public SalesTool.DataAccess.DBEngine Engine
    {
        get
        {
            if (_engine == null)
            {
                _engine = new SalesTool.DataAccess.DBEngine();
                _engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
                //_engine.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
                _engine.Init(ApplicationSettings.ADOConnectionString);
            }
            return _engine;
        }
    }

    public EventCalendarHelper(System.Web.UI.Page page)
    {
        _page = page;
    }

    public void CloseWindow()
    {
        Type type = Page.GetType();
        string key = "CloseWindow";

        if (!Page.ClientScript.IsClientScriptBlockRegistered(type, key))
        {
            Page.ClientScript.RegisterClientScriptBlock(type, key, "this.close();", true);
        }
    }

    public string ConvertDateTimetoSeconds(DateTime dateTime)
    {
        TimeSpan timeSpan = dateTime.Subtract(DateTime.Now);

        // if Past Due
        if (timeSpan.TotalMilliseconds <= 0)
        {
            return "3000";
        }

        return dateTime.Subtract(DateTime.Now).TotalMilliseconds.ToString();
    }

    public string GetBaseUrl()
    {
        return this.GetBaseUrl("~/Leads/EventReminder.aspx");
    }

    public string GetBaseUrl(string relativePath)
    {
        return Page.ResolveUrl(relativePath);
    }

    private string SetReminderTimeOut(long eventId,bool? IsOpened, DateTime dateTime)
    {
        //var evt = Engine.EventCalendarActions.Get(eventId);

        //if (!ApplicationSettings.ShowEventPopup || Convert.ToBoolean(evt.IsOpened))
        if (!Engine.ApplicationSettings.ShowEventPopup || Convert.ToBoolean(IsOpened))
        {
            return "";
        }
        else
        {
            string script = "window.setTimeout(function() {";
            script += "window.open('" + this.GetBaseUrl() + "?EventId=" + eventId + "','_blank','height=200, width=600,status= no, resizable= no, scrollbars=no, toolbar=no,location=no,menubar=no ');";
            script += "}, " + this.ConvertDateTimetoSeconds(dateTime) + ");";



            return script;
        }
    }

    public void SetRemindersForNextHoursByUserID(Guid currentUserKey)
    {
        //if (!ApplicationSettings.ShowEventPopup)
        if (!Engine.ApplicationSettings.ShowEventPopup)
        {
            return;
        }

        var events = Engine.EventCalendarActions.GetNextHourByUserID(currentUserKey, Konstants.K_NEXT_HOURS).Where(x => x.AlertPopup).ToList();

        if (events.Count() > 0)
        {
            string script = "";
            
            foreach (var evt in events)
            {
                script += SetReminderTimeOut(evt.ID, evt.IsOpened,evt.SpecificDateTimeFromNow);
            }

            Type type = Page.GetType();
            string key = "RemindersForNextHoursByUserID";

            if (!Page.ClientScript.IsClientScriptBlockRegistered(type, key))
            {
                Page.ClientScript.RegisterClientScriptBlock(type, key, script, true);
            }
        }
    }

    public void SetReminderAfterAddNewEditOrSnooze(long eventId, string calledFrom)
    {
        var evt = Engine.EventCalendarActions.Get(eventId);

        if (evt == null)
        {
            return;
        }

        if (!evt.AlertPopup)
        {
            return;
        }

        if (evt.SpecificDateTimeFromNow.Subtract(DateTime.Now).TotalHours > Konstants.K_NEXT_HOURS)
        {
            return;
        }

        Type type = Page.GetType();
        string key = calledFrom + eventId.ToString();

        if (!Page.ClientScript.IsClientScriptBlockRegistered(type, key))
        {
            Page.ClientScript.RegisterClientScriptBlock(type, key, this.SetReminderTimeOut(evt.ID,evt.IsOpened, evt.SpecificDateTimeFromNow), true);
            return;
        }

        return; //SZ [Mar 26, 2103] what if the function forgets to return?
    }

    private void UpdateEventCalendarIsOpenedFlag()
    {
        SqlConnection sqlCon = new SqlConnection(ApplicationSettings.ADOConnectionString);
        sqlCon.Open();

        try
        {
            using (SqlCommand command = new SqlCommand("proj_UpdateEventCalendarIsOpenedFlag", sqlCon))
            {
                command.ExecuteNonQuery();
            }
        }
        catch
        {
            sqlCon.Close();
        }
        finally { sqlCon.Close(); }

    }

    public void SetNewMostRecentToHappenEventDateTime(long accountId = 0)
    {
        DateTime? dateTime = Engine.EventCalendarActions.GetMostRecentToHappenEventDateTimeByAccountId(accountId);

        SetNewMostRecentToHappenEventDateTime(accountId, dateTime);
    }

    public void SetNewMostRecentToHappenEventDateTime(long accountId = 0, DateTime? dateTime = null)
    {
        var account = Engine.AccountActions.Get(accountId);

        if (account == null)
        {
            return;
        }

        if (!account.NextEvenDate.HasValue && !dateTime.HasValue)
        {
            return;
        }

        if (account.NextEvenDate.HasValue && dateTime.HasValue)
        {
            if (account.NextEvenDate.Value.Equals(dateTime.Value))
            {
                return;
            }
        }

        account.NextEvenDate = dateTime;

        Engine.AccountActions.Update(account);
    }
    public void StoreLeadLastCalendarDetails(long leadKey, Guid userKey)
    {
        Lead L = Engine.LeadsActions.Get(leadKey);
        var U = Engine.UserActions.Get(userKey);
        if ((L == null || L.Account == null) || U == null) return;//New Account

        DateTime dt = DateTime.Now;
        Guid assignedUser = L.Account.AssignedUserKey.HasValue ? L.Account.AssignedUserKey.Value : Guid.Empty;


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
        string userName = "";
        if(U!=null)
            userName = U.FullName;
        Engine.LeadsActions.Update(L, userName);

    }

}