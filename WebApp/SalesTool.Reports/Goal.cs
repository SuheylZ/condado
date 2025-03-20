namespace SalesTool.Reports
{
    using System;


    /// <summary>
    /// Summary description for Sample.
    /// </summary>
    public partial class Goal : Telerik.Reporting.Report
    {
        public Goal()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();

            //
            // TODO: Add any constructor code after InitializeComponent call
            //

            //DateTime nTodayDateTime = new DateTime(DateTime.Now.Ticks);
            //int paramYear = Convert.ToInt32(ReportParameters["year"].Value);
            //int paramMonth = Convert.ToInt32(ReportParameters["month"].Value);

            //int daysInMonthMinusWeekends = 0, daysInMonthWorked = 0;

            //for (int i = 1; i <= DateTime.DaysInMonth(paramYear, paramMonth); i++)
            //{
            //    DateTime thisDay = new DateTime(paramYear, paramMonth, i);
            //    if (thisDay.DayOfWeek != DayOfWeek.Saturday && thisDay.DayOfWeek != DayOfWeek.Sunday)
            //    {
            //        daysInMonthMinusWeekends += 1;
            //    }
            //}
            //txtTotalDaysWorked.Value = daysInMonthMinusWeekends.ToString();

            //if (nTodayDateTime.Month == paramMonth && nTodayDateTime.Year == paramYear)
            //{
            //    DateTime nValue = new DateTime(paramYear, paramMonth, nTodayDateTime.Day);

            //    for (int i = 1; i <= DateTime.DaysInMonth(paramYear, paramMonth); i++)
            //    {
            //        DateTime thisDay = new DateTime(paramYear, paramMonth, i);
            //        if (thisDay.DayOfWeek != DayOfWeek.Saturday && thisDay.DayOfWeek != DayOfWeek.Sunday)
            //        {
            //            daysInMonthWorked += 1;
            //        }
            //    }
            //    txtDaysWorked.Value = daysInMonthWorked.ToString();
            //}
            //else
            //{
            //    txtDaysWorked.Value = daysInMonthMinusWeekends.ToString();
            //}
            //txtDaysWorked.Value =new DateTime(Convert.ToInt32(ReportParameters["year"].Value),Convert.ToInt32(ReportParameters["month"].Value),
            
        }
        public static int CalculateEndDateofMonth()
        {
            DateTime today = DateTime.Today;
            DateTime endOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(1).AddDays(-1);
            return endOfMonth.Day;
        }

        
    }
}