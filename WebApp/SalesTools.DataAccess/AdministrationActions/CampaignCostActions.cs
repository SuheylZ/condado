using System;
using System.Collections.Generic;
using System.Linq;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class CampaignCostActions 
    {
        private DBEngine _engine = null;

        internal CampaignCostActions(DBEngine reng)
        {
            _engine = reng;
        }

        public void Add(CampaignCost CampaignCost)
        {
            _engine.adminEntities.CampaignCosts.AddObject(CampaignCost);
            _engine.Save();
        }

        public void Change(CampaignCost CampaignCost)
        {
            _engine.Save();
        }

        public IQueryable<CampaignCost> GetAll()
        {
            return _engine.adminEntities.CampaignCosts;
        }

        public CampaignCost Get(int id)
        {
            return _engine.adminEntities.CampaignCosts.Where(x => x.CampaignCostId == id).FirstOrDefault();
        }

        // SZ [July 17, 2014] Checking the delete functionality.
        // Reference: [7/16/2014 5:45:17 PM] John Dobrotka: Please make sure that every area that allows deleting on 
        // Normal View, Prioritized View and Leads.aspx (Tabs) is security controlled. they should all be soft deletes 
        // and should only have the delete option show if the Soft Delete permission is set in thir security
        public void Delete(int id, bool bSoft = false)
        {
            var U = (from T in _engine.adminEntities.CampaignCosts.Where(x => x.CampaignCostId == id) select T).FirstOrDefault();
            if (bSoft) {  }
            else
                _engine.adminEntities.CampaignCosts.DeleteObject(U);
            _engine.Save();
        }

        public void DeleteWhenParentCampaignDeleted(int campaignId)
        {
            var U = (from T in _engine.adminEntities.CampaignCosts.Where(x => x.CampaignId == campaignId) select T).FirstOrDefault();

            if (U != null)
            {
                _engine.adminEntities.CampaignCosts.DeleteObject(U);
                _engine.Save();
            }
        }
    }
}
