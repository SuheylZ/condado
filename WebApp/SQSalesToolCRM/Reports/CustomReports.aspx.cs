using System;
using System.Collections.Generic;
using System.Linq;

using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;

using SalesTool.DataAccess.Models;
using S = System.Web.Security.Membership;
using UserControls;
using Telerik.Web.UI;
using System.Linq.Dynamic;
using System.Data;
using System.Text.RegularExpressions;

public partial class Reports_CustomReports : SalesBasePage
{
    #region Members/Properties
    /// <summary>
    /// Page display modes in different scenarios
    /// </summary>
    public enum PageDisplayMode
    {
        GridDisplay = 1, 
        FormStep1Forward = 2,
        FormStep1Backward = 3,
        FormStep2Forward = 4,
        FormStep2Backward = 5,
        FormStep3Forward = 6,
        ReportUsers = 7,
        EmailEditor=8
    }
    /// <summary>
    /// Report ID used in the whole form
    /// </summary>
    int ReportId
    {
        get
        {
            int lAns = 0;
            int.TryParse(hdnFieldReportID.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldReportID.Value = value.ToString();
        }
    }
    /// <summary>
    /// Check the page state is it in grid showing mode
    /// </summary>
    bool IsGridMode
    {
        get
        {
            bool lAns = true;
            bool.TryParse(hdnFieldIsGridMode.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldIsGridMode.Value = value.ToString();
        }
    }
    /// <summary>
    /// Flag for checking the temporary save
    /// </summary>
    bool IsTemporarySave
    {
        get
        {
            bool lAns = false;
            bool.TryParse(hdnFieldIsTemporarySave.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldIsTemporarySave.Value = value.ToString();
        }
    }
    /// <summary>
    /// Base data value
    /// </summary>
    int BaseData
    {
        get
        {
            int lAns = 0;
            int.TryParse(hdnFieldBaseData.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldBaseData.Value = value.ToString();
        }
    }
    /// <summary>
    /// Form view selected base data id
    /// </summary>
    int FormBaseDataId
    {
        get
        {
            int lAns = 0;
            int.TryParse(ddlBaseData.SelectedValue, out lAns);
            return lAns;
        }        
    }
    /// <summary>
    /// Grid view selected base data id.
    /// </summary>
    int GridBaseDataId
    {
        get
        {
            int lAns = 0;
            int.TryParse(ddlBaseDataGrid.SelectedValue, out lAns);
            return lAns;
        }
    }
    /// <summary>
    /// Is the user going for new record
    /// </summary>
    bool IsNewRecord
    {
        get
        {
            return ReportId < 1;
        }

    }
    #endregion

    #region Methods
    /// <summary>
    /// Called with in the page_initialize for initial state values loading
    /// </summary>
    /// <param name="bFirstTime">Is the first time loading</param>
    protected void InnerLoad(bool bFirstTime)
    {
        if (bFirstTime)
        {
            //Set the filter control initial values
            ManageFiltersControl.ParentType = FilterParentType.CustomReport;
            ManageFiltersControl.AddedBy = CurrentUser.Email;
            ManageFiltersControl.ChangedBy = CurrentUser.Email;
            FillBaseDataDropdowns();
            ClearFormInfo();            
            SetPageMode(PageDisplayMode.GridDisplay);
        }
    }
    /// <summary>
    /// Bind the base data dropdowns
    /// </summary>
    private void FillBaseDataDropdowns()
    {
        var T = Engine.BaseQueryDataActions.GetAllByInsuranceType((short)ApplicationSettings.InsuranceType);
        if (ddlBaseData.Items.Count > 0)
            ddlBaseData.Items.Clear();
        ddlBaseData.DataTextField = "Title";
        ddlBaseData.DataValueField = "Id";
        ddlBaseData.DataSource = T;
        ddlBaseData.DataBind();

        if (ddlBaseDataGrid.Items.Count > 0)
            ddlBaseDataGrid.Items.Clear();
        ddlBaseDataGrid.AppendDataBoundItems = true;
        ddlBaseDataGrid.DataTextField = "Title";
        ddlBaseDataGrid.DataValueField = "Id";
        ddlBaseDataGrid.Items.Add(new ListItem("--All Base Data--", "-1"));
        ddlBaseDataGrid.DataSource = T;
        ddlBaseDataGrid.DataBind();
        ddlBaseDataGrid.AppendDataBoundItems = false;
    }
    /// <summary>
    /// Initialize column selection
    /// </summary>
    private void InitializeColumnSelection()
    {
        //Initialize and bind column names to the control
        ctlColumnSelection.Initialize();
        BindColumnsName();
    }
    /// <summary>
    /// Binds the data to the reports grid
    /// </summary>
    /// <param name="HasSerchText">Flag to identify the records search</param>
    void BindGrid(bool HasSearchText = false)
    {
        try
        {
            var records = Engine.CustomReportsAction.GetAll();
            if (HasSearchText) records = records.Where(x => x.ReportTitle.Contains(txtSerachReport.Text));
            if (BaseData > 0) records = records.Where(x => x.BaseData == BaseData);            
            var recordSelection = records.Select(x => new { x.ReportID, x.ReportTitle, Owner = x.Added.By, x.BaseData }).AsQueryable();
            grdReports.DataSource = ctlPager.ApplyPaging(Helper.SortRecords(recordSelection, ctlPager.SortBy, ctlPager.SortAscending));
            grdReports.DataBind();
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    /// <summary>
    /// Get the basedata string according to the value specified in param
    /// </summary>
    /// <param name="nBaseData">Base data value stored in DB</param>
    /// <returns></returns>
    public string GetBaseDataString(object pBaseData)
    {
        short nBaseData = 0;
        short.TryParse(pBaseData.ToString(), out nBaseData);
        string result = String.Empty;
        switch (nBaseData)
        {
            case 0:
                result = "";
                break;
            case 1:
                result = "Accounts";
                break;
            case 2:
                result = "Account History";
                break;
            case 3:
                result = "Lead History";
                break;
            case 4:
                result = "Carrier Issue";
                break;
            case 5:
                result = "Policy";
                break;
            case 6:
                result = "Quote";
                break;
            case 7:
                result = "Medicare Supplement";
                break;
            case 8:
                result = "MAPDP";
                break;
            case 9:
                result = "Dental & Vision";
                break;
            default:
                break;
        }
        return result;
    }
    /// <summary>
    /// When new record work flow initiated
    /// </summary>
    void AddNewRecord()
    {
        IsTemporarySave = false;
        lblStep1Dynamic.Text = "[New Report]";
        ManageFiltersControl.Parent_key = -1;
        if (BaseData > 0) ddlBaseData.SelectedValue = ddlBaseDataGrid.SelectedValue;
        else ddlBaseData.SelectedIndex = 0;
        ClearFormInfo();
        SetPageMode(PageDisplayMode.FormStep1Forward);
    }
    /// <summary>
    /// Binds the columns for selection for the custom reports
    /// </summary>
    private void BindColumnsName()
    {
        if (ctlColumnSelection.AvailableItems.Count > 0)
            ctlColumnSelection.AvailableItems.Clear();
        ListItem firstItem = new ListItem("***Select Column***", "-1");
        firstItem.Enabled = false;
        ctlColumnSelection.AvailableItems.Add(firstItem);
        var separators = Engine.TagFieldsActions.GetAll().Select(m => new { m.Group }).Distinct();
        //Add items with group wise separation in the Item collection of control
        foreach (var titleGroup in separators)
        {
            ListItem nlistitem = new ListItem("---" + titleGroup.Group.ToString() + "---", "-1");
            //nlistitem.Enabled = false;            
            nlistitem.Attributes.Add("style","background-color:green;");
            ctlColumnSelection.AvailableItems.Add(nlistitem);
            IQueryable<TagFields> U = null;           
            //Get all columns according to selected base data
            U = Engine.TagFieldsActions.GetAllReportTagsByBaseDataID(FormBaseDataId);
            if (U == null) continue;
            var groupColumns = U.Select(k => new { Key = k.Id, Name = k.Name, k.Group, k.FilterDataType, k.IsSpecialSubqueryField }).Where(m => m.Group == titleGroup.Group && m.IsSpecialSubqueryField != true).OrderBy(m => m.Name).ToList();
            foreach (var item in groupColumns)
            {
                ctlColumnSelection.AvailableItems.Add(new ListItem(item.Name, item.Key.ToString()));
            }
        }

    }
    /// <summary>
    /// Clears form's controls
    /// </summary>
    void ClearFormInfo()
    {
        ReportId = -1;
        ctlStatus.Clear();
        txtSerachReport.Text = "";
        txtReportTitle.Text = "";
        txtCustomFilter.Text = "";
        rdBtnlstFilterSelection.SelectedIndex = 0;
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
        InitializeColumnSelection();
    }
    /// <summary>
    /// Copy report records
    /// </summary>
    private void CopyRecords()
    {
        IsTemporarySave = true;
        int oldReportID = ReportId;
        Report oReport = Engine.CustomReportsAction.Get(oldReportID);
        Report nReport = new Report();
        nReport.BaseData = oReport.BaseData;
        nReport.ReportTitle = "Copy of " + oReport.ReportTitle;
        nReport.FilterSelection = oReport.FilterSelection;
        nReport.FilterCustomValue = oReport.FilterCustomValue;
        nReport.Added.By = CurrentUser.FullName;
        ReportId = Engine.CustomReportsAction.Add(nReport, IsTemporarySave);
        //Copy all report columns
        Engine.ReportColumnsAction.CopyAll(oldReportID, ReportId);
        //Copy all filters
        Engine.FilterAreaActions.CopyAll(oldReportID, ReportId, (short)FilterParentType.CustomReport, CurrentUser.FullName);
    }    
    /// <summary>
    /// Load report users
    /// </summary>
    private void EditReportUsers()
    {
        Report nReport = Engine.CustomReportsAction.Get(ReportId);
        lblStep1Dynamic.Text = nReport.ReportTitle;
        ctlSelection.Initialize();
        foreach (var U in Engine.CustomReportsAction.UsersNotInReport(ReportId))
            ctlSelection.AvailableItems.Add(new ListItem(U.FullName ,U.Key.ToString()));
        foreach (var U in Engine.CustomReportsAction.UsersInReport(ReportId))
            ctlSelection.SelectedItems.Add(new ListItem(U.FullName, U.Key.ToString()));
        ctlSelection.TitleAvailable = "Users Not Assigned to " + nReport.ReportTitle;
        SetPageMode(PageDisplayMode.ReportUsers);
    }
    /// <summary>
    /// Load report email editor
    /// </summary>
    private void LoadEmailEditor()
    {        
        Report nReport = Engine.CustomReportsAction.Get(ReportId);
        lblStep1Dynamic.Text = nReport.ReportTitle;
        //Initialize email editor control
        EmailEditorCustomReport.Clear();
        EmailEditorCustomReport.AddUsers(Engine.UserActions.GetAll());        
        EmailEditorCustomReport.EmailInformation = Engine.EmailActions.GetByReport(ReportId);                
        SetPageMode(PageDisplayMode.EmailEditor);        
    }
    /// <summary>
    /// On edit of report load data 
    /// </summary>
    private void EditReport()
    {
        IsTemporarySave = false;
        Report nReport = Engine.CustomReportsAction.Get(ReportId);
        //Set Setp 1 values
        lblStep1Dynamic.Text = txtReportTitle.Text = nReport.ReportTitle;
        if (nReport.BaseData.HasValue)
            ddlBaseData.SelectedValue = nReport.BaseData.Value.ToString();

        //Set step 2 values
        InitializeColumnSelection();
        IQueryable<ReportColumns> T = Engine.ReportColumnsAction.GetAllByReportID(ReportId);
        foreach (var U in T)
        {
            var tagColumn = Engine.TagFieldsActions.GetAll().Select(k => new { Key = k.Id, Name = k.Name, k.Group }).Where(m => m.Key == U.Tagkey).FirstOrDefault();
            if (U.HasAggregateFunction == true)
                ctlColumnSelection.SelectedItems.Add(new ListItem(ctlColumnSelection.GetGroupFunctionText((Konstants.AggregateFunctionType)U.AggregateFunctionType.Value, tagColumn.Name), tagColumn.Key.ToString()));
            else
                ctlColumnSelection.SelectedItems.Add(new ListItem(tagColumn.Name, tagColumn.Key.ToString()));
        }
        //Set Step 3 Values
        rdBtnlstFilterSelection.SelectedValue = nReport.FilterSelection == null ? "0" : nReport.FilterSelection.ToString();
        if (nReport.FilterSelection == 2)
        {
            txtCustomFilter.Text = nReport.FilterCustomValue;
        }
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
        ManageFiltersControl.Parent_key= ReportId;
        
        ManageFiltersControl.BindEmailFilterGrid();
        
        SetPageMode(PageDisplayMode.FormStep1Forward);
    }
    /// <summary>
    /// Delete report and its associated data.
    /// </summary>
    private void DeleteReport()
    {
        //Delete Report Columns
        Engine.ReportColumnsAction.DeleteByReportID(ReportId);
        //Delete Report Filters
        Engine.FilterAreaActions.DeleteAll(ReportId, (short)FilterParentType.CustomReport);
        //Delete Report 
        Engine.CustomReportsAction.Delete(ReportId);
        BindGrid();
    }
    /// <summary>
    /// Save report record
    /// </summary>
    private void SaveReport()
    {
        if (IsDuplicateTitle(ReportId))
        {
            ctlStatus.SetStatus(ErrorMessages.DuplicateReportTitle);
            return;
        }
        if (IsNewRecord)
        {
            Report nReport = new Report();
            if (SetReportData(nReport)) return;
            nReport.Added.By = CurrentUser.FullName;
            ReportId = Engine.CustomReportsAction.Add(nReport, IsTemporarySave);
            InsertReportColumns();
        }
        else
        {
            Report nReport = Engine.CustomReportsAction.Get(ReportId);
            //YA[May 15, 2013] Added as temporary save functionality is implemented and on temporary save IsDeleted is true, 
            //Temporary save is implemented as filters creation could not be done unless saved report ID is there.
            nReport.IsDeleted = false;
            if (SetReportData(nReport)) return;
            nReport.Changed.By = CurrentUser.FullName;
            Engine.CustomReportsAction.Change(nReport);
            InsertReportColumns();
        }
        ctlStatus.SetStatus(Messages.RecordSavedSuccess);
    }
    /// <summary>
    /// Save report users
    /// </summary>
    private void SaveReportUsers()
    {
        List<Guid> userskey = new List<Guid>();
        foreach (ListItem item in ctlSelection.SelectedItems)
        {
            Guid key = Guid.Empty;
            Guid.TryParse(item.Value, out key);
            userskey.Add(key);
        }
        Engine.CustomReportsAction.AddUserForReport(ReportId, userskey);
        ctlStatus.SetStatus(Messages.RecordSavedSuccess);
        SetPageMode(PageDisplayMode.GridDisplay);
    }
    /// <summary>
    /// Save report email data
    /// </summary>
    private void SaveEmailData()
    {
        var X = EmailEditorCustomReport.EmailInformation;        
        if (X.Recipients.Count > 0)
        {
            var T = Engine.EmailActions.EmailsByReport(ReportId);
            if (T != null)
            {
                var U = T.ToList();
                foreach (var item in U)
                {
                    Engine.EmailActions.Delete(item.Id, ReportId);    
                }
            }
            Engine.EmailActions.Add(X.Subject, X.Message,X.FilterByRole,(byte)X.Format, (byte)X.SendingFrequency, ReportId, CurrentUser.FullName, X.UserKeys);
            ctlStatus.SetStatus(Messages.RecordSavedSuccess);
            SetPageMode(PageDisplayMode.GridDisplay);
        }
        else
            ctlStatus.SetStatus(ErrorMessages.SelectEmailRecipients);        
    }
    /// <summary>
    /// Insert report column in DB.
    /// </summary>
    private void InsertReportColumns()
    {
        //Delete Report Columns        
        Engine.ReportColumnsAction.DeleteByReportID(ReportId);
        int columnOrder = 1;
        foreach (ListItem item in ctlColumnSelection.SelectedItems)
        {
            int key = 0;
            int.TryParse(item.Value, out key);
            ReportColumns nReportColumn = new ReportColumns();
            nReportColumn.ReportID = ReportId;
            nReportColumn.Tagkey = key;
            nReportColumn.ColumnOrder = columnOrder;
            string pattern = @"\(.*?\)";
            string orgString = item.Text;
            var matches = Regex.Matches(item.Text, pattern);
            if (matches.Count > 0)
            {
                nReportColumn.HasAggregateFunction = true;
                nReportColumn.AggregateFunctionType = GetAggregateType(item.Text);
            }
            Engine.ReportColumnsAction.Add(nReportColumn);
            columnOrder++;
        }
    }
    /// <summary>
    /// Get the aggregate type
    /// </summary>
    /// <param name="inputString"></param>
    /// <returns></returns>
    private short GetAggregateType(string inputString)
    {
        short result = 0;
        if (inputString.StartsWith("Count"))
        {
            result = (short)Konstants.AggregateFunctionType.Count;
        }
        else if (inputString.StartsWith("Sum"))
        {
            result = (short)Konstants.AggregateFunctionType.Sum;
        }
        else if (inputString.StartsWith("Min"))
        {
            result = (short)Konstants.AggregateFunctionType.Min;
        }
        else if (inputString.StartsWith("Max"))
        {
            result = (short)Konstants.AggregateFunctionType.Max;
        }
        else if (inputString.StartsWith("Avg"))
        {
            result = (short)Konstants.AggregateFunctionType.Average;
        }
        return result;
    }
    /// <summary>
    /// Check for duplicate report title
    /// </summary>
    /// <param name="editID"></param>
    /// <returns></returns>
    private bool IsDuplicateTitle(int editID = 0)
    {
        return Engine.CustomReportsAction.IsDuplicateTitle(txtReportTitle.Text, editID);
    }
    /// <summary>
    /// Set report entity fields data for insertion
    /// </summary>
    /// <param name="nReport"></param>
    /// <returns></returns>
    private bool SetReportData(Report nReport)
    {
        bool valueReturn = false;
        nReport.BaseData = (short)FormBaseDataId;
        nReport.ReportTitle = txtReportTitle.Text;
        short nFilterSelection = 0;
        short.TryParse(rdBtnlstFilterSelection.SelectedValue, out nFilterSelection);
        nReport.FilterSelection = nFilterSelection;
        bool hasCustomFilterError = false;
        if (nFilterSelection == 2) //Custom Filter value
        {
            hasCustomFilterError = !CheckForValidCustomString();
            nReport.FilterCustomValue = txtCustomFilter.Text;
        }
        if (hasCustomFilterError) valueReturn = true;
        return valueReturn;
    }
    /// <summary>
    /// Set page mode
    /// </summary>
    /// <param name="mode"></param>
    private void SetPageMode(PageDisplayMode mode)
    {
        switch (mode)
        {
            case PageDisplayMode.GridDisplay:
                IsGridMode = true;
                divGrid.Visible = true;
                divForm.Visible = false;                
                ReportId = 0;
                BindGrid();
                break;
            case PageDisplayMode.FormStep1Forward:
            case PageDisplayMode.FormStep1Backward:
                IsGridMode = false;
                divGrid.Visible = false;
                divForm.Visible = true;
                divAssignUsers.Visible = false;
                divStep1.Visible = true;
                divStep2.Visible = false;
                divStep3.Visible = false;
                divEmailEditor.Visible = false;
                btnStep1Backward.Visible = false;
                btnStep2Forward.Visible = true;
                btnStep3Forward.Visible = !IsNewRecord;
                btnStep2Backward.Visible = false;
                btnSaveAndClose.Visible = false;
                btnSaveAndRun.Visible = false;
                btnSaveUsersAndClose.Visible = false;
                btnSaveEmailAndClose.Visible = false;
                lblStep1Static.Visible = true;
                break;
            case PageDisplayMode.FormStep2Forward:
            case PageDisplayMode.FormStep2Backward:
                IsGridMode = false;
                divGrid.Visible = false;
                divForm.Visible = true;
                divAssignUsers.Visible = false;
                divStep1.Visible = false;
                divStep2.Visible = true;
                divStep3.Visible = false;
                divEmailEditor.Visible = false;
                btnStep1Backward.Visible = true;
                btnStep2Forward.Visible = false;
                btnStep3Forward.Visible = true;
                btnStep2Backward.Visible = false;
                btnSaveAndClose.Visible = false;
                btnSaveAndRun.Visible = false;
                btnSaveUsersAndClose.Visible = false;
                btnSaveEmailAndClose.Visible = false;
                lblStep1Static.Visible = true;
                break;
            case PageDisplayMode.FormStep3Forward:
                IsGridMode = false;
                divGrid.Visible = false;
                divForm.Visible = true;
                divAssignUsers.Visible = false;
                divStep1.Visible = false;
                divStep2.Visible = false;
                divStep3.Visible = true;
                divEmailEditor.Visible = false;
                btnStep1Backward.Visible = true;
                btnStep2Forward.Visible = false;
                btnStep3Forward.Visible = false;
                btnStep2Backward.Visible = true;
                btnSaveAndClose.Visible = true;
                btnSaveAndRun.Visible = true;
                btnSaveUsersAndClose.Visible = false;
                btnSaveEmailAndClose.Visible = false;
                lblStep1Static.Visible = true;
                break;
            case PageDisplayMode.ReportUsers:
                IsGridMode = false;
                divGrid.Visible = false;
                divForm.Visible = true;
                divAssignUsers.Visible = true;
                divStep1.Visible = false;
                divStep2.Visible = false;
                divStep3.Visible = false;
                divEmailEditor.Visible = false;
                btnStep1Backward.Visible = false;
                btnStep2Forward.Visible = false;
                btnStep3Forward.Visible = false;
                btnStep2Backward.Visible = false;
                btnSaveAndClose.Visible = false;
                btnSaveAndRun.Visible = false;
                btnSaveUsersAndClose.Visible = true;
                btnSaveEmailAndClose.Visible = false;
                lblStep1Static.Visible = false;
                break;
            case PageDisplayMode.EmailEditor:
                IsGridMode = false;
                divGrid.Visible = false;
                divForm.Visible = true;
                divAssignUsers.Visible = false;
                divStep1.Visible = false;
                divStep2.Visible = false;
                divStep3.Visible = false;
                divEmailEditor.Visible = true;
                btnStep1Backward.Visible = false;
                btnStep2Forward.Visible = false;
                btnStep3Forward.Visible = false;
                btnStep2Backward.Visible = false;
                btnSaveAndClose.Visible = false;
                btnSaveAndRun.Visible = false;
                btnSaveUsersAndClose.Visible = false;
                btnSaveEmailAndClose.Visible = true;
                lblStep1Static.Visible = false;
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Get aggregate function text
    /// </summary>
    /// <param name="nType"></param>
    /// <param name="itemText"></param>
    /// <returns></returns>
    public string GetGroupFunctionText(Konstants.AggregateFunctionType nType, string itemText = "")
    {
        switch (nType)
        {
            case Konstants.AggregateFunctionType.Count:
                itemText = "Count(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Sum:
                itemText = "Sum(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Min:
                itemText = "Min(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Max:
                itemText = "Max(" + itemText + ")";
                break;
            case Konstants.AggregateFunctionType.Average:
                itemText = "Avg(" + itemText + ")";
                break;
        }
        return itemText;
    }
    /// <summary>
    /// Checking the custom values parsing of the filter.
    /// </summary>
    /// <returns></returns>
    private bool CheckForValidCustomString()
    {
        try
        {
            CustomFilterParser nCustomFilter = new CustomFilterParser(txtCustomFilter.Text);
            float result = nCustomFilter.ParseInput();
            List<string> listOpds = nCustomFilter.listOperands;
            return ManageFiltersControl.CheckOrderNumberValues(listOpds);
        }
        catch (Exception ex)
        {
            ctrlStatusCustomFilter.SetStatus(ErrorMessages.ErrorParsing + ex.Message);
            return false;
        }
    }
    #endregion

    #region Events
    /// <summary>
    /// Initialize page
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_Initialize(object sender, EventArgs args)
    {
        if (!IsPostBack)
        {
            InnerLoad(true);
            if (Request.QueryString[Konstants.K_REPORT_QUERY_STRING] != null)
            {
                int id = 0;
                int.TryParse(Request.QueryString[Konstants.K_REPORT_QUERY_STRING].ToString(), out id);
                ReportId = id;
                EditReport();
            }
        }
    }
    /// <summary>
    /// Calls on every post back
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_PostBack(object sender, EventArgs args)
    {
        ctlStatus.SetStatus("");        
        BaseData = GridBaseDataId;
        ManageFiltersControl.ParentType = FilterParentType.CustomReport;
        ManageFiltersControl.Parent_key = ReportId;        
        ManageFiltersControl.ReportBaseDataID = FormBaseDataId;
    }   
    /// <summary>
    /// Refresh the grid on page size change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        BindGrid();
    }
    /// <summary>
    /// Refreshes the grid on page number change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        BindGrid();
    }
    /// <summary>
    /// Get the row command from the grid record and load the data accordingly.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>   
    protected void grdReports_RowCommand(object sender, GridCommandEventArgs e)
    {
        string command = e.CommandName;
        int id = 0;
        if (int.TryParse(e.CommandArgument.ToString(), out id))
        {
            ManageFiltersControl.Parent_key = ReportId = id;
        }
        switch (command)
        {
            case "EditX":
                EditReport();
                break;
            case "DeleteX":
                DeleteReport();
                break;
            case "CopyX":
                CopyReport();              
                break;
            case "RunX":
                Redirect(string.Format(Konstants.K_DISPLAY_REPORT_PAGE, ReportId));
                break;
            case "UsersX":
                EditReportUsers();
                break;
            case "EmailX":
                LoadEmailEditor();
                break;
        }
    }
    /// <summary>
    /// Copy report data and display values on the form
    /// </summary>
    private void CopyReport()
    {
        CopyRecords();
        EditReport();
        IsTemporarySave = true;
    }
    /// <summary>
    /// Sorting of grid according to the specified column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdReports_SortGrid(object sender, GridSortCommandEventArgs e)
    {
        if (ctlPager.SortBy == e.SortExpression)
            ctlPager.SortAscending = !ctlPager.SortAscending;
        else
        {
            ctlPager.SortBy = e.SortExpression;
            ctlPager.SortAscending = true;
        }
        BindGrid();
    }
    /// <summary>
    /// New report button event, initialize the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnNewReport_Click(object sender, EventArgs e)
    {
        AddNewRecord();
    }
    /// <summary>
    /// For going back to grid mode at any step of workflow
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnReturn_Click(object sender, EventArgs e)
    {
        if (IsTemporarySave)
        {
            DeleteReport();
        }
        IsTemporarySave = false;
        SetPageMode(PageDisplayMode.GridDisplay);
    }
    /// <summary>
    /// Go to the grid display without save
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelOnForm_Click(object sender, EventArgs e)
    {
        SetPageMode(PageDisplayMode.GridDisplay);
    }
    /// <summary>
    /// Save the report record and close the form and shows up the grid.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSaveAndCloseOnForm_Click(object sender, EventArgs e)
    {
        IsTemporarySave = false;
        SaveReport();
        SetPageMode(PageDisplayMode.GridDisplay);
    }
    /// <summary>
    /// Save the report record and close the form and shows up the grid.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSaveAndClose_Click(object sender, EventArgs e)
    {
        IsTemporarySave = false;
        SaveReportUsers();
        SetPageMode(PageDisplayMode.GridDisplay);
    }
    /// <summary>
    /// Save the record without closing the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSaveOnForm_Click(object sender, EventArgs e)
    {
        IsTemporarySave = false;
        SaveReport();
    }
    /// <summary>
    /// Search the report 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>    
    protected void btnSerachGo_Click(object sender, EventArgs e)
    {
        BindGrid(true);
    }   
    /// <summary>
    /// Filter selection criteria radio button list selection change event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rdBtnlstFilterSelection_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdBtnlstFilterSelection.SelectedValue == "2")//Custom selection criteria selected
        {
            txtCustomFilter.Enabled = true;
        }
        else
        {
            txtCustomFilter.Enabled = false;
        }
    }   
    /// <summary>
    /// Custom filter text change event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtCustomFilter_TextChanged(object sender, EventArgs e)
    {
        CheckForValidCustomString();
    }
    /// <summary>
    /// Go forward to the step2 of report create/edit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnStep2Forward_Click(object sender, EventArgs e)
    {
        if (txtReportTitle.Text == "")
        {            
            ctlStatus.SetStatus(ErrorMessages.EnterReportTitle);
            return;
        }
        SetPageMode(PageDisplayMode.FormStep2Forward);
    }
    /// <summary>
    /// Go backward to the step1 of report create/edit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnStep1Backward_Click(object sender, EventArgs e)
    {
        SetPageMode(PageDisplayMode.FormStep1Backward);
    }
    /// <summary>
    /// Go backward to the step2 of report create/edit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnStep2Backward_Click(object sender, EventArgs e)
    {
        SetPageMode(PageDisplayMode.FormStep2Backward);
    }
    /// <summary>
    /// Go forward to the step3 of report create/edit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnStep3Forward_Click(object sender, EventArgs e)
    {
        ManageFiltersControl.BindFilterFields();
        if (ctlColumnSelection.SelectedItems.Count < 1)
        {
            ctlStatus.SetStatus(ErrorMessages.SelectReportColumns);
            return;
        }
        SetPageMode(PageDisplayMode.FormStep3Forward);
        if (IsNewRecord)
        {
            //Temporarily saving the records  
            IsTemporarySave = true;
            SaveReport();
        }
    }
    /// <summary>
    /// Base data dropdown selection change event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlBaseData_SelectedIndexChanged(object sender, EventArgs e)
    {
        InitializeColumnSelection();        
    }
    /// <summary>
    /// Base data grid dropdown selection change event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void ddlBaseDataGrid_SelectedIndexChanged(object sender, EventArgs e)
    {
        BindGrid();
    }
    /// <summary>
    /// Save and run the report
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSaveAndRun_Click(object sender, EventArgs e)
    {
        IsTemporarySave = false;
        SaveReport();
        Redirect(string.Format(Konstants.K_DISPLAY_REPORT_PAGE, ReportId));
    }
    /// <summary>
    /// Save report email data and close the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSaveEmailAndClose_Click(object sender, EventArgs e)
    {
        IsTemporarySave = false;
        SaveEmailData();
    }
    #endregion
    
}