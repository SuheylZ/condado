using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;
using System.Text.RegularExpressions;

public partial class UserControls_OverrideAccountAttachments : AccountsBaseControl
{
    #region Members/Properties

    /// <summary>
    /// Grid Sort column name
    /// </summary>
    private string SortColumn
    {
        get
        {
            return hdSortColumn.Value.Trim();
        }
        set
        {
            hdSortColumn.Value = value.Trim();
        }
    }
    /// <summary>
    /// Grid records sort direction
    /// </summary>
    private bool SortAscending
    {
        get
        {
            bool bAsc = false;
            bool.TryParse(hdSortDirection.Value, out bAsc);
            return bAsc;
        }
        set
        {
            hdSortDirection.Value = value.ToString();
        }
    }

    public int EmailTemplateKey
    {
        get
        {
            int bValue = 0;
            int.TryParse(hdnFieldEmailTemplateKey.Value, out bValue);
            return bValue;
        }
        set
        {
            hdnFieldEmailTemplateKey.Value = value.ToString();
        }
    }

    public long EmailQueueKey
    {
        get
        {
            long bValue = 0;
            long.TryParse(hdnFieldEmailQueueKey.Value, out bValue);
            return bValue;
        }
        set
        {
            hdnFieldEmailQueueKey.Value = value.ToString();
        }
    }
    
    #endregion

    #region Methods


    public override bool IsValidated
    {
        get
        {
            //SZ [Apr 19, 2013] this control does not hide or show grid so it must always return false

            return true;
        }
    }

    protected override void InnerSave()
    {
        throw new InvalidOperationException("This call is not valid for this control");
    }

    protected override void InnerInit()
    {
        if (base.SalesPage != null)
        {
            fUploadAsync.TemporaryFolder = base.SalesPage.TemporaryFolder;
        }
        SortColumn = String.Empty;
        //BindTemplateAttachmentGrid();
    }
    protected override void InnerPostBack()
    {
        lblMessageForm.SetStatus("");
    }
    protected override void InnerEnableControls(bool bEnable)
    {
        Helper.EnableControls(divAttachment, bEnable);
    }

    public static byte[] StrToByteArray(string str)
    {
        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
        return encoding.GetBytes(str);
    }
    protected void btnUpload_Click(object sender, EventArgs e)
    {
        SaveAttachments();
        BindTemplateAttachmentGrid();
    }

    public void BindTemplateAttachmentGrid()
    {
        try
        {
            grdTemplateAttachments.DataSource = null;
            grdTemplateAttachments.DataBind();
            if (EmailTemplateKey == 0)
            {
                return;
            }
            var TemplateTemplates = Engine.EmailAttachmentActions.GetAllByTemplateQueueKey(EmailTemplateKey, EmailQueueKey);
            var Records = ((from T in TemplateTemplates select new { TemplateAttachmentKey = T.Id, TemplateAttachmentFileName = T.FileName, TemplateAttachmentDescription = T.FileDescription, UserName = T.Added.By }).OrderBy(m => m.TemplateAttachmentDescription)).AsQueryable();
            string userFullName = base.SalesPage.CurrentUser.FullName;
            if (!string.IsNullOrEmpty(userFullName))
            {
                Records = Records.Where(x => x.UserName == userFullName);
            }
            //var sorted = (SortColumn == string.Empty) ? Records : (SortAscending) ? Records.OrderBy(SortColumn) : Records.OrderBy(SortColumn + " desc");
            grdTemplateAttachments.DataSource = Helper.SortRecords(Records.AsQueryable(), SortColumn, SortAscending);
            grdTemplateAttachments.DataBind();
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
    }
    public bool HasAttachments()
    {
        return grdTemplateAttachments.Rows.Count > 0;
    }
    private void SaveAttachments()
    {
        try
        {
            if (EmailTemplateKey != 0)
            {
                String uploadedFileName = "";
                EmailAttachment nEmailAttachment = new EmailAttachment();
                if (fUploadAsync.UploadedFiles.Count > 0)
                {
                    uploadedFileName = fUploadAsync.UploadedFiles[0].FileName;
                    UploadedFile file = fUploadAsync.UploadedFiles[0];
                    byte[] fileData = new byte[file.InputStream.Length];
                    file.InputStream.Read(fileData, 0, (int)file.InputStream.Length);
                    nEmailAttachment.Attachment = fileData;
                }
                nEmailAttachment.FileName = uploadedFileName;
                nEmailAttachment.FileDescription = txtDescription.Text;
                if (base.SalesPage.CurrentUser != null)
                {
                    nEmailAttachment.Added.By = base.SalesPage.CurrentUser.FullName;
                }
                nEmailAttachment.EmailTemplateKey = EmailTemplateKey;
                if (EmailQueueKey != 0)
                    nEmailAttachment.EmailQueueKey = EmailQueueKey;
                Engine.EmailAttachmentActions.Add(nEmailAttachment, true, true);
                lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
            }
            txtDescription.Text = "";
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
    }

    public void CloneTemplateAttachments(bool IsTemporary = false)
    {
        Engine.EmailAttachmentActions.DeleteAllTemporary(EmailTemplateKey, base.SalesPage.CurrentUser.FullName);
        foreach (EmailAttachment itemAttachment in Engine.EmailAttachmentActions.GetAllByTemplateKey(EmailTemplateKey, true).ToList())
        {
            if (itemAttachment.Attachment != null)
            {
                EmailAttachment nEmailAttachment = new EmailAttachment();
                nEmailAttachment.Attachment = itemAttachment.Attachment;
                nEmailAttachment.FileName = itemAttachment.FileName;
                nEmailAttachment.FileDescription = itemAttachment.FileDescription;
                if (base.SalesPage.CurrentUser != null)
                {
                    nEmailAttachment.Added.By = base.SalesPage.CurrentUser.FullName;
                }
                nEmailAttachment.EmailTemplateKey = EmailTemplateKey;
                if (EmailQueueKey != 0)
                    nEmailAttachment.EmailQueueKey = EmailQueueKey;
                Engine.EmailAttachmentActions.Add(nEmailAttachment, true, IsTemporary);
            }
        }
    }

    protected override void InnerLoad(bool bFirstTime)
    {
        if (bFirstTime)
        {
            BindTemplateAttachmentGrid();
        }
    }


    #endregion

    #region Events

    protected void grdTemplateAttachments_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "DeleteAttachment")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdTemplateAttachments.DataKeys[row.RowIndex].Value.ToString();

            Engine.EmailAttachmentActions.Delete(Convert.ToInt32(dataKeyValue));
            lblMessageForm.SetStatus(Messages.RecordDeletedSuccess);
            BindTemplateAttachmentGrid();
        }
        if (e.CommandName == "Download")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdTemplateAttachments.DataKeys[row.RowIndex].Value.ToString();
            var U = Engine.EmailAttachmentActions.Get(Convert.ToInt32(dataKeyValue));

            if (U.Attachment != null)
            {
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.Clear();
                Response.Buffer = true;
                Response.ContentType = "application/octet-stream";
                // to open file prompt Box open or Save file
                Response.AddHeader("content-disposition", "attachment;filename=" + MakeValidFileName(U.FileName));

                Response.OutputStream.Write(U.Attachment, 0, U.Attachment.Length);
                Response.Flush();
            }
        }
    }
    public string MakeValidFileName(string name)
    {
        string invalidChars = Regex.Escape(new string(System.IO.Path.GetInvalidFileNameChars()));
        string invalidReStr = string.Format(@"[{0}]+", invalidChars);
        string replace = Regex.Replace(name, invalidReStr, "_").Replace(";", "").Replace(",", "");
        return replace;
    }
    protected void grdTemplateAttachments_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (SortColumn == e.SortExpression)
            SortAscending = !SortAscending;
        else
        {
            SortColumn = e.SortExpression;
            SortAscending = true;
        }
        BindTemplateAttachmentGrid();
    }

    #endregion
    
    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
    //interface ILeadControlSave, in the accounts base page
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        //SZ [Apr 19, 2013] this control does not hide or show grid so it must always return false
    //        return false;
    //    }
    //}

}
