using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    /// <summary>
    /// 
    /// </summary>
   public  class MedsupActions: BaseActions
    {
        internal MedsupActions(DBEngine engine): base(engine)
        {}

       public Models.Medsup Add(Models.Medsup nMedsup)
        {
            nMedsup.IsActive = true;
            nMedsup.IsDeleted = false;
            nMedsup.AddedOn = DateTime.Now;
            E.Lead.Medsups.AddObject(nMedsup);
            E.Save();
            return nMedsup;
        }


        public void Update(Models.Medsup nMedsup)
        {
            nMedsup.ChangedOn = DateTime.Now;
            E.Save();
        }


        public void Delete(long? inputKey)
        {
            var U = (from T in E.Lead.Medsups.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            E.Save();
        }

        //public void InActivate(long? inputKey)
        //{
        //    var U = (from T in E.Lead.Medsups.Where(x => x.Key==inputKey) select T).FirstOrDefault();
        //    //U.ms_delete_flag = false;
        //    E.Save();
        //}

        //public void Activate(Int64 inputKey)
        //{
        //    var U = (from T in E.Lead.Medsups.Where(x => x.Key==inputKey) select T).FirstOrDefault();
        //    U.IsActive = true;
        //    E.Save();
        //}

        //public IEnumerable<Models.Medsup> GetAll()
        //{
        //    return E.Lead.Medsups.Where( x => x.IsDeleted != true);
        //}
        //public IEnumerable<Models.Medsup> GetAll(long? inputIndvID)
        //{

        //    if (inputIndvID != null && inputIndvID > 0)
        //    {
        //        return E.Lead.Medsups.Where(x => x.Key == inputIndvID && (x.IsActive ?? false));
        //    }
        //    else
        //    {
        //        return E.Lead.Medsups.Where(x => (x.IsActive?? false));
        //    }

        //}


       public Models.Medsup GetPolicyInfo(long? inputKey)
        {
            if (inputKey > 0)
            {
                return E.leadEntities.Medsups.Where(x =>
                    x.Key == inputKey).FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

       public IQueryable<Models.MedicalSupplement> GetByAccount(long accountID)
        {
            //if (inputID > 0)
            //{
            //    return E.leadEntities.Medsups.Where(x =>
            //        x.AccountId == inputID && (x.IsDeleted != true));
            //}
            //else
            //{
            //    return null;
            //}

            return E.leadEntities.MedicalSupplements.Where(x => x.AccountId == accountID).AsQueryable();
        }

       public IQueryable<Models.Medsup> GetAllByAccount(long accID)
       {
           return E.leadEntities.Medsups.Where(x => x.AccountId == accID && !x.IsDeleted);
       }

       public IQueryable<Models.MedsupAppType> MedicalSupplimentApplicationTypes
       {
           get
           {
               return E.Lead.medsup_app_type.AsQueryable();
           }
       }


    }
}
