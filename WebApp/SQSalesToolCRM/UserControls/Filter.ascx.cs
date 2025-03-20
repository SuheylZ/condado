using System;
using System.Linq;
using System.Web.UI.WebControls;

namespace SalesTool.Web.UserControls
{
    

    /// <summary>
    /// This control displays all the reports on the left/right pane of the report
    /// </summary>
    public partial class FilterControl : HomeUserControl, 
        IStatisticsMetricsHandler
    {
        //SZ [Jul 17, 2013] These constants are nolt made public as they are only useful inside this class.
        // hence encapsulated in this class
        const string K_NoValue = "-1";
        const string K_AllValues = "-2";
        const string K_MetricsStore = "__Statictis_Metrics_Store__";


        HiddenField _hdnMetricsType = null;

        public void MetricsChanged(StatisticsMetricsType current)
        {
            ddlLeadType.Visible = current != StatisticsMetricsType.Sales;
        }

        protected override void InnerInit(bool bFirstTime)
        {
            _hdnMetricsType = new HiddenField
            {
                ID = K_MetricsStore,
                Value = StatisticsMetricsType.Sales.ToString(),
                ClientIDMode = System.Web.UI.ClientIDMode.Predictable
            };
            Controls.Add(_hdnMetricsType);

            BindEvent();

            if (bFirstTime)
            {
                InitializeUI();
            }
        }
        protected override void InnerLoad(bool bFirstTime)
        {

        }
        
        
        void BindEvent()
        {
            tlkClose.Click += (o, a) => CloseDateWindow();
            ddlTime.ItemSelected += (o, a) => FilterByTime(Convert.ToInt32(a.Value));
        }
        void FilterByTime(int value)
        {
            if (value == 8)
            {
                OpenDateWindow();
            }
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
        void InitializeUI()
        {
            ddlCampaign.DataSource = Engine.ManageCampaignActions.GetAll();
            ddlCampaign.DataBind();
            if (ddlCampaign.Items.Count > 0)
                ddlCampaign.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("All", K_AllValues));
            ddlCampaign.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("Campaign", K_NoValue));
            //ddlCampaign.SelectedValue = K_NoValue;

            ddlAgentGroups.DataSource = Engine.SkillGroupActions.All;
            ddlAgentGroups.DataBind();
            if (ddlAgentGroups.Items.Count > 0)
                ddlAgentGroups.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("All", K_AllValues));
            ddlAgentGroups.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("Agent Type", K_NoValue));
            ddlAgentGroups.SelectedValue = K_NoValue;

            //ddlLeadType.DataSource = Engine.SkillGroupActions.All;
            //ddlLeadType.DataBind();
            if (ddlLeadType.Items.Count > 0)
                ddlLeadType.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("All", K_AllValues));
            ddlLeadType.Items.Insert(0, new Telerik.Web.UI.DropDownListItem("Lead Type", K_NoValue));
            ddlLeadType.SelectedValue = K_NoValue;

        }
    };
}