using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class ManageStates
    {
        private DBEngine _engine = null;

        internal ManageStates(DBEngine reng)
        {
            _engine = reng;
        }

        public void Add(State entity)
        {
            _engine.adminEntities.States.AddObject(entity);
            _engine.Save();
        }

        public void Change(State entity)
        {
            _engine.Save();
        }

        public void Delete(int id)
        {
            var U = (from T in _engine.adminEntities.States.Where(x => x.Id == id) select T).FirstOrDefault();
            _engine.Save();
        }

        public IQueryable<State> GetAll()
        {
            return _engine.adminEntities.States;
        }

        public State Get(int id)
        {
            return _engine.adminEntities.States.Where(x => x.Id == id).FirstOrDefault();
        }

        public State Get(string abbreviation)
        {
            return _engine.adminEntities.States.FirstOrDefault(p=>p.Abbreviation==abbreviation);
        }

        public IQueryable<State> GetSelectedRecords(int skipRecords = 0, int takeRecords = 0)
        {
            return _engine.adminEntities.States.OrderBy(x => x.FullName).Skip(skipRecords).Take(takeRecords);
        }
    }
}
