using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;
using Sql = System.Data.SqlClient;

namespace SalesTool.DataAccess
{
    public class EmailQueueActions : BaseActions
    {
        public event EventHandler<ItemEventArgs<long>> EmailQueueAdded;

        protected virtual void OnEmailQueueAdded(long id)
        {
            EventHandler<ItemEventArgs<long>> handler = EmailQueueAdded;
            if (handler != null) handler(this, new ItemEventArgs<long>(id));
        }

        internal EmailQueueActions(DBEngine engine) : base(engine) { }

        public IQueryable<EmailQueue> All
        {
            get
            {
                return E.leadEntities.EmailQueues.Where(x => x.Status != 0).AsQueryable();
            }
        }
        public IQueryable<EmailQueue> GetAll(bool bFresh = false)
        {
            IQueryable<Models.EmailQueue> R = null;
            if (!bFresh)
                R = this.All;
            else
            {
                E.leadEntities.EmailQueues.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = this.All;
                E.leadEntities.EmailQueues.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R;
        }

        public EmailQueue Get(long id)
        {
            return E.leadEntities.EmailQueues.Where(x => x.key == id).FirstOrDefault();
        }

        public EmailQueue Add(long accountKey, DateTime runDateTime, int emailTemplateKey, short status, int mainStatusID = 0, bool pIsTemplateOverride = false,
            string pOverrideFrom = "", string pOverrideTo = "", string pOverrideCC = "",
            string pOverrideBCC = "", bool pOverrideFormat = false, string pOverrideSubject = "", string pOverrideBodyMessage = "", string pOverrideBCCHidden = "")
        {
            EmailQueue nEmailQueue = new EmailQueue
            {
                key = GetId(),
                AccountKey = accountKey,
                EmailTemplateKey = emailTemplateKey,
                RunDateTime = runDateTime,
                Status = status,
                ChangedOn = DateTime.Now,
                MainStatusID = mainStatusID,
                IsTemplateOverride = pIsTemplateOverride,
                OverrideFrom = pOverrideFrom,
                OverrideTo = pOverrideTo,
                OverrideCC = pOverrideCC,
                OverrideBCC = pOverrideBCC,
                OverrideBCCHidden = pOverrideBCCHidden,
                OverrideFormat = pOverrideFormat,
                OverrideSubject = pOverrideSubject,
                OverrideBodyMessage = pOverrideBodyMessage
            };
            E.leadEntities.AddToEmailQueues(nEmailQueue);
            E.Save();
            //[MH:07-jan-2014]
            OnEmailQueueAdded(nEmailQueue.key);
            return Get(nEmailQueue.key);
        }
        public EmailQueue AddOverride(long accountKey, DateTime runDateTime, int emailTemplateKey, short status, int mainStatusID = 0,
            string pOverrideFrom = "", string pOverrideTo = "", string pOverrideCC = "",
            string pOverrideBCC = "", bool pOverrideFormat = false, string pOverrideSubject = "", string pOverrideBodyMessage = "")
        {
            EmailQueue nEmailQueue = new EmailQueue
            {
                key = GetId(),
                AccountKey = accountKey,
                EmailTemplateKey = emailTemplateKey,
                RunDateTime = runDateTime,
                Status = status,
                ChangedOn = DateTime.Now,
                MainStatusID = mainStatusID,
                IsTemplateOverride = true,
                OverrideFrom = pOverrideFrom,
                OverrideTo = pOverrideTo,
                OverrideCC = pOverrideCC,
                OverrideBCC = pOverrideBCC,
                OverrideFormat = pOverrideFormat,
                OverrideSubject = pOverrideSubject,
                OverrideBodyMessage = pOverrideBodyMessage
            };
            E.leadEntities.AddToEmailQueues(nEmailQueue);
            E.Save();
            return Get(nEmailQueue.key);
        }
        public void Change(EmailQueue nQueue)
        {
            if (nQueue.EntityState == EntityState.Detached)
            {
                E.leadEntities.EmailQueues.Attach(nQueue);
                E.leadEntities.ObjectStateManager.ChangeObjectState(nQueue, EntityState.Modified);
            }
            E.Save();
        }
        public void Delete(long key)
        {
            E.leadEntities.EmailQueues.DeleteObject(Get(key));
            E.Save();
        }

        private long GetId()
        {
            return E.leadEntities.EmailQueues.Count() > 0 ?
                E.leadEntities.EmailQueues.Max(x => x.key) + 1 :
                1;
        }
        public void UpdateByQuery(long id, short status)
        {
            const string K_QUERY = "Update email_queue set eq_status = @status, eq_modified_datetime = GETDATE() where eq_key = @id";
            E.Lead.ExecuteStoreCommand(K_QUERY, new Sql.SqlParameter[] { new Sql.SqlParameter("id", id), new Sql.SqlParameter("status", status) });
        }
    }
}
