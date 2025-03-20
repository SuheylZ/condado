
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class AutoHomePolicyActions: BaseActions
    {
        

        internal AutoHomePolicyActions(DBEngine rengine): base(rengine)
        {
        }

        public void Add(Models.AutoHomePolicy entity)
        {
            entity.IsActive= true;
            entity.IsDeleted = false;
            entity.Added.On1 = DateTime.Now;
            entity.BoundOn = DateTime.Now;

            E.Lead.AutoHomePolicies1.AddObject(entity);

            E.Save();
        }

        public void AddWithoutSave(Models.AutoHomePolicy entity)
        {
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.Added.On1 = DateTime.Now;
            entity.BoundOn = DateTime.Now;

            E.Lead.AutoHomePolicies1.AddObject(entity);

            //E.Save();
        }

        public void Change(Models.AutoHomePolicy entity)
        {
            entity.Changed.On1 = DateTime.Now;

            E.Save();
        }

        public void Delete(long key)
        {
            this.Get(key).IsDeleted = true;

            E.Save();
        }

        public void Activate(long key, bool bActivate = true)
        {
            this.Get(key).IsActive = bActivate;

            E.Save();
        }

        public IQueryable<Models.AutoHomePolicy> GetAllByAccountID(long id)
        {
            return this.GetAll().Where(x => x.AccountId == id);
        }

        //public IQueryable<Models.AutoHomePolicy> GetAllByIndividualID(long id)
        //{
        //    return this.GetAll().Where(x => x.IndividualId == id);
        //}

        public IQueryable<Models.AutoHomePolicy> GetAll()
        {
            return E.Lead.AutoHomePolicies1.Where(x => x.IsActive == true && x.IsDeleted == false);
        }

        public Models.AutoHomePolicy Get(Int64 id)
        {
            return this.GetAll().Where(x => x.ID == id).FirstOrDefault();
        }

        public IQueryable<Models.ViewAutoHomePolicy> GetPolicies(long accId)
        {
            return E.Lead.ViewAutoHomePolicy.Where(x => x.AccountId == accId);
        }
    }
}
