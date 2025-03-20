using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class ListRetentionAccount : BaseActions
    {
        internal ListRetentionAccount(DBEngine engine) : base(engine) { }

        public IQueryable<ListRetention> All
        {
            get
            {
                return E.adminEntities.ListRetentions.OrderBy(x => x.Priority).AsQueryable();
            }
        }
        public ListRetention Get(int id)
        {
            return E.adminEntities.ListRetentions.Where(x => x.Key == id).FirstOrDefault();
        }

        public ListRetention Add(long accountKey)
        {
            ListRetention lpr = new ListRetention
            {                
                AccountKey = accountKey,
                Priority = GetPriority()
            };
            E.adminEntities.ListRetentions.AddObject(lpr);
            E.Save();
            return Get(lpr.Key);
        }
        public void Change(ListRetention rule)
        {
            E.Save();            
        }
        public void Delete(int id)
        {
            E.adminEntities.ListRetentions.DeleteObject(Get(id));
            E.Save();
        }                
        private int GetPriority()
        {
            return All.Count() > 0 ? All.Max(x => x.Priority) + 1 : 1;
        }

        //SZ [Mar 19, 2013] added the functionality so that the record may also be removed using the accountid
        public void DeleteByAccountID(long accountID)
        {
            var T = E.Admin.ListRetentions.Where(x => x.AccountKey == accountID).FirstOrDefault();
            if (T != null)
            {
                E.Admin.ListRetentions.DeleteObject(T);
                E.Save();
            }
        }
    }
}
