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
using CONFIG = System.Configuration.ConfigurationManager;
namespace SQS_Dialer
{
	public partial class AgentEdit : SalesBasePage
	{

		//Private Leads360CS As com.leads360.service.ClientService = New com.leads360.service.ClientService
		public System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
		public System.Data.SqlClient.SqlDataAdapter SqlAdapter = new System.Data.SqlClient.SqlDataAdapter();
		public System.Data.SqlClient.SqlCommand SqlCommand = new System.Data.SqlClient.SqlCommand();
		public System.Data.SqlClient.SqlParameter SqlParameter;

		public System.Data.SqlClient.SqlDataReader SqlReader;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			AvailableCampaignsDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
			AddedCampaignsDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
			CampaignModifyDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
			AgentInfoDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;

			if (string.IsNullOrEmpty(Request.QueryString["key"])) {
				Session["AgentCampaignKey"] = null;
				Response.Redirect("AgentManager.aspx");
			} else {
				try {
					Session["AgentCampaignKey"] = Guid.Parse(Request.QueryString["key"].ToString());
				} catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
					Session["AgentCampaignKey"] = null;
					Response.Redirect("AgentManager.aspx");
				}
			}

			DataView dvSql = (DataView)AgentInfoDataSource.Select(DataSourceSelectArguments.Empty);

			foreach (DataRowView drvSql in dvSql) {
				AgentName.Text = drvSql["AgentName"].ToString() + " - " + drvSql["location_name"].ToString() + "";
			}
		}

		protected void btnReturnToAgentManager_Click(object sender, EventArgs e)
		{
			Response.Redirect("AgentManager.aspx");
		}

		protected void btnReturnToAgentManager0_Click(object sender, EventArgs e)
		{
			Response.Redirect("AgentManager.aspx");
		}

		protected void btnAddSelected_Click(object sender, EventArgs e)
		{
			CampaignAddRemove("AddSelected");
		}

		protected void btnAddAll_Click(object sender, EventArgs e)
		{
			CampaignAddRemove("AddAll");
		}

		protected void btnRemoveAll_Click(object sender, EventArgs e)
		{
			CampaignAddRemove("RemoveAll");
		}

		protected void btnRemoveSelected_Click(object sender, EventArgs e)
		{
			CampaignAddRemove("RemoveSelected");
		}

		protected void CampaignAddRemove(string MoveCommand)
		{
			SqlCommand.Connection = SqlConnection;
			SqlConnection.Open();
			if (MoveCommand == "AddAll") {
				foreach (ListItem i in this.lstAvailableCampaigns.Items) {
					string InsertSQL = null;
					InsertSQL = "INSERT INTO Campaign2Agent ([cmp2agt_campaign_id], [cmp2agt_agent_id], [cmp2agt_max]) " + "SELECT '" + i.Value.ToString() + "', '" + Request.QueryString["key"].ToString() + "', (select campaign_default_max from Campaigns where campaign_id = '" + i.Value.ToString() + "')";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;

					SqlCommand.ExecuteNonQuery();
				}
			} else if (MoveCommand == "AddSelected") {
				foreach (ListItem i in this.lstAvailableCampaigns.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
						InsertSQL = "INSERT INTO Campaign2Agent ([cmp2agt_campaign_id], [cmp2agt_agent_id], [cmp2agt_max]) " + "SELECT '" + i.Value.ToString() + "', '" + Request.QueryString["key"].ToString() + "', (select campaign_default_max from Campaigns where campaign_id = '" + i.Value.ToString() + "')";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;

						SqlCommand.ExecuteNonQuery();
					}
				}
			} else if (MoveCommand == "RemoveAll") {
				foreach (ListItem i in this.lstAddedCampaigns.Items) {
					string InsertSQL = null;
					InsertSQL = "DELETE FROM Campaign2Agent WHERE cmp2agt_id = '" + i.Value.ToString() + "'";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;

					SqlCommand.ExecuteNonQuery();
				}
			} else if (MoveCommand == "RemoveSelected") {
				foreach (ListItem i in this.lstAddedCampaigns.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
						InsertSQL = "DELETE FROM Campaign2Agent WHERE cmp2agt_id = '" + i.Value.ToString() + "'";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;

						SqlCommand.ExecuteNonQuery();
					}
				}
			}
			SqlConnection.Close();
			lstAddedCampaigns.DataBind();
			lstAvailableCampaigns.DataBind();
			GridView1.DataBind();
		}

		protected void GridView1_DataBound(object sender, System.EventArgs e)
		{
			if (GridView1.Rows.Count < 1) {
				btnReturnToAgentManager.Visible = false;
				lblAgentCampaignsTitle0.Visible = false;
			} else {
				btnReturnToAgentManager.Visible = true;
				lblAgentCampaignsTitle0.Visible = true;
			}
		}

	}
}
