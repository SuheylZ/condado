using System;
using System.Drawing;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Linq;
using System.Collections.Generic;

namespace Leads.UserControls
{
    /// <summary>
    /// Developed by: Imran H.
    /// Dated:29.08.13
    /// Descriptions: 
    /// </summary>
    public partial class LeadsUserControlsUcCalandar : AccountsBaseControl
    {

        public static long SelectedAccountId { get; set; }
        // Delegate declaration 
        public delegate void OnButtonClick(string strValue);
        // Event declaration 
        public event OnButtonClick BtnHandler;
        /// <summary>
        /// On page load event the clander event will be displayed on user and current date basis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {


            // EntityDataSource1.Where = "it.[UserID]= GUID'" + SalesPage.CurrentUser.Key + "'";
            if (!IsPostBack)
                PageInitilization(DateTime.Now);
        }


        protected override void InnerInit()
        {
            IsGridMode = false;

        }



        /// <summary>
        /// This mentioned is called whwn 
        /// </summary>
        /// <param name="selectedDate"></param>
        private void PageInitilization(DateTime selectedDate)
        {
            try
            {

                var result = Engine.EventCalendarActions.GetByUserID(SalesPage.CurrentUser.Key).AsQueryable().Select(T => new
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
                    StatusId = T.EventStatus,
                    Description = T.Description
                });

                List<EventCalendarControlHelper> lstHelper = new List<EventCalendarControlHelper>();
                if (result.Count() > 0)
                {
                    EventCalendarControlHelper obj = new EventCalendarControlHelper();
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
                        //    obj = new EventCalendarControlHelper() { Description = v.Description, PrimaryName = v.UserName, ID = v.ID, SpecificDateTimeFromNow =
                        //        v.Date, //Helper.ConvertTimeFromUtc(v.Date, v.TimeZoneId, galOstTime, galDstTime), 
                        //        Status = v.Status, Title = v.Title, UserName = v.UserName, AccountID = v.AccountID, StatusId = v.StatusId, UserId = v.UserId };
                        //}
                        //else
                        //  obj = new EventCalendarControlHelper() { Description  = v.Description , PrimaryName = v.UserName , ID = v.ID, SpecificDateTimeFromNow = v.Date, Status = v.Status, Title = v.Title, UserName = v.UserName, AccountID = v.AccountID, StatusId = v.StatusId, UserId = v.UserId };
                       
                        DateTime dt = new DateTime();
                        dt = Helper.ConvertTimeFromUtc(v.Date, userTimeZoneID, galOstTime, galDstTime);
                       
                        obj = new EventCalendarControlHelper() { Description  = v.Description , PrimaryName = v.UserName , ID = v.ID, SpecificDateTimeFromNow = dt /*Changed Date as per users timezone!*/ , Status = v.Status, Title = v.Title, UserName = v.UserName, AccountID = v.AccountID, StatusId = v.StatusId, UserId = v.UserId };
                        lstHelper.Add(obj);
                    }
                }

                RadScheduler1.DataSource = lstHelper;
                RadScheduler1.DataBind();
                RadScheduler1.SelectedDate = selectedDate;

                RadCalendar1.SelectedDate = selectedDate;
                RadCalendar1.FocusedDate = RadCalendar1.SelectedDate;
                RadScheduler1.DayStartTime = new TimeSpan(6, 0, 0);
                RadScheduler1.DayEndTime = new TimeSpan(19, 0, 0);
            }
            catch (Exception ex)
            {

                lblMessage.Text = @"Error: " + ex.Message;
            }



        }

        /// <summary>
        /// For tool tips.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadScheduler1_AppointmentDataBound(object sender, SchedulerEventArgs e)
        {
            string colorAttribute = e.Appointment.Attributes["AppointmentColor"];

            if (!string.IsNullOrEmpty(colorAttribute))
            {
                int colorValue;
                if (int.TryParse(colorAttribute, out colorValue))
                {
                    var appointmentColor = Color.FromArgb(colorValue);
                    e.Appointment.BackColor = appointmentColor;
                    e.Appointment.BorderColor = Color.Black;
                    e.Appointment.BorderStyle = BorderStyle.Solid;
                    e.Appointment.BorderWidth = Unit.Pixel(1);
                }
            }
            e.Appointment.ToolTip = e.Appointment.Subject + ": " + e.Appointment.Description;
        }

        /// <summary>
        /// On calender date selection the user's evenet will be apperead.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadCalendar1_SelectionChanged(object sender, Telerik.Web.UI.Calendar.SelectedDatesEventArgs e)
        {
            if (!(RadCalendar1.SelectedDate.Date == DateTime.Parse("1/1/0001 12:00:00 AM") || RadCalendar1.SelectedDate.Date == DateTime.Parse("1/1/1900 12:00:00 AM")))
            {
                PageInitilization(RadCalendar1.SelectedDate);
                RadScheduler1.Rebind();
            }
        }
        /// <summary>
        /// Event add/edit popup will closed on btnCloseEventCalendar click.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnCloseEventCalendar_Click(object sender, EventArgs e)
        {
            dlgEventCalendar.VisibleOnPageLoad = false;

        }
        /// <summary>
        /// On btnAddEvent1 click, the event add edit popup will be apperead. user can add new events.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btnAddEvent1_Click(object sender, EventArgs e)
        {
            if (BtnHandler != null)
                BtnHandler(hdnAccountId.Value == "" ? "0" : hdnAccountId.Value);
            EventCalendarAddEdit1.Initialize();
            //EventCalendarAddEdit1.IsCalenderEventCall();
            dlgEventCalendar.VisibleOnPageLoad = true;



        }
        protected void btnRefresh_Click(object sender, EventArgs e)
        {

            PageInitilization(DateTime.Now);
            // RadScheduler1.Rebind();
        }
        /// <summary>
        /// On clander navigation command, the clander events selected on selected dates basis.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void RadScheduler1_NavigationCommand(object sender, SchedulerNavigationCommandEventArgs e)
        {

            switch (e.Command.ToString())
            {
                case "SwitchToSelectedDay":
                case "NavigateToSelectedDate":
                    PageInitilization(e.SelectedDate);
                    break;
            }

        }

        public override bool IsValidated
        {
            get
            {
                //this control does not hide or show grid so it must always return false
                return true;
            }
        }
        protected void RadCalendar1_DataBinding(object sender, EventArgs e)
        {

        }
    }
}
class EventCalendarControlHelper
{
    public string Title { get; set; }
    public string UserName { get; set; }
    public int Status { get; set; }
    public Int64 ID { get; set; }
    public DateTime SpecificDateTimeFromNow { get; set; }
    public Int64 AccountID { get; set; }
    public Guid UserId { get; set; }
    public int StatusId { get; set; }
    public string Description { get; set; }
    public string PrimaryName { get; set; }
}