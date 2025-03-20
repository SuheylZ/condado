using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using OLEBridge.Properties;
using OLEBridge.Functionality;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace OLEBridge.Forms
{
    public partial class AppForm : Form
    {
        NLog.Logger _log = NLog.LogManager.GetLogger(Resources.LogTitle);
        ArcHubClient _client = null;

        public AppForm()
        {
            InitializeComponent();
            AppIconTaskbar.Text = Resources.Title;
            label1.Text = SCService.Instance.UserName;
            _client = new ArcHubClient(AppConfiguration.HubUrl, AppConfiguration.HubName);
            _client.Register();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.Hide();

        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);  // SZ Sept 8, 2014:  if is can be removed with any side effects, plz do.
            if (WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void AppIconTaskbar_DoubleClick(object sender, EventArgs e)
        {
            if (!this.Visible && WindowState == FormWindowState.Minimized)
            {
                this.Show();
                WindowState = FormWindowState.Normal;
            }
            else
            {
                WindowState = FormWindowState.Minimized;
                this.Hide();
            }

        }

        private void mnuExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        private bool ExitApplication()
        {
            bool bExit = false;
            if (MessageBox.Show(Resources.ExitMessage, Resources.ExitTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Warning ) == System.Windows.Forms.DialogResult.Yes){
                                _log.Info(Messages.EXIT_LOG);
                                _client.Dispose();
                bExit = true;
            }
            return bExit;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = !ExitApplication();
        }

        private void mnuLog_Click(object sender, EventArgs e)
        {
            if (Visible == false)
            {
                Show();
                WindowState = FormWindowState.Normal;
            }
            Focus();
        }


   };
}
