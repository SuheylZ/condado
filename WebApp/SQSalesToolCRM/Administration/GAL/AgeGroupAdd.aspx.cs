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
	public partial class AgeGroupAdd : SalesBasePage 
	{

		protected void Page_Load(object sender, System.EventArgs e)
		{
			AgeGroupDataSource.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            //MenuSelectionChange();
		}



        protected void FormView1_ItemInserted(object sender, System.Web.UI.WebControls.FormViewInsertedEventArgs e)
		{
			Response.Redirect("AgeGroupModify.aspx");
		}

        protected void FormView1_ItemInserting(object sender, System.Web.UI.WebControls.FormViewInsertEventArgs e)
		{
			AgeGroupDataSource.InsertParameters["Age_group_add_date"].DefaultValue = DateTime.Now.ToString();
            AgeGroupDataSource.InsertParameters["age_group_acd_flag"].DefaultValue = Master.GetGALType().ToString();
		}

        //protected void MenuSelectionChange()
        //{
        //    //Menu MasterMenu = null;
        //    //MasterMenu = (Menu)Master.FindControl("Menu1");
        //    //MasterMenu.Items[2].Items[0].Items[0].Selected = true;
        //    Master.GALMenu.Items[2].Items[0].Items[0].Selected = true;
        //}

		protected void priority_build(object sender, EventArgs e)
		{
			for (int x = 1; x <= 99; x++) {
                (sender as DropDownList).Items.Add(x.ToString());
			}
		}

        protected void FormView1_ItemCommand(object sender, FormViewCommandEventArgs e)
        {
            //protected void FormView1_ItemCommand(object sender, System.Web.UI.WebControls.FormViewCommandEventArgs e)
		
			if (e.CommandName == "Cancel") {
				Response.Redirect("AgeGroupModify.aspx");
			}
		
        }

	}
}
