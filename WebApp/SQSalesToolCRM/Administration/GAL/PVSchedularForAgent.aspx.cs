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
	public partial class PVSchedularForAgent : SalesBasePage
	{

		public Guid RecordKey {
			get {
				Guid _RecordKey = Guid.Empty;
				Guid.TryParse(hdnFieldRecordKey.Value, out _RecordKey);
				return _RecordKey;
			}
			set { hdnFieldRecordKey.Value = value.ToString(); }
		}
		private bool HasRecordKey {
			get { return Request.QueryString["Key"] != null; }
		}
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack) {
                PVSchedularAgent.SetGALType(Master.GetGALType().ToString());
				if (HasRecordKey) {
					Guid keyValue = Guid.Empty;
					Guid.TryParse(Request.QueryString["Key"].ToString(), out keyValue);
					RecordKey = keyValue;
				}
				UpdateAgentPVScheduleOverride.SelectParameters["agent_id"].DefaultValue = RecordKey.ToString();
                DSAgentGroups.SelectParameters["agent_id"].DefaultValue = RecordKey.ToString();
                DSAgentGroups.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();            
                DataView dviewAG = (DataView)DSAgentGroups.Select(new DataSourceSelectArguments());
                //DSAgentGroups.DataBind();
                ddlAgentGroups.DataSource = dviewAG.Table;
                ddlAgentGroups.DataBind();

				DataSourceSelectArguments args = new DataSourceSelectArguments();
				DataView dview = (DataView)UpdateAgentPVScheduleOverride.Select(args);
				if (dview != null) {
					//Dim dt As DataTable = view.ToTable()
					foreach (DataRow drow in dview.Table.Rows) {
                        if(drow["agent_override_pv_schedule"]!=DBNull.Value)
						    chkOverride.Checked = Convert.ToBoolean(drow["agent_override_pv_schedule"]);

                        if (!Master.GetGALType())
                        {
                            if (drow["agent_default_agent_group_id"] != DBNull.Value)
                            {
                                if (ddlAgentGroups.Items.FindByValue(drow["agent_default_agent_group_id"].ToString()) != null)
                                    ddlAgentGroups.SelectedValue = drow["agent_default_agent_group_id"].ToString();
                            }
                        }
                        else
                        {
                            if (drow["agent_default_agent_group_id_acd"] != DBNull.Value)
                            {
                                if (ddlAgentGroups.Items.FindByValue(drow["agent_default_agent_group_id_acd"].ToString()) != null)
                                    ddlAgentGroups.SelectedValue = drow["agent_default_agent_group_id_acd"].ToString();
                            }
                        }
					}
				}
				ShowControl();
			}

		}

		private void ShowControl()
		{
			if (chkOverride.Checked) {
				PVSchedularAgentGroup.Visible = false;
				PVSchedularAgent.Visible = true;
				PVSchedularAgent.LoadValues();
			} else {
				PVSchedularAgentGroup.Visible = true;
				PVSchedularAgent.Visible = false;
				PVSchedularAgentGroup.LoadValues();
			}
		}

        protected void chkOverride_CheckedChanged(object sender, EventArgs e)
        {
            UpdateRecord();
        }

		protected void btnGO_Click(object sender, EventArgs e)
		{
            UpdateRecord();
		}

        private void UpdateRecord()
        {
            UpdateAgentPVScheduleOverride.UpdateParameters["OverrideValue"].DefaultValue = chkOverride.Checked.ToString();
            UpdateAgentPVScheduleOverride.UpdateParameters["default_agentgroup_id"].DefaultValue = ddlAgentGroups.SelectedIndex > -1 ? ddlAgentGroups.SelectedValue : "";
            UpdateAgentPVScheduleOverride.UpdateParameters["agent_modify_date"].DefaultValue = DateTime.Now.ToString();
            UpdateAgentPVScheduleOverride.UpdateParameters["agent_id"].DefaultValue = RecordKey.ToString();
            UpdateAgentPVScheduleOverride.Update();

            UpdateAgentPVScheduleOverride.DeleteParameters["agent_id"].DefaultValue = RecordKey.ToString();
            UpdateAgentPVScheduleOverride.Delete();
            ShowControl();
        }

		protected void btnBacktoMain0_Click(object sender, EventArgs e)
		{
			Response.Redirect("AgentManager.aspx");
		}
        protected void ddlAgentGroups_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateRecord();
            Response.Redirect("PVSchedularForAgent.aspx?key=" + RecordKey.ToString() + "&GroupID=" + ddlAgentGroups.SelectedValue, true);
        }
}
}
