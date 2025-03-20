using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Leads_EventCalendar : AccountBasePage
{
    
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && AccountID > 0)
        {
            EventCalendarAddEdit1.Initialize();
            //EventCalendarAddEdit1.Register(EventCalendarList2 as ICalenderNotification);
        }
    }

    protected void btnCloseEventCalendar_Click(object sender, EventArgs e)
    {
        //CloseRadWindow();        
    }
    /// <summary>
    /// This function is used to set session values for AccountId & LeadId. It clear temporary...
    /// ...session variable and then closed popup window.
    /// </summary>
    //private void CloseRadWindow()
    //{
    //    String strPath = string.Format("Leads.aspx?accountid={0}&IsParentPopupClose=true", AccountID);
    //    lblCloseRadWindow.Text = "<script type='text/javascript'>SetPageAndClose(" + (char)(39) + strPath + (char)(39) + ");</script>";
    //}
}