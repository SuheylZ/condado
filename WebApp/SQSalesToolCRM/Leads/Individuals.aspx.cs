using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Leads_Individuals : AccountBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        dlgConsent.AlertClosed += (o, a) => DefaultAlertBoxHandler(a.Value);
        IndividualsAddEdit1.OnClose += (o, a) => Close();
    }
  
    void Close()
    {
        string x = "CloseRadWindow();";
        Page.ClientScript.RegisterClientScriptBlock(x.GetType(), "1234567890", x);
    }

    public override void ShowAlertBox()
    {
        dlgConsent.Show();
    }
}