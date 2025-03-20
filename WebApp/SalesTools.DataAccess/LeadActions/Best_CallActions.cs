using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
   public class Best_CallActions
    {


        private DBEngine _engine = null;

        internal Best_CallActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(Models.BestCall nbest_call)
        {
            nbest_call.IsActive = true;
            nbest_call.IsDeleted = false;
            nbest_call.AddedOn = DateTime.Now;
            _engine.Lead.BestCalls.AddObject(nbest_call);
            _engine.Save();
        }


        public void Update(Models.BestCall nbest_call)
        {
            nbest_call.ChangedOn = DateTime.Now;
            _engine.Save();
        }

        public void Delete(long inputKey)
        {
            var U = (from T in _engine.Lead.BestCalls.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            _engine.Save();
        }
       
        public void InActivate(long? inputKey)
        {
            var U = (from T in _engine.Lead.BestCalls.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.IsDeleted = false;
            _engine.Save();
        }

        public void Activate(long? inputKey)
        {
            var U = (from T in _engine.Lead.BestCalls.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }




        public IQueryable<Models.BestCall> GetAll()
        {
            return _engine.Lead.BestCalls;
        }


        public IQueryable<Models.BestCall> GetAll(long? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return _engine.Lead.BestCalls.Where(x => x.Key == inputIndvID && x.IsActive == true && x.IsDeleted == true);
            }
            else
            {
                return _engine.Lead.BestCalls.Where(x => x.IsActive == true && x.IsDeleted != false);
            }

        }


    }
}
