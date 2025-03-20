using Microsoft.AspNet.SignalR.Client;
using NLog;
using System;
using System.Threading.Tasks;
using System.Configuration;
using Resources = OLEBridge.Properties.Resources;
using System.ServiceModel;
using DB = Microsoft.ApplicationBlocks.Data.SqlHelper;
using System.Data;
using System.Data.SqlClient;
using JSON = Newtonsoft.Json;


namespace OLEBridge.Functionality
{
    public class Konstants
    {
        public const string LOGGER_NAME = "Ole bridge application";

        public const string NOTIFICATION_NOT_SENT = "signalr hub could not be contacted for sending the notification";
        public const string NOTIFICATION_SENT = "notification to the client was sent successfully";
        public const string NOTIFICATION_CANCELED = "sending the notification was canceled";
        public const string SECONDARY = "secondary";
    }

    public class Messages
    {
        public const string EXIT_LOG = "User has chosen to exit the application";
        public const string ALREADY_RUNNING="Another instance is already running";
        public const string CONNECTION_INITIATED = "connecting to the SignalR at {0} with hub {1}";
        public const string CONNECTION_ESTABLISHED = "Connected to the hub, configuring command listener";

        public const string OPENED_IN_ARC = "the reference number has been opened in Arc";
        public const string NOT_OPENED_IN_ARC = "Could not open the reference number in Arc";
        public const string CONNECTION_CLOSED = "Closing the signalr connection ...";
        public const string INVALID_CREDENTIALS = "Credentials are not valid, try again";
        public const string UNKNOWN_COMMAND = "Unknown command received from the selectcare crm";
        public const string LOOLI_LANGRI_COMMAND = "The command argument is invalid: command needs wheelchair";
    }
    //public enum OleBridgeStatus { Unknown = 0, Ready = 1, Busy = 2, Disconnected = 3, Error = 4 };

    public class ArcHubClient : IDisposable
    {
        HubConnection _cnn = null;
        IHubProxy _hub = null;
        Logger _log = LogManager.GetLogger(Konstants.LOGGER_NAME);
        bool _isOk = false;
        public ArcHubClient(string url, string hub)
        {
            string queryStr = "";

            _log.Trace(Messages.CONNECTION_INITIATED, url, hub);
            _cnn = new HubConnection(url, queryStr);
            _log.Trace(Messages.CONNECTION_ESTABLISHED);
            _hub = _cnn.CreateHubProxy(hub);

            _log.Trace(Messages.CONNECTION_ESTABLISHED);

            // NewCase() creates a case in the Arc
            // OpenCase() opens a case in the Arc
            // GetStatus() gets the status of the ole bridge  OleBridgeStatus

            _hub.On<string, string>("messageReceived", (command, arg) =>
            {
                _log.Trace(string.Format("command: {0} arguments: {1}", command, arg));
                //Make a call to the arc system to create a case
                switch (command)
                {
                    case "notify":
                        _log.Info("Command Received: notify({0})", arg);
                        break;
                    case "newcase":
                        {
                            long id = default(long);
                            long.TryParse(arg, out id);
                            NewCase(id);
                        }
                        break;
                    case "opencase":
                        OpenCase(arg);
                        break;
                    case "newcase2":
                        {
                            
                            long id = default(long);
                            long.TryParse(arg, out id);
                            NewCase(id);
                        }
                        break;
                    case "opencase2":
                        {
                            try
                            {
                                string[] words = arg.Split(':');
                                OpenCase2(words[0], words[1], words[2]);
                            }
                            catch {
                                Notify(Messages.LOOLI_LANGRI_COMMAND);
                            }
                        }
                        break;
                    default:
                        Notify(Messages.UNKNOWN_COMMAND);
                        break;
                }
            });
            _hub.On("ping", () => {  });
            try
            {
                _cnn.Start().Wait();
                _isOk = true;
            }
            catch (Exception ex)
            {
                _log.Error("Could not contact message hub: {0}", ex.ToString());
            }
        }

        public void Register()
        {
            if (_isOk)
                _hub.Invoke("register", new object[] { "bridge", SCService.Instance.UserKey });
        }

        private void OpenCase(string id)
        {
            try
            {
                _log.Info("command received: opencase({0})", id);
                Arc.IArcServer svr = new Arc.ArcServer();
                svr.OpenCase(id);
                Notify(Messages.OPENED_IN_ARC);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                Notify(Messages.NOT_OPENED_IN_ARC);
            }
        }
        private void OpenCase2(string id, string acct, string indv)
        {
            string Ans = string.Empty;
            try
            {
                _log.Info("command received: opencase({0})", id);
                Arc2.IArcServer2 svr = (new Arc2.ArcServer()) as Arc2.IArcServer2;
                svr.OpenCase2(id, indv, acct, out Ans);
                Notify(string.Format("{0}:{1}", Messages.OPENED_IN_ARC,Ans));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                Notify(Messages.NOT_OPENED_IN_ARC);
            }
        }
        private void NewCase(long id)
        {
            _log.Info("command received: newcase({0}) ", id);
            try
            {
                string response = SCService.Instance.Service.GetIndividualForArc(id);
                _log.Trace(response);
                var O = JSON.JsonConvert.DeserializeObject<IndividualData>(response);
                Arc.IArcServer svr = new Arc.ArcServer();
                svr.NewCase(O.SourceCode, O.Title, O.FirstName, O.MiddleName, O.LastName, O.Suffix, O.Gender, O.State, O.Birthdate);
                Notify(Messages.OPENED_IN_ARC);
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        private void NewCase2(long id)
        {
            string refno = string.Empty,
                indv_res = string.Empty, act_res = string.Empty;

            _log.Info("command received: newcase({0}) ", id);
            try
            {
                string response = SCService.Instance.Service.GetIndividualForArc(id);
                _log.Trace(response);
                var O = JSON.JsonConvert.DeserializeObject<IndividualData>(response);
                
                Arc2.IArcServer2 svr = (new Arc2.ArcServer()) as Arc2.IArcServer2;
                svr.NewCase2(O.SourceCode, O.Title, O.FirstName, O.MiddleName, O.LastName, O.Suffix, O.Gender, O.State, O.Birthdate, O.indv_key, O.AccountKey, out refno, out indv_res, out act_res);
                Notify(string.Format("{0}:{1}:{2}:{3}", Messages.OPENED_IN_ARC, refno, indv_res, act_res));
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }
        
        
        private void Notify(string text)
        {
            string[] args = new string[] { "notify", text };
            try
            {
                _hub.Invoke<string>("sendMessage", args).ContinueWith(t =>
                {
                    if (t.IsCompleted)
                        _log.Trace(Konstants.NOTIFICATION_SENT);
                    else if (t.IsCanceled)
                        _log.Trace(Konstants.NOTIFICATION_CANCELED);
                    else if (t.IsFaulted)
                        _log.Error(Konstants.NOTIFICATION_NOT_SENT);
                });
            }
            catch (Exception ex)
            {
                _log.Error("Error in notifying: {0}", ex.Message);
            }

        }
        public void Dispose()
        {
            if (_cnn != null && _cnn.State == Microsoft.AspNet.SignalR.Client.ConnectionState.Connected && _isOk)
            {
                _log.Trace(Messages.CONNECTION_CLOSED);
                _cnn.Stop();
                _cnn.Dispose();
                _cnn = null;
            }
        }
    }

    internal class AppConfiguration
    {
        internal static string ConnectionString
        {
            set
            {
                const string SECTION_NAME = "connectionStrings";
                var config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                var section = (ConnectionStringsSection)config.GetSection(SECTION_NAME);
                section.ConnectionStrings[Resources.DBConnectionName].ConnectionString = value;
                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(SECTION_NAME);
            }
            get
            {
                return ConfigurationManager.ConnectionStrings[Resources.DBConnectionName].ConnectionString.Trim();
            }
        }

        private static string GetStringFromDB(string key)
        {
            string ans = string.Empty;
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@key", key);
            var obj = DB.ExecuteScalar(AppConfiguration.ConnectionString,
                CommandType.Text,
                "Select [tvalue] from [application_storage] where [key]=@key",
                pars
            );
            if (obj != null)
                ans = obj.ToString();
            return ans;
        }
        private static bool GetBoolFromDB(string key)
        {
            bool ans = false;
            SqlParameter[] pars = new SqlParameter[1];
            pars[0] = new SqlParameter("@key", key);
            var obj = DB.ExecuteScalar(AppConfiguration.ConnectionString,
                CommandType.Text,
                "Select [bValue] from [application_storage] where [key]=@key",
                pars
            );
            if (obj != null)
                bool.TryParse(obj.ToString(), out ans);
            return ans;
        }
        internal static string ServicePath
        {
            get
            {
                return GetStringFromDB(Resources.ServiceKey);
            }
        }

        public static string HubUrl
        {
            get
            {
                return GetStringFromDB(Resources.HubUrlKey);
            }
        }

        public static string HubName
        {
            get
            {
                string Ans = System.Configuration.ConfigurationManager.AppSettings["hubname"];
                if (string.IsNullOrEmpty(Ans))
                    Ans = "selectCareHub";
                return Ans;
            }
        }
        public static bool UseArc2
        {
            get
            {
                return GetBoolFromDB(Resources.ArcTypeKey);
            }
        }
    }

    internal class SCService {
        static SCService _this = null;
       
        SelectCare.SelectCareSoapClient _ctx = null;
        Guid _userKey = Guid.Empty;

        string _name = "";

        SCService()
        {
            dynamic binding;
            if(IsSecure)
                binding= new BasicHttpsBinding();
            else
                binding =new BasicHttpBinding();

            EndpointAddress address = new EndpointAddress(AppConfiguration.ServicePath);
            _ctx = new SelectCare.SelectCareSoapClient(binding, address);
        }
        public static SCService Instance
        {
            get
            {
                if (_this == null)
                    _this = new SCService();
                return _this;
            }
        }

        public SelectCare.SelectCareSoapClient Service { get { return _ctx; } }

        internal bool SetUser(Guid key)
        {
            bool bAns = false;
            if (key != Guid.Empty)
            {
                _userKey = key;
                _name = _ctx.GetUserName(_userKey);
                if (!string.IsNullOrEmpty(_name.Trim()))
                    bAns = true;
            }
            return bAns;
        }

        internal Guid UserKey { get { return _userKey; } }
        internal string UserName { get { return _name; } }

        internal Guid AuthenticateUser(string user, string password)
        {
            Guid ans = _ctx.AuthenticateUser(user, password);
            if (ans != Guid.Empty)
            {
                _userKey = ans;
                _name = _ctx.GetUserName(_userKey);
            }
            return ans;
        }



        public bool IsSecure
        {
            get
            {
                return AppConfiguration.ServicePath.Trim().StartsWith("https");
            }
        }
    }
}