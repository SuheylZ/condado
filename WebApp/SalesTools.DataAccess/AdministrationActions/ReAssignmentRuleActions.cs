using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess.AdministrationActions
{

    public class ReAssignmentRuleActions : BaseActions
    {
        internal ReAssignmentRuleActions(DBEngine engine) : base(engine) { }

        public IQueryable<LeadReassignmentRules> All
        {
            get
            {

                return E.adminEntities.LeadReassignmentRules.OrderBy(x => x.Id).AsQueryable();
            }
        }

        public IQueryable<LeadReassignmentRules> GetAll(bool bFresh = false)
        {
            IQueryable<LeadReassignmentRules> r = null;
            if (!bFresh)
                r = this.All;
            else
            {
                E.adminEntities.LeadReassignmentRules.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                r = this.All;
                E.adminEntities.LeadReassignmentRules.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return r;
        }
        public IQueryable<LeadReassignmentRules> AllActive
        {
            get
            {
                return E.adminEntities.LeadReassignmentRules
                        .Where(x => x.IsActive)
                        .OrderBy(x => x.Id)
                        .AsQueryable();
            }
        }

        public IQueryable<LeadReassignmentRules> AllActiveFresh
        {
            get
            {
                E.adminEntities.LeadReassignmentRules.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                var result = E.adminEntities.LeadReassignmentRules
                        .Where(x => x.IsActive)
                        .OrderByDescending(t => t.Priority)
                        .AsQueryable();
                E.adminEntities.LeadReassignmentRules.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
                return result;
            }
        }

        public LeadRuleDetailActions GetDetails(int id)
        {
            return new LeadRuleDetailActions(this.E, id, RecordType.Reasssignment);
        }
        public LeadReassignmentRules Get(int id)
        {
            return E.adminEntities.LeadReassignmentRules.FirstOrDefault(x => x.Id == id);
        }

        public LeadReassignmentRules Add(string title, Guid? userKey,byte? usrType, string description, bool isEnabled, string by, short selectedFilter, string customFilterValue, bool isChangeStatus, int? statusKey, bool isIncludeSubStatus, int? subStatusKey, int? rmType, short? skillId,int? scheduleId,bool isCheckWebCap,bool isCheckStateLicense)
        {
            var lpr = new LeadReassignmentRules
            {
                Id = GetId(),
                UsrKey = userKey,
                Title = title,
                IsActive = isEnabled,
                Description = description,
                Priority = GetPriority(),
                RM_Type = rmType,
                IsChangeStatus = isChangeStatus,
                IsIncludeSubStatus = isIncludeSubStatus,
                StatusKey = statusKey,
                SubStatusKey = subStatusKey,
                SkillId = skillId,
                ScheduleId = scheduleId,
                UserType = usrType,
                IsCheckWebCap = isCheckWebCap,
                IsCheckStateLicense = isCheckStateLicense,
                //IsIncludeSubStatus = isChangeStatus && isIncludeSubStatus,
                //StatusKey =isChangeStatus? statusKey:null,
                //SubStatusKey =isChangeStatus&&isIncludeSubStatus? subStatusKey:null,
                Added = new History
                {
                    By = by,
                    On = DateTime.Now
                },
                FilterSelection = selectedFilter,
                FilterCustomValue = customFilterValue
            };
            E.adminEntities.AddToLeadReassignmentRules(lpr);
            E.Save();
            return Get(lpr.Id);
        }
        public void Change(LeadReassignmentRules rule, string by)
        {
            
            rule.Changed.By = by;
            rule.Changed.On = DateTime.Now;
            //rule.IsChangeStatus = rule.IsChangeStatus;
            //rule.IsIncludeSubStatus = rule.IsChangeStatus && rule.IsIncludeSubStatus;
            //rule.StatusKey = rule.IsChangeStatus ? rule.StatusKey : null;
            //rule.SubStatusKey = rule.IsChangeStatus && rule.IsIncludeSubStatus ? rule.SubStatusKey : null;
            E.Save();
            Move(rule.Id, rule.Priority);
        }
        public void Delete(int id, short parentType)
        {
            E.FilterAreaActions.DeleteAll(id, parentType);
            GetDetails(id).DeleteAll();
            E.adminEntities.LeadReassignmentRules.DeleteObject(Get(id));
            E.Save();
        }
        public bool TitleExists(string title)
        {
            return All.Any(x => string.Compare(x.Title, title, true) == 0);
            // return All.Where(x => string.Compare(x.Title, title, true) == 0).Count() > 0;
        }

        private int GetId()
        {
            return E.adminEntities.LeadReassignmentRules.Any() ?
                E.adminEntities.LeadReassignmentRules.Max(x => x.Id) + 1 :
                1;
        }
        private int GetPriority()
        {
            return All.Any() ? All.Max(x => x.Priority) + 1 : 1;
        }

        //Change of priority order
        public void MoveUp(int recordId)
        {
            Move(recordId, Get(recordId).Priority + 1);
        }
        public void MoveDown(int recordId)
        {
            //done as indexing starts from 0.
            Move(recordId, Get(recordId).Priority - 2);
        }
        public void Move(int recordId, int priority)
        {
            E.adminEntities.ReassignmentPriorityChange(recordId, priority);
        }

        public void ToggleState(int recordId)
        {
            var u = (from T in E.adminEntities.LeadReassignmentRules.Where(x => x.Id.Equals(recordId)) select T).FirstOrDefault();
            if (u != null) u.IsActive = !u.IsActive;
            E.Save();
        }



        public LeadReassignmentRules Copy(int recordId, string by)
        {
            var x = Get(recordId);

            var y = Add("Copy of " + x.Title, x.UsrKey == Guid.Empty ? Guid.Empty : x.UsrKey,x.UserType, x.Description, x.IsActive, by, x.FilterSelection ?? 0, x.FilterCustomValue, x.IsChangeStatus, x.StatusKey, x.IsIncludeSubStatus, x.SubStatusKey, x.RM_Type, x.SkillId,x.ScheduleId,x.IsCheckWebCap,x.IsCheckStateLicense);

            var xdetails = GetDetails(recordId);
            var ydetails = GetDetails(y.Id);

            List<LeadRuleDetails> details = xdetails.All.ToList();
            foreach (var t in details)
                ydetails.Add(t.Shift, t.WeekDay, t.Working.Starts, t.Working.Ends);

            //add filters?
            IList<FilterArea> areas = E.FilterAreaActions.GetAll().Where(z => z.ParentKey == recordId && z.ParentType == 2).ToList();
            foreach (var t in areas)
                E.FilterAreaActions.Copy(t, y.Id, by);

            return y;
        }

        public bool IsEnabled(int id)
        {
            bool bAns = false;
            var u = (from T in E.adminEntities.LeadReassignmentRules.Where(x => x.Id.Equals(id)) select T).FirstOrDefault();
            if (u != null)
                bAns = u.IsActive;
            return bAns;
        }

        public void Enable(int id)
        {
            var u = (from T in E.adminEntities.LeadReassignmentRules.Where(x => x.Id.Equals(id)) select T).FirstOrDefault();
            if (u != null) u.IsActive = true;
            E.Save();
        }
    }
}





