using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Transactions;
using System.Data.SqlClient;


namespace SalesTool.DataAccess.Models
{

    public enum EReportFormat { Unknown = 0, Excel =1 , Text =2 ,CSV =3, PDF = 4, Doc =5 };
    public enum EmailFrequency { Unknown = 0, Now, Once, Daily, Weekly, Monthly };

    public class SalesToolEmailRecipient
    {
        public string Name { get; set; }
        public Guid Id { get; set; }

        public SalesToolEmailRecipient(Guid id, string name)
        {
            Name = name;
            Id = id;
        }

        public SalesToolEmailRecipient(SalesTool.DataAccess.Models.User user)
        {
            Name = user.FullName;
            Id = user.Key;
        }
    };


    public struct EmailData
    {
        public int Id;
        public EReportFormat Format;
        public List<SalesToolEmailRecipient> Recipients;
        public string Subject;
        public string Message;
        public bool FilterByRole;
        public EmailFrequency SendingFrequency;

        public EmailData Init()
        {
            Id = 0;
            Format = EReportFormat.Unknown;
            Recipients = new List<SalesToolEmailRecipient>();
            Subject = string.Empty;
            Message = string.Empty;
            FilterByRole = false;
            SendingFrequency = EmailFrequency.Now;
            return this;
        }
        public Guid[] UserKeys
        {
            get
            {
                List<Guid> lst = new List<Guid>();
                foreach (var item in Recipients)
                    lst.Add(item.Id);
                return lst.ToArray();
            }

        }
    };
}

namespace SalesTool.DataAccess
{
    /// <summary>
    /// This class implements the Email functionality. It can add, edit, remove email and can list them by 
    /// report or list reports by email.
    /// </summary>
    public class EmailActions : BaseActions
    {
        internal EmailActions(DBEngine engine) : base(engine) { }

        public int Add(string subject, string body, bool isFiltered, byte format, byte frequency, int report, string by, Guid[] recipients)
        {
            Models.Emails email = new Models.Emails { Subject = subject, Body = body, CreatedBy = by, CreatedOn = DateTime.Now, IsSent = false };

            email.Id = E.Lead.Emails.Count() > 0 ? E.Lead.Emails.Max(x => x.Id) + 1 : 1;

            // Step 1:add email.
            using (TransactionScope scope = new TransactionScope())
            {
                E.Lead.Emails.AddObject(email);
                E.Save();

                //Step 2: add report 
                //INSERT INTO [dbo].[email_report]([emr_eml_id], [emr_rpt_id], [emr_format], [emr_frequency], [emr_filter])VALUES(@p1,@p2,@p3,@p4,@p5)
                Models.ReportInEmail RE = new Models.ReportInEmail { EmailId = email.Id, ReportId = report, FormatId = format, Frequency = frequency, IsFiltered = isFiltered };
                E.Lead.ReportInEmail.AddObject(RE);
                E.Save();

                //Step 3:  add recipients
                AddRecipients(RE.Id, recipients);
                scope.Complete();
            }
            return email.Id;
        }
        public int Add(Models.EmailData data, int report, string by)
        {
            return Add(data.Subject, data.Message, data.FilterByRole, (byte)data.Format, (byte)data.SendingFrequency, report, by, data.UserKeys);
        }

        public IQueryable<Models.Emails> EmailsByReport(int reportId)
        {
            return E.Lead.Emails.GroupJoin(E.Lead.ReportInEmail.Where(x => x.ReportId == reportId),
                a => a.Id, b => b.EmailId,
                (a, b) => new { Email = a, Report = b })
                .SelectMany(x => x.Report.DefaultIfEmpty(), (x, y) => new { Email = x.Email, Report = y })
                .Select(x => x.Email)
                .AsQueryable();
        }
        public IQueryable<Models.Report> ReportsByEmail(int emailId)
        {
            return E.Admin.Reports1.GroupJoin(E.Lead.ReportInEmail.Where(x => x.EmailId == emailId),
                a => a.ReportID, b => b.ReportId,
                (a, b) => new { Report = a, Email = b }).SelectMany(x => x.Email.DefaultIfEmpty(), (x, y) => new { Report = x.Report, Email = y })
                .Select(x => x.Report)
                .AsQueryable();

        }

        public void SetEmailStatus(int emailid, bool flag = true)
        {
            System.Data.SqlClient.SqlParameter[] arr = new System.Data.SqlClient.SqlParameter[2];
            arr[0] = new System.Data.SqlClient.SqlParameter("@p1", emailid);
            arr[1] = new System.Data.SqlClient.SqlParameter("@p2", flag);

            E.Lead.ExecuteStoreCommand("update [emails] set [eml_sent_flag] = @p2 where eml_id = @p1", arr);
        }

        public Models.EmailData Get(int emailid, int reportid)
        {
            Models.EmailData Ans = new Models.EmailData().Init();
            var X = E.Lead.Emails.Where(x => x.Id == emailid).FirstOrDefault();
            if (X != null)
            {
                Ans.Id = X.Id;
                Ans.Subject = X.Subject;
                Ans.Message = X.Body;
            }

            Models.ReportInEmail re = E.Lead.ReportInEmail.Where(x => x.EmailId == emailid && x.ReportId == reportid).FirstOrDefault();
            if (re != null)
            {
                Ans.Format = (Models.EReportFormat)re.FormatId;
                Ans.SendingFrequency = (Models.EmailFrequency)re.Frequency;
                Ans.FilterByRole = re.IsFiltered;
            }

            foreach (var r in E.Lead.ViewEmailRecipients.Where(x => x.EmailId == emailid && x.ReportId == reportid))
                Ans.Recipients.Add(new Models.SalesToolEmailRecipient(r.UserId, r.FullName));

            return Ans;
        }
        public Models.EmailData GetByReport(int reportid, int order = 1)
        {
            order = order > 0 ? order - 1 : 0;
            Models.EmailData Ans = new Models.EmailData().Init();

            var Emails = EmailsByReport(reportid);
            if (Emails != null)
            {
                int id = 0;
                if (Emails.Count() > 0 && order < Emails.Count())
                    id = Emails.ToList()[order].Id;
                else
                {
                    var T = Emails.FirstOrDefault();
                    if (T != null) id = T.Id;
                }

                Ans = Get(id, reportid);
            }
            return Ans;
        }

        public void Change(Models.EmailData data, int reportId, string by)
        {
            Models.Emails email = E.Lead.Emails.Where(x => x.Id == data.Id).FirstOrDefault();
            email.Subject = data.Subject;
            email.Body = data.Message;
            email.ModifiedBy = by;
            email.ModifiedOn = DateTime.Now;

            Models.ReportInEmail RE = E.Lead.ReportInEmail.Where(x => x.EmailId == data.Id && x.ReportId == reportId).FirstOrDefault();

            RE.IsFiltered = data.FilterByRole;
            RE.FormatId = (byte)data.Format;
            RE.Frequency = (byte)data.SendingFrequency;


            ClearRecipients(RE.Id);
            AddRecipients(RE.Id, data.UserKeys);

            E.Save();
        }
        public bool Delete(int emailid, int reportid)
        {
            bool Ans = false;
            List<int> ids = new List<int>();
            int iTotalRecord = E.Lead.ReportInEmail.Count(x => x.EmailId == emailid);

            IEnumerable<Models.ReportInEmail> REs = E.leadEntities.ReportInEmail.Where(x => x.EmailId == emailid && x.ReportId == reportid);
            foreach (var re in REs)
            {
                ids.Add(re.Id);
                ClearRecipients(re.Id);
                E.Lead.ReportInEmail.DeleteObject(re);
            }

            if (iTotalRecord == ids.Count)
            {
                Ans = true;
                Models.Emails eml = E.Lead.Emails.Where(x => x.Id == emailid).FirstOrDefault();
                E.Lead.Emails.DeleteObject(eml);
            }

            E.Save();
            return Ans;
        }

        void ClearRecipients(int EmailReportId)
        {
            System.Data.SqlClient.SqlParameter[] arr = new System.Data.SqlClient.SqlParameter[1];
            arr[0] = new System.Data.SqlClient.SqlParameter("@Id", EmailReportId);
            E.Lead.ExecuteStoreCommand("delete from email_recipients where rcp_eml_id=@id", arr);
        }
        void AddRecipients(int EmailReportId, Guid[] users)
        {
            if (users != null)
                foreach (var id in users)
                {
                    System.Data.SqlClient.SqlParameter[] arr = new System.Data.SqlClient.SqlParameter[2];
                    arr[0] = new System.Data.SqlClient.SqlParameter("@p1", EmailReportId);
                    arr[1] = new System.Data.SqlClient.SqlParameter("@p2", id);

                    E.Lead.ExecuteStoreCommand(
                        "INSERT INTO [email_recipients] ([rcp_eml_id],[rcp_usr_id]) VALUES (@p1, @p2)",
                        arr);
                }
        }

        public IQueryable<Models.EmailsForReport> GetEmails(Models.EmailFrequency freq)
        {
            IQueryable<Models.EmailsForReport> Ans = null;

            switch (freq)
            {
                case Models.EmailFrequency.Once:
                    Ans = E.leadEntities.EmailsForReport.Where(x => (x.LastSent == null) && (x.Frequency == (byte)freq));
                    break;

                case Models.EmailFrequency.Now:
                    Ans = E.leadEntities.EmailsForReport.Where(x => (x.LastSent == null) && (x.Frequency == (byte)freq));
                    break;

                case Models.EmailFrequency.Daily:
                    Ans = E.leadEntities.EmailsForReport.Where(x => (x.DaysLastSent > 0) && (x.Frequency == (byte)freq));
                    break;

                case Models.EmailFrequency.Weekly:
                    Ans = E.leadEntities.EmailsForReport.Where(x => (x.DaysLastSent >= 7) && (x.Frequency == (byte)freq));
                    break;

                case Models.EmailFrequency.Monthly:
                    Ans = E.leadEntities.EmailsForReport.Where(x => (x.DaysLastSent >= 30) && (x.Frequency == (byte)freq));
                    break;
            }
            return Ans;
        }

        public void UpdateLastSentStatus(int emailId)
        {
            const string K_SQL = "update emails set eml_last_sent = @sent where (eml_ID = @id)";

            E.leadEntities.ExecuteStoreCommand(K_SQL, new SqlParameter[] {
                new SqlParameter("@sent", DateTime.Now), 
                new SqlParameter("@id", emailId)
            });
        }
    }
}
