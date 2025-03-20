using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{    
    public class ManageAlertsActions : BaseActions
    {
        internal ManageAlertsActions(DBEngine engine) : base(engine) { }
        
        
        public void Add(Models.Alert nAlert)
        {
            nAlert.Id = GetMaxID() + 1;
            nAlert.IsDeleted = false;
            nAlert.Added.On = DateTime.Now;            
            E.adminEntities.Alerts1.AddObject(nAlert);            
            E.Save();
        }
        public int GetMaxID()
        {
            int id = 0;
            if (E.adminEntities.Alerts1.Count() > 0)
                id = E.adminEntities.Alerts1.Max(x => x.Id);
            return id;
        }
        public void Change(Models.Alert nAlert)
        {
            nAlert.Changed.On = DateTime.Now;
            E.Save();
        }


        // SZ [July 17, 2014] Checking the delete functionality.
        // Reference: [7/16/2014 5:45:17 PM] John Dobrotka: Please make sure that every area that allows deleting on 
        // Normal View, Prioritized View and Leads.aspx (Tabs) is security controlled. they should all be soft deletes 
        // and should only have the delete option show if the Soft Delete permission is set in thir security
        public void Delete(int alertID)
        {
            var U = (from T in E.adminEntities.Alerts1.Where(x => x.Id == alertID) select T).FirstOrDefault();
            U.IsDeleted = true;
            E.Save();
        }

        public void MakeEnabled(int alertID)
        {
            var U = (from T in E.adminEntities.Alerts1.Where(x => x.Id == alertID) select T).FirstOrDefault();
            U.Enabled = !U.Enabled;
            E.Save();
        }

        public IQueryable<Models.Alert> GetAll()
        {
            return E.adminEntities.Alerts1.Where(x => x.IsDeleted != true );
        }

        public IQueryable<Models.Alert> GetAllCampaignAlerts()
        {
            return E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign == true );
        }
        public IQueryable<Models.Alert> GetAllCampaignAlertsByCampaignId(string campaignId)
        {
            return E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign == true && x.Enabled== true && x.Value.Equals(campaignId));
        }
        public IQueryable<Models.Alert> GetAllCampaignAlertsByConditions(string campaignId,int statusID = 0)
        {
            IQueryable<Models.Alert> T = null;
            if(statusID > 0)
                T = E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign == true && x.Enabled == true && x.Value.Equals(campaignId) && (x.StatusKey == statusID || x.StatusKey == 0 || x.StatusKey == null));
            else
                T = E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign == true && x.Enabled == true && x.Value.Equals(campaignId));
            return T;
        }

        public IQueryable<Models.Alert> GetAllTimerAlerts()
        {
            return E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign != true);
        }

        public IQueryable<Models.Alert> GetAllEnabledTimerAlerts()
        {
            return E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign != true && x.Enabled == true);
        }

        public IQueryable<Models.Alert> GetAllTimerAlertsByCampaignID(string campaignID = "")
        {
            return E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign != true && x.Enabled == true && (x.Value.Equals(campaignID) || x.Value.Equals("")));
        }

        public IQueryable<Models.Alert> GetAllTimerAlertsByConditions(string campaignID = "", int statusID = 0)
        {
            IQueryable<Models.Alert> T = E.adminEntities.Alerts1.Where(x => x.IsDeleted != true && x.IsCampaign != true && x.Enabled == true );
            if(statusID > 0)
                T = T.Where(x => (x.StatusKey == statusID || x.StatusKey == 0 || x.StatusKey == null));
            if(!string.IsNullOrEmpty(campaignID))
                T = T.Where(x => (x.Value.Equals(campaignID) || x.Value.Equals("") || x.Value == null));
            return T;
        }

        public Models.Alert Get(int alertID)
        {
            return E.adminEntities.Alerts1.Where(x => x.Id == alertID && x.IsDeleted != true).FirstOrDefault();
        }

        public IQueryable<Models.AlertType> GetAllAlertTypes()
        {
            return E.adminEntities.AlertTypes;
        }
    }

}
