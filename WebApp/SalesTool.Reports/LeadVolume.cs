namespace SalesTool.Reports
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for Sample.
    /// </summary>
    public partial class LeadVolume : Telerik.Reporting.Report
    {
        public LeadVolume()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            //txtSubTitle.Value = "Lead volume (primary only) for unknown";
        }


        
    }
}