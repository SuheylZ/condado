using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class MedsupApplicationActions
    {
        private DBEngine _engine = null;

        internal MedsupApplicationActions(DBEngine engine)
        {
            _engine = engine;
        }



        public medsupApplication Add(medsupApplication medsupApp)
        {
            //medsupApp.ms_application_flag = true;
            //medsupApp.ms_application_delete_flag = false;

            _engine.leadEntities.medsupApplications.AddObject(medsupApp);
            _engine.Save();
            return medsupApp;
        }


        public void Update(medsupApplication medsupApp)
        {

            _engine.Save();
        }


        public void Delete(Int64 inputKey)
        {
            var U = (from T in _engine.leadEntities.medsupApplications.Where(x => x.Key.Equals(inputKey)) select T).FirstOrDefault();
            U.IsDeleted = true;
            _engine.Save();
        }

        public void InActivate(Int64 inputKey)
        {
            var U = (from T in _engine.leadEntities.medsupApplications.Where(x => x.Key.Equals(inputKey)) select T).FirstOrDefault();
            U.IsActive = false;
            _engine.Save();
        }

        public void Activate(Int64 inputKey)
        {
            var U = (from T in _engine.leadEntities.medsupApplications.Where(x => x.Key.Equals(inputKey)) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }

        public IQueryable<medsupApplication> GetAll()
        {
            return _engine.leadEntities.medsupApplications.Where(x => x.IsDeleted != true);
        }

        public medsupApplication GetAppTrackByKey(Int64? inputKey)
        {
            return _engine.leadEntities.medsupApplications.Where(x => x.Key == inputKey).FirstOrDefault();
        }

        public IQueryable<medsupApplication> GetAllTrackingPerAccount(long? AccountID)
        {
            return _engine.leadEntities.medsupApplications.Where(x =>
                x.AccountId == AccountID && (x.IsDeleted != true));
        }

        public IQueryable<medsupApplication> GetByIndividual(Int64 inputIndvID)
        {
            var result = (from T in _engine.leadEntities.Medsups.Where(x => x.IndividualId.Equals(inputIndvID)) select T).FirstOrDefault();

            var U = (from T in _engine.leadEntities.medsupApplications.Where(x => x.MedsupId.Equals(result.Key) && x.IsDeleted != true) select T);
            return U;
          

        }
    }
}
