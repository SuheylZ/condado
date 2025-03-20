using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Administration_GAL_AcdStatisticsView :SalesBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        LoadData(null);
    }
    protected override void OnInit(EventArgs e)
    {
        if (!User.Identity.IsAuthenticated || User.Identity.Name != "admin@condado.com")
        {
            Response.Redirect(Konstants.K_LOGINPAGE, true);
        }
        base.OnInit(e);
    }
    private class acd_statistics
    {
        public Guid qas_agent_id { get; set; }
        public int? qas_acd_count { get; set; }
        public int? qas_max_acd { get; set; }
        public int? qas_max_quota { get; set; }
        public int? qas_min_level { get; set; }
        public int? qas_next_refresh { get; set; }
        public int? qas_acd_call_taken { get; set; }
        public int? qas_PV_schedule_result { get; set; }
        public bool? qas_is_enabled { get; set; }
        public string qas_reason { get; set; }
    }


    public void LoadData(Guid? agentId)
    {
        var _helper = new SalesTool.DataAccess.ApplicationStorageHelper("ApplicationServices");
        List<acd_statistics> list = new List<acd_statistics>();
        if (!agentId.HasValue)
        {

            list = _helper.Context.Database.SqlQuery<acd_statistics>(command).ToList();
        }
        else
        {
            var parm = new System.Data.SqlClient.SqlParameterFluent().Add("agentId", agentId).ToObjectArray();
            list = _helper.Context.Database.SqlQuery<acd_statistics>(command + " WHERE qas_agent_id=@agentID", parm).ToList();
        }
        this.statGrid.DataSource = list;
        this.statGrid.DataBind();
    }
    protected void filterResult_Click(object sender, EventArgs e)
    {
        if (agentDropdownlist.SelectedValue != null)
        {
            Guid agentId;
            if (Guid.TryParse(agentDropdownlist.SelectedValue, out agentId))
            {
                LoadData(agentId);
            }
            else
            {
                LoadData(null);
            }
        }
    }
    private const string command = @"EXEC dbo.GetACDStatistics; SELECT  qas_agent_id ,
        qas_acd_count ,
        qas_max_acd ,
        qas_max_quota ,
        qas_min_level ,
        qas_next_refresh ,
        qas_acd_call_taken ,
        qas_PV_schedule_result ,
        qas_is_enabled,
        qas_reason
        FROM dbo.queue_acd_statistics";

    private const string AgentComand = @"SELECT u.usr_key,u.usr_first_name+' ' +u.usr_last_name AS Name FROM dbo.users u
		JOIN dbo.gal_agents g
		ON u.usr_key=g.agent_id
		ORDER BY usr_first_name,usr_last_name";
}