// This class will only use leadEntities to reduce the time 
// if you need cross context please use storeCommand like <see cref="GetCampaignIdByMapId"/>

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.IO;
using System.Linq;
using SalesTool.DataAccess.Models;
using System.Data.Entity;
namespace SalesTool.DataAccess.Arc
{
    public class ArcActions : BaseActions
    {
        public ArcActions(DBEngine rengine)
            : base(rengine)
        { }

        public IQueryable<ArcCases> Get()
        {
            return E.leadEntities.ArcCases;
        }
        public IQueryable<ArcHistory> GetHistory()
        {
            return E.leadEntities.arc_history.Include(p => p.UserL);
        }
        /// <summary>
        /// Get ArcCase by Reference number from Arc System.
        /// </summary>
        /// <param name="reference">Arc Lead Reference</param>
        /// <param name="includeGraph"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public ArcCases Get(string reference, bool includeGraph)
        {

            if (includeGraph)
                return E.leadEntities.ArcCases
                     .Include(p => p.PrimaryLead)
                     .Include(p => p.Account)
                     .Include(p => p.Account.Individuals)
                     .Include(p => p.Account.PrimaryLead)
                     .Include(p => p.Account.PrimaryIndividual)
                     .Include(p => p.ArcIndividual)
                     .Include(p => p.ArcHistory)
                    .FirstOrDefault(p => p.ArcRefreanceKey == reference);
            return E.leadEntities.ArcCases.FirstOrDefault(p => p.ArcRefreanceKey == reference);
        }
        /// <summary>
        /// Get Account by External Reference 
        /// </summary>
        /// <param name="reference"></param>
        /// <param name="matchedIndividual"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public Account GetByExternalReference(string reference, out Individual matchedIndividual)
        {
            reference = reference.Trim();
            var individual = E.leadEntities.Individuals.Include(p => p.arc_cases).FirstOrDefault(p => p.ExternalReferenceID.Trim().Equals(reference));
            if (individual != null)
            {
                matchedIndividual = individual;
                return E.leadEntities.Accounts

                    .Include(p => p.PrimaryLead)
                    .Include(p => p.PrimaryIndividual)
                    .Include(p => p.SecondaryIndividual)
                    .Include(p => p.ArcCases)
                    .Include("ArcCases.ArcIndividual")
                    .FirstOrDefault(p => p.Key == individual.AccountId);
            }
            matchedIndividual = null;
            return null;
        }

        public Individual GetByExternalReference(string reference)
        {
            reference = reference.Trim();
            var individual = E.leadEntities.Individuals
                .Include(p => p.Account)
                .Include(p => p.Account.Leads)
                .Include(p => p.Account.PrimaryLead)
                .Include(p => p.arc_cases)
                .Include("arc_cases.ArcHistory")
                .FirstOrDefault(p => p.ExternalReferenceID.Trim().Equals(reference));
            return individual;
        }

        //YA[07 Dec 2013]
        public IQueryable<ArcCases> GetAll()
        {
            return E.leadEntities.ArcCases
                 .Include(p => p.PrimaryLead)
                 .Include(p => p.Account)
                 .Include(p => p.Account.Individuals)
                 .Include(p => p.Account.PrimaryLead)
                 .Include(p => p.Account.PrimaryIndividual)
                 .Include(p => p.ArcIndividual)
                 .Include(p => p.ArcHistory)
                 .Include("ArcHistory.UserL")
                .OrderBy(p => p.AddOn).AsQueryable();
        }
        public IQueryable<ArcCases> GetLatestArcCase(long accountid = 0)
        {
            return E.leadEntities.ArcCases.Where(x => x.AccountKey == accountid).OrderByDescending(p => p.AddOn);
        }
        //YA[07 Dec 2013]
        public IQueryable<object> GetAllArcCases(long accountid = 0, bool hasLeadNewLayout = false)
        {
            if (hasLeadNewLayout)
            {
                var account = E.leadEntities.Accounts.FirstOrDefault(f => f.Key == accountid);
                if (account != null && account.AccountParent != null)
                {
                    // get parent id
                    accountid = (long)account.AccountParent;
                }
                return E.leadEntities.ArcCases
                    .Where(x => x.AccountKey == accountid || x.Account.AccountParent == accountid)
                    .Join(E.IndividualsActions.GetAll(),
                    e => e.ArcIndividualKey,
                    ae => ae.Key,
                    (e, ae) => new { Cases = e, Indv = ae }).Select(x => new { x.Cases.AddOn, x.Cases.Notes, x.Cases.Status, x.Cases.ArcRefreanceKey, x.Cases.AccountKey, FullName = x.Indv.FirstName + " " + x.Indv.LastName, IndividualId=x.Indv.Key });

            }
            var query = E.leadEntities.ArcCases.Where(x => x.AccountKey == accountid).Join(E.IndividualsActions.GetAll(),
               e => e.ArcIndividualKey,
               ae => ae.Key,
               (e, ae) => new { Cases = e, Indv = ae }).Select(x => new { x.Cases.AddOn, x.Cases.Notes, x.Cases.Status, x.Cases.ArcRefreanceKey, x.Cases.AccountKey, FullName = x.Indv.FirstName + " " + x.Indv.LastName, IndividualId = x.Indv.Key });
            return query;
        }

        //YA[07 Dec 2013]
        public IQueryable<Object> GetAllArcHistory(long accountid = 0, bool hasNewLeadLayout = false)
        {
            if (hasNewLeadLayout)
            {
                var account = E.leadEntities.Accounts.FirstOrDefault(f => f.Key == accountid);
                if (account != null && account.AccountParent != null)
                {
                    // get parent id
                    accountid = (long)account.AccountParent;
                }
                return E.leadEntities.arc_history.Join(
                    E.leadEntities.ArcCases.Where(x => x.AccountKey == accountid || x.Account.AccountParent == accountid),
                    e => e.ArcKey,
                    ae => ae.Key,
                    (e, ae) => new { ArcHistory = e, ArcCase = ae })
                        .Select(x => new { x.ArcHistory, x.ArcCase })
                        .Join(E.IndividualsActions.GetAll(),
                              e => e.ArcCase.ArcIndividualKey,
                              ae => ae.Key,
                              (e, ae) => new { Cases = e, Indv = ae })
                        .Select(
                            x =>
                            new
                                {
                                    x.Cases.ArcHistory.AddedOn,
                                    x.Cases.ArcHistory.Notes,
                                    x.Cases.ArcHistory.Status,
                                    x.Cases.ArcCase.ArcRefreanceKey,
                                    FullName = x.Indv.FirstName + " " + x.Indv.LastName
                                });

            }
            return E.leadEntities.arc_history.Join(
                E.leadEntities.ArcCases.Where(x => x.AccountKey == accountid),
                e => e.ArcKey,
                ae => ae.Key,
                (e, ae) => new { ArcHistory = e, ArcCase = ae })
                    .Select(x => new { x.ArcHistory, x.ArcCase })
                    .Join(E.IndividualsActions.GetAll(),
                    e => e.ArcCase.ArcIndividualKey,
                    ae => ae.Key,
                    (e, ae) => new { Cases = e, Indv = ae })
                    .Select(
                        x =>
                            new
                                {
                                    x.Cases.ArcHistory.AddedOn,
                                    x.Cases.ArcHistory.Notes,
                                    x.Cases.ArcHistory.Status,
                                    x.Cases.ArcCase.ArcRefreanceKey,
                                    FullName = x.Indv.FirstName + " " + x.Indv.LastName
                                });
        }

        /// <summary>
        /// Get Lead if properties satisfied
        /// </summary>
        /// <param name="firstName">First Name auto toLower</param>
        /// <param name="lastName">LastName  auto toLower</param>
        /// <param name="dob"></param>
        /// <param name="dayPhone"></param>
        /// <param name="nightPh"></param>
        /// <param name="cell"></param>
        /// <param name="email"></param>
        /// <param name="address"></param>
        /// <param name="gunder"></param>
        /// <param name="individual"></param>
        /// <returns></returns>
        public Account GetLeadBy(string firstName, string lastName, DateTime? dob, long? dayPhone, long? nightPh, long? cell, string email, string address, string gender, out Individual individual)
        {
            #region

            //you check to see if the reference number exists in the arc cases table first
            //[7:44:19 PM] John Dobrotka: if it doesn't, then you see if there is a companion ref number
            //[7:44:29 PM] John Dobrotka: if there is, you check that against the arc cases table to find a match
            //[7:44:43 PM] John Dobrotka: if no match is found there, then you do the search based on the fields
            //[7:46:05 PM | Edited 7:47:03 PM] John Dobrotka: First Name, Last Name, Any Phone Number, Email, Street Address, Birth Date

            //use the fields I listed above
            //[7:46:37 PM] John Dobrotka: Only 3 matches need to be made
            //[7:47:39 PM] John Dobrotka: If you find a matching record based on a phone number match, birt date match and last name match, you will update that record and add the reference number to the arc cases table
            //[7:48:02 PM] John Dobrotka: so next time there is an update, it will now be found in the arc cases table which will be a faster lookup

            #endregion
            // Changed on 29 July 2014 reference G2m meeting contents.
            firstName = firstName.ToLower();
            lastName = lastName.ToLower();
            var query = E.leadEntities.Individuals.Select(p => p);
            query = query.Where(p => p.FirstName.ToLower() == firstName && p.LastName.ToLower() == lastName && p.Birthday == dob && p.Gender == gender);
            //// any phone number
            //query = query.Where(p => p.DayPhone == dayPhone || p.EveningPhone == nightPh || p.CellPhone == cell);
            //var indi = query.FirstOrDefault();

            //if (indi == null)
            //{
            //    if (!string.IsNullOrEmpty(firstName))
            //    {
            //        query = query.Where(p => p.FirstName == firstName);
            //    }
            //    if (!string.IsNullOrEmpty(lastName))
            //    {
            //        query = query.Where(p => p.LastName == lastName);
            //    }
            //    if (dayPhone != null)
            //    {
            //        query = query.Where(p => p.DayPhone.Value == dayPhone);
            //    }
            //    if (nightPh != null)
            //    {
            //        query = query.Where(p => p.EveningPhone == nightPh);
            //    }
            //    if (cell != null)
            //    {
            //        query = query.Where(p => p.CellPhone == cell);
            //    }
            //    if (!string.IsNullOrEmpty(email))
            //    {
            //        query = query.Where(p => p.Email == email);
            //    }
            //    if (!string.IsNullOrEmpty(address))
            //    {
            //        query = query.Where(p => p.Address1 == address || p.Address2 == address);
            //    }
            //    if (dob != null)
            //    {
            //        query = query.Where(p => p.Birthday == dob);
            //    }
            //    indi = query.FirstOrDefault();
            //}
            ////reset query
            //if (indi == null)
            //{

            //    query = E.leadEntities.Individuals.Select(p => p);
            //    if (!string.IsNullOrEmpty(lastName))
            //    {
            //        query = query.Where(p => p.LastName == lastName);
            //    }
            //    // any phone number
            //    query = query.Where(p => p.DayPhone == dayPhone || p.EveningPhone == nightPh || p.CellPhone == cell);

            //    if (!string.IsNullOrEmpty(address))
            //    {
            //        query = query.Where(p => p.Address1 == address || p.Address2 == address);
            //    }
            //    if (dob != null)
            //    {
            //        query = query.Where(p => p.Birthday == dob);
            //    }

            //}

            var indi = query.FirstOrDefault();
            if (indi != null)
            {
                individual = indi;
                long? accountId = indi.AccountId;
                if (accountId != null)
                    return E.leadEntities.Accounts.Where(p => p.Key == accountId)
                        .Include(p => p.Individuals)
                 .Include(p => p.PrimaryLead)
                 .Include(p => p.PrimaryIndividual)
                 .Include(p => p.SecondaryIndividual).FirstOrDefault();
            }
            individual = indi;
            return null;
            //return E.leadEntities.Individuals.Where(p => p.FirstName.Trim().ToLower() == firstName &&
            //                                      p.LastName == lastName &&
            //                                      p.Birthday == dob)
            // .Select(p => p.Account)
            // .Include(p => p.Individuals)
            // .Include(p => p.PrimaryLead)
            // .Include(p => p.PrimaryIndividual)
            // .Include(p => p.SecondaryIndividual).FirstOrDefault();


        }

        public ArcCases Get(long Key)
        {
            return E.leadEntities.ArcCases.FirstOrDefault(p => p.Key == Key);
        }

        public ArcCases GetByLeadKey(long leadKey)
        {
            return E.leadEntities.ArcCases.FirstOrDefault(p => p.LeadKey == leadKey);
        }

        public bool IsArcCaseExist(string reference)
        {
            return E.leadEntities.ArcCases.Any(p => p.ArcRefreanceKey == reference);
        }


        public void Add(Account account)
        {
            E.leadEntities.Accounts.AddObject(account);
            E.Save();
        }
        public bool Save(Account account)
        {
            E.Save();
            return true;
        }
        public bool Save(ArcCases arcCases)
        {
            E.Save();
            return true;
        }
        public bool Save(UserL userL)
        {
            E.Save();
            return true;
        }
        public bool Save(Account account, long? primaryIndividualKey, long? secondaryIndividualKey, long? primaryLeadId)
        {
            long key = account.Key;
            var firstOrDefault = E.leadEntities.Accounts.FirstOrDefault(p => p.Key == key);
            if (firstOrDefault != null)
            {
                if (primaryIndividualKey != null)
                    firstOrDefault.PrimaryIndividualId = primaryIndividualKey;

                if (secondaryIndividualKey != null)
                    firstOrDefault.SecondaryIndividualId = secondaryIndividualKey;

                if (primaryLeadId != null)
                    firstOrDefault.PrimaryLeadKey = primaryLeadId;
                E.Save();
                return true;
            }
            return false;
        }


        //public static void AttachAsModified<T>(this ObjectSet<T> objectSet, T entity) where T : class
        //{
        //    objectSet.Attach(entity);
        //    objectSet.Context.ObjectStateManager.ChangeObjectState(entity, EntityState.Modified);
        //}
        private static void SetAllPropertiesModified(ObjectContext context,
                                             object entity)
        {
            var stateEntry = context.ObjectStateManager.GetObjectStateEntry(entity);
            foreach (var propertyName in from fm in stateEntry.CurrentValues.
                                                        DataRecordInfo.FieldMetadata
                                         select fm.FieldType.Name)
            {
                stateEntry.SetModifiedProperty(propertyName);
            }
        }

        public string GetArcAgentId(string agentInitials)
        {
            //Guid? a = E.leadEntities.ExecuteStoreQuery<Guid?>("SELECT usr_key FROM dbo.users WHERE usr_arc_id=@arcId", new System.Data.SqlClient.SqlParameter("arcId",SqlDbType.NVarChar){Value = agentInitials}).FirstOrDefault();
            var agent = E.leadEntities.UserLs.FirstOrDefault(p => p.ArcId == agentInitials);
            if (agent != null)
                return agent.Key.ToString();
            return null;
        }
        public UserL GetArcAgent(string agentInitials)
        {
            var agent = E.leadEntities.UserLs.FirstOrDefault(p => p.ArcId == agentInitials);
            return agent;
        }

        public int? GetCampaignIdByMapId(string campaign)
        {
            if (string.IsNullOrEmpty(campaign))
                return null;
            int? id = E.leadEntities.ExecuteStoreQuery<int?>(
               "SELECT c.cmp_id FROM dbo.campaigns c WHERE c.cmp_arc_map=@campaign",
               new System.Data.SqlClient.SqlParameter("campaign", SqlDbType.NVarChar) { Value = campaign })
            .FirstOrDefault();
            //Campaign campaignObj = E.adminEntities.Campaigns1.FirstOrDefault(p => p.ArcMap == campaign);
            //if (campaignObj != null)
            //    return campaignObj.ID;
            //return null;
            return id;
        }
        public bool ValidateCredential(Guid userId)
        {
            return E.leadEntities.UserLs.Any(p => p.IsArcApiUser == true && p.Key == userId);
        }
        /// <summary>
        /// Get Account by campairing campanionReference with individuals's external-Reference-id or by indiv_key
        /// </summary>
        /// <param name="companionRefernce"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public Account GetAccountbyCampanionKey(string companionRefernce, long? indv_key = null, long? companianAccountKey = null)
        {
            Individual individual = null;
            if (!string.IsNullOrEmpty(companionRefernce))
            {
                companionRefernce = companionRefernce.Trim();
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.ExternalReferenceID.Trim().Equals(companionRefernce));
            }
            else if (companianAccountKey.HasValue && indv_key.HasValue)
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.AccountId == companianAccountKey.Value && p.Key == indv_key);
            else if (companianAccountKey.HasValue && !indv_key.HasValue)
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.AccountId == companianAccountKey.Value);
            else if (indv_key != null)
            {
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.Key == indv_key);
            }
            //else
            //{
            //    throw new ArgumentException("companion reference is null and Indv_key is null");
            //}
            if (individual != null)
            {
                return E.leadEntities.Accounts

                    .Include(p => p.PrimaryLead)
                    .Include(p => p.PrimaryIndividual)
                    .Include(p => p.SecondaryIndividual)
                    .Include(p => p.ArcCases)
                    .Include("ArcCases.ArcIndividual")
                    .FirstOrDefault(p => p.Key == individual.AccountId);
            }
            return null;
        }

        /// <summary>
        /// Get Account and individual by indiv_key
        /// </summary>
        /// <param name="indv_key">individual key to search</param>
        /// <param name="matchedIndividual">Matched individual</param>
        /// <returns>Account</returns>
        /// <author>MH:22 April 2014
        /// Modified: YA[25 Sept, 2014]
        /// </author>
        public Account GetAccountbyIndiv_key(long? indv_key, long? AccountKey, out Individual matchedIndividual)
        {
            Individual individual = null;

            if (AccountKey.HasValue && indv_key.HasValue)
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.AccountId == AccountKey.Value && p.Key == indv_key.Value);
            else if (AccountKey.HasValue && !indv_key.HasValue)
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.AccountId == AccountKey.Value);
            else if (indv_key.HasValue)
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.Key == indv_key.Value);

            if (individual != null)
            {
                matchedIndividual = individual;
                return E.leadEntities.Accounts

                    .Include(p => p.PrimaryLead)
                    .Include(p => p.PrimaryIndividual)
                    .Include(p => p.SecondaryIndividual)
                    .Include(p => p.ArcCases)
                    .Include("ArcCases.ArcIndividual")
                    .FirstOrDefault(p => p.Key == individual.AccountId);
            }
            matchedIndividual = null;
            return null;
        }
        /// <summary>
        /// Get IndividualStatus Id by StatusCode.
        /// </summary>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        /// <author>MH:</author>
        public int? GetIndividualStatusId(string statusCode)
        {
            var individualStatus = E.leadEntities.IndividualStatus.FirstOrDefault(p => p.Title.Trim().Equals(statusCode.Trim(), StringComparison.OrdinalIgnoreCase));
            if (individualStatus != null)
            {
                return individualStatus.ID;
            }
            return null;
        }

        public string GetIndividualStatusTitle(int statusId)
        {
            var individualStatus = E.leadEntities.IndividualStatus.FirstOrDefault(p => p.ID == statusId);
            if (individualStatus != null)
            {
                return individualStatus.Title;
            }
            return null;
        }
        #region Arc Client
        /// <summary>
        /// Get non delivered AccountHistory to the arc system with Entry Type Log and Comments "User assigned" 
        /// </summary>
        /// <returns></returns>
        /// <author>MH:</author>
        public IQueryable<vw_ArcChangeAgent> GetChangedAgentArcCases()
        {
            int type = (int)ActionHistoryType.Log;
            // logic moved to vw_ArcChangeAgent
            //var res = E.leadEntities.AccountHistories.Where(
            //    p => p.EntryType == type
            //        && p.IsDeliveredToArc == false
            //        && p.Comment.Equals("User assigned")
            //        && p.Account.ArcCases.Any())
            //    .Include(p => p.Account)
            //    .Include(p => p.Account.User)
            //    .Include(p => p.Account.ArcCases);
            var res = E.leadEntities.vw_ArcChangeAgent.Where(p => p.Account.ArcCases.Any())
                .Include(p => p.Account)
                .Include(p => p.Account.User)
                .Include(p => p.Account.ArcCases);
            return res;
        }
        /// <summary>
        /// Get Actions from vm_ArcActions view
        /// Get Data from Account history with entry type 1 with join to action
        /// </summary>
        /// <returns></returns>
        /// <author>MH:</author>
        public IQueryable<vw_ArcActions> GetArcActions()
        {
            return E.leadEntities.vw_ArcActions.Where(p => p.IsDelivered == false && p.ArcAccountId != null);
        }

        /// <summary>
        /// Get non-delivered Actions by list of accountsIds
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public IQueryable<vw_ArcActions> GetArcActions(List<long> ids)
        {
            return E.leadEntities.vw_ArcActions.Where(p => ids.Contains(p.ActionId) && p.IsDelivered == false && p.ArcAccountId != null);
        }

        /// <summary>
        /// Get Arc Cases whom individuals are marked as IndividualEmailOptOutQueuedChange
        /// Graph included Account,ArcIndividual
        /// </summary>
        /// <returns></returns>
        /// <author>MH</author>
        public IQueryable<ArcCases> GetArcCasesForStopLetters()
        {
            var res = E.leadEntities.ArcCases.Where(p => p.ArcIndividual.IndividualEmailOptOutQueuedChange)
             .Include(p => p.Account)
             .Include(p => p.ArcIndividual);
            return res;
        }
        /// <summary>
        /// Get Letter logs data from vw_ArcLetterLog view which is marked as non delived 
        /// <remarks>
        /// Get Data from Email Que and email template which is marked as not delivered 
        /// </remarks>
        /// </summary>
        /// <returns></returns>
        /// <author>MH</author>
        public IQueryable<vw_ArcLetterLog> GetArcLetterLogs()
        {
            return E.leadEntities.vw_ArcLetterLog.Where(p => p.IsDelivered == false);
        }
        #endregion

        /// <summary>
        /// Get non delivered opportunities data
        /// From AccountHistroy with EntryType 4
        /// </summary>
        /// <author>MH</author>
        public IQueryable<AccountHistory> GetNonDeliveredArcOpportunities()
        {
            int type = (int)ActionHistoryType.Calls;
            return E.leadEntities.AccountHistories
                .Include(p => p.UserL)
                .Where(p => p.IsDeliveredToArc == false && p.EntryType == type);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        /// <author>MH</author>
        public IQueryable<ArcCases> GetArcConsentUpdate()
        {
            return E.leadEntities.ArcCases
                 .Include(p => p.PrimaryLead)
                 .Include(p => p.Account)
                 .Include(p => p.Account.Individuals)
                 .Include(p => p.Account.PrimaryLead)
                 .Include(p => p.Account.PrimaryIndividual)
                 .Include(p => p.ArcIndividual)
                 .Include(p => p.ArcHistory)
                 .Include("ArcHistory.UserL")
                 .Where(p => p.Account.PrimaryIndividual.IndividualTCPAChanged && p.ArcIndividual.Key == p.Account.PrimaryIndividualId)

                .OrderBy(p => p.AddOn).AsQueryable();
        }

        //MH:
        /// <summary>
        /// Get All Cases which has individual's data changed
        /// </summary>
        /// <returns></returns>
        /// <author>MH</author>
        public IQueryable<ArcCases> GetArcCasesForContactChanged()
        {
            return E.leadEntities.ArcCases
                //.Include(p => p.PrimaryLead) not used
                .Include(p => p.Account)
                //.Include(p => p.Account.Individuals)
                //.Include(p => p.Account.PrimaryLead)
                //.Include(p => p.Account.PrimaryIndividual)
                .Include(p => p.ArcIndividual)
                //.Include(p => p.ArcHistory)
                .Include("ArcHistory.UserL")
                .Where(p => p.ArcIndividual.IsChanged == true)// MH:because account can have multiple cases and each case contain individual so primary individual is not candidate for this situation
               .OrderBy(p => p.AddOn).AsQueryable();
        }
        /// <summary>
        /// Gets createOp data need to send to arc 
        /// Maps to Store Procedure "proj_Arc_GetCreateOp"
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public List<Arc_GetCreateOp_Result> GetCreateOpData(DateTime startDate, DateTime endDate)
        {
            List<Arc_GetCreateOp_Result> opResults = E.leadEntities.proj_Arc_GetCreateOp(startDate, endDate).ToList();
            return opResults;
        }

        //        /// <summary>
        //        /// Arc Related setting in a single round trip
        //        /// </summary>
        //        /// <returns></returns>
        //        /// <autor>MH</autor>
        //        public ArcDefaultSetting LoadSettings()
        //        {
        //            string sql = @"SELECT 
        //(SELECT iValue  FROM dbo.Application_Storage WHERE [Key]='default_post_campaign') AS 'Default_Post_Campaign',
        //(SELECT c.iValue  FROM dbo.Application_Storage c WHERE c.[Key]='default_post_status') AS 'Default_Post_Status',
        //(SELECT ISNULL(c.bValue,0)  FROM dbo.Application_Storage c WHERE c.[Key]='SQL_LEAD_NEW_LAYOUT') AS 'HasLeadNewLayout'";
        //            var res = E.leadEntities.ExecuteStoreQuery<ArcDefaultSetting>(sql).FirstOrDefault();
        //            if (res != null)
        //                res.ValidateSettings();
        //            return res;
        //        }

        /// <summary>
        /// Get main status by its title.
        /// </summary>
        /// <param name="title"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public StatusModel GetLeadStatusByTitle(string title)
        {
            var status = E.leadEntities.ExecuteStoreQuery<StatusModel>("SELECT * FROM dbo.statuses WHERE sta_level=0 and REPLACE(sta_title,' ','')=@title",
                                                     new System.Data.SqlClient.SqlParameter("title", SqlDbType.NVarChar)
                                                         {
                                                             Value = title
                                                         }).FirstOrDefault();
            return status;
        }

        public List<StatusModel> GetLeadStatuses()
        {

            return E.leadEntities.ExecuteStoreQuery<StatusModel>("SELECT * FROM dbo.statuses WHERE sta_level=0 ",
                           null).ToList();
        }

        /// <summary>
        /// Get State by its abbreviation.
        /// </summary>
        /// <param name="appState"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public StateModel GetStateByAbbreviation(string appState)
        {
            var state = E.leadEntities.ExecuteStoreQuery<StateModel>("SELECT * FROM dbo.states WHERE sta_abbreviation=@stateCode",
                                                    new System.Data.SqlClient.SqlParameter("stateCode",
                                                                                           SqlDbType.NVarChar)
                                                        {
                                                            Value = appState
                                                        }).FirstOrDefault();
            return state;
        }

        public List<StateModel> GetState()
        {
            return E.leadEntities.ExecuteStoreQuery<StateModel>("SELECT * FROM dbo.states ", null).ToList();
        }

        /// <summary>
        /// Get All States by Executeing StoreQuery
        /// </summary>
        /// <returns></returns>
        /// <author>MH</author>
        public List<StateModel> GetAllStates()
        {
            List<StateModel> states = E.leadEntities.ExecuteStoreQuery<StateModel>("SELECT * FROM dbo.states").ToList();
            return states;
        }
        /// <summary>
        /// Gets All non-deleted campaigns by executing storeQuery
        /// </summary>
        /// <returns></returns>
        /// <author>MH</author>
        public List<CampaignModel> GetAllCampaign()
        {
            List<CampaignModel> campaigns = E.leadEntities.ExecuteStoreQuery<CampaignModel>("SELECT * FROM dbo.campaigns WHERE cmp_delete_flag !=@state", new System
                .Data.SqlClient.SqlParameter("state", SqlDbType.Bit) { Value = false }).ToList();
            return campaigns;
        }
        /// <summary>
        /// Get Individual by external Reference or by indivial_key
        /// Reference will get top periority
        /// </summary>
        /// <param name="companionReference"></param>
        /// <param name="companionIndvKey"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public Individual GetIndividualByExternalRefOrByKey(string companionReference, long? companionIndvKey, out bool hasParentAssigned)
        {
            Individual individual = null;
            hasParentAssigned = false;
            if (!string.IsNullOrEmpty(companionReference))
            {
                companionReference = companionReference.Trim();
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.ExternalReferenceID.Trim().Equals(companionReference));
            }
            else if (companionIndvKey != null)
            {
                individual = E.leadEntities.Individuals.FirstOrDefault(p => p.Key == companionIndvKey);
            }
            // MH: 17 June 2014
            var accountId = individual.IfNotNull(p => p.AccountId);
            if (accountId.HasValue)
                hasParentAssigned = GetAccountsParentId(accountId.Value).HasValue;
            return individual;
        }
        /// <summary>
        /// Determines weather account was assigned before
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        /// <author>MH: 14 May 2014</author>
        public bool IsUserAssignedBefore(long accountId)
        {
            return Engine.leadEntities.AccountHistories.Any(p => p.Comment == "User Assigned" && p.EntryType == 2 && p.AccountId == accountId);
        }

        /// <summary>
        /// Gets Account's parentId by AccountId
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns>Parent AccountId</returns>
        /// <author>MH: 17 June 2014</author>
        public long? GetAccountsParentId(long accountId)
        {
            return E.leadEntities.Accounts.FirstOrDefault(p => p.Key == accountId).IfNotNull(l => l.AccountParent);
        }
    }

    public class ArcDefaultSetting
    {
        public int? Default_Post_Campaign { get; set; }

        public int? Default_Post_Status { get; set; }

        public bool HasLeadNewLayout { get; set; }

        public void ValidateSettings()
        {
            if (Default_Post_Campaign == null)
            {
                throw new InvalidDataException(string.Format("Invalid ApplicationSettings Data for {0}",
                                                             "Default_Post_Campaign"));
            }
            if (Default_Post_Status == null)
            {
                throw new InvalidDataException(string.Format("Invalid ApplicationSettings Data for {0}",
                                                             "Default_Post_Status"));
            }
        }

    }

    [System.Diagnostics.DebuggerDisplay("Id=[{Id}] FullName=[{FullName}] Abbreviation=[{Abbreviation}]")]
    public class StateModel
    {

        private byte sta_key { get; set; }

        private string sta_full_name { get; set; }

        private string sta_abbreviation { get; set; }

        public Byte Id { get { return sta_key; } set { sta_key = value; } }

        public string FullName { get { return sta_full_name; } set { sta_full_name = value; } }

        public string Abbreviation { get { return sta_abbreviation; } set { sta_abbreviation = value; } }

        public static implicit operator State(StateModel model)
        {
            return new State
                {
                    Abbreviation = model.Abbreviation,
                    FullName = model.FullName,
                    Id = model.Id,
                };

        }
    }
    [System.Diagnostics.DebuggerDisplay("Id=[{Id}] Title=[{Title}]")]
    public class StatusModel
    {
        private int sta_key { get; set; }
        public int Id { get { return sta_key; } }

        private string sta_title { get; set; }
        public string Title { get { return sta_title; } }

        private string sta_add_user { get; set; }
        public string CreatedBy { get { return sta_add_user; } }

        private DateTime sta_add_date { get; set; }
        public DateTime AddedOn { get { return sta_add_date; } }

        private string sta_change_user { get; set; }
        public string ChangeBy { get { return sta_change_user; } }

        private DateTime? sta_change_date { get; set; }
        public DateTime? ChangedAt { get { return sta_change_date; } }

        private int? sta_priority { get; set; }
        public int? Priority { get { return sta_priority; } }

        private byte? sta_level { get; set; }
        public byte? Level { get { return sta_level; } }

        private bool sta_progress_flag { get; set; }
        public bool Progress { get { return sta_progress_flag; } }

        private int? sta_filter_selection { get; set; }
        public int? Filterselection { get { return sta_filter_selection; } }

        private string sta_filter_customValue { get; set; }
        public string FilterCustomValue { get { return sta_filter_customValue; } }


    }

    public class CampaignModel
    {
        private int cmp_id { get; set; }
        public int ID { get { return cmp_id; } }

        private string cmp_title { get; set; }
        public string Title { get { return cmp_title; } }

        private string cmp_alt_title { get; set; }
        public string AlternateTitle { get { return cmp_alt_title; } }

        private int? cmp_cpt_key { get; set; }
        public int? CampaignTypeKey { get { return cmp_cpt_key; } }

        private decimal? cmp_cpl { get; set; }
        public decimal? CmpaignCPL { get { return cmp_cpl; } }

        private string cmp_email { get; set; }
        public string email { get { return cmp_email; } }

        private string cmp_notes { get; set; }
        public string Notes { get { return cmp_notes; } }

        private bool? cmp_active_flag { get; set; }
        public bool? IsActive { get { return cmp_active_flag; } }

        private bool? cmp_delete_flag { get; set; }
        public bool? IsDeleted { get { return cmp_delete_flag; } }

        private Guid? cmp_add_user { get; set; }
        public Guid? AddedBy { get { return cmp_add_user; } }

        private DateTime? cmp_add_date { get; set; }
        public DateTime? AddedOn { get { return cmp_add_date; } }

        private Guid? cmp_change_user { get; set; }
        public Guid? ChangedBy { get { return cmp_change_user; } }

        private DateTime? cmp_change_date { get; set; }
        public DateTime? ChangedOn { get { return cmp_change_date; } }

        private int? cmp_cpy_key { get; set; }
        public int? CompanyID { get { return cmp_cpy_key; } }

        private int? cmp_sp_outpulse_type { get; set; }
        public int? OutpulseType { get { return cmp_sp_outpulse_type; } }

        private string cmp_sp_outpulse_id { get; set; }
        public string OutpulseId { get { return cmp_sp_outpulse_id; } }

        private string cmp_description { get; set; }
        public string Description { get { return cmp_description; } }

        private string cmp_arc_map { get; set; }
        public string ArcMap { get { return cmp_arc_map; } }

        private bool? cmp_consumer_type { get; set; }
        public bool? HasDTE { get { return cmp_consumer_type; } }
    }
}
namespace SalesTool.DataAccess.Models
{
    public partial class ArcHistory
    {
        public string UserInitial
        {
            get
            {
                if (this.UserL != null)
                {
                    return UserL.ArcId;
                }
                return "";

            }
        }
    }
}