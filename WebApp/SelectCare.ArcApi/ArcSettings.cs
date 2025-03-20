using System;
using SalesTool.Common;

namespace SelectCare.ArcApi
{
    [Obsolete("Use Application settings on DBEngine class instead")]
    public class ArcSettings
    {
       
        private const string Key_BaseUrl = "ARC_API_BASE_ADDRESS";
        private const string Key_RequestSize = "ARC_API_REQUEST_SIZE";
        private const string Key_UserID = "ARC_API_USER_ID";
        private const string Key_Password = "ARC_API_USER_PASSWORD";

        private CGlobalStorage Storage;
        public ArcSettings(string connectionString)
        {
            ConnectionString = connectionString;
            //Storage = new CGlobalStorage(connectionString);
            Storage = CGlobalStorage.Instance;

        }
        private string ConnectionString { get; set; }

        public string BaseUrl
        {
            get
            {
               
                return Storage.Get<string>(Key_BaseUrl);
            }
        }

        public string UserId
        {
            get
            {
                return Storage.Get<string>(Key_UserID);
            }
        }

        public string UserPassword
        {
            get
            {
                return Storage.Get<string>(Key_Password);
            }
        }

        public int RequestSize
        {
            get
            {
                return Storage.Get<int>(Key_RequestSize);
            }
        }



    }
}