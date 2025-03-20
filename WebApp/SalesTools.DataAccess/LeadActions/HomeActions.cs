using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class HomeActions
    {
        private DBEngine _engine = null;

        internal HomeActions(DBEngine engine)
        {
            _engine = engine;
        }

        #region "Add and Update Functions"

        public long Add(Models.Home entity)
        {
            entity.IsActive= true;
            entity.IsDeleted = false;
            entity.AddedOn = DateTime.Now;

            _engine.Lead.Homes.AddObject(entity);
            _engine.Save();
            return entity.Id;
        }

        public void Change(Models.Home entity)
        {
            entity.ChangedOn = DateTime.Now;

            _engine.Save();
        }

        #endregion "Add and Update Functions"

        #region "Active/inactivate Homes and soft delete Function"

        //public void InActivate (Int64 inputKey)
        //{
        //    var U = (from T in _engine.Lead.Homes.Where(x =>
        //        x.Id==inputKey)
        //             select T).FirstOrDefault();
        //    U.IsActive = false;
        //    _engine.Save();
        //}

        //public void Activate(Int64 inputKey)
        //{
        //    var U = (from T in _engine.Lead.Homes.Where(x =>
        //        x.Id==inputKey)
        //             select T).FirstOrDefault();
        //    U.IsActive = true;
        //    _engine.Save();
        //}

        public void Delete(long key, bool bPermanent=false)
        {
            if (!bPermanent)
                this.Get(key).IsDeleted = true;
            else
                _engine.leadEntities.Homes.DeleteObject(Get(key));

            _engine.Save();
        }

        public void Activate(long key, bool bActivate = true)
        {
            this.Get(key).IsActive = bActivate;

            _engine.Save();
        }

        #endregion "Active/inactivate Homes and soft delete Function"

        #region "Retrieve Functions"

        public IQueryable<Models.Home> GetAllByAccountID(long id)
        {
            return this.GetAll().Where(x => x.AccountId == id);
        }

        public IQueryable<Models.Home> GetAllByIndividualID(long id)
        {
            return this.GetAll().Where(x => x.Individualkey == id);
        }

        public IQueryable<Models.Home> GetAll()
        {
            return _engine.Lead.Homes.Where(x => x.IsActive == true && x.IsDeleted == false);
        }

        //public IEnumerable<Models.Home> All(Int64? inputIndvID=0)
        //{
        //    if (inputIndvID > 0)
        //    {
        //        return _engine.Lead.Homes.Where(x => x.IsActive == true && x.IsDeleted == false);

        //        return _engine.Lead.Homes.Where(x =>
        //            x.Id==inputIndvID
        //            && x.IsDeleted != false
        //            && x.IsActive == true);
        //    }
        //    else
        //    {
        //        return _engine.Lead.Homes.Where(x =>
        //            (x.IsDeleted != false
        //            && x.IsActive == true));
        //    }
        //}

        public Models.Home Get(Int64 id)
        {
            return this.GetAll().Where(x => x.Id == id).FirstOrDefault();

            //return _engine.Lead.Homes.Where(x => 
            //    x.Id==inputHomeId
            //    && x.IsDeleted != true).FirstOrDefault();
        }

        //public IEnumerable<Models.Home> getSelectedHomes(int skipRecords = 0, int takeRecords = 0)
        //{
        //    return _engine.Lead.Homes.Where(x => 
        //        x.IsDeleted != true).OrderBy(x =>
        //            x.IndvId).Skip(skipRecords).Take(takeRecords);
        //}

        #endregion "Retrieve Functions"

    }
}
