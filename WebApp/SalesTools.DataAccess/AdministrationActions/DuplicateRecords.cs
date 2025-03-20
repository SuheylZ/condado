using System;
using System.Collections.Generic;
using System.Linq;
using SalesTool.DataAccess.Models;
using Sql = System.Data.SqlClient;


namespace SalesTool.DataAccess.AdministrationActions
{
    /// <summary>
    /// It provides backed functionality and allows the addition, updation, deletion and various other functionality
    /// </summary>
    public class DuplicateRecords: BaseActions
    {
        internal DuplicateRecords(DBEngine engine) : base(engine) { }

        public int Add(string title, short parent, short duplicateCriteria, bool active, bool manual, int actionid, string comment, short incommingFilter, string incomingValue, short exisitngFilter, string existingValue, string By ){
            DuplicateManagement D = new DuplicateManagement
            {
                Title = title,
                Priority = HighestPriority +1 ,
                ActionId = actionid,
                ActionComment = comment,
                IsActive = active,
                IsManual = manual,
                IncommingLeadFilterSelection = incommingFilter,
                IncommingLeadFilterCustomValue = incomingValue,
                ExistingLeadFilterSelection = exisitngFilter,
                ExistingLeadFilterCustomValue = existingValue,
                MultipleDuplicateCriteria = duplicateCriteria,
                SelectedParent = parent
            };
            return Add(D, By);
        }
        public int Add(Models.DuplicateManagement D, string by)
        {
            D.Id = NewId;
            D.Added.By = by;
            D.Added.On = DateTime.Now;
            D.Priority = HighestPriority + 1;
            E.Admin.AddToDuplicateManagements(D);
            E.Save();

            return D.Id;
        }

        // SZ [July 17, 2014] Checking the delete functionality.
        // Reference: [7/16/2014 5:45:17 PM] John Dobrotka: Please make sure that every area that allows deleting on 
        // Normal View, Prioritized View and Leads.aspx (Tabs) is security controlled. they should all be soft deletes 
        // and should only have the delete option show if the Soft Delete permission is set in thir security
        public void Delete(int id, bool bSoft = true)
        {
            if (Exists(id))
            {
                var X = Get(id, false);
                X.FieldTagsRulesColumns.Clear();
                if (bSoft)
                    X.IsActive = false;
                else
                    E.Admin.DuplicateManagements.DeleteObject(X);

                E.Save();
            }
        }
        public void Change(Models.DuplicateManagement D, string by) {
            D.Changed.By = by;
            D.Changed.On = DateTime.Now;
            E.Save();
        }

        public IQueryable<Models.DuplicateManagement> All { get { return E.Admin.DuplicateManagements.AsQueryable(); } }
        public Models.DuplicateManagement Get(int id, bool bFresh=false)
        {
            if (bFresh)
                E.Admin.DuplicateManagements.MergeOption = System.Data.Objects.MergeOption.NoTracking;

            var obj = E.Admin.DuplicateManagements.Where(x => x.Id == id).FirstOrDefault();

            if (bFresh)
            {
                E.Admin.Refresh(System.Data.Objects.RefreshMode.StoreWins, obj.FieldTagsRulesColumns);
                E.Admin.DuplicateManagements.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;    
            }

            return obj;
        }
        public IQueryable<Models.DuplicateManagement> GetAllActive()
        {
            return All.Where(x => x.IsActive == true);
        }

        //SZ [June 07, 2013] Dont use these functions, these will be obsolete soon. 
        // they also bypass the EntityFramework.
        /// <summary>
        /// This fucntion assigns the field tags to the duplicate management (Do not use)
        /// </summary>
        /// <param name="id">id of duplicate management</param>
        /// <param name="tagids">array containing the Models.TagField ids</param>
        [System.Obsolete("do not use this method. It by passes the regular mechanism.")]
        public void _AddTags(int id, int[] tagids)
        {
  
                const string K_SQL = "INSERT INTO [dbo].[duplicate_merge_column] ([tag_key], [dm_id]) VALUES (@tagid, @dmId)";
                for (int i = 0; i < tagids.Length; i++)
                    E.Admin.ExecuteStoreCommand(K_SQL, new Sql.SqlParameter[] { new Sql.SqlParameter("tagid", tagids[i]), new Sql.SqlParameter("dmid", id) });
        }

        /// <summary>
        /// This function clears the assigned tags from the duplicate rule (Do not use)
        /// </summary>
        /// <param name="id">the id of the duplicate management</param>
        [System.Obsolete("do not use this method. It by passes the regular mechanism.")]
        public void _ClearTags(int id)
        {
            const string K_SQL = "DELETE [dbo].[duplicate_merge_column] WHERE [dm_id]=@dmId";
            E.Admin.ExecuteStoreCommand(K_SQL, new Sql.SqlParameter[] { new Sql.SqlParameter("dmid", id) });
        }


        /// <summary>
        /// Retrieves the TagField ids which are unassigned
        /// </summary>
        /// <param name="id">duplicate management id</param>
        /// <returns>list of TagField ids which are not assigned to this Duplicate Rule</returns>
        [System.Obsolete("do not use this method. It by passes the regular mechanism.")]
        public int[] _GetFreeTags(int id)
        {
            const string K_SQL = "Select F.tag_key FROM field_tags F WHERE F.tag_key NOT IN (SELECT D.tag_key FROM duplicate_merge_column D Where D.dm_id= @id)";
            var x = E.Admin.ExecuteStoreQuery<int>(K_SQL, new Sql.SqlParameter[] { new Sql.SqlParameter("id", id) });
            List<int> Ans= x.ToList();
            return Ans.ToArray();
        }


//        public IEnumerable<Models.DuplicateManagement> AllRaw
//        {
//            get
//            {
//                const string K_SQL = @"Select  dm_id AS Id, dm_title AS Title, dm_isactive AS IsActive, dm_ismanual AS IsManual, dm_priority AS Priority, dm_add_user AS [Added.By], 
//                      dm_add_date AS [Added.On], dm_change_user AS [Changed.By], dm_change_date AS [Changed.On], 
//                      dm_incoming_lead_filter_selection AS IncomingLeadFilterSelection, dm_incoming_lead_filter_customValue AS IncomingLeadFilterValue, 
//                      dm_existing_lead_filter_selection AS ExistingLeadFilterSelection, dm_existing_lead_filter_customValue AS ExistingLeadFilterValue, 
//                      dm_multiple_duplicate_criteria AS DuplicateCirteria, dm_selected_parent AS Parent, dm_action_id AS ActionId, dm_action_comment AS ActionComment
//                      FROM duplicate_management";
//                var A = E.Admin.ExecuteFunction<Models.DuplicateManagement>(K_SQL);
//                return A;
//            }
//        }


        public void AddTags(int id, int[] tags)
        {
            var X = Get(id);
            if (X != null)
            {
                foreach (int I in tags)
                    X.FieldTagsRulesColumns.Add(E.TagFieldsActions.Get(I));
                E.Save();
            }

        }
        public void ClearTags(int id)
        {
            var X = Get(id);
            if (X != null)
            {
                X.FieldTagsRulesColumns.Clear();
                E.Save();
            }
        }
        public void AddTagsForMergeRules(int id, int[] tags)
        {
            var X = Get(id);
            if (X != null)
            {
                foreach (int I in tags)
                    X.FieldTagsMergeColumns.Add(E.TagFieldsActions.Get(I));
                E.Save();
            }

        }
        public void ClearTagsForMergeRules(int id)
        {
            var X = Get(id);
            if (X != null)
            {
                X.FieldTagsMergeColumns.Clear();
                E.Save();
            }
        }

        public IEnumerable<Models.TagFields> GetFreeTags(int id)
        {
            var list = Get(id).FieldTagsRulesColumns.Select(x=>x.Id);
            return E.TagFieldsActions.GetAll().Where(x => !list.Contains(x.Id)).AsEnumerable();
        }

        public IEnumerable<Models.TagFields> GetFreeTags(int id,short baseDataId)
        {
            var list = Get(id).FieldTagsRulesColumns.Select(x => x.Id);
            return Engine.TagFieldsActions.GetAllReportTagsByBaseDataID(baseDataId).Where(x => !list.Contains(x.Id)).AsEnumerable();
        }

        public IEnumerable<Models.TagFields> GetFreeTagsForMergeRules(int id, short baseDataId)
        {
            var list = Get(id).FieldTagsMergeColumns.Select(x => x.Id);
            return Engine.TagFieldsActions.GetAllReportTagsByBaseDataID(baseDataId).Where(x => !list.Contains(x.Id)).AsEnumerable();
        }

        public bool Exists(int id)
        {
            const string K_QUERY = "SELECT count(*) from dbo.duplicate_management where dm_id =@id";
            System.Data.Objects.ObjectResult<int> x = E.Lead.ExecuteStoreQuery<int>(K_QUERY, new Sql.SqlParameter[] {new Sql.SqlParameter("id", id)});
            return x.FirstOrDefault<int>() > 0;
        }
        public bool Exists(string title)
        {
            const string K_QUERY = "SELECT count(*) from dbo.duplicate_management where dm_title =@title";
            System.Data.Objects.ObjectResult<int> x = E.Lead.ExecuteStoreQuery<int>(K_QUERY, new Sql.SqlParameter[] { new Sql.SqlParameter("title", title) });
            return x.FirstOrDefault<int>() > 0;
        }
        
        
        public void Enabled(int id)
        {
            const string K_QUERY = "UPDATE dbo.duplicate_management set dm_isactive = 1 - dm_isactive WHERE dm_id = @id";
            E.Lead.ExecuteStoreCommand(K_QUERY, new Sql.SqlParameter[] { new Sql.SqlParameter("id", id) });
        }

        public int HighestPriority
        {
            get
            {
                string K_SQL = "SELECT ISNULL(MAX(DM.dm_priority), 0) FROM [duplicate_management] DM";
                var x = E.Admin.ExecuteStoreQuery<int>(K_SQL);
                return x.FirstOrDefault();
            }
        }
        public int LowestPriority
        {
            get
            {
                string K_SQL = "SELECT ISNULL(MIN(DM.dm_priority), 0) FROM [duplicate_management] DM";
                var x = E.Admin.ExecuteStoreQuery<int>(K_SQL);
                return x.FirstOrDefault();
            }
        }

        int NewId
        {   get
        {
                const string K_SQL = "SELECT ISNULL(MAX(dm_id), 0) FROM duplicate_management";
                var x = E.Admin.ExecuteStoreQuery<int>(K_SQL);
                return x.FirstOrDefault() + 1;
            }
        }

        int _Priority(int id)
        {
            string K_SQL = "SELECT ISNULL(D.dm_priority, 0) FROM [duplicate_management] D WHERE D.dm_id=@id ";
            return E.Admin.ExecuteStoreQuery<int>(K_SQL, new Sql.SqlParameter[] {new Sql.SqlParameter("id", id)}).FirstOrDefault();
        }

        public void Move(int id, int position)
        {
            E.Admin.DuplicatePriorityChange(id, position);
        }
        public void Up(int id)
        {
            Move(id, _Priority(id) +1);
        }
        public void Down(int id)
        {
            Move(id, _Priority(id) -1 -1);
        }

        public IQueryable<Models.FilterLead> GetFiltersForExisitng(int ruleid)
        {
            return E.adminEntities.ViewFilterLeads.Where(x => x.RULEID == ruleid && x.FILTERTYPE==6);
        }
        public IQueryable<Models.FilterLead> GetFiltersForIncomming(int ruleid)
        {
            return E.adminEntities.ViewFilterLeads.Where(x => x.RULEID == ruleid && x.FILTERTYPE == 7);
        }
        public IQueryable<Models.DuplicateColumn> GetDuplicateColumns(int ruleid)
        {
            return E.adminEntities.ViewDuplicateColumns.Where(x => x.RULEID == ruleid);
        }
        //YA[June 13, 2013] Reconciliation Report
        public IQueryable<ReconciliationReport> GetReconciliationReport()
        {
            return E.adminEntities.ReconciliationReports;            
        }
        public bool ExecuteQuery(string query)
        {
            string K_QUERY = query;
            System.Data.Objects.ObjectResult<int> x = E.Lead.ExecuteStoreQuery<int>(K_QUERY);
            return x.FirstOrDefault<int>() > 0;
        }
        public System.Data.Objects.ObjectResult<long> ExecuteQuery2(string query)
        {
            string K_QUERY = query;
            return E.Lead.ExecuteStoreQuery<long>(K_QUERY);            
        }
        public bool HasDuplicateRuleColumns(int ruleID)
        {
            var result = Engine.DuplicateRecordActions.Get(ruleID).FieldTagsRulesColumns;
            return result.Count > 0 ? true : false;

        }
        //YA[June 19, 2013] ----Start--------
        //These functionality could be put in the lead actions folder with new class but I placed it here so that all duplicate management related 
        //code reside in a single class.
        public void AddPotentialDuplicates(int duplicateRuleID, long incomingLeadId, long[] existingLeadIds)
        {
            foreach (long leadId in existingLeadIds)
	        {
		        Models.DuplicatesView nDuplicate = new DuplicatesView();
                nDuplicate.DuplicateRuleId = duplicateRuleID;
                nDuplicate.IncomingLeadId = incomingLeadId;
                nDuplicate.ExistingLeadId = leadId;
                E.leadEntities.DuplicatesViews.AddObject(nDuplicate);
	        }
            E.Save();
        }
        public void DeletePotentialDuplicatesByRuleId(int duplicateRuleID)
        {
            var T=  E.leadEntities.DuplicatesViews.Where(x => x.DuplicateRuleId == duplicateRuleID);
            foreach(var item in T)
            {
                E.leadEntities.DuplicatesViews.DeleteObject(item);    
            }
            E.Save();
        }
        public void DeletePotentialDuplicatesByIncomingLeadId(long incomingLeadID)
        {
            var T = E.leadEntities.DuplicatesViews.Where(x => x.IncomingLeadId == incomingLeadID);
            foreach (var item in T)
            {
                E.leadEntities.DuplicatesViews.DeleteObject(item);
            }
            E.Save();
        }
        public void DeletePotentialDuplicatesByExistingLeadId(long ExistingLeadID)
        {
            var T = E.leadEntities.DuplicatesViews.Where(x => x.ExistingLeadId== ExistingLeadID);
            foreach (var item in T)
            {
                E.leadEntities.DuplicatesViews.DeleteObject(item);
            }
            E.Save();
        }
        //YA[June 19, 2013] ----End--------
    }

}


