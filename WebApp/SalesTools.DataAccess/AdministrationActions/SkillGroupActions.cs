using System;
using System.Collections.Generic;
using System.Linq;
using SalesTool.DataAccess.Models;
using DBG = System.Diagnostics.Debug;

namespace SalesTool.DataAccess
{
    public class SkillGroupActions
    {
        private DBEngine engine = null;

        internal SkillGroupActions(DBEngine reng)
        {
            DBG.Assert(reng != null);
            engine = reng;
        }

        public IQueryable<Models.SkillGroup> All
        {
            get
            {
                return engine.adminEntities.SkillGroups.Where(x => (x.IsDeleted ?? false) == false).OrderBy(x => x.Name).AsQueryable();
            }
        }
        public IQueryable<NameIntValueLookup> SkillGroupLookups
        {
            get
            {

                return engine.adminEntities.SkillGroups
                    .Where(p=>p.IsDeleted==false)
                    .OrderBy(p => p.Name)
                    .Select(s => new NameIntValueLookup() { Name = s.Name, Value = s.Id });
                //.ToList();
            }
        }
        public Models.SkillGroup Get(int id)
        {
            return engine.adminEntities.SkillGroups.Where(x => (x.IsDeleted ?? false) == false && x.Id == id).FirstOrDefault();
        }

        public short Add(Models.SkillGroup skillGroup)
        {
            int Id = All.Count() > 0 ? engine.adminEntities.SkillGroups.Max(x => x.Id) + 1 : 1;
            skillGroup.Id = Convert.ToInt16(Id);
            skillGroup.IsDeleted = false;
            engine.adminEntities.SkillGroups.AddObject(skillGroup);
            engine.Save();
            return Convert.ToInt16(Id); 
        }
        public void Delete(int id)
        {
            var skillGroup = Get(id);
            if (skillGroup != null)
            {
                skillGroup.IsDeleted = true;
                engine.Save();
            }
        }
        public void Change(Models.SkillGroup group)
        {
            DBG.Assert(!group.IsDeleted??false);
            var tmp = Get(group.Id);
            tmp.Name = group.Name;
            tmp.Description = group.Description;
            tmp.IsDeleted = group.IsDeleted;
            tmp.Added = group.Added;
            tmp.Changed = group.Changed;
            engine.Save();
        }

        public bool IsUserAssignedToSkillGroups(Guid userId)
        {
            return IsUserAssignedToSkillGroups(userId, engine.SkillGroupActions.All);
        }

        public bool IsUserAssignedToSkillGroups(Guid userId, IEnumerable<SkillGroup> skillGroups)
        {
            return skillGroups.Any(k => k.Users.Any(u => u.Key.ToString() == userId.ToString()));
        }

        public IQueryable<User> UsersNotIn(int id)
        {           
            var users = engine.UserActions.GetAll().Where(x=>!x.SkillGroups.Any(y=>y.Id==id));
            //var users = engine.UserActions.GetAll().Where(x => x.SkillGroups.All(y => y.Id != id));
            return users;
        }


        public bool IsStatusAssignedToSkillGroups(int statusId)
        {
            return IsStatusAssignedToSkillGroups(statusId, engine.SkillGroupActions.All);
        }

        public bool IsStatusAssignedToSkillGroups(int statusId, IEnumerable<SkillGroup> skillGroups)
        {
            return skillGroups.Any(k => k.Statuses.Any(u => u.Id == statusId));
        }

        public IQueryable<Status> StatusesNotIn(int id)
        {
            var statuses = engine.StatusActions.All.Where(x => !x.SkillGroups.Any(y => y.Id == id));
            return statuses;
        }


        public void AssignQuickLink(short skillGroupId, int quickLinkID,string user, bool bRemove = false)
        {
            //  SZ [Dec 14, 2012] Add the link or remove it 
            DBG.Assert(Get(skillGroupId) != null);
            var obj = engine.adminEntities.QuickLinkSkills.Where(x => x.SkillGroupId == skillGroupId && x.QuickLinkId == quickLinkID).FirstOrDefault();
            if (obj == null && !bRemove)
            {
                obj = QuickLinkSkill.CreateQuickLinkSkill(1, new History() { By = user, On = DateTime.Now }, new History() { By = string.Empty, On = null });
                obj.QuickLinkId = quickLinkID;
                obj.SkillGroupId = skillGroupId;
                engine.adminEntities.QuickLinkSkills.AddObject(obj);
            }
            else if (obj != null && bRemove)
            {
                engine.adminEntities.QuickLinkSkills.DeleteObject(obj);
            }
            engine.Save();
        }

        public void AssignUsers(short skillGroupId, Guid[] keys, bool bAssign=true)
        {
            var SG = Get(skillGroupId);
            if (SG != null)
            {
                foreach (var u in keys)
                {
                    if (bAssign)
                    {
                        if (!SG.Users.Any(x => x.Key == u))
                            SG.Users.Add(engine.UserActions.Get(u));
                    }
                    else
                    {
                        if (SG.Users.Any(x => x.Key == u))
                            SG.Users.Remove(engine.UserActions.Get(u));
                    }
                }
                engine.Save();
            }
        }

        // SZ [May 15, 2013] Observed during a chat that skill group page removes users one by one. 
        // below is an efficient implementation that removes all users at once.
        public void UnassignAll(short skillGroupId)
        {
            var sg = Get(skillGroupId);
            if (sg != null)
            {
                sg.Users.Clear();
                engine.Save();
            }
        }


        public void AssignStatuses(short skillGroupId, int[] keys, bool bAssign = true)
        {
            var SG = Get(skillGroupId);
            foreach (var u in keys)
            {
                if (bAssign)
                {
                    if (!SG.Statuses.Any(x => x.Id == u))
                        SG.Statuses.Add(engine.StatusActions.Get(u));
                }
                else
                {
                    if (SG.Statuses.Any(x => x.Id == u))
                        SG.Statuses.Remove(engine.StatusActions.Get(u));
                }
            }
            engine.Save();
        }

        
    }
}
