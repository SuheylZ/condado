using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess.Models;
using System.Linq;
using Telerik.Web.UI;

public partial class Leads_CampaignDescription : SalesDataPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        ////[QN, 01/5/2013]
        //// It display the Campaign description on the popup, it is accessed from leads "More" button... 
        //// ...click and display the description of Campaign selected in Source dropdown on leads page.
        
        
        //SZ [Jun 12, 2013] deprecated in favour of simpler implementation 
        //if (Request["campIden"] != null)
        //{
        //    Campaign nCampaign = Engine.ManageCampaignActions.Get(Convert.ToInt32(Request["campIden"]));

        //    if (Convert.ToString(nCampaign.Description) != "" && Convert.ToString(nCampaign.Description) != null)
        //        lblCampaignDescription.Text = Convert.ToString(nCampaign.Description);
        //}
        string description = Engine.ManageCampaignActions.DescriptionOf(Helper.SafeConvert<int>(Request["campIden"]));
        lblCampaignDescription.Text = description.Length>0? description:"No description found";
    }
}
