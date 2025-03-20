using System;
using System.Configuration.Install;
using System.Reflection;


namespace SalesTool.Service
{
    // SZ [Mar 18, 2013] This is the service class that takes care of service communication with the SCM
    // the class must not do any thing else. all the functionality should go into the service engine class 
    // that takes care of database, timer and other related stuff. 

    public partial class ServiceListCreation : System.ServiceProcess.ServiceBase
    {
        public ServiceListCreation()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            DEBUG_WAIT();

            if (!ServiceEngine.Instance.Init())
                this.Stop();  
        }
        protected override void OnStop()
        {
            ServiceEngine.Instance.Dispose();
        }



        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                string parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
                System.ServiceProcess.ServiceBase.Run(new System.ServiceProcess.ServiceBase[] { new SalesTool.Service.ServiceListCreation() });
        }

        void DEBUG_WAIT()
        {
#if DEBUG
            //RequestAdditionalTime(Konstants.Wait_For_Debug);
            //System.Diagnostics.Debugger.Launch();
            //System.Threading.Thread.Sleep(Konstants.Wait_For_Debug/2);
#endif
        }
    };
}
