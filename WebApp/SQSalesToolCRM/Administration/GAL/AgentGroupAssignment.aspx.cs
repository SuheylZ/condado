using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using CONFIG = System.Configuration.ConfigurationManager;
using System.Web.UI.WebControls;

public partial class Administration_GAL_AgentGroupAssignment : SalesBasePage
{
    public System.Data.SqlClient.SqlConnection SqlConnection = new System.Data.SqlClient.SqlConnection(CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString);
    public System.Data.SqlClient.SqlCommand SqlCommand = new System.Data.SqlClient.SqlCommand();
    public System.Data.SqlClient.SqlParameter SqlParameter;
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            Available.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
            Assigned.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
            lstAddedGroups.DataBind();
            lstAvailableGroups.DataBind();
        }
    }

    protected void btnReturnToAgentManager_Click(object sender, EventArgs e)
    {
        Response.Redirect("AgentManager.aspx");
    }

    protected void btnAddSelected_Click(object sender, EventArgs e)
    {
        AgentAddRemove("AddSelected");
    }

    protected void btnAddAll_Click(object sender, EventArgs e)
    {
        AgentAddRemove("AddAll");
    }

    protected void btnRemoveAll_Click(object sender, EventArgs e)
    {
        AgentAddRemove("RemoveAll");
    }

    protected void btnRemoveSelected_Click(object sender, EventArgs e)
    {
        AgentAddRemove("RemoveSelected");
    }

    public Guid? AgentId
    {
        get
        {
            return Request.ReadQueryStringAs<Guid?>("agentId");
        }
    }
    private void AgentAddRemove(string MoveCommand)
    {
        //Guid? agentId = Request.ReadQueryStringAs<Guid?>("agentId");
        SqlCommand.Connection = SqlConnection;
        SqlConnection.Open();
        if (MoveCommand == "AddAll")
        {
            foreach (ListItem i in this.lstAvailableGroups.Items)
            {
                string InsertSQL = null;
                //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = '" + Session["AgentGroupKey"].ToString() + "' WHERE agent_id = '" + i.Value.ToString() + "'";
                InsertSQL = "INSERT INTO dbo.gal_agent2agentgroups( agent_group_id ,agent_id ,assignment_date ,assigned_usr)VALUES  ( '" + i.Value.ToString() + "','" + AgentId.ToString() + "',GETDATE(),'" + CurrentUser.Key + "')";
                SqlCommand.CommandText = InsertSQL;
                SqlCommand.CommandType = CommandType.Text;

                SqlCommand.ExecuteNonQuery();
            }
            SqlConnection.Close();
            string returnData = CheckForSingleGroup();
            if(!string.IsNullOrEmpty(returnData))
                MakeDefault(returnData);
        }
        else if (MoveCommand == "AddSelected")
        {
            foreach (ListItem i in this.lstAvailableGroups.Items)
            {
                if (i.Selected == true)
                {
                    string InsertSQL = null;
                    //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = '" + Session["AgentGroupKey"].ToString() + "' WHERE agent_id = '" + i.Value.ToString() + "'";
                    InsertSQL = "INSERT INTO dbo.gal_agent2agentgroups( agent_group_id ,agent_id ,assignment_date ,assigned_usr)VALUES  ( '" + i.Value.ToString() + "','" + AgentId.ToString() + "',GETDATE(),'" + CurrentUser.Key + "')";
                    SqlCommand.CommandText = InsertSQL;
                    SqlCommand.CommandType = CommandType.Text;

                    SqlCommand.ExecuteNonQuery();

                }
            }
            SqlConnection.Close();
            string returnData = CheckForSingleGroup();
            if(!string.IsNullOrEmpty(returnData))
                MakeDefault(returnData);
        }
        else if (MoveCommand == "RemoveAll")
        {
            foreach (ListItem i in this.lstAddedGroups.Items)
            {
                string InsertSQL = null;
                //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = null WHERE agent_id = '" + i.Value.ToString() + "'";
                InsertSQL = "DELETE FROM dbo.gal_agent2agentgroups WHERE agent_id='" + AgentId.ToString() + "'";
                SqlCommand.CommandText = InsertSQL;
                SqlCommand.CommandType = CommandType.Text;

                SqlCommand.ExecuteNonQuery();                
            }
            SqlConnection.Close();
        }
        else if (MoveCommand == "RemoveSelected")
        {
            foreach (ListItem i in this.lstAddedGroups.Items)
            {
                if (i.Selected == true)
                {
                    string InsertSQL = null;
                    //InsertSQL = "UPDATE gal_Agents SET agent_agent_group_id = null WHERE agent_id = '" + i.Value.ToString() + "'";
                    InsertSQL = "DELETE FROM dbo.gal_agent2agentgroups WHERE agent_group_id='" + i.Value.ToString() + "' and agent_id='" + AgentId.ToString() + "'";
                    SqlCommand.CommandText = InsertSQL;
                    SqlCommand.CommandType = CommandType.Text;

                    SqlCommand.ExecuteNonQuery();                    
                }
            }
            SqlConnection.Close();
            string returnData = CheckForSingleGroup();
            if(!string.IsNullOrEmpty(returnData))
                MakeDefault(returnData);
        }
        else if (MoveCommand == "MakeDefault")
        {
            string selectedvalue = lstAddedGroups.SelectedValue;
            MakeDefault(selectedvalue);
        }
        if(SqlConnection.State == ConnectionState.Open)
            SqlConnection.Close();
        lstAddedGroups.DataBind();
        lstAvailableGroups.DataBind();
    }

    private void MakeDefault(string selectedGroupID)
    {        
        Guid groupId;
        if (!string.IsNullOrEmpty(selectedGroupID) && Guid.TryParse(selectedGroupID, out groupId))
        {
            SqlCommand.Connection = SqlConnection;
		    SqlConnection.Open();
            string command;
            if (IsAcdMode)
            {
                command = "UPDATE dbo.gal_agents SET agent_default_agent_group_id_acd='" + selectedGroupID + "' WHERE agent_id='" + AgentId.ToString() + "'";
            }
            else
            {
                command = "UPDATE dbo.gal_agents SET agent_default_agent_group_id='" + selectedGroupID + "' WHERE agent_id='" + AgentId.ToString() + "'";
            }

            SqlCommand.CommandText = command;
            SqlCommand.CommandType = CommandType.Text;
            SqlCommand.ExecuteNonQuery();
            SqlConnection.Close();
        }
        
    }

    private string CheckForSingleGroup()
    {
        SqlCommand.Connection = SqlConnection;
		SqlConnection.Open();
        string returnData = string.Empty;
        string query = @"SELECT  g.agent_group_id as agent_group_id
                        FROM    gal_AgentGroups g
                        JOIN dbo.gal_agent2agentgroups ag ON ag.agent_group_id = g.agent_group_id
                        AND ag.agent_id = '"+AgentId.ToString()+"' AND ISNULL(g.agent_group_acd_flag,0)='"+ Master.GetGALType().ToString() +"' JOIN dbo.gal_agents a ON a.agent_id = ag.agent_id ";
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
        SqlConnection.Close();
        return returnData;
    }
    protected void btnMakeDefault_Click(object sender, EventArgs e)
    {
        AgentAddRemove("MakeDefault");
    }
    public bool IsAcdMode
    {
        get { return Master.GetGALType(); }
    }
}