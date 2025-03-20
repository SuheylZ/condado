using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess.AdministrationActions
{
    
    public class ListReassignmentAccount : BaseActions
    {
        internal ListReassignmentAccount(DBEngine engine) : base(engine) { }

        public IQueryable<ListReassignments> All
        {
            get
            {
                return E.adminEntities.ListReassignments.OrderBy(x => x.Priority).AsQueryable();
            }
        }

        public IQueryable<ListReassignments> GetAll(bool bFresh = false)
        {
            IQueryable<ListReassignments> r = null;
            if (!bFresh)
                r = this.All;
            else
            {
                E.leadEntities.EmailQueues.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                r = this.All;
                E.leadEntities.EmailQueues.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return r;
        }

        public ListReassignments Get(int id)
        {
            return E.adminEntities.ListReassignments.FirstOrDefault(x => x.Key == id);
        }

        public ListReassignments GetByAccount(long id)
        {
            return E.adminEntities.ListReassignments.FirstOrDefault(x => x.AccountKey == id);
        }

        public ListReassignments Add(long accountKey)
        {
            var lpr = new ListReassignments
            {
                AccountKey = accountKey,
                Priority = GetPriority()
            };
            E.adminEntities.ListReassignments.AddObject(lpr);
            E.Save();
            return Get(lpr.Key);
        }
        public void Change(ListReassignments rule)
        {
            E.Save();
        }
        public void Delete(int id)
        {
            var T = Get(id);
            if (T == null) return;
            E.adminEntities.ListReassignments.DeleteObject(T);
            E.Save();
        }
        public void DeleteByAccountId(long id)
        {
            var T = E.Admin.ListReassignments.FirstOrDefault(x => x.AccountKey == id);
            if (T == null) return;
            E.Admin.ListReassignments.DeleteObject(T);
            E.Save();
        }
        private int GetPriority()
        {
            return All.Any() ? All.Max(x => x.Priority) + 1 : 1;
           // return All.Count() > 0 ? All.Max(x => x.Priority) + 1 : 1;
        }
    }
}
