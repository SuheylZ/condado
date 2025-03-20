using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{    
    public class ManageQuickLinkActions
    {
        private DBEngine engine = null;

        internal ManageQuickLinkActions(DBEngine rengine)
        {
            engine = rengine;
        }

        public void Add(Models.QuickLink nQuickLink)
        {
            nQuickLink.IsDeleted= false;
            engine.adminEntities.QuickLinks.AddObject(nQuickLink);
            nQuickLink.Changed.On= DateTime.Now;
            engine.Save();
        }

        public void Change(Models.QuickLink nQuickLink)
        {
            engine.Save();
        }

        public void Delete(int quickLinkID)
        {
            var U = (from T in engine.adminEntities.QuickLinks.Where(x => x.Id.Equals(quickLinkID)) select T).FirstOrDefault();
            U.IsDeleted = true;
            engine.Save();
        }

        public void MakeEnabled(int quickLinkID)
        {
            var U = (from T in engine.adminEntities.QuickLinks.Where(x => x.Id.Equals(quickLinkID)) select T).FirstOrDefault();
            U.Enabled = !U.Enabled;
            engine.Save();
        }

        public IQueryable<Models.QuickLink> GetAll()
        {
            return engine.adminEntities.QuickLinks.Where(x => x.IsDeleted!= true).OrderBy(p=>p.Id);
        }

        public Models.QuickLink Get(int quickLinkId)
        {
            return engine.adminEntities.QuickLinks.Where(x => x.Id.Equals(quickLinkId) && x.IsDeleted!= true).FirstOrDefault();
        }

        public IQueryable<Models.SkillGroup> SkillGroupsAssignedToQuickLinks(int id)
        {
            return engine.adminEntities.SkillGroups.Where(x => x.QuickLinkSkills.Count() > 0 && x.IsDeleted==false && x.QuickLinkSkills.Any(y => y.QuickLinkId == id)).AsQueryable<SkillGroup>();
        }
        public IQueryable<Models.SkillGroup> SkillGroupsNotAssignedToQuickLinks(int id)
        {         

            var skillGroupsAssigned = engine.adminEntities.SkillGroups.Where(x => x.IsDeleted == false && !x.QuickLinkSkills.Any(y => y.QuickLinkId == id));
            return skillGroupsAssigned.AsQueryable();
            
        }

        public IQueryable<Models.QuickLink> LoadUsersQuickLinks(Guid userKey)
        {
            //var U = engine.entities.QuickLinks.Where(x => x.QuickLinkSkills.Count() > 0 && x.IsDeleted == false && x.QuickLinkSkills.Any(y => y.SkillGroups.Users.Any(z => z.Key == userKey)));
            var U = engine.adminEntities.QuickLinks.Where(x => x.IsDeleted == false);
            return U.AsQueryable();
        }

        public IQueryable<Models.FilteredQuickLinks> GetQuickLinksFor(Guid userKey)
        {
            IQueryable<Models.FilteredQuickLinks> Ans = null;
            Ans = engine.Admin.FilteredQuickLinks.Where(x => userKey.Equals(x.UserKey));
           return Ans;
            //IEnumerable<Models.QuickLink> results = null;

            //results = engine.QuickLinksActions.GetAll().Where(
            //        x => x.QuickLinkSkills.Count() > 0 && !(x.IsDeleted ?? false) &&

            //            x.QuickLinkSkills.Any(
            //            y => y.SkillGroups.IsDeleted == false &&
            //            y.SkillGroups.Users.Count() > 0 &&
            //            y.SkillGroups.Users.Any(
            //                z => z.Key == userKey
            //                )
            //            )
            //          ).AsEnumerable().ToList();

            //results = engine.QuickLinksActions.GetAll().Where(x => !(x.IsDeleted ?? false))
            //    .Join(engine.Admin.QuickLinkSkills,
            //        a => a.Id,
            //        b => b.QuickLinkId,
            //        (a, b) => new { QL = a, QLS = b })
            //        .Join(engine.Admin.SkillGroups.Where(s1 => !s1.IsDeleted),
            //        a => a.QLS.SkillGroupId,
            //        b => b.Id,
            //        (a, b) => new { QL = a.QL, SG = b })
            //        .Where(x => x.SG.Users.Any(y => y.Key == userKey)).Select(
            //        x => x.QL).AsEnumerable().ToList();
            // return results;
        }
    }

}
