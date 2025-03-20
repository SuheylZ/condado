using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{    
    public class FilterAreaActions
    {
        private DBEngine _engine = null;

        internal FilterAreaActions(DBEngine reng)
        {
            _engine = reng;
        }

        public void Add(FilterArea nFilter)
        {
            _engine.adminEntities.FilterAreas.AddObject(nFilter);
            _engine.Save();
        }

        public void Change(FilterArea nFilter)
        {
            _engine.Save();
        }

        public void Delete(int filterKey)
        {
            var U = (from T in _engine.adminEntities.FilterAreas.Where(x => x.Id ==filterKey) select T).FirstOrDefault();
            _engine.adminEntities.FilterAreas.DeleteObject(U);
            _engine.Save();
        }

        public void Copy(FilterArea nFilter, int parent, string by)
        {
            FilterArea copyFilter = new FilterArea();
            copyFilter.FilteredColumnTagkey = nFilter.FilteredColumnTagkey;
            copyFilter.Operator = nFilter.Operator;
            copyFilter.OrderNumber = GetMaxOrderNumber(parent, nFilter.ParentType) + 1; 
            copyFilter.ParentKey = parent;
            copyFilter.ParentType = nFilter.ParentType;
            copyFilter.Value = nFilter.Value;
            copyFilter.WithinLastNext = nFilter.WithinLastNext;
            copyFilter.WithinLastNextUnit = nFilter.WithinLastNextUnit;
            copyFilter.WithinPredefined = nFilter.WithinPredefined;
            copyFilter.WithinRadioButtonSelection = nFilter.WithinRadioButtonSelection;
            copyFilter.WithinSelect = nFilter.WithinSelect;            
            copyFilter.Added.By = by;
            copyFilter.Added.On = DateTime.Now;
            _engine.adminEntities.FilterAreas.AddObject(copyFilter);
            _engine.Save();
        }
        //YA[May 15, 2013] 
        public void CopyAll(int oldReportID,int newReportID, short parentType,string by)
        {
            IEnumerable<FilterArea> T = GetAllByParent(oldReportID, parentType);
            foreach (FilterArea nFilter in T)
            {
                //Does not use the copy function already exist as there are multiple records to copy and _engine.Save() gave error.
                //So, this manual copy of record and after all copy process then save will work fine.
                FilterArea copyFilter = new FilterArea();
                copyFilter.FilteredColumnTagkey = nFilter.FilteredColumnTagkey;
                copyFilter.Operator = nFilter.Operator;
                copyFilter.OrderNumber = GetMaxOrderNumber(newReportID, nFilter.ParentType) + 1;
                copyFilter.ParentKey = newReportID;
                copyFilter.ParentType = nFilter.ParentType;
                copyFilter.Value = nFilter.Value;
                copyFilter.WithinLastNext = nFilter.WithinLastNext;
                copyFilter.WithinLastNextUnit = nFilter.WithinLastNextUnit;
                copyFilter.WithinPredefined = nFilter.WithinPredefined;
                copyFilter.WithinRadioButtonSelection = nFilter.WithinRadioButtonSelection;
                copyFilter.WithinSelect = nFilter.WithinSelect;
                copyFilter.Added.By = by;
                copyFilter.Added.On = DateTime.Now;
                _engine.adminEntities.FilterAreas.AddObject(copyFilter);                                
            }
            _engine.Save();
        }

        public IEnumerable<FilterArea> GetAll(bool bFresh=false)
        {
            IEnumerable<FilterArea> R = null;

            if (!bFresh)
                R = _engine.adminEntities.FilterAreas;
            else
            {
                _engine.Admin.FilterAreas.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = _engine.adminEntities.FilterAreas;
                _engine.Admin.FilterAreas.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R;
        }

        public IEnumerable<FilterArea> GetAllByParent(int parentKey, short parentType)
        {
            return _engine.adminEntities.FilterAreas.Where(y => y.ParentKey == parentKey && y.ParentType == parentType).AsQueryable();                        
        }


        public FilterArea Get(int filterKey)
        {
            return _engine.adminEntities.FilterAreas.Where(x => x.Id==filterKey).FirstOrDefault();
        }

        public int GetMaxOrderNumber(int parentKey, short parentType)
        {
            var records = _engine.adminEntities.FilterAreas.Select(s => new { s.OrderNumber, s.ParentKey, s.ParentType }).Where(m => m.ParentKey == parentKey && m.ParentType == parentType).OrderByDescending(m => m.OrderNumber).FirstOrDefault();
            if (records != null && records.OrderNumber != null)
            {
                return records.OrderNumber.Value;
            }
            else
            {
                return 0;
            }
            
        }
        public void DeleteAll(int parentKey, short parentType)
        {
            IQueryable<FilterArea> allItems = GetAll().Where(y => y.ParentKey == parentKey && y.ParentType == parentType).AsQueryable();
            foreach (FilterArea item in allItems)
                _engine.adminEntities.FilterAreas.DeleteObject(item);
            _engine.Save();
        }
    }
}
