using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using System.Data;
using SalesTool.DataAccess.Models;
using System.Text;
using System.Collections;
using System.Linq;


public class DialClickedEventArgs : EventArgs
{
    public DialClickedEventArgs(string phoneNumber, string message = "")
    {
        PhoneNumber = phoneNumber;
        Message = message;
    }

    public string PhoneNumber { get; set; }
    public string Message { get; set; }
}

public partial class Leads_UserControls_IndividualsInformation :
    AccountsBaseControl,
    IIndividualNotification,
    ITCPAClient
{
    //private Individual storeIndividual = new Individual();
    //private Individual storeSpouseIndv = new Individual();
    //protected void BindState()
    //{
    //    //try
    //    //{
    //        ddlStateIndividual.DataSource = Engine.Constants.States;
    //        //ddlStateIndividual.DataValueField = "ID";
    //        //ddlStateIndividual.DataTextField = "FullName";
    //        ddlStateIndividual.DataBind();
    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    ctlMessage.SetStatus(ex); // lblMessageForm.Text = "Error: " + ex.Message;
    //    //}
    //}




    // SZ [Jan 23, 2013] base class already implements it, not required anymore
    //public long AccountID
    //{
    //    get
    //    {
    //        return base.SalesPage.AccountID;
    //    }
    //    set
    //    {
    //        base.SalesPage.AccountID = value;
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

    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        return !IsGridMode;
    //    }
    //}
    //private string GetOutpulseId(int campaignId = 0)
    //{
    //    var c = Engine.ManageCampaignActions.Get(campaignId);

    //    if (c == null)
    //    {
    //        return "";
    //    }

    //    if (c.OutpulseType == 0) // general
    //    {
    //        return c.OutpulseId;
    //    }
    //    // else

    //    var umb = Engine.UserMultiBusinesses.GetAll().FirstOrDefault(x => x.CompanyId == c.CompanyID && x.UserKey == this.SalesPage.CurrentUser.Key);

    //    if (umb == null)
    //    {
    //        return c.OutpulseId;
    //    }

    //    return umb.OutpulseId;
    //}

    //private string getSortDirectionString(SortDirection sortDirection)
    //{
    //    //string newSortDirection = String.Empty;

    //    //switch (sortDirection)
    //    //{
    //    //    case SortDirection.Ascending:
    //    //        newSortDirection = "ASC";
    //    //        break;

    //    //    case SortDirection.Descending:
    //    //        newSortDirection = "DESC";
    //    //        break;
    //    //}

    //    //return newSortDirection;

    //    // SZ [Apr 22, 2013] very bad way of coding above. following line does the same
    //    return sortDirection == SortDirection.Ascending ? "ASC" : "DESC";
    //}
    //protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    //{
    //    BindGrid();
    //}
    //protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    //{
    //   BindGrid();
    //}
    //public void grdIndividual_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //   BindGrid();
    //}
    //public Individual currentIndividual
    //{
    //    get
    //    {
    //        var Indv = new Individual();
    //        if (storeIndividual != null)
    //        {
    //            Indv.Key = storeIndividual.Key;
    //        }
    //        //Indv.FirstName = txtContactFirstName.Text;
    //        //Indv.LastName = txtContactLastName.Text;
    //        //Indv.Birthday = Convert.ToDateTime(txtGNINFODOB);

    //        return Indv;
    //    }
    //    set
    //    {
    //        storeIndividual = value;
    //        //txtFirstName.Text = storeIndividual.FirstName;
    //        //txtLastName.Text = storeIndividual.LastName;
    //        //radDOB.SelectedDate = storeIndividual.Birthday;
    //        //if (storeIndividual.indv_smoking == true)
    //        //    ddlSmokingGeneral.SelectedIndex = 1;
    //        //else
    //        //    ddlSmokingGeneral.SelectedIndex = 0;
    //        //if (storeIndividual.indv_gender == "Male")
    //        //    ddlGeneralGender.SelectedIndex = 0;
    //        //else
    //        //    ddlGeneralGender.SelectedIndex = 1;

    //    }
    //}
    //public Individual currentSpouse
    //{
    //    get
    //    {

    //        var spIndv = new Individual();
    //        if (storeSpouseIndv != null)
    //        {
    //            spIndv.Key = storeSpouseIndv.Key;
    //        }
    //        //spIndv.FirstName = txtSpouseFNameSpouse.Text;
    //        //spIndv.LastName = txtSpouseLNameSpouse.Text;
    //        //spIndv.Birthday = radspDOB.SelectedDate;
    //        //if (ddlGenderSpouse.SelectedIndex == 0)
    //        //{
    //        //    spIndv.indv_gender = "Male";
    //        //}
    //        //else
    //        //    spIndv.indv_gender = "Female";
    //        //if (ddlSmokingSpouse.SelectedIndex == 0)
    //        //    spIndv.indv_smoking = false;
    //        //else
    //        //    spIndv.indv_smoking = true;

    //        return spIndv;
    //    }
    //    set
    //    {
    //        storeSpouseIndv = value;
    //        //txtSpouseFNameSpouse.Text = storeSpouseIndv.FirstName;
    //        //txtSpouseLNameSpouse.Text = storeSpouseIndv.LastName;
    //        //radspDOB.SelectedDate = storeSpouseIndv.Birthday;
    //        //if (storeSpouseIndv.indv_smoking == true)
    //        //    ddlSmokingSpouse.SelectedIndex = 1;
    //        //else
    //        //    ddlSmokingSpouse.SelectedIndex = 0;
    //        //if (storeSpouseIndv.indv_gender == "Male")
    //        //    ddlGenderSpouse.SelectedIndex = 0;
    //        //else
    //        //    ddlGenderSpouse.SelectedIndex = 1;
    //    }
    //}



    //public IndividualDetail currentIndividualDetails
    //{
    //    get
    //    {
    //        var spIndvCurrentDetail = new IndividualDetail();

    //        spIndvCurrentDetail.Individual = currentIndividual;
    //        return spIndvCurrentDetail;
    //    }

    //}

    //public IndividualDetail currentSpouseDetails
    //{
    //    get
    //    {
    //        var spIndvSpouseDetail = new IndividualDetail();

    //        spIndvSpouseDetail.Individual = currentSpouse;
    //        return spIndvSpouseDetail;
    //    }
    //}

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
    //    if (masterPage != null)
    //        masterPage.buttonYes.Click += new EventHandler(CancelOnForm_Click);

    //    //if (!Page.IsPostBack)
    //    //{
    //    //    ShowGrid();
    //    //    BindState();
    //    //    btnDelete.Visible = false;
    //    //    txtFirstName.Visible = false;
    //    //    txtLastName.Visible = false;
    //    //    radDOB.Visible = false;
    //    //}
    //    //BindGrid();
    //    }
    //private void UpdateAccount(long id, long individualId)
    //{
    //    Account account = Engine.AccountActions.Get(id);
    //    this.UpdateAccount(account, individualId);
    //}
    //protected void Save_Click(object sender, EventArgs e)
    //{
    //    if(IsValidated)
    //        SaveForm();
    //}

    //protected void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    if(IsValidated)
    //        SaveForm(true);
    //}


    //protected void btnReturn_Click(object sender, EventArgs e)
    //{
    //    ShowGrid();
    //    //divGrid.Visible = true;
    //    //divForm.Visible = false;
    //    //IsGridMode = true;
    //}
    // must be public to access in the parent form
    //public void CancelOnForm_Click(object sender, EventArgs e)
    //{
    //    ShowGrid();
    //    ctlMessage.Clear();
    //    //divGrid.Visible = true;
    //    //divForm.Visible = false;
    //}

    //protected void grdIndividual_RowDataBound(object sender, GridViewRowEventArgs e)
    //{
    //    if (e.Row.RowType == DataControlRowType.DataRow)
    //    {


    //    }
    //}

    //public void testGridBind()
    //{
    //    List<Individual> lstindv = new List<Individual>();
    //    Individual indv = new Individual();
    //    indv.FirstName = "mainak";
    //    indv.LastName = "pan";
    //    lstindv.Add(indv);

    //    Individual indv1 = new Individual();
    //    indv1.FirstName = "raja";
    //    indv1.FirstName = "sarkar";
    //    lstindv.Add(indv1);

    //    grdIndividual.DataSource = lstindv;
    //    grdIndividual.DataBind();
    //}

    //SZ [Apr 17, 2013] Added for primary and secondard check boxes
    //void SaveForm(bool closeForm = false)
    //{
    //    try
    //    {
    //         if (hdnFieldIsEditMode.Value == "no")
    //        {
    //            Individual P = new Individual();
    //            P.AccountId = AccountID;

    //            //SZ [Apr 22, 2013] Small call to get all the data
    //            GetIndividualInfo(ref P);


    //            //SZ [Jan 23, 2013] since the tabs are disabled now unless the account is saved,
    //            //temporary account is not required as tabs are only available when THERE IS AN ACCOUNT

    //            //if (this.AccountID == 0)
    //            //{
    //            //    Account account = new Account();
    //            //    Engine.AccountActions.Add(account);
    //            //    this.AccountID = account.Key;
    //            //}

    //        //    Individual entity = new Individual
    //        //{
    //        //    AccountId = this.AccountID,
    //        //    FirstName = txtFirstName.Text,
    //        //    LastName = txtLastName.Text,
    //        //    Birthday = radDOB.SelectedDate,
    //        //    Zipcode = txtZipCode.Text,
    //        //    Gender = ddlGender.SelectedValue,
    //        //    Smoking = ddlTobacco.SelectedIndex == 1,

    //        //    DayPhone = txtDayPhone.Text!=string.Empty? Convert.ToInt64(txtDayPhone.Text): (long?)null,
    //        //    EveningPhone = txtEveningPhone.Text != string.Empty ? Convert.ToInt64(txtEveningPhone.Text) : (long?)null,
    //        //    CellPhone = txtCellPhone.Text!=string.Empty? Convert.ToInt64(txtCellPhone.Text): (long?)null,
    //        //    FaxNmbr = txtFax.Text!=string.Empty? Convert.ToInt64(txtFax.Text): (long?)null,

    //        //    Address1 = txtAddress1.Text,
    //        //    Address2 = txtAddress2.Text,
    //        //    City = txtCity.Text,
    //        //    Email = txtEmail.Text,
    //        //    ExternalReferenceID = txtExternalReferenceId.Text,

    //        //    Notes = txtNotes.Text,

    //        //    //IsActive = true,
    //        //    //IsDeleted = false,
    //        //    //AddedOn = DateTime.Now,
    //        //    AddedBy = null //CurrentUser.Key;//Logged In User Id
    //        //    //ChangedOn = set in edit
    //        //    //ChangedBy = set in edit
    //        //};


    //        //    if (ddlStateIndividual.SelectedValue != string.Empty)
    //        //        entity.StateID = byte.Parse(ddlStateIndividual.SelectedValue);


    //            Engine.IndividualsActions.Add(P);
    //            RecordId = P.Key;
    //            hdnFieldIsEditMode.Value = "yes";

    //            //this.UpdateAccount(this.AccountID, P.Key);
    //        }

    //        else if (hdnFieldIsEditMode.Value == "yes")
    //        {
    //            if (hdnFieldEditIndividual.Value != "")
    //            {
    //                var P = Engine.IndividualsActions.Get(RecordId); // Convert.ToInt64(hdnFieldEditIndividual.Value));
    //                //SZ [Apr 22, 2013] Small call to get all the data
    //                GetIndividualInfo(ref P);

    //                //entity.FirstName = txtFirstName.Text;
    //                //entity.LastName = txtLastName.Text;
    //                //entity.Birthday = radDOB.SelectedDate;
    //                //entity.Zipcode =txtZipCode.Text;
    //                //entity.Gender = ddlGender.SelectedValue;
    //                //entity.Smoking = ddlTobacco.SelectedIndex == 1;

    //                //entity.DayPhone = txtDayPhone.Text != string.Empty ? Convert.ToInt64(txtDayPhone.Text) : (long?)null;
    //                //entity.EveningPhone = txtEveningPhone.Text != string.Empty ? Convert.ToInt64(txtEveningPhone.Text) : (long?)null;
    //                //entity.CellPhone = txtCellPhone.Text!=string.Empty? Convert.ToInt64(txtCellPhone.Text): (long?)null;
    //                //entity.FaxNmbr = txtFax.Text!=string.Empty? Convert.ToInt64(txtFax.Text): (long?)null;

    //                //entity.Address1 = txtAddress1.Text;
    //                //entity.Address2 = txtAddress2.Text;
    //                //entity.City = txtCity.Text;
    //                //entity.Email = txtEmail.Text;
    //                //entity.ExternalReferenceID = txtExternalReferenceId.Text;
    //                //if(ddlStateIndividual.SelectedValue!=string.Empty)
    //                //    entity.StateID = byte.Parse(ddlStateIndividual.SelectedValue);

    //                //entity.Notes = txtNotes.Text;

    //                //IsActive = only when add new
    //                //IsDeleted = only when add new
    //                //entity.AddedOn = only when add new
    //                //entity.AddedBy =  only when add new
    //                // entity.ChangedOn = set in DAL
    //                //P.ChangedBy = null;// Last Modified User
    //                string user = (Page as SalesBasePage).CurrentUser.FullName;
    //                Engine.IndividualsActions.Change(P, user);

    //                //this.UpdateAccount(P.AccountId, P.Key);
    //            }
    //        }
    //         //Account account = Engine.AccountActions.Get(id);
    //         UpdateAccount(AccountID, RecordId);
    //         GetIndividualsByAccount(true);

    //         DialPhone(RecordId);

    //        if (!closeForm)
    //        {
    //            ctlMessage.SetStatus(Messages.RecordSavedSuccess);
    //            //lblMessageGrid.Text = "";
    //        }
    //        else
    //        {
    //            ctlMessage.SetStatus(Messages.RecordSavedSuccess);
    //            //lblMessageForm.Text = "";
    //            //lblMessageGrid.Text = "Record saved successful.";
    //            ShowGrid();
    //            //divGrid.Visible = true;
    //            //divForm.Visible = false;
    //            //BindGrid();
    //        }

    //SR
    public string IndividualWindowClientID
    {
        get { return dlgNewIndividual.ClientID; }
    }

    int _tcpaId = 0;
    /// <summary>
    /// Current Outpulse ID
    /// </summary>
    string CurrentOutpulseID
    {
        get
        {
            return hdnCurrentOutPulseID.Value;
        }
        set
        {
            //SR hdnCurrentOutPulseID.Value =  value.ToString();
            hdnCurrentOutPulseID.Value = Helper.SafeConvert<string>(value);
        }
    }
    public void ProcessConsent(string controlId, TCPAConsentType choice)
    {
        switch (choice)
        {
            case TCPAConsentType.Blank: ddlConsent.SelectedValue = "0"; break;
            case TCPAConsentType.No: ddlConsent.SelectedValue = "2"; break;
            case TCPAConsentType.Yes: ddlConsent.SelectedValue = "1"; break;
            case TCPAConsentType.Undefined: ddlConsent.SelectedValue = "3"; break;
        }

        //if (choice==TCPAConsentType.No && !ApplicationSettings.IsTermLife)
        if (choice == TCPAConsentType.No && !Engine.ApplicationSettings.IsTermLife)
        {
            // Sz [Oct 3, 2013] Clear the field that caused the trigger
            System.Web.UI.Control ctl = string.Compare(controlId, "txtDayPhone", true) == 0 ? txtDayPhone :
                string.Compare(controlId, "txtEveningPhone", true) == 0 ? txtEveningPhone :
                string.Compare(controlId, "txtCellPhone", true) == 0 ? txtCellPhone :
                string.Compare(controlId, "txtFax", true) == 0 ? txtFax : null;

            if (ctl != null && ctl is ITextControl)
                (ctl as ITextControl).Text = string.Empty;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {

        //if (ApplicationSettings.InsuranceType == 0)
        if (Engine.ApplicationSettings.InsuranceType == 0)
        {
            btnQuote.Visible = true;
            btnFillForm.Visible = false;
        }
        ctlMessage.SetStatus("");
    }

    public void BindIndividualStatuses()
    {
        ddlIndividualStatuses.DataSource = Engine.IndividualStatusActions.All;
        ddlIndividualStatuses.DataBind();

        ddlIndividualStatuses.Items.Insert(0, new ListItem("-- None --", "-1"));

        ddlIndividualPDPStatuses.DataSource = Engine.IndividualPDPStatusActions.All;
        ddlIndividualPDPStatuses.DataBind();

        ddlIndividualPDPStatuses.Items.Insert(0, new ListItem("-- None --", "-1"));
    }

    public void IndividualChanged(IIndividual handle)
    {
        if (IsGridMode)
            BindGrid();
    }

    public event EventHandler<DialClickedEventArgs> DialClicked;

    // public event EventHandler<IndividualEventArgs> IndividualUpdated;

    void ClearFields()
    {
        RecordId = 0;
        //YA[27 Feb 2014]
        //bool resultForVisibility = (ApplicationSettings.IsTermLife && ApplicationSettings.HasLeadNewLayout);   
        bool resultForVisibility = (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout);
        checkBoxesPrimarySecondary.Visible = !resultForVisibility;
        //grdIndividual.Columns[7].Visible = !resultForVisibility;

        ddlIndividualStatuses.SelectedIndex = 0;
        ddlIndividualPDPStatuses.SelectedIndex = 0;
        txtHRASubsidyAmount.Text = "";

        txtFirstName.Text = string.Empty;
        txtMiddle.Text = string.Empty;
        txtLastName.Text = string.Empty;
        radDOB.SelectedDate = null;
        radDOB2.SelectedDate = null; //kamran
        txtZipCode.Text = string.Empty;
        ddlGender.SelectedIndex = 0;
        ddlTobacco.SelectedIndex = 0;
        txtDayPhone.Text = string.Empty;
        txtEveningPhone.Text = string.Empty;
        txtCellPhone.Text = string.Empty;
        txtFax.Text = string.Empty;

        txtAddress1.Text = string.Empty;
        txtAddress2.Text = string.Empty;
        txtCity.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtExternalReferenceId.Text = string.Empty;

        ddlStateIndividual.DataSource = ((SalesDataPage)Page).USStates; // Engine.Constants.States;
        ddlStateIndividual.DataBind();
        //ddlStateIndividual.Items.Insert(0, new ListItem("--Select State--", "-1"));
        ddlStateIndividual.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlStateIndividual.SelectedIndex = 0;

        ddlAppState.DataSource = ((SalesDataPage)Page).USStates; // Engine.Constants.States;
        ddlAppState.DataBind();
        ddlAppState.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlAppState.SelectedIndex = 0;


        txtNotes.Text = string.Empty;

        cbxPrimary.Checked = Engine.ApplicationSettings.IsMultipleAccountsAllowed;
        cbxSecondary.Checked = false;


        //SZ [Apr 18, 2013] fix for disabled boxes
        cbxPrimary.Enabled = true;
        cbxSecondary.Enabled = true;

        ddlConsent.SelectedIndex = 0;
        chkEmailOptOut.Checked = false;
        //if (ApplicationSettings.IsTermLife)
        if (Engine.ApplicationSettings.IsTermLife)
        {
            txtExternalReferenceId.Enabled = false;
        }
        CheckRequiredFields();
        PopulateOnBoardFields(null);
    }
    //MH:02 August 2014
    private void PopulateOnBoardFields(Individual individual)
    {
        //clear value.
        //chk_indv_ob_billing.Checked = chk_indv_ob_dental.Checked = chk_indv_ob_auto_home_life.Checked = chk_indv_ob_annuity.Checked = chk_indv_ob_inspection.Checked = chk_indv_ob_app_eSign.Checked = false;
        chk_indv_ob_auto_home.Enabled = chk_indv_ob_cs_prep.Enabled = chk_indv_ob_billing.Enabled = chk_indv_ob_dental.Enabled = chk_indv_ob_auto_home_life.Enabled = chk_indv_ob_annuity.Enabled = chk_indv_ob_inspection.Enabled = chk_indv_ob_app_eSign.Enabled = false;
        chk_indv_ob_billing.Visible = chk_indv_ob_annuity.Visible = chk_indv_ob_dental.Visible = chk_indv_ob_auto_home_life.Visible = chk_indv_ob_inspection.Visible = chk_indv_ob_app_eSign.Visible = false;
        lbl_indv_ob_billing.Visible = lbl_indv_ob_dental.Visible = false;
        chk_row_cs_prep_autohome.Visible = false;
        chk_row_onboard_sql.Visible = false;
        chk_row_annuty_autohomelife.Visible = false;
        // assigning values
        chk_indv_ob_dental.Checked = individual.IfNotNull(p => p.OnBoardDental);
        chk_indv_ob_billing.Checked = individual.IfNotNull(p => p.OnBoardBilling);
        chk_indv_ob_auto_home_life.Checked = individual.IfNotNull(p => p.OnBoardAutoHomeLife);
        chk_indv_ob_annuity.Checked = individual.IfNotNull(p => p.OnBoardAnnuity);
        chk_indv_ob_inspection.Checked = individual.IfNotNull(p => p.OnBoardInspection);
        chk_indv_ob_app_eSign.Checked = individual.IfNotNull(p => p.OnBoardApplicationeSign);
        chk_indv_ob_auto_home.Checked = individual.IfNotNull(p => p.OnBoardAutoHome);
        chk_indv_ob_cs_prep.Checked = individual.IfNotNull(p => p.OnBoardCsPrep);
        // setting visibilty based on application mode.
        if (Engine.ApplicationSettings.IsSenior)
        {
            chk_row_cs_prep_autohome.Visible = false;
            chk_row_annuty_autohomelife.Visible = true;
            if (SalesPage.CurrentUser.IsOnboardType == true)
            {
                chk_indv_ob_dental.Enabled = chk_indv_ob_auto_home_life.Enabled = chk_indv_ob_annuity.Enabled = true;
            }
            lbl_indv_ob_dental.Visible = chk_indv_ob_annuity.Visible = chk_indv_ob_dental.Visible = chk_indv_ob_auto_home_life.Visible = true;

        }
        else if (Engine.ApplicationSettings.IsAutoHome)
        {
            chk_row_cs_prep_autohome.Visible = true;
            chk_row_annuty_autohomelife.Visible = false;
            if (SalesPage.CurrentUser.IsOnboardType == true)
            {
                chk_indv_ob_billing.Enabled = chk_indv_ob_inspection.Enabled = chk_indv_ob_app_eSign.Enabled = true;
            }
            lbl_indv_ob_billing.Visible = chk_indv_ob_billing.Visible = chk_indv_ob_inspection.Visible = chk_indv_ob_app_eSign.Visible = true;
        }
        else if (Engine.ApplicationSettings.IsTermLife)
        {
            chk_indv_ob_dental.Visible = lbl_indv_ob_dental.Visible = chk_row_onboard_sql.Visible = true;
            if (SalesPage.CurrentUser.IsOnboardType == true)
            {
                chk_indv_ob_dental.Enabled = chk_indv_ob_auto_home.Enabled = chk_indv_ob_cs_prep.Enabled = true;
            }

        }
    }

    void SetIndividualInfo(Individual I)
    {
        RecordId = I.Key;

        if (!I.IndividualStatusID.HasValue)
            ddlIndividualStatuses.SelectedIndex = 0;
        else
            ddlIndividualStatuses.SelectedValue = I.IndividualStatusID.Value.ToString();

        if (!I.IndividualPDPStatusID.HasValue)
            ddlIndividualPDPStatuses.SelectedIndex = 0;
        else
            ddlIndividualPDPStatuses.SelectedValue = I.IndividualPDPStatusID.Value.ToString();

        if (I.HRASubsidyAmount.HasValue)
            txtHRASubsidyAmount.Text = I.HRASubsidyAmount.Value.ToString();
        else
            txtHRASubsidyAmount.Text = string.Empty;
        txtFirstName.Text = I.FirstName;
        txtMiddle.Text = I.MiddleName;
        txtLastName.Text = I.LastName;
        radDOB.SelectedDate = I.Birthday;
        radDOB2.SelectedDate = I.indv_ap_date; //kamran
        txtZipCode.Text = I.Zipcode;
        chkEmailOptOut.Checked = I.IndividualEmailOptOut;
        //SZ [Apr 26, 2013] this is a hack. Drop Downs should never have strings as values.
        if (!string.IsNullOrEmpty(I.Gender))
        {
            //ddlGender.SelectedIndex = String.Compare("Male", I.Gender, StringComparison.OrdinalIgnoreCase) == 0 || !string.IsNullOrEmpty(I.Gender)
            //                              ? 1
            //                              : 2;
            //IH 05.10-13
            //ddlGender.SelectedIndex = String.Compare("Male", I.Gender, StringComparison.OrdinalIgnoreCase) == 0 || !string.IsNullOrEmpty(I.Gender)
            //                              ? ddlGender.Items.IndexOf(ddlGender.Items.FindByText("Male"))
            //                              : ddlGender.Items.IndexOf(ddlGender.Items.FindByText("Female"));

            //[MH: 17 Jan 2014]
            var item = ddlGender.Items.FindByText(I.Gender.Trim());
            if (item != null)
            {
                int i = ddlGender.Items.IndexOf(item);
                ddlGender.SelectedIndex = i;
            }
        }
        //ddlTobacco.SelectedIndex = I.Smoking == true ? 1 : 0;
        //IH 05.10-13
        ddlTobacco.SelectedIndex = I.Smoking == null ? 0 : (I.Smoking == true ? ddlTobacco.Items.IndexOf(ddlTobacco.Items.FindByText("Yes")) : ddlTobacco.Items.IndexOf(ddlTobacco.Items.FindByText("No")));
        txtDayPhone.Text = I.DayPhone.HasValue ? I.DayPhone.Value.ToString() : string.Empty;
        txtEveningPhone.Text = I.EveningPhone.HasValue ? I.EveningPhone.ToString() : string.Empty;
        txtCellPhone.Text = I.CellPhone.HasValue ? I.CellPhone.ToString() : string.Empty;
        txtFax.Text = I.FaxNmbr.HasValue ? I.FaxNmbr.ToString() : string.Empty;

        txtAddress1.Text = I.Address1;
        txtAddress2.Text = I.Address2;
        txtCity.Text = I.City;
        txtEmail.Text = I.Email;
        txtExternalReferenceId.Text = I.ExternalReferenceID;

        if (I.StateID.HasValue) //SZ [Apr 26, 2013] fixed the issue of null value
            ddlStateIndividual.SelectedValue = I.StateID.Value.ToString();
        if (I.ApplicationState.HasValue) //SZ [Apr 26, 2013] fixed the issue of null value
            ddlAppState.SelectedValue = I.ApplicationState.Value.ToString();


        txtNotes.Text = I.Notes;

        cbxPrimary.Checked = Engine.AccountActions.IsIndividual(AccountID, IndividualType.Primary, I.Key);
        cbxSecondary.Checked = Engine.AccountActions.IsIndividual(AccountID, IndividualType.Secondary, I.Key);



        PopulateOnBoardFields(I);
        //YA[Jan 29, 2014]
        CheckRequiredFields();

        // SZ [Sep 22, 2013] added for the TCPA stuff
        switch (I.HasConsent)
        {
            case TCPAConsentType.Blank: ddlConsent.SelectedValue = "0"; break;
            case TCPAConsentType.No: ddlConsent.SelectedValue = "2"; break;
            case TCPAConsentType.Yes: ddlConsent.SelectedValue = "1"; break;
            case TCPAConsentType.Undefined: ddlConsent.SelectedValue = "3"; break;
        }

        //ddlConsent.SelectedValue = ((int)I.HasConsent).ToString();
        lblConsent.Visible = cbxPrimary.Checked;
        ddlConsent.Visible = cbxPrimary.Checked;

        //SZ [Apr 18, 2013] fix  for feature request for mantis #117
        if (!cbxPrimary.Checked && !cbxSecondary.Checked)
        {
            cbxPrimary.Enabled = true;
            cbxSecondary.Enabled = true;
        }
        else
        {
            cbxPrimary.Enabled = !cbxPrimary.Checked;
            cbxSecondary.Enabled = !cbxSecondary.Checked;
            //TM [May 27, 2014] Fix  for error when clicking the enableb checkbox when other is checked and disabled
            if (!cbxPrimary.Enabled || !cbxSecondary.Enabled)
            {
                cbxPrimary.Enabled = false;
                cbxSecondary.Enabled = false;
            }

        }

        DialPhone(I.Key);
        // life insurance arc
        //if(ApplicationSettings.IsTermLife)
        if (Engine.ApplicationSettings.IsTermLife)
        {
            tblRowArcLifeInsurance.Visible = true;
            chkHasExistingIssurance.Checked = I.indv_exist_ins == "Y" || I.indv_exist_ins == "1";
            txtExistingInsuranceAmount.Text = I.indv_exist_ins_amt != null ? I.indv_exist_ins_amt.Value.ToString("#.##") : "";
            chkExistingInsuranceReplacement.Checked = I.indv_exist_ins_rplc == "Y" || I.indv_exist_ins_rplc == "1";

            TxtDesiredInsuranceAmount.Text = I.indv_desire_ins_amt.HasValue ? I.indv_desire_ins_amt.Value.ToString("#.##") : "";
            txtDesirAltAmount.Text = I.indv_desire_ins_alt_amt.HasValue ? I.indv_desire_ins_alt_amt.Value.ToString("#.##") : "";

            Helper.SafeAssignSelectedValueToDropDown(ddlDesireInsuranceTerms, I.indv_desire_ins_term.HasValue ? I.indv_desire_ins_term.Value.ToString() : "");

        }
        else
        {
            tblRowArcLifeInsurance.Visible = false;
        }
    }

    private void CheckRequiredFields()
    {
        //YA[Jan 29, 2014]
        //if (cbxPrimary.Checked && ApplicationSettings.IsTermLife)
        if (cbxPrimary.Checked && Engine.ApplicationSettings.IsTermLife)
        {
            var enabled = CheckNewForSplit;
            txtAddress1.Enabled = enabled;
            txtAddress2.Enabled = enabled;
            txtEmail.Enabled = enabled;
            txtCity.Enabled = enabled;
            txtZipCode.Enabled = enabled;
            ddlStateIndividual.Enabled = enabled;
            //ddlAppState.Enabled = false;

            vldReqradDOB.Enabled = cbxPrimary.Checked;
            //vldReqradDOB2.Enabled = cbxPrimary.Checked; //kamran
            vldReqGender.Enabled = cbxPrimary.Checked;
            vldLastName.Enabled = cbxPrimary.Checked;
            vldFirstName.Enabled = cbxPrimary.Checked;
        }
        else
        {
            txtAddress1.Enabled = true;
            txtAddress2.Enabled = true;
            txtEmail.Enabled = true;
            txtCity.Enabled = true;
            txtZipCode.Enabled = true;
            ddlStateIndividual.Enabled = true;
            ddlAppState.Enabled = true;

            vldReqradDOB.Enabled = false;
            //vldReqradDOB2.Enabled = false;
            vldReqGender.Enabled = false;
            vldLastName.Enabled = false;
            vldFirstName.Enabled = false;
        }
    }

    void GetIndividualInfo(ref Individual P)
    {
        // SZ [Apr 22, 2013] This function has been added for easy access for setting& getting properties

        P.IndividualStatusID = string.IsNullOrWhiteSpace(ddlIndividualStatuses.SelectedValue) ||
                               ddlIndividualStatuses.SelectedValue == "-1"
                                   ? (int?)null
                                   : Convert.ToInt32(ddlIndividualStatuses.SelectedValue);

        P.IndividualPDPStatusID = string.IsNullOrWhiteSpace(ddlIndividualPDPStatuses.SelectedValue) ||
                                  ddlIndividualPDPStatuses.SelectedValue == "-1"
                                      ? (int?)null
                                      : Convert.ToInt32(ddlIndividualPDPStatuses.SelectedValue);

        P.HRASubsidyAmount = Helper.SafeConvert<decimal>(txtHRASubsidyAmount.Text);

        P.FirstName = txtFirstName.Text;
        P.MiddleName = txtMiddle.Text;
        P.LastName = txtLastName.Text;
        P.Birthday = radDOB.SelectedDate;
        P.indv_ap_date = radDOB2.SelectedDate;
        P.Zipcode = txtZipCode.Text;
        P.Gender = ddlGender.SelectedValue;
        P.Smoking = ddlTobacco.SelectedIndex == 2 ? true : ((ddlTobacco.SelectedIndex == 1) ? false : default(bool?));

        P.DayPhone = txtDayPhone.Text != string.Empty ? Convert.ToInt64(txtDayPhone.Text) : (long?)null;
        P.EveningPhone = txtEveningPhone.Text != string.Empty ? Convert.ToInt64(txtEveningPhone.Text) : (long?)null;
        P.CellPhone = txtCellPhone.Text != string.Empty ? Convert.ToInt64(txtCellPhone.Text) : (long?)null;
        P.FaxNmbr = txtFax.Text != string.Empty ? Convert.ToInt64(txtFax.Text) : (long?)null;

        P.Address1 = txtAddress1.Text;
        P.Address2 = txtAddress2.Text;
        P.City = txtCity.Text;
        P.Email = txtEmail.Text;

        // WM - 19.07.2013
        P.Notes = txtNotes.Text;

        P.ExternalReferenceID = txtExternalReferenceId.Text;
        if (ddlStateIndividual.SelectedValue != string.Empty && ddlStateIndividual.SelectedValue != "-1")
            P.StateID = byte.Parse(ddlStateIndividual.SelectedValue);

        if (ddlAppState.SelectedValue != string.Empty && ddlAppState.SelectedValue != "-1")
            P.ApplicationState = byte.Parse(ddlAppState.SelectedValue);


        P.IndividualEmailOptOut = chkEmailOptOut.Checked;
        //if (ApplicationSettings.IsTermLife)
        if (Engine.ApplicationSettings.IsTermLife)
        {
            P.indv_exist_ins = chkHasExistingIssurance.Checked ? "Y" : "N";
            decimal amout;
            if (decimal.TryParse(txtExistingInsuranceAmount.Text, out amout))
            {
                P.indv_exist_ins_amt = amout;
            }
            P.indv_exist_ins_rplc = chkExistingInsuranceReplacement.Checked ? "Y" : "N";

            decimal desireAmount;
            if (decimal.TryParse(TxtDesiredInsuranceAmount.Text, out desireAmount))
            {
                P.indv_desire_ins_amt = desireAmount;
            }
            decimal desireAmountAlt;
            if (decimal.TryParse(txtDesirAltAmount.Text, out desireAmountAlt))
            {
                P.indv_desire_ins_alt_amt = desireAmountAlt;
            }
            int i;
            if (int.TryParse(ddlDesireInsuranceTerms.SelectedValue, out i))
            {
                if (i == -1)
                {
                    P.indv_desire_ins_term = null;
                }
                else
                {
                    P.indv_desire_ins_term = i;
                }

            }
            else
            {
                P.indv_desire_ins_term = null;
            }

        }
        if (SalesPage.CurrentUser.IsOnboardType == true)
        {
            P.OnBoardAnnuity = chk_indv_ob_annuity.Checked;
            P.OnBoardInspection = chk_indv_ob_inspection.Checked;
            P.OnBoardAutoHomeLife = chk_indv_ob_auto_home_life.Checked;
            P.OnBoardApplicationeSign = chk_indv_ob_app_eSign.Checked;
            P.OnBoardDental = chk_indv_ob_dental.Checked;
            P.OnBoardBilling = chk_indv_ob_billing.Checked;
            P.OnBoardCsPrep = chk_indv_ob_cs_prep.Checked;
            P.OnBoardAutoHome = chk_indv_ob_auto_home.Checked;
        }
    }

    void CommandRouter(string command, long Id)
    {
        switch (command)
        {
            case "EditX": Edit(Id); break;
            case "DeleteX": Delete(Id); break;
            case "ViewX": Edit(Id); ReadOnly = true; break;
            case "QuoteX": Quote(Id); break;
            case "FillFormX": FillForm(Id); break;
        }
        //long Id = 
        //GridViewRow row = ((e.CommandSource as Control).NamingContainer as GridViewRow).RowIndex;
        //String dataKeyValue = grdIndividual.DataKeys[((e.CommandSource as Control).NamingContainer as GridViewRow).RowIndex].Value.ToString();
        //long Id = default(long);
        //long.TryParse(dataKeyValue, out Id);
        //ctlMessage.Clear();
        //lblMessageForm.Text = "";
        //lblMessageGrid.Text = "";
        //{
        //    //IsGridMode = false;
        //    ClearFields();
        //    //SZ [Apr 22, 2013] Wrong place to do convertion
        //    //String dataKeyValue = grdIndividual.DataKeys[row.RowIndex].Value.ToString();
        //    hdnFieldIsEditMode.Value = "yes";
        //    SetIndividualInfo(AccountID, Id);
        //    //hdnFieldEditIndividual.Value = dataKeyValue;
        //    ShowGrid(false);
        //    //divGrid.Visible = false;
        //    //divForm.Visible = true;
        //    //Individual entity = Engine.IndividualsActions.Get(RecordId); // iConvert.ToInt64(dataKeyValue));
        //    ////Account account = Engine.AccountActions.Get(entity.AccountId);
        //    //txtFirstName.Text = entity.FirstName;
        //    //txtLastName.Text = entity.LastName;
        //    //radDOB.SelectedDate = entity.Birthday;
        //    //txtZipCode.Text = entity.Zipcode; //Helper.ConvertToInt(entity.Zipcode) <= 0 ? "" : entity.Zipcode.ToString();
        //    //ddlGender.SelectedValue = (entity.Gender == "" || entity.Gender == null) ? "Male" : entity.Gender;
        //    //ddlTobacco.SelectedIndex = entity.Smoking == true ? 1 : 0;
        //    //txtDayPhone.Text = (entity.DayPhone?? default(long)).ToString();
        //    //txtEveningPhone.Text = (entity.EveningPhone ?? default(long)).ToString();
        //    //txtCellPhone.Text = (entity.CellPhone ?? default(long)).ToString(); 
        //    //txtFax.Text = (entity.FaxNmbr ?? default(long)).ToString();
        //    //txtAddress1.Text = entity.Address1;
        //    //txtAddress2.Text = entity.Address2;
        //    //txtCity.Text = entity.City;
        //    //txtEmail.Text = entity.Email;
        //    //txtExternalReferenceId.Text = entity.ExternalReferenceID;
        //    //try
        //    //{
        //    //    ddlStateIndividual.SelectedValue = entity.StateID.ToString();
        //    //}
        //    //catch { }
        //    //txtNotes.Text = entity.Notes;
        //    //cbxPrimary.Checked = Engine.AccountActions.IsIndividual(accId, IndividualType.Primary, entity.Key);
        //    //cbxSecondary.Checked = Engine.AccountActions.IsIndividual(accId, IndividualType.Secondary, entity.Key);
        //    ////SZ [Apr 18, 2013] fix  for feature request for mantis #117
        //    //if (!cbxPrimary.Checked && !cbxSecondary.Checked)
        //    //{
        //    //    cbxPrimary.Enabled = true;
        //    //    cbxSecondary.Enabled = true;
        //    //}
        //    //else
        //    //{
        //    //    cbxPrimary.Enabled = !cbxPrimary.Checked;
        //    //    cbxSecondary.Enabled = !cbxSecondary.Checked;
        //    //}
        //    //DialPhone();
        //}
        //else if (e.CommandName == "DeleteX")
        //{
        //    //GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
        //    //String dataKeyValue = grdIndividual.DataKeys[row.RowIndex].Value.ToString();
        //    Engine.IndividualsActions.Delete(Id);
        //    GetIndividualsByAccount(true);
        //    //lblMessageGrid.Text = "Record delete successful.";
        //    ctlMessage.SetStatus(Messages.RecordDeletedSuccess);
        //    BindGrid();
        //}
    }

    void Add()
    {
        //if (ApplicationSettings.IsTermLife && ApplicationSettings.HasLeadNewLayout)
        if (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout)
        {
            Session[Konstants.K_LEAD_INDIVIDUAL_ADD_NEW_ACCOUNT] = "true";
            Account oldAccount = Engine.AccountActions.Get(AccountID, false);
            long parentAccountID = oldAccount.Key;
            if (oldAccount.AccountParent.HasValue)
                parentAccountID = Engine.AccountActions.Get(oldAccount.AccountParent.Value, false).Key;
            Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=-1&parentaccountid=" + parentAccountID.ToString());
            //CreateNewAccount();
        }
        else
        {
            ClearFields();
            ShowGrid(false);
            //TM[May 23, 2014]
            if (Engine.ApplicationSettings.IsAutoHome || Engine.ApplicationSettings.IsSenior)
            {
                if (!Engine.AccountActions.HasAccountSecondary(AccountID))
                {
                    cbxPrimary.Checked = false;

                    cbxSecondary.Checked = true;
                    cbxSecondary.Enabled = false;
                    cbxPrimary.Enabled = false;
                }
            }
        }
    }

    private void CreateNewAccount()
    {
        Account nAccount = Engine.AccountActions.Add(new Account());
        Account oldAccount = Engine.AccountActions.Get(AccountID, true);

        long parentAccountID = oldAccount.Key;
        if (oldAccount.AccountParent.HasValue)
            parentAccountID = Engine.AccountActions.Get(oldAccount.AccountParent.Value, false).Key;

        nAccount.AssignedCsrKey = oldAccount.AssignedCsrKey;
        nAccount.AssignedUserKey = oldAccount.AssignedUserKey;
        nAccount.TransferUserKey = oldAccount.TransferUserKey;
        nAccount.Notes = oldAccount.Notes;
        nAccount.ExternalAgent = oldAccount.ExternalAgent;
        nAccount.LifeInfo = oldAccount.LifeInfo;
        nAccount.AddedBy = SalesPage.CurrentUser.FullName;


        Engine.AccountActions.Update(nAccount);

        Lead oldLead = Engine.LeadsActions.Get(oldAccount.PrimaryLeadKey ?? -1);
        Lead nlead = oldLead.Duplicate();

        nlead.AccountId = nAccount.Key;
        nlead.AddedBy = SalesPage.CurrentUser.Added.By;
        nlead.AddedOn = DateTime.Now;

        var U = Engine.LeadsActions.Add(nlead);
        nAccount.PrimaryLeadKey = U == null ? default(long) : U.Key;
        nAccount.AccountParent = parentAccountID;
        Engine.AccountActions.Update(nAccount);

        //Individual IPrimary = new Individual();
        //IPrimary.AccountId = nAccount.Key;
        //IPrimary = Engine.IndividualsActions.Add(IPrimary);
        //Engine.AccountActions.SetIndividual(nAccount.Key, IndividualType.Primary, IPrimary.Key);
        Session[Konstants.K_LEAD_INDIVIDUAL_ADD_NEW_ACCOUNT] = "true";

        AccountLog(nAccount.Key, string.Format("An account has been created. Url is {0}", Request.ToString()));

        Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + nAccount.Key.ToString());
    }
    public void Edit(long id)
    {
        ClearFields();

        //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
        //if (masterPage != null)
        //{
        //    masterPage.DirtyFlag = false;
        //}

        Individual I = Engine.IndividualsActions.Get(id);
        long? indvAccountID = 0;
        if (I != null)
            indvAccountID = I.AccountId;
        //if (ApplicationSettings.IsTermLife && ApplicationSettings.HasLeadNewLayout && AccountID != indvAccountID)
        if (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout && AccountID != indvAccountID)
        {
            Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_MODE] = "true";
            Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_KEY] = id.ToString();
            Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + indvAccountID.ToString() + "&" + Konstants.K_AVOID_REASSIGNMENT + "=true");
        }
        else
        {
            Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_MODE] = "false";
            Session[Konstants.K_LEAD_INDIVIDUAL_EDIT_KEY] = id.ToString();
            SetIndividualInfo(I);
            ShowGrid(false);
        }
        //Quote(id);
    }
    void Delete(long id)
    {
        try
        {
            Individual nIndividual = Engine.IndividualsActions.Get(id);
            bool IsPrimary = Engine.AccountActions.IsIndividual(AccountID, IndividualType.Primary, nIndividual.Key);
            if (!IsPrimary)
            {
                Engine.IndividualsActions.Delete(id);
                BindGrid();
                //SR   ctlDeleteMessage.SetStatus(Messages.RecordDeletedSuccess); //lblMessageGrid.Text = ;

                if (Page is IIndividual)
                    (Page as IIndividual).UpdateIndividuals();
            }
            else
            {
                ctlDeleteMessage.SetStatus(Messages.RecordPrimaryIndividualDeleteFail);
            }

        }
        catch (Exception ex)
        {
            ctlDeleteMessage.SetStatus(ex);// lblMessageForm.Text = "Error: " + ex.Message;
        }
    }

    void Quote(Int64 quoteId = 0)
    {
        StringBuilder popupScript = new StringBuilder();
        Account A = Engine.AccountActions.Get(AccountID);

        var user = Engine.UserActions.Get(A.AssignedUserKey != null ? A.AssignedUserKey.Value : new Guid("00000000-0000-0000-0000-000000000000"));
        String agentEmail = user != null ? user.Email : "";
        String agentPosition = user != null ? user.Position : "";
        String agentName = user != null ? user.FullName : "";
        String agentWorkPhone = user != null ? user.WorkPhone : "";
        if (quoteId > 0)
        {
            Individual I = Engine.IndividualsActions.Get(quoteId);
            string stateAbbreviation = string.Empty;
            if (I.StateID != null)
            {
                var stateAbb = ((SalesDataPage)Page).USStates.Where(x => x.Id == Convert.ToByte(I.StateID));
                if (stateAbb != null)
                    stateAbbreviation = stateAbb.First().Abbreviation;
            }
            //string urlEncode = @"http://quoteengine.selectquotesenior.com/default.aspx?firstname=" + Server.UrlEncode(I.FirstName) + "&lastname=" + Server.UrlEncode(I.LastName) + "&adr1=" + Server.UrlEncode(I.Address1) + "&adr2=" + Server.UrlEncode(I.Address2) + "&City=" + Server.UrlEncode(I.City) + "&state=" + Server.UrlEncode(stateAbbreviation) + "&zip=" + Server.UrlEncode(I.Zipcode) + "&birthdate=" + I.Birthday + "&email=" + Server.UrlEncode(I.Email) + "&gender=" + Server.UrlEncode(I.Gender) + "&tobacco=" + I.Smoking.HasValue ?? I.Smoking.Value + "&agent_email=" + Server.UrlEncode(agentEmail) + "&agent_title=" + Server.UrlEncode(agentPosition) + "&agent_phone=" + agentWorkPhone + "&agent_name=" + Server.UrlEncode(agentName);

            //TM [28 may, 2014] Quote Link opens different form,
            //If the individual primary or secondary and the "APPLICATION_BUTTON_QUOTE" [bvalue] == true in "Application_Storage"
            string urlEncode = string.Empty;
            urlEncode = Engine.ApplicationSettings.ButtonQuoteURL + "?accountid=" + A.Key;
            //if (Engine.ApplicationSettings.HasButtonQuote)
            //{
            //    if (A.PrimaryIndividualId == I.Key || A.SecondaryIndividualId == I.Key)
            //    {
            //        urlEncode = Engine.ApplicationSettings.ButtonQuoteURL + "?accountid=" + A.Key;
            //            //@"http://aqe.condadogroup.com/ApplicantProfile.aspx?accountid=" + A.Key;
            //    }
            //    else
            //    {
            //        urlEncode = Engine.ApplicationSettings.ButtonQuoteURL + "?indvidualid=" + I.Key;
            //            //@"http://aqe.condadogroup.com/ApplicantProfile.aspx?indvidualid=" + I.Key;
            //    }
            //}
            //else
            //{
            //    urlEncode = @"http://quoteengine.selectquotesenior.com/default.aspx?firstname=" + Server.UrlEncode(I.FirstName) + "&lastname=" + Server.UrlEncode(I.LastName) + "&adr1=" + Server.UrlEncode(I.Address1) + "&adr2=" + Server.UrlEncode(I.Address2) + "&City=" + Server.UrlEncode(I.City) + "&state=" + Server.UrlEncode(stateAbbreviation) + "&zip=" + Server.UrlEncode(I.Zipcode) + "&birthdate=" + I.Birthday + "&email=" + Server.UrlEncode(I.Email) + "&gender=" + Server.UrlEncode(I.Gender) + "&tobacco=" + I.Smoking.HasValue + "&agent_email=" + Server.UrlEncode(agentEmail) + "&agent_title=" + Server.UrlEncode(agentPosition) + "&agent_phone=" + agentWorkPhone + "&agent_name=" + Server.UrlEncode(agentName);
            //}
            popupScript.Append("<script language='JavaScript'> window.open('" + urlEncode + "' , '');</script>");
        }
        //if (RecordId > 0)
        else
        {
            SavePerson();
            var stateAbb = (Page as SalesDataPage).USStates.Where(x => x.Id == Convert.ToByte(ddlStateIndividual.SelectedValue != "-1" ? ddlStateIndividual.SelectedValue : "1"));
            String state = stateAbb != null ? stateAbb.First().Abbreviation : "AL";
            Individual I = Engine.IndividualsActions.Get(RecordId);
            //TM [28 may, 2014] Quote Link opens different form,
            //If the individual primary or secondary and the "APPLICATION_BUTTON_QUOTE" [bvalue] == true in "Application_Storage"

            string urlEncode = string.Empty;
            urlEncode = Engine.ApplicationSettings.ButtonQuoteURL + "?accountid=" + A.Key;
            //if (Engine.ApplicationSettings.HasButtonQuote)
            //{
            //    if (A.PrimaryIndividualId == I.Key || A.SecondaryIndividualId == I.Key)
            //    {
            //        urlEncode = urlEncode = Engine.ApplicationSettings.ButtonQuoteURL + "?accountid=" + A.Key;
            //            //@"http://aqe.condadogroup.com/ApplicantProfile.aspx?accountid=" + A.Key;
            //    }
            //    else
            //    {
            //        urlEncode = Engine.ApplicationSettings.ButtonQuoteURL + "?indvidualid=" + I.Key;
            //            //@"http://aqe.condadogroup.com/ApplicantProfile.aspx?indvidualid=" + I.Key;
            //    }
            //}
            //else
            //{                
            //    urlEncode = @"http://quoteengine.selectquotesenior.com/default.aspx?firstname=" + Server.UrlEncode(txtFirstName.Text) + "&lastname=" + Server.UrlEncode(txtLastName.Text) + "&adr1=" + Server.UrlEncode(txtAddress1.Text) + "&adr2=" + Server.UrlEncode(txtAddress2.Text) + "&City=" + Server.UrlEncode(txtCity.Text) + "&state=" + Server.UrlEncode(state) + "&zip=" + Server.UrlEncode(txtZipCode.Text) + "&birthdate=" + radDOB.SelectedDate + "&email=" + Server.UrlEncode(txtEmail.Text) + "&birthdate=" + radDOB.SelectedDate + "&gender=" + ddlGender.SelectedValue + "&tobacco=" + ddlTobacco.SelectedValue + "&agent_email=" + Server.UrlEncode(agentEmail) + "&agent_title=" + Server.UrlEncode(agentPosition) + "&agent_phone=" + Server.UrlEncode(agentWorkPhone) + "&agent_name=" + Server.UrlEncode(agentName);
            //}

            popupScript.Append("<script language='JavaScript'> window.open('" + urlEncode + "' , '');</script>");
        }

        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "PopupScript", popupScript.ToString(), false);
        //else
        //SZ [Jul 18, 2013] This code below chnaged due to the following warning
        //warning CS0618: 'System.Web.UI.Page.RegisterStartupScript(string, string)' is obsolete: 
        //The recommended alternative is ClientScript.RegisterStartupScript(Type type, string key, string script). 
        //http://go.microsoft.com/fwlink/?linkid=14202'
        //Page.RegisterStartupScript("PopupScript", popupScript.ToString());
        Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript.ToString());
    }

    void FillForm(Int64 quoteId = 0)
    {
        //IH 05.10.13 Optimized the statement
        StringBuilder popupScript = new StringBuilder();
        Account A = Engine.AccountActions.Get(AccountID);

        var user = Engine.UserActions.Get(A.AssignedUserKey != null ? A.AssignedUserKey.Value : new Guid("00000000-0000-0000-0000-000000000000"));
        if (user == null) return;
        String agentEmail = user.Email;
        String agentPosition = user.Position;
        String agentName = user.FullName;
        String agentWorkPhone = user.WorkPhone;

        if (quoteId > 0)
        {
            Individual I = Engine.IndividualsActions.Get(quoteId);
            string stateAbbreviation = string.Empty;
            if (I != null && I.StateID != null)
            {
                var stateAbb = ((SalesDataPage)Page).USStates.Where(x => x.Id == Convert.ToByte(I.StateID));
                if (stateAbb != null) stateAbbreviation = stateAbb.First().Abbreviation;
            }
            string urlEncode = @"http://fillform.selectquotesenior.com/default.aspx?firstname=" +
                               Server.UrlEncode(I.FirstName) + "&lastname=" + Server.UrlEncode(I.LastName) +
                               "&adr1=" + Server.UrlEncode(I.Address1) + "&adr2=" + Server.UrlEncode(I.Address2) +
                               "&City=" + Server.UrlEncode(I.City) + "&state=" + Server.UrlEncode(stateAbbreviation) +
                               "&zip=" + Server.UrlEncode(I.Zipcode) + "&birthdate=" + I.Birthday + "&email=" +
                               Server.UrlEncode(I.Email) + "&gender=" + Server.UrlEncode(I.Gender) + "&tobacco=" +
                               I.Smoking + "&agent_email=" + Server.UrlEncode(agentEmail) + "&agent_title=" +
                               Server.UrlEncode(agentPosition) + "&agent_phone=" + agentWorkPhone + "&agent_name=" +
                               Server.UrlEncode(agentName);
            popupScript.Append("<script language='JavaScript'> window.open('" + urlEncode + "' , '');</script>");
        }
        //if (RecordId > 0)
        else
        {
            SavePerson();
            var stateAbb =
                ((SalesDataPage)Page).USStates.Where(
                    x =>
                    x.Id ==
                    Convert.ToByte(ddlStateIndividual.SelectedValue != "-1" ? ddlStateIndividual.SelectedValue : "1"));
            String state = stateAbb != null ? stateAbb.First().Abbreviation : "AL";
            Individual I = Engine.IndividualsActions.Get(RecordId);
            string urlEncode = @"http://fillform.selectquotesenior.com/default.aspx?firstname=" +
                               Server.UrlEncode(txtFirstName.Text) + "&lastname=" +
                               Server.UrlEncode(txtLastName.Text) + "&adr1=" + Server.UrlEncode(txtAddress1.Text) +
                               "&adr2=" + Server.UrlEncode(txtAddress2.Text) + "&City=" +
                               Server.UrlEncode(txtCity.Text) + "&state=" + Server.UrlEncode(state) + "&zip=" +
                               Server.UrlEncode(txtZipCode.Text) + "&birthdate=" + radDOB.SelectedDate + "&email=" +
                               Server.UrlEncode(txtEmail.Text) + "&birthdate=" + radDOB.SelectedDate + "&gender=" +
                               ddlGender.SelectedValue + "&tobacco=" + ddlTobacco.SelectedValue + "&agent_email=" +
                               Server.UrlEncode(agentEmail) + "&agent_title=" + Server.UrlEncode(agentPosition) +
                               "&agent_phone=" + agentWorkPhone + "&agent_name=" + Server.UrlEncode(agentName);
            popupScript.Append("<script language='JavaScript'> window.open('" + urlEncode + "' , '');</script>");
        }

        ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "PopupScript", popupScript.ToString(), false);
        //else
        //SZ [Jul 18, 2013] This code below chnaged due to the following warning
        //warning CS0618: 'System.Web.UI.Page.RegisterStartupScript(string, string)' is obsolete: 
        //The recommended alternative is ClientScript.RegisterStartupScript(Type type, string key, string script). 
        //http://go.microsoft.com/fwlink/?linkid=14202'
        Page.ClientScript.RegisterStartupScript(this.GetType(), "PopupScript", popupScript.ToString());
        //Page.RegisterStartupScript("PopupScript", popupScript.ToString());
    }

    public event EventHandler<ItemEventArgs<Account>> SaveSplitAccount;
    bool CheckNewForSplit
    {
        get
        {
            var actId = Request.ReadQueryStringAs<long?>(Konstants.K_ACCOUNT_ID);
            var parentActId = Request.ReadQueryStringAs<long?>(Konstants.K_PARENT_ACCOUNT_ID);
            return actId.HasValue && actId.Value < 0 && parentActId.HasValue;
        }
    }
    void SavePerson()
    {
        if (CheckNewForSplit)
        {
            long? ActKey = default(long?);
            try
            {
                Engine.BeginTransaction();
                var act = new Account();
                act.AccountParent = Request.ReadQueryStringAs<long?>(Konstants.K_PARENT_ACCOUNT_ID);
                if (SaveSplitAccount != null)
                    SaveSplitAccount(this, new ItemEventArgs<Account>(act));
                var l = act.Leads.FirstOrDefault();
                Engine.AccountActions.Add(act);
                act.PrimaryLeadKey = l.Key;
                var Indv = new Individual
                {
                    AccountId = act.Key,
                    AddedBy = SalesPage.CurrentUser.FullName,
                    AddedOn = DateTime.Now,
                    IsActive = true,
                    IsDeleted = false
                };
                GetIndividualInfo(ref Indv);
                Indv.SetConsent(ddlConsent.SelectedValue);
                RecordId = Engine.IndividualsActions.Add(Indv).Key;
                act.PrimaryIndividualId = Indv.Key;
                ActKey = act.Key;
                Engine.AccountActions.Update(act);
                Engine.Commit();

            }
            catch (Exception e)
            {
                Engine.Rollback();
                ActKey = default(long?);
                ctlMessage.SetStatus(e);
            }
            if (ActKey.HasValue)
            {

                Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + ActKey + "&" + Konstants.K_AVOID_REASSIGNMENT +
                                  "=true", false);
                System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();


            }
            return;
        }
        else
        {
            Individual I = RecordId > 0
                               ? Engine.IndividualsActions.Get(RecordId)
                               : new Individual
                               {
                                   AccountId = AccountID,
                                   AddedBy = SalesPage.CurrentUser.FullName,
                                   AddedOn = DateTime.Now,
                                   IsActive = true,
                                   IsDeleted = false
                               };
            GetIndividualInfo(ref I);

            try
            {
                if (RecordId > 0)
                    Engine.IndividualsActions.Change(I, SalesPage.CurrentUser.FullName);
                else
                    RecordId = Engine.IndividualsActions.Add(I).Key;

                UpdateAccount(AccountID, RecordId);
                //  ctlMessage.SetStatus(Messages.RecordSavedSuccess);

                if (Page is IIndividual)
                    (Page as IIndividual).UpdateIndividuals();
            }
            catch (Exception ex)
            {
                ctlMessage.SetStatus(ex);
            }
        }


        DialPhone(RecordId);
    }

    void BindEvents()
    {
        if (Page is IIndividual)
            (Page as IIndividual).Notify(this);

        if (Page is ITCPAProvider)
        {
            _tcpaId = (Page as ITCPAProvider).Register(this as ITCPAClient);
            if (_tcpaId != 0)
            {
                txtDayPhone.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
                txtEveningPhone.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
                txtCellPhone.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
                txtFax.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
            }
        }
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

        btnSaveOnForm.Click += (o, a) => { if (IsValidated) SavePerson(); };
        btnSaveAndCloseOnForm.Click += (o, a) => { if (IsValidated) { SavePerson(); ShowGrid(); } };

        btnQuote.Click += (o, a) => { Quote(); };
        btnFillForm.Click += (o, a) => { FillForm(); };

        btnReturn.Click += (o, a) => ShowGrid();
        btnCancelOnForm.Click += (o, a) => CancelForm();

        btnAddNewIndividual.Click += (o, a) => Add();

        grdIndividual.RowCommand += (o, a) => CommandRouter(a.CommandName, Helper.SafeConvert<long>(grdIndividual.DataKeys[((a.CommandSource as Control).NamingContainer as GridViewRow).RowIndex].Value.ToString()));
        PagingNavigationBar.IndexChanged += (o, a) => BindGrid();
        PagingNavigationBar.SizeChanged += (o, a) => BindGrid();
        grdIndividual.Sorting += (o, a) => Sort(a.SortExpression);

        // WM - 03.06.2013
        lnkDayPhone.ServerClick += (obj, args) => ClickedToDial(txtDayPhone.Text);
        lnkEveningPhone.ServerClick += (obj, args) => ClickedToDial(txtEveningPhone.Text);
        lnkCellPhone.ServerClick += (obj, args) => ClickedToDial(txtCellPhone.Text);
        lnkFax.ServerClick += (obj, args) => ClickedToDial(txtFax.Text);


    }

    //WM - 03.06.2013
    private void SetClientClick(LinkButton lbtn, string outpulseId)
    {
        if (lbtn == null)
            return;

        string phoneNumber = Helper.ConvertMaskToPlainText(lbtn.Text);
        if (phoneNumber.Trim().Length < 10)
        {
            lbtn.Attributes.Add("onclick", "javascript:alert('Invalid phone number.')");
            return;
        }
        //if (ApplicationSettings.IsPhoneSystemFive9)
        if (Engine.ApplicationSettings.IsPhoneSystemFive9)
        {
            lbtn.Attributes.Add("onclick", Helper.GetPhoneWindowScript(phoneNumber, outpulseId));
        }
        else
            lbtn.Attributes.Add("onclick", "");
    }

    public void ClickedToDial1(object sender, EventArgs e)
    {
        LinkButton lbtn = sender as LinkButton;
        string outPulseID = (!string.IsNullOrEmpty(lbtn.CommandArgument)) ? lbtn.CommandArgument : "";
        if (lbtn == null)
            return;

        this.ClickedToDial(Helper.ConvertMaskToPlainText(lbtn.Text), outPulseID);
    }

    public void ClickedToDial(string phoneNumber, string outpulseID = "")
    {
        if (phoneNumber.Trim().Length < 10)
            return;
        string message = string.Empty;

        Engine.AccountHistory.LogCall(AccountID, phoneNumber, SalesPage.CurrentUser.Key);
        if (Engine.ApplicationSettings.IsPhoneSystemInContact)
            message = InContactCall(phoneNumber, outpulseID);
        if (DialClicked != null)
            DialClicked(this, new DialClickedEventArgs(phoneNumber, message));
    }
    //YA[Dec 16, 2013]
    private string InContactCall(string phoneNumber = "", string outpulseID = "")
    {
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);

        inContactAuthorizationResponse authToken;
        JoinSessionResponse sessionResponse;
        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(outpulseID))
        {
            exceptionMessage = "Outpulse ID Not Found.";
        }
        else if (string.IsNullOrEmpty(SalesPage.CurrentUser.PhoneSystemUsername) && string.IsNullOrEmpty(SalesPage.CurrentUser.PhoneSystemPassword))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
        }
        else
        {

            inContactFunctions funcs = new inContactFunctions();
            //authToken = funcs.inContactAuthorization(ApplicationSettings.PhoneSystemAPIGrantType, ApplicationSettings.PhoneSystemAPIScope, SalesPage.CurrentUser.PhoneSystemUsername, SalesPage.CurrentUser.PhoneSystemPassword, ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
            authToken = funcs.inContactAuthorization(Engine.ApplicationSettings.PhoneSystemAPIGrantType, Engine.ApplicationSettings.PhoneSystemAPIScope, SalesPage.CurrentUser.PhoneSystemUsername, SalesPage.CurrentUser.PhoneSystemPassword, Engine.ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
            if (authToken == null)
            {
                exceptionMessage = "Unable to authenticate with Softphone.";
            }
            else
            {
                sessionResponse = funcs.inContactJoinSession(authToken, ref exceptionMessage);
                if (sessionResponse != null)
                {
                    exceptionMessage = funcs.inContactDialNumber(authToken, sessionResponse, phoneNumber.Replace("-", ""), outpulseID);
                }
            }
        }
        return exceptionMessage;
    }

    protected void Evt_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        //IH 05.10.13 Optimized the statement

        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            var dataKey = grdIndividual.DataKeys[e.Row.RowIndex];
            if (dataKey == null) return;
            var lbtnDayPhone = e.Row.FindControl("lnkDayPhoneGrid") as LinkButton;
            var lbtnEveningPhone = e.Row.FindControl("lnkEveningPhoneGrid") as LinkButton;
            var lbtnCellPhone = e.Row.FindControl("lnkCellPhoneGrid") as LinkButton;
            var lbtnFax = e.Row.FindControl("lnkFaxGrid") as LinkButton;

            var id = Helper.SafeConvert<long>(dataKey.Value.ToString());
            var outpulseId =
                ((IIndividual)Page).Individuals.Where(x => x.Id == id).Select(x => x.OutpulseId).FirstOrDefault();

            lbtnDayPhone.CommandArgument = outpulseId;
            lbtnEveningPhone.CommandArgument = outpulseId;
            lbtnCellPhone.CommandArgument = outpulseId;
            //lbtnFax.CommandArgument = outpulseId;

            SetClientClick(lbtnDayPhone, outpulseId);
            SetClientClick(lbtnEveningPhone, outpulseId);
            SetClientClick(lbtnCellPhone, outpulseId);
            SetClientClick(lbtnFax, outpulseId);

            //SZ [Aug 2014] delete fucntionality asp.net GridView
            DisableDeleteInGridView(e.Row, "lnkDelete");
            //var ctl = e.Row.FindControl("lnkDelete") as LinkButton;
            //if (ctl != null && !SalesPage.CurrentUser.Security.Administration.CanDelete)
            //    ctl.Visible = false;
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
            BindIndividualStatuses();
            ShowGrid();
        }
    }
    protected override void InnerSave()
    {
        SavePerson();
        if (CloseForm)
            ShowGrid();
    }
    public override bool IsValidated
    {
        get
        {
            //WM - 04.07.2013
            //John Said - "on the Individuals tab on Leads.aspx, none of the fields should be required"
            return true;
            ////bool bAns = false;

            ////vldFirstname.Validate();
            ////vldAddress1.Validate();
            ////vldLastName.Validate();

            ////bAns = vldFirstname.IsValid && vldLastName.IsValid && vldAddress1.IsValid;

            ////if ((txtDayPhone.Text == string.Empty) && (txtEveningPhone.Text == string.Empty) && (txtCellPhone.Text == string.Empty))
            ////{
            ////    bAns &= false;
            ////    ctlMessage.SetStatus(new Exception("Atleast one of the phone numbers must be provided!"));
            ////}

            //////if(txtDayPhone.Text!=string.Empty)
            //////    RegularExpressionValidator1.Validate();

            //////if (txtEveningPhone.Text != string.Empty)
            //////    RegularExpressionValidator2.Validate();

            //////if (txtCellPhone.Text != string.Empty)
            //////    RegularExpressionValidator3.Validate();

            //////if (txtFax.Text != string.Empty)
            //////    RegularExpressionValidator4.Validate();

            //////bAns &= RegularExpressionValidator1.IsValid && RegularExpressionValidator2.IsValid && RegularExpressionValidator3.IsValid && RegularExpressionValidator4.IsValid;

            ////return bAns;
        }
    }

    void DialPhone(long id)
    {
        //int campaignId = 0;
        //long l = 0;
        //if (Session["LeadKey"] != null)
        //    l= Helper.SafeConvert<long>(Session["LeadKey"].ToString());
        //var lead = Engine.LeadsActions.Get(l);

        //if (lead != null)
        //{
        //    campaignId = lead.CampaignId ?? 0;
        //}


        string outpulseId = ((IIndividual)Page).Individuals.Where(x => x.Id == id).Select(x => x.OutpulseId).FirstOrDefault();
        string alertMessage = "javascript:alert('Invalid phone number.')";
        CurrentOutpulseID = outpulseId;
        //if (ApplicationSettings.IsPhoneSystemFive9)
        if (Engine.ApplicationSettings.IsPhoneSystemFive9)
        {
            //IH 05.10.13--optimize the if conditions
            lnkDayPhone.Attributes.Add("onclick",
                                       txtDayPhone.Text.Trim().Length != 10
                                           ? alertMessage
                                           : Helper.GetPhoneWindowScript(txtDayPhone.Text, outpulseId));

            lnkEveningPhone.Attributes.Add("onclick",
                                           txtEveningPhone.Text.Trim().Length != 10
                                               ? alertMessage
                                               : Helper.GetPhoneWindowScript(txtEveningPhone.Text, outpulseId));

            lnkCellPhone.Attributes.Add("onclick",
                                        txtCellPhone.Text.Trim().Length != 10
                                            ? alertMessage
                                            : Helper.GetPhoneWindowScript(txtCellPhone.Text, outpulseId));

            lnkFax.Attributes.Add("onclick",
                                  txtFax.Text.Trim().Length != 10
                                      ? alertMessage
                                      : Helper.GetPhoneWindowScript(txtFax.Text, outpulseId));
        }

        //IH 05.10.13--commited by IH 
        //if (txtDayPhone.Text.Trim().Length != 10)
        //    lnkDayPhone.Attributes.Add("onclick", alertMessage);
        //else
        //    lnkDayPhone.Attributes.Add("onclick", Helper.GetPhoneWindowScript(txtDayPhone.Text, outpulseId));

        //if (txtEveningPhone.Text.Trim().Length != 10)
        //    lnkEveningPhone.Attributes.Add("onclick", alertMessage);
        //else
        //    lnkEveningPhone.Attributes.Add("onclick", Helper.GetPhoneWindowScript(txtEveningPhone.Text, outpulseId));

        //if (txtCellPhone.Text.Trim().Length != 10)
        //    lnkCellPhone.Attributes.Add("onclick", alertMessage);
        //else
        //    lnkCellPhone.Attributes.Add("onclick", Helper.GetPhoneWindowScript(txtCellPhone.Text, outpulseId));

        //if (txtFax.Text.Trim().Length != 10)
        //    lnkFax.Attributes.Add("onclick", alertMessage);
        //else
        //    lnkFax.Attributes.Add("onclick", Helper.GetPhoneWindowScript(txtFax.Text, outpulseId));
    }

    void BindGrid()
    {
        try
        {
            var records = ((IIndividual)Page).Individuals;
            grdIndividual.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(records.AsQueryable(), PagingNavigationBar.SortBy, PagingNavigationBar.SortAscending));
            grdIndividual.DataBind();
            //IH 05.10.13 Optimized the statement
            //String insuranceType = System.Configuration.ConfigurationManager.AppSettings[Konstants.K_INSURANCE_TYPE];
            // System.Configuration.ConfigurationManager.AppSettings["InsuranceType"];
            bool splitAccountMode = (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout);
            //YA[10 Dec, 2013]
            //if (ApplicationSettings.IsTermLife || grdIndividual.Columns.Count <= 0)
            if (Engine.ApplicationSettings.IsTermLife || grdIndividual.Columns.Count <= 0)
            {
                grdIndividual.Columns[3].Visible = false;
                grdIndividual.Columns[7].Visible = true;
            }
            else
            {
                grdIndividual.Columns[3].Visible = true;
                grdIndividual.Columns[7].Visible = false;
            }
            
            grdIndividual.Columns[14].Visible = !(splitAccountMode);
            
            grdIndividual.Columns[0].Visible = /*AccountId*/
            grdIndividual.Columns[8].Visible = /*Status*/
            grdIndividual.Columns[9].Visible = /*SubStatus*/
            grdIndividual.Columns[10].Visible = splitAccountMode;/*NextCalendarEventDate*/

            if (!Engine.ApplicationSettings.IsSenior || grdIndividual.Columns.Count <= 0) return;
            grdIndividual.Columns[11].Visible = true;
            grdIndividual.Columns[12].Visible = false;
        }
        catch (Exception ex)
        {
            ctlMessage.SetStatus(ex); //lblMessageGrid.Text = "Error: " + ex.Message;
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
    private void CancelForm()
    {
        if (Engine.ApplicationSettings.IsMultipleAccountsAllowed && CheckNewForSplit)
        {
            long? parentId = Request.ReadQueryStringAs<long?>(Konstants.K_PARENT_ACCOUNT_ID);
            Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + parentId, false);
            System.Web.HttpContext.Current.ApplicationInstance.CompleteRequest();
        }
        else
        {
            ShowGrid();
        }
    }
    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;
        //SR divGrid.Visible = bShow;
        //SR divForm.Visible = !bShow;


        if (bShow)
        {
            dlgNewIndividual.Dispose();
            dlgNewIndividual.VisibleOnPageLoad = false;
            dlgNewIndividual.Visible = false;
            BindGrid();
            RecordId = 0;
        }
        else
        {
            dlgNewIndividual.VisibleOnPageLoad = true;
            dlgNewIndividual.Visible = true;
            dlgNewIndividual.CenterIfModal = true;

            if (!Engine.ApplicationSettings.IsTermLife)
            {
                lblMiddle.Visible = false;
                txtMiddle.Visible = false;
                lblApplicationState.Visible = false;
                ddlAppState.Visible = false;
            }
        }
    }

    long RecordId
    {
        get
        {
            long Ans = default(long);
            long.TryParse(hdnRecordId.Value, out Ans);
            return Ans;
        }
        set
        {
            hdnRecordId.Value = value.ToString();
        }
    }



    void UpdateAccount(long accId, long individualId)
    {
        if (cbxPrimary.Checked)
            Engine.AccountActions.SetIndividual(accId, IndividualType.Primary, individualId);
        if (cbxSecondary.Checked)
        {
            Engine.AccountActions.SetIndividual(accId, IndividualType.Secondary, individualId);
            //[MH 17 Dec] spouse issue on save
            CurrentAccount.SecondaryIndividualId = individualId;
        }
        else if (Engine.AccountActions.IsIndividual(accId, IndividualType.Secondary, individualId))
            Engine.AccountActions.SetIndividual(accId, IndividualType.Secondary, 0);

        //if (IndividualUpdated != null)
        //    IndividualUpdated(this, new IndividualEventArgs(RecordId));

        //if (Page is IIndividual)
        //    (Page as IIndividual).UpdateIndividuals();

        //GetIndividualsByAccount(true); //SZ [May 10, 2013] Update Cache
    }



    //TM [may 27, 2014] Commented out, not required any more
    //bool ArePrimarySecondaryChecked
    //{
    //    get
    //    {
    //        return cbxPrimary.Checked && cbxSecondary.Checked;
    //    }
    //}

    //TM [may 27, 2014] Commented out, not required any more
    bool ArePrimarySecondaryEnabled()
    {
        return cbxPrimary.Enabled && cbxSecondary.Enabled;
    }

    //SZ [Apr 17, 2013] Added for secondard check box
    protected void Evt_Secondary_Check_Changed(object sender, EventArgs e)
    {
        bool bMsg = false;
        //TM [may 27, 2014] Commented out, not required any more
        //if (ArePrimarySecondaryChecked)
        //{
        //    ctlMessage.SetStatus(new Exception("An individual cannot be both primary and secondary"));
        //    cbxSecondary.Checked = false;
        //    bMsg = true;
        //}

        //TM [may 27, 2014] Enhanced to prevent error messages
        if (ArePrimarySecondaryEnabled() && cbxSecondary.Checked)
        {
            cbxPrimary.Checked = false;
        }

        if (!bMsg)
            ctlMessage.Clear();

    }
    //SZ [Apr 17, 2013] Added for primary check box
    protected void Evt_Primary_Check_Changed(object sender, EventArgs e)
    {
        //IH 05.10.13 Optimized the statement
        bool bMsg = false;
        if (RecordId < 1 && !cbxPrimary.Checked && IsCurrentRecordPrimary)
        {
            ctlMessage.SetStatus(new Exception("an account must have a primary individual"));
            cbxPrimary.Checked = true;
            bMsg = true;
        }

        //TM [may 27, 2014] Commented out, not required any more
        //if (ArePrimarySecondaryChecked)
        //{
        //    ctlMessage.SetStatus(new Exception("an individual cannot be both primary and secondary individual"));
        //    cbxPrimary.Checked = false;
        //    bMsg = true;
        //}

        //TM [may 27, 2014] Enhanced to prevent error messages
        if (ArePrimarySecondaryEnabled() && cbxPrimary.Checked)
        {
            cbxSecondary.Checked = false;
        }

        if (!bMsg)
            ctlMessage.Clear();
        CheckRequiredFields();
    }

    bool IsCurrentRecordPrimary
    {
        get
        {
            bool bAns = false;
            try
            {
                long pid = Engine.AccountActions.GetIndividual(AccountID, IndividualType.Primary).Key;
                if (RecordId == pid)
                    bAns = true;
            }
            catch { }
            return bAns;
        }
    }

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddNewIndividual.Visible = bEnable;
            //grdIndividual.Columns[grdIndividual.Columns.Count - 2].Visible = bEnable;
            //grdIndividual.Columns[grdIndividual.Columns.Count - 1].Visible = !bEnable;
            var colEdit = grdIndividual.Columns[grdIndividual.Columns.Count - 3];
            var colDelete = grdIndividual.Columns[grdIndividual.Columns.Count - 2];
            var colView = grdIndividual.Columns[grdIndividual.Columns.Count - 1];
            colEdit.Visible = bEnable;
            colDelete.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
        {
            EnableControls(tblControls, bEnable);
            //SZ [Sep22, 2013] Always disabled
            ddlConsent.Enabled = false;
        }
    }

    protected void AccountLog(long accountId, string message)
    {
        Engine.AccountHistory.Log(accountId, message, SalesPage.CurrentUser.Key);
    }
    //protected void Evt_TextChanged(object sender, EventArgs e)
    //{
    //    string controlId = ((WebControl)sender).ID;
    //    string text = ((ITextControl)sender).Text;
    //    ((ITCPAProvider)Page).PerformTCPA(_tcpaId, controlId, Helper.SafeConvert<long>(text));
    //}

    public void ShowEditWindow()
    {
        ShowGrid(false);
        ClearFields();
    }
}
