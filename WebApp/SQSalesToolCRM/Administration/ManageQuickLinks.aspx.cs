using System;

using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using S = System.Web.Security.Membership;
using System.Linq.Dynamic;

public partial class Admin_ManageQuickLinks : SalesBasePage
{

    #region Members/Properties

    enum PageDisplayMode { QuickLinkGrid = 1, QuickLinkEdit = 2, QuickLinkAssignSkills = 3 }
    
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

    private void BindQuickLinksGrid()
    {
        try
        {
            var quickLinksRecords = Engine.QuickLinksActions.GetAll();
            var Records = (from T in quickLinksRecords select new { Key = T.Id, Name = T.Name, Description= T.Description, Url = T.Url, Status = T.Enabled == true ? "Yes" : "No" }).AsQueryable();
            //PagingNavigationBar.RecordCount = Records.Count();
            var sorted = (SortColumn == string.Empty) ? Records : (SortAscending) ? Records.OrderBy(SortColumn) : Records.OrderBy(SortColumn + " desc");
            PagingNavigationBar.RecordCount = sorted.Count();
            grdQuickLinks.DataSource = sorted.Skip(PagingNavigationBar.SkipRecords).Take(PagingNavigationBar.PageSize);
            grdQuickLinks.DataBind();
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
            case PageDisplayMode.QuickLinkGrid:
                divForm.Visible = false;
                divGrid.Visible = true;
                hdnFieldIsEditMode.Value = "no";
                hdnFieldEditRecordKey.Value = "";
                tabContQuickLinks.Enabled = false;
                break;
            case PageDisplayMode.QuickLinkEdit:
                divForm.Visible = true;
                divGrid.Visible = false;
                tlQuickLinksStrip.SelectedIndex = 0;
                tabContQuickLinks.SelectedIndex = 0;
                tabContQuickLinks.Enabled = true;
                break;
            case PageDisplayMode.QuickLinkAssignSkills:
                divForm.Visible = true;
                divGrid.Visible = false;
                tlQuickLinksStrip.SelectedIndex = 1;
                tabContQuickLinks.SelectedIndex = 1;
                tabContQuickLinks.Enabled = true;
                break;
        }
    }

    public bool SaveRecord(bool ConvertToEditMode = false)
    {
        try
        {
            if (hdnFieldIsEditMode.Value == "no")
            {                
                QuickLink nQuickLink = new QuickLink();

                nQuickLink.Name = txtName.Text;
                nQuickLink.Url = txtUrl.Text;
                nQuickLink.Target = Convert.ToByte(ddlTarget.SelectedValue);
                nQuickLink.Description = txtDescription.Text;
                nQuickLink.Message = txtMessage.Text;
                nQuickLink.IsAlert = chkAlertBox.Checked;

                
                nQuickLink.Added.By = CurrentUser.Email;
                
                nQuickLink.Added.On = DateTime.Now;

                nQuickLink.IsDeleted = false;
                nQuickLink.Enabled = true;


                Engine.QuickLinksActions.Add(nQuickLink);
                if (ConvertToEditMode)
                {
                    hdnFieldEditRecordKey.Value = nQuickLink.Id.ToString();
                    hdnFieldIsEditMode.Value = "yes";
                    SkillGroupsAssignment(nQuickLink.Id);
                    tabContQuickLinks.Enabled = true;
                }
            }
            else if (hdnFieldIsEditMode.Value == "yes")
            {
                if (hdnFieldEditRecordKey.Value != "")
                {
                    QuickLink nQuickLink = Engine.QuickLinksActions.Get(Convert.ToInt32(hdnFieldEditRecordKey.Value));

                    nQuickLink.Name = txtName.Text;
                    nQuickLink.Url = txtUrl.Text;
                    nQuickLink.Target = Convert.ToByte(ddlTarget.SelectedValue);
                    nQuickLink.Description = txtDescription.Text;                    
                    nQuickLink.Message = txtMessage.Text;
                    nQuickLink.IsAlert = chkAlertBox.Checked;
                   
                        nQuickLink.Changed.By = CurrentUser.Email;                   
                    nQuickLink.Changed.On = DateTime.Now;

                    nQuickLink.IsDeleted = false;

                    Engine.QuickLinksActions.Change(nQuickLink);

                    foreach (ListItem item in ctlSkillGroupsAssignment.SelectedItems)
                    {
                        Engine.SkillGroupActions.AssignQuickLink(Convert.ToInt16(item.Value), nQuickLink.Id, CurrentUserDetails!=null? CurrentUserDetails.Email: "",false);                        
                    }                    
                    foreach (ListItem item in ctlSkillGroupsAssignment.AvailableItems)
                    {
                        Engine.SkillGroupActions.AssignQuickLink(Convert.ToInt16(item.Value), nQuickLink.Id, CurrentUserDetails != null ? CurrentUserDetails.Email : "", true);                        
                    }
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

    public void LoadEditFormValues(int quickLinkId)
    {        
        QuickLink nQuickLink = Engine.QuickLinksActions.Get(quickLinkId);
        txtName.Text = nQuickLink.Name;
        txtUrl.Text = nQuickLink.Url;
        ddlTarget.SelectedValue = nQuickLink.Target.ToString();
        txtDescription.Text = nQuickLink.Description;
        chkAlertBox.Checked = nQuickLink.IsAlert ?? false;
        txtMessage.Text = nQuickLink.Message;

        SkillGroupsAssignment(quickLinkId);
    }

    public void ClearFields()
    {
        txtName.Text = "";
        txtDescription.Text = "";
        txtMessage.Text = "";        
        txtUrl.Text = "";
        chkAlertBox.Checked = false;
        ddlTarget.SelectedIndex = 0;       
    }

    public void SkillGroupsAssignment(int editRecordId =-1)
    {
        ctlSkillGroupsAssignment.Initialize();
        foreach (var U in Engine.QuickLinksActions.SkillGroupsNotAssignedToQuickLinks(editRecordId))
            ctlSkillGroupsAssignment.AvailableItems.Add(new ListItem(U.Name, U.Id.ToString()));
        foreach (var U in Engine.QuickLinksActions.SkillGroupsAssignedToQuickLinks(editRecordId))
            ctlSkillGroupsAssignment.SelectedItems.Add(new ListItem(U.Name, U.Id.ToString()));
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
                        
            BindQuickLinksGrid();      

        }
        lblMessageForm.SetStatus("");
        lblMessageGrid.SetStatus("");        
    }

    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        int size = e.PageSize;
        size = size > 100 ? 100 : size;
        grdQuickLinks.PageSize = size;
        BindQuickLinksGrid();
    }

    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        grdQuickLinks.PageIndex = e.PageNumber;
        BindQuickLinksGrid();

    }

    protected void grdQuickLinks_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdQuickLinks.PageIndex = e.NewPageIndex;
        BindQuickLinksGrid();
    }

    protected void grdQuickLinks_RowCommand(object sender, GridViewCommandEventArgs e)
    {

        if (e.CommandName == "EditX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdQuickLinks.DataKeys[row.RowIndex].Value.ToString();
            hdnFieldEditRecordKey.Value = dataKeyValue;            
            hdnFieldIsEditMode.Value = "yes";
            LoadEditFormValues(Convert.ToInt32(dataKeyValue));
            SetPageMode(PageDisplayMode.QuickLinkEdit);
        }
        else if (e.CommandName == "DeleteX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdQuickLinks.DataKeys[row.RowIndex].Value.ToString();

            Engine.QuickLinksActions.Delete(Convert.ToInt32(dataKeyValue));
            lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
            BindQuickLinksGrid();
        }
        else if (e.CommandName == "EnabledX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdQuickLinks.DataKeys[row.RowIndex].Value.ToString();

            Engine.QuickLinksActions.MakeEnabled(Convert.ToInt32(dataKeyValue));

            lblMessageGrid.SetStatus(Messages.RecordUpdatedSuccess);
            BindQuickLinksGrid();
        }
        else if (e.CommandName == "AssignSkillsX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = grdQuickLinks.DataKeys[row.RowIndex].Value.ToString();
            hdnFieldEditRecordKey.Value = dataKeyValue;            
            hdnFieldIsEditMode.Value = "yes";
            LoadEditFormValues(Convert.ToInt32(dataKeyValue));
            SetPageMode(PageDisplayMode.QuickLinkAssignSkills);
        }

    }

    protected void grdQuickLinks_Sorting(object sender, GridViewSortEventArgs e)
    {
        PagingNavigationBar.PageNumber = 1;
        if (SortColumn == e.SortExpression)
            SortAscending = !SortAscending;
        else
        {
            SortColumn = e.SortExpression;
            SortAscending = true;
        }
        BindQuickLinksGrid();
    }

    protected void btnAddNewQuickLink_Click(object sender, EventArgs e)
    {
        hdnFieldIsEditMode.Value = "no";
        ClearFields();        
        SetPageMode(PageDisplayMode.QuickLinkEdit);
        
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
                SetPageMode(PageDisplayMode.QuickLinkGrid);
                BindQuickLinksGrid();
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
        SetPageMode(PageDisplayMode.QuickLinkGrid);
        BindQuickLinksGrid();
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