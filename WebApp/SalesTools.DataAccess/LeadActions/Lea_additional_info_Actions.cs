using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
   public class Lea_additional_info_Actions
    {
        private DBEngine _engine = null;

        internal Lea_additional_info_Actions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(Models.LeadAdditionalInfo nLea_additional_info)
        {
            nLea_additional_info.IsActive = true;
            nLea_additional_info.IsDeleted = false;
            nLea_additional_info.AddedOn  = DateTime.Now;
            _engine.Lead.LeaAdditionalInfos.AddObject(nLea_additional_info);
            _engine.Save();
        }


        public void Update(Models.LeadAdditionalInfo nLea_additional_info)
        {
            nLea_additional_info.ChangedOn = DateTime.Now;
            _engine.Save();
        }


        public void Delete(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.LeaAdditionalInfos.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            _engine.Save();
        }

        public void InActivate(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.LeaAdditionalInfos.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.IsDeleted = false;
            _engine.Save();
        }

        public void Activate(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.LeaAdditionalInfos.Where(x => x.Key ==inputKey) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }

        public IQueryable<Models.LeadAdditionalInfo> GetAll()
        {
            return _engine.Lead.LeaAdditionalInfos;
        }


        public IQueryable<Models.LeadAdditionalInfo> GetAll(Int64? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return _engine.Lead.LeaAdditionalInfos.Where(x =>
                   x.Key ==inputIndvID 
                    && x.IsActive == true && (x.IsDeleted??false) );
            }
            else
            {
                return _engine.Lead.LeaAdditionalInfos.Where(x => x.IsActive??false && !(x.IsDeleted??false));
            }

        }
    }
}
