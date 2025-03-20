using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Configuration;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;
using System.Web.Security;
using System.Web.Profile;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
namespace SQS_Dialer
{
	public partial class PVScheduleAgent : System.Web.UI.UserControl
	{
		//YA[Oct 01, 13]
		#region "Constants/Properties"
		public bool IsAgent {
			get {
				bool _IsAgent = false;
				bool.TryParse(hdnFieldIsAgent.Value, out _IsAgent);
				return _IsAgent;
			}
			set { hdnFieldIsAgent.Value = value.ToString(); }
		}

		public bool IsFormMode {
			get {
				bool _IsFormMode = false;
				bool.TryParse(hdnFieldIsFormMode.Value, out _IsFormMode);
				return _IsFormMode;
			}
			set {
				if (value == true) {
					SetFormMode();
				} else {
					SetGridMode();
				}
				hdnFieldIsFormMode.Value = value.ToString();
			}
		}

		public Guid RecordKey {
			get {
				Guid _RecordKey = Guid.Empty;
				Guid.TryParse(hdnFieldRecordKey.Value, out _RecordKey);
				return _RecordKey;
			}
			set { hdnFieldRecordKey.Value = value.ToString(); }
		}

		public Int64 EditRecordKey {
			get {
				Int64 _RecordKey = 0;
				Int64.TryParse(hdnFieldEditRecordKey.Value, out _RecordKey);
				return _RecordKey;
			}
			set { hdnFieldEditRecordKey.Value = value.ToString(); }
		}

		private bool HasRecordKey {
			get { return Request.QueryString["Key"] != null; }
		}
		private bool HasAgentQueryString {
			get { return Request.QueryString["IsAgent"] != null; }
		}

		#endregion
		#region "Events"
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack) {
				for (int index = 0; index <= 23; index++) {
					ddlHour.Items.Add(new ListItem(index.ToString(), index.ToString()));
					ddlHour0.Items.Add(new ListItem(index.ToString(), index.ToString()));
				}
				for (int index = 0; index <= 59; index++) {
					ddlMinutes.Items.Add(new ListItem(index.ToString(), index.ToString()));
					ddlMinutes0.Items.Add(new ListItem(index.ToString(), index.ToString()));
				}
				InitializeForm();
				//ResetDefault()
			}
			lblErrorMessage.Text = "";
		}
		/// <summary>
		/// Returns back to the main page
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void btnBackToMain_Click(object sender, EventArgs e)
		{
			Response.Redirect("AgentManager.aspx");
		}

		/// <summary>
		/// Saves the record whether it is in Edit Mode or New Record
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void btnAddRecord_Click(object sender, EventArgs e)
		{
			SaveAgentSchedule();
			ResetDefault();
		}

		/// <summary>
		/// Resets the form to Add Mode
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void btnCancelRecord_Click(object sender, EventArgs e)
		{
			ResetDefault();
		}
		/// <summary>
		/// Row Command event of the Grid, Currently capturing the Edit request of the record.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		/// <remarks></remarks>
		protected void grdAgentPVSchedules_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
		{
			if (e.CommandName == "EditRecord") {
				int index = Convert.ToInt32(e.CommandArgument);
				GridViewRow row = grdAgentPVSchedules.Rows[index];
				DataKey rowkey = grdAgentPVSchedules.DataKeys[row.RowIndex];
				string ID = rowkey[0].ToString();
				Int64 _editKey = 0;
				Int64.TryParse(ID, out _editKey);
				EditRecordKey = _editKey;
				if (lstAgents.Items.FindByText(row.Cells[2].Text) != null) {
					lstAgents.SelectedIndex = -1;
					lstAgents.Items.FindByText(row.Cells[2].Text).Selected = true;
				}
				DateTime startTime = DateTime.Now;
				DateTime.TryParse(row.Cells[3].Text, out startTime);

				ddlHour.SelectedValue = startTime.Hour.ToString();
				//(((startTime.Hour + 11) Mod 12) + 1).ToString()
				ddlMinutes.SelectedValue = startTime.Minute.ToString();

				DateTime endTime = DateTime.Now;
				DateTime.TryParse(row.Cells[4].Text, out endTime);

				ddlHour0.SelectedValue = endTime.Hour.ToString();
				//(((endTime.Hour + 11) Mod 12) + 1).ToString()
				ddlMinutes0.SelectedValue = endTime.Minute.ToString();
				txtPVMax.Text = row.Cells[5].Text;

				IsFormMode = true;
			}
		}

		#endregion
		#region "Methods"
        public void SetGALType(string galType = "0")
        {
            AgentList.SelectParameters["bGALType"].DefaultValue = galType;
            lstAgents.DataBind();
        }
		/// <summary>
		/// Initializes the form
		/// </summary>
		/// <remarks></remarks>
		private void InitializeForm()
		{
			IsFormMode = false;            
            lstAgents.DataBind();
			if (HasRecordKey) 
            {
				Guid keyValue = Guid.Empty;
				Guid.TryParse(Request.QueryString["Key"].ToString(), out keyValue);
				RecordKey = keyValue;
				if (lstAgents.Items.FindByValue(RecordKey.ToString()) != null) 
                {
				    lstAgents.SelectedValue = RecordKey.ToString();
			    }
				BindGrid();
			}
		}

		private void SetFormMode()
		{
			divFormMode.Visible = true;
			divGridMode.Visible = true;

		}
		private void SetGridMode()
		{
			divFormMode.Visible = true;
			divGridMode.Visible = true;
		}
		/// <summary>
		/// Resets the form to Add Mode
		/// </summary>
		/// <remarks></remarks>

		private void ResetDefault()
		{
			ddlHour.SelectedIndex = 0;
			ddlHour0.SelectedIndex = 0;
			ddlMinutes.SelectedIndex = 0;
			ddlMinutes0.SelectedIndex = 0;
            lstAgents.DataBind();
			txtPVMax.Text = "0";
			if (lstAgents.Items.FindByValue(RecordKey.ToString()) != null) {
				lstAgents.SelectedValue = RecordKey.ToString();
			}
			BindGrid();
			IsFormMode = false;
		}
		/// <summary>
		/// Loads the values from outside call from the control as its Public 
		/// Actually there was problem of loading values when control is loaded from the Postback event
		/// </summary>
		/// <remarks></remarks>
		public void LoadValues()
		{
			InitializeForm();
		}
		/// <summary>
		/// Binds the grid with specified DataSource
		/// </summary>
		/// <remarks></remarks>
		private void BindGrid()
		{
			AgentPVScheduleList.SelectParameters["pvs2agt_agent_id"].DefaultValue = RecordKey.ToString();
			grdAgentPVSchedules.DataBind();
		}
		/// <summary>
		/// Saves the record whether it is in Edit Mode or New Record
		/// </summary>
		/// <remarks></remarks>
		private void SaveAgentSchedule()
		{
			int startHour = 0;
			int.TryParse(ddlHour.SelectedValue, out startHour);

			int endHour = 0;
			int.TryParse(ddlHour0.SelectedValue, out endHour);

			int startMin = 0;
			int.TryParse(ddlMinutes.SelectedValue, out startMin);

			int endMin = 0;
			int.TryParse(ddlMinutes0.SelectedValue, out endMin);

			//Checks the End Time must be greater than Start Time
			if (startHour > endHour | (startHour == endHour & startMin > endMin)) {
				lblErrorMessage.Text = "Select correct start and end time.";
				return;
			}

			//Check for Is Edit mode
			if (IsFormMode) {
				AgentPVScheduleList.UpdateParameters["pvs2agt_agent_id"].DefaultValue = lstAgents.SelectedValue;
				AgentPVScheduleList.UpdateParameters["pvs2agt_start_time"].DefaultValue = ddlHour.SelectedValue + ":" + ddlMinutes.SelectedValue;
				AgentPVScheduleList.UpdateParameters["pvs2agt_end_time"].DefaultValue = ddlHour0.SelectedValue + ":" + ddlMinutes0.SelectedValue;
				if (!string.IsNullOrEmpty(txtPVMax.Text)) {
					AgentPVScheduleList.UpdateParameters["pvs2agt_pv_max"].DefaultValue = txtPVMax.Text;
				} else {
					AgentPVScheduleList.UpdateParameters["pvs2agt_pv_max"].DefaultValue = "0";
				}


				AgentPVScheduleList.UpdateParameters["original_pvs2agt_id"].DefaultValue = EditRecordKey.ToString();
				AgentPVScheduleList.Update();
			} else {
				AgentPVScheduleList.InsertParameters["pvs2agt_agent_id"].DefaultValue = lstAgents.SelectedValue;
				AgentPVScheduleList.InsertParameters["pvs2agt_start_time"].DefaultValue = ddlHour.SelectedValue + ":" + ddlMinutes.SelectedValue;
				AgentPVScheduleList.InsertParameters["pvs2agt_end_time"].DefaultValue = ddlHour0.SelectedValue + ":" + ddlMinutes0.SelectedValue;
				if (!string.IsNullOrEmpty(txtPVMax.Text)) {
					AgentPVScheduleList.InsertParameters["pvs2agt_pv_max"].DefaultValue = txtPVMax.Text;
				} else {
					AgentPVScheduleList.InsertParameters["pvs2agt_pv_max"].DefaultValue = "0";
				}
				AgentPVScheduleList.Insert();
			}
		}

		public static int GetTwelveCycleHour(DateTime dateTime)
		{
			return Convert.ToInt32(dateTime.ToString("h"));
		}
		#endregion

	}
}
