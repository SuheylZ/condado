using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class DashboardV3 : System.Web.UI.Page {
    public DataSet dsAgents = new DataSet();
    public DataSet dsCampaigns = new DataSet();
    public DataSet dsSkillGroup = new DataSet();
    protected void Page_Load(object sender, EventArgs e) {
        LoadAllAgentDropDownList();
        LoadAllCampaignsDropDownList();
        LoadAllSkillGroupDropDownList();
        LoadAllDateDropDownList();
    }

    #region Agrents
    private void LoadAllAgentDropDownList() {
        // Load DataSet for Agents
        dsAgents = GetAgents();

        // Load Sales Metrics 
        lstSmAgents.DataSource = dsAgents;
        lstSmAgents.DataValueField = "usr_key";
        lstSmAgents.DataTextField = "AgentName";
        lstSmAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstSmAgents.SelectedIndex = 0;
        lstSmAgents.DataBind();


        // Dispose of DataSet
        dsAgents.Dispose();
    }

    private DataSet GetAgents() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        string strSQL = "SELECT usr_first_name + ' ' + usr_last_name AS AgentName, usr_key FROM dbo.users WHERE usr_delete_flag = 0 AND (usr_position LIKE N'%Agent%') ORDER BY AgentName";
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
        DataSet ds = new DataSet();
        da.Fill(ds);
        da.Dispose();
        conn.Close();
        return ds;
    }
    #endregion

    #region Campaigns

    private void LoadAllCampaignsDropDownList() {
        // Load DataSet for Agents
        dsCampaigns = GetCampaigns();

        // Load Sales Metrics 
        lstSmCampaign.DataSource = dsCampaigns;
        lstSmCampaign.DataValueField = "cmp_id";
        lstSmCampaign.DataTextField = "cmp_title";
        lstSmCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstSmCampaign.SelectedIndex = 0;
        lstSmCampaign.DataBind();


        // Dispose of DataSet
        dsCampaigns.Dispose();
    }

    private DataSet GetCampaigns() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        string strSQL = "SELECT cmp_title, cmp_id FROM dbo.campaigns WHERE cmp_delete_flag=0 ORDER BY cmp_title";
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
        DataSet ds = new DataSet();
        da.Fill(ds);
        da.Dispose();
        conn.Close();
        return ds;
    }

    #endregion

    #region Skill Group
    private void LoadAllSkillGroupDropDownList() {
        // Load DataSet for Agents
        dsSkillGroup = GetSkillGroup();

        // Load Sales Metrics 
        lstSmSkillGroup.DataSource = dsSkillGroup;
        lstSmSkillGroup.DataValueField = "skl_id";
        lstSmSkillGroup.DataTextField = "skl_name";
        lstSmSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstSmSkillGroup.SelectedIndex = 0;
        lstSmSkillGroup.DataBind();


        // Dispose of DataSet
        dsSkillGroup.Dispose();
    }

    private DataSet GetSkillGroup() {
        SqlConnection conn = null;
        string ConnectString = ApplicationSettings.ADOConnectionString;
        string strSQL = "SELECT skl_id, skl_name FROM dbo.skill_groups WHERE skl_delete_flag = 0 ORDER BY skl_name";
        conn = new SqlConnection(ConnectString);
        conn.Open();
        SqlDataAdapter da = new SqlDataAdapter(strSQL, conn);
        DataSet ds = new DataSet();
        da.Fill(ds);
        da.Dispose();
        conn.Close();
        return ds;
    }
    #endregion

    #region DateBoxes
    private void LoadAllDateDropDownList() {
        lstSmDateSelected.Items.Insert(0, new ListItem("Today", "0"));
        lstSmDateSelected.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstSmDateSelected.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstSmDateSelected.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstSmDateSelected.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstSmDateSelected.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstSmDateSelected.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstSmDateSelected.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstSmDateSelected.SelectedIndex = 0;
        lstSmDateSelected.DataBind();
    }
    #endregion
}