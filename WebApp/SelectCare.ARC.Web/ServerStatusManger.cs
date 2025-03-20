using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Web;
using SalesTool.DataAccess;

namespace SelectCare.ARC
{
    public class ServerStatusManger:IDisposable
    {
        private bool _isDisposed = false;
        private string AdoConnectionString { get; set; }

        /// <summary>
        /// Base Address for CRM Application.
        /// </summary>
        /// <author>MH:10 June 2014</author>
        public  string CrmBaseAddress
        {
            get { return VirtualPathUtility.RemoveTrailingSlash(HttpContext.Current.Request.BaseAddress().Replace("ArcService/", string.Empty)); }
        }
        
        /// <summary>
        /// Gets Complete Url for GAL Status page that is being evaluated for GAL Running status 
        /// </summary>
        /// <author>MH:10 June 2014</author>
        public string ArcGalStatusPageUrl
        {
            get
            {
                string path = DBEngine.ArcApplicationSettingsInstance.ArcGalStatusPage;
                string address = CrmBaseAddress + path.FixRelativeUrl();
                return address;
            }
        }
        /// <summary>
        /// Gets Complete Url for CRM application Status page that is being evaluated for CRM Application Status.
        /// </summary>
        /// <author>MH:10 June 2014</author>
        public string ArcCrMStatusPageUrl
        {
            get
            {
                string path = DBEngine.ArcApplicationSettingsInstance.ArcCrmStatusPage;
                string address = CrmBaseAddress + path.FixRelativeUrl();
                return address;
            }
        }

        public ServerStatusManger(string adoConnectionString)
        {
            AdoConnectionString = adoConnectionString;
        }

        /// <summary>
        /// Core Method that is responsible for Collectiong and Gernating for Server Application status.
        /// </summary>
        /// <returns>Returns ServiceResponse</returns>
         /// <author>MH:10 June 2014</author>
        public ServiceResponse GetServerApplicationStatuses()
        {
            var response = new ServiceResponse { ServiceStatus = new List<ServiceStatus>() };
            var initialCatalog = new ServiceStatus()
                {
                    ServiceType = "Database",
                    //ServiceName = "SelectCARE_SQL",
                    // Status = "Running"

                };
            //Calculate Database name and its status.
            initialCatalog.ServiceName = new System.Data.SqlClient.SqlConnection(AdoConnectionString).IfNotNull(p => p.Database);
            var settings = DBEngine.ArcApplicationSettingsInstance;
            initialCatalog.Status = settings != null ? EServiceStatus.Running.Value() : EServiceStatus.NotRunning.Value();
            response.ServiceStatus.Add(initialCatalog);


            var serverAgent = new ServiceStatus()
                {
                    ServiceType = "Database",
                    ServiceName = "SQL Server Agent",
                    // Status = "Running"
                };

            // Get AgentName from GlobalSettings and find its state.
            if (settings != null) serverAgent.Status = GetWinServiceStatus(settings.IfNotNull(p => p.AgentServiceName));

            response.ServiceStatus.Add(serverAgent);
            var appCRM = new ServiceStatus()
                {
                    ServiceType = "Web Application",
                    ServiceName = "SelectCARE",
                    //      Status = "Running"
                };

            appCRM.Status = GetOnlineApplicationStatus(ArcCrMStatusPageUrl);
            response.ServiceStatus.Add(appCRM);

            var appGAL = new ServiceStatus()
                {
                    ServiceType = "Web Application",
                    ServiceName = "SelectCARE GAL",
                    //    Status = "Running"
                };

            appGAL.Status = GetOnlineApplicationStatus(ArcGalStatusPageUrl);
            response.ServiceStatus.Add(appGAL);
            return response;
        }

        /// <summary>
        /// Evaluate online Application status for Given url.
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        /// <author>MH:10 June 2014</author>
        private string GetOnlineApplicationStatus(string url)
        {
            try
            {
                var myRequest = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);

                var response = (System.Net.HttpWebResponse)myRequest.GetResponse();

                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return EServiceStatus.Running.Value();
                }
                string actualStatus = string.Format("{0} Returned, but with status: {1}", url, response.StatusDescription);
                return EServiceStatus.NotRunning.Value();
            }
            catch (Exception ex)
            {
                return EServiceStatus.NotRunning.Value();
            }
        }

        /// <summary>
        /// Gets windows service running status for given svcName
        /// </summary>
        /// <param name="svcName">Service Name which is being evaluated for status</param>
        /// <returns>returns status</returns>
        /// <author>MH:10 June 2014</author>
        private string GetWinServiceStatus(string svcName)
        {
            if (string.IsNullOrEmpty(svcName))
            {
                svcName = "SQLSERVERAGENT";//default
            }
            try
            {
                ServiceController sc = new ServiceController(svcName);
                sc.Refresh();
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        return "Running";
                    case ServiceControllerStatus.Stopped:
                        return "Stopped";
                    case ServiceControllerStatus.Paused:
                        return "Paused";
                    case ServiceControllerStatus.StopPending:
                        return "Stopping";
                    case ServiceControllerStatus.StartPending:
                        return "Starting";
                    default:
                        return "Status Changing";
                }
                sc.Close();
            }
            catch (Exception e)
            {
                return "Not Running";
            }

        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_isDisposed) return;
            if (isDisposing)
            {
                // free managed resources
             
            }
            AdoConnectionString = null;
            _isDisposed = true;
        }

        #endregion
    }
}