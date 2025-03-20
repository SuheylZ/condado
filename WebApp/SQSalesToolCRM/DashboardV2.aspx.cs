using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;

public partial class DashboardV2 : SalesBasePage {
    public DataSet dsAgents = new DataSet();
    public DataSet dsCampaigns = new DataSet();
    public DataSet dsSkillGroup = new DataSet();


    protected void Page_Load(object sender, EventArgs e) {
        LoadAllAgentDropDownList();
        LoadAllCampaignsDropDownList();
        LoadAllSkillGroupDropDownList();
        LoadAllDateDropDownList();
        LoadAllPlanTypeDropDownList();
    }

    #region Agents
    private void LoadAllAgentDropDownList() {
        // Load DataSet for Agents
        dsAgents = GetAgents();

        // Load Lead Metrics 
        lstlmAgents.DataSource = dsAgents;
        lstlmAgents.DataValueField = "usr_key";
        lstlmAgents.DataTextField = "AgentName";
        lstlmAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstlmAgents.SelectedIndex = 0;
        lstlmAgents.DataBind();

        // Load Pipeline Agents 
        lstPipelineAgents.DataSource = dsAgents;
        lstPipelineAgents.DataValueField = "usr_key";
        lstPipelineAgents.DataTextField = "AgentName";
        lstPipelineAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstPipelineAgents.SelectedIndex = 0;
        lstPipelineAgents.DataBind();

        // Load Incentive Tracking Agents 
        lstIncentiveTrackingAgents.DataSource = dsAgents;
        lstIncentiveTrackingAgents.DataValueField = "usr_key";
        lstIncentiveTrackingAgents.DataTextField = "AgentName";
        lstIncentiveTrackingAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstIncentiveTrackingAgents.SelectedIndex = 0;
        lstIncentiveTrackingAgents.DataBind();

        // Load Fill Form Agents 
        lstFillFormAgents.DataSource = dsAgents;
        lstFillFormAgents.DataValueField = "usr_key";
        lstFillFormAgents.DataTextField = "AgentName";
        lstFillFormAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstFillFormAgents.SelectedIndex = 0;
        lstFillFormAgents.DataBind();

        // Fall Out Agents 
        lstFallOffAgents.DataSource = dsAgents;
        lstFallOffAgents.DataValueField = "usr_key";
        lstFallOffAgents.DataTextField = "AgentName";
        lstFallOffAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstFallOffAgents.SelectedIndex = 0;
        lstFallOffAgents.DataBind();

        // Lead Volume Agents 
        lstLeadVolumeAgents.DataSource = dsAgents;
        lstLeadVolumeAgents.DataValueField = "usr_key";
        lstLeadVolumeAgents.DataTextField = "AgentName";
        lstLeadVolumeAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstLeadVolumeAgents.SelectedIndex = 0;
        lstLeadVolumeAgents.DataBind();

        //  Carrier Mix Agents 
        lstCarrierMixAgents.DataSource = dsAgents;
        lstCarrierMixAgents.DataValueField = "usr_key";
        lstCarrierMixAgents.DataTextField = "AgentName";
        lstCarrierMixAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstCarrierMixAgents.SelectedIndex = 0;
        lstCarrierMixAgents.DataBind();


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

        // Load Lead Metrics 
        lstlmCampaign.DataSource = dsCampaigns;
        lstlmCampaign.DataValueField = "cmp_id";
        lstlmCampaign.DataTextField = "cmp_title";
        lstlmCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstlmCampaign.SelectedIndex = 0;
        lstlmCampaign.DataBind();

        // Load Fill Form Metrics 
        lstFillFormCampaign.DataSource = dsCampaigns;
        lstFillFormCampaign.DataValueField = "cmp_id";
        lstFillFormCampaign.DataTextField = "cmp_title";
        lstFillFormCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstFillFormCampaign.SelectedIndex = 0;
        lstFillFormCampaign.DataBind();

        // Load Fall Out Metrics 
        lstFallOffCampaign.DataSource = dsCampaigns;
        lstFallOffCampaign.DataValueField = "cmp_id";
        lstFallOffCampaign.DataTextField = "cmp_title";
        lstFallOffCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstFallOffCampaign.SelectedIndex = 0;
        lstFallOffCampaign.DataBind();

        // Load Lead Volume 
        lstLeadVolumeCampaign.DataSource = dsCampaigns;
        lstLeadVolumeCampaign.DataValueField = "cmp_id";
        lstLeadVolumeCampaign.DataTextField = "cmp_title";
        lstLeadVolumeCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstLeadVolumeCampaign.SelectedIndex = 0;
        lstLeadVolumeCampaign.DataBind();

        // Load Carrier Mix
        lstCarrierMixCampaign.DataSource = dsCampaigns;
        lstCarrierMixCampaign.DataValueField = "cmp_id";
        lstCarrierMixCampaign.DataTextField = "cmp_title";
        lstCarrierMixCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstCarrierMixCampaign.SelectedIndex = 0;
        lstCarrierMixCampaign.DataBind();


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

        // Load Lead Metrics 
        lstlmSkillGroup.DataSource = dsSkillGroup;
        lstlmSkillGroup.DataValueField = "skl_id";
        lstlmSkillGroup.DataTextField = "skl_name";
        lstlmSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstlmSkillGroup.SelectedIndex = 0;
        lstlmSkillGroup.DataBind();


        // Fill Form Metrics 
        lstFillFormSkillGroup.DataSource = dsSkillGroup;
        lstFillFormSkillGroup.DataValueField = "skl_id";
        lstFillFormSkillGroup.DataTextField = "skl_name";
        lstFillFormSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstFillFormSkillGroup.SelectedIndex = 0;
        lstFillFormSkillGroup.DataBind();

        // Fall Out Metrics 
        lstFallOffSkillGroup.DataSource = dsSkillGroup;
        lstFallOffSkillGroup.DataValueField = "skl_id";
        lstFallOffSkillGroup.DataTextField = "skl_name";
        lstFallOffSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstFallOffSkillGroup.SelectedIndex = 0;
        lstFallOffSkillGroup.DataBind();

        // Lead Volume Metrics 
        lstLeadVolumeSkillGroup.DataSource = dsSkillGroup;
        lstLeadVolumeSkillGroup.DataValueField = "skl_id";
        lstLeadVolumeSkillGroup.DataTextField = "skl_name";
        lstLeadVolumeSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstLeadVolumeSkillGroup.SelectedIndex = 0;
        lstLeadVolumeSkillGroup.DataBind();

        // Lead Volume Metrics 
        lstCarrierMixSkillGroup.DataSource = dsSkillGroup;
        lstCarrierMixSkillGroup.DataValueField = "skl_id";
        lstCarrierMixSkillGroup.DataTextField = "skl_name";
        lstCarrierMixSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstCarrierMixSkillGroup.SelectedIndex = 0;
        lstCarrierMixSkillGroup.DataBind();

        // Lead Volume Metrics 
        lstCaseSpecialistSkillGroup.DataSource = dsSkillGroup;
        lstCaseSpecialistSkillGroup.DataValueField = "skl_id";
        lstCaseSpecialistSkillGroup.DataTextField = "skl_name";
        lstCaseSpecialistSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstCaseSpecialistSkillGroup.SelectedIndex = 0;
        lstCaseSpecialistSkillGroup.DataBind();


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
        lstlmDateSelected.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstlmDateSelected.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstlmDateSelected.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstlmDateSelected.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstlmDateSelected.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstlmDateSelected.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstlmDateSelected.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstlmDateSelected.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstlmDateSelected.SelectedIndex = 0;
        lstlmDateSelected.DataBind();

        lstQuotaTrackingCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstQuotaTrackingCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstQuotaTrackingCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstQuotaTrackingCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstQuotaTrackingCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstQuotaTrackingCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstQuotaTrackingCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstQuotaTrackingCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstQuotaTrackingCalendar.SelectedIndex = 0;
        lstQuotaTrackingCalendar.DataBind();

        lstCommisionDashboardCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstCommisionDashboardCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstCommisionDashboardCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstCommisionDashboardCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstCommisionDashboardCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstCommisionDashboardCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstCommisionDashboardCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstCommisionDashboardCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstCommisionDashboardCalendar.SelectedIndex = 0;
        lstCommisionDashboardCalendar.DataBind();

        lstGoalCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstGoalCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstGoalCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstGoalCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstGoalCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstGoalCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstGoalCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstGoalCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstGoalCalendar.SelectedIndex = 0;
        lstGoalCalendar.DataBind();

        lstCaseSpecialistCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstCaseSpecialistCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstCaseSpecialistCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstCaseSpecialistCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstCaseSpecialistCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstCaseSpecialistCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstCaseSpecialistCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstCaseSpecialistCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstCaseSpecialistCalendar.SelectedIndex = 0;
        lstCaseSpecialistCalendar.DataBind();

        lstSubmissionEnrollCalendar.Items.Insert(0, new ListItem("Date - Current Year", "0"));
        lstSubmissionEnrollCalendar.Items.Insert(1, new ListItem("Last Year", "1"));
        lstSubmissionEnrollCalendar.SelectedIndex = 0;
        lstSubmissionEnrollCalendar.DataBind();

        lstCarrierMixCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstCarrierMixCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstCarrierMixCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstCarrierMixCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstCarrierMixCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstCarrierMixCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstCarrierMixCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstCarrierMixCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstCarrierMixCalendar.SelectedIndex = 0;
        lstCarrierMixCalendar.DataBind();

        lstFillFormCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstFillFormCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstFillFormCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstFillFormCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstFillFormCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstFillFormCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstFillFormCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstFillFormCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstFillFormCalendar.SelectedIndex = 0;
        lstFillFormCalendar.DataBind();

        lstFallOffCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstFallOffCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstFallOffCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstFallOffCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstFallOffCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstFallOffCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstFallOffCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstFallOffCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstFallOffCalendar.SelectedIndex = 0;
        lstFallOffCalendar.DataBind();

        lstStackedRankingCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstStackedRankingCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstStackedRankingCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstStackedRankingCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstStackedRankingCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstStackedRankingCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstStackedRankingCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstStackedRankingCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstStackedRankingCalendar.SelectedIndex = 0;
        lstStackedRankingCalendar.DataBind();

        lstScoreCardCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstScoreCardCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstScoreCardCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstScoreCardCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstScoreCardCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstScoreCardCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstScoreCardCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstScoreCardCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstScoreCardCalendar.SelectedIndex = 0;
        lstScoreCardCalendar.DataBind();

        lstLeadVolumeCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstLeadVolumeCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstLeadVolumeCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstLeadVolumeCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstLeadVolumeCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstLeadVolumeCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstLeadVolumeCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstLeadVolumeCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstLeadVolumeCalendar.SelectedIndex = 0;
        lstLeadVolumeCalendar.DataBind();

        lstCpaCalendar.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstCpaCalendar.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstCpaCalendar.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstCpaCalendar.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstCpaCalendar.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstCpaCalendar.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstCpaCalendar.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstCpaCalendar.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstCpaCalendar.SelectedIndex = 0;
        lstCpaCalendar.DataBind();
        
    }
    #endregion

    #region PlanType
    private void LoadAllPlanTypeDropDownList() {
        lstQuotaTrackingPlanType.Items.Insert(0, new ListItem("Plan Type - All", "0"));
        lstQuotaTrackingPlanType.Items.Insert(1, new ListItem("MA", "1"));
        lstQuotaTrackingPlanType.Items.Insert(2, new ListItem("MS", "2"));
        lstQuotaTrackingPlanType.Items.Insert(3, new ListItem("PDP", "3"));
        lstQuotaTrackingPlanType.Items.Insert(4, new ListItem("Dental", "4"));
        lstQuotaTrackingPlanType.SelectedIndex = 0;
        lstQuotaTrackingPlanType.DataBind();
    }
    #endregion



}