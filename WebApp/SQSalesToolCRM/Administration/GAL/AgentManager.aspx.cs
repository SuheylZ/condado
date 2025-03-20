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
using System.Xml;
using CONFIG = System.Configuration.ConfigurationManager;
namespace SQS_Dialer
{

    public partial class AgentManager : SalesBasePage
    {
        protected void Page_Load(object sender, System.EventArgs e)
        {
            string str = System.Globalization.CultureInfo.CurrentCulture.ToString();
            AgentManagerDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            //AgentGroupList.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            //MenuSelectionChange();
            Page.MaintainScrollPositionOnPostBack = true;
            if (IsAcdMode)
            {
                AgentManagerDataSource.UpdateParameters.Add("agent_default_agent_group_id_acd", DbType.Guid, null);
                AgentManagerDataSource.UpdateCommand =
                    "UPDATE [gal_Agents] SET [agent_inactive] = @agent_inactive,[agent_first_call]=@agent_first_call, [agent_modify_date] = @agent_modify_date, [agent_max_daily_leads] = @agent_max_daily_leads,[agent_max_daily_acd_leads] = @agent_max_daily_acd_leads,[agent_default_agent_group_id_acd]=@agent_default_agent_group_id_acd  WHERE [agent_id] = @original_agent_id";
                GridView1.Columns[3].Visible = false;
                GridView1.Columns[5].Visible = false;
            }
            else
            {
                AgentManagerDataSource.UpdateParameters.Add("agent_default_agent_group_id", DbType.Guid, null);
                AgentManagerDataSource.UpdateCommand =
                    "UPDATE [gal_Agents] SET [agent_inactive] = @agent_inactive,[agent_first_call]=@agent_first_call, [agent_modify_date] = @agent_modify_date, [agent_max_daily_leads] = @agent_max_daily_leads,[agent_max_daily_acd_leads] = @agent_max_daily_acd_leads,[agent_default_agent_group_id]=@agent_default_agent_group_id  WHERE [agent_id] = @original_agent_id";
                GridView1.Columns[4].Visible = false;
                GridView1.Columns[6].Visible = false;
            }
            if (!IsPostBack)
            {
                //AgentGroupList.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                BindGrid();
            }
            pager.IndexChanged += (o, a) => BindGrid();
            pager.SizeChanged += (o, a) => BindGrid();
          
        }

        void BindGrid()
        {
            AgentManagerDataSource.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();

            System.Data.DataView dv = (System.Data.DataView)AgentManagerDataSource.Select(new DataSourceSelectArguments());
            pager.RecordCount = dv.Count;
            //pager.RecordCount = AgentManagerDataSource.RecordCount();            
            GridView1.PageSize = pager.PageSize;
            GridView1.PageIndex = pager.PageNumber - 1;
            GridView1.DataBind();
        }

        public bool IsAcdMode
        {
            get { return Master.GetGALType(); }
        }

      
        protected void GridView1_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CampaignEdit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                DataKey rowkey = GridView1.DataKeys[row.RowIndex];
                string ID = rowkey[0].ToString();
                Response.Redirect("AgentCampaigns.aspx?key=" + ID.ToString());
            }
            else if (e.CommandName == "StateEdit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                DataKey rowkey = GridView1.DataKeys[row.RowIndex];
                string ID = rowkey[0].ToString();
                Response.Redirect("AgentStates.aspx?key=" + ID.ToString());
            }
            else if (e.CommandName == "Edit")
            {
                //GridView1.Columns(11).Visible = False
                //GridView1.Columns(9).Visible = False

            }
            else if (e.CommandName == "Cancel")
            {
                //GridView1.Columns(11).Visible = True
                //GridView1.Columns(9).Visible = True
            }
            else if (e.CommandName == "AgentsPVScheduleEdit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                DataKey rowkey = GridView1.DataKeys[row.RowIndex];
                string ID = rowkey[0].ToString();
                string GroupID = rowkey[1].ToString();
                if (!(string.IsNullOrEmpty(GroupID)))
                {
                    Response.Redirect("PVSchedularForAgent.aspx?key=" + ID.ToString() + "&GroupID=" + GroupID);
                }
            }
            else if (e.CommandName == "AgentsEdit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                DataKey rowkey = GridView1.DataKeys[row.RowIndex];
                string ID = rowkey[0].ToString();
                Response.Redirect("AgentGroupAssignment.aspx?agentId=" + ID.ToString());
            }
        }

        protected void GridView1_RowEditing(object sender, System.Web.UI.WebControls.GridViewEditEventArgs e)
        {
            //Normal turning to edit 
            GridView1.EditIndex = e.NewEditIndex;
            //BindGrid(); 
            //Set the focus to control on the edited row 
            GridView1.Rows[e.NewEditIndex].Controls[0].Focus();
        }

        protected void GridView1_RowUpdated(object sender, System.Web.UI.WebControls.GridViewUpdatedEventArgs e)
        {
            //GridView1.Columns(11).Visible = True
            //GridView1.Columns(9).Visible = True
        }

        protected void GridView1_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            AgentManagerDataSource.UpdateParameters["agent_modify_date"].DefaultValue = DateTime.Now.ToString();
            //AgentManagerDataSource.UpdateParameters("agent_first_call").DefaultValue = Now().ToString()
            //Add Leads360 Agent Check

        }

        protected object GetDataSource(object agentId, bool isAcd)
        {
            return LoadData(Guid.Parse(agentId.ToString()), isAcd);
        }

        internal List<AgentGroups> LoadData(Guid agentId,bool isAcd)
        {
            var _helper = new SalesTool.DataAccess.ApplicationStorageHelper("ApplicationServices");
            var parm =
          new System.Data.SqlClient.SqlParameterFluent().Add("agentId", agentId).Add("bGALType", isAcd)
              .ToObjectArray();
            List<AgentGroups> list = _helper.Context.Database.SqlQuery<AgentGroups>(Command, parm).ToList();
            return list;
        }
        internal class AgentGroups
        {
            public Guid? agent_group_id { get; set; }
            public string agent_group_name { get; set; }
            public int? o { get; set; }
        }
        private const string Command =
            @"SELECT  G.agent_group_id ,
        G.agent_group_name ,
        o = 2
FROM    [gal_AgentGroups] G
        JOIN dbo.gal_agent2agentgroups ag ON ag.agent_group_id = G.agent_group_id
                                             AND AG.agent_id = @agentId AND ISNULL(g.agent_group_acd_flag,0)=@bGALType
UNION
SELECT  NULL ,
        NULL ,
        0
ORDER BY o ,
        agent_group_name";
    }
}
