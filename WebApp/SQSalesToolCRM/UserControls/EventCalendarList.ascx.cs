using System;
using System.Drawing;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using AjaxControlToolkit;

using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using System.Data.Objects;

public partial class Leads_UserControls_EventCalendarList : AccountsBaseControl, ICalenderNotification
{
    #region Members/Properties

    public bool ShowCurrentUserEvents
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

    public long CurrentEventID
    {
        get
        {
            return Helper.SafeConvert<long>(hdnCurrentEventID.Value);
        }
        set
        {
            hdnCurrentEventID.Value = value.ToString();
        }
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

    #endregion

    #region methods

    public void grdEventCalendar_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        lblMessageGrid.Text = "";

        if (e.CommandName == "SelectX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            //this.grdEventCalendar.SelectedIndex = row.RowIndex;

            long accountId = Helper.SafeConvert<long>(row.Cells[0].Text);
            Session[Konstants.K_ACCOUNT_ID] = accountId;

            var account = Engine.AccountActions.Get(accountId);

            if (account != null)
            {
                Session[Konstants.K_LEAD_ID] = account.PrimaryLeadKey;
            }

            Response.Redirect("~/Leads/Leads.aspx");

            //this.grdEventCalendar_SelectedIndexChanged(sender, new EventArgs());
        }
        else if (e.CommandName == "EditX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string dataKeyValue = grdEventCalendar.DataKeys[row.RowIndex].Value.ToString();

            this.CurrentEventID = Helper.SafeConvert<long>(dataKeyValue);
        }
        else if (e.CommandName == "CompleteX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string dataKeyValue = grdEventCalendar.DataKeys[row.RowIndex].Value.ToString();

            Engine.EventCalendarActions.Complete(Helper.SafeConvert<long>(dataKeyValue));
            lblMessageGrid.Text = "Status changed successfully.";
            BindgrdEventCalendar();
        }

        else if (e.CommandName == "DeleteX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string dataKeyValue = grdEventCalendar.DataKeys[row.RowIndex].Value.ToString();

            Engine.EventCalendarActions.Delete(Helper.SafeConvert<long>(dataKeyValue));
            lblMessageGrid.Text = Messages.RecordDeletedSuccess;
            BindgrdEventCalendar();
        }
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

    private string SortColumn
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
    private bool SortAscending
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

    public void grdEventCalendar_Sorting(object sender, GridViewSortEventArgs e)
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

    public void grdEventCalendar_PageIndexChanging(object sender, GridViewPageEventArgs e)
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

    protected override void InnerInit()
    {
        Refresh();
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
            case 0:
                status = "Pending";
                break;
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

    public void BindgrdEventCalendar()
    {
        try
        {
            //// var result = Engine.EventCalendarActions.GetAll().Where(x => !x.Dismissed && !x.Completed).AsEnumerable()
            //var result = Engine.EventCalendarActions.GetAll().Where(x => !x.Dismissed).AsEnumerable()
            //     .Select(T => new
            //     {
            //         ID = T.ID,
            //         AccountID = T.AccountID,
            //         UserId = T.UserID,
            //         UserName = T.User == null ? "" : T.User.FirstName + " " + T.User.LastName,
            //         Date = T.SpecificDateTimeFromNow,
            //         Title = T.Title,
            //         Status = this.GetEventStatus(T.EventStatus),
            //         IsCompleted = this.IsCompleted(T.EventStatus),
            //         StatusId = T.EventStatus
            //         //  }).Where(x => x.Date > DateTime.Now).OrderBy(x => x.Date).AsQueryable();
            //         //Imran H [12.08.13] show today's items raised by client.
            //         // }).Where(x => x.Date.Date == DateTime.Today).OrderBy(x => x.Date).AsQueryable();
            //     }).Where(x => x.Date.Date <= DateTime.Today).OrderBy(x => x.StatusId).ThenByDescending(x => x.Date).AsQueryable();
            int? type = (hdnEventType.Value == "" ? "-1" : hdnEventType.Value).ConvertOrDefault<int?>();

            var result = //current day && missed open events from prior days and current day
                (Engine.EventCalendarActions.GetAll().Where(x => (!x.Dismissed && !x.Completed) || (x.Completed && EntityFunctions.TruncateTime(x.SpecificDateTimeFromNow) == DateTime.Today)).AsQueryable()
                                .Select(T => new
                                    {
                                        ID = T.ID,
                                        AccountID = T.AccountID,
                                        UserId = T.UserID,
                                        UserName = T.User == null ? "" : T.User.FirstName + " " + T.User.LastName,
                                        Date = T.SpecificDateTimeFromNow,// ? T.Added.On1.Value : DateTime.MinValue,
                                        Title = T.Title,
                                        Status = (T.SpecificDateTimeFromNow < DateTime.Now && T.EventStatus == 0) ? 1 : T.EventStatus,//this.GetEventStatus(1) : this.GetEventStatus(T.EventStatus),
                                        IsTimeFromNow = T.IsTimeFromNow,
                                        TimeZoneId = T.TimeZoneId,
                                        StatusId = T.EventStatus
                                    })
                                .Where(x => EntityFunctions.TruncateTime(x.Date) <= DateTime.Today)
                                .OrderBy(x => x.StatusId)
                 .ThenBy(x => x.Date)
                                .AsQueryable());


            //YA[Aug 31, 2013] No need to union the record as same table is used, condition is applied for completed in above statement.
            //.Union
            //// Current day compelted events
            //(Engine.EventCalendarActions.GetAll().Where(x => x.Completed).AsEnumerable()
            //       .Select(T => new
            //           {
            //               ID = T.ID,
            //               AccountID = T.AccountID,
            //               UserId = T.UserID,
            //               UserName = T.User == null ? "" : T.User.FirstName + " " + T.User.LastName,
            //               Date = T.SpecificDateTimeFromNow,
            //               Title = T.Title,
            //               Status = this.GetEventStatus(T.EventStatus),
            //               IsCompleted = this.IsCompleted(T.EventStatus),
            //               StatusId = T.EventStatus

            //           }).Where(x => x.Date.Date == DateTime.Today)
            //.OrderBy(x => x.Date)
            //.AsQueryable().AsQueryable());


            result = ShowCurrentUserEvents ? result.Where(x => x.UserId == this.SalesPage.CurrentUser.Key) : result.Where(x => x.AccountID == AccountID);

            //if (ShowCurrentUserEvents)
            //{

            //    result = result.Where(x => x.UserId == this.SalesPage.CurrentUser.Key);
            //}
            //else
            //{
            //    result = result.Where(x => x.AccountID == this.AccountID);
            //}
            //-1 - all
            // 0 - Pending
            // 1 - Past Due
            // 2 - Completed
            // 3 - Dismissed
            if (type.HasValue && type != -1)
            {
                result = result.Where(p => p.Status == type);

            }
            List<EventCalendarListHelper> lstHelper = new List<EventCalendarListHelper>();
            if (result.Count() > 0)
            {
                EventCalendarListHelper obj = new EventCalendarListHelper();
                List<SalesTool.DataAccess.Models.TimeZone> lstGalTimezones = Engine.EventCalendarActions.GetTimeZones().ToList();

                //TM [23 09 2014] get Current users Time zone ID and convert time to users timezone.
                int userTimeZoneID = (int)Engine.UserActions.Get(this.SalesPage.CurrentUser.Key, false).TimeZoneID;
                int galOstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementOst.Value;
                int galDstTime = lstGalTimezones.Find(r => r.Id == userTimeZoneID).IncrementDst.Value;

                foreach (var v in result.AsEnumerable().ToList())
                {
                    //if (v.TimeZoneId != null && lstGalTimezones.Find(r => r.Id == v.TimeZoneId) != null)
                    //{
                    //    int galOstTime = lstGalTimezones.Find(r => r.Id == v.TimeZoneId).IncrementOst.Value;
                    //    int galDstTime = lstGalTimezones.Find(r => r.Id == v.TimeZoneId).IncrementDst.Value;
                    //    obj = new EventCalendarListHelper() { ID = v.ID, Date = v.Date, //Helper.ConvertTimeFromUtc(v.Date, v.TimeZoneId, galOstTime, galDstTime), 
                    //        Status = v.Status, Title = v.Title, UserName = v.UserName, AccountID = v.AccountID, StatusId = v.StatusId, UserId = v.UserId };
                    //}
                    //else
                    //    obj = new EventCalendarListHelper() { ID = v.ID, Date = v.Date, Status = v.Status, Title = v.Title, UserName = v.UserName, AccountID = v.AccountID, StatusId = v.StatusId, UserId = v.UserId };

                    DateTime dt = new DateTime();
                    dt = Helper.ConvertTimeFromUtc(v.Date, userTimeZoneID, galOstTime, galDstTime);

                    obj = new EventCalendarListHelper() { ID = v.ID, Date = dt, Status = v.Status, Title = v.Title, UserName = v.UserName, AccountID = v.AccountID, StatusId = v.StatusId, UserId = v.UserId };
                    lstHelper.Add(obj);
                }
            }
            grdEventCalendar.DataSource = PagingNavigationBar.ApplyPaging(lstHelper);
            grdEventCalendar.DataBind();
            if (grdEventCalendar.Rows.Count == 0)
            {
                grdEventCalendar.BorderWidth = new Unit("0px");
                pnlCalendar.ScrollBars = ScrollBars.None;
            }
        }
        catch (Exception ex)
        {
            lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }

    public int? EventType
    {
        get { return (hdnEventType.Value == "" ? "-1" : hdnEventType.Value).ConvertOrDefault<int?>(); }
    }

    public bool? EventTypeFilterApply
    {
        get { return (hdnEventTypeFilter.Value == "" ? "false" : hdnEventTypeFilter.Value).ConvertOrDefault<bool?>(); }
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        if (IsPostBack && EventTypeFilterApply == true)
        {
            BindgrdEventCalendar();
            hdnEventTypeFilter.Value = "false";
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.PagingNavigationBar.Visible = this.DisplayPagingBar;

            BindgrdEventCalendar();
        }
    }

    //public event EventHandler<EventArgs> RowChanged = null;

    //protected void grdEventCalendar_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (RowChanged != null)
    //    {
    //        RowChanged(this.grdEventCalendar, e);
    //    }
    //}

    protected void grdEventCalendar_RowCreated(object sender, GridViewRowEventArgs e)
    {
        if (e.Row != null && e.Row.RowType == DataControlRowType.DataRow)
        {
            PopupControlExtender pce = e.Row.FindControl("PopupControlExtender1") as PopupControlExtender;

            string behaviorID = "pce_" + e.Row.RowIndex;
            pce.BehaviorID = behaviorID;

            //string OnMouseOverScript = string.Format("$find('{0}').showPopup();", behaviorID);
            //string OnMouseOutScript = string.Format("$find('{0}').hidePopup();", behaviorID);
            string OnMouseOutScript = string.Format("$find('{0}');", behaviorID);

            //e.Row.Attributes.Add("onmouseover", OnMouseOverScript);
            e.Row.Attributes.Add("onmouseout", OnMouseOutScript);



            //e.Row.Attributes.Add("onclick", "grdEventCalendar_SelectedIndexChanged");

        }
    }

    [System.Web.Services.WebMethodAttribute(),
   System.Web.Script.Services.ScriptMethodAttribute()]
    public static string GetDynamicContent(string contextKey)
    {
        DBEngine engine = new DBEngine();
        engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());

        //engine.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        //engine.Init(ApplicationSettings.ADOConnectionString);
        engine.Init(ApplicationSettings.ADOConnectionString);

        var evt = engine.EventCalendarActions.Get(Helper.SafeConvert<long>(contextKey));

        

        string accountId = "&nbsp;";
        string primaryName = "&nbsp;";
        string lastAction = "&nbsp;";
        string lastActionDate = "&nbsp;";
        string status = "&nbsp;";
        string subStatus = "&nbsp;";
        string lastNotes = "&nbsp;";

        accountId = evt.AccountID.ToString();

        if (evt.Account != null)
        {
            var individual = engine.IndividualsActions.Get(evt.Account.PrimaryIndividualId ?? 0);
            if (individual != null)
            {
                primaryName = individual.FirstName + ", " + individual.LastName;
            }

            var lead = engine.LeadsActions.Get(evt.Account.PrimaryLeadKey ?? 0);
            if (lead != null)
            {
                var action = engine.LocalActions.Get(lead.ActionId ?? 0);
                if (action != null)
                {
                    lastAction = action.Title;
                }

                if (lead.LastActionDate != null)
                {
                    lastActionDate = lead.LastActionDate.Value.ToShortDateString();
                }

                if (lead.StatusL != null)
                {
                    status = lead.StatusL.Title;

                    var entitySubStatus = engine.StatusActions.Get(lead.SubStatusId ?? 0);
                    if (entitySubStatus != null)
                    {
                        subStatus = entitySubStatus.Title;
                    }
                }
            }
        }

        StringBuilder b = new StringBuilder();

        b.Append("<table style='background-color:#f3f3f3; border: #336699 3px solid; ");
        b.Append("width:350px; font-size:10pt; font-family:Verdana;' cellspacing='0' cellpadding='3'>");

        b.Append("<tr><th colspan='2' style='background-color:#336699; color:white;'>");
        b.Append("<b>Details</b>");
        b.Append("</th></tr>");

        b.Append("<tr><td><b>Account ID:</b></td>");
        b.Append("<td>" + accountId + "</td></tr>");

        b.Append("<tr><td><b>Primary Name:</b></td>");
        b.Append("<td>" + primaryName + "</td></tr>");

        b.Append("<tr><td><b>Last Action:</b></td>");
        b.Append("<td>" + lastAction + "</td></tr>");

        b.Append("<tr><td><b>Last Action Date:</b></td>");
        b.Append("<td>" + lastActionDate + "</td></tr>");

        b.Append("<tr><td><b>Status:</b></td>");
        b.Append("<td>" + status + "</td></tr>");

        b.Append("<tr><td><b>Sub Status:</b></td>");
        b.Append("<td>" + subStatus + "</td></tr>");

        b.Append("<tr><td><b>Last Notes:</b></td>");
        b.Append("<td>" + lastNotes + "</td></tr>");

        b.Append("</table>");

        return b.ToString();
    }
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
    }
    protected void grdEventCalendar_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //IH [14.08.13] Appointments list will display the following in calendar event datetime order: Missed open events from prior days and current day with a red background; Current day non passed open events with normal background; Completed items for the current day at the bottom with a grey background
        if (e.Row.RowType == DataControlRowType.DataRow && e.Row.DataItem != null)
        {
            int statusId = 0;
            string strStatus = int.TryParse(DataBinder.Eval(e.Row.DataItem, "Status").ToString(), out statusId) ? GetEventStatus(statusId) : DataBinder.Eval(e.Row.DataItem, "Status").ToString();
            if (Convert.ToDateTime(DataBinder.Eval(e.Row.DataItem, "Date")) < DateTime.Today &&
                (strStatus != "Completed"))
            {
                e.Row.BackColor = Color.Red;
                e.Row.ForeColor = Color.White;
            }
            else if (strStatus == "Completed")
                e.Row.BackColor = Color.Gray;

        }
    }

    public void EventsUpdated()
    {
        BindgrdEventCalendar();
    }
}

class EventCalendarListHelper
{
    public string Title { get; set; }
    public string UserName { get; set; }
    public int Status { get; set; }
    public Int64 ID { get; set; }
    public DateTime Date { get; set; }
    public Int64 AccountID { get; set; }
    public Guid UserId { get; set; }
    public int StatusId { get; set; }
}