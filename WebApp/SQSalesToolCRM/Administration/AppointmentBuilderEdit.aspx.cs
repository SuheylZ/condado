using System;

using System.Linq;


using System.Web.UI;
using System.Web.UI.WebControls;
using UserControls;

using Mail = System.Net.Mail;
using System.Linq.Dynamic;

using DAL = SalesTool.DataAccess;
using Telerik.Web.UI;
using System.Data;
using System.Text;
using System.Collections.Generic;


public partial class Admin_AppointmentBuilderEdit : SalesBasePage
{
    #region ----- Properties -----

    private long ApptBuilderKey
    {
        get
        {
            long val = 0;
            long.TryParse(Request.QueryString["id"], out val);

            return val;
        }
    }

    private int ApptYear
    {
        get
        {
            int val = 0;
            int.TryParse(Request.QueryString["year"], out val);

            return val;
        }
    }

    private int ApptMonth
    {
        get
        {
            int month = 1;
            int.TryParse(ddlMonth.SelectedValue, out month);

            return month;
        }
    }

    private long ApptTypeKey
    {
        get
        {
            long val = 0;
            long.TryParse(Request.QueryString["type"], out val);

            return val;
        }
    }

    #endregion

    #region ----- Helper Functions -----

    private void PopulateAppointmentMonth(int month)
    {
        if (ApptBuilderKey > 0 && ApptYear > 0 && month > 0)
        {
            var apptMonths = Engine.AppointmentBuilderActions.GetAppointmentMonth(ApptBuilderKey, month);

            if (apptMonths != null)
            {
                hiddenApptMonthKey.Value = Convert.ToString(apptMonths.Key);

                btnSaveSchedule.Visible = false;
                btnUpdateSchedule.Visible = true;
                panelScheduleList.Visible = true;

                ddlMonth.SelectedValue = month.ToString();
                checkMon.Checked = apptMonths.Mon;
                checkTue.Checked = apptMonths.Tue;
                checkWed.Checked = apptMonths.Wed;
                checkThu.Checked = apptMonths.Thu;
                checkFri.Checked = apptMonths.Fri;
                checkSat.Checked = apptMonths.Sat;
                checkSun.Checked = apptMonths.Sun;

                txtIncrements.Text = apptMonths.Increment.ToString();

                PopulateSchedules(month);
                PopulateExceptions(month);

                // if schedules already generated, then disable buttons
                if (Engine.AppointmentBuilderActions.IsScheduleExists(apptMonths.Key))
                {
                    ShowHideGenerateSchedule(false);
                    
                //    btnUpdateSchedule.Visible = false;
                //    btnGenerateSch.Visible = false;

                //    ShowHideEditControls(false);
                }
                //else
                //{
                //    ShowHideEditControls(true);
                //}
            }
            else
            {
                // clear form
                hiddenApptMonthKey.Value = "";

                btnSaveSchedule.Visible = true;
                btnUpdateSchedule.Visible = false;
                panelScheduleList.Visible = false;


                checkMon.Checked = false;
                checkTue.Checked = false;
                checkWed.Checked = false;
                checkThu.Checked = false;
                checkFri.Checked = false;
                checkSat.Checked = false;
                checkSun.Checked = false;

                txtIncrements.Text = "";

                PopulateSchedules(month);
                PopulateExceptions(month);

                ShowHideGenerateSchedule(true);
            }
        }
    }

    /// <summary>
    /// Populate appointment builder information
    /// </summary>
    private void PopulateAppointmentBuilders()
    {
        if (ApptBuilderKey > 0)
        {
            var builder = Engine.AppointmentBuilderActions.GetAppointmentBuilder(ApptBuilderKey);

            if (builder != null)
            {
                literalType.Text = builder.AppointmentType.Name;
                literalYear.Text = builder.Year.ToString();
            }
        }
    }

    private void PopulateSchedules(int month)
    {
        btnGenerateSch.Visible = false;

        if (ApptBuilderKey > 0)
        {
            var schedules = Engine.AppointmentBuilderActions.GetAppointmentSchedules(ApptBuilderKey, month);

            if (schedules != null)
            {
                grdSchedules.DataSource = schedules;
                if (schedules.Count() > 0) btnGenerateSch.Visible = true;
            }
            else
            {
                grdSchedules.DataSource = new List<string>();
            }

            grdSchedules.DataBind();
        }


        // if schedule is already generated, then show update button instead of "Continue"
        long apptMonthKey = 0;
        long.TryParse(hiddenApptMonthKey.Value, out apptMonthKey);
        
        if (Engine.AppointmentBuilderActions.IsScheduleExists(apptMonthKey))
            ShowHideGenerateSchedule(false);
        else
            ShowHideGenerateSchedule(true);
    }        


    private void PopulateExceptions(int month)
    {
        if (ApptBuilderKey > 0)
        {
            try
            {
                txtExDate.MinDate = Convert.ToDateTime(month.ToString() + "/1/" + ApptYear.ToString());
                txtExDate.MaxDate = Convert.ToDateTime(month.ToString() + "/" + DateTime.DaysInMonth(ApptYear, month).ToString() + "/" + ApptYear.ToString());
            }
            catch { }

            var exceptions = Engine.AppointmentBuilderActions.GetAppointmentExceptions(ApptBuilderKey, month);

            if (exceptions != null)
                grdExceptions.DataSource = exceptions;
            else
                grdExceptions.DataSource = new List<string>();

            grdExceptions.DataBind();
        }
    }

    private void DisplayException(Exception ex)
    {
        if (ex.InnerException != null)
            lblError.Text = ex.InnerException.Message;
        else
            lblError.Text = ex.Message;

        lblError.Visible = true;
    }

    internal void ShowHideEditControls(bool isVisible)
    {
        btnGeneratedSchedulesImport.Visible = isVisible;
        btnGeneratedSchedulesUpdate.Visible = !isVisible;

        btnAddSchedule.Visible = isVisible;
        btnAddException.Visible = isVisible;

        grdSchedules.Columns[3].Visible = isVisible;
        grdExceptions.Columns[4].Visible = isVisible;
    }

    internal void ShowHideGenerateSchedule(bool isVisible)
    {
        btnGeneratedSchedulesImport.Visible = isVisible;
        btnGeneratedSchedulesUpdate.Visible = !isVisible;
    }

    #endregion

    #region ----- Events -----

    protected void Page_Init(object sender, EventArgs e)
    {
        if (!ClientScript.IsStartupScriptRegistered(GetType(), "MaskedEditFix"))
        {
            ClientScript.RegisterStartupScript(GetType(), "MaskedEditFix", String.Format("<script type='text/javascript' src='{0}'></script>", Page.ResolveUrl("/Scripts/MaskedEditFix.js")));
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            PopulateAppointmentBuilders();
            PopulateAppointmentMonth(1);
        }
    }

    protected void ddlMonth_SelectedIndexChanged(object sender, EventArgs e)
    {
        PopulateAppointmentMonth(ApptMonth);
    }

    protected void btnSaveSchedule_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Visible = false;

            if (checkSun.Checked == false && checkMon.Checked == false && checkTue.Checked == false && checkWed.Checked == false && checkThu.Checked == false && checkFri.Checked == false && checkSat.Checked == false)
            {
                lblError.Text = "Please select days";
                lblError.Visible = true;
                return;
            }

            var apptMonth = new SalesTool.DataAccess.Models.AppointmentMonth()
            {
                AppointmentBuilderKey = ApptBuilderKey,
                Sun = checkSun.Checked,
                Mon = checkMon.Checked,
                Tue = checkTue.Checked,
                Wed = checkWed.Checked,
                Thu = checkThu.Checked,
                Fri = checkFri.Checked,
                Sat = checkSat.Checked,
                Increment = int.Parse(txtIncrements.Text),
                Month = ApptMonth,
                AddedBy = CurrentUser.FullName,
                AddedOn = DateTime.Now,
                IsDeleted = false
            };

            Engine.AppointmentBuilderActions.AddAppointmentMonth(apptMonth);

            hiddenApptMonthKey.Value = apptMonth.Key.ToString();
            ShowHideEditControls(true);

            panelScheduleList.Visible = true;
            btnSaveSchedule.Visible = false;
            btnUpdateSchedule.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    protected void btnUpdateSchedule_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Visible = false;

            if (checkSun.Checked == false && checkMon.Checked == false && checkTue.Checked == false && checkWed.Checked == false && checkThu.Checked == false && checkFri.Checked == false && checkSat.Checked == false)
            {
                lblError.Text = "Please select days";
                lblError.Visible = true;
                return;
            }

            var apptMonth = new SalesTool.DataAccess.Models.AppointmentMonth()
            {
                Key = long.Parse(hiddenApptMonthKey.Value),
                AppointmentBuilderKey = ApptBuilderKey,
                Sun = checkSun.Checked,
                Mon = checkMon.Checked,
                Tue = checkTue.Checked,
                Wed = checkWed.Checked,
                Thu = checkThu.Checked,
                Fri = checkFri.Checked,
                Sat = checkSat.Checked,
                Increment = int.Parse(txtIncrements.Text),
                Month = ApptMonth,
                ModifiedBy = CurrentUser.FullName,
                ModifiedOn = DateTime.Now,
                IsDeleted = false
            };

            Engine.AppointmentBuilderActions.UpdateAppointmentMonth(apptMonth);

            panelScheduleList.Visible = true;
            btnSaveSchedule.Visible = false;
            btnUpdateSchedule.Visible = true;
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    protected void btnGenerateSch_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";

            var schedules = Engine.AppointmentBuilderActions.GenerateSchedules(ApptBuilderKey, ApptYear, long.Parse(hiddenApptMonthKey.Value));
            grdGeneratedSchedules.DataSource = schedules;
            grdGeneratedSchedules.DataBind();

            Session["GeneratedSchedules"] = schedules;

            dlgGeneratedSchedules.VisibleOnPageLoad = true;
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    protected void btnGeneratedSchedulesCancel_Click(object sender, EventArgs e)
    {
        dlgGeneratedSchedules.VisibleOnPageLoad = false;
    }

    protected void btnGeneratedSchedulesImport_Click(object sender, EventArgs e)
    {
        try
        {
            //var schedules = grdGeneratedSchedules.DataSource as List<SalesTool.DataAccess.Models.Schedules>;
            //var schedules = Engine.AppointmentBuilderActions.GenerateSchedules(ApptBuilderKey, ApptYear, long.Parse(hiddenApptMonthKey.Value));
            var schedules = Session["GeneratedSchedules"] as List<SalesTool.DataAccess.Models.Schedules>;

            if (schedules != null)
            {
                dlgGeneratedSchedules.VisibleOnPageLoad = false;
                Engine.AppointmentBuilderActions.SaveSchedules(schedules, ApptTypeKey, long.Parse(hiddenApptMonthKey.Value));

                ShowHideGenerateSchedule(false);

                //btnUpdateSchedule.Visible = false;
                //btnGenerateSch.Visible = false;

                //ShowHideEditControls(false);
            }
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    protected void btnGeneratedSchedulesUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            var schedules = Session["GeneratedSchedules"] as List<SalesTool.DataAccess.Models.Schedules>;

            if (schedules != null)
            {
                dlgGeneratedSchedules.VisibleOnPageLoad = false;
                Engine.AppointmentBuilderActions.UpdateSchedules(schedules, ApptTypeKey, long.Parse(hiddenApptMonthKey.Value));

                ShowHideGenerateSchedule(false);

                //btnUpdateSchedule.Visible = false;
                //btnGenerateSch.Visible = false;

                //ShowHideEditControls(false);
            }
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    #endregion

    #region ----- Schedules -----

    protected void btnAddSchedule_Click(object sender, EventArgs e)
    {
        //panelNewSchedule.Visible = true;
        //panelGridSchedules.Visible = false;

        lblError.Text = "";
        dlgSchedules.VisibleOnPageLoad = true;
        dlgSchedules.Title = "Add Schedule";
        btnSchOkay.Visible = true;
        btnSchUpdate.Visible = false;
    }
    protected void btnSchCancel_Click(object sender, EventArgs e)
    {
        //panelGridSchedules.Visible = true;
        //panelNewSchedule.Visible = false;
        dlgSchedules.VisibleOnPageLoad = false;
    }
    protected void btnSchOkay_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            var schedule = new SalesTool.DataAccess.Models.AppointmentSchedules()
            {
                AppointmentMonthKey = long.Parse(hiddenApptMonthKey.Value),
                StartTime = txtSchStart.SelectedDate,
                EndTime = txtSchEnd.SelectedDate,
                Spaces = int.Parse(txtSchSpaces.Text),
                AddedBy = CurrentUser.FullName,
                AddedOn = DateTime.Now,
                IsDeleted = false
            };

            // save schedule
            Engine.AppointmentBuilderActions.AddAppointmentSchedule(schedule);
            PopulateSchedules(ApptMonth);

            panelScheduleList.Visible = true;
            dlgSchedules.VisibleOnPageLoad = false;

            txtSchStart.Clear();
            txtSchEnd.Clear();
            txtSchSpaces.Text = "";
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }
    protected void btnSchDelete_Click(object sender, EventArgs e)
    {
        lblError.Text = "";
        long id = 0;
        long.TryParse((sender as LinkButton).CommandArgument, out id);

        if (id > 0)
        {
            Engine.AppointmentBuilderActions.DeleteAppointmenSchedule(id, CurrentUser.FullName);
            PopulateSchedules(ApptMonth);
        }
    }
    protected void btnSchEdit_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            hiddenScheduleKey.Value = "";

            // Load schedule info in popup, for edit
            txtSchStart.SelectedDate = DateTime.ParseExact(((sender as LinkButton).NamingContainer as GridViewRow).Cells[0].Text, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture); // start time
            txtSchEnd.SelectedDate = DateTime.ParseExact(((sender as LinkButton).NamingContainer as GridViewRow).Cells[1].Text, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture); // end time
            txtSchSpaces.Text = ((sender as LinkButton).NamingContainer as GridViewRow).Cells[2].Text; // spaces
            hiddenScheduleKey.Value = (sender as LinkButton).CommandArgument; // schedule key

            btnSchUpdate.Visible = true;
            btnSchOkay.Visible = false;

            dlgSchedules.VisibleOnPageLoad = true;
            dlgSchedules.Title = "Update Schedule";
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }
    protected void btnSchUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            var schedule = new SalesTool.DataAccess.Models.AppointmentSchedules()
            {
                Key = long.Parse(hiddenScheduleKey.Value),
                StartTime = txtSchStart.SelectedDate,
                EndTime = txtSchEnd.SelectedDate,
                Spaces = int.Parse(txtSchSpaces.Text),
                ModifiedBy = CurrentUser.FullName,
                ModifiedOn = DateTime.Now
            };

            // save schedule
            Engine.AppointmentBuilderActions.UpdateAppointmentSchedule(schedule);
            PopulateSchedules(ApptMonth);


            dlgSchedules.VisibleOnPageLoad = false;
            txtSchStart.Clear();
            txtSchEnd.Clear();
            txtSchSpaces.Text = "";
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    #endregion

    #region ----- Exceptions ------

    protected void btnAddException_Click(object sender, EventArgs e)
    {
        dlgExceptions.VisibleOnPageLoad = true;
        dlgExceptions.Title = "Add Exception";
        btnExOkay.Visible = true;
        btnExUpdate.Visible = false;
    }

    protected void btnExCancel_Click(object sender, EventArgs e)
    {
        dlgExceptions.VisibleOnPageLoad = false;
    }
    protected void btnExOkay_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            var exception = new SalesTool.DataAccess.Models.AppointmentExceptions()
            {
                AppointmentMonthKey = long.Parse(hiddenApptMonthKey.Value),
                ExceptionDate = txtExDate.SelectedDate,
                StartTime = txtExStart.SelectedDate,
                EndTime = txtExEnd.SelectedDate,
                Spaces = int.Parse(txtExSpaces.Text),
                AddedBy = CurrentUser.FullName,
                AddedOn = DateTime.Now,
                IsDeleted = false
            };

            // save schedule
            Engine.AppointmentBuilderActions.AddAppointmentException(exception);
            PopulateExceptions(ApptMonth);

            panelScheduleList.Visible = true;
            dlgExceptions.VisibleOnPageLoad = false;

            txtExDate.Clear();
            txtExStart.Clear();
            txtExEnd.Clear();
            txtExSpaces.Text = "";
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }
    protected void btnExDelete_Click(object sender, EventArgs e)
    {
        long id = 0;
        long.TryParse((sender as LinkButton).CommandArgument, out id);

        if (id > 0)
        {
            Engine.AppointmentBuilderActions.DeleteAppointmenException(id, CurrentUser.FullName);
            PopulateExceptions(ApptMonth);
        }
    }
    protected void btnExEdit_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";
            hiddenExceptionKey.Value = "";

            // Load exception info in popup, for edit
            txtExDate.SelectedDate = DateTime.ParseExact(((sender as LinkButton).NamingContainer as GridViewRow).Cells[0].Text, "MM/dd/yyyy", System.Globalization.CultureInfo.InvariantCulture); // date
            txtExStart.SelectedDate = DateTime.ParseExact(((sender as LinkButton).NamingContainer as GridViewRow).Cells[1].Text, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture); // start time
            txtExEnd.SelectedDate = DateTime.ParseExact(((sender as LinkButton).NamingContainer as GridViewRow).Cells[2].Text, "h:mm tt", System.Globalization.CultureInfo.InvariantCulture); // end time
            txtExSpaces.Text = ((sender as LinkButton).NamingContainer as GridViewRow).Cells[3].Text; // spaces
            hiddenExceptionKey.Value = (sender as LinkButton).CommandArgument; // exception key

            btnExUpdate.Visible = true;
            btnExOkay.Visible = false;

            dlgExceptions.VisibleOnPageLoad = true;
            dlgExceptions.Title = "Update Exception";
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }
    protected void btnExUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Text = "";

            var exception = new SalesTool.DataAccess.Models.AppointmentExceptions()
            {
                Key = long.Parse(hiddenExceptionKey.Value),
                ExceptionDate = txtExDate.SelectedDate,
                StartTime = txtExStart.SelectedDate,
                EndTime = txtExEnd.SelectedDate,
                Spaces = int.Parse(txtExSpaces.Text),
                ModifiedBy = CurrentUser.FullName,
                ModifiedOn = DateTime.Now
            };

            // save schedule
            Engine.AppointmentBuilderActions.UpdateAppointmentException(exception);
            PopulateExceptions(ApptMonth);


            dlgExceptions.VisibleOnPageLoad = false;
            txtExStart.Clear();
            txtExEnd.Clear();
            txtExSpaces.Text = "";
        }
        catch (Exception ex)
        {
            DisplayException(ex);
        }
    }

    #endregion

}