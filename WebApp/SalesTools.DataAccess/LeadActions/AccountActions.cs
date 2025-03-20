using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Data.SqlClient;
using MODELS = SalesTool.DataAccess.Models;
using System.Data.Objects;


namespace SalesTool.DataAccess
{

    public enum IndividualType
    {
        Primary = 2,
        Secondary = 3
    }


    public class AccountActions : BaseActions
    {
        internal AccountActions(DBEngine engine)
            : base(engine)
        { }


        // public void QuickSave(ref long aid, ref long leadid, string fname, string lname, string gender, byte? nullConvert, int campaignId, int statusId,  DateTime? dob)
        //{
        //    MODELS.Account A = Engine.AccountActions.Add(new MODELS.Account());
        //    aid = A.Key;

        //    long personid = Engine.IndividualsActions.Add(new MODELS.Individual { FirstName = fname, LastName = lname, Birthday = dob, Gender = gender, AccountId = aid }).Key;
        //    A.PrimaryIndividualId = personid;

        //    leadid = Engine.LeadsActions.Add(new MODELS.Lead { AccountId = aid, AccountKey = aid, CampaignId=campaignId, StatusId=statusId }).Key;
        //    A.PrimaryLeadKey = leadid;

        //    Update(A);
        //}

        public void QuickSave(ref long aid, ref long leadid, string fname, string lname, string gender, Guid user, byte? appState, int campaignId, int statusId, DateTime? dob
            , string mName, long? dayPhone, long? evenPhone, long? cellPhone, string email, bool emailOptOut, string address1, string address2, string city, byte? state, string zipCode, DateTime? apDate,long? parentActId)
        {
            MODELS.User usr = E.UserActions.Get(user);
            string code = (usr != null) ? usr.ArcId : "";
            string userName = (usr ?? new Models.User()).FullName;
            MODELS.Account A = Engine.AccountActions.Add(new MODELS.Account { AddedBy = userName, AddedOn = DateTime.Now, AssignedUserKey = user,OriginalUserKey = user,IsDeleted = false, IsActive = true ,AccountParent = parentActId}, userName);
            aid = A.Key;

            long personid = Engine.IndividualsActions.Add(new MODELS.Individual
                {
                    AddedBy = userName,
                    AddedOn = DateTime.Now,
                    FirstName = fname,
                    LastName = lname,
                    Birthday = dob,
                    Gender = gender,
                    AccountId = aid,
                    ApplicationState = appState,
                    StateID = state,
                    MiddleName = mName,
                    DayPhone = dayPhone,
                    EveningPhone = evenPhone,
                    CellPhone = cellPhone,
                    Email = email,
                    IndividualEmailOptOut = emailOptOut,
                    Address1 = address1,
                    Address2 = address2,
                    City = city,
                    Zipcode = zipCode,
                    indv_ap_date = apDate
                }, userName).Key;
            A.PrimaryIndividualId = personid;

            leadid = Engine.LeadsActions.Add(new MODELS.Lead { AddedBy = userName, AddedOn = DateTime.Now, AccountId = aid, AccountKey = aid, CampaignId = campaignId, StatusId = statusId, SourceCode = code }).Key;
            A.PrimaryLeadKey = leadid;

            Update(A, userName);
        }

        public event EventHandler<ItemEventArgs<long>> ConsentUpdated;

        protected virtual void OnConsentUpdated(long id)
        {
            var handler = ConsentUpdated;
            if (handler != null) handler(this, new ItemEventArgs<long>(id));
        }

        /* SZ [Mar 21, 2013] This fucntion returns the next account in the priority list that is made by the service. 
          */
        public SalesTool.DataAccess.Models.Account GetNextPriorityAccount(long accountID, Guid userkey)
        {
            var P = E.ListPrioritizationAccount.All.AsEnumerable();
            var Accounts = E.AccountActions.GetAll().Where(x => x.AssignedUserKey == userkey || (x.TransferUserKey == userkey && x.AssignedUserKey == null)).Join(
                    P,
                    a => a.Key,
                    b => b.AccountKey,
                    (a, b) => new { Account = a, Priority = b.Priority }).OrderBy(x => x.Priority).Select(x => x.Account);

            long accID = -1;
            List<long> ids = Accounts.Select(x => x.Key).ToList();


            if (ids.Count > 0)
            {
                int index = ids.IndexOf(accountID);
                if (index == -1) // account absent 
                    accID = ids[0];
                else
                {
                    if (index == ids.Count - 1)
                        accID = -1;
                    else
                        accID = ids[index + 1];
                }
            }

            //Sz [apr 3, 2013] an issue, that will be checked later. BAD LOGIC above ^
            //if (!ids.Contains(accountID) && ids.Count > 0)
            //    accID = ids.First();
            //else
            //{
            //    int index = ids.IndexOf(accountID);
            //    accID = (index == -1 && ids.Count > 0) ? ids.First() : ids[index + 1] ;
            //}

            return E.AccountActions.Get(accID);
        }

        public SalesTool.DataAccess.Models.Lead GetPrimaryLead(long key)
        {
            //Models.Lead L = null;
            //Models.Account A= Get(key);
            //if (A != null)
            //{
            //    long leadId = A.PrimaryLeadKey?? default(long);
            //    L = E.LeadsActions.Get(leadId);
            //}
            return E.LeadsActions.Get(GetPrimaryLeadId(key) ?? 0);
        }

        // SZ [Apr 2 2013] Created to get the fastest access for the primary lead retrieval
        long? GetPrimaryLeadId(long accId)
        {
            const string K_QUERY = "Select [act_lead_primary_lead_key] from [Accounts] WHERE act_key=@id";
            System.Data.SqlClient.SqlParameter[] arr = new System.Data.SqlClient.SqlParameter[1];
            arr[0] = new System.Data.SqlClient.SqlParameter("@id", accId);
            System.Data.Objects.ObjectResult<long?> x = E.Lead.ExecuteStoreQuery<long?>(K_QUERY, arr);
            return x.FirstOrDefault();
        }

        public void SetPrimaryLead(long accID, long leadID, string by="")
        {
            //SZ [Apr 22, 2013] modified for fastest access withotu resortignto ADO.NET
            const string K_QUERY = "Update [Accounts] Set [act_lead_primary_lead_key] = @leadId,  [act_modified_user]=@by, [act_modified_date]=@dt  WHERE act_key=@id";
            System.Data.SqlClient.SqlParameter[] arr = new System.Data.SqlClient.SqlParameter[4];
            arr[0] = new System.Data.SqlClient.SqlParameter("@leadId", leadID);
            arr[1] = new System.Data.SqlClient.SqlParameter("@id", accID);
            arr[2] = new System.Data.SqlClient.SqlParameter("@by", by);
            arr[2] = new System.Data.SqlClient.SqlParameter("@dt", DateTime.Now);
            E.Lead.ExecuteStoreCommand(K_QUERY, arr);
        }
        //YA[April 12, 2013] According to new requirements Multiple accounts could be returned.
        public IEnumerable<Models.Account> FindByPhone(string phone)
        {
            long lphone = default(long);
            long.TryParse(phone, out lphone);

            IEnumerable<Models.Individual> I = Engine.Lead.Individuals.Where(x =>
                (x.DayPhone ?? default(long)) == lphone ||
                (x.EveningPhone ?? default(long)) == lphone ||
                (x.CellPhone ?? default(long)) == lphone && !(x.IsDeleted ?? false));

            return I == null ? null : Engine.AccountActions.GetAll().Join(I, a => a.PrimaryIndividualId, b => b.Key, (a, b) => new { Accounts = b.Account }).Select(x => x.Accounts);
        }

        public Models.Account Add(Models.Account nAccount, string by="")
        {
            nAccount.IsActive = true;
            nAccount.IsDeleted = false;

            nAccount.AddedBy = by;
            nAccount.AddedOn = DateTime.Now;
            
            E.Lead.Accounts.AddObject(nAccount);
            E.Save();
            return nAccount;
        }

        //public Models.Account AddAccount(Models.Account nAccount)
        //{
        //    //SZ [Jan 21, 2013] For creating a new account key
        //    nAccount.Key = (E.Lead.Accounts.Count() > 0) ? E.Lead.Accounts.Max(x => x.Key) + 1 : 1;

        //    nAccount.IsActive = true;
        //    nAccount.IsDeleted = false;
        //    nAccount.AddedOn = DateTime.Now;
        //    E.Lead.Accounts.AddObject(nAccount);
        //    E.Save();

        //    return Get(nAccount.Key);
        //}

        public void Update(Models.Account nAccount, string by="")
        {
            nAccount.ChangedBy = by;
            nAccount.ChangedOn = DateTime.Now;
            E.Save();
        }


        public void Delete(long inputKey, string by="", bool bPermanent = false)
        {
            //var U = (from T in E.Lead.Accounts.Where(x => x.Key ==inputKey) select T).FirstOrDefault();
            //U.IsDeleted = true;
            //E.Save();
            //Models.LeadEntities DeleteLead = new Models.LeadEntities();
            //var deleteObject = DeleteLead.Accounts.Where(x => x.Key == inputKey).FirstOrDefault();
            //deleteObject.IsDeleted = true;
            //DeleteLead.SaveChanges();

            Models.Account A = E.leadEntities.Accounts.Where(x => x.Key == inputKey).FirstOrDefault();
            if (!bPermanent)
            {
                A.ChangedBy = by;
                A.ChangedOn = DateTime.Now;
                A.IsDeleted = true;
            }
            else
                E.Lead.Accounts.DeleteObject(A);

            E.Save();
        }

        // SZ [Nov 11, 2013] added to perform a delete by providing the lead id
        public void DeleteByLeads(long[] leadIds, bool bPermanent = false)
        {
            long[] accids = E.Lead.Leads.Where(x => leadIds.Contains(x.Key)).Select(x => x.AccountId).ToArray<long>();
            foreach (long id in accids)
                Delete(id, "", bPermanent);
        }
        public void Delete(long[] accIds, string by="", bool bPermanent = false)
        {
            Models.Account[] A = E.Lead.Accounts.Where(x => accIds.Contains(x.Key)).ToArray<Models.Account>();
            if (bPermanent)
                foreach (var item in A)
                    E.Lead.Accounts.DeleteObject(item);
            else
                for (int i = 0; i < A.Length; i++)
                {
                    A[i].ChangedBy = by;
                    A[i].ChangedOn = DateTime.Now;
                    A[i].IsDeleted = true;
                }

            E.Save();
        }


        public void InActivate(long inputKey, string by="")
        {
            var U = (from T in E.Lead.Accounts.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.ChangedBy = by;
            U.ChangedOn = DateTime.Now;
            U.IsDeleted = false;
            E.Save();
        }

        public void Activate(long? inputKey, string by="")
        {
            var U = (from T in E.Lead.Accounts.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.ChangedBy = by;
            U.ChangedOn = DateTime.Now;
            U.IsActive = true;
            E.Save();
        }

        // Attiq - April 04-2014
        // Added bWebServiceCall so that the E.Lead.Refresh(RefreshMode.StoreWins, R); is not executed if the 
        // function is called from WebService.cs.
        // Will be initialzied with false in case of being null and will work fine with existing calls to this function.
        public Models.Account Get(long id, bool bNonTracked = false, bool bWebServiceCall = false)
        {
            Models.Account R = null;
            if (!bNonTracked) // SZ [Apr 5, 2013] this has been alterted to improve fectch time
                R = E.Lead.Accounts.Where(x => !(x.IsDeleted ?? false) && x.Key == id).FirstOrDefault();
            else
            {
                E.leadEntities.Accounts.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = E.Lead.Accounts.Where(x => !(x.IsDeleted ?? false) && x.Key == id).FirstOrDefault();
                E.leadEntities.Accounts.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            if (R != null && !(bWebServiceCall))
                E.Lead.Refresh(RefreshMode.StoreWins, R);
            return R;
        }

        public IQueryable<Models.Account> GetAll()
        {
            //E.Lead.Accounts.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return E.Lead.Accounts.Where(x => !(x.IsDeleted ?? false));
        }


        public IQueryable<Models.Account> GetAll(long? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return E.Lead.Accounts.Where(x => x.Key == inputIndvID && (x.IsActive ?? false) && (x.IsDeleted ?? false));
            }
            else
            {
                return E.Lead.Accounts.Where(x => (x.IsDeleted ?? false) && (x.IsDeleted ?? false));
            }

        }

        //SZ A
        public void SetIndividual(long accId, IndividualType type, long IndvId, string by = "")
        {
            string SQL_PRIMARY = "Update [Accounts] Set [act_primary_individual_id] = @indvId, [act_modified_user]=@usr, [act_modified_date]=@dt WHERE [act_key] = @accId";
            string SQL_SECONDARY = "Update [Accounts] Set [act_secondary_individual_id] = @indvId, [act_modified_user]=@usr, [act_modified_date]=@dt WHERE [act_key] = @accId";
            string Query = type == IndividualType.Primary ? SQL_PRIMARY : SQL_SECONDARY; 
         
            //  @"Update [Accounts] Set [act_primary_individual_id] = @indvId  WHERE [act_key] = @accId" :
            //  @"Update [Accounts] Set [act_secondary_individual_id] = @indvId WHERE [act_key] = @accId";

            System.Data.SqlClient.SqlParameter[] Params = new System.Data.SqlClient.SqlParameter[4];

            Params[0] = new System.Data.SqlClient.SqlParameter("@indvId", IndvId == 0 ? DBNull.Value : (object)IndvId);
            Params[1] = new System.Data.SqlClient.SqlParameter("@accId", accId);
            Params[2] = new System.Data.SqlClient.SqlParameter("@usr", by);
            Params[3] = new System.Data.SqlClient.SqlParameter("@dt", DateTime.Now);
            
            E.Lead.ExecuteStoreCommand(Query, Params);
        }

        
        //TM [May 23, 2014] The functions checks by AccountID if an Account has a secondary individual registered in Accounts table.
        /// <summary>
        /// The function checks if the account entry in Accounts table's SecondaryAccountID is not null.
        /// </summary>
        /// <param name="accID">Account ID</param>
        /// <returns>Boolean, True if the account entry has a SecondryAccountID in Accounts table, False otherwise </returns>
        public bool HasAccountSecondary(long accID)
        {
            //string Query = @"select act_secondary_individual_id  from Accounts where act_key = @accId";
            //System.Data.SqlClient.SqlParameter[] Params = new System.Data.SqlClient.SqlParameter[1];
            //Params[0] = new System.Data.SqlClient.SqlParameter("@accId", accID);
            //System.Data.Objects.ObjectResult<long?> Res = E.Lead.ExecuteStoreQuery<long?>(Query, Params);
            //return (Res.FirstOrDefault() != null);
            var res = Engine.leadEntities.Accounts.Where(a => a.Key == accID && a.SecondaryIndividualId != null);
            return (res != null && res.Any());
        }

        public bool IsIndividual(long accID, IndividualType type, long IndvId)
        {
            string Query = type == IndividualType.Primary ?
                @"select I.indv_key from Individuals I right outer join Accounts A on I.indv_key = A.act_primary_individual_id where A.act_key = @accId" :
                @"select I.indv_key from Individuals I right outer join Accounts A on I.indv_key = A.act_secondary_individual_id where A.act_key = @accId";


            System.Data.SqlClient.SqlParameter[] Params = new System.Data.SqlClient.SqlParameter[1];
            Params[0] = new System.Data.SqlClient.SqlParameter("@accId", accID);
            System.Data.Objects.ObjectResult<long?> Res = E.Lead.ExecuteStoreQuery<long?>(Query, Params);
            long? iid = Res.FirstOrDefault();

            return iid.HasValue ? iid.Value == IndvId : false;

            //Models.Account A = Get(accID);
            //Models.Individual I = null;
            //long id = 0;

            //switch (type)
            //{
            //    case IndividualType.Primary:
            //        id = (A != null) ? (A.PrimaryIndividualId ?? default(long)) : 0;
            //        I = E.Lead.Individuals.Where(x => x.Key == id && x.AccountId == accID).FirstOrDefault();
            //        break;
            //    case IndividualType.Secondary:
            //        id = (A != null) ? (A.SecondaryIndividualId ?? default(long)) : 0;
            //        I = E.Lead.Individuals.Where(x => x.Key == id && x.AccountId == accID).FirstOrDefault();
            //        break;
            //}
            //return I;
        }
        public Models.Individual GetIndividual(long accID, IndividualType type)
        {
            string Query = type == IndividualType.Primary ?
                @"select I.indv_key from Individuals I right outer join Accounts A on I.indv_key = A.act_primary_individual_id where A.act_key = @accId" :
                @"select I.indv_key from Individuals I right outer join Accounts A on I.indv_key = A.act_secondary_individual_id where A.act_key = @accId";


            System.Data.SqlClient.SqlParameter[] Params = new System.Data.SqlClient.SqlParameter[1];
            Params[0] = new System.Data.SqlClient.SqlParameter("@accId", accID);
            System.Data.Objects.ObjectResult<long?> Res = E.Lead.ExecuteStoreQuery<long?>(Query, Params);
            long? iid = Res.FirstOrDefault();
            return iid.HasValue ? E.IndividualsActions.Get(iid ?? 0) : null;

            //Models.Account A = Get(accID);
            //Models.Individual I = null;
            //long id = 0;

            //switch (type)
            //{
            //    case IndividualType.Primary:
            //        id = (A != null) ? (A.PrimaryIndividualId ?? default(long)) : 0;
            //        I = E.Lead.Individuals.Where(x => x.Key == id && x.AccountId == accID).FirstOrDefault();
            //        break;
            //    case IndividualType.Secondary:
            //        id = (A != null) ? (A.SecondaryIndividualId ?? default(long)) : 0;
            //        I = E.Lead.Individuals.Where(x => x.Key == id && x.AccountId == accID).FirstOrDefault();
            //        break;
            //}
            //return I;
        }
        //YA[April 17, 2013] Add this overloaded function for calling at the places where Account object is already there.
        public Models.Individual GetIndividual(Models.Account A, IndividualType type)
        {
            Models.Individual I = null;
            long id = 0;

            switch (type)
            {
                case IndividualType.Primary:
                    id = (A != null) ? (A.PrimaryIndividualId ?? default(long)) : 0;
                    I = E.Lead.Individuals.Where(x => x.Key == id && x.AccountId == A.Key).FirstOrDefault();
                    break;
                case IndividualType.Secondary:
                    id = (A != null) ? (A.SecondaryIndividualId ?? default(long)) : 0;
                    I = E.Lead.Individuals.Where(x => x.Key == id && x.AccountId == A.Key).FirstOrDefault();
                    break;
            }
            return I;
        }

        public IQueryable<Models.Lead> GetLeads(long accid)
        {
            return E.Lead.Leads.Where(x => x.AccountId == accid && x.IsDuplicate != true);
        }
        public IQueryable<Models.Lead> GetLeadByPrimary(long primaryLeadKey)
        {
            return E.Lead.Leads.Where(x => x.Key == primaryLeadKey && x.IsDuplicate != true);
        }

        public Models.Lead GetLeadAt(long accId, int iIndex = 0)
        {
            Models.Lead L = null;
            IEnumerable<Models.Lead> Leads = Engine.AccountActions.GetLeads(accId).ToList();
            if (Leads.Count() > 0 && iIndex < Leads.Count())
                L = Leads.ElementAt(iIndex);
            return L;
        }

        public bool Exists(long accountId)
        {
            const string K_SQL = "SELECT COUNT(*) FROM [accounts] A WHERE A.[act_key] = @id";
            System.Data.Objects.ObjectResult<int> A = E.Lead.ExecuteStoreQuery<int>(K_SQL, new System.Data.SqlClient.SqlParameter("@id", accountId));
            return A.FirstOrDefault() > 0;
            //return E.Lead.Accounts.Where(x => x.Key == accountId && !(x.IsDeleted??false)).Count() > 0;
        }

        #region ViewLeads
        public IQueryable<Models.ViewLead> AllViewLeads
        {
            get
            {
                //                const string K_QUERY = @"SELECT 
                //                       [leadid] LeadId
                //                      ,[accountId] AccountId
                //                      ,[act_add_date] AddedOn
                //                      ,[dateOfBirth] BirthDate
                //                      ,[dateCreated] CreatedOn
                //                      ,[assigneduserkey] 
                //                      ,[transferuserkey] 
                //                      ,[csruserkey] 
                //                      ,[campaignId] 
                //                      ,[firstName] FirstName
                //                      ,[lastName] LastName
                //                      ,[dayPhone] DayPhone
                //                      ,[eveningPhone] EveningPhone
                //                      ,[cellPhone] CellPhone
                //                      ,[userAssigned] UserAssigned
                //                      ,[CSR]
                //                      ,[TA] 
                //                      ,[OutpulseId]
                //                      ,[leadStatus] 
                //                      ,[Status]
                //                      ,[SubStatus1]
                //                      ,[leadCampaign]
                //                      ,[state]
                //                      ,[SubstatusId]
                //                  FROM [SQ_SalesTool].[dbo].[vw_leads]";

                //return E.Lead.ExecuteStoreQuery<Models.ViewLead>(K_QUERY).AsQueryable(); 

                E.Lead.ViewLeads.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                return E.Lead.ViewLeads;
            }
        }
        public int ViewLeadCount
        {
            get
            {
                const string K_QUERY = "SELECT dbo.LeadCount()";
                System.Data.Objects.ObjectResult<int> x = E.Lead.ExecuteStoreQuery<int>(K_QUERY);
                return x.FirstOrDefault();
                //return E.Lead.ViewLeads.Count();
            }
        }
        public IQueryable<Models.ViewLead> ApplySorting(IQueryable<Models.ViewLead> query, string sortcolumn, bool bAscending = true)
        {
            switch (sortcolumn)
            {
                case "accountId":
                    query = bAscending ? query.OrderBy(x => x.AccountId) : query.OrderByDescending(x => x.AccountId); break;
                case "leadId":
                    query = bAscending ? query.OrderBy(x => x.LeadId) : query.OrderByDescending(x => x.LeadId); break;
                //case "individualId":
                //    query = bAscending ? query.OrderBy(x => x.LeadId) : query.OrderByDescending(x => x.LeadId); break;
                case "dateCreated":
                    query = bAscending ? query.OrderBy(x => x.CreatedOn) : query.OrderByDescending(x => x.CreatedOn); break;
                case "firstName":
                    query = bAscending ? query.OrderBy(x => x.FirstName) : query.OrderByDescending(x => x.FirstName); break;
                case "lastName":
                    query = bAscending ? query.OrderBy(x => x.Lastname) : query.OrderByDescending(x => x.Lastname); break;
                case "dateOfBirth":
                    query = bAscending ? query.OrderBy(x => x.BirthDate) : query.OrderByDescending(x => x.BirthDate); break;
                case "dayPhone":
                    query = bAscending ? query.OrderBy(x => x.DayPhone) : query.OrderByDescending(x => x.DayPhone); break;
                case "eveningPhone":
                    query = bAscending ? query.OrderBy(x => x.EveningPhone) : query.OrderByDescending(x => x.EveningPhone); break;
                case "leadCampaign":
                    query = bAscending ? query.OrderBy(x => x.leadCampaign) : query.OrderByDescending(x => x.leadCampaign); break;
                case "leadStatus":
                    query = bAscending ? query.OrderBy(x => x.leadStatus) : query.OrderByDescending(x => x.leadStatus); break;
                case "SubStatus1":
                    query = bAscending ? query.OrderBy(x => x.SubStatus1) : query.OrderByDescending(x => x.SubStatus1); break;
                case "userAssigned":
                    query = bAscending ? query.OrderBy(x => x.UserAssigned) : query.OrderByDescending(x => x.UserAssigned); break;
                case "CSR":
                    query = bAscending ? query.OrderBy(x => x.CSR) : query.OrderByDescending(x => x.CSR); break;
                case "TA":
                    query = bAscending ? query.OrderBy(x => x.TA) : query.OrderByDescending(x => x.TA); break;
                case "State":
                    query = bAscending ? query.OrderBy(x => x.state) : query.OrderByDescending(x => x.state); break;
                default:
                    query = bAscending ? query.OrderBy(x => x.AccountId) : query.OrderByDescending(x => x.AccountId); break;
            }
            return query;
        }
        #endregion


        public long GetPrimaryIndividual(long id)
        {
            const string K_SQL = "SELECT dbo.GetPrimaryPersonId(@id)";
            var obj = E.leadEntities.ExecuteStoreQuery<long>(K_SQL, new SqlParameter[] { new SqlParameter("id", id) });
            return obj.FirstOrDefault();
        }
        public TCPAConsentType GetConsent(long id)
        {
            const string K_SQL = "SELECT dbo.GetConsent(@id)";
            var obj = E.leadEntities.ExecuteStoreQuery<string>(K_SQL, new SqlParameter[] { new SqlParameter("id", id) });

            TCPAConsentType type = TCPAConsentType.Blank;
            char ch = obj.FirstOrDefault().Trim().ToCharArray()[0];
            switch (ch)
            {
                case '0': type = TCPAConsentType.NoPrimary; break;
                case 'x': type = TCPAConsentType.Blank; break;
                case 'y': type = TCPAConsentType.Yes; break;
                case 'n': type = TCPAConsentType.No; break;
                case 'a': type = TCPAConsentType.Undefined; break;
            }
            return type;
        }
        public void SetConsent(long id, TCPAConsentType typ, string by="")
        {
            char? ch = null;
            switch (typ)
            {
                case TCPAConsentType.No: ch = 'n'; break;
                case TCPAConsentType.Yes: ch = 'y'; break;
                case TCPAConsentType.Undefined: ch = 'a'; break;
            }
            E.leadEntities.ExecuteStoreCommand("EXECUTE proj_SetConsent @id, @value, @by", new SqlParameter[] { new SqlParameter("@id", id), new SqlParameter("@value", ch.HasValue ? ch.Value : (object)DBNull.Value), new SqlParameter("@by", by) });

            var A = Get(id);
            Update(A, by);

            OnConsentUpdated(id);
        }
        public void AddDummyPrimaryIndividual(long id)
        {
            const string K_SQL = "EXECUTE proj_AddDummyPrimary @id";
            E.leadEntities.ExecuteStoreCommand(K_SQL, new SqlParameter[] { new SqlParameter("@id", id) });
        }

        // Sz [jan 27, 2014] added for finding if the new arc call is required
        public bool HasArcCase(long accountId)
        {
            const string K_SQL = @"Select case Count(AR.arc_ref) when 0 then 0 else 1 end from arc_cases AR where AR.act_key = @id";
            var obj = E.Lead.ExecuteStoreQuery<int>(K_SQL, new SqlParameter[] { new SqlParameter("id", accountId) });
            return obj.FirstOrDefault<int>() > 0;
        }

        /// <summary>
        /// Gets Associated accounts ids form account id
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        /// <author>MH[05 March 2014]:</author>
        public List<long> GetAssociatedAccountsIds(long accountId)
        {

            //            const string query = @"SELECT a.act_key FROM dbo.Accounts a INNER JOIN (
            //            SELECT DISTINCT act_parent_key AS parent_key FROM dbo.Accounts WHERE (act_key=@id OR act_parent_key=@id) AND act_parent_key IS NOT NULL) 
            //            sub ON sub.parent_key=a.act_key OR sub.parent_key=a.act_parent_key";
            // functionality move to store function
            const string query = @"SELECT * FROM dbo.RelatedAccountIds(@id)";
            var list = E.leadEntities.ExecuteStoreQuery<long>(query, new System.Data.SqlClient.SqlParameter("id", SqlDbType.BigInt)
            {
                Value = accountId
            }).ToList();
            return list;
        }
        public bool IsMultipleAccountsAllowed()
        {
            const string query = @"SELECT  CASE WHEN res.HasLeadNewLayout = 1
                  AND res.APPLICATION_TYPE = 2 THEN 1
             ELSE 0
        END
FROM    ( SELECT    ( SELECT    ISNULL(c.bValue, 0)
                      FROM      dbo.Application_Storage c
                      WHERE     c.[Key] = 'SQL_LEAD_NEW_LAYOUT'
                    ) AS 'HasLeadNewLayout' ,
                    ( SELECT    c.iValue
                      FROM      dbo.Application_Storage c
                      WHERE     c.[Key] = 'APPLICATION_TYPE'
                    ) AS 'APPLICATION_TYPE'
        ) AS res";

            bool status = Convert.ToBoolean(E.leadEntities.ExecuteStoreQuery<int>(query).FirstOrDefault());
            return status;
        }

        /// <summary>
        /// Update All associated Accounts' Assigned Agent, given AccountId (regardless parent/child) and AgentId
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="agentId">Null agent Reset assigned agent.</param>
        /// <author>MH[05 March 2014]</author>
        public void UpdateAssignedAgentToAssociatedAccounts(long accountId, Guid? agentId)
        {
            const string query = @"UPDATE  dbo.Accounts
                                    SET     act_assigned_usr = @AgentId
                                    WHERE   act_key IN (
                                            SELECT  a.act_key
                                            FROM    dbo.Accounts a
                                                    INNER JOIN ( SELECT DISTINCT
                                                                        act_parent_key AS parent_key
                                                                 FROM   dbo.Accounts
                                                                 WHERE  ( act_key = @AccountId
                                                                          OR act_parent_key = @AccountId
                                                                        )
                                                                        AND act_parent_key IS NOT NULL
                                                               ) sub ON sub.parent_key = a.act_key
                                                                        OR sub.parent_key = a.act_parent_key )";

            var array = new SqlParameterFluent()
                .Add("AccountId", accountId, SqlDbType.BigInt)
                .Add("AgentId", agentId, SqlDbType.UniqueIdentifier)
                .ToObjectArray();
            int affected = E.leadEntities.ExecuteStoreCommand(query, array);
        }

        public void AssignCalenderDatesToAccount(long accountId)
        {
            var parm =
                new System.Data.SqlClient.SqlParameterFluent().Add("Act_Id", accountId)
                                                              .ToObjectArray();
            var res=E.leadEntities.ExecuteStoreProcedure("proj_AssignCalenderDates", parm);

        }
        public void CheckAndAssignAgent(long accountId, Guid agentId)
        {
            var parm =
                new System.Data.SqlClient.SqlParameterFluent().Add("agent", agentId)
                                                              .Add("actId", accountId)
                                                              .ToObjectArray();
            int res = E.leadEntities.ExecuteStoreCommand("UPDATE dbo.Accounts SET act_assigned_usr=@agent, act_modified_date=GETDATE()  WHERE act_key=@actId AND act_assigned_usr IS null", parm);
            if (res > 0)
            {
                Engine.AccountHistory.Log(accountId, "User assigned", agentId);
                Engine.AccountHistory.LogAssignment(accountId, "User assigned", "from AccountAction.CheckAndAssignAgent", agentId);
            }

        }
    }
}
