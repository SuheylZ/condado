using System;
using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;
using System.Diagnostics;



namespace SalesTool.Service.Installer
{
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        public ProjectInstaller()
        {
            InitializeComponent();
        }

        //public override void Install(System.Collections.IDictionary stateSaver)
        //{
        //    base.Install(stateSaver);
        //}

        private void ProjectInstaller_Committed(object sender, InstallEventArgs e)
        {
            try
            {
                ServiceController myServiceController = new ServiceController("SCSalesTool");
                myServiceController.Start();
                
            }
            catch (Exception ex)
            {
                EventLog.WriteEntry(Konstants.Service_Name, string.Format(" Could not start Condado Service. You may have to start the service manually. \r\n Exception Details: {0}", ex.Message), EventLogEntryType.Error);                
            }
        }
    }
}
