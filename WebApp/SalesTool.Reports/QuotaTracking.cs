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
    public partial class QuotaTracking : Telerik.Reporting.Report
    {
        public QuotaTracking()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            
        }

        public QuotaTracking(string username): this()
        {
            
          
        }
        public static int CalculateEndDateofMonth()
        {
            DateTime today = DateTime.Today;
            DateTime endOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
            return endOfMonth.Day;
        }
        
    }
}