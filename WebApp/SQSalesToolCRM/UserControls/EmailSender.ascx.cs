using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net.Mail;
using System.Text.RegularExpressions;
using SalesTool.Schema;
using System.Data;
using SalesTool.DataAccess.Models;
using System.Linq.Dynamic;

public partial class UserControls_EmailSender : AccountsBaseControl
{

    #region  Members/Properties
    
    string _query = String.Empty, _emlFrom = string.Empty;
    SmtpClient _smtp = null;
    /// <summary>
    /// General Mode for Send Email button on leads page.
    /// </summary>
    public Boolean IsGeneralMode
    {
        get
        {
            bool nValue = false;
            bool.TryParse(hdnFieldIsGeneralMode.Value, out nValue);
            return nValue;
        }
        set
        {
            listEmailTemplates.Visible = value;
            if (value && HasCustomTemplate == false) BindAllEmailTemplates();
            hdnFieldIsGeneralMode.Value = value.ToString();
        }
    }
    /// <summary>
    /// Is the template editable
    /// </summary>
    public Boolean HasCustomTemplate
    {
        get
        {
            bool nValue = false;
            bool.TryParse(hdnFieldHasCustomTemplate.Value, out nValue);
            return nValue;
        }
        set
        {
            hdnFieldHasCustomTemplate.Value = value.ToString();
        }
    }
    /// <summary>
    /// Email Template key
    /// </summary>
    public int EmailTemplateKey
    {
        get
        {
            int nValue = 0;
            int.TryParse(hdnFieldEmailTemplateKey.Value, out nValue);
            return nValue;
        }
        set
        {
            hdnFieldEmailTemplateKey.Value = value.ToString();
        }
    }

    #endregion

    #region  Methods
    /// <summary>
    /// Override the InnerInit() to initialize the the variables
    /// </summary>
    protected override void InnerInit()
    {

    }
    /// <summary>
    ///  Override the InnerLoad() function making judicious use of FirstTime boolean value. It is true only first time when the page is created. for instance grid binding shoudl happen only when it is true. Avoid binding data to controls that are not visible.
    /// </summary>
    /// <param name="bFirstTime"></param>
    protected override void InnerLoad(bool bFirstTime)
    {
        if (bFirstTime)
        {
            Initialize();
        }
    }

    /// <summary>
    /// Calls on every postback
    /// </summary>
    protected override void InnerPostBack()
    {
        lblMessageForm.SetStatus("");
    }

    /// <summary>
    /// Checking for validation of the form values
    /// </summary>
    public override bool IsValidated
    {
        get
        {
            return true;
        }
    }

    /// <summary>
    /// Initialization of values
    /// </summary>
    public void Initialize()
    {
        EmailTemplateKey = 0;
        ctlOverrideEmailAttachments.EmailTemplateKey = 0;
        ctlOverrideEmailAttachments.BindTemplateAttachmentGrid();
        txtBCCEmail.Text = "";
        rdlFormatHtml.Checked = true;
        rdlFormatText.Checked = false;
        txtEmailBody.Content = "";

        txtEmailBody.EnableTextareaMode = false;
        txtEmailBody.EditModes = Telerik.Web.UI.EditModes.All;

        txtCCEmail.Text = "";
        txtFromEmail.Text = "";
        txtSubject.Text = "";
        txtToEmail.Text = "";
        //if (ddlEmailTemplates.Items.Count > 0)
        //    ddlEmailTemplates.SelectedIndex = 0;
    }

    /// <summary>
    /// Bind All Email templates to drop down list
    /// </summary>
    public void BindAllEmailTemplates()
    {
        if (ddlEmailTemplates.Items.Count > 0)
            ddlEmailTemplates.Items.Clear();
        ddlEmailTemplates.AppendDataBoundItems = true;
        ddlEmailTemplates.DataTextField = "Title";
        ddlEmailTemplates.DataValueField = "Id";
        ddlEmailTemplates.Items.Add(new ListItem("--Select Email Template--", "-1"));
        ddlEmailTemplates.DataSource = Engine.ManageEmailTemplateActions.GetAllIQueryable().Where(x => x.Enabled ?? false).OrderBy(Engine.ApplicationSettings.EmailOrderClause);
        ddlEmailTemplates.DataBind();
        ddlEmailTemplates.SelectedIndex = 0;
        ddlEmailTemplates.AppendDataBoundItems = false;
    }
    /// <summary>
    /// Bind only selected email templates with dropdown list
    /// </summary>
    /// <param name="nEmailTemplates"></param>
    public void BindCustomEmailTemplates(IEnumerable<EmailTemplate> nEmailTemplates)
    {
        listEmailTemplates.Visible = true;
        if (ddlEmailTemplates.Items.Count > 0)
            ddlEmailTemplates.Items.Clear();
        ddlEmailTemplates.AppendDataBoundItems = true;
        ddlEmailTemplates.DataTextField = "Title";
        ddlEmailTemplates.DataValueField = "Id";
        ddlEmailTemplates.Items.Add(new ListItem("--Select Email Template--", "-1"));
        ddlEmailTemplates.DataSource = nEmailTemplates.OrderBy(x => x.Title);
        ddlEmailTemplates.DataBind();
        ddlEmailTemplates.SelectedIndex = 0;
        ddlEmailTemplates.AppendDataBoundItems = false;
    }
   
    /// <summary>
    /// Replace field tags with original values from currently selected account.
    /// </summary>
    /// <param name="sourceString">Field Tag to be replaced</param>
    /// <param name="accountID">Current Account ID</param>
    /// <returns>Replaced field tag with original value</returns>
    string ReplaceFieldTags(string sourceString, long? accountID = 0)
    {
        //Field tags are enclosed with curly braces i.e {Account.ID}, these values will be replaced with the original values
        string pattern = @"\{.*?\}";
        string orgString = sourceString;
        string FieldTitle = string.Empty;

        try
        {
            
            var matches = Regex.Matches(sourceString, pattern);
            foreach (Match resultMatches in matches)
            {
                string tag=resultMatches.ToString();
                var record = Engine.TagFieldsActions.GetAll(true).FirstOrDefault(x => x.TagDisplayName.Contains(tag));
                if (record == null) continue;

                FieldTitle = record.FieldSystemName;
                if (record.FilterDataType == 3)// For lookup table 
                {
                    var lookupTable = Engine.SQTablesActions.GetAll().OrderBy(m => m.Name).Where(l => l.Id == record.TableKey).FirstOrDefault();
                    if (!string.IsNullOrEmpty(lookupTable.TitleFieldName))
                        FieldTitle = string.Concat(lookupTable.SystemTableName, '.', lookupTable.TitleFieldName);
                }

                TableStructure nTable = new TableStructure();

                string query = Engine.ApplicationSettings.DefaultQuery.Replace("*", " distinct " + FieldTitle + " as SystemFieldName");
                //Add account ID to the where clause of default query defined in app.config
                query += " accounts.act_key  = " + AccountID.ToString();
                string valueToReplace = string.Empty;

                DataTable dtRecords = nTable.GetDatatable(ApplicationSettings.ADOConnectionString, query);

                for (int i = 0; i < dtRecords.Rows.Count; i++)
                {
                    DataRow itemDataRow = dtRecords.Rows[i];
                    string result = itemDataRow["SystemFieldName"].ToString();
                    orgString = orgString.Replace(tag, result);
                }
            }
            return orgString;
        }
        catch 
        {
            return orgString;
        }
        
    }

    

    /// <summary>
    /// Loads the selected email template data
    /// </summary>
    /// <param name="emailKey">Selected Email template key</param>
    public void LoadEmailTemplateData(int emailKey)
    {
        EmailTemplateKey = emailKey;
        EmailTemplate nEmailTemplate = Engine.ManageEmailTemplateActions.Get(EmailTemplateKey, true);
        if (nEmailTemplate != null)
        {
            bool IsLocked = false;
            bool.TryParse(nEmailTemplate.LockTemplate.ToString(), out IsLocked);
            //Makes the email template loaded data readonly.
            AllowEditing(IsLocked);

            //if (IsLocked) { AllowEditingpnlEmailForm.Enabled = false; txtEmailBody.Enabled = false;  }
            //else { pnlEmailForm.Enabled = true; txtEmailBody.Enabled = true;  }

            txtFromEmail.Text = string.IsNullOrEmpty(nEmailTemplate.FromEmail) ? "" : ReplaceFieldTags(nEmailTemplate.FromEmail);
            txtToEmail.Text = string.IsNullOrEmpty(nEmailTemplate.ToEmail) ? "" : ReplaceFieldTags(nEmailTemplate.ToEmail);
            txtCCEmail.Text = string.IsNullOrEmpty(nEmailTemplate.CC) ? "" : ReplaceFieldTags(nEmailTemplate.CC);
            txtBCCEmail.Text = string.IsNullOrEmpty(nEmailTemplate.BCC) ? "" : ReplaceFieldTags(nEmailTemplate.BCC);
            txtSubject.Text = string.IsNullOrEmpty(nEmailTemplate.SubjectEmail) ? "" : ReplaceFieldTags(nEmailTemplate.SubjectEmail);
            txtEmailBody.Content = string.IsNullOrEmpty(nEmailTemplate.BodyMessage) ? "" : ReplaceFieldTags(nEmailTemplate.BodyMessage);

            rdlFormatHtml.Checked = nEmailTemplate.EmailFormat;
            rdlFormatText.Checked = !nEmailTemplate.EmailFormat;

            //YA[Nov 20, 2013] Load attachments from email template, these will be temporary and will be permanent when email will be queued successfully.
            ctlOverrideEmailAttachments.EmailTemplateKey = EmailTemplateKey;
            ctlOverrideEmailAttachments.CloneTemplateAttachments(true);
            ctlOverrideEmailAttachments.BindTemplateAttachmentGrid();

            FormatText_CheckedChanged(this, null);
        }
    }

    private void AllowEditing(bool IsLocked)
    {
        txtFromEmail.Enabled = !IsLocked;
        txtToEmail.Enabled = !IsLocked;
        txtCCEmail.Enabled = !IsLocked;
        txtBCCEmail.Enabled = !IsLocked;
        txtSubject.Enabled = !IsLocked;
        rdlFormatText.Enabled = !IsLocked;
        rdlFormatHtml.Enabled = !IsLocked;
        txtEmailBody.Enabled = !IsLocked;
        lblEmailSepration.Visible = !IsLocked;
        lblEmailSepration1.Visible = !IsLocked;
        lblEmailSepration2.Visible = !IsLocked;
    }
    /// <summary>
    /// Required fields in editable email form
    /// </summary>
    /// <returns></returns>
    public bool IsQueueEmailFormFilled()
    {
        bool result = false;
        if (EmailTemplateKey > 0)
        {
            if (txtFromEmail.Text == "" || txtToEmail.Text == "")
            {
                lblMessageForm.SetStatus("From and To Email fields are required.");
                result = false;
            }
            else
            {
                result = true;
            }
        }
        return result;
    }

    /// <summary>
    /// Queues the email for sending
    /// </summary>
    /// <returns></returns>
    public bool QueueEmail()
    {
        bool result = false;
        if (EmailTemplateKey > 0)
        {
            EmailTemplate nEmailTemplate = Engine.ManageEmailTemplateActions.Get(EmailTemplateKey, true);
            bool IsAttachmentRequired = false;
            if (nEmailTemplate != null)
            {
                IsAttachmentRequired = nEmailTemplate.NeedAttachment == null ? false : nEmailTemplate.NeedAttachment.Value;
                if (txtFromEmail.Text == "" || txtToEmail.Text == "")
                {
                    lblMessageForm.SetStatus("From and To Email fields are required.");
                    result = false;
                }
                else if (IsAttachmentRequired && !ctlOverrideEmailAttachments.HasAttachments())
                {
                    lblMessageForm.SetStatus("Atleast one attachment required.");
                    result = false;
                }
                else
                {
                    var T = Engine.EmailQueueActions.Add(AccountID, DateTime.Now, EmailTemplateKey, (short)Konstants.EmailQueueStatus.Queued
                        , 0, true, txtFromEmail.Text, txtToEmail.Text, txtCCEmail.Text, txtBCCEmail.Text, rdlFormatHtml.Checked, txtSubject.Text, txtEmailBody.Content, nEmailTemplate.BCCHidden);
                    //YA[Nov 20, 2013] Change the temporary attachments to permanent
                    if (T != null && T.key > 0)
                    {
                        Engine.EmailAttachmentActions.UpdateAllTemporary(EmailTemplateKey, base.SalesPage.CurrentUser.FullName, T.key);
                    }
                    lblMessageForm.SetStatus("Email Queued successfully.");
                    result = true;
                }
            }
            
        }
        return result;
    }
   
    /// <summary>
    /// Disables the form, makes it readonly
    /// </summary>
    public void DisableForm()
    {
        pnlEmailForm.Enabled = false;
    }
    /// <summary>
    /// Changes the height of email body control
    /// </summary>
    /// <param name="height"></param>
    public void ChangeHeight(int height)
    {
        txtEmailBody.Height = height;
    }
    #endregion

    #region  Events

    /// <summary>
    /// Selected index change event for email template selection
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlEmailTemplates_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlEmailTemplates.SelectedIndex > 0)
        {
            int nValue = 0;
            if (int.TryParse(ddlEmailTemplates.SelectedValue, out nValue))
            {
                //pnlEmailForm.Enabled = true;
                LoadEmailTemplateData(nValue);
            }
        }
        else
        {
            Initialize();
            //pnlEmailForm.Enabled = false;
        }
    }
    /// <summary>
    /// Email format: Text or HTML change event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void FormatText_CheckedChanged(object sender, EventArgs e)
    {
        if (rdlFormatHtml.Checked)
        {
            txtEmailBody.EnableTextareaMode = false;
            //txtEmailBody.EditModes = Telerik.Web.UI.EditModes.All;
            txtEmailBody.CssClass = "";
            //for (int i = 0; i < txtEmailBody.Tools.Count; i++)
            //{
            //    for (int j = 0; j < txtEmailBody.Tools[i].Tools.Count; j++)
            //    {
            //        txtEmailBody.Tools[i].Tools[j].Visible = true;
            //    }
            //}
        }
        else
        {
            //txtEmailBody.EditModes = Telerik.Web.UI.EditModes.Design;
            
            //txtEmailBody.ToolbarMode = Telerik.Web.UI.EditorToolbarMode.
            txtEmailBody.EnableTextareaMode = true;
            txtEmailBody.CssClass = "divOverflow";
        }
    }
    #endregion


    

   

    
    
}