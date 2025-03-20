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
	public partial class PVScheduleAgentGroup : System.Web.UI.UserControl
	{
		//YA[Oct 01, 13]

		#region "Constants/Properties"
		public bool IsAgentGroup {
			get {
				bool _IsAgentGroup = false;
				bool.TryParse(hdnFieldIsAgentGroup.Value, out _IsAgentGroup);
				return _IsAgentGroup;
			}
			set { hdnFieldIsAgentGroup.Value = value.ToString(); }
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
			get { return Request.QueryString["GroupID"] != null; }
		}
		private bool HasAgentGroupQueryString {
			get { return Request.QueryString["IsAgentGroup"] != null; }
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

		protected void btnBackToMain_Click(object sender, EventArgs e)
		{
			Response.Redirect("AgentGroupModify.aspx");
		}
		protected void btnAddRecord_Click(object sender, EventArgs e)
		{
			SaveAgentGroupSchedule();
			ResetDefault();
		}

		protected void btnCancelRecord_Click(object sender, EventArgs e)
		{
			ResetDefault();
		}
		protected void grdAgentGroupPVSchedules_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
		{
			if (e.CommandName == "EditRecord") {
				int index = Convert.ToInt32(e.CommandArgument);
				GridViewRow row = grdAgentGroupPVSchedules.Rows[index];
				DataKey rowkey = grdAgentGroupPVSchedules.DataKeys[row.RowIndex];
				string ID = rowkey[0].ToString();
				Int64 _editKey = 0;
				Int64.TryParse(ID, out _editKey);
				EditRecordKey = _editKey;
				if (lstAgentGroups.Items.FindByText(row.Cells[2].Text) != null) {
					lstAgentGroups.SelectedIndex = -1;
					lstAgentGroups.Items.FindByText(row.Cells[2].Text).Selected = true;
				}
				DateTime startTime = DateTime.Now;
				DateTime.TryParse(row.Cells[3].Text, out startTime);

				ddlHour.SelectedValue = startTime.Hour.ToString();
				//(((startTime.Hour + 11) Mod 12) + 1).ToString()
				ddlMinutes.SelectedValue = startTime.Minute.ToString();
				//ddlAMPM.SelectedValue = startTime.ToString("tt")

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
		/// <summary>
		/// Initializes the form
		/// </summary>
		/// <remarks></remarks>
		private void InitializeForm()
		{
			IsFormMode = false;
			if (HasRecordKey) {
				Guid keyValue = Guid.Empty;
				if (Guid.TryParse(Request.QueryString["GroupID"].ToString(), out keyValue)) {
					RecordKey = keyValue;
					lstAgentGroups.SelectedValue = RecordKey.ToString();
					BindGrid();
				}
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

			txtPVMax.Text = "0";
			if (lstAgentGroups.Items.FindByValue(RecordKey.ToString()) != null) {
				lstAgentGroups.SelectedValue = RecordKey.ToString();
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
		/// Binds the grid to specified DataSource
		/// </summary>
		/// <remarks></remarks>
		private void BindGrid()
		{
			AgentGroupPVScheduleList.SelectParameters["pvs2agtgrp_agent_id"].DefaultValue = RecordKey.ToString();
			grdAgentGroupPVSchedules.DataBind();
		}
		/// <summary>
		/// Saves the reocord whether it is in Edit Mode or Add mode
		/// </summary>
		/// <remarks></remarks>

		private void SaveAgentGroupSchedule()
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
			//Check for checking the Edit Mode
			if (IsFormMode) {
				AgentGroupPVScheduleList.UpdateParameters["pvs2agtgrp_agent_id"].DefaultValue = lstAgentGroups.SelectedValue;
				AgentGroupPVScheduleList.UpdateParameters["pvs2agtgrp_start_time"].DefaultValue = ddlHour.SelectedValue + ":" + ddlMinutes.SelectedValue;
				AgentGroupPVScheduleList.UpdateParameters["pvs2agtgrp_end_time"].DefaultValue = ddlHour0.SelectedValue + ":" + ddlMinutes0.SelectedValue;
				if (!string.IsNullOrEmpty(txtPVMax.Text)) {
					AgentGroupPVScheduleList.UpdateParameters["pvs2agtgrp_pv_max"].DefaultValue = txtPVMax.Text;
				} else {
					AgentGroupPVScheduleList.UpdateParameters["pvs2agtgrp_pv_max"].DefaultValue = "0";
				}
				AgentGroupPVScheduleList.UpdateParameters["original_pvs2agtgrp_id"].DefaultValue = EditRecordKey.ToString();
				AgentGroupPVScheduleList.Update();
			} else {
				AgentGroupPVScheduleList.InsertParameters["pvs2agtgrp_agent_id"].DefaultValue = lstAgentGroups.SelectedValue;
				AgentGroupPVScheduleList.InsertParameters["pvs2agtgrp_start_time"].DefaultValue = ddlHour.SelectedValue + ":" + ddlMinutes.SelectedValue;
				AgentGroupPVScheduleList.InsertParameters["pvs2agtgrp_end_time"].DefaultValue = ddlHour0.SelectedValue + ":" + ddlMinutes0.SelectedValue;
				if (!string.IsNullOrEmpty(txtPVMax.Text)) {
					AgentGroupPVScheduleList.InsertParameters["pvs2agtgrp_pv_max"].DefaultValue = txtPVMax.Text;
				} else {
					AgentGroupPVScheduleList.InsertParameters["pvs2agtgrp_pv_max"].DefaultValue = "0";
				}
				AgentGroupPVScheduleList.Insert();
			}


		}

		public static int GetTwelveCycleHour(DateTime dateTime)
		{
			return Convert.ToInt32(dateTime.ToString("h"));
		}

        public void ToggleBackToMainButton(bool bVisiblity = true)
        {
            btnBackToMain.Visible = bVisiblity;
        }
		#endregion

	}
}
