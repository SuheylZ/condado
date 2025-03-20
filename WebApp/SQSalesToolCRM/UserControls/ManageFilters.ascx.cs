using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.Schema;
using Telerik.Web.UI;
using SalesTool.DataAccess.Models;


//// SZ [jan 10, 2013] 
//public enum FilterParentType
//{
//    Unknown = 99,
//    EmailWebForm = 0,
//    PostsWebForm = 1,
//    PrioritizationWebForm = 2,
//    RetentionWebForm = 3,
//    SubStatus2 = 4,
//    CustomReport = 5,
//    DuplicateCheckingForIncomingLeads = 6,
//    DuplicateCheckingForExistingLeads = 7

//};

public partial class UserControls_ManageFilters : SalesUserControl
{

    #region Members/Properties

    enum OperatorMode { TwoOperators = 1, FourOperators = 2, SixOperators = 3, EightOperators = 4, TenOperators = 5 }
    enum ValueMode { BitField = 1, LookupTableDropDown = 2, LookupTableMultiSelect = 3, NumericField = 4, DateOnlyField = 5, DateTimeField = 6, DateWithinOrNot = 7, TextField = 8 }
    enum ValueOption { OptionOne = 1, OptionTwo = 2 }
    enum ControlDisplayMode { AddFilter = 1, EditFilter = 2 }


    public string Title { set { lblTitle.InnerText = value; } }
    public int Parent_key
    {
        get
        {
            if (hdnFieldParentkey.Value == "") return 0;
            return Convert.ToInt32(hdnFieldParentkey.Value);
        }
        set
        {
            hdnFieldParentkey.Value = value.ToString();
        }
    }

    public FilterParentType ParentType
    {
        get
        {
            //FilterParentType type= FilterParentType.Unknown;
            //switch (Parent_Type)
            //{
            //    case 0: type = FilterParentType.EmailWebForm; break;
            //    case 1: type = FilterParentType.PostsWebForm; break;
            //    case 2: type = FilterParentType.PrioritizationWebForm; break;
            //    case 3: type = FilterParentType.RetentionWebForm; break;
            //}
            return (FilterParentType)Parent_Type;
        }
        set
        {
            Parent_Type = Convert.ToInt16((int)value);
        }
    }

    // SZ [Jan 10, 2013] integers must not be used for this purpose. 
    // use enumerated, use enumerated, use enumerated, use enumerated
    // DO NOT USE INT and SHORT DIRECTLY, FOR GOD'S SAKE!!!
    // chnaged from public to private. Use ParentType

    private short Parent_Type
    {
        get
        {

            if (hdnFieldParentType.Value == "") return 0;
            return Convert.ToInt16(hdnFieldParentType.Value);
        }
        set
        {

            if (value == (short)FilterParentType.SubStatus2)
                IsTagWorkFlow = true;
            //YA[June 4, 2013] Added to show field tags based on basedata of custom report.
            else if (value == (short)FilterParentType.CustomReport)
                IsCustomReport = true;
            else if (value == (short)FilterParentType.DuplicateCheckingForIncomingLeads || value == (short)FilterParentType.DuplicateCheckingForExistingLeads)
                IsDuplicateCheck = true;
            hdnFieldParentType.Value = value.ToString();
        }
    }
    //YA[June 4, 2013] Base Data ID for custom reports
    public int ReportBaseDataID
    {
        get
        {
            int nValue = 0;
            int.TryParse(hdnCustomReportBaseDataID.Value, out nValue);
            return nValue;
        }
        set
        {
            hdnCustomReportBaseDataID.Value = value.ToString();
        }
    }
    public string AddedBy
    {
        get
        {
            return hdnFieldAddedBy.Value;
        }
        set
        {
            hdnFieldAddedBy.Value = value;
        }
    }


    public string ChangedBy
    {
        get
        {
            return hdnFieldChangedBy.Value;
        }
        set
        {
            hdnFieldChangedBy.Value = value;
        }
    }
    public DateTime AddedOn { get; set; }
    public DateTime ChangedOn { get; set; }

    private bool IsTagWorkFlow
    {
        get
        {
            bool bAns = false;
            bool.TryParse(hdnTaqWorkflow.Value, out bAns);
            return bAns;
        }
        set
        {
            hdnTaqWorkflow.Value = value.ToString();
        }
    }

    private bool IsCustomReport
    {
        get
        {
            bool bAns = false;
            bool.TryParse(hdnIsCustomReport.Value, out bAns);
            return bAns;
        }
        set
        {
            hdnIsCustomReport.Value = value.ToString();
        }
    }

    private bool IsDuplicateCheck
    {
        get
        {
            bool bAns = false;
            bool.TryParse(hdnIsDuplicateCheck.Value, out bAns);
            return bAns;
        }
        set
        {
            hdnIsDuplicateCheck.Value = value.ToString();
        }
    }

    public string ConnectionString
    {
        get
        {
            if (Application["ConnectionString"] == null)
            {
                return System.Configuration.ConfigurationManager.ConnectionStrings[Konstants.K_ADO_CONNECTION].ToString();
            }
            else
            {
                return Application["ConnectionString"].ToString();
            }
        }
    }

    #endregion

    #region Methods

    protected override void InnerInit()
    {
        InitializeFilter();
    }

    private void ClearFilter()
    {
        divCheckBox.Visible = false;
        divDate.Visible = false;
        divDateTime.Visible = false;
        divDateWithInorNotValue.Visible = false;
        divLookupTableValue.Visible = false;
        divLookupTableMultiSelect.Visible = false;
        divNumericValue.Visible = false;
        divTextValue.Visible = false;
        ddlOperators.Visible = false;
        ddlFieldsColumn.SelectedIndex = 0;
        txtDayHourMin.Text = "";
        txtValueNumeric.Text = "";
        txtValueText.Text = "";
        tlDateInputValue.SelectedDate = null;
        tlDateOnlyValue.SelectedDate = null;
        ddlCriteria.SelectedIndex = 0;
        ddlDayHourMin.SelectedIndex = 0;
        ddlLastNext.SelectedIndex = 0;
        ddlLookupTableValue.SelectedIndex = -1;
        ddlOperators.SelectedIndex = 0;
    }
    private void SetFilterValue(String columnField, string filterValue, ValueOption optionOperator = ValueOption.OptionOne)
    {
        switch (columnField)
        {
            case "0":
                txtValueNumeric.Text = filterValue;
                break;
            case "1":
                txtValueText.Text = filterValue;
                break;
            case "2":
                if (optionOperator == ValueOption.OptionOne)
                    tlDateOnlyValue.SelectedDate = Convert.ToDateTime(filterValue);
                //else
                //    return "SelectedWithInOrNot";
                break;
            case "3":
                if (optionOperator == ValueOption.OptionOne)
                    ddlLookupTableValue.SelectedValue = filterValue;
                else
                {
                    string selectedItems = String.Empty;
                    if (lstBoxLookupMultitext.Items.Count > 0)
                    {
                        string[] strValuesArray = filterValue.Split(',');
                        for (int i = 0; i < strValuesArray.Length; i++)
                        {
                            lstBoxLookupMultitext.Items.FindByValue(strValuesArray[i]).Selected = true;
                        }
                    }
                }
                break;
            case "4":
                chkValue.Checked = filterValue == "False" ? false : true;
                break;
            case "5":
                if (optionOperator == ValueOption.OptionOne)
                    tlDateInputValue.SelectedDate = Convert.ToDateTime(filterValue);
                //tlDateInputValue.DateInput.SelectedDate.ToString() + " " + txtTimeValue.Text;
                //else
                //    return "SelectedWithInOrNot";
                break;
        }
    }
    private string GetFilterValue(String columnField, ValueOption optionOperator = ValueOption.OptionOne)
    {
        switch (columnField)
        {
            case "0":
                return txtValueNumeric.Text;
            case "1":
                return txtValueText.Text;
            case "2":
                if (optionOperator == ValueOption.OptionOne)
                    return tlDateOnlyValue.DateInput.SelectedDate.Value.ToShortDateString();
                else
                    return "SelectedWithInOrNot";
            case "3":
                if (optionOperator == ValueOption.OptionOne)
                    return ddlLookupTableValue.SelectedItem.Value;
                else
                {
                    string selectedItems = String.Empty;
                    if (lstBoxLookupMultitext.Items.Count > 0)
                    {
                        for (int i = 0; i < lstBoxLookupMultitext.Items.Count; i++)
                        {
                            if (lstBoxLookupMultitext.Items[i].Selected)
                            {
                                selectedItems += lstBoxLookupMultitext.Items[i].Value;
                                selectedItems += ",";
                            }
                        }
                    }
                    return selectedItems.TrimEnd(',');
                }
            case "4":
                return chkValue.Checked.ToString();
            case "5":
                if (optionOperator == ValueOption.OptionOne)
                    return tlDateInputValue.DateInput.SelectedDate.Value.ToString();
                else
                    return "SelectedWithInOrNot";

        }
        return null;
    }
    private void SetDataTypeColumn(string filterDataType)
    {
        switch (filterDataType)
        {
            case "0":
                SetOperatorMode(OperatorMode.SixOperators);// For Numberic DataType
                break;
            case "1":
                SetOperatorMode(OperatorMode.TenOperators);// For Text DataType
                break;
            case "2":
                SetOperatorMode(OperatorMode.EightOperators);// For Date DataType
                break;
            case "3":
                SetOperatorMode(OperatorMode.FourOperators);// For Table DataType
                break;
            case "4":
                SetOperatorMode(OperatorMode.TwoOperators);// For Checkbox DataType
                break;
            case "5":
                SetOperatorMode(OperatorMode.EightOperators);// For DateTime DataType
                break;
            default:
                break;
        }
    }
    private void SetOperatorMode(OperatorMode opMode)
    {
        if (ddlOperators.Items.Count > 0)
            ddlOperators.Items.Clear();
        ddlOperators.Visible = true;
        switch (opMode)
        {
            case OperatorMode.TwoOperators:
                ddlOperators.Items.Add(new ListItem("Equal to", "0"));
                ddlOperators.Items.Add(new ListItem("Not Equal to", "1"));
                break;
            case OperatorMode.FourOperators:
                ddlOperators.Items.Add(new ListItem("Equal to", "0"));
                ddlOperators.Items.Add(new ListItem("Not Equal to", "1"));
                ddlOperators.Items.Add(new ListItem("Contains", "6"));
                ddlOperators.Items.Add(new ListItem("Does not contains", "7"));
                break;
            case OperatorMode.SixOperators:
                ddlOperators.Items.Add(new ListItem("Equal to", "0"));
                ddlOperators.Items.Add(new ListItem("Not Equal to", "1"));
                ddlOperators.Items.Add(new ListItem("Less than", "2"));
                ddlOperators.Items.Add(new ListItem("Less than or equal to", "3"));
                ddlOperators.Items.Add(new ListItem("Greater than", "4"));
                ddlOperators.Items.Add(new ListItem("Greater than or equal to", "5"));
                break;
            case OperatorMode.EightOperators:
                ddlOperators.Items.Add(new ListItem("Equal to", "0"));
                ddlOperators.Items.Add(new ListItem("Not Equal to", "1"));
                ddlOperators.Items.Add(new ListItem("Less than", "2"));
                ddlOperators.Items.Add(new ListItem("Less than or equal to", "3"));
                ddlOperators.Items.Add(new ListItem("Greater than", "4"));
                ddlOperators.Items.Add(new ListItem("Greater than or equal to", "5"));
                ddlOperators.Items.Add(new ListItem("Within", "8"));
                ddlOperators.Items.Add(new ListItem("Not Within", "9"));

                break;
            case OperatorMode.TenOperators:
                ddlOperators.Items.Add(new ListItem("Equal to", "0"));
                ddlOperators.Items.Add(new ListItem("Not Equal to", "1"));
                ddlOperators.Items.Add(new ListItem("Less than", "2"));
                ddlOperators.Items.Add(new ListItem("Less than or equal to", "3"));
                ddlOperators.Items.Add(new ListItem("Greater than", "4"));
                ddlOperators.Items.Add(new ListItem("Greater than or equal to", "5"));
                ddlOperators.Items.Add(new ListItem("Contains", "6"));
                ddlOperators.Items.Add(new ListItem("Does not contains", "7"));
                break;
            default:
                break;
        }
        ddlOperators.SelectedIndex = 0;
        ddlOperators_SelectedIndexChanged(this, null);
    }
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
    private string GetValueText(FilterArea nFilter, TagFields nTagFields)
    {
        if (nFilter.WithinSelect == true)
        {
            if (nFilter.WithinRadioButtonSelection == false)
            {
                ddlCriteria.SelectedValue = nFilter.WithinPredefined.ToString();
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
        else if (nTagFields.FilterDataType == 3)// For lookup table 
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

                    DataTable dtRecords = nTable.GetDatatable(ConnectionString, query);
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
    private void SetValueMode(ValueMode valMode)
    {
        switch (valMode)
        {
            case ValueMode.BitField:
                divCheckBox.Visible = true;
                divDate.Visible = false;
                divDateTime.Visible = false;
                divDateWithInorNotValue.Visible = false;
                divLookupTableValue.Visible = false;
                divLookupTableMultiSelect.Visible = false;
                divNumericValue.Visible = false;
                divTextValue.Visible = false;
                break;
            case ValueMode.LookupTableDropDown:
                divCheckBox.Visible = false;
                divDate.Visible = false;
                divDateTime.Visible = false;
                divDateWithInorNotValue.Visible = false;
                divLookupTableValue.Visible = true;
                divLookupTableMultiSelect.Visible = false;
                divNumericValue.Visible = false;
                divTextValue.Visible = false;
                break;
            case ValueMode.LookupTableMultiSelect:
                divCheckBox.Visible = false;
                divDate.Visible = false;
                divDateTime.Visible = false;
                divDateWithInorNotValue.Visible = false;
                divLookupTableValue.Visible = false;
                divLookupTableMultiSelect.Visible = true;
                divNumericValue.Visible = false;
                divTextValue.Visible = false;
                break;
            case ValueMode.NumericField:
                divCheckBox.Visible = false;
                divDate.Visible = false;
                divDateTime.Visible = false;
                divDateWithInorNotValue.Visible = false;
                divLookupTableValue.Visible = false;
                divLookupTableMultiSelect.Visible = false;
                divNumericValue.Visible = true;
                divTextValue.Visible = false;
                break;
            case ValueMode.DateOnlyField:
                divCheckBox.Visible = false;
                divDate.Visible = true;
                divDateTime.Visible = false;
                divDateWithInorNotValue.Visible = false;
                divLookupTableValue.Visible = false;
                divLookupTableMultiSelect.Visible = false;
                divNumericValue.Visible = false;
                divTextValue.Visible = false;
                break;
            case ValueMode.DateTimeField:
                divCheckBox.Visible = false;
                divDate.Visible = false;
                divDateTime.Visible = true;
                divDateWithInorNotValue.Visible = false;
                divLookupTableValue.Visible = false;
                divLookupTableMultiSelect.Visible = false;
                divNumericValue.Visible = false;
                divTextValue.Visible = false;
                break;
            case ValueMode.DateWithinOrNot:
                divCheckBox.Visible = false;
                divDate.Visible = false;
                divDateTime.Visible = false;
                divDateWithInorNotValue.Visible = true;
                divLookupTableValue.Visible = false;
                divLookupTableMultiSelect.Visible = false;
                divNumericValue.Visible = false;
                divTextValue.Visible = false;
                break;
            case ValueMode.TextField:
                divCheckBox.Visible = false;
                divDate.Visible = false;
                divDateTime.Visible = false;
                divDateWithInorNotValue.Visible = false;
                divLookupTableValue.Visible = false;
                divLookupTableMultiSelect.Visible = false;
                divNumericValue.Visible = false;
                divTextValue.Visible = true;
                break;
            default:
                break;
        }
    }
    private void SetValueModeWithOption(String valueField, ValueOption optionOperator = ValueOption.OptionOne)
    {
        switch (valueField)
        {
            case "0":
                SetValueMode(ValueMode.NumericField);
                break;
            case "1":
                SetValueMode(ValueMode.TextField);
                break;
            case "2":
                if (optionOperator == ValueOption.OptionOne)
                    SetValueMode(ValueMode.DateOnlyField);
                else
                    SetValueMode(ValueMode.DateWithinOrNot);
                break;
            case "3":
                if (optionOperator == ValueOption.OptionOne)
                    SetValueMode(ValueMode.LookupTableDropDown);
                else
                    SetValueMode(ValueMode.LookupTableMultiSelect);
                break;
            case "4":
                SetValueMode(ValueMode.BitField);
                break;
            case "5":
                if (optionOperator == ValueOption.OptionOne)
                    SetValueMode(ValueMode.DateTimeField);
                else
                    SetValueMode(ValueMode.DateWithinOrNot);
                break;
            default:
                break;
        }
    }
    public void BindEmailFilterGrid()
    {
        try
        {
            if (Parent_key == 0)
            {
                grdTemplateFilters.DataSource = null;
                grdTemplateFilters.DataBind();
                return;
            }
            //var addedFilters = Engine.FilterAreaActions.GetAll();
            //var tagFieldsColumns = Engine.TagFieldsActions.GetAll();

            //SZ [Jan 30, 2013] Join works fine in LINQ SQL commented out below
            // but I wanted a little experimentation with lambda join :) Duh!
            // Old King Cole was a merry old soul
            // And a merry old soul was he;
            // He called for his pipe, and he called for his bowl
            // And he called for his fiddlers three.



            //var result = (from x in addedFilters
            //              join y in tagFieldsColumns
            //              on x.FilteredColumnTagkey equals y.Id
            //              where x.ParentKey == Parent_key && x.ParentType == Parent_Type
            //              select new { key= x.Id, FilterText = y.Name + " " + GetOperatorText(x.Operator.ToString()) + " " + GetValueText(x,y), OrderNumber= x.OrderNumber }).OrderBy(m => m.OrderNumber);

            var result = Engine.FilterAreaActions.GetAll().Join(Engine.TagFieldsActions.GetAll(),
                a => a.FilteredColumnTagkey,
                b => b.Id,
                (a, b) => new { Filters = a, Columns = b })
                .Where(c => c.Filters.ParentKey == Parent_key && c.Filters.ParentType == Parent_Type)
                .Select(c => new
                {
                    key = c.Filters.Id,
                    FilterText = string.Format("[{0}] {1} {2}", c.Columns.Name, GetOperatorText(c.Filters.Operator.ToString()), GetValueText(c.Filters, c.Columns)),
                    OrderNumber = c.Filters.OrderNumber
                }).OrderBy(x => x.OrderNumber);

            grdTemplateFilters.DataSource = result;
            grdTemplateFilters.DataBind();

        }
        catch (Exception ex)
        {
            ctrlStatusLabel.SetStatus(ex);
        }
    }

    public Object GetFilters(short parentType)
    {
        var result = Engine.FilterAreaActions.GetAll().Join(Engine.TagFieldsActions.GetAll(),
            a => a.FilteredColumnTagkey,
            b => b.Id,
            (a, b) => new { Filters = a, Columns = b })
            .Where(c => c.Filters.ParentType == parentType)
            .Select(c => new
            {
                key = c.Columns.Id,
                FilterText = string.Format("[{0}] {1} {2}", c.Columns.Name, GetOperatorText(c.Filters.Operator.ToString()), GetValueText(c.Filters, c.Columns)),
                OrderNumber = c.Filters.OrderNumber
            }).OrderBy(x => x.OrderNumber);
        return result;
    }

    public void BindFilterFields()
    {
        try
        {
            if (ddlFieldsColumn.Items.Count > 0)
                ddlFieldsColumn.Items.Clear();
            ddlFieldsColumn.Items.Add(new RadComboBoxItem("--Select Column---", "-1"));

            var separators = Engine.TagFieldsActions.GetAll().Select(m => new { m.Group }).Distinct();
            //Add items in the Item collection of RadComboBox.
            foreach (var titleGroup in separators)
            {
                RadComboBoxItem itemTitle = new RadComboBoxItem();
                itemTitle.Text = titleGroup.Group.ToString();
                itemTitle.IsSeparator = true;
                
                IEnumerable<TagFields> U = null;
                if (IsTagWorkFlow)
                    U = Engine.TagFieldsActions.GetAll().Where(x => x.IsWorkflowIncluded);
                else if (IsCustomReport)
                    U = Engine.TagFieldsActions.GetAllReportTagsByBaseDataID(ReportBaseDataID);
                else if (IsDuplicateCheck)
                    U = Engine.TagFieldsActions.GetAllReportTagsByBaseDataID(ReportBaseDataID);
                else
                    U = Engine.TagFieldsActions.GetAllApplicationFieldTags();
                    //U = Engine.TagFieldsActions.GetAllByInstanceType((short)ApplicationSettings.InsuranceType);
                           
                var groupColumns = U.Select(k => new { Key = k.Id, Name = k.Name, k.Group, k.IsAnyPhoneNumber }).Where(m => m.Group == titleGroup.Group && !m.IsAnyPhoneNumber.Value).OrderBy(m => m.Name);
                if (groupColumns.Count() > 0)
                {
                    ddlFieldsColumn.Items.Add(itemTitle);
                    btnFilterAdd.Enabled = true;
                    ddlFieldsColumn.DataSource = groupColumns;
                    ddlFieldsColumn.DataBind();
                }
            }
        }
        catch (Exception ex)
        {
            ctrlStatusLabel.SetStatus(ex);
        }
    }

    private void BindLookupTable(int tableKey)
    {
        try
        {
            var lookupTable = Engine.SQTablesActions.GetAll().OrderBy(m => m.Name).Where(l => l.Id == tableKey).FirstOrDefault();

            TableStructure nTable = new TableStructure();
            string query = "select " + lookupTable.KeyFieldName + " as [Key], " + lookupTable.TitleFieldName + " as Title from " + lookupTable.SystemTableName + " Order by " + lookupTable.TitleFieldName;

            DataTable dtRecords = nTable.GetDatatable(ConnectionString, query);
            string searchString = "Unassigned";
            string title = string.Empty;
            
            title = (from DataRow dr in dtRecords.Rows
                      where dr["Title"].ToString().Contains(searchString)
                      select (string)dr["Title"]).FirstOrDefault();
            if (ddlLookupTableValue.Items.Count > 0)
                ddlLookupTableValue.Items.Clear();
            ddlLookupTableValue.AppendDataBoundItems = true;
            if(string.IsNullOrEmpty(title))
                ddlLookupTableValue.Items.Add(new ListItem("--Unassigned--", ""));            
            ddlLookupTableValue.DataSource = dtRecords;
            ddlLookupTableValue.DataBind();
            ddlLookupTableValue.AppendDataBoundItems = false;

            if (lstBoxLookupMultitext.Items.Count > 0)
                lstBoxLookupMultitext.Items.Clear();
            lstBoxLookupMultitext.AppendDataBoundItems = true;
            //lstBoxLookupMultitext.Items.Add(new ListItem("--Unassigned--", ""));
            lstBoxLookupMultitext.DataSource = dtRecords;
            lstBoxLookupMultitext.DataBind();
            lstBoxLookupMultitext.AppendDataBoundItems = false;
        }
        catch (Exception ex)
        {
            ctrlStatusLabel.SetStatus(ex);
        }
    }
    private void SetControlMode(ControlDisplayMode mode)
    {
        switch (mode)
        {
            case ControlDisplayMode.AddFilter:
                hdnFieldIsEditMode.Value = "no";
                hdnFieldEditFilterKey.Value = "";
                btnCancelFilter.Visible = false;
                btnUpdateFilter.Visible = false;
                btnFilterAdd.Visible = true;
                ClearFilter();
                break;
            case ControlDisplayMode.EditFilter:
                btnCancelFilter.Visible = true;
                btnUpdateFilter.Visible = true;
                btnFilterAdd.Visible = false;
                break;
        }
    }
    public void SetControlModeFromOutside(bool IsAddMode)
    {
        if (IsAddMode)
        {
            SetControlMode(ControlDisplayMode.AddFilter);
        }
        else
        {
            SetControlMode(ControlDisplayMode.EditFilter);
        }
    }
    private void LoadEditFilterValues(int filterkey)
    {
        try
        {
            FilterArea nFilter = Engine.FilterAreaActions.Get(filterkey);

            int operatorField = nFilter.Operator;

            int filterColumnTagKey = nFilter.FilteredColumnTagkey;
            ddlFieldsColumn.SelectedValue = filterColumnTagKey.ToString();
            ddlFieldsColumn_SelectedIndexChanged(this, null);

            TagFields nTagField = Engine.TagFieldsActions.Get(filterColumnTagKey);
            int filterDataType = nTagField.FilterDataType;
            hdnFieldSelectedDataType.Value = filterDataType.ToString();
            SetDataTypeColumn(hdnFieldSelectedDataType.Value);

            ddlOperators.SelectedValue = nFilter.Operator.ToString();
            ddlOperators_SelectedIndexChanged(this, null);
            string filterValue = String.Empty;
            if (operatorField == 6 || operatorField == 7 || operatorField == 8 || operatorField == 9)
            {
                SetFilterValue(hdnFieldSelectedDataType.Value, nFilter.Value, ValueOption.OptionTwo);
            }
            else
            {
                SetFilterValue(hdnFieldSelectedDataType.Value, nFilter.Value, ValueOption.OptionOne);
            }

            if (nFilter.WithinSelect == true)
            {
                if (nFilter.WithinRadioButtonSelection == false)
                {
                    rdBtnCriteriaWithinOrNotFirst.Checked = true;
                    ddlCriteria.SelectedValue = nFilter.WithinPredefined.ToString();
                }
                else
                {
                    rdCriteriaWithinOrNotSecond.Checked = true;
                    ddlLastNext.SelectedValue = nFilter.WithinLastNext == false ? "0" : "1";
                    ddlDayHourMin.SelectedValue = nFilter.WithinLastNextUnit.ToString();
                    txtDayHourMin.Text = nFilter.Value;
                }
            }

        }
        catch (Exception ex)
        {
            ctrlStatusLabel.SetStatus(ex);
        }
    }
    public bool CheckOrderNumberValues(List<string> listOperands)
    {
        try
        {
            int orderNumberGrid = GridMaxOrderNumber();
            foreach (string item in listOperands)
            {
                if (grdTemplateFilters.Rows.Count == 0)
                {
                    return false;
                }
                int orderNumberOperand = 0;
                bool resultOperandNumber = int.TryParse(item, out orderNumberOperand);
                if (resultOperandNumber == false || orderNumberOperand > orderNumberGrid || orderNumberOperand < 1)
                {
                    return false;
                }
            }
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
    private int GridMaxOrderNumber()
    {
        int maxOrderNumber = 0;
        for (int i = 0; i < grdTemplateFilters.Rows.Count; i++)
        {
            int orderNumberGrid = 0;
            bool resultGridNumber = int.TryParse(grdTemplateFilters.Rows[i].Cells[2].Text, out orderNumberGrid);
            if (orderNumberGrid > maxOrderNumber)
            {
                maxOrderNumber = orderNumberGrid;
            }
        }
        return maxOrderNumber;
    }
    public void InitializeFilter()
    {
        IsTagWorkFlow = false;
        IsCustomReport = false;
        IsDuplicateCheck = false;
        ddlOperators.Visible = false;
        divCheckBox.Visible = false;
        divDate.Visible = false;
        divDateTime.Visible = false;
        divDateWithInorNotValue.Visible = false;
        divLookupTableValue.Visible = false;
        divLookupTableMultiSelect.Visible = false;
        divNumericValue.Visible = false;
        divTextValue.Visible = false;

        Parent_key = 0;
        SetControlModeFromOutside(true);
        BindFilterFields();
        BindEmailFilterGrid();
    }

    public bool HasRecords
    {
        get
        {
            return
                Engine.FilterAreaActions.GetAll().Join(Engine.TagFieldsActions.GetAll(),
                           a => a.FilteredColumnTagkey,
                           b => b.Id,
                           (a, b) => new { Filters = a, Columns = b })
                           .Where(c => c.Filters.ParentKey == Parent_key && c.Filters.ParentType == Parent_Type)
                           .Count() > 0;
        }
    }
    //SZ [Feb 28, 2013] function has been added to reduce the function calls made by the client
    public void InitializeFilter(FilterParentType type, int recordID, bool bControlFromOutside = true)
    {
        ParentType = type;
        Parent_key = recordID;
        SetControlModeFromOutside(bControlFromOutside);
        BindFilterFields();
        BindEmailFilterGrid();
    }
    #endregion

    #region Events

    protected void btnFilterAdd_Click(object sender, EventArgs e)
    {
        try
        {
            if (Parent_key == 0)
            {
                return;
            }
            FilterArea nFilter = new FilterArea();
            string filterValue = String.Empty;
            if (ddlOperators.SelectedValue == "6" || ddlOperators.SelectedValue == "7" || ddlOperators.SelectedValue == "8" || ddlOperators.SelectedValue == "9")
            {
                filterValue = GetFilterValue(hdnFieldSelectedDataType.Value, ValueOption.OptionTwo);
            }
            else
            {
                filterValue = GetFilterValue(hdnFieldSelectedDataType.Value, ValueOption.OptionOne);
            }
            if (filterValue == "SelectedWithInOrNot")
            {
                if (rdBtnCriteriaWithinOrNotFirst.Checked)
                {
                    nFilter.WithinSelect = true;
                    nFilter.WithinRadioButtonSelection = false;
                    nFilter.WithinPredefined = Convert.ToInt16(ddlCriteria.SelectedValue);
                }
                else
                {
                    nFilter.WithinSelect = true;
                    nFilter.WithinRadioButtonSelection = true;
                    nFilter.WithinLastNext = ddlLastNext.SelectedValue == "0" ? false : true;
                    nFilter.WithinLastNextUnit = Convert.ToInt32(ddlDayHourMin.SelectedValue);
                    filterValue = txtDayHourMin.Text;
                }
            }
            else
                nFilter.WithinSelect = false;
            nFilter.ParentKey = Parent_key;
            nFilter.ParentType = Parent_Type;
            nFilter.FilteredColumnTagkey = Convert.ToInt32(ddlFieldsColumn.SelectedValue);
            nFilter.Operator = Convert.ToByte(ddlOperators.SelectedValue);
            nFilter.Value = filterValue;

            nFilter.Added.By = AddedBy;
            nFilter.Added.On = DateTime.Now;

            int nOrderNumber = 0;
            try
            {
                nOrderNumber = Engine.FilterAreaActions.GetMaxOrderNumber(Parent_key, Parent_Type);
            }
            catch (Exception)
            {
                nOrderNumber = 0;
            }
            nFilter.OrderNumber = nOrderNumber + 1;
            Engine.FilterAreaActions.Add(nFilter);
            BindEmailFilterGrid();

            ClearFilter();
        }
        catch (Exception ex)
        {
            ctrlStatusLabel.SetStatus(ex);
        }
    }
    protected void ddlFieldsColumn_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
    {
        if (ddlFieldsColumn.SelectedIndex > 0)
        {
            var fieldColumnTagTableRecord = Engine.TagFieldsActions.Get(Convert.ToInt32(ddlFieldsColumn.SelectedValue));
            hdnFieldSelectedDataType.Value = fieldColumnTagTableRecord.FilterDataType.ToString();
            SetDataTypeColumn(fieldColumnTagTableRecord.FilterDataType.ToString());
            if (fieldColumnTagTableRecord.FilterDataType == 3)
            {
                BindLookupTable(fieldColumnTagTableRecord.TableKey.Value);
            }
        }
        else
        {
            InitializeFilter();
        }
    }
    protected void ddlOperators_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (ddlOperators.SelectedValue == "6" || ddlOperators.SelectedValue == "7" || ddlOperators.SelectedValue == "8" || ddlOperators.SelectedValue == "9")
        {
            SetValueModeWithOption(hdnFieldSelectedDataType.Value, ValueOption.OptionTwo);
        }
        else
        {
            SetValueModeWithOption(hdnFieldSelectedDataType.Value, ValueOption.OptionOne);
        }
    }
    protected void grdEmailFilters_RowCommand(object sender, GridViewCommandEventArgs e)
    {
        try
        {
            if (e.CommandName == "EditX")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                String dataKeyValue = grdTemplateFilters.DataKeys[row.RowIndex].Value.ToString();
                hdnFieldEditFilterKey.Value = dataKeyValue;
                hdnFieldIsEditMode.Value = "yes";
                SetControlMode(ControlDisplayMode.EditFilter);
                LoadEditFilterValues(Convert.ToInt32(dataKeyValue));
                //BindEmailFilterGrid();
            }
            else if (e.CommandName == "DeleteX")
            {
                GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
                String dataKeyValue = grdTemplateFilters.DataKeys[row.RowIndex].Value.ToString();
                string strGVOrderNumber = grdTemplateFilters.Rows[row.RowIndex].Cells[2].Text;
                Engine.FilterAreaActions.Delete(Convert.ToInt32(dataKeyValue));
                ctrlStatusLabel.SetStatus("Record delete successful.");
                TableStructure nTable = new TableStructure();
                nTable.UpdateFilterOrderNumbers(ConnectionString, Convert.ToInt32(strGVOrderNumber), Parent_key, Parent_Type);
                BindEmailFilterGrid();
                ClearFilter();
            }
        }
        catch (Exception ex)
        {
            ctrlStatusLabel.SetStatus(ex);
        }
    }
    protected void btnCancelFilter_Click(object sender, EventArgs e)
    {
        SetControlMode(ControlDisplayMode.AddFilter);
    }
    protected void btnUpdateFilter_Click(object sender, EventArgs e)
    {
        try
        {
            FilterArea nFilter = Engine.FilterAreaActions.Get(Convert.ToInt32(hdnFieldEditFilterKey.Value));
            string filterValue = String.Empty;
            if (ddlOperators.SelectedValue == "6" || ddlOperators.SelectedValue == "7" || ddlOperators.SelectedValue == "8" || ddlOperators.SelectedValue == "9")
            {
                filterValue = GetFilterValue(hdnFieldSelectedDataType.Value, ValueOption.OptionTwo);
            }
            else
            {
                filterValue = GetFilterValue(hdnFieldSelectedDataType.Value, ValueOption.OptionOne);
            }
            if (filterValue == "SelectedWithInOrNot")
            {
                if (rdBtnCriteriaWithinOrNotFirst.Checked)
                {
                    nFilter.WithinSelect = true;
                    nFilter.WithinRadioButtonSelection = false;
                    nFilter.WithinPredefined = Convert.ToInt16(ddlCriteria.SelectedValue);
                }
                else
                {
                    nFilter.WithinSelect = true;
                    nFilter.WithinRadioButtonSelection = true;
                    nFilter.WithinLastNext = ddlLastNext.SelectedValue == "0" ? false : true;
                    nFilter.WithinLastNextUnit = Convert.ToInt32(ddlDayHourMin.SelectedValue);
                    filterValue = txtDayHourMin.Text;
                }
            }
            else
                nFilter.WithinSelect = false;
            nFilter.ParentKey = Parent_key;
            nFilter.ParentType = Parent_Type;
            nFilter.FilteredColumnTagkey = Convert.ToInt32(ddlFieldsColumn.SelectedValue);
            nFilter.Operator = Convert.ToByte(ddlOperators.SelectedValue);
            nFilter.Value = filterValue;

            nFilter.Changed.By = ChangedBy;
            nFilter.Changed.On = DateTime.Now;

            Engine.FilterAreaActions.Change(nFilter);
            BindEmailFilterGrid();
            ClearFilter();
            SetControlMode(ControlDisplayMode.AddFilter);
        }
        catch (Exception ex)
        {
            ctrlStatusLabel.SetStatus(ex);
        }
    }

    #endregion


}
