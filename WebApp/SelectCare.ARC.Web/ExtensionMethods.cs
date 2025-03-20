using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace SelectCare.ARC
{
    public static class ExtensionMethods
    {
        public const string UtcDateFormat = "yyyy-MM-dd H:mm:ss UTC";
        public const string CST_TimeZoneId = "Central Standard Time";
        public const string UTC_TimeZoneId = "UTC";
        public static string Truncate(this string value, int maxLength)
        {
            return value.Length <= maxLength ? value : value.Substring(0, maxLength);
        }
        public static Guid? AsGuid(this string value)
        {
            Guid id;
            if (Guid.TryParse(value, out id))
            {
                return id;
            }
            return null;
        }
        ///// <summary>
        ///// Parce utc time string to utc datetime
        ///// </summary>
        ///// <param name="date"></param>
        ///// <returns></returns>
        //public static DateTime? ConvertUTCToCST(this string date)
        //{

        //    DateTime dateTime;
        //    if (DateTime.TryParseExact(date, UtcDateFormat, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
        //    {
        //        //DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, "Central Standard Time");
        //        DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, UTC_TimeZoneId, CST_TimeZoneId);
        //        return centralDateTime;
        //    }
        //    return null;
        //}

        public static DateTime? ConvertUTCToLocal2(this string date, string localZoneId)
        {

            DateTime dateTime;
            if (DateTime.TryParseExact(date, UtcDateFormat, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
            {
                //DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, "Central Standard Time");
                DateTime centralDateTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(dateTime, UTC_TimeZoneId, localZoneId);
                return centralDateTime;
            }
            return null;
        }
        public static DateTime? ConvertUTCToLocal(this string date, string localZoneId)
        {

            DateTime dateTime;
            if (DateTime.TryParseExact(date, UtcDateFormat, System.Globalization.CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out dateTime))
            {
                TimeZoneInfo cstZone = TimeZoneInfo.FindSystemTimeZoneById(localZoneId);
                DateTime centralDateTime = TimeZoneInfo.ConvertTimeFromUtc(dateTime, cstZone);
                return centralDateTime;
            }
            return null;
        }
        public static U IfNotNull<T, U>(this T t, Func<T, U> fn)
        {
            return t != null ? fn(t) : default(U);
        }
        public static void SafeLogging(Action<string> action, string msg)
        {
            bool logged = false;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    action(msg);
                    logged = true;
                    break;
                }
                catch
                {
                    // just to avoid error.
                }
            }
            if (!logged)
                action(msg);
        }
        /// <summary>
        /// Provides uncertain exceptions handling while creating logs
        /// </summary>
        /// <param name="action"></param>
        /// <param name="msg"></param>
        public static void SafeLog(this NLog.Logger logger, string msg)
        {
            SafeLogging(logger.Info, msg);
        }
        public static void SafeLogError(this NLog.Logger logger, string msg)
        {
            SafeLogging(logger.Error, msg);
        }

        public static string Value(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }

        /// <summary>
        /// Gets the base Address of the site
        /// </summary>
        /// <param name="request"></param>
        /// <returns>www.example.com/virtual directory/</returns>
        ///<author>Muzammil H</author>
        public static string BaseAddress(this System.Web.HttpRequest request)
        {

            return request != null
                       ? System.Web.VirtualPathUtility.AppendTrailingSlash(request.Url.GetLeftPart(UriPartial.Authority) +
                                                                request.ApplicationPath)
                       : null;
            //return request != null ? request.Url.GetLeftPart(UriPartial.Authority) : null;
        }
        public static string FixRelativeUrl(this string relativeUrl)
        {
            if (string.IsNullOrEmpty(relativeUrl))
                return relativeUrl;
            
            if (!relativeUrl.StartsWith("/"))
                relativeUrl = relativeUrl.Insert(0, "/");
            
            return relativeUrl;
        }
    }
}