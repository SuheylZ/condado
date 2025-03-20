#pragma warning disable 618

using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Web.Services;



public partial class DefaultPage : SalesBasePage
{
    public DataSet dsAgents = new DataSet();
    public DataSet dsCampaigns = new DataSet();
    public DataSet dsSkillGroup = new DataSet();
    public string strUserKey = "";
    public bool blIsAutoHome;
    public bool blIsSenior;
    public bool blIsTermLife;

    protected void Page_Load(object sender, EventArgs e)
    {
        LoadAllAgentDropDownList();
        LoadAllCampaignsDropDownList();
        LoadAllSkillGroupDropDownList();
        LoadAllDateDropDownList();
        strUserKey = (Page as SalesBasePage).CurrentUser.Key.ToString();
        UserInfo.Value = strUserKey;
        //blIsAutoHome = ApplicationSettings.IsAutoHome;
        //blIsSenior = ApplicationSettings.IsSenior;
        //blIsTermLife = ApplicationSettings.IsTermLife;
        blIsAutoHome =Engine.ApplicationSettings.IsAutoHome;
        blIsSenior = Engine.ApplicationSettings.IsSenior;
        blIsTermLife =Engine.ApplicationSettings.IsTermLife;
        IsAutoHome.Value = blIsAutoHome.ToString();
        IsSenior.Value = blIsSenior.ToString();
        IsTermLife.Value = blIsTermLife.ToString();

        //string[] mystring = LoadAllReports(1);
        //Response.Write(mystring[0]);

    }

    #region Agents
    private void LoadAllAgentDropDownList()
    {
        // Load DataSet for Agents
        dsAgents = GetAgents();

        // Load Sales Metrics 
        lstSmAgents.DataSource = dsAgents;
        lstSmAgents.DataValueField = "usr_key";
        lstSmAgents.DataTextField = "AgentName";
        lstSmAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstSmAgents.SelectedIndex = 0;
        lstSmAgents.DataBind();

        // Load Lead Metrics 
        lstLmAgents.DataSource = dsAgents;
        lstLmAgents.DataValueField = "usr_key";
        lstLmAgents.DataTextField = "AgentName";
        lstLmAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstLmAgents.SelectedIndex = 0;
        lstLmAgents.DataBind();

        //  Carrier Mix Agents 
        lstCarrierMixAgents.DataSource = dsAgents;
        lstCarrierMixAgents.DataValueField = "usr_key";
        lstCarrierMixAgents.DataTextField = "AgentName";
        lstCarrierMixAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstCarrierMixAgents.SelectedIndex = 0;
        lstCarrierMixAgents.DataBind();

        // Load Fill Form Agents 
        lstFillFormAgents.DataSource = dsAgents;
        lstFillFormAgents.DataValueField = "usr_key";
        lstFillFormAgents.DataTextField = "AgentName";
        lstFillFormAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstFillFormAgents.SelectedIndex = 0;
        lstFillFormAgents.DataBind();

        // Load Incentive Tracking Agents 
        lstIncentiveTrackingAgents.DataSource = dsAgents;
        lstIncentiveTrackingAgents.DataValueField = "usr_key";
        lstIncentiveTrackingAgents.DataTextField = "AgentName";
        lstIncentiveTrackingAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstIncentiveTrackingAgents.SelectedIndex = 0;
        lstIncentiveTrackingAgents.DataBind();

        // Lead Volume Agents 
        lstLeadVolumeAgents.DataSource = dsAgents;
        lstLeadVolumeAgents.DataValueField = "usr_key";
        lstLeadVolumeAgents.DataTextField = "AgentName";
        lstLeadVolumeAgents.Items.Insert(0, new ListItem("Agents - All", "0"));
        lstLeadVolumeAgents.SelectedIndex = 0;
        lstLeadVolumeAgents.DataBind();

        // Load Pipeline Agents 
        lstPipelineAgents.DataSource = dsAgents;
        lstPipelineAgents.DataValueField = "usr_key";
        lstPipelineAgents.DataTextField = "AgentName";
        lstPipelineAgents.DataBind();

        //  Prioritized List Agents 
        lstPrioritizedListAgents.DataSource = dsAgents;
        lstPrioritizedListAgents.DataValueField = "usr_key";
        lstPrioritizedListAgents.DataTextField = "AgentName";
        //lstPrioritizedListAgents.SelectedValue = strUserKey;
        lstPrioritizedListAgents.DataBind();



        // Dispose of DataSet
        dsAgents.Dispose();
    }

    private DataSet GetAgents()
    {
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

    private void LoadAllCampaignsDropDownList()
    {
        // Load DataSet for Agents
        dsCampaigns = GetCampaigns();

        // Load Sales Metrics 
        lstSmCampaign.DataSource = dsCampaigns;
        lstSmCampaign.DataValueField = "cmp_id";
        lstSmCampaign.DataTextField = "cmp_title";
        lstSmCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstSmCampaign.SelectedIndex = 0;
        lstSmCampaign.DataBind();

        // Load Lead Metrics 
        lstLmCampaign.DataSource = dsCampaigns;
        lstLmCampaign.DataValueField = "cmp_id";
        lstLmCampaign.DataTextField = "cmp_title";
        lstLmCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstLmCampaign.SelectedIndex = 0;
        lstLmCampaign.DataBind();

        // Load Carrier Mix
        lstCarrierMixCampaign.DataSource = dsCampaigns;
        lstCarrierMixCampaign.DataValueField = "cmp_id";
        lstCarrierMixCampaign.DataTextField = "cmp_title";
        lstCarrierMixCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstCarrierMixCampaign.SelectedIndex = 0;
        lstCarrierMixCampaign.DataBind();

        // Load Fill Form Metrics 
        lstFillFormCampaign.DataSource = dsCampaigns;
        lstFillFormCampaign.DataValueField = "cmp_id";
        lstFillFormCampaign.DataTextField = "cmp_title";
        lstFillFormCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstFillFormCampaign.SelectedIndex = 0;
        lstFillFormCampaign.DataBind();

        // Load Lead Volume 
        lstLeadVolumeCampaign.DataSource = dsCampaigns;
        lstLeadVolumeCampaign.DataValueField = "cmp_id";
        lstLeadVolumeCampaign.DataTextField = "cmp_title";
        lstLeadVolumeCampaign.Items.Insert(0, new ListItem("Campaigns - All", "0"));
        lstLeadVolumeCampaign.SelectedIndex = 0;
        lstLeadVolumeCampaign.DataBind();


        // Dispose of DataSet
        dsCampaigns.Dispose();
    }

    private DataSet GetCampaigns()
    {
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
    private void LoadAllSkillGroupDropDownList()
    {
        // Load DataSet for Agents
        dsSkillGroup = GetSkillGroup();

        // Load Sales Metrics 
        lstSmSkillGroup.DataSource = dsSkillGroup;
        lstSmSkillGroup.DataValueField = "skl_id";
        lstSmSkillGroup.DataTextField = "skl_name";
        lstSmSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstSmSkillGroup.SelectedIndex = 0;
        lstSmSkillGroup.DataBind();

        // Load Lead Metrics 
        lstLmSkillGroup.DataSource = dsSkillGroup;
        lstLmSkillGroup.DataValueField = "skl_id";
        lstLmSkillGroup.DataTextField = "skl_name";
        lstLmSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstLmSkillGroup.SelectedIndex = 0;
        lstLmSkillGroup.DataBind();

        // Carrier Mix Metrics 
        lstCarrierMixSkillGroup.DataSource = dsSkillGroup;
        lstCarrierMixSkillGroup.DataValueField = "skl_id";
        lstCarrierMixSkillGroup.DataTextField = "skl_name";
        lstCarrierMixSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstCarrierMixSkillGroup.SelectedIndex = 0;
        lstCarrierMixSkillGroup.DataBind();

        // Case Specialist Metrics 
        lstCaseSpecialistSkillGroup.DataSource = dsSkillGroup;
        lstCaseSpecialistSkillGroup.DataValueField = "skl_id";
        lstCaseSpecialistSkillGroup.DataTextField = "skl_name";
        lstCaseSpecialistSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstCaseSpecialistSkillGroup.SelectedIndex = 0;
        lstCaseSpecialistSkillGroup.DataBind();

        // Fill Form Metrics 
        lstFillFormSkillGroup.DataSource = dsSkillGroup;
        lstFillFormSkillGroup.DataValueField = "skl_id";
        lstFillFormSkillGroup.DataTextField = "skl_name";
        lstFillFormSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstFillFormSkillGroup.SelectedIndex = 0;
        lstFillFormSkillGroup.DataBind();

        // Lead Volume Metrics 
        lstLeadVolumeSkillGroup.DataSource = dsSkillGroup;
        lstLeadVolumeSkillGroup.DataValueField = "skl_id";
        lstLeadVolumeSkillGroup.DataTextField = "skl_name";
        lstLeadVolumeSkillGroup.Items.Insert(0, new ListItem("Skill Group - All", "0"));
        lstLeadVolumeSkillGroup.SelectedIndex = 0;
        lstLeadVolumeSkillGroup.DataBind();

        // Dispose of DataSet
        dsSkillGroup.Dispose();
    }

    private DataSet GetSkillGroup()
    {
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
    private void LoadAllDateDropDownList()
    {
        // Sales Metrics Dates
        lstSmDateSelected.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstSmDateSelected.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstSmDateSelected.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstSmDateSelected.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstSmDateSelected.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstSmDateSelected.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstSmDateSelected.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstSmDateSelected.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstSmDateSelected.SelectedIndex = 0;
        lstSmDateSelected.DataBind();

        // Lead Metrics Dates
        lstLmDateSelected.Items.Insert(0, new ListItem("Date - Today", "0"));
        lstLmDateSelected.Items.Insert(1, new ListItem("Yesterday", "1"));
        lstLmDateSelected.Items.Insert(2, new ListItem("Week to Date", "2"));
        lstLmDateSelected.Items.Insert(3, new ListItem("Month to Date", "3"));
        lstLmDateSelected.Items.Insert(4, new ListItem("Last 7 Days", "4"));
        lstLmDateSelected.Items.Insert(5, new ListItem("Last 14 Days", "5"));
        lstLmDateSelected.Items.Insert(6, new ListItem("Last 30 days", "6"));
        lstLmDateSelected.Items.Insert(7, new ListItem("Custom Date", "7"));
        lstLmDateSelected.SelectedIndex = 0;
        lstLmDateSelected.DataBind();

        // ScoreCard Dates
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

        // Stacked Ranking Dates
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

        // Carrier Mix Dates
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

        // Case Specialist Dates
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

        // Fill Form Dates
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

        // Goal Dates
        // Month
        int currentMonth = DateTime.Now.Month;
        lstGoalMonth.Items.Insert(0, new ListItem("January", "1"));
        lstGoalMonth.Items.Insert(1, new ListItem("February", "2"));
        lstGoalMonth.Items.Insert(2, new ListItem("March", "3"));
        lstGoalMonth.Items.Insert(3, new ListItem("April", "4"));
        lstGoalMonth.Items.Insert(4, new ListItem("May", "5"));
        lstGoalMonth.Items.Insert(5, new ListItem("June", "6"));
        lstGoalMonth.Items.Insert(6, new ListItem("July", "7"));
        lstGoalMonth.Items.Insert(7, new ListItem("August", "8"));
        lstGoalMonth.Items.Insert(8, new ListItem("September", "9"));
        lstGoalMonth.Items.Insert(9, new ListItem("October", "10"));
        lstGoalMonth.Items.Insert(10, new ListItem("November", "11"));
        lstGoalMonth.Items.Insert(11, new ListItem("December", "12"));
        lstGoalMonth.SelectedIndex = currentMonth - 1;
        lstGoalMonth.DataBind();
        // Year
        int currentYear = DateTime.Now.Year;
        for (int i = 0; i < 3; i++)
        {
            lstGoalYear.Items.Insert(i, new ListItem(currentYear.ToString(), currentYear.ToString()));
            currentYear = currentYear - 1;
        }
        lstGoalYear.DataBind();


        // Lead Volume Dates
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

        // Quota Tracking Dates
        // Month
        int currentMonth1 = DateTime.Now.Month;
        lstQuotaTrackingMonth.Items.Insert(0, new ListItem("January", "1"));
        lstQuotaTrackingMonth.Items.Insert(1, new ListItem("February", "2"));
        lstQuotaTrackingMonth.Items.Insert(2, new ListItem("March", "3"));
        lstQuotaTrackingMonth.Items.Insert(3, new ListItem("April", "4"));
        lstQuotaTrackingMonth.Items.Insert(4, new ListItem("May", "5"));
        lstQuotaTrackingMonth.Items.Insert(5, new ListItem("June", "6"));
        lstQuotaTrackingMonth.Items.Insert(6, new ListItem("July", "7"));
        lstQuotaTrackingMonth.Items.Insert(7, new ListItem("August", "8"));
        lstQuotaTrackingMonth.Items.Insert(8, new ListItem("September", "9"));
        lstQuotaTrackingMonth.Items.Insert(9, new ListItem("October", "10"));
        lstQuotaTrackingMonth.Items.Insert(10, new ListItem("November", "11"));
        lstQuotaTrackingMonth.Items.Insert(11, new ListItem("December", "12"));
        lstQuotaTrackingMonth.SelectedIndex = currentMonth1 - 1;
        lstQuotaTrackingMonth.DataBind();
        // Year
        int currentYear1 = DateTime.Now.Year;
        for (int i = 0; i < 3; i++)
        {
            lstQuotaTrackingYear.Items.Insert(i, new ListItem(currentYear1.ToString(), currentYear1.ToString()));
            currentYear1 = currentYear1 - 1;
        }
        lstQuotaTrackingYear.DataBind();

        // Submission Enroll Dates

        // Year
        int currentYear2 = DateTime.Now.Year;
        for (int i = 0; i < 3; i++)
        {
            lstSubmissionEnrollCalendar.Items.Insert(i, new ListItem(currentYear2.ToString(), currentYear2.ToString()));
            currentYear2 = currentYear2 - 1;
        }
        lstSubmissionEnrollCalendar.DataBind();

    }
    #endregion

}
