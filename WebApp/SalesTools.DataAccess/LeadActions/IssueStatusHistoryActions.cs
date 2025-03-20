
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class IssueStatusHistoryActions
    {
        private DBEngine _engine = null;

        internal IssueStatusHistoryActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Add(IssueStatusesHistory entity)
        {
            entity.AddedOn = DateTime.Now;

            _engine.Lead.IssueStatusesHistory.AddObject(entity);

            _engine.Save();
        }

        public void Change(IssueStatusesHistory entity)
        {
            entity.ChangedOn = DateTime.Now;

            _engine.Save();
        }

        public void Delete(long key)
        {
            _engine.Lead.IssueStatusesHistory.DeleteObject(this.Get(key));

            _engine.Save();
        }

        public void DeleteByCarrierIssueID(long key)
        {
            var entities = this.GetAllByCarrierIssueID(key);

            foreach (var entity in entities)
            {
                _engine.Lead.IssueStatusesHistory.DeleteObject(entity);
            }

            _engine.Save();
        }

        public IssueStatusesHistory GetLastIssueStatusByCarrierIssueID(long id)
        {
            var entities = GetAllByCarrierIssueID(id);

            if (entities == null)
            {
                return null;
            }

            return entities.OrderByDescending(x=>x.ID).FirstOrDefault();
        }

        public IQueryable<IssueStatusesHistory> GetAllByCarrierIssueID(long id)
        {
            return this.GetAll().Where(x => x.CarrierIssueID == id);
        }

        //public IQueryable<IssueStatusesHistory> GetAllByIndividualID(long id)
        //{
        //    return this.All().Where(x => x.IndividualId == id);
        //}

        public IQueryable<IssueStatusesHistory> GetAll()
        {
            return _engine.Lead.IssueStatusesHistory;
        }

        public IssueStatusesHistory Get(long id)
        {
            return this.GetAll().Where(x => x.ID == id).FirstOrDefault();
        }
    }
}
