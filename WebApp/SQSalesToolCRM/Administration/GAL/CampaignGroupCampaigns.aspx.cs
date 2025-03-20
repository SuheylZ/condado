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
//CONFIG.ConnectionStrings[CONFIG.AppSettings["ConnectionToUse"]]

namespace SQS_Dialer
{
	public partial class CampaignGroupCampaigns : SalesBasePage
	{
        
		public System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
		public System.Data.SqlClient.SqlDataAdapter SqlAdapter = new System.Data.SqlClient.SqlDataAdapter();
		public System.Data.SqlClient.SqlCommand SqlCommand = new System.Data.SqlClient.SqlCommand();
		public System.Data.SqlClient.SqlParameter SqlParameter;

		public System.Data.SqlClient.SqlDataReader SqlReader;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Available.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
			Assigned.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            if (!IsPostBack)
            {
                Available.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                Assigned.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                AgentGroupInfoDataSource.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
            }
			if (string.IsNullOrEmpty(Request.QueryString["key"])) {
				Session["CampaignGroupKey"] = null;
				Response.Redirect("CampaignGroupModify.aspx.aspx");
			} else {
				try {
					Session["CampaignGroupKey"] = Guid.Parse(Request.QueryString["key"].ToString());
				} catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
					Session["CampaignGroupKey"] = null;
					Response.Redirect("CampaignGroupModify.aspx.aspx");
				}
			}

            DataView dvSql = (DataView)AgentGroupInfoDataSource.Select(DataSourceSelectArguments.Empty);

			foreach (DataRowView drvSql in dvSql) {
				AgentGroup.Text = drvSql["campaign_group_name"].ToString();
			}
		}

		protected void btnReturnToAgentManager_Click(object sender, EventArgs e)
		{
			Response.Redirect("CampaignGroupModify.aspx");
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

		private void CampaignAddRemove(string MoveCommand)
		{
			SqlCommand.Connection = SqlConnection;
			SqlConnection.Open();
			if (MoveCommand == "AddAll") {
				foreach (ListItem i in this.lstAvailableCampaigns.Items) {
					string InsertSQL = null;
					InsertSQL = "UPDATE gal_Campaigns SET campaign_campaign_group_id = '" + Session["CampaignGroupKey"].ToString() + "' WHERE campaign_id = '" + i.Value.ToString() + "'";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;

					SqlCommand.ExecuteNonQuery();
				}
			} else if (MoveCommand == "AddSelected") {
				foreach (ListItem i in this.lstAvailableCampaigns.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
						InsertSQL = "UPDATE gal_Campaigns SET campaign_campaign_group_id = '" + Session["CampaignGroupKey"].ToString() + "' WHERE campaign_id = '" + i.Value.ToString() + "'";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;

						SqlCommand.ExecuteNonQuery();
					}
				}
			} else if (MoveCommand == "RemoveAll") {
				foreach (ListItem i in this.lstAddedCampaigns.Items) {
					string InsertSQL = null;
					InsertSQL = "UPDATE gal_Campaigns SET campaign_campaign_group_id = null WHERE campaign_id = '" + i.Value.ToString() + "'";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;

					SqlCommand.ExecuteNonQuery();
				}
			} else if (MoveCommand == "RemoveSelected") {
				foreach (ListItem i in this.lstAddedCampaigns.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
						InsertSQL = "UPDATE gal_Campaigns SET campaign_campaign_group_id = null WHERE campaign_id = '" + i.Value.ToString() + "'";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;

						SqlCommand.ExecuteNonQuery();
					}
				}
			}
			SqlConnection.Close();
			lstAddedCampaigns.DataBind();
			lstAvailableCampaigns.DataBind();
		}


	}
}
