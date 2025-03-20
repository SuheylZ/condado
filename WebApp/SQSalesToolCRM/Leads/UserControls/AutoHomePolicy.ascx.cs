using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;

public partial class Leads_UserControls_Policy : AccountsBaseControl, IIndividualNotification
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

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
    //    if (masterPage != null)
    //    {
    //        masterPage.buttonYes.Click += new EventHandler(CancelOnForm_Click);
    //    }
    //    //if (!Page.IsPostBack)
    //    //{
    //    //    BindgrdHome(Convert.ToInt32(ddlPolicyTypeMain.SelectedValue));
    //    //    BindIndividualsDropDown();
    //    //    divGrid.Visible = true;
    //    //    divForm.Visible = false;
    //    //}
    //}
    //private void SaveForm(bool closeForm = false)
    //{
    //    try
    //    {
    //        if (hdnFieldIsEditMode.Value == "no")
    //        {
    //            var entity = SavePolicy(ViewMode.AddNew);
    //            Engine.AutoHomePolicyActions.Add(entity);
    //            hdnFieldEditForm.Value = entity.ID.ToString();
    //            hdnFieldIsEditMode.Value = "yes";
    //        }
    //        else if (hdnFieldIsEditMode.Value == "yes")
    //        {
    //            if (hdnFieldEditForm.Value != "")
    //            {
    //                var entity = SavePolicy(ViewMode.Edit); //Engine.AutoHomePolicyActions.Get(Convert.ToInt16(hdnFieldEditForm.Value));
    //                Engine.AutoHomePolicyActions.Change(entity);
    //            }
    //        }
    //        if (!closeForm)
    //        {
    //            ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    //            //lblMessageForm.Text = ;
    //            //lblMessageGrid.Text = "";
    //        }
    //        else
    //        {
    //            ChangeView();
    //            //lblMessageForm.Text = "";
    //            //lblMessageGrid.Text = "Record saved successful.";
    //            ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex);
    //       // lblMessageForm.Text = "Error: " + ex..Message;
    //    }
    //}
    //protected void Save_Click(object sender, EventArgs e)
    //{
    //    SaveForm();
    //}
    //protected void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    SaveForm(true);
    //}
    //protected void AddNew_Click(object sender, EventArgs e)
    //{
    //    AddNewView();
    //}
    //protected void btnReturn_Click(object sender, EventArgs e)
    //{
    //    GridView();
    //}
    // must be public to access in the parent form
    //public void CancelOnForm_Click(object sender, EventArgs e)
    //{
    //    GridView();
    //}
    //private void GridView()
    //{
    //    ChangeView();
    //}
    //void AddPolicy()
    //{
    //    ClearFields();
    //    ShowGrid(false);
    //}
    //private void EditView()
    //{
    //    ChangeView(ViewMode.Edit);
    //}
    //private enum ViewMode { GridView = 0, AddNew = 1, Edit = 2 }
    //private void ChangeView(ViewMode viewMode = ViewMode.GridView)
    //{
    //    //divGrid.Visible = viewMode == ViewMode.GridView;
    //    //divForm.Visible = viewMode != ViewMode.GridView;
    //    ShowGrid(viewMode == ViewMode.GridView);
    //    //IsGridMode = viewMode == ViewMode.GridView;
    //    if (viewMode == 0)
    //    {
    //        //[QN, 12-04-2013]: New Ploicy types Renter, Umberella has been introduced. Now grid is
    //        //is bind on the bases of selected policy type.
    //        //BindgrdHome(IsHomePolicyTypeMain);
    //        BindGrid(Convert.ToInt32(ddlPolicyTypeMain.SelectedValue));
    //        return;
    //    }
    //    ddlPolicyType.Enabled = viewMode == ViewMode.AddNew;
    //    if (viewMode == ViewMode.AddNew)
    //    {
    //        hdnFieldIsEditMode.Value = "no";
    //        hdnFieldEditForm.Value = "";
    //    }
    //    else
    //    {
    //        hdnFieldIsEditMode.Value = "yes";
    //    }
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
    //private void SetSelectedValue(DropDownList ddl, int? val)
    //{
    //    this.SetSelectedValue(ddl,(decimal?)val);
    //}
    //private void SetSelectedValue(DropDownList ddl, long? val)
    //{
    //    this.SetSelectedValue(ddl, (decimal?)val);
    //}
    //private void SetSelectedValue(DropDownList ddl, decimal? val)
    //{
    //    string v = val.HasValue ? val.Value.ToString() : "";
    //    this.SetSelectedValue(ddl, v);
    //}
    //private void SetSelectedValue(DropDownList ddl, string val)
    //{
    //    if (ddl.Items.Count == 0)
    //    {
    //        return;
    //    }
    //    ListItem itm = ddl.Items.FindByValue(val);
    //    if (itm == null)
    //    {
    //        ddl.SelectedIndex = 0;
    //        return;
    //    }
    //    ddl.SelectedValue = val;
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
    //public void BindIndividualsDropDown(string selectedValue)
    //{
    //    BindIndividualsDropDown();
    //    //if (ddIndividualName.Items == null || ddIndividualName.Items.Count == 0)
    //    //{
    //    //    return;
    //    //}
    //    ListItem itm = ddIndividualName.Items.FindByValue(selectedValue);
    //    if (itm != null)
    //        itm.Selected = true;
    //}
    //private void BindIndividualsDropDown()
    //{
    //    //try
    //    //{
    //    var Data = GetIndividualsByAccount(); // Engine.IndividualsActions.GetByAccountID(AccountID);
    //       // var Data = GetIndividualsByAccount();
    //        //
    //        var result = (from x in Data
    //                      select new
    //                      {
    //                          Key = x.Id,
    //                          Name = x.LastName + ", " + x.FirstName
    //                      });
    //        ddIndividualName.DataSource = result;
    //        ddIndividualName.DataValueField = "Key";
    //        ddIndividualName.DataTextField = "Name";
    //        ddIndividualName.DataBind();
    //    //}
    //    //catch
    //    //{
    //    //}
    //}
    //public void BindgrdHome(bool home)
    //{
    //    try
    //    {
    //        int policyType = home ? 1 : 0;
    //        var policies = Engine.AutoHomePolicyActions.GetAllByAccountID(this.AccountID).Where(x => x.PolicyType == policyType);
    //        var individuals = Engine.IndividualsActions.GetByAccountID(this.AccountID);
    //        var records = (from T in policies
    //                       join i in individuals.DefaultIfEmpty() on T.IndividualKey equals i.Key  //Engine.AutoHomePolicyActions.GetAllByAccountID(this.AccountID).Where(x => x.PolicyType == policyType).Select(T => new
    //                       select new
    //                       {
    //                           Id = T.ID,
    //                           PolicyType = T.PolicyType == 0 ? "Auto" : T.PolicyType == 1 ? "Home" : T.PolicyType == 2 ? "Renter" : "Umbrella",
    //                           PolicyNumber = T.PolicyNumber,
    //                           Carrier = T.Carrier.Name,
    //                           CurrentCarrier = T.CurrentCarrier.Name,
    //                           MonthlyPremium = T.MonthlyPremium,
    //                           Term = T.Term == 0 ? "Renter" : T.Term == 6 ? "6 Months" : T.Term == 12 ? "12 Months" : "",
    //                           UmbrellaPolicy = T.PolicyType == 0 || T.UmbrellaPolicy == null ? null : T.UmbrellaPolicy == 0 ? "No" : "Yes",
    //                           EffectiveDate = T.EffectiveDate,
    //                           BoundDate = T.BoundOn,
    //                           PolicyHolder = i == null ? "" : i.LastName + ", " + i.FirstName,
    //                       }).AsQueryable();
    //        grdHome.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(records, PagingNavigationBar.SortBy, PagingNavigationBar.SortAscending));
    //        grdHome.DataBind();
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessageGrid.Text = "Error: " + ex.Message;
    //    }
    //}
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
    ////public long AccountID
    ////{
    ////    get
    ////    {
    ////        return base.SalesPage.AccountID;
    ////    }
    ////}
    ////bool IsGridMode
    ////{
    ////    get
    ////    {
    ////        bool bAns = false;
    ////        bool.TryParse(hdnGridMode.Value, out bAns);
    ////        return bAns;
    ////    }
    ////    set
    ////    {
    ////        hdnGridMode.Value = value.ToString();
    ////    }
    ////}
    //#endregion
    //private void BindAutoHomeCarriers(bool home)
    //{
    //    if (home)
    //    {
    //        BindAutoCarriers();
    //    }
    //    else
    //    {
    //        BindHomeCarriers();
    //    }
    //}
    //private void BindAutoCarriers()
    //{
    //    var U = Engine.CarrierActions.GetAutoCarriers();
    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();
    //    ddlCarrier.DataSource = U;
    //    ddlCarrier.DataBind();
    //}
    //private void BindHomeCarriers()
    //{
    //    var U = Engine.CarrierActions.GetHomeCarriers();
    //    ddlCurrentCarrier.DataSource = U;
    //    ddlCurrentCarrier.DataBind();
    //    ddlCarrier.DataSource = U;
    //    ddlCarrier.DataBind();
    //}
    //protected void ddlPolicyTypeMain_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    //[QN, 12-04-2013]: New Ploicy types Renter, Umberella has been introduced. Now grid is
    //    //is bind on the bases of selected policy type.
    //    //BindgrdHome(IsHomePolicyTypeMain);
    //    BindGrid(); //Convert.ToInt32(ddlPolicyTypeMain.SelectedValue));
    //}
    //protected void ddlPolicyType_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    SetTermList(IsHomePolicyType);
    //}
    //public bool IsHomePolicyTypeMain
    //{
    //    get
    //    {
    //        return ddlPolicyTypeMain.SelectedIndex == 1;
    //    }
    //}
    //protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    //{
    //    int size = e.PageSize;
    //    size = size > 100 ? 100 : size;
    //    grdHome.PageSize = size;

    //    //[QN, 12-04-2013]: New Ploicy types Renter, Umberella has been introduced. Now grid is
    //    //is bind on the bases of selected policy type.
    //    //BindgrdHome(IsHomePolicyTypeMain);
    //    BindGrid(); //Convert.ToInt32(ddlPolicyTypeMain.SelectedValue));
    //}
    //protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    //{
    //    grdHome.PageIndex = e.PageNumber;

    //    //[QN, 12-04-2013]: New Ploicy types Renter, Umberella has been introduced. Now grid is
    //    //is bind on the bases of selected policy type.
    //    //BindgrdHome(IsHomePolicyTypeMain);
    //    BindGrid(); //Convert.ToInt32(ddlPolicyTypeMain.SelectedValue));
    //}
    //protected void Evt_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    try
    //    {
    //        grdHome.PageIndex = e.NewPageIndex;

    //        //[QN, 12-04-2013]: New Ploicy types Renter, Umberella has been introduced. Now grid is
    //        //is bind on the bases of selected policy type.
    //        //BindgrdHome(IsHomePolicyTypeMain);
    //        BindGrid(); //Convert.ToInt32(ddlPolicyTypeMain.SelectedValue));
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex); //lblMessageGrid.Text = "Error: " + ex.Message;
    //    }
    //}

    public string AutoHomeRadWindowClientID { get { return dlgAutoHomePolicy.ClientID; } }
    public event EventHandler OnAddIndividual = null;
    //YA[28 May 2014] 
    public event EventHandler FreshPolicyStatus;
    protected void OnFreshPolicyStatus(EventArgs e)
    {
        if(FreshPolicyStatus != null)
        {
            FreshPolicyStatus(this, e);
        }
    }

    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
            BindIndividuals();
    }
    public void BindIndividuals(long Id = 0)
    {
        string oldValue = ddIndividualName.SelectedValue;
        ddIndividualName.DataSource = (Page as IIndividual).Individuals;
        ddIndividualName.DataBind();
        //IH-19.07.13
        ddIndividualName.Items.Insert(0, new ListItem(string.Empty, "-1"));

        if (Id == 0)
        {
            if (ddIndividualName.Items.FindByValue(oldValue) != null)
                ddIndividualName.SelectedValue = oldValue;
        }
        else
            ddIndividualName.SelectedValue = Id.ToString();
    }

    public void BindPolicyTypes()
    {
        var U =Engine.PolicyStatusActions.GetAllPolicyTypes().ToList();

        ddlPolicyType.DataTextField = "Name";
        ddlPolicyType.DataValueField = "ID";
        ddlPolicyType.DataSource = U;
        ddlPolicyType.DataBind(); 
        ddlPolicyType.Items.Insert(0, new ListItem(string.Empty, "-1"));

        ddlPolicyTypeMain.DataTextField = "Name";
        ddlPolicyTypeMain.DataValueField = "ID";
        ddlPolicyTypeMain.DataSource = U;
        ddlPolicyTypeMain.DataBind(); 
        ddlPolicyTypeMain.Items.Insert(0, new ListItem("All Filter", "-1"));
    }

    void ClearFields()
    {
        BindIndividuals();

        //ddlPolicyType.SelectedValue = ddlPolicyTypeMain.SelectedValue=="-1"? "1": ddlPolicyTypeMain.SelectedValue;
        ddlPolicyType.SelectedValue = ddlPolicyTypeMain.SelectedValue == "-1" ? null : ddlPolicyTypeMain.SelectedValue;
        SetTermList(ddlPolicyType.SelectedValue == "0");

        txtCompanyName.Text = "";
        txtPolicyNumber.Text = string.Empty;
        rdpEffective.SelectedDate = null;

        ddlUmbrellaPolicy.SelectedIndex = 0;

        txtMonthlyPremium.Text = string.Empty;
        txtCurrentMonthlyPremium.Text = string.Empty;

        ddlTerm.SelectedIndex = 0;

        cbxDidWeIncreaseCoverage.Checked = false;

        rdpBound.SelectedDate = DateTime.Now;
        rdpEffective.SelectedDate = null;
        rdpLapse.SelectedDate = null;

        txtCurrentCarrierPolicy.Text = string.Empty;

        //ddlWritingAgent.SelectedIndex = -1;
        ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
        ctlStatus.Clear();
    }
    void SetAutoHomePolicyInfo(AutoHomePolicy HP)
    {
        if (HP.IndividualKey.HasValue)
            ddIndividualName.SelectedValue = HP.IndividualKey.ToString();
        //SetSelectedValue(ddIndividualName, HP.IndividualKey);

        if (HP.PolicyType.HasValue)
            ddlPolicyType.SelectedValue = HP.PolicyType.ToString();

        SetTermList(HP.PolicyType == 1);

        txtCompanyName.Text = HP.CompanyName;
        txtPolicyNumber.Text = HP.PolicyNumber;
        rdpEffective.SelectedDate = HP.EffectiveDate;

        if (HP.UmbrellaPolicy.HasValue)
            ddlUmbrellaPolicy.SelectedValue = HP.UmbrellaPolicy.ToString();

        if (HP.CarrierID.HasValue)
            ddlCarrier.SelectedValue = HP.CarrierID.Value.ToString();

        if (HP.CurrentCarrierID.HasValue)
            ddlCurrentCarrier.SelectedValue = HP.CurrentCarrierID.ToString();

        txtMonthlyPremium.Text = HP.MonthlyPremium.HasValue ? HP.MonthlyPremium.ToString() : "";
        txtCurrentMonthlyPremium.Text = HP.CurrentMonthlyPremium.HasValue ? HP.CurrentMonthlyPremium.ToString() : "";

        if (HP.Term.HasValue)
            ddlTerm.SelectedValue = HP.Term.ToString();

        cbxDidWeIncreaseCoverage.Checked = HP.IsCoverageIncreased ?? false;

        rdpBound.SelectedDate = HP.BoundOn.HasValue ? HP.BoundOn.Value : DateTime.Now;
        rdpLapse.SelectedDate = HP.LapsedOn;
        rdpEffective.SelectedDate = HP.EffectiveDate;
        txtCurrentCarrierPolicy.Text = HP.CurrentCarrierText;

        //WM - [23.07.2013]
        if (HP.WritingAgent.HasValue)
        {
            //        ddlWritingAgent.SelectedValue = (HP.WritingAgent == null || HP.WritingAgent == Guid.Empty) ? (ddlWritingAgent.SelectedValue = "-1") : HP.WritingAgent.ToString();
            ddlWritingAgent.SelectedValue = HP.WritingAgent.Value.ToString();
        }
        else
        {
            ddlWritingAgent.SelectedIndex = -1;
        }
        //WM - [31.07.2013]
        if (HP.PolicyStatus.HasValue)
        {
            ddlPolicyStatus.SelectedValue = HP.PolicyStatus.Value.ToString();
        }
        else
        {
            ddlPolicyStatus.SelectedIndex = 0;
        }

        //if (Convert.ToInt64(HP.PolicyStatus) > 0)
        //{
        //    var psEntity = Engine.PolicyStatusActions.Get(Convert.ToInt64(HP.PolicyStatus));
        //    txtPolicyStatus.Text = psEntity.Name;
        //}
    }
    void GetAutoHomePolicyInfo(ref AutoHomePolicy HP)
    {
        HP.PolicyType = Helper.NullConvert<int>(ddlPolicyType.SelectedValue);
        HP.IndividualKey = Helper.NullConvert<long>(ddIndividualName.SelectedValue);
        HP.CompanyName = txtCompanyName.Text;
        HP.PolicyNumber = txtPolicyNumber.Text;
        HP.EffectiveDate = rdpEffective.SelectedDate;
        HP.UmbrellaPolicy = !IsHomePolicySelected ? null : Helper.NullConvert<int>(ddlUmbrellaPolicy.SelectedValue);
        HP.CarrierID = (ddlCarrier.SelectedValue == "-1" || string.IsNullOrWhiteSpace(ddlCarrier.SelectedValue)) ? null : Helper.NullConvert<long>(ddlCarrier.SelectedValue);
        //if (ddlCarrier.SelectedValue == "-1" || string.IsNullOrWhiteSpace(ddlCarrier.SelectedValue))
        //    HP.CarrierID = null;
        //else
        //    HP.CarrierID = Helper.NullConvert<long>(ddlCarrier.SelectedValue);
        HP.CurrentCarrierID = Helper.NullConvert<long>(ddlCurrentCarrier.SelectedValue);
        HP.MonthlyPremium = Helper.NullConvert<decimal>(txtMonthlyPremium.Text);  // Helper.ConvertToNullDecimal(txtMonthlyPremium.Text);
        HP.CurrentMonthlyPremium = Helper.NullConvert<decimal>(txtCurrentMonthlyPremium.Text); //Helper.ConvertToNullDecimal(txtCurrentMonthlyPremium.Text);
        HP.Term = Helper.NullConvert<int>(ddlTerm.SelectedValue);
        HP.IsCoverageIncreased = cbxDidWeIncreaseCoverage.Checked;
        HP.CurrentCarrierText = txtCurrentCarrierPolicy.Text;
        HP.BoundOn = rdpBound.SelectedDate;
        HP.EffectiveDate = rdpEffective.SelectedDate;
        HP.LapsedOn = rdpLapse.SelectedDate;

        HP.WritingAgent = !string.IsNullOrWhiteSpace(ddlWritingAgent.SelectedValue)
                              ? (Guid?)new Guid(ddlWritingAgent.SelectedValue)
                              : null;
        //YA[01 May 2014] Log the policy status change
        if (Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue) != HP.PolicyStatus)
        {
            Engine.AccountHistory.PolicyStatusChanged(AccountID, ddlPolicyStatus.SelectedItem.Text, "Policy - " + ddlPolicyType.SelectedItem.Text, base.SalesPage.CurrentUser.Key, 0 , Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue).Value);
            OnFreshPolicyStatus(EventArgs.Empty);
        }
        //WM - [31.07.2013]
        HP.PolicyStatus = string.IsNullOrWhiteSpace(ddlPolicyStatus.SelectedValue) ? null : Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue);
    }
    void SetTermList(bool home)
    {
        const string K_Carriers_Auto = "carieers_auto";
        const string K_Carriers_Home = "carieers_home";

        ListItem renterListItem = new ListItem("Renter", "0");

        // SZ [may 9, 2013] Although the Add2Cache() has this condition, 
        // why make a function call if cache already has it?
        if (Cache[K_Carriers_Auto] == null)
            Add2Cache(K_Carriers_Auto, Engine.CarrierActions.GetAutoCarriers().ToList());

        if (Cache[K_Carriers_Home] == null)
            Add2Cache(K_Carriers_Home, Engine.CarrierActions.GetHomeCarriers().ToList());

        var U = Cache[home ? K_Carriers_Home : K_Carriers_Auto] as IEnumerable<Carrier>;

        if (home)
        {
            lblUmbrellaPolicy.Visible = true;
            ddlUmbrellaPolicy.Visible = true;
            ddlTerm.Items.Add(renterListItem);
        }
        else
        {
            lblUmbrellaPolicy.Visible = false;
            ddlUmbrellaPolicy.Visible = false;
            ddlTerm.Items.Remove(renterListItem);
        }

        ddlCurrentCarrier.DataSource = U;
        ddlCarrier.DataSource = U;

        ddlCurrentCarrier.DataBind();
        ddlCarrier.DataBind();
        //[IH, 16-07-2013]
        ddlCarrier.Items.Insert(0, new ListItem(String.Empty, "-1"));
        ddlCarrier.SelectedIndex = 0;


        //[QN, 29-05-2013]
        ddlWritingAgent.DataSource = Engine.UserActions.GetWritingAgents();
        ddlWritingAgent.DataBind();
        //ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
        ddlWritingAgent.Items.Insert(0, new ListItem("", null));
        ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
        //  ddlWritingAgent.SelectedIndex = 0;

        /*
             * Policy status type is as following at the moment
             autohome = 1
             Medicare Supplement	= 2
             MAPDP = 3
             Dental and Vision = 4
             */
        ddlPolicyStatus.DataSource = Engine.PolicyStatusActions.GetAll(/*Policy status type = */1);
        ddlPolicyStatus.DataBind();
        ddlPolicyStatus.Items.Insert(0, new ListItem(""));

        //ddlCurrentCarrier.SelectedIndex = 0;
        //ddlCarrier.SelectedIndex = 0;
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
        //    M.buttonYes.Click += (o, a) => ShowGrid();

        btnAddNew.Click += (o, a) => AddPolicy();

        btnCancelOnForm.Click += (o, a) => ShowGrid();
        btnReturn.Click += (o, a) => ShowGrid();
        btnSaveOnForm.Click += (o, a) =>
        {
            if (IsValidated)
                SavePolicy();
        };
        btnSaveAndCloseOnForm.Click += (o, a) =>
        {
            if (IsValidated)
            {
                SavePolicy();
                ShowGrid();
            }
        };

        grdHome.RowCommand += (o, a) => Command_Router(a.CommandName,
            Helper.SafeConvert<long>(grdHome.DataKeys[((a.CommandSource as Control).NamingContainer as GridViewRow).RowIndex].Value.ToString()));
        grdHome.Sorting += (o, a) => Sort(a.SortExpression);

        ddlPolicyTypeMain.SelectedIndexChanged += (o, a) => { ctlStatus.Clear(); BindGrid(); };
        ddlPolicyType.SelectedIndexChanged += (o, a) => SetTermList(ddlPolicyType.SelectedValue == "1");

        PagingNavigationBar.IndexChanged += (o, a) => BindGrid();
        PagingNavigationBar.SizeChanged += (o, a) => BindGrid();
    }
    void BindGrid()
    {
        int policyType = Helper.SafeConvert<int>(ddlPolicyTypeMain.SelectedValue);

        try
        {
            //[QN, 12-04-2013]: New Ploicy types Renter, Umberella has been introduced.
            //var policies = Engine.AutoHomePolicyActions.GetAllByAccountID(AccountID).Where(x => x.PolicyType == policyType || policyType == -1).ToList();

            // SZ [May 10, 2013] this is the new query that john has requested if the policy holder is not defined
            //var records =  policies.GroupJoin( (Page as IIndividual).Individuals, //GetIndividualsByAccount(), 
            //                 a => a.IndividualKey, 
            //                 b=> b.Id, 
            //                 (a, b) => new { a, b })
            //                 .SelectMany(x=>x.b.DefaultIfEmpty(), (x, y) => new {a=x.a, b=x.b})
            //                 .Select(x=> new {
            //                   Id = x.a.ID,
            //                   PolicyType = x.a.PolicyType == 0 ? "Auto" : x.a.PolicyType == 1 ? "Home" : x.a.PolicyType == 2 ? "Renter" : "Umbrella",
            //                   PolicyNumber = x.a.PolicyNumber,
            //                   Carrier = x.a.Carrier.Name,
            //                   CurrentCarrier = x.a.CurrentCarrier.Name,
            //                   MonthlyPremium = x.a.MonthlyPremium,
            //                   Term = x.a.Term == 0 ? "Renter" : x.a.Term == 6 ? "6 Months" : x.a.Term == 12 ? "12 Months" : "",
            //                   UmbrellaPolicy = x.a.PolicyType == 0 || x.a.UmbrellaPolicy == null ? null : x.a.UmbrellaPolicy == 0 ? "No" : "Yes",
            //                   EffectiveDate = x.a.EffectiveDate,
            //                   BoundDate = x.a.BoundOn,
            //                   PolicyHolder = x.b == null? string.Empty : x.b.FirstOrDefault().FullName });
            //grdHome.DataSource = PagingNavigationBar.ApplyPaging(
            //    Helper.SortRecords(records.AsQueryable(),
            //        PagingNavigationBar.SortBy,
            //        PagingNavigationBar.SortAscending));

            var recs = Engine.AutoHomePolicyActions.GetPolicies(AccountID).Where(x => x.PolicyTypeVal == policyType || policyType == -1).AsQueryable();
            grdHome.DataSource = PagingNavigationBar.ApplyPaging(
                Helper.SortRecords(recs, PagingNavigationBar.SortBy, PagingNavigationBar.SortAscending)
                );
            grdHome.DataBind();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;
        //    divGrid.Visible = bShow;
        // divForm.Visible = !bShow;
        if (bShow)
        {
            dlgAutoHomePolicy.Dispose();
            dlgAutoHomePolicy.VisibleOnPageLoad = false;
            dlgAutoHomePolicy.Visible = false;
            BindGrid();
            RecordId = 0;
        }
        else
        {
            dlgAutoHomePolicy.VisibleOnPageLoad = true;
            dlgAutoHomePolicy.Visible = true;
            dlgAutoHomePolicy.CenterIfModal = true;
        }
    }
    void Sort(string column)
    {
        //try
        //{
        if (PagingNavigationBar.SortBy == column)
            PagingNavigationBar.SortAscending = !PagingNavigationBar.SortAscending;
        else
        {
            PagingNavigationBar.SortBy = column;
            PagingNavigationBar.SortAscending = true;
        }
        BindGrid();
        //[QN, 12-04-2013]: New Ploicy types Renter, Umberella has been introduced. Now grid is
        //is bind on the bases of selected policy type.
        //BindgrdHome(IsHomePolicyTypeMain);
        //BindGrid(); //Convert.ToInt32(ddlPolicyTypeMain.SelectedValue));
        //}
        //catch (Exception ex)
        //{
        //    ctlStatus.SetStatus(ex);// lblMessageGrid.Text = "Error: " + ex.Message;
        //}
    }
    void Command_Router(string command, long id)
    {
        //string command = e.CommandName;
        //long id = Helper.SafeConvert<long>(grdHome.DataKeys[((e.CommandSource as Control).NamingContainer as GridViewRow).RowIndex].Value.ToString());
        switch (command)
        {
            case "EditX":
                EditPolicy(id);
                break;
            case "DeleteX":
                DeletePolicy(id);
                break;
            case "ViewX":
                EditPolicy(id);
                ReadOnly = true;
                break;
        }
    }

    void AddPolicy()
    {
        ClearFields();
        ShowGrid(false);
    }
    void EditPolicy(long id)
    {
        ClearFields();
        var HP = Engine.AutoHomePolicyActions.Get(id);
        RecordId = HP.ID;
        SetAutoHomePolicyInfo(HP);
        ShowGrid(false);
    }
    void DeletePolicy(long id)
    {
        try
        {
            Engine.AutoHomePolicyActions.Delete(id);
            BindGrid();
            //SR  ctlStatus.SetStatus(Messages.RecordDeletedSuccess); //lblMessageGrid.Text =;
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex); //lblMessageForm.Text = "Error: " + ex.Message;
        }
    }
    void SavePolicy()
    {
        AutoHomePolicy HP = RecordId > 0 ? Engine.AutoHomePolicyActions.Get(RecordId) : new AutoHomePolicy
        {
            AccountId = AccountID,
            Added = new History1
            {
                By1 = SalesPage.CurrentUser.FullName,
                On1 = DateTime.Now
            }
        };

        GetAutoHomePolicyInfo(ref HP);

        try
        {
            if (RecordId < 1)
            {
                Engine.AutoHomePolicyActions.Add(HP);
                RecordId = HP.ID;
            }
            else
            {
                HP.Changed = new History1
                {
                    By1 = SalesPage.CurrentUser.FullName,
                    On1 = DateTime.Now
                };
                Engine.AutoHomePolicyActions.Change(HP);
            }
            //SR ctlStatus.SetStatus(Messages.RecordSavedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
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
        {
            //BindGrid();
            BindPolicyTypes();
            ShowGrid();
          
        }
    }
    protected override void InnerSave()
    {
        SavePolicy();
        if (CloseForm)
            ShowGrid();
    }
    public override bool IsValidated
    {
        get
        {
            bool bAns = false;

            bAns = ddIndividualName.SelectedValue != string.Empty;
            string errorMessage = "Error Required Field(s): ";
            vldPolicyNumber.Validate();
            if(!vldPolicyNumber.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + vldPolicyNumber.ErrorMessage));
            bAns &= vldPolicyNumber.IsValid;

            return bAns;
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
    bool IsHomePolicySelected
    {
        get
        {
            return ddlPolicyType.SelectedValue == "1";
        }
    }

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddNew.Visible = bEnable;
            grdHome.Columns[grdHome.Columns.Count - 1].Visible = !bEnable;
            DataControlField
                colEdit = grdHome.Columns[grdHome.Columns.Count - 2],
                colView = grdHome.Columns[grdHome.Columns.Count - 1];
            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
        {
            EnableControls(tblForm, bEnable);
        }
    }
    protected void btnAddNewIndividual_Click(object sender, EventArgs e)
    {
        if (OnAddIndividual != null)
            OnAddIndividual(this, new EventArgs());
    }
    protected void grdHome_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DisableDeleteInGridView(e.Row, "lnkDelete");
    }
}