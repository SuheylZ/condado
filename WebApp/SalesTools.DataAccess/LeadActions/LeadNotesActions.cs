using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
  public  class LeadNotesActions
    {

       private DBEngine _engine = null;

       internal LeadNotesActions(DBEngine engine)
        {
            _engine = engine;
        }

       public void Add(Models.LeadNote nlead_notes)
        {
            nlead_notes.IsActive = true;
            nlead_notes.IsDeleted = false;
            nlead_notes.AddedOn = DateTime.Now;
            _engine.Lead.LeadNotes.AddObject(nlead_notes);
            _engine.Save();
        }


       public void Update(Models.LeadNote nlead_notes)
        {
            nlead_notes.ChangedOn = DateTime.Now;
            _engine.Save();
        }


        public void Delete(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadNotes.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            _engine.Save();
        }

        public void InActivate(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadNotes.Where(x => x.Key==inputKey) select T).FirstOrDefault();
            U.IsDeleted = false;
            _engine.Save();
        }

        public void Activate(long? inputKey)
        {
            var U = (from T in _engine.Lead.LeadNotes.Where(x => x.Key== inputKey) select T).FirstOrDefault();
            U.IsActive = true;
            _engine.Save();
        }

        public IQueryable<Models.LeadNote> GetAll()
        {
            return _engine.Lead.LeadNotes;
        }


        public IQueryable<Models.LeadNote> GetAll(long? inputIndvID)
        {

            if (inputIndvID != null && inputIndvID > 0)
            {
                return _engine.Lead.LeadNotes.Where(x =>
                   (x.Key.Equals(inputIndvID)
                    && x.IsActive == true && x.IsDeleted == true));
            }
            else
            {
                return _engine.Lead.LeadNotes.Where(x =>
                     (x.IsActive == true) && x.IsDeleted != false);
            }

        }

    }
}
