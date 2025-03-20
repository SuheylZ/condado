using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class PolicyActions
    {
        private DBEngine _engine = null;

        internal PolicyActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(PolicyInformation entity)
        {
            //entity.IsActive = true;
            //entity.IsDeleted = false;
            entity.AddedOn = DateTime.Now;

            _engine.Lead.PolicyInformations.AddObject(entity);

            _engine.Save();
        }

        public void Change(PolicyInformation entity)
        {
            entity.ChangedOn = DateTime.Now;

            _engine.Save();
        }

        public void Delete(long key)
        {
            //this.Get(key).IsDeleted = true;

            _engine.Save();
        }

        public void Activate(long key, bool bActivate = true)
        {
            //this.Get(key).IsActive = bActivate;

            _engine.Save();
        }

        //public IQueryable<PolicyInformation> GetAllByAccountID(long id)
        //{
        //    return this.GetAll().Where(x => x.AccountId == id);
        //}

        public IQueryable<PolicyInformation> GetAllByIndividualID(long id)
        {
            return this.GetAll().Where(x => x.IndividualId == id);
        }

        public IQueryable<PolicyInformation> GetAll()
        {
            return _engine.Lead.PolicyInformations;//.Where(x => x.IsActive == true && x.IsDeleted == false);
        }

        public PolicyInformation Get(long id)
        {
            return this.GetAll().Where(x => x.Key == id).FirstOrDefault();
        }
    }
}
