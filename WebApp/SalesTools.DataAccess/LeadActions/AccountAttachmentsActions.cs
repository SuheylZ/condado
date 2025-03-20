using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class AccountAttachmentActions
    {
        private DBEngine engine = null;

        internal AccountAttachmentActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(Models.AccountAttachment nAccountAttachment)
        {
            nAccountAttachment.IsDeleted = false;
            engine.Lead.AccountAttachments1.AddObject(nAccountAttachment);
            engine.Save();
            
        }

        public void Change(Models.AccountAttachment nAccountAttachment)
        {
            engine.Save();
        }

        public void Delete(int AttachmentKey)
        {
            var U = Get(AttachmentKey);
            U.IsDeleted = true;
            engine.Save();
        }

        public IQueryable<Models.AccountAttachment> All
        {
            get
            {
                return engine.Lead.AccountAttachments1.Where(x => x.IsDeleted != true);
            }
        }
        public IQueryable<Models.AccountAttachment> AllQueryable
        {
            get
            {
                return engine.Lead.AccountAttachments1.Where(x => x.IsDeleted != true);
            }
        }

        public Models.AccountAttachment Get(int accountAttachmentKey)
        {
            return All.Where(x => x.Id == accountAttachmentKey && x.IsDeleted != true).FirstOrDefault();
        }

        public IQueryable<Models.AccountAttachment> GetAllByAccountID(long nAccountID)
        {

            return All.Where(x => x.IsDeleted != true && x.AccountId == nAccountID);            
        }

        public IQueryable<Models.AccountAttachment> GetAllOverride(long nAccountID, int emailQueueKey)
        {

            return AllQueryable.Where(x=> x.AccountId == nAccountID );
        }
    }

}
