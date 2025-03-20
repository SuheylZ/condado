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
using System.Configuration;



namespace OLEBridge
{
    public partial class LocationForm : Form
    {
        public LocationForm()
        {
            InitializeComponent();
            txtLocation.TextChanged += (o, arg) => { btnOK.Enabled = !string.IsNullOrEmpty(this.Text); };
            btnOK.Click += (o, arg) =>
            {
                //Store in the database

                

                

            };
        }

        public LocationForm(string caption, string defaultvalue)
        {
            InitializeComponent();
            label1.Text = caption;
            Text = string.Format(Resources.Title + " ({0}) ", caption);
            txtLocation.Text = defaultvalue;
            txtLocation.TextChanged += (o, arg) => { btnOK.Enabled = !string.IsNullOrEmpty(this.Text); };
            btnOK.Click += (o, arg) =>
            {
                //Store in the database

            };
        }


        public string Value { get { return txtLocation.Text.Trim(); } }
    }
}
