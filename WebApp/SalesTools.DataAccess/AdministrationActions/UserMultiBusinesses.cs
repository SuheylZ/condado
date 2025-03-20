using System;
using System.Collections.Generic;
using System.Linq;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class UserMultiBusinesses
    {
        private DBEngine _engine = null;

        internal UserMultiBusinesses(DBEngine reng)
        {
            _engine = reng;
        }

        public void Add(UserMultiBusiness entity)
        {
            _engine.adminEntities.UserMultiBusinesses.AddObject(entity);
            _engine.Save();
        }

        public void Change(UserMultiBusiness entity)
        {
            _engine.Save();
        }

        public void Delete(int id)
        {
            var U = (from T in _engine.adminEntities.UserMultiBusinesses.Where(x => x.Id==id) select T).FirstOrDefault();
            _engine.adminEntities.UserMultiBusinesses.DeleteObject(U);
            _engine.Save();
        }

        public IQueryable<UserMultiBusiness> GetAll()
        {
            return _engine.adminEntities.UserMultiBusinesses;
        }

        public UserMultiBusiness Get(int id)
        {
            return _engine.adminEntities.UserMultiBusinesses.Where(x => x.Id == id).FirstOrDefault();
        }
        public IQueryable<UserMultiBusiness> GetSelectedRecords(int skipRecords = 0, int takeRecords = 0)
        {
            return _engine.adminEntities.UserMultiBusinesses.OrderBy(x => x.OutpulseId).Skip(skipRecords).Take(takeRecords);
        }
    }
}
