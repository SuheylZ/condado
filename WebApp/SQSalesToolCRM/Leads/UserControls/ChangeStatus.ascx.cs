using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Leads_UserControls_ChangeStatus : AccountsBaseControl
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    public override bool IsValidated
    {
        get
        {
            //vldtxtCarrierIssues.Validate();
            //return vldtxtCarrierIssues.IsValid;

            return true;
        }
    }

    protected override void InnerInit()
    {
        if (!Page.IsPostBack)
        {
        }
    }

    public string SubStatusClientID
    {
        get
        {
            return this.ddlSubStatus.ClientID;
        }
    }

    public string StatusClientID
    {
        get
        {
            return this.ddlStatus.ClientID;
        }
    }

    public string IncludeSubStatusCheckBoxClientID
    {
        get
        {
            return this.chkInclueSubStatus.ClientID;
        }
    }
    public void DataBindStatusCombo(IQueryable<object> source, string textFieldName, string valueFieldName)
    {
        if (!IsPostBack)
        {
            ddlStatus.AppendDataBoundItems = true;
            ddlStatus.Items.Add(new Telerik.Web.UI.RadComboBoxItem() { Text = "", Value = "" });
            ddlStatus.DataTextField = textFieldName;
            ddlStatus.DataValueField = valueFieldName;
            ddlStatus.DataSource = source.ToList();
            ddlStatus.DataBind();
            ddlStatus.SelectedIndex = 0;
        }
    }
    //SR
    //public void DataBindSubStatusCombo(Int64 statusId)
    //{
    //    if (!IsPostBack)
    //    {
    //        ddlSubStatus.AppendDataBoundItems = true;
    //        ddlSubStatus.Items.Add(new Telerik.Web.UI.RadComboBoxItem() { Text = "", Value = "" });
    //        ddlSubStatus.DataTextField = textFieldName;
    //        ddlSubStatus.DataValueField = valueFieldName;
    //        ddlSubStatus.DataSource = source;
    //        ddlSubStatus.DataBind();
    //        ddlSubStatus.SelectedIndex = 0;
    //    }
    //}

    protected void ddlStatus_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        chkInclueSubStatus.Checked = false;
        ddlSubStatus.Items.Clear();
        ddlSubStatus.AppendDataBoundItems = true;
        int statusId = Convert.ToInt32(e.Value);
        ddlSubStatus.DataTextField = "Title";
        ddlSubStatus.DataValueField = "Id";
        ddlSubStatus.Items.Add(new Telerik.Web.UI.RadComboBoxItem() { Text = "", Value = "" });
        ddlSubStatus.DataSource = Engine.StatusActions.GetSubStatuses(statusId, false);
        ddlSubStatus.DataBind();
        ddlSubStatus.SelectedIndex = 0;
    }
}