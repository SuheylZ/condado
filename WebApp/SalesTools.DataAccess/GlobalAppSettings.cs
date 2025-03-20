// This provides the mapping metadata for dynamic loading of corresponding key form application_storage 
// Purpose of this class is to  reduce database call per web request
// This class is used by ApplicationStorageHelper class to generate and loading of corresponding property value.
// KeyValue attribute provide the basic data required for dynamic sql generation
// KeyValue.Key represent the key value in database table.
// KeyValue.Default    Default value for nullable property
// KeyValue.Mode Application Type description 
// Design By Muzammil.H
// Creation Date: 17 March 2014
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class GlobalAppSettings
    {
        private string GetAppSettingsVal(string key)
        {
            return System.Configuration.ConfigurationManager.AppSettings[key] ?? string.Empty;
        }

        [KeyValue(Key = "runEmailQueue")]
        public bool CanRunEmailUpdater { get; set; }

        [KeyValue(Key = "DefaultCalendarDismiss")]
        public bool DefaultCalendarDismiss { get; set; }

        [KeyValue(Key = "ApplicationServiceURL")]
        public string ApplicationServiceURL { get; set; }

        [KeyValue(Key = "IdleTimeOut")]
        public int IdleTimeOut { get; set; }

        [KeyValue(Key = "PopupLogOutTimer")]
        public int PopupLogOutTimer { get; set; }

        [KeyValue(Key = "logFilePath")]
        public string LogFilePath { get; set; }


        [KeyValue(Key = "RunPostQueue")]
        public bool CanRunPostQueueUpdater { get; set; }

        [KeyValue(Key = "AccountDefaultCampaignId", Mode = GlobalSettingAppMode.ALL)]
        public int DefaultCampaignId { get; set; }

        [KeyValue(Key = "AccountDefaultStatusId", Mode = GlobalSettingAppMode.ALL)]
        public int DefaultStatusId { get; set; }

        [KeyValue(Key = "DefaultQuery")]
        public string DefaultQueryKey { get; set; }

        public string DefaultQuery
        {
            get
            {
                string val = DefaultQueryKey;
                if (string.IsNullOrEmpty(val))
                {
                    val = GetAppSettingsVal("DefaultQuery");
                }
                return val;

            }
        }

        [KeyValue(Key = "DefaultQueryForPL")]
        public string DefaultQueryForPLDb { get; set; }

        public string DefaultQueryForPL
        {
            get
            {
                string val = DefaultQueryForPLDb;
                if (string.IsNullOrEmpty(val))
                {
                    val = GetAppSettingsVal("DefaultQueryForPL");
                }
                return val;
            }
        }

        [KeyValue(Key = "DefaultQueryForRA")]
        public string DefaultQueryForRA_Db { get; set; }

        public string DefaultQueryForRA
        {
            get
            {
                string val = DefaultQueryForRA_Db;
                if (string.IsNullOrEmpty(val))
                {
                    val = GetAppSettingsVal("DefaultQueryForRA");

                }
                return val;
            }
        }

        [KeyValue(Key = "DefaultQueryForDuplicateCheck")]
        public string DefaultQueryForDuplicateCheck_Db { get; set; }

        public string DefaultQueryForDuplicateCheck
        {
            get
            {
                string val = DefaultQueryForDuplicateCheck_Db;
                if (string.IsNullOrEmpty(val))
                {
                    val = GetAppSettingsVal("DefaultQueryForDuplicateCheck");
                }
                return val;
            }
        }

        [KeyValue(Key = "APPLICATION_TYPE", Mode = GlobalSettingAppMode.ALL)]
        public int InsuranceType { get; set; }

        [KeyValue(Key = "CISCO_API_URL")]
        public string CiscoAPIUrl { get; set; }

        [KeyValue(Key = "CISCO_WEB_APP_PATH")]
        public string CiscoWebAPIPath { get; set; }


        [KeyValue(Key = "PHONE_SYSTEM_API_KEY")]
        public string PhoneSystemAPIKey { get; set; }

        [KeyValue(Key = "PHONE_QUERY_SKILLSWAP")]
        public string PhoneQuerySkillSwap { get; set; }

        [KeyValue(Key = "PHONE_SYSTEM_TYPE")]
        public string PhoneSystemType { get; set; }

        [KeyValue(Key = "PHONE_SYSTEM_ACD_BUTTON")]
        public bool PhoneSystemAcdButton { get; set; }

        /// <summary>
        /// Controls the ACD button visibility at Master Page
        /// </summary>
        public bool IsPhoneSystemAcdButtonVisible { get { return IsPhoneSystemInContact && PhoneSystemAcdButton; } }

        [KeyValue(Key = "PHONE_SYSTEM_API_GRANT_TYPE")]
        public string PhoneSystemAPIGrantType { get; set; }


        [KeyValue(Key = "PHONE_SYSTEM_API__SCOPE")]
        public string PhoneSystemAPIScope { get; set; }

        [KeyValue(Key = "PHONE_SYSTEM_API__USERNAME")]
        public string PhoneSystemAPIUsername { get; set; }



        [KeyValue(Key = "PHONE_SYSTEM_API__PASSWORD")]
        public string PhoneSystemAPIPassword { get; set; }

        [KeyValue(Key = "PHONE_SYSTEM_API__OUTPULSE_ID")]
        public string PhoneSystemAPIOutpulseID { get; set; }

        [KeyValue(Key = "APPLICATION_BUTTON_QUOTE")]
        public bool HasButtonQuote { get; set; }

        [KeyValue(Key = "APPLICATION_BUTTON_QUOTE_URL")]
        public string ButtonQuoteURL { get; set; }
        /// <summary>
        /// This property is used in ARC Web service and with screen pop url generation on InContact Phone dial successful
        /// </summary>
        [KeyValue(Key = "default_source_code", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string SourceCode { get; set; }

        [KeyValue(Key = "ShowEventPopup")]
        public bool ShowEventPopup { get; set; }


        [KeyValue(Key = "IsSSOMode")]
        public bool IsSSOMode { get; set; }


        [KeyValue(Key = "DefaultSSOPassword")]
        public string DefaultSSOPassword { get; set; }


        [KeyValue(Key = "DefaultSSOLoginPage")]
        public string DefaultSSOLoginPage { get; set; }

        [KeyValue(Key = "EmailOrderClause")]
        public string EmailOrderClause { get; set; }

        [KeyValue(Key = "SQL_LEAD_NEW_LAYOUT")]
        public bool HasLeadNewLayout { get; set; }

        [KeyValue(Key = "GAL_Height")]
        public string KGalHeight { get; set; }

        [KeyValue(Key = "ARC_NEW_CALL_DATE_FORMAT")]
        public string NewArcCallDateFormat { get; set; }

        [KeyValue(Key = "GAL_Width")]
        public string KGalWidth { get; set; }


        public bool IsSenior
        {
            get { return InsuranceType == 0; }
        }

        public bool IsAutoHome
        {
            get { return InsuranceType == 1; }
        }

        public bool IsTermLife
        {
            get { return InsuranceType == 2; }
        }

        public bool IsPhoneSystemFive9
        {
            get { return PhoneSystemType == "Five9"; }
        }

        public bool IsPhoneSystemInContact
        {
            get { return PhoneSystemType == "inContact"; }
        }

        [KeyValue(Mode = GlobalSettingAppMode.ALL)]
        public string Title
        {
            get { return "SalesTool "; }
        }

        [KeyValue(Key = "MAJOR_VERSION_NUMBER", Mode = GlobalSettingAppMode.ALL)]
        public int MajorVersion { get; set; }

        [KeyValue(Key = "MINOR_VERSION_NUMBER", Mode = GlobalSettingAppMode.ALL)]
        public int MinorVersion { get; set; }

        [KeyValue(Key = "ARC_QUICKSAVE_DATE_INCREMENT", Mode = GlobalSettingAppMode.ALL)]
        public int GetDayDifference { get; set; }

        //public string ADOConnectionString
        //{
        //    get
        //    {
        //        return System.Configuration.ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
        //    }
        //}
        [KeyValue(Mode = GlobalSettingAppMode.SQL_Split)]
        public bool IsMultipleAccountsAllowed
        {
            get { return IsTermLife && HasLeadNewLayout; }
        }

        [KeyValue(Key = "CallAttemptRequiredSeconds")]
        public int CallAttemptRequiredSeconds { get; set; }

        [KeyValue(Key = "ActionDisableSeconds")]
        public int ActionDisabledSeconds { get; set; }

        /// <summary>
        /// This property value is using in screen pop url generation on InContact Phone dial successful
        /// </summary>
        [KeyValue(Key = "GAL_SCREEN_POP_REDIRECT_TYPE")]
        public string GAL_ScreenPopRedirectionType { get; set; }

        /// <summary>
        /// This property value is used at Master page to show and hide InContact phone bar
        /// </summary>
        [KeyValue(Key = "PHONE_BAR", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsPhoneBarEnabled { get; set; }
        /// <summary>
        /// This property value is used at Master page to show and hide InContact Gal part of phone bar
        /// </summary>
        [KeyValue(Key = "PHONE_BAR_GAL", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsPhoneBarGALEnabled { get; set; }

        [KeyValue(Key = "USE_ARC_NEW_IMPLEMENTATION", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool USE_ARC_NEW_IMPLEMENTATION { get; set; }
        /// <summary>
        /// This property value is used at Master page to show and hide InContact hotkeys in phonebar.
        /// </summary>
        [KeyValue(Key = "PHONE_BAR_HK_VISIBLE", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsHotKeysVisible { get; set; }

        [KeyValue(Key = "Phone_System_HK1_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey1Label { get; set; }
        [KeyValue(Key = "Phone_System_HK1_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey1Type { get; set; }

        [KeyValue(Key = "Phone_System_HK1_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey1ID { get; set; }

        [KeyValue(Key = "Phone_System_HK2_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey2Label { get; set; }

        [KeyValue(Key = "Phone_System_HK2_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey2Type { get; set; }

        [KeyValue(Key = "Phone_System_HK2_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey2ID { get; set; }


        [KeyValue(Key = "Phone_System_HK3_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey3Label { get; set; }

        [KeyValue(Key = "Phone_System_HK3_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey3Type { get; set; }

        [KeyValue(Key = "Phone_System_HK3_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey3ID { get; set; }

        [KeyValue(Key = "Phone_System_HK4_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey4Label { get; set; }

        [KeyValue(Key = "Phone_System_HK4_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey4Type { get; set; }

        [KeyValue(Key = "Phone_System_HK4_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey4ID { get; set; }

        [KeyValue(Key = "Phone_System_HK5_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey5Label { get; set; }

        [KeyValue(Key = "Phone_System_HK5_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey5Type { get; set; }

        [KeyValue(Key = "Phone_System_HK5_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey5ID { get; set; }

        [KeyValue(Key = "Phone_System_HK6_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey6Label { get; set; }

        [KeyValue(Key = "Phone_System_HK6_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey6Type { get; set; }

        [KeyValue(Key = "Phone_System_HK6_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemHotKey6ID { get; set; }


        [KeyValue(Key = "CS_PHONE_BAR_HK_VISIBLE", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsCSHotKeysVisible { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK1_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey1Label { get; set; }
        [KeyValue(Key = "CS_Phone_System_HK1_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey1Type { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK1_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey1ID { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK2_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey2Label { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK2_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey2Type { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK2_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey2ID { get; set; }


        [KeyValue(Key = "CS_Phone_System_HK3_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey3Label { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK3_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey3Type { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK3_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey3ID { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK4_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey4Label { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK4_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey4Type { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK4_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey4ID { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK5_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey5Label { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK5_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey5Type { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK5_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey5ID { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK6_Lbl", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey6Label { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK6_Type", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey6Type { get; set; }

        [KeyValue(Key = "CS_Phone_System_HK6_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CSPhoneSystemHotKey6ID { get; set; }

        [KeyValue(Key = "Phone_System_ScreenPop", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemScreenPop { get; set; }

        [KeyValue(Key = "PHONE_SYSTEM_API__CLIENT_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemAPIClientID { get; set; }

        [KeyValue(Key = "PHONE_SYSTEM_API__SECRET", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemAPISecret { get; set; }

        [KeyValue(Key = "PHONE_SYSTEM_API_SERVICE_URI", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string PhoneSystemAPIServiceURI { get; set; }

        [KeyValue(Key = "PHONEBAR_MAJOR_VERSION_NUMBER")]
        public int PhoneBarMajorVersionNumber { get; set; }

        [KeyValue(Key = "PHONEBAR_MINOR_VERSION_NUMBER")]
        public int PhoneBarMinorVersionNumber { get; set; }

        
        [KeyValue(Key = "CISCO_PHONEBAR_ARC_SERVICE_USERNAME", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CiscoPhoneBarArcServiceUserName { get; set; }

        [KeyValue(Key = "CISCO_PHONEBAR_ARC_SERVICE_PASSWORD", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CiscoPhoneBarArcServiceUserPassword { get; set; }

        [KeyValue(Key = "CISCO_PHONEBAR_ARC_SERVICE_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CiscoPhoneBarArcServiceId { get; set; }

        [KeyValue(Key = "CISCO_PHONEBAR_ARC_SERVICE_URL", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string CiscoPhoneBarArcServiceURL { get; set; }


        [Obsolete("Use ApplicationSetting property at DBEngine ")]
        
        /// <summary>
        /// Base Address to consume ARC-API
        /// </summary>
        [KeyValue(Key = "ARC_API_BASE_ADDRESS", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ARC_API_BASE_ADDRESS { get; set; }

        /// <summary>
        /// No of record sent to ARC-API in a single request.
        /// </summary>
        [KeyValue(Key = "ARC_API_REQUEST_SIZE", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public int ARC_API_REQUEST_SIZE { get; set; }

        /// <summary>
        /// UserId for ARC Post 
        /// </summary>
        [KeyValue(Key = "ARC_API_USER_ID", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ARC_API_USER_ID { get; set; }

        /// <summary>
        /// User Password for ARC POSt
        /// </summary>
        [KeyValue(Key = "ARC_API_USER_PASSWORD", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ARC_API_USER_PASSWORD { get; set; }

        /// <summary>
        /// Controls the SelectCare Arc service Request/Response logging
        /// </summary>
        [KeyValue(Key = "ARC_API_POST_LOGGING_REAL-TIME", DefaultValue = true, Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledRealTimeArcApiLogging { get; set; }

        /// <summary>
        /// Controls the Stop Letters real-time method call to arcapi
        /// </summary>
        [KeyValue(Key = "ARC_API_POST_STOP-LETTERS_REAL-TIME", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledStopLettersRealTimeArcApiCall { get; set; }

        /// <summary>
        /// Controls the ConsentUpdate real-time method call to arcapi
        /// </summary>
        [KeyValue(Key = "ARC_API_POST_UPDATE-CONSENT_REAL-TIME", DefaultValue = true, Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledConsentUpdateRealTimeArcApiCall { get; set; }

        /// <summary>
        /// Controls the LetterLogs real-time method call to arcapi
        /// </summary>
        [KeyValue(Key = "ARC_API_POST_LETTER_LOG_REAL-TIME", DefaultValue = true, Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledLetterLogRealTimeArcApiCall { get; set; }

        /// <summary>
        /// Controls the AddAction real-time method call to arcapi
        /// </summary>
        [KeyValue(Key = "ARC_API_POST_ADD-ACTION_REAL-TIME", DefaultValue = true, Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledAddActionRealTimeApiCall { get; set; }

        /// <summary>
        /// Controls the ChangeAgent real-time method call to arcapi
        /// </summary>
        [KeyValue(Key = "ARC_API_POST_CHANGE-AGENT_LOG_REAL-TIME", DefaultValue = true, Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledChangeAgentRealTimeApiCall { get; set; }

        /// <summary>
        /// Controls the real-time ARC-API calls
        /// </summary>
        [KeyValue(Key = "ARC_API_POST_REAL-TIME", DefaultValue = true, Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledRealTimeArcApiCalls { get; set; }

        /// <summary>
        /// Controls the SelectCare ArcService logging
        /// </summary>
        [KeyValue(Key = "CANLOG ARC SERVICE", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledArcServiceLogging { get; set; }

        [KeyValue(Key = "default_post_campaign")]
        public int SC_ARC_DefaultPostCampaign { get; set; }

        [KeyValue(Key = "default_post_status")]
        public int SC_ARC_DefaultPostStatus { get; set; }

        //[KeyValue(Key = "SC_ARC_SERVICE_EMPTY_LOGGING")]
        //public bool SC_ARC_ServiceLogEmptyRequests { get; set; }

        [KeyValue(Key = "ARC_SC_TIMEZONE_ID", DefaultValue = "Pacific Standard Time", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ARC_SC_TIMEZONE_ID { get; set; }

        [KeyValue(Key = "InSide_DataSync_TimeZoneId", DefaultValue = "Central Standard Time", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string InSideCallDataSyncTimeZoneId { get; set; }

        [KeyValue(Key = "default_post_IndividualStateId", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public int SC_ARC_DefaultPostIndividualStatusId { get; set; }

        [KeyValue(Key = "LeadPrioritizationPageSize", Mode = GlobalSettingAppMode.ALL, DefaultValue = 500)]
        public int LeadPrioritizationPageSize { get; set; }

        /// <summary>
        /// This property value is used at Master page to show and hide InContact phone bar
        /// </summary>
        [KeyValue(Key = "DialerVisibility", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsDialerEnabled { get; set; }

        /// <summary>
        /// This property value is used at Master page to show and hide InContact phone bar
        /// </summary>
        [KeyValue(Key = "DialerDisableDialSeconds", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public int DisableWebGalDialLeadSeconds { get; set; }

        /// <summary>
        /// Base Address to consume ARC-API
        /// </summary>
        [KeyValue(Key = "ARCHUBPATH", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ARC_HUB_PATH { get; set; }

    }
    public class ArcGlobalSettings
    {
        [KeyValue(Key = "SqlAgentServiceName", DefaultValue = "SQLAgent$SQLEXPRESS2012", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string AgentServiceName { get; set; }

        [KeyValue(Key = "ArcCrmStatusPage", DefaultValue = "/Default.aspx", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ArcCrmStatusPage { get; set; }

        [KeyValue(Key = "ArcGalStatusPage", DefaultValue = "/GAL/default.aspx", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ArcGalStatusPage { get; set; }

        [KeyValue(Key = "ARC_SC_TIMEZONE_ID", DefaultValue = "Pacific Standard Time", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public string ARC_SC_TIMEZONE_ID { get; set; }

        [KeyValue(Key = "default_post_campaign")]
        public int SC_ARC_DefaultPostCampaign { get; set; }

        [KeyValue(Key = "default_post_status")]
        public int SC_ARC_DefaultPostStatus { get; set; }

        [KeyValue(Key = "SQL_LEAD_NEW_LAYOUT")]
        public bool HasLeadNewLayout { get; set; }

        [KeyValue(Key = "default_post_IndividualStateId", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public int SC_ARC_DefaultPostIndividualStatusId { get; set; }

        /// <summary>
        /// Controls the SelectCare ArcService logging
        /// </summary>
        [KeyValue(Key = "CANLOG ARC SERVICE", Mode = GlobalSettingAppMode.SQL | GlobalSettingAppMode.SQL_Split)]
        public bool IsEnabledArcServiceLogging { get; set; }
    }

    public class AppointmentGlobalSettings
    {
        /// <summary>
        /// This property value is used at Import Individual page to be used while account access data import
        /// </summary>
        [KeyValue(Key = "ApptCampaignID", Mode = GlobalSettingAppMode.ALL, DefaultValue = 185)]
        public int AppointmentCampaignID { get; set; }

        /// <summary>
        /// This property value is used at Import Individual page to be used while account access data import
        /// </summary>
        [KeyValue(Key = "ApptCampaignStatus", Mode = GlobalSettingAppMode.ALL, DefaultValue = 1)]
        public int AppointmentCampaignStatus { get; set; }

        /// <summary>
        /// This property value is used at Import Individual page to get drug information from DRX api
        /// </summary>
        [KeyValue(Key = "ApptDrugsService", Mode = GlobalSettingAppMode.ALL, DefaultValue = "http://cqe.condadogroup.com/QuoteEngineWebService.asmx/GetDrugInfo")]
        public string AppointmentDrugsService { get; set; }
    }

}
