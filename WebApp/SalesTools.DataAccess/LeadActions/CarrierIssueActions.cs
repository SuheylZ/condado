using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class CarrierIssueActions
    {
        private DBEngine engine = null;

        internal CarrierIssueActions(DBEngine reng)
        {
            engine = reng;
        }

        public Models.CarrierIssue Add(Models.CarrierIssue nCarrierIssue)
        {
            long id = 0;
            if (engine.Lead.CarrierIssues.Count() > 0)
                id = engine.Lead.CarrierIssues.Max(x => x.Key);
            nCarrierIssue.Key = id + 1;
            engine.Lead.CarrierIssues.AddObject(nCarrierIssue);
            engine.Save();
            return nCarrierIssue;
        }

        public void Change(Models.CarrierIssue nCarrierIssue)
        {
            engine.Save();
        }

        public void Delete(long nCarrierIssueID)
        {
            var objCarrierIssues =
                (from T in engine.leadEntities.CarrierIssues.Where(x => x.Key.Equals(nCarrierIssueID)) select T)
                    .FirstOrDefault();
            if (objCarrierIssues != null)
            {
                objCarrierIssues.IsDeleted = true;
            }

            engine.Save();
            // }

            //engine.Lead.CarrierIssues.DeleteObject(Get(nCarrierIssueID));
            //engine.Save();
        }

        public IEnumerable<Models.CarrierIssue> All
        {
            get
            {
                return engine.Lead.CarrierIssues;
            }
        }

        public IQueryable<Models.CarrierIssue> AllQueryable
        {
            get
            {
                return engine.Lead.CarrierIssues.AsQueryable();
            }
        }

        public Models.CarrierIssue Get(long nCarrierIssueKey)
        {
            return All.Where(x => x.Key == nCarrierIssueKey).FirstOrDefault();
        }

        public Models.CarrierIssue Get(long nCarrierIssueKey, long nAccountID)
        {
            return All.Where(x => x.Key == nCarrierIssueKey && x.AccountId == nAccountID).FirstOrDefault();
        }

        public IEnumerable<Models.CarrierIssue> GetAllByAccountID(long nAccountID, string searchText = "")
        {
            return All.Where(y => y.AccountId == nAccountID).Where(x => searchText == "" ? x.Issues == x.Issues : x.Issues.Contains(searchText));
        }
        public IQueryable<Models.CarrierIssue> GetAllQueryableByAccountID(long nAccountID)
        {
            return AllQueryable.Where(y => y.AccountId == nAccountID && (y.IsDeleted == false || y.IsDeleted == null));
        }
    }
}
