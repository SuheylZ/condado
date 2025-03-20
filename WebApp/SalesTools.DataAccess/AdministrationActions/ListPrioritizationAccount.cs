using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class ListPrioritizationAccount : BaseActions
    {
        internal ListPrioritizationAccount(DBEngine engine) : base(engine) { }

        public IQueryable<ListPrioritization> All
        {
            get
            {
                return E.adminEntities.ListPrioritizations.OrderBy(x => x.Priority).AsQueryable();
            }
        }

        public IEnumerable<ListPrioritization> GetAll(bool bFresh = false)
        {
            IEnumerable<Models.ListPrioritization> R = null;
            if (!bFresh)
                R = this.All;
            else
            {
                E.leadEntities.EmailQueues.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = this.All;
                E.leadEntities.EmailQueues.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R.ToList<ListPrioritization>();
        }

        public ListPrioritization Get(int id)
        {
            return E.adminEntities.ListPrioritizations.Where(x => x.Key == id).FirstOrDefault();
        }

        public ListPrioritization GetByAccount(long id)
        {
            return E.adminEntities.ListPrioritizations.Where(x => x.AccountKey == id).FirstOrDefault();
        }

        public ListPrioritization Add(long accountKey)
        {
            ListPrioritization lpr = new ListPrioritization
            {                
                AccountKey = accountKey,
                Priority = GetPriority()
            };
            E.adminEntities.ListPrioritizations.AddObject(lpr);
            E.Save();
            return Get(lpr.Key);
        }
        public void Change(ListPrioritization rule)
        {
            E.Save();            
        }
        public void Delete(int id)
        {
            var T = Get(id);
            if (T != null)
            {
                E.adminEntities.ListPrioritizations.DeleteObject(T);
                E.Save();    
            }
            
        }
        public void DeleteByAccountID(long id)
        {
            
            try
            {
                var T = E.Admin.ListPrioritizations.Where(x => x.AccountKey == id).FirstOrDefault();
                if (T != null)
                {
                    E.Admin.ListPrioritizations.DeleteObject(T);
                    //MH:16 May some-time exception is raised: transaction operation cannot be performed because there are pending requests working on this transaction
                    E.Save();
                }
            }
            catch (System.Data.SqlClient.SqlException e)
            {
                //2nd try
                E.Save();
            }
        }        
        private int GetPriority()
        {
            return All.Count() > 0 ? All.Max(x => x.Priority) + 1 : 1;
        }
    }
}
