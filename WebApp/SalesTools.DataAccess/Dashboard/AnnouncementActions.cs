using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Data.Objects;

namespace SalesTool.DataAccess.Dashboard
{
    public class AnnouncementActions: BaseActions
    {
        internal AnnouncementActions(DBEngine rengine) : base(rengine) { }

        public IQueryable<Models.GetAnnouncementBySectionRS> GetAnnouncementsBySection(int sectionId)
        {
            return E.Dashboard.GetAnnouncementBySection(sectionId).AsQueryable();
        }

        public void Add(int sectionId, string title, int order, string body)
        {
            E.Dashboard.AddAnnouncementBySection(sectionId, title, order, body);
        }
        public void Add(Models.Announcement rec)
        {
            rec.Id = E.Dashboard.Announcements.Count() > 0 ? E.Dashboard.Announcements.Max(x => x.Id) + 1 : 1;
            rec.DateAdded = DateTime.Now;
            E.Dashboard.AddToAnnouncements(rec);
            E.Save();
        }

        public void Delete(int Id)
        {
            E.Dashboard.DeleteAnnouncement(Id);
        }

        public void Change(int id, int sectionId, bool isActive, string title, int order, string body)
        {
            E.Dashboard.UpdateAnnouncement(id, sectionId, isActive, title, order, body);
        }
        public void Change(Models.Announcement rec)
        {
            E.Save();
        }
        public Models.Announcement Get(int id)
        {
            return E.Dashboard.Announcements.Where(x => x.Id == id).FirstOrDefault();
        }
    }

    public class ReportTypeActions:BaseActions
    {
        internal ReportTypeActions(DBEngine rengine) : base(rengine) { }

        public IEnumerable<Models.ReportType> All
        {
            get
            {
                return E.Dashboard.ReportTypes.OrderBy(x => x.IsDefault).AsEnumerable();
            }
        }

    }

    public class DashboardActions : BaseActions
    {
        internal DashboardActions(DBEngine rengine) : base(rengine) { }

        public Models.Section Get(int id)
        {
            return E.Dashboard.Sections.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Models.Section> All{
            get{
                return E.Dashboard.Sections;
            }
        }

        public int GetNewLeads(Guid user)
        {
            ObjectParameter[] arr =new ObjectParameter[2];

            arr[0] = new ObjectParameter("@agent", user);
            arr[1] = new ObjectParameter("@fragment", "new");

            var Ans = E._dashboard.ExecuteFunction<int>("dbo.GetLeadByStatusForAgent", arr);
            return Ans.FirstOrDefault();
        }

    };


}
    

