using System;
using System.Text;
using System.Linq;


public partial class UserControls_StatusLabel : SalesUserControl
{
    StringBuilder sb = new StringBuilder();

    protected override void InnerInit()
    {
        lblMessage.Text = "";
    }

    public void Clear()
    {
        lblMessage.Text = "";
    }

    public void SetStatus(string message)
    {
        if (message == string.Empty)
            lblMessage.Visible = false;
        else
        {
            lblMessage.Visible = true;
            lblMessage.Text = message + "<br/>";
            lblMessage.CssClass = "information";
        }
    }
    public void SetStatus(Exception ex)
    {
        while(ex.Message.Contains("inner exception for details"))
            ex = ex.InnerException;

        lblMessage.Visible = true;
        lblMessage.Text = ex.Message + "<br/>";
        lblMessage.CssClass = "exception";
    }

    public String ErrorMessage
    {
        get {return lblMessage.Text; }
    }

  


    public override string ClientID
    {
        get
        {
            return lblMessage.ClientID;
        }
    }
   
}