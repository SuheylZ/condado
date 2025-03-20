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


public partial class Admin_AppointmentBuilder : SalesBasePage
{
    #region ----- Helper Functions -----

    private void PopulateAppointmentTypes()
    {
        ddlScheduleTypes.DataSource = Engine.AppointmentBuilderActions.GetAppointmentTypes();
        ddlScheduleTypes.DataTextField = "Name";
        ddlScheduleTypes.DataValueField = "Key";
        ddlScheduleTypes.DataBind();
    }

    private void PopulateAppointmentBuilders()
    {
        var builders = Engine.AppointmentBuilderActions.GetAppointmentBuilders();
        grdAppointmentBuilder.DataSource = builders;
        grdAppointmentBuilder.DataBind();

        if (builders != null)
        {
            if (builders.Count() == 0)
            {
                grdAppointmentBuilder.DataSource = new List<string>();
                grdAppointmentBuilder.DataBind();
            }
        }
    }

    #endregion

    #region Events

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

            divAddNew.Visible = false;
            divScheduleList.Visible = true;
        }
    }

    #endregion

    protected void btnAddNewSchedule_Click(object sender, EventArgs e)
    {
        try
        {
            lblError.Visible = false;

            var builder = new SalesTool.DataAccess.Models.AppointmentBuilder()
            {
                AddedBy = CurrentUser.FullName,
                AddedOn = DateTime.Now,
                IsDeleted = false,
                
                AppointmentTypeKey = long.Parse(ddlScheduleTypes.SelectedValue),
                Year = int.Parse(txtYear.Text)
            };

            // save new entry for appt_builder
            Engine.AppointmentBuilderActions.AddAppointmenBuilder(builder);

            // bind grig
            PopulateAppointmentBuilders();

            // show/hide controls
            btnCancelSchedule_Click(null, null);
        }
        catch (Exception ex)
        {
            if (ex.InnerException != null)
                lblError.Text = ex.InnerException.Message;
            else
                lblError.Text = ex.Message;

            lblError.Visible = true;
        }
    }
    protected void btnCancelSchedule_Click(object sender, EventArgs e)
    {
        divAddNew.Visible = false;
        divScheduleList.Visible = true;
    }

    protected void btnAddAppointment_Click(object sender, EventArgs e)
    {
        PopulateAppointmentTypes();

        divAddNew.Visible = true;
        divScheduleList.Visible = false;
        ddlScheduleTypes.Focus();
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        long id = 0;
        long.TryParse((sender as LinkButton).CommandArgument, out id);

        if (id > 0)
        {
            // delete appointment builder entry
            Engine.AppointmentBuilderActions.DeleteAppointmenBuilder(id, CurrentUser.FullName);

            // refresh grid
            PopulateAppointmentBuilders();
        }
    }
}