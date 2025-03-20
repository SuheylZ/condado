using System;

using System.Collections.Generic;
using System.Linq;


namespace SalesTool.DataAccess
{
    public enum SubType { S1 = 1, S2 = 2, S3 = 3 };
    public enum RecordType { Prioritization = 1, Retention = 2, Reasssignment = 8 }

    public class LeadRuleDetailActions
    {
        DBEngine engine = null;
        bool bIsRetention = true; // 0: Lead_prioritization, 1: Lead_retention
        int parentId = 0;

        internal LeadRuleDetailActions(DBEngine reng, int parent, RecordType type = DataAccess.RecordType.Retention)
        {
            engine = reng;
            bIsRetention = type == DataAccess.RecordType.Retention;
            parentId = parent;
        }

        public IQueryable<Models.LeadRuleDetails> All
        {
            get
            {
                return engine.adminEntities.LeadRuleDetails
                    .Where(x => x.Key == parentId && x.RecordType == bIsRetention)
                    .OrderBy(x => x.Day)
                    .AsQueryable();
            }
        }
        public Models.LeadRuleDetails Get(int id)
        {
            return All
                .Where(x => x.Id == id).FirstOrDefault();
        }
        public IQueryable<Models.LeadRuleDetails> GetBy(SubType stype)
        {
            return All.Where(x => x.SubType == (short)(stype)).OrderBy(x => x.Day).AsQueryable();
        }

        public Models.LeadRuleDetails Add(SubType stype, DayOfWeek day, DateTime starts, DateTime ends)
        {
            Models.LeadRuleDetails obj = new Models.LeadRuleDetails
            {
                Id = GetId(),
                Key = parentId,
                RecordType = bIsRetention,
                Working = { Starts = starts, Ends = ends },
                SubType = Convert.ToByte((int)(stype))
            };
            obj.WeekDay = day;
            engine.adminEntities.AddToLeadRuleDetails(obj);
            engine.Save();
            return obj;
        }
        public void Change(Models.LeadRuleDetails record)
        {
            engine.Save();
        }
        public void Delete(int id)
        {
            engine.adminEntities.LeadRuleDetails.DeleteObject(Get(id));
            engine.Save();
        }
        public void DeleteAll()
        {
            foreach (var item in All)
                engine.adminEntities.LeadRuleDetails.DeleteObject(item);
            engine.Save();
        }

        private int GetId()
        {
            return engine.adminEntities.LeadRuleDetails.Any() ? engine.adminEntities.LeadRuleDetails.Max(x => x.Id) + 1 : 1;
            // return (engine.adminEntities.LeadRuleDetails.Count() > 0) ? engine.adminEntities.LeadRuleDetails.Max(x => x.Id) + 1 : 1;
        }
    }
}
