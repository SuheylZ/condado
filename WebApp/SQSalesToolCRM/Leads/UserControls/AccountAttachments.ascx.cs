using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;
using System.Text.RegularExpressions;

public partial class UserControls_AccountAttachments : AccountsBaseControl
{
    //public long AccountID
    //{
    //    get
    //    {
    //        return base.SalesPage.AccountID;
    //    }
    //}

    protected override void InnerLoad(bool bFirstTime)
    {
        if (bFirstTime)
        {
            BindTemplateAttachmentGrid();
        }
    }

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
    

    protected override void InnerInit()
    {
        if (base.SalesPage != null)
        {
            fUploadAsync.TemporaryFolder = base.SalesPage.TemporaryFolder;
        }
        SortColumn = String.Empty;
        BindTemplateAttachmentGrid();
    }
    protected override void InnerPostBack()
    {
        lblMessageForm.SetStatus("");        
    }
    protected override void InnerEnableControls(bool bEnable)
    {
        Helper.EnableControls(divAttachment, bEnable);
    }
    protected void grdTemplateAttachments_RowCommand(object sender, GridViewCommandEventArgs e)
    {
              
        if (e.CommandName == "DeleteAttachment")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdTemplateAttachments.DataKeys[row.RowIndex].Value.ToString();

            Engine.AccountAttachmentActions.Delete(Convert.ToInt32(dataKeyValue));
            lblMessageForm.SetStatus(Messages.RecordDeletedSuccess);
            BindTemplateAttachmentGrid();
        }
        if (e.CommandName == "Download")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdTemplateAttachments.DataKeys[row.RowIndex].Value.ToString();
            var U=  Engine.AccountAttachmentActions.Get(Convert.ToInt32(dataKeyValue));

            if (U.Attachment != null)
            {
                Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                Response.Clear();
                //Response.Buffer = true;
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
            
            if (AccountID == 0)
            {
                grdTemplateAttachments.DataSource = null;
                grdTemplateAttachments.DataBind();
                return;
            }
            var TemplateTemplates = Engine.AccountAttachmentActions.AllQueryable;
            //YA[13 March, 2014]
            if (Engine.AccountActions.IsMultipleAccountsAllowed())
            {
                var relatedAccountIds = Engine.AccountActions.GetAssociatedAccountsIds(AccountID);           
                var Records = ((from T in TemplateTemplates where relatedAccountIds.Contains(T.AccountId) select new { TemplateAttachmentKey = T.Id, TemplateAttachmentFileName = T.FileName, TemplateAttachmentDescription = T.FileDescription }).OrderBy(m => m.TemplateAttachmentDescription)).AsQueryable();                
                grdTemplateAttachments.DataSource = Helper.SortRecords(Records.AsQueryable(), SortColumn, SortAscending);
                grdTemplateAttachments.DataBind();
                relatedAccountIds = null;
            }
            else
            {                
                var Records = ((from T in TemplateTemplates where T.AccountId == AccountID select new { TemplateAttachmentKey = T.Id, TemplateAttachmentFileName = T.FileName, TemplateAttachmentDescription = T.FileDescription }).OrderBy(m => m.TemplateAttachmentDescription)).AsQueryable();                
                grdTemplateAttachments.DataSource = Helper.SortRecords(Records.AsQueryable(), SortColumn, SortAscending);
                grdTemplateAttachments.DataBind();
            }
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
    }
    private void SaveAttachments()
    {
        try
        {
            if (AccountID != 0)
            {
                String uploadedFileName = "";
                AccountAttachment nAccountAttachment = new AccountAttachment();
                nAccountAttachment.AccountId = AccountID;
                if (fUploadAsync.UploadedFiles.Count > 0)
                {
                    uploadedFileName = fUploadAsync.UploadedFiles[0].FileName;
                    UploadedFile file = fUploadAsync.UploadedFiles[0];
                    byte[] fileData = new byte[file.InputStream.Length];
                    file.InputStream.Read(fileData, 0, (int)file.InputStream.Length);
                    nAccountAttachment.Attachment = fileData;                    
                }

                nAccountAttachment.FileName = uploadedFileName;
                nAccountAttachment.IsDeleted = false;
                nAccountAttachment.FileDescription = txtDescription.Text;
                if (base.SalesPage.CurrentUser != null)
                {
                    nAccountAttachment.AddedBy = base.SalesPage.CurrentUser.FullName;
                }
                nAccountAttachment.AddedOn = DateTime.Now;
                Engine.AccountAttachmentActions.Add(nAccountAttachment);
                lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
            }
            txtDescription.Text = "";            
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
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
        throw new InvalidOperationException("This call is not valid for this fucntion"); 
    }

}
