using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
   public class Lead_SourceActions
    {

        private DBEngine _engine = null;

        internal Lead_SourceActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(Models.LeadSource nlead_source)
        {
            nlead_source.IsActive = true;
            //nlead_source.source_delete_flag = false;
            nlead_source.AddedOn = DateTime.Now;
            _engine.Lead.LeadSources.AddObject(nlead_source);
            _engine.Save();
        }


        public void Update(Models.LeadSource nlead_source)
        {
            nlead_source.ChangedOn = DateTime.Now;
            _engine.Save();
        }


        public void Delete(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadSources.Where(x => x.Key== inputKey) select T).FirstOrDefault();
          //  U.source_delete_flag = true;
            _engine.Save();
        }

        public void InActivate(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadSources.Where(x => x.Key== inputKey) select T).FirstOrDefault();
           // U.source_delete_flag = false;
            _engine.Save();
        }

        public void Activate(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadSources.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }

        public IQueryable<Models.LeadSource> GetAll()
        {
            return _engine.Lead.LeadSources;
        }


        public IQueryable<Models.LeadSource> GetAll(long? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return _engine.Lead.LeadSources.Where(x =>
                   (x.Key==inputIndvID
                    && x.IsActive == true
                       //&&x.source_delete_flag == true
                    ));
            }
            else
            {
                return _engine.Lead.LeadSources.Where(x =>
                     (x.IsActive == true)
                    //&& x.source_delete_flag != false
                     );
            }

        }


    }
}
