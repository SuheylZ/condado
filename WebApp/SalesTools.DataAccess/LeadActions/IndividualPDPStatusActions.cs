
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class IndividualPDPStatusActions
    {
        private DBEngine engine = null;

        internal IndividualPDPStatusActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(Models.IndividualPDPStatuses entity)
        {
            entity.AddedOn = DateTime.Now;

            engine.Lead.IndividualPDPStatuses.AddObject(entity);
            engine.Save();
        }

        public void Change(Models.IndividualPDPStatuses entity)
        {
            entity.ChangedOn = DateTime.Now;

            engine.Save();
        }

        public void Delete(int id)
        {
            engine.Lead.IndividualPDPStatuses.DeleteObject(this.Get(id));
            engine.Save();
        }

        public IQueryable<Models.IndividualPDPStatuses> All
        {
            get
            {
                return engine.Lead.IndividualPDPStatuses;
            }
        }

        public Models.IndividualPDPStatuses Get(int id)
        {
            return All.Where(x => x.ID == id).FirstOrDefault();
        }
    }
}
