using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;
using System.Data.Objects;

public partial class Leads_UserControls_carrierIssuesInformation : AccountsBaseControl
{
    public string CarrierRadWindowClientID
    { get { return dlgNewCarrierIssueInformation.ClientID; } }
    
    long RecordId
    {
        get
        {
            long Id = 0;
            long.TryParse(hdnFieldEditTemplateKey.Value, out Id);
            return Id;
        }
        set
        {
            hdnFieldEditTemplateKey.Value = value.ToString();
        }
    }

    internal class issueStatusHistoryDataItem
    {
        public string Status { get; set; }
        public string User { get; set; }
        public string Notes { get; set; }

        public DateTime? Date { get; set; }
    }
    void BindIssueStatusHistoryGrid()
    {
        var history = Engine.IssueStatusHistoryActions.GetAllByCarrierIssueID(RecordId);
        var statuses = Engine.IssueStatusActions.All;

        var source = from h in history
                     join s in statuses on h.IssueStatusID equals s.ID
                     select new issueStatusHistoryDataItem { Status = s.Title, User = h.AddedBy, Notes = h.Comment, Date = h.AddedOn };

        grdStatusesNotes.DataSource = source.ToList();// Engine.IssueStatusHistoryActions.GetAllByCarrierIssueID(EditKey);
        grdStatusesNotes.DataBind();
    }
    void BindIssueStatuses()
    {
        ddlIssueStatuses.DataSource = Engine.IssueStatusActions.All.ToList();
        ddlIssueStatuses.DataBind();

        ddlIssueStatuses.Items.Insert(0, new ListItem("-- None --", "-1"));
    }
    void ClearIssueStatusHistoryRecord()
    {
        ddlIssueStatuses.SelectedIndex = -1;
        txtStatusNotes.Text = "";
    }
    void SaveIssueStatusHistoryRecord(bool createExplicitly)
    {
        lblCarrierIssueError.Text = "";

        int issueStatusId1 = -1;
        int.TryParse(ddlIssueStatuses.SelectedValue, out issueStatusId1);

        int? issueStatusId = null;

        if (issueStatusId1 > 0)
        {
            issueStatusId = issueStatusId1;
        }

        string notes = txtStatusNotes.Text.Trim();

        if (createExplicitly)
        {
            if (RecordId == 0)
            {
                lblCarrierIssueError.Text = "Create Carrier Issue first or select an existing.";

                return;
            }

            if (issueStatusId == null)
            {
                lblCarrierIssueError.Text = "Issue Status Required.";

                return;
            }

            if (notes == "")
            {
                lblCarrierIssueError.Text = "Issue Notes Required.";

                return;
            }
        }
        else
        {
            if ((issueStatusId ?? 0) == 0 && notes != "")
            {
                lblCarrierIssueError.Text = "Select Issue Status or Empty Notes.";

                return;
            }
        }

        if (RecordId > 0 && ((issueStatusId ?? 0) > 0 || notes != ""))
        {
            Engine.IssueStatusHistoryActions.Add(new IssueStatusesHistory { CarrierIssueID = RecordId, IssueStatusID = issueStatusId, Comment = Server.HtmlEncode(notes), AddedBy = this.SalesPage.CurrentUser.FullName });

            ClearIssueStatusHistoryRecord();
        }
    }
    void DeleteIssueStatusHistory()
    {
        Engine.IssueStatusHistoryActions.DeleteByCarrierIssueID(RecordId);
    }
    protected void btnApplyNotesAndStatuses_Click(object sender, EventArgs e)
    {
        SaveIssueStatusHistoryRecord(true);
        //YA[July 22, 2013] 
        if (RecordId > 0)
        {
            var T = Engine.CarrierIssueActions.Get(RecordId);
            T.LastIssueStatus = this.GetLastIssueStatus();
            Engine.CarrierIssueActions.Change(T);
        }
        lbLastIssueStatus.Text = this.GetLastIssueStatus();
        BindIssueStatusHistoryGrid();
    }


    void ShowGrid(bool bShow = true)
    {
        IsGridMode = bShow;
        if (bShow)
        {
            dlgNewCarrierIssueInformation.Dispose();
            dlgNewCarrierIssueInformation.VisibleOnPageLoad = false;
            dlgNewCarrierIssueInformation.Visible = false;
            //divForm.Visible = false;
            divGrid.Visible = true;
            BindGrid();
        }
        else
        {
            dlgNewCarrierIssueInformation.VisibleOnPageLoad = true;
            dlgNewCarrierIssueInformation.Visible = true;
            // divGrid.Visible = false;
            //  divForm.Visible = true;
        }
    }

    void ClearFields()
    {
        ctlStatus.Initialize();
        ctlStatus.SetStatus("");
        lblCarrierIssueError.Text = "";

        RecordId = 0;
        lblCareerId.Text = "";
        tlCarrierIssueDetectDate.Clear();
        tlCarrierIssueDetectDate.SelectedDate = DateTime.Now;

        tlCarrierIssueResolveDate.Clear();
        ////tlResearchCloseDate.Clear();
        ////tlResearchOpenDate.Clear();
        txtCarrierContactFaxNumber.Text = "";
        txtCarrierContactNumber.Text = "";
        txtCarrierContactPerson.Text = "";
        ////txtCarrierIssues.Text = "";
        ////txtCarrierIssueStatusNote.Text = "";
        lbLastIssueStatus.Text = "";
        //txtDetailedIssueNotes.Text = "";
        //txtDetailedIssueNotes2.Text = "";
        //txtDetailedIssueNotes3.Text = "";
        //txtDetailedIssueNotes4.Text = "";
        ////txtResearchRequest.Text = "";
        ////txtResearchResponse.Text = "";
        //txtSearch.Text = "";
        ddCarrierIssueCaseSpecialist.SelectedIndex = 0;
        //IH 26.07.13
        ddlCarrierIssueType.Enabled = true;
        ddlCarrierIssueType.DataSource = Engine.UserActions.GetCarrierIssueTypeList();
        ddlCarrierIssueType.DataBind();
        ddlCarrierIssueType.Items.Insert(0, new ListItem(String.Empty, String.Empty));
        ddlCarrierIssueType.SelectedIndex = 0;


        ////ddOpenResearchRequest.SelectedIndex = 0;
        ////ddResearchCaseSpecialist.SelectedIndex = 0;

    }
    void GetValues(ref CarrierIssue nCarrierIssue)
    {
        nCarrierIssue.DetectDate = tlCarrierIssueDetectDate.SelectedDate;
        nCarrierIssue.ResolveDate = tlCarrierIssueResolveDate.SelectedDate;
        //nCarrierIssue.ResearchCloseDate= tlResearchCloseDate.SelectedDate;
        //nCarrierIssue.ResearchOpenDate= tlResearchOpenDate.SelectedDate;
        nCarrierIssue.ContactFax = txtCarrierContactFaxNumber.Text;
        nCarrierIssue.ContactNumber = txtCarrierContactNumber.Text;
        nCarrierIssue.ContactPerson = txtCarrierContactPerson.Text;
        ////nCarrierIssue.Issues= txtCarrierIssues.Text;
        //nCarrierIssue.StatusNote= txtCarrierIssueStatusNote.Text;
        ////nCarrierIssue.DetailedNote = "";// txtDetailedIssueNotes.Text;
        ////nCarrierIssue.DetailedNote2 = "";//txtDetailedIssueNotes2.Text;
        ////nCarrierIssue.DetailedNote3 = "";//txtDetailedIssueNotes3.Text;
        ////nCarrierIssue.DetailedNote4 = "";//txtDetailedIssueNotes4.Text;
        //nCarrierIssue.ResearchRequest= txtResearchRequest.Text;
        //nCarrierIssue.ResearchResponse =txtResearchResponse.Text;
        nCarrierIssue.CaseSpecialist = ddCarrierIssueCaseSpecialist.SelectedValue;
        nCarrierIssue.LastIssueStatus = GetLastIssueStatus();
        //nCarrierIssue.OpenResearchRequest=ddOpenResearchRequest.SelectedValue ;
        //nCarrierIssue.ResearchCaseSpecialist= ddResearchCaseSpecialist.SelectedValue ;
        //IH 26.07.13
        nCarrierIssue.IssueTypeId = ddlCarrierIssueType.SelectedValue == string.Empty
            ? (int?)null : Convert.ToInt32(ddlCarrierIssueType.SelectedValue);

    }
    void SetValues(CarrierIssue Record)
    {
        RecordId = Record.Key;
        lblCareerId.Text = Record.Key.ToString();
        tlCarrierIssueDetectDate.SelectedDate = Record.AddedOn.HasValue ? Record.AddedOn.Value : DateTime.Now;

        tlCarrierIssueDetectDate.SelectedDate = Record.DetectDate;
        tlCarrierIssueResolveDate.SelectedDate = Record.ResolveDate;
        //tlResearchCloseDate.SelectedDate= nCarrierIssue.ResearchCloseDate;
        //tlResearchOpenDate.SelectedDate= nCarrierIssue.ResearchOpenDate;
        txtCarrierContactFaxNumber.Text = Record.ContactFax;
        txtCarrierContactNumber.Text = Record.ContactNumber;
        txtCarrierContactPerson.Text = Record.ContactPerson;
        //txtCarrierIssues.Text = nCarrierIssue.Issues;
        lbLastIssueStatus.Text = this.GetLastIssueStatus();
        //txtDetailedIssueNotes.Text = nCarrierIssue.DetailedNote;
        //txtDetailedIssueNotes2.Text = nCarrierIssue.DetailedNote2;
        //txtDetailedIssueNotes3.Text = nCarrierIssue.DetailedNote3;
        //txtDetailedIssueNotes4.Text = nCarrierIssue.DetailedNote4;
        //txtResearchRequest.Text = nCarrierIssue.ResearchRequest;
        //txtResearchResponse.Text = nCarrierIssue.ResearchResponse;
        ddCarrierIssueCaseSpecialist.SelectedValue = Record.CaseSpecialist;
        //ddOpenResearchRequest.SelectedValue = nCarrierIssue.OpenResearchRequest;
        //ddResearchCaseSpecialist.SelectedValue = nCarrierIssue.ResearchCaseSpecialist;
        //IH 26.07.13
        ddlCarrierIssueType.SelectedValue = Convert.ToString(Record.IssueTypeId);
        //    ddlCarrierIssueType.Enabled = RecordId != 0;
        var userPermission = ((SalesBasePage)Page).CurrentUser.UserPermissions.FirstOrDefault();
        if (RecordId != 0 && userPermission != null && userPermission.Permissions.Account != null)
            ddlCarrierIssueType.Enabled = userPermission.Permissions.Account.CarrierIssueType;
    }

    void BindGrid(string sortby = "", bool bAscending = true)
    {
        //YA [March 06, 2013] Changed the GetAllByAccountID call to one argument as search textbox is removed from the control page.
        var records = Helper.SortRecords(Engine.CarrierIssueActions.GetAllQueryableByAccountID(AccountID)//(AccountID,txtSearch.Text)
        .Select(x =>
            new
            {
                x.Key,
                CarrierIssueType = x.IssueTypeId != null ? x.issue_types.Name : null,
                ResearchOpenDate = x.ResearchOpenDate != null ? EntityFunctions.TruncateTime(x.ResearchOpenDate.Value) : null,
                ResearchCloseDate = x.ResearchCloseDate != null ? EntityFunctions.TruncateTime(x.ResearchCloseDate.Value) : null,
                OpenResearchRequest = x.OpenResearchRequest, //GetOpenResearchRequestText(x.OpenResearchRequest),
                ResolveDate = x.ResolveDate != null ? EntityFunctions.TruncateTime(x.ResolveDate.Value) : null,
                DetectDate = x.DetectDate != null ? EntityFunctions.TruncateTime(x.DetectDate.Value) : null,
                x.AccountId,
                LastIssueStatus = x.IssueStatusesHistory.OrderByDescending(h => h.ID).FirstOrDefault() == null ? "" : x.IssueStatusesHistory.OrderByDescending(h => h.ID).FirstOrDefault().IssueStatus == null ? "" : x.IssueStatusesHistory.OrderByDescending(h => h.ID).FirstOrDefault().IssueStatus.Title
            }).AsQueryable(), sortby, bAscending);
        gridCarrierIssues.DataSource = ctlPaging.ApplyPaging(records);
        gridCarrierIssues.DataBind();
        RecordId = 0;
    }
    protected string GetOpenResearchRequestText(object requestID)
    {
        if (requestID != null)
        {
            switch (requestID.ToString())
            {
                case "1":
                    return "Yes";
                case "2":
                    return "No";
            }
        }
        return "";
    }

    void BindCaseSpecialists()
    {

        if (ddCarrierIssueCaseSpecialist.Items.Count > 0) ddCarrierIssueCaseSpecialist.Items.Clear();
        ddCarrierIssueCaseSpecialist.AppendDataBoundItems = true;
        ddCarrierIssueCaseSpecialist.Items.Add(new ListItem(String.Empty, String.Empty));
        // ddCarrierIssueCaseSpecialist.Items.Add(new ListItem("--Select Value--", ""));

        //if (ddResearchCaseSpecialist.Items.Count > 0) ddResearchCaseSpecialist.Items.Clear();
        //ddResearchCaseSpecialist.AppendDataBoundItems = true;
        //ddResearchCaseSpecialist.Items.Add(new ListItem("--Select Value--", ""));

        var U = Engine.UserActions.GetCSR();

        ddCarrierIssueCaseSpecialist.DataSource = U;
        ddCarrierIssueCaseSpecialist.DataBind();
        //ddCarrierIssueCaseSpecialist.Items.Insert(0, new ListItem(string.Empty, string.Empty));
        //ddCarrierIssueCaseSpecialist.SelectedIndex = 0;

        //ddResearchCaseSpecialist.DataSource = U;
        //ddResearchCaseSpecialist.DataBind();



        ddCarrierIssueCaseSpecialist.AppendDataBoundItems = false;


        //ddResearchCaseSpecialist.AppendDataBoundItems = false;
    }
    string GetLastIssueStatus()
    {
        var entity = Engine.IssueStatusHistoryActions.GetLastIssueStatusByCarrierIssueID(RecordId);

        if (entity == null || entity.IssueStatus == null)
        {
            return "";
        }

        return entity.IssueStatus.Title;
    }

    protected override void InnerInit()
    {
        if (!Page.IsPostBack)
        {
            //WM - 30.05.2013
            BindIssueStatuses();

            IsGridMode = true;
            Close(); // SetPageMode(PageDisplayMode.GridQueueTemplate);
            BindCaseSpecialists();
            ctlStatus.SetStatus("");
            ctlStatus.SetStatus("");
        }

    }
    protected override void InnerLoad(bool bFirstTime)
    {
        btnAddnewCarrierIssues.Click += (o, a) => AddRecord();
        btnReturn.Click += (o, a) => Close();
        btnCancelOnForm.Click += (o, a) => Close();

        btnApply.Click += (o, a) => SaveRecord();
        btnSubmit.Click += (o, a) => { if (IsValidated) SaveRecord(true); };

        if (bFirstTime)
        {
            BindGrid();
        }

        gridCarrierIssues.ItemCommand += (o, a) => CommandRouter(Helper.SafeConvert<long>(a.CommandArgument.ToString()), a.CommandName);
        gridCarrierIssues.SortCommand += (o, a) => BindGrid(a.SortExpression, a.NewSortOrder == GridSortOrder.Ascending);

        ctlPaging.IndexChanged += (o, a) => BindGrid();
        ctlPaging.SizeChanged += (o, a) => BindGrid();
    }
    protected override void InnerPostBack()
    {
        ctlStatus.SetStatus("");
        ctlStatus.SetStatus("");
    }
    public override bool IsValidated
    {
        get
        {
            //vldtxtCarrierIssues.Validate();
            //return vldtxtCarrierIssues.IsValid;

            return true;
        }
    }
    protected override void InnerSave()
    {
        SaveRecord(CloseForm);
    }
    protected override void InnerEnableControls(bool bEnable)
    {
        if (IsGridMode)
        {
            btnAddnewCarrierIssues.Visible = bEnable;
            var colEdit = gridCarrierIssues.Columns.FindByUniqueName("colEdit");
            var colView = gridCarrierIssues.Columns.FindByUniqueName("colView");

            colEdit.Visible = bEnable;
            colView.Visible = !bEnable;
        }
        else
        {
            EnableControls(tblControls, bEnable);
        }
    }

    void AddRecord()
    {
        ClearFields();
        ClearIssueStatusHistoryRecord();
        BindIssueStatusHistoryGrid();
        ShowGrid(false); // SetPageMode(PageDisplayMode.EditQueueTemplate);
    }
    void EditRecord(long Id)
    {
        ClearFields();
        var Record = Engine.CarrierIssueActions.Get(Id);
        //RecordId = Record.Key;
        if (Record != null)
        {
            SetValues(Record);
            ShowGrid(false);
        }
        else
            ctlStatus.SetStatus(ErrorMessages.RecordDoesNotExist);
    }

    void SaveRecord(bool bClose = false)
    {
        CarrierIssue Record = (RecordId == 0) ? new CarrierIssue { AccountId = AccountID, AddedBy = SalesPage.CurrentUser.FullName, AddedOn = DateTime.Now, IsDeleted = false } :
            Engine.CarrierIssueActions.Get(RecordId);

        GetValues(ref Record);

        try
        {
            if (RecordId <= 0)
            {
                Engine.CarrierIssueActions.Add(Record);

                lblCareerId.Text = Record.Key.ToString();
                RecordId = Record.Key;
            }
            else
            {
                Record.ChangedBy = SalesPage.CurrentUser.FullName;
                Record.ChangedOn = DateTime.Now;
                Record.IsDeleted = false;
                Engine.CarrierIssueActions.Change(Record);
            }

            SaveIssueStatusHistoryRecord(false);
            // ddlCarrierIssueType.Enabled = ddlCarrierIssueType.SelectedValue == string.Empty;
           //SR ctlStatus.SetStatus(Messages.RecordSavedSuccess);
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
        if (bClose)
        {
            ShowGrid();
        }
        else
        {
            lbLastIssueStatus.Text = this.GetLastIssueStatus();
            BindIssueStatusHistoryGrid();
        }

    }
    void DeleteRecord()
    {

        Engine.CarrierIssueActions.Delete(RecordId);
        //SR ctlStatus.SetStatus(Messages.RecordDeletedSuccess);
        BindGrid();
    }
    void Close()
    {
        ShowGrid(); //SetPageMode(PageDisplayMode.GridQueueTemplate);
    }

    void CommandRouter(long id, string command)
    {
        switch (command)
        {
            case "EditX":
                EditRecord(id);
                ClearIssueStatusHistoryRecord();
                BindIssueStatusHistoryGrid();
                //ShowGrid(false);// SetPageMode(PageDisplayMode.EditQueueTemplate);
                //SetValues(RecordId);
                break;
            case "DeleteX":
                RecordId = id;
                DeleteIssueStatusHistory();
                DeleteRecord();
                break;
            case "ViewX":
                ClearIssueStatusHistoryRecord();
                BindIssueStatusHistoryGrid();
                EditRecord(id);
                ReadOnly = true;

                //ShowGrid(); // SetPageMode(PageDisplayMode.EditQueueTemplate);
                //SetValues(RecordId);
                break;
        }
    }



    //bool SaveRecord(bool ConvertToEditMode = false)
    //{
    //    try
    //    {
    //        if (RecordId == 0)
    //        {
    //            CarrierIssue nCarrierIssue = new CarrierIssue();
    //            GetValues(ref nCarrierIssue);
    //            var recordAdded = Engine.CarrierIssueActions.Add(nCarrierIssue);
    //            lblCareerId.Text = recordAdded.Key.ToString();

    //            //if (ConvertToEditMode)
    //            {
    //                RecordId = recordAdded.Key;
    //            }
    //        }
    //        else
    //        {
    //            CarrierIssue nCarrierIssue = Engine.CarrierIssueActions.Get(RecordId);
    //            GetValues(ref nCarrierIssue);
    //            Engine.CarrierIssueActions.Change(nCarrierIssue);
    //        }

    //        SaveIssueStatusHistoryRecord(false);
    //        lbLastIssueStatus.Text = this.GetLastIssueStatus();
    //        BindIssueStatusHistoryGrid();


    //        ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    //        return true;
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex);
    //        return false;
    //    }
    //}
    //void InitializeDetail()
    //{
    //    ClearFields();
    //    //BindCarrierGrid();        
    //}
    //enum PageDisplayMode { GridQueueTemplate = 1, EditQueueTemplate = 2 }
    //protected void btnSubmit_Click(object sender, EventArgs e)
    //{
    //    try
    //    {
    //        bool hasSavedRecordSuccessful = SaveRecord();
    //        if (hasSavedRecordSuccessful)
    //        {
    //            Close(); // SetPageMode(PageDisplayMode.GridQueueTemplate);
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        ctlStatus.SetStatus(ex);
    //    }
    //}
    //public void PagingBar_Event(object sender, PagingEventArgs e)
    //{
    //    BindCarrierGrid();
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
    //protected void gridCarrierIssues_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    //{
    //    long id = 0;
    //    long.TryParse(e.CommandArgument.ToString(), out id);
    //    EditKey = id;
    //    if (e.CommandName == "EditX")
    //    {
    //        ClearIssueStatusHistoryRecord();
    //        BindIssueStatusHistoryGrid();
    //        ShowGrid(false);// SetPageMode(PageDisplayMode.EditQueueTemplate);
    //        LoadEditFormValues(EditKey);
    //    }
    //    else if (e.CommandName == "DeleteX")
    //    {
    //        DeleteIssueStatusHistory();
    //        DeleteRecord();
    //    }
    //    else if (e.CommandName == "ViewX")
    //    {
    //        ClearIssueStatusHistoryRecord();
    //        BindIssueStatusHistoryGrid();
    //        ShowGrid(); // SetPageMode(PageDisplayMode.EditQueueTemplate);
    //        LoadEditFormValues(EditKey);
    //        ReadOnly = true;
    //    }

    //}

    //protected void gridCarrierIssues_SortCommand(object sender, GridSortCommandEventArgs e)
    //{
    //    BindCarrierGrid(e.SortExpression, e.NewSortOrder == GridSortOrder.Ascending);
    //}
    //protected void btnSearchIssue_Click(object sender, EventArgs e)
    //{
    //    Close(); // SetPageMode(PageDisplayMode.GridQueueTemplate);
    //}
    //protected void txtSearch_TextChanged(object sender, EventArgs e)
    //{
    //    Close(); // SetPageMode(PageDisplayMode.GridQueueTemplate);
    //}
    //private void SetPageMode(PageDisplayMode mode)
    //{
    //    divForm.Visible = false;
    //    divGrid.Visible = false;
    //    IsGridMode = mode == PageDisplayMode.GridQueueTemplate;

    //    switch (mode)
    //    {
    //        case PageDisplayMode.GridQueueTemplate:
    //            divGrid.Visible = true;
    //            BindCarrierGrid();
    //            break;
    //        case PageDisplayMode.EditQueueTemplate:
    //            divForm.Visible = true;
    //            InitializeDetail();
    //            break;
    //        default:
    //            break;
    //    }
    //}

    protected void gridCarrierIssues_ItemDataBound(object sender, GridItemEventArgs e)
    {
        DisableDeleteInRadGrid(e.Item, "lnkDelete", "lnkDeleteSeperator");
    }
}