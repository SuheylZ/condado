using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.Service.Interfaces;

namespace SalesTool.Service.Plugins.Sample
{
    public class SampleClient: 
        SalesTool.Service.Interfaces.IClientInformation, 
        SalesTool.Service.Interfaces.IClientTask, 
        SalesTool.Service.Interfaces.IClient
    {
        public IClientTask Task{get{ return this as IClientTask;}}
        public IClientInformation Information{get{ return this as IClientInformation;}}


        public void Init(IServiceDataAccess db, IServiceLog log, string path)
        {
            log.Information("Sample has been loaded and has initialized");
        }
        public void Execute(IServiceDataAccess da, IServiceLog log, string path)
        {
            log.Information("tick tock tick tock, who runs down the clock?");
        }
        public void Dispose(){}


        public string Name{  get{ return System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;   } }
        public string Version{ get{ return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();}}
        public string Description{get{return "this is a sample plugin made for the service. It does not implement anything. the code serves to aid in developing real plugin";}}
    }
}
