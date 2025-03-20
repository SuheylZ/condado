using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace SalesTool.DataAccess
{
    public class LeadsActions : BaseActions
    {
        internal LeadsActions(DBEngine inputEngine)
            : base(inputEngine)
        { }

        public bool Exists(long leadID)
        {
            return E.Lead.Leads.Count(x => x.Key == leadID) > 0;
        }
        public Models.Lead Add(Models.Lead inputLead, string by="")
        {
            inputLead.IsActive = true;
            inputLead.IsDeleted = false;
            inputLead.IsDuplicate = false;
            
            if(!string.IsNullOrEmpty(by))
                inputLead.AddedBy = by;

            inputLead.AddedOn = DateTime.Now;
            E.leadEntities.Leads.AddObject(inputLead);
            E.Save();
            return inputLead;
        }
        public IQueryable<Models.Lead> All
        {
            get
            {
                //E.leadEntities.Leads.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                return E.leadEntities.Leads.Where(x => x.IsDeleted != true && x.IsDuplicate != true);
            }
        }
        public void Update(Models.Lead inputLead, string by="")
        {
            if(!string.IsNullOrEmpty(by))
                inputLead.ChangedBy = by;
            inputLead.ChangedOn = DateTime.Now;

            inputLead.Account.ChangedBy = inputLead.ChangedBy;
            inputLead.Account.ChangedOn = DateTime.Now;
            E.Save();
        }
        //public void DeleteFlag(Int64 Key)
        //{
        //    var U = E.Lead.Leads.Where(x => x.Key == Key).FirstOrDefault();
        //    //var U = (from T in E.Lead.Leads.Where(x => x.Key == Key) select T).FirstOrDefault();
        //    U.IsDeleted = true;
        //    E.Save();

        //    //Models.LeadEntities DeleteLead = new Models.LeadEntities();
        //    //var deleteObject = DeleteLead.Leads.Where(x => x.Key == Key).FirstOrDefault();
        //    //deleteObject.IsDeleted = true;
        //    //DeleteLead.SaveChanges();
        //}
        public void Delete(Int64 Key, string by="", bool bPermanent = false)
        {
            var U = (from T in E.Lead.Leads.Where(x => x.Key == Key) select T).FirstOrDefault();
            if (!bPermanent)
            {
                U.Account.ChangedBy = by;
                U.Account.ChangedOn = DateTime.Now;
                U.ChangedBy = by;
                U.ChangedOn = DateTime.Now;
                U.IsDeleted = true;
            }
            else
                E.Lead.Leads.DeleteObject(U);

            E.Save();
        }
        public Models.Lead Get(Int64 Key)
        {
            return E.leadEntities.Leads.Where(x => x.Key == Key).FirstOrDefault();
        }
        public List<Models.Lead> Get(List<Int64> leadIds)
        {
            var foo = E.leadEntities.Leads.AsQueryable<Models.Lead>()
                 .Where(codeData => leadIds.Contains(codeData.Key));
            return foo.ToList();
        }
        public bool IsLeadAccountSoftDeleted(long accountId)
        {


            return E.Lead.Leads.Any(x => x.AccountId == accountId && x.IsDeleted == true);
            //return E.Lead.Accounts.Where(x => x.Key == accountId && x.IsDeleted == true).Any();
            //const string K_SQL = "SELECT COUNT(*) FROM [accounts] A WHERE A.[act_key] = @id";
            //System.Data.Objects.ObjectResult<int> A = E.Lead.ExecuteStoreQuery<int>(K_SQL, new System.Data.SqlClient.SqlParameter("@id", accountId));
            //return A.FirstOrDefault() > 0;
            //return E.Lead.Accounts.Where(x => x.Key == accountId && !(x.IsDeleted??false)).Count() > 0;
        }

        public IEnumerable<leadView> GetAllViews()
        {
            //Models.LeadEntities dbView = new Models.LeadEntities();
            var loadAccounts = E.AccountActions.GetAll();
            var loadLeads = All;
            var loadIndividuals = E.IndividualsActions.GetAll().Where(x => x.IsDeleted != true);

            var firstJoin = (from x in loadAccounts
                             join y in loadLeads
                             on x.Key equals y.AccountId
                             select new
                             {
                                 leadId = y.Key,
                                 accountId = x.Key,
                                 primaryId = x.PrimaryIndividualId,
                                 datecreated = y.AddedOn
                             });

            var secondjoin = (from y in loadIndividuals

                              join x in firstJoin
                              on y.AccountId equals x.primaryId
                              select new
                              {
                                  leadId = x.leadId,
                                  accountId = x.accountId,
                                  individualId = y.Key,
                                  firstName = y.FirstName,
                                  lastName = y.LastName,
                                  dateOfBirth = y.Birthday,
                                  dateCreated = x.datecreated,
                                  dayPhone = y.DayPhone,
                                  eveningPhone = y.EveningPhone,
                                  cellPhone = y.CellPhone,
                              });
            List<leadView> returnvalue = new List<leadView>();
            return returnvalue;
        }

        public Models.Status GetStatus(long leadid)
        {
            return E.StatusActions.Get(Get(leadid).StatusId ?? default(int));
        }
        //YA[June 20, 2013] 
        public void EnableDuplicateFlag(long leadId, string by="")
        {
            var T = Get(leadId);

            T.ChangedBy = by;
            T.ChangedOn = DateTime.Now;
            T.Account.ChangedBy = by;
            T.Account.ChangedOn = DateTime.Now;

            T.IsDuplicate = true;
            E.Save();
        }
        public void DisableDuplicateFlag(long leadId)
        {
            var T = Get(leadId);
            T.IsDuplicate = false;
            E.Save();
        }

        //SZ [Jan 30, 2013] This is a dummy sample function that involves the join of entities with two differnt contexts
        // DO NOT USE THIS FUNCTION, IT IS ONLY A SAMPLE
        // The key is ToList(). get the data from entities on app server and perform join.
        // Limitation of EntityFramework!!!

        // Jack be nimble
        // Jack be quick!
        // Jack jump over 
        // the candlestick
        object Jack_Jump_Now(long accID)
        {
            return Engine.AccountActions.GetLeads(accID).ToList().Join(Engine.ManageCampaignActions.GetAll().ToList(),
                x => x.CampaignId, y => y.ID, (x, y) => new { Leads = x, Campaign = y })
                .Select(c => new
                {
                    Id = c.Leads.Key,
                    Title = c.Campaign.Title,
                    CreatedOn = c.Leads.TimeCreated,
                    TrackingCode = c.Leads.TrackingCode,
                    SourceCode = c.Leads.SourceCode
                }).AsEnumerable();
        }

        //private Object GetLeadsView()
        //{

        //    var leads = Engine.LeadsActions.All;
        //    var accounts = Engine.AccountActions.GetAll();
        //    var individuals = Engine.IndividualsActions.GetAll();

        //    var campaigns = Engine.ManageCampaignActions.GetAll();
        //    var states = Engine.ManageStates.GetAll();

        //    // for left outer join
        //    //from c in campaigns.Where(c => c.Id == l.CampaignId).DefaultIfEmpty()


        //    var result = from l in leads
        //                 join c in campaigns on l.CampaignId equals c.ID
        //                 join a in accounts on l.Key equals a.PrimaryLeadKey
        //                 join i in individuals on a.PrimaryIndividualId equals i.Key
        //                 from s in states.Where(x => x.Id == i.StateID).DefaultIfEmpty()

        //                 select new //leadView()
        //                 {
        //                     leadId = l.Key,
        //                     accountId = a.Key,
        //                     individualId = i.Key,
        //                     firstName = i.FirstName,
        //                     lastName = i.LastName,
        //                     dateOfBirth = i.Birthday,
        //                     dateCreated = l.AddedOn,
        //                     dayPhone = i.DayPhone,
        //                     eveningPhone = i.EveningPhone,
        //                     cellPhone = i.CellPhone,
        //                     userAssigned = a.User == null ? "-- Unassigned --" : a.User.FirstName + " " + a.User.LastName,
        //                     CSR = a.Csr == null ? "-- Unassigned --" : a.Csr.FirstName + " " + a.Csr.LastName,
        //                     TA = a.TransferUserKey == null ? "-- Unassigned --" : Engine.UserActions.Get(a.TransferUserKey.Value).FullName, //.FirstName + " " + a.TransferUser.LastName,
        //                     OutpulseId = string.Empty, // this.GetOutpulseId(l.CampaignId ?? 0),
        //                     leadStatus = l.StatusL == null ? "" : l.StatusL.Title,
        //                     Status = l.StatusL,
        //                     SubStatus1 = string.Empty, // Engine.StatusActions.Exists(l.SubStatusId ?? 0) ? Engine.StatusActions.Get(l.SubStatusId ?? 0).Title : "",
        //                     Campaign = c,
        //                     leadCampaign = c.Title,
        //                     User = a.User,
        //                     state = (s == null) ? "" : s.FullName,
        //                     AssignedUserKey = a.AssignedUserKey
        //                 };
        //}
        //private string formatDate(DateTime? inputDate)
        //{
        //    if (inputDate != null)
        //    {
        //        return null;
        //    }
        //    else
        //    {
        //        return (Convert.ToDateTime(inputDate)).ToString("M/dd/yyyy");
        //    }
        //}

        public IQueryable<Models.ViewLeadMarketing> LeadMarketing
        {
            get
            {
                return E.leadEntities.ViewLeadMarketing;
            }
        }

        /// <summary>
        /// Merges the information from the source account to the target account and makes the mentioned persons as primary and secondary.
        /// </summary>
        /// <param name="targetLeadId">Target lead id which the information is written to</param>
        /// <param name="sourceLeadId">source lead the information is read from</param>
        /// <param name="primaryPersonId">person id to be made as primary in the target account</param>
        /// <param name="secondaryPersonId">person id to be made as secondary in the target account</param>
        public void Merge(long targetLeadId, long sourceLeadId, long primaryPersonId, long secondaryPersonId, string By,List<Int64> lstRemovedIndividuals = null)
        {
            EntityCloner Cloner = new EntityCloner(E);
            // SZ [Jul 12, 2013] get the leads and copy the lists from the source from the target, also get the accounts to prepare for merging
            Models.Account tAccount = Get(targetLeadId).Account, sAccount = Get(sourceLeadId).Account;

            // SZ[Jul 12, 2013] copy the indivuduals and make the primary and secondary individual
            Cloner.CloneIndividuals(tAccount, sAccount, primaryPersonId, secondaryPersonId, By, lstRemovedIndividuals);
            // SZ[Jul 12, 2013] copy the Carrier Issues.
            Cloner.CloneCarrierIssues(tAccount, sAccount, By);
            // SZ[Jul 12, 2013] copy the Medical Supplements
            Cloner.CloneMedsUp(tAccount, sAccount, By);
            // SZ[Jul 12, 2013] copy the Application  tracking
            Cloner.CloneMedsupApp(tAccount, sAccount, By);
            // SZ[Jul 12, 2013] copy the Map & DP for this lead and Account
            Cloner.CloneMapDP(tAccount, sAccount, targetLeadId, By);
            // SZ[Jul 12, 2013] clone dental and vission
            Cloner.CloneDentalVision(tAccount, sAccount, By);
        }

        // SZ [Sept 6, 2013] added to quickly retrieve the campaign title
        public string GetCampaignTitle(long leadId)
        {
            string SQL = "Select C.[cmp_title] as Title from [Leads]  L inner join [Campaigns] C on L.[lea_cmp_id]=C.[cmp_id] where L.[lea_key]=@id";
            var x = E.leadEntities.ExecuteStoreQuery<string>(SQL, new object[] { new SqlParameter("@id", leadId) }).FirstOrDefault();
            return x != null ? x : "";
        }

        //SZ [Dec 16, 2013] Added to retrive the primary lead by accoutn id
        public Models.Lead GetPrimaryLeadByAccountId(long accId)
        {
            long leadId = E.leadEntities.Accounts
                               .Where(x => x.Key == accId)
                               .Select(x => x.PrimaryLeadKey)
                               .FirstOrDefault() ?? 0;


            return E.leadEntities.Leads.Where(x => x.Key == leadId)
                               .FirstOrDefault();
        }

        //SR [Mar 14, 2014] Added to Change the Status and SubStatus of Lead record
        public void ChangeStatusAndSubStatus(long[] leadIds, int statusId, int subStatusId, bool isSubStatusRequired)
        {
            foreach (long leadId in leadIds)
            {
                Models.Lead lead = Get(leadId);
                lead.StatusId = statusId;
                if (isSubStatusRequired && subStatusId != 0)
                        lead.SubStatusId = subStatusId;
                Update(lead, "");
            }
        }
    };
}


