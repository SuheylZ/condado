using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess;
using System.Data;
using SalesTool.DataAccess.Models;

public partial class Leads_UserControls_HomeInformation : AccountsBaseControl, IIndividualNotification
{

    /*
     * SZ [may 9, 2013] I have not written seperate comments. as 
     * A. I didnt have much time and there were many controls I had to work on
     * B. The work on the leads control Im doing is almost similiar, in some cases identical so it does not make much sense to 
     *    give same comments again and again
     * 
     * Basically what I have been doing to perform optimization and this applies to all user controls in leads:
     * 
     * 1. Make use of InnerInit(), InnerLoad(), InnerSave() & IsValidated. they are optimized & they are can be called explicitly (Initialize, Refresh, Save, IsValidated)
     * 
     * 2. Remove useless/misued HiddenFields. There is just 1 hiddenflied required which is for holding record id. wrap it in property
     * 
     * 3. Reduce function clutter by making use of Dynamic Event binding. I use Lambda coz they are so cool but it depends on programmer
     * 
     * 4. Try eliminating try-catch. coz they are performance kill. use them when making a DB call only.
     * 
     * 5. for paging, sorting make use of Paging bar, dont use default paging fucntionality
     * 
     * 6. look in the base classes for any properties/functions that are duplicated. remove the duplicate and use the base class property/function.
     * 
     * As long as these simple rules are followed, control will be automatically optimized.
     */


    //#region Members/Properties

    ////private string SortColumn
    ////{
    ////    get
    ////    {
    ////        return hdSortColumn.Value.Trim();
    ////    }
    ////    set
    ////    {
    ////        hdSortColumn.Value = value.Trim();
    ////    }
    ////}
    ////private bool SortAscending
    ////{
    ////    get
    ////    {
    ////        bool bAsc = false;
    ////        bool.TryParse(hdSortDirection.Value, out bAsc);
    ////        return bAsc;
    ////    }
    ////    set
    ////    {
    ////        hdSortDirection.Value = value.ToString();
    ////    }
    ////}

    //// SZ [Jan 23, 2013] base class already implements it, not required anymore
    ////public long AccountID
    ////{
    ////    get
    ////    {
    ////        return base.SalesPage.AccountID;
    ////    }
    ////}

    //#endregion
    //ChangeView(ViewMode.Edit);

    //GridViewRow row = ;
    //string dataKeyValue = grdHome.DataKeys[row.RowIndex].Value.ToString();
    //hdnFieldEditForm.Value = dataKeyValue;
    // HomeData = Engine.HomeActions.Get(Id);

    //if (ddlIndividual.Items.Count > 0)
    //    ddlIndividual.SelectedValue = entity.Individualkey.ToString();

    //txtYear.Text = entity.YearBuilt;
    //txtSquareFootage.Text = Helper.ConvertToString(entity.SqFootage);
    //txtDwellingType.Text = entity.DwellingType;
    //txtDesignType.Text = entity.DesignType;
    //txtRoofAge.Text = Helper.ConvertToString(entity.RoofAge);
    //txtRoofType.Text = entity.RoofType;
    //txtFoundationType.Text = entity.FoundationType;
    //txtHeatingType.Text = entity.HeatingType;
    //txtExteriorWallType.Text = entity.ExteriorWallType;
    //txtNumberOfHomeClaims.Text = Helper.ConvertToString(entity.NumberOfClaims);
    //txtNumberOfBedrooms.Text = Helper.ConvertToString(entity.NumberOfBedrooms);
    //txtNumberOfBathrooms.Text = Helper.ConvertToString(entity.NumberOfBathrooms);
    //txtRequestedCoverage.Text = entity.ReqCoverage;
    ////txtMonthlyPremium.Text = Helper.ConvertToString(entity.MonthlyPremium);

    ////SZ [Apr 5, 2013] for the bug 50 in mentis
    //txtHomeAddress1.Text = entity.Address1;
    //txtHomeAddress2.Text = entity.Address2;
    //txtHomeCity.Text = entity.City;
    //txtHomeZip.Text = entity.ZipCode;
    //if (entity.StateId.HasValue)
    //    ddlHomeStates.SelectedValue = entity.StateId.Value.ToString();

    //if (ddlCurrentCarrier.Items.Count > 0)
    //{
    //    ddlCurrentCarrier.SelectedValue = entity.CurrentCarrier;
    //}
    //    }
    //    else if (e.CommandName ==)
    //    {
    //        //GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
    //        //string dataKeyValue = grdHome.DataKeys[row.RowIndex].Value.ToString();

    //        //lblMessageForm.Text = "";
    //        //lblMessageGrid.Text = "";
    //        ctlStatus.Clear();

    //        Engine.HomeActions.Delete(Id);
    //        ctlStatus.SetStatus(Messages.RecordDeletedSuccess); //lblMessageGrid.Text = ;
    //        BindGrid();
    //    }
    //}
    //protected void BindIndividuals()
    //{
    //    try
    //    {
    //        //ddlIndividual.DataSource = Engine.IndividualsActions.GetByAccountID(this.AccountID).Select(T => new { ID = T.Id, FullName = T.FirstName + " " + T.LastName });
    //        ddlIndividual.DataSource = GetIndividualsByAccount(); //.Select(T => new { ID = T.Id, FullName = T.FirstName + " " + T.LastName });
    //        //ddlIndividual.DataValueField = "ID";
    //        //ddlIndividual.DataTextField = "FullName";
    //        ddlIndividual.DataBind();
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex);
    //        //lblMessageForm.Text = "Error: " + ex.Message;
    //    }
    //}

    //protected void BindCarriers(DropDownList ddl)
    //{
    //    try
    //    {
    //        ddl.DataSource = Engine.CarrierActions.GetAll().Where(x => x.IsHome==true).Select(T => new { ID = T.Key, T.Name });
    //        //ddl.DataValueField = "ID";
    //        //ddl.DataTextField = "Name";
    //        ddl.DataBind();
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex); //lblMessageForm.Text = "Error: " + ex.Message;
    //    }
    //}

    //protected void BindCurrentCarriers()
    //{
    //    BindCarriers(ddlCurrentCarrier);
    //}
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
    //    //if (masterPage != null)
    //    //{
    //    //    masterPage.buttonYes.Click += new EventHandler(CancelOnForm_Click);
    //    //}

    //    //if (!Page.IsPostBack)
    //    //{
    //    //    BindGrid();
    //    //    divGrid.Visible = true;
    //    //    divForm.Visible = false;

    //    //    BindIndividuals();
    //    //    BindCurrentCarriers();
    //    //}
    //}
    //protected void Save_Click(object sender, EventArgs e)
    //{
    //    SaveHome();
    //}
    //protected void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    SaveHome();
    //    ShowGrid();
    //}
    //protected void AddNew_Click(object sender, EventArgs e)
    //{
    //    AddHome();
    //}
    //protected void btnReturn_Click(object sender, EventArgs e)
    //{
    //    ShowGrid();
    //    //GridView();
    //}
    //// must be public to access in the parent form
    //public void CancelOnForm_Click(object sender, EventArgs e)
    //{
    //    ShowGrid();
    //    //GridView();
    //}
    //private void GridView()
    //{
    //    ShowGrid(false);
    //    //ChangeView();
    //}
    //private enum ViewMode { GridView = 0, AddNew = 1, Edit = 2 }
    //private void ChangeView(ViewMode viewMode = ViewMode.GridView)
    //{
    //    divGrid.Visible = viewMode == ViewMode.GridView;
    //    divForm.Visible = viewMode != ViewMode.GridView;

    //    if (viewMode == 0)
    //    {
    //        BindGrid();
    //        IsGridMode = true;
    //        return;
    //    }
    //    else
    //        IsGridMode = false;

    //    ddlIndividual.Enabled = viewMode == ViewMode.AddNew;

    //    if (viewMode == ViewMode.AddNew)
    //    {
    //        hdnFieldIsEditMode.Value = "no";
    //        hdnFieldEditForm.Value = "";
    //    }
    //    else
    //    {
    //        hdnFieldIsEditMode.Value = "yes";
    //    }

    //    HomeData = null; // ClearFields();
    //}

    //bool IsNew
    //{
    //    get
    //    {
    //        bool Ans = false;
    //        string tmp = hdnFieldIsEditMode.Value.Trim();
    //        Ans = (tmp == "no") ? true : (tmp == "yes") ? false : false;
    //        return Ans;
    //    }
    //}
    //long HomeId
    //{
    //    get
    //    {
    //        long Ans = default(long);
    //        long.TryParse(hdnFieldEditForm.Value, out Ans);
    //        return Ans;
    //    }
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
    //protected void btnDelete_Click(object sender, EventArgs e)

    // #region methods
    //#endregion
    // void DeadCode(){

    //try
    //{
    //    if (RecordId<1)//hdnFieldIsEditMode.Value == "no")
    //    {
    //        // SZ [Apr 5, 2013] rem,oved the code below
    //        //if (this.AccountID == 0)
    //        //{
    //        //    Account account = new Account();

    //        //    Engine.AccountActions.Add(account);

    //        //    this.AccountID = account.Key;
    //        //}
    //        //byte StateID = default(byte);
    //        //byte.TryParse(ddlHomeStates.SelectedValue, out StateID);

    //        //Home entity = new Home
    //        //{
    //        //    AccountId = this.AccountID,
    //        //    Individualkey = Helper.ConvertToLong(ddlIndividual.SelectedValue),

    //        //    YearBuilt = txtYear.Text,
    //        //    SqFootage = Helper.ConvertToLong(txtSquareFootage.Text),
    //        //    DwellingType = txtDwellingType.Text,
    //        //    DesignType = txtDesignType.Text,
    //        //    RoofAge = Helper.ConvertToLong(txtRoofAge.Text),
    //        //    RoofType = txtRoofType.Text,
    //        //    FoundationType = txtFoundationType.Text,
    //        //    HeatingType = txtHeatingType.Text,
    //        //    ExteriorWallType = txtExteriorWallType.Text,
    //        //    NumberOfClaims = Helper.ConvertToLong(txtNumberOfHomeClaims.Text),
    //        //    NumberOfBedrooms = Helper.ConvertToLong(txtNumberOfBedrooms.Text),
    //        //    NumberOfBathrooms = Helper.ConvertToLong(txtNumberOfBathrooms.Text),
    //        //    ReqCoverage = txtRequestedCoverage.Text,


    //        //    //SZ [Apr 5, 2013] for the bug 50 in mentis
    //        //    Address1 = txtHomeAddress1.Text.Trim(), 
    //        //    Address2 = txtHomeAddress2.Text.Trim(),
    //        //    City = txtHomeCity.Text.Trim(),
    //        //    ZipCode = txtHomeZip.Text.Trim(),
    //        //    StateId = StateID,

    //        //    //MonthlyPremium = Helper.ConvertToLong(txtMonthlyPremium.Text),
    //        //    CurrentCarrier = ddlCurrentCarrier.SelectedValue,

    //        //    //IsActive = true,
    //        //    //IsDeleted = false,
    //        //    //AddedOn = DateTime.Now,
    //        //    AddedBy = null //CurrentUser.Key;//Logged In User Id
    //        //    //ChangedOn = set in edit
    //        //    //ChangedBy = set in edit
    //        //};



    //        RecordId = Engine.HomeActions.Add(HomeData);
    //        //hdnFieldEditForm.Value = id.ToString();
    //        //hdnFieldIsEditMode.Value = "yes";
    //        ddlIndividual.Enabled = false;
    //    }

    //    else if (hdnFieldIsEditMode.Value == "yes")
    //    {
    //        if (hdnFieldEditForm.Value != "")
    //        {
    //            long Id = Convert.ToInt64(hdnFieldEditForm.Value);
    //            var entity = Engine.HomeActions.Get(Id);
    //            entity = HomeData;
    //            entity.Id = Id;

    //            //SZ [Apr 5, 2013] removed the code below

    //            //entity.YearBuilt = txtYear.Text;
    //            //entity.SqFootage = Helper.ConvertToLong(txtSquareFootage.Text);
    //            //entity.DwellingType = txtDwellingType.Text;
    //            //entity.DesignType = txtDesignType.Text;
    //            //entity.RoofAge = Helper.ConvertToLong(txtRoofAge.Text);
    //            //entity.RoofType = txtRoofType.Text;
    //            //entity.FoundationType = txtFoundationType.Text;
    //            //entity.HeatingType = txtHeatingType.Text;
    //            //entity.ExteriorWallType = txtExteriorWallType.Text;
    //            //entity.NumberOfClaims = Helper.ConvertToLong(txtNumberOfHomeClaims.Text);
    //            //entity.NumberOfBedrooms = Helper.ConvertToLong(txtNumberOfBedrooms.Text);
    //            //entity.NumberOfBathrooms = Helper.ConvertToLong(txtNumberOfBathrooms.Text);
    //            //entity.ReqCoverage = txtRequestedCoverage.Text;
    //            ////entity.MonthlyPremium = Helper.ConvertToDecimal(txtMonthlyPremium.Text);
    //            //entity.CurrentCarrier = ddlCurrentCarrier.SelectedValue;

    //            ////entity.IsActive = only when add new
    //            ////entity.IsDeleted = only when add new
    //            ////entity.AddedOn = only when add new
    //            ////entity.AddedBy =  only when add new
    //            //// entity.ChangedOn = set in DAL
    //            entity.ChangedBy = null;// Last Modified User
    //            Engine.HomeActions.Change(entity);
    //        }
    //    }

    //    if (!closeForm)
    //    {
    //        ctlStatus.SetStatus(Messages.RecordSavedSuccess);// lblMessageForm.Text = ;
    //        //lblMessageGrid.Text = "";
    //    }
    //    else
    //    {
    //        ShowGrid();
    //        //ChangeView();
    //        ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    //        //lblMessageForm.Text = "";
    //        //lblMessageGrid.Text = ;
    //    }
    //}
    //catch (Exception ex)
    //{
    //    ctlStatus.SetStatus(ex);  //lblMessageForm.Text = "Error: " + ex.Message;
    //}
    //protected string getSortDirectionString(SortDirection sortDirection)
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
    //protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    //{
    //    int size = e.PageSize;
    //    size = size > 100 ? 100 : size;
    //    grdHome.PageSize = size;
    //    BindGrid();
    //}
    //protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    //{
    //    grdHome.PageIndex = e.PageNumber;
    //    BindGrid();
    //}
    //protected void grdHome_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    grdHome.PageIndex = e.NewPageIndex;
    //    BindGrid();
    //}
    public string HomeRadWindowClientID
    { get { return dlgNewHome.ClientID; } }

    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
        {
            ddlIndividual.DataSource = handle.Individuals;
            ddlIndividual.DataBind();
            //IH 19.07.13
            ddlIndividual.Items.Insert(0, new ListItem(string.Empty, "-1"));
            ddlIndividual.SelectedIndex = 0;
        }
    }


    long RecordId
    {
        get
        {
            long lAns = 0;
            long.TryParse(hdnRecordId.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnRecordId.Value = value.ToString();
        }
    }
    public override bool IsValidated
    {
        get
        {
            vldYear.Validate();
            string errorMessage = "Error Required Field(s): ";
            if(!vldYear.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + "Year"));
            return vldYear.IsValid;
        }
    }

    Home GetHomeInfo(ref Home home)
    {
        byte StateID = default(byte);
        byte.TryParse(ddlHomeStates.SelectedValue, out StateID);

        home.AccountId = this.AccountID;
        home.Individualkey = Helper.SafeConvert<long>(ddlIndividual.SelectedValue);
        home.YearBuilt = txtYear.Text;
        home.SqFootage = Helper.SafeConvert<long>(txtSquareFootage.Text);
        home.DwellingType = txtDwellingType.Text;
        home.DesignType = txtDesignType.Text;
        home.RoofAge = Helper.SafeConvert<long>(txtRoofAge.Text);
        home.RoofType = txtRoofType.Text;
        home.FoundationType = txtFoundationType.Text;
        home.HeatingType = txtHeatingType.Text;
        home.ExteriorWallType = txtExteriorWallType.Text;
        home.NumberOfClaims = Helper.SafeConvert<long>(txtNumberOfHomeClaims.Text);
        home.NumberOfBedrooms = Helper.SafeConvert<long>(txtNumberOfBedrooms.Text);
        home.NumberOfBathrooms = Helper.SafeConvert<long>(txtNumberOfBathrooms.Text);
        home.ReqCoverage = txtRequestedCoverage.Text;


        //SZ [Apr 5; 2013] for the bug 50 in mentis
        home.Address1 = txtHomeAddress1.Text.Trim();
        home.Address2 = txtHomeAddress2.Text.Trim();
        home.City = txtHomeCity.Text.Trim();
        home.ZipCode = txtHomeZip.Text.Trim();
        //IH-22.07.13
        home.StateId = StateID == 0 ? (byte?)null : StateID;
        //Ans.MonthlyPremium = Helper.ConvertToLong(txtMonthlyPremium.Text);
        home.CurrentCarrier = ddlCurrentCarrier.SelectedValue;
        home.AddedBy = null; //CurrentUser.Key;//Logged In User Id

        return home;
    }
    void SetHomeInfo(Home value)
    {
        if (ddlIndividual.Items.Count > 0)
            ddlIndividual.SelectedValue = value.Individualkey.ToString();

        txtYear.Text = value.YearBuilt;
        txtSquareFootage.Text = value.SqFootage.HasValue ? value.SqFootage.ToString() : "";
        txtDwellingType.Text = value.DwellingType;
        txtDesignType.Text = value.DesignType;
        txtRoofAge.Text = value.RoofAge.HasValue ? value.RoofAge.ToString() : "";
        txtRoofType.Text = value.RoofType;
        txtFoundationType.Text = value.FoundationType;
        txtHeatingType.Text = value.HeatingType;
        txtExteriorWallType.Text = value.ExteriorWallType;
        txtNumberOfHomeClaims.Text = value.NumberOfClaims.HasValue ? value.NumberOfClaims.ToString() : "";
        txtNumberOfBedrooms.Text = value.NumberOfBedrooms.ToString();
        txtNumberOfBathrooms.Text = value.NumberOfBathrooms.HasValue ? value.NumberOfBathrooms.ToString() : "";
        txtRequestedCoverage.Text = value.ReqCoverage;
        //txtMonthlyPremium.Text = Helper.ConvertToString(value.MonthlyPremium);

        //SZ [Apr 5, 2013] for the bug 50 in mentis
        txtHomeAddress1.Text = value.Address1;
        txtHomeAddress2.Text = value.Address2;
        txtHomeCity.Text = value.City;
        txtHomeZip.Text = value.ZipCode;
        if (value.StateId.HasValue)
            ddlHomeStates.SelectedValue = value.StateId.Value.ToString();

        if (ddlCurrentCarrier.Items.Count > 0)
            ddlCurrentCarrier.SelectedValue = value.CurrentCarrier;
    }
    void ClearFields()
    {
        RecordId = 0;
        ddlIndividual.DataSource = ((IIndividual)Page).Individuals;
        ddlIndividual.DataBind();
        //IH 19.07.13
        ddlIndividual.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlIndividual.SelectedIndex = 0;


        ddlCurrentCarrier.DataSource = Engine.CarrierActions.GetAll().Where(x => x.IsHome == true);
        ddlCurrentCarrier.DataBind();
        //[IH, 17-07-2013]
        ddlCurrentCarrier.Items.Insert(0, new ListItem(String.Empty, String.Empty));
        ddlCurrentCarrier.SelectedIndex = 0;

        txtYear.Text = "";
        txtSquareFootage.Text = "";
        txtDwellingType.Text = "";
        txtDesignType.Text = "";
        txtRoofAge.Text = "";
        txtRoofType.Text = "";
        txtFoundationType.Text = "";
        txtHeatingType.Text = "";
        txtExteriorWallType.Text = "";
        txtNumberOfHomeClaims.Text = "";
        txtNumberOfBedrooms.Text = "";
        txtNumberOfBathrooms.Text = "";
        txtRequestedCoverage.Text = "";
        //txtMonthlyPremium.Text = "";

        //SZ [Apr 5, 2013] for the bug 50 in mentis
        txtHomeAddress1.Text = string.Empty;
        txtHomeAddress2.Text = string.Empty;
        txtHomeCity.Text = string.Empty;
        txtHomeZip.Text = string.Empty;
        ddlHomeStates.DataSource = (Page as SalesDataPage).USStates; // Engine.Constants.States;
        ddlHomeStates.DataBind();
        //// IH= 19.07.13
        ddlHomeStates.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlHomeStates.SelectedIndex = 0;

        ////ddlHomeStates.Items.Insert(0, new ListItem(string.Empty, string.Empty));
        ////ddlHomeStates.SelectedIndex = 0;
        //ddlHomeStates.Items.Insert(0, new ListItem("--Select State--", "-1"));
        //ddlHomeStates.SelectedIndex = 0;


        if (ddlCurrentCarrier.Items.Count > 0)
            ddlCurrentCarrier.SelectedIndex = 0;

        ctlStatus.Clear();
        //lblMessageForm.Text = "";
        //lblMessageGrid.Text = "";
    }

    void BindGrid()
    {
        try
        {
            var records = Engine.HomeActions.GetAllByAccountID(this.AccountID).Select(T =>
                new
                {
                    ID = T.Id,
                    FullName = T.Individual.FirstName + " " + T.Individual.LastName,
                    T.YearBuilt,
                    T.DwellingType,
                    T.DesignType,
                    T.NumberOfBedrooms,
                    T.NumberOfBathrooms,
                    SquareFootage = T.SqFootage,
                    Carrier = T.CurrentCarrier,
                    //T.MonthlyPremium

                }).AsQueryable();

            grdHome.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(records, PagingNavigationBar.SortBy, PagingNavigationBar.SortAscending));

            grdHome.DataBind();

        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex); //lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }
    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;
        //SR divGrid.Visible = bShow;
        //SR divForm.Visible = !bShow;
        if (bShow)
        {
            dlgNewHome.Dispose();
            dlgNewHome.VisibleOnPageLoad = false;
            dlgNewHome.Visible = false;
            BindGrid();
            RecordId = 0;
        }
        else
        {
            dlgNewHome.VisibleOnPageLoad = true;
            dlgNewHome.Visible = true;
            dlgNewHome.CenterIfModal = true;
        }
    }
    void Sort(string column)
    {
        if (PagingNavigationBar.SortBy == column)
            PagingNavigationBar.SortAscending = !PagingNavigationBar.SortAscending;
        else
        {
            PagingNavigationBar.SortBy = column;
            PagingNavigationBar.SortAscending = true;
        }
        BindGrid();
    }



    void BindEvents()
    {
        if (Page is IIndividual)
            (Page as IIndividual).Notify(this);
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
        //var M = base.SalesPage.Master as SiteMasterPage;
        //if (M != null)
        //    M.buttonYes.Click += (o, a) => ShowGrid(); // new EventHandler(CancelOnForm_Click);

        btnReturn.Click += (o, a) => ShowGrid();
        btnCancelOnForm.Click += (o, a) => ShowGrid();

        btnAddNew.Click += (o, a) => AddHome();
        btnSaveOnForm.Click += (o, a) =>
        {
            if (IsValidated)
                SaveHome();
        };

        btnSaveAndCloseOnForm.Click += (o, a) => { if (IsValidated) { SaveHome(); ShowGrid(); } };

        grdHome.RowCommand += (o, a) => CommandRouter(a.CommandName, Helper.SafeConvert<long>(
            grdHome.DataKeys[((a.CommandSource as Control).NamingContainer as GridViewRow).RowIndex].Value.ToString()
            ));

        grdHome.Sorting += (o, a) => Sort(a.SortExpression);

        PagingNavigationBar.SizeChanged += (o, a) => BindGrid();
        PagingNavigationBar.IndexChanged += (o, a) => BindGrid();

    }
    void CommandRouter(string command, long id)
    {
        switch (command)
        {
            case "EditX":
                EditHome(id);
                break;
            case "ViewX":
                EditHome(id);
                ReadOnly = true;
                break;
            case "DeleteX":
                DeleteHome(id);
                break;
        }
    }

    protected override void InnerInit()
    {
        IsGridMode = true;
        RecordId = 0;
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        BindEvents();

        if (bFirstTime)
            ShowGrid(true);
    }
    protected override void InnerSave()
    {
        SaveHome();
        if (CloseForm)
            ShowGrid();
    }


    void AddHome()
    {
        ClearFields();
        ShowGrid(false);
        //ChangeView(ViewMode.AddNew);
    }
    void EditHome(long Id)
    {
        ClearFields();
        Home H = Engine.HomeActions.Get(Id);
        RecordId = H.Id;
        SetHomeInfo(H);
        ShowGrid(false);
    }
    void DeleteHome(long id)
    {
        try
        {
            Engine.HomeActions.Delete(id);
            BindGrid();
            //divGrid.Visible = true;
            //divForm.Visible = false;
            //SR  ctlStatus.SetStatus(Messages.RecordDeletedSuccess); //lblMessageGrid.Text = ;
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex); //lblMessageForm.Text = "Error: " + ex.Message;
        }
    }
    void SaveHome()
    {
        Home H = null;

        H = (RecordId < 1) ? new Home { AccountId = AccountID, AddedBy = SalesPage.CurrentUser.FullName, AddedOn = DateTime.Now } :
            Engine.HomeActions.Get(RecordId);
        GetHomeInfo(ref H);

        try
        {
            if (RecordId < 1)
            {
                Engine.HomeActions.Add(H);
                RecordId = H.Id;
            }
            else
            {
                H.ChangedBy = SalesPage.CurrentUser.FullName;
                H.ChangedOn = DateTime.Now;
                Engine.HomeActions.Change(H);
            }
            //SR ctlStatus.SetStatus(Messages.RecordSavedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }

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
    protected void grdHome_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DisableDeleteInGridView(e.Row, "lnkDelete");
    }
}
