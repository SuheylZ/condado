using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class ManageCompanyActions: BaseActions
    {
        //private DBEngine _engine = null;

        internal ManageCompanyActions(DBEngine reng):base(reng)
        {}

        public void Add(Company entity)
        {
            entity.IsActive = true;
            E.adminEntities.Companies1.AddObject(entity);
            E.Save();
        }

        public void Change(Company entity)
        {
            E.Save();
        }
        // SZ [July 17, 2014] Checking the delete functionality.
        // Reference: [7/16/2014 5:45:17 PM] John Dobrotka: Please make sure that every area that allows deleting on 
        // Normal View, Prioritized View and Leads.aspx (Tabs) is security controlled. they should all be soft deletes 
        // and should only have the delete option show if the Soft Delete permission is set in thir security
        public void Delete(int id)
        {
            var U = (from T in E.adminEntities.Companies1.Where(x => x.Id == id) select T).FirstOrDefault();
            U.IsActive = false; 
            E.Save();
        }

        public IQueryable<Company> GetAll()
        {
            return E.adminEntities.Companies1.Where(x => x.IsActive != false);
        }

        public Company Get(int id)
        {
            return E.adminEntities.Companies1.Where(x => x.Id == id && x.IsActive != false).FirstOrDefault();
        }
        public IQueryable<Company> GetSelectedRecords(int skipRecords = 0, int takeRecords = 0)
        {
            return E.adminEntities.Companies1.Where(x => x.IsActive != false).OrderBy(x => x.Title).Skip(skipRecords).Take(takeRecords);
        }
    }
}
