using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class ReportColumnsAction :BaseActions
    {       

        internal ReportColumnsAction(DBEngine engine) : base(engine) { }

        public void Add(Models.ReportColumns nReportColumn)
        {
            nReportColumn.ReportColumnID = GetMaxID() +1;
            E.adminEntities.ReportColumns.AddObject(nReportColumn);            
            E.Save();
        }
        public int GetMaxID()
        {
            int id = 0;
            if (E.adminEntities.ReportColumns.Count() > 0)
                id = E.adminEntities.ReportColumns.Max(x => x.ReportColumnID);
            return id;
        }
        public void Change(Models.ReportColumns nReportColumn)
        {
            E.Save();
        }

        public void Delete(int reportColumnID)
        {
            var U = (from T in E.adminEntities.ReportColumns.Where(x => x.ReportColumnID.Equals(reportColumnID)) select T).FirstOrDefault();
            E.adminEntities.ReportColumns.DeleteObject(U);
            E.Save();
        }

        public void DeleteByReportID(int reportID)
        {
            var X = E.adminEntities.ReportColumns.Where(x => x.ReportID == reportID).OrderBy(x => x.ColumnOrder);
            foreach (var item in X)
            {                
                E.adminEntities.ReportColumns.DeleteObject(item);
            }
            E.Save();
        }

        public IQueryable<Models.ReportColumns> GetAll()
        {
            return E.adminEntities.ReportColumns;
        }

        public IQueryable<Models.ReportColumns> GetAllByReportID(int reportID)
        {
            return GetAll().Where(x => x.ReportID == reportID).OrderBy(x => x.ColumnOrder);
        }

        public Models.ReportColumns Get(int reportColumnID)
        {
            return E.adminEntities.ReportColumns.Where(x => x.ReportColumnID.Equals(reportColumnID)).FirstOrDefault();
        }

        public void CopyAll(int oldReportID, int newReportID)
        {
            IQueryable<Models.ReportColumns> T = GetAllByReportID(oldReportID);
            int reportColumnID = GetMaxID() + 1;
            foreach (var oReportColumn in T)
            {
                Models.ReportColumns nReportColumn = new Models.ReportColumns();
                nReportColumn.ReportID = newReportID;
                nReportColumn.Tagkey = oReportColumn.Tagkey;
                nReportColumn.ColumnOrder = oReportColumn.ColumnOrder;
                nReportColumn.HasAggregateFunction = oReportColumn.HasAggregateFunction;
                nReportColumn.AggregateFunctionType = oReportColumn.AggregateFunctionType;
                nReportColumn.ReportColumnID = reportColumnID;
                E.adminEntities.ReportColumns.AddObject(nReportColumn);
                reportColumnID++;
            }
            E.Save();
        }

    }
}
