using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
  public  class IndividualDetailActions
    {

     private DBEngine _engine = null;

       internal IndividualDetailActions(DBEngine engine)
        {
            _engine = engine;
        }

      public void Add(Models.IndividualDetail nIndividual_Details)
        {
           // nIndividual_Details.dtl_active_flag = true;
           // nIndividual_Details.dtl_delete_flag = false;
           // nIndividual_Details.dtl_add_date = DateTime.Now;
            _engine.Lead.IndividualDetails.AddObject(nIndividual_Details);
            _engine.Save();
        }


      public void Update(Models.IndividualDetail nIndividual_Details)
        {
           // nIndividual_Details.dtl_modified_date = DateTime.Now;
            _engine.Save();
        }


        public void Delete(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.IndividualDetails.Where(x => x.Key==inputKey) select T).FirstOrDefault();
           // U.dtl_delete_flag = true;
            _engine.Save();
        }

        public void InActivate(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.IndividualDetails.Where(x => x.Key.Equals(inputKey)) select T).FirstOrDefault();
           // U.dtl_delete_flag = false;
            _engine.Save();
        }

        public void Activate(Int64 inputKey)
        {
            var U = (from T in _engine.Lead.IndividualDetails.Where(x => x.Key==inputKey) select T).FirstOrDefault();
           // U.dtl_active_flag = true;
            _engine.Save();
        }

        public IQueryable<Models.IndividualDetail> GetAll()
        {
            return _engine.Lead.IndividualDetails;
        }


        //public IEnumerable<Individual_Details> All(Int64 inputIndvID)
        //{

        //    if (inputIndvID != null && inputIndvID > 0)
        //    {
        //        return _engine.Entities.Individual_Details.Where(x =>
        //           (x.dtl_key.Equals(inputIndvID)
        //            && x.dtl_active_flag == true && x.dtl_delete_flag == true));
        //    }
        //    else
        //    {
        //        return _engine.Entities.Individual_Details.Where(x =>
        //             (x.dtl_active_flag == true) && x.dtl_delete_flag != false);
        //    }

        //}

    }
}
