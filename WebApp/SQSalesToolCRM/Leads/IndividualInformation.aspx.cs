using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess.Models;

public partial class Leads_IndividualInformation : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

        txtFaxNo.Attributes.Add("onblur","javascript:ChcekFaxnoEmpty_DisableCellPhDial();");

    }
}