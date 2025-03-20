using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
  public  class DriverIncidenceActions
    {

       private DBEngine _engine = null;

       internal DriverIncidenceActions(DBEngine engine)
        {
            _engine = engine;
        }

       public void Add(DriverIncidence ndriver_incidence)
        {
            ndriver_incidence.IsActive = true;
            ndriver_incidence.IsDeleted= false;
            ndriver_incidence.AddedOn = DateTime.Now;
            _engine.Lead.DriverIncidences.AddObject(ndriver_incidence);
            _engine.Save();
        }


       public void Update(DriverIncidence ndriver_incidence)
        {
            ndriver_incidence.ChangedOn = DateTime.Now;
            _engine.Save();
        }


        public void Delete(long? inputKey)
        {
            var U = (from T in _engine.Lead.DriverIncidences.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            _engine.Save();
        }

        public void InActivate(long? inputKey)
        {
            var U = (from T in _engine.Lead.DriverIncidences.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsDeleted = false;
            _engine.Save();
        }

        public void Activate(long? inputKey)
        {
            var U = (from T in _engine.Lead.DriverIncidences.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }


        public IEnumerable<DriverIncidence> GetAll()
        {
            return _engine.Lead.DriverIncidences;
        }


        public IEnumerable<DriverIncidence> GetAll(long? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return _engine.Lead.DriverIncidences.Where(x =>      x.Key==inputIndvID
                                            && (x.IsActive??false) && 
                                                (x.IsDeleted??false) 
                );
            }
            else
            {
                return _engine.Lead.DriverIncidences.Where(x =>
                     (x.IsActive??false) && (x.IsDeleted?? false));
            }

        }

    }
}
