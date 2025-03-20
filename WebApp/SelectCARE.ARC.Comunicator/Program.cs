using Microsoft.Owin.Cors;
using Microsoft.Owin.Hosting;
using Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC.Communicator
{
    class Program
    {
        static void Main(string[] args)
        {
            NLog.Logger log = NLog.LogManager.GetLogger(Konstants.LOGGER_NAME);
            string command = string.Empty;

            // This will *ONLY* bind to localhost, if you want to bind to all addresses use http://*:8080
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx for more information.

            string url = args[0];
            log.Trace("initializing ...");

            using (WebApp.Start(url))
            {
                log.Trace("Server is running at {0}...", url);
                log.Trace("Type exit to stop and shutdown");
            
            AGAIN:
                command = Console.ReadLine();
                if (command.Trim().ToLower() != "exit")
                    goto AGAIN;
            }
        }
    }

    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(CorsOptions.AllowAll);
            app.MapSignalR();
        }
    }

    internal class Konstants
    {
        internal const string LOGGER_NAME = "ArcLogger";
    }
}
