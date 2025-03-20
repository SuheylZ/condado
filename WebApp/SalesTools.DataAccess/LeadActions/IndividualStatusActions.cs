using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class IndividualStatusActions
    {
        private DBEngine engine = null;

        internal IndividualStatusActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(Models.IndividualStatus entity)
        {
            entity.AddedOn = DateTime.Now;

            engine.Lead.IndividualStatus.AddObject(entity);
            engine.Save();
        }

        public void Change(Models.IndividualStatus entity)
        {
            entity.ChangedOn = DateTime.Now;

            engine.Save();
        }

        public void Delete(int id)
        {
            engine.Lead.IndividualStatus.DeleteObject(this.Get(id));
            engine.Save();
        }

        public IQueryable<Models.IndividualStatus> All
        {
            get
            {
                return engine.Lead.IndividualStatus;
            }
        }

        public Models.IndividualStatus Get(int id)
        {
            return All.Where(x => x.ID == id).FirstOrDefault();
        }
    }
}
