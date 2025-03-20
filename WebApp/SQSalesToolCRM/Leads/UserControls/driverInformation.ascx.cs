using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;

public partial class Leads_UserControls_driverInformation : AccountsBaseControl, IIndividualNotification
{
    public string DriverInfoRadWindowClientID { get { return dlgDriverInformation.ClientID; } }
    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
        {
            ddlIndividual.DataSource = handle.Individuals;
            ddlIndividual.DataBind();
        }

    }

    #region methods

    protected override void InnerLoad(bool bFirstTime)
    {
        dynamic d = Page.Master;
        if (d != null)
        {
            dynamic yesButton = d.buttonYes;
            if (yesButton != null)
            {
                var button = yesButton as Button;
                if (button != null) button.Click += (o, a) => ShowGrid();
            }
        }
        //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
        //if (masterPage != null)
        //    masterPage.buttonYes.Click += (o, a) => ShowGrid();//new EventHandler(CancelOnForm_Click);
        BindEvents();

        if (bFirstTime)
        {
            ShowGrid();
            BindGrid();
        }
    }
    protected override void InnerInit()
    {
        IsGridMode = true;
        DriverId = -1;
    }
    protected override void InnerSave()
    {
        SaveDriver();
        if (CloseForm)
            ShowGrid(CloseForm);
    }
    public override bool IsValidated
    {
        get
        {
            string errorMessage = "Error Required Field(s): ";
            vldLicenseNumber.Validate();
            rfvddlIndividual.Validate();
            if (!rfvddlIndividual.IsValid && !vldLicenseNumber.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + "Driver's Name, License Number"));
            else if(!rfvddlIndividual.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + "Driver's Name"));
            else if(!vldLicenseNumber.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + "License Number"));
            return (vldLicenseNumber.IsValid && rfvddlIndividual.IsValid);
        }
    }

    void ClearDriverInfo()
    {
        DriverId = 0;
        ddlIndividual.DataSource = (Page as IIndividual).Individuals;
        //GetIndividualsByAccount().Select(T => new
        //{
        //    T.Id,
        //    FullName = string.Format("{0} {1}", T.FirstName, T.LastName)
        //});

        ddlIndividual.DataBind();
        //[IH, 17-07-2013]
        ddlIndividual.Items.Insert(0, new ListItem(String.Empty, "-1"));
        ddlIndividual.SelectedIndex = 0;

        txtLicenseNumber.Text = string.Empty;
        txtLicenseState.Text = string.Empty;
        txtLicenseStatus.Text = string.Empty;
        txtMaritalStatus.Text = string.Empty;
        txtAgeLicensed.Value = null;
        txtYearsAtResidence.Value = null;
        txtOccupation.Text = string.Empty;
        txtYearsWithCompany.Value = null;
        txtYearsInField.Value = null;
        txtEducation.Text = string.Empty;
        txtTickets.Text = string.Empty;
        txtNumberOfIncidents.Value = null;
        txtIncidentType.Text = string.Empty;
        txtIncidentDescription.Text = string.Empty;
        rdpLastIncidentDate.SelectedDate = null;
        txtClaimPaidAmount.Value = null;
        txtSR22.Text = string.Empty;
        txtPolicyYears.Value = null;

        ctlStatus.Clear();
    }
    void SetDriverInfo(DriverInfo entity)
    {
        if (entity.IndividualId.HasValue && ddlIndividual.Items.Count > 0)
            ddlIndividual.SelectedValue = entity.IndividualId.ToString();

        txtLicenseNumber.Text = entity.LisenceNumber;
        txtLicenseState.Text = entity.DlState;
        txtLicenseStatus.Text = entity.LicenseStatus;
        txtMaritalStatus.Text = entity.MaritalStatus;
        txtAgeLicensed.Value = entity.AgeLicensed;
        txtYearsAtResidence.Value = entity.YearsAtResidence;
        txtOccupation.Text = entity.Occupation;
        txtYearsWithCompany.Value = entity.YearsWithCompany;
        txtYearsInField.Value = entity.YrsInField;
        txtEducation.Text = entity.Education;
        txtTickets.Text = entity.TicketsAccidentsClaims;
        txtNumberOfIncidents.Value = entity.NmbrIncidents;
        txtIncidentType.Text = entity.IncidentType;
        txtIncidentDescription.Text = entity.IncidentDescription;
        rdpLastIncidentDate.SelectedDate = entity.IncidentDate;
        txtClaimPaidAmount.Text = entity.ClaimPaidAmount.HasValue ? entity.ClaimPaidAmount.ToString() : "";
        txtSR22.Text = entity.St22;
        txtPolicyYears.Value = entity.PolicyYears;
    }
    void GetDriverInfo(ref DriverInfo entity)
    {
        entity.LisenceNumber = txtLicenseNumber.Text;
        entity.DlState = txtLicenseState.Text;
        entity.LicenseStatus = txtLicenseStatus.Text;
        entity.MaritalStatus = txtMaritalStatus.Text;
        entity.AgeLicensed = Helper.SafeConvert<long>(txtAgeLicensed.Text);
        entity.YearsAtResidence = Helper.SafeConvert<long>(txtYearsAtResidence.Text);
        entity.Occupation = txtOccupation.Text;
        entity.YearsWithCompany = Helper.SafeConvert<long>(txtYearsWithCompany.Text);
        entity.YrsInField = Helper.SafeConvert<long>(txtYearsInField.Text);
        entity.Education = txtEducation.Text;
        entity.TicketsAccidentsClaims = txtTickets.Text;
        entity.NmbrIncidents = Helper.SafeConvert<long>(txtNumberOfIncidents.Text);
        entity.IncidentType = txtIncidentType.Text;
        entity.IncidentDescription = txtIncidentDescription.Text;
        entity.IncidentDate = rdpLastIncidentDate.SelectedDate;
        entity.ClaimPaidAmount = Helper.SafeConvert<decimal>(txtClaimPaidAmount.Text);
        entity.St22 = txtSR22.Text;
        entity.PolicyYears = Helper.SafeConvert<long>(txtPolicyYears.Text);
    }

    void AddDriver()
    {
        ClearDriverInfo();
        ShowGrid(false);
        //ChangeView(ViewMode.AddNew);
    }
    void EditDriver(long Id)
    {
        ClearDriverInfo();
        DriverId = Id;
        SetDriverInfo(Engine.DriverActions.Get(Id));
    }
    void SaveDriver()
    {
        try
        {
            DriverInfo DI = null;
            if (IsNewRecord)
            {
                DI = new DriverInfo
                {
                    AccountId = AccountID,
                    IndividualId = Helper.SafeConvert<long>(ddlIndividual.SelectedValue),
                    AddedBy = SalesPage.CurrentUser.FullName
                };
            }
            else
            {
                DI = Engine.DriverActions.Get(DriverId);
                DI.ChangedBy = SalesPage.CurrentUser.FullName;
            }

            GetDriverInfo(ref DI);

            if (IsNewRecord)
            {
                Engine.DriverActions.Add(DI);
                DriverId = DI.Key;
                ddlIndividual.Enabled = false;
            }
            else
                Engine.DriverActions.Change(DI);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;
        // divGrid.Visible = bShow;
        //  divForm.Visible = !bShow;
        if (bShow)
        {
            dlgDriverInformation.Dispose();
            dlgDriverInformation.VisibleOnPageLoad = false;
            dlgDriverInformation.Visible = false;
            DriverId = 0;
        }
        else
        {
            dlgDriverInformation.VisibleOnPageLoad = true;
            dlgDriverInformation.Visible = true;
            dlgDriverInformation.CenterIfModal = true;
        }
        BindGrid();
    }
    void BindGrid()
    {
        try
        {
            var records = Engine.DriverActions.GetAllByAccountID(this.AccountID).Select(T => new
            {
                Id = T.Key,
                T.IndividualId,
                T.Individual.FirstName,
                T.Individual.LastName,
                T.LicenseStatus,
                T.LisenceNumber,
                T.AgeLicensed,

            }).AsQueryable();

            grdHome.DataSource = ctlPager.ApplyPaging(Helper.SortRecords(records, ctlPager.SortBy, ctlPager.SortAscending));
            grdHome.DataBind();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void DeleteDriver(long id)
    {
        try
        {
            Engine.DriverActions.Delete(id);
            BindGrid();
            //SR ctlStatus.SetStatus(Messages.RecordDeletedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }

    }
    void BindEvents()
    {
        if (Page is IIndividual)
            (Page as IIndividual).Notify(this);


        btnAddNew.Click += (o, a) => AddDriver();

        btnReturn.Click += (o, a) => ShowGrid();
        btnCancelOnForm.Click += (o, a) => ShowGrid();
        btnSaveAndCloseOnForm.Click += (o, a) =>
        {
            if (IsValidated)
            {
                SaveDriver(); ShowGrid();
            }
        };

        btnSaveOnForm.Click += (o, a) =>
        {
            if (IsValidated)
                SaveDriver();
        };

        grdHome.RowCommand += (o, arg) => CommandRouter(
            arg.CommandName,
            Helper.SafeConvert<long>(grdHome.DataKeys[((arg.CommandSource as Control).NamingContainer as GridViewRow).RowIndex].Value.ToString())
            );
        grdHome.Sorting += (o, a) => Sort(a.SortExpression);

        ctlPager.SizeChanged += (o, a) => BindGrid();
        ctlPager.IndexChanged += (o, a) => BindGrid();
    }
    protected void CommandRouter(string command, long id)
    {
        switch (command)
        {
            case "EditX":
                ShowGrid(false);
                EditDriver(id);
                break;
            case "ViewX":
                ShowGrid(false);
                EditDriver(id);
                ReadOnly = true;
                break;
            case "DeleteX":
                DeleteDriver(id);
                break;
        }
    }
    bool IsNewRecord
    {
        get
        {
            //bool bAns = false;
            return DriverId < 1;
            //bool.TryParse(hdnFieldIsEditMode.Value, out bAns);
            //return bAns;
        }
        //set
        //{
        //    hdnFieldIsEditMode.Value = value.ToString();
        //}
    }
    long DriverId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnFieldEditForm.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldEditForm.Value = value.ToString();
        }
    }

    #endregion

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddNew.Visible = bEnable;
            var colEdit = grdHome.Columns[grdHome.Columns.Count - 2];
            var colView = grdHome.Columns[grdHome.Columns.Count - 1];
            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
        {
            EnableControls(tblControls, bEnable);
        }
    }
    void Sort(string column)
    {
        //try
        //{
        if (ctlPager.SortBy == column)
            ctlPager.SortAscending = !ctlPager.SortAscending;
        else
        {
            ctlPager.SortBy = column;
            ctlPager.SortAscending = true;
        }

        BindGrid();
        //}
        //catch (Exception ex)
        //{
        //    lblMessageGrid.Text = "Error: " + ex.Message;
        //}
    }


    //protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    //{
    //    //int size = e.PageSize;
    //    //size = size > 100 ? 100 : size;
    //    //grdHome.PageSize = size;
    //    BindGrid();
    //}
    //protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    //{
    //    //grdHome.PageIndex = e.PageNumber;
    //    BindGrid();
    //}
    //protected void grdHome_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    try
    //    {
    //        grdHome.PageIndex = e.NewPageIndex;
    //        BindGrid();
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex);
    //    }
    //}
    ////public long AccountID
    ////{
    ////    get
    ////    {
    ////        return base.SalesPage.AccountID;
    ////    }
    ////}

    //#endregion
    //private void GridView()
    //{
    //    ChangeView();
    //}
    //private enum ViewMode { GridView = 0, AddNew = 1, Edit = 2 }

    //private void ChangeView(ViewMode viewMode = ViewMode.GridView)
    //{
    //    ShowGrid(viewMode == ViewMode.GridView);
    //    //IsGridMode = viewMode == ViewMode.GridView;
    //    //divGrid.Visible = isGridviewMode == ViewMode.GridView;
    //    //divForm.Visible = viewMode != ViewMode.GridView;
    //    //IsGridMode = viewMode == ViewMode.GridView;
    //    if (viewMode == 0)
    //    {
    //        BindGrid();

    //        return;
    //    }

    //    //ddlIndividual.Enabled = viewMode == ViewMode.AddNew;

    //    //if (viewMode == ViewMode.AddNew)
    //    //{
    //    //    hdnFieldIsEditMode.Value = "no";
    //    //    hdnFieldEditForm.Value = "";
    //    //}
    //    //else
    //    //{
    //    //    hdnFieldIsEditMode.Value = "yes";
    //    //}

    //    ClearFields();
    //}

    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
    //interface ILeadControlSave, in the accounts base page
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        return !IsGridMode;
    //    }
    //}

    //protected void btnDelete_Click(object sender, EventArgs e)
    //{
    //    //try
    //    //{
    //    //    Engine.DriverActions.Delete(Convert.ToInt64(hdnFieldEditForm.Value));
    //    //    BindGrid();

    //    //    //divGrid.Visible = true;
    //    //    //divForm.Visible = false;

    //    //    lblMessageGrid.Text = Messages.RecordDeletedSuccess;
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    lblMessageForm.Text = "Error: " + ex.Message;
    //    //}
    //}

    //protected void BindIndividuals()
    //{
    //    //try
    //    //{
    //        //ddlIndividual.DataSource = Engine.IndividualsActions.GetByAccountID(this.AccountID).Select(T => new { ID = T.Id, FullName = T.FirstName + " " + T.LastName });
    //        //ddlIndividual.DataSource = GetIndividualsByAccount().Select(T => new { ID = T.Id, FullName = T.FirstName + " " + T.LastName });
    //        //ddlIndividual.DataValueField = "ID";
    //        //ddlIndividual.DataTextField = "FullName";
    //        //ddlIndividual.DataBind();
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    lblMessageForm.Text = "Error: " + ex.Message;
    //    //}
    //}
    //private void SaveForm(bool closeForm = false)
    //{
    //    InnerSave();
    //    //try
    //    //{
    //    //    if (hdnFieldIsEditMode.Value == "no")
    //    //    {
    //    //        var entity = new DriverInfo();

    //    //        GetDriverInfo(entity, ViewMode.AddNew);

    //    //        Engine.DriverActions.Add(entity);

    //    //        hdnFieldEditForm.Value = entity.Key.ToString();
    //    //        hdnFieldIsEditMode.Value = "yes";
    //    //        ddlIndividual.Enabled = false;
    //    //    }

    //    //    else if (hdnFieldIsEditMode.Value == "yes")
    //    //    {
    //    //        if (hdnFieldEditForm.Value != "")
    //    //        {
    //    //            var entity = Engine.DriverActions.Get(Convert.ToInt64(hdnFieldEditForm.Value));

    //    //            GetDriverInfo(entity, ViewMode.Edit);

    //    //            Engine.DriverActions.Change(entity);
    //    //        }
    //    //    }

    //        if (!closeForm)
    //        {
    //            lblMessageForm.Text = Messages.RecordSavedSuccess;
    //            lblMessageGrid.Text = "";
    //        }
    //        else
    //        {
    //            ChangeView();

    //            lblMessageForm.Text = "";
    //            lblMessageGrid.Text = Messages.RecordSavedSuccess;
    //        }
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    lblMessageForm.Text = "Error: " + ex.Message;
    //    //}
    //}

    //private string getSortDirectionString(SortDirection sortDirection)
    //{
    //    string newSortDirection = String.Empty;

    //    switch (sortDirection)
    //    {
    //        case SortDirection.Ascending:
    //            newSortDirection = "ASC";
    //            break;

    //        case SortDirection.Descending:
    //            newSortDirection = "DESC";
    //            break;
    //    }

    //    return newSortDirection;
    //}
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
    //    //if (masterPage != null)
    //    //    masterPage.buttonYes.Click += (o, a) => ShowGrid();//new EventHandler(CancelOnForm_Click);

    //    //BindEvents();
    //}



    //protected void Save_Click(object sender, EventArgs e)
    //{
    //    if (IsValidated)
    //        InnerSave();
    //}
    //protected void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    if (IsValidated)
    //    {
    //        Save();
    //        ShowGrid();
    //    }
    //    //SaveForm(true);
    //}
    //protected void AddNew_Click(object sender, EventArgs e)
    //{
    //    AddDriver();
    //}
    //protected void btnReturn_Click(object sender, EventArgs e)
    //{
    //    //GridView();
    //    ShowGrid();
    //}
    //protected void CancelOnForm_Click(object sender, EventArgs e)
    //{
    //    //GridView();
    //    ShowGrid();
    //}
    protected void grdHome_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DisableDeleteInGridView(e.Row, "lnkDelete");
    }
}