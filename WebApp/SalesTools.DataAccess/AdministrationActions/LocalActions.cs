using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess.AdministrationActions
{
    public class LocalActions: BaseActions
    {
        internal LocalActions(DBEngine reng) : base(reng) { }

        public SalesTool.DataAccess.Models.Action Get(int id)
        {
            return E.Admin.Actions1.Where(x => x.Id == id).FirstOrDefault();
        }

        public Models.Action Add(string title, bool requiresComment, bool hasContactAttempted, bool hasContact, bool hasCalender, bool lockSubstatus, string By)
        {
            int id = ((All.Count() > 0) ? All.Max(x => x.Id): 0)+1;
            
            Models.Action action = new Models.Action();
            action.Id = id;
            action.Title = title;
            action.HasComment = requiresComment;
            action.HasAttempt = hasContactAttempted;
            action.HasContact = hasContact;
            action.HasCalender = hasCalender;
            action.LockSubstatus = lockSubstatus;
            
            action.Added.By = By;
            action.Added.On = DateTime.Now;

            E.Admin.AddToActions1(action);
            E.Save();
            return Get(id);
        }
        public void Change(Models.Action action, string by)
        {
            // SZ [Jan 31, 2013] The key is to get the stuff, get it chnaged thru properties, 
            // no magic here in this function. EF handles is automatically
            action.Changed.By = by;
            action.Changed.On = DateTime.Now;

            E.Save();
        }
        public bool CanDelete(int actionID)
        {
            //var T = E.Lead.Leads.Where(x => !x.IsDeleted??false && x.ActionId == actionID); 
            var T = E.Lead.Leads.Where(x => x.IsDeleted == false && x.ActionId == actionID); 
            return T.Count() == 0;
        }
        public void Delete(int actionID)
        {
            //SZ [Feb 27, 2013] Remove the dependencies except for leads
            List<int> ids = E.Admin.ActionEmails.Where(x=>x.ActionId==actionID).Select(x=>x.Id).ToList();
            foreach (int tid in ids)
                E.Admin.ActionEmails.DeleteObject(E.Admin.ActionEmails.Where(x => x.Id == tid).FirstOrDefault());
            ids.Clear();

            ids = E.Admin.ActionPosts.Where(x => x.ActionId == actionID).Select(x => x.Id).ToList();
            foreach (int tid in ids)
                E.Admin.ActionPosts.DeleteObject(E.Admin.ActionPosts.Where(x => x.Id == tid).FirstOrDefault());
            ids.Clear();

            ids = E.Admin.StatusActions.Where(x => x.ActionId == actionID).Select(x => x.Id).ToList();
            foreach (int tid in ids)
                E.Admin.StatusActions.DeleteObject(E.Admin.StatusActions.Where(x => x.Id == tid).FirstOrDefault());
            ids.Clear();

            var A = Get(actionID);
            if (A != null)
                E.Admin.Actions1.DeleteObject(A);
            E.Save();
        }
        public IQueryable<Models.Action> All
        {
            get
            {
                return E.adminEntities.Actions1.OrderByDescending(x =>
                    x.Added.On);
            }
        }

        public IQueryable<Models.EmailTemplate> GetEmailTemplates(int actionId, bool bAvailable = true)
        {
            IQueryable<Models.EmailTemplate> Ans = null;

            if(bAvailable)
                Ans = E.ManageEmailTemplateActions.GetAll().Where(x => !x.action_emails.Any(y => y.ActionId == actionId));
            else
            {
                Ans = E.ManageEmailTemplateActions.GetAll().Join(E.Admin.ActionEmails,
                                    e => e.Id,
                                    ae => ae.EmailTemplateId,
                                    (e, ae) => new { EmailTemplates = e, ActionEmail = ae }).Where(x => x.ActionEmail.ActionId == actionId).Select(x => x.EmailTemplates);
            }

            return Ans;
        }
        //YA[Nov 01, 2013]
        public IQueryable<Models.EmailTemplate> GetManualEmailTemplates(int actionId)
        {
            //Get the email templates attached to the Actions with having email trigger type: Manual =2 and Both = 3
            IQueryable<Models.EmailTemplate> Ans = null;                       
            Ans = E.ManageEmailTemplateActions.GetAll().Join(E.Admin.ActionEmails,
                                    e => e.Id,
                                    ae => ae.EmailTemplateId,
                                    (e, ae) => new { EmailTemplates = e, ActionEmail = ae }).Where(x => x.ActionEmail.ActionId == actionId && (x.ActionEmail.TriggerType == 2 || x.ActionEmail.TriggerType == 3) && x.EmailTemplates.Enabled ==true && x.EmailTemplates.IsDeleted != true ).Select(x => x.EmailTemplates).AsQueryable();           

            return Ans;
        }

        //YA[Nov 01, 2013]
        public IQueryable<Models.EmailTemplate> GetAutoEmailTemplates(int actionId)
        {
            //Get the email templates attached to the Actions with having email trigger type: Auto =1 and Both = 3
            IQueryable<Models.EmailTemplate> Ans = null;
            Ans = E.ManageEmailTemplateActions.GetAll().Join(E.Admin.ActionEmails,
                                    e => e.Id,
                                    ae => ae.EmailTemplateId,
                                    (e, ae) => new { EmailTemplates = e, ActionEmail = ae }).Where(x => x.ActionEmail.ActionId == actionId && x.EmailTemplates.Enabled == true && (x.ActionEmail.TriggerType == 1 || x.ActionEmail.TriggerType == 3) && x.EmailTemplates.IsDeleted != true).Select(x => x.EmailTemplates).AsQueryable();

            return Ans;
        }

        public void AssignEmailTemplates(int actionId, int[] idsEmailTemplate, int TriggerType = 0 , string by = "" )
        {
            //var deletedIds = E.Admin.ActionEmails.Where(x => x.ActionId == actionId);
            //foreach(Models.ActionEmail item in deletedIds){
            //    E.Admin.ActionEmails.DeleteObject(item);
            //}
            //foreach (int id in ids)
            //{
            //    Models.ActionEmail ae = new Models.ActionEmail {Id = (E.Admin.ActionEmails.Count()>0? E.Admin.ActionEmails.Max(x=>x.Id): 0)+1, 
            //        ActionId = actionId, EmailTemplateId = id, Added = { By = by, On = DateTime.Now} , TriggerType = Convert.ToByte(TriggerType) };
            //    E.Admin.ActionEmails.AddObject(ae);
            //    // SZ [Feb 5, 2013] Save inside loop is expensive but is neessary for the Id generation. 
            //    // if we move this save outside loop then the id generation statement will always return the same value.
            //    // if we save the id and keep incrementing it then it might have a chnace of conflict with soem other user
            //    // using this fucntionality at the same time.
            //    E.Save();
            //}
            Stack<int> stkAdd = new Stack<int>(), stkRemove = new Stack<int>();
            IQueryable<Models.ActionEmail> SA = E.Admin.ActionEmails.Where(x => x.ActionId == actionId).AsQueryable();

            foreach (int item in idsEmailTemplate)  //SZ [Feb 6, 2013] find all emements that are not present
                if (!SA.Any(x => x.EmailTemplateId== item))
                    stkAdd.Push(item); //note the emailtemplate id

            foreach (Models.ActionEmail item in SA) //SZ [Feb 6, 2013] find all emements shoudl be removed
                if (!idsEmailTemplate.Any(x => x == item.EmailTemplateId))
                    stkRemove.Push(item.Id); //note the ActionEmail Id

            while (stkRemove.Count > 0)
            {
                int i = stkRemove.Pop();
                E.Admin.ActionEmails.DeleteObject(E.Admin.ActionEmails.Where(x => x.Id == i).FirstOrDefault());
            }

            int newId = (E.Admin.ActionEmails.Count() > 0 ? E.Admin.ActionEmails.Max(x => x.Id) + 1 : 0) + 1 + stkAdd.Count;
            while (stkAdd.Count > 0)
                E.Admin.ActionEmails.AddObject(new Models.ActionEmail
                {
                    Id = --newId,
                    ActionId = actionId,
                    EmailTemplateId = stkAdd.Pop(),
                    Added = { By = by, On = DateTime.Now },
                    TriggerType = Convert.ToByte(TriggerType)
                });

            E.Save();
            
        }

        //YA[Oct 28, 2013]
        public void AssignTriggerChange2ActionEmails(int recordId, int[] emailIDs, byte iTriggerType, string by)
        {
            foreach (int emtId in emailIDs)
            {
                Models.ActionEmail SE = E.Admin.ActionEmails.Where(x => x.ActionId == recordId && x.EmailTemplateId == emtId).FirstOrDefault();
                if (SE == null)
                {
                    int seId = E.Admin.ActionEmails.Count() > 0 ? E.Admin.ActionEmails.Max(x => x.Id) + 1 : 1;
                    SE = new Models.ActionEmail { Id = seId, Added = { On = DateTime.Now, By = by }, ActionId = recordId, EmailTemplateId = emtId, TriggerType = iTriggerType };
                    E.Admin.ActionEmails.AddObject(SE);
                }
                else
                {
                    SE.TriggerType = iTriggerType;
                }
            }
            E.Save();
        }
        //YA[Oct 28, 2013]
        public SalesTool.DataAccess.Models.ActionEmail GetAssignedActionEmailDetails(int ActionId, int emlID)
        {
            return E.Admin.ActionEmails.Where(x => x.ActionId == ActionId && x.EmailTemplateId == emlID).FirstOrDefault();
        }


        public IQueryable<Models.Post> GetPostTemplates(int actionId, bool bAvailable = true)
        {
            IQueryable<Models.Post> Ans = null;

            if(bAvailable)
                Ans =E.ManagePostsActions.GetAll().Where(x=> !x.action_posts.Any(y=>y.ActionId==actionId));
                
            else
                Ans = E.ManagePostsActions.GetAll().Join(E.Admin.ActionPosts, 
                        p=>p.Id, 
                        ap=>ap.PostId, 
                        (p, ap)=> new {Posts=p, ActionPosts=ap}).Where(x=>x.ActionPosts.ActionId == actionId).Select(x=>x.Posts);




            return Ans;
        }

        public IQueryable<Models.Post> GetEnabledPostTemplates(int actionId, bool bAvailable = true)
        {
            IQueryable<Models.Post> Ans = null;

            if(bAvailable)
                Ans =E.ManagePostsActions.GetAll().Where(x=> !x.action_posts.Any(y=>y.ActionId==actionId) && x .Enabled == true);
                
            else
                Ans = E.ManagePostsActions.GetAll().Join(E.Admin.ActionPosts, 
                        p=>p.Id, 
                        ap=>ap.PostId, 
                        (p, ap)=> new {Posts=p, ActionPosts=ap}).Where(x=>x.ActionPosts.ActionId == actionId && x.Posts.Enabled == true).Select(x=>x.Posts);




            return Ans;
        }

        public void AssignPostTemplates(int id, int[] ids, string fullName)
        {
            var deletedIds = E.Admin.ActionPosts.Where(x => x.ActionId == id);
            foreach (var item in deletedIds)
                E.Admin.ActionPosts.DeleteObject(item);
            //E.Save();

            foreach (var i in ids)
            {
                Models.ActionPost item = new Models.ActionPost { Id= (E.Admin.ActionPosts.Count() > 0 ? E.Admin.ActionPosts.Max(x => x.Id) : 0)+1,
                    ActionId = id, PostId = i, Added = { By = fullName, On = DateTime.Now } };
                E.Admin.ActionPosts.AddObject(item);
                // SZ [Feb 5, 2013] Save inside loop is expensive but is neessary for the Id generation. 
                // if we move this save outside loop then the id generation statement will always return the same value.
                // if we save the id and keep incrementing it then it might have a chnace of conflict with soem other user
                // using this fucntionality at the same time.
                E.Save();
            }
            //E.Save();
        }
    }
}
