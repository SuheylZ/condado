using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class DriverActions
    {
        private DBEngine _engine = null;

        internal DriverActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(DriverInfo entity)
        {
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.AddedOn = DateTime.Now;

            _engine.Lead.DriverInfos.AddObject(entity);

            _engine.Save();
        }

        public void Change(DriverInfo entity)
        {
            entity.ChangedOn = DateTime.Now;

            _engine.Save();
        }

        public void Delete(long key, bool bPremanent=false)
        {
            if (!bPremanent)
                this.Get(key).IsDeleted = true;
            else
                _engine.Lead.DriverInfos.DeleteObject(Get(key));

            _engine.Save();
        }

        public void Activate(long key, bool bActivate = true)
        {
            this.Get(key).IsActive = bActivate;

            _engine.Save();
        }

        public IQueryable<DriverInfo> GetAllByAccountID(long id)
        {
            return this.GetAll().Where(x => x.AccountId == id);
        }

        //public IQueryable<DriverInfo> GetAllByIndividualID(long id)
        //{
        //    return this.All().Where(x => x.IndividualId == id);
        //}

        public IQueryable<DriverInfo> GetAll()
        {
            return _engine.Lead.DriverInfos.Where(x => x.IsActive == true && x.IsDeleted == false);
        }

        public DriverInfo Get(long id)
        {
            return this.GetAll().Where(x => x.Key == id).FirstOrDefault();
        }
    }
}
