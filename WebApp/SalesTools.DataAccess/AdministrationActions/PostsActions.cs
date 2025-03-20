using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{    
    public class ManagePostsActions
    {
        private DBEngine engine = null;

        internal ManagePostsActions(DBEngine reng)
        {
            engine = reng;
        }

        public void Add(Models.Post nPost)
        {
            nPost.IsDeleted = false;
            engine.adminEntities.Posts1.AddObject(nPost);
            nPost.Added.On = DateTime.Now;
            engine.Save();
        }

        public void Change(Models.Post nPost)
        {
            engine.Save();
        }

        public void Delete(int postKey)
        {
            var U = (from T in engine.adminEntities.Posts1.Where(x => x.Id==postKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            engine.Save();
        }

        public void MakeEnabled(int postKey)
        {
            var U = (from T in engine.adminEntities.Posts1.Where(x => x.Id==postKey) select T).FirstOrDefault();
            U.Enabled = !U.Enabled;
            engine.Save();
        }

        public IQueryable<Models.Post> GetAll()
        {
            return engine.adminEntities.Posts1.Where(x => x.IsDeleted != true).OrderBy(p=>p.Title);
        }

        public Models.Post Get(int postKey)
        {
            return engine.adminEntities.Posts1.Where(x => x.Id==postKey && x.IsDeleted != true).FirstOrDefault();
        }
    }

}
