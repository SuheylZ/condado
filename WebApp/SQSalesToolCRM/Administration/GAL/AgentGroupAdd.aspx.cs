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
using CONFIG = System.Configuration.ConfigurationManager;

namespace SQS_Dialer
{
	public partial class AgentGroupAdd : SalesBasePage
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			AgentGroupDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
			//MenuSelectionChange();
		}

        protected void FormView1_ItemCommand(object sender, System.Web.UI.WebControls.FormViewCommandEventArgs e)
		{
			if (e.CommandName == "Cancel") {
				Response.Redirect("AgentGroupModify.aspx");
			}
		}

        protected void FormView1_ItemInserted(object sender, System.Web.UI.WebControls.FormViewInsertedEventArgs e)
		{
			Response.Redirect("AgentGroupModify.aspx");
		}

        protected void FormView1_ItemInserting(object sender, System.Web.UI.WebControls.FormViewInsertEventArgs e)
		{
            AgentGroupDataSource.InsertParameters["agent_group_add_date"].DefaultValue = DateTime.Now.ToString();
            AgentGroupDataSource.InsertParameters["agent_group_acd_flag"].DefaultValue = Master.GetGALType().ToString();
		}

        //protected void MenuSelectionChange()
        //{
        //    //Menu MasterMenu = null;
        //    //MasterMenu = (Menu)Master.FindControl("Menu1");
        //    //MasterMenu.Items[0].ChildItems[1].ChildItems[0].Selected = true;
        //    Master.GALMenu.Items[0].Items[1].Items[0].Selected = true;
        //}

	}
}
