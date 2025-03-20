using System;
using System.Collections.Generic;
using System.Linq;
using SalesTool.DataAccess;

namespace SalesTool.DataAccess
{
    public class TagFieldsActions
    {
        private DBEngine engine = null;

        internal TagFieldsActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(Models.TagFields nTagFields)
        {
            engine.adminEntities.TagFields.AddObject(nTagFields);
            engine.Save();

        }

        public void Change(Models.TagFields nTagFields)
        {
            engine.Save();
        }

        public void Delete(int tagFieldsKey)
        {
            var U = (from T in engine.adminEntities.TagFields.Where(x => x.Id == tagFieldsKey) select T).FirstOrDefault();
            engine.adminEntities.TagFields.DeleteObject(U);
            engine.Save();
        }

        public IQueryable<Models.TagFields> GetAll(bool bFresh = false)
        {
            IQueryable<Models.TagFields> R = null;
            if (!bFresh)
                R = engine.adminEntities.TagFields;
            else
            {
                engine.Admin.TagFields.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = engine.adminEntities.TagFields;
                engine.Admin.TagFields.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R;
        }
        public IQueryable<Models.TagFields> GetAllApplicationFieldTags(bool bFresh = false)
        {
            IQueryable<Models.TagFields> R = null;
            if (!bFresh)
                R = engine.adminEntities.TagFields.Where(x=>x.HasFilter == true);
            else
            {
                engine.Admin.TagFields.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = engine.adminEntities.TagFields.Where(x => x.HasFilter == true);
                engine.Admin.TagFields.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R;
        }

        public List<Models.TagFields> GetAllByInstanceType(short nInsuranceType)
        {
            List<Models.TagFields> R = null;
            if (nInsuranceType == 0)//Senior
                R = engine.adminEntities.TagFields.Where(p => p.basedatas.Any(p1 => p1.Type == 1 || p1.Type == 3)).ToList();
            if (nInsuranceType == 1 || nInsuranceType == 2) //Auto & Home, Life
                R = engine.adminEntities.TagFields.Where(p => p.basedatas.Any(p1 => p1.Type == 1 || p1.Type == 2)).ToList();
            return R;
        }

        public IQueryable<Models.TagFields> GetAllReportTagsByBaseDataID(int baseDataID = 1)
        {
            var T = engine.adminEntities.BaseQueryDatas.SingleOrDefault(x => x.Id == baseDataID);
            if (T != null)
                return T.field_tags.AsQueryable();
            else
                return null;
        }

        public Models.TagFields Get(int tagFieldsKey)
        {
            return engine.adminEntities.TagFields.Where(x => x.Id == tagFieldsKey).FirstOrDefault();
        }
    }

}
