using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class ManageEmailTemplateActions
    {
        private DBEngine engine = null;

        internal ManageEmailTemplateActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(EmailTemplate nEmailTemplate)
        {
            nEmailTemplate.IsDeleted= false;
            //int id = 0;
            //if (engine.entities.EmailTemplates.Count() > 0)
            //    id = engine.entities.EmailTemplates.Max(x => x.Id);
            //nEmailTemplate.Id = id + 1;
            engine.adminEntities.EmailTemplates.AddObject(nEmailTemplate);
            engine.Save();

        }

        public void Change(EmailTemplate nEmailTemplate)
        {
            engine.Save();
        }

        public void Delete(int emailTemplateKey)
        {
            var U = (from T in engine.adminEntities.EmailTemplates.Where(x => x.Id.Equals(emailTemplateKey)) select T).FirstOrDefault();
            U.IsDeleted = true;
            engine.Save();
        }

        public void MakeEnabled(int emailTemplateKey)
        {
            var U = (from T in engine.adminEntities.EmailTemplates.Where(x => x.Id.Equals(emailTemplateKey)) select T).FirstOrDefault();
            U.Enabled = !U.Enabled;
            engine.Save();
        }

        public IQueryable<EmailTemplate> GetAll(bool bFresh = false)
        {
            IQueryable<Models.EmailTemplate> R = null;
            if (!bFresh)
                R = engine.adminEntities.EmailTemplates.OrderBy(p=>p.Id);
            else
            {
                engine.adminEntities.EmailTemplates.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = engine.adminEntities.EmailTemplates.OrderBy(p => p.Id);
                engine.adminEntities.EmailTemplates.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R.Where(x => x.IsDeleted != true);
        }

        public IQueryable<EmailTemplate> GetAllIQueryable()
        {
            IQueryable<Models.EmailTemplate> R = null;            
            R = engine.adminEntities.EmailTemplates;            
            return R.Where(x => x.IsDeleted != true);
        }

        public EmailTemplate Get(int emailTemplateKey, bool bFresh = false)
        {
            IEnumerable<Models.EmailTemplate> R = null;
            if (!bFresh)
                R = engine.adminEntities.EmailTemplates;
            else
            {
                engine.adminEntities.EmailAttachments.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = engine.adminEntities.EmailTemplates;
                engine.adminEntities.EmailAttachments.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R.Where(x => x.Id == emailTemplateKey && x.IsDeleted != true).FirstOrDefault();
        }
    }

    public class EmailAttachmentActions
    {
        private DBEngine engine = null;

        internal EmailAttachmentActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(EmailAttachment nEmailAttachment,bool IsOverride = false, bool Istemporary = false)
        {
            nEmailAttachment.IsDeleted = false;
            nEmailAttachment.IsOverride = IsOverride;
            nEmailAttachment.IsTemporaryEntry = Istemporary;
            nEmailAttachment.Added.On = DateTime.Now;    
            engine.adminEntities.EmailAttachments.AddObject(nEmailAttachment);
            engine.Save();
        }
        
        public void Change(EmailAttachment nEmailAttachment)
        {
            engine.Save();
        }

        public void Delete(int emailAttachmentKey)
        {
            var U = (from T in engine.adminEntities.EmailAttachments.Where(x => x.Id.Equals(emailAttachmentKey)) select T).FirstOrDefault();
            U.IsDeleted = true;
            engine.Save();
        }
        //YA[Nov 19, 2013] Delete temporary attachments
        public void DeleteAllTemporary(int emailTemplatekey, string userFullName)
        {
            var U = engine.adminEntities.EmailAttachments.Where(x => x.IsDeleted != true && x.IsTemporaryEntry == true && x.IsOverride == true && x.EmailTemplateKey == emailTemplatekey && x.Added.By == userFullName);            
            foreach (var item in U)
            {
                item.IsDeleted = true;
                item.IsTemporaryEntry = false;
                item.IsOverride = false;
            }
            engine.Save();    
        }
        //YA[Nov 19, 2013] Change all temporary attachments to permanent against the queued record.
        public void UpdateAllTemporary(int emailTemplatekey, string userFullName, long emailQueueKey = 0)
        {
            var U = engine.adminEntities.EmailAttachments.Where(x => x.IsDeleted != true && x.IsTemporaryEntry == true && x.IsOverride == true  && x.EmailTemplateKey == emailTemplatekey && x.Added.By == userFullName);
            foreach (var item in U)
            {
                item.IsTemporaryEntry = false;
                if (emailQueueKey != 0)
                {
                    item.EmailQueueKey = emailQueueKey;
                    
                }
            }
            engine.Save();
        }

        public IQueryable<EmailAttachment> GetAll()
        {
            return engine.adminEntities.EmailAttachments.Where(x => x.IsDeleted != true);
        }

        public EmailAttachment Get(int emailAttachmentKey)
        {
            return engine.adminEntities.EmailAttachments.Where(x => x.Id == emailAttachmentKey && x.IsDeleted != true).FirstOrDefault();
        }

        public IQueryable<EmailAttachment> GetAllByTemplateKey(int emailTemplatekey, bool bFresh = false)
        {
            IQueryable<Models.EmailAttachment> R = null;
            if (!bFresh)
                R = engine.adminEntities.EmailAttachments;
            else
            {
                engine.adminEntities.EmailAttachments.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = engine.adminEntities.EmailAttachments;
                engine.adminEntities.EmailAttachments.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R.Where(x => x.EmailTemplateKey == emailTemplatekey && x.IsDeleted != true && x.IsOverride == false);
        }
        //YA[Nov 19, 2013] To get the override email templates attachments
        public IQueryable<EmailAttachment> GetAllByTemplateQueueKey(int emailTemplatekey, long? emailQueuekey, bool bFresh = false, string userFullName = "")
        {
            IQueryable<Models.EmailAttachment> R = null;
            if (!bFresh)
                R = engine.adminEntities.EmailAttachments;
            else
            {
                engine.adminEntities.EmailAttachments.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = engine.adminEntities.EmailAttachments;
                engine.adminEntities.EmailAttachments.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            if(emailQueuekey > 0)
                R = R.Where(x => x.EmailTemplateKey == emailTemplatekey && x.IsDeleted != true && x.EmailQueueKey == emailQueuekey && x.IsOverride == true );
            else
                R = R.Where(x => x.EmailTemplateKey == emailTemplatekey && x.IsDeleted != true && x.IsOverride == true && x.IsTemporaryEntry == true);
            if (userFullName != "")
            {
                R = R.Where(x => x.Added.By == userFullName);
            }
            return R;
        }
    }

   }
