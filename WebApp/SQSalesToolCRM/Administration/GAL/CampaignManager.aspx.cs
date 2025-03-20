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
    public partial class CampaignManager : SalesBasePage
    {

        protected void Page_Load(object sender, System.EventArgs e)
        {
            CampaignManagerDataSource.ConnectionString =CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;
            CampaignGroupList.ConnectionString = CONFIG.ConnectionStrings["ApplicationServices"].ConnectionString;


            if (!IsPostBack)
            {
                CampaignGroupList.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();
                BindGrid();
            }
            pager.IndexChanged += (o, a) => BindGrid();
            pager.SizeChanged += (o, a) => BindGrid();
            
        }


        private void BindGrid()
        {
            CampaignManagerDataSource.SelectParameters["bGALType"].DefaultValue = Master.GetGALType().ToString();            
			System.Data.DataView dv = (System.Data.DataView)CampaignManagerDataSource.Select(new DataSourceSelectArguments());
            pager.RecordCount = dv.Count;
            //pager.RecordCount = CampaignManagerDataSource.RecordCount();
            GridView1.PageSize = pager.PageSize;
            GridView1.PageIndex = pager.PageNumber - 1;
            GridView1.DataBind();
        }

        protected string GetAgentValue(string agentValueID)
        {
            return agentValueID;
        }
        
}
}
