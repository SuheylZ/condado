using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;


public partial class Leads_UserControls_applicationInformation :
    AccountsBaseControl,
    IIndividualNotification
{
    public string ApplicationInformationRadWindowClientID
    { get { return dlgApplicationInformation.ClientID; } }
    public void SetIndividual(string selectedValue)
    {
        if (ddFullName.Items.FindByValue(selectedValue) != null)
        {
            ddFullName.SelectedValue = selectedValue;
        }
    }

    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
        {
            ddFullName.DataSource = (Page as IIndividual).Individuals;
            ddFullName.DataBind();

            // WM - [16.07.2013]
            ddFullName.Items.Insert(0, new ListItem("-- None --", ""));
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

    public bool HideGrid
    {
        get
        {
            return hdnHideGrid.Value == "1";
        }
        set
        {
            hdnHideGrid.Value = value ? "1" : "0";
        }
    }

    public bool IsOnActionWizard
    {
        get
        {
            return hdnIsOnActionWizard.Value == "1";
        }
        set
        {
            hdnIsOnActionWizard.Value = value ? "1" : "0";
        }
    }

    void AddRecord()
    {
        ClearFields();
        ShowGrid(false);
    }

    public bool AddNewApplicationInformation(bool clearFields, Int64 AccountId, ref long key, bool checkRequiredFields = false)
    {
        bool saveFlagError = false;
        medsupApplication A = new medsupApplication
           {
               // WM - 16.07.2013, IndividualId is being set in GetInfo 
               //            IndividualId = Helper.SafeConvert<long>(ddFullName.SelectedValue),
               AccountId = AccountId,
               AddedBy = SalesPage.CurrentUser.FullName,
               AddedOn = DateTime.Now,
               IsActive = true,
               IsDeleted = false
           };

        GetInfo(ref A);
        if (checkRequiredFields)
        {
            saveFlagError = CheckRequiredFields(A);
            if (!saveFlagError)
            {
                Engine.MedsupApplicationActions.Add(A);
            }
        }
        else
            Engine.MedsupApplicationActions.Add(A);

        // WM - 15.07.2013
        //ddFullName.Enabled = false;

        if (clearFields)
            ClearFields();

        key = A.Key;
        return saveFlagError;
    }
    /// <summary>
    /// Check the required fields setup at Sub Status II
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private bool CheckRequiredFields(medsupApplication entity)
    {
        //YA[July 15, 2013] Duplicate record implementation.        
        bool hasErrors = false;
        string errorMessage = "";
        RequiredFieldChecker nduplicate = new RequiredFieldChecker();
        nduplicate.RequiredFieldsChecking(entity, ref hasErrors, ref errorMessage, "Online Submission", this.AccountID);
        ctlStatus.SetStatus(errorMessage);
        return hasErrors;
    }
    void SaveRecord()
    {
        medsupApplication A = RecordId > 0 ? Engine.MedsupApplicationActions.GetAppTrackByKey(RecordId) :
            new medsupApplication
        {
            // WM - 16.07.2013, IndividualId is being set in GetInfo 
            //            IndividualId = Helper.SafeConvert<long>(ddFullName.SelectedValue),
            AccountId = AccountID,
            AddedBy = SalesPage.CurrentUser.FullName,
            AddedOn = DateTime.Now,
            IsActive = true,
            IsDeleted = false
        };

        GetInfo(ref A);

        if (RecordId > 0)
        {
            A.ChangedBy = SalesPage.CurrentUser.FullName;
            A.ChangedOn = DateTime.Now;
            Engine.MedsupApplicationActions.Update(A);
        }
        else
        {
            Engine.MedsupApplicationActions.Add(A);
            // WM - 15.07.2013
            //ddFullName.Enabled = false;
            RecordId = A.Key;
        }
        //  ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    }
    void DeleteRecord(long id)
    {
        try
        {
            Engine.MedsupApplicationActions.Delete(id);
            BindGrid();
            // ctlStatus.SetStatus(Messages.RecordDeletedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    void EditRecord(long id)
    {
        ClearFields();
        var A = Engine.MedsupApplicationActions.GetAppTrackByKey(id);
        SetInfo(A);
        ShowGrid(false);
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

        btnAddnewApplicationTracking.Click += (o, a) => AddRecord();
        btnCancelOnForm.Click += (o, a) => ShowGrid();
        btnSaveAndCloseOnForm.Click += (o, a) =>
        {
            if (IsValidated)
            {
                SaveRecord();
                ShowGrid();
            }
        };
        btnSaveOnForm.Click += (o, a) =>
        {
            if (IsValidated)
                SaveRecord();
        };
        btnReturn.Click += (o, a) => ShowGrid();

        gridAppTrackingInfo.ItemCommand += (o, a) => CommandRouter(a.CommandName, Helper.SafeConvert<long>(a.CommandArgument.ToString()));
        ctlPager.IndexChanged += (o, a) => BindGrid();
        ctlPager.SizeChanged += (o, a) => BindGrid();

    }
    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;

        //divGrid.Visible = bShow;
        // divForm.Visible = !bShow;
        if (!bShow)
        {
            dlgApplicationInformation.DestroyOnClose = true;
            dlgApplicationInformation.VisibleOnPageLoad = true;
            dlgApplicationInformation.Visible = true;
            dlgApplicationInformation.Modal = true;
            dlgApplicationInformation.RenderMode = RenderMode.Classic;
            dlgApplicationInformation.VisibleTitlebar = true;
            dlgApplicationInformation.Height = 750;
        }
        if (bShow)
        {
            dlgApplicationInformation.DestroyOnClose = true;
            dlgApplicationInformation.VisibleOnPageLoad = false;
            dlgApplicationInformation.Visible = false;
            RecordId = 0;
            BindGrid();
        }
    }
    void BindGrid()
    {
        try
        {
            //var carrierRecords = Engine
            //var individuals = (Page as IIndividual).Individuals.AsQueryable();
            //var individuals = GetIndividualsByAccount().Select(//Engine.IndividualsActions.GetByAccountID(this.AccountID).Select(
            //    T => new
            //    {
            //        IndividualID = T.Id,
            //        FullName = T.LastName + "," + T.FirstName
            //    }).AsQueryable();


            //var entity = Engine.MedsupApplicationActions.GetAllTrackingPerAccount(AccountID).Select(
            //    T => new
            //    {
            //        T.Key,
            //        T.IndividualId,
            //        T.AccountId,
            //        T.MedsupId,
            //        T.ApplicationSentToCustomerMethod,
            //        T.ActualApplicationSentDate,
            //        T.PolicySubmitToCarrierDate,
            //        T.SentToCarrierTrackingNumber,
            //        T.SubmitToCarrierStatusNote
            //    }).AsQueryable();
            var Z = Engine.MedsupApplicationActions.GetAllTrackingPerAccount(AccountID).ToList();

            var result = Z
                .GroupJoin((Page as IIndividual).Individuals,
                            a => a.IndividualId,
                            b => b.Id,
                            (a, b) => new { a, b })
                .SelectMany(x => x.b.DefaultIfEmpty(), (x, y) => new { x.a, x.b })
                .Select(x => new
                {
                    Key = x.a.Key,
                    IndividualId = x.a.IndividualId,
                    FullName = x.b == null ? "" : x.b.FirstOrDefault() == null ? "" : x.b.FirstOrDefault().FullName,
                    AccountID = x.a.AccountId,
                    MedSubID = x.a.MedsupId,
                    ApplSentDate = x.a.RequestedApplicationSentDate,
                    ActualAppSentDate = x.a.ActualApplicationSentDate,
                    DeliveryMethod = x.a.ApplicationSentToCustomerMethod,
                    PolicySubmitToCarrierDate = x.a.PolicySubmitToCarrierDate,
                    SentToCarrierTrackingNumber = x.a.SentToCarrierTrackingNumber,
                    SubmitToCarrierStatusNote = x.a.SubmitToCarrierStatusNote
                })
                .AsQueryable();


            //if (entity.Count() > 0)
            //{
            //var result = (from x in entity
            //              join y in individuals
            //              on x.IndividualId equals y.Id
            //              select new
            //              {
            //                  Key = x.Key,
            //                  IndividualId = x.IndividualId,
            //                  FullName = y.FullName,
            //                  AccountID = x.AccountID,
            //                  MedSubID = x.MedSubID,
            //                  ApplSentDate = x.ApplSentDate,
            //                  ActualAppSentDate = x.ActualAppSentDate,
            //                  DeliveryMethod = x.DeliveryMethod,
            //                  PolicySubmitToCarrierDate = x.PolicySubmitToCarrierDate,
            //                  SentToCarrierTrackingNumber = x.SentToCarrierTrackingNumber,
            //                  SubmitToCarrierStatusNote = x.SubmitToCarrierStatusNote
            //              }).AsQueryable();



            gridAppTrackingInfo.DataSource = ctlPager.ApplyPaging(result);
            gridAppTrackingInfo.DataBind();

            //Helper.SortRecords(result, ctlPager.SortBy, ctlPager.SortAscending)
            //ctlPager.Visible = true;
            //gridAppTrackingInfo.Visible = true;
            //}

            //else
            //{
            //    lblMessageGrid.Text = "No Tracking Records Available";
            //    ctlPager.Visible = false;
            //    gridAppTrackingInfo.Visible = false;
            //}

        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex); //lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }


    void GetInfo(ref medsupApplication A)
    {
        if (ddFullName.Items.Count > 0 && !string.IsNullOrWhiteSpace(ddFullName.SelectedValue))
        {
            A.IndividualId = Helper.SafeConvert<long>(ddFullName.SelectedValue);
        }
        else
        {
            A.IndividualId = null;
        }

        if (ddPolicies.Items.Count > 0 && !string.IsNullOrWhiteSpace(ddPolicies.SelectedValue))
        {
            A.MedsupId = Helper.SafeConvert<long>(ddPolicies.SelectedValue);
        }
        else
        {
            A.MedsupId = null;
        }

        if (ddExpectedReturnApplicationMethod.Items.Count > 0 && !string.IsNullOrWhiteSpace(ddExpectedReturnApplicationMethod.SelectedValue))
        {
            A.ExpectedReturnApplicationMethod = ddExpectedReturnApplicationMethod.SelectedValue;
        }
        else
        {
            A.ExpectedReturnApplicationMethod = null;
        }

        if (ddApplicationSenttoCustomerMethod.Items.Count > 0 && !string.IsNullOrWhiteSpace(ddApplicationSenttoCustomerMethod.SelectedValue))
        {
            A.ApplicationSentToCustomerMethod = ddApplicationSenttoCustomerMethod.SelectedValue;
        }
        else
        {
            A.ApplicationSentToCustomerMethod = null;
        }

        if (ddFillFormCaseSpecialist.Items.Count > 0 && !string.IsNullOrWhiteSpace(ddFillFormCaseSpecialist.SelectedValue))
        {
            A.CaseSpecialist = ddFillFormCaseSpecialist.SelectedValue;
        }
        else
        {
            A.CaseSpecialist = null;
        }

        A.RequestedApplicationSentDate = rdpRequestedApplicationSentDate.SelectedDate;
        A.ActualApplicationSentDate = rdpActualApplicationSentDate.SelectedDate;
        A.SubmitToCarrierStatusNote = tbSubmitToCarrierStatusNote.Text;
        A.StatusNote = tbfillFormStatusNote.Text;
        A.ReturnLabelNumber = tbReturnLabelNumber.Text;
        A.StatusNote = tbfillFormStatusNote.Text;
        A.SubmitToCarrierCaseSpecialist = ddSubmittoCarrierCaseSpecialist.SelectedValue;
        A.CarrierStatusNote = tbSubmitToCarrierStatusNote.Text;
        A.PolicySubmitToCarrierDate = rdpPolicySubmitToCarrierDate.SelectedDate;
        A.SentToCarrierMethod = ddSentToCarrierMethod.SelectedValue;
        A.SentToCarrierTrackingNumber = tbSentToCarrierTrackingNumber.Text;
        A.SalesAgentNotes = tbSalesAgentNotes.Text;
        A.CaseSpecialistNote = tbCaseSpecialistNotes.Text;
        A.CaseSpecialistNote2 = tbCaseSpecialistNotes2.Text;
        A.CaseSpecialistNote3 = tbCaseSpecialistNotes3.Text;
    }

    void SetInfo(medsupApplication M)
    {

        RecordId = M.Key;
        // WM - 15.07.2013
        //ddFullName.Enabled = false;

        if (ddFullName.Items.Count > 0)
        {
            if (M.IndividualId.HasValue && M.IndividualId.Value > 0)
            {
                ddFullName.SelectedValue = M.IndividualId.Value.ToString();
            }
            else
            {
                ddFullName.SelectedIndex = -1;
            }
        }

        if (ddPolicies.Items.Count > 0)
        {
            if (M.MedsupId.HasValue && M.MedsupId.Value > 0)
            {
                ddPolicies.SelectedValue = M.MedsupId.Value.ToString();
            }
            else
            {
                ddPolicies.SelectedIndex = -1;
            }
        }

        if (ddExpectedReturnApplicationMethod.Items.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(M.ExpectedReturnApplicationMethod))
            {
                ddExpectedReturnApplicationMethod.SelectedValue = M.ExpectedReturnApplicationMethod;
            }
            else
            {
                ddExpectedReturnApplicationMethod.SelectedIndex = -1;
            }
        }

        if (ddApplicationSenttoCustomerMethod.Items.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(M.ApplicationSentToCustomerMethod))
            {
                ddApplicationSenttoCustomerMethod.SelectedValue = M.ApplicationSentToCustomerMethod;
            }
            else
            {
                ddApplicationSenttoCustomerMethod.SelectedIndex = -1;
            }
        }

        if (ddFillFormCaseSpecialist.Items.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(M.CaseSpecialist))
            {
                ddFillFormCaseSpecialist.SelectedValue = M.CaseSpecialist;
            }
            else
            {
                ddFillFormCaseSpecialist.SelectedIndex = -1;
            }
        }
        if (ddSubmittoCarrierCaseSpecialist.Items.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(M.SubmitToCarrierCaseSpecialist))
            {
                ddSubmittoCarrierCaseSpecialist.SelectedValue = M.SubmitToCarrierCaseSpecialist;
            }
            else
            {
                ddSubmittoCarrierCaseSpecialist.SelectedIndex = -1;
            }
        }
        if (ddSentToCarrierMethod.Items.Count > 0)
        {
            if (!string.IsNullOrWhiteSpace(M.SentToCarrierMethod))
            {
                ddSentToCarrierMethod.SelectedValue = M.SentToCarrierMethod;
            }
            else
            {
                ddSentToCarrierMethod.SelectedIndex = -1;
            }
        }

        rdpRequestedApplicationSentDate.SelectedDate = M.RequestedApplicationSentDate;
        rdpActualApplicationSentDate.SelectedDate = M.ActualApplicationSentDate;
        tbReturnLabelNumber.Text = M.ReturnLabelNumber;
        tbSubmitToCarrierStatusNote.Text = M.SubmitToCarrierStatusNote;
        rdpPolicySubmitToCarrierDate.SelectedDate = M.PolicySubmitToCarrierDate;

        //tbTrackingNumber.Text=
        tbSentToCarrierTrackingNumber.Text = M.SentToCarrierTrackingNumber;
        tbSalesAgentNotes.Text = M.SalesAgentNotes;
        tbCaseSpecialistNotes.Text = M.CaseSpecialistNote;
        tbCaseSpecialistNotes2.Text = M.CaseSpecialistNote2;
        tbCaseSpecialistNotes3.Text = M.CaseSpecialistNote3;
        tbfillFormStatusNote.Text = M.StatusNote;
    }

    void ClearFields()
    {
        if (ddExpectedReturnApplicationMethod.Items.Count > 0)
        {
            ddExpectedReturnApplicationMethod.SelectedIndex = -1;
        }

        if (ddApplicationSenttoCustomerMethod.Items.Count > 0)
        {
            ddApplicationSenttoCustomerMethod.SelectedIndex = -1;
        }

        //if (ddSpouseExpectedReturnApplicationMethod.Items.Count > 0)
        //{
        //    ddSpouseExpectedReturnApplicationMethod.SelectedIndex = -1;
        //}
        //if (ddHasApplicationBeenSentToSpouse.Items.Count > 0)
        //{
        //    ddHasApplicationBeenSentToSpouse.SelectedIndex = -1;
        //}
        //if (ddApplicationSentToSpouseMethod.Items.Count > 0)
        //{
        //    ddApplicationSentToSpouseMethod.SelectedIndex = -1;
        //}



        //if (ddFillFormCaseSpecialist.Items.Count > 0)
        //    ddFillFormCaseSpecialist.SelectedIndex = -1;
        //if (ddSubmittoCarrierCaseSpecialist.Items.Count > 0)
        //    ddSubmittoCarrierCaseSpecialist.SelectedIndex = -1;
        if (ddSentToCarrierMethod.Items.Count > 0)
        {
            ddSentToCarrierMethod.SelectedIndex = -1;
        }
        //if (ddSpouseSentToCarrierMethod.Items.Count > 0)
        //{
        //    ddSpouseSentToCarrierMethod.SelectedIndex = -1;
        //}
        //rdpSpouseRequestedApplicationSentDate.SelectedDate = null;
        //rdpSpousePolicySubmitToCarrierDate.SelectedDate = null;

        RecordId = 0;

        ddFullName.DataSource = (Page as IIndividual).Individuals;
        ddFullName.DataBind();
        // WM - [16.07.2013]
        ddFullName.Items.Insert(0, new ListItem("-- None --", ""));

        var users = Engine.UserActions.GetCSR();
        ddFillFormCaseSpecialist.DataSource = users;
        ddFillFormCaseSpecialist.DataBind();
        // WM - [16.07.2013]
        ddFillFormCaseSpecialist.Items.Insert(0, new ListItem("-- None --", ""));

        ddSubmittoCarrierCaseSpecialist.DataSource = users;
        ddSubmittoCarrierCaseSpecialist.DataBind();
        // WM - [16.07.2013]
        ddSubmittoCarrierCaseSpecialist.Items.Insert(0, new ListItem("-- None --", ""));

        rdpRequestedApplicationSentDate.SelectedDate = null;
        rdpActualApplicationSentDate.SelectedDate = null;
        tbReturnLabelNumber.Text = "";

        tbSubmitToCarrierStatusNote.Text = "";
        rdpPolicySubmitToCarrierDate.SelectedDate = null;

        tbSentToCarrierTrackingNumber.Text = "";
        tbSalesAgentNotes.Text = "";
        tbCaseSpecialistNotes.Text = "";
        tbCaseSpecialistNotes2.Text = "";
        tbCaseSpecialistNotes3.Text = "";
        tbfillFormStatusNote.Text = "";

        ddPolicies.DataSource = Engine.MedsupActions.GetByAccount(AccountID).Select(T => new { T.Key, Policy = T.Key });
        ddPolicies.DataBind();
        // WM - [16.07.2013]
        ddPolicies.Items.Insert(0, new ListItem("-- None --", ""));
    }

    public override bool IsValidated
    {
        get
        {
            //TM [14 06 2014] Make policy optional
            string errorMessage = "Error Required Field(s): ";
            //vldPolicy.Validate();
            //if (!vldPolicy.IsValid)
            //    ctlStatus.SetStatus(new Exception(errorMessage + vldPolicy.ErrorMessage));
            //return vldPolicy.IsValid;
            return true;
        }
    }
    protected override void InnerSave()
    {
        if (IsValidated)
            SaveRecord();
        if (CloseForm)
            ShowGrid();
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        BindEvents();

        if (bFirstTime)
        {
            ClearFields();

            ShowGrid();
        }
    }
    protected override void InnerInit()
    {
        IsGridMode = true;
        RecordId = 0;
    }

    void CommandRouter(string command, long id)
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

    //void SaveForm(bool closeForm = false)
    //{
    //    try
    //    {
    //        //            long accountId = Helper.ConvertToLong(hdnFieldAccountId.Value);
    //        //long individualId = Helper.ConvertToLong(hdnFieldEditIndividual.Value);
    //        //long policyId = Helper.ConvertToLong(hdnFieldPolicyId.Value);

    //        if (hdnFieldIsEditMode.Value == "no")
    //        {
    //            medsupApplication entity = new medsupApplication();

    //            entity = GetInfo(entity);
    //            entity.AddedBy = base.SalesPage.CurrentUser.FullName;

    //            var returnvalue = Engine.MedsupApplicationActions.Add(entity);

    //            hdnFieldIsEditMode.Value = "yes";
    //            hdnFieldPolicyId.Value = returnvalue.Key.ToString();
    //            ddFullName.Enabled = false;
    //        }
    //        else if (hdnFieldIsEditMode.Value == "yes")
    //        {
    //            if (hdnFieldPolicyId.Value != "")
    //            {
    //                var entity = Engine.MedsupApplicationActions.GetAppTrackByKey(Convert.ToInt64(hdnFieldPolicyId.Value));
    //                entity = GetInfo(entity);
    //                entity.ChangedBy = base.SalesPage.CurrentUser.FullName;
    //                Engine.MedsupApplicationActions.Update(entity);
    //            }
    //        }

    //        if (!closeForm)
    //        {
    //            lblMessageForm.Text = Messages.RecordSavedSuccess;
    //            lblMessageGrid.Text = "";
    //            gridAppTrackingInfo.Visible = true;

    //        }
    //        else
    //        {
    //            lblMessageGrid.Text = Messages.RecordSavedSuccess;
    //            lblMessageForm.Text = "";
    //            mainDV.Visible = true;
    //            FormGrid.Visible = false;

    //            BindGrid();
    //        }
    //        IsGridMode = closeForm;
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessageForm.Text = "Error: " + ex.Message;
    //    }
    //}
    //if (e.CommandName == "EditX")
    //{
    //    IsGridMode = false;
    //    ClearFields();

    //    Int64 policyKey = Convert.ToInt64(e.CommandArgument);

    //    hdnFieldIsEditMode.Value = "yes";
    //    hdnFieldPolicyId.Value = policyKey.ToString();
    //    ddFullName.Enabled = false;

    //    mainDV.Visible = false;
    //    FormGrid.Visible = true;

    //    //medsupApplication currentEntity = Engine.MapdpActions.GetByKey(policyKey);
    //    medsupApplication currentEntity = Engine.MedsupApplicationActions.GetAppTrackByKey(policyKey);
    //    //            hdnFieldAccountId.Value = currentEntity.AccountId.ToString();

    //    hdnFieldEditIndividual.Value = currentEntity.IndividualId.ToString();

    //    if (currentEntity.IndividualId > 0)
    //    {
    //        ddFullName.SelectedValue = currentEntity.IndividualId.ToString();
    //    }
    //    else
    //        ddFullName.SelectedIndex = -1;

    //    if (ddExpectedReturnApplicationMethod.Items.Count > 0)
    //    {
    //        if (currentEntity.ExpectedReturnApplicationMethod != null
    //            && currentEntity.ExpectedReturnApplicationMethod.Trim() != "")
    //        {
    //            ddExpectedReturnApplicationMethod.SelectedValue = currentEntity.ExpectedReturnApplicationMethod;
    //        }
    //    }
    //    if (ddApplicationSenttoCustomerMethod.Items.Count > 0)
    //    {
    //        if (currentEntity.ApplicationSentToCustomerMethod != null
    //            && currentEntity.ApplicationSentToCustomerMethod.Trim() != ""
    //            )
    //            ddApplicationSenttoCustomerMethod.SelectedValue = currentEntity.ApplicationSentToCustomerMethod;
    //    }
    //if (ddSpouseExpectedReturnApplicationMethod.Items.Count > 0)
    //{
    //    if (currentEntity.SpouseExpectedReturnApplicationSentMethod.Trim() != "" &&
    //        currentEntity.SpouseExpectedReturnApplicationSentMethod.Trim() != null)
    //    ddSpouseExpectedReturnApplicationMethod.SelectedValue = currentEntity.SpouseExpectedReturnApplicationSentMethod;
    //}
    //if (ddHasApplicationBeenSentToSpouse.Items.Count > 0)
    //{
    //    ddHasApplicationBeenSentToSpouse.SelectedValue = ConvertBoolToString(currentEntity.ApplicationSentToSpouse);
    //}
    //if (ddApplicationSentToSpouseMethod.Items.Count > 0)
    //{
    //    if (currentEntity.ApplicationSentToSpouseMethod.Trim() != "" &&
    //        currentEntity.ApplicationSentToSpouseMethod.Trim() != null)
    //    ddApplicationSentToSpouseMethod.SelectedValue = currentEntity.ApplicationSentToSpouseMethod;
    //}
    //    if (ddFillFormCaseSpecialist.Items.Count > 0)
    //    {
    //        if (currentEntity.CaseSpecialist != null
    //            && currentEntity.CaseSpecialist.Trim() != ""
    //            )
    //            ddFillFormCaseSpecialist.SelectedValue = currentEntity.CaseSpecialist;
    //    }
    //    if (ddSubmittoCarrierCaseSpecialist.Items.Count > 0)
    //    {
    //        if (currentEntity.SubmitToCarrierCaseSpecialist != null
    //            && currentEntity.SubmitToCarrierCaseSpecialist.Trim() != ""
    //            )
    //            ddSubmittoCarrierCaseSpecialist.SelectedValue = currentEntity.SubmitToCarrierCaseSpecialist;
    //    }
    //    if (ddSentToCarrierMethod.Items.Count > 0)
    //    {
    //        if (currentEntity.SentToCarrierMethod != null
    //            && currentEntity.SentToCarrierMethod.Trim() != ""
    //            )
    //            ddSentToCarrierMethod.SelectedValue = currentEntity.SentToCarrierMethod;
    //    }
    //    //if (ddSpouseSentToCarrierMethod.Items.Count > 0)
    //    //{
    //    //    if (currentEntity.SentToCarrierMethod.Trim() != "" &&
    //    //        currentEntity.SentToCarrierMethod.Trim() != null)
    //    //        ddSpouseSentToCarrierMethod.SelectedValue = currentEntity.ApplicationSentToSpouseMethod;
    //    //}
    //    rdpRequestedApplicationSentDate.SelectedDate = currentEntity.RequestedApplicationSentDate;
    //    rdpActualApplicationSentDate.SelectedDate = currentEntity.ActualApplicationSentDate;
    //    tbReturnLabelNumber.Text = currentEntity.ReturnLabelNumber;
    //    //rdpSpouseRequestedApplicationSentDate.SelectedDate = currentEntity.SpouseRequestedApplicationSentDate;
    //    tbSubmitToCarrierStatusNote.Text = currentEntity.SubmitToCarrierStatusNote;
    //    rdpPolicySubmitToCarrierDate.SelectedDate = currentEntity.PolicySubmitToCarrierDate;
    //    //rdpSpousePolicySubmitToCarrierDate.SelectedDate = currentEntity.SpousePolicySubmitToCarrierDate;

    //    //tbTrackingNumber.Text = currentEntity.SentToCarrierTrackingNumber;  -- field not found

    //    tbSentToCarrierTrackingNumber.Text = currentEntity.SentToCarrierTrackingNumber;
    //    tbSalesAgentNotes.Text = currentEntity.SalesAgentNotes;
    //    tbCaseSpecialistNotes.Text = currentEntity.CaseSpecialistNote;
    //    tbCaseSpecialistNotes2.Text = currentEntity.CaseSpecialistNote2;
    //    tbCaseSpecialistNotes3.Text = currentEntity.CaseSpecialistNote3;
    //    tbfillFormStatusNote.Text = currentEntity.StatusNote;

    //}

    //else if (e.CommandName == "DeleteX")
    //{
    //    try
    //    {
    //        Int64 policyKey = Convert.ToInt64(e.CommandArgument);
    //        lblMessageGrid.Text = Messages.RecordDeletedSuccess;
    //        Engine.MedsupApplicationActions.Delete(policyKey);
    //        BindGrid();
    //    }
    //    catch (Exception ex)
    //    {
    //        lblMessageGrid.Text = "Error: " + ex.Message;
    //    }
    //}


    //public void Evt_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    //{

    //    lblMessageGrid.Text = "";
    //    lblMessageForm.Text = "";

    //    if (e.CommandName == "EditX")
    //    {
    //        IsGridMode = false;
    //        ClearFields();

    //        Int64 policyKey = Convert.ToInt64(e.CommandArgument);

    //        hdnFieldIsEditMode.Value = "yes";
    //        hdnFieldPolicyId.Value = policyKey.ToString();
    //        ddFullName.Enabled = false;

    //        mainDV.Visible = false;
    //        FormGrid.Visible = true;

    //        //medsupApplication currentEntity = Engine.MapdpActions.GetByKey(policyKey);
    //        medsupApplication currentEntity = Engine.MedsupApplicationActions.GetAppTrackByKey(policyKey);
    //        //            hdnFieldAccountId.Value = currentEntity.AccountId.ToString();

    //        hdnFieldEditIndividual.Value = currentEntity.IndividualId.ToString();

    //        if (currentEntity.IndividualId > 0)
    //        {
    //            ddFullName.SelectedValue = currentEntity.IndividualId.ToString();
    //        }
    //        else
    //            ddFullName.SelectedIndex = -1;

    //        if (ddExpectedReturnApplicationMethod.Items.Count > 0)
    //        {
    //            if (currentEntity.ExpectedReturnApplicationMethod != null
    //                && currentEntity.ExpectedReturnApplicationMethod.Trim() != "")
    //            {
    //                ddExpectedReturnApplicationMethod.SelectedValue = currentEntity.ExpectedReturnApplicationMethod;
    //            }
    //        }
    //        if (ddApplicationSenttoCustomerMethod.Items.Count > 0)
    //        {
    //            if (currentEntity.ApplicationSentToCustomerMethod != null
    //                && currentEntity.ApplicationSentToCustomerMethod.Trim() != ""
    //                )
    //                ddApplicationSenttoCustomerMethod.SelectedValue = currentEntity.ApplicationSentToCustomerMethod;
    //        }
    //        //if (ddSpouseExpectedReturnApplicationMethod.Items.Count > 0)
    //        //{
    //        //    if (currentEntity.SpouseExpectedReturnApplicationSentMethod.Trim() != "" &&
    //        //        currentEntity.SpouseExpectedReturnApplicationSentMethod.Trim() != null)
    //        //    ddSpouseExpectedReturnApplicationMethod.SelectedValue = currentEntity.SpouseExpectedReturnApplicationSentMethod;
    //        //}
    //        //if (ddHasApplicationBeenSentToSpouse.Items.Count > 0)
    //        //{
    //        //    ddHasApplicationBeenSentToSpouse.SelectedValue = ConvertBoolToString(currentEntity.ApplicationSentToSpouse);
    //        //}
    //        //if (ddApplicationSentToSpouseMethod.Items.Count > 0)
    //        //{
    //        //    if (currentEntity.ApplicationSentToSpouseMethod.Trim() != "" &&
    //        //        currentEntity.ApplicationSentToSpouseMethod.Trim() != null)
    //        //    ddApplicationSentToSpouseMethod.SelectedValue = currentEntity.ApplicationSentToSpouseMethod;
    //        //}
    //        if (ddFillFormCaseSpecialist.Items.Count > 0)
    //        {
    //            if (currentEntity.CaseSpecialist != null
    //                && currentEntity.CaseSpecialist.Trim() != ""
    //                )
    //                ddFillFormCaseSpecialist.SelectedValue = currentEntity.CaseSpecialist;
    //        }
    //        if (ddSubmittoCarrierCaseSpecialist.Items.Count > 0)
    //        {
    //            if (currentEntity.SubmitToCarrierCaseSpecialist != null
    //                && currentEntity.SubmitToCarrierCaseSpecialist.Trim() != ""
    //                )
    //                ddSubmittoCarrierCaseSpecialist.SelectedValue = currentEntity.SubmitToCarrierCaseSpecialist;
    //        }
    //        if (ddSentToCarrierMethod.Items.Count > 0)
    //        {
    //            if (currentEntity.SentToCarrierMethod != null
    //                && currentEntity.SentToCarrierMethod.Trim() != ""
    //                )
    //                ddSentToCarrierMethod.SelectedValue = currentEntity.SentToCarrierMethod;
    //        }
    //        //if (ddSpouseSentToCarrierMethod.Items.Count > 0)
    //        //{
    //        //    if (currentEntity.SentToCarrierMethod.Trim() != "" &&
    //        //        currentEntity.SentToCarrierMethod.Trim() != null)
    //        //        ddSpouseSentToCarrierMethod.SelectedValue = currentEntity.ApplicationSentToSpouseMethod;
    //        //}
    //        rdpRequestedApplicationSentDate.SelectedDate = currentEntity.RequestedApplicationSentDate;
    //        rdpActualApplicationSentDate.SelectedDate = currentEntity.ActualApplicationSentDate;
    //        tbReturnLabelNumber.Text = currentEntity.ReturnLabelNumber;
    //        //rdpSpouseRequestedApplicationSentDate.SelectedDate = currentEntity.SpouseRequestedApplicationSentDate;
    //        tbSubmitToCarrierStatusNote.Text = currentEntity.SubmitToCarrierStatusNote;
    //        rdpPolicySubmitToCarrierDate.SelectedDate = currentEntity.PolicySubmitToCarrierDate;
    //        //rdpSpousePolicySubmitToCarrierDate.SelectedDate = currentEntity.SpousePolicySubmitToCarrierDate;

    //        //tbTrackingNumber.Text = currentEntity.SentToCarrierTrackingNumber;  -- field not found

    //        tbSentToCarrierTrackingNumber.Text = currentEntity.SentToCarrierTrackingNumber;
    //        tbSalesAgentNotes.Text = currentEntity.SalesAgentNotes;
    //        tbCaseSpecialistNotes.Text = currentEntity.CaseSpecialistNote;
    //        tbCaseSpecialistNotes2.Text = currentEntity.CaseSpecialistNote2;
    //        tbCaseSpecialistNotes3.Text = currentEntity.CaseSpecialistNote3;
    //        tbfillFormStatusNote.Text = currentEntity.StatusNote;

    //    }

    //    else if (e.CommandName == "DeleteX")
    //    {
    //        try
    //        {
    //            Int64 policyKey = Convert.ToInt64(e.CommandArgument);
    //            lblMessageGrid.Text = Messages.RecordDeletedSuccess;
    //            Engine.MedsupApplicationActions.Delete(policyKey);
    //            BindGrid();
    //        }
    //        catch (Exception ex)
    //        {
    //            lblMessageGrid.Text = "Error: " + ex.Message;
    //        }
    //    }

    //}

    //public void Save_Click(object sender, EventArgs e)
    //{
    //    SaveForm();
    //}

    //public void CancelOnForm_Click(object sender, EventArgs e)
    //{
    //    ShowGrid();
    //}



    //public void SaveAndClose_Click(object sender, EventArgs e)
    //{
    //    SaveForm(true);
    //}







    //public void Evt_Search_Clicked(object sender, EventArgs e)
    //{
    //    // TODO: Implement this method
    //    throw new NotImplementedException();
    //}

    //public void Evt_SortGrid(object sender, GridSortCommandEventArgs e)
    //{

    //}

    //public void Evt_ItemDataBound(object sender, GridItemEventArgs e)
    //{

    //}

    //public void btnAddNewApplicationTracking_Click(object sender, EventArgs e)
    //{
    //    ClearFields();
    //    lblMessageForm.Text = "";
    //    lblMessageGrid.Text = "";

    //    hdnFieldIsEditMode.Value = "no";
    //    hdnFieldPolicyId.Value = "";
    //    hdnFieldEditIndividual.Value = "";
    //    ddFullName.Enabled = true;

    //    mainDV.Visible = false;
    //    FormGrid.Visible = true;
    //}

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            this.divGrid.Visible = !this.HideGrid;

            if (this.IsOnActionWizard)
            {
                btnReturn.Visible = false;
                btnAddnewApplicationTracking.Visible = false;
                btnCancelOnForm.Visible = false;
                btnSaveAndCloseOnForm.Visible = false;
                btnSaveOnForm.Visible = false;
                ClearFields();
            }
        }

    }

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddnewApplicationTracking.Visible = bEnable;

            var colEdit = gridAppTrackingInfo.Columns.FindByUniqueName("colEdit");
            var colView = gridAppTrackingInfo.Columns.FindByUniqueName("colView");

            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
        {
            EnableControls(tblControls, bEnable);
        }
    }

    public void SetIndividualANDPolicy(Int64 IndividualId, Int64 PolicyNumber)
    {
        ddPolicies.SelectedValue = Convert.ToString(PolicyNumber);
        ddFullName.SelectedValue = Convert.ToString(IndividualId);
    }

    //public void PagingBar_Event(object sender, PagingEventArgs e)
    //{
    //    BindGrid();
    //}

    //private string ConvertBoolToString(bool? inputBool)
    //{
    //    if (inputBool == true)
    //        return "Yes";
    //    else
    //        return "No";
    //}

    //private bool CovertStringToBool(string inputString)
    //{
    //    if (inputString != null)
    //    {
    //        if (inputString == "Yes")
    //            return true;
    //        else
    //            return false;
    //    }
    //    else
    //        return false;
    //}
    //private void LoadIndividuals()
    //{
    //    ddFullName.DataSource = (Page as IIndividual).Individuals;
    //    ddFullName.DataBind();

    //    //try
    //    //{
    //    //    var gettingIndividuals = GetIndividualsByAccount();//Engine.IndividualsActions.GetByAccountID(AccountID);
    //    //    var result = (from x in gettingIndividuals
    //    //                  select new
    //    //                  {
    //    //                      Key = x.Id,
    //    //                      Name = x.LastName + ", " + x.FirstName
    //    //                  });
    //    //    ddFullName.DataSource = result;

    //    //    ddFullName.DataValueField = "Key";
    //    //    ddFullName.DataTextField = "Name";
    //    //    ddFullName.DataBind();
    //    //}
    //    //catch { }
    //}
    //private void loadingCaseSpecialist()
    //{
    //    //var gettingUsers = Engine.UserActions.GetCSR();
    //    //var result = (from x in gettingUsers
    //    //              select new
    //    //              {
    //    //                  Key = x.Key,
    //    //                  FullName = x.FirstName + " " + x.LastName

    //    //              }).AsQueryable();
    //    var users = Engine.UserActions.GetCSR();
    //    ddFillFormCaseSpecialist.DataSource = users;
    //    ddFillFormCaseSpecialist.DataBind();
    //    ddSubmittoCarrierCaseSpecialist.DataSource = users;
    //    ddSubmittoCarrierCaseSpecialist.DataBind();


    //    //ddFillFormCaseSpecialist.DataValueField = "Key";
    //    //ddFillFormCaseSpecialist.DataTextField = "FullName";



    //    //ddSubmittoCarrierCaseSpecialist.DataValueField = "Key";
    //    //ddSubmittoCarrierCaseSpecialist.DataTextField = "FullName";

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
    //private void LoadingPolicies()
    //{
    //    //try
    //    //{
    //    if (AccountID != 0) // SZ [Mar 15, 2013] with this simple check, no exception handling is required for this fucntion.
    //    {
    //        var result = Engine.MedsupActions.GetByAccountID(AccountID)
    //            .Select(T => new { Key = T.Key, Policy = T.Key });
    //        // to string not work in linq
    //        //.Select(T => new  { Key = T.Key, Policy = T.Key.ToString() + T.AddedOn == null ? "" : " - " + T.AddedOn.Value.ToString("dd/mm/yyyy") });

    //        ddPolicies.DataSource = result;
    //        ddPolicies.DataValueField = "Key";
    //        ddPolicies.DataTextField = "Policy";
    //        ddPolicies.DataBind();
    //    }
    //    //}
    //    //catch { }
    //}  
    protected void gridAppTrackingInfo_ItemDataBound(object sender, GridItemEventArgs e)
    {
        DisableDeleteInRadGrid(e.Item, "lnkDelete", "lnkDeleteSeperator");
    }
}
