using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public enum ActionHistoryType
    {
        Actions = 1, Log = 2, All = 3, Calls = 4, ArcCritical = 5, PolicyStatus = 6
    }
    public class AccountHistoryActions : BaseActions
    {
        internal AccountHistoryActions(DBEngine engine)
            : base(engine)
        { }

        internal event EventHandler<AccountHistoryEventArgs> AccountHistoryAdded;

        protected virtual void OnAccountHistoryAdded(Models.AccountHistory history)
        {
            var handler = AccountHistoryAdded;
            if (handler != null) handler(this, new AccountHistoryEventArgs(history));

        }

        // SZ [Mar 29, 2013] this has been commented out as newer functions have been added for recording the history of the accoutn changes
        // the new functions are easier to use

        //public Int64 Add(Models.AccountHistory accountHistory)
        //{
        //    accountHistory.Datetime = DateTime.Now;
        //    E.Lead.AccountHistories.AddObject(accountHistory);
        //    E.Save();

        //    return accountHistory.AccountHistoryId;
        //}



        // SZ [Mar 29, 2013] this has been added for quickly adding the history of action change


        ///[QN, 14/05/2013] New parameters (Int64 ActionId, Int64 CurrentStatusId, 
        ///Int64 CurrentSubstatusId, Int64 NewStatusId, Int64 NewSubstatusId) 
        ///has been added in this function. 
        ///following fields has been added in account_history table
        /// ach_action_key (action key)
        /// ach_cur_status_key (status key)
        /// ach_cur_sub_status_key (status key)
        /// ach_new_status_key (status key)
        /// ach_new_sub_status_key (status key)
        ///... the work is done against change request mentioned in mantis item 148.
        ///For details see link. (http://bugs.condadogroup.com/view.php?id=148)
        public long ActionChanged(long accID, string actionName, string comment, Guid userKey, Int64 actionId, Int64 currentStatusId, Int64 currentSubstatusId, Int64 newStatusId, Int64 newSubstatusId, int ruleId = 0)
        {
            //MH:21 April 2014;
            if (userKey.IsEmpty())
            {
                throw new ArgumentException("Empty AgentId while ActionChanged");
            }
            var U = E.UserActions.Get(userKey);
            bool Ass = false;

            //SZ [April 18, 2014] This was the original line, after an issue raised by Muzamil, 
            //the line is modified to pass parameters , all of them the discussion
            //var X = E.AccountActions.Get(accID).AssignedUserKey;

            var X = E.AccountActions.Get(accID, false, true).AssignedUserKey;
            if (X != null && U != null && X == U.Key)
                Ass = true;

            var AH = new Models.AccountHistory
             {
                 AccountId = accID,
                 Comment = comment,
                 Datetime = DateTime.Now,
                 Entry = actionName,
                 EntryType = 1,
                 User = userKey,
                 Action = actionId,
                 CurrentStatus = currentStatusId,
                 CurrentSubstatus = currentSubstatusId,
                 NewStatus = newStatusId,
                 NewSubstatus = newSubstatusId,
                 IsUserCSR = U.DoesCSRWork ?? false,
                 IsUserAP = U.IsAlternateProductType ?? false,
                 IsUserOB = U.IsOnboardType ?? false,
                 IsUserTA = U.IsTransferAgent ?? false,
                 IsUserAssigned = Ass,
                 RuleId = ruleId == 0 ? (int?)null : ruleId,
             };
            E.Lead.AccountHistories.AddObject(AH);
            E.Save();
            OnAccountHistoryAdded(AH);
            return AH.AccountHistoryId;
        }

        /// <summary>
        /// Update Action On Related Accounts and also makes history entry for all related accounts
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="actionId"></param>
        /// <param name="actionName"></param>
        /// <param name="comments"></param>
        /// <param name="userId"></param>
        /// <param name="curStatusKey"></param>
        /// <param name="curSubStatusKey"></param>
        /// <param name="newStatusKey"></param>
        /// <param name="newSubStatusKey"></param>
        /// <author>MH</author>
        public List<long> ActionChangedRelatedAccounts(long accountId, string actionName, string comments, Guid userId, int actionId,
             long? curStatusKey, long? curSubStatusKey,
            long? newStatusKey, long? newSubStatusKey, int? ruleid = 0)
        {
            //MH:21 April 2014;
            if (userId.IsEmpty())
            {
                throw new ArgumentException("Empty AgentId while ActionChanged Related Accounts");
            }
            int entryType = 1;
            string updateQuery = @"
                             UPDATE dbo.leads
                             SET    lea_status = @statusID ,
                                    lea_sub_status = @substatusID ,
                                    lea_modified_date = GETDATE() ,
                                    lea_modified_user = @userId ,
                                    lea_last_action_date = GETDATE() ,
                                    lea_last_action = @ActionId
                             WHERE  lea_key IN (
                                    SELECT  l.lea_key
                                    FROM    dbo.Accounts a
                                            INNER JOIN dbo.leads l ON l.lea_key = a.act_lead_primary_lead_key
                                                                      AND a.act_key IN (
                                                                      SELECT    id
                                                                      FROM      dbo.RelatedAccountIds(@accountId) ) )";

            var affected = E.leadEntities.ExecuteStoreCommand(updateQuery, new System.Data.SqlClient.SqlParameterFluent()
                .Add("accountId", accountId, SqlDbType.BigInt)
                .Add("ActionId", actionId, SqlDbType.Int)
                .Add("statusID", newStatusKey, SqlDbType.BigInt)
                .Add("substatusID", newSubStatusKey, SqlDbType.BigInt)
                .Add("userId", userId, SqlDbType.UniqueIdentifier)
                .ToObjectArray());


            string insertHistoryQuery = @"
                                    DECLARE @output TABLE (id BIGINT)
                                    INSERT  INTO dbo.account_history
                                    ( ach_entry ,
                                      ach_account_key ,
                                      ach_comment ,
                                      ach_userid ,
                                      ach_added_date ,
                                      ach_entryType ,
                                      ach_action_key ,
                                      ach_cur_status_key ,
                                      ach_cur_sub_status_key ,
                                      ach_new_status_key ,
                                      ach_new_sub_status_key ,
                                      ach_delivered_to_arc,
                                        ach_pv_key
       
                                    )
                                    OUTPUT inserted.ach_key INTO @output
                                    SELECT  @entry , 
                                            id , 
                                            @comments ,
                                            @userId , 
                                            GETDATE() ,
                                            @entryType ,
                                            @actionKey ,
                                            @cur_status_key ,
                                            @cur_sub_status_key ,
                                            @new_status_key ,
                                            @new_sub_status_key , 
                                            0 ,
                                            @ruleID 
                
                                    FROM    dbo.RelatedAccountIds(@accountId)
                                    select * from @output";
            var insertedIds = E.leadEntities.ExecuteStoreQuery<long>(insertHistoryQuery, new System.Data.SqlClient.SqlParameterFluent()
                .Add("accountId", accountId, SqlDbType.BigInt)
                .Add("entry", actionName, SqlDbType.NVarChar, 100)
                .Add("comments", comments, SqlDbType.NVarChar, -1)//max
                .Add("userId", userId, SqlDbType.UniqueIdentifier)
                .Add("entryType", entryType, SqlDbType.TinyInt)
                .Add("actionKey", actionId, SqlDbType.BigInt)
                .Add("cur_status_key", curStatusKey, SqlDbType.BigInt)
                .Add("cur_sub_status_key", curSubStatusKey, SqlDbType.BigInt)
                .Add("new_status_key", newStatusKey, SqlDbType.BigInt)
                .Add("new_sub_status_key", newSubStatusKey, SqlDbType.BigInt)
                .Add("ruleID", ruleid == 0 ? null : ruleid, SqlDbType.Int)
                .ToObjectArray()).ToList();
            if (insertedIds.Any())
                E.OnMultipleAccountHistoryAdded(insertedIds);
            return insertedIds;
        }
        /// <summary>
        /// [QN, 14/05/2013] this function is used to add... 
        /// ... records in the Account history table.
        /// </summary>
        public void AddAccountHistorySubstatusII(Int64 accountHistoryId, Int64 substatus2)
        {
            E.Lead.account_history_sub_status.AddObject(new Models.AccountHistorySubstatusII
            {
                AccountHistoryId = accountHistoryId,
                Substatus2Id = substatus2
            });
            E.Save();
        }
        //[MH:07 March 2014]
        public void AddAccountHistorySubstatusII(List<long> accountHistoryIds, Int64 substatus2, bool isSave = true)
        {
            foreach (var historyId in accountHistoryIds)
            {
                E.Lead.account_history_sub_status.AddObject(new Models.AccountHistorySubstatusII
                    {
                        AccountHistoryId = historyId,
                        Substatus2Id = substatus2
                    });
            }
            if (isSave)
                if (accountHistoryIds.Any())
                    E.Save();
        }
        //[MH:07 March 2014]
        public void AddAccountHistorySubstatusII(List<long> accountHistoryIds, List<long> substatus2List)
        {
            substatus2List.ForEach(s => AddAccountHistorySubstatusII(accountHistoryIds, s, false));
            if (accountHistoryIds.Any() && substatus2List.Any())
                E.Save();
        }
        // SZ [Mar 29, 2013] this has been added for quickly adding the loging of account
        public void Log(long accID, string comment, Guid userKey, int ruleId = 0, string contactid = "", byte? entryType=2)
        {
            //MH:21 April 2014; fixing multiple un-intensional bugs as occured at new Arc call button from Selectcare service.
            if (userKey.IsEmpty())
            {
                throw new ArgumentException("Empty AgentId while History Log");
            }

            //SZ [Apr 21, 2014] Client required, user type should be fetched from the account, not the user logged in
            //var U = E.UserActions.Get(userKey);
            //bool Ass = false;
            //MH:19 April fix of split account feature broken
            //var X = E.AccountActions.Get(accID,false,true).AssignedUserKey;
            //if(X!=null && U!=null && X==U.Key)
            //    Ass= true;
            var X = E.AccountActions.Get(accID, false, true);

            //MH:25 April Fix: TCPA object reference not found issue with new account under SQL NewLayout
            var history = new AccountHistory
                {
                    AccountId = accID,
                    Comment = comment,
                    Datetime = DateTime.Now,
                    Entry = "Log",
                    EntryType = entryType,
                    User = userKey,
                    RuleId = ruleId > 0 ? ruleId : (int?)null,
                    IsUserCSR = X != null && X.AssignedCsrKey != null,
                    IsUserAP = X != null && X.AlternateProductUser != null,
                    IsUserOB = X != null && X.OnBoardUser != null,
                    IsUserTA = X != null && X.TransferUser != null,
                    IsUserAssigned = X != null && X.AssignedUserKey != null,
                    ArcContactId = contactid
                };

            //MH:14 May 2014 related to arc real-time update for changes agent arcapi call
            bool already = false;
            if (comment == "User assigned")
            {
                already=Engine.ArcActions.IsUserAssignedBefore(accID);
                history.IsDeliveredToArc = already;
                //history.IsDeliveredToArc = Engine.ArcActions.IsUserAssignedBefore(accID);
                //if (!Engine.MarkArcUserAssignmentDeliveryAs)
                //    Engine.MarkArcUserAssignmentDeliveryAs = true;
            }
            E.Lead.AccountHistories.AddObject(history);
            E.Save();
            if (!already)
                OnAccountHistoryAdded(history);
        }


        
        public void LogAssignment(long accId, string prefix,string comment, Guid userKey, int ruleId = 0, string contactid = "")
        {
            comment = string.Format("{0} - {1}", prefix, comment);
            Log(accId,comment,userKey,ruleId,contactid,7);
        }
        //YA[01 May 2014] Policy Status Changed entry
        public void PolicyStatusChanged(long accID, string msgEntry, string comment, Guid userKey, int ruleId = 0, long policyStatusID = 0)
        {
            var X = E.AccountActions.Get(accID, false, true);
            var history = new AccountHistory
                {
                    AccountId = accID,
                    Comment = comment,
                    Datetime = DateTime.Now,
                    Entry = msgEntry,
                    EntryType = 6,
                    User = userKey,
                    RuleId = ruleId > 0 ? ruleId : (int?)null,
                    IsUserCSR = X != null && X.AssignedCsrKey != null,
                    IsUserAP = X != null && X.AlternateProductUser != null,
                    IsUserOB = X != null && X.OnBoardUser != null,
                    IsUserTA = X != null && X.TransferUser != null,
                    IsUserAssigned = X != null && X.AssignedUserKey != null,
                    PolicyStatusId = (long?)policyStatusID
                };

            E.Lead.AccountHistories.AddObject(history);
            E.Save();
            OnAccountHistoryAdded(history);
        }

        // WM [May 31, 2013] this has been added for quickly adding the calls
        public void AddCall(long accID, string phoneNumber, Guid userKey, string contactid = "")
        {
            // MH:21 April 2014: fix object reference exception with empty use key
            if (userKey.IsEmpty())
            {
                throw new ArgumentException("Empty AgentId while AddCall");
            }

            //SZ [Apr 21, 2014] Client required, user type should be fetched from the account, not the user logged in
            //var U = E.UserActions.Get(userKey);
            //bool Ass = false;
            //MH: fix of causing loss of changes.

            //var X = E.AccountActions.Get(accID, false, true).AssignedUserKey;
            //if (X != null && U != null && X == U.Key)
            //    Ass = true;
            var X = E.AccountActions.Get(accID, false, true);

            //MH:25 April Fix: TCPA object reference not found issue with new account under SQL NewLayout
            var history = new AccountHistory
                {
                    AccountId = accID,
                    Comment = phoneNumber,
                    Datetime = DateTime.Now,
                    //Entry = "Calls",    // SZ [Apr 4, 2014] i. Rename action log type of “Call” to “Call Contact”
                    Entry = "Call Contact",
                    EntryType = 4,
                    User = userKey,
                    IsUserCSR = X != null && X.AssignedCsrKey != null,
                    IsUserAP = X != null && X.AlternateProductUser != null,
                    IsUserOB = X != null && X.OnBoardUser != null,
                    IsUserTA = X != null && X.TransferUserKey != null,
                    IsUserAssigned = X != null && X.AssignedUserKey != null,
                    ArcContactId = contactid
                };
            E.Lead.AccountHistories.AddObject(history);
            E.Save();
            OnAccountHistoryAdded(history);

        }

        // After effects of WM even though he is gone! 
        //Entry = "Calls",    // SZ [Apr 4, 2014] i. Rename action log type of “Call” to “Call Contact”
        public void LogCall(long accID, string phoneNumber, Guid userKey, string bMsg = "Call Attempt")
        {
            // MH:21 April 2014: fix object reference exception with empty use key
            if (userKey.IsEmpty())
            {
                throw new ArgumentException("Empty AgentId while AddCall");
            }

            //SZ [Apr 21, 2014] Client required, user type should be fetched from the account, not the user logged in
            //var U = E.UserActions.Get(userKey);
            //bool Ass = false;
            //MH: fix of causing loss of modified data at account.
            //var X = E.AccountActions.Get(accID,false,true).AssignedUserKey;
            //if (X != null && U != null && X == U.Key)
            //    Ass = true;

            var X = E.AccountActions.Get(accID, false, true);
            //MH:25 April Fix: TCPA object reference not found issue with new account under SQL NewLayout
            var history = new AccountHistory
                    {
                        AccountId = accID,
                        Comment = phoneNumber,
                        Datetime = DateTime.Now,
                        Entry = bMsg,
                        EntryType = 4,
                        User = userKey,
                        IsUserCSR = X != null && X.AssignedCsrKey != null,
                        IsUserAP = X != null && X.AlternateProductUser != null,
                        IsUserOB = X != null && X.OnBoardUser != null,
                        IsUserTA = X != null && X.TransferUserKey != null,
                        IsUserAssigned = X.AssignedUserKey != null
                    };
            E.Lead.AccountHistories.AddObject(history);
            E.Save();
            OnAccountHistoryAdded(history);
        }

        /// <summary>
        /// Update Agent First Call and update leads's last call if updateLast is true
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="actId"></param>
        /// <param name="updateLast"></param>
        /// <returns></returns>
        /// <author>MH</author>
        public bool CheckAndLogAgentFirstCall(string agentId, long actId, bool updateLast = true)
        {
            Guid? agent = agentId.AsGuid();
            if (agent.IsNullOrEmpty())
            {
                throw new ArgumentException("Empty AgentId while CheckAndLogAgentFirstCall");
            }
            ////If agent_first_call is null, write the call log timestamp to the agent_first_call field
            bool isLog = false;
            var parameters = new System.Data.SqlClient.SqlParameterFluent().Add(
                "userid", agentId).ToObjectArray();
            DateTime? dateTime = Engine.leadEntities.ExecuteStoreProcedure<DateTime?>("proj_CheckAgentFirstCall", parameters).FirstOrDefault();
            if (!dateTime.HasValue)
            {
                
                var para = new System.Data.SqlClient.SqlParameterFluent().Add(
                 "userid", agentId).ToObjectArray();
                Engine.leadEntities.ExecuteStoreProcedure("proj_UpdateAgentFirstCall", para);

                isLog = true;
            }
            if (updateLast)
            {
                var para1 = new System.Data.SqlClient.SqlParameterFluent().Add(
                 "actId", actId).ToObjectArray();
                Engine.leadEntities.ExecuteStoreCommand(@" UPDATE dbo.leads SET lea_last_call_date=GETDATE()
                                                         FROM dbo.Accounts A
                                                         INNER JOIN dbo.leads L  ON
                                                          A.act_lead_primary_lead_key=l.lea_key WHERE A.act_key=@actId", para1);
                isLog = true;
            }

            return isLog;
        }
        public Models.AccountHistoryEntry GetRecentHistoryByAccountID(long id)
        {
            return E.Lead.AccountHistoryEntries.Where(x => x.AccountId == id).OrderByDescending(x => x.AddedOn).FirstOrDefault();
            //return E.Lead.AccountHistories.Where(x => x.AccountId == id).OrderByDescending(x => x.Datetime).FirstOrDefault();
        }

        //public Models.AccountHistory GetRecentAccountHistory(int id)
        //{
        //   // return E.Lead.AccountHistories.Where(x => x.AccountId == id);//.Max(Datetime);
        //}
        // SZ [Mar 15, 2013] dead function, commented out
        //public Models.AccountHistory GetAccountHistory(Int64 accountId)
        //{
        //    return this.GetAllAccountHistory().FirstOrDefault(x => x.AccountId == accountId);
        //}

        //public IEnumerable<Models.AccountHistory> GetAllAccountHistory()
        //{
        //    return E.Lead.AccountHistories;//.FirstOrDefault(x => x.AccountId == accountId);
        //}
        public void Save(Models.AccountHistory history)
        {
            Engine.Save();
        }

        public void MergeAccounts(Int64 accountId, Int64 parentAccountId)
        {
            Int64 accountId1 = E.Lead.Leads.Where(x => x.Key == accountId).FirstOrDefault().Account.Key;
            Int64 parentAccountId1 = E.Lead.Leads.Where(x => x.Key == parentAccountId).FirstOrDefault().Account.Key;

            string updateQuery = @"
                             UPDATE dbo.account_history
                             SET    ach_account_key = @parentAccountId 
                             WHERE   ach_account_key = @accountId";

            var affected = E.leadEntities.ExecuteStoreCommand(updateQuery, new System.Data.SqlClient.SqlParameterFluent()
                .Add("accountId", accountId1, SqlDbType.BigInt)
                .Add("parentAccountId", parentAccountId1, SqlDbType.BigInt)
                .ToObjectArray());
        }

        public IQueryable<Models.AccountHistoryEntry> GetHistory(long accountID, ActionHistoryType type = ActionHistoryType.Actions)
        {
            IQueryable<Models.AccountHistoryEntry> Ans = null;

            switch (type)
            {
                case ActionHistoryType.Actions:
                    Ans = E.Lead.AccountHistoryEntries.Where(x => x.AccountId == accountID && x.EntryType == 1).OrderByDescending(x => x.AddedOn);
                    break;
                case ActionHistoryType.Calls:
                    Ans = E.Lead.AccountHistoryEntries.Where(x => x.AccountId == accountID && x.EntryType == 4).OrderByDescending(x => x.AddedOn);
                    break;
                case ActionHistoryType.Log:
                    Ans = E.Lead.AccountHistoryEntries.Where(x => x.AccountId == accountID && x.EntryType == 2).OrderByDescending(x => x.AddedOn);
                    break;
                case ActionHistoryType.PolicyStatus:
                    Ans = E.Lead.AccountHistoryEntries.Where(x => x.AccountId == accountID && x.EntryType == 6).OrderByDescending(x => x.AddedOn);
                    break;
                case ActionHistoryType.All:
                    Ans = E.Lead.AccountHistoryEntries.Where(x => x.AccountId == accountID).OrderByDescending(x => x.AddedOn);
                    break;
            }
            return Ans;
            //var users = E.UserActions.GetAll().ToList();
            //var history = E.Lead.AccountHistories
            //    .Where(x => x.AccountId == accountID)
            //    .OrderByDescending(x=>x.Datetime) 
            //    .ToList()
            //    .Join(users, 
            //        a=>a.User, 
            //        b=>b.Key,
            //        (a, b)=> new { UserName = b.FullName, History = a})
            //    .Select(x => new { 
            //        EntryType = x.History.EntryType,
            //        Entry = x.History.Entry, 
            //        Comment = x.History.Comment, 
            //        UserKey = x.History.User,
            //        UserName = x.UserName, 
            //        Date = x.History.Datetime})
            //     .AsQueryable();

            //// SZ [Mar 29, 2013] This has been added so that filtering can be performed based on EntryType.
            //// it is not performed above in the where clause because if All is selected then no filtering is required at all
            //switch (type)
            //{
            //    case ActionHistoryType.Actions:
            //        history = history.Where(x => x.EntryType == 1); break;
            //    case ActionHistoryType.Log:
            //        history = history.Where(x => x.EntryType == 2); break;
            //}

            //return history.AsEnumerable();
        }

        public bool CheckHistoryExistence(long? accountid, string entry, int entryType, string comment)
        {
            return E.leadEntities.AccountHistories.Any(
                p => p.AccountId == accountid
                    && p.Entry == entry
                    && p.EntryType == entryType
                    && p.Comment == comment);

        }
    };
    public class AccountHistoryEventArgs : EventArgs
    {
        public Models.AccountHistory History { get; private set; }

        public AccountHistoryEventArgs(Models.AccountHistory history)
        {
            History = history;
        }
    }
}
