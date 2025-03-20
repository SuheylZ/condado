using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class Driver_InfoActions
    {
        private DBEngine _engine = null;

        internal Driver_InfoActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(DriverInfo ndriver_info)
        {
            ndriver_info.IsActive = true;
            ndriver_info.IsDeleted = false;
            ndriver_info.AddedOn = DateTime.Now;
            _engine.leadEntities.DriverInfos.AddObject(ndriver_info);
            //_engine.Lead.DriverInfos.AddObject(ndriver_info);
            _engine.Save();
        }


        public void Update(DriverInfo ndriver_info)
        {
            // ndriver_info.dri_modified_date = DateTime.Now;
            _engine.Save();
        }


        public void Delete(long? inputKey)
        {
            var U = (from T in _engine.Lead.DriverInfos.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            _engine.Save();
        }

        public void InActivate(long? inputKey)
        {
            var U = (from T in _engine.Lead.DriverInfos.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            // U.dri_delete_flag = false;
            _engine.Save();
        }

        public void Activate(long? inputKey)
        {
            var U = (from T in _engine.Lead.DriverInfos.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }

        public IEnumerable<DriverInfo> GetAll()
        {
            return _engine.Lead.DriverInfos;
        }

        public IQueryable<DriverInfo> GetDriversByAccountID(long? inputAccountKey)
        {
            if (inputAccountKey > 0)
            {
                return _engine.leadEntities.DriverInfos.Where(x =>
                    x.IsDeleted != true && (x.AccountId == inputAccountKey));
            }
            else
                return null;
        }

        public DriverInfo GetInfoByKey(long? inputKey)
        {
            return _engine.leadEntities.DriverInfos.Where(x =>
                x.Key == inputKey).FirstOrDefault();
        }


        public IEnumerable<DriverInfo> GetAll(long? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return _engine.Lead.DriverInfos.Where(x =>
                   (x.Key == inputIndvID
                    && x.IsActive == true
                       //&& x.dri_delete_flag == true 
                    ));
            }
            else
            {
                return _engine.Lead.DriverInfos.Where(x =>
                     (x.IsActive == true)
                     );
            }

        }

    }
}
