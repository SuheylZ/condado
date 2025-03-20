using System;
using System.Linq;


public partial class RadWindowControl : System.Web.UI.UserControl
{
    protected void Page_Load(object sender, EventArgs e)
    {
        dlgTimerAlert.VisibleOnPageLoad = true;
        
    }
    protected void btnCloseTimerAlert_Click(object sender, EventArgs e)
    {
        dlgTimerAlert.VisibleOnPageLoad = false;
    }

    public void SetLabel(string alertName, string alertMessage, string alertType, string CampaignName= "") 
    {
        dlgTimerAlert.Title = alertType;
        lblTimerAlertCampaignName.Text = CampaignName;                
        lblInnerTimerAlertAlertName.Text = alertName;
        lblInnerTimerAlertMessage.Text = alertMessage;
    }
}