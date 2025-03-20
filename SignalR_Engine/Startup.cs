using Microsoft.Owin;
using Microsoft.Owin.Cors;
using Owin;
using Microsoft.AspNet.SignalR;
using System.IO;
using System.Reflection;

[assembly: OwinStartup(typeof(SignalR_Engine.Startup))]
namespace SignalR_Engine
{
    
    public class Startup {
        public void Configuration(IAppBuilder app) {
            // Any connection or hub wire up and configuration should go here
            
            HubConfiguration hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableJSONP = true;
            //hubConfiguration.EnableJavaScriptProxies = true;
            //hubConfiguration.EnableDetailedErrors = true;


            

            //app.Map("/signalr", map =>
            //{
            //    map.UseCors(CorsOptions.AllowAll);
            //    map.RunSignalR(hubConfiguration);
            //});
            
            app.UseCors(CorsOptions.AllowAll);
            app.RunSignalR(hubConfiguration);

            //string exeFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //string webFolder = Path.Combine(exeFolder, "Web");
            //app.UseStaticFiles(webFolder);
        }
    }
}