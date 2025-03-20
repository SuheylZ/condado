using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
   public class Account_DetailActions
    {

        private DBEngine _engine = null;

        internal Account_DetailActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(Models.AccountDetail nAccount_Detail)
        {
            // nAccount_Detail.dtl_active_flag = true;
            // nAccount_Detail.dtl_delete_flag = false;
            nAccount_Detail.AddedOn = DateTime.Now;
            _engine.Lead.AccountDetails.AddObject(nAccount_Detail);
            _engine.Save();
        }


        public void Update(Models.AccountDetail nAccount_Detail)
        {
            //nAccount_Detail.dtl_modified_date = DateTime.Now;
            _engine.Save();
        }


        public void Delete(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.AccountDetails.Where(x => x.Key == inputKey) select T).FirstOrDefault();
           // U.dtl_delete_flag = true;
            _engine.Save();
        }

        public void InActivate(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.AccountDetails.Where(x => x.Key==inputKey) select T).FirstOrDefault();
           // U.dtl_delete_flag = false;
            _engine.Save();
        }

        public void Activate(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.AccountDetails.Where(x => x.Key==inputKey) select T).FirstOrDefault();
           // U.dtl_active_flag = true;
            _engine.Save();
        }

        public IQueryable<Models.AccountDetail> GetAll()
        {
            return _engine.Lead.AccountDetails;
        }


    }
}
