using System;
using System.Collections.Generic;
using System.Linq;
using SalesTool.DataAccess.Models;
using Sql = System.Data.SqlClient;


namespace SalesTool.DataAccess
{
    public class ManageCampaignActions: BaseActions
    {
        internal ManageCampaignActions(DBEngine reng): base(reng){}
        //{
        //    E = reng;
        //}

        public bool Exists(int campaignId)
        {
            return E.Admin.Campaigns1.Count(x => x.ID == campaignId) > 0;
        }

        public int Add(Campaign nCampaign)
        {
            nCampaign.IsDeleted = false;            
            E.adminEntities.Campaigns1.AddObject(nCampaign);
            E.Save();
            int CampaignId = nCampaign.ID;

            return CampaignId;
        }

        public void Change(Campaign nCampaign)
        {
            E.Save();
        }


        // SZ [July 17, 2014] Checking the delete functionality.
        // Reference: [7/16/2014 5:45:17 PM] John Dobrotka: Please make sure that every area that allows deleting on 
        // Normal View, Prioritized View and Leads.aspx (Tabs) is security controlled. they should all be soft deletes 
        // and should only have the delete option show if the Soft Delete permission is set in thir security
        public void Delete(int campaignKey)
        {
            var U = (from T in E.adminEntities.Campaigns1.Where(x => x.ID == campaignKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            E.Save();
        }

        public IQueryable<Campaign> GetAllEvenSoftDeleted()
        {
            return E.adminEntities.Campaigns1;
        }

        public IQueryable<Campaign> GetAll()
        {
            return E.adminEntities.Campaigns1.Where(x => x.IsDeleted != true).AsQueryable().OrderBy(x=>x.Title);
        }

        public Campaign Get(int campaignKey)
        {
            return E.adminEntities.Campaigns1.Where(x => x.ID == campaignKey && x.IsDeleted != true).FirstOrDefault();
        }
        public IQueryable<Campaign> GetSelectedRecords(int skipRecords=0, int takeRecords = 0)
        {
            return E.adminEntities.Campaigns1.Where(x => x.IsDeleted != true).OrderBy(x => x.Title).Skip(skipRecords).Take(takeRecords);
        }

        public string GetOutpulse(int campaignId, Guid userId)
        {
            const string K_QUERY = "SELECT dbo.CalculateOutpulseId(@campaignid, @userid)";
            System.Data.Objects.ObjectResult<string> x = E.Lead.ExecuteStoreQuery<string>(K_QUERY, new Sql.SqlParameter[] {new Sql.SqlParameter("campaignid", campaignId),new Sql.SqlParameter("userid", userId) });
            return x.FirstOrDefault();
        }

        public string DescriptionOf(int id)
        {
            return
                E.adminEntities.ExecuteStoreQuery<string>("select ISNULL([cmp_description], '') from [campaigns] where [cmp_id] = @id",
                new System.Data.SqlClient.SqlParameter[] { new System.Data.SqlClient.SqlParameter("id", id) })
                .FirstOrDefault()?? string.Empty;
        }
    }

    public class ManageCampaignTypeActions: BaseActions
    {
        internal ManageCampaignTypeActions(DBEngine reng): base(reng){}
        //{
        //    E = reng;
        //}

        public void Add(CampaignType nCampaignType)
        {
            E.adminEntities.CampaignTypes.AddObject(nCampaignType);
            E.Save();
        }

        public void Change(CampaignType nCampaignType)
        {
            E.Save();
        }

        // SZ [July 17, 2014] Checking the delete functionality.
        // Reference: [7/16/2014 5:45:17 PM] John Dobrotka: Please make sure that every area that allows deleting on 
        // Normal View, Prioritized View and Leads.aspx (Tabs) is security controlled. they should all be soft deletes 
        // and should only have the delete option show if the Soft Delete permission is set in thir security

        
        public void Delete(int campaignTypeKey)
        {
            var U = (from T in E.adminEntities.CampaignTypes.Where(x => x.Id == campaignTypeKey) select T).FirstOrDefault();

            //////////////////////////////////
            // SZ July 17, 2014: What exactly does this piece of code do without the line below? 
            // added the line below. Coders forgetting to write such tiny things should be fucked till death!
            U.IsDeleted = true;
            ///////////////////////////////////

            E.Save();
        }

        public IQueryable<CampaignType> GetAll()
        {
            return E.adminEntities.CampaignTypes.Where(c => c.IsActive == true && c.IsDeleted == false).OrderBy(m => m.Text);
        }

       
        public CampaignType Get(int campaignTypeKey)
        {
            return E.adminEntities.CampaignTypes.Where(x => x.Id == campaignTypeKey).FirstOrDefault();
        }

    
    }

}
