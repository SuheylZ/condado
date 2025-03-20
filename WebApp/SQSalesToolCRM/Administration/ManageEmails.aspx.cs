using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;

using SalesTool.DataAccess.Models;
using S = System.Web.Security.Membership;

using Telerik.Web.UI;
using System.Linq.Dynamic;
using System.Text.RegularExpressions;

public partial class Admin_ManageEmails : SalesBasePage 
{
	#region Members/Properties

	enum PageDisplayMode { EmailTemplate = 1, EmailTemplateEdit = 2, EmailTemplateSettings = 3 }

	enum SettingTabMode { SendImmediately = 1, SendAfterTrigger = 2, SendBeforeAfter = 3 }

	private User CurrentUserDetails
	{
		get
		{
			try
			{                
				Guid key = Guid.Parse(S.GetUser().ProviderUserKey.ToString());
				if (Engine.UserActions.Get(key) != null)
				{
					return Engine.UserActions.Get(key);
				}
				else
					return null;
			}
			catch (Exception)
			{
				return null;
			}

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

	#endregion

	#region Methods

	private void BindDateFields()
	{        
		try
		{            
			if (ddlFieldsBeforeAfter.Items.Count > 0)
				ddlFieldsBeforeAfter.Items.Clear();
			ddlFieldsBeforeAfter.Items.Add(new ListItem("---Select Value---", "-1"));
			var filterFieldsValue = Engine.TagFieldsActions.GetAll().Select(k => new { key = k.Id, Name = k.Name, k.FilterDataType }).OrderBy(m => m.Name).Where(l => l.FilterDataType == 2); //Filter Field data type of date
			ddlFieldsBeforeAfter.DataSource = filterFieldsValue;
			ddlFieldsBeforeAfter.DataBind();
		}
		catch (Exception ex)
		{
			lblMessageGrid.SetStatus(ex);
		}
	}

	private void BindEmailTemplateGrid()
	{
		try
		{
			var emailTemplates = Engine.ManageEmailTemplateActions.GetAll();
			var Records = (from T in emailTemplates select new { EmailKey = T.Id, EmailTitle = T.Title, SubjectEmail = T.SubjectEmail, Status = T.Enabled.Value == true ? "Yes" : "No" }).AsQueryable();
			PagingNavigationBar.RecordCount = Records.Count();
			var sorted = (SortColumn == string.Empty) ? Records : (SortAscending) ? Records.OrderBy(SortColumn) : Records.OrderBy(SortColumn + " desc");
			PagingNavigationBar.RecordCount = sorted.Count(); 
			grdEmailTemplates.DataSource = sorted.Skip(PagingNavigationBar.SkipRecords).Take(PagingNavigationBar.PageSize); 
			grdEmailTemplates.DataBind();
		}
		catch (Exception ex)
		{
			lblMessageGrid.SetStatus(ex);
		}
	}

	private void BindEmailAttachmentGrid()
	{
		try
		{
			if (hdnFieldEditEmailTemplateKey.Value == "")
			{
				grdEmailAttachments.DataSource = null;
				grdEmailAttachments.DataBind();
				return;
			}
            var emailTemplates = Engine.EmailAttachmentActions.GetAllByTemplateKey(Convert.ToInt32(hdnFieldEditEmailTemplateKey.Value));
			var Records = (from T in emailTemplates select new { EmailAttachmentKey = T.Id, EmailAttachmentFileName = T.FileName, EmailAttachmentDescription = T.FileDescription }).OrderBy(m => m.EmailAttachmentDescription);
			grdEmailAttachments.DataSource = Records;
			grdEmailAttachments.DataBind();
		}
		catch (Exception ex)
		{
			lblMessageGrid.SetStatus(ex);
		}
	}

	private void SaveAttachments()
	{
		try
		{
			if (hdnFieldIsEditMode.Value == "yes")
			{
				if (hdnFieldEditEmailTemplateKey.Value != "")
				{
					String uploadedFileName = "";


					EmailAttachment nEmailAttachment = new EmailAttachment();
					nEmailAttachment.EmailTemplateKey = Convert.ToInt32(hdnFieldEditEmailTemplateKey.Value);
					if (fUploadAsync.UploadedFiles.Count > 0)
					{
						uploadedFileName = fUploadAsync.UploadedFiles[0].FileName;
						UploadedFile file = fUploadAsync.UploadedFiles[0];
						byte[] fileData = new byte[file.InputStream.Length];
						file.InputStream.Read(fileData, 0, (int)file.InputStream.Length);
						nEmailAttachment.Attachment = fileData;
					}

					nEmailAttachment.FileName = uploadedFileName;
					nEmailAttachment.IsDeleted = false;
					nEmailAttachment.FileDescription = txtDescription.Text;
					if (CurrentUserDetails != null)
					{
						nEmailAttachment.Added.By = CurrentUserDetails.Email;
					}
					nEmailAttachment.Added.On = DateTime.Now;
					Engine.EmailAttachmentActions.Add(nEmailAttachment);
				}
			}
			txtDescription.Text = "";
			lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
		}
		catch (Exception ex)
		{
			lblMessageForm.SetStatus(ex);
		}
	}

	private void SetPageMode(PageDisplayMode mode)
	{
		switch (mode)
		{
			case PageDisplayMode.EmailTemplate:
				divForm.Visible = false;
				divGrid.Visible = true;
				hdnFieldIsEditMode.Value = "no";
				hdnFieldEditEmailTemplateKey.Value = "";
				btnUpload.Enabled = false;
				break;
			case PageDisplayMode.EmailTemplateEdit:
				divForm.Visible = true;
				divGrid.Visible = false;
				tlEmailTemplateStrip.SelectedIndex = 0;
				tabContEmailTemplate.SelectedIndex= 0;
				//tlEmailTemplateStrip.MultiPage.PageViews[1].Visible = 
				//tabContEmailTemplate.PageViews[1].Visible = false;                
				break;
			case PageDisplayMode.EmailTemplateSettings:
				divForm.Visible = true;
				divGrid.Visible = false;
				tlEmailTemplateStrip.SelectedIndex = 1;
				tabContEmailTemplate.SelectedIndex = 1;
				//tabContEmailTemplate.PageViews[0].Visible = false;                
				break;
		}
	}

	public bool SaveRecord(bool ConvertToEditMode = false)
	{
		try
		{
			if (hdnFieldIsEditMode.Value == "no")
			{
				EmailTemplate nEmailTemplate = new EmailTemplate();
				nEmailTemplate.Title = txtTitle.Text;
				if (CurrentUserDetails != null)
				{
					nEmailTemplate.Added.By = CurrentUserDetails.Email;
				}
				nEmailTemplate.Added.On = DateTime.Now;
				nEmailTemplate.BCC = txtBCCEmail.Text;
                nEmailTemplate.BCCHidden = txtBCCEmailHidden.Text;
                nEmailTemplate.BodyMessage = txtEmailBody.Content;
				nEmailTemplate.CC = txtCCEmail.Text;
				nEmailTemplate.IsDeleted = false;
				if (rdlFormatHtml.Checked)
				{
					nEmailTemplate.EmailFormat = true;
				}
				else
				{
					nEmailTemplate.EmailFormat = false;
				}
				nEmailTemplate.Enabled = true;
				nEmailTemplate.FromEmail = txtFromEmail.Text;
				//nEmailTemplate.IsPopup = chkPopNotifcation.Checked;
                nEmailTemplate.LockTemplate = chkLockTemplate.Checked;
                nEmailTemplate.NeedAttachment = chkRequireAttachment.Checked;
				nEmailTemplate.SubjectEmail = txtSubject.Text;
				nEmailTemplate.ToEmail = txtToEmail.Text;
				if (rdSendImmediately.Checked)
				{
					nEmailTemplate.EmailSend = 0;
				}
				else if (rdSendAfterTrigger.Checked)
				{
					nEmailTemplate.EmailSend = 1;
					if (txtDuration.Text=="")
					{
						nEmailTemplate.TriggerIncrement = 0;
					}
					else
						nEmailTemplate.TriggerIncrement = Convert.ToInt16(txtDuration.Text);
					nEmailTemplate.TriggerIncrementType = Convert.ToInt16(ddlSpan.SelectedValue);
				}
				else if (rdSendBeforeAfter.Checked)
				{
					nEmailTemplate.EmailSend = 2;
					if (txtDurationBeforeAfter.Text=="")
					{
						nEmailTemplate.SpecificDateIncrement = 0;
					}
					else
						nEmailTemplate.SpecificDateIncrement = Convert.ToInt16(txtDurationBeforeAfter.Text);
					nEmailTemplate.SpecificDateIncrementType = Convert.ToInt16(ddlSpanBeforeAfter.SelectedValue);
					nEmailTemplate.SpecificDateBeforeAfter = ddlTypeBeforeAfter.SelectedValue == "0" ? true : false;                    
					nEmailTemplate.SpecificDateField = Convert.ToInt16(ddlFieldsBeforeAfter.SelectedValue);
				}
				nEmailTemplate.CancelUponStatus = chkCancel.Checked;
				nEmailTemplate.FilterSelection = Convert.ToInt16(rdBtnlstFilterSelection.SelectedValue);                
				bool hasCustomFilterError = false;
				if (rdBtnlstFilterSelection.SelectedValue == "2")
				{
					hasCustomFilterError = !CheckForValidCustomString();
					
					nEmailTemplate.FilterCustomValue = txtCustomFilter.Text;
				}
				if (hasCustomFilterError == true)
				{
					return false;
				}

				Engine.ManageEmailTemplateActions.Add(nEmailTemplate);
				if (ConvertToEditMode)
				{
					hdnFieldEditEmailTemplateKey.Value = nEmailTemplate.Id.ToString();
					hdnFieldIsEditMode.Value = "yes";
					btnUpload.Enabled = true;
					ManageFiltersControl.Parent_key = nEmailTemplate.Id;
				}                
			}
			else if (hdnFieldIsEditMode.Value == "yes")
			{
				if (hdnFieldEditEmailTemplateKey.Value != "")
				{
					EmailTemplate nEmailTemplate = Engine.ManageEmailTemplateActions.Get(Convert.ToInt32(hdnFieldEditEmailTemplateKey.Value));

					nEmailTemplate.Title = txtTitle.Text;
					if (CurrentUserDetails != null)
					{
						nEmailTemplate.Changed.By = CurrentUserDetails.Email;
					}
					nEmailTemplate.Changed.On = DateTime.Now;
					nEmailTemplate.BCC = txtBCCEmail.Text;
                    nEmailTemplate.BCCHidden = txtBCCEmailHidden.Text;
					nEmailTemplate.BodyMessage = txtEmailBody.Content;
					nEmailTemplate.CC = txtCCEmail.Text;
					nEmailTemplate.IsDeleted = false;
					if (rdlFormatHtml.Checked)
					{
						nEmailTemplate.EmailFormat = true;
					}
					else
					{
						nEmailTemplate.EmailFormat = false;
					}                    
					nEmailTemplate.FromEmail = txtFromEmail.Text;
					//nEmailTemplate.IsPopup = chkPopNotifcation.Checked;
                    nEmailTemplate.LockTemplate =chkLockTemplate.Checked;
                    nEmailTemplate.NeedAttachment = chkRequireAttachment.Checked;
					nEmailTemplate.SubjectEmail = txtSubject.Text;
					nEmailTemplate.ToEmail = txtToEmail.Text;
					if (rdSendImmediately.Checked)
					{
						nEmailTemplate.EmailSend = 0;
					}
					else if (rdSendAfterTrigger.Checked)
					{
						nEmailTemplate.EmailSend = 1;
						if (txtDuration.Text == "")
						{
							nEmailTemplate.TriggerIncrement = 0;
						}
						else
							nEmailTemplate.TriggerIncrement = Convert.ToInt16(txtDuration.Text);
						nEmailTemplate.TriggerIncrementType = Convert.ToInt16(ddlSpan.SelectedValue);
					}
					else if (rdSendBeforeAfter.Checked)
					{
						nEmailTemplate.EmailSend = 2;
						if (txtDurationBeforeAfter.Text == "")
						{
							nEmailTemplate.SpecificDateIncrement = 0;
						}
						else
							nEmailTemplate.SpecificDateIncrement = Convert.ToInt16(txtDurationBeforeAfter.Text);
						nEmailTemplate.SpecificDateIncrementType = Convert.ToInt16(ddlSpanBeforeAfter.SelectedValue);
						nEmailTemplate.SpecificDateBeforeAfter = ddlTypeBeforeAfter.SelectedValue == "0" ? true : false;
						nEmailTemplate.SpecificDateField = Convert.ToInt16(ddlFieldsBeforeAfter.SelectedValue);
					}
					nEmailTemplate.CancelUponStatus = chkCancel.Checked;
					nEmailTemplate.FilterSelection = Convert.ToInt16(rdBtnlstFilterSelection.SelectedValue);                                       
					bool hasCustomFilterError = false;
					if (rdBtnlstFilterSelection.SelectedValue == "2")
					{
						hasCustomFilterError = !CheckForValidCustomString();                        
						nEmailTemplate.FilterCustomValue = txtCustomFilter.Text;
					}
					if (hasCustomFilterError) return false;                    
					Engine.ManageEmailTemplateActions.Change(nEmailTemplate);
				}

			}
			
			lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
			return true;
		}
		catch (Exception ex)
		{
			lblMessageForm.SetStatus(ex);
			return false;
		}
	}

	public void LoadEditFormValues(int emailTemplatekey)
	{
		EmailTemplate nEmailTemplate = Engine.ManageEmailTemplateActions.Get(emailTemplatekey);
		txtTitle.Text = nEmailTemplate.Title;
		txtBCCEmail.Text = nEmailTemplate.BCC;
        txtBCCEmailHidden.Text = nEmailTemplate.BCCHidden;
		txtEmailBody.Content = nEmailTemplate.BodyMessage;
		txtCCEmail.Text = nEmailTemplate.CC;
		if (nEmailTemplate.EmailFormat == true)
		{
			rdlFormatHtml.Checked = true;
		}
		else
		{
			rdlFormatText.Checked = true;
		}
		txtFromEmail.Text = nEmailTemplate.FromEmail;
		//chkPopNotifcation.Checked = nEmailTemplate.IsPopup == null ? false : nEmailTemplate.IsPopup.Value;
        chkLockTemplate.Checked = nEmailTemplate.LockTemplate == null? false : nEmailTemplate.LockTemplate.Value;
        chkRequireAttachment.Checked = nEmailTemplate.NeedAttachment == null ? false : nEmailTemplate.NeedAttachment.Value;
		txtSubject.Text = nEmailTemplate.SubjectEmail;
		txtToEmail.Text = nEmailTemplate.ToEmail;

		if (nEmailTemplate.EmailSend == 0)
		{
			rdSendImmediately.Checked= true;
			SetSettingTabMode(SettingTabMode.SendImmediately);
		}
		else if (nEmailTemplate.EmailSend == 1)
		{
			rdSendAfterTrigger.Checked = true;
			SetSettingTabMode(SettingTabMode.SendAfterTrigger);
			txtDuration.Text = nEmailTemplate.TriggerIncrement.ToString();
			ddlSpan.SelectedValue = nEmailTemplate.TriggerIncrementType.ToString();
		}
		else if (nEmailTemplate.EmailSend == 2)
		{
			rdSendBeforeAfter.Checked = true;
			SetSettingTabMode(SettingTabMode.SendBeforeAfter);
			txtDurationBeforeAfter.Text= nEmailTemplate.SpecificDateIncrement.ToString();
			ddlSpanBeforeAfter.SelectedValue =nEmailTemplate.SpecificDateIncrementType.ToString();
			if (nEmailTemplate.SpecificDateBeforeAfter == true)
			{
				ddlTypeBeforeAfter.SelectedValue = "0";
			}
			else
			{
				ddlTypeBeforeAfter.SelectedValue = "1";    
			}
			ddlFieldsBeforeAfter.SelectedValue = nEmailTemplate.SpecificDateField.ToString();
		}
		if (nEmailTemplate.CancelUponStatus!= null)
		{
			chkCancel.Checked= nEmailTemplate.CancelUponStatus.Value; 
		}
		rdBtnlstFilterSelection.SelectedValue = nEmailTemplate.FilterSelection==null? "0": nEmailTemplate.FilterSelection.ToString();
		if (nEmailTemplate.FilterSelection == 2)
		{            
			txtCustomFilter.Text = nEmailTemplate.FilterCustomValue;
		}
		rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
	}

	public void ClearFields()
	{
		txtTitle.Text = "";
		txtBCCEmail.Text = "";
        txtBCCEmailHidden.Text = "";
		txtEmailBody.Content = "";
		txtCCEmail.Text = "";
		rdlFormatHtml.Checked = false;
		rdlFormatText.Checked = true;
		txtFromEmail.Text = "";
		//chkPopNotifcation.Checked = false;
        chkLockTemplate.Checked = false;
        chkRequireAttachment.Checked = false;
		txtSubject.Text = "";
		txtToEmail.Text = "";
		txtCustomFilter.Text = "";
		rdBtnlstFilterSelection.SelectedIndex = 0;
		rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
		rdSendAfterTrigger.Checked = false;
		rdSendBeforeAfter.Checked = false;
		rdSendImmediately.Checked = true;        
		SetSettingTabMode(SettingTabMode.SendImmediately);
		chkCancel.Checked = false;
		grdEmailAttachments.DataSource = null;
		grdEmailAttachments.DataBind();
	}

	private void SetSettingTabMode(SettingTabMode nSetting)
	{
		switch (nSetting)
		{
			case SettingTabMode.SendImmediately:
				txtDuration.Enabled = false;
				ddlSpan.Enabled = false;
				txtDurationBeforeAfter.Enabled = false;                
				ddlSpanBeforeAfter.Enabled = false;
				ddlTypeBeforeAfter.Enabled = false;
				ddlFieldsBeforeAfter.Enabled = false;
				txtDuration.Text = "";
				ddlSpan.SelectedIndex = 0;
				txtDurationBeforeAfter.Text = "";
				ddlSpanBeforeAfter.SelectedIndex = 0;
				ddlTypeBeforeAfter.SelectedIndex = 0;
				if(ddlFieldsBeforeAfter.Items.Count > 0 ) ddlFieldsBeforeAfter.SelectedIndex = 0;                
				break;
			case SettingTabMode.SendAfterTrigger:
				txtDuration.Enabled = true;
				ddlSpan.Enabled = true;
				txtDurationBeforeAfter.Enabled = false;
				ddlSpanBeforeAfter.Enabled = false;
				ddlTypeBeforeAfter.Enabled = false;
				ddlFieldsBeforeAfter.Enabled = false;
				txtDurationBeforeAfter.Text = "";
				ddlSpanBeforeAfter.SelectedIndex = 0;
				ddlTypeBeforeAfter.SelectedIndex = 0;
                if (ddlFieldsBeforeAfter.Items.Count > 0) ddlFieldsBeforeAfter.SelectedIndex = 0;               
				break;
			case SettingTabMode.SendBeforeAfter:
				txtDuration.Enabled = false;
				ddlSpan.Enabled = false;                
				txtDurationBeforeAfter.Enabled = true;
				ddlSpanBeforeAfter.Enabled = true;
				ddlTypeBeforeAfter.Enabled = true;
				ddlFieldsBeforeAfter.Enabled = true;
				txtDuration.Text = "";
				ddlSpan.SelectedIndex = 0;                
				break;
			default:
				break;
		}
	}

	#endregion

	#region Events

	protected void Page_Init(object sender, EventArgs args)
	{
		PagingNavigationBar.SizeChanged += Evt_PageSizeChanged;
		PagingNavigationBar.IndexChanged += Evt_PageNumberChanged;

		this.Master.buttonYes.Click += new EventHandler(btnCancelOnForm_Click);

	}

	protected void Page_Load(object sender, EventArgs e)
	{
		if (!Page.IsPostBack)
		{
			// SZ [Dec 12. 2012] This code has been added to remove the dependency on temp folder in App_Data folder
			// with this code, the ASP.NEt temporary folder is used
			fUploadAsync.TemporaryFolder = TemporaryFolder;

			SortColumn = String.Empty;
			SortAscending = true;
			divGrid.Visible = true;
			divForm.Visible = false;

			//SZ [Jan 10, 2013] Chnaged for better management
			ManageFiltersControl.ParentType = FilterParentType.EmailWebForm; 
			
			 if (CurrentUserDetails != null)
			 {
				 ManageFiltersControl.AddedBy = CurrentUserDetails.Email;
				 ManageFiltersControl.ChangedBy = CurrentUserDetails.Email;                
			 }

			
			BindDateFields();
			BindEmailTemplateGrid();
		}
		lblMessageForm.SetStatus("");
		lblMessageGrid.SetStatus("");
		ctrlStatusCustomFilter.SetStatus("");
	}

	protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
	{
		int size = e.PageSize;
		size = size > 100 ? 100 : size;
		grdEmailTemplates.PageSize = size;
		BindEmailTemplateGrid();

	}

	protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
	{
		grdEmailTemplates.PageIndex = e.PageNumber;
		BindEmailTemplateGrid();

	}

	protected void grdEmailTemplates_PageIndexChanging(object sender, GridViewPageEventArgs e)
	{
		grdEmailTemplates.PageIndex = e.NewPageIndex;
		BindEmailTemplateGrid();
	}

	protected void grdEmailTemplates_RowCommand(object sender, GridViewCommandEventArgs e)
	{

		if (e.CommandName == "EditX")
		{
			GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
			String dataKeyValue = grdEmailTemplates.DataKeys[row.RowIndex].Value.ToString();
			hdnFieldEditEmailTemplateKey.Value = dataKeyValue;
			ManageFiltersControl.Parent_key = Convert.ToInt32(dataKeyValue);
			ManageFiltersControl.SetControlModeFromOutside(true);
			ManageFiltersControl.BindEmailFilterGrid();
			hdnFieldIsEditMode.Value = "yes";
			LoadEditFormValues(Convert.ToInt32(dataKeyValue));
			SetPageMode(PageDisplayMode.EmailTemplateEdit);
			BindEmailAttachmentGrid();
			btnUpload.Enabled = true;
		}
		else if (e.CommandName == "DeleteX")
		{
			GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
			String dataKeyValue = grdEmailTemplates.DataKeys[row.RowIndex].Value.ToString();
			Engine.FilterAreaActions.DeleteAll(Convert.ToInt32(dataKeyValue), (short)FilterParentType.EmailWebForm);
			Engine.ManageEmailTemplateActions.Delete(Convert.ToInt32(dataKeyValue));
			lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
			BindEmailTemplateGrid();
		}
		else if (e.CommandName == "EnabledX")
		{
			GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
			String dataKeyValue = grdEmailTemplates.DataKeys[row.RowIndex].Value.ToString();

			Engine.ManageEmailTemplateActions.MakeEnabled(Convert.ToInt32(dataKeyValue));
			lblMessageGrid.SetStatus(Messages.RecordUpdatedSuccess);
			BindEmailTemplateGrid();
		}
		else if (e.CommandName == "SettingX")
		{
			GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
			String dataKeyValue = grdEmailTemplates.DataKeys[row.RowIndex].Value.ToString();
			hdnFieldEditEmailTemplateKey.Value = dataKeyValue;
			ManageFiltersControl.Parent_key = Convert.ToInt32(dataKeyValue);
			ManageFiltersControl.BindEmailFilterGrid();
			ManageFiltersControl.SetControlModeFromOutside(true);
			hdnFieldIsEditMode.Value = "yes";
			LoadEditFormValues(Convert.ToInt32(dataKeyValue));
			SetPageMode(PageDisplayMode.EmailTemplateSettings);
		}

	}

	protected void grdEmailTemplates_Sorting(object sender, GridViewSortEventArgs e)
	{
		if (SortColumn == e.SortExpression)
			SortAscending = !SortAscending;
		else
		{
			SortColumn = e.SortExpression;
			SortAscending = true;
		}
		BindEmailTemplateGrid();
	}

	protected void grdEmailAttachments_RowCommand(object sender, GridViewCommandEventArgs e)
	{
		if (e.CommandName == "DeleteAttachment")
		{
			GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
			String dataKeyValue = grdEmailAttachments.DataKeys[row.RowIndex].Value.ToString();

			Engine.EmailAttachmentActions.Delete(Convert.ToInt32(dataKeyValue));
			lblMessageForm.SetStatus(Messages.RecordDeletedSuccess);
			BindEmailAttachmentGrid();
		}
		if (e.CommandName == "Download")
		{
			GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
			String dataKeyValue = grdEmailAttachments.DataKeys[row.RowIndex].Value.ToString();
			var U=  Engine.EmailAttachmentActions.Get(Convert.ToInt32(dataKeyValue));                            
			
			Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			if (U.Attachment != null)
			{
				Response.Clear();
				Response.Buffer = true;
				Response.ContentType = "application/octet-stream";
				// to open file prompt Box open or Save file
                Response.AddHeader("content-disposition", "attachment;filename=" + MakeValidFileName(U.FileName));    
				Response.OutputStream.Write(U.Attachment, 0, U.Attachment.Length);
				//Response.BinaryWrite(U.Attachment);
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
	protected void btnApply_Click(object sender, EventArgs e)
	{
		try
		{
		   SaveRecord(true);
		}
		catch (Exception ex)
		{
			lblMessageForm.SetStatus(ex);
		}
	}

	protected void btnSubmit_Click(object sender, EventArgs e)
	{
		try
		{
			bool hasSavedRecordSuccessful =  SaveRecord();
			if (hasSavedRecordSuccessful)
			{
				SetPageMode(PageDisplayMode.EmailTemplate);
				BindEmailTemplateGrid();
				ClearFields();    
			}            
		}
		catch (Exception ex)
		{
			lblMessageForm.SetStatus(ex);
		}
	}

	protected void btnCancelOnForm_Click(object sender, EventArgs e)
	{
		SetPageMode(PageDisplayMode.EmailTemplate);
		BindEmailTemplateGrid();
	}

	protected void btnAddNewEmail_Click(object sender, EventArgs e)
	{
		hdnFieldIsEditMode.Value = "no";
		ClearFields();
		ManageFiltersControl.Parent_key = 0;
		ManageFiltersControl.BindEmailFilterGrid();
		SetPageMode(PageDisplayMode.EmailTemplateEdit);
		ManageFiltersControl.SetControlModeFromOutside(true);
	}

	protected void btnUpload_Click(object sender, EventArgs e)
	{
		SaveAttachments();
		BindEmailAttachmentGrid();
	}

	protected void rdSendImmediately_CheckedChanged(object sender, EventArgs e)
	{
		if (rdSendImmediately.Checked)
		{
			SetSettingTabMode(SettingTabMode.SendImmediately);
		}
	}

	protected void rdSendAfterTrigger_CheckedChanged(object sender, EventArgs e)
	{
		if (rdSendAfterTrigger.Checked)
		{
			SetSettingTabMode(SettingTabMode.SendAfterTrigger);
		}
	}

	protected void rdSendBeforeAfter_CheckedChanged(object sender, EventArgs e)
	{
		if (rdSendBeforeAfter.Checked)
		{
			SetSettingTabMode(SettingTabMode.SendBeforeAfter);
		}
	}

	protected void rdBtnlstFilterSelection_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (rdBtnlstFilterSelection.SelectedValue == "2")
		{
			txtCustomFilter.Enabled = true;            
		}
		else
		{
			txtCustomFilter.Enabled = false;
		}
	}

	protected void txtCustomFilter_TextChanged(object sender, EventArgs e)
	{
		CheckForValidCustomString();
	}

	private bool CheckForValidCustomString()
	{
		try
		{
			CustomFilterParser nCustomFilter = new CustomFilterParser(txtCustomFilter.Text);
			float result = nCustomFilter.ParseInput();
			List<string> listOpds = nCustomFilter.listOperands;
			return ManageFiltersControl.CheckOrderNumberValues(listOpds);
		}
		catch (Exception ex)
		{
			ctrlStatusCustomFilter.SetStatus(ErrorMessages.ErrorParsing + ex.Message);
			return false;
		}
	}

   
	protected void tlEmailTemplateStrip_TabClick1(object sender, RadTabStripEventArgs e)
	{        
		if (!e.Tab.PageView.Visible)
		{
			e.Tab.PageView.Visible = true;     
		}
	}
	#endregion





    protected void grdEmailTemplates_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var ctl = e.Row.FindControl("lblEmailSep") as Label;
            var ctl2 = e.Row.FindControl("lnkDelete") as LinkButton;
            if (ctl2 != null && ctl!=null && !CurrentUser.Security.Administration.CanDelete)
            {
                ctl.Visible = false;
                ctl2.Visible = false;
            }
        }
    }
}