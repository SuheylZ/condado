using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class PolicyStatusesActions : BaseActions
    {
        internal PolicyStatusesActions(DBEngine rengine): base(rengine)
        {
        }

        public long Add(Models.PolicyStatuses entity)
        {
            entity.AddedDate = DateTime.Now;
            entity.ChangedDate = DateTime.Now;

            E.Lead.policy_statuses.AddObject(entity);
            E.Save();
            
            return entity.Key;
        }

        public Models.PolicyStatuses Get(Int64 id)
        {
            return this.GetAll().Where(x => x.Key == id).FirstOrDefault();
        }

        public IQueryable<Models.PolicyStatuses> GetAll(int type)
        {
            return E.Lead.policy_statuses.Where(x=> x.Type == type);
        }

        public IQueryable<Models.PolicyStatuses> GetAll()
        {
            return E.Lead.policy_statuses;
        }

        public IQueryable<Models.PolicyStatusType> GetAllPolicyTypes()
        {
            return E.Lead.policy_status_type;
        }
    }
}
