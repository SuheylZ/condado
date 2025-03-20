using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SQL = System.Data.Objects.SqlClient.SqlFunctions;
using SalesTool.DataAccess.Models;
using SalesTool.DataAccess;

namespace SalesTool.Web.UserControls
{
    public partial class StatisticsControl : HomeUserControl, IStatisticsMetricsNotifier
    {
        #region Members/Properties
        
        const string K_Sales = "1";
        const string K_Leads = "2";
        const string K_CallCenter = "3";

        IStatisticsMetricsHandler _handler = null;

        //SZ [Jul 17, 2013] These constants are not made public as they are only useful inside this class.
        // hence encapsulated in this class
        const string K_NoValue = "-1";
        const string K_AllValues = "-2";
        const string K_MetricsStore = "__Statictis_Metrics_Store__";

        HiddenField _hdnMetricsType = null;

        #endregion

        #region Events
        /// <summary>
        /// Override event calls on page load
        /// </summary>
        /// <param name="bFirstTime"></param>
        protected override void InnerInit(bool bFirstTime)
        {
            _hdnMetricsType = new HiddenField
            {
                ID = K_MetricsStore,
                Value = StatisticsMetricsType.Sales.ToString(),
                ClientIDMode = System.Web.UI.ClientIDMode.Predictable
            };
            Controls.Add(_hdnMetricsType);
            if (bFirstTime)
            {
                BindEvents();
                InitializeUI();
                ProcessLeadAndSalesMetric();
            }
            
        }
        /// <summary>
        /// Override event calls on page load
        /// </summary>
        /// <param name="bFirstTime"></param>
        protected override void InnerLoad(bool bFirstTime)
        {
            BindEvents();
        }
        /// <summary>
        /// Binds the event to different controls
        /// </summary>
        void BindEvents()
        {
            tlkTabs.TabClick += (o, a) => StatisticsMetricsChanged(a.Tab.Value);
            tlkClose.Click += (o, a) => CloseDateWindow();
            ddlTime.ItemSelected += (o, a) => FilterByTime(Convert.ToInt32(a.Value));
            ddlCampaign.SelectedIndexChanged += (o, a) => ProcessLeadAndSalesMetric();
            ddlAgents.SelectedIndexChanged += (o, a) => ProcessLeadAndSalesMetric();
            ddlAgentSkillGroups.SelectedIndexChanged += (o, a) => ProcessLeadAndSalesMetric();
            tlkGetResults.Click += (o, a) => GetCustomDateResults();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Registering the statistics metric event handler
        /// </summary>
        /// <param name="handler"></param>
        public void Register(IStatisticsMetricsHandler handler)
        {
            _handler = handler;
            _handler.MetricsChanged(StatisticsMetricsType.Sales);
        }
        /// <summary>
        /// Identify the metric tab for changed event
        /// </summary>
        /// <param name="value"></param>
        private void StatisticsMetricsChanged(string value)
        {
            if (_handler != null)
            {
                switch (value)
                {
                    case K_Sales: _handler.MetricsChanged(StatisticsMetricsType.Sales); break;
                    case K_Leads: _handler.MetricsChanged(StatisticsMetricsType.Leads); break;
                    case K_CallCenter:_handler.MetricsChanged(StatisticsMetricsType.CallCenter); break;
                }
            }
        }
        /// <summary>
        /// Closes the Custom Date window.
        /// </summary>
        void CloseDateWindow()
        {
            tlkDateWindow.Visible = false;
            tlkDateWindow.VisibleOnPageLoad = false;
        }
        /// <summary>
        /// Initializes the UI controls
        /// </summary>
        void InitializeUI()
        {
            ddlTime.SelectedIndex = 0;
            //Bind campaigns
            ddlCampaign.DataSource = Engine.ManageCampaignActions.GetAll();
            ddlCampaign.DataBind();            
            ddlCampaign.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("--All Campaigns--", K_NoValue));
            ddlCampaign.SelectedValue = K_NoValue;

            //Bind Skill groups
            ddlAgentSkillGroups.DataSource = Engine.SkillGroupActions.All;
            ddlAgentSkillGroups.DataBind();            
            ddlAgentSkillGroups.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("--All Skill Groups--", K_NoValue));
            ddlAgentSkillGroups.SelectedValue = K_NoValue;

            //Bind agents(users)
            ddlAgents.DataSource = Engine.UserActions.GetAll().OrderBy(x => x.FirstName);
            ddlAgents.DataBind();            
            ddlAgents.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("--All Agents--", K_NoValue));
            ddlAgents.SelectedValue = K_NoValue;
        }
        /// <summary>
        /// Get/Apply the selected time filter 
        /// </summary>
        /// <param name="value">Selected Time value</param>
        void FilterByTime(int value)
        {
            if (value == 8)
            {
                OpenDateWindow();
            }
            else
            {
                ProcessLeadAndSalesMetric();
            }
        }
        /// <summary>
        /// Open custom date window and reset the controls in it.
        /// 
        /// </summary>
        void OpenDateWindow()
        {
            tlkStartDate.SelectedDate = null;
            tlkEndDate.SelectedDate = null;
            tlkDateWindow.Visible = true;
            tlkDateWindow.VisibleOnPageLoad = true;
        }
        /// <summary>
        /// YA[Aug 24, 2013] Processes and displays the Leads Metric values
        /// </summary>
        private void ProcessLeadsMetrics()
        {
            IQueryable<LeadMetric> nLeadMetric = Engine.LeadMetricActions.All;
            Guid userKey = (Page as SalesBasePage).CurrentUser.Key;
            //nLeadMetric = nLeadMetric.Where(x => x.UserKey == userKey);
            //Checks agent filter selection
            if (ddlAgents.SelectedIndex > 0)
            {
                Guid agentID = Guid.Empty;
                if (Guid.TryParse(ddlAgents.SelectedValue, out agentID))
                    nLeadMetric = nLeadMetric.Where(x => x.UserKey == agentID);
            }
            //Checks campaign filter selection
            if (ddlCampaign.SelectedIndex > 0)
            {
                int campaignID = 0;
                if (int.TryParse(ddlCampaign.SelectedValue, out campaignID))
                    nLeadMetric = nLeadMetric.Where(x => x.CampaignID == campaignID);
            }
            //Checks skill group filter selection
            if (ddlAgentSkillGroups.SelectedIndex > 0)
            {
                int skillGroupID = 0;
                if (int.TryParse(ddlAgentSkillGroups.SelectedValue, out skillGroupID))
                    nLeadMetric = nLeadMetric.Where(x => x.SkillGroupID == skillGroupID);
            }
            //Checks time filter selection
            if (ddlTime.SelectedIndex >= 0)
                nLeadMetric = FilterbyTimeForAccountCreateDate(nLeadMetric);

            IQueryable<LeadMetric> nLeadMetricValidLeads = nLeadMetric;
            IQueryable<LeadMetric> nLeadMetricPendingStatus = nLeadMetric;
            IQueryable<LeadMetric> nLeadMetricPercentValidLeads = nLeadMetric;

            string queryGenerated = ((System.Data.Objects.ObjectQuery)nLeadMetric).ToTraceString();

            int totalValidLeads = nLeadMetricPercentValidLeads.Select(x => x.LeadKey).ToList().Distinct().Count();
            //New leads calculation and its value showing
            nLeadMetric = nLeadMetric.Where(x => x.StatusTitle.StartsWith("New"));
            lblLeadMetricNewLeads.Text = nLeadMetric.Select(x => x.LeadKey).ToList().Distinct().Count().ToString();

            //Valid leads query condition
            nLeadMetricValidLeads = nLeadMetricValidLeads.Where(x => !x.StatusTitle.StartsWith("New") && !x.StatusTitle.StartsWith("Invalid") && !x.StatusTitle.StartsWith("Duplicate"));

            //Percent Valid leads calculation and its value showing
            nLeadMetricPercentValidLeads = nLeadMetricPercentValidLeads.Where(x => !x.StatusTitle.StartsWith("New"));
            float forPercentValidLeads = nLeadMetricPercentValidLeads.Select(x => x.LeadKey).ToList().Distinct().Count();
            float LeadMetricValidLeads = nLeadMetricValidLeads.Select(x => x.LeadKey).ToList().Distinct().Count();
            float percentLeadMetricValidLeads = 0;
            if (LeadMetricValidLeads > 0 && totalValidLeads > 0)
                percentLeadMetricValidLeads = LeadMetricValidLeads / totalValidLeads * 100;
            lblLeadMetricValidLeads.Text = LeadMetricValidLeads.ToString();
            lblLeadMetricPercentValid.Text = string.Format("{0:##0.##}", percentLeadMetricValidLeads) + "%";

            //Contacted Valid leads and Percent Contacted valid leads calculation and its value showing
            float ValidLeadsContacted = nLeadMetricValidLeads.Where(x => x.ActionContactFlag == true).Select(x => x.LeadKey).ToList().Distinct().Count();
            float percentValidLeadsContacted = 0;
            if (ValidLeadsContacted > 0 && LeadMetricValidLeads > 0)
                percentValidLeadsContacted = ValidLeadsContacted / LeadMetricValidLeads * 100;
            lblLeadMetricContacted.Text = ValidLeadsContacted.ToString();
            lblLeadMetricPercentContacted.Text = string.Format("{0:##0.##}", percentValidLeadsContacted) + "%";

            //No Quoted leads and Percent No Quoted leads calculation and its value showing
            float LeadMetricNoQuoted = FilterbyTimeForQuoted(nLeadMetricValidLeads).Select(x => x.LeadKey).ToList().Distinct().Count();
            float percentLeadMetricNoQuoted = 0;
            if (LeadMetricNoQuoted > 0 && LeadMetricValidLeads > 0)
                percentLeadMetricNoQuoted = LeadMetricNoQuoted / LeadMetricValidLeads * 100;
            lblLeadMetricNoQuoted.Text = LeadMetricNoQuoted.ToString();
            lblLeadMetricPercentQuoted.Text = string.Format("{0:##0.##}", percentLeadMetricNoQuoted) + "%";

            //Closed Valid leads and Percent Closed valid leads calculation and its value showing
            float LeadMetricPercentClosed = FilterbyTimeForSubmitEnrollDate(nLeadMetricValidLeads).Select(x => x.LeadKey).ToList().Distinct().Count();
            float percentLeadMetricPercentClosed = 0;
            if (LeadMetricPercentClosed > 0 && LeadMetricValidLeads > 0)
                percentLeadMetricPercentClosed = LeadMetricPercentClosed / LeadMetricValidLeads * 100;
            lblLeadMetricClosed.Text = LeadMetricPercentClosed.ToString();
            lblLeadMetricPercentClosed.Text = string.Format("{0:##0.##}", percentLeadMetricPercentClosed) + "%";

            //Pending Status leads and Percent Pending Status leads calculation and its value showing
            float LeadMetricPendingStatus = nLeadMetricPendingStatus.Where(x => !x.StatusTitle.StartsWith("Invalid") || !x.StatusTitle.StartsWith("Withdrawn") || !x.StatusTitle.StartsWith("Submitted")).Select(x => x.LeadKey).ToList().Distinct().Count();
            float percentLeadMetricPendingStatus = 0;
            if (LeadMetricPendingStatus > 0 && totalValidLeads > 0)
                percentLeadMetricPendingStatus = LeadMetricPendingStatus / totalValidLeads * 100;
            //lblLeadMetricPercentPendingStatus.Text = string.Format("{0:##0.##}", percentLeadMetricPendingStatus) + "%";             
        }
        /// <summary>
        /// Processes and displays the Sales Metric values
        /// </summary>
        private void ProcessSalesMetrics()
        {
            IQueryable<LeadMetric> nLeadMetric = Engine.LeadMetricActions.All;
            Guid userKey = (Page as SalesBasePage).CurrentUser.Key;
            //nLeadMetric = nLeadMetric.Where(x => x.UserKey == userKey);
            nLeadMetric = nLeadMetric.Where(x => x.AccountHistoryComments.Contains("User assigned"));
            nLeadMetric = nLeadMetric.Where(x => !x.StatusTitle.StartsWith("New") && !x.StatusTitle.StartsWith("Invalid"));
            //Checks agent filter selection
            if (ddlAgents.SelectedIndex > 0)
            {
                Guid agentID = Guid.Empty;
                if (Guid.TryParse(ddlAgents.SelectedValue, out agentID))
                    nLeadMetric = nLeadMetric.Where(x => x.UserKey == agentID);
            }
            //Checks campaign filter selection
            if(ddlCampaign.SelectedIndex > 0)
            {
                int campaignID = 0;
                if (int.TryParse(ddlCampaign.SelectedValue, out campaignID))
                    nLeadMetric = nLeadMetric.Where(x => x.CampaignID == campaignID);
            }
            //Checks skill group filter selection
            if(ddlAgentSkillGroups.SelectedIndex > 0) 
            {
                int skillGroupID = 0;
                if (int.TryParse(ddlAgentSkillGroups.SelectedValue, out skillGroupID))
                    nLeadMetric = nLeadMetric.Where(x => x.SkillGroupID == skillGroupID);
            }
            //Checks time filter selection
            if (ddlTime.SelectedIndex >= 0)
                nLeadMetric = FilterbyTimeForAccountHistory(nLeadMetric);
            IQueryable<LeadMetric> nLeadMetricValidLeads = nLeadMetric;

            
            //Valid Leads calculation and value display.
            var T = nLeadMetric.Select(x => new { ID = x.LeadKey }).GroupBy(p => p.ID).Select(g => g.FirstOrDefault()).Distinct();
            if (T != null) { int count = T.Count(); }
            //lblValidLeads.Text = nLeadMetric.Select(x => x.LeadKey).ToList().Distinct().Count();
            lblValidLeads.Text = nLeadMetric.Select(x => x.LeadKey).ToList().Distinct().Count().ToString();
            //string queryGenerated = ((System.Data.Objects.ObjectQuery)nLeadMetric).ToTraceString();
            //queryGenerated = ((System.Data.Objects.ObjectQuery)T).ToTraceString();
            
            //Contacted Leads calculation and value display.
            lblNoOfContacts.Text = nLeadMetric.Where(x => x.ActionContactFlag == true).Select(x => x.LeadKey).ToList().Distinct().Count().ToString();

            

            //Closed Leads calculation and value display.
            var U = FilterbyTimeForSubmitEnrollDate(nLeadMetric);
            lblCloses.Text = U.Select(x => x.LeadKey).ToList().Distinct().Count().ToString();
            //Quoted Leads calculation and value display.
            lblNoQuoted.Text = FilterbyTimeForQuoted(nLeadMetricValidLeads).Select(x => x.LeadKey).ToList().Distinct().Count().ToString();

        }
        /// <summary>
        /// YA[Aug 24, 2013] Time filter based on Submit and Enrollment Date
        /// </summary>
        /// <param name="query">Base Query</param>
        /// <returns>Query result after applying the filter</returns>
        private IQueryable<LeadMetric> FilterbyTimeForSubmitEnrollDate(IQueryable<LeadMetric> query)
        {
            DateTime dtTarget = DateTime.Now, dtTarget2 = DateTime.Now;
            //Time filter selected from the GUI
            string filter = ddlTime.SelectedItem.Text;
            //Check the switch cases for different time filters
            switch (filter)
            {
                case "Today":
                    query = query.Where(x => SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget) == 0 || SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget) == 0 || SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget) == 0 || SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget) == 0);
                    break;

                case "Yesterday":
                    query = query.Where(x => SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget) == 1 || SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget) == 1 || SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget) == 1 || SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget) == 1);
                    break;
                case "Week to Date":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(6);
                    query = query.Where(x =>
                        (SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget2) >= 0)                        
                        );
                    break;
                case "Month to Date":
                    query = query.Where(x => SQL.DateDiff("month", x.MedSuppSubmitDate, dtTarget) == 0
                        || SQL.DateDiff("month", x.MAPDPEnrollmentDate, dtTarget) == 0
                        || SQL.DateDiff("month", x.DentalVisionSubmitDate, dtTarget) == 0
                        || SQL.DateDiff("month", x.AutoHomePolicyBoundDate, dtTarget) == 0
                        );                   
                    break;
                case "Last 7 Days":                    
                    dtTarget = dtTarget.AddDays(-7);
                    query = query.Where(x =>
                        (SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget2) >= 0)
                        );
                    break;
                case "Last 14 Days":
                    dtTarget = dtTarget.AddDays(-14);
                    query = query.Where(x =>
                        (SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget2) >= 0)
                        );
                    break;
                case "Last 30 Days":
                    dtTarget = dtTarget.AddDays(-30);
                    query = query.Where(x =>
                        (SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget2) >= 0)
                        );
                    break;
                case "Last Month":
                    query = query.Where(x => SQL.DateDiff("month", x.MedSuppSubmitDate, dtTarget) == 1
                        || SQL.DateDiff("month", x.MAPDPEnrollmentDate, dtTarget) == 1
                        || SQL.DateDiff("month", x.DentalVisionSubmitDate, dtTarget) == 1
                        || SQL.DateDiff("month", x.AutoHomePolicyBoundDate, dtTarget) == 1
                        );
                    break;
                case "This Month":
                   query = query.Where(x => SQL.DateDiff("month", x.MedSuppSubmitDate, dtTarget) == 0
                        || SQL.DateDiff("month", x.MAPDPEnrollmentDate, dtTarget) == 0
                        || SQL.DateDiff("month", x.DentalVisionSubmitDate, dtTarget) == 0
                        || SQL.DateDiff("month", x.AutoHomePolicyBoundDate, dtTarget) == 0
                        );
                    break;
                case "Custom":
                    dtTarget = tlkStartDate.SelectedDate.HasValue ? tlkStartDate.SelectedDate.Value : DateTime.Now;
                    dtTarget2 = tlkEndDate.SelectedDate.HasValue ? tlkEndDate.SelectedDate.Value : DateTime.Now;
                    query = query.Where(x =>
                        (SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MedSuppSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.MAPDPEnrollmentDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.DentalVisionSubmitDate, dtTarget2) >= 0)
                        || (SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AutoHomePolicyBoundDate, dtTarget2) >= 0)
                        );
                    break;
            }
            return query;
        }
        /// <summary>
        /// YA[Aug 24, 2013] Time filter based on Account history date
        /// </summary>
        /// <param name="query">Base Query</param>
        /// <returns>Query result after applying the filter</returns>
        private IQueryable<LeadMetric> FilterbyTimeForAccountHistory(IQueryable<LeadMetric> query)
        {
            DateTime dtTarget = DateTime.Now, dtTarget2 = DateTime.Now;
            //Time filter selected from the GUI
            string filter = ddlTime.SelectedItem.Text;
            //Check the switch cases for different time filters
            switch (filter)
            {
                case "Today":
                    query = query.Where(x => SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget) == 0);
                    break;
                case "Yesterday":
                    query = query.Where(x => SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget) == 1);
                    break;
                case "Week to Date":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(6);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget2) >= 0);
                    break;
                case "Month to Date":
                    query = query.Where(x => SQL.DateDiff("month", x.AccountHistoryAddedDate, dtTarget) == 0);                    
                    break;
                case "Last 7 Days":                    
                    dtTarget = dtTarget.AddDays(-7);                    
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget2) >= 0);
                    break;
                case "Last 14 Days":
                    dtTarget = dtTarget.AddDays(-14);                    
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget2) >= 0);                    
                    break;
                case "Last 30 Days":
                    dtTarget = dtTarget.AddDays(-30);                    
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget2) >= 0);                    
                    break;
                case "Last Month":
                    query = query.Where(x => SQL.DateDiff("month", x.AccountHistoryAddedDate, dtTarget) == 1);
                    break;
                case "This Month":
                    query = query.Where(x => SQL.DateDiff("month", x.AccountHistoryAddedDate, dtTarget) == 0);
                    break;
                case "Custom":
                    dtTarget = tlkStartDate.SelectedDate.HasValue ? tlkStartDate.SelectedDate.Value : DateTime.Now;
                    dtTarget2 = tlkEndDate.SelectedDate.HasValue ? tlkEndDate.SelectedDate.Value : DateTime.Now;
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountHistoryAddedDate, dtTarget2) >= 0);
                    break;
            }
            return query;
        }
        /// <summary>
        /// YA[Aug 23, 2013] Time filter based on Quoted date
        /// </summary>
        /// <param name="query">Base Query</param>
        /// <returns>Query result after applying the filter</returns>
        private IQueryable<LeadMetric> FilterbyTimeForQuoted(IQueryable<LeadMetric> query)
        {
            DateTime dtTarget = DateTime.Now, dtTarget2 = DateTime.Now;
            //Time filter selected from the GUI
            string filter = ddlTime.SelectedItem.Text;
            //Check the switch cases for different time filters
            switch (filter)
            {
                case "Today":
                    query = query.Where(x => SQL.DateDiff("day", x.QuotedDate, dtTarget) == 0);
                    break;
                case "Yesterday":
                    query = query.Where(x => SQL.DateDiff("day", x.QuotedDate, dtTarget) == 1);
                    break;
                case "Week to Date":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(6);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.QuotedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.QuotedDate, dtTarget2) >= 0);
                    break;
                case "Month to Date":
                    query = query.Where(x => SQL.DateDiff("month", x.QuotedDate, dtTarget) == 0);
                    break;
                case "Last 7 Days":
                    dtTarget = dtTarget.AddDays(-7);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.QuotedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.QuotedDate, dtTarget2) >= 0);                    
                    break;                    
                case "Last 14 Days":
                    dtTarget = dtTarget.AddDays(-14);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.QuotedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.QuotedDate, dtTarget2) >= 0);                    
                    break;
                case "Last 30 Days":
                    dtTarget = dtTarget.AddDays(-30);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.QuotedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.QuotedDate, dtTarget2) >= 0);                    
                    break;
                case "Last Month":
                    query = query.Where(x => SQL.DateDiff("month", x.QuotedDate, dtTarget) == 1);
                    break;
                case "This Month":
                    query = query.Where(x => SQL.DateDiff("month", x.QuotedDate, dtTarget) == 0);
                    break;
                case "Custom":
                    dtTarget = tlkStartDate.SelectedDate.HasValue ? tlkStartDate.SelectedDate.Value : DateTime.Now;
                    dtTarget2 = tlkEndDate.SelectedDate.HasValue ? tlkEndDate.SelectedDate.Value : DateTime.Now;
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.QuotedDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.QuotedDate, dtTarget2) >= 0);                    
                    break;
            }
            return query;
        }
        /// <summary>
        /// YA[Aug 23, 2013] Time filter based on Account create date
        /// </summary>
        /// <param name="query">Base Query</param>
        /// <returns>Query result after applying the filter</returns>
        private IQueryable<LeadMetric> FilterbyTimeForAccountCreateDate(IQueryable<LeadMetric> query)
        {
            DateTime dtTarget = DateTime.Now, dtTarget2 = DateTime.Now;
            //Time filter selected from the GUI
            string filter = ddlTime.SelectedItem.Text;
            //Check the switch cases for different time filters
            switch (filter)
            {
                case "Today":
                    query = query.Where(x => SQL.DateDiff("day", x.AccountCreateDate, dtTarget) == 0);
                    break;

                case "Yesterday":
                    query = query.Where(x => SQL.DateDiff("day", x.AccountCreateDate, dtTarget) == 1);
                    break;
                case "Week to Date":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(6);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget2) >= 0);
                    break;

                case "Month to Date":
                    query = query.Where(x => SQL.DateDiff("month", x.AccountCreateDate, dtTarget) == 0);
                    break;
                case "Last 7 Days":
                    dtTarget = dtTarget.AddDays(-7);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget2) >= 0);
                    break;                    
                case "Last 14 Days":
                    dtTarget = dtTarget.AddDays(-14);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget2) >= 0);
                    break;
                case "Last 30 Days":
                    dtTarget = dtTarget.AddDays(-30);
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget2) >= 0);
                    break;
                case "Last Month":
                    query = query.Where(x => SQL.DateDiff("month", x.AccountCreateDate, dtTarget) == 1);
                    break;
                case "This Month":
                    query = query.Where(x => SQL.DateDiff("month", x.AccountCreateDate, dtTarget) == 0);
                    break;
                case "Custom":
                    dtTarget = tlkStartDate.SelectedDate.HasValue ? tlkStartDate.SelectedDate.Value : DateTime.Now;
                    dtTarget2 = tlkEndDate.SelectedDate.HasValue ? tlkEndDate.SelectedDate.Value : DateTime.Now;
                    query = query.Where(x =>
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget) <= 0 &&
                        SQL.DateDiff("day", x.AccountCreateDate, dtTarget2) >= 0);
                    break;
            }
            return query;
        }
        
        /// <summary>
        /// YA[Aug 23, 2013] Calls the Sales and leads Metric values
        /// </summary>
        private void ProcessLeadAndSalesMetric()
        {
            ProcessSalesMetrics();
            ProcessLeadsMetrics();
        }
        /// <summary>
        /// YA[Aug 23, 2013] Displays results based on custom date
        /// </summary>
        private void GetCustomDateResults()
        {
            CloseDateWindow();
            ProcessLeadAndSalesMetric();
        }

        #endregion
    }
    
}