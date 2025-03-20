//test
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

public class IndividualEventArgs2 : EventArgs
{
    public Individual NewIndividual { get; set; }
}

[System.Runtime.InteropServices.GuidAttribute("989A3272-7F4B-49D7-971B-BAB03C54F3FD")]
public partial class Leads_UserControls_IndividualsAddEdit : AccountsBaseControl, ITCPAClient
{

    int _tcpaId = 0;


    public event EventHandler<LongArgs> OnClose = null;
  

    public event EventHandler<IndividualEventArgs2> IndividualAdded = null;
    
    #region methods

    //void BindStates()
    //{
    //    ddlStateIndividual.DataSource = (Page as SalesDataPage).USStates;
    //    ddlStateIndividual.DataBind();
    //    ddlStateIndividual.Items.Insert(0, new ListItem("--Select State--", "-1"));
    //    ddlStateIndividual.SelectedIndex = 0;
    //}

    void BindIndividualStatuses()
    {
        ddlIndividualStatuses.DataSource = Engine.IndividualStatusActions.All.ToList();
        ddlIndividualStatuses.DataBind();

        ddlIndividualStatuses.Items.Insert(0, new ListItem("-- None --", "-1"));

        ddlIndividualPDPStatuses.DataSource = Engine.IndividualPDPStatusActions.All.ToList();
        ddlIndividualPDPStatuses.DataBind();

        ddlIndividualPDPStatuses.Items.Insert(0, new ListItem("-- None --", "-1"));
    }

    void DialPhone()
    {
        string outpulseId = this.GetOutpulseId();
        string dialPhoneUrl = "http://localhost:9998/makeCall?number={0}&campaignId={1}&checkDnc=true";

        string phoneNumber = txtDayPhone.Text;
        string eveningPhoneNumber = txtEveningPhone.Text;
        string cellNumber = txtCellPhone.Text;
        string faxNumber = txtFax.Text;

        if (phoneNumber.Length < 10)
        {
            lnkDayPhone.HRef = "";
        }
        else
        {
            lnkDayPhone.HRef = string.Format(dialPhoneUrl, phoneNumber, outpulseId);
        }

        if (eveningPhoneNumber.Length < 10)
        {
            lnkEveningPhone.HRef = "";
        }
        else
        {
            lnkEveningPhone.HRef = string.Format(dialPhoneUrl, eveningPhoneNumber, outpulseId);
        }
        
        if (cellNumber.Length < 10)
        {
            lnkCellPhone.HRef = "";
        }
        else
        {
            lnkCellPhone.HRef = string.Format(dialPhoneUrl, cellNumber, outpulseId);
        }
        
        if (faxNumber.Length < 10)
        {
            lnkFax.HRef = "";
        }
        else
        {
            lnkFax.HRef = string.Format(dialPhoneUrl, faxNumber, outpulseId);
        }
    }

    string GetOutpulseId()
    {
        //SalesTool.Schema.LeadsDirect leadsDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);
        SalesTool.Schema.LeadsDirect leadsDirect = new SalesTool.Schema.LeadsDirect(ApplicationSettings.ADOConnectionString);

        long l = 0;
        if(Session[Konstants.K_LEAD_ID]!=null)
            long.TryParse(Session[Konstants.K_LEAD_ID].ToString(), out l);
        
        return leadsDirect.GetOutpulseIdByLeadId(l, SalesPage.CurrentUser.Key).ToString();
    }

    void ClearFields()
    {
        RecordId = 0;
        IsEditMode = false;

        ctlStatus.Clear();

        ddlStateIndividual.DataSource = (Page as SalesDataPage).USStates; 
        ddlStateIndividual.DataBind();
        ddlStateIndividual.Items.Insert(0, new ListItem("--Select State--", "-1"));
        ddlStateIndividual.SelectedIndex = 0;

        ddlAppState.DataSource = ((SalesDataPage)Page).USStates; 
        ddlAppState.DataBind();
        ddlAppState.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlAppState.SelectedIndex = 0;

        ddlIndividualStatuses.DataSource = Engine.IndividualStatusActions.All;
        ddlIndividualStatuses.DataBind();
        ddlIndividualStatuses.Items.Insert(0, new ListItem("-- None --", "-1"));
        ddlIndividualStatuses.SelectedIndex = 0;

        ddlIndividualPDPStatuses.DataSource = Engine.IndividualPDPStatusActions.All;
        ddlIndividualPDPStatuses.DataBind();
        ddlIndividualPDPStatuses.Items.Insert(0, new ListItem("-- None --", "-1"));
        ddlIndividualPDPStatuses.SelectedIndex = 0;
                
        
        txtHRASubsidyAmount.Text = "";
        txtFirstName.Text = string.Empty;
        txtMiddle.Text = string.Empty;

        txtLastName.Text = string.Empty;
        radDOB.SelectedDate = null;
        txtZipCode.Text = string.Empty;
        ddlGender.SelectedIndex = -1;
        ddlTobacco.SelectedIndex = -1;
        txtDayPhone.Text = string.Empty;
        txtEveningPhone.Text = string.Empty;
        txtCellPhone.Text = string.Empty;
        txtFax.Text = string.Empty;
        txtAddress1.Text = string.Empty;
        txtAddress2.Text = string.Empty;
        txtCity.Text = string.Empty;
        txtEmail.Text = string.Empty;
        txtExternalReferenceId.Text = string.Empty;

        chkEmailOptOut.Checked = false;

        rdoNormal.Checked = false;
        rdoPrimary.Checked = false;
        rdoSecondary.Checked = false;

        if (!Engine.ApplicationSettings.IsTermLife)
        {
            lblMiddle.Visible = false;
            txtMiddle.Visible = false;
            lblApplicationState.Visible = false;
            ddlAppState.Visible = false;
        }
        // BindStates();
       // BindIndividualStatuses();
    }

    //MH:March 07 2014
    private void CheckRequiredFields()
    {
       
        if (
            //rdoPrimary.Checked && 
            Engine.ApplicationSettings.IsTermLife)
        {
            txtAddress1.Enabled = false;
            txtAddress2.Enabled = false;
            txtEmail.Enabled = false;
            txtCity.Enabled = false;
            txtZipCode.Enabled = false;
            ddlStateIndividual.Enabled = false;
            //ddlAppState.Enabled = false;
            vldAddress1.Enabled = false;
            vldddlStateIndividual.Enabled = false;

            vldLastName.Enabled = false;
            vldFirstname.Enabled = false;
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
            
            vldAddress1.Enabled = false;
            vldddlStateIndividual.Enabled = false;
            vldLastName.Enabled = false;
            vldFirstname.Enabled = false;
        }


    }
    void SetInfo(Individual I)
    {
        RecordId = I.Key;
        IsEditMode = I.Key != 0;
        
        if (!I.IndividualStatusID.HasValue)
        {
            ddlIndividualStatuses.SelectedIndex = 0;
        }
        else
        {
            ddlIndividualStatuses.SelectedValue = I.IndividualStatusID.Value.ToString();
        }

        if (!I.IndividualPDPStatusID.HasValue)
        {
            ddlIndividualPDPStatuses.SelectedIndex = 0;
        }
        else
        {
            ddlIndividualPDPStatuses.SelectedValue = I.IndividualPDPStatusID.Value.ToString();
        }

        if (I.HRASubsidyAmount.HasValue)
        {
            txtHRASubsidyAmount.Text = I.HRASubsidyAmount.Value.ToString();
        }
        else
        {
            txtHRASubsidyAmount.Text = "";
        }
        txtFirstName.Text = I.FirstName;
        txtMiddle.Text = I.MiddleName;
        txtLastName.Text = I.LastName;
        radDOB.SelectedDate = I.Birthday;
        txtZipCode.Text = I.Zipcode; //Helper.ConvertToInt(entity.Zipcode) <= 0 ? "" : entity.Zipcode.ToString();
        chkEmailOptOut.Checked = I.IndividualEmailOptOut;
        ddlGender.SelectedValue = (I.Gender == "" || I.Gender == null) ? "Male" : I.Gender;
        ddlTobacco.SelectedIndex = I.Smoking==null?0:(I.Smoking == true ? 2 : 1);
        txtDayPhone.Text = I.DayPhone.HasValue? I.DayPhone.ToString(): "";
        txtEveningPhone.Text = I.EveningPhone.HasValue? I.EveningPhone.ToString(): "";
        txtCellPhone.Text = I.CellPhone.HasValue? I.CellPhone.ToString(): "";
        txtFax.Text = I.FaxNmbr.HasValue? I.FaxNmbr.ToString(): "";
        txtAddress1.Text = I.Address1;
        txtAddress2.Text = I.Address2;
        txtCity.Text = I.City;
        txtEmail.Text = I.Email;
        txtExternalReferenceId.Text = I.ExternalReferenceID;

        if(I.StateID.HasValue)
            ddlStateIndividual.SelectedValue = I.StateID.ToString();

        if(I.ApplicationState.HasValue)
            ddlAppState.SelectedValue = I.ApplicationState.ToString();

        txtNotes.Text = I.Notes;

        if (I.Key == 0)
        {
            rdoNormal.Checked = true;
            rdoNormal.Enabled = true;
            pnlPrimarySecondary.Enabled = true;

            lnkDayPhone.HRef = "";
            lnkEveningPhone.HRef = "";
            lnkCellPhone.HRef = "";
            lnkFax.HRef = "";
        }
        else
        {
            if (I.Account.PrimaryIndividualId == I.Key)
            {
                rdoPrimary.Checked = true;

                pnlPrimarySecondary.Enabled = false;
            }
            else if (I.Account.SecondaryIndividualId == I.Key)
            {
                rdoSecondary.Checked = true;

                rdoNormal.Enabled = false;
            }
            else
            {
                rdoNormal.Checked = true;
            }

            switch (I.HasConsent)
            {
                case TCPAConsentType.Blank: ddlConsent.SelectedValue = "0"; break;
                case TCPAConsentType.No: ddlConsent.SelectedValue = "2"; break;
                case TCPAConsentType.Yes: ddlConsent.SelectedValue = "1"; break;
                case TCPAConsentType.Undefined: ddlConsent.SelectedValue = "3"; break;
            }

            bool bShow = I.Key == (I.Account.PrimaryIndividualId ?? 0L);
            ddlConsent.Visible = bShow;
            lblConsent.Visible = bShow;

            DialPhone();
        }
        CheckRequiredFields();
    }
    void GetInfo(ref Individual entity)
    {
        if (entity == null)
        {
            return;
        }

        var currentUserName = base.SalesPage.CurrentUser.FullName;

        if (entity.Key == 0)
        {
            entity.AccountId = this.AccountID;
            entity.AddedBy = currentUserName;
        }
        else
        {
            entity.ChangedBy = currentUserName;
        }

        if (string.IsNullOrWhiteSpace(ddlIndividualStatuses.SelectedValue) || ddlIndividualStatuses.SelectedValue == "-1")
        {
            entity.IndividualStatusID = null;
        }
        else
        {
            entity.IndividualStatusID = Convert.ToInt32(ddlIndividualStatuses.SelectedValue);
        }

        if (string.IsNullOrWhiteSpace(ddlIndividualPDPStatuses.SelectedValue) || ddlIndividualPDPStatuses.SelectedValue == "-1")
        {
            entity.IndividualPDPStatusID = null;
        }
        else
        {
            entity.IndividualPDPStatusID = Convert.ToInt32(ddlIndividualPDPStatuses.SelectedValue);
        }

        entity.HRASubsidyAmount = Helper.SafeConvert<decimal>(txtHRASubsidyAmount.Text);

        entity.FirstName = txtFirstName.Text;
        entity.MiddleName = txtMiddle.Text;
        entity.LastName = txtLastName.Text;
        entity.Birthday = radDOB.SelectedDate;
        entity.Zipcode = txtZipCode.Text;
        entity.Gender = ddlGender.SelectedValue;
        entity.Smoking = (ddlTobacco.SelectedIndex == 2) ? true : (ddlTobacco.SelectedIndex == 1)?false:default(bool?);

        if(txtDayPhone.Text!=string.Empty)
            entity.DayPhone = Helper.SafeConvert<long>(txtDayPhone.Text);

        if(txtEveningPhone.Text!=string.Empty)
            entity.EveningPhone = Helper.SafeConvert<long>(txtEveningPhone.Text);

        if(txtCellPhone.Text!=string.Empty)
            entity.CellPhone = Helper.SafeConvert<long>(txtCellPhone.Text);

        if(txtFax.Text!=string.Empty)
            entity.FaxNmbr = Helper.SafeConvert<long>(txtFax.Text);

        entity.Address1 = txtAddress1.Text;
        entity.Address2 = txtAddress2.Text;
        entity.City = txtCity.Text;
        entity.Email = txtEmail.Text;
        entity.ExternalReferenceID = txtExternalReferenceId.Text;
        //entity.StateID = byte.Parse(ddlStateIndividual.SelectedValue);
        if (ddlStateIndividual.SelectedValue != string.Empty && ddlStateIndividual.SelectedValue != "-1")
            entity.StateID = byte.Parse(ddlStateIndividual.SelectedValue);

        if (ddlAppState.SelectedValue != string.Empty && ddlAppState.SelectedValue != "-1")
            entity.ApplicationState = byte.Parse(ddlAppState.SelectedValue);
        entity.IndividualEmailOptOut = chkEmailOptOut.Checked;
        entity.Notes = txtNotes.Text;
    }

    public void LoadPerson(Individual I)
    {
        ClearFields();
        SetInfo(I);
    }
    public void LoadPerson(long personID)
    {
        LoadPerson(Engine.IndividualsActions.Get(personID));
    }

    protected override void InnerInit()
    {
        IsGridMode = false;
        ClearFields();
    }

    protected override void InnerLoad(bool bFirstTime)
    {
        if (Page is ITCPAProvider)
        {
            _tcpaId = (Page as ITCPAProvider).Register(this as ITCPAClient);
            if (_tcpaId != 0)
            {
                txtDayPhone.AutoPostBack =true;
                txtEveningPhone.AutoPostBack =true;
                txtCellPhone.AutoPostBack =true;
                txtFax.AutoPostBack = true;
                txtDayPhone.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
                txtEveningPhone.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
                txtCellPhone.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
                txtFax.TextChanged += (o, a) => (Page as ITCPAProvider).InvokeTCPA(o, _tcpaId);
            }
        }
        btnCancelOnForm.Click += (o, a) => btnClose_Click(o, a);
        btnSaveOnForm.Click += (o, a) => SaveForm();
        if (bFirstTime)
        {
            //BindIndividualStatuses();
            //ClearFields();
        }
    }

    #endregion

    //public Individual currentIndividual
    //{
    //    get
    //    {
    //        var Indv = new Individual();
    //        if (storeIndividual != null)
    //        {
    //            Indv.Key = storeIndividual.Key;
    //        }

    //        return Indv;
    //    }
    //    set
    //    {
    //        storeIndividual = value;
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
           
    //        return spIndv;
    //    }
    //    set
    //    {
    //        storeSpouseIndv = value;
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
    //    if (!Page.IsPostBack)
    //    {
    //        BindState();
    //    }
    //}
    //public bool ShowSaveButton
    //{
    //    get
    //    {
    //        return btnSaveOnForm.Visible;
    //    }
    //    set
    //    {
    //        btnSaveOnForm.Visible = value;
    //    }
    //}

    //public bool ShowCloseButton
    //{
    //    get
    //    {
    //        return btnCancelOnForm.Visible;
    //    }
    //    set
    //    {
    //        btnCancelOnForm.Visible = value;
    //    }
    //}
    //protected void btnAddNewIndividual_Click(object sender, EventArgs e)
    //{
    //    ClearFields();
    //}
    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
    //interface ILeadControlSave, in the accounts base page
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        bool bAns = false;
    //#region Members/Properties

    //private Individual storeIndividual = new Individual();
    //private Individual storeSpouseIndv = new Individual();
    //#endregion
    //protected void BindState()
    //{
    //    //try
    //    //{

    //    ddlStateIndividual.DataSource = (Page as SalesDataPage).USStates; // Engine.ManageStates.GetAll();
    //    ddlStateIndividual.DataBind();    

    //    //ddlStateIndividual.DataValueField = "Id";
    //        //ddlStateIndividual.DataTextField = "FullName";

    //    //}
    //    //catch (Exception ex)
    //    //{
    //    //    ctlStatus.SetStatus(ex); // lblMessageForm.Text = "Error: " + ex.Message;
    //    //}
    //}


    //        return bAns;
    //    }
    //}

    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            Engine.IndividualsActions.Delete(RecordId);// Convert.ToInt64(hdnFieldEditForm.Value));
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex); // lblMessageForm.Text = "Error: " + ex.Message;
        }
    }

    public long RecordId
    {
        get
        {
            return Helper.SafeConvert<long>(hdnFieldEditForm.Value);
        }
        set
        {
            hdnFieldEditForm.Value = value.ToString();
        }
    }
    public bool IsEditMode
    {
        get
        {
            return hdnFieldIsEditMode.Value == "1";
        }
        set
        {
            hdnFieldIsEditMode.Value = value ? "1" : "0";
        }
    }
    public string Status
    {
        get
        {
            return ddlIndividualStatuses.SelectedItem.Text;
        }
    }

    public string PDPStatus
    {
        get
        {
            return ddlIndividualPDPStatuses.SelectedItem.Text;
        }
       
    }

    public bool ShowButtons
    {
        set
        {
            btnSaveOnForm.Visible = value;
            btnCancelOnForm.Visible = value;
        }
    }
    
    void SaveForm(bool closeForm = false)
    {
        try
        {
            Individual entity=null;

            if (!IsEditMode)
            {
                entity = new Individual()
                    {
                        //MH:03 June 2014
                        IsActive = true,
                        IsDeleted = false,
                        AddedBy = (Page as SalesBasePage).CurrentUser.FullName,
                        AddedOn = DateTime.Now
                    };
                GetInfo(ref entity);
                Engine.IndividualsActions.Add(entity);
                RecordId = entity.Key;
                IsEditMode = true;
            }
            else //if (IsEditMode)
            {
                if (RecordId != 0)
                {
                    entity = Engine.IndividualsActions.Get(RecordId);
                    GetInfo(ref entity);
                    Engine.IndividualsActions.Change(entity, (Page as SalesBasePage).CurrentUser.FullName);
                }
            }


            if (entity != null)
            {
                // call updateaccount before IndividualAdded event
                this.UpdateAccount(entity.AccountId??0, entity.Key);

                // SZ [May 13, 2013] intefaces are used instead of events
                // WM - 27.06.2013 This is required in many cases of Action workflow
                if (IndividualAdded != null)
                {
                    var individualEventArgs = new IndividualEventArgs2();

                    individualEventArgs.NewIndividual = entity;

                    IndividualAdded(this, individualEventArgs);
                }
            }

            //GetIndividualsByAccount(true);

            if (IndividualAdded != null)
                IndividualAdded(this, new IndividualEventArgs2());
            if (Page is IIndividual)
                (Page as IIndividual).UpdateIndividuals();
            ClearFields();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    
    private void UpdateAccount(long id, long individualId)
    {
        var account = Engine.AccountActions.Get(id);

        this.UpdateAccount(account, individualId);
    }
    private void UpdateAccount(Account account, long individualId)
    {
        if (rdoPrimary.Checked)
        {
            if (account.PrimaryIndividualId != individualId)
            {
                account.PrimaryIndividualId = individualId;

                pnlPrimarySecondary.Enabled = false;
            }
        }
        else if (rdoSecondary.Checked)
        {
            if (account.SecondaryIndividualId != individualId)
            {
                account.SecondaryIndividualId = individualId;

                rdoNormal.Enabled = false;
            }
        }
        //else
        //{
        //    if (account.SecondaryIndividualId == individualId)
        //    {
        //        account.SecondaryIndividualId = null;
        //    }
        //}

        Engine.AccountActions.Update(account);
    }

    //protected void Save_Click(object sender, EventArgs e)
    //{
    //    SaveForm();
    //}
    //protected void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    SaveForm(true);
    //}



    protected void btnClose_Click(object sender, EventArgs e)
    {
        if (OnClose != null)
            OnClose(this, new LongArgs(RecordId));
    }

    public override bool IsValidated
    {
        get
        {
            bool bAns = false;
            return bAns;
        }
    }

    protected override void InnerSave()
    {
        SaveForm(CloseForm);
    }

    protected override void InnerEnableControls(bool bEnable)
    {
        EnableControls(divForm, bEnable);
    }

    public void ProcessConsent(string controlId, TCPAConsentType choice)
    {
        //TCPAConsentType type = choice == 'y' ? TCPAConsentType.Yes : choice == 'n' ? TCPAConsentType.No : choice == 'a' ? TCPAConsentType.Undefined : TCPAConsentType.Blank;
        SalesTool.DataAccess.Models.Individual I = Engine.IndividualsActions.Get(RecordId);
        if (I != null)
            Engine.IndividualsActions.SetConsent(I.Key, choice);
        
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
    //protected void Evt_TextChanged(object sender, EventArgs e)
    //{
    //    string controlId = (sender as WebControl).ID;
    //    string text = (sender as ITextControl).Text;
    //    (Page as ITCPAProvider).PerformTCPA(_tcpaId, controlId, Helper.SafeConvert<long>(text));
    //}
    public void BindCloseEvent(string clientID)
    {
        btnCancelOnForm.OnClientClick = "javascript:return closeDlg('" + clientID + "');";
    }
}
