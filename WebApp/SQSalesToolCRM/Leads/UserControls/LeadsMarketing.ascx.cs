using System;

using System.Linq;

using System.Web.UI;
using System.Web.UI.WebControls;

using SalesTool.DataAccess.Models;

public partial class Leads_UserControls_LeadsMarketing : AccountsBaseControl
{
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

    //#endregion
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
    //private void SaveForm(bool closeForm = false)
    //{
    //    try
    //    {
    //        if (hdnFieldIsEditMode.Value == "no")
    //        {
    //            var entity = new Lead();

    //            MaptoEntity(entity, ViewMode.AddNew);

    //            Engine.LeadsActions.Add(entity);

    //            hdnFieldEditForm.Value = entity.Key.ToString();
    //            hdnFieldIsEditMode.Value = "yes";
    //        }

    //        else if (hdnFieldIsEditMode.Value == "yes")
    //        {
    //            if (hdnFieldEditForm.Value != "")
    //            {
    //                var entity = Engine.LeadsActions.Get(Convert.ToInt64(hdnFieldEditForm.Value));

    //                MaptoEntity(entity, ViewMode.Edit);

    //                Engine.LeadsActions.Update(entity);
    //            }
    //        }

    //        if (!closeForm)
    //        {
    //            ctlStatus.SetStatus(Messages.RecordSavedSuccess); 
    //        }
    //        else
    //        {
    //            ChangeView();

    //            ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex);  //lblMessageForm.Text = "Error: " + ex.Message;
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
    //// must be public to access in the parent form
    //public void CancelOnForm_Click(object sender, EventArgs e)
    //{
    //    GridView();
    //}

    //private void GridView()
    //{
    //    ChangeView();
    //}

    //void Edit(long )
    //{
    //    ChangeView(ViewMode.Edit);
    //}
    //protected void Page_Load(object sender, EventArgs e)
    //{
    //    SiteMasterPage masterPage = base.SalesPage.Master as SiteMasterPage;
    //    if (masterPage != null)
    //    {
    //        masterPage.buttonYes.Click += new EventHandler(CancelOnForm_Click);
    //    }

    //    if (!Page.IsPostBack)
    //    {
    //        BindGrid();
    //        divGrid.Visible = true;
    //        divForm.Visible = false;
    //    }
    //}
    //private bool ControlIsValid()
    //{
    //    return vldSourceCode.IsValid;
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
    //{
    //    try
    //    {
    //        Engine.LeadsActions.Delete(Convert.ToInt64(hdnFieldEditForm.Value));
    //        BindGrid();

    //        divGrid.Visible = true;
    //        divForm.Visible = false;

    //        lblMessageGrid.Text = Messages.RecordDeletedSuccess;
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex); // lblMessageForm.Text = "Error: " + ex.Message;
    //    }
    //}
    //private enum ViewMode { GridView = 0, AddNew = 1, Edit = 2 }

    public string LeadsMarketingRadWindowClientID
    { get { return dlgLeadsMarketing.ClientID; } }
    void ClearFields()
    {
        RecordId = 0;

        //Clear the fields
        lbStatus.Text = string.Empty;
        lbSubStatus.Text = string.Empty;
        lbLastAction.Text = string.Empty;
        lbLastActionDate.Text = string.Empty;
        lbCampaign.Text = string.Empty;

        txtPublisherSubId.Text = string.Empty;
        txtSourceCode.Text = string.Empty;
        txtEmailTrackingCode.Text = string.Empty;

        txtPublisherId.Text = string.Empty;
        txtAdVariation.Text = string.Empty;
        txtIPAddress.Text = string.Empty;
        radFirstContactDateTime.SelectedDate = null;

        txtCompany.Text = string.Empty;
        txtGroup.Text = string.Empty;

        txtTrackingCode.Text = string.Empty;
        txtTrackingInfo.Text = string.Empty;

        ctlStatus.Clear();
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

    void CommandRouter(string command, long id)
    {
        switch (command)
        {
            case "EditX":
                EditRecord(id);
                break;
            case "ViewX":
                EditRecord(id);
                ReadOnly = true;
                break;
            case "DeleteX":
                DeleteRecord(id);
                break;
        }
        //if (e.CommandName == "EditX")
        //{
        //    ChangeView(ViewMode.Edit);
        //    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
        //    string dataKeyValue = grdHome.DataKeys[row.RowIndex].Value.ToString();
        //    hdnFieldEditForm.Value = dataKeyValue;
        //    Lead entity = Engine.LeadsActions.Get(Helper.SafeConvert<long>(dataKeyValue));
        //    MaptoForm(entity);
        //}
        /*[QN, 12-04-2013] Mantis Item # 074: Delete button is to be removed from 
         Leads and Marketing grid, instead of removing the column i am comment its code, 
         just in case it is required again in future
         */
        //else if (e.CommandName == "DeleteX")
        //{
        //    GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
        //    string dataKeyValue = grdHome.DataKeys[row.RowIndex].Value.ToString();
        //    lblMessageForm.Text = "";
        //    lblMessageGrid.Text = "";
        //    Engine.LeadsActions.Delete(Helper.ConvertToLong(dataKeyValue));
        //    lblMessageGrid.Text = Messages.RecordDeletedSuccess;
        //    BindgrdHome();
        //}
    }

    void DeleteRecord(long id)
    {
        try
        {
            // TODO: Implement this method
            BindGrid();
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    public void EditRecord(long id)
    {
        ShowGrid(false);
        ClearFields();

        try
        {
            RecordId = id;
            var L = Engine.LeadsActions.Get(RecordId);
            //YA[13 March 2014]
            long accountIDLeadMarketng = L.AccountId;
            //if (ApplicationSettings.IsTermLife && ApplicationSettings.HasLeadNewLayout && AccountID != accountIDLeadMarketng)
            if (Engine.ApplicationSettings.IsTermLife && Engine.ApplicationSettings.HasLeadNewLayout && AccountID != accountIDLeadMarketng)
            {
                Session[Konstants.K_LEAD_LEADMARKETING_EDIT_MODE] = "true";
                Session[Konstants.K_LEAD_LEADMARKETING_EDIT_KEY] = id.ToString();
                Response.Redirect(Konstants.K_LEADS_PAGE + "?AccountId=" + accountIDLeadMarketng.ToString() + "&" + Konstants.K_AVOID_REASSIGNMENT + "=true");
            }
            else
            {
                Session[Konstants.K_LEAD_LEADMARKETING_EDIT_MODE] = "false";
                Session[Konstants.K_LEAD_LEADMARKETING_EDIT_KEY] = id.ToString();
                SetDetails(L);
            }
        }
        catch (Exception ex)
        {
            ShowGrid();
            ctlStatus.SetStatus(ex);
        }
    }
    void AddRecord()
    {
        ClearFields();
        ShowGrid(false);
    }
    void SaveRecord()
    {
        if (IsValidated)
        {
            Lead L = null;
            if (RecordId > 0)
            {
                L = Engine.LeadsActions.Get(RecordId);
                L.ChangedBy = (Page as SalesBasePage).CurrentUser.FullName;
                L.ChangedOn = DateTime.Now;
            }
            else
                L = new Lead { AddedBy = (Page as SalesBasePage).CurrentUser.FullName, AddedOn = DateTime.Now };

            GetDetails(ref L);

            try
            {
                if (RecordId > 0)
                    Engine.LeadsActions.Update(L);
                else
                    RecordId = Engine.LeadsActions.Add(L).Key;

                // ctlStatus.SetStatus(Messages.RecordSavedSuccess);
            }
            catch (Exception ex)
            {
                ctlStatus.SetStatus(ex);
            }
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
    //protected void Evt_PageIndexChanging(object sender, GridViewPageEventArgs e)
    //{
    //    try
    //    {
    //        grdHome.PageIndex = e.NewPageIndex;
    //        BindGrid();
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex); // lblMessageGrid.Text = "Error: " + ex.Message;
    //    }
    //}


    public void ShowGrid(bool bShow = true)
    {
        // IsGridMode = bShow;
        //divGrid.Visible = bShow;
        dlgLeadsMarketing.VisibleOnPageLoad = !bShow;
        dlgLeadsMarketing.Visible = !bShow;

        if (bShow)
        {
            dlgLeadsMarketing.Dispose();
            dlgLeadsMarketing.VisibleOnPageLoad = false;
            dlgLeadsMarketing.Visible = false;
            RecordId = 0;
            BindGrid();
        }
    }
    void BindGrid()
    {
        try
        {
            //var account = Engine.AccountActions.Get(this.AccountID);
            //var leads = Engine.AccountActions.GetLeads(AccountID)
            //    .Select(x => new
            //    {
            //        LeadsId = x.Key,
            //        ActionId = x.ActionId,
            //        CampaignId = x.CampaignId,
            //        StatusId = x.StatusId,

            //        TimeCreated = x.TimeCreated,
            //        TrackingCode = x.TrackingCode,
            //        SourceCode = x.SourceCode

            //    }).AsEnumerable();

            //var actions = Engine.LocalActions.All
            //  .Select(x => new
            //  {
            //      Id = x.Id,
            //      Action = x.Title

            //  });

            //var campaigns = Engine.ManageCampaignActions.GetAll()
            //   .Select(x => new
            //   {
            //       Id = x.ID,
            //       Campaign = x.Title

            //   });

            //var statuses = Engine.StatusActions.All
            // .Select(x => new
            // {
            //     Id = x.Id,
            //     Status = x.Title

            // });

            ////var substatuses = Engine.SubStatusActions.All
            ////.Select(x => new
            ////{
            ////    Id = x.Id,
            ////    SubStatus = x.Title

            ////});//.AsQueryable();

            // left join
            //var records = (from l in leads
            //               //                           from a in actions.Where(a => a.Id == l.ActionId).DefaultIfEmpty()
            //               from c in campaigns.Where(c => c.Id == l.CampaignId).DefaultIfEmpty()
            //               from s in statuses.Where(s => s.Id == l.StatusId).DefaultIfEmpty()
            //               select new
            //               {
            //                   Id = l.LeadsId,
            //                   Campaign = l.CampaignId == null ? "" : c == null ? "" : c.Campaign,
            //                   Status = l.StatusId == null ? "" : s == null ? "" : s.Status,
            //                   //Status = l.StatusId == null ? "" : s.Status,
            //                   SubStatus = "",//ss.SubStatus,
            //                   DateCreated = l.TimeCreated,
            //                   TrackingCode = l.TrackingCode,
            //                   SourceCode = l.SourceCode,
            //               }).AsQueryable();
            bool isMultiAccountsAllowed = Engine.AccountActions.IsMultipleAccountsAllowed();
            IQueryable<ViewLeadMarketing> records;
            if (isMultiAccountsAllowed)
            {
                // Getting accounts records 
                var ids = Engine.AccountActions.GetAssociatedAccountsIds(AccountID);
                records = Engine.LeadsActions.LeadMarketing.Where(p => ids.Contains(p.AccountId));
            }
            else
            {
                records = Engine.LeadsActions.LeadMarketing.Where(x => x.AccountId == AccountID);
            }
            grdHome.DataSource = PagingNavigationBar.ApplyPaging(Helper.SortRecords(records, PagingNavigationBar.SortBy, PagingNavigationBar.SortAscending));

            grdHome.DataBind();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);  //lblMessageGrid.Text = "Error: " + ex.Message;
        }
    }

    void BindEvents()
    {
        //var M = base.SalesPage.Master as SiteMasterPage;
        //if (M != null)
        //    M.buttonYes.Click += (o, a) => ShowGrid(); // new EventHandler(CancelOnForm_Click);
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
        btnReturn.Click += (o, a) => ShowGrid();
        btnCancelOnForm.Click += (o, a) => ShowGrid();

        //btnAddNew.Click += (o, a) => AddRecord();
        btnSaveOnForm.Click += (o, a) =>
        {
            if (IsValidated)
                SaveRecord();
        };

        btnSaveAndCloseOnForm.Click += (o, a) => { if (IsValidated) { SaveRecord(); ShowGrid(); } };

        grdHome.RowCommand += (o, a) => CommandRouter(a.CommandName, Helper.SafeConvert<long>(
            grdHome.DataKeys[((a.CommandSource as Control).NamingContainer as GridViewRow).RowIndex].Value.ToString()
            ));

        grdHome.Sorting += (o, a) => Sort(a.SortExpression);

        PagingNavigationBar.IndexChanged += (o, a) => BindGrid();
        PagingNavigationBar.SizeChanged += (o, a) => BindGrid();

    }
    protected override void InnerInit()
    {
        IsGridMode = true;
    }
    protected override void InnerLoad(bool bFirstTime)
    {
        BindEvents();
        if (bFirstTime)
        {
            ShowGrid();
        }
    }
    protected override void InnerSave()
    {
        SaveRecord();
        if (CloseForm)
            ShowGrid();
    }



    public override bool IsValidated
    {
        get
        {
            //vldSourceCode.Validate();
            return true;//vldSourceCode.IsValid;
        }
    }

    void SetDetails(Lead entity)
    {
        //lbStatus.Text = "";
        //lbSubStatus.Text = "";
        //lbLastAction.Text = "";
        //lbLastActionDate.Text = "";

        // WM [11 July,2013] - It is a date last action applied on. This is stored in the lead
        if (entity != null && entity.LastActionDate.HasValue)
        {
            lbLastActionDate.Text = entity.LastActionDate.Value.ToShortDateString();
        }

        // SZ [may 29, 2013] The reason for this approach was that, strictly speaking, foreign keys are a physical implementation 
        //detail and don't belong in a conceptual data model. However, as a practical matter, 
        //it's often easier to work with entities in code when you have direct access to the foreign keys.
        if (entity.ActionId != null)
        {
            var action = Engine.LocalActions.All.FirstOrDefault(x => x.Id == entity.ActionId);
            if (action != null)
            {
                lbLastAction.Text = action.Title;
                // WM [11 July,2013] - LastActionDate is not action created date
                // It is a date last action applied on. This is stored in the lead
                //if (action.Added.On.HasValue)
                //    lbLastActionDate.Text = action.Added.On.Value.ToShortDateString();
            }
        }

        if (entity.CampaignId != null)
        {
            var campaign = Engine.ManageCampaignActions.Get(entity.CampaignId ?? 0);
            if (campaign != null)
                lbCampaign.Text = campaign.Title;
        }

        if (entity.StatusId != null)
        {
            var status = Engine.StatusActions.Get(entity.StatusId ?? 0);
            if (status != null)
                lbStatus.Text = status.Title;
        }

        if (entity.SubStatusId != null)
        {
            var substatus = Engine.StatusActions.Get(entity.SubStatusId ?? 0);
            if (substatus != null)
            {
                // SZ [may 29, 2013] WTF? a string is converted to a string and stored in a string??? 
                //lbSubStatus.Text = Helper.ConvertToString(substatus.Title);
                lbSubStatus.Text = substatus.Title;
            }
        }

        txtPublisherSubId.Text = entity.PubSubId;
        txtSourceCode.Text = entity.SourceCode;
        txtEmailTrackingCode.Text = entity.EmailTrackingCode;

        txtPublisherId.Text = entity.PublisherID;
        txtAdVariation.Text = entity.AdVariation;
        txtIPAddress.Text = entity.IPAddress;
        radFirstContactDateTime.SelectedDate = entity.FirstContactAppointment;
        txtCompany.Text = entity.Company;
        txtGroup.Text = entity.Group;

        txtTrackingCode.Text = entity.TrackingCode;
        txtTrackingInfo.Text = entity.TrackingInformation;
    }
    void GetDetails(ref Lead entity)
    {
        //if (viewMode == ViewMode.AddNew)
        //{
        //    entity.AccountId = AccountID;
        //    //entity.IndividualId = Helper.ConvertToNullLong(ddlIndividual.SelectedValue);
        //    entity.TimeCreated = DateTime.Now.ToShortDateString();//.ToShortTimeString();
        //    entity.AddedBy = (Page as SalesBasePage).CurrentUser.FullName;
        //}
        //else
        //{
        //    entity.ChangedBy = (Page as SalesBasePage).CurrentUser.FullName;// Last Modified User
        //}

        //entity.StatusId = Helper.NullConvert<int>(txtStatus.Text);
        //entity.SubStatusId = Helper.NullConvert<int>(ddlSubStatus.SelectedValue);
        //entity.ActionId = Helper.NullConvert<int>(ddlLastAction.SelectedValue);
        //entity.LastActionDate = lblLastActionDate. rdpLastActionDate.SelectedDate;

        entity.PubSubId = txtPublisherSubId.Text;
        entity.SourceCode = txtSourceCode.Text;
        entity.EmailTrackingCode = txtEmailTrackingCode.Text;

        entity.PublisherID = txtPublisherId.Text;
        entity.AdVariation = txtAdVariation.Text;
        entity.IPAddress = txtIPAddress.Text;
        entity.FirstContactAppointment = radFirstContactDateTime.DateInput.SelectedDate;
        entity.Company = txtCompany.Text;
        entity.Group = txtGroup.Text;

        entity.TrackingCode = txtTrackingCode.Text;
        entity.TrackingInformation = txtTrackingInfo.Text;
    }

    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
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











}