using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Windows.Forms.VisualStyles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess.Tests
{
    /// <summary>
    /// Created by: Imran H
    /// Dated: 03.10.13
    /// Description: Testing performed Item-157 Normal View All Time Filter i.e called on viewLeads.aspx .
    /// </summary>
    [TestClass]
    public class MyFirstUnitTest : EngineBase
    {


        [TestMethod()]
        public void TestFilterAllLeads()
        {
            LeadsSearchBy();
        }
        [TestMethod()]
        public void TestFilterLeadsOnLast7Days()
        {
            LeadsSearchBy("Last 7 Days");

        }
        [TestMethod()]
        public void TestFilterLeadsOnToday()
        {
            LeadsSearchBy("Today");

        }

        [TestMethod()]
        public void TestFilterLeadsOnYesterday()
        {
            LeadsSearchBy("Yesterday");

        }

        [TestMethod()]
        public void TestFilterLeadsOnLast30Days()
        {
            LeadsSearchBy("Last 30 Days");

        }
        [TestMethod()]
        public void TestFilterLeadsOnThisMonth()
        {
            LeadsSearchBy("This Month");

        }

        [TestMethod()]
        public void TestFilterLeadsOnLastMonth()
        {
            LeadsSearchBy("Last Month");
        }
        [TestMethod()]
        public void TestFilterLeadsOnLastWeekMonToFri()
        {
            LeadsSearchBy("Last Week (MON-FRI)");
        }
        [TestMethod()]
        public void TestFilterLeadsOnLastWeekMonToSun()
        {
            LeadsSearchBy("Last Week (MON-SUN)");
        }



        public void LeadsSearchBy(string filterBy = "All Time")
        {
            var totalCounts = 0;
            bool success = false;
            IQueryable<Models.ViewLead> query = E.AccountActions.AllViewLeads;
            if (query == null)
                throw new InternalTestFailureException();
            totalCounts = query.Count();


            if (filterBy == "All Time")
            {
                success = totalCounts > 0;
                if (success)
                    Assert.IsTrue(success, String.Format("{0} Record(s) available in All Leads", totalCounts));
                else
                    Assert.Fail(String.Format("{0} Record(s) available in All Leads", totalCounts));
            }
            else
            {
                query = FilterbyTime(query, filterBy);
                var afterfilterCounts = query.Count();
                success = totalCounts != afterfilterCounts;
                if (success)
                    Assert.IsTrue(success,
                                  String.Format(
                                      "Total Record Fetch : {0} After Filter Record Fetch : {1} Record(s) available in All Leads",
                                      totalCounts, afterfilterCounts));
                else
                    Assert.Fail("Test failed due to the equal counts after filter");
            }

        }

     
        private IQueryable<Models.ViewLead> FilterbyTime(IQueryable<Models.ViewLead> query, string filter = "All Time")
        {
            DateTime dtTarget = DateTime.Now, dtTarget2 = DateTime.Now;
            //string filter = lbtnTime.Text;

            switch (filter)
            {
                case "Today":
                    query = query.Where(x => SqlFunctions.DateDiff("day", x.CreatedOn, dtTarget) == 0);
                    break;

                case "Yesterday":
                    query = query.Where(x => SqlFunctions.DateDiff("day", x.CreatedOn, dtTarget) == 1);
                    break;

                case "Last 7 Days":
                    //IH 26.09.2013
                    var sevenDaysAgo = DateTime.Now.Date.AddDays(-7);
                    query = query.Where(x => x.CreatedOn > sevenDaysAgo);

                    //query = query.Where(x => SQL.DateDiff("day", x.CreatedOn, dtTarget) == 7);
                    break;

                case "Last Week (MON-SUN)":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(6);
                    query = query.Where(x =>
                        SqlFunctions.DateDiff("day", x.CreatedOn, dtTarget) <= 0 &&
                        SqlFunctions.DateDiff("day", x.CreatedOn, dtTarget2) >= 0);
                    break;


                case "Last Week (MON-FRI)":
                    dtTarget = dtTarget.LastDateOn(DayOfWeek.Monday);
                    dtTarget2 = dtTarget.AddDays(4);
                    query = query.Where(x =>
                        SqlFunctions.DateDiff("day", x.CreatedOn, dtTarget) <= 0 &&
                        SqlFunctions.DateDiff("day", x.CreatedOn, dtTarget2) >= 0);
                    break;

                case "Last 30 Days":
                    //IH 26.09.2013
                    var seven30DaysAgo = DateTime.Now.Date.AddDays(-30);
                    query = query.Where(x => x.CreatedOn > seven30DaysAgo);
                    // query = query.Where(x => SQL.DateDiff("day", x.CreatedOn, dtTarget) == 30);
                    break;

                case "Last Month":
                    query = query.Where(x => SqlFunctions.DateDiff("month", x.CreatedOn, dtTarget) == 1);
                    break;

                case "This Month":
                    query = query.Where(x => SqlFunctions.DateDiff("month", x.CreatedOn, dtTarget) == 0);
                    break;

                //default:
                //    query = query.Where(x =>
                //        SqlFunctions.DateDiff("day", x.CreatedOn, rdpAllTimeFrom.SelectedDate) <= 0 &&
                //        SqlFunctions.DateDiff("day", x.CreatedOn, rdpAllTimeTo.SelectedDate) >= 0
                //        );
                //    break;
            }
            return query;
        }

       
        //[TestMethod]
        //[UrlToTest("http://localhost:8080/Default.aspx")]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("$(SolutionDir)\\Project.Web")]
        //public void TestMethodUrl()
        //{
        //    string url = HttpContext.Current.Request.Url.ToString();
        //}

    }
    public static class DateTimeExtensions
    {
        [System.Diagnostics.DebuggerStepThrough()]
        public static DateTime LastDateOn(this DateTime dt, DayOfWeek target)
        {
            return DateTime.Today.AddDays(-1 * ((int)DateTime.Today.DayOfWeek - (int)target) - 7);
        }
    }
}
