using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class UserSavedSearchActions
    {
        private DBEngine _engine = null;

        internal UserSavedSearchActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(UserSavedSearch entity)
        {
            _engine.Lead.UserSavedSearches.AddObject(entity);

            _engine.Save();
        }

        public void Change(UserSavedSearch entity)
        {
            _engine.Save();
        }

        public void Delete(long id)
        {
            var entity = this.Get(id);

            _engine.Lead.UserSavedSearches.DeleteObject(entity);

            _engine.Save();
        }

        public void AddCriteria(long searchId, string statuses, string subStatuses, string skillGroups, string users, string campaigns, string time)
        {
            UserSavedSearchCriteria entity;

            if (statuses.Length > 0)
            {
                entity = new UserSavedSearchCriteria
                {
                    SearchID = searchId,
                    SearchTypeID = 1,
                    Value = statuses
                };

                _engine.Lead.UserSavedSearchCriterias.AddObject(entity);
            }

            if (subStatuses.Length > 0)
            {
                entity = new UserSavedSearchCriteria
                {
                    SearchID = searchId,
                    SearchTypeID = 2,
                    Value = subStatuses
                };

                _engine.Lead.UserSavedSearchCriterias.AddObject(entity);
            }

            if (skillGroups.Length > 0)
            {
                entity = new UserSavedSearchCriteria
                {
                    SearchID = searchId,
                    SearchTypeID = 3,
                    Value = skillGroups
                };

                _engine.Lead.UserSavedSearchCriterias.AddObject(entity);
            }

            if (users.Length > 0)
            {
                entity = new UserSavedSearchCriteria
                {
                    SearchID = searchId,
                    SearchTypeID = 4,
                    Value = users
                };

                _engine.Lead.UserSavedSearchCriterias.AddObject(entity);
            }

            if (campaigns.Length > 0)
            {
                entity = new UserSavedSearchCriteria
                {
                    SearchID = searchId,
                    SearchTypeID = 5,
                    Value = campaigns
                };

                _engine.Lead.UserSavedSearchCriterias.AddObject(entity);
            }

            if (time.Length > 0 && time.ToUpper() != "ALL TIME")
            {
                entity = new UserSavedSearchCriteria
                {
                    SearchID = searchId,
                    SearchTypeID = 6,
                    Value = time
                };

                _engine.Lead.UserSavedSearchCriterias.AddObject(entity);
            }

            _engine.Save();
        }

        public void DeleteCriteriasBySearchId(long id)
        {
            var criterias = this.GetCriteriasBySearchId(id);

            foreach (var criteria in criterias)
            {
                _engine.Lead.UserSavedSearchCriterias.DeleteObject(criteria);
            }

            _engine.Save();
        }

        public IQueryable<UserSavedSearchCriteria> GetCriteriasBySearchId(long id)
        {
            return _engine.Lead.UserSavedSearchCriterias.Where(x => x.SearchID == id);
        }

        public IQueryable<UserSavedSearch> GetAllByUserId(Guid id)
        {
            return _engine.Lead.UserSavedSearches.Where(x => x.UserID == id);
        }

        public UserSavedSearch Get(long id)
        {
            return _engine.Lead.UserSavedSearches.Where(x => x.ID == id).FirstOrDefault();
        }
    }
}
