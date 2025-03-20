using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using SalesTool.DataAccess;
using SalesTool.DataAccess.Models;
//using S = System.Web.Security.Membership;
using System.Linq.Dynamic;
using Telerik.Web.UI;


public partial class Admin_ManageReassignments : SalesBasePage
{
    /// <summary>
    ///  Developed By: Imran H
    ///  Date: 08-11-13
    /// 
    /// </summary>

    #region Members/Properties

    enum PageDisplayMode { GridQueueTemplate = 1, EditQueueTemplate = 2 }
    enum RetentionProcess
    {
        NotStarted = 0,
        QueriedByUser = 1,
        Executed = 2
    }

    public int TotalGridRecords = 0;
    public short ParentType = 8; // For reassignment
    RetentionProcess ProcessState
    {
        get
        {
            RetentionProcess Ans = RetentionProcess.NotStarted;
            if (!string.IsNullOrEmpty(hdnProcessState.Value))
                Ans = (RetentionProcess)Helper.SafeConvert<int>(hdnProcessState.Value);
            return Ans;
        }
        set
        {
            hdnProcessState.Value = ((int)value).ToString();
        }
    }
    int ProcessInitiatorId
    {
        get
        {
            int iAns = 0;
            if (!string.IsNullOrEmpty(hdnProcessInitiatorId.Value))
                iAns = Helper.SafeConvert<int>(hdnProcessInitiatorId.Value);
            return iAns;
        }
        set { hdnProcessInitiatorId.Value = value.ToString(); }
    }
    bool WasProcessInitiatorEnabled
    {
        get
        {
            return Helper.SafeConvert<bool>(hdnProcessInitiatorOldState.Value);
        }
        set { hdnProcessInitiatorOldState.Value = value.ToString(); }
    }

    const string K_CONTINUE = "666_1";
    const string K_ROLLBACK = "666_0";


    // <summary>
    // EditKey used in the whole form to add or edit record id
    // </summary>
    //public string EditKey
    //{
    //    get
    //    {
    //        return hdnFieldEditTemplateKey.Value;
    //    }
    //    set
    //    {
    //        hdnFieldEditTemplateKey.Value = value;
    //    }
    //}

    public int RecordId
    {
        get
        {
            int iAns = 0;
            if (!string.IsNullOrEmpty(hdnFieldEditTemplateKey.Value))
                iAns = Helper.SafeConvert<int>(hdnFieldEditTemplateKey.Value);
            return iAns;
        }
        set { hdnFieldEditTemplateKey.Value = value.ToString(); }
    }
    /// <summary>
    /// Is the user going for new record
    /// </summary>
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
    /// <summary>
    /// To check the copy mode is yes or no.
    /// </summary>
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
    byte? UserTypeOnForm
    {
        get { return ddlUserType.SelectedValue == "-1" ? (byte?)null : Helper.NullConvert<byte>(ddlUserType.SelectedValue); }
        set { if (value == null) ddlUserType.SelectedValue = "-1"; else ddlUserType.SelectedValue = (value >= 1 && value <= 5) ? value.Value.ToString() : "-1"; }
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
    int FilterByRMType
    {
        get { return rm_type.SelectedValue.ConvertOrDefault<int>(); }
        set
        {
            rm_type.SelectedValue = value.ToString();
        }
    }
    #endregion



    public static string GetUserType(object it)
    {
        int itype = Helper.SafeConvert<byte>((it ?? "0").ToString());
        string sAns = string.Empty;

        switch (itype)
        {
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



    /// <summary>
    /// Added to update the RA SP recreation according to the rule change.
    /// </summary>

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
                IsEditMode = "no";
                //EditKey = string.Empty;
                RecordId = 0;
                break;
            case PageDisplayMode.EditQueueTemplate:
                divForm.Visible = true;
                divGrid.Visible = false;
                tlMultiPage.SelectedIndex = 0;
                tlPrioritizationStrip.SelectedIndex = 0;
                tlMultiPage.PageViews[1].Visible = false;
                //tlMultiPage.PageViews[2].Visible = false;
                //  ctrlShiftSchedule.Visible = true;
                break;
        }
    }
    PageDisplayMode GetPageMode()
    {
        PageDisplayMode mode = PageDisplayMode.EditQueueTemplate;

        if (divForm.Visible == false && divGrid.Visible == true)
            mode = PageDisplayMode.GridQueueTemplate;
        return mode;
    }


    /// <summary>
    /// call selected Reassignment user 
    /// </summary>
    private Guid? GetReAssignmentUser()
    {
        //Guid.TryParse(ddlUsers.SelectedValue, out userKey);
        if (ddlUsers.SelectedValue == "-1")
            return null;
        var userKey = ddlUsers.SelectedValue.ConvertOrDefault<Guid?>();
        return userKey;
    }
    /// <summary>
    /// Check for custom value for the filters for incoming leads
    /// </summary>
    /// <returns></returns>
    private bool CheckForValidCustomString()
    {
        try
        {
            CustomFilterParser nCustomFilter = new CustomFilterParser(txtCustomFilter.Text);
            nCustomFilter.ParseInput();
            List<string> listOpds = nCustomFilter.listOperands;
            return ManageFiltersControl.CheckOrderNumberValues(listOpds);
        }
        catch (Exception ex)
        {
            ctrlStatusCustomFilter.SetStatus(ErrorMessages.ErrorParsing + ex.Message);
            return false;
        }
    }
    /// <summary>
    /// Save reassignment management record.
    /// </summary>
    /// <param name="convertToEditMode"></param>
    /// <returns></returns>
    public bool SaveRecord(bool convertToEditMode = false)
    {
        try
        {

            short selectedFilter = Convert.ToInt16(rdBtnlstFilterSelection.SelectedValue);
            string customFilterValue = string.Empty;
            bool isCustomFilterError = false;
            // if (rdBtnlstFilterSelection.SelectedValue == "2")
            if (selectedFilter == 2)
            {
                isCustomFilterError = !CheckForValidCustomString();
                customFilterValue = txtCustomFilter.Text;
            }
            if ((string.IsNullOrEmpty(ddlUsers.SelectedValue) || ddlUsers.SelectedValue == "-1") && ddlrmType.SelectedValue == "1")
            {
                lblMessageForm.SetStatus("Reassignment user required");
                SetStatusVisibility(chk_ChangeStatus.Checked, chk_inl_subStatus.Checked,
                    ddlrmType.SelectedValue.ConvertOrDefault<int?>());
                return false;
            }
            if (ddlrmType.SelectedValue == "2" && ddlSkillGroup.SelectedValue.ConvertOrDefault<short?>() == null)
            {
                SetStatusVisibility(chk_ChangeStatus.Checked, chk_inl_subStatus.Checked,
                    ddlrmType.SelectedValue.ConvertOrDefault<int?>());
                lblMessageForm.SetStatus("Reassignment skill group required");
                return false;
            }
            if (isCustomFilterError)
            {
                SetStatusVisibility(chk_ChangeStatus.Checked, chk_inl_subStatus.Checked,
                    ddlrmType.SelectedValue.ConvertOrDefault<int?>());
                lblMessageForm.SetStatus(ErrorMessages.CustomFilterValueError);
                return false;
            }
            if (UserTypeOnForm == null)
                throw new Exception("user type required");
            if (IsEditMode == "no")
            {

                if (Engine.LeadReAssignmentRuleActions.TitleExists(txtTitle.Text))
                {
                    lblMessageForm.SetStatus(ErrorMessages.SameTitleError);
                    return false;
                }

                var recordAdded = Engine.LeadReAssignmentRuleActions.Add(txtTitle.Text, GetReAssignmentUser(),UserTypeOnForm, txtDescription.Text, false, CurrentUser.Email, selectedFilter, customFilterValue, chk_ChangeStatus.Checked, ddlLeadStatus.SelectedValue.ConvertOrDefault<int?>(), chk_inl_subStatus.Checked, ddlSubStatus.SelectedValue.ConvertOrDefault<int?>(), ddlrmType.SelectedValue.ConvertOrDefault<int?>(), ddlSkillGroup.SelectedValue.ConvertOrDefault<short?>(), ddlSchedule.SelectedValue.ConvertOrDefault<int?>(),chk_webGalCap.Checked,chk_StateLicense.Checked);
                // SZ [May 1, 2014] disucssed with yasir. seems like an old code not used.
                //AddSchedules(recordAdded.Id);
                ddlLeadStatus.SelectedValue = recordAdded.StatusKey.ToString();
                ddlSubStatus.SelectedValue = recordAdded.SubStatusKey.ToString();
                ddlSkillGroup.SelectedValue = recordAdded.SkillId.ToString();
                SetStatusVisibility(recordAdded.IsChangeStatus, recordAdded.IsIncludeSubStatus, recordAdded.RM_Type);
                if (convertToEditMode)
                {
                    //EditKey = recordAdded.Id.ToString();
                    RecordId = recordAdded.Id;
                    IsEditMode = "yes";
                    ManageFiltersControl.Parent_key = recordAdded.Id;
                    //tlPrioritizationStrip.Tabs[2].Enabled = true;
                    tlPageFilters.Enabled = true;
                }

            }
            else if (IsEditMode == "yes")
            {
                //if (EditKey != string.Empty)
                if (RecordId > 0)
                {
                    //LeadReassignmentRules nReassignmentRules = Engine.LeadReAssignmentRuleActions.Get(Convert.ToInt32(EditKey));
                    LeadReassignmentRules nReassignmentRules = Engine.LeadReAssignmentRuleActions.Get(RecordId);
                    nReassignmentRules.Title = txtTitle.Text;
                    nReassignmentRules.ScheduleId = ddlSchedule.SelectedValue.ConvertOrDefault<int>();
                    nReassignmentRules.RM_Type = ddlrmType.SelectedValue.ConvertOrDefault<int?>();
                    nReassignmentRules.IsChangeStatus = chk_ChangeStatus.Checked;
                    nReassignmentRules.IsIncludeSubStatus = chk_inl_subStatus.Checked;
                    nReassignmentRules.StatusKey = ddlLeadStatus.SelectedValue.ConvertOrDefault<int?>();
                    nReassignmentRules.SubStatusKey = ddlSubStatus.SelectedValue.ConvertOrDefault<int?>();
                    nReassignmentRules.SkillId = ddlSkillGroup.SelectedValue.ConvertOrDefault<short?>();
                    nReassignmentRules.Description = txtDescription.Text;
                    if (ddlUsers.SelectedValue == "-1")
                    {
                        nReassignmentRules.UsrKey = null;
                    }
                    else if (!string.IsNullOrEmpty(ddlUsers.SelectedValue))
                    {
                        nReassignmentRules.UsrKey = Guid.Parse(ddlUsers.SelectedValue);
                    }

                    if (!CanEnableRule(nReassignmentRules.Id) && chkEnabled.Checked)
                        throw new Exception("At least one filter should be defined");
                    //Sz [May 1, 2014] dont dfo it here. just ask user
                    if (!chkEnabled.Checked)
                        nReassignmentRules.IsActive = false;

                    nReassignmentRules.FilterSelection = selectedFilter;
                    nReassignmentRules.FilterCustomValue = customFilterValue;
                    nReassignmentRules.UserType = UserTypeOnForm;
                    nReassignmentRules.IsCheckWebCap = chk_webGalCap.Checked;
                    nReassignmentRules.IsCheckStateLicense = chk_StateLicense.Checked;

                    Engine.LeadReAssignmentRuleActions.Change(nReassignmentRules, CurrentUser.FullName);
                    Engine.LeadReAssignmentRuleActions.GetDetails(nReassignmentRules.Id).DeleteAll();
                    ddlLeadStatus.SelectedValue = nReassignmentRules.StatusKey.ToString();
                    ddlSubStatus.SelectedValue = nReassignmentRules.SubStatusKey.ToString();
                    SetStatusVisibility(nReassignmentRules.IsChangeStatus, nReassignmentRules.IsIncludeSubStatus, nReassignmentRules.RM_Type);
                    // SZ [May 1, 2014] disucssed with yasir. seems like an old code not used.
                    //AddSchedules(nReassignmentRules.Id);
                    ExecuteUserReassignment(0);
                    SetStatusVisibility(nReassignmentRules.IsChangeStatus, nReassignmentRules.IsIncludeSubStatus, nReassignmentRules.RM_Type);
                }
            }

            lblMessageForm.SetStatus(Messages.RecordSavedSuccess);
            if (chkEnabled.Checked)
            {
                if (ShouldQueryUser(RecordId, chkEnabled.Checked))
                    AskUserToInitiateProcess(RecordId);
            }
            return true;
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
            return false;
        }
    }


    /// <summary>
    ///  Adding the reassignment Schedules
    /// </summary>
    /// <param name="id"></param>
    //private void AddSchedules(int id)
    //{
    //    //Yasir A [?] Commented on John's request
    //    bool visible = ctrlShiftSchedule.Visible;
    //    ctrlShiftSchedule.Visible = true;

    //    //Detail schedule for Sunday
    //    TimeSpan? sundayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Sunday, true);
    //    TimeSpan? sundayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Sunday, false);
    //    TimeSpan? sundayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Sunday, true);
    //    TimeSpan? sundayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Sunday, false);
    //    TimeSpan? sundayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Sunday, true);
    //    TimeSpan? sundayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Sunday, false);

    //    if (sundayS1StartTime != ctrlShiftSchedule.defaultTime && sundayS1StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S1, DayOfWeek.Sunday, Convert.ToDateTime(sundayS1StartTime.ToString()), Convert.ToDateTime(sundayS1EndTime.ToString()));
    //    }
    //    if (sundayS2StartTime != ctrlShiftSchedule.defaultTime && sundayS2StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S2, DayOfWeek.Sunday, Convert.ToDateTime(sundayS2StartTime.ToString()), Convert.ToDateTime(sundayS2EndTime.ToString()));
    //    }
    //    if (sundayS3StartTime != ctrlShiftSchedule.defaultTime && sundayS3StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S3, DayOfWeek.Sunday, Convert.ToDateTime(sundayS3StartTime.ToString()), Convert.ToDateTime(sundayS3EndTime.ToString()));
    //    }

    //    //Detail schedule for Monday
    //    TimeSpan? mondayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Monday, true);
    //    TimeSpan? mondayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Monday, false);
    //    TimeSpan? mondayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Monday, true);
    //    TimeSpan? mondayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Monday, false);
    //    TimeSpan? mondayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Monday, true);
    //    TimeSpan? mondayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Monday, false);

    //    if (mondayS1StartTime != ctrlShiftSchedule.defaultTime && mondayS1StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S1, DayOfWeek.Monday, Convert.ToDateTime(mondayS1StartTime.ToString()), Convert.ToDateTime(mondayS1EndTime.ToString()));
    //    }
    //    if (mondayS2StartTime != ctrlShiftSchedule.defaultTime && mondayS2StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S2, DayOfWeek.Monday, Convert.ToDateTime(mondayS2StartTime.ToString()), Convert.ToDateTime(mondayS2EndTime.ToString()));
    //    }
    //    if (mondayS3StartTime != ctrlShiftSchedule.defaultTime && mondayS3StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S3, DayOfWeek.Monday, Convert.ToDateTime(mondayS3StartTime.ToString()), Convert.ToDateTime(mondayS3EndTime.ToString()));
    //    }

    //    //Detail schedule for Tuesday
    //    TimeSpan? tuesdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Tuesday, true);
    //    TimeSpan? tuesdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Tuesday, false);
    //    TimeSpan? tuesdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Tuesday, true);
    //    TimeSpan? tuesdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Tuesday, false);
    //    TimeSpan? tuesdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Tuesday, true);
    //    TimeSpan? tuesdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Tuesday, false);

    //    if (tuesdayS1StartTime != ctrlShiftSchedule.defaultTime && tuesdayS1StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S1, DayOfWeek.Tuesday, Convert.ToDateTime(tuesdayS1StartTime.ToString()), Convert.ToDateTime(tuesdayS1EndTime.ToString()));
    //    }
    //    if (tuesdayS2StartTime != ctrlShiftSchedule.defaultTime && tuesdayS2StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S2, DayOfWeek.Tuesday, Convert.ToDateTime(tuesdayS2StartTime.ToString()), Convert.ToDateTime(tuesdayS2EndTime.ToString()));
    //    }
    //    if (tuesdayS3StartTime != ctrlShiftSchedule.defaultTime && tuesdayS3StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S3, DayOfWeek.Tuesday, Convert.ToDateTime(tuesdayS3StartTime.ToString()), Convert.ToDateTime(tuesdayS3EndTime.ToString()));
    //    }
    //    //Detail schedule for Wednesday
    //    TimeSpan? wednesdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Wednesday, true);
    //    TimeSpan? wednesdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Wednesday, false);
    //    TimeSpan? wednesdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Wednesday, true);
    //    TimeSpan? wednesdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Wednesday, false);
    //    TimeSpan? wednesdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Wednesday, true);
    //    TimeSpan? wednesdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Wednesday, false);

    //    if (wednesdayS1StartTime != ctrlShiftSchedule.defaultTime && wednesdayS1StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S1, DayOfWeek.Wednesday, Convert.ToDateTime(wednesdayS1StartTime.ToString()), Convert.ToDateTime(wednesdayS1EndTime.ToString()));
    //    }
    //    if (wednesdayS2StartTime != ctrlShiftSchedule.defaultTime && wednesdayS2StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S2, DayOfWeek.Wednesday, Convert.ToDateTime(wednesdayS2StartTime.ToString()), Convert.ToDateTime(wednesdayS2EndTime.ToString()));
    //    }
    //    if (wednesdayS3StartTime != ctrlShiftSchedule.defaultTime && wednesdayS3StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S3, DayOfWeek.Wednesday, Convert.ToDateTime(wednesdayS3StartTime.ToString()), Convert.ToDateTime(wednesdayS3EndTime.ToString()));
    //    }
    //    //Detail schedule for Thursday
    //    TimeSpan? thursdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Thursday, true);
    //    TimeSpan? thursdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Thursday, false);
    //    TimeSpan? thursdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Thursday, true);
    //    TimeSpan? thursdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Thursday, false);
    //    TimeSpan? thursdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Thursday, true);
    //    TimeSpan? thursdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Thursday, false);

    //    if (thursdayS1StartTime != ctrlShiftSchedule.defaultTime && thursdayS1StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S1, DayOfWeek.Thursday, Convert.ToDateTime(thursdayS1StartTime.ToString()), Convert.ToDateTime(thursdayS1EndTime.ToString()));
    //    }
    //    if (thursdayS2StartTime != ctrlShiftSchedule.defaultTime && thursdayS2StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S2, DayOfWeek.Thursday, Convert.ToDateTime(thursdayS2StartTime.ToString()), Convert.ToDateTime(thursdayS2EndTime.ToString()));
    //    }
    //    if (thursdayS3StartTime != ctrlShiftSchedule.defaultTime && thursdayS3StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S3, DayOfWeek.Thursday, Convert.ToDateTime(thursdayS3StartTime.ToString()), Convert.ToDateTime(thursdayS3EndTime.ToString()));
    //    }
    //    //Detail schedule for Friday
    //    TimeSpan? fridayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Friday, true);
    //    TimeSpan? fridayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Friday, false);
    //    TimeSpan? fridayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Friday, true);
    //    TimeSpan? fridayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Friday, false);
    //    TimeSpan? fridayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Friday, true);
    //    TimeSpan? fridayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Friday, false);

    //    if (fridayS1StartTime != ctrlShiftSchedule.defaultTime && fridayS1StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S1, DayOfWeek.Friday, Convert.ToDateTime(fridayS1StartTime.ToString()), Convert.ToDateTime(fridayS1EndTime.ToString()));
    //    }
    //    if (fridayS2StartTime != ctrlShiftSchedule.defaultTime && fridayS2StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S2, DayOfWeek.Friday, Convert.ToDateTime(fridayS2StartTime.ToString()), Convert.ToDateTime(fridayS2EndTime.ToString()));
    //    }
    //    if (fridayS3StartTime != ctrlShiftSchedule.defaultTime && fridayS3StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S3, DayOfWeek.Friday, Convert.ToDateTime(fridayS3StartTime.ToString()), Convert.ToDateTime(fridayS3EndTime.ToString()));
    //    }
    //    //Detail schedule for Saturday
    //    TimeSpan? saturdayS1StartTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Saturday, true);
    //    TimeSpan? saturdayS1EndTime = ctrlShiftSchedule.GetTime(SubType.S1, DayOfWeek.Saturday, false);
    //    TimeSpan? saturdayS2StartTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Saturday, true);
    //    TimeSpan? saturdayS2EndTime = ctrlShiftSchedule.GetTime(SubType.S2, DayOfWeek.Saturday, false);
    //    TimeSpan? saturdayS3StartTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Saturday, true);
    //    TimeSpan? saturdayS3EndTime = ctrlShiftSchedule.GetTime(SubType.S3, DayOfWeek.Saturday, false);

    //    if (saturdayS1StartTime != ctrlShiftSchedule.defaultTime && saturdayS1StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S1, DayOfWeek.Saturday, Convert.ToDateTime(saturdayS1StartTime.ToString()), Convert.ToDateTime(saturdayS1EndTime.ToString()));
    //    }
    //    if (saturdayS2StartTime != ctrlShiftSchedule.defaultTime && saturdayS2StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S2, DayOfWeek.Saturday, Convert.ToDateTime(saturdayS2StartTime.ToString()), Convert.ToDateTime(saturdayS2EndTime.ToString()));
    //    }
    //    if (saturdayS3StartTime != ctrlShiftSchedule.defaultTime && saturdayS3StartTime != null)
    //    {
    //        Engine.LeadReAssignmentRuleActions.GetDetails(id).Add(SubType.S3, DayOfWeek.Saturday, Convert.ToDateTime(saturdayS3StartTime.ToString()), Convert.ToDateTime(saturdayS3EndTime.ToString()));
    //    }


    //    ctrlShiftSchedule.Visible = visible;
    //}

    /// <summary>
    /// Bind the reassignmetn rules to the grid
    /// </summary>
    private void BindGrid()
    {

        var qryLeadReAssignmentRule = //FilterByUserType <= 0 ?
            //Engine.LeadReAssignmentRuleActions.All.Select(T => new LeadAssignementRule()
            // {
            //     ID = T.Id,
            //     UsrKey = T.UsrKey,
            //     Title = T.Title,
            //     Description = T.Description,
            //     UserName = T.user.FirstName == null ? string.Empty : T.user.FirstName + " " + T.user.LastName,
            //     FilterSelection = T.FilterSelection,
            //     FilterCustomValue = T.FilterCustomValue,
            //     Priority = T.Priority,
            //     IsActive = T.IsActive,
            //     Added = T.Added,
            //     Changed = T.Changed,
            //     UserType = T.UserType,
            //     LastRun = T.LastRun,
            //     RM_type = T.RM_Type,
            // }) :
            Engine.LeadReAssignmentRuleActions.All.Select(T => new LeadAssignementRule()
            {
                ID = T.Id,
                UsrKey = T.UsrKey,
                Title = T.Title,
                Description = T.Description,
                UserName = T.user.FirstName == null ? string.Empty : T.user.FirstName + " " + T.user.LastName,
                FilterSelection = T.FilterSelection,
                FilterCustomValue = T.FilterCustomValue,
                Priority = T.Priority,
                IsActive = T.IsActive,
                Added = T.Added,
                Changed = T.Changed,
                UserType = T.UserType,
                LastRun = T.LastRun,
                RM_type = T.RM_Type,
                RM_Type_Name = T.RM_Type == null ? "Unassigned" : T.RM_Type == 1 ? "User" : "Skill Group",
                SkillName = T.skill_groups != null ? T.skill_groups.Name : ""
            });//.Where(x => (x.UserType ?? -1) == FilterByUserType&& ((x.RM_type??-1)==FilterByRMType));
        if (FilterByUserType != 0)
            qryLeadReAssignmentRule = qryLeadReAssignmentRule.Where(x => (x.UserType ?? -1) == FilterByUserType);
        if (FilterByRMType != -1)
            qryLeadReAssignmentRule = qryLeadReAssignmentRule.Where(x => (x.RM_type ?? -1) == FilterByRMType);

        var records = Helper.SortRecords(qryLeadReAssignmentRule, "Priority", false);
        //var records = Helper.SortRecords(Engine.LeadReAssignmentRuleActions.All, "Priority", false);
        TotalGridRecords = records.Count();
        grdRassignment.DataSource = records;
        grdRassignment.DataBind();
    }

    private void BindStaticDropdowns()
    {

        var statusLookups = Engine.StatusActions.StatusLookups.ToList();
        statusLookups.Insert(0, new NameIntValueLookup() { Name = "---  Select Status  ---", Value = default(int?) });
        ddlLeadStatus.DataTextField = "Name";
        ddlLeadStatus.DataValueField = "Value";
        ddlLeadStatus.DataSource = statusLookups;
        ddlLeadStatus.DataBind();

        var subStatusLookups = Engine.StatusActions.SubStatusLookups.ToList();
        subStatusLookups.Insert(0, new NameIntValueLookup() { Name = "---  Select Sub Status  ---", Value = default(int?) });
        ddlSubStatus.DataTextField = "Name";
        ddlSubStatus.DataValueField = "Value";
        ddlSubStatus.DataSource = subStatusLookups;
        ddlSubStatus.DataBind();

        var skillGroups = Engine.SkillGroupActions.SkillGroupLookups.ToList();
        skillGroups.Insert(0, new NameIntValueLookup() { Name = "---  Select Skill Group  ---", Value = default(int?) });
        ddlSkillGroup.DataTextField = "Name";
        ddlSkillGroup.DataValueField = "Value";
        ddlSkillGroup.DataSource = skillGroups;
        ddlSkillGroup.DataBind();
    }
    internal class LeadAssignementRule
    {
        public int ID { get; set; }
        public Guid? UsrKey { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string UserName { get; set; }
        public short? FilterSelection { get; set; }
        public string FilterCustomValue { get; set; }
        public int Priority { get; set; }
        public bool IsActive { get; set; }
        public SalesTool.DataAccess.Models.History Added { get; set; }
        public SalesTool.DataAccess.Models.History Changed { get; set; }
        public byte? UserType { get; set; }
        public DateTime? LastRun { get; set; }
        public int? RM_type { get; set; }
        public string RM_Type_Name { get; set; }
        public string SkillName { get; set; }
    }
    /// <summary>
    /// Clear form fields
    /// </summary>
    public void ClearFields()
    {
        SetStatusVisibility(false, false, null);
        //ddlUsers.DataSource = Engine.UserActions.ReassignmentUsers;
        var users = Engine.UserActions.AllUsersLookups.ToList();
        users.Insert(0, new NameValueLookup() { Name = "--- Unassigned User  ---", Value = "-1" });
        ddlUsers.DataSource = users;
        ddlUsers.DataTextField = "Name";
        ddlUsers.DataValueField = "Value";
        ddlUsers.DataBind();
        BindStaticDropdowns();
        txtTitle.Text = string.Empty;
        chk_webGalCap.Checked = false;
        chk_webGalCap.Checked = false;
        ddlSchedule.SelectedValue = "1";
        ddlrmType.SelectedValue = null;
        chk_ChangeStatus.Checked = false;
        chk_inl_subStatus.Checked = false;
        txtDescription.Text = string.Empty;
        chkEnabled.Checked = false;
        txtCustomFilter.Text = string.Empty;
        rdBtnlstFilterSelection.SelectedIndex = 0;
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);
        // tlPrioritizationStrip.Tabs[2].Enabled = false;
        tlPageFilters.Enabled = false;
        //ctrlShiftSchedule.ClearAllValues();
    }

    private void SetStatusVisibility(bool status, bool substatus, int? rmType = null)
    {
        if (!status)
            rm_change_status.Style.Add("display", "none");
        else
            rm_change_status.Style.Remove("display");
        if (!substatus)
            rm_sub_status.Style.Add("display", "none");
        else
            rm_sub_status.Style.Remove("display");
        switch (rmType)
        {
            case null:
                rm_type_user.Style.Add("display", "none");
                rm_type_skillgroup.Style.Add("display", "none");
                break;
            case 1:
                rm_type_skillgroup.Style.Add("display", "none");
                rm_type_user.Style.Remove("display");
                break;
            case 2:
                rm_type_user.Style.Add("display", "none");
                rm_type_skillgroup.Style.Remove("display");
                break;

        }

    }



    /// <summary>
    /// Load the edit form values 
    /// </summary>
    /// <param name="id"></param>
    public void LoadEditFormValues(int recordId)
    {
        LeadReassignmentRules nReassignmentRule = Engine.LeadReAssignmentRuleActions.Get(recordId);
        txtTitle.Text = nReassignmentRule.Title;
        chk_webGalCap.Checked = nReassignmentRule.IsCheckWebCap;
        chk_StateLicense.Checked = nReassignmentRule.IsCheckStateLicense;
        ddlSchedule.SelectedValue = nReassignmentRule.ScheduleId.ToString();
        ddlrmType.SelectedValue = nReassignmentRule.RM_Type.ToString();
        chk_ChangeStatus.Checked = nReassignmentRule.IsChangeStatus;
        chk_inl_subStatus.Checked = nReassignmentRule.IsIncludeSubStatus;
        ddlLeadStatus.SelectedValue = nReassignmentRule.StatusKey.ToString();
        ddlSubStatus.SelectedValue = nReassignmentRule.SubStatusKey.ToString();
        ddlSkillGroup.SelectedValue = nReassignmentRule.SkillId.ToString();
        chk_inl_subStatus.Checked = nReassignmentRule.IsIncludeSubStatus;
        SetStatusVisibility(nReassignmentRule.IsChangeStatus, nReassignmentRule.IsIncludeSubStatus, nReassignmentRule.RM_Type);
        txtDescription.Text = nReassignmentRule.Description;
        chkEnabled.Checked = nReassignmentRule.IsActive;
        ddlUsers.SelectedValue = nReassignmentRule.user == null ? "-1" : nReassignmentRule.user.Key.ToString();
        UserTypeOnForm = nReassignmentRule.UserType;
        rdBtnlstFilterSelection.SelectedValue = nReassignmentRule.FilterSelection == null ? "0" : nReassignmentRule.FilterSelection.ToString();
        if (nReassignmentRule.FilterSelection == 2)
        {
            txtCustomFilter.Text = nReassignmentRule.FilterCustomValue;
        }
        rdBtnlstFilterSelection_SelectedIndexChanged(this, null);

        //foreach (var itemDetail in Engine.LeadReAssignmentRuleActions.GetDetails(nReassignmentRule.Id).All)
        //{
        //    ctrlShiftSchedule.SetTime(itemDetail.Shift, itemDetail.WeekDay, true, new TimeSpan(itemDetail.Working.Starts.Hour, itemDetail.Working.Starts.Minute, itemDetail.Working.Starts.Second));
        //    ctrlShiftSchedule.SetTime(itemDetail.Shift, itemDetail.WeekDay, false, new TimeSpan(itemDetail.Working.Ends.Hour, itemDetail.Working.Ends.Minute, itemDetail.Working.Ends.Second));
        //}
    }

    #endregion

    #region Events
    protected override void RaisePostBackEvent(IPostBackEventHandler sourceControl, string eventArgument)
    {
        if (eventArgument == K_CONTINUE)
        {
            int id = ProcessInitiatorId;
            ExecuteUserReassignment(ProcessInitiatorId);
            if (GetPageMode() == PageDisplayMode.GridQueueTemplate)
                BindGrid();
            else
                LoadEditFormValues(id);
        }
        else if (eventArgument == K_ROLLBACK)
            RollBackProcess();
        else
            base.RaisePostBackEvent(sourceControl, eventArgument);
    }
    /// <summary>
    ///  page load event functionality call
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!Page.IsPostBack)
        {
            ProcessState = RetentionProcess.NotStarted;

            ManageFiltersControl.ParentType = FilterParentType.ReassignmentWebForm;

            ManageFiltersControl.AddedBy = CurrentUser.FullName;
            ManageFiltersControl.ChangedBy = CurrentUser.FullName;

            SetPageMode(PageDisplayMode.GridQueueTemplate);
            BindGrid();
        }
        lblMessageForm.SetStatus(string.Empty);
        lblMessageGrid.SetStatus(string.Empty);
        ctrlStatusCustomFilter.SetStatus(string.Empty);
        rm_type.SelectedIndexChanged += (o, a) => BindGrid();
        ddlFilterByUserType.SelectedIndexChanged += (o, a) => BindGrid();
    }
    /// <summary>
    ///  Calls when adding new record.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddNewQueue_Click(object sender, EventArgs e)
    {
        IsEditMode = "no";
        SetPageMode(PageDisplayMode.EditQueueTemplate);
        ClearFields();
        ManageFiltersControl.Parent_key = 0;
        ManageFiltersControl.BindEmailFilterGrid();
        ManageFiltersControl.SetControlModeFromOutside(true);
    }
    /// <summary>
    /// Calls when on Filter Selection
    /// ALL, Any, Custom
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void rdBtnlstFilterSelection_SelectedIndexChanged(object sender, EventArgs e)
    {
        //Custom filter selection option selected.
        txtCustomFilter.Enabled = rdBtnlstFilterSelection.SelectedValue == "2";
    }
    /// <summary>
    ///  Calls when on custom Filter text changed event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void txtCustomFilter_TextChanged(object sender, EventArgs e)
    {
        CheckForValidCustomString();
    }
    /// <summary>
    /// call to save the record
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnApply_Click(object sender, EventArgs e)
    {
        try
        {
            SaveRecord(true);
            IsCopyMode = string.Empty;
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
    }
    /// <summary>
    /// call event to save the record
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        try
        {

            if (SaveRecord())
            {
                SetPageMode(PageDisplayMode.GridQueueTemplate);
                BindGrid();
                ClearFields();
                IsCopyMode = string.Empty;
            }
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
        }
    }
    /// <summary>
    /// call event to execute the sp spRAUpdate
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnPVRefresh_Click(object sender, EventArgs e)
    {

        var nQueryParser = new RaQueryParser();
        nQueryParser.ExecuteManageReassignmentSp();
        nQueryParser.Dispose();
        BindGrid();


    }
    /// <summary>
    /// call envt on c
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnCancelOnForm_Click(object sender, EventArgs e)
    {
        if (IsCopyMode == "yes")
        {
            //Engine.LeadReAssignmentRuleActions.Delete(Convert.ToInt32(EditKey), ParentType);
            Engine.LeadReAssignmentRuleActions.Delete(RecordId, ParentType);
            IsCopyMode = string.Empty;
        }
        SetPageMode(PageDisplayMode.GridQueueTemplate);
        BindGrid();
    }
    /// <summary>
    ///  bind controls on item data bound event
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdRassignment_ItemDataBound(object sender, GridItemEventArgs e)
    {
        int index;
        if (e.Item is GridDataItem)
        {

            var item = (GridDataItem)e.Item;
            index = item.ItemIndex;
            if (index == 0)
            {
                var imgBtnUpOrder = (ImageButton)item.FindControl("imgBtnUpOrder");
                if (imgBtnUpOrder != null) imgBtnUpOrder.Visible = false;
            }
            if (TotalGridRecords - 1 == index)
            {
                var imgBtnDownOrder = (ImageButton)item.FindControl("imgBtnDownOrder");
                if (imgBtnDownOrder != null) imgBtnDownOrder.Visible = false;
            }
            // SZ [Aug, 2014] handle the user right delete
            {
                var ctl = e.Item.FindControl("lnkDelete") as LinkButton;
                var lbl = e.Item.FindControl("lblSepDel") as Label;
                if (ctl != null && !CurrentUser.Security.Administration.CanDelete)
                {
                    ctl.Visible = false;
                    if (lbl != null)
                        lbl.Visible = false;
                }
            }

        }
    }
    /// <summary>
    ///  event call on 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdRassignment_RowDrop(object sender, Telerik.Web.UI.GridDragDropEventArgs e)
    {
        if (string.IsNullOrEmpty(e.HtmlElement))
        {
            if (e.DraggedItems[0].OwnerGridID == grdRassignment.ClientID)
            {
                int destPriorityId = (int)e.DestDataItem.GetDataKeyValue("Priority");
                int recordId;
                foreach (GridDataItem draggedItem in e.DraggedItems)
                {
                    recordId = (int)draggedItem.GetDataKeyValue("Id");
                    if (e.DropPosition == GridItemDropPosition.Below)
                        Engine.LeadReAssignmentRuleActions.Move(recordId, destPriorityId - 1);
                    else
                        Engine.LeadReAssignmentRuleActions.Move(recordId, destPriorityId);
                    lblMessageGrid.SetStatus(Messages.RecordMovedSuccess);
                    BindGrid();
                    // Run the dynamic query parser to update the SP for PL.
                    ExecuteUserReassignment(0);
                }
            }
        }
    }
    /// <summary>
    ///  call event on item command event 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void grdRassignment_ItemCommand(object sender, GridCommandEventArgs e)
    {
        switch (e.CommandName)
        {
            case "EditX":
                {

                    String dataKeyValue = e.CommandArgument.ToString();
                    //EditKey = dataKeyValue;
                    RecordId = Helper.SafeConvert<int>(dataKeyValue);
                    ManageFiltersControl.Parent_key = Convert.ToInt32(dataKeyValue);
                    ManageFiltersControl.SetControlModeFromOutside(true);
                    ManageFiltersControl.BindEmailFilterGrid();
                    IsEditMode = "yes";
                    SetPageMode(PageDisplayMode.EditQueueTemplate);
                    ClearFields();
                    LoadEditFormValues(Convert.ToInt32(dataKeyValue));
                    //BindGrid();
                    //tlPrioritizationStrip.Tabs[2].Enabled = true;
                    tlPageFilters.Enabled = true;
                    return;
                }
            case "CopyX":
                {
                    String dataKeyValue = e.CommandArgument.ToString();
                    LeadReassignmentRules nCopyReassignmentRules = Engine.LeadReAssignmentRuleActions.Copy(Convert.ToInt32(dataKeyValue), CurrentUser.Email);
                    IsEditMode = "yes";
                    //EditKey = nCopyReassignmentRules.Id.ToString();
                    RecordId = nCopyReassignmentRules.Id;
                    SetPageMode(PageDisplayMode.EditQueueTemplate);
                    ClearFields();
                    ManageFiltersControl.Parent_key = Convert.ToInt32(nCopyReassignmentRules.Id);
                    ManageFiltersControl.SetControlModeFromOutside(true);
                    ManageFiltersControl.BindEmailFilterGrid();
                    ManageFiltersControl.SetControlModeFromOutside(true);
                    LoadEditFormValues(Convert.ToInt32(nCopyReassignmentRules.Id));
                    //tlPrioritizationStrip.Tabs[2].Enabled = true;
                    tlPageFilters.Enabled = true;
                    IsCopyMode = "yes";
                    return;
                }
            case "EnabledX":
                {
                    int id = Helper.SafeConvert<int>(e.CommandArgument.ToString());
                    bool bState = !Engine.LeadReAssignmentRuleActions.IsEnabled(id);
                    if (ShouldQueryUser(id, bState))
                        AskUserToInitiateProcess(id);
                    else
                    {
                        if (CanEnableRule(id))
                        {
                            Engine.LeadReAssignmentRuleActions.ToggleState(id);
                            ExecuteUserReassignment(0);
                            BindGrid();
                        }
                        else
                            lblMessageGrid.SetStatus(new Exception("rule cannot be enabled"));
                    }
                }
                break;
            case "DeleteX":
                {
                    String dataKeyValue = e.CommandArgument.ToString();
                    Engine.LeadReAssignmentRuleActions.Delete(Convert.ToInt32(dataKeyValue), ParentType);
                    lblMessageGrid.SetStatus(Messages.RecordDeletedSuccess);
                    BindGrid();
                }
                break;
            case "UpOrder":
                {
                    String dataKeyValue = e.CommandArgument.ToString();
                    Engine.LeadReAssignmentRuleActions.MoveUp(Convert.ToInt32(dataKeyValue));
                    lblMessageGrid.SetStatus(Messages.RecordMovedUpSuccess);
                    BindGrid();
                }
                break;
            case "DownOrder":
                {
                    String dataKeyValue = e.CommandArgument.ToString();
                    Engine.LeadReAssignmentRuleActions.MoveDown(Convert.ToInt32(dataKeyValue));
                    lblMessageGrid.SetStatus(Messages.RecordMovedDownSuccess);
                    BindGrid();
                }
                break;
        }
        //Run the dynamic query parser to update the SP for RL.
        //RunDynamicQueryParser();
    }



    protected void tlPrioritizationStrip_TabClick(object sender, RadTabStripEventArgs e)
    {
        e.Tab.PageView.Visible = true;
    }





    bool ShouldQueryUser(int id, bool requested)
    {
        bool bAns = false;
        bool bCurrent = Engine.LeadReAssignmentRuleActions.IsEnabled(id);

        if (!bCurrent && !requested)
            bAns = false;
        else if (!bCurrent && requested)
            bAns = true;
        else if (bCurrent && requested)
            bAns = false;
        else if (bCurrent && !requested)
            bAns = false;

        return bAns;
    }
    void ExecuteUserReassignment(int id)
    {
        try
        {
            if (id > 0)
                Engine.LeadReAssignmentRuleActions.Enable(id);

            using (var x = new RaQueryParser())
                x.Run();
        }
        catch (Exception ex)
        {
            lblMessageForm.SetStatus(ex);
            lblMessageGrid.SetStatus(ex);
        }
        finally
        {
            string sMsg = "User reassignment process executed";
            ProcessInitiatorId = 0;
            ProcessState = RetentionProcess.Executed;
            WasProcessInitiatorEnabled = false;
            lblMessageGrid.SetStatus(sMsg);
            lblMessageForm.SetStatus(sMsg);
        }
    }
    private bool CanEnableRule(int id)
    {
        bool Ans = true;
        var rule = Engine.LeadReAssignmentRuleActions.Get(id);
        if (rule != null && !rule.IsActive)
            Ans = Engine.FilterAreaActions.GetAll().Where(c => c.ParentKey == id && c.ParentType == (short)Konstants.FilterParentType.Reassignment).Count() > 0;
        return Ans;
    }


    private void AskUserToInitiateProcess(int id)
    {
        long lcount = GetAffectedRecordCount(id);
        //Preserve the state
        ProcessState = RetentionProcess.QueriedByUser;
        ProcessInitiatorId = id;

        //ProcessState = Engine.LeadReAssignmentRuleActions.IsEnabled(id);

        string msg = string.Format("With the current settings, {0} {1} would get affected. Are you sure you want to continue?", lcount.ToString(), lcount <= 1 ? "record" : "records");
        Master.WindowManager.RadConfirm(msg, "fnProcessConsent", 330, 180, null, "Retention Process Confirmation", null);
    }

    private long GetAffectedRecordCount(int id)
    {
        long lAns = default(long);
        bool bEnabld = Engine.LeadReAssignmentRuleActions.IsEnabled(id);

        try
        {
            using (var X = new RaQueryParser())
                lAns = X.Run(false);

            Engine.LeadReAssignmentRuleActions.ToggleState(id);
            using (var X = new RaQueryParser())
                lAns = X.Run(false) - lAns;

            Engine.LeadReAssignmentRuleActions.ToggleState(id);

            //YA[21 May, 2014] Create the store procedure with original change.
            using (var X = new RaQueryParser())
                X.Run();

        }
        catch (Exception ex)
        {
            if (Engine.LeadReAssignmentRuleActions.IsEnabled(id) != bEnabld)
                Engine.LeadReAssignmentRuleActions.ToggleState(id);
        }
        return lAns;
    }
    void RollBackProcess()
    {
        if (GetPageMode() == PageDisplayMode.GridQueueTemplate)
        {
            BindGrid();
            lblMessageGrid.SetStatus(Messages.NoRecordModified);
        }
        else
        {
            chkEnabled.Checked = WasProcessInitiatorEnabled;
            lblMessageForm.SetStatus("Records were not updated. Rule was reverted to its previous state");
        }

        ProcessState = RetentionProcess.NotStarted;
        ProcessInitiatorId = 0;

    }

    #endregion

  
}


