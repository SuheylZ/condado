using System;

using System.Linq;


public partial class Admin_ManageGetLead : SalesBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        //[QN 6/4/2013]
        //this code populate iframe in the Get A Lead area of Leads.aspx.
        getALeadFrame.Attributes.Add("src", "http://sqsst.condadogroup.com/gal/admin/default.aspx?agentid=" + CurrentUser.Key);
        //getALeadFrame.Attributes.Add("src", this.GetBaseUrl("~/gal/admin/default.aspx?agentid=" + CurrentUser.Key));
        //
    }
}