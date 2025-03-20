using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess;

public partial class Leads_UserControls_screenerInformation : AccountsBaseControl
{
    protected override void InnerInit()
    {
    }

    protected void Page_Load(object sender, EventArgs e)
    {

    }
    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
    //interface ILeadControlSave, in the accounts base page
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        bool bAns = false;



    //        return bAns;
    //    }
    //}

    public override bool IsValidated
    {
        get
        {
            bool bAns = false;



            return bAns;
        }
    }

    protected override void InnerSave()
    {
    }
}