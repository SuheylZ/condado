using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class IssueStatusActions
    {
        private DBEngine engine = null;

        internal IssueStatusActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(Models.IssueStatuses entity)
        {
            entity.AddedOn = DateTime.Now;

            engine.Lead.IssueStatuses.AddObject(entity);
            engine.Save();
        }

        public void Change(Models.IssueStatuses entity)
        {
            entity.ChangedOn = DateTime.Now;

            engine.Save();
        }

        public void Delete(int id)
        {
            engine.Lead.IssueStatuses.DeleteObject(this.Get(id));
            engine.Save();
        }

        public IQueryable<Models.IssueStatuses> All
        {
            get
            {
                return engine.Lead.IssueStatuses;
            }
        }

        public Models.IssueStatuses Get(int id)
        {
            return All.Where(x => x.ID == id).FirstOrDefault();
        }
    }
}
