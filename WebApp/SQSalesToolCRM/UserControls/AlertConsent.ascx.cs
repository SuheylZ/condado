using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalesTool.Web.UserControls
{
    public partial class AlertConsentControl : System.Web.UI.UserControl, IDialogBox
    {
        const string
                      K_CONSENT_KEY = "CONSENT_ALERT_MESSAGE",
                      K_CONSENT_BOX_WIDTH = "CONSENT_ALERT_BOX_WIDTH",
                      K_CONSENT_BOX_HEIGHT = "CONSENT_ALERT_BOX_HEIGHT";
        
        public event EventHandler<TCPAConsentArgs> AlertClosed;


        protected void Page_Load(object sender, EventArgs e)
        {
            btnAlertClose.Click += (o, a) => Close(); 
        }

        void Show(string message, int width=0, int height=0)
        {
            ddlChoice.SelectedValue = "-1";
            lblAlert.Text = message + "<br/>";
            tlWindow.Visible = true;
            tlWindow.VisibleOnPageLoad = true;
            if(width>100)
                tlWindow.Width = Unit.Pixel(width);
            if(height>100)
                tlWindow.Height = new Unit(height, UnitType.Pixel);
        }
        public void Show()
        {
            string Message = SalesTool.Common.CGlobalStorage.Instance.Get<string>(K_CONSENT_KEY);
            int width = SalesTool.Common.CGlobalStorage.Instance.Get<int>(K_CONSENT_BOX_WIDTH), height = SalesTool.Common.CGlobalStorage.Instance.Get<int>(K_CONSENT_BOX_HEIGHT);
            Show(Message, width, height);
        }

        void Close()
        {
            int i = Helper.SafeConvert<int>(ddlChoice.SelectedValue);
            TCPAConsentType t = i == -1 ? TCPAConsentType.Blank : i == 0 ? TCPAConsentType.No : i == 1 ? TCPAConsentType.Yes : TCPAConsentType.Undefined;
            if (t!=TCPAConsentType.Blank)
            {
                tlWindow.Visible = false;
                tlWindow.VisibleOnPageLoad = false;
                if (AlertClosed != null)
                    AlertClosed(this, new TCPAConsentArgs(t));
            }
        }


        public Telerik.Web.UI.RadWindow GetWindow()
        {
            return tlWindow;
        }
    };
}