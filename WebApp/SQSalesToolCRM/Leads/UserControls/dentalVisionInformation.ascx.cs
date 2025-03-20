using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess.Models;
using Telerik.Web.UI;

public partial class Leads_UserControls_dentalVisionInformation : AccountsBaseControl, IIndividualNotification, IWrittingAgentSet
{
    #region Members/Properties

    //public String DefaultPolicyStatus = string.Empty;

    // SZ [Jan 23, 2013] base class already implements it, not required anymore
    //public long AccountID
    //{
    //    get
    //    {
    //        return base.SalesPage.AccountID;
    //    }
    //}
    public string DentalVisionRadWindowClientID
    { get { return dlgDentalVision.ClientID; } }
    public bool IsAutoPostBackPolicyStatus = true;

    //YA[28 May 2014] 
    public event EventHandler FreshPolicyStatus;
    protected void OnFreshPolicyStatus(EventArgs e)
    {
        if(FreshPolicyStatus != null)
        {
            FreshPolicyStatus(this, e);
        }
    }

    #endregion

    #region methods
    //SZ [Aug 28, 2013] implements the interfcae for setting the writting agent
    public void SetAgent(Guid agentId)
    {
        if (ddlWritingAgent.Items.FindByValue(agentId.ToString()) != null)
            ddlWritingAgent.SelectedValue = agentId.ToString();
    }

    public void SetIndividual(string selectedValue)
    {
        if (ddlIndividual.Items.FindByValue(selectedValue) != null)
        {
            ddlIndividual.SelectedValue = selectedValue;
        }
    }

    public void IndividualChanged(IIndividual handle)
    {
        if (!IsGridMode)
            BindIndividuals();
    }

    protected void BindIndividuals()
    {
        ddlIndividual.DataSource = ((IIndividual)Page).Individuals; //GetIndividualsByAccount().Select(T => new { ID = T.Id, FullName = T.FirstName + " " + T.LastName });
        ddlIndividual.DataValueField = "ID";
        ddlIndividual.DataTextField = "FullName";
        ddlIndividual.DataBind();
        ddlIndividual.Items.Insert(0, new ListItem(string.Empty, "-1"));
        ddlIndividual.SelectedIndex = 0;
    }
    //SR
    public void BindCarrier()
    {
        ddlCarrier.DataSource = Engine.CarrierActions.GetDentalVision();
        ddlCarrier.DataBind();
        ddlCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
        ddlCarrier.SelectedIndex = 0;
    }

    protected void ClearFields()
    {
        BindIndividuals();
        ddlWritingAgent.SelectedIndex = 0;

        rdpSubmitDate.SelectedDate = null;
        rdpEffectiveDate.SelectedDate = null;
        txtAnnualPremium.Text = string.Empty;
        txtPolicyNumber.Text = string.Empty;
        txtCompanyName.Text = string.Empty;
        if (ddlPolicyStatus.Items.Count > 0)
            ddlPolicyStatus.SelectedIndex = 0;

        lblMessageForm.Text = string.Empty;
        lblMessageGrid.Text = string.Empty;

        ddlCarrier.DataSource = Engine.CarrierActions.GetDentalVision();
        ddlCarrier.DataBind();

        rdpIssueDate.SelectedDate = null;
        rdpLapseDate.SelectedDate = null;
        ddlCarrier.Items.Insert(0, new ListItem(String.Empty, string.Empty));
        ddlCarrier.SelectedIndex = 0;

    }

    private void Get_values(long id)
    {
        DentalVision entity = Engine.DentalVisionActions.Get(id);
        if (ddlIndividual.Items.Count == 0)
            BindIndividuals();

        ddlIndividual.SelectedValue = entity.IndividualId.ToString();

        rdpSubmitDate.SelectedDate = entity.SubmissionDate;
        rdpEffectiveDate.SelectedDate = entity.EffectiveDate;
        txtAnnualPremium.Text = entity.AnnualPremium.HasValue ? entity.AnnualPremium.ToString() : "";
        txtPolicyNumber.Text = entity.PolicyNumber;
        // ddlWritingAgent.SelectedValue = (entity.Enroller == null) ? (ddlWritingAgent.SelectedValue = "-1") : entity.Enroller.ToString();
        //IH 10.10.13
        ddlWritingAgent.SelectedValue = (entity.Enroller == null) ? SalesPage.CurrentUser.Key.ToString() : entity.Enroller.ToString();
        txtCompanyName.Text = entity.CompanyName;

        //WM - [31.07.2013]
        if (entity.PolicyStatus.HasValue)
        {
            ddlPolicyStatus.SelectedValue = entity.PolicyStatus.Value.ToString();
        }
        else
        {
            ddlPolicyStatus.SelectedIndex = 0;
        }

        //Mehross - [14.03.14]
        if (entity.Carriers != null && ddlCarrier.Items.FindByValue(entity.Carriers.Key.ToString()) != null)
            ddlCarrier.SelectedValue = entity.Carriers.Key.ToString();
    }

    public void grdHome_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        if (e.CommandName == "EditX")
        {
            lblMessageForm.Text = "";
            ChangeView(ViewMode.Edit);

            long id = Helper.SafeConvert<long>(grdHome.DataKeys[((GridViewRow)((Control)e.CommandSource).NamingContainer).RowIndex].Value.ToString());
            hdnFieldEditForm.Value = id.ToString();
            Get_values(id);


        }
        else if (e.CommandName == "DeleteX")
        {
            GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
            string dataKeyValue = grdHome.DataKeys[row.RowIndex].Value.ToString();

            lblMessageForm.Text = "";
            lblMessageGrid.Text = "";

            Engine.DentalVisionActions.Delete(Helper.SafeConvert<long>(dataKeyValue));
            //lblMessageGrid.Text = Messages.RecordDeletedSuccess;
            BindgrdHome();
        }
        else if (e.CommandName == "ViewX")
        {
            lblMessageForm.Text = "";
            ChangeView(ViewMode.Edit);

            long id = Helper.SafeConvert<long>(grdHome.DataKeys[((GridViewRow)((Control)e.CommandSource).NamingContainer).RowIndex].Value.ToString());
            hdnFieldEditForm.Value = id.ToString();
            Get_values(id);
            ReadOnly = true;


        }
    }

    public void grdHome_Sorting(object sender, GridViewSortEventArgs e)
    {
        //try
        //{
        if (PagingNavigationBar.SortBy == e.SortExpression)
        {
            PagingNavigationBar.SortAscending = !PagingNavigationBar.SortAscending;
        }
        else
        {
            PagingNavigationBar.SortBy = e.SortExpression;
            PagingNavigationBar.SortAscending = true;
        }

        BindgrdHome();
        // }
        //catch (Exception ex)
        //{
        //    lblMessageGrid.Text = "Error: " + ex.Message;
        //}

    }

    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        int size = e.PageSize;
        size = size > 100 ? 100 : size;
        grdHome.PageSize = size;
        BindgrdHome();
    }

    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        grdHome.PageIndex = e.PageNumber;
        BindgrdHome();
    }

    public void grdHome_PageIndexChanging(object sender, GridViewPageEventArgs e)
    {
        grdHome.PageIndex = e.NewPageIndex;
        BindgrdHome();

    }

    protected override void InnerInit()
    {
        IsGridMode = true;

        Refresh();
    }

    public void BindgrdHome()
    {
        try
        {
            var records = Engine.DentalVisionActions.GetAllByAccountID(this.AccountID).Select(T =>
                new
            {
                ID = T.Key,
                //PersonAttached = T.Individual.FirstName + " " + T.Individual.LastName,
                SubmitDate = T.SubmissionDate,
                PolicyNumber = T.PolicyNumber,
                AnnualPremium = T.AnnualPremium,
                EffectiveDate = T.EffectiveDate,
                CarrierName = T.Carriers.Name,
                PolicyStatus = T.PolicyStatus1 == null ? "" : T.PolicyStatus1.Name,
            }).AsQueryable();

            grdHome.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(records, PagingNavigationBar.SortBy, PagingNavigationBar.SortAscending)).ToList();

            grdHome.DataBind();

        }
        catch (Exception ex)
        {
            lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }

    private string getSortDirectionString(SortDirection sortDirection)
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

    protected override void InnerLoad(bool bFirstTime)
    {
        if (Page is IIndividual)
            (Page as IIndividual).Notify(this);

        if (!bFirstTime)
        {
            ////WM - don't know how somewhere lblMessageForm.Visible set to false
            //lblMessageForm.Visible = true;
            BindgrdHome();
            divGrid.Visible = true;
            //SR divForm.Visible = false;
            ShowHideModal(false);
            // BindIndividuals();
        }
    }

    #endregion

    protected void Page_Load(object sender, EventArgs e)
    {
        dynamic d = Page.Master;
        if (d != null)
        {
            dynamic yesButton = d.buttonYes;
            if (yesButton != null)
            {
                var button = yesButton as Button;
                if (button != null) button.Click += new EventHandler(CancelOnForm_Click);
            }
        }

        //SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
        //if (masterPage != null)
        //{
        //    masterPage.buttonYes.Click += new EventHandler(CancelOnForm_Click);
        //}

        //if (!Page.IsPostBack)
        //{
        //    BindgrdHome();
        //    divGrid.Visible = true;
        //    divForm.Visible = false;

        //    BindIndividuals();
        //}

        this.lblMessageForm.Text = "";
        this.lblMessageGrid.Text = "";

        if (!Page.IsPostBack)
        {
            // this.divGrid.Visible = !this.HideGrid;

            if (this.IsOnActionWizard)
            {
                btnReturn.Visible = false;
                btnAddNew.Visible = false;
                btnCancelOnForm.Visible = false;
                btnSaveAndCloseOnForm.Visible = false;
                btnSaveOnForm.Visible = false;
                BindIndividuals();
            }

            ddlWritingAgent.DataSource = Engine.UserActions.GetWritingAgents();
            ddlWritingAgent.DataBind();
            //ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
            ddlWritingAgent.Items.Insert(0, new ListItem("", "-1"));
            ddlWritingAgent.SelectedIndex = 0;
            // ddlWritingAgent.SelectedValue = this.SalesPage.CurrentUser.Key.ToString();
            /*
                 * Policy status type is as following at the moment
                 autohome = 1
                 Medicare Supplement	= 2
                 MAPDP = 3
                 Dental and Vision = 4
                 */
            ddlPolicyStatus.DataSource = Engine.PolicyStatusActions.GetAll(/*Policy status type = */4);
            ddlPolicyStatus.DataBind();
            //IH 23.07.13
            ddlPolicyStatus.Items.Insert(0, new ListItem(""));
            BindCarrier();

            //if (ddlCarrier.Items.FindByValue(Engine.de  CarrierId.ToString()) != null)
            //    ddlCarrier.SelectedValue = Engine.CarrierId.ToString();

        }


    }
    //IH 16.09.13--get or set policy status when apply action is submitted online/sent to customer/submit to carrier
    public void SetPolicyStatus(string selectedStatus)
    {

        if (ddlPolicyStatus != null && ddlPolicyStatus.Items.Count > 0)
            ddlPolicyStatus.SelectedIndex = ddlPolicyStatus.Items.IndexOf(ddlPolicyStatus.Items.FindByText(selectedStatus));
    }
    

    //IH 16.09.13--get or set policy status when apply action is submitted online/sent to customer/submit to carrier
    public void SetWritingAgent(string selectedUser)
    {

        if (ddlWritingAgent != null && ddlWritingAgent.Items.Count > 0)
            ddlWritingAgent.SelectedIndex = ddlWritingAgent.Items.IndexOf(ddlWritingAgent.Items.FindByValue(selectedUser));
    }
    protected void btnDelete_Click(object sender, EventArgs e)
    {
        try
        {
            Engine.DentalVisionActions.Delete(Convert.ToInt64(hdnFieldEditForm.Value));
            BindgrdHome();
            
            divGrid.Visible = true;
            //SR divForm.Visible = false;
            ShowHideModal(false);

           //SR 27.3.2014 lblMessageGrid.Text = Messages.RecordDeletedSuccess;
        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }
    }




    public void SaveForm(bool closeForm = false)
    {
        try
        {
            if (hdnFieldIsEditMode.Value == "no")
            {
                //if (this.AccountID == 0)
                //{
                //    Account account = new Account();

                //    Engine.AccountActions.Add(account);

                //    this.AccountID = account.Key;
                //}

                var entity = new DentalVision
                {
                    AccountId = this.AccountID,
                    IndividualId = Helper.SafeConvert<long>(ddlIndividual.SelectedValue),

                    SubmissionDate = rdpSubmitDate.SelectedDate,
                    EffectiveDate = rdpEffectiveDate.SelectedDate,
                    AnnualPremium = Helper.SafeConvert<decimal>(txtAnnualPremium.Text),
                    PolicyNumber = txtPolicyNumber.Text,//Helper.SafeConvert<long>(txtPolicyNumber.Text),
                    
                    //WM - [31.07.2013]
                    PolicyStatus = string.IsNullOrWhiteSpace(ddlPolicyStatus.SelectedValue) ? null : Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue),
                    CompanyName = txtCompanyName.Text,
                    Enroller = ddlWritingAgent.SelectedValue != "-1" ? new Guid(ddlWritingAgent.SelectedValue) : (Guid?)null,
                    Carriers = Engine.CarrierActions.Get(Helper.SafeConvert<Int64>(ddlCarrier.SelectedValue)),
                    LapseDate = rdpLapseDate.SelectedDate,
                    IssueDate = rdpIssueDate.SelectedDate,
                    //IsActive = true,
                    //IsDeleted = false,
                    //AddedOn = DateTime.Now,
                    //  AddedBy = null //CurrentUser.Key;//Logged In User Id
                    //ChangedOn = set in edit
                    //ChangedBy = set in edit
                };                
                Engine.DentalVisionActions.Add(entity);

                hdnFieldEditForm.Value = entity.Key.ToString();
                hdnFieldIsEditMode.Value = "yes";
                ddlIndividual.Enabled = false;

                BindgrdHome();
            }

            else if (hdnFieldIsEditMode.Value == "yes")
            {
                if (hdnFieldEditForm.Value != "")
                {
                    var entity = Engine.DentalVisionActions.Get(Convert.ToInt64(hdnFieldEditForm.Value));

                    entity.SubmissionDate = rdpSubmitDate.SelectedDate;
                    entity.EffectiveDate = rdpEffectiveDate.SelectedDate;
                    entity.AnnualPremium = Helper.SafeConvert<decimal>(txtAnnualPremium.Text);
                    entity.PolicyNumber = txtPolicyNumber.Text;//Helper.SafeConvert<long>(txtPolicyNumber.Text);
                    entity.CompanyName = txtCompanyName.Text;
                    if (Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue) != entity.PolicyStatus)
                    {
                        Engine.AccountHistory.PolicyStatusChanged(AccountID, ddlPolicyStatus.SelectedItem.Text, "Dental & Vision", base.SalesPage.CurrentUser.Key, 0 , Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue).Value);
                        OnFreshPolicyStatus(EventArgs.Empty);
                    }
                    //WM - [31.07.2013]
                    entity.PolicyStatus = string.IsNullOrWhiteSpace(ddlPolicyStatus.SelectedValue) ? null : Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue);

                    entity.Enroller = ddlWritingAgent.SelectedValue != "-1"
                                          ? new Guid(ddlWritingAgent.SelectedValue)
                                          : (Guid?)null;
                    entity.Carriers = Engine.CarrierActions.Get(Convert.ToInt64(ddlCarrier.SelectedValue));
                    entity.LapseDate = rdpLapseDate.SelectedDate;
                    entity.IssueDate = rdpIssueDate.SelectedDate;

                    Engine.DentalVisionActions.Change(entity);
                }
            }

            if (!closeForm)
            {
                lblMessageForm.Text = Messages.RecordSavedSuccess;
                lblMessageGrid.Text = "";
                ChangeView(ViewMode.Edit);
            }
            else
            {
                ChangeView();

                lblMessageForm.Text = "";
               //SR 27.3.2014  lblMessageGrid.Text = Messages.RecordSavedSuccess;
            }
        }
        catch (Exception ex)
        {
            lblMessageForm.Text = "Error: " + ex.Message;
        }
    }

    protected void Save_Click(object sender, EventArgs e)
    {
        SaveForm();
    }

    protected void SaveAndClose_Click(object sender, EventArgs e)
    {
        SaveForm(true);
    }

    protected void AddNew_Click(object sender, EventArgs e)
    {
        AddNewView();
    }
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        GridView();
        lblMessageForm.Text = "";
        lblMessageGrid.Text = "";
    }
    // must be public to access in the parent form
    public void CancelOnForm_Click(object sender, EventArgs e)
    {
        GridView();
        lblMessageForm.Text = string.Empty;
        lblMessageGrid.Text = string.Empty;
    }

    private void GridView()
    {
        ChangeView();
    }
    private void AddNewView()
    {
        ChangeView(ViewMode.AddNew);
    }

    private void EditView()
    {
        ChangeView(ViewMode.Edit);
    }

    private enum ViewMode { GridView = 0, AddNew = 1, Edit = 2 }

    private void ChangeView(ViewMode viewMode = ViewMode.GridView)
    {
        //SR divGrid.Visible = viewMode == ViewMode.GridView;
        //SR divForm.Visible = viewMode != ViewMode.GridView;
        ShowHideModal(viewMode != ViewMode.GridView);
        //SR IsGridMode = viewMode == ViewMode.GridView;

        if (viewMode == 0)
        {
            BindgrdHome();

            return;
        }

        ddlIndividual.Enabled = viewMode == ViewMode.AddNew;

        if (viewMode == ViewMode.AddNew)
        {
            hdnFieldIsEditMode.Value = "no";
            hdnFieldEditForm.Value = string.Empty;
            ClearFields();
        }
        else
        {
            hdnFieldIsEditMode.Value = "yes";
        }


    }

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
    public override bool IsValidated
    {
        get
        {
            vldSubmitDate.Validate();
            return vldSubmitDate.IsValid;
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

    protected override void InnerSave()
    {
        SaveForm(CloseForm);
    }
    public bool AddNewDentalVision(bool clearFields, Int64 accountId, ref long key, bool checkRequiredFields = false)
    {
        bool saveFlagError = false;
        var entity = new DentalVision
        {
            AccountId = accountId,
            IndividualId = Helper.SafeConvert<long>(ddlIndividual.SelectedValue),

            SubmissionDate = rdpSubmitDate.SelectedDate,
            EffectiveDate = rdpEffectiveDate.SelectedDate,
            AnnualPremium = Helper.SafeConvert<decimal>(txtAnnualPremium.Text),
            PolicyNumber = txtPolicyNumber.Text,
            CompanyName = txtCompanyName.Text,
            PolicyStatus = string.IsNullOrWhiteSpace(ddlPolicyStatus.SelectedValue) ? null : Helper.NullConvert<long>(ddlPolicyStatus.SelectedValue),

            /*Start Add by Mehross 14/03/14 */
            Carriers = Engine.CarrierActions.Get(Helper.SafeConvert<Int32>(ddlCarrier.SelectedValue)),
            LapseDate = rdpLapseDate.SelectedDate,
            IssueDate = rdpIssueDate.SelectedDate,
            /*End Add by Mehross 14/03/14 */

            Enroller = ddlWritingAgent.SelectedValue != "-1" ? new Guid(ddlWritingAgent.SelectedValue) : (Guid?)null

            //AddedBy = null
        };
        if (checkRequiredFields)
        {
            saveFlagError = CheckRequiredFields(entity);

            //WM - if saveFlagError don't do anything else
            if (saveFlagError)
            {
                //SR this.divGrid.Visible = false;
                //SR this.divForm.Visible = true;
                ShowHideModal(true);
                return true;
            }

            //if (!saveFlagError)
            //{
            Engine.DentalVisionActions.Add(entity);
            //}
        }
        else
            Engine.DentalVisionActions.Add(entity);
        //SR  this.divGrid.Visible = false;

        if (clearFields)
        {
            ClearFields();
            //SR this.divForm.Visible = true;
            ShowHideModal(true);
        }
        key = entity.Key;
        return saveFlagError;
    }
    /// <summary>
    /// Check the required fields setup at Sub Status II
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    private bool CheckRequiredFields(DentalVision entity)
    {
        //YA[July 15, 2013] Duplicate record implementation.        
        bool hasErrors = false;
        string errorMessage = "";
        RequiredFieldChecker nduplicate = new RequiredFieldChecker();
        nduplicate.RequiredFieldsChecking(entity, ref hasErrors, ref errorMessage, "Dental & Vision", this.AccountID);
        lblMessageForm.Text = errorMessage;

        return hasErrors;
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

    public void Action_AddRecord()
    {
        divButtons.Visible = false;
        divGrid.Visible = false;
        ShowHideModal(true);
    }
    //SR
    private void ShowHideModal(bool value)
    {
        if (value && IsAutoPostBackPolicyStatus)
        {
            dlgDentalVision.DestroyOnClose = true;
            dlgDentalVision.VisibleOnPageLoad = true;
            dlgDentalVision.Visible = true;
            dlgDentalVision.Modal = true;
            dlgDentalVision.RenderMode = RenderMode.Classic;
            dlgDentalVision.VisibleTitlebar = true;
            dlgDentalVision.Height = 350;
        }
        else if (!IsAutoPostBackPolicyStatus)
        {
            dlgDentalVision.DestroyOnClose = false;
            divGrid.Visible = false;
            dlgDentalVision.VisibleOnPageLoad = true;
            dlgDentalVision.Visible = true;

            dlgDentalVision.Modal = false;
            dlgDentalVision.RenderMode = RenderMode.Lightweight;
            dlgDentalVision.VisibleTitlebar = false;
            dlgDentalVision.BorderStyle = BorderStyle.None;
            dlgDentalVision.Height = 300;
            dlgDentalVision.BorderWidth = 0;
            dlgDentalVision.CssClass = "borderLessDialog";
            dlgDentalVision.DestroyOnClose = false;
            dlgDentalVision.RestrictionZoneID = "restrictionZone";
            dlgDentalVision.Width = 960;
            dlgDentalVision.Left = 0;
            divFormFS.Style.Remove("margin");
        }
        else
        {
            dlgDentalVision.DestroyOnClose = false;
            divGrid.Visible = true;
            dlgDentalVision.VisibleOnPageLoad = false;
            dlgDentalVision.Visible = false;
        }
    }
    protected void grdHome_RowDataBound(object sender, GridViewRowEventArgs e)
    {
        DisableDeleteInGridView(e.Row, "lnkDelete", "lnkDeleteSeperator");
    }
}
