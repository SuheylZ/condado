using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
using Telerik.Web.UI;


public partial class Admin_ManageRetentions : SalesBasePage
{

    #region Members/Properties

    enum PageDisplayMode { GridList = 1, ListDetail = 2 }

    public int EditKey
    {
        get
        {
            int Id = 0;
            int.TryParse(hdnFieldEditTemplateKey.Value, out Id);
            return Id;
        }
        set
        {
            hdnFieldEditTemplateKey.Value = value.ToString();
        }
    }
    //public string IsEditMode
    //{
    //    get
    //    {
    //        return hdnFieldIsEditMode.Value;
    //    }
    //    set
    //    {
    //        hdnFieldIsEditMode.Value = value;
    //    }
    //}
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

    #endregion

    #region Methods

    private PageDisplayMode PageMode
    {
        set
        {
            pnlDetail.Visible = false;
            pnlGrid.Visible = false;

            switch (value)
            {
                case PageDisplayMode.GridList:
                    InitializeGrid();
                    pnlGrid.Visible = true;
                    break;
                case PageDisplayMode.ListDetail:
                    InitializeDetail();
                    pnlDetail.Visible = true;
                    break;
            }
        }
    }
  
    private void InitializeDetail()
    {
        ddlUsers.DataSource = Engine.UserActions.RetentionUsers;
        ddlUsers.DataBind();

        txtTitle.Text = "";
        txtDescription.Text = "";
        chkEnabled.Checked = false;
        txtCustomFilter.Text = "";
        rdBtnlstFilterSelection.SelectedIndex = 0;
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
        tlRetentionStrip.Tabs[2].Enabled = false;
        pgFilters.Enabled = false;
        ctrlShiftSchedule.ClearAllValues();

        ManageFiltersControl.Parent_key = 0;
        ManageFiltersControl.ParentType = FilterParentType.RetentionWebForm;// For Retention
        ManageFiltersControl.AddedBy = CurrentUser.FullName;
        ManageFiltersControl.ChangedBy = CurrentUser.FullName;
        ManageFiltersControl.BindEmailFilterGrid();

        tlRetentionStrip.SelectedIndex = 0;
        tlPages.SelectedIndex = 0;
        foreach (Telerik.Web.UI.RadPageView pg in tlPages.PageViews)
            pg.Visible = false;
        tlPages.PageViews[0].Visible = true;
    }
    private void InitializeGrid()
    {
        var records = Helper.SortRecords(Engine.LeadRetentionActions.All, "Priority", false);
        totalGridRecords = records.Count();
        grdRetention.DataSource = records;
        grdRetention.DataBind();

        //IsEditMode = "no";
        EditKey = 0;
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
    
    private void Edit(int recordId)
    {
        LeadRetentionRules nRetention = Engine.LeadRetentionActions.Get(recordId);
        txtTitle.Text = nRetention.Title;
        txtDescription.Text = nRetention.Description;
        chkEnabled.Checked = nRetention.IsActive;
        ddlUsers.SelectedValue = nRetention.UserKey.ToString();

        ManageFiltersControl.Parent_key = recordId;
        ManageFiltersControl.SetControlModeFromOutside(true);
        ManageFiltersControl.BindEmailFilterGrid();
        //IsEditMode = "yes";

        rdBtnlstFilterSelection.SelectedValue = nRetention.FilterSelection == null ? "0" : nRetention.FilterSelection.ToString();
        if (nRetention.FilterSelection == 2)
        {
            txtCustomFilter.Text = nRetention.FilterCustomValue;
        }
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);

        foreach (var itemDetail in Engine.LeadRetentionActions.GetDetails(nRetention.Id).All)
        {
            ctrlShiftSchedule.SetTime(itemDetail.Shift, itemDetail.WeekDay, true, new TimeSpan(itemDetail.Working.Starts.Hour, itemDetail.Working.Starts.Minute, itemDetail.Working.Starts.Second));
            ctrlShiftSchedule.SetTime(itemDetail.Shift, itemDetail.WeekDay, false, new TimeSpan(itemDetail.Working.Ends.Hour, itemDetail.Working.Ends.Minute, itemDetail.Working.Ends.Second));
        }

        tlRetentionStrip.Tabs[2].Enabled = true;
        tlRetentionStrip.Tabs[2].PageView.Enabled = true;
    }
    private void Save(bool convertToEditMode = false)
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
            throw new Exception(ErrorMessages.CustomFilterValueError);
        //{
        //    lblMessageForm.SetStatus();
        //    return false;
        //}
        if (EditKey == 0) //IsEditMode == "no")
        {
            if (Engine.LeadRetentionActions.TitleExists(txtTitle.Text))
                throw new Exception(ErrorMessages.SameTitleError);
            //{
            //    lblMessageForm.SetStatus("Same title already exists, please change the title text.");
            //    return false;
            //}

            var recordAdded = Engine.LeadRetentionActions.Add(txtTitle.Text, txtDescription.Text, chkEnabled.Checked, GetRetentionUser(), CurrentUser.Email, selectedFilter, customFilterValue);

            AddSchedules(recordAdded.Id);
            if (convertToEditMode)
            {
                EditKey = recordAdded.Id;
                //IsEditMode = "yes";
                ManageFiltersControl.Parent_key = recordAdded.Id;
                tlRetentionStrip.Tabs[2].Enabled = true;
                pgFilters.Enabled = true;
            }
        }
        else //if (IsEditMode == "yes")
        {
            //if (EditKey != 0)
            //{
                LeadRetentionRules nRetention = Engine.LeadRetentionActions.Get(EditKey);
                nRetention.Title = txtTitle.Text;
                nRetention.Description = txtDescription.Text;
                nRetention.UserKey = GetRetentionUser();
                nRetention.IsActive = chkEnabled.Checked;
                nRetention.FilterSelection = selectedFilter;
                nRetention.FilterCustomValue = customFilterValue;
                Engine.LeadRetentionActions.Change(nRetention, CurrentUser.FullName);
                Engine.LeadRetentionActions.GetDetails(nRetention.Id).DeleteAll();
                AddSchedules(nRetention.Id);
            //}

        }

        lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
        IsCopyMode = "";
    }
    private void Close()
    {
        PageMode = PageDisplayMode.GridList;
    }
  
    private Guid GetRetentionUser()
    {
        Guid userKey = Guid.Empty;
        Guid.TryParse(ddlUsers.SelectedValue, out userKey);
        return userKey;
    }

    private void AddSchedules(int recordId)
    {

        //Detail schedule for Sunday
        TimeSpan? SundayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Sunday, true);
        TimeSpan? SundayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Sunday, false);
        TimeSpan? SundayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Sunday, true);
        TimeSpan? SundayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Sunday, false);
        TimeSpan? SundayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Sunday, true);
        TimeSpan? SundayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Sunday, false);

        if (SundayS1StartTime != ctrlShiftSchedule.defaultTime && SundayS1StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Sunday, Convert.ToDateTime(SundayS1StartTime.ToString()), Convert.ToDateTime(SundayS1EndTime.ToString()));
        }
        if (SundayS2StartTime != ctrlShiftSchedule.defaultTime && SundayS2StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Sunday, Convert.ToDateTime(SundayS2StartTime.ToString()), Convert.ToDateTime(SundayS2EndTime.ToString()));
        }
        if (SundayS3StartTime != ctrlShiftSchedule.defaultTime && SundayS3StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Sunday, Convert.ToDateTime(SundayS3StartTime.ToString()), Convert.ToDateTime(SundayS3EndTime.ToString()));
        }

        //Detail schedule for Monday
        TimeSpan? MondayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Monday, true);
        TimeSpan? MondayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Monday, false);
        TimeSpan? MondayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Monday, true);
        TimeSpan? MondayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Monday, false);
        TimeSpan? MondayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Monday, true);
        TimeSpan? MondayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Monday, false);

        if (MondayS1StartTime != ctrlShiftSchedule.defaultTime && MondayS1StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Monday, Convert.ToDateTime(MondayS1StartTime.ToString()), Convert.ToDateTime(MondayS1EndTime.ToString()));
        }
        if (MondayS2StartTime != ctrlShiftSchedule.defaultTime && MondayS2StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Monday, Convert.ToDateTime(MondayS2StartTime.ToString()), Convert.ToDateTime(MondayS2EndTime.ToString()));
        }
        if (MondayS3StartTime != ctrlShiftSchedule.defaultTime && MondayS3StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Monday, Convert.ToDateTime(MondayS3StartTime.ToString()), Convert.ToDateTime(MondayS3EndTime.ToString()));
        }

        //Detail schedule for Tuesday
        TimeSpan? TuesdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Tuesday, true);
        TimeSpan? TuesdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Tuesday, false);
        TimeSpan? TuesdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Tuesday, true);
        TimeSpan? TuesdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Tuesday, false);
        TimeSpan? TuesdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Tuesday, true);
        TimeSpan? TuesdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Tuesday, false);

        if (TuesdayS1StartTime != ctrlShiftSchedule.defaultTime && TuesdayS1StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Tuesday, Convert.ToDateTime(TuesdayS1StartTime.ToString()), Convert.ToDateTime(TuesdayS1EndTime.ToString()));
        }
        if (TuesdayS2StartTime != ctrlShiftSchedule.defaultTime && TuesdayS2StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Tuesday, Convert.ToDateTime(TuesdayS2StartTime.ToString()), Convert.ToDateTime(TuesdayS2EndTime.ToString()));
        }
        if (TuesdayS3StartTime != ctrlShiftSchedule.defaultTime && TuesdayS3StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Tuesday, Convert.ToDateTime(TuesdayS3StartTime.ToString()), Convert.ToDateTime(TuesdayS3EndTime.ToString()));
        }
        //Detail schedule for Wednesday
        TimeSpan? WednesdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Wednesday, true);
        TimeSpan? WednesdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Wednesday, false);
        TimeSpan? WednesdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Wednesday, true);
        TimeSpan? WednesdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Wednesday, false);
        TimeSpan? WednesdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Wednesday, true);
        TimeSpan? WednesdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Wednesday, false);

        if (WednesdayS1StartTime != ctrlShiftSchedule.defaultTime && WednesdayS1StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Wednesday, Convert.ToDateTime(WednesdayS1StartTime.ToString()), Convert.ToDateTime(WednesdayS1EndTime.ToString()));
        }
        if (WednesdayS2StartTime != ctrlShiftSchedule.defaultTime && WednesdayS2StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Wednesday, Convert.ToDateTime(WednesdayS2StartTime.ToString()), Convert.ToDateTime(WednesdayS2EndTime.ToString()));
        }
        if (WednesdayS3StartTime != ctrlShiftSchedule.defaultTime && WednesdayS3StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Wednesday, Convert.ToDateTime(WednesdayS3StartTime.ToString()), Convert.ToDateTime(WednesdayS3EndTime.ToString()));
        }
        //Detail schedule for Thursday
        TimeSpan? ThursdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Thursday, true);
        TimeSpan? ThursdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Thursday, false);
        TimeSpan? ThursdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Thursday, true);
        TimeSpan? ThursdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Thursday, false);
        TimeSpan? ThursdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Thursday, true);
        TimeSpan? ThursdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Thursday, false);

        if (ThursdayS1StartTime != ctrlShiftSchedule.defaultTime && ThursdayS1StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Thursday, Convert.ToDateTime(ThursdayS1StartTime.ToString()), Convert.ToDateTime(ThursdayS1EndTime.ToString()));
        }
        if (ThursdayS2StartTime != ctrlShiftSchedule.defaultTime && ThursdayS2StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Thursday, Convert.ToDateTime(ThursdayS2StartTime.ToString()), Convert.ToDateTime(ThursdayS2EndTime.ToString()));
        }
        if (ThursdayS3StartTime != ctrlShiftSchedule.defaultTime && ThursdayS3StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Thursday, Convert.ToDateTime(ThursdayS3StartTime.ToString()), Convert.ToDateTime(ThursdayS3EndTime.ToString()));
        }
        //Detail schedule for Friday
        TimeSpan? FridayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Friday, true);
        TimeSpan? FridayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Friday, false);
        TimeSpan? FridayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Friday, true);
        TimeSpan? FridayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Friday, false);
        TimeSpan? FridayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Friday, true);
        TimeSpan? FridayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Friday, false);

        if (FridayS1StartTime != ctrlShiftSchedule.defaultTime && FridayS1StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Friday, Convert.ToDateTime(FridayS1StartTime.ToString()), Convert.ToDateTime(FridayS1EndTime.ToString()));
        }
        if (FridayS2StartTime != ctrlShiftSchedule.defaultTime && FridayS2StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Friday, Convert.ToDateTime(FridayS2StartTime.ToString()), Convert.ToDateTime(FridayS2EndTime.ToString()));
        }
        if (FridayS3StartTime != ctrlShiftSchedule.defaultTime && FridayS3StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Friday, Convert.ToDateTime(FridayS3StartTime.ToString()), Convert.ToDateTime(FridayS3EndTime.ToString()));
        }
        //Detail schedule for Saturday
        TimeSpan? SaturdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Saturday, true);
        TimeSpan? SaturdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Saturday, false);
        TimeSpan? SaturdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Saturday, true);
        TimeSpan? SaturdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Saturday, false);
        TimeSpan? SaturdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Saturday, true);
        TimeSpan? SaturdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Saturday, false);

        if (SaturdayS1StartTime != ctrlShiftSchedule.defaultTime && SaturdayS1StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S1, DayOfWeek.Saturday, Convert.ToDateTime(SaturdayS1StartTime.ToString()), Convert.ToDateTime(SaturdayS1EndTime.ToString()));
        }
        if (SaturdayS2StartTime != ctrlShiftSchedule.defaultTime && SaturdayS2StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S2, DayOfWeek.Saturday, Convert.ToDateTime(SaturdayS2StartTime.ToString()), Convert.ToDateTime(SaturdayS2EndTime.ToString()));
        }
        if (SaturdayS3StartTime != ctrlShiftSchedule.defaultTime && SaturdayS3StartTime != null)
        {
            Engine.LeadRetentionActions.GetDetails(recordId).Add(SubType.S3, DayOfWeek.Saturday, Convert.ToDateTime(SaturdayS3StartTime.ToString()), Convert.ToDateTime(SaturdayS3EndTime.ToString()));
        }
    }
 
    #endregion


    #region Events

    protected override void Page_Initialize(object sender, EventArgs args)
    {
        if (!IsPostBack)
        {
            InitializeGrid();            
        }
        lblMessageForm.SetStatus("");
        lblMessageGrid.SetStatus("");
        ctrlStatusCustomFilter.SetStatus("");
    }

    protected void btnAddNewQueue_Click(object sender, EventArgs e)
    {
        //IsEditMode = "no";
        //ClearFields();
        PageMode = PageDisplayMode.ListDetail;
        //ManageFiltersControl.Parent_key = 0;
        //ManageFiltersControl.BindEmailFilterGrid();
        
        ManageFiltersControl.SetControlModeFromOutside(true);
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
            Save(true);
            //IsCopyMode = "";
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
            Save();
            //IsCopyMode = "";
            Close();
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
            Engine.LeadRetentionActions.Delete(EditKey);
            IsCopyMode = "";
        }
        //PageMode=PageDisplayMode.GridList;
        //InitializeGrid();
        Close();
    }
    protected void grdRetention_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            int count = grdRetention.Items.Count;
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

        }
    }
    protected void grdRetention_RowDrop(object sender, Telerik.Web.UI.GridDragDropEventArgs e)
    {
        if (string.IsNullOrEmpty(e.HtmlElement))
        {
            if (e.DraggedItems[0].OwnerGridID == grdRetention.ClientID)
            {
                int destPriorityId = (int)e.DestDataItem.GetDataKeyValue("Priority");
                foreach (GridDataItem draggedItem in e.DraggedItems)
                {
                    int recordId = (int)draggedItem.GetDataKeyValue("Id");
                    if (e.DropPosition == GridItemDropPosition.Below)
                    {
                        Engine.LeadRetentionActions.Move(recordId, destPriorityId - 1);
                    }
                    else
                        Engine.LeadRetentionActions.Move(recordId, destPriorityId);
                    lblMessageGrid.SetStatus(Messages.RecordMovedSuccess);
                    InitializeGrid();

                }
            }
        }
    }
    protected void grdRetention_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
    {
        int id = 0;
        int.TryParse(e.CommandArgument.ToString(), out id);
        EditKey = id;

        switch (e.CommandName)
        {
            case "EditX":
                PageMode = PageDisplayMode.ListDetail;
                Edit(EditKey);
                break;

            case "CopyX":
                PageMode = PageDisplayMode.ListDetail;
                LeadRetentionRules nCopyRetentionRule = Engine.LeadRetentionActions.Copy(id, CurrentUser.Email);
                //IsEditMode = "yes";
                EditKey = nCopyRetentionRule.Id;

                ManageFiltersControl.Parent_key = Convert.ToInt32(nCopyRetentionRule.Id);
                ManageFiltersControl.SetControlModeFromOutside(true);
                ManageFiltersControl.BindEmailFilterGrid();

                ManageFiltersControl.SetControlModeFromOutside(true);
                Edit(EditKey);
                IsCopyMode = "yes";
                break;

            case "EnabledX":
                Engine.LeadRetentionActions.MakeEnabled(EditKey);
                lblMessageGrid.SetStatus(Messages.RecordUpdatedSuccess);
                InitializeGrid();
                break;

            case "DeleteX":
                Engine.LeadRetentionActions.Delete(EditKey);
                lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
                InitializeGrid();
                break;

            case "UpOrder":
                Engine.LeadRetentionActions.MoveUp(EditKey);
                lblMessageGrid.SetStatus(Messages.RecordMovedUpSuccess);
                InitializeGrid();
                break;

            case "DownOrder":
                Engine.LeadRetentionActions.MoveDown(EditKey);
                lblMessageGrid.SetStatus(Messages.RecordMovedDownSuccess);
                InitializeGrid();
                break;
        }
    }

    protected void tlRetentionStrip_TabClick(object sender, RadTabStripEventArgs e)
    {
        foreach (Telerik.Web.UI.RadPageView pg in tlPages.PageViews)
            pg.Visible = false;

        e.Tab.PageView.Visible = true;

    }

    #endregion
    
}