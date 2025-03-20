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
	public partial class AgentGroupAgents : SalesBasePage
	{

		//Private Leads360CS As com.leads360.service.ClientService = New com.leads360.service.ClientService
		public System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
		public System.Data.SqlClient.SqlDataAdapter SqlAdapter = new System.Data.SqlClient.SqlDataAdapter();
		public System.Data.SqlClient.SqlCommand SqlCommand = new System.Data.SqlClient.SqlCommand();
		public System.Data.SqlClient.SqlParameter SqlParameter;

		public System.Data.SqlClient.SqlDataReader SqlReader;
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Available.ConnectionString =CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            Assigned.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;

            if (!IsPostBack)
            {
                Available.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                Assigned.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                AgentGroupInfoDataSource.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
            }
			if (string.IsNullOrEmpty(Request.QueryString["key"])) {
				Session["AgentGroupKey"] = null;
				Response.Redirect("AgentGroupModify.aspx");
			} else {
				try {
					Session["AgentGroupKey"] = Guid.Parse(Request.QueryString["key"].ToString());
				} catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex);
					Session["AgentGroupKey"] = null;
					Response.Redirect("AgentGroupModify.aspx");
				}
			}

            DataView dvSql = (DataView)AgentGroupInfoDataSource.Select(DataSourceSelectArguments.Empty);

			foreach (DataRowView drvSql in dvSql) {
				AgentGroup.Text = drvSql["agent_group_name"].ToString();
			}
		}

		protected void btnReturnToAgentManager_Click(object sender, EventArgs e)
		{
			Response.Redirect("AgentGroupModify.aspx");
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
                    //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = '" + Session["AgentGroupKey"].ToString() + "' WHERE agent_id = '" + i.Value.ToString() + "'";
                    InsertSQL = "INSERT INTO dbo.gal_agent2agentgroups( agent_group_id ,agent_id ,assignment_date ,assigned_usr)VALUES  ( '" + Session["AgentGroupKey"].ToString() + "','" + i.Value.ToString() + "',GETDATE(),'" + CurrentUser.Key + "')";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;
					SqlCommand.ExecuteNonQuery();
                    string returnData = CheckForSingleGroup(i.Value.ToString());
                    if(!string.IsNullOrEmpty(returnData))
                        MakeDefault(returnData,i.Value.ToString());
				}                
			} else if (MoveCommand == "AddSelected") {
				foreach (ListItem i in this.lstAvailableCampaigns.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
                        //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = '" + Session["AgentGroupKey"].ToString() + "' WHERE agent_id = '" + i.Value.ToString() + "'";
                        InsertSQL = "INSERT INTO dbo.gal_agent2agentgroups( agent_group_id ,agent_id ,assignment_date ,assigned_usr)VALUES  ( '" + Session["AgentGroupKey"].ToString() + "','" + i.Value.ToString() + "',GETDATE(),'"+CurrentUser.Key+"')";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;
						SqlCommand.ExecuteNonQuery();
                        string returnData = CheckForSingleGroup(i.Value.ToString());
                        if(!string.IsNullOrEmpty(returnData))
                            MakeDefault(returnData,i.Value.ToString());
					}
				}                
			} else if (MoveCommand == "RemoveAll") {
				foreach (ListItem i in this.lstAddedCampaigns.Items) {
					string InsertSQL = null;
                    //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = null WHERE agent_id = '" + i.Value.ToString() + "'";
                    InsertSQL="DELETE FROM dbo.gal_agent2agentgroups WHERE agent_group_id='"+ Session["AgentGroupKey"].ToString()+"'";
					SqlCommand.CommandText = InsertSQL;
					SqlCommand.CommandType = CommandType.Text;
					SqlCommand.ExecuteNonQuery();
                    string returnData = CheckForSingleGroup(i.Value.ToString());
                    if(!string.IsNullOrEmpty(returnData))
                        MakeDefault(returnData,i.Value.ToString());
				}                
			} else if (MoveCommand == "RemoveSelected") {
				foreach (ListItem i in this.lstAddedCampaigns.Items) {
					if (i.Selected == true) {
						string InsertSQL = null;
                        //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = null WHERE agent_id = '" + i.Value.ToString() + "'";
                        InsertSQL = "DELETE FROM dbo.gal_agent2agentgroups WHERE agent_group_id='" + Session["AgentGroupKey"].ToString() + "' and agent_id='" + i.Value.ToString() + "'";
						SqlCommand.CommandText = InsertSQL;
						SqlCommand.CommandType = CommandType.Text;
						SqlCommand.ExecuteNonQuery();
                        string returnData = CheckForSingleGroup(i.Value.ToString());
                        if(!string.IsNullOrEmpty(returnData))
                            MakeDefault(returnData,i.Value.ToString());
					}
				}                
			}
            if(SqlConnection.State == ConnectionState.Open)
			    SqlConnection.Close();
			lstAddedCampaigns.DataBind();
			lstAvailableCampaigns.DataBind();
		}

        private void MakeDefault(string selectedGroupID,string agentID)
    {        
        Guid groupId;
        if (!string.IsNullOrEmpty(selectedGroupID) && Guid.TryParse(selectedGroupID, out groupId))
        {            
            string command;
            if (IsAcdMode)
            {
                command = "UPDATE dbo.gal_agents SET agent_default_agent_group_id_acd='" + selectedGroupID + "' WHERE agent_id='" + agentID + "'";
            }
            else
            {
                command = "UPDATE dbo.gal_agents SET agent_default_agent_group_id='" + selectedGroupID + "' WHERE agent_id='" + agentID + "'";
            }

            SqlCommand.CommandText = command;
            SqlCommand.CommandType = CommandType.Text;
            SqlCommand.ExecuteNonQuery();
            
        }
        
    }

    private string CheckForSingleGroup(string agentID)
    {        
        string returnData = string.Empty;
        string query = @"SELECT  g.agent_group_id as agent_group_id
                        FROM    gal_AgentGroups g
                        JOIN dbo.gal_agent2agentgroups ag ON ag.agent_group_id = g.agent_group_id
                        AND ag.agent_id = '"+agentID+"' AND ISNULL(g.agent_group_acd_flag,0)='"+ Master.GetGALType().ToString() +"' JOIN dbo.gal_agents a ON a.agent_id = ag.agent_id ";
        SqlCommand.CommandText = query;
        SqlCommand.CommandType = CommandType.Text;
        
        DataTable myTable = new DataTable();
        myTable.Load(SqlCommand.ExecuteReader());

        if (myTable.Rows.Count == 1)
        {
            foreach (DataRow drItem in myTable.Rows)
            {
                returnData = drItem["agent_group_id"].ToString();
            }
        }        
        return returnData;
    }

    public bool IsAcdMode
    {
        get { return Master.GetGALType(); }
    }

	}
}
