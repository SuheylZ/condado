using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Routing;
using Microsoft.AspNet.SignalR;

namespace SignalR_Engine {
    public class Global : System.Web.HttpApplication {

        protected void Application_Start(object sender, EventArgs e) 
        {
            //TODO: no thread blocking,uncomment below line if you want acd_gal Engine functionlity it will dispach instant notification than gal_engine separate application.
            //Task.Factory.StartNew(SignalR_Engine.GAL_Engine.GAL_AcdEngine.StartACDEngine);
            try
            {
            //    SelectCareHub.RegisterInDatabase(System.Configuration.ConfigurationManager.AppSettings["url"]);
            }
            catch { }
        }

        protected void Session_Start(object sender, EventArgs e) {

        }

        protected void Application_BeginRequest(object sender, EventArgs e) {
            //try
            //{
            //    ArcHub.RegisterInDatabase(HttpContext.Current.Request.Url.ToString());
            //}
            //catch { }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e) {
            

        }

        protected void Application_Error(object sender, EventArgs e) {

        }

        protected void Session_End(object sender, EventArgs e) {

        }

        protected void Application_End(object sender, EventArgs e) {

        }
    }
}