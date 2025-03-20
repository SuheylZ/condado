using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Resources = OLEBridge.Properties.Resources;
using DAAB = Microsoft.ApplicationBlocks.Data.SqlHelper;
using System.Data.SqlClient;
using OLEBridge.Functionality;
using System.Security.Principal;


namespace OLEBridge
{

    enum AuthenticationType { Unknown=-1, Windows=0, Forms=2};
    public partial class LoginForm : Form
    {
        NLog.Logger _log = NLog.LogManager.GetLogger(Resources.LogTitle);
        public LoginForm()
        {
            InitializeComponent();
            txtUserId.TextChanged += (o, arg) => { btnOK.Enabled = !string.IsNullOrEmpty(this.Text); };
        }

        //private void button1_Click(object sender, EventArgs e)
        //{
        //    //Authenticate credentials here
        //    this.DialogResult = System.Windows.Forms.DialogResult.OK;
        //    Close();
        //}

        //private void button2_Click(object sender, EventArgs e)
        //{
        //    DialogResult = System.Windows.Forms.DialogResult.Cancel;
        //    Close();
        //}

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtUserId.Text = txtPassword.Text = string.Empty;
            lblMessage.Text = string.Empty;
            btnOK.Enabled = false;

            LoadCredentials();
        }

        private void LoadCredentials()
        {
            if (Properties.Settings.Default.LastUserId != string.Empty)
                txtUserId.Text = Properties.Settings.Default.LastUserId;
            if (Properties.Settings.Default.LastPassword != string.Empty)
                txtPassword.Text = Properties.Settings.Default.LastPassword;
        }

        string UserId { get { return txtUserId.Text.Trim();  } }
        string Password { get { return txtPassword.Text; } }


        public Guid Authenticate()
        {
            Guid Ans = Guid.Empty;
            AuthenticationType type = AuthenticationType.Unknown;

            type = AuthenticationType.Windows;
            Ans = DoWindowsAuthentication();

            if (Ans != Guid.Empty)
            {
                if (!SCService.Instance.SetUser(Ans))
                    Ans = Guid.Empty;  // user culd notbe identified, clear it
            }

            if (Ans == Guid.Empty)
            {
                type = AuthenticationType.Forms;
                Ans = DoFormsAuthentication();
            }

            // SZ: Remeber the login settings
            SaveLoginSettings(Ans, type);

            return Ans;
        }

        private void SaveLoginSettings(Guid Ans, AuthenticationType type)
        {
            Memory.LastLoginType = (int)type;
            Memory.LastUserId = (type == AuthenticationType.Forms) ? txtUserId.Text : GetLoggedInUserName();
            Memory.LastUserKey = Ans;
            Memory.LastLoginAt = DateTime.Now;
            Memory.Save();
        }

        private Guid DoWindowsAuthentication()
        {
            Guid Ans = Guid.Empty;
            // Get the user name
            // identify if the user name is in the DB
            // yes? get the id
            //      use the id as user credentials
            // no?  continue as before
            string user = GetLoggedInUserName();
            var ret = DAAB.ExecuteScalar(AppConfiguration.ConnectionString, 
                CommandType.Text, 
                "Select [usr_key] from [users] where [usr_net_login] like @value", 
                new SqlParameter[]{ new SqlParameter("value", user)}
                );
            if (ret != null && ret != DBNull.Value)
                Guid.TryParse(ret.ToString(), out Ans);
            return Ans;
        }

        Guid DoFormsAuthentication()
        {
            Guid Ans = Guid.Empty;

        LABEL_AGAIN:

            DialogResult res = ShowDialog();
            switch (res)
            {
                case System.Windows.Forms.DialogResult.OK:
                    try
                    {
                        SaveCredentials();

                        Ans = SCService.Instance.AuthenticateUser(UserId, Password);
                        if (Ans == Guid.Empty)
                            throw new Exception(Messages.INVALID_CREDENTIALS);
                        

                    }
                    catch (Exception ex)
                    {
                        string text = string.Format("Issue while verifying the credentials at {0}. \r\nDetails: {1} ", AppConfiguration.ServicePath, ex.Message);
                        _log.Error(text);
                        MessageBox.Show(string.Format(text, AppConfiguration.ServicePath), Resources.Title, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        goto LABEL_AGAIN;
                    }
                    break;
                case System.Windows.Forms.DialogResult.Cancel:
                    break;
            }
            Close();
            return Ans;
        }

        private void SaveCredentials()
        {
            Properties.Settings.Default.LastUserId = txtUserId.Text;
            Properties.Settings.Default.LastPassword = txtPassword.Text;
        }

        string GetLoggedInUserName()
        {
            string Ans = WindowsIdentity.GetCurrent().Name;
            return Ans;
        }

        Properties.Settings Memory { get { return Properties.Settings.Default; } }
    }
}
