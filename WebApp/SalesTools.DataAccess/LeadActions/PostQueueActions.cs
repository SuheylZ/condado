using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class PostQueueActions : BaseActions
    {
        internal PostQueueActions(DBEngine engine) : base(engine) { }

        public IQueryable<PostQueue> All
        {
            get
            {
                return E.leadEntities.PostQueues.Where(x => x.Status != 0).AsQueryable();
            }
        }
        public IQueryable<PostQueue> GetAll(bool bFresh = false)
        {
            IQueryable<Models.PostQueue> R = null;
            if (!bFresh)
                R = this.All;
            else
            {
                E.leadEntities.PostQueues.MergeOption = System.Data.Objects.MergeOption.NoTracking;
                R = this.All;
                E.leadEntities.PostQueues.MergeOption = System.Data.Objects.MergeOption.PreserveChanges;
            }
            return R;
        }

        public PostQueue Get(long id)
        {
            return E.leadEntities.PostQueues.Where(x => x.Key == id).FirstOrDefault();
        }

        public PostQueue Add(long accountKey, DateTime runDateTime, int postTemplateKey, short status,int mainStatusID = 0)
        {
            PostQueue nPostQueue = new PostQueue
            {
                Key = GetId(),
                AccountKey = accountKey,
                PostTemplateKey = postTemplateKey,
                RunDateTime= runDateTime,
                Status = status, 
                ChangedOn = DateTime.Now,
                MainStatusID = mainStatusID
            };
            E.leadEntities.AddToPostQueues(nPostQueue);
            E.Save();
            return Get(nPostQueue.Key);
        }
        public void Change(PostQueue nQueue)
        {            
            E.Save();            
        }
        public void Delete(long key)
        {
            E.leadEntities.PostQueues.DeleteObject(Get(key));
            E.Save();
        }        

        private long GetId()
        {
            return E.leadEntities.PostQueues.Count() > 0 ?
                E.leadEntities.PostQueues.Max(x => x.Key) + 1 :
                1;
        }

    }
}
