using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class GAL_MasterPage : System.Web.UI.MasterPage
{

    private bool GALType
    {
        get
        {
            bool iVal = false;
            if (Session["GALType"] != null)             
            bool.TryParse(Session["GALType"].ToString(), out iVal);
            return iVal;
        }
        set
        {
            Session["GALType"] = value.ToString();
        }
    }
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            if (Session["GALType"] == null) GALType = false;
            ddlIsACD.SelectedValue = GALType? "1" : "0";            
        }
    }
    
    public bool GetGALType()
    {
        return GALType;
    }
    //YA[01 Aug, 2014]
    protected void ddlIsACD_SelectedIndexChanged(object sender, EventArgs e)
    {
        GALType = ddlIsACD.SelectedValue == "1"? true: false;
        Response.Redirect(Request.RawUrl);
    }
    //protected void rdlistIsACD_SelectedIndexChanged(object sender, EventArgs e)
    //{        
    //    GALType = rdlistIsACD.SelectedValue == "1"? true: false;
    //    Response.Redirect(Request.RawUrl);
    //}
    
}
