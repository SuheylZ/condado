using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class CarrierActions
    {
        private DBEngine _engine = null;

        internal CarrierActions(DBEngine engine)
        {
            _engine = engine;
        }

        public Models.Carrier Get(long carrierID)
        {
            return _engine.leadEntities.Carriers.Where(x => x.Key == carrierID).FirstOrDefault();
        }

        public IQueryable<Models.Carrier> GetAll()
        {
            return _engine.leadEntities.Carriers.Where(x => x.IsDeleted != true);
        }

        public IQueryable<Models.Carrier> GetMedSup()
        {
            var returnValue = _engine.leadEntities.Carriers.Where(x => x.MsFlag == true);
            return returnValue;
        }

        public IQueryable<Models.Carrier> GetDentalVision()
        {
            var returnValue = _engine.leadEntities.Carriers.Where(x => x.IsDental == true);
            return returnValue;
        } 

        public IQueryable<Models.Carrier> GetMaPDP()
        {
            var returnValue = _engine.leadEntities.Carriers.Where(x => x.IsMapdp == true);
            return returnValue;
        }

        public IQueryable<Models.Carrier> GetHomeCarriers()
        {
            var returnValue = _engine.leadEntities.Carriers.Where(x => x.IsHome == true);
            return returnValue;
        }

        public IQueryable<Models.Carrier> GetAutoCarriers()
        {
            var returnValue = _engine.leadEntities.Carriers.Where(x => x.IsAuto == true);
            return returnValue;
        }   
    }
}
