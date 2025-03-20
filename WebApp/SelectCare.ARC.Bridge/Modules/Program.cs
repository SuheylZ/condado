using System;
using System.Collections.Generic;
using System.Linq;
using OLEBridge.Properties;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

using System.Web;
using System.Web.Caching;

using Resources = OLEBridge.Properties.Resources;



namespace OLEBridge.Functionality
{
    static class Program
    {
        static NLog.Logger _log = NLog.LogManager.GetLogger(Resources.LogTitle);
        static Guid UserKey { get; set; }

        private static Mutex _mutex;
        [STAThread]
        static void Main(String[] args)
        {
            bool bAllowOtherInstance = false;
         
            if (args.Length > 0)
                bAllowOtherInstance = string.Compare(Konstants.SECONDARY, args[0].Trim(), true) == 0;
            UserKey = Guid.Empty;

            if (IsAppRunning(bAllowOtherInstance))
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);

                ChecknSetDatabaseConnection();

                using (LoginForm form = new LoginForm())
                    UserKey = form.Authenticate();

                if (UserKey != Guid.Empty)
                {
                    _log.Info("user [{0}] has logged in successfully.", UserKey.ToString());
                    Application.Run(new Forms.AppForm());
                    Cleanup();
                }
                
            }
            else
                MessageBox.Show(Messages.ALREADY_RUNNING, Resources.Title, MessageBoxButtons.OK);
        }

        private static void ChecknSetDatabaseConnection()
        {
            if (string.IsNullOrEmpty(AppConfiguration.ConnectionString))
            {
                using (LocationForm frm = new LocationForm("Database Connection", AppConfiguration.ConnectionString))
                {
                    switch (frm.ShowDialog())
                    {
                        case DialogResult.OK:
                            AppConfiguration.ConnectionString = frm.Value;
                            break;
                        case DialogResult.Cancel:
                            break;
                    }
                   frm.Close();
                }
            } 
        }
        private static void Cleanup()
        {
            _log.Trace("Program has finished its execution gracefully");
            UserKey = Guid.Empty;
            _mutex.Close();
            _mutex.Dispose();
        }
        private static bool IsAppRunning(bool bAForcedAllow = false)
        {
            bool bResult = false;
            _mutex = new Mutex(true, Resources.Mutex, out bResult);
            return bResult;
        }
    }
}
