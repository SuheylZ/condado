using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class PrioritizationRuleActions : BaseActions
    {
        internal PrioritizationRuleActions(DBEngine engine) : base(engine) { }

        public IQueryable<LeadPrioritizationRules> All
        {
            get
            {
                return E.adminEntities.LeadPrioritizationRules.OrderBy(x => x.Id).AsQueryable();
            }
        }

        public IQueryable<LeadPrioritizationRules> GetAll(bool bFresh = false)
        {
            IQueryable<Models.LeadPrioritizationRules> R = null;
            if (!bFresh)
                R = this.All;
            else
            {
                E.adminEntities.LeadPrioritizationRules.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = this.All;
                E.adminEntities.LeadPrioritizationRules.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R;
        }
        public IQueryable<LeadPrioritizationRules> AllActive
        {
            get
            {
                return E.adminEntities.LeadPrioritizationRules
                        .Where(x => x.IsActive)
                        .OrderBy(x => x.Id)
                        .AsQueryable();
            }
        }
        //YA[April 18, 2013] This will be called from window service.
        public IQueryable<LeadPrioritizationRules> AllActiveFresh
        {
            get
            {
                E.adminEntities.LeadPrioritizationRules.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                var result = E.adminEntities.LeadPrioritizationRules
                        .Where(x => x.IsActive)
                        .OrderByDescending(t => t.Priority)
                        .AsQueryable();
                E.adminEntities.LeadPrioritizationRules.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
                return result;
            }
        }

        public LeadRuleDetailActions GetDetails(int id)
        {
            return new LeadRuleDetailActions(this.E, id, RecordType.Prioritization);
        }
        public LeadPrioritizationRules Get(int id)
        {
            return E.adminEntities.LeadPrioritizationRules.Where(x => x.Id == id).FirstOrDefault();
        }

        public LeadPrioritizationRules Add(string title, string description,bool IsEnabled, string by, short selectedFilter, string customFilterValue)
        {
            LeadPrioritizationRules lpr = new LeadPrioritizationRules
            {
                Id = GetId(),
                Title = title,
                IsActive = IsEnabled,
                Description = description,
                Priority = GetPriority(),
                Added = new History
                {
                    By = by,
                    On = DateTime.Now
                },
                FilterSelection = selectedFilter,
                FilterCustomValue = customFilterValue
            };
            E.adminEntities.AddToLeadPrioritizationRules(lpr);
            E.Save();
            return Get(lpr.Id);
        }
        public void Change(LeadPrioritizationRules rule, string by)
        {
            rule.Changed.By = by;
            rule.Changed.On = DateTime.Now;
            E.Save();
            Move(rule.Id, rule.Priority);
        }
        public void Delete(int id,short parentType)
        {
            E.FilterAreaActions.DeleteAll(id, parentType);
            GetDetails(id).DeleteAll();
            E.adminEntities.LeadPrioritizationRules.DeleteObject(Get(id));
            E.Save();
        }
        public bool TitleExists(string title)
        {
            return All.Where(x => string.Compare(x.Title, title, true) == 0).Count() > 0;
        }

        private int GetId()
        {
            return E.adminEntities.LeadPrioritizationRules.Count() > 0 ?
                E.adminEntities.LeadPrioritizationRules.Max(x => x.Id) + 1 :
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
            //SZ [Jan 3, 2013] done as indexing starts from 0.
            Move(recordId, Get(recordId).Priority - 2);
        }
        public void Move(int recordId, int priority)
        { 
            E.adminEntities.ChangePrioritizationPriority(recordId, priority); 
        }
    
        public void MakeEnabled(int recordId)
        {
            var U = (from T in E.adminEntities.LeadPrioritizationRules.Where(x => x.Id.Equals(recordId)) select T).FirstOrDefault();
            U.IsActive = !U.IsActive;
            E.Save();
        }



        public LeadPrioritizationRules Copy(int recordId, string by)
        {
            var x = Get(recordId);
            var y = Add("Copy of " + x.Title, x.Description, x.IsActive, by, x.FilterSelection ?? 0, x.FilterCustomValue);

            var xdetails = GetDetails(recordId);
            var ydetails = GetDetails(y.Id);

            List<LeadRuleDetails> details = xdetails.All.ToList();
            foreach (var t in details)
                ydetails.Add(t.Shift, t.WeekDay, t.Working.Starts, t.Working.Ends); 

            //SZ [Jan 3, 2013] add filters?
            IList<FilterArea> areas = E.FilterAreaActions.GetAll().Where(z => z.ParentKey == recordId && z.ParentType == 2).ToList();
            foreach (var t in areas)
                E.FilterAreaActions.Copy(t, y.Id, by);

            return y;
        }

    }
}
