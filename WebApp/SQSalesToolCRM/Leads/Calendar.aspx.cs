using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using Telerik.Web.UI;

public partial class Leads_Calendar : AccountBasePage
{   /// <summary>
    /// Developed By : Imran H
    /// Description: Displays event secheduler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_Initialize(object sender, EventArgs args)
    {
        uc_calandar1.BtnHandler += GetAccoutnId;

    }
    void GetAccoutnId(string selectedAccoutnId)
    {
        if (selectedAccoutnId != string.Empty)
            AccountID = long.Parse(selectedAccoutnId);
    }

}