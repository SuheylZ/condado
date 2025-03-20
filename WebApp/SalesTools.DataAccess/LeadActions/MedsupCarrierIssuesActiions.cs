using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
   public class MedsupCarrierIssuesActiions
    {
       private DBEngine _engine = null;

       internal MedsupCarrierIssuesActiions(DBEngine engine)
        {
            _engine = engine;
        }


       public void Add(Models.CarrierIssue crIssue)
       {
           //crIssue.car_iss_active_flag = true;
           //crIssue.car_iss_delete_flag = false;

           _engine.leadEntities.CarrierIssues.AddObject(crIssue);
           _engine.Save();
       }


       public void Update(Models.CarrierIssue crIssue)
       {
           crIssue.ChangedOn = DateTime.Now;
           _engine.Save();
       }


       public void Delete(Int64 inputKey)
       {
           var U = (from T in _engine.leadEntities.CarrierIssues.Where(x => x.Key.Equals(inputKey)) select T).FirstOrDefault();
           U.IsDeleted = true;
           _engine.Save();
       }

        // public void InActivate(Int64 inputKey)
        // {
        //     var U = (from T in _engine.Entities.carrier_issues.Where(x => x.key.Equals(inputKey)) select T).FirstOrDefault();
        //     // U.car_iss_delete_flag = false;
        //     _engine.Save();
        // }

        // public void Activate(Int64 inputKey)
        // {
        //     var U = (from T in _engine.Entities.carrier_issues.Where(x => x.key.Equals(inputKey)) select T).FirstOrDefault();
        //     U.car_iss_active_flag = true;
        //     _engine.Save();
        // }

       public IQueryable<Models.CarrierIssue> GetAll()
       {
           return _engine.leadEntities.CarrierIssues;
       }


        // public carrier_issues GetIndividual(Int64 inputIndvID)
        // {

        //     if (inputIndvID != null && inputIndvID > 0)
        //     {
        //         var U = (from T in _engine.Entities.carrier_issues.Where(x => x.Equals(inputIndvID))
        //                  select T).FirstOrDefault();
        //         return U;
        //         //return _engine.Entities.carrier_issues.Where(x =>
        //         //   (x.key.Equals(inputIndvID)
        //         //    && x.car_iss_active_flag == true
        //         //    && x.car_iss_delete_flag == true
        //         //    ))  ;
        //     }
        //     else
        //     {
        //         return null;
        //     }

        // }

    }
}
