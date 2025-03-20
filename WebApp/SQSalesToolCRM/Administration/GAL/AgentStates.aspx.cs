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
	public partial class AgentStates : SalesBasePage
	{

		public System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
		public System.Data.SqlClient.SqlDataAdapter SqlAdapter = new System.Data.SqlClient.SqlDataAdapter();
		public System.Data.SqlClient.SqlCommand SqlCommand = new System.Data.SqlClient.SqlCommand();
		public System.Data.SqlClient.SqlParameter SqlParameter;

		public System.Data.SqlClient.SqlDataReader SqlReader;
		protected void Page_Load(object sender, System.EventArgs e)
		{
            AvailableStatesDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            AddedStatesDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            AgentInfoDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;

			if (string.IsNullOrEmpty(Request.QueryString["key"])) {
				Session["AgentStatesKey"] = null;
				Response.Redirect("AgentManager.aspx");
			} else {
				try {
					Session["AgentStatesKey"] = Guid.Parse(Request.QueryString["key"].ToString());
				} catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
					Session["AgentStatesKey"] = null;
					Response.Redirect("AgentManager.aspx");
				}
			}

            DataView dvSql = (DataView)AgentInfoDataSource.Select(DataSourceSelectArguments.Empty);

			foreach (DataRowView drvSql in dvSql) {
				AgentName.Text = drvSql["AgentName"].ToString() + " - " + drvSql["location_name"].ToString() + "";
			}
		}

		protected void btnReturnToAgentManager0_Click(object sender, EventArgs e)
		{
			Response.Redirect("AgentManager.aspx");
		}

		protected void btnAddSelected_Click(object sender, EventArgs e)
		{
			StatesAddRemove("AddSelected");
		}

		protected void btnAddAll_Click(object sender, EventArgs e)
		{
			StatesAddRemove("AddAll");
		}

		protected void btnRemoveAll_Click(object sender, EventArgs e)
		{
			StatesAddRemove("RemoveAll");
		}

		protected void btnRemoveSelected_Click(object sender, EventArgs e)
		{
			StatesAddRemove("RemoveSelected");
		}

		private void StatesAddRemove(string MoveCommand)
		{
			SqlCommand.Connection = SqlConnection;
			SqlConnection.Open();
			if (MoveCommand == "AddAll") {
				foreach (ListItem i in this.lstAvailableStates.Items) {
					string InsertSQL = null;
					InsertSQL = "INSERT INTO AgentStateLicensure ([agent_state_licensure_state_id], [agent_state_licensure_agent_id]) " + "SELECT '" + i.Value.ToString() + "', '" + Request.QueryString["key"].ToString() + "'";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;

					SqlCommand.ExecuteNonQuery();
				}
			} else if (MoveCommand == "AddSelected") {
				foreach (ListItem i in this.lstAvailableStates.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
						InsertSQL = "INSERT INTO AgentStateLicensure ([agent_state_licensure_state_id], [agent_state_licensure_agent_id]) " + "SELECT '" + i.Value.ToString() + "', '" + Request.QueryString["key"].ToString() + "'";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;

						SqlCommand.ExecuteNonQuery();
					}
				}
			} else if (MoveCommand == "RemoveAll") {
				foreach (ListItem i in this.lstAddedStates.Items) {
					string InsertSQL = null;
					InsertSQL = "DELETE FROM AgentStateLicensure WHERE agent_state_licensure_id = '" + i.Value.ToString() + "'";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;

					SqlCommand.ExecuteNonQuery();
				}
			} else if (MoveCommand == "RemoveSelected") {
				foreach (ListItem i in this.lstAddedStates.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
						InsertSQL = "DELETE FROM AgentStateLicensure WHERE agent_state_licensure_id = '" + i.Value.ToString() + "'";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;

						SqlCommand.ExecuteNonQuery();
					}
				}
			}
			SqlConnection.Close();
			lstAddedStates.DataBind();
			lstAvailableStates.DataBind();
		}

	}
}
