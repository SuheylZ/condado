namespace SalesTool.Reports
{
    using System;


    /// <summary>
    /// Summary description for Sample.
    /// </summary>
    public partial class SubmitEnrollment : Telerik.Reporting.Report
    {
        public SubmitEnrollment()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
             //if(ReportParameters["type"] !=null )
            //
            // TODO: Add any constructor code after InitializeComponent call
            //
            
        }

        private void SubmitEnrollment_ItemDataBinding(object sender, EventArgs e)
        {
            if (ReportParameters["type"] != null)
            {

            }
        }

        
    }
}