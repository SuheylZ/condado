/*
   This class provides an interface to application global settings
 * Reads Settings from Application-Storage table
 * Reads Connection default connection string
 * Reads App-settings from app.config
 */
using System.Configuration;
using SalesTool.Common;

namespace SelectCARE.CallDataSync
{
    public class ApplicationSettings
    {
        #region Constants

        private const string K_BUSS_NO = "InContact_CallDataSync_Buss_No";
        private const string K_PASSWORD = "InContact_CallDataSync_Password";
        private const string K_REPORT_NO = "InContact_CallDataSync_ReportNo";

        #endregion

        #region Properties
        /// <summary>
        /// Gets Bus-No from Database Application-Storage Table
        /// </summary>
        public int BusNo
        {
            get { return CGlobalStorage.Instance.Get<int>(K_BUSS_NO); }
        }

        /// <summary>
        /// Gets Password from Database Application-Storage Table
        /// </summary>
        public string Password
        {
            get { return CGlobalStorage.Instance.Get<string>(K_PASSWORD); }
        }

        /// <summary>
        /// Gets Report No from Database Application-Storage Table
        /// </summary>
        public int ReportNo
        {
            get { return CGlobalStorage.Instance.Get<int>(K_REPORT_NO); }
        }

        #endregion

        #region Sigulton

        private static volatile ApplicationSettings _instance;
        private static object syncRoot = new object();

        private ApplicationSettings()
        {
        }

        /// <summary>
        /// Gets the ApplicationSettings Instance as a Singleton
        /// </summary>
        public static ApplicationSettings Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (syncRoot)
                    {
                        if (_instance == null)
                            _instance = new ApplicationSettings();
                    }
                }
                return _instance;
            }
        }

        /// <summary>
        /// Gets Default Connection string from Database.
        /// </summary>
        public string ConnectionString
        {

            get
            {
                return ConfigurationManager.ConnectionStrings["ApplicationServices"].ConnectionString;
            }

        }
        /// <summary>
        /// Gets Key value from App-Setting section of the App.config
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private static string GetKeyValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }

        #endregion

    }
}