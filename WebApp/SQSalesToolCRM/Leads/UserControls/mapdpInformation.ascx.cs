using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;

public partial class Leads_UserControls_mapdpInformation :
    AccountsBaseControl,
    IIndividualNotification,
    IWrittingAgentSet
{
    public string MAPInformationRadWindowClientID
    { get { return dlgMAPDpInformation.ClientID; } }
    public bool IsDisplayTimeStamp = false;
    public bool IsAutoPostBackPolicyStatus = true;
    public String DefaultPolicyStatus = string.Empty;
    //YA[28 May 2014]
    public event EventHandler FreshPolicyStatus;
    protected void OnFreshPolicyStatus(EventArgs e)
    {
        if(FreshPolicyStatus != null)
        {
            FreshPolicyStatus(this, e);
        }
    }

    //YA[6 June 2014]
    public event EventHandler OnNewIndividual = null;
    public bool ShowAddIndividualButton
    {
        set
        {
            btnAddNewIndividual.Visible = value;
        }
    }
    //IH 21.10.13 - IsSelectedWritingAgent set to true from apply action work flow. On true basis ddlWritingAgent selected as a default loggedin users.
    //public bool IsSelectedWritingAgent = false;

    //SZ [Aug 28, 2013] implements the interfcae for setting the writting agent
    public void SetAgent(Guid agentId)
    {
        if (ddEnroller.Items.FindByValue(agentId.ToString()) != null)
            ddEnroller.SelectedValue = agentId.ToString();
    }

    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
            BindIndividuals();
    }
    public void SetIndividual(string selectedValue)
    {
        if (ddFullName.Items.FindByValue(selectedValue) != null)
            ddFullName.SelectedValue = selectedValue;
    }

    public void SelectMAPDPType(String type)
    {
        //WM - [01.08.2013]
        //ddMapdOrPDP.SelectedItem.Text = type;
               
        switch (type)
        {
            case "MAPD":
                ddMapdOrPDP.SelectedIndex = 1;
                break;
            case "Standalone PDP":
                ddMapdOrPDP.SelectedIndex = 2;
                break;
            case "MA only":
                ddMapdOrPDP.SelectedIndex = 3;
                break;
        }
    }

    void GetValues(ref Mapdp entity)
    {


        entity.IndividualId = Convert.ToInt64(ddFullName.SelectedValue);

        if (!String.IsNullOrEmpty(ddMapdOrPDP.SelectedValue))
            entity.Type = Convert.ToInt64(ddMapdOrPDP.SelectedValue);
        entity.CompanyName = txtCompanyName.Text;
        entity.Carrier = ddCarrier.SelectedValue == string.Empty ? (long?)null : Convert.ToInt64(ddCarrier.SelectedValue);
        entity.PlanNumber = tbPlanNumber.Text;
        entity.PlanType = tbPlanType.Text;
        entity.PlanName = tbPlanName.Text;

        entity.EnrollmentDate = rdpEnrollmentDate.SelectedDate;
        entity.EffectiveDate = rdpEffectiveDate.SelectedDate;
        entity.WritingNumber = tbWritingNumber.Text;
        //entity.MaritalStatus = tbMaritalStatus.Text;
        entity.PolicyIdNumber = tbPolicyIDNumber.Text;

        if (!string.IsNullOrEmpty(ddFirstTimeorSwitcher.SelectedValue))
            entity.Switcher = ddFirstTimeorSwitcher.SelectedValue;

        if (!string.IsNullOrEmpty(ddCovertryPDPReferal.SelectedValue))
            entity.CoventryPdpReferal = CovertStringToBool(ddCovertryPDPReferal.SelectedValue);
        entity.VoiceSigSentDate = rdpCoventryVoiceSigSentDate.SelectedDate;
        entity.MaIssueDate = rdpMAIssueDate.SelectedDate;
        entity.LapseDate = rdpMAMAPDPDPLapseDate.SelectedDate;
        entity.MedicareId = tbMedicareId.Text;

        if (!string.IsNullOrEmpty(txtCommissionAmount.Text))
            entity.CommissionAmount = Convert.ToDouble(txtCommissionAmount.Text);
        entity.CommissionPaidDate = rdpMAPDCommissionPaidDate.SelectedDate;
        entity.DtePurchasedPdp = chkPurchasedPDP.Checked;
        entity.PaidFromCarrier = chkPaidFromCarrier.Checked;
        entity.IsActive = true;
        entity.IsDeleted = false;

        if (Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue) != entity.PolicyStatus)
        {
            Engine.AccountHistory.PolicyStatusChanged(AccountID, ddlPolicyStatus.SelectedItem.Text, "Policy - " + ddMapdOrPDP.SelectedItem.Text, base.SalesPage.CurrentUser.Key, 0 , Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue).Value);
            OnFreshPolicyStatus(EventArgs.Empty);
        }

        //WM-[31.07.13]
        entity.PolicyStatus = string.IsNullOrWhiteSpace(ddlPolicyStatus.SelectedValue) ? null : Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue);
        //IH
        entity.Enroller = ddEnroller.SelectedValue != string.Empty ? new Guid(ddEnroller.SelectedValue) : (Guid?)null;//ddEnroller.SelectedValue == string.Empty ? null : Convert.ToString(ddEnroller.SelectedValue);// ddEnroller.SelectedValue != "-1" ? new Guid(ddEnroller.SelectedValue) : (Guid?)null;
        entity.ReasonCodeKey = Helper.NullConvert<int>(ddlReasonCode.SelectedValue);
    }
    void SetValues(Mapdp record)
    {
        RecordId = record.Key;
        hdnFieldEditIndividual.Value = record.IndividualId.ToString();

        if (record.IndividualId > 0)
            ddFullName.SelectedValue = record.IndividualId.ToString();

        //if (ddMapdOrPDP.Items.Count > 0)
        ddMapdOrPDP.SelectedValue = record.Type.ToString();
        //ddEnroller.SelectedValue = record.Enroller;

        //IH  03.09.13
        ddEnroller.SelectedValue = (record.Enroller == null) ? (ddEnroller.SelectedValue = string.Empty) : record.Enroller.ToString();

        // ddEnroller.SelectedValue = (record.Enroller == null) ? SalesPage.CurrentUser.Key.ToString() : record.Enroller.ToString();


        ddCarrier.SelectedValue = record.Carrier.ToString();

        txtCompanyName.Text = record.CompanyName;
        tbPlanNumber.Text = record.PlanNumber;
        tbPlanType.Text = record.PlanType;
        tbPlanName.Text = record.PlanName;
        //tbSpecialNotes.Text = currentEntity.SpecialNotes;
        rdpEnrollmentDate.SelectedDate = record.EnrollmentDate;
        hdnPolicyStatus.Value = record.EnrollmentDate == null ? "0" : "1"; //ddlPolicyStatus event selected index changed event will be raised on hidden field value.
        ddlPolicyStatus.AutoPostBack = record.EnrollmentDate == null;
        rdpEffectiveDate.SelectedDate = record.EffectiveDate;
        tbWritingNumber.Text = record.WritingNumber;
        //tbMaritalStatus.Text = currentEntity.MaritalStatus;
        tbPolicyIDNumber.Text = record.PolicyIdNumber;
        if (ddFirstTimeorSwitcher.Items.Count > 0)
            ddFirstTimeorSwitcher.SelectedValue = record.Switcher;
        if (ddCovertryPDPReferal.Items.Count > 0)
            ddCovertryPDPReferal.SelectedValue = record.CoventryPdpReferal == null ? null : ConvertBoolToString(record.CoventryPdpReferal);

        rdpCoventryVoiceSigSentDate.SelectedDate = record.VoiceSigSentDate;
        rdpMAIssueDate.SelectedDate = record.MaIssueDate;
        rdpMAMAPDPDPLapseDate.SelectedDate = record.LapseDate;
        tbMedicareId.Text = record.MedicareId;

        txtCommissionAmount.Text = record.CommissionAmount.HasValue ? record.CommissionAmount.ToString() : "";
        rdpMAPDCommissionPaidDate.SelectedDate = record.CommissionPaidDate;
        chkPurchasedPDP.Checked = record.DtePurchasedPdp ?? false;
        chkPaidFromCarrier.Checked = record.PaidFromCarrier ?? false;

        //ddlWritingAgent.SelectedValue = (currentEntity.Enroller == null) ? (ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString()) : currentEntity.Enroller.ToString();

        //WM - [30.07.2013]
        if (record.PolicyStatus.HasValue)
        {
            ddlPolicyStatus.SelectedValue = record.PolicyStatus.Value.ToString();
        }
        else
        {
            ddlPolicyStatus.SelectedIndex = 0;
        }

        //IH 25.07.13
        rdpSubmitDateTimeStamp.SelectedDate = ddlPolicyStatus.SelectedItem.Text.ToUpper().Contains("SUBMITTED") ||
                                             ddlPolicyStatus.SelectedItem.Text.ToUpper() == "ENROLLED"
                                                 ? (record.ChangedOn ?? record.AddedOn)
                                                 : null;
        //IH 15.08.13
        var userPermission = ((SalesBasePage)Page).CurrentUser.UserPermissions.FirstOrDefault();
        if (RecordId != 0 && userPermission != null && userPermission.Permissions.Account != null)
            rdpEnrollmentDate.Enabled = userPermission.Permissions.Account.EditSubmitEnrollDates;
        if (record.ReasonCodeKey.HasValue)
            ddlReasonCode.SelectedValue = record.ReasonCodeKey.Value.ToString();
    }
    void ClearFields()
    {
        RecordId = 0;
        ctlStatus.Initialize();
        hdnPolicyStatus.Value = "0";// IsAutoPostBackPolicyStatus ? "0" : "1";

        //SZ [July 3, 2013] Addeed the call to fix various bugs. This is the right place to call BindIndivudlas
        BindIndividuals();

        var enrollers = Engine.UserActions.GetWritingAgents().ToList().Select(x => new
        {
            Key = x.Key,
            UserName = x.FirstName + ", " + x.LastName
        });

        //Added by Tauseef and Removed temporary fix
        BindPolicyTypes();


        //var enrollerresult = (from x in enrollers
        //                      select new
        //                      {
        //                          Key = x.Key,
        //                          UserName = x.LastName + ", " + x.FirstName
        //                      });

        ddEnroller.DataSource = enrollers;
        ddEnroller.DataBind();
        //ddEnroller.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
        ddEnroller.Items.Insert(0, new ListItem(string.Empty, String.Empty));
        ddEnroller.SelectedIndex = 0;
        ////IH 21.10.13 - IsSelectedWritingAgent set to true from apply action work flow. On true basis ddEnroller select as a default loggedin users. 
        //if (IsSelectedWritingAgent)
        //    ddEnroller.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
        ddCarrier.DataSource = Engine.CarrierActions.GetMaPDP();
        ddCarrier.DataBind();
        //IH 22.07.13
        ddCarrier.Items.Insert(0, new ListItem(String.Empty, String.Empty));
        ddCarrier.SelectedIndex = 0;

        ddlPolicyStatus.DataSource = Engine.PolicyStatusActions.GetAll(/*Policy status type = */3);
        ddlPolicyStatus.DataBind();

        //IH 22.07.13
        ddlPolicyStatus.Items.Insert(0, new ListItem(""));
        ddlPolicyStatus.AutoPostBack = hdnPolicyStatus.Value == "0";//IsAutoPostBackPolicyStatus;
        //ddlPolicyStatus.SelectedIndex = 0;
        rdpEnrollmentDate.SelectedDate = null;
        rdpEnrollmentDate.Clear();
        //IH 16.09.13
        if (DefaultPolicyStatus != string.Empty)
        {
            ddlPolicyStatus.SelectedIndex = ddlPolicyStatus.Items.IndexOf(ddlPolicyStatus.Items.FindByText(DefaultPolicyStatus));
            if (DefaultPolicyStatus == "Active")
                rdpEnrollmentDate.SelectedDate = DateTime.Now;
        }
        if (ddMapdOrPDP.Items.Count > 0)
            ddMapdOrPDP.SelectedIndex = -1;
        //if (ddEnroller.Items.Count > 0)
        //    ddEnroller.SelectedValue = "-1";// this.SalesPage.CurrentUser.Key.ToString();
        if (ddCarrier.Items.Count > 0)
            ddCarrier.SelectedIndex = 0;

        txtCompanyName.Text = string.Empty;
        tbPlanNumber.Text = "";
        tbPlanType.Text = "";
        tbPlanName.Text = "";
        //tbSpecialNotes.Text = "";

        rdpEffectiveDate.SelectedDate = null;
        tbWritingNumber.Text = "";
        tbWritingNumber.Text = "";
        tbMedicareId.Text = "";
        //tbMaritalStatus.Text = "";
        tbPolicyIDNumber.Text = "";
        if (ddFirstTimeorSwitcher.Items.Count > 0)
            ddFirstTimeorSwitcher.SelectedIndex = -1;
        if (ddCovertryPDPReferal.Items.Count > 0)
            ddCovertryPDPReferal.SelectedIndex = -1;

        rdpCoventryVoiceSigSentDate.Clear();
        rdpEffectiveDate.Clear();
        //rdpEnrollmentDate.Clear();
        rdpMAIssueDate.Clear();
        rdpMAMAPDPDPLapseDate.Clear();
        txtCommissionAmount.Text = "";
        rdpMAPDCommissionPaidDate.Clear();
        chkPurchasedPDP.Checked = false;
        chkPaidFromCarrier.Checked = false;
        //IH 25.07.13
        rdpSubmitDateTimeStamp.SelectedDate = IsDisplayTimeStamp == false ? (DateTime?)null : DateTime.Now;

        ddlReasonCode.DataSource = Engine.MapdpActions.MapDPReasonCodes.OrderBy(x=>x.Reason);
        ddlReasonCode.DataBind();
        if (ddlReasonCode.Items.Count > 0)
            ddlReasonCode.SelectedIndex = 0;
    }

    // SZ [Apr 22, 2013] Created to encapsulete the grid displauy and hiding fucntionality
    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;
        // divGrid.Visible = bShow;
        //  divForm.Visible = !bShow;
        if (IsAutoPostBackPolicyStatus)
        {
            dlgMAPDpInformation.DestroyOnClose = true;
            dlgMAPDpInformation.VisibleOnPageLoad = true;
            dlgMAPDpInformation.Visible = true;
            dlgMAPDpInformation.Modal = true;
            dlgMAPDpInformation.RenderMode = RenderMode.Classic;
            dlgMAPDpInformation.VisibleTitlebar = true;
            dlgMAPDpInformation.Height = 550;
        }
        else
        {
            dlgMAPDpInformation.DestroyOnClose = false;
            divGrid.Visible = IsGridMode;
            dlgMAPDpInformation.VisibleOnPageLoad = !IsGridMode;
            dlgMAPDpInformation.Visible = !IsGridMode;
            dlgMAPDpInformation.Modal = false;
            dlgMAPDpInformation.RenderMode = RenderMode.Lightweight;
            dlgMAPDpInformation.VisibleTitlebar = false;
            dlgMAPDpInformation.BorderStyle = BorderStyle.None;
            dlgMAPDpInformation.Height = 460;
            dlgMAPDpInformation.BorderWidth = 0;
            dlgMAPDpInformation.CssClass = "borderLessDialog";
            dlgMAPDpInformation.DestroyOnClose = false;
            dlgMAPDpInformation.RestrictionZoneID = "restrictionZone";
            dlgMAPDpInformation.Width = 960;
            dlgMAPDpInformation.Left = 0;
            divFormFS.Style.Remove("margin");
        }
        if (bShow)
        {
            dlgMAPDpInformation.DestroyOnClose = false;
            dlgMAPDpInformation.VisibleOnPageLoad = false;
            dlgMAPDpInformation.Visible = false;
        }
    }
    void BindGrid(string sortby = "", bool bAscending = true)
    {
        //try
        //{
        //    //var carrierRecords = Engine
        //    if (this.AccountID > 0)
        //    {
        //var individuals = (Page as IIndividual).Individuals;
        //var individuals = Engine.IndividualsActions.GetAllAccountID(this.AccountID);
        //var individuals = (Page as IIndividual).Individuals.Select(
        //    T => new
        //    {
        //        IndividualID = T.Id,
        //        FullName = T.LastName + "," + T.FirstName
        //    }).AsQueryable();

        //var individuals = Engine.IndividualsActions.GetByAccountID(this.AccountID).Select(
        //var individuals = GetIndividualsByAccount().Select(
        //    T => new
        //    {
        //        IndividualID = T.Id,
        //        FullName = T.LastName + "," + T.FirstName
        //    }).AsQueryable();
        //var carriers = Engine.CarrierActions.GetMaPDP().Select(
        //    T => new
        //    {
        //        Key = T.Key,
        //        CarrierName = T.Name
        //    }).AsQueryable();

        //var carriers = Engine.CarrierActions.GetMedSup();

        //var entity = Engine.MapdpActions.GetAllRecordByAccountID(this.AccountID).Select(T => new
        //{
        //    Key = T.Key,
        //    IndividualId = T.IndividualId,
        //    FullName = "",
        //    AccountID = T.AccountId,
        //    Type = T.Type,
        //    CarrierId = T.Carrier,
        //    PolicyID = T.PolicyIdNumber,
        //    PlanNumber = T.PlanNumber,
        //    PlanType = T.PlanType,
        //    Enroller = T.Enroller,
        //    EnrollmentDate = T.EnrollmentDate,
        //    EffectiveDate = T.EffectiveDate
        //}).AsQueryable();
        //if (entity.Count() > 0)
        //{
        //    var firstJoin = (from x in entity
        //                     join y in individuals
        //                     on x.IndividualId equals y.Id
        //                     select new
        //                     {
        //                         Key = x.Key,
        //                         FullName = (y.FirstName != null) ? y.LastName + " " + y.FirstName : "",
        //                         AccountID = (x.AccountID != null) ? x.AccountID : 0,
        //                         Type = (x.Type != null) ? x.Type : 0,
        //                         CarrierId = (x.CarrierId != null) ? x.CarrierId : 0,
        //                         PolicyID = (x.PolicyID != null) ? x.PolicyID : "",
        //                         PlanNumber = (x.PlanNumber != null) ? x.PlanNumber : "",
        //                         PlanType = (x.PlanType != null) ? x.PlanType : "",
        //                         Enroller = (x.Enroller != null) ? x.Enroller : "",
        //                         EnrollmentDate = (x.EnrollmentDate != null) ? x.EnrollmentDate : DateTime.Now,
        //                         EffectiveDate = (x.EffectiveDate != null) ? x.EffectiveDate : DateTime.Now
        //                     }).AsQueryable();

        /*
         * (from x in medsupRecords
                        join y in carrierRecords
                        on x.Carrier equals y.Key into car
                        from y in car.DefaultIfEmpty()
                        where y == null
                        select new
         */

        //var result = (from x in firstJoin
        //              join y in carriers
        //              on x.CarrierId equals y.Key into car
        //              from y in car.DefaultIfEmpty()
        //              where y != null
        //              select new
        //              {
        //                  Key = x.Key,
        //                  IndividualId = x.Key != null ? x.Key : 0,
        //                  FullName = x.FullName != null ? x.FullName : "",
        //                  AccountID = x.AccountID > 0? x.AccountID:0,
        //                  Type = x.Type>0?x.Type:0,
        //                  CarrierId = x.CarrierId > 0 ? x.CarrierId : 0,
        //                  Carrier = y != null ? y.Name : "",
        //                  PolicyID = x.PolicyID != null?x.PolicyID:"",
        //                  PlanNumber = x.PlanNumber != null ? x.PlanNumber : "",
        //                  PlanType = x.PlanType != null ? x.PlanType : "",
        //                  Enroller = x.Enroller != null ? x.Enroller : "",
        //                  EnrollmentDate = (x.EnrollmentDate != null) ? x.EnrollmentDate : DateTime.Now,
        //                  EffectiveDate = (x.EffectiveDate != null) ? x.EffectiveDate : DateTime.Now
        //              }).AsQueryable();

        //gridMAPDP.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(firstJoin, sortby, bAscending));
        var records = Engine.MapdpActions.GetAllByAccount(AccountID);
        gridMAPDP.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(records, sortby, bAscending));
        gridMAPDP.DataBind();
        //            return;
        //        }
        //    }            
        //    //gridMAPDP.DataSource = null;
        //    //gridMAPDP.DataBind();
        //    PagingNavigationBar.Visible = false;
        //}
        //catch (Exception ex)
        //{
        //    lblMessageGrid.Text = "Error: " + ex.Message;
        //}
    }



    void BindIndividuals()
    {
        ddFullName.DataSource = ((IIndividual)Page).Individuals;
        ddFullName.DataBind();
        //[IH, 17-07-2013]
        ddFullName.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddFullName.SelectedIndex = 0;
    }
    //YA[6 June 2014]
    public void BindClientEvent(string clientID)
    { 
        btnAddNewIndividual.OnClientClick = "javascript:return showDlg('" + clientID +"');";
    }
    void BindEvents()
    {
        btnAddnewMAPDP.Click += (o, a) => AddRecord();
        btnCancelOnForm.Click += (o, a) => CloseRecord();
        btnSaveOnForm.Click += (o, a) => { if (IsValidated) SaveRecord(); };
        btnSaveAndCloseOnForm.Click += (o, a) => { if (IsValidated) { SaveRecord(); CloseRecord(); } };
        btnReturn.Click += (o, a) => CloseRecord();
        //IH 02.08.10
        ddlPolicyStatus.AutoPostBack = hdnPolicyStatus.Value == "0";//IsAutoPostBackPolicyStatus && IsDisplayTimeStamp;
        if (hdnPolicyStatus.Value == "0")
            ddlPolicyStatus.SelectedIndexChanged += (o, a) => PolicyStatusChanged();
        PagingNavigationBar.IndexChanged += (o, a) => BindGrid();
        PagingNavigationBar.SizeChanged += (o, a) => BindGrid();

        gridMAPDP.SortCommand += (o, a) => BindGrid(a.SortExpression, a.NewSortOrder == GridSortOrder.Ascending);
        gridMAPDP.ItemCommand += (o, a) => CommandRouter(Convert.ToInt64(a.CommandArgument), a.CommandName);
        //YA[06 June 2014] 
        btnAddNewIndividual.Click += (o, a) => { if (OnNewIndividual != null) OnNewIndividual(this, new EventArgs()); };
    }
    public void PolicyStatusChanged()
    {
        rdpSubmitDateTimeStamp.SelectedDate = null;
        if (ddlPolicyStatus.SelectedItem.Text != string.Empty)
        {
            if (ddlPolicyStatus.SelectedItem.Text.ToUpper().Contains("SUBMITTED") ||
                ddlPolicyStatus.SelectedItem.Text.ToUpper() == "ENROLLED")
                rdpSubmitDateTimeStamp.SelectedDate = DateTime.Now;

            rdpEnrollmentDate.SelectedDate = ddlPolicyStatus.SelectedItem.Text.ToUpper().Contains("ACTIVE")
                                                 ? DateTime.Now
                                                 : (DateTime?)null;

        }


    }
    // SZ [July 3, 2013] Avoid hard coded Dropdown values as they are problematic to properly convert
    bool CovertStringToBool(string inputString)
    {
        //IH 05.10.13 Optimized the statement
        return inputString != null && inputString == "Yes";
        //if (inputString != null)
        //{
        //    if (inputString == "Yes")
        //        return true;
        //    else
        //        return false;
        //}
        //else
        //    return false;
    }

    string ConvertBoolToString(bool? inputBool)
    {
        //IH 05.10.13 Optimized the statement
        return inputBool == true ? "Yes" : "No";
        //if (inputBool == true)
        //    return "Yes";
        //else
        //    return "No";
    }


    long RecordId
    {
        get
        {
            long l = 0L;
            long.TryParse(hdnFieldPolicyId.Value, out l);
            return l;
        }
        set
        {
            hdnFieldPolicyId.Value = value.ToString();
        }
    }
    void AddRecord()
    {
        ClearFields();
        ShowGrid(false);
    }
    void EditRecord(long id)
    {
        try
        {
            Mapdp Record = Engine.MapdpActions.GetByKey(id);
            if (Record != null)
            {
                ClearFields();
                SetValues(Record);
                ShowGrid(false);
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void CloseRecord()
    {
        ShowGrid();
        BindGrid();
    }
    void DeleteRecord(long Id)
    {
        try
        {
            Engine.MapdpActions.Delete(Id);
            BindGrid();
            //SR ctlStatus.SetStatus(Messages.RecordDeletedSuccess);
        }
        catch (Exception ex)
        {
            ctlDeleteStatus.SetStatus(ex);
        }
    }
    bool SaveRecord(bool checkRequiredFields = false)
    {
        bool saveFlagError = false;
        try
        {
            Mapdp record = (RecordId == 0) ? new Mapdp { AccountId = AccountID, AddedBy = SalesPage.CurrentUser.FullName, AddedOn = IsDisplayTimeStamp ? Convert.ToDateTime(rdpSubmitDateTimeStamp.SelectedDate.ToString()) : DateTime.Now } :
                Engine.MapdpActions.GetByKey(RecordId);

            GetValues(ref record);

            if (checkRequiredFields)
            {
                saveFlagError = CheckRequiredFields(record);
                if (!saveFlagError)
                {
                    RecordSave(record);
                }
            }
            else
                RecordSave(record);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
        return saveFlagError;
    }

    private void RecordSave(Mapdp record)
    {
        if (RecordId == 0)
        {
            Engine.MapdpActions.Add(record);
            RecordId = record.Key;
        }
        else
        {
            record.ChangedBy = SalesPage.CurrentUser.FullName;
            record.ChangedOn = DateTime.Now;
            Engine.MapdpActions.Update(record);
        }

        //SR ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    }
    /// <summary>
    /// Check the required fields setup at Sub Status II
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private bool CheckRequiredFields(Mapdp entity)
    {
        //YA[July 15, 2013] Duplicate record implementation.
        // string nTypeValues = "";//new string[] { "medicare advantage (ma)", "prescription drug plans (pdp)" };
        string nTypeValues = ddMapdOrPDP.SelectedItem.Text == "MA only" ? "Medicare Advantage (MA)" : ddMapdOrPDP.SelectedItem.Text == "Standalone PDP" ? "Prescription Drug Plans (PDP)" : "Medicare Advantage / Prescription Drugs (MAPDP)";
        bool hasErrors = false;
        string errorMessage = string.Empty;
        RequiredFieldChecker nduplicate = new RequiredFieldChecker();
        nduplicate.RequiredFieldsChecking(entity, ref hasErrors, ref errorMessage, nTypeValues, this.AccountID);
        ctlStatus.SetStatus(new Exception(errorMessage));
        return hasErrors;
    }

    void CommandRouter(long id, string command)
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

    public override bool IsValidated
    {
        get
        {
            string errorMessage = "Error Required Field(s): ";            
            vldIndividualName.Validate();
            if (!vldIndividualName.IsValid)
                ctlStatus.SetStatus(new Exception(errorMessage + vldIndividualName.ErrorMessage));
            return vldIndividualName.IsValid;
        }
    }
    protected override void InnerInit()
    {
        IsGridMode = true;
        
        //AccountIDForm = AccountID;
    }
    protected override void InnerSave()
    {
        if (IsValidated)
            SaveRecord();
        if (CloseForm)
            CloseRecord();
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        BindEvents();

        if (Page is IIndividual)
            (Page as IIndividual).Notify(this);

        if (bFirstTime)
        {
            ShowGrid();
            BindGrid();
            //BindPolicyTypes();
        }
    }

    public void BindPolicyTypes()
    {
        var U =Engine.PolicyStatusActions.GetAllPolicyTypes().ToList();

        ddMapdOrPDP.DataTextField = "Name";
        ddMapdOrPDP.DataValueField = "ID";
        ddMapdOrPDP.DataSource = U;
        ddMapdOrPDP.DataBind(); 
        ddMapdOrPDP.Items.Insert(0, new ListItem(string.Empty, "0"));
        
    }

    public void Action_Add()
    {
        AddRecord();
        divButtons.Visible = false;
        btnReturn.Visible = false;
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
            btnAddnewMAPDP.Visible = bEnable;
            var colEdit = gridMAPDP.Columns.FindByUniqueName("colEdit");
            var colView = gridMAPDP.Columns.FindByUniqueName("colView");
            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
        {
            EnableControls(tblControls, bEnable);
        }
    }
    //SZ [Apr 19, 2013] These are teh functions that make it posisble for the leads page to call the saving 
    // This is the fix for the bug #118 as reported on mantis. The three functions are the implementation of the 
    //interface ILeadControlSave, in the accounts base page
    //public override bool IsEditingRecord
    //{
    //    get
    //    {
    //        return !this.IsGridMode;
    //    }
    //}
    //protected override void InnerPostBack()
    //{
    //    lblMessageForm.Text = "";
    //    lblMessageGrid.Text = "";
    //}
    //public bool IsGridMode
    //{
    //    get
    //    {
    //        bool bAns = default(bool);
    //        bool.TryParse(hdnGridMode.Value, out bAns);
    //        return bAns;
    //    }
    //    set
    //    {
    //        hdnGridMode.Value = value.ToString();
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
    //public void BindDropDowns()
    //{
    //    // SZ [Jul3, 2013] this should not be called seperately
    //    BindIndividuals();

    //    ddEnroller.DataSource = Engine.UserActions.GetAll().Select(x => new { Key = x.Key, UserName = string.Format("{0} {1}", x.LastName, x.FirstName) }).ToList();
    //    ddEnroller.DataBind();
    //    ddEnroller.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();

    //    ddCarrier.DataSource = Engine.CarrierActions.GetMaPDP();
    //    ddCarrier.DataBind();
    //    ddCarrier.SelectedIndex = -1;

    //}
    // void LoadEditFormValues(long policyKey)
    //{
    //    Mapdp currentEntity = Engine.MapdpActions.GetByKey(policyKey);
    //    //AccountIDForm = currentEntity.AccountId ?? 0;
    //    hdnFieldEditIndividual.Value = currentEntity.IndividualId.ToString();
    //    if (currentEntity.IndividualId > 0)
    //    {
    //        ddFullName.SelectedValue = currentEntity.IndividualId.ToString();
    //    }
    //    else
    //        ddFullName.SelectedIndex = -1;

    //    if (ddMapdOrPDP.Items.Count > 0)
    //    {
    //        ddMapdOrPDP.SelectedValue = currentEntity.Type.ToString();
    //    }
    //    if (ddEnroller.Items.Count > 0)
    //    {
    //        ddEnroller.SelectedValue = currentEntity.Enroller;
    //    }
    //    if (ddCarrier.Items.Count > 0)
    //    {
    //        ddCarrier.SelectedValue = currentEntity.Carrier.ToString();
    //    }
    //    //txtCompanyName.Text = currentEntity.CompanyName;
    //    tbPlanNumber.Text = currentEntity.PlanNumber;
    //    tbPlanType.Text = currentEntity.PlanType;
    //    tbPlanName.Text = currentEntity.PlanName;
    //    //tbSpecialNotes.Text = currentEntity.SpecialNotes;
    //    rdpEnrollmentDate.SelectedDate = currentEntity.EnrollmentDate;
    //    rdpEffectiveDate.SelectedDate = currentEntity.EffectiveDate;
    //    tbWritingNumber.Text = currentEntity.WritingNumber;
    //    //tbMaritalStatus.Text = currentEntity.MaritalStatus;
    //    tbPolicyIDNumber.Text = currentEntity.PolicyIdNumber;
    //    if (ddFirstTimeorSwitcher.Items.Count > 0)
    //    {
    //        ddFirstTimeorSwitcher.SelectedValue = currentEntity.Switcher;
    //    }
    //    if (ddCovertryPDPReferal.Items.Count > 0)
    //    {
    //        ddCovertryPDPReferal.SelectedValue = ConvertBoolToString(currentEntity.CoventryPdpReferal);
    //    }
    //    rdpCoventryVoiceSigSentDate.SelectedDate = currentEntity.VoiceSigSentDate;
    //    rdpMAIssueDate.SelectedDate = currentEntity.MaIssueDate;
    //    rdpMAMAPDPDPLapseDate.SelectedDate = currentEntity.LapseDate;
    //    tbMedicareId.Text = currentEntity.MedicareId;

    //    txtCommissionAmount.Text = currentEntity.CommissionAmount.HasValue ? currentEntity.CommissionAmount.ToString() : "";
    //    rdpMAPDCommissionPaidDate.SelectedDate =currentEntity.CommissionPaidDate;
    //    chkPurchasedPDP.Checked= currentEntity.DtePurchasedPdp ?? false;
    //    chkPaidFromCarrier.Checked= currentEntity.PaidFromCarrier ?? false;

    //    //ddlWritingAgent.SelectedValue = (currentEntity.Enroller == null) ? (ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString()) : currentEntity.Enroller.ToString();

    //    if (ddlPolicyStatus.Items.Count > 0 && currentEntity.PolicyStatus > 0)
    //    {
    //        ddlPolicyStatus.SelectedValue = currentEntity.PolicyStatus.ToString();
    //    }

    //}

    //public void CancelOnForm_Click(object sender, EventArgs e)
    //{
    //    //mainDV.Visible = true;
    //    //editGrid.Visible = false;
    //    ShowGrid();
    //}

    //public void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    SaveForm(true);
    //}

    //public void Save_Click(object sender, EventArgs e)
    //{
    //    SaveForm();
    //}


    //public void PagingBar_Event(object sender, PagingEventArgs e)
    //{
    //    BindGrid();
    //}
    //public void Evt_SortGrid(object sender, GridSortCommandEventArgs e)
    //{
    //    BindGrid(e.SortExpression, e.NewSortOrder == GridSortOrder.Ascending);
    //}

    //public void btnAddNewMAPDP_Click(object sender, EventArgs e)
    //{
    //    ClearFields();
    //    hdnFieldIsEditMode.Value = "no";
    //    //mainDV.Visible = false;
    //    //editGrid.Visible = true;
    //    ShowGrid(false);
    //}
    // void SaveForm(bool closeForm = false)
    //{
    //    try
    //    {
    //        long accountId = AccountID; // AccountIDForm;
    //        long individualId = Helper.SafeConvert<long>(hdnFieldEditIndividual.Value);
    //        long policyId = Helper.SafeConvert<long>(hdnFieldPolicyId.Value);

    //        if (hdnFieldIsEditMode.Value == "no")
    //        {
    //            Mapdp entity = new Mapdp();
    //            GetValues(accountId, entity);
    //            entity.AddedBy = base.SalesPage.CurrentUser.FullName;

    //            Engine.MapdpActions.Add(entity);

    //        }
    //        else if (hdnFieldIsEditMode.Value == "yes")
    //        {
    //            if (hdnFieldPolicyId.Value != "")
    //            {
    //                //var entity = Engine.DriverInfoActions.GetInfoByKey(Convert.ToInt64(hdnFieldPolicyId.Value));
    //                var entity = Engine.MapdpActions.GetByKey(Convert.ToInt64(hdnFieldPolicyId.Value));
    //                GetValues(accountId, entity);

    //                entity.ChangedBy = base.SalesPage.CurrentUser.FullName;

    //                Engine.MapdpActions.Update(entity);
    //            }
    //        }
    //        if (!closeForm)
    //        {
    //            lblMessageForm.Text = Messages.RecordSavedSuccess;
    //            lblMessageGrid.Text = "";
    //        }
    //        else
    //        {
    //            lblMessageGrid.Text = Messages.RecordSavedSuccess;
    //            lblMessageForm.Text = "";
    //            //mainDV.Visible = true;
    //            //editGrid.Visible = false;
    //            ShowGrid();
    //            BindGrid();
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessageForm.Text = "Error: " + ex.Message;
    //    }
    //}
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    if (!Page.IsPostBack)
    //    {
    //        this.divButtons.Visible = !this.HideGrid;

    //        if (this.IsOnActionWizard)
    //        {
    //            btnAddnewMAPDP.Visible = false;

    //            //SZ [July 3, 2013] This can easily be acheieved by div hidding
    //            divButtons.Visible = false;

    //            //btnCancelOnForm.Visible = false;
    //            //btnSaveAndCloseOnForm.Visible = false;
    //            //btnSaveOnForm.Visible = false;
    //            BindIndividuals();
    //            BindDropDowns();
    //        }

    //        //ddlWritingAgent.DataSource = Engine.UserActions.GetWritingAgents();
    //        //ddlWritingAgent.DataBind();
    //        //ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();

    //        /*
    //             * Policy status type is as following at the moment
    //             autohome = 1
    //             Medicare Supplement	= 2
    //             MAPDP = 3
    //             Dental and Vision = 4
    //             */
    //        ddlPolicyStatus.DataSource = Engine.PolicyStatusActions.GetAll(/*Policy status type = */3);
    //        ddlPolicyStatus.DataBind();
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
    //public Int64 AddNewMAPDPInformation(bool clearFields, Int64 AccountId)
    //{
    //    //long accountId = AccountId;
    //    //long individualId = Helper.SafeConvert<long>(hdnFieldEditIndividual.Value);


    //    Mapdp entity = new Mapdp();
    //    GetValues(ref entity);
    //    entity.AddedBy = base.SalesPage.CurrentUser.FullName;

    //    Engine.MapdpActions.Add(entity);

    //    if (clearFields)
    //        ClearFields();

    //    return entity.Key;

    //}
    protected void gridMAPDP_ItemDataBound(object sender, GridItemEventArgs e)
    {
        DisableDeleteInRadGrid(e.Item, "lnkDelete", "lnkDeleteSeperator");
    }
}
