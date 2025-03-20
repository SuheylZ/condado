using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Linq;
using System.Linq.Expressions;


namespace SalesTool.DataAccess.Tests
{
    [TestClass]
    public class EmailFunctionality: EngineBase
    {
        Guid[] K_users = new Guid[] {new Guid("32E95E39-EAB7-42A7-9DAE-0670BE37FFC4"), new Guid("A10D0922-1431-4583-9CE5-0A5D8F701720"), new Guid("DC55DBC6-3E68-4907-BF2A-0A816F76925D")};
        const int K_ReportId = 1;

        [TestMethod]
        public void AddnGet()
        {
            string subject = "My test email No:" + DateTime.Now.Ticks.ToString();
            string body = "This is an email, please check";

            int iret = E.EmailActions.Add(subject, body, false, 1, 1, K_ReportId, "Unit Test", K_users);

            Models.EmailData data = E.EmailActions.Get(iret, K_ReportId);
            Assert.AreEqual(data.Id, iret);
            Assert.AreEqual(data.Subject, subject);
            Assert.IsFalse(data.FilterByRole);
            Assert.AreEqual(data.Message, body);
            Assert.IsTrue(data.Recipients.Count == K_users.Length);

            E.EmailActions.Delete(iret, K_ReportId);
        }


        [TestMethod]
        public void EmailBulkRetrieval()
        {
            int[] ids = new int[6];

            //Prepare the DB
            for (int i = 0; i < 6; i++)
            {
                string subject = "My test email No:" + DateTime.Now.Ticks.ToString();
                string body = "This is an email, please check";
                ids[i] = E.EmailActions.Add(subject, body, false, 1, 1, K_ReportId, "Unit Test", K_users);
            }


            var emails = E.EmailActions.EmailsByReport(K_ReportId).ToList();

            Assert.IsTrue(emails.Count == ids.Count());
            

            

             //Clean up
            foreach(var i in ids)
                E.EmailActions.Delete(i, K_ReportId);
        }



    }
}
