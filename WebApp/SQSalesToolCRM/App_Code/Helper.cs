using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Web;
using Telerik.Web.UI;
using DBG = System.Diagnostics.Debug;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Reflection;


/// <summary>
/// Summary description for Helper
/// </summary>
public class Helper
{
    public static string ConvertMaskToPlainText(string v)
    {
        try
        {
            return v.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
        }
        catch
        {
            return "";
        }
    }

    public static string GetPhoneWindowScript(string phone, string outpulseId)
    {
        if (phone.Trim().Length < 10)
            return "";

        string url = string.Format("http://localhost:9998/makeCall?number={0}&campaignId={1}&checkDnc=true", phone, outpulseId);
        return string.Format("window.open('{0}','_blank','height=400, width=800,status= no, resizable= no, scrollbars=no, toolbar=no,location=no,menubar=no ');", url);
    }

    public static DateTime AddTimeSpantoDate(DateTime dateTime, string unit)
    {
        int minutes = 0;
        unit = unit.ToLower();

        if (unit.Contains("minute"))
            minutes = Helper.SafeConvert<int>(unit.Replace("minutes", "").Replace("minute", ""));
        else if (unit.Contains("hour"))
            minutes = Helper.SafeConvert<int>(unit.Replace("hours", "").Replace("hour", "")) * 60;
        else if (unit.Contains("day"))
            minutes = Helper.SafeConvert<int>(unit.Replace("days", "").Replace("day", "")) * 60 * 24;
        else if (unit.Contains("week"))
            minutes = Helper.SafeConvert<int>(unit.Replace("weeks", "").Replace("week", "")) * 60 * 24 * 7;

        return dateTime.AddMinutes(minutes);
    }
    public static DateTime AddTimeSpantoDate(DateTime? dateTime, TimeSpan? timeSpan)
    {
        if (dateTime == null)
        {
            dateTime = DateTime.Today;
        }

        if (timeSpan == null)
        {
            timeSpan = DateTime.Now.TimeOfDay;
        }

        return dateTime.Value.Add(timeSpan.Value);
    }

    public static DateTime ConvertTimeToUtc(DateTime? dateTime, TimeSpan? timeSpan, int? timeZoneId, int ostTime, int dstTime)
    {
        if (timeSpan != null)
        {
            dateTime = dateTime.Value.Add(timeSpan.Value);

            dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(dstTime) : dateTime.Value.AddHours(ostTime);

            //switch (timeZoneId.Value)
            //{
            //    case 1:
            //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(dstTime) : dateTime.Value.AddHours(ostTime);
            //        break;
            //    case 2:
            //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(dstTime) : dateTime.Value.AddHours(ostTime);
            //        break;
            //    case 3:
            //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(dstTime) : dateTime.Value.AddHours(ostTime);
            //        break;
            //    case 4:
            //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(dstTime) : dateTime.Value.AddHours(ostTime);
            //        break;
            //    case 5:
            //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(dstTime) : dateTime.Value.AddHours(ostTime);
            //        break;
            //    case 6:
            //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(dstTime) : dateTime.Value.AddHours(ostTime);
            //        break;
            //    default:
            //        break;
            //}
        }
        return dateTime.Value;
    }

    public static DateTime ConvertTimeFromUtc(DateTime? dateTime, int? timeZoneId, int ostTime, int dstTime)
    {
        //TM [22 09 2014] Commented out the existing code and used the code from within switch cases.

        //DateTime existingDateTime = DateTime.Now;
        //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Pacific SA Standard Time");

        //DateTime newDateTime = TimeZoneInfo.ConvertTime(existingDateTime, timeZoneInfo);


        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(-dstTime) : dateTime.Value.AddHours(-ostTime);
        

        //switch (timeZoneId.Value)
        //{
        //    case 1:
        //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(-dstTime) : dateTime.Value.AddHours(-ostTime);
        //        break;
        //    case 2:
        //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(-dstTime) : dateTime.Value.AddHours(-ostTime);
        //        break;
        //    case 3:
        //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(-dstTime) : dateTime.Value.AddHours(-ostTime);
        //        break;
        //    case 4:
        //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(-dstTime) : dateTime.Value.AddHours(-ostTime);
        //        break;
        //    case 5:
        //        //TimeZoneInfo timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("ALASKAN Standard Time");
        //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(-dstTime) : dateTime.Value.AddHours(-ostTime);
        //        break;
        //    case 6:
        //        dateTime = TimeZoneInfo.Local.IsDaylightSavingTime(DateTime.Now) ? dateTime.Value.AddHours(-dstTime) : dateTime.Value.AddHours(-ostTime);
        //        break;
        //    default:
        //        break;
        //}

        return dateTime.Value;
    }
    public static Nullable<T> NullConvert<T>(string s) where T : struct
    {
        Nullable<T> temp = null;


        if (!(string.IsNullOrEmpty(s) || string.IsNullOrWhiteSpace(s)))
        { // dropdowns are using selectedvalue=0 // && s!="0" && s!="0.0")
            bool bConverted = false;
            temp = SafeConvert<T>(s, ref bConverted);
            if (!bConverted)
                temp = null;
        }

        return temp;
    }
    public static T SafeConvert<T>(string s)
    {
        T Ans = default(T);
        object temp = null;

        if (s != string.Empty)
        {
            if (Ans is short)
            {
                short si = 0;
                short.TryParse(s, out si);
                temp = si;
            }
            if (Ans is int)
            {
                int i = 0;
                int.TryParse(s, out i);
                temp = i;
            }
            else if (Ans is long)
            {
                long l = 0L;
                long.TryParse(s, out l);
                temp = l;
            }
            else if (Ans is decimal)
            {
                decimal d = default(decimal);
                decimal.TryParse(s, out d);
                temp = d;
            }
            else if (Ans is DateTime)
            {
                DateTime dt = default(DateTime);
                DateTime.TryParse(s, out dt);
                temp = dt;
            }
            else if (Ans is byte)
            {
                byte b = default(byte);
                byte.TryParse(s, out b);
                temp = b;
            }
            else if (Ans is bool)
            {
                bool b = default(bool);
                bool.TryParse(s, out b);
                temp = b;
            }
            Ans = (T)temp;
        }
        return Ans;
    }
    public static T SafeConvert<T>(string s, ref bool success)
    {
        T Ans = default(T);
        object temp = null;

        if (s != string.Empty)
        {
            if (Ans is short)
            {
                short si = 0;
                success = short.TryParse(s, out si);
                temp = si;
            }
            if (Ans is int)
            {
                int i = 0;
                success = int.TryParse(s, out i);
                temp = i;
            }
            else if (Ans is long)
            {
                long l = 0L;
                success = long.TryParse(s, out l);
                temp = l;
            }
            else if (Ans is decimal)
            {
                decimal d = default(decimal);
                success = decimal.TryParse(s, out d);
                temp = d;
            }
            else if (Ans is DateTime)
            {
                DateTime dt = default(DateTime);
                success = DateTime.TryParse(s, out dt);
                temp = dt;
            }
            else if (Ans is byte)
            {
                byte b = default(byte);
                success = byte.TryParse(s, out b);
                temp = b;
            }
            Ans = (T)temp;
        }
        return Ans;
    }
    /*
     * SZ [May 1, 2013] 
     * 
     * --comment removed --
     * the above 2 fucntions take care of every thing. In fact it's only the 1 function. 
     * 
     * --comment removed --
     * 
     

    public static decimal? ConvertToNullDecimal(string v)
    {
        return v == string.Empty ? (decimal?)null : SafeConvert<decimal>(v);
    }
    public static int? ConvertToNullInt(string v)
    {
        //int? Ans = null;
        //int temp = 0;
        //if (int.TryParse(v, out temp))
        //    Ans = temp;
        return v == string.Empty ? (int?)null : SafeConvert<int>(v);
    }
    public static long? ConvertToNullLong(string v)
    {
        return v == string.Empty ? (long?)null : SafeConvert<long>(v);
    }
     for event calendar
    public static int ConvertToMinutes(string unit)
    {
        int mins = 0;
        unit = unit.ToLower();
        if (unit.Contains("minute"))
        {
            mins = Helper.ConvertToInt(unit.Replace("minutes", "").Replace("minute", ""));
        }
        else if (unit.Contains("hour"))
        {
            mins = Helper.ConvertToInt(unit.Replace("hours", "").Replace("hour", "")) * 60;
        }
        else if (unit.Contains("day"))
        {
            mins = Helper.ConvertToInt(unit.Replace("days", "").Replace("day", "")) * 60 * 24;
        }
        else if (unit.Contains("week"))
        {
            mins = Helper.ConvertToInt(unit.Replace("weeks", "").Replace("week", "")) * 60 * 24 * 7;
        }
        return mins;
    }
    public static string ConvertMaskToPlainText(string v)
    {
        try
        {
            return v.Replace(" ", "").Replace("(", "").Replace(")", "").Replace("-", "");
        }
        catch
        {
            return "";
        }
    }

    public static DateTime? ConvertToNullDate(object v)
    {
        try
        {
            return DateTime.Parse(v.ToString());
        }
        catch
        {
            return null;
        }
    }

    public static DateTime? ConvertToNullDate(string v)
    {
        DateTime? Ans = null;
        DateTime dtAns = DateTime.MinValue;
        if (DateTime.TryParse(v, out dtAns))
            Ans = dtAns;
        return Ans;
        //return Helper.ConvertToNullDate((object)v);
    }

    public static string ConvertToString(object v)
    {
        return (v?? "").ToString();
        //try
        //{
        //    return v.ToString();
        //}
        //catch
        //{
        //    return "";
        //}
    }

    public static int ConvertToInt(object v){
        try
        {
            return int.Parse(v.ToString());
        }
        catch
        {
            return 0;
        }
    }

    public static int ConvertToInt(string v)
    {
        int iAns = 0;
        int.TryParse(v, out iAns);
        return iAns;
        //return Helper.ConvertToInt((object)v);
    }

    public static int? ConvertToNullInt(object v)
    {
        try
        {
            return int.Parse(v.ToString());
        }
        catch
        {
            return null;
        }
    }
    public static long ConvertToLong(string v)
    {
        long l = default(long);
        long.TryParse(v, out l);
        return l;
        //try
        //{
        //    return long.Parse(v.ToString());
        //}
        //catch
        //{
        //    return 0;
        //}
    }

    public static long ConvertToLong(string v)
    {

        return Helper.ConvertToLong((object) v);
    }

    public static long? ConvertToNullLong(object v)
    {
        try
        {
            return long.Parse(v.ToString());
        }
        catch
        {
            return null;
        }
    }

    public static decimal ConvertToDecimal(object v)
    {
        try
        {
            return decimal.Parse(v.ToString());
        }
        catch
        {
            return 0M;
        }
    }

    public static decimal? ConvertToDecimal(string s)
    {
        bool bSuccess = false;
        decimal temp = default(decimal);
        bSuccess = decimal.TryParse(s, out temp);

        return bSuccess ? temp : (decimal?)null;
    }
    public static decimal? ConvertToNullDecimal(object v)
    {
        try
        {
            return decimal.Parse(v.ToString());
        }
        catch
        {
            return null;
        }
    }
    */


    public static IQueryable<object> SortRecords(IQueryable<object> source, string dataColumn = "", bool bAscending = true)
    {
        if (dataColumn != string.Empty)
            source = source.OrderBy(dataColumn + (bAscending ? "" : " desc"));
        return source;
    }
    public static string RemoveParenthesis(string source)
    {
        DBG.Assert(source != string.Empty);

        int iStart = 0, iEnd = 0;
        iStart = source.IndexOf('(');
        iEnd = source.IndexOf(')');
        iEnd = iEnd > 0 ? iEnd : source.Length;
        if (iStart > 0 && iEnd > 0)
            source = source.Remove(iStart, iEnd - iStart + 1).Trim();
        return source;
    }

    public static void EnableControls(Control top, bool bEnable = true)
    {
        if (top != null)
        {
            if (top is WebControl)
                (top as WebControl).Enabled = false;

            foreach (Control C in top.Controls)
            {
                if (C.HasControls())
                    Helper.EnableControls(C, bEnable);
                if (C is WebControl)
                {
                    (C as WebControl).Enabled = bEnable;
                    if (C is Button)
                        (C as Button).Visible = false;
                }
            }
        }
    }

    /// <summary>
    /// Assign Selected Value to DropDown control if it exists in its items collection.
    /// </summary>
    /// <param name="control">DropDown Control (RadDropDown,DropDownList)</param>
    /// <param name="selectedValue">Value to set</param>
    public static void SafeAssignSelectedValueToDropDown(Control control, string selectedValue)
    {
        // SZ [Nov 1, 2013] Function written by Muzamil H, compacted the code and removed the initial check of empty string 
        // as that will not allow the -1 to get selected.
        if (control is DropDownList)
        {
            var obj = control as DropDownList;
            obj.ClearSelection();
            if (obj.Items.FindByValue(selectedValue ?? "") != null)
                obj.SelectedValue = selectedValue;
            else
                if (obj.Items.FindByValue("-1") != null)
                    obj.SelectedValue = "-1";
        }
        else if (control is Telerik.Web.UI.RadDropDownList)
        {
            var obj = control as Telerik.Web.UI.RadDropDownList;
            obj.ClearSelection();
            if (obj.FindItemByValue(selectedValue) != null)
                obj.SelectedValue = selectedValue;
            else
                if (obj.FindItemByValue("-1") != null)
                    obj.SelectedValue = "-1";
        }
    }

}

public class ApplicationPages : IDisposable
{
    System.Collections.Generic.Dictionary<string, string> _pages = null;

    public void Init(string path)
    {
        System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
        doc.Load(path);
        var Nodes = doc.ChildNodes[1].ChildNodes;
        _pages = new Dictionary<string, string>(Nodes.Count);
        foreach (System.Xml.XmlNode node in Nodes)
            _pages.Add(node.Attributes["name"].Value.Trim(),
                "~//Administration//" + node.Attributes["page"].Value.Trim());
    }

    public void Dispose()
    {
        _pages.Clear();
        _pages = null;
    }

    public string this[string index]
    {
        get
        {
            return _pages[index];
        }
    }
};

//SZ [Apr 9, 2013] This has been created to give unified access to the Application setting in the web.config
// 1. Only provide read only properties 
// 2. Do not do any exception handling, the caller must take care of the things
// 3. The property should return the correct type not "object"
public class ApplicationSettings
{
    static int GetIntFromConfig(string name)
    {
        int i = default(int);
        int.TryParse(System.Configuration.ConfigurationManager.AppSettings[name], out i);
        return i;
    }

    // SZ [Sep 20, 2013] Added for TCPA settings
    // SZ [Oct 3, 2013] Removed as the TCPA settings are now determined through the presence of <phoneValidation/> tag in web.config

    //public static bool IsTCPAEnabled{    get { return GetIntFromConfig("TCPAEnabled") == 1; }   }


    //YA[26 Dec, 2013]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool CanRunEmailUpdater
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(Konstants.K_RUN_EMAIL_QUEUE);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool DefaultCalendarDismiss
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(Konstants.K_Default_Calendar_Dismiss);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string ApplicationServiceURL
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_Application_Service_URL);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static int IdleTimeOut
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_IDLE_TIMEOUT);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static int PopupLogOutTimer
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_Popup_LogOut_Timer);
        }
    }

    //YA[April 22, 2013] Log file path 
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string LogFilePath
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_LOG_FILE_PATH);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool CanRunPostQueueUpdater
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(Konstants.K_RUN_POST_QUEUE);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static int DefaultCampaignId
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_ACCOUNT_DEFAULT_CAMPAIGN_ID);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static int DefaultStatusId
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_ACCOUNT_DEFAULT_STATUS_ID);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string DefaultQuery
    {
        get
        {
            string strvalue = SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_DEFAULT_QUERY);
            if (string.IsNullOrEmpty(strvalue))
            {
                strvalue = System.Configuration.ConfigurationManager.AppSettings[Konstants.K_DEFAULT_QUERY] ?? string.Empty;
            }
            return strvalue;
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    //YA[April 22, 2013] Default query for PL 
    public static string DefaultQueryForPL
    {
        get
        {
            string strvalue = SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_DEFAULT_QUERY_PL);
            if (string.IsNullOrEmpty(strvalue))
            {
                strvalue = System.Configuration.ConfigurationManager.AppSettings[Konstants.K_DEFAULT_QUERY_PL] ?? string.Empty;
            }
            return strvalue;
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    //IH[November 07, 2013] Default query for RA [Reassignment] 
    public static string DefaultQueryForRA
    {
        get
        {
            string strvalue = SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_DEFAULT_QUERY_RA);
            if (string.IsNullOrEmpty(strvalue))
            {
                strvalue = System.Configuration.ConfigurationManager.AppSettings[Konstants.K_DEFAULT_QUERY_RA] ?? string.Empty;
            }
            return strvalue;
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    //YA[August 29, 2013] Default query for Duplicate checking 
    public static string DefaultQueryForDuplicateCheck
    {
        get
        {
            string strvalue = SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_DEFAULT_QUERY_DC);
            if (string.IsNullOrEmpty(strvalue))
            {
                strvalue = System.Configuration.ConfigurationManager.AppSettings[Konstants.K_DEFAULT_QUERY_DC] ?? string.Empty;
            }
            return strvalue;
        }
    }

    [Obsolete("Use ApplicationSetting property at DBEngine ")]

    public static int InsuranceType
    {
        get
        {
            //int Ans = 0;
            //int.TryParse(System.Configuration.ConfigurationManager.AppSettings[Konstants.K_INSURANCE_TYPE] ?? "", out Ans);
            //return Ans;
            //return ApplicationType;
            const string K_Application_Type = "APPLICATION_TYPE";
            return SalesTool.Common.CGlobalStorage.Instance.Get<int>(K_Application_Type);
        }
    }
    //YA[12 Dec, 2013]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemAPIKey
    {
        get
        {
            const string K_PHONE_SYSTEM_API_KEY = "PHONE_SYSTEM_API_KEY";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_API_KEY);
        }
    }
    //YA[31 Jan, 2014]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneQuerySkillSwap
    {
        get
        {
            const string K_PHONE_QUERY_SKILLSWAP = "PHONE_QUERY_SKILLSWAP";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_QUERY_SKILLSWAP);
        }
    }
    //YA[12 Dec, 2013]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemType
    {
        get
        {
            const string K_PHONE_SYSTEM_TYPE = "PHONE_SYSTEM_TYPE";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_TYPE);
        }
    }
    //YA[13 Dec, 2013]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemAPIGrantType
    {
        get
        {
            const string K_PHONE_SYSTEM_GRANT_TYPE = "PHONE_SYSTEM_API_GRANT_TYPE";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_GRANT_TYPE);
        }
    }


    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey1Label
    {
        get
        {
            const string K_PHONE_SYSTEM_HK1_Label = "Phone_System_HK1_Lbl";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK1_Label);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey1Type
    {
        get
        {
            const string K_PHONE_SYSTEM_HK1_Type = "Phone_System_HK1_Type";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK1_Type);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey1ID
    {
        get
        {
            const string K_PHONE_SYSTEM_HK1_ID = "Phone_System_HK1_ID";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK1_ID);
        }
    }

    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey2Label
    {
        get
        {
            const string K_PHONE_SYSTEM_HK2_Label = "Phone_System_HK2_Lbl";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK2_Label);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey2Type
    {
        get
        {
            const string K_PHONE_SYSTEM_HK2_Type = "Phone_System_HK2_Type";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK2_Type);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey2ID
    {
        get
        {
            const string K_PHONE_SYSTEM_HK2_ID = "Phone_System_HK2_ID";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK2_ID);
        }
    }

    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey3Label
    {
        get
        {
            const string K_PHONE_SYSTEM_HK3_Label = "Phone_System_HK3_Lbl";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK3_Label);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey3Type
    {
        get
        {
            const string K_PHONE_SYSTEM_HK3_Type = "Phone_System_HK3_Type";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK3_Type);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey3ID
    {
        get
        {
            const string K_PHONE_SYSTEM_HK3_ID = "Phone_System_HK3_ID";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK3_ID);
        }
    }


    // ///////////////////////////


    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey4Label
    {
        get
        {
            const string K_PHONE_SYSTEM_HK4_Label = "Phone_System_HK4_Lbl";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK4_Label);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey4Type
    {
        get
        {
            const string K_PHONE_SYSTEM_HK4_Type = "Phone_System_HK4_Type";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK4_Type);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey4ID
    {
        get
        {
            const string K_PHONE_SYSTEM_HK4_ID = "Phone_System_HK4_ID";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK4_ID);
        }
    }

    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey5Label
    {
        get
        {
            const string K_PHONE_SYSTEM_HK5_Label = "Phone_System_HK5_Lbl";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK5_Label);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey5Type
    {
        get
        {
            const string K_PHONE_SYSTEM_HK5_Type = "Phone_System_HK5_Type";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK5_Type);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey5ID
    {
        get
        {
            const string K_PHONE_SYSTEM_HK5_ID = "Phone_System_HK5_ID";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK5_ID);
        }
    }

    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey6Label
    {
        get
        {
            const string K_PHONE_SYSTEM_HK6_Label = "Phone_System_HK6_Lbl";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK6_Label);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey6Type
    {
        get
        {
            const string K_PHONE_SYSTEM_HK6_Type = "Phone_System_HK6_Type";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK6_Type);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemHotKey6ID
    {
        get
        {
            const string K_PHONE_SYSTEM_HK6_ID = "Phone_System_HK6_ID";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_HK6_ID);
        }
    }


    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemAPIUsername
    {
        get
        {
            const string K_PHONE_SYSTEM_API__USERNAME = "PHONE_SYSTEM_API__USERNAME";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_API__USERNAME);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemAPIPassword
    {
        get
        {
            const string K_PHONE_SYSTEM_API__PASSWORD = "PHONE_SYSTEM_API__PASSWORD";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_API__PASSWORD);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string PhoneSystemAPIOutpulseID
    {
        get
        {
            const string K_PHONE_SYSTEM_API__OUTPULSE_ID = "PHONE_SYSTEM_API__OUTPULSE_ID";
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_PHONE_SYSTEM_API__OUTPULSE_ID);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool PhoneBarGAL
    {
        get
        {
            const string K_Phone_Bar_GAL = "PHONE_BAR_GAL";
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(K_Phone_Bar_GAL);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool HasButtonQuote
    {
        get
        {
            const string K_APPLICATION_BUTTON_QUOTE = "APPLICATION_BUTTON_QUOTE";
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(K_APPLICATION_BUTTON_QUOTE);
        }
    }
    public static string ADOConnectionString
    {
        get
        {
            return System.Configuration.ConfigurationManager.ConnectionStrings[Konstants.K_ADO_CONNECTION].ConnectionString;
        }
    }
    /// <summary>
    /// Lead Default source code from Application setting
    /// </summary>
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string SourceCode
    {
        get
        {
            const string K_Default_Source_Code = "default_source_code";
            string code = SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_Default_Source_Code);
            return code;
        }
    }

    //SZ [Nov 21, 2013] not needed any more the Db engine can now build them dynamically
    //public static string AdminEF
    //{
    //    get
    //    {
    //        return System.Configuration.ConfigurationManager.ConnectionStrings[Konstants.K_ADMIN_CNN].ConnectionString;
    //    }
    //}
    //public static string DashboardEF
    //{
    //    get
    //    {
    //        return System.Configuration.ConfigurationManager.ConnectionStrings[Konstants.K_DASHBOARD_CNN].ConnectionString;
    //    }
    //}
    //public static string LeadEF
    //{
    //    get
    //    {
    //        return System.Configuration.ConfigurationManager.ConnectionStrings[Konstants.K_LEAD_CNN].ConnectionString;
    //    }
    //}
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool ShowEventPopup
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(Konstants.K_SHOW_EVENT_POPUP);
        }
    }

    public static Konstants.UseDuplicateManagementFeature CanUseDuplicateManagementFeature
    {
        get
        {
            Konstants.UseDuplicateManagementFeature nDuplicateFeature = new Konstants.UseDuplicateManagementFeature();
            string Ans = SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_USE_DUPLICATE_MANAGEMENT_FEATURE);
            //System.Configuration.ConfigurationManager.AppSettings[Konstants.K_USE_DUPLICATE_MANAGEMENT_FEATURE] ?? string.Empty;
            if (!string.IsNullOrEmpty(Ans))
            {
                switch (Ans)
                {
                    case "Basic":
                        nDuplicateFeature = Konstants.UseDuplicateManagementFeature.Basic;
                        break;
                    case "Posted":
                        nDuplicateFeature = Konstants.UseDuplicateManagementFeature.Posted;
                        break;
                    case "User":
                        nDuplicateFeature = Konstants.UseDuplicateManagementFeature.User;
                        break;
                    case "Both":
                        nDuplicateFeature = Konstants.UseDuplicateManagementFeature.Both;
                        break;
                    default:
                        nDuplicateFeature = Konstants.UseDuplicateManagementFeature.Basic;
                        break;
                }
            }
            return nDuplicateFeature;
        }
    }



    //SZ [May 1, 2013] Temporary folder and file
    public string TemporaryFolder
    {
        get
        {
            return System.IO.Path.GetTempPath();
        }
    }

    public string TemporaryFile
    {
        get
        {
            return System.IO.Path.GetTempFileName();
        }
    }
    //YA [Aug 20, 2013] 
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool IsSSOMode
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(Konstants.K_IS_SSO_MODE);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string DefaultSSOPassword
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_DEFAULT_SSO_PASSWORD);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string DefaultSSOLoginPage
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_DEFAULT_SSO_LOGINPAGE);
        }
    }
    //YA[Nov 06, 2013]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string EmailOrderClause
    {
        get
        {
            //return System.Configuration.ConfigurationManager.AppSettings[Konstants.K_EMAIL_ORDER_CLAUSE] ?? string.Empty;
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_EMAIL_ORDER_CLAUSE);
        }
    }
    //YA[26 Feb 2014]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool HasLeadNewLayout
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<bool>(Konstants.K_LEAD_NEW_LAYOUT);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string KGalHeight
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_GAL_HEIGHT);
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string NewArcCallDateFormat
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_ArcNewCallDateFormat);
        }
    }


    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string KGalWidth
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<string>(Konstants.K_GAL_WIDTH);
        }
    }

    //SZ [Sep 26, 2013] Added for TCPA Phone Validator service but can also be used elsewhere
    // 0: Senior, 1:AutoHome
    //private static int ApplicationType{  
    //    get{
    //        return InsuranceType;
    //        //const string K_APP_TYPE = "APPLICATION_TYPE";
    //        //return SalesTool.Common.CGlobalStorage.Instance.Get<int>(K_APP_TYPE);
    //    }
    //}
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool IsSenior { get { return InsuranceType == 0; } }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool IsAutoHome { get { return InsuranceType == 1; } }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool IsTermLife { get { return InsuranceType == 2; } }

    //YA[12 Dec, 2013]
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool IsPhoneSystemFive9 { get { return PhoneSystemType == "Five9"; } }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static bool IsPhoneSystemInContact { get { return PhoneSystemType == "inContact"; } }

    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static string Title { get { return "SalesTool "; } }

    // SZ [Jan 27, 2014] added the versioning on client's request
    // the major and minor are retrieved from the database while build and reviosn numbers are dynamic
    public static string Version
    {
        get
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
            sb.AppendFormat("{0}.{1}",
                SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_MAJOR_VERSION).ToString(),
                SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_MINOR_VERSION).ToString()
                );

            string myVersion = string.Empty;
            foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.FullName.Contains("SalesTool.DataAccess"))
                {
                    AssemblyName webName = a.GetName();
                    sb.AppendFormat(".{0}.{1}", webName.Version.Build.ToString(), webName.Version.Revision.ToString());
                    break;
                }
            return sb.ToString();
        }
    }

    public static string PhoneBarVersion
    {
        get
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
            sb.AppendFormat("{0}.{1}",
                SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_PHONEBAR_MAJOR_VERSION).ToString(),
                SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.K_PHONEBAR_MINOR_VERSION).ToString()
                );

            string myVersion = string.Empty;
            foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                if (a.FullName.Contains("PhoneBarUserControl"))
                {
                    AssemblyName webName = a.GetName();
                    sb.AppendFormat(".{0}.{1}", webName.Version.Build.ToString(), webName.Version.Revision.ToString());
                    break;
                }
            return sb.ToString();
        }
    }
    [Obsolete("Use ApplicationSetting property at DBEngine ")]
    public static int GetDayDifference
    {
        get
        {
            return SalesTool.Common.CGlobalStorage.Instance.Get<int>(Konstants.ARC_QUICKSAVE_DATE_INCREMENT);
        }
    }
}

public static partial class ExtensionMethods
{
    public static string GetPageName(this Page myPage)
    {
        var fi = new FileInfo(myPage.MapPath(myPage.AppRelativeVirtualPath));
        return fi.Name;
    }

    public static bool SimilarAs(this string value, string target)
    {
        return string.Compare(value ?? string.Empty, target ?? string.Empty, true) == 0;
    }

    public static bool IsDateEqual(this DateTime s, DateTime t)
    {
        bool bAns = false;
        if (s.Day == t.Day && s.Month == t.Month && s.Year == t.Year)
            bAns = true;
        return bAns;
    }

    /// <summary>
    /// Set selected value if value exist in drop-down-list
    /// </summary>
    /// <param name="downList"></param>
    /// <param name="value"></param>
    /// <param name="defaultVal"></param>
    /// <returns></returns>
    /// <author>MH: 09 2014</author>
    public static bool SetSelectedValue(this DropDownList downList, string value, string defaultVal = null)
    {
        bool isSet = false;
        if (downList != null)
        {
            if (downList.Items.FindByValue(value) != null)
            {
                downList.SelectedValue = value;
                isSet = true;
            }
            else
            {

            }
        }
        return isSet;
    }
    /// <summary>
    /// prevents exception: dropDownXXX has a SelectedValue which is invalid because it does not exist in the list of items.
    /// </summary>
    /// <param name="downList"></param>
    /// <param name="dataSource"></param>
    /// <param name="selectedValue"></param>
    /// <param name="dataTextField"></param>
    /// <param name="valueTextField"></param>
    /// <author>MH: 25 May 2014</author>
    public static void SafeBind(this DropDownList downList, object dataSource, string selectedValue = null, string dataTextField = null, string valueTextField = null)
    {
        downList.Items.Clear();
        downList.SelectedValue = null;
        if (!string.IsNullOrEmpty(dataTextField))
            downList.DataTextField = dataTextField;
        if (!string.IsNullOrEmpty(valueTextField))
            downList.DataValueField = valueTextField;
        downList.DataSource = dataSource;
        downList.DataBind();
        if (!string.IsNullOrEmpty(selectedValue))
            downList.SetSelectedValue(selectedValue);
    }

    /// <summary>
    /// 	Converts an object to the specified target type or returns the default value if
    ///     those 2 types are not convertible.
    ///     <para>
    ///     If the <paramref name="value"/> can't be convert even if the types are 
    ///     convertible with each other, an exception is thrown.</para>
    /// </summary>
    /// <typeparam name = "T"></typeparam>
    /// <param name = "value">The value.</param>
    /// <param name = "defaultValue">The default value.</param>
    /// <returns>The target type</returns>
    private static T ConvertTo<T>(this object value, T defaultValue)
    {
        if (value != null)
        {
            var targetType = typeof(T);

            if (value.GetType() == targetType) return (T)value;

            var converter = System.ComponentModel.TypeDescriptor.GetConverter(value);
            if (converter != null)
            {
                if (converter.CanConvertTo(targetType))
                    return (T)converter.ConvertTo(value, targetType);
            }

            converter = System.ComponentModel.TypeDescriptor.GetConverter(targetType);
            if (converter != null)
            {
                if (converter.CanConvertFrom(value.GetType()))
                    return (T)converter.ConvertFrom(value);
            }
        }
        return defaultValue;
    }

    /// <summary>
    /// Get Attribute from collection with respect to type
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <author>MH: 16 April 2014</author>
    public static T GetAttribute<T>(this AttributeCollection collection, string key)
    {
        if (collection != null && collection.Count > 0)
        {
            return collection[key].ConvertTo(default(T));
        }
        return default(T);
    }
    /// <summary>
    /// read string as typed
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="key"></param>
    /// <returns></returns>
    /// <author>MH: 16 April 2014</author>
    public static T ReadStringAs<T>(this System.Collections.Specialized.NameValueCollection collection, string key)
    {
        if (collection != null && collection.Count > 0)
        {
            return collection[key].ConvertTo(default(T));
        }
        return default(T);
    }
    public static SalesTool.DataAccess.GlobalAppSettings ApplicationSettings(this System.Web.HttpContext context)
    {
        const string kApplicationSettings = "ApplicationSettings";
        if (context.Application[kApplicationSettings] != null)
        {
            return context.Application[kApplicationSettings] as SalesTool.DataAccess.GlobalAppSettings;
        }
        var settings = SalesTool.DataAccess.DBEngine.ApplicationSettingsInstance;
        context.Application[kApplicationSettings] = settings;
        return settings;
    }

    public static SalesTool.DataAccess.GlobalAppSettings ApplicationSettings(this HttpApplicationState application)
    {
        const string kApplicationSettings = "ApplicationSettings";
        if (application != null)
        {
            var settings = application[kApplicationSettings] as SalesTool.DataAccess.GlobalAppSettings;
            if (settings == null)
            {
                settings = SalesTool.DataAccess.DBEngine.ApplicationSettingsInstance;
                application[kApplicationSettings] = settings;
            }
            return settings;

        }
        return null;
    }

    public static SalesTool.DataAccess.GlobalAppSettings ReloadSettings(this HttpApplicationState application)
    {
        const string kApplicationSettings = "ApplicationSettings";
        var settings = SalesTool.DataAccess.DBEngine.ApplicationSettingsInstance;
        application[kApplicationSettings] = settings;
        return settings;
    }

    ////MH:04 June 2014
    //// Null proppogation.
    //public static U IfNotNull<T, U>(this T t, Func<T, U> fn)
    //{
    //    return t != null ? fn(t) : default(U);
    //}

    public static int RecordCount(this SqlDataSource value)
    {
        System.Data.DataView dv = (System.Data.DataView)value.Select(DataSourceSelectArguments.Empty);
        return dv.Count;
    }
    /// <summary>
    /// Gets the value of the column without raising exception
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="colIndex"></param>
    /// <returns></returns>
    /// <author>MH: 19 Sep 2014</author>
    public static string GetSafeString(this System.Data.SqlClient.SqlDataReader reader, int colIndex)
    {
        if (!reader.IsDBNull(colIndex))
            return reader.GetString(colIndex);
        else
            return string.Empty;
    }

    /// <summary>
    /// Gets the value of the column without raising exception
    /// </summary>
    /// <param name="reader"></param>
    /// <param name="colIndex"></param>
    /// <returns></returns>
    /// <author>MH: 19 Sep 2014</author>
    public static object GetSafeValue(this System.Data.SqlClient.SqlDataReader reader, int colIndex)
    {
        if (colIndex < reader.FieldCount)
            if (!reader.IsDBNull(colIndex))
                return reader.GetValue(colIndex);
            else
                return string.Empty;
        return string.Empty;
    }
}

