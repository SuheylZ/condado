using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{

    public class RetentionRuleActions : BaseActions
    {
        internal RetentionRuleActions(DBEngine engine) : base(engine){}
        internal const short K_PARENT_TYPE = 3;

        public void RemoveAll(Guid key)
        {
            Stack<int> stIds = new Stack<int>();
            foreach (var item in E.adminEntities.LeadRetentionRules.Where(x => x.UserKey == key))
                stIds.Push(item.Id);

            while(stIds.Count>0)
                Delete(stIds.Pop());
            
        }

        public IQueryable<Models.LeadRetentionRules> All
        {
            get{
                return E.adminEntities.LeadRetentionRules.OrderBy(x=>x.Id).AsQueryable();
            }
        }

        public IQueryable<Models.LeadRetentionRules> AllActive
        {
            get{ 
                return E.adminEntities.LeadRetentionRules
                        .Where(x=>x.IsActive)
                        .OrderBy(x=>x.Id)
                        .AsQueryable();
            }
        }
        public IQueryable<Models.LeadRetentionRules> AllInactive
        {
            get
            {
                return E.adminEntities.LeadRetentionRules
                        .Where(x => !x.IsActive)
                        .OrderBy(x => x.Id)
                        .AsQueryable();
            }
        }

        public LeadRuleDetailActions GetDetails(int id){
            return new LeadRuleDetailActions(this.E, id);
        }
        public Models.LeadRetentionRules Get(int id)
        {
            return E.adminEntities.LeadRetentionRules.Where(x=>x.Id==id).FirstOrDefault();
        }

        public Models.LeadRetentionRules Add(string title, string description, bool IsEnabled, Guid userkey, string by, short selectedFilter, string customFilterValue)
        {
            Models.LeadRetentionRules lrr = new Models.LeadRetentionRules
            {
                 Id = GetId(),
                 Title = title,
                 IsActive =IsEnabled,
                 UserKey = userkey,
                 Description = description, 
                 Priority =GetPriority(),
                 Added = new Models.History
                 { 
                     By = by, 
                     On = DateTime.Now
                 },
                 FilterSelection = selectedFilter,
                 FilterCustomValue = customFilterValue
            };
            E.adminEntities.AddToLeadRetentionRules(lrr);
            E.Save();
            return Get(lrr.Id);
        }
        public void Change(Models.LeadRetentionRules rule, string by)
        {
            rule.Changed.By = by;
            rule.Changed.On = DateTime.Now;
            E.Save();
            Move(rule.Id, rule.Priority);
        }
        public void Delete(int id){
            var D = Get(id);
            E.FilterAreaActions.DeleteAll(id, K_PARENT_TYPE);
            GetDetails(id).DeleteAll();
            E.adminEntities.LeadRetentionRules.DeleteObject(D);
            E.Save();
        }
        public bool TitleExists(string title)
        {
            return All.Where(x => string.Compare(x.Title, title, true) == 0).Count() > 0;
        }
        
        private int GetId()
        {
            return E.adminEntities.LeadRetentionRules.Count()>0?
                E.adminEntities.LeadRetentionRules.Max(x=>x.Id) +1:
                1;
        }
        private int GetPriority()
        {
            return All.Count() > 0 ? All.Max(x => x.Priority) + 1 : 1;
        }


        //SZ [Jan 1, 2012] Change of priority order
        public void MoveUp(int recordId)
        {
            Move(recordId, Get(recordId).Priority + 1);
        }
        public void MoveDown(int recordId)
        {
            //SZ [Jan 3, 2013] Since the indexing starts at 0, moving down requires 2 units down
            Move(recordId, Get(recordId).Priority - 2);
        }
        public void Move(int recordId, int priority)
        {
            E.adminEntities.ChangeRetentionPriority(recordId, priority);
        }

        public void MakeEnabled(int recordId)
        {
            var U = (from T in E.adminEntities.LeadRetentionRules.Where(x => x.Id.Equals(recordId)) select T).FirstOrDefault();
            U.IsActive = !U.IsActive;
            E.Save();
        }

        public Models.LeadRetentionRules Copy(int recordId, string by)
        {
            var x = Get(recordId);
            var y = Add("Copy of " + x.Title, x.Description, x.IsActive, x.UserKey,  by, x.FilterSelection ?? 0, x.FilterCustomValue);

            var xdetails = GetDetails(recordId);
            var ydetails = GetDetails(y.Id);

            IList<LeadRuleDetails> details = xdetails.All.ToList();
            foreach (var t in details)
                ydetails.Add(t.Shift, t.WeekDay, t.Working.Starts, t.Working.Ends);

            //SZ [Jan 3, 2013] add filters
            IList<FilterArea> areas = E.FilterAreaActions.GetAll().Where(z => z.ParentKey == recordId && z.ParentType == 3).ToList();
            foreach (var t in areas)
                E.FilterAreaActions.Copy(t, y.Id, by);

            return y;
        }
    }

    

}
