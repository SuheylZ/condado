using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
   public class LeadStatusActions
    {
        private DBEngine _engine = null;

        internal LeadStatusActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(Models.LeadStatus nlead_status)
        {
            nlead_status.IsActive = true;
            nlead_status.IsDeleted = false;
            nlead_status.AddedOn = DateTime.Now;
            _engine.Lead.LeadStatuses.AddObject(nlead_status);
            _engine.Save();
        }


        public void Update(Models.LeadStatus nlead_status)
        {
            nlead_status.ChangedOn = DateTime.Now;
            _engine.Save();
        }


        public void Delete(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadStatuses.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            _engine.Save();
        }

        public void InActivate(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadStatuses.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsDeleted = false;
            _engine.Save();
        }

        public void Activate(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadStatuses.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }

        public IQueryable<Models.LeadStatus> GetAll()
        {
            return _engine.Lead.LeadStatuses;
        }


        public IQueryable<Models.LeadStatus> GetAll(long? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return _engine.Lead.LeadStatuses.Where(x =>
                   x.Key ==inputIndvID
                    && x.IsActive == true && x.IsDeleted == true);
            }
            else
            {
                return _engine.Lead.LeadStatuses.Where(x =>
                     (x.IsActive == true) && x.IsDeleted != false);
            }

        }

    }
}
