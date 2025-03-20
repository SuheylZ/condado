// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      C:\Projects\Live\SQ Sales Tool\SQSalesToolCRM\
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   SZ
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 
  
using System;

public partial class Error : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {
        var error= Server.GetLastError();
        if (error != null)
        {
            while (error.Message.Contains("inner exception") && error.InnerException != null)
                error = error.InnerException;

            lblError.Text = "Details: " + error.Message;
        }
    }
}
