using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using S = System.Web.Security.Membership;
using System.Linq.Dynamic;


public partial class Admin_ManagePosts : SalesBasePage
{
    #region Members/Properties

    enum PageDisplayMode { PostTemplate = 1, PostTemplateEdit = 2, PostTemplateSettings = 3 }

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
            var filterFieldsValue = Engine.TagFieldsActions.GetAll().Select(k => new { key = k.Id, Name = k.Name, k.FilterDataType }).OrderBy(m => m.Name).Where(l => l.FilterDataType == 2);
            ddlFieldsBeforeAfter.DataSource = filterFieldsValue;
            ddlFieldsBeforeAfter.DataBind();
        }
        catch (Exception ex)
        {
            lblMessageGrid.SetStatus(ex);
        }
    }

    private void BindPostTemplateGrid()
    {
        try
        {
            var postTemplates = Engine.ManagePostsActions.GetAll();
            var Records = (from T in postTemplates select new { PostKey = T.Id, PostTitle = T.Title, PostUrl = T.Url, Status = T.Enabled == true ? "Yes" : "No" }).AsQueryable();
            PagingNavigationBar.RecordCount = Records.Count();
            var sorted = (SortColumn == string.Empty) ? Records : (SortAscending) ? Records.OrderBy(SortColumn) : Records.OrderBy(SortColumn + " desc");
            PagingNavigationBar.RecordCount = sorted.Count(); 
            grdPosts.DataSource = sorted.Skip(PagingNavigationBar.SkipRecords).Take(PagingNavigationBar.PageSize); 
            grdPosts.DataBind();
        }
        catch (Exception ex)
        {
            lblMessageGrid.SetStatus(ex);
        }
    }
     
    private void SetPageMode(PageDisplayMode mode)
    {
        switch (mode)
        {
            case PageDisplayMode.PostTemplate:
                divForm.Visible = false;
                divGrid.Visible = true;
                hdnFieldIsEditMode.Value = "no";
                hdnFieldEditPostTemplateKey.Value = "";               
                break;
            case PageDisplayMode.PostTemplateEdit:
                divForm.Visible = true;
                divGrid.Visible = false;
                tlEmailTemplateStrip.SelectedIndex = 0;
                tabContPostTemplate.SelectedIndex = 0;
                break;
            case PageDisplayMode.PostTemplateSettings:
                divForm.Visible = true;
                divGrid.Visible = false;
                tlEmailTemplateStrip.SelectedIndex = 1;
                tabContPostTemplate.SelectedIndex = 1;
                break;
        }
    }

    public bool SaveRecord(bool ConvertToEditMode = false)
    {
        try
        {
            if (hdnFieldIsEditMode.Value == "no")
            {
                Post nPost = new Post();
                nPost.Title = txtTitle.Text;
                nPost.Url = txtUrl.Text;
                nPost.Type = Convert.ToInt16(ddlType.SelectedValue);
                nPost.Header = txtHeader.Text;
                nPost.RegexResponse = txtSuccessResponse.Text;
                nPost.Body = txtTemplateBody.Text;

                if (CurrentUserDetails != null)
                {
                    nPost.Added.By = CurrentUserDetails.Email;
                }
                nPost.Added.On = DateTime.Now;

                nPost.IsDeleted = false;
                nPost.Enabled = true;
                
                if (rdSendImmediately.Checked)
                {
                    nPost.PostSend = 0;
                }
                else if (rdSendAfterTrigger.Checked)
                {
                    nPost.PostSend = 1;
                    if (txtDuration.Text == "")
                    {
                        nPost.TriggerIncrement = 0;
                    }
                    else
                        nPost.TriggerIncrement = Convert.ToInt16(txtDuration.Text);
                    nPost.TriggerIncrementType = Convert.ToInt16(ddlSpan.SelectedValue);
                }
                else if (rdSendBeforeAfter.Checked)
                {
                    nPost.PostSend = 2;
                    if (txtDurationBeforeAfter.Text == "")
                    {
                        nPost.SpecificDateIncrement = 0;
                    }
                    else
                        nPost.SpecificDateIncrement = Convert.ToInt16(txtDurationBeforeAfter.Text);
                    nPost.SpecificDateIncrementType = Convert.ToInt16(ddlSpanBeforeAfter.SelectedValue);
                    nPost.SpecificDateBeforeAfter = ddlTypeBeforeAfter.SelectedValue == "0" ? true : false;
                    nPost.SpecificDateField = Convert.ToInt16(ddlFieldsBeforeAfter.SelectedValue);
                }
                nPost.CancelUponStatus = chkCancel.Checked;
                nPost.FilterSelection = Convert.ToInt16(rdBtnlstFilterSelection.SelectedValue);
                bool hasCustomFilterError = false;
                if (rdBtnlstFilterSelection.SelectedValue == "2")
                {
                    hasCustomFilterError = !CheckForValidCustomString();

                    nPost.FilterCustomValue = txtCustomFilter.Text;
                }
                if (hasCustomFilterError == true)
                {
                    return false;
                }

                Engine.ManagePostsActions.Add(nPost);
                if (ConvertToEditMode)
                {
                    hdnFieldEditPostTemplateKey.Value = nPost.Id.ToString();
                    hdnFieldIsEditMode.Value = "yes";
                    ManageFiltersControl.Parent_key = nPost.Id;
                }
            }
            else if (hdnFieldIsEditMode.Value == "yes")
            {
                if (hdnFieldEditPostTemplateKey.Value != "")
                {
                    Post nPost = Engine.ManagePostsActions.Get(Convert.ToInt32(hdnFieldEditPostTemplateKey.Value));

                    nPost.Title = txtTitle.Text;
                    nPost.Url = txtUrl.Text;
                    nPost.Type = Convert.ToInt16(ddlType.SelectedValue);
                    nPost.Header = txtHeader.Text;
                    nPost.RegexResponse = txtSuccessResponse.Text;
                    nPost.Body = txtTemplateBody.Text;

                    if (CurrentUserDetails != null)
                    {
                        nPost.Changed.By = CurrentUserDetails.Email;
                    }
                    nPost.Changed.On = DateTime.Now;

                    nPost.IsDeleted = false;
                    
                    if (rdSendImmediately.Checked)
                    {
                        nPost.PostSend = 0;
                    }
                    else if (rdSendAfterTrigger.Checked)
                    {
                        nPost.PostSend = 1;
                        if (txtDuration.Text == "")
                        {
                            nPost.TriggerIncrement = 0;
                        }
                        else
                            nPost.TriggerIncrement = Convert.ToInt16(txtDuration.Text);
                        nPost.TriggerIncrementType = Convert.ToInt16(ddlSpan.SelectedValue);
                    }
                    else if (rdSendBeforeAfter.Checked)
                    {
                        nPost.PostSend = 2;
                        if (txtDurationBeforeAfter.Text == "")
                        {
                            nPost.SpecificDateIncrement = 0;
                        }
                        else
                            nPost.SpecificDateIncrement = Convert.ToInt16(txtDurationBeforeAfter.Text);
                        nPost.SpecificDateIncrementType = Convert.ToInt16(ddlSpanBeforeAfter.SelectedValue);
                        nPost.SpecificDateBeforeAfter = ddlTypeBeforeAfter.SelectedValue == "0" ? true : false;
                        nPost.SpecificDateField = Convert.ToInt16(ddlFieldsBeforeAfter.SelectedValue);
                    }
                    nPost.CancelUponStatus = chkCancel.Checked;
                    nPost.FilterSelection = Convert.ToInt16(rdBtnlstFilterSelection.SelectedValue);
                    bool hasCustomFilterError = false;
                    if (rdBtnlstFilterSelection.SelectedValue == "2")
                    {
                        hasCustomFilterError = !CheckForValidCustomString();
                        nPost.FilterCustomValue = txtCustomFilter.Text;
                    }
                    if (hasCustomFilterError == true)
                    {
                        return false;
                    }
                    Engine.ManagePostsActions.Change(nPost);
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

    public void LoadEditFormValues(int postTemplatekey)
    {
        Post nPost = Engine.ManagePostsActions.Get(postTemplatekey);
        txtTitle.Text = nPost.Title;
        txtUrl.Text= nPost.Url;
        ddlType.SelectedValue= nPost.Type.ToString();
        txtHeader.Text= nPost.Header;
        txtSuccessResponse.Text= nPost.RegexResponse;
        txtTemplateBody.Text= nPost.Body;

        if (nPost.PostSend == 0)
        {
            rdSendImmediately.Checked = true;
            SetSettingTabMode(SettingTabMode.SendImmediately);
        }
        else if (nPost.PostSend == 1)
        {
            rdSendAfterTrigger.Checked = true;
            SetSettingTabMode(SettingTabMode.SendAfterTrigger);
            txtDuration.Text = nPost.TriggerIncrement.ToString();
            ddlSpan.SelectedValue = nPost.TriggerIncrementType.ToString();
        }
        else if (nPost.PostSend == 2)
        {
            rdSendBeforeAfter.Checked = true;
            SetSettingTabMode(SettingTabMode.SendBeforeAfter);
            txtDurationBeforeAfter.Text = nPost.SpecificDateIncrement.ToString();
            ddlSpanBeforeAfter.SelectedValue = nPost.SpecificDateIncrementType.ToString();
            if (nPost.SpecificDateBeforeAfter == true)
            {
                ddlTypeBeforeAfter.SelectedValue = "0";
            }
            else
            {
                ddlTypeBeforeAfter.SelectedValue = "1";
            }
            ListItem nItem = ddlFieldsBeforeAfter.Items.FindByValue(nPost.SpecificDateField.ToString());
            if (nItem!=null) ddlFieldsBeforeAfter.SelectedValue = nPost.SpecificDateField.ToString();
        }
        if (nPost.CancelUponStatus != null)
        {
            chkCancel.Checked = nPost.CancelUponStatus.Value;
        }
        rdBtnlstFilterSelection.SelectedValue = nPost.FilterSelection == null ? "0" : nPost.FilterSelection.ToString();
        if (nPost.FilterSelection == 2)
        {
            txtCustomFilter.Text = nPost.FilterCustomValue;
        }
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
    }

    public void ClearFields()
    {
        txtTitle.Text = "";
        txtTemplateBody.Text = "";
        txtSuccessResponse.Text = "";
        txtHeader.Text = "";
        txtUrl.Text = "";
        ddlType.SelectedIndex = 0;

        txtCustomFilter.Text = "";
        rdBtnlstFilterSelection.SelectedIndex = 0;
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
        rdSendAfterTrigger.Checked = false;
        rdSendBeforeAfter.Checked = false;
        rdSendImmediately.Checked = true;
        SetSettingTabMode(SettingTabMode.SendImmediately);
        chkCancel.Checked = false;        
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
                ddlFieldsBeforeAfter.SelectedIndex = 0;
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
                ddlFieldsBeforeAfter.SelectedIndex = 0;
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
            SortColumn = String.Empty;
            SortAscending = true;
            divGrid.Visible = true;
            divForm.Visible = false;

            //SZ [jan 10, 2013] Chnaged from short to enum
            //ManageFiltersControl.Parent_Type = 1;
            ManageFiltersControl.ParentType = FilterParentType.PostsWebForm;

            if (CurrentUserDetails != null)
            {
                ManageFiltersControl.AddedBy = CurrentUserDetails.Email;
                ManageFiltersControl.ChangedBy = CurrentUserDetails.Email;
            }

            BindDateFields();
            BindPostTemplateGrid();
        }
        lblMessageForm.SetStatus("");
        lblMessageGrid.SetStatus("");
        ctrlStatusCustomFilter.SetStatus("");
    }

    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        int size = e.PageSize;
        size = size > 100 ? 100 : size;
        grdPosts.PageSize = size;
        BindPostTemplateGrid();
    }

    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        grdPosts.PageIndex = e.PageNumber;
        BindPostTemplateGrid();

    }

    protected void grdPostTemplates_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdPosts.PageIndex = e.NewPageIndex;
        BindPostTemplateGrid();
    }

    protected void grdPostTemplates_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "EditX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdPosts.DataKeys[row.RowIndex].Value.ToString();
            hdnFieldEditPostTemplateKey.Value = dataKeyValue;
            ManageFiltersControl.Parent_key = Convert.ToInt32(dataKeyValue);
            ManageFiltersControl.SetControlModeFromOutside(true);
            ManageFiltersControl.BindEmailFilterGrid();
            hdnFieldIsEditMode.Value = "yes";
            LoadEditFormValues(Convert.ToInt32(dataKeyValue));
            SetPageMode(PageDisplayMode.PostTemplateEdit);
            
        }
        else if (e.CommandName == "DeleteX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdPosts.DataKeys[row.RowIndex].Value.ToString();

            Engine.ManagePostsActions.Delete(Convert.ToInt32(dataKeyValue));
            lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
            BindPostTemplateGrid();
        }
        else if (e.CommandName == "EnabledX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdPosts.DataKeys[row.RowIndex].Value.ToString();

            Engine.ManagePostsActions.MakeEnabled(Convert.ToInt32(dataKeyValue));

            lblMessageGrid.SetStatus(Messages.RecordUpdatedSuccess);
            BindPostTemplateGrid();
        }
        else if (e.CommandName == "SettingX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdPosts.DataKeys[row.RowIndex].Value.ToString();
            hdnFieldEditPostTemplateKey.Value = dataKeyValue;
            ManageFiltersControl.Parent_key = Convert.ToInt32(dataKeyValue);
            ManageFiltersControl.BindEmailFilterGrid();
            ManageFiltersControl.SetControlModeFromOutside(true);
            hdnFieldIsEditMode.Value = "yes";
            LoadEditFormValues(Convert.ToInt32(dataKeyValue));
            SetPageMode(PageDisplayMode.PostTemplateSettings);
        }

    }

    protected void grdPostTemplates_Sorting(object sender, GridViewSortEventArgs e)
    {
        if (SortColumn == e.SortExpression)
            SortAscending = !SortAscending;
        else
        {
            SortColumn = e.SortExpression;
            SortAscending = true;
        }
        BindPostTemplateGrid();
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
            bool hasSavedRecordSuccessful = SaveRecord();
            if (hasSavedRecordSuccessful)
            {
                SetPageMode(PageDisplayMode.PostTemplate);
                BindPostTemplateGrid();
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
        SetPageMode(PageDisplayMode.PostTemplate);
        BindPostTemplateGrid();
    }

    protected void btnAddNewPost_Click(object sender, EventArgs e)
    {
        hdnFieldIsEditMode.Value = "no";
        ClearFields();
        ManageFiltersControl.Parent_key = 0;
        ManageFiltersControl.BindEmailFilterGrid();
        SetPageMode(PageDisplayMode.PostTemplateEdit);
        ManageFiltersControl.SetControlModeFromOutside(true);
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
        if (rdBtnlstFilterSelection.SelectedValue == "2")//Custom filter selection option selected.
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

    protected void Grid_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var ctl = e.Row.FindControl("lblSep") as Label;
            var ctl2 = e.Row.FindControl("lnkDelete") as LinkButton;
            if (ctl2 != null && ctl != null && !CurrentUser.Security.Administration.CanDelete)
            {
                ctl.Visible = false;
                ctl2.Visible = false;
            }
        }
    }

    #endregion
}