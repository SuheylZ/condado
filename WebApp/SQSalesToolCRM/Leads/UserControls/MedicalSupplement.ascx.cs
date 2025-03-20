using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;

public partial class Leads_UserControls_policyInformation : AccountsBaseControl, IIndividualNotification, IWrittingAgentSet
{
    public string MedicalSupplementRadWindowClientID
    { get { return dlgMedicalSupplement.ClientID; } }
    //YA[28 May 2014]
    public event EventHandler FreshPolicyStatus;
    protected void OnFreshPolicyStatus(EventArgs e)
    {
        if(FreshPolicyStatus != null)
        {
            FreshPolicyStatus(this, e);
        }
    }

    enum PageDisplayMode
    {
        GridQueueTemplate = 1,
        EditQueueTemplate = 2
    }
    public void Page_Preinit(object sender, EventArgs e)
    {
        dlgMedicalSupplement.SkinID = "";
    }
    public event EventHandler OnNewIndividual = null;

    public bool ShowAddIndividualButton
    {
        set
        {
            btnAddNewIndividual.Visible = value;
        }
    }

    long RecordId
    {
        get
        {
            long Id = 0;
            long.TryParse(hdnFieldPolicyId.Value, out Id);
            return Id;
        }
        set
        {
            hdnFieldPolicyId.Value = value.ToString();
        }
    }
    public long SelectedIndividualId
    {
        set
        {
            if (ddIndividualName.Items.FindByValue(value.ToString()) != null)
                ddIndividualName.SelectedValue = value.ToString();
        }
    }


    public bool IsDisplayTimeStamp = false;
    public bool IsAutoPostBackPolicyStatus = true;
    //IH 21.10.13 - IsSelectedWritingAgent set to true from apply action work flow. On true basis ddlWritingAgent selected as a default loggedin users.
    public bool IsSelectedWritingAgent = false;
    public String DefaultPolicyStatus = string.Empty;
    PageDisplayMode PageMode
    {
        set
        {
            IsGridMode = value == PageDisplayMode.GridQueueTemplate;
            //divForm.Visible = !IsGridMode;
            //divGrid.Visible = IsGridMode;
            if (!IsGridMode && IsAutoPostBackPolicyStatus)
            {
                dlgMedicalSupplement.DestroyOnClose = false;
                dlgMedicalSupplement.VisibleOnPageLoad = true;
                dlgMedicalSupplement.Visible = true;
                dlgMedicalSupplement.Modal = true;
                dlgMedicalSupplement.RenderMode = RenderMode.Classic;
                dlgMedicalSupplement.VisibleTitlebar = true;
                dlgMedicalSupplement.Height = 500;
                divFormFS.Style.Add("margin","10px");
            }
            else
            {
                dlgMedicalSupplement.DestroyOnClose = false;
                divGrid.Visible = IsGridMode;
                dlgMedicalSupplement.VisibleOnPageLoad = !IsGridMode;
                dlgMedicalSupplement.Visible = !IsGridMode;
                dlgMedicalSupplement.Modal = false;
                dlgMedicalSupplement.RenderMode = RenderMode.Lightweight;
                dlgMedicalSupplement.VisibleTitlebar = false;
                dlgMedicalSupplement.BorderStyle = BorderStyle.None;
                dlgMedicalSupplement.Height = 480;
                dlgMedicalSupplement.BorderWidth = 0;
                dlgMedicalSupplement.CssClass = "borderLessDialog";
                dlgMedicalSupplement.DestroyOnClose = false;
                dlgMedicalSupplement.RestrictionZoneID = "restrictionZone";
                dlgMedicalSupplement.Width = 960;
                dlgMedicalSupplement.Left = 0;
                
                divFormFS.Style.Remove("margin");
            }
        }
    }

    public void BindClientEvent(string clientID)
    { 
        btnAddNewIndividual.OnClientClick = "javascript:return showDlg('" + clientID +"');";
    }

    public void bindLeaveBtn()
    {
        dynamic d = Page.Master;
        if (d != null)
        {
            dynamic yesButton = d.buttonYes;
            if (yesButton != null)
            {
                var button = yesButton as Button;
                if (button != null) button.Click += (o, a) => CloseEditing();
            }
        }
    }
    public void BindEvents()
    {
        btnCancelOnForm.Click += (o, a) => CloseEditing();
        bindLeaveBtn();
        btnSaveOnForm.Click += (o, a) => { if (IsValidated) SaveRecord(); };
        btnAddnewMedicare.Click += (o, a) => AddRecord();
        btnSaveAndCloseOnForm.Click += (o, a) => { if (IsValidated) { SaveRecord(); CloseEditing(); } };
        PagingNavigationBar.IndexChanged += (o, a) => BindGrid();
        PagingNavigationBar.SizeChanged += (o, a) => BindGrid();
        //IH 02.08.10
        ddlPolicyStatus.AutoPostBack = hdnPolicyStatus.Value == "0";//IsAutoPostBackPolicyStatus && IsDisplayTimeStamp;
        if (hdnPolicyStatus.Value == "0")
            ddlPolicyStatus.SelectedIndexChanged += (o, a) => PolicyStatusChanged();//IH 02.08.10
        gridMedSup.SortCommand += (o, e) => BindGrid(e.SortExpression, e.NewSortOrder == GridSortOrder.Ascending);
        gridMedSup.ItemCommand += (o, a) => CommandRouter(Helper.SafeConvert<long>(a.CommandArgument.ToString()), a.CommandName);
        btnReturn.Click += (o, a) => CloseEditing();
        btnAddNewIndividual.Click += (o, a) => { if (OnNewIndividual != null) OnNewIndividual(this, new EventArgs()); };
    }
    //IH 02.08.10
    public void PolicyStatusChanged()
    {
        rdpSubmitDateTimeStamp.SelectedDate = null;
        if (ddlPolicyStatus.SelectedItem.Text != string.Empty)
        {
            if (ddlPolicyStatus.SelectedItem.Text.ToUpper().Contains("SUBMITTED") ||
                ddlPolicyStatus.SelectedItem.Text.ToUpper() == "ENROLLED")
                rdpSubmitDateTimeStamp.SelectedDate = DateTime.Now;
        }


    }


    void BindIndividuals()
    {
        //SZ [Jul 3, 2013] Why used long code when the 2 lines below do the same job
        ddIndividualName.DataSource = (Page as IIndividual).Individuals;
        ddIndividualName.DataBind();
        //[IH, 17-07-2013]
        ddIndividualName.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddIndividualName.SelectedIndex = 0;
        //try
        //{
        //    var gettingIndividuals = GetIndividualsByAccount();// Engine.IndividualsActions.GetByAccountID(AccountID);
        //    var result = (from x in gettingIndividuals
        //                  select new
        //                  {
        //                      Key = x.Id,
        //                      Name = x.LastName + ", " + x.FirstName
        //                  });
        //    ddIndividualName.DataSource = result;

        //    ddIndividualName.DataValueField = "Key";
        //    ddIndividualName.DataTextField = "Name";
        //    ddIndividualName.DataBind();
        //}
        //catch
        //{

        //}
    }
    void BindGrid(string sortby = "", bool bAscending = true)
    {
        try
        {

            //if (this.AccountID > 0)
            //{
            //var individuals = (Page as IIndividual).Individuals;
            // var individuals = Engine.IndividualsActions.GetAllAccountID(this.AccountID);
            //var individuals = Engine.IndividualsActions.GetByAccountID(this.AccountID).Select(
            //var individuals = GetIndividualsByAccount().Select(
            //    T => new
            //    {
            //        IndividualID = T.Id,
            //        FullName = T.LastName + "," + T.FirstName
            //    }).AsQueryable();
            //var medsupRecords = Engine.MedsupActions.GetByAccount(this.AccountID).Select(T => new
            //{
            //    Key = T.Key,
            //    IndividualId = T.IndividualId,
            //    AccountID = T.AccountId,
            //    PolicyNumber = T.PolicyNumber,
            //    Carrier = T.CarrierId,
            //    AnnualPremium = T.AnnualPremium,
            //    Plan = T.Plan,
            //    IssueDate = T.IssueDate, 
            //    EffectiveDate = T.EffectiveDate,
            //    ExpirationDate = T.ExpirationDate,

            //}).AsQueryable();
            //var carrierRecords = Engine.CarrierActions.GetMedSup();

            /*
             from p in Programs
        join pl in ProgramLocations
            on p.ProgramID equals pl.ProgramID into pp
        from pl in pp.DefaultIfEmpty()
        where pl == null
        select p;
             */
            //var medsupJoined = (from x in medsupRecords
            //                    join y in carrierRecords
            //                    on x.Carrier equals y.Key into car
            //                    from y in car.DefaultIfEmpty()
            //                    where y == null
            //                    select new
            //                    {
            //                        Key = x.Key,
            //                        IndividualId = x.IndividualId,
            //                        AccountID = x.AccountID,
            //                        PolicyNumber = x.PolicyNumber,
            //                        Carrier = y != null? y.Name:"",
            //                        AnnualPremium = x.AnnualPremium,
            //                        Plan = x.Plan,
            //                        IssueDate = x.IssueDate,
            //                        EffectiveDate = x.EffectiveDate,
            //                        ExpirationDate = x.ExpirationDate
            //                    }).AsQueryable();

            //if (medsupJoined.Count() > 0)
            //{
            //    var records = (from x in medsupJoined
            //                   join y in individuals
            //                   on x.IndividualId equals y.Key //.Id
            //                   select new
            //                   {
            //                       Key = x.Key,
            //                       FullName = y !=null? y.LastName + ", " + y.FirstName : "", // y.FullName,
            //                       AccountID = x.AccountID,
            //                       PolicyNumber = x.PolicyNumber,
            //                       Carrier = x.Carrier,
            //                       AnnualPremium = x.AnnualPremium,
            //                       Plan = x.Plan,
            //                       IssueDate = x.IssueDate,
            //                       EffectiveDate = x.EffectiveDate,
            //                       ExpirationDate = x.ExpirationDate
            //                   }).AsQueryable();

            var records = Engine.MedsupActions.GetByAccount(AccountID);
            gridMedSup.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(records, sortby, bAscending));
            gridMedSup.DataBind();

            //PagingNavigationBar.Visible = true;
            //}
            //else
            //{
            //    ctlStatus.SetStatus("No Medicare Records Available");
            //    PagingNavigationBar.Visible = false;
            //}
            //}
            //else
            //{
            //    ctlStatus.SetStatus("No Medicare Records Available");
            //    PagingNavigationBar.Visible = false;
            //}
            //EditKey = 0;
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }

    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
            BindIndividuals();
    }

    void GetValues(ref Medsup entity /*long accountId, long leadId, */)
    {
        //if (ddCarrier.Items.Count > 0)
        //    entity.CarrierId = (ddCarrier.Items.Count > 0) ? Convert.ToInt64(ddCarrier.SelectedValue) : 0;
        //IH-23.07.12
        entity.CarrierId = ddCarrier.SelectedValue == string.Empty ? (long?)null : Convert.ToInt64(ddCarrier.SelectedValue);

        entity.Plan = ddPlan.SelectedValue;
        entity.GuarenteedIssue = ddGuaranteedIssue.SelectedValue;
        entity.GuarenteedIssueReason = ddGuaranteedIssueReason.SelectedValue;
        entity.PreviousPlan = ddPreviousPlan.SelectedValue;
        if (!string.IsNullOrEmpty(tbAnnualPremium.Text))
            entity.AnnualPremium = Convert.ToDouble(tbAnnualPremium.Text);
        if (!string.IsNullOrEmpty(tbPolicyNumber.Text))
            entity.PolicyNumber = tbPolicyNumber.Text;
        entity.IssueDate = rdpIssueDate.SelectedDate;
        entity.EffectiveDate = rdpEffectiveDate.SelectedDate;
        entity.ExpirationDate = rdpExpirationDate.SelectedDate;
        entity.Favkey = tbFAVKey.Text;
        entity.CompanyName = txtCompanyName.Text;
        entity.FavkeySentToCarrierDate = rdpFAVKeySenttoCarrierDate.SelectedDate;
        entity.CancelDeclineDate = rdpCancelorDeclineDate.SelectedDate;
        entity.PaymentMethod = ddPaymentMode.SelectedValue;
        entity.ReissueDate = rdpReissueDate.SelectedDate;

        entity.LapseDate = rdpLapseDate.SelectedDate;
        entity.PaidFromCarrier = chkPaidFromCarrier.Checked;
        if (txtCommissionAmount.Text != "")
            entity.CommissionAmount = Convert.ToInt64(txtCommissionAmount.Text);
        entity.CommissionPaidDate = rdpCommisionpaidDate.SelectedDate;
        entity.IsDeleted = false;
        entity.IndividualId = Convert.ToInt64(ddIndividualName.SelectedValue);

        //SZ [Jul4, 2013] this is not needed        
        //entity.LeadId = Convert.ToInt64(leadId);
        //entity.AccountId = Convert.ToInt64(accountId);

        //YA[01 May 2014] Log the policy status change
        //Account A = Engine.AccountActions.Get(AccountID);
        if (Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue) != entity.PolicyStatus)
        {
            Engine.AccountHistory.PolicyStatusChanged(AccountID, ddlPolicyStatus.SelectedItem.Text, "Medicare Supplement", base.SalesPage.CurrentUser.Key, 0 , Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue).Value);
            OnFreshPolicyStatus(EventArgs.Empty);
        }

        //WM - [31.07.2013]
        entity.PolicyStatus = string.IsNullOrWhiteSpace(ddlPolicyStatus.SelectedValue) ? null : Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue);

        entity.Enroller = ddlWritingAgent.SelectedValue != "-1" ? new Guid(ddlWritingAgent.SelectedValue) : (Guid?)null; // new Guid(ddlWritingAgent.SelectedValue);
        //entity.Enroller = ddlWritingAgent.SelectedValue != "-1"
        //                      ? new Guid(ddlWritingAgent.SelectedValue)
        //                      : SalesPage.CurrentUser.Key;//(Guid?)null; // new Guid(ddlWritingAgent.SelectedValue);

        //IH 08.01.13
        entity.MedicareId = txtMedicareId.Text;
        entity.SubmissionDate = rdpSubmitDateTimeStamp.SelectedDate;
        //if (!string.IsNullOrEmpty(lblSubmitDateTimeStamp.Text))
        //    entity.AddedOn = Convert.ToDateTime(lblSubmitDateTimeStamp.Text);
        // return entity;

        int? val = Helper.NullConvert<int>(ddlApplicationType.SelectedValue);
        entity.ApplicationTypeKey = val==-1?null:val;
    }
    void SetValues(Medsup entity)
    {
        hdnFieldLeadId.Value = entity.LeadId.ToString();
        hdnFieldEditIndividual.Value = entity.IndividualId.ToString();

        //AccountIDForm = CurrentPolicy.AccountId ?? 0;
        //Individual allIndividuals = Engine.IndividualsActions.GetByAccountID(

        if (ddCarrier.Items.FindByValue(entity.CarrierId.ToString()) != null)
            ddCarrier.SelectedValue = entity.CarrierId.ToString();

        ddPlan.SelectedValue = entity.Plan;

        ddIndividualName.SelectedValue = entity.IndividualId.ToString();
        ddGuaranteedIssue.SelectedValue = entity.GuarenteedIssue;
        ddGuaranteedIssueReason.SelectedValue = entity.GuarenteedIssueReason;
        ddPreviousPlan.SelectedValue = entity.PreviousPlan;
        tbPolicyNumber.Text = Convert.ToString(entity.PolicyNumber);
        tbAnnualPremium.Text = entity.AnnualPremium.ToString();
        tbFAVKey.Text = entity.Favkey;
        txtCompanyName.Text = entity.CompanyName;
        rdpFAVKeySenttoCarrierDate.SelectedDate = entity.FavkeySentToCarrierDate;
        rdpCancelorDeclineDate.SelectedDate = entity.CancelDeclineDate;

        rdpIssueDate.SelectedDate = entity.IssueDate;
        rdpEffectiveDate.SelectedDate = entity.EffectiveDate;
        rdpExpirationDate.SelectedDate = entity.ExpirationDate;

        rdpPaymentDate.SelectedDate = entity.CommissionPaidDate; // needs updating
        ddPaymentMode.SelectedValue = entity.PaymentMethod;

        rdpReissueDate.SelectedDate = entity.ReissueDate;

        rdpLapseDate.SelectedDate = entity.LapseDate;
        chkPaidFromCarrier.Checked = entity.PaidFromCarrier ?? false;
        txtCommissionAmount.Text = entity.CommissionAmount.ToString();
        rdpCommisionpaidDate.SelectedDate = entity.CommissionPaidDate;

        ddlWritingAgent.SelectedValue = (entity.Enroller == null) ? (ddlWritingAgent.SelectedValue = "-1") : entity.Enroller.ToString();

        // ddlWritingAgent.SelectedValue = (entity.Enroller == null) ? SalesPage.CurrentUser.Key.ToString() : entity.Enroller.ToString();



        //WM - [31.07.2013]
        if (entity.PolicyStatus.HasValue)
        {
            ddlPolicyStatus.SelectedValue = entity.PolicyStatus.Value.ToString();
        }
        else
        {
            ddlPolicyStatus.SelectedIndex = 0;
        }

        //IH 25.07.13
        rdpSubmitDateTimeStamp.SelectedDate = entity.SubmissionDate;
        hdnPolicyStatus.Value = entity.SubmissionDate == null ? "0" : "1";
        ddlPolicyStatus.AutoPostBack = entity.SubmissionDate == null;
        //IH 01.08.13
        txtMedicareId.Text = entity.MedicareId;
        //IH 15.08.13
        var userPermission = ((SalesBasePage)Page).CurrentUser.UserPermissions.FirstOrDefault();
        if (RecordId != 0 && userPermission != null && userPermission.Permissions.Account != null)
            rdpSubmitDateTimeStamp.Enabled = userPermission.Permissions.Account.EditSubmitEnrollDates;
        if (entity.ApplicationTypeKey.HasValue)
            ddlApplicationType.SelectedValue = entity.ApplicationTypeKey.Value.ToString();
    }




    public void CommandRouter(long id, string command)
    {
        switch (command)
        {
            case "EditX":
                EditRecord(id);
                break;

            case "DeleteX":
                DeleteRecord(id);
                break;

            case "ViewX":
                EditRecord(id);
                ReadOnly = true;
                break;

        }
    }


    void ClearFields()
    {
        BindIndividuals();
        hdnPolicyStatus.Value = IsAutoPostBackPolicyStatus ? "0" : "1";
        ddCarrier.DataSource = Engine.CarrierActions.GetMedSup();
        ddCarrier.DataBind();
        //IH-23.07.12
        ddCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
        ddCarrier.SelectedIndex = 0;



        if (ddPlan.Items.Count > 0)
            ddPlan.SelectedIndex = 0;
        if (ddGuaranteedIssue.Items.Count > 0)
            ddGuaranteedIssue.SelectedIndex = 0;
        if (ddGuaranteedIssueReason.Items.Count > 0)
            ddGuaranteedIssueReason.SelectedIndex = 0;
        if (ddPreviousPlan.Items.Count > 0)
            ddPreviousPlan.SelectedIndex = 0;


        ddlWritingAgent.DataSource = Engine.UserActions.GetWritingAgents();
        ddlWritingAgent.DataBind();
        ddlWritingAgent.Items.Insert(0, new ListItem("", "-1"));
        ddlWritingAgent.SelectedIndex = 0;
        //IH 21.10.13 - IsSelectedWritingAgent set to true from apply action work flow. On true basis ddlWritingAgent selected as a default loggedin users. 
        if (IsSelectedWritingAgent)
            ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
        /*
         * Policy status type is as following at the moment
         autohome = 1
         Medicare Supplement	= 2
         MAPDP = 3
         Dental and Vision = 4
         */

        ddlPolicyStatus.DataSource = Engine.PolicyStatusActions.GetAll(/*Policy status type = */2);
        ddlPolicyStatus.DataBind();
        //IH 23.07.13
        ddlPolicyStatus.Items.Insert(0, new ListItem(""));
        //ddlPolicyStatus.SelectedIndex = 0;
        //IH 16.09.13
        if (DefaultPolicyStatus != string.Empty)
            ddlPolicyStatus.SelectedIndex = ddlPolicyStatus.Items.IndexOf(ddlPolicyStatus.Items.FindByText(DefaultPolicyStatus));

        tbPolicyNumber.Text = "";
        tbAnnualPremium.Text = "";
        rdpIssueDate.SelectedDate = null;
        rdpEffectiveDate.SelectedDate = null;
        tbFAVKey.Text = "";
        txtCompanyName.Text = "";
        rdpFAVKeySenttoCarrierDate.SelectedDate = null;
        rdpCancelorDeclineDate.SelectedDate = null;
        rdpPaymentDate.SelectedDate = null;
        ddPaymentMode.SelectedIndex = 0;
        rdpReissueDate.SelectedDate = null;

        rdpLapseDate.Clear();
        chkPaidFromCarrier.Checked = false;
        txtCommissionAmount.Text = string.Empty;
        //IH 25.07.13
        // lblSubmitDate.Visible = lblSubmitDateTimeStamp.Visible = IsDisplayTimeStamp;
        rdpSubmitDateTimeStamp.SelectedDate = IsDisplayTimeStamp == false ? (DateTime?)null : DateTime.Now;
        // lblSubmitDateTimeStamp.Text = DateTime.Now.ToString();
        txtMedicareId.Text = string.Empty;
        rdpCommisionpaidDate.Clear();

        // SZ [Aug 7, 2014] 
        ddlApplicationType.DataSource = Engine.MedsupActions.MedicalSupplimentApplicationTypes;
        ddlApplicationType.DataBind();
        ddlApplicationType.Items.Insert(0, new ListItem("", "-1"));
        ddlApplicationType.SelectedIndex = 0;
    }
    void DeleteRecord(long id)
    {
        try
        {
            Engine.MedsupActions.Delete(id);
            BindGrid();
            //SR ctlStatus.SetStatus(Messages.RecordDeletedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void EditRecord(long id)
    {
        try
        {
            Medsup entity = Engine.MedsupActions.GetPolicyInfo(id);
            RecordId = id;

            ClearFields();
            SetValues(entity);
            PageMode = PageDisplayMode.EditQueueTemplate;
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void CloseEditing()
    {
        PageMode = PageDisplayMode.GridQueueTemplate;
        BindGrid();
    }
    
    void AddRecord()
    {
        RecordId = 0;
        ClearFields();
        PageMode = PageDisplayMode.EditQueueTemplate;
    }
    bool SaveRecord(bool checkRequiredFields = false)
    {

        Medsup entity = (RecordId == 0) ?
            new Medsup
            {
                AccountId = AccountID,
                AddedOn = DateTime.Now,

                AddedBy = base.SalesPage.CurrentUser.FullName
            } :
            Engine.MedsupActions.GetPolicyInfo(RecordId);
        GetValues(ref entity);
        bool saveFlagError = false;
        try
        {
            if (checkRequiredFields)
            {
                saveFlagError = CheckRequiredFields(entity);
                if (!saveFlagError)
                {
                    RecordSave(entity);
                }
            }
            else
                RecordSave(entity);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
        return saveFlagError;
    }

    private void RecordSave(Medsup entity)
    {
        if (RecordId == 0)
        {
            Engine.MedsupActions.Add(entity);
            RecordId = entity.Key;
        }
        else
        {
            entity.ChangedBy = base.SalesPage.CurrentUser.FullName;
            Engine.MedsupActions.Update(entity);
        }
        //SR  ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    }
    /// <summary>
    /// Check the required fields setup at Sub Status II
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private bool CheckRequiredFields(Medsup entity)
    {
        //YA[July 15, 2013] Duplicate record implementation.
        bool hasErrors = false;
        string errorMessage = "";
        RequiredFieldChecker nduplicate = new RequiredFieldChecker();
        nduplicate.RequiredFieldsChecking(entity, ref hasErrors, ref errorMessage, "Medicare Supplement", this.AccountID);
        ctlStatus.SetStatus(new Exception(errorMessage));
        return hasErrors;
    }




    public override bool IsValidated
    {
        get
        {
            string errorMessage = "Error Required Field(s): ";
            vlddIndividualName.Validate();
            if (!vlddIndividualName.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + vlddIndividualName.ErrorMessage));
            return vlddIndividualName.IsValid;
        }
    }
    protected override void InnerInit()
    {

        //AccountIDForm = AccountID;
        //BindDropDown();  // WM - 05.06.2013 -- called in SetPageMode
        PageMode = PageDisplayMode.GridQueueTemplate;
        ctlStatus.Initialize();
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        BindEvents();

        if (bFirstTime && IsGridMode)
            BindGrid();

        if (Page is IIndividual)
            (Page as IIndividual).Notify(this);
    }
    protected override void InnerSave()
    {
        if (IsValidated)
        {
            SaveRecord();
            if (CloseForm)
                CloseEditing();
        }
    }



    public void Action_AddRecord()
    {
        AddRecord();
        divButtons.Visible = false; //SZ [July 4, 2013]
        //SR btnReturn.Visible = false; [March 31, 2014]
    }
    public bool Action_Save(bool checkRequiredFields = false)
    {
        return SaveRecord(checkRequiredFields);
    }
    public long Action_RecordId
    {
        get
        {
            return RecordId;
        }
    }

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddnewMedicare.Visible = bEnable;
            var colEdit = gridMedSup.Columns.FindByUniqueName("colEdit");
            var colView = gridMedSup.Columns.FindByUniqueName("colView");
            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
            Helper.EnableControls(tblControls, bEnable);
    }
    //SZ [Aug 28, 2013] implements the interfcae for setting the writting agent
    public void SetAgent(Guid agentId)
    {
        if (ddlWritingAgent.Items.FindByValue(agentId.ToString()) != null)
            ddlWritingAgent.SelectedValue = agentId.ToString();
    }



    //public long AddRecord(bool cleafFields, long AccountId)
    //{
    //    //long accountId = AccountId;
    //    //long individualID = Helper.SafeConvert<long>(hdnFieldEditIndividual.Value);
    //    //long leadId = Helper.SafeConvert<long>(hdnFieldLeadId.Value);

    //    Medsup entity = new Medsup { AccountId = AccountId, AddedBy = this.SalesPage.CurrentUser.FullName, AddedOn = DateTime.Now };
    //    GetValues(ref entity);

    //    //entity.AddedBy = base.SalesPage.CurrentUser.FullName;

    //    Engine.MedsupActions.Add(entity);

    //    if (cleafFields)
    //        ClearFields();

    //    return entity.Key;
    //}


    //SZ [Jul 4, 2013] this function was the culprit and none of its logic gets called in ApplyAction.ascx due to if(!Page.IsPostback). 
    // However if you remove that condition the control starts behaving weirdly in the Lead.aspx page. 

    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!Page.IsPostBack)
    //    {
    //        this.divGrid.Visible = !this.HideGrid;

    //        if (this.IsOnActionWizard)
    //        {
    //            btnAddNewIndividual.Visible = false;
    //            btnCancelOnForm.Visible = false;
    //            btnSaveAndCloseOnForm.Visible = false;
    //            btnSaveOnForm.Visible = false;
    //            BindIndividuals();
    //        }
    //        // SZ [Jul 4, 2013] moved to innerLoad
    //        ////writing agent  = enroller according to John.
    //        //ddlWritingAgent.DataSource = Engine.UserActions.GetWritingAgents();
    //        //ddlWritingAgent.DataBind();
    //        //ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();

    //        /*
    //         * Policy status type is as following at the moment
    //         autohome = 1
    //         Medicare Supplement	= 2
    //         MAPDP = 3
    //         Dental and Vision = 4
    //         */
    //        //ddlPolicyStatus.DataSource = Engine.PolicyStatusActions.GetAll(/*Policy status type = */2);
    //        //ddlPolicyStatus.DataBind();
    //    }
    //}
    //public long AccountIDForm
    //{
    //    get
    //    {
    //        long Id = 0;
    //        long.TryParse(hdnFieldAccountId.Value, out Id);
    //        return Id;
    //    }
    //    set
    //    {
    //        hdnFieldAccountId.Value = value.ToString();
    //    }
    //}

    //protected void BindDropDown()
    //{
    //    try
    //    {
    //        //ddCarrier.DataSource = Engine.CarrierActions.GetMedSup();

    //        ddCarrier.DataSource = Engine.CarrierActions.GetMedSup();

    //        ddCarrier.DataValueField = "Key";
    //        ddCarrier.DataTextField = "Name";
    //        ddCarrier.DataBind();

    //    }
    //    catch { }

    //    BindIndividuals();
    //}

    //public void BindIndividualsDropDown(string selectedValue)
    //{
    //    BindIndividuals();

    //    if (ddIndividualName.Items == null || ddIndividualName.Items.Count == 0)
    //    {
    //        return;
    //    }

    //    ListItem itm = ddIndividualName.Items.FindByValue(selectedValue);

    //    if (itm != null)
    //    {
    //        itm.Selected = true;
    //    }
    //}

    //private string formatDate(DateTime? inputdate)
    //{
    //    if (inputdate == null)
    //        return null;
    //    else
    //        return (inputdate.Value.ToShortDateString());
    //}
    //void InitializeDetail()
    //{

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
    //public void CancelOnForm_Click(object sender, EventArgs e)
    //{

    //}
    //public void Save_Click(object sender, EventArgs e)
    //{
    //    SaveForm(true);
    //}
    //protected override void InnerPostBack()
    //{
    //}
    //public void btnAddNewMedicare_Click(object sender, EventArgs e)
    //{
    //    SetPageMode(PageDisplayMode.EditQueueTemplate);
    //}
    //public void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        SaveAndClose();
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex);
    //    }
    //}
    //void SaveAndClose()
    //{
    //    bool hasSavedRecordSuccessful = SaveForm();
    //    if (hasSavedRecordSuccessful)
    //    {
    //        SetPageMode(PageDisplayMode.GridQueueTemplate);
    //    }
    //}
    //public bool HideGrid
    //{
    //    get
    //    {
    //        return hdnHideGrid.Value == "1";
    //    }
    //    set
    //    {
    //        hdnHideGrid.Value = value ? "1" : "0";
    //    }
    //}
    //public bool IsOnActionWizard
    //{
    //    get
    //    {
    //        return hdnIsOnActionWizard.Value == "1";
    //    }
    //    set
    //    {
    //        hdnIsOnActionWizard.Value = value ? "1" : "0";
    //    }
    //}
    //public long IndividualId
    //{
    //    get
    //    {
    //        return Convert.ToInt64(ddIndividualName.SelectedValue);
    //    }
    //}
    //public void PagingBar_Event(object sender, PagingEventArgs e)
    //{
    //    BindGrid();
    //}
    //bool SaveForm(bool ConvertToEditMode = false)
    //{
    //    try
    //    {
    //        //long accountId = AccountIDForm;
    //        long individualID = Helper.SafeConvert<long>((hdnFieldEditIndividual.Value != "")?hdnFieldEditIndividual.Value:"0");
    //        long leadId = Helper.SafeConvert<long>(hdnFieldLeadId.Value);
    //        long policyId = RecordId;//Helper.ConvertToLong(hdnFieldPolicyId.Value);

    //        if (RecordId == 0)
    //        {
    //            Medsup entity = new Medsup { AccountId = AccountID, AddedOn = DateTime.Now, AddedBy = base.SalesPage.CurrentUser.FullName };
    //            GetValues(/*AccountID, leadId, */ ref entity);
    //            //entity.AddedBy = base.SalesPage.CurrentUser.FullName;
    //            var recordAdded = Engine.MedsupActions.Add(entity);
    //            if (ConvertToEditMode)
    //                RecordId = recordAdded.Key;
    //        }
    //        else
    //        {
    //            var entity = Engine.MedsupActions.GetPolicyInfo(RecordId);
    //            GetValues(/*AccountID, leadId, */ref entity);
    //            entity.ChangedBy = base.SalesPage.CurrentUser.FullName;
    //            Engine.MedsupActions.Update(entity);

    //        }
    //        ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    //        return true;

    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus("Error: " + ex.Message);
    //        return false;
    //    }
    //}
    //public void SetIndividual(string selectedValue)
    //{

    //}
    //public void Evt_SortGrid(object sender, GridSortCommandEventArgs e)
    //{
    //    BindGrid(e.SortExpression, e.NewSortOrder == GridSortOrder.Ascending);
    //}   
    //protected void ddlPolicyStatus_SelectedIndexChanged(object sender, EventArgs e)
    //{
    //    if (ddlPolicyStatus.SelectedItem.Text != string.Empty)
    //    {
    //        if (ddlPolicyStatus.SelectedItem.Text.ToUpper().Contains("SUBMITTED") ||
    //            ddlPolicyStatus.SelectedItem.Text.ToUpper() == "ENROLLED")
    //            rdpSubmitDateTimeStamp.SelectedDate = DateTime.Now;
    //    }
    //    else
    //        rdpSubmitDateTimeStamp.SelectedDate = null;
    //}
    protected void gridMedSup_ItemDataBound(object sender, GridItemEventArgs e)
    {
        DisableDeleteInRadGrid(e.Item, "lnkDelete", "lnkDeleteSeperator");
    }
}
