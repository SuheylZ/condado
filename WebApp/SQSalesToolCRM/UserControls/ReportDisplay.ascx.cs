using System;
using System.Linq;

using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using Telerik.Reporting;

namespace SalesTool.Web.UserControls
{
    public partial class ReportDisplay : HomeUserControl
    {
        //SZ [Jul 17, 2013] These constants are not public as they are only useful inside this class.
        const string K_AllValues = "-2";

        protected override void InnerInit(bool bFirstTime)
        {
            if (bFirstTime)
                InitializeUI();
            BindEvents();
        }
        protected override void InnerLoad(bool bFirstTime){}

        void InitializeUI()
        {
            // Aug 23, 2013. Im not putting the dates on every comment, Runnin outa time too much work!
            //SZ: Initialize the reporting dropdown accordingly. the defaults are loaded later.
            ddlReportType.DataSource = (Engine.ApplicationSettings.InsuranceType == 1) ?
                Engine.ReportActions.All.Where(x => !x.IsDefault && x.IsSenior == false).OrderBy(x => x.Order) :
                Engine.ReportActions.All.Where(x => !x.IsDefault).OrderBy(x => x.Order);
            ddlReportType.DataBind();
            var list = Engine.ReportActions.All.Where(x => x.IsDefault).OrderBy(x => x.Order).ToArray();
            for (int i = 0; i < list.Length; i++)
                ddlReportType.Items.Insert(i, new Telerik.Web.UI.DropDownListItem(string.Format("{0} - Default", list[i].Title), list[i].Id.ToString()));

            InitializeFilters();
            ChangeFilters(0);
            DisplayReport(0);
        }
        void BindEvents()
        {
            ddlReportType.SelectedIndexChanged += (o, a) => { int i = Convert.ToInt32(a.Value); ChangeFilters(i);
            if (i == 6 || i == 9) DisplayReport(i, true);
            else DisplayReport(i, false); };
            btnApply.Click += (o, a) => DisplayReport(Convert.ToInt32(ddlReportType.SelectedValue), true);
            btnClear.Click += (o, a) => { ClearFilter(); DisplayReport(Convert.ToInt32(ddlReportType.SelectedValue), false); };

            tlkClose.Click += (o, a) => CloseDateWindow();
            ddlTime.ItemSelected += (o, a) => { if (Convert.ToInt32(a.Value) == 9) OpenDateWindow(); };
        }
  
        void GetFilters(ref SalesTool.Reports.ReportManager manager, int report)
        {
            switch (report)
            {
                case 1://  1	Scorecard 
                    //ddlAgents.Visible = true;
                    //ddlAgentGroups.Visible = true;
                    //ddlCampaign.Visible = true;
                    break;
                case 2://  2	Stack Rankings
                    //ddlAgents.Visible = true;
                    //ddlAgentGroups.Visible = true;
                    //ddlTime.Visible = true;
                    //ddlCampaign.Visible = true;
                    break;
                case 3://  3	CPA Report
                    //ddlTime.Visible = true;
                    break;
                case 4://  4	Pipeline Report
                    ddlAgents.Visible = true;
                    break;
                case 5://  5	Incentive Tracking Report
                    break;
                case 6://  6	Quota Tracking Report
                    int month = (tlkMonthYr.SelectedDate.HasValue) ? tlkMonthYr.SelectedDate.Value.Month : DateTime.Now.Month;
                    int year = (tlkMonthYr.SelectedDate.HasValue) ? tlkMonthYr.SelectedDate.Value.Year : DateTime.Now.Year;
                    int daysInMonthMinusWeekends = 0, daysInMonthWorked = 0;

                    CalculateDaysWorked(month, year, ref daysInMonthMinusWeekends, ref daysInMonthWorked);

                    manager.Add("month", month);
                    manager.Add("year", year);
                    manager.Add("totaldaysworked", daysInMonthMinusWeekends);
                    manager.Add("daysworked", daysInMonthWorked);

                    break;
                case 7://  7	Commision Dashboard
                    //ddlGoalType.Visible = true;
                    break;
                case 8://  8	Lead Volume Report
                    if (ddlCampaign.SelectedValue != K_AllValues)
                        manager.Add("campaignid", Helper.SafeConvert<int>(ddlCampaign.SelectedValue));
                    if(ddlAgentGroups.SelectedValue!= K_AllValues)
                        manager.Add("skillgroupid", Helper.SafeConvert<int>(ddlAgentGroups.SelectedValue));
                    manager.Add("startdate", GetStartDate());
                    manager.Add("enddate", GetEndDate());
                    break;
                case 9://  9	Goal Report
                    int monthGR = (tlkMonthYr.SelectedDate.HasValue) ? tlkMonthYr.SelectedDate.Value.Month : DateTime.Now.Month;
                    int yearGR = (tlkMonthYr.SelectedDate.HasValue) ? tlkMonthYr.SelectedDate.Value.Year : DateTime.Now.Year;
                    int daysInMonthMinusWeekendsGR = 0, daysInMonthWorkedGR = 0;

                    CalculateDaysWorked(monthGR, yearGR, ref daysInMonthMinusWeekendsGR, ref daysInMonthWorkedGR);

                    manager.Add("month", monthGR);
                    manager.Add("year", yearGR);
                    manager.Add("totaldaysworked", daysInMonthMinusWeekendsGR);
                    manager.Add("daysworked", daysInMonthWorkedGR);
                    break;
                case 10://  10	Case Specialist Dashboard
                    if (ddlAgentGroups.SelectedValue != K_AllValues)
                        manager.Add("skillgroup", Helper.SafeConvert<int>(ddlAgentGroups.SelectedValue));
                    manager.Add("startdate", GetStartDate());
                    manager.Add("enddate", GetEndDate());
                    break;
                case 11://  11	Submission/Enrolled Report
                    manager.Add("year", ddlYear.SelectedValue=="1"? DateTime.Now.Year: DateTime.Now.AddYears(-1).Year);
                    break;
                case 13://  13	Premium Report
                    //ddlAgents.Visible = true;
                    //ddlAgentGroups.Visible = true;
                    //ddlTime.Visible = true;
                    //ddlCampaign.Visible = true;
                    break;
                case 14://  14	Carrier Report
                    if (ddlCampaign.SelectedValue != K_AllValues)
                        manager.Add("campaign", Helper.SafeConvert<int>(ddlCampaign.SelectedValue));
                    if(ddlAgentGroups.SelectedValue!= K_AllValues)
                        manager.Add("skillgroup", Helper.SafeConvert<int>(ddlAgentGroups.SelectedValue));
                    manager.Add("startdate", GetStartDate());
                    manager.Add("enddate", GetEndDate());
                    manager.Add("type", Engine.ApplicationSettings.InsuranceType);
                    // manager.Add("month", DateTime.Now.Month);
                    //manager.Add("year", DateTime.Now.Year);
                    break;
                case 15://  15	Fill Form Speed
                    if (ddlCampaign.SelectedValue != K_AllValues)
                        manager.Add("campaign", Helper.SafeConvert<int>(ddlCampaign.SelectedValue));
                    if(ddlAgentGroups.SelectedValue!= K_AllValues)
                        manager.Add("skillgroup", Helper.SafeConvert<int>(ddlAgentGroups.SelectedValue));
                    manager.Add("startdate", GetStartDate());
                    manager.Add("enddate", GetEndDate());
                    break;
                case 16://  16	Falloff Report
                   if (ddlCampaign.SelectedValue != K_AllValues)
                        manager.Add("campaign", Helper.SafeConvert<int>(ddlCampaign.SelectedValue));
                    if(ddlAgentGroups.SelectedValue!= K_AllValues)
                        manager.Add("skillgroup", Helper.SafeConvert<int>(ddlAgentGroups.SelectedValue));
                    manager.Add("date", GetStartDate());
                    //manager.Add("enddate", GetEndDate());
                    break;
                case 17://  17	Prioritized List Report
                    //ddlAgents.Visible = true;
                    break;
            }
        }

        private static void CalculateDaysWorked(int month, int year, ref int daysInMonthMinusWeekends, ref int daysInMonthWorked)
        {
            DateTime nTodayDateTime = new DateTime(DateTime.Now.Ticks);
            int paramYear = year;
            int paramMonth = month;


            for (int i = 1; i <= DateTime.DaysInMonth(paramYear, paramMonth); i++)
            {
                DateTime thisDay = new DateTime(paramYear, paramMonth, i);
                if (thisDay.DayOfWeek != DayOfWeek.Saturday && thisDay.DayOfWeek != DayOfWeek.Sunday)
                {
                    daysInMonthMinusWeekends += 1;
                }
            }

            if (nTodayDateTime.Month == paramMonth && nTodayDateTime.Year == paramYear)
            {
                DateTime nValue = new DateTime(paramYear, paramMonth, nTodayDateTime.Day);

                for (int i = 1; i <= DateTime.DaysInMonth(paramYear, paramMonth); i++)
                {
                    DateTime thisDay = new DateTime(paramYear, paramMonth, i);
                    if (thisDay.Day > nValue.Day) break;
                    if (thisDay.DayOfWeek != DayOfWeek.Saturday && thisDay.DayOfWeek != DayOfWeek.Sunday)
                    {
                        daysInMonthWorked += 1;
                    }
                }
            }
            else
            {
                daysInMonthWorked = daysInMonthMinusWeekends;
            }
        }
        void InitializeFilters()
        {
            //SZ: Initialize the agents. 
            ddlAgents.DataSource = Engine.UserActions.GetAll().OrderBy(x => x.FirstName);
            ddlAgents.DataBind();
            ddlAgents.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("All", K_AllValues));

            ddlCampaign.DataSource = Engine.ManageCampaignActions.GetAll();
            ddlCampaign.DataBind();
            if (ddlCampaign.Items.Count > 0)
                ddlCampaign.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("All", K_AllValues));

            ddlAgentGroups.DataSource = Engine.SkillGroupActions.All;
            ddlAgentGroups.DataBind();
            if (ddlAgentGroups.Items.Count > 0)
                ddlAgentGroups.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("All", K_AllValues));

            ddlAgents.SelectedIndex = 0;
            ddlAgentGroups.SelectedIndex = 0;
            ddlCampaign.SelectedIndex = 0;
            ddlTime.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
            ddlGoalType.SelectedIndex = 0;
            tlkMonthYr.SelectedDate = DateTime.Now;
        }
        void ChangeFilters(int iReport)
        {
            bool bShowButtons = true;

            ddlAgents.Visible = false;
            ddlAgentGroups.Visible = false;
            ddlCampaign.Visible = false;
            ddlTime.Visible = false;
            ddlYear.Visible = false;
            ddlGoalType.Visible = false;
            tlkMonthYr.Visible = false;
            btnApply.Visible = false;
            btnClear.Visible = false;
            

            lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = lblDate.Visible = lblYear.Visible = lblType.Visible = lblMonthYr.Visible = false;

            ClearFilter();

            switch (iReport)
            {
                case 1://  1	Scorecard 
                    ddlAgents.Visible = true;
                    ddlAgentGroups.Visible = true;
                    ddlCampaign.Visible = true;
                    lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = true;
                    break;
                case 2://  2	Stack Rankings
                    ddlAgents.Visible = true;
                    ddlAgentGroups.Visible = true;
                    ddlTime.Visible = true;
                    ddlCampaign.Visible = true;
                    lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = lblDate.Visible = true;
                    break;
                case 3://  3	CPA Report
                    ddlTime.Visible = true;
                    lblDate.Visible = true;
                    break;
                case 4://  4	Pipeline Report
                    ddlAgents.Visible = true;
                    lblAgents.Visible = true;
                    break;
                case 5://  5	Incentive Tracking Report
                    bShowButtons = false;
                    break;
                case 6://  6	Quota Tracking Report
                    lblMonthYr.Visible = true;
                    tlkMonthYr.Visible = true;
                    break;
                case 7://  7	Commision Dashboard
                    ddlGoalType.Visible = true;
                    lblType.Visible = true;
                    break;
                case 8://  8	Lead Volume Report
                    ddlAgents.Visible = true;
                    ddlAgentGroups.Visible = true;
                    ddlTime.Visible = true;
                    ddlCampaign.Visible = true;
                    lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = lblDate.Visible =true;
                    break;
                case 9://  9	Goal Report
                    //ddlGoalType.Visible = true;
                    //lblType.Visible = true;
                    lblMonthYr.Visible = true;
                    tlkMonthYr.Visible = true;
                    break;
                case 10://  10	Case Specialist Dashboard
                    ddlAgentGroups.Visible = true;
                    ddlTime.Visible = true;
                    lblAgentType.Visible = lblDate.Visible = true;
                    break;
                case 11://  11	Submission/Enrolled Report
                    ddlYear.Visible = true;
                    lblYear.Visible = true;
                    break;
                case 13://  13	Premium Report
                    ddlAgents.Visible = true;
                    ddlAgentGroups.Visible = true;
                    ddlTime.Visible = true;
                    ddlCampaign.Visible = true;
                    lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = lblDate.Visible = true;
                    break;
                case 14://  14	Carrier Report
                    ddlAgents.Visible = true;
                    ddlAgentGroups.Visible = true;
                    ddlTime.Visible = true;
                    ddlCampaign.Visible = true;
                    lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = lblDate.Visible = true;
                    break;
                case 15://  15	Fill Form Speed
                    ddlAgents.Visible = true;
                    ddlAgentGroups.Visible = true;
                    ddlTime.Visible = true;
                    ddlCampaign.Visible = true;
                    lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = lblDate.Visible = true;
                    break;
                case 16://  16	Falloff Report
                    ddlAgents.Visible = true;
                    ddlAgentGroups.Visible = true;
                    ddlTime.Visible = true;
                    ddlCampaign.Visible = true;
                    lblAgents.Visible = lblAgentType.Visible = lblCampaign.Visible = lblDate.Visible = true;
                    break;
                case 17://  17	Prioritized List Report
                    ddlAgents.Visible = true;
                    lblAgents.Visible = true;
                    break;
                default:
                    bShowButtons = false;
                    break;
            }
            if (bShowButtons)
                btnApply.Visible = btnClear.Visible = true;
        }
        void ClearFilter()
        {
            ddlAgents.SelectedIndex = 0;
            ddlAgentGroups.SelectedIndex = 0;
            ddlCampaign.SelectedIndex = 0;
            ddlTime.SelectedIndex = 0;
            ddlYear.SelectedIndex = 0;
            ddlGoalType.SelectedIndex = 0;
            tlkMonthYr.SelectedDate = DateTime.Now;
        }
        DateTime GetStartDate()
        {
            DateTime Ans=DateTime.Now;
            switch (Helper.SafeConvert<int>(ddlTime.SelectedValue))
            {
                case -2: Ans = new DateTime(1990, 1, 1); break;
                case 1: Ans = Convert.ToDateTime(DateTime.Today.ToShortDateString()); break;//Today" 
                case 2: Ans = Convert.ToDateTime(DateTime.Today.AddDays(-1).ToShortDateString()); break;//Yesterday"
                case 3: Ans = Convert.ToDateTime(DateTime.Today.AddDays(-1).ToShortDateString()); break;//Week to Date
                case 4: Ans = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1); break;//Month to Date
                case 5: Ans = Convert.ToDateTime(DateTime.Now.AddDays(-7).ToShortDateString());break;//Last 7 Days
                case 6: Ans = Convert.ToDateTime(DateTime.Now.AddDays(-14).ToShortDateString()); break;//Last 14 Days
                case 7: Ans = Convert.ToDateTime(DateTime.Now.AddDays(-30).ToShortDateString()); break; //Last 30 Days 
                case 8: Ans = new DateTime(DateTime.Now.Year, 1, 1); break;
                case 9: Ans = tlkStartDate.SelectedDate.HasValue ? tlkStartDate.SelectedDate.Value : DateTime.Now; break;//Custom Date
                default:
                    Ans = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
            }
            return Ans;
        }
        DateTime GetEndDate()
        {
            DateTime Ans = DateTime.Now;
            switch (Helper.SafeConvert<int>(ddlTime.SelectedValue))
            {
                case -2: Ans = DateTime.Now; break;
                case 1: Ans = DateTime.Now; break;//Today" 
                case 2: Ans = DateTime.Now; break;//Yesterday"
                case 3: Ans = DateTime.Now; break;//Week to Date
                case 4: Ans = DateTime.Now; break;//Month to Date
                case 5: Ans = DateTime.Now; break;//Last 7 Days
                case 6: Ans = DateTime.Now; break;//Last 14 Days
                case 7: Ans = DateTime.Now; break; //Last 30 Days 
                case 8: Ans = new DateTime(DateTime.Now.Year, 12, 31); break;
                case 9: Ans = tlkEndDate.SelectedDate.HasValue? tlkEndDate.SelectedDate.Value: DateTime.Now; break;//Custom Date
                default:
                    Ans = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                    break;
            }
            return Ans;
        }
        void DisplayReport(int value, bool bUserFilter= false, string selectedAgent = "" )
        {
            Guid agentID = Guid.Empty;
            if (ddlAgents.SelectedValue != K_AllValues && ddlAgents.Visible)
            {
                Guid.TryParse(ddlAgents.SelectedValue, out agentID);
            }

            SalesTool.Reports.ReportManager manager = new Reports.ReportManager(Engine.ApplicationSettings.InsuranceType, agentID);
            var reportSource = new Telerik.Reporting.InstanceReportSource();

            if(bUserFilter)
                GetFilters(ref manager, value);
            
            reportSource.ReportDocument = manager.GetReport(value);
            tlkViewer.ReportSource = reportSource;
            tlkViewer.RefreshReport();
        }

        void OpenDateWindow()
        {
            tlkStartDate.SelectedDate = null;
            tlkEndDate.SelectedDate = null;
            tlkDateWindow.Visible = true;
            tlkDateWindow.VisibleOnPageLoad = true;
        }
        void CloseDateWindow()
        {
            // string script = "<script language='javascript' type='text/javascript'>Sys.Application.add_load(CloseWindow);</script>";
            //(Application[Konstants.K_SCRIPT_MANAGER] as Telerik.Web.UI.RadScriptManager).ClientSc
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "CloseWindow", "CloseWindow();");
            tlkDateWindow.Visible = false;
        }
    };
}