using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using SelectCare.ArcApi;

namespace SelectCare.ARC.Client.Config
{
    public static class ApplicationSettings
    {
        static ArcSettings _settings=new ArcSettings(new ConnectionStringHelper().ConnectionString);

        private const string KeyDebug = "Debug";
        private const string KeyShowDeliveredIds = "ShowDeliveredIds";
        private const string KeyShowLogs = "ShowLogs";

        #region Moved to Application_Store

        //private const string KeyUserId = "UserId";
        //private const string KeyPassword = "Password";
        //private const string KeyBaseUrl = "BaseUrl";
        //private const string KeyRequestSize = "RequestSize";
        //public static string UserId
        //{
        //    get {return ConfigurationManager.AppSettings[KeyUserId]; }
        //}

        //public static string Password
        //{
        //    get { return ConfigurationManager.AppSettings[KeyPassword]; }
        //} 

        //public static string BaseUrl
        //{
        //    get { return ConfigurationManager.AppSettings[KeyBaseUrl]; }
        //}
        //public static int RequestSize
        //{

        //    get
        //    {
        //        bool status;
        //        var size = GenericExtensionMethod.Parse<int>(ConfigurationManager.AppSettings[KeyRequestSize], out status);
        //        return status ? size : 25;
        //    }
        //}

        #endregion

        public static string UserId
        {
            get { return _settings.UserId; }
        }

        public static string Password
        {
            get { return _settings.UserPassword; }
        }

        public static string BaseUrl
        {
            get { return _settings.BaseUrl; }
        }
        public static bool Debug
        {
            get
            {
                string val=ConfigurationManager.AppSettings[KeyDebug];
                return Convert.ToBoolean(val);
            }
        } 
        
        public static bool ShowDeliveredIds
        {
            get
            {
                string val = ConfigurationManager.AppSettings[KeyShowDeliveredIds];
                return Convert.ToBoolean(val);
            }
        }  
        
        public static bool ShowLogs
        {
            get
            {
                string val = ConfigurationManager.AppSettings[KeyShowLogs];
                return Convert.ToBoolean(val);
            }
        }

        public static int RequestSize
        {

            get
            {
                var size = _settings.RequestSize;
                return size;
            }
        }

    }
}
