namespace SalesTool.Reports
{
    using System;


    /// <summary>
    /// Summary description for Sample.
    /// </summary>
    public partial class FillFormSpeed : Telerik.Reporting.Report
    {
        public FillFormSpeed()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            
        }

        public FillFormSpeed(int month, int year): this()
        {
            
            this.ReportParameters["month"].Value = month;
            this.ReportParameters["year"].Value = year;
        }
    }
}