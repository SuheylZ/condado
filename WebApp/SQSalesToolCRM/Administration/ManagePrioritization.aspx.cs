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


public partial class Admin_ManagePrioritizartion : SalesBasePage
{


    #region Members/Properties

    enum PageDisplayMode { GridQueueTemplate = 1, EditQueueTemplate = 2 }

    public string EditKey
    {
        get
        {
            return hdnFieldEditTemplateKey.Value;
        }
        set
        {
            hdnFieldEditTemplateKey.Value = value;
        }
    }
    public string IsEditMode
    {
        get
        {
            return hdnFieldIsEditMode.Value;
        }
        set
        {
            hdnFieldIsEditMode.Value = value;
        }
    }
    public string IsCopyMode
    {
        get
        {
            return hdnFieldIsCopyMode.Value;
        }
        set
        {
            hdnFieldIsCopyMode.Value = value;
        }
    }
    public int totalGridRecords = 0;
    public short parentType = 2; // For prioritization
    byte? UserTypeOnForm
    {
        get { return ddlUserType.SelectedValue == "-1" ? (byte?)null : Helper.NullConvert<byte>(ddlUserType.SelectedValue); }
        set { if (value == null) ddlUserType.SelectedValue = "-1"; else ddlUserType.SelectedValue=(value >= 1 && value <= 5) ? value.Value.ToString() : "-1"; }
    }
    byte FilterByUserType
    {
        get
        {
            byte i = Helper.SafeConvert<byte>(ddlFilterByUserType.SelectedValue);
            return i;
        }
        set
        {
            ddlFilterByUserType.SelectedValue = value.ToString();
        }
    }
    #endregion



    #region Methods
    private void SetPageMode(PageDisplayMode mode)
    {
        switch (mode)
        {
            case PageDisplayMode.GridQueueTemplate:
                divForm.Visible = false;
                divGrid.Visible = true;
                IsEditMode = "no";
                EditKey = "";
                break;
            case PageDisplayMode.EditQueueTemplate:
                divForm.Visible = true;
                divGrid.Visible = false;
                tlMultiPage.SelectedIndex = 0;
                tlPrioritizationStrip.SelectedIndex = 0;
                tlMultiPage.PageViews[1].Visible = false;
                tlMultiPage.PageViews[2].Visible = false;
                ctrlShiftSchedule.Visible = true;
                break;
            default:
                break;
        }
    }
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
    public bool SaveRecord(bool ConvertToEditMode = false)
    {
        try
        {

            short selectedFilter = Convert.ToInt16(rdBtnlstFilterSelection.SelectedValue);
            string customFilterValue = "";
            bool hasCustomFilterError = false;
            if (rdBtnlstFilterSelection.SelectedValue == "2")
            {
                hasCustomFilterError = !CheckForValidCustomString();

                customFilterValue = txtCustomFilter.Text;
            }
            if (hasCustomFilterError == true)
            {
                lblMessageForm.SetStatus(ErrorMessages.CustomFilterValueError);
                return false;
            }
            if (UserTypeOnForm==null)
                throw new Exception(ErrorMessages.PrioritizationUserTypeRequired);

            if (IsEditMode == "no")
            {
                if (Engine.LeadPrioritizationActions.TitleExists(txtTitle.Text))
                {
                    lblMessageForm.SetStatus(ErrorMessages.SameTitleError);
                    return false;
                }
                var recordAdded = Engine.LeadPrioritizationActions.Add(txtTitle.Text, txtDescription.Text, chkEnabled.Checked, CurrentUser.Email, selectedFilter, customFilterValue);

                AddSchedules(recordAdded.Id);
                if (ConvertToEditMode)
                {
                    EditKey = recordAdded.Id.ToString();
                    IsEditMode = "yes";
                    ManageFiltersControl.Parent_key = recordAdded.Id;
                    
                    tlPrioritizationStrip.Tabs[1].Enabled = true;
                    tlPrioritizationStrip.Tabs[2].Enabled = true;
                    
                    //tlPageFilters.Enabled = true;
                    //tlPageSchedule.Enabled = true;
                }
            }
            else if (IsEditMode == "yes")
            {
                if (EditKey != "")
                {
                    LeadPrioritizationRules nPriorityRule = Engine.LeadPrioritizationActions.Get(Convert.ToInt32(EditKey));
                    nPriorityRule.Title = txtTitle.Text;
                    nPriorityRule.Description = txtDescription.Text;
                    nPriorityRule.IsActive = chkEnabled.Checked;
                    nPriorityRule.FilterSelection = selectedFilter;
                    nPriorityRule.FilterCustomValue = customFilterValue;
                    nPriorityRule.UserType = UserTypeOnForm;

                    Engine.LeadPrioritizationActions.Change(nPriorityRule, CurrentUser.FullName);
                    Engine.LeadPrioritizationActions.GetDetails(nPriorityRule.Id).DeleteAll();
                    AddSchedules(nPriorityRule.Id);
                }

            }
            RunDynamicQueryParser();
            lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
            return true;
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
            return false;
        }
    }
    
    void AddSchedules(int recordId)
    {
        //YA[03 April 2014] UnCommented the following code.
        //Commented on John's request 18-feb-2014
        
        //WM - 29.05.2013
        bool visible = ctrlShiftSchedule.Visible;
        ctrlShiftSchedule.Visible = true;

        //Detail schedule for Sunday
        TimeSpan? SundayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Sunday, true);
        TimeSpan? SundayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Sunday, false);
        TimeSpan? SundayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Sunday, true);
        TimeSpan? SundayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Sunday, false);
        TimeSpan? SundayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Sunday, true);
        TimeSpan? SundayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Sunday, false);

        if (SundayS1StartTime != null /*&& SundayS1StartTime < SundayS1EndTime//YA[08 April 2014] Could be enabled for all start and end time if required*/)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Sunday, Convert.ToDateTime(SundayS1StartTime.ToString()), Convert.ToDateTime(SundayS1EndTime.ToString()));
        }
        //else
        //    Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Sunday, Convert.ToDateTime(DateTime.Now.Date.ToString() + " 00:00:01 AM"), Convert.ToDateTime(" 11:59:59 PM"));
        if (SundayS2StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Sunday, Convert.ToDateTime(SundayS2StartTime.ToString()), Convert.ToDateTime(SundayS2EndTime.ToString()));
        }
        if (SundayS3StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Sunday, Convert.ToDateTime(SundayS3StartTime.ToString()), Convert.ToDateTime(SundayS3EndTime.ToString()));
        }

        //Detail schedule for Monday
        TimeSpan? MondayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Monday, true);
        TimeSpan? MondayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Monday, false);
        TimeSpan? MondayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Monday, true);
        TimeSpan? MondayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Monday, false);
        TimeSpan? MondayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Monday, true);
        TimeSpan? MondayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Monday, false);

        if (MondayS1StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Monday, Convert.ToDateTime(MondayS1StartTime.ToString()), Convert.ToDateTime(MondayS1EndTime.ToString()));
        }
        if (MondayS2StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Monday, Convert.ToDateTime(MondayS2StartTime.ToString()), Convert.ToDateTime(MondayS2EndTime.ToString()));
        }
        if (MondayS3StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Monday, Convert.ToDateTime(MondayS3StartTime.ToString()), Convert.ToDateTime(MondayS3EndTime.ToString()));
        }

        //Detail schedule for Tuesday
        TimeSpan? TuesdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Tuesday, true);
        TimeSpan? TuesdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Tuesday, false);
        TimeSpan? TuesdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Tuesday, true);
        TimeSpan? TuesdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Tuesday, false);
        TimeSpan? TuesdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Tuesday, true);
        TimeSpan? TuesdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Tuesday, false);

        if (TuesdayS1StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Tuesday, Convert.ToDateTime(TuesdayS1StartTime.ToString()), Convert.ToDateTime(TuesdayS1EndTime.ToString()));
        }
        if (TuesdayS2StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Tuesday, Convert.ToDateTime(TuesdayS2StartTime.ToString()), Convert.ToDateTime(TuesdayS2EndTime.ToString()));
        }
        if (TuesdayS3StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Tuesday, Convert.ToDateTime(TuesdayS3StartTime.ToString()), Convert.ToDateTime(TuesdayS3EndTime.ToString()));
        }
        //Detail schedule for Wednesday
        TimeSpan? WednesdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Wednesday, true);
        TimeSpan? WednesdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Wednesday, false);
        TimeSpan? WednesdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Wednesday, true);
        TimeSpan? WednesdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Wednesday, false);
        TimeSpan? WednesdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Wednesday, true);
        TimeSpan? WednesdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Wednesday, false);

        if (WednesdayS1StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Wednesday, Convert.ToDateTime(WednesdayS1StartTime.ToString()), Convert.ToDateTime(WednesdayS1EndTime.ToString()));
        }
        if (WednesdayS2StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Wednesday, Convert.ToDateTime(WednesdayS2StartTime.ToString()), Convert.ToDateTime(WednesdayS2EndTime.ToString()));
        }
        if (WednesdayS3StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Wednesday, Convert.ToDateTime(WednesdayS3StartTime.ToString()), Convert.ToDateTime(WednesdayS3EndTime.ToString()));
        }
        //Detail schedule for Thursday
        TimeSpan? ThursdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Thursday, true);
        TimeSpan? ThursdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Thursday, false);
        TimeSpan? ThursdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Thursday, true);
        TimeSpan? ThursdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Thursday, false);
        TimeSpan? ThursdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Thursday, true);
        TimeSpan? ThursdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Thursday, false);

        if (ThursdayS1StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Thursday, Convert.ToDateTime(ThursdayS1StartTime.ToString()), Convert.ToDateTime(ThursdayS1EndTime.ToString()));
        }
        if (ThursdayS2StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Thursday, Convert.ToDateTime(ThursdayS2StartTime.ToString()), Convert.ToDateTime(ThursdayS2EndTime.ToString()));
        }
        if (ThursdayS3StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Thursday, Convert.ToDateTime(ThursdayS3StartTime.ToString()), Convert.ToDateTime(ThursdayS3EndTime.ToString()));
        }
        //Detail schedule for Friday
        TimeSpan? FridayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Friday, true);
        TimeSpan? FridayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Friday, false);
        TimeSpan? FridayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Friday, true);
        TimeSpan? FridayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Friday, false);
        TimeSpan? FridayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Friday, true);
        TimeSpan? FridayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Friday, false);

        if (FridayS1StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Friday, Convert.ToDateTime(FridayS1StartTime.ToString()), Convert.ToDateTime(FridayS1EndTime.ToString()));
        }
        if (FridayS2StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Friday, Convert.ToDateTime(FridayS2StartTime.ToString()), Convert.ToDateTime(FridayS2EndTime.ToString()));
        }
        if (FridayS3StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Friday, Convert.ToDateTime(FridayS3StartTime.ToString()), Convert.ToDateTime(FridayS3EndTime.ToString()));
        }
        //Detail schedule for Saturday
        TimeSpan? SaturdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Saturday, true);
        TimeSpan? SaturdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Saturday, false);
        TimeSpan? SaturdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Saturday, true);
        TimeSpan? SaturdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Saturday, false);
        TimeSpan? SaturdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Saturday, true);
        TimeSpan? SaturdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Saturday, false);

        if (SaturdayS1StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Saturday, Convert.ToDateTime(SaturdayS1StartTime.ToString()), Convert.ToDateTime(SaturdayS1EndTime.ToString()));
        }
        if (SaturdayS2StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Saturday, Convert.ToDateTime(SaturdayS2StartTime.ToString()), Convert.ToDateTime(SaturdayS2EndTime.ToString()));
        }
        if (SaturdayS3StartTime != null)
        {
            Engine.LeadPrioritizationActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Saturday, Convert.ToDateTime(SaturdayS3StartTime.ToString()), Convert.ToDateTime(SaturdayS3EndTime.ToString()));
        }

        //WM - 29.05.2013
        ctrlShiftSchedule.Visible = visible;
    }
    void BindGrid()
    {
        var records = FilterByUserType <= 0 ?
            Helper.SortRecords(Engine.LeadPrioritizationActions.All, "Priority", false) :
            Helper.SortRecords(Engine.LeadPrioritizationActions.All.Where(x =>(x.UserType??0)== FilterByUserType), "Priority", false);
        totalGridRecords = records.Count();
        grdPrioritization.DataSource = records;
        grdPrioritization.DataBind();
        
    }

    public static string GetUserType(object it)
    {
        int itype = Helper.SafeConvert<byte>((it ?? "0").ToString());
        string sAns = string.Empty;

        switch(itype){
            case 1: sAns = "Assigned User"; break;
            case 2: sAns = "CSR User"; break;
            case 3: sAns = "TA User"; break;
            case 4: sAns = "AP User"; break;
            case 5: sAns = "OB User"; break;
            default:
                break;
        }
        return sAns;
    }
    public void ClearFields()
    {
        txtTitle.Text = "";
        txtDescription.Text = "";
        chkEnabled.Checked = false;
        txtCustomFilter.Text = "";
        rdBtnlstFilterSelection.SelectedIndex = 0;
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);

        tlPrioritizationStrip.Tabs[1].Enabled = false;
        tlPrioritizationStrip.Tabs[2].Enabled = false;
        //tlPageFilters.Enabled = false;
        //tlPageSchedule.Enabled = false;
        ctrlShiftSchedule.ClearAllValues();
    }
    public void LoadEditFormValues(int recordId)
    {
        LeadPrioritizationRules nPriorityRule = Engine.LeadPrioritizationActions.Get(recordId);
        txtTitle.Text = nPriorityRule.Title;
        txtDescription.Text = nPriorityRule.Description;
        chkEnabled.Checked = nPriorityRule.IsActive;
        UserTypeOnForm = nPriorityRule.UserType;

        rdBtnlstFilterSelection.SelectedValue = nPriorityRule.FilterSelection == null ? "0" : nPriorityRule.FilterSelection.ToString();
        if (nPriorityRule.FilterSelection == 2)
        {
            txtCustomFilter.Text = nPriorityRule.FilterCustomValue;
        }
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);

        foreach (var itemDetail in Engine.LeadPrioritizationActions.GetDetails(nPriorityRule.Id).All)
        {
            ctrlShiftSchedule.SetTime(itemDetail.Shift, itemDetail.WeekDay, true, new TimeSpan(itemDetail.Working.Starts.Hour, itemDetail.Working.Starts.Minute, itemDetail.Working.Starts.Second));
            ctrlShiftSchedule.SetTime(itemDetail.Shift, itemDetail.WeekDay, false, new TimeSpan(itemDetail.Working.Ends.Hour, itemDetail.Working.Ends.Minute, itemDetail.Working.Ends.Second));
        }
    }
    #endregion

    #region Events

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            //SZ [Jan 10, 2013] used enum
            //ManageFiltersControl.Parent_Type = parentType;// For prioritization
            ManageFiltersControl.ParentType = FilterParentType.PrioritizationWebForm;

            ManageFiltersControl.AddedBy = CurrentUser.FullName;
            ManageFiltersControl.ChangedBy = CurrentUser.FullName;

            SetPageMode(PageDisplayMode.GridQueueTemplate);
            BindGrid();
        }
        lblMessageForm.SetStatus("");
        lblMessageGrid.SetStatus("");
        ctrlStatusCustomFilter.SetStatus("");
        ddlFilterByUserType.SelectedIndexChanged += (o, a) => BindGrid(); 
    }
    protected void btnAddNewQueue_Click(object sender, EventArgs e)
    {
        IsEditMode = "no";
        SetPageMode(PageDisplayMode.EditQueueTemplate);
        ClearFields();
        ManageFiltersControl.Parent_key = 0;
        ManageFiltersControl.BindEmailFilterGrid();
        ManageFiltersControl.SetControlModeFromOutside(true);
    }
    protected void btnPVRefresh_Click(object sender, EventArgs e)
    {
       
        var nQueryParser = new QueryParser();
        nQueryParser.ExecuteManagePrioritizationSp();
        nQueryParser.Dispose();
        BindGrid();


    }

    protected void rdBtnlstFilterSelection_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (rdBtnlstFilterSelection.SelectedValue == "2")//Custom filter selection option selected.
        {
            txtCustomFilter.Enabled = true;
        }
        else
        {
            txtCustomFilter.Enabled = false;
        }
    }
    protected void txtCustomFilter_TextChanged(object sender, EventArgs e)
    {
        CheckForValidCustomString();
    }
    protected void btnApply_Click(object sender, EventArgs e)
    {
        try
        {
            SaveRecord(true);
            IsCopyMode = "";
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
    }
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {
            bool hasSavedRecordSuccessful = SaveRecord();
            if (hasSavedRecordSuccessful)
            {
                SetPageMode(PageDisplayMode.GridQueueTemplate);
                BindGrid();
                ClearFields();
                IsCopyMode = "";
            }
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
    }
    protected void btnCancelOnForm_Click(object sender, EventArgs e)
    {
        if (IsCopyMode == "yes")
        {
            Engine.LeadPrioritizationActions.Delete(Convert.ToInt32(EditKey), parentType);
            IsCopyMode = "";
        }
        SetPageMode(PageDisplayMode.GridQueueTemplate);
        BindGrid();
    }
    protected void grdPrioritization_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            int count = grdPrioritization.Items.Count;
            GridDataItem item = (GridDataItem)e.Item;
            int index = item.ItemIndex;
            if (index == 0)
            {
                ImageButton imgBtnUpOrder = (ImageButton)item.FindControl("imgBtnUpOrder");
                imgBtnUpOrder.Visible = false;
            }
            if (totalGridRecords - 1 == index)
            {
                ImageButton imgBtnDownOrder = (ImageButton)item.FindControl("imgBtnDownOrder");
                imgBtnDownOrder.Visible = false;
            }

            // SZ [Aug, 2014] handle the user right delete
            {
                var ctl = e.Item.FindControl("lnkDelete") as LinkButton;
                var lbl = e.Item.FindControl("lblSepDel") as Label;
                if (ctl != null && !CurrentUser.Security.Administration.CanDelete)
                {
                    ctl.Visible = false;
                    if(lbl!=null) 
                        lbl.Visible = false;
                }
            }

        }
    }
    protected void grdPrioritization_RowDrop(object sender, Telerik.Web.UI.GridDragDropEventArgs e)
    {
        if (string.IsNullOrEmpty(e.HtmlElement))
        {
            if (e.DraggedItems[0].OwnerGridID == grdPrioritization.ClientID)
            {
                int destPriorityId = (int)e.DestDataItem.GetDataKeyValue("Priority");
                foreach (GridDataItem draggedItem in e.DraggedItems)
                {
                    int recordId = (int)draggedItem.GetDataKeyValue("Id");
                    if (e.DropPosition == GridItemDropPosition.Below)
                    {
                        Engine.LeadPrioritizationActions.Move(recordId, destPriorityId - 1);
                    }
                    else
                        Engine.LeadPrioritizationActions.Move(recordId, destPriorityId);
                    lblMessageGrid.SetStatus(Messages.RecordMovedSuccess);
                    BindGrid();
                    //YA[April 22, 2013] Run the dynamic query parser to update the SP for PL.
                    RunDynamicQueryParser();
                }
            }
        }
    }
    protected void grdPrioritization_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        if (e.CommandName == "EditX")
        {

            String dataKeyValue = e.CommandArgument.ToString();
            EditKey = dataKeyValue;
            ManageFiltersControl.Parent_key = Convert.ToInt32(dataKeyValue);
            ManageFiltersControl.SetControlModeFromOutside(true);
            ManageFiltersControl.BindEmailFilterGrid();
            IsEditMode = "yes";
            SetPageMode(PageDisplayMode.EditQueueTemplate);
            LoadEditFormValues(Convert.ToInt32(dataKeyValue));
            BindGrid();
            tlPrioritizationStrip.Tabs[1].Enabled = true;
            tlPrioritizationStrip.Tabs[2].Enabled = true;
            //tlPageFilters.Enabled = true;
            //tlPageSchedule.Enabled = true;
            return;
        }
        else if (e.CommandName == "CopyX")
        {
            String dataKeyValue = e.CommandArgument.ToString();
            LeadPrioritizationRules nCopyPriroritizationRule = Engine.LeadPrioritizationActions.Copy(Convert.ToInt32(dataKeyValue), CurrentUser.Email);
            IsEditMode = "yes";
            EditKey = nCopyPriroritizationRule.Id.ToString();
            SetPageMode(PageDisplayMode.EditQueueTemplate);
            ClearFields();
            ManageFiltersControl.Parent_key = Convert.ToInt32(nCopyPriroritizationRule.Id);
            ManageFiltersControl.SetControlModeFromOutside(true);
            ManageFiltersControl.BindEmailFilterGrid();
            ManageFiltersControl.SetControlModeFromOutside(true);
            LoadEditFormValues(Convert.ToInt32(nCopyPriroritizationRule.Id));

            tlPrioritizationStrip.Tabs[1].Enabled = true;
            tlPrioritizationStrip.Tabs[2].Enabled = true;
            //tlPageFilters.Enabled = true;
            //tlPageSchedule.Enabled = true;
            IsCopyMode = "yes";
            return;
        }
        else if (e.CommandName == "EnabledX")
        {
            String dataKeyValue = e.CommandArgument.ToString();
            Engine.LeadPrioritizationActions.MakeEnabled(Convert.ToInt32(dataKeyValue));
            lblMessageGrid.SetStatus(Messages.RecordUpdatedSuccess);
            BindGrid();
        }
        else if (e.CommandName == "DeleteX")
        {
            String dataKeyValue = e.CommandArgument.ToString();
            Engine.LeadPrioritizationActions.Delete(Convert.ToInt32(dataKeyValue), parentType);
            lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
            BindGrid();
        }
        else if (e.CommandName == "UpOrder")
        {
            String dataKeyValue = e.CommandArgument.ToString();
            Engine.LeadPrioritizationActions.MoveUp(Convert.ToInt32(dataKeyValue));
            lblMessageGrid.SetStatus(Messages.RecordMovedUpSuccess);
            BindGrid();
        }
        else if (e.CommandName == "DownOrder")
        {
            String dataKeyValue = e.CommandArgument.ToString();
            Engine.LeadPrioritizationActions.MoveDown(Convert.ToInt32(dataKeyValue));
            lblMessageGrid.SetStatus(Messages.RecordMovedDownSuccess);
            BindGrid();
        }
        //YA[April 22, 2013] Run the dynamic query parser to update the SP for PL.
        RunDynamicQueryParser();
    }
    protected void tlPrioritizationStrip_TabClick(object sender, RadTabStripEventArgs e)
    {
        e.Tab.PageView.Visible = true;
    }
    void RunDynamicQueryParser()
    {
        //YA[April 22, 2013] Added to update the PL SP recreation according to the rule change.
        QueryParser nQueryParser = new QueryParser();
        nQueryParser.Run();
        nQueryParser.Dispose();
    }

    #endregion

}
