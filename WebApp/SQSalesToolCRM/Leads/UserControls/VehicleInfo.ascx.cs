using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using Telerik.Web.UI;

public partial class UserControls_VehicleInfo : AccountsBaseControl, IIndividualNotification
{


    public string VehicleInfoRadWindowClientID
    { get { return dlgVehicleInfo.ClientID; } }

    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
        {
            ddlIndividuals.DataSource = handle.Individuals;
            ddlIndividuals.DataBind();
        }
    }

    enum PageDisplayMode
    {
        GridList = 1,
        ListDetail = 2
    }

    public Int64 EditKey
    {
        get
        {
            Int64 Id = 0;
            Int64.TryParse(hdnFieldEditTemplateKey.Value, out Id);
            return Id;
        }
        set
        {
            hdnFieldEditTemplateKey.Value = value.ToString();
        }
    }

    //public long AccountID
    //{
    //    get
    //    {
    //        return base.SalesPage.AccountID;
    //    }
    //}

    //public int totalGridRecords = 0;


    private PageDisplayMode PageMode
    {
        set
        {
            pnlDetail.Visible = false;
            pnlGrid.Visible = false;
            dlgVehicleInfo.Dispose();
            dlgVehicleInfo.VisibleOnPageLoad = false;
            dlgVehicleInfo.Visible = false;
            IsGridMode = value == PageDisplayMode.GridList;
            switch (value)
            {
                case PageDisplayMode.GridList:
                    InitializeGrid();
                    pnlGrid.Visible = true;
                    break;
                case PageDisplayMode.ListDetail:
                    InitializeDetail();
                    pnlGrid.Visible = true;
                    pnlDetail.Visible = true;
                    dlgVehicleInfo.VisibleOnPageLoad = true;
                    dlgVehicleInfo.Visible = true;
                    dlgVehicleInfo.CenterIfModal = true;
                    break;
            }
        }
    }

    private void InitializeDetail()
    {
        //ddlIndividuals.DataSource = Engine.IndividualsActions.GetByAccountID(AccountID).Select(x=> new {FullName = x.FirstName + " " + x.LastName, Key =x.Id});
        //ddlIndividuals.DataSource = GetIndividualsByAccount().Select(x => new { FullName = x.FirstName + " " + x.LastName, Key = x.Id });

        ddlIndividuals.DataSource = (Page as IIndividual).Individuals; // GetIndividualsByAccount().Select(x => new { FullName = x.FirstName + " " + x.LastName, Key = x.Id });
        ddlIndividuals.DataBind();
        //[IH, 17-07-2013]
        ddlIndividuals.Items.Insert(0, new ListItem(String.Empty, "-1"));
        ddlIndividuals.SelectedIndex = 0;


        txtYear.Text = "";
        txtMake.Text = "";
        txtModel.Text = "";
        txtSubModel.Text = "";
        txtAnnualMileage.Text = "";
        txtCollisionDeductible.Text = "";
        txtComprehensiveDeductible.Text = "";
        txtParkingLocation.Text = "";
        txtPrimaryUse.Text = "";
        txtSecuritySystem.Text = "";
        txtWeeklyCommuteDays.Text = "";
        //rdBtnlstFilterSelection.SelectedIndex = 0;
        //rdBtnlstFilterSelection_SelectedIndexChanged(this, null);


    }
    private void InitializeGrid()
    {
        //QN [April 1st, 2013]
        //The below commented code was throwing exception when T.Indivisual is null
        /////var records = Helper.SortRecords(Engine.VehiclesActions.GetByAccount(AccountID).Select(T=>new { FullName= T.Individual.FirstName + " " + T.Individual.LastName, T.Key, T.Make  , T.Model, T.Year, T.AnnualMileage, T.CollisionDeductable, T.ComprehensiveDeductable, T.IsActive }).AsQueryable(),"Key",false);

        //var records = Engine.VehiclesActions.GetByAccount(AccountID).Select(T => new { FullName = ((T.Individual == null) ? string.Empty : ((T.Individual.FirstName == null) ? string.Empty : T.Individual.FirstName)) +" "+ ((T.Individual == null) ? string.Empty : ((T.Individual.LastName == null) ? string.Empty : T.Individual.LastName)), T.Key, T.Make, T.Model, T.Year, T.AnnualMileage, T.CollisionDeductable, T.ComprehensiveDeductable, T.IsActive }).AsQueryable();
        //var records = Engine.VehiclesActions.GetByAccount(AccountID).Select(T => new { 
        //    FullName = string.Format("{0} {1}", T.Individual==null? "": (T.Individual.FirstName?? ""), T.Individual==null? "":T.Individual.LastName??""), 
        //    T.Key, 
        //    T.Make, 
        //    T.Model, 
        //    T.Year, 
        //    T.AnnualMileage, 
        //    T.CollisionDeductable, 
        //    T.ComprehensiveDeductable, 
        //    T.IsActive 
        //}).AsQueryable();

        //totalGridRecords = records.Count();

        grdVehicles.DataSource = PagingNavigationBar.ApplyPaging(Engine.VehiclesActions.GetVehicles(AccountID)); // PagingNavigationBar.ApplyPaging(records); 
        grdVehicles.DataBind();

        //EditKey = 0;
    }


    private void Edit(Int64 recordId)
    {
        Vehicle nVehicle = Engine.VehiclesActions.Get(recordId);
        txtYear.Text = nVehicle.Year.ToString();
        txtMake.Text = nVehicle.Make;
        txtModel.Text = nVehicle.Model;
        txtSubModel.Text = nVehicle.Submodel;
        txtAnnualMileage.Text = nVehicle.AnnualMileage.ToString();
        txtCollisionDeductible.Text = nVehicle.CollisionDeductable;
        txtComprehensiveDeductible.Text = nVehicle.ComprehensiveDeductable;
        txtParkingLocation.Text = nVehicle.WhereParked;
        txtPrimaryUse.Text = nVehicle.PrimaryUse;
        txtSecuritySystem.Text = nVehicle.SecuritySystem;
        txtWeeklyCommuteDays.Text = nVehicle.WeeklyCommuteDays.ToString();
        ddlIndividuals.SelectedValue = (nVehicle.Individualkey > 0) ? nVehicle.Individualkey.ToString() : null;

    }
    private void SaveRecord(bool convertToEditMode = false)
    {

        // if (EditKey == 0) 
        if (hdnFieldIsEditMode.Value == "no")
        {

            Vehicle nVehicle = new Vehicle();
            nVehicle = SetEntityValues(nVehicle);
            var recordAdded = Engine.VehiclesActions.Add(nVehicle, base.SalesPage.CurrentUser.FullName);

            if (convertToEditMode)
            {
                EditKey = recordAdded.Key;
                ddlIndividuals.Enabled = false;
            }

            hdnFieldIsEditMode.Value = "yes";
        }
        else
        {
            Vehicle nVehicle = Engine.VehiclesActions.Get(Convert.ToInt64(hdnFieldVehicleId.Value));
            nVehicle = SetEntityValues(nVehicle);

            Engine.VehiclesActions.Change(nVehicle, base.SalesPage.CurrentUser.FullName);
        }

        //SR  ctlStatus.SetStatus("Record saved successful.");        
    }

    private Vehicle SetEntityValues(Vehicle nVehicle)
    {
        if (txtYear.Text != "")
            nVehicle.Year = Convert.ToInt64(txtYear.Text);
        nVehicle.Make = txtMake.Text;
        nVehicle.Model = txtModel.Text;
        nVehicle.Submodel = txtSubModel.Text;
        if (txtAnnualMileage.Text != "")
            nVehicle.AnnualMileage = Convert.ToInt64(txtAnnualMileage.Text);
        nVehicle.CollisionDeductable = txtCollisionDeductible.Text;
        nVehicle.ComprehensiveDeductable = txtComprehensiveDeductible.Text;
        nVehicle.WhereParked = txtParkingLocation.Text;
        nVehicle.PrimaryUse = txtPrimaryUse.Text;
        nVehicle.SecuritySystem = txtSecuritySystem.Text;
        if (txtWeeklyCommuteDays.Text != "")
            nVehicle.WeeklyCommuteDays = Convert.ToInt64(txtWeeklyCommuteDays.Text);
        nVehicle.Individualkey = GetIndividual();
        nVehicle.AccountId = AccountID;
        return nVehicle;
    }
    private void Close()
    {
        PageMode = PageDisplayMode.GridList;
    }

    private Int64 GetIndividual()
    {
        Int64 individualKey = 0;
        Int64.TryParse(ddlIndividuals.SelectedValue, out individualKey);
        return individualKey;
    }



    void BindEvents()
    {
        if (Page is IIndividual)
            (Page as IIndividual).Notify(this);

    }

    #region Events

    //protected override void Page_Initialize(object sender, EventArgs args)
    //{
    //    if (!IsPostBack)
    //    {
    //        InitializeGrid();
    //        lblMessageForm.SetStatus("");
    //        lblMessageGrid.SetStatus("");            
    //    }
    //}
    protected override void InnerInit()
    {
        IsGridMode = true;
        //InitializeManageFilter();
        //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
        //if (masterPage != null)
        //    masterPage.buttonYes.Click += new EventHandler(btnCancelOnForm_Click);
        dynamic d = Page.Master;
        if (d != null)
        {
            dynamic yesButton = d.buttonYes;
            if (yesButton != null)
            {
                var button = yesButton as Button;
                if (button != null) button.Click += new EventHandler(btnCancelOnForm_Click);
            }
        }
        ctlStatus.SetStatus("");
        ctlStatus.SetStatus("");
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        BindEvents();

        if (IsGridMode)
        {
            InitializeGrid();
        }
    }
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
    //    //if (masterPage != null)
    //    //    masterPage.buttonYes.Click += new EventHandler(btnCancelOnForm_Click);

    //    //if (!IsPostBack)
    //    //{
    //    //    InitializeGrid();
    //    //}
    //    //lblMessageForm.SetStatus("");
    //    //lblMessageGrid.SetStatus("");
    //}
    protected void btnAddNewQueue_Click(object sender, EventArgs e)
    {
        ddlIndividuals.Enabled = true;
        PageMode = PageDisplayMode.ListDetail;
        hdnFieldIsEditMode.Value = "no";
        ctlStatus.SetStatus("");
    }

    protected void btnApply_Click(object sender, EventArgs e)
    {
        try
        {
            if (IsValidated)
            {
                SaveRecord(true);
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            if (IsValidated)
            {
                SaveRecord();
                Close();
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    // must be public to access in the parent form
    public void btnCancelOnForm_Click(object sender, EventArgs e)
    {
        Close();
    }


    protected void grdVehicles_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        Int64 id = 0;
        Int64.TryParse(e.CommandArgument.ToString(), out id);
        EditKey = id;

        switch (e.CommandName)
        {
            case "EditX":
                ddlIndividuals.Enabled = false;
                hdnFieldIsEditMode.Value = "yes";
                PageMode = PageDisplayMode.ListDetail;
                hdnFieldVehicleId.Value = EditKey.ToString();
                ctlStatus.SetStatus("");
                Edit(EditKey);
                break;
            case "ViewX":
                ddlIndividuals.Enabled = false;
                hdnFieldIsEditMode.Value = "yes";
                PageMode = PageDisplayMode.ListDetail;
                hdnFieldVehicleId.Value = EditKey.ToString();
                ctlStatus.SetStatus("");
                Edit(EditKey);
                ReadOnly = true;
                break;
            case "EnabledX":
                Engine.VehiclesActions.ToggleActivation(EditKey);
                //SR  ctlStatus.SetStatus("Record update successful.");
                InitializeGrid();
                break;
            case "DeleteX":
                Engine.VehiclesActions.Delete(EditKey);
                //SR ctlStatus.SetStatus("Record delete successful.");
                InitializeGrid();
                break;
        }
    }

    public void PagingBar_Event(object sender, PagingEventArgs e)
    {
        InitializeGrid();
    }

    #endregion
    //bool IsGridMode
    //{
    //    get
    //    {
    //        bool bAns = false;
    //        bool.TryParse(hdnGridMode.Value, out bAns);
    //        return bAns;
    //    }
    //    set
    //    {
    //        hdnGridMode.Value = value.ToString();
    //    }
    //}
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        
    //        return !IsGridMode;
    //    }
    //}

    public override bool IsValidated
    {
        get
        {
            rfvddlIndividuals.Validate();
            vldYear.Validate();
            string errorMessage = "Error Required Field(s): ";
            if (!rfvddlIndividuals.IsValid && !vldYear.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + "Individual, Year"));
            else if (!rfvddlIndividuals.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + "Individual"));
            else if (!vldYear.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + "Year"));
            return (rfvddlIndividuals.IsValid && vldYear.IsValid);
        }
    }

    protected override void InnerSave()
    {
        SaveRecord();
        if (CloseForm)
            PageMode = PageDisplayMode.GridList;

    }

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddNewQueue.Visible = bEnable;
            var colEdit = grdVehicles.Columns.FindByUniqueName("colEdit");
            var colView = grdVehicles.Columns.FindByUniqueName("colView");

            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
            EnableControls(tblControls, bEnable);
    }
    protected void grdVehicles_ItemDataBound(object sender, GridItemEventArgs e)
    {
        DisableDeleteInRadGrid(e.Item, "lnkDelete");
    }
}