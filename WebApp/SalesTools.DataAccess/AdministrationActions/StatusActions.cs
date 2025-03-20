using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using SalesTool.DataAccess.Models;
using DBG = System.Diagnostics.Debug;


namespace SalesTool.DataAccess.AdministrationActions
{
    public class StatusActions : BaseActions
    {
        //private const int K_INCREMENT = 30;
        internal StatusActions(DBEngine rengine) : base(rengine) { }

        public SalesTool.DataAccess.Models.StatusEmail GetAssignedEmailDetails(int statusId, int emlID)
        {
            return E.Admin.StatusEmails.Where(x => x.StatusId == statusId && x.EmailtemplateId == emlID).FirstOrDefault();
        }

        public bool Exists(int statusId)
        {
            return E.Admin.Status.Count(x => x.Id == statusId) > 0;
        }

        public void AssignStatusChange2Actions(int recordId, int[] actionIDs, int iNew, string by)
        {
            IQueryable<Models.StatusAction> SA = E.Admin.StatusActions.Where(x => x.StatusId == recordId && actionIDs.Contains(x.ActionId));

            foreach (Models.StatusAction item in SA)
            {
                item.Changed.By = by;
                item.Changed.On = DateTime.Now;
                if (iNew == -1)
                    item.NewStatusId = null;
                else
                    item.NewStatusId = iNew;
            }
            E.Save();
        }
        public void AssignTriggerChange2Emails(int recordId, int[] emailIDs, byte iTriggerType, string by)
        {
            foreach (int emtId in emailIDs)
            {
                Models.StatusEmail SE = E.Admin.StatusEmails.Where(x => x.StatusId == recordId && x.EmailtemplateId == emtId).FirstOrDefault();
                if (SE == null)
                {
                    int seId = E.Admin.StatusEmails.Count() > 0 ? E.Admin.StatusEmails.Max(x => x.Id) + 1 : 1;
                    SE = new Models.StatusEmail { Id = seId, Added = { On = DateTime.Now, By = by }, StatusId = recordId, EmailtemplateId = emtId, TriggerType = iTriggerType };
                    E.Admin.StatusEmails.AddObject(SE);
                }
                else
                {
                    SE.TriggerType = iTriggerType;
                }
            }
            E.Save();
        }

        //public int GetChangedStatusId(

        public IQueryable<Models.Status> All
        {
            get
            {
                return E.Admin.Status.Where(x => x.Level == 0).OrderByDescending(x => x.Priority);
            }
        }

        public IQueryable<Models.Status> AllSubStatus1
        {
            get
            {
                return E.Admin.Status.Where(x => x.Level == 1).OrderByDescending(x => x.Priority);
            }
        }
        public IQueryable<Models.Status> AllSubStatus2
        {
            get
            {
                return E.Admin.Status.Where(x => x.Level == 2).OrderByDescending(x => x.Priority);
            }
        }

        public IQueryable<NameIntValueLookup> StatusLookups
        {
            get
            {

                return E.Admin.Status.Where(x => x.Level == 0)
                    .OrderBy(p => p.Title)
                    .Select(s => new NameIntValueLookup() {Name = s.Title, Value = s.Id});
                //.ToList();
            }
        }
        public IQueryable<NameIntValueLookup> SubStatusLookups
        {
            get
            {

                return E.Admin.Status.Where(x => x.Level == 1)
                    .OrderBy(p => p.Title)
                    .Select(s => new NameIntValueLookup() {Name = s.Title, Value = s.Id});
                //.ToList();
            }
        }
        //WM - 17.05.2013 

        public IQueryable<Models.Status> GetAllStatusesBySkillGroups(int level, IEnumerable<Models.SkillGroup> currentUserSkillGroups)
        {
            var statuses = E.Admin.Status.Where(st => st.Level == level);
            var skillGroupsIDs = currentUserSkillGroups.Select(x => x.Id);

            var s =
             from st in statuses
             from sk in st.SkillGroups
             where skillGroupsIDs.Contains(sk.Id)
             orderby st.Priority descending
             select st;

            return s;//.OrderByDescending(x => x.Priority);
        }



        public Models.Status Get(int id)
        {
            return E.Admin.Status.Where(x => x.Id == id).FirstOrDefault();
        }
        public Models.Status Add(string title, bool progress, string by, byte level = 0)
        {
            Models.Status S = new Models.Status();

            int id = ((E.Admin.Status.Count() > 0) ? E.Admin.Status.Max(x => x.Id) : 0) + 1;

            S.Id = id;
            S.Title = title;
            S.Progress = progress;
            S.Priority = 0;
            S.Level = level;
            S.Added.By = by;
            S.Added.On = DateTime.Now;

            E.Admin.AddToStatus(S);
            E.Save();
            Move(S.Id, -1);

            return Get(id);
        }
        public void Change(Models.Status status, string by)
        {
            status.Changed.By = by;
            status.Changed.On = DateTime.Now;

            E.Save();

            Move(status.Id, status.Priority ?? int.MaxValue);
        }
        public void Delete(int Id)
        {
            var D = Get(Id);
            int iLevel = D.Level ?? 0;
            if (D != null)
            {
                DeleteAll(Id);
                E.Admin.Status.DeleteObject(D);
                E.Save();

                var X = E.Admin.Status.Where(x => x.Level == iLevel).FirstOrDefault();
                if (X != null)
                    Move(X.Id, X.Priority ?? int.MaxValue);
            }
        }

        public int GetChangedStatusId(int statusId, int actionId)
        {
            return E.Admin.StatusActions.Where(x => x.StatusId == statusId && x.ActionId == actionId).FirstOrDefault().NewStatusId ?? 0;
        }
        public Models.Status GetChangedStatus(int statusId, int actionId)
        {
            return E.Admin.StatusActions.Where(x => x.StatusId == statusId && x.ActionId == actionId).FirstOrDefault().statusChanged;
        }

        public IEnumerable<Models.EmailTemplate> GetEmailTemplates(int Id, bool bAvailable = true)
        {
            IEnumerable<Models.EmailTemplate> Ans = (bAvailable) ?
                E.ManageEmailTemplateActions.GetAll().Where(x => !x.status_emails.Any(y => y.StatusId == Id)).AsEnumerable() :
            E.ManageEmailTemplateActions.GetAll().Join(
                    E.Admin.StatusEmails.Where(x => x.StatusId == Id),
                    a => a.Id,
                    b => b.EmailtemplateId,
                    (a, b) => new { EmailTemplate = a, Status = b.status })
                    .Select(x => x.EmailTemplate)
                    .AsEnumerable();

            return Ans;
        }

        public IEnumerable<Models.EmailTemplate> GetEnabledEmailTemplates(int Id, bool bAvailable = true)
        {
            IEnumerable<Models.EmailTemplate> Ans = (bAvailable) ?
                E.ManageEmailTemplateActions.GetAll().Where(x => !x.status_emails.Any(y => y.StatusId == Id) && x.Enabled == true).AsEnumerable() :
            E.ManageEmailTemplateActions.GetAll().Join(
                    E.Admin.StatusEmails.Where(x => x.StatusId == Id),
                    a => a.Id,
                    b => b.EmailtemplateId,
                    (a, b) => new { EmailTemplate = a, Status = b.status }).Where(x => x.EmailTemplate.Enabled == true)
                    .Select(x => x.EmailTemplate)
                    .AsEnumerable();

            return Ans;
        }

        public void AssignEmailTemplates(int id, int[] emailTemplates, int TriggerType, string by)
        {
            Stack<int> stkAdd = new Stack<int>(), stkRemove = new Stack<int>();
            IQueryable<Models.StatusEmail> SA = E.Admin.StatusEmails.Where(x => x.StatusId == id).AsQueryable();

            foreach (int item in emailTemplates)  //SZ [Feb 6, 2013] find al emements that are not present
                if (!SA.Any(x => x.EmailtemplateId == item))
                    stkAdd.Push(item); //note the emailtemplate id

            foreach (Models.StatusEmail item in SA) //SZ [Feb 6, 2013] find all emements shoudl be removed
                if (!emailTemplates.Any(x => x == item.EmailtemplateId))
                    stkRemove.Push(item.Id); //note the StatusEmail Id

            while (stkRemove.Count > 0)
            {
                int i = stkRemove.Pop();
                E.Admin.StatusEmails.DeleteObject(E.Admin.StatusEmails.Where(x => x.Id == i).FirstOrDefault());
            }

            int newId = (E.Admin.StatusEmails.Count() > 0 ? E.Admin.StatusEmails.Max(x => x.Id) + 1 : 0) + 1 + stkAdd.Count;
            while (stkAdd.Count > 0)
                E.Admin.StatusEmails.AddObject(new Models.StatusEmail
                {
                    Id = --newId,
                    StatusId = id,
                    EmailtemplateId = stkAdd.Pop(),
                    Added = { By = by, On = DateTime.Now },
                    TriggerType = Convert.ToByte(TriggerType)
                });

            E.Save();
        }

        public Models.StatusEmail GetStatusEmails(int statusID, int emailTemplateID)
        {

            return E.Admin.StatusEmails.Where(x => x.StatusId == statusID && x.EmailtemplateId == emailTemplateID).FirstOrDefault();
        }

        //YA[Nov 21, 2013]
        public IQueryable<Models.EmailTemplate> GetManualEmailTemplatesForStatus(int statusId, int subStatuID = 0)
        {
            //Get the email templates attached to the status with having email trigger type: Manual =2 and Both = 3
            IQueryable<Models.EmailTemplate> Ans = null;
            if (subStatuID > 0)
            {
                Ans = E.ManageEmailTemplateActions.GetAll().Join(E.Admin.StatusEmails,
                                        e => e.Id,
                                        ae => ae.EmailtemplateId,
                                        (e, ae) => new { EmailTemplates = e, StatusEmail = ae }).Where(x => (x.StatusEmail.StatusId == statusId || x.StatusEmail.StatusId == subStatuID) && (x.StatusEmail.TriggerType == 2 || x.StatusEmail.TriggerType == 3) && x.EmailTemplates.Enabled == true && x.EmailTemplates.IsDeleted != true).Select(x => x.EmailTemplates).AsQueryable();
            }
            else
            {
                Ans = E.ManageEmailTemplateActions.GetAll().Join(E.Admin.StatusEmails,
                                        e => e.Id,
                                        ae => ae.EmailtemplateId,
                                        (e, ae) => new { EmailTemplates = e, StatusEmail = ae }).Where(x => x.StatusEmail.StatusId == statusId && (x.StatusEmail.TriggerType == 2 || x.StatusEmail.TriggerType == 3) && x.EmailTemplates.Enabled == true && x.EmailTemplates.IsDeleted != true).Select(x => x.EmailTemplates).AsQueryable();
            }
            return Ans;
        }

        //YA[Nov 21, 2013]
        public IQueryable<Models.EmailTemplate> GetAutoEmailTemplatesForStatus(int statusId, int subStatuID = 0)
        {
            //Get the email templates attached to the status with having email trigger type: Auto =1 and Both = 3
            IQueryable<Models.EmailTemplate> Ans = null;
            if (subStatuID > 0)
            {
                Ans = E.ManageEmailTemplateActions.GetAll().Join(E.Admin.StatusEmails,
                                        e => e.Id,
                                        ae => ae.EmailtemplateId,
                                        (e, ae) => new { EmailTemplates = e, StatusEmail = ae }).Where(x => (x.StatusEmail.StatusId == statusId || x.StatusEmail.StatusId == subStatuID) && x.EmailTemplates.Enabled == true && (x.StatusEmail.TriggerType == 1 || x.StatusEmail.TriggerType == 3) && x.EmailTemplates.IsDeleted != true).Select(x => x.EmailTemplates).Distinct().AsQueryable();
            }
            else
            {
                Ans = E.ManageEmailTemplateActions.GetAll().Join(E.Admin.StatusEmails,
                                        e => e.Id,
                                        ae => ae.EmailtemplateId,
                                        (e, ae) => new { EmailTemplates = e, StatusEmail = ae }).Where(x => x.StatusEmail.StatusId == statusId && x.EmailTemplates.Enabled == true && (x.StatusEmail.TriggerType == 1 || x.StatusEmail.TriggerType == 3) && x.EmailTemplates.IsDeleted != true).Select(x => x.EmailTemplates).Distinct().AsQueryable();
            }
            return Ans;
        }


        //YA[July 26, 2013] 
        public Models.StatusPost GetStatusPosts(int statusID, int postTemplateID)
        {
            return E.Admin.StatusPosts.Where(x => x.StatusId == statusID && x.PostTemplateId == postTemplateID).FirstOrDefault();
        }

        public IEnumerable<Models.Post> GetPostTemplates(int Id, bool bAvailable = true)
        {
            IEnumerable<Models.Post> Ans = (bAvailable) ?
                E.ManagePostsActions.GetAll().Where(x => !x.status_posts.Any(y => y.StatusId == Id)).AsEnumerable() :
                E.ManagePostsActions.GetAll().Join(
                E.Admin.StatusPosts.Where(x => x.StatusId == Id),
                A => A.Id,
                B => B.PostTemplateId,
                (A, B) => new { PostTemplate = A, Status = B }
                )
                    .Select(x => x.PostTemplate)
                    .AsEnumerable();

            return Ans;
        }

        public IEnumerable<Models.Post> GetEnabledPostTemplates(int Id, bool bAvailable = true)
        {
            IEnumerable<Models.Post> Ans = (bAvailable) ?
                E.ManagePostsActions.GetAll().Where(x => !x.status_posts.Any(y => y.StatusId == Id) && x.Enabled == true).AsEnumerable() :
                E.ManagePostsActions.GetAll().Join(
                E.Admin.StatusPosts.Where(x => x.StatusId == Id),
                A => A.Id,
                B => B.PostTemplateId,
                (A, B) => new { PostTemplate = A, Status = B }
                ).Where(x => x.PostTemplate.Enabled == true)
                    .Select(x => x.PostTemplate)
                    .AsEnumerable();

            return Ans;
        }

        public void AssignPostTemplates(int id, int[] postTemplates, string by)
        {
            Stack<int> stkAdd = new Stack<int>(), stkRemove = new Stack<int>();
            IQueryable<Models.StatusPost> SP = E.Admin.StatusPosts.Where(x => x.StatusId == id).AsQueryable();

            foreach (int item in postTemplates) //SZ [Feb 6, 2013] find al emements that are not present
                if (!SP.Any(x => x.PostTemplateId == item))
                    stkAdd.Push(item); //note the Post id

            foreach (Models.StatusPost item in SP) //SZ [Feb 6, 2013] find all emements shoudl be removed
                if (!postTemplates.Any(x => x == item.PostTemplateId))
                    stkRemove.Push(item.Id); //note the StatusPost Id

            while (stkRemove.Count > 0)
            {
                int i = stkRemove.Pop();
                E.Admin.StatusPosts.DeleteObject(E.Admin.StatusPosts.Where(x => x.Id == i).FirstOrDefault());
            }


            int newId = (E.Admin.StatusPosts.Count() > 0 ? E.Admin.StatusPosts.Max(x => x.Id) + 1 : 0) + 1 + stkAdd.Count;
            while (stkAdd.Count > 0)
                E.Admin.StatusPosts.AddObject(new Models.StatusPost
                {
                    Id = --newId,
                    StatusId = id,
                    PostTemplateId = stkAdd.Pop(),
                    Added = { By = by, On = DateTime.Now }
                });

            E.Save();
        }

        public IEnumerable<Models.Action> GetActionTemplates(int Id, bool bAvailable = true)
        {
            IEnumerable<Models.Action> Ans = (bAvailable) ?
                E.LocalActions.All.Where(x => !x.status_actions.Any(y => y.StatusId == Id)).AsEnumerable() :
                E.LocalActions.All.Join(E.Admin.StatusActions.Where(x => x.StatusId == Id),
                    A => A.Id,
                    B => B.ActionId,
                    (A, B) => new { Action = A, Status = B })
                    .Select(x => x.Action)
                    .AsEnumerable();


            // Zil-e-Ilahi ke problems: (Parveen Shakir)
            // Raj Paat karne waloon ki jaan
            // hameesha hatheli per rehti hai
            // becharoon ke Masail ajeeb hotey hain
            // kabi is baj guzar riyasat ki shorida sari
            // kabi us zair-e-nageen soobay ki na-farmani
            // dushman jald he khul jatay hain
            // uljhao to paon choomney walon se parta ha
            // jo un ke paon chattay rehtey hain
            // magar jab samandar char ke ata hai
            // saray darbari ghaib ho jatay hain
            // aur zil-e-Ilahi apney
            // paon dhundtey reh jatay hain


            return Ans;
        }

        public Models.Status GetActionChangedStatus(int actionId, int statusid)
        {
            return E.Admin.StatusActions.Where(x => x.ActionId == actionId && x.StatusId == statusid).FirstOrDefault().statusChanged;
        }


        public void AssignActions(int id, int[] actions, byte TriggerType, string by)
        {
            Stack<int> stkAdd = new Stack<int>(), stkRemove = new Stack<int>();
            IQueryable<Models.StatusAction> SA = E.Admin.StatusActions.Where(x => x.StatusId == id).AsQueryable();

            foreach (int item in actions) //SZ [Feb 6, 2013] find al emements that are not present
                if (!SA.Any(x => x.ActionId == item))
                    stkAdd.Push(item); //note the action id

            foreach (Models.StatusAction item in SA) //SZ [Feb 6, 2013] find all emements shoudl be removed
                if (!actions.Any(x => x == item.ActionId))
                    stkRemove.Push(item.Id); //note the StatusAction Id

            while (stkRemove.Count > 0)
            {
                int i = stkRemove.Pop();
                E.Admin.StatusActions.DeleteObject(E.Admin.StatusActions.Where(x => x.Id == i).FirstOrDefault());
            }


            int newId = (E.Admin.StatusActions.Count() > 0 ? E.Admin.StatusActions.Max(x => x.Id) + 1 : 0) + 1 + stkAdd.Count;
            while (stkAdd.Count > 0)
                E.Admin.StatusActions.AddObject(new Models.StatusAction
                {
                    Id = --newId,
                    StatusId = id,
                    ActionId = stkAdd.Pop(),
                    Added = { By = by, On = DateTime.Now }
                    //, Trigger = TriggerType // does not exist in DB
                });

            E.Save();
        }

        public IEnumerable<Models.Status> GetSubStatuses(int Id, bool bAvailable = true)
        {
            IEnumerable<Models.Status> Ans = null;
            byte Level = Get(Id).Level ?? 0;

            if (bAvailable)
            {
                var tmp = E.Admin.Status_Substatus.Where(x => x.ParentId == Id).Select(x => x.Children);
                Ans = E.Admin.Status.Where(x => x.Level == Level + 1 && !tmp.Any(y => y.Id == x.Id)).OrderByDescending(x => x.Priority);
            }
            else
            {
                Ans = E.Admin.Status_Substatus.Where(x => x.ParentId == Id).Select(x => x.Children).OrderByDescending(x => x.Priority);
            }

            return Ans;
        }
        public void AssignSubStatuses(int recordId, int[] selectedItemIDs, string by, string color)
        {
            List<Models.Status_Substatus> list = null;

            list = E.Admin.Status_Substatus.Where(x => x.ParentId == recordId).ToList();
            foreach (var I in list)
                E.Admin.Status_Substatus.DeleteObject(I);

            foreach (int id in selectedItemIDs)
            {
                Models.Status_Substatus tmp = new Models.Status_Substatus
                {
                    ParentId = recordId,
                    ChildId = id,
                    Color = color,
                    Added = { By = by, On = DateTime.Now }
                };
                E.Admin.Status_Substatus.AddObject(tmp);
            }
            E.Save();
        }

        // SZ [Feb 6, 2013] adjust the priority of the status elements. 
        // they can be moved at random locations or they can be moved 1 position up or down
        // The movement takes place in the database only. 
        public void Move(int Id, int priority)
        {

            // SZ [Feb 7, 2013] No need to pass the level, the stored procedure will itself identify the level of element 
            // and sorts only the elements at teh same level.

            //Models.Status X = Get(Id);
            //int iLevel = (X!= null) ? X.Level ?? 0 : 0;

            E.adminEntities.ChangeStatusPriority(Id, priority);
        }
        public void Up(int Id)
        {
            Move(Id, (Get(Id).Priority ?? default(int)) + 1);
        }
        public void Down(int Id)
        {
            Move(Id, (Get(Id).Priority ?? default(int)) - 1);
        }

        private void DeleteAll(int Id)
        {
            List<int> tmpList = new List<int>();

            tmpList = E.Admin.StatusActions.Where(x => x.StatusId == Id).Select(x => x.Id).ToList();
            foreach (var I in tmpList)
                E.Admin.StatusActions.DeleteObject(E.Admin.StatusActions.Where(x => x.Id == I).FirstOrDefault());


            tmpList = E.Admin.StatusEmails.Where(x => x.StatusId == Id).Select(x => x.Id).ToList();
            foreach (var I in tmpList)
                E.Admin.StatusEmails.DeleteObject(E.Admin.StatusEmails.Where(x => x.Id == I).FirstOrDefault());


            tmpList = E.Admin.StatusPosts.Where(x => x.StatusId == Id).Select(x => x.Id).ToList();
            foreach (var I in tmpList)
                E.Admin.StatusPosts.DeleteObject(E.Admin.StatusPosts.Where(x => x.Id == I).FirstOrDefault());

            tmpList = E.Admin.Status_Substatus.Where(x => x.ParentId == Id).Select(x => x.Id).ToList();
            foreach (var I in tmpList)
                E.Admin.Status_Substatus.DeleteObject(E.Admin.Status_Substatus.Where(x => x.Id == I).FirstOrDefault());

            E.Save();
        }

        //SZ [Feb 27, 2013] added to retrieve the related sub statuses or parent statuses
        IQueryable<Models.Status> GetChildren(int id)
        {
            return GetSubStatuses(id, false).AsQueryable();
        }
        IQueryable<Models.Status> GetParents(int id)
        {
            return E.Admin.Status_Substatus.Where(x => x.ParentId == id).Select(x => x.Parents).AsQueryable();
        }
        bool HasChildren(int id)
        {
            return E.Admin.Status_Substatus.Where(x => x.ParentId == id).Count() > 0;
        }

        //SZ [Feb 28, 2013] field_tag functionality added
        public IEnumerable<Models.TagFields> GetTagFields(int id, bool bAvailable = false)
        {
            IEnumerable<Models.TagFields> oAns = null;

            if (bAvailable)
            {
                List<int> tmp = E.Admin.StatusFieldtags.Where(x => x.StatusId == id).Select(x => x.FieldTagId).ToList();
                oAns = E.Admin.TagFields.Where(x => !tmp.Any(y => y == x.Id) && x.IsWorkflowIncluded).AsEnumerable();
            }
            else
                oAns = E.Admin.StatusFieldtags.Where(x => x.StatusId == id).Select(x => x.field_tags).AsEnumerable();

            return oAns;
        }
        public void AssignTagFields(int id, int[] tagFieldIds)
        {
            //SZ [Feb 28, 2013] Added assert so that only sub statusII can be used. It is not present in release version 
            DBG.Assert(Get(id).Level == 2);
            foreach (var item in E.Admin.StatusFieldtags.Where(x => x.StatusId == id).ToList())
                E.Admin.StatusFieldtags.DeleteObject(item);

            foreach (int fieldtagid in tagFieldIds)
                E.Admin.StatusFieldtags.AddObject(new Models.StatusFieldtag { StatusId = id, FieldTagId = fieldtagid });

            E.Save();
        }
        public bool HasRequiredFields(int statusID)
        {
            return E.Admin.StatusFieldtags.Where(x => x.StatusId == statusID).AsEnumerable().Count() > 0;
        }


        //SZ [Mar 20, 2013] Retrieves the next sub status OR NULL based on the given current substatus priority. 
        // that is, it picks a sub status with priority lower than the priority of the current sub status
        // function can be used in a loop with While(NextSubStatus(...) !=null){}
        public Models.Status NextSubStatus(int statusID, int substatusID = -1)
        {
            var sub = Get(substatusID);
            int targetPriority = sub != null ? sub.Priority ?? int.MinValue : int.MaxValue;

            return GetSubStatuses(statusID, false).Where(x => x.Priority < targetPriority).FirstOrDefault();
        }

        /// <summary>
        /// Created By Syrus R
        /// </summary>
        /// <returns></returns>
        public IQueryable<Models.Status> GetAllStatuses()
        {
            var statuses = E.Admin.Status.Where(st => st.Level == 0);
            var s = from st in statuses
                    orderby st.Title ascending
                    select st;
            return s;
        }

        public IEnumerable<Models.Status> GetAllSubStatuses()
        {
            var statuses = E.Admin.Status.Where(st => st.Level == 0);
            var subStatuses = E.Admin.Status_Substatus;

            var s = from st in E.Admin.Status
                    join sub in E.Admin.Status_Substatus
                    on st.Id equals sub.ParentId
                    join st1 in E.Admin.Status
                    on sub.ChildId equals st1.Id
                    where st.Level.Value.Equals(0)
                    orderby st.Title, st1.Title ascending
                    select st1;
            return s;
        }

        public int? GetSubStatusIdIfUnderStatus(int statusKey, int? subStatusKey)
        {
            var res= GetSubStatuses(statusKey, false).IfNotNull(p => p.Any(a => a.Id == subStatusKey)) ? subStatusKey : default(int?);
            return res;
        }
    }
}
