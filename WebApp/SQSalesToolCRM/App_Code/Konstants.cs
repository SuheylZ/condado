// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      C:\Projects\Live\SQ Sales Tool\SQSalesToolCRM\
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   suheyl.z
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 
using System;

using System.Linq;

/// <summary>
/// Summary description for Konstants
/// </summary>
public class Konstants
{
    //wm
    public const double K_NEXT_HOURS = 2;

    // end wm

    //Connection strings
    public const string K_ADMIN_CNN = "AdminModel";
    public const string K_LEAD_CNN = "LeadModel";

    //session constants
    public const string K_USERID = "__user_identity__";
    public const string K_PERMISSIONS = "__user_permissions__";
    public const string K_ENGINE_KEY = "SQ_SalesDatabaseEngine";
    public const string K_TEMPORARY_FOLDER = "__temporary__";
    public const string K_REQUESTED_URL = "__requested_url__";

    // page/url constants
    public const string K_LOGINPAGE = "~//Login.aspx";
    public const string K_HOMEPAGE = "~//default.aspx";
    public const string K_MANAGEUSERSPAGE = "~//Administration//ManageUsers.aspx";
    public const string K_PRIORITIZEDLEADSPAGE = "~//Leads//viewPrioritizedLeads.aspx";
    public const string K_VIEW_LEADS_PAGE = "~//Leads//viewLeads.aspx";
    public const string K_LEADS_PAGE = "~//Leads//Leads.aspx";

    //YA[May 16, 2013] Report pages    
    public const string K_CUSTOM_REPORT_PAGE = "~//Reports//CustomReports.aspx";
    public const string K_REPORT_QUERY_STRING = "ReportID";
    public const string K_DISPLAY_REPORT_PAGE = "~/reports/reportdisplay.aspx?reportid={0}";

    //web.config constants
    public const string K_CURRENT_MODEL = "CurrentDataAccessModel";
    public const string K_ADO_CONNECTION = "ApplicationServices";
    public const string K_DEBUG_CREDENTIALS = "DebugUser";

    // Attiq - April-10-04-2014
    //public const string K_APPLICATION_TITLE = "SQ Sales Tool";
    public const string K_APPLICATION_TITLE_SQS = "SQS";
    public const string K_APPLICATION_TITLE_SQL = "SQL";
    public const string K_APPLICATION_TITLE_SQAH = "SQAH";
    public const string K_DEFAULT_QUERY = "DefaultQuery";
    public const string K_GAL_HEIGHT = "GAL_Height";
    public const string K_GAL_WIDTH = "GAL_Width";

    // Attiq - April-10-2014 Application Pages and Titles Constants.
    public const string K_DEFAULT_TITLE = " - Home Page";
    public const string K_DEFAULT_PAGE = "default.aspx";

    public const string K_NORMALVIEW_TITLE = " - Normal View";
    public const string K_NORMALVIEW_PAGE = "viewleads.aspx";

    //----------------
    public const string K_MANAGESKILL = "ManageSkillGroups.aspx";
    public const string K_MANAGESKILL_TITLE = " - Manage Skill Groups";

    public const string K_MANAGECAMPAIGN = "ManageCampaign.aspx";
    public const string K_MANAGECAMPAIGN_TITLE = " - Manage Campaigns";

    public const string K_MANAGEACTIONS = "ManageActions.aspx";
    public const string K_MANAGEACTIONS_TITLE = " - Manage Actions & Statuses";

    public const string K_MANAGEEMAILS = "ManageEmails.aspx";
    public const string K_MANAGEEMAILS_TITLE = " - Manage Emails";

    public const string K_MANAGEPOSTS = "ManagePosts.aspx";
    public const string K_MANAGEPOSTS_TITLE = " - Manage Posts";

    public const string K_MANAGEQUICKLINKS = "ManageQuickLinks.aspx";
    public const string K_MANAGEQUICKLINKS_TITLE = " - Manage Quick Links";

    public const string K_MANAGEAGENT = "AgentManager.aspx";
    public const string K_MANAGEAGENT_TITLE = " - Manage GAL";

    public const string K_MANAGEALERTS = "ManageAlerts.aspx";
    public const string K_MANAGEALERTS_TITLE = " - ManageAlerts";

    public const string K_MANAGEPRIORITIZATION = "ManagePrioritization.aspx";
    public const string K_MANAGEPRIORITIZATION_TITLE = " - Manage Prioritizations";

    public const string K_MANAGEREASSIGNMENTS = "ManageReassignments.aspx";
    public const string K_MANAGEREASSIGNMENTS_TITLE = " - Manage Reassignments";

    public const string K_MANAGEDUPLICATES = "ManageDuplicates.aspx";
    public const string K_MANAGEDUPLICATES_TITLE = " - Manage Duplicates";

    public const string K_MANAGEDUPLICATES2 = "ViewDuplicates.aspx";
    public const string K_MANAGEDUPLICATES2_TITLE = " - View Reconciliation Report";

    public const string K_MANAGEDASHBOARD = "ManageDashboard.aspx";
    public const string K_MANAGEDASHBOARD_TITLE = " - Manage Dashboard";
    //-------------------
    public const string K_PRIORITIZEDVIEW_TITLE = " - Prioritized View";
    public const string K_PRIORITIZEDVIEW_PAGE = "viewprioritizedleads.aspx";

    public const string K_NEWACCOUNT_TITLE = " - New Account";
    public const string K_NEWACCOUNT_PAGE = "leads.aspx";

    public const string K_EDITACCOUNT_TITLE = " - Account - ";

    public const string K_CALENDAR_TITLE = " - Calendar Event - Campaign Occurrence";
    public const string K_CALENDAR_PAGE = "calendar.aspx";

    public const string K_CUSTOMREPORTS_TITLE = " - Report Designer";
    public const string K_CUSTOMREPORTS_PAGE = "customreports.aspx";

    public const string K_REPORTDISPLAY_TITLE = " - Report";
    public const string K_REPORTDISPLAY_PAGE = "reportdisplay.aspx";

    public const string K_MANAGEUSERS_TITLE = " - Manage Users";
    public const string K_MANAGEUSERS_PAGE = "manageusers.aspx";

    public const string K_MANAGEROLES_TITLE = " - Manage Roles";
    public const string K_MANAGEROLES_PAGE = "manageroles.aspx";

    public const string K_OUTBOUNDROUTING_TITLE = " - Manage Outbound Routing";
    public const string K_OUTBOUNDROUTING_PAGE = "outboundrouting.aspx";

    public const string K_ADMINMYREOPRTS_TITLE = " - Administration-My Reports";
    public const string K_ADMINMYREOPRTS_PAGE = "reports.aspx";



    //YA[April 22, 2013] Default query app setting for PL.
    public const string K_DEFAULT_QUERY_PL = "DefaultQueryForPL";
    public const string K_DEFAULT_QUERY_DC = "DefaultQueryForDuplicateCheck";
    public const string K_INSURANCE_TYPE = "InsuranceType";
    public const string K_ACCOUNT_DEFAULT_CAMPAIGN_ID = "AccountDefaultCampaignId";
    public const string K_ACCOUNT_DEFAULT_STATUS_ID = "AccountDefaultStatusId";
    public const string K_RUN_EMAIL_QUEUE = "runEmailQueue";
    public const string K_RUN_POST_QUEUE = "RunPostQueue";
    public const string K_LOG_FILE_PATH = "logFilePath";
    public const string K_Default_Calendar_Dismiss = "DefaultCalendarDismiss";
    public const string K_Application_Service_URL = "ApplicationServiceURL";

    //IH[November 07, 2013] Default query app setting for RA.
    public const string K_DEFAULT_QUERY_RA = "DefaultQueryForRA";


    //roles konstants
    public const string K_ROLE_OVERRIDDEN_TEXT = " (Overridden)";
    public const string K_ROLE_RESET_N_VALIDATE_JS = "askOverride();validate();";
    public const string K_ROLE_VALIDATE_JS = "validate();";

    //SZ [Jan 23, 2013] Some constatnts for the leads
    public const string K_ACCOUNT_ID = "AccountId";
    public const string K_PARENT_ACCOUNT_ID = "parentaccountid";
    public const string K_LEAD_ID = "LeadKey";
    //YA[28 Feb 2014]
    public const string K_LEAD_INDIVIDUAL_EDIT_MODE = "IsIndividualEditMode";
    public const string K_LEAD_INDIVIDUAL_ADD_NEW_ACCOUNT = "IsIndividualAddNewAccount";

    public const string K_LEAD_LEADMARKETING_EDIT_MODE = "IsLeadMarketingEditMode";
    public const string K_LEAD_LEADMARKETING_ADD_NEW_ACCOUNT = "IsLeadMarketingAddNewAccount";

    public const string K_LEAD_INDIVIDUAL_EDIT_KEY = "IndividualEditKey";
    public const string K_LEAD_LEADMARKETING_EDIT_KEY = "LeadMarketingEditKey";

    public const string K_AVOID_REASSIGNMENT = "avoidreassignment";
    public const string K_LEAD_INDEX = "Account_Lead_Index";

    public const string K_FIRST_TIME = "First_time_user_logged_in";
    public const string K_SHOW_EVENT_POPUP = "ShowEventPopup";
    public const string K_USE_DUPLICATE_MANAGEMENT_FEATURE = "UseDuplicateManagementFeature";

    public const string K_SEARCH_BY = "by";
    public const string K_SEARCH_FOR = "for";

    public const string K_CURRENT_USER = "SalesTool_Current_User";
    public const string K_IS_SSO_MODE = "IsSSOMode";
    public const string K_DEFAULT_SSO_PASSWORD = "DefaultSSOPassword";
    public const string K_DEFAULT_SSO_LOGINPAGE = "DefaultSSOLoginPage";
    //YA[Nov 06, 2013]
    public const string K_EMAIL_ORDER_CLAUSE = "EmailOrderClause";
    public const string K_IDLE_TIMEOUT = "IdleTimeOut";
    public const string K_Popup_LogOut_Timer = "PopupLogOutTimer";

    //YA[26 Feb 2014]
    public const string K_LEAD_NEW_LAYOUT = "SQL_LEAD_NEW_LAYOUT";

    public static readonly DateTime K_Caching_Duration = DateTime.Now.AddMinutes(20);

    //YA[April 24, 2013] Added emums those will be used through out the application.    


    public const string K_DASHBOARD_CNN = "DashboardModel";

    public const string K_SCRIPT_MANAGER = "___master_page_script_manager___";
    public const string K_ArcNewCallDateFormat = "ARC_NEW_CALL_DATE_FORMAT";



    #region Application Enums



    public enum AccountPriorityView
    {
        Off = 0,
        ShowFirstSelectFirst = 1,
        ShowAllSelectFirst = 2,
        ShowAllSelectAny = 3
    }

    public enum NextAccountSettings
    {
        Off = 0,
        Topfirst = 1,
        NextRecord = 2
    }
    public enum TimeLapseType
    {
        Seconds = 1,
        Minutes = 2,
        Hours = 3
    }
    public enum FilterSelected
    { All = 0, Any = 1, Custom = 2 }
    public enum FilterParentType
    {
        Unknown = 99,
        Email = 0,
        Posts = 1,
        Prioritization = 2,
        Retention = 3,
        SubStatus2 = 4,
        CustomReport = 5,
        DuplicateCheckingForIncomingLeads = 6,
        DuplicateCheckingForExistingLeads = 7,
        Reassignment = 8

    }
    public enum FilterFieldDataType
    {
        Numeric = 0,
        Text = 1,
        Date = 2,
        Table = 3,
        Checkbox = 4,
        DateTime = 5,
    }
    public enum DateUnits
    {
        Days = 0,
        Hours = 1,
        Minutes = 2
    }
    public enum PredefinedDates
    {
        Today = 0,
        SinceMonday = 1,
        ThisCalendarMonth = 2,
        ThisCalendarYear = 3,
        InPast = 4,
        InFuture = 5
    }
    public enum OperatorValue
    {
        Equal = 0,
        NotEqual = 1,
        LessThan = 2,
        LessThanOrEqual = 3,
        GreaterThan = 4,
        GreaterThanOrEqual = 5,
        Contains = 6,
        DoesNotContains = 7,
        WithIn = 8,
        NotWithIn = 9
    }
    public enum DeliveryUnits
    {
        SendImmediately = 0,
        SendAfterTrigger = 1,
        SendBeforeOrAfterSpecificDate = 2
    }
    public enum DeliveryTimeSpan
    {
        Minutes = 0,
        Hours = 1,
        Days = 2,
        Weeks = 3
    }
    public enum EmailQueueStatus
    {
        Error = 0,
        Queued = 1,
        Delivered = 2,
        Duplicate = 3
    }
    public enum PostQueueStatus
    {
        Error = 0,
        Queued = 1,
        Posted = 2
    }
    public enum EmailDateUnits
    {
        Minutes = 0,
        Hours = 1,
        Days = 2,
        Weeks = 3
    }

    public enum EmailTriggerType
    {
        Auto = 1, Manual = 2, Both = 3
    }
    public enum PostDateUnits
    {
        Minutes = 0,
        Hours = 1,
        Days = 2,
        Weeks = 3
    }
    public enum PostTriggerType
    {
        Auto = 1, Manual = 2, Both = 3
    }

    public enum CustomReportBaseData
    {
        None = 0,
        Accounts = 1,
        AccountHistory = 2,
        LeadHistory = 3
    }
    public enum AggregateFunctionType
    {
        None = 0,
        Count = 1,
        Sum = 2,
        Min = 3,
        Max = 4,
        Average = 5
    }

    public enum ReportFilter
    {
        AssignedOnly = 0,
        SkillGroupOnly = 1,
        All = 2
    }

    public enum UseDuplicateManagementFeature
    {
        None = 0,
        Basic = 1,
        Posted = 2,
        User = 3,
        Both = 4
    }

    #endregion

    public const string K_MAJOR_VERSION = "MAJOR_VERSION_NUMBER";
    public const string K_MINOR_VERSION = "MINOR_VERSION_NUMBER";
    public const string K_PHONEBAR_MAJOR_VERSION = "PHONEBAR_MAJOR_VERSION_NUMBER";
    public const string K_PHONEBAR_MINOR_VERSION = "PHONEBAR_MINOR_VERSION_NUMBER";
    public const string APPLICATION_VERSION = "SALESTOOL_APPLICATION_VERSION";

    public const string ARC_QUICKSAVE_DATE_INCREMENT = "ARC_QUICKSAVE_DATE_INCREMENT";
}

public static class DateTimeExtensions
{
    [System.Diagnostics.DebuggerStepThrough()]
    public static DateTime LastDateOn(this DateTime dt, DayOfWeek target)
    {
        return DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek - (int)target) - 7);
    }
}

// SZ [jan 10, 2013] 
public enum FilterParentType
{
    Unknown = 99,
    EmailWebForm = 0,
    PostsWebForm = 1,
    PrioritizationWebForm = 2,
    RetentionWebForm = 3,
    SubStatus2 = 4,
    CustomReport = 5,
    DuplicateCheckingForIncomingLeads = 6,
    DuplicateCheckingForExistingLeads = 7,
    ReassignmentWebForm = 8


};
