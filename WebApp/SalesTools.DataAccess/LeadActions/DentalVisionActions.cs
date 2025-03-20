using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class DentalVisionActions
    {
        private DBEngine _engine = null;

        internal DentalVisionActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(Models.DentalVision entity)
        {
            entity.IsActive= true;
            entity.IsDeleted = false;
            entity.AddedOn = DateTime.Now;

            _engine.Lead.DentalVisions.AddObject(entity);

            _engine.Save();
        }

        public void Change(Models.DentalVision entity)
        {
            entity.ChangedOn = DateTime.Now;

            _engine.Save();
        }

        public void Delete(long key)
        {
            this.Get(key).IsDeleted = true;

            _engine.Save();
        }

        public void Activate(long key, bool bActivate = true)
        {
            this.Get(key).IsActive = bActivate;

            _engine.Save();
        }

        public IQueryable<Models.DentalVision> GetAllByAccountID(long id)
        {
            return this.GetAll().Where(x => x.AccountId == id && !(x.IsDeleted??false));
        }

        public IQueryable<Models.DentalVision> GetAllByIndividualID(long id)
        {
            return this.GetAll().Where(x => x.IndividualId == id);
        }

        public IQueryable<Models.DentalVision> GetAll()
        {
            return _engine.Lead.DentalVisions.Where(x => x.IsActive == true && x.IsDeleted == false);
        }

        public Models.DentalVision Get(Int64 id)
        {
            return this.GetAll().Where(x => x.Key == id).FirstOrDefault();
        }
    }
}
