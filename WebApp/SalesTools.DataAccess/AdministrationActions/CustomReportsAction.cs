using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class CustomReportsAction : BaseActions
    {
        internal CustomReportsAction(DBEngine engine) : base(engine) { }

        public int Add(Models.Report nReport, bool IsTemporary = false)
        {
            nReport.ReportID = GetMaxID() + 1;
            nReport.IsDeleted = IsTemporary;
            E.adminEntities.Reports1.AddObject(nReport);
            nReport.Added.On = DateTime.Now;
            E.Save();
            return nReport.ReportID;
        }

        public int GetMaxID()
        {
            int id = 0;
            if (E.adminEntities.Reports1.Count() > 0)
                id = E.adminEntities.Reports1.Max(x => x.ReportID);
            return id;
        }

        public void Change(Models.Report nReport)
        {
            nReport.Changed.On = DateTime.Now;
            E.Save();
        }

        public void Delete(int reportID)
        {
            var U = (from T in E.adminEntities.Reports1.Where(x => x.ReportID.Equals(reportID)) select T).FirstOrDefault();
            U.IsDeleted = true;
            E.Save();
        }

        public IQueryable<Models.Report> GetAll()
        {
            return E.adminEntities.Reports1.Where(x => x.IsDeleted != true);
        }

        public Models.Report Get(int reportID)
        {
            return E.adminEntities.Reports1.Where(x => x.ReportID == reportID).FirstOrDefault();
        }

        public bool IsDuplicateTitle(string reportTitle, int editID = 0)
        {
            Models.Report T = null;
            if (editID > 0)
            {
                T = E.adminEntities.Reports1.Where(x => x.ReportTitle == reportTitle && x.IsDeleted != true && x.ReportID != editID).FirstOrDefault();
            }
            else
                T = E.adminEntities.Reports1.Where(x => x.ReportTitle == reportTitle && x.IsDeleted != true).FirstOrDefault();
            return !(T == null);
        }
        public IQueryable<Object> GetReportsData(string dbquery)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            System.Data.Objects.ObjectResult<Object> nResult = E.adminEntities.ExecuteStoreQuery<Object>(dbquery);
            return nResult.AsQueryable();
            //using (var cmd = E.Admin.Connection.CreateCommand())
            //{
            //    cmd.CommandText = dbquery;
            //    using (var reader = cmd.ExecuteReader())
            //    {
            //        reader.AsQueryable();
            //        dt.Load(reader);
            //    }
            //}
            //return result;            
        }

        public void AddUserForReport(int reportID, List<Guid> usersKey)
        {
            DeleteUsersForReport(reportID);
            int reportUserID = GetMaxReportUserID() + 1;
            foreach (Guid userkey in usersKey)
            {
                Models.ReportUsers nReportUser = new Models.ReportUsers();
                nReportUser.Id = reportUserID;
                nReportUser.ReportID = reportID;
                nReportUser.UserKey = userkey;
                E.adminEntities.ReportUsers.AddObject(nReportUser);
                reportUserID++;
            }
            E.Save();
        }

        public Models.ReportUsers GetReportUser(int reportID, Guid userKey)
        {
            return E.adminEntities.ReportUsers.Where(x => x.UserKey == userKey && x.ReportID == reportID).FirstOrDefault();
        }

        public IQueryable<Models.ReportUsers> GetReportUsersByReportID(int reportID)
        {
            return E.adminEntities.ReportUsers.Where(x => x.ReportID == reportID);
        }

        public IQueryable<Models.ReportUsers> GetReportUsersByUserKey(Guid userKey)
        {
            return E.adminEntities.ReportUsers.Where(x => x.UserKey == userKey && x.Report.IsDeleted == false);
        }

        public int GetMaxReportUserID()
        {
            int id = 0;
            if (E.adminEntities.ReportUsers.Count() > 0)
                id = E.adminEntities.ReportUsers.Max(x => x.Id);
            return id;
        }

        public void DeleteUsersForReport(int reportID)
        {
            foreach (Models.ReportUsers nReportUser in GetReportUsersByReportID(reportID))
            {
                E.adminEntities.ReportUsers.DeleteObject(nReportUser);
            }
            E.Save();
        }
        public IQueryable<Models.User> UsersNotInReport(int reportId)
        {
            var users = E.adminEntities.Users.Where(x => !x.report_users.Any(y => y.ReportID == reportId) && !(x.IsDeleted ?? false)).OrderBy(x=>x.FirstName).ThenBy(y=>y.LastName);
            return users.AsQueryable();
        }
        public IQueryable<Models.User> UsersInReport(int reportId)
        {
            var users = E.adminEntities.Users.Where(x => x.report_users.Any(y => y.ReportID == reportId) && !(x.IsDeleted ?? false)).OrderBy(x => x.FirstName).ThenBy(y => y.LastName);
            return users.AsQueryable();
        }

    }
}
