using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SalesTool.Web.UserControls
{
    public partial class IndividualBox : System.Web.UI.UserControl, IDialogBox
    {
        public event EventHandler<LongArgs> OnClose = null;
        protected void Page_Load(object sender, EventArgs args)
        {
            ctlIndividual.OnClose += (o, a) => Close(a.Id);

        }
        public void Show(SalesTool.DataAccess.Models.Individual I)
        {
            if (I != null)
                ctlIndividual.LoadPerson(I);
            else
                ctlIndividual.Initialize();
            
            dlgIndividual.Visible = true;
            dlgIndividual.VisibleOnPageLoad = true;
            dlgIndividual.Enabled = true;
        }
        void Close(long id)
        {
            dlgIndividual.Visible = false;
            dlgIndividual.VisibleOnPageLoad = false;
            if (OnClose != null)
                OnClose(this, new LongArgs(id));
        }

        public Telerik.Web.UI.RadWindow GetWindow()
        {
            return dlgIndividual;
        }

        public void BindClientEvent(string clientID)
        {
            ctlIndividual.BindCloseEvent(clientID);
        }
    };
}