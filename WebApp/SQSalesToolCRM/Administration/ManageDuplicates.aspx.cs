using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using S = System.Web.Security.Membership;
using System.Linq.Dynamic;
using Telerik.Web.UI;
using System.Text.RegularExpressions;
using SalesTool.Schema;
using System.Data;
using System.Text;


public partial class Admin_ManageDuplicates : SalesBasePage
{
   

    #region Members/Properties
    /// <summary>
    /// Page display mode
    /// </summary>
    enum PageDisplayMode { GridQueueTemplate = 1, EditQueueTemplate = 2 }

    /// <summary>
    /// Record ID used in the whole form
    /// </summary>
    int RecordId
    {
        get
        {
            int lAns = 0;
            int.TryParse(hdnFieldEditTemplateKey.Value, out lAns);
            return lAns;
        }
        set
        {
            hdnFieldEditTemplateKey.Value = value.ToString();
        }
    }
    /// <summary>
    /// Is the user going for new record
    /// </summary>
    bool IsNewRecord
    {
        get
        {
            return RecordId < 1;
        }
    }
    public int totalGridRecords = 0;    
    #endregion

    #region Methods
    
    /// <summary>
    /// Sets the page display mode
    /// </summary>
    /// <param name="mode"></param>
    private void SetPageMode(PageDisplayMode mode)
    {
        switch (mode)
        {
            case PageDisplayMode.GridQueueTemplate:
                divForm.Visible = false;
                divGrid.Visible = true;                
                RecordId = 0;
                break;
            case PageDisplayMode.EditQueueTemplate:
                ctlFiltersForIncomingLeads.BindFilterFields();
                ctlFiltersForExistingLeads.BindFilterFields();
                divForm.Visible = true;
                divGrid.Visible = false;
                tlMultiPage.SelectedIndex = 0;
                tlDuplicatesStrip.SelectedIndex = 0;
                tlMultiPage.PageViews[1].Visible = false;
                tlMultiPage.PageViews[2].Visible = false;                
                break;
            default:
                break;
        }
    }
    /// <summary>
    /// Check for custom value for the filters for incoming leads
    /// </summary>
    /// <returns></returns>
    private bool CheckForValidCustomStringForIncoming()
    {
        try
        {
            CustomFilterParser nCustomFilter = new CustomFilterParser(txtCustomFilterForIncoming.Text);
            float result = nCustomFilter.ParseInput();
            List<string> listOpds = nCustomFilter.listOperands;
            return ctlFiltersForIncomingLeads.CheckOrderNumberValues(listOpds);
        }
        catch (Exception ex)
        {
            ctrlStatusCustomFilterForIncoming.SetStatus(ErrorMessages.ErrorParsing + ex.Message);
            return false;
        }
    }
    /// <summary>
    /// Check for custom value for the filters for existing leads
    /// </summary>
    /// <returns></returns>
    private bool CheckForValidCustomStringForExisting()
    {
        try
        {
            CustomFilterParser nCustomFilter = new CustomFilterParser(txtCustomFilterForExisting.Text);
            float result = nCustomFilter.ParseInput();
            List<string> listOpds = nCustomFilter.listOperands;
            return ctlFiltersForExistingLeads.CheckOrderNumberValues(listOpds);
        }
        catch (Exception ex)
        {
            ctrlStatusCustomFilterForExisting.SetStatus(ErrorMessages.ErrorParsing + ex.Message);
            return false;
        }
    }
    /// <summary>
    /// Save duplicate management record.
    /// </summary>
    /// <param name="ConvertToEditMode"></param>
    /// <returns></returns>
    public bool SaveRecord(bool ConvertToEditMode = false)
    {
        try
        {
            //Filter criteria for incoming leads
            short selectedFilterForIncoming = 0;
            short.TryParse(rdBtnlstFilterSelectionForIncoming.SelectedValue, out selectedFilterForIncoming);
            //Filter criteria for existing leads
            short selectedFilterForExisting= 0;
            short.TryParse(rdBtnlstFilterSelectionForExisting.SelectedValue, out selectedFilterForExisting);

            string customFilterValueForIncoming = "";
            string customFilterValueForExisting = "";
            
            bool hasCustomFilterError = false;
            if (selectedFilterForIncoming == (short)Konstants.FilterSelected.Custom)
            {
                //Check for valid Custom value expression like 1 AND 2 OR 3
                hasCustomFilterError = !CheckForValidCustomStringForIncoming();
                customFilterValueForIncoming = txtCustomFilterForIncoming.Text;
            }
            if (selectedFilterForExisting == (short)Konstants.FilterSelected.Custom)
            {
                //Check for valid Custom value expression like 1 AND 2 OR 3
                hasCustomFilterError = !CheckForValidCustomStringForExisting();
                customFilterValueForExisting= txtCustomFilterForExisting.Text;
            }
            if (hasCustomFilterError == true)
            {
                ctlStatus.SetStatus(ErrorMessages.CustomFilterValueError);
                return false;
            }
            if (IsNewRecord)
            {
                if (Engine.DuplicateRecordActions.Exists(txtTitle.Text))
                {
                    ctlStatus.SetStatus(ErrorMessages.SameTitleError);
                    return false;
                }                
                DuplicateManagement nDuplicateRecord = new DuplicateManagement();
                SetDuplicateRecord(selectedFilterForIncoming, selectedFilterForExisting, customFilterValueForIncoming, customFilterValueForExisting, nDuplicateRecord);
                int recordAddedId = Engine.DuplicateRecordActions.Add(nDuplicateRecord, CurrentUser.FullName);
                AddColumns(recordAddedId);
                AddColumnsForMerging(recordAddedId);
                if (ConvertToEditMode)
                {
                    RecordId = recordAddedId;
                    ctlFiltersForIncomingLeads.Parent_key = recordAddedId;
                    ctlFiltersForIncomingLeads.BindEmailFilterGrid();
                    ctlFiltersForExistingLeads.Parent_key = recordAddedId;
                    ctlFiltersForExistingLeads.BindEmailFilterGrid();
                    //tlDuplicatesStrip.Tabs[2].Enabled = true;
                    //tlPageFilters.Enabled = true;
                }
            }
            else 
            {
                DuplicateManagement nDuplicateRecord = Engine.DuplicateRecordActions.Get(RecordId);                    
                SetDuplicateRecord(selectedFilterForIncoming, selectedFilterForExisting, customFilterValueForIncoming, customFilterValueForExisting, nDuplicateRecord);
                Engine.DuplicateRecordActions.ClearTags(nDuplicateRecord.Id);
                AddColumns(nDuplicateRecord.Id);
                AddColumnsForMerging(nDuplicateRecord.Id);
            }            
            ctlStatus.SetStatus(Messages.RecordSavedSuccess);
            return true;
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
            return false;
        }
    }
    /// <summary>
    /// Add selected columns for duplicate record checking
    /// </summary>
    /// <param name="recordAddedId"></param>
    private void AddColumns(int recordAddedId)
    {               
        List<int> tagIdsList = new List<int>();        
        foreach (ListItem itemSelected in ctlColumnSelection.SelectedItems)
        {
            if (itemSelected.Value != "-1")
            {
                int nValue = 0;
                if (int.TryParse(itemSelected.Value, out nValue))
                {
                    tagIdsList.Add(nValue);
                }
            }
        }
        Engine.DuplicateRecordActions.AddTags(recordAddedId, tagIdsList.ToArray());
    }
    /// <summary>
    /// Add selected columns for merging duplicate records
    /// </summary>
    /// <param name="recordAddedId"></param>
    private void AddColumnsForMerging(int recordAddedId)
    {
        List<int> tagIdsList = new List<int>();
        foreach (ListItem itemSelected in ctlColumnSelectionMergeRules.SelectedItems)
        {
            if (itemSelected.Value != "-1")
            {
                int nValue = 0;
                if (int.TryParse(itemSelected.Value, out nValue))
                {
                    tagIdsList.Add(nValue);
                }
            }
        }
        Engine.DuplicateRecordActions.AddTagsForMergeRules(recordAddedId, tagIdsList.ToArray());
    }
    /// <summary>
    /// Set the duplicate values for data insertion
    /// </summary>
    /// <param name="selectedFilterForIncoming">Selected filter criteria for incoming leads</param>
    /// <param name="selectedFilterForExisting">Selected filter criteria for existing leads</param>
    /// <param name="customFilterValueForIncoming">Custom Value for the filters of incoming leads</param>
    /// <param name="customFilterValueForExisting">Custom Value for the filters of existing leads</param>
    /// <param name="nDuplicateRecord"></param>
    private void SetDuplicateRecord(short selectedFilterForIncoming, short selectedFilterForExisting, string customFilterValueForIncoming, string customFilterValueForExisting, DuplicateManagement nDuplicateRecord)
    {
        nDuplicateRecord.Title = txtTitle.Text;
        //nDuplicateRecord.ActionComment 
        //nDuplicateRecord.ActionId
        nDuplicateRecord.ExistingLeadFilterSelection = selectedFilterForExisting;
        nDuplicateRecord.ExistingLeadFilterCustomValue = customFilterValueForExisting;
        nDuplicateRecord.IncommingLeadFilterSelection = selectedFilterForIncoming;
        nDuplicateRecord.IncommingLeadFilterCustomValue = customFilterValueForIncoming;
        //nDuplicateRecord.FieldTags.Add
        nDuplicateRecord.IsActive = chkEnabled.Checked;
        nDuplicateRecord.IsManual = chkManual.Checked;
        //nDuplicateRecord.MultipleDuplicateCriteria
        //nDuplicateRecord.SelectedParent
    }
    /// <summary>
    /// Bind the duplicate rules to the grid
    /// </summary>
    private void BindDuplicateRulesGrid()
    {                
        var endRecords = Helper.SortRecords(Engine.DuplicateRecordActions.All, "Priority", false);
        totalGridRecords = endRecords.Count();
        grdLeadDuplicateRules.DataSource = ctlPager.ApplyPaging(endRecords.AsEnumerable());
        grdLeadDuplicateRules.DataBind();
    }
    /// <summary>
    /// Get the operator text of the filter
    /// </summary>
    /// <param name="operatorValue">Operator Value</param>
    /// <returns>Operator Text</returns>
    private String GetOperatorText(string operatorValue)
    {
        switch (operatorValue)
        {
            case "0":
                return "Equal to";
            case "1":
                return "Not Equal to";
            case "2":
                return "Less than";
            case "3":
                return "Less than or equal to";
            case "4":
                return "Greater than";
            case "5":
                return "Greater than or equal to";
            case "6":
                return "Contains";
            case "7":
                return "Does not contains";
            case "8":
                return "Within";
            case "9":
                return "Not Within";
            default:
                return "";
        }
    }
    /// <summary>
    /// Get the value for the current filter
    /// </summary>
    /// <param name="nFilter"></param>
    /// <param name="nTagFields"></param>
    /// <returns></returns>
    private string GetValueText(FilterArea nFilter, TagFields nTagFields)
    {
        if (nFilter.WithinSelect == true)
        {
            if (nFilter.WithinRadioButtonSelection == false)
            {                
                switch (nFilter.WithinPredefined.ToString())
                {
                    case "0":
                        return "Today";
                    case "1":
                        return "Since Monday";
                    case "2":
                        return "This calendar month";
                    case "3":
                        return "This calendar year";
                    case "4":
                        return "In past";
                    case "5":
                        return "In future";
                    default:
                        break;
                }
                return nFilter.Value;
            }
            else
            {
                string valueWithin = "";
                valueWithin = nFilter.WithinLastNext == false ? "Last " : "Next ";
                valueWithin += nFilter.Value + " ";
                switch (nFilter.WithinLastNextUnit.ToString())
                {
                    case "0":
                        valueWithin += "Days";
                        break;
                    case "1":
                        valueWithin += "Hours";
                        break;
                    case "2":
                        valueWithin += "Minutes";
                        break;
                    default:
                        break;
                }

                return valueWithin;
            }
        }
        else if (nTagFields.FilterDataType == (byte)Konstants.FilterFieldDataType.Table)// For lookup table 
        {
            var lookupTable = Engine.SQTablesActions.GetAll().OrderBy(m => m.Name).Where(l => l.Id == nTagFields.TableKey).FirstOrDefault();
            string[] strValuesArray = nFilter.Value.Split(',');
            string selectedItems = "";
            for (int i = 0; i < strValuesArray.Length; i++)
            {
                try
                {
                    TableStructure nTable = new TableStructure();
                    string query = "select " + lookupTable.KeyFieldName + " as [Key], " + lookupTable.TitleFieldName + " as Title from " + lookupTable.SystemTableName + " Where " + lookupTable.KeyFieldName + " = '" + strValuesArray[i].ToString() + "'";

                    //DataTable dtRecords = nTable.GetDatatable(ApplicationSettings.ADOConnectionString, query);
                    DataTable dtRecords = nTable.GetDatatable(ApplicationSettings.ADOConnectionString, query);
                    if (dtRecords.Rows.Count > 0)
                    {
                        DataRow dr = dtRecords.Rows[0];

                        selectedItems += dr["Title"].ToString();
                        selectedItems += ",";
                    }
                }
                catch (Exception)
                {
                    continue;
                }
            }
            return selectedItems.TrimEnd(',');
        }
        else
        {
            return nFilter.Value;
        }
    }
    /// <summary>
    /// Clear form fields
    /// </summary>
    public void ClearFields()
    {
        txtTitle.Text = "";
        
        chkEnabled.Checked = true;
        chkManual.Checked = true;
        txtCustomFilterForIncoming.Text = "";
        rdBtnlstFilterSelectionForIncoming.SelectedIndex = 0;
        rdBtnlstFilterSelectionForIncoming_SelectedIndexChanged(this, null);

        txtCustomFilterForExisting.Text = "";
        rdBtnlstFilterSelectionForExisting.SelectedIndex = 0;
        rdBtnlstFilterSelectionForExisting_SelectedIndexChanged(this, null);
        //tlDuplicatesStrip.Tabs[2].Enabled = false;
        //tlPageFilters.Enabled = false;        
    }
    /// <summary>
    /// Load the edit form values 
    /// </summary>
    /// <param name="recordId"></param>
    public void LoadEditFormValues(int recordId)
    {
        DuplicateManagement nDuplicateRule = Engine.DuplicateRecordActions.Get(recordId);
        txtTitle.Text = nDuplicateRule.Title;
        chkEnabled.Checked = nDuplicateRule.IsActive.HasValue? nDuplicateRule.IsActive.Value : false;
        chkManual.Checked = nDuplicateRule.IsManual.HasValue ? nDuplicateRule.IsManual.Value : false;
        //Filters for Incoming leads
        rdBtnlstFilterSelectionForIncoming.SelectedValue = nDuplicateRule.IncommingLeadFilterSelection == null ? "0" : nDuplicateRule.IncommingLeadFilterSelection.ToString();
        if (nDuplicateRule.IncommingLeadFilterSelection == (short)Konstants.FilterSelected.Custom)
        {
            txtCustomFilterForIncoming.Text = nDuplicateRule.IncommingLeadFilterCustomValue;
        }
        rdBtnlstFilterSelectionForIncoming_SelectedIndexChanged(this, null);
        ctlFiltersForIncomingLeads.Parent_key = RecordId;
        ctlFiltersForIncomingLeads.BindEmailFilterGrid();
        //Filters for Existing leads
        rdBtnlstFilterSelectionForExisting.SelectedValue = nDuplicateRule.ExistingLeadFilterSelection == null ? "0" : nDuplicateRule.ExistingLeadFilterSelection.ToString();
        if (nDuplicateRule.ExistingLeadFilterSelection == (short)Konstants.FilterSelected.Custom)
        {
            txtCustomFilterForExisting.Text = nDuplicateRule.ExistingLeadFilterCustomValue;
        }
        rdBtnlstFilterSelectionForExisting_SelectedIndexChanged(this, null);
        ctlFiltersForExistingLeads.Parent_key = RecordId;
        ctlFiltersForExistingLeads.BindEmailFilterGrid();

        InitializeColumnSelection();
        //IQueryable<ReportColumns> T = Engine.ReportColumnsAction.GetAllByReportID(ReportId);
        var tagColumn = Engine.DuplicateRecordActions.Get(RecordId).FieldTagsRulesColumns.Select(k => new { Key = k.Id, Name = k.Name, k.Group });            
        foreach (var U in tagColumn)
        {     
            ListItem nlistitem = new ListItem("---" + U.Group.ToString() + "---", "-1");
            ctlColumnSelection.SelectedItems.Insert(ctlColumnSelection.SelectedItems.IndexOf(nlistitem) + 1, new ListItem(U.Name, U.Key.ToString()));            
        }
        var tagColumnForMergeRules = Engine.DuplicateRecordActions.Get(RecordId).FieldTagsMergeColumns.Select(k => new { Key = k.Id, Name = k.Name, k.Group });
        foreach (var U in tagColumnForMergeRules)
        {
            ListItem nlistitem = new ListItem("---" + U.Group.ToString() + "---", "-1");
            ctlColumnSelectionMergeRules.SelectedItems.Insert(ctlColumnSelectionMergeRules.SelectedItems.IndexOf(nlistitem) + 1, new ListItem(U.Name, U.Key.ToString()));
        }
    }

    /// <summary>
    /// Initialize column selection
    /// </summary>
    private void InitializeColumnSelection()
    {
        //Initialize and bind column names to the control
        ctlColumnSelection.Initialize();
        ctlColumnSelection.SetGroupFlag();

        ctlColumnSelectionMergeRules.Initialize();
        ctlColumnSelectionMergeRules.SetGroupFlag();
        BindColumnsName();
    }
    /// <summary>
    /// Binds the columns for selection for the duplicate record rules
    /// </summary>
    private void BindColumnsName()
    {
        if (ctlColumnSelection.AvailableItems.Count > 0)
                ctlColumnSelection.AvailableItems.Clear();
        if (ctlColumnSelectionMergeRules.AvailableItems.Count > 0)
            ctlColumnSelectionMergeRules.AvailableItems.Clear();
        ListItem firstItem = new ListItem("***Select Column***", "-1");
        firstItem.Enabled = false;
        ctlColumnSelection.AvailableItems.Add(firstItem);
        ctlColumnSelectionMergeRules.AvailableItems.Add(firstItem);
        var separators = Engine.TagFieldsActions.GetAll().Select(m => new { m.Group }).Distinct();
        //Add items with group wise separation in the Item collection of control
        foreach (var titleGroup in separators)
        {
            ListItem nlistitem = new ListItem("---" + titleGroup.Group.ToString() + "---", "-1");
            ctlColumnSelection.AvailableItems.Add(nlistitem);
            ctlColumnSelection.SelectedItems.Add(nlistitem);

            ctlColumnSelectionMergeRules.AvailableItems.Add(nlistitem);
            ctlColumnSelectionMergeRules.SelectedItems.Add(nlistitem);
            IEnumerable<TagFields> U = null;
            IEnumerable<TagFields> T = null;
            //All field tags not used here anymore, Here we used the account history base query data as same will be used for the duplicate rules implementation.
            if (IsNewRecord)
                U = T = Engine.TagFieldsActions.GetAllReportTagsByBaseDataID((short)Konstants.CustomReportBaseData.AccountHistory);
            else
            {
                U = Engine.DuplicateRecordActions.GetFreeTags(RecordId, (short)Konstants.CustomReportBaseData.AccountHistory);
                T = Engine.DuplicateRecordActions.GetFreeTagsForMergeRules(RecordId, (short)Konstants.CustomReportBaseData.AccountHistory);
            }
            var groupColumns = U.Select(k => new { Key = k.Id, Name = k.Name, k.Group, k.FilterDataType }).Where(m => m.Group == titleGroup.Group).OrderBy(m => m.Name).ToList();
            foreach (var item in groupColumns)
            {
                ctlColumnSelection.AvailableItems.Add(new ListItem(item.Name, item.Key.ToString()));               
            }
            var groupColumnsForMergeRules = T.Select(k => new { Key = k.Id, Name = k.Name, k.Group, k.FilterDataType }).Where(m => m.Group == titleGroup.Group).OrderBy(m => m.Name).ToList();
            foreach (var item in groupColumnsForMergeRules)
            {                
                ctlColumnSelectionMergeRules.AvailableItems.Add(new ListItem(item.Name, item.Key.ToString()));
            }
        }

    }
    /// <summary>
    /// Load form values in edit mode
    /// </summary>
    private void EditRecord()
    {
        SetPageMode(PageDisplayMode.EditQueueTemplate);
        LoadEditFormValues(RecordId);        
    }
    #endregion
    
    #region Events

    /// <summary>
    /// Refresh the grid on page size change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PageSizeChanged(object sender, PagingEventArgs e)
    {
        BindDuplicateRulesGrid();
    }
    /// <summary>
    /// Refreshes the grid on page number change
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Evt_PageNumberChanged(object sender, PagingEventArgs e)
    {
        BindDuplicateRulesGrid();
    }
    
    /// <summary>
    /// Page Initialize event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_Initialize(object sender, EventArgs args)
    {
        if (!Page.IsPostBack)
        {            
            ctlFiltersForIncomingLeads.ParentType = FilterParentType.DuplicateCheckingForIncomingLeads;
            ctlFiltersForIncomingLeads.AddedBy = CurrentUser.FullName;
            ctlFiltersForIncomingLeads.ChangedBy = CurrentUser.FullName;
            ctlFiltersForIncomingLeads.Title = "By default this program will check all incoming leads for duplicates. You may apply filters to define a subset of leads to be checked.";

            ctlFiltersForExistingLeads.ParentType = FilterParentType.DuplicateCheckingForExistingLeads;
            ctlFiltersForExistingLeads.AddedBy = CurrentUser.FullName;
            ctlFiltersForExistingLeads.ChangedBy = CurrentUser.FullName;
            ctlFiltersForExistingLeads.Title = "By default all existing leads are checked for matches. You may apply filters to define a subset of leads to be checked.";
            //All field tags not used here anymore, Here we used the account history base query data as same will be used for the duplicate rules implementation.
            ctlFiltersForIncomingLeads.ReportBaseDataID = ctlFiltersForExistingLeads.ReportBaseDataID = (short)Konstants.CustomReportBaseData.AccountHistory;
            
            SetPageMode(PageDisplayMode.GridQueueTemplate);
            BindDuplicateRulesGrid();

        }
    }
    /// <summary>
    /// Page postback event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    protected override void Page_PostBack(object sender, EventArgs args)
    {
        ctlStatus.SetStatus("");
        ctrlStatusCustomFilterForIncoming.SetStatus("");
        ctrlStatusCustomFilterForExisting.SetStatus("");
        ctlFiltersForIncomingLeads.ParentType = FilterParentType.DuplicateCheckingForIncomingLeads;
        ctlFiltersForExistingLeads.ParentType = FilterParentType.DuplicateCheckingForExistingLeads;
        ctlFiltersForIncomingLeads.Parent_key = ctlFiltersForExistingLeads.Parent_key = RecordId;
        //All field tags not used here anymore, Here we used the account history base query data as same will be used for the duplicate rules implementation.
        ctlFiltersForIncomingLeads.ReportBaseDataID = ctlFiltersForExistingLeads.ReportBaseDataID = (short)Konstants.CustomReportBaseData.AccountHistory;
        
    }   
    /// <summary>
    /// Calls when adding new record.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddNewQueue_Click(object sender, EventArgs e)
    {
        RecordId = 0;
        SetPageMode(PageDisplayMode.EditQueueTemplate);
        ClearFields();        

        ctlFiltersForIncomingLeads.Parent_key = 0;
        ctlFiltersForIncomingLeads.BindEmailFilterGrid();        
        ctlFiltersForIncomingLeads.SetControlModeFromOutside(true);

        ctlFiltersForExistingLeads.Parent_key = 0;
        ctlFiltersForExistingLeads.BindEmailFilterGrid();
        ctlFiltersForExistingLeads.SetControlModeFromOutside(true);

        InitializeColumnSelection();
    }
    /// <summary>
    /// Filter criteria for the incoming leads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rdBtnlstFilterSelectionForIncoming_SelectedIndexChanged(object sender, EventArgs e)
    {
        short nFilterSelection = 0;
        short.TryParse(rdBtnlstFilterSelectionForIncoming.SelectedValue , out nFilterSelection);
        //Custom filter selection option selected.
        txtCustomFilterForIncoming.Enabled = (nFilterSelection == (short)Konstants.FilterSelected.Custom) ? true : false;        
    }
    /// <summary>
    /// Filter criteria for the existing leads
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rdBtnlstFilterSelectionForExisting_SelectedIndexChanged(object sender, EventArgs e)
    {
        short nFilterSelection = 0;
        short.TryParse(rdBtnlstFilterSelectionForExisting.SelectedValue, out nFilterSelection);
        //Custom filter selection option selected.
        txtCustomFilterForExisting.Enabled = (nFilterSelection == (short)Konstants.FilterSelected.Custom) ? true : false;        
    }
    /// <summary>
    /// Custom value for Incoming leads filter text changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtCustomFilterForIncoming_TextChanged(object sender, EventArgs e)
    {
        CheckForValidCustomStringForIncoming();
    }
    /// <summary>
    /// Custom value for existing leads filter text changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtCustomFilterForExisting_TextChanged(object sender, EventArgs e)
    {
        CheckForValidCustomStringForExisting();
    }
    /// <summary>
    /// On apply changes of the form, it does not closes the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnApply_Click(object sender, EventArgs e)
    {       
        SaveRecord(true);                    
    }
    /// <summary>
    /// Save and close the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            bool hasSavedRecordSuccessful = SaveRecord();
            if (hasSavedRecordSuccessful)
            {
                SetPageMode(PageDisplayMode.GridQueueTemplate);
                BindDuplicateRulesGrid();
                ClearFields();                
            }
        }
        catch (Exception ex)
        {
            ctlStatus.SetStatus(ex);
        }
    }
    /// <summary>
    /// On Cancel of the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelOnForm_Click(object sender, EventArgs e)
    {       
        SetPageMode(PageDisplayMode.GridQueueTemplate);
        BindDuplicateRulesGrid();
    }
    /// <summary>
    /// Use for setting the priority buttons on first and last row.
    /// On First row up priority button will not be visible.
    /// On Last row down priority button will not be visible.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdLeadDuplicateRules_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            int count = grdLeadDuplicateRules.Items.Count;
            GridDataItem item = (GridDataItem)e.Item;
            int index = item.ItemIndex;
            if (index == 0)
            {
                ImageButton imgBtnUpOrder = (ImageButton)item.FindControl("imgBtnUpOrder");
                imgBtnUpOrder.Visible = false;    
            }
            if (totalGridRecords-1 == index)
            {
                ImageButton imgBtnDownOrder = (ImageButton)item.FindControl("imgBtnDownOrder");
                imgBtnDownOrder.Visible = false;                
            }
            int recordID = 0;
            if (int.TryParse(item.GetDataKeyValue("Id").ToString(), out recordID))
            {
                StringBuilder nFilterText = ExtractFiltersText(recordID);      
                item["FilterText"].Text = nFilterText.ToString();
            }
        }        
    }

    private StringBuilder ExtractFiltersText(int recordID)
    {
        var tagColumn = Engine.DuplicateRecordActions.Get(recordID).FieldTagsRulesColumns.Select(k => new { Key = k.Id, Name = k.Name, k.Group });
        string columnNames = "Fields : ";
        foreach (var U in tagColumn)
        {
            columnNames += U.Name + ",";
        }
        columnNames = columnNames.EndsWith(",") ? columnNames.Remove(columnNames.Length - 1) : columnNames;
        StringBuilder nFilterText = new StringBuilder();
        nFilterText.AppendLine(columnNames);
        nFilterText.AppendLine("<br/>");
        nFilterText.AppendLine("Filters : ");
        var result = Engine.FilterAreaActions.GetAll().Join(Engine.TagFieldsActions.GetAll(),
            a => a.FilteredColumnTagkey,
            b => b.Id,
            (a, b) => new { Filters = a, Columns = b })
            .Where(c => c.Filters.ParentKey == recordID && c.Filters.ParentType == (short)FilterParentType.DuplicateCheckingForExistingLeads)
            .Select(c => new
            {
                key = c.Columns.Id,
                FilterText = string.Format("<b>[{0}]</b> {1} <b>{2}</b>", c.Columns.Name, GetOperatorText(c.Filters.Operator.ToString()), GetValueText(c.Filters, c.Columns)),
                OrderNumber = c.Filters.OrderNumber
            }).OrderBy(x => x.OrderNumber);
        foreach (var T in result)
        {
            nFilterText.AppendLine(T.FilterText);
            nFilterText.AppendLine("<br/>");
        }
        return nFilterText;
    }
    /// <summary>
    /// Calls when when row is dropped somewhere on the grid, it changes the priority of the record according 
    /// to the place where it is dropped.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdLeadDuplicateRules_RowDrop(object sender, Telerik.Web.UI.GridDragDropEventArgs e)
    {
        if (string.IsNullOrEmpty(e.HtmlElement))
        {
            if (e.DraggedItems[0].OwnerGridID == grdLeadDuplicateRules.ClientID)
            {
                int destPriorityId=(int)e.DestDataItem.GetDataKeyValue("Priority");
                foreach (GridDataItem draggedItem in e.DraggedItems)
                {
                    int recordId = (int)draggedItem.GetDataKeyValue("Id");
                    if (e.DropPosition == GridItemDropPosition.Below)
                    {
                       Engine.DuplicateRecordActions.Move(recordId, destPriorityId - 1);    
                    }
                    else
                        Engine.DuplicateRecordActions.Move(recordId, destPriorityId);
                    ctlStatus.SetStatus(Messages.RecordMovedSuccess);
                    BindDuplicateRulesGrid();                    
                }
            }
        }
    }
    /// <summary>
    /// ItemCommand event is raised when any button is clicked in the DataGrid control. 
    /// This event is commonly used to handle buttons controls with a custom CommandName value in the DataGrid control.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdLeadDuplicateRules_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        string command = e.CommandName;
        int id = 0;
        if (int.TryParse(e.CommandArgument.ToString(), out id))
        {
            ctlFiltersForIncomingLeads.Parent_key = RecordId= id;
            ctlFiltersForIncomingLeads.SetControlModeFromOutside(true);
            ctlFiltersForIncomingLeads.BindEmailFilterGrid();

            ctlFiltersForExistingLeads.Parent_key = RecordId;
            ctlFiltersForExistingLeads.SetControlModeFromOutside(true);
            ctlFiltersForExistingLeads.BindEmailFilterGrid();
        }
        switch (e.CommandName)
        {
            case "EditX":
                EditRecord();
                break;
            case "EnabledX":
                Engine.DuplicateRecordActions.Enabled(RecordId);
                ctlStatus.SetStatus(Messages.RecordUpdatedSuccess);
                break;
            case "DeleteX":
                Engine.FilterAreaActions.DeleteAll(RecordId, (short)FilterParentType.DuplicateCheckingForIncomingLeads);
                Engine.FilterAreaActions.DeleteAll(RecordId, (short)FilterParentType.DuplicateCheckingForExistingLeads);
                Engine.DuplicateRecordActions.ClearTags(RecordId);
                Engine.DuplicateRecordActions.DeletePotentialDuplicatesByRuleId(RecordId);
                Engine.DuplicateRecordActions.Delete(RecordId);
                ctlStatus.SetStatus(Messages.RecordDeletedSuccess);
                break;
            case "UpOrder":
                Engine.DuplicateRecordActions.Up(RecordId);
                ctlStatus.SetStatus(Messages.RecordMovedUpSuccess);
                break;
            case "DownOrder":
                Engine.DuplicateRecordActions.Down(RecordId);
                ctlStatus.SetStatus(Messages.RecordMovedDownSuccess);
                break;                        
        }        
        BindDuplicateRulesGrid();
    }
    /// <summary>
    /// On Tab click of the form
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void tlDuplicatesStrip_TabClick(object sender, RadTabStripEventArgs e)
    {
        if (IsNewRecord) SaveRecord(true);
        e.Tab.PageView.Visible = true;
    }    
    #endregion


   
}
