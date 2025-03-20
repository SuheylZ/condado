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
using System.Data;


public partial class Admin_ManageCampaign : SalesBasePage//System.Web.UI.Page
{
    #region Members/Properties

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

    private string SortColumnCC
    {
        get
        {
            return hdSortColCC.Value.Trim();
        }
        set
        {
            hdSortColCC.Value = value.Trim();
        }

    }

    private bool SortAscendingCC
    {
        get
        {
            bool bAsc = false;
            bool.TryParse(hdSortDirectionCC.Value, out bAsc);
            return bAsc;
        }
        set
        {
            hdSortDirectionCC.Value = value.ToString();
        }
    }

    public int totalGridRecords = 0;

    #endregion

    #region Methods

    protected void ClearFields()
    {
        txtOutpulseId.Text = "";
        ddlOutpulseType.SelectedIndex = 0;
        ddlCompanyKey.SelectedIndex = 0;

        txtTitle.Text = "";
        txtNote.Text = "";
        txtAlternateTitle.Text = "";

        ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
        ////fields from add/edit form as per customer requirement.
        //if (ddlCampaignType.Items.Count > 0)
        //    ddlCampaignType.SelectedIndex = 0;
        //txtCostPerLead.Text = "";


        chkActive.Checked = true;
        txtEmail.Text = "";
        tlEditor.Content = "";
        txtArcMap.Text = "";
        chkDTE.Checked = false;
        //if (ApplicationSettings.IsTermLife)
        //{
        //    lblDteType.Visible = true;
        //    chkDTE.Visible = true;
        //}
    }

    protected void ClearCampaignCostFormFields()
    {
        //rdStartDatePicker.SelectedDate = DateTime.Now;
        //rdEndDatePicker.SelectedDate = DateTime.Now;
        SetDefaultDates();
        txtCostPerLead.Text = "";
        txtReturnCampCost.Text = "100";
        txtTimer.Text = "";

        if (ddlCampaignType.Items.Count > 0)
        {
            ddlCampaignType.SelectedIndex = 0;
        }

    }

    private string ConvertSortDirection(SortDirection sortDirection)
    {
        string newSortDirection = String.Empty;

        switch (sortDirection)
        {
            case SortDirection.Ascending:
                newSortDirection = "ASC";
                break;

            case SortDirection.Descending:
                newSortDirection = "DESC";
                break;
        }

        return newSortDirection;
    }

    private void LoadCampaignCostDetail(int dataKeyValue)
    {

        //txtTimer.Text = Convert.ToString(campaignCost.Timer);
    }

    private bool TimePeriodOverlap(DateTime InputStartDate, DateTime InputEndDate)
    {
        Boolean result = false;
        int CampaignId = Convert.ToInt32(hdnFieldEditCampaignKey.Value);
        var campaignCostTemplates = Engine.CampaignCostActions.GetAll();
        var Records = (from T in campaignCostTemplates.Where(x => x.CampaignId == CampaignId) select new TimePeriodOverlapDataItem { CampaignCostId = T.CampaignCostId, StartDate = T.StartDate, EndDate = T.EndDate }).AsQueryable();


        //var Records = (from T in campaignCostTemplates.Take(1).Where(x => x.CampaignId == CampaignId) select new { CampaignCostId = T.CampaignCostId, StartDate = T.StartDate, EndDate = T.EndDate }).AsQueryable();


        foreach (var item in Records)
        {
            DateTime ExistingStartDate = Convert.ToDateTime(item.StartDate);
            DateTime ExistingEndDate = Convert.ToDateTime(item.EndDate);

            result = ((ExistingStartDate >= InputStartDate && ExistingStartDate < InputEndDate) || (ExistingEndDate <= InputEndDate && ExistingEndDate > InputStartDate) || (ExistingStartDate <= InputStartDate && ExistingEndDate >= InputEndDate));

            if (result)
            {
                return result;
            }
        }

        return result;
    }

    private void SetDefaultDates()
    {
        /* What this function do!
         * on a new record, the start date should always default to the end date of the current oldest record.  
         * if this is the first record for that campaign, it should default to the Sunday date of the current week
        the end date should default to 6 days after the start date. 
        */
        //Boolean result = false;
        int CampaignId = Convert.ToInt32(hdnFieldEditCampaignKey.Value);
        var campaignCostTemplates = Engine.CampaignCostActions.GetAll();

        var Records = (from T in campaignCostTemplates.Where(x => x.CampaignId == CampaignId).OrderByDescending(x => x.CampaignCostId).Take(1) select new TimePeriodOverlapDataItem { CampaignCostId = T.CampaignCostId, StartDate = T.StartDate, EndDate = T.EndDate }).AsQueryable();

        if (Records.Count() > 0)
        {
            foreach (var item in Records)
            {
                rdStartDatePicker.SelectedDate = Convert.ToDateTime(item.EndDate);
                rdEndDatePicker.SelectedDate = Convert.ToDateTime(item.EndDate).AddDays(6);
            }
        }
        else
        {
            var today = DateTime.Today;
            var sunday = today.AddDays(-(int)today.DayOfWeek).AddDays(7);

            rdStartDatePicker.SelectedDate = Convert.ToDateTime(sunday);
            rdEndDatePicker.SelectedDate = Convert.ToDateTime(sunday).AddDays(6);
        }

    }

    #endregion

    #region Events

    #region Event Handlers

    protected void Page_Init(object sender, EventArgs args)
    {

        PagingNavigationBar.SizeChanged += Evt_PageSizeChanged;
        PagingNavigationBar.IndexChanged += Evt_PageNumberChanged;

        pgBarCampaignCost.SizeChanged += Evt_CampaignCostPageSizeChanged;
        pgBarCampaignCost.IndexChanged += Evt_CampaignCostPageNumberChanged;

        this.Master.buttonYes.Click += new EventHandler(btnCancelOnForm_Click);
    }


    protected void Page_Load(object sender, EventArgs e)
    {
        //arcMapVld.Enabled = ApplicationSettings.IsTermLife;
        arcMapVld.Enabled = Engine.ApplicationSettings.IsTermLife;

        //arcMapVld_ValidatorCalloutExtender.Enabled = ApplicationSettings.IsTermLife;
        arcMapVld_ValidatorCalloutExtender.Enabled = Engine.ApplicationSettings.IsTermLife;
        if (!Page.IsPostBack)
        {

            SortColumn = String.Empty;
            SortAscending = true;
            SortColumnCC = String.Empty;
            SortAscendingCC = true;

            divGrid.Visible = true;
            divForm.Visible = false;
            btnDelete.Visible = false;
            lblCampaignID.Visible = false;
            txtCampaignID.Visible = false;
            trCampaignID.Visible = false;
            BindCampaignGrid();

            lblSuccessMsg.Text = Messages.RecordSavedSuccess;
            //BindCampaignCostGrid();
            ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
            BindCampaignType();

            BindCompany();

        }

        //rdStartDatePicker.EnableAjaxSkinRendering = true;
        //rdEndDatePicker.EnableAjaxSkinRendering = true;
        // LoadCampaignCostDetail(1);

        lblMessageForm.Text = "";
        lblMessageGrid.SetStatus("");
    }
    #endregion

    #region Controls events
    protected void btnAddNewCampaign_Click(object sender, EventArgs e)
    {
        hdnFieldIsEditMode.Value = "no";
        divGrid.Visible = false;
        divForm.Visible = true;
        btnDelete.Visible = false;
        lblCampaignID.Visible = false;
        txtCampaignID.Visible = false;
        trCampaignID.Visible = false;
        ClearFields();
        lblAddEditCampaign.Text = "Add Campaign";
        tabDiv.Visible = true;
        tlCampaignStrip.SelectedIndex = 0;


        SetEditMode();
    }

    private void SetEditMode()
    {
        divGrid.Visible = false;
        divForm.Visible = true;
        pnlAddBoth.Visible = true;
        divCampaignCost.Visible = false;
        lblSuccessMsg.Visible = true;
        btnRetunToCampaignCost.Visible = false;

        tlCampaignStrip.Tabs[0].Enabled = true;
        tlCampaignStrip.Tabs[0].Selected = true;
        tlCampaignStrip.Tabs[1].Enabled = false;
        pgCampaign.Selected = true;

        if (ddlCampaignType.Items.Count == 1)
        {
            BindCampaignType();
        }
        if (ddlCompanyKey.Items.Count == 1)
        {
            BindCampaignType();
        }
    }

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            Engine.ManageCampaignActions.Delete(Convert.ToInt16(hdnFieldEditCampaignKey.Value));
            Engine.CampaignCostActions.DeleteWhenParentCampaignDeleted(Convert.ToInt16(hdnFieldEditCampaignKey.Value));

            BindCampaignGrid();
            divGrid.Visible = true;
            divForm.Visible = false;
            lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
            divCampaignCost.Visible = false;
            divAddUpdateForm.Visible = false;

            tabDiv.Visible = false;
        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }

    }

    protected void btnDeleteCampCost_Click(object sender, EventArgs e)
    {
        try
        {
            Engine.CampaignCostActions.Delete(Convert.ToInt16(hdnFieldEditCampaignCostKey.Value));

            BindCampaignCostGrid();
            divCampaignCost.Visible = true;
            divAddUpdateForm.Visible = false;
            lblMessageCampaignCostGrid.Text = "Record delete successful.";
        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (hdnFieldIsEditMode.Value == "no")
            {
                Campaign nCampaign = new Campaign();

                nCampaign.OutpulseId = txtOutpulseId.Text;
                nCampaign.OutpulseType = ddlOutpulseType.SelectedIndex;

                if (ddlCompanyKey.Items.Count > 0 && ddlCompanyKey.SelectedIndex != 0)
                {
                    nCampaign.CompanyID = int.Parse(ddlCompanyKey.SelectedValue);
                }
                else
                {
                    nCampaign.CompanyID = -1;
                }
                nCampaign.Title = txtTitle.Text;

                //SZ [Jan 10, 2013] asked by client in discussion.
                nCampaign.Description = tlEditor.Content;

                nCampaign.Notes = txtNote.Text;
                nCampaign.AlternateTitle = txtAlternateTitle.Text;
                nCampaign.ArcMap = txtArcMap.Text;
                nCampaign.HasDTE = chkDTE.Checked;
                ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
                ////fields from add/edit form as per customer requirement.
                //if (ddlCampaignType.Items.Count > 0 && ddlCampaignType.SelectedIndex != 0)
                //{
                //    nCampaign.CampaignTypeKey = Convert.ToInt16(ddlCampaignType.SelectedValue);
                //}
                //else
                //{
                //    nCampaign.CampaignTypeKey = -1;
                //}

                nCampaign.IsActive = chkActive.Checked;
                nCampaign.AddedOn = DateTime.Now;
                //if ( CurrentUser == null)
                //{
                //    nCampaign.AddedBy = null;//Logged In User Id
                //}
                //else
                nCampaign.AddedBy = CurrentUser.Key;//Logged In User Id
                nCampaign.ChangedOn = DateTime.Now;//Last Modified Date
                nCampaign.ChangedBy = null;// Last Modified User

                ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
                ////fields from add/edit form as per customer requirement.
                //if (txtCostPerLead.Text != "")
                //nCampaign.CampaignCPL = Convert.ToDecimal(txtCostPerLead.Text);


                nCampaign.IsDeleted = false;
                nCampaign.email = txtEmail.Text;

                int CampId = Engine.ManageCampaignActions.Add(nCampaign);

                hdnFieldEditCampaignKey.Value = Convert.ToString(CampId);


                lblMessageGrid.SetStatus(Messages.RecordSavedSuccess);
                //divGrid.Visible = true;
                //divForm.Visible = false;
                //BindCampaignGrid();

                //divAddUpdateForm.Visible = false;
                //divCampaignCost.Visible = false;

                divGrid.Visible = false;
                divForm.Visible = false;
                //BindCampaignGrid();

                ClearCampaignCostFormFields();
                divAddUpdateForm.Visible = true;
                pnlAddCampCostButtons.Visible = false;
                pnlAddBoth.Visible = true;
                divCampaignCost.Visible = false;
                lblSuccessMsg.Visible = true;
                btnRetunToCampaignCost.Visible = false;

                //tlCampaignStrip.SelectedIndex = 1;
                //tlMultipage.SelectedIndex = 1;


                tlCampaignStrip.Tabs[1].Selected = true;
                tlCampaignStrip.Tabs[1].Enabled = true;

                //tlCampaignStrip.Tabs[1].Selected = false;
                //tlMultipage.SelectedIndex = 1;

                tlCampaignStrip.Tabs[0].Enabled = false;
                tlCampaignStrip.Tabs[0].Selected = false;
                pgCampaignCost.Selected = true;

            }
            else if (hdnFieldIsEditMode.Value == "yes")
            {

                if (hdnFieldEditCampaignKey.Value != "")
                {
                    Campaign nCampaign = Engine.ManageCampaignActions.Get(Convert.ToInt16(hdnFieldEditCampaignKey.Value));

                    nCampaign.OutpulseId = txtOutpulseId.Text;
                    nCampaign.OutpulseType = ddlOutpulseType.SelectedIndex;

                    if (ddlCompanyKey.Items.Count > 0 && ddlCompanyKey.SelectedIndex != 0)
                    {
                        nCampaign.CompanyID = int.Parse(ddlCompanyKey.SelectedValue);
                    }
                    else
                    {
                        nCampaign.CompanyID = -1;
                    }

                    nCampaign.Title = txtTitle.Text;

                    //SZ [Jan 10, 2013] asked by client in discussion.
                    nCampaign.Description = tlEditor.Content;

                    nCampaign.Notes = txtNote.Text;
                    nCampaign.ArcMap = txtArcMap.Text;
                    nCampaign.AlternateTitle = txtAlternateTitle.Text;
                    nCampaign.HasDTE = chkDTE.Checked;

                    ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
                    ////fields from add/edit form as per customer requirement.
                    //if (ddlCampaignType.Items.Count > 0 && ddlCampaignType.SelectedIndex != 0)
                    //{
                    //    nCampaign.CampaignTypeKey = Convert.ToInt16(ddlCampaignType.SelectedValue);
                    //}
                    //else
                    //{
                    //    nCampaign.CampaignTypeKey = -1;
                    //}

                    nCampaign.IsActive = chkActive.Checked;


                    nCampaign.ChangedBy = CurrentUser.Key;// Last Modified User
                    nCampaign.ChangedOn = DateTime.Now;

                    ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
                    ////fields from add/edit form as per customer requirement.
                    //if (txtCostPerLead.Text != "")
                    //nCampaign.CampaignCPL = Convert.ToDecimal(txtCostPerLead.Text);

                    nCampaign.IsDeleted = false;
                    nCampaign.email = txtEmail.Text;
                    Engine.ManageCampaignActions.Change(nCampaign);

                    lblMessageGrid.SetStatus(Messages.RecordSavedSuccess);
                    divGrid.Visible = true;
                    divForm.Visible = false;
                    BindCampaignGrid();

                    divAddUpdateForm.Visible = false;
                    divCampaignCost.Visible = false;
                    btnRetunToCampaignCost.Visible = true;

                    tabDiv.Visible = false;
                }

            }


        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }

    }

    protected void btnAddNewCampaignCost_Click(object sender, EventArgs e)
    {
        hdnFieldIsEditMode.Value = "no";

        ClearCampaignCostFormFields();
        divAddUpdateForm.Visible = true;
        divCampaignCost.Visible = false;

        pnlAddBoth.Visible = false;
        pnlAddCampCostButtons.Visible = true;
        btnDeleteCampCost.Visible = false;
    }

    protected void btnSubmitCampCost_Click(object sender, EventArgs e)
    {
        try
        {

            //Page.Validate("gpCampaignCost");
            //if (Page.IsValid)
            // {
            if (hdnFieldIsEditMode.Value == "no")
            {
                CampaignCost campaigncost = new CampaignCost();

                campaigncost.CampaignId = Convert.ToInt32(hdnFieldEditCampaignKey.Value);
                campaigncost.Cost = Convert.ToDecimal(txtCostPerLead.Text);


                if (ddlCampaignType.Items.Count > 0 && ddlCampaignType.SelectedIndex != 0)
                {
                    campaigncost.CampaignTypeId = Convert.ToInt16(ddlCampaignType.SelectedValue);
                }
                else
                {
                    campaigncost.CampaignTypeId = -1;
                }

                campaigncost.Return = Convert.ToDecimal(txtReturnCampCost.Text);
                campaigncost.Timer = Convert.ToInt16(txtTimer.Text);

                campaigncost.StartDate = rdStartDatePicker.SelectedDate;
                campaigncost.EndDate = rdEndDatePicker.SelectedDate;

                Boolean res = TimePeriodOverlap(Convert.ToDateTime(rdStartDatePicker.SelectedDate), Convert.ToDateTime(rdEndDatePicker.SelectedDate));

                if (!res)
                {
                    lblErrorMsg.Visible = false;
                    Engine.CampaignCostActions.Add(campaigncost);

                    divAddUpdateForm.Visible = false;
                    BindCampaignCostGrid();
                    divCampaignCost.Visible = true;
                }
                else
                {
                    lblErrorMsg.Visible = true;
                }
            }
            if (hdnFieldIsEditMode.Value == "yes")
            {
                if (hdnFieldEditCampaignCostKey.Value != "")
                {
                    CampaignCost campaignCost = Engine.CampaignCostActions.Get(Convert.ToInt16(hdnFieldEditCampaignCostKey.Value));

                    campaignCost.StartDate = rdStartDatePicker.SelectedDate;
                    campaignCost.EndDate = rdEndDatePicker.SelectedDate;

                    if (ddlCampaignType.Items.Count > 0 && ddlCampaignType.SelectedIndex != 0)
                    {
                        campaignCost.CampaignTypeId = Convert.ToInt16(ddlCampaignType.SelectedValue);
                    }
                    else
                    {
                        campaignCost.CampaignTypeId = -1;
                    }

                    campaignCost.Cost = Convert.ToDecimal(txtCostPerLead.Text);
                    campaignCost.Return = Convert.ToDecimal(txtReturnCampCost.Text);
                    campaignCost.Timer = Convert.ToInt32(txtTimer.Text);

                    Boolean res = false;

                    if (campaignCost.StartDate != rdStartDatePicker.SelectedDate || campaignCost.EndDate != rdEndDatePicker.SelectedDate)
                    {
                        res = TimePeriodOverlap(Convert.ToDateTime(rdStartDatePicker.SelectedDate), Convert.ToDateTime(rdEndDatePicker.SelectedDate));
                    }

                    if (!res)
                    {
                        lblErrorMsg.Visible = false;
                        Engine.CampaignCostActions.Change(campaignCost);

                        divAddUpdateForm.Visible = false;
                        BindCampaignCostGrid();
                        divCampaignCost.Visible = true;
                    }
                    else
                    {
                        lblErrorMsg.Visible = true;
                    }
                }
            }
            //}


        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }
    }

    //private User CurrentUserDetails
    //{
    //    get
    //    {
    //        try
    //        {
    //            Guid key = Guid.Parse(S.GetUser().ProviderUserKey.ToString());
    //            if (Engine.UserActions.Get(key) != null)
    //            {
    //                return Engine.UserActions.Get(key);
    //            }
    //            else
    //                return null;
    //        }
    //        catch (Exception)
    //        {
    //            return null;
    //        }

    //    }
    //}
    protected void btnCancelOnForm_Click(object sender, EventArgs e)
    {
        divGrid.Visible = true;
        divForm.Visible = false;
        divCampaignCost.Visible = false;
        divAddUpdateForm.Visible = false;

        tabDiv.Visible = false;
    }

    protected void btnCancelOnFormCampCost_Click(object sender, EventArgs e)
    {
        divCampaignCost.Visible = true;
        divAddUpdateForm.Visible = false;
    }

    protected void btnRetunToCampaignCost_Click(object sender, EventArgs e)
    {
        BindCampaignCostGrid();
        divAddUpdateForm.Visible = false;
        divCampaignCost.Visible = true;
    }

    protected void btnCancelBoth_Click(object sender, EventArgs e)
    {
        divAddUpdateForm.Visible = false;
        BindCampaignGrid();
        divGrid.Visible = true;
        divCampaignCost.Visible = false;

        btnRetunToCampaignCost.Visible = true;
        lblSuccessMsg.Visible = false;
        tabDiv.Visible = false;
    }
    protected void btnAddBoth_Click(object sender, EventArgs e)
    {

        if (hdnFieldIsEditMode.Value == "no")
        {
            CampaignCost campaigncost = new CampaignCost();

            campaigncost.CampaignId = Convert.ToInt32(hdnFieldEditCampaignKey.Value);
            campaigncost.Cost = Convert.ToDecimal(txtCostPerLead.Text);


            if (ddlCampaignType.Items.Count > 0 && ddlCampaignType.SelectedIndex != 0)
            {
                campaigncost.CampaignTypeId = Convert.ToInt16(ddlCampaignType.SelectedValue);
            }
            else
            {
                campaigncost.CampaignTypeId = -1;
            }

            campaigncost.Return = Convert.ToDecimal(txtReturnCampCost.Text);
            campaigncost.Timer = Convert.ToInt16(txtTimer.Text);

            campaigncost.StartDate = rdStartDatePicker.SelectedDate;
            campaigncost.EndDate = rdEndDatePicker.SelectedDate;

            Boolean res = TimePeriodOverlap(Convert.ToDateTime(rdStartDatePicker.SelectedDate), Convert.ToDateTime(rdEndDatePicker.SelectedDate));

            if (!res)
            {
                lblErrorMsg.Visible = false;
                Engine.CampaignCostActions.Add(campaigncost);

                divGrid.Visible = true;
                divForm.Visible = false;
                BindCampaignGrid();

                divAddUpdateForm.Visible = false;
                divCampaignCost.Visible = false;
                tabDiv.Visible = false;
            }
            else
            {
                lblMessageCampaignCostGrid.Visible = true;
            }
        }

        btnRetunToCampaignCost.Visible = true;
        lblSuccessMsg.Visible = false;

    }

    #endregion

    #region Data Events
    protected void BindCampaignGrid(bool customPageNumber = false)
    {
        try
        {
            var campaignTemplates = Engine.ManageCampaignActions.GetAll();
            var Records = (from T in campaignTemplates select new campaignDataItem { CampaignID = T.ID, CampaignTitle = T.Title, CampaignActive = T.IsActive }).AsQueryable();// CampaignCPL = String.Format("{0:f2}", T.CampaignCPL == null ? 0 : T.CampaignCPL),T.IsActive == true ? "Active" : "Inactive" }).AsQueryable();

            // var sorted = Helper.SortRecords(Records, PagingNavigationBar.SortBy, PagingNavigationBar.SortAscending);

            PagingNavigationBar.RecordCount = Records.Count();
            var sorted = (SortColumn == string.Empty) ? Records.OrderBy(x => x.CampaignID) : (SortAscending) ? Records.OrderBy(SortColumn) : Records.OrderBy(SortColumn + " desc");
            PagingNavigationBar.RecordCount = sorted.Count();


            grdCampaign.DataSource = null;
            grdCampaign.DataSource = sorted.Skip(PagingNavigationBar.SkipRecords).Take(PagingNavigationBar.PageSize);
            grdCampaign.DataBind();

        }
        catch (Exception ex)
        {
            lblMessageGrid.SetStatus("Error: " + ex.Message);
        }

    }

    public void Evt_Paging_Event(object sender, PagingEventArgs e)
    {
        // TODO: Implement this method
        BindCampaignGrid();
    }

    protected void BindCampaignCostGrid(bool customPageNumber = false)
    {
        try
        {
            var campaignTypeTemplate = Engine.ManageCampaignTypeActions.GetAll();

            int CampaignId = Convert.ToInt32(hdnFieldEditCampaignKey.Value);
            var campaignCostTemplates = Engine.CampaignCostActions.GetAll();


            var Records = (from T in campaignCostTemplates
                           join y in campaignTypeTemplate
                         on T.CampaignTypeId equals y.Id
                           where T.CampaignId == CampaignId
                           orderby T.StartDate, T.EndDate, T.CampaignTypeId, T.Cost
                           select new campaignCostDataItem { CampaignCostId = T.CampaignCostId, Type = T.CampaignTypeId, Cost = T.Cost, Return = T.Return, Timer = T.Timer, StartDate = T.StartDate, EndDate = T.EndDate, Text = y.Text }).AsQueryable();


            //var Records2 = (from T in Records.Where(x => x.CampaignId == CampaignId) select new { CampaignCostId = T.CampaignCostId, Type = T.CampaignTypeId, Cost = T.Cost, Return = T.Return, Timer = T.Timer, StartDate = T.StartDate, EndDate = T.EndDate }).AsQueryable();

            pgBarCampaignCost.RecordCount = Records.Count();
            var sorted = (SortColumnCC == string.Empty) ? Records : (SortAscendingCC) ? Records.OrderBy(SortColumnCC) : Records.OrderBy(SortColumnCC + " desc");
            pgBarCampaignCost.RecordCount = sorted.Count();

            grdCampaignCost.DataSourceID = null;
            var sorted2 = (SortColumnCC == string.Empty) ? Records : (SortAscendingCC) ? Records.OrderBy(SortColumnCC) : Records.OrderBy(SortColumnCC + " desc");
            grdCampaignCost.DataSource = sorted;
            grdCampaignCost.DataBind();

        }
        catch (Exception ex)
        {
            lblMessageCampaignCostGrid.Text = "Error: " + ex.Message;
        }

    }

    protected void BindCampaignType()
    {
        try
        {
            ddlCampaignType.DataSource = Engine.ManageCampaignTypeActions.GetAll();
            ddlCampaignType.DataBind();
        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }
    }

    protected void BindCompany()
    {
        try
        {
            ddlCompanyKey.DataSource = Engine.ManageCompanyActions.GetAll();
            ddlCompanyKey.DataValueField = "ID";
            ddlCompanyKey.DataTextField = "Title";
            ddlCompanyKey.DataBind();
        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }
    }



    #endregion

    #region Grid/Form Events

    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        int size = e.PageSize;
        size = size > 100 ? 100 : size;
        grdCampaign.PageSize = size;
        BindCampaignGrid();

    }

    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        //grdCampaign.PageIndex = e.PageNumber;
        BindCampaignGrid();
    }


    protected void Evt_CampaignCostPageSizeChanged(object sender, PagingEventArgs e)
    {
        int size = e.PageSize;
        size = size > 100 ? 100 : size;
        grdCampaign.PageSize = size;
        BindCampaignCostGrid();

    }

    protected void Evt_CampaignCostPageNumberChanged(object sender, PagingEventArgs e)
    {
        grdCampaignCost.PageIndex = e.PageNumber;
        BindCampaignCostGrid();
    }

    protected void ddlCampaignType_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlCampaignType.SelectedItem.Text.Equals("CPCall", StringComparison.CurrentCultureIgnoreCase))
        {
            lblTimer.Visible = true;
            txtTimer.Visible = true;
        }
        else
        {
            lblTimer.Visible = false;
            txtTimer.Visible = false;
            txtTimer.Text = "0.0";
        }
    }

    protected void grdCampaign_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            //grdCampaign.PageIndex = e.NewPageIndex;
            BindCampaignGrid();
        }
        catch (Exception ex)
        {
            lblMessageGrid.SetStatus(ex);
        }
    }

    protected void grdCampaignCost_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        try
        {
            grdCampaignCost.PageIndex = e.NewPageIndex;
            BindCampaignCostGrid();
        }
        catch (Exception ex)
        {
            lblMessageCampaignCostGrid.Text = "Error: " + ex.Message;
        }
    }

    protected void grdCampaign_Sorting(object sender, GridViewSortEventArgs e)
    {
        try
        {
            if (SortColumn == e.SortExpression)
                SortAscending = !SortAscending;
            else
            {
                SortColumn = e.SortExpression;
                SortAscending = true;
            }
            BindCampaignGrid();
        }
        catch (Exception ex)
        {
            lblMessageGrid.SetStatus(ex);
        }

    }

    protected void grdCampaignCost_Sorting(object sender, GridViewSortEventArgs e)
    {
        try
        {
            if (SortColumnCC == e.SortExpression)
                SortAscendingCC = !SortAscendingCC;
            else
            {
                SortColumnCC = e.SortExpression;
                SortAscendingCC = true;
            }
            BindCampaignCostGrid();
        }
        catch (Exception ex)
        {
            lblMessageCampaignCostGrid.Text = "Error: " + ex.Message;
        }

    }

    protected void grdCampaign_RowCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "EditX")
        {
            lblAddEditCampaign.Text = "Edit Campaign";
            hdnFieldIsEditMode.Value = "yes";
            divGrid.Visible = false;
            divForm.Visible = true;
            trCampaignID.Visible = true;
            lblCampaignID.Visible = true;
            txtCampaignID.Visible = true;
            btnDelete.Visible = true;

            tlCampaignStrip.Tabs[0].Selected = true;
            tlCampaignStrip.Tabs[0].Enabled = true;

            pgCampaignCost.Enabled = true;
            tabDiv.Visible = true;
            SetEditMode();
            tlCampaignStrip.Tabs[1].Enabled = true;
            //GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = e.CommandArgument.ToString();//grdCampaign.DataKeys[row.RowIndex].Value.ToString();
            hdnFieldEditCampaignKey.Value = dataKeyValue;
            Campaign nCampaign = Engine.ManageCampaignActions.Get(Convert.ToInt16(dataKeyValue));
            txtCampaignID.Text = nCampaign.ID.ToString();
            txtTitle.Text = nCampaign.Title;

            tlEditor.Content = nCampaign.Description;
            txtNote.Text = nCampaign.Notes;
            txtAlternateTitle.Text = nCampaign.AlternateTitle;
            txtArcMap.Text = nCampaign.ArcMap;

            //SZ [Dec 12, 2013] Added for Term Life
            chkDTE.Checked = nCampaign.HasDTE.GetValueOrDefault();
            ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
            ////fields from add/edit form as per customer requirement.
            /*try
            {
                ddlCampaignType.SelectedValue = nCampaign.CampaignTypeKey.ToString();
            }
            catch
            {
                ddlCampaignType.SelectedIndex = 0;
            }*/

            chkActive.Checked = nCampaign.IsActive == true ? true : false;

            ////QN [Jan 31, 2013] Commented the following code in order to remove Campaign Type and Costs 
            ////fields from add/edit form as per customer requirement.
            //txtCostPerLead.Text = String.Format("{0:f2}", nCampaign.CampaignCPL == null ? 0 : nCampaign.CampaignCPL);

            txtEmail.Text = nCampaign.email;

            txtOutpulseId.Text = nCampaign.OutpulseId;
            ddlOutpulseType.SelectedIndex = nCampaign.OutpulseType ?? 0;
            try
            {
                ddlCompanyKey.SelectedValue = (nCampaign.CompanyID ?? -1).ToString();
            }
            catch
            {
                ddlCompanyKey.SelectedIndex = 0;
            }

            BindCampaignCostGrid();
            divCampaignCost.Visible = true;
            divAddUpdateForm.Visible = false;
        }
        else if (e.CommandName == "DeleteX")
        {
            //GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            String dataKeyValue = e.CommandArgument.ToString();//grdCampaign.DataKeys[row.RowIndex].Value.ToString();


            Engine.ManageCampaignActions.Delete(Convert.ToInt16(dataKeyValue));
            Engine.CampaignCostActions.DeleteWhenParentCampaignDeleted(Convert.ToInt16(dataKeyValue));
            lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
            BindCampaignGrid();
        }
    }

    protected void grdCampaignCost_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditX1")
        {
            try
            {
                hdnFieldIsEditMode.Value = "yes";
                GridViewRow row2 = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
                int index = row2.RowIndex;

                String dataKeyValue = Convert.ToString(((Label)row2.FindControl("lblID")).Text);

                divCampaignCost.Visible = false;
                divAddUpdateForm.Visible = true;

                hdnFieldEditCampaignCostKey.Value = dataKeyValue;
                CampaignCost campaignCost = Engine.CampaignCostActions.Get(Convert.ToInt16(dataKeyValue));


                rdStartDatePicker.SelectedDate = Convert.ToDateTime(campaignCost.StartDate);
                rdEndDatePicker.SelectedDate = Convert.ToDateTime(campaignCost.EndDate);
                txtCostPerLead.Text = Convert.ToString(campaignCost.Cost);
                ddlCampaignType.SelectedValue = Convert.ToString(campaignCost.CampaignTypeId);
                txtReturnCampCost.Text = Convert.ToString(campaignCost.Return);
                txtTimer.Text = Convert.ToString(campaignCost.Timer);
                btnDeleteCampCost.Visible = true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }
        if (e.CommandName == "DeleteX")
        {
            GridViewRow row2 = (GridViewRow)((LinkButton)e.CommandSource).NamingContainer;
            int index = row2.RowIndex;

            String dataKeyValue = Convert.ToString(((Label)row2.FindControl("lblID")).Text);

            Engine.CampaignCostActions.Delete(Convert.ToInt16(dataKeyValue));
            lblMessageCampaignCostGrid.Text = "Record delete successful.";
            BindCampaignCostGrid();
        }
    }
    protected void grdCampaign_SortGrid(object sender, GridSortCommandEventArgs e)
    {
        //SortColumn = e.SortExpression;
        //SortAscending = e.NewSortOrder == GridSortOrder.Ascending;
        //PagingNavigationBar.PageNumber = 1;
        //BindCampaignGrid();

        try
        {
            if (SortColumn == e.SortExpression)
                SortAscending = !SortAscending;
            else
            {
                SortColumn = e.SortExpression;
                SortAscending = true;
            }
            BindCampaignGrid();
        }
        catch (Exception ex)
        {
            lblMessageGrid.SetStatus(ex);
        }
    }

    #endregion

    #endregion

    internal class campaignCostDataItem
    {
        public long CampaignCostId { get; set; }

        public int Type { get; set; }

        public decimal? Cost { get; set; }

        public decimal? Return { get; set; }

        public int? Timer { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string Text { get; set; }
    }

    internal class campaignDataItem
    {
        public int CampaignID { get; set; }
        public string CampaignTitle { get; set; }
        public bool? CampaignActive { get; set; }
    }

    internal class TimePeriodOverlapDataItem
    {
        public long CampaignCostId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

    }
    protected void grdCampaign_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
      if (e.Item is GridDataItem)
      {
          GridDataItem item = (GridDataItem)e.Item;
          if(!CurrentUser.Security.Administration.CanDelete)
          {
                    var lbl = e.Item.FindControl("lblMenuSep") as System.Web.UI.WebControls.Label;
                    if(lbl!=null)
                        lbl.Visible = false;
                    var lnk = e.Item.FindControl("lnkDelete") as System.Web.UI.WebControls.LinkButton;
                    if (lnk != null) 
                        lnk.Visible = false;
          }
      }
    }
    protected void grdCampaignCost_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var control = e.Row.FindControl("lnkDelete1") as LinkButton;
            if (control != null && !CurrentUser.Security.Administration.CanDelete)
                control.Visible = false;
        }
    }
}



