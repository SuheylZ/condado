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

    public partial class AgentGroupModify : SalesBasePage
    {


        protected void Page_Load(object sender, System.EventArgs e)
        {
            AgentGroupDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            if (IsAcdMode)
            {
                GridView1.Columns[2].Visible = false;
            }
            else
            {
                GridView1.Columns[3].Visible = false;
            }

            //MenuSelectionChange();
            Page.MaintainScrollPositionOnPostBack = true;

            if (!IsPostBack) BindGrid();
            pager.IndexChanged += (o, a) => BindGrid();
            pager.SizeChanged += (o, a) => BindGrid();

        }

        protected void GridView1_RowCommand(object sender, System.Web.UI.WebControls.GridViewCommandEventArgs e)
        {
            if (e.CommandName == "AgentsEdit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                DataKey rowkey = GridView1.DataKeys[row.RowIndex];
                string ID = rowkey[0].ToString();
                Response.Redirect("AgentGroupAgents.aspx?key=" + ID.ToString());
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
                //GridView1.Columns(7).Visible = False
                //GridView1.Columns(9).Visible = False
            }
            else if (e.CommandName == "Cancel")
            {
                //GridView1.Columns(7).Visible = True
                //GridView1.Columns(9).Visible = True
            }
            else if (e.CommandName == "AgentsPVScheduleEdit")
            {
                int index = Convert.ToInt32(e.CommandArgument);
                GridViewRow row = GridView1.Rows[index];
                DataKey rowkey = GridView1.DataKeys[row.RowIndex];
                string ID = rowkey[0].ToString();
                Response.Redirect("PVSchedular.aspx?GroupID=" + ID.ToString());
            }
        }

        protected void GridView1_RowUpdated(object sender, System.Web.UI.WebControls.GridViewUpdatedEventArgs e)
        {
            // GridView1.Columns(7).Visible = True
        }

        protected void GridView1_RowUpdating(object sender, System.Web.UI.WebControls.GridViewUpdateEventArgs e)
        {
            AgentGroupDataSource.UpdateParameters["agent_group_modify_date"].DefaultValue = DateTime.Now.ToString();
        }

        protected void btnAddAgent_Click(object sender, EventArgs e)
        {
            Response.Redirect("AgentGroupAdd.aspx");
        }

        void BindGrid()
        {
            AgentGroupDataSource.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();            
			System.Data.DataView dv = (System.Data.DataView)AgentGroupDataSource.Select(new DataSourceSelectArguments());
            pager.RecordCount = dv.Count;
            //pager.RecordCount = AgentGroupDataSource.RecordCount();
            GridView1.PageSize = pager.PageSize;
            GridView1.PageIndex = pager.PageNumber-1;
            GridView1.DataBind();
        }

        public bool IsAcdMode
        {
            get { return Master.GetGALType(); }
        }
    }
}

