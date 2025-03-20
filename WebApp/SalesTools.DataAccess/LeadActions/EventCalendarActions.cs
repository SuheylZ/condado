using System;
using System.Data;
using System.Data.Objects.SqlClient;
using System.Linq;
using SalesTool.DataAccess.Models;
using System.Collections.Generic;

namespace SalesTool.DataAccess
{
    public class EventCalendarActions
    {
        //EventStatus 

        // 0 - Pending
        // 1 - Past Due
        // 2 - Completed
        // 3 - Dismissed

        private DBEngine _engine = null;

        internal EventCalendarActions(DBEngine engine)
        {
            _engine = engine;
        }

        public void Snooze(long key, int timeFromNow, DateTime dateTime)
        {
            var entity = this.Get(key);

            if (entity == null)
            {
                return;
            }

            entity.IsTimeFromNow = true;
            entity.IsSpecificDateTimeFromNow = false;
            entity.TimeFromNow = timeFromNow;
            entity.SpecificDateTimeFromNow = dateTime;
            entity.IsOpened = false;
            entity.EventStatus = 0;  // pending

            this.Change(entity);
        }

        public void Complete(long key)
        {
            var entity = this.Get(key);

            if (entity == null)
            {
                return;
            }

            entity.Completed = true;
            entity.EventStatus = 2;  // Completed
            entity.Dismissed = entity.DismissUponStatusChange;
            entity.IsOpened = false;


            this.Change(entity);
        }

        public void Dismiss(long key)
        {
            var entity = this.Get(key);

            if (entity == null)
            {
                return;
            }

            entity.Dismissed = true;
            entity.EventStatus = 3;  // Dismissed
            entity.IsOpened = false;

            this.Change(entity);
        }

        /// <summary>
        /// turns on/off IsOpened flag
        /// of event calendar entity.
        /// </summary>
        /// <param name="key">event calendar key</param>
        /// <param name="IsOpened"></param>
        public void OpenClose(long key, bool IsOpened = false)
        {
            var entity = this.Get(key);

            if (entity == null)
            {
                return;
            }

            entity.IsOpened = IsOpened;

            this.Change(entity);
        }
        //public void DismissUponActionChange(Guid userId, string excludeEventIDs)
        //{
        //    this.DismissUponActionChange(userId, excludeEventIDs.Split(','));
        //}

        //public void DismissUponActionChange(Guid userId, string[] excludeEventIDs)
        //{
        //    var entities = this.GetByUserID(userId).AsEnumerable()
        //        .Where(x => !x.Dismissed
        //            && !x.Completed
        //            && x.DismissUponStatusChange
        //            && x.SpecificDateTimeFromNow >= DateTime.Now
        //        && !excludeEventIDs.Contains(x.ID.ToString())
        //        );//.AsQueryable();

        //    if (entities == null || entities.Count() == 0)
        //    {
        //        return;
        //    }

        //    foreach (var entity in entities)
        //    {
        //        entity.Dismissed = true;
        //        entity.EventStatus = 3;  // Dismissed

        //        entity.Changed.On1 = DateTime.Now;
        //    }

        //    _engine.Save();
        //}

        public bool DismissUponActionChange(long accountId, string excludeEventIDs)
        {
            return this.DismissUponActionChange(accountId, excludeEventIDs.Split(','));
        }

        public bool DismissUponActionChange(long accountId, string[] excludeEventIDs)
        {
            bool hasChanged = false;
            List<long> ids = excludeEventIDs.Where(p => !string.IsNullOrEmpty(p))
                                      .Select(p => Convert.ToInt64(excludeEventIDs)).ToList();
            var entities = this.GetByAccountID(accountId)
                .Where(x => !x.Dismissed
                    && !x.Completed
                    && x.DismissUponStatusChange
                    &&
                    System.Data.Objects.EntityFunctions.TruncateTime(x.SpecificDateTimeFromNow) >= DateTime.Today
                && !ids.Contains(x.ID)

                ).ToList();//.AsQueryable();

            if (entities == null)
            {
                return false;
            }

            if (entities.Count() == 0)
            {
                return false;
            }

            foreach (var entity in entities)
            {
                var c = ApplyChangeOnEventView(entity);
                c.Dismissed = true;
                c.EventStatus = 3;  // Dismissed

                c.Changed.On1 = DateTime.Now;
            }

            _engine.Save();
            hasChanged = true;
            return hasChanged;
        }




        public void DismissAllUponActionChange(long accountId)
        {
            var entities = this.GetByAccountID(accountId).AsEnumerable()
                .Where(x => !x.Dismissed
                    && !x.Completed
                    && x.DismissUponStatusChange
                    && x.SpecificDateTimeFromNow >= DateTime.Now
                );//.AsQueryable();

            if (entities == null)
            {
                return;
            }

            if (entities.Count() == 0)
            {
                return;
            }

            foreach (var entity in entities)
            {
                var c = ApplyChangeOnEventView(entity);
                c.Dismissed = true;
                c.EventStatus = 3;  // Dismissed
                c.Changed.On1 = DateTime.Now;
            }

            _engine.Save();
        }

        public void Add(EventCalendar entity)
        {
            if (entity == null)
            {
                return;
            }

            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.Added.On1 = DateTime.Now;

            // Safe Guard code
            {
                if (entity.SpecificDateTimeFromNow.CompareTo(new DateTime(1753, 1, 1)) < 0) // January 1, 1753
                {
                    entity.SpecificDateTimeFromNow = DateTime.Today;
                }

            }

            _engine.Lead.EventCalendars1.AddObject(entity);

            _engine.Save();
        }

        //public void Change(EventCalendar entity)
        //{
        //    if (entity == null)
        //    {
        //        return;
        //    }

        //    {
        //        if (entity.SpecificDateTimeFromNow.CompareTo(new DateTime(1753, 1, 1)) < 0) // January 1, 1753
        //        {
        //            entity.SpecificDateTimeFromNow = DateTime.Today;
        //        }

        //    }

        //    entity.Changed.On1 = DateTime.Now;

        //    _engine.Save();
        //}
        public void Change(vw_AccountEventCalendar entity)
        {
            if (entity == null)
            {
                return;
            }
            EventCalendar c = ApplyChangeOnEventView(entity);
            if (c.SpecificDateTimeFromNow.CompareTo(new DateTime(1753, 1, 1)) < 0) // January 1, 1753
            {
                c.SpecificDateTimeFromNow = DateTime.Today;
            }



            c.Changed.On1 = DateTime.Now;

            _engine.Save();
        }

        private EventCalendar ApplyChangeOnEventView(vw_AccountEventCalendar entity)
        {
            EventCalendar c = entity;
            _engine.leadEntities.ObjectStateManager.ChangeObjectState(entity.Added, EntityState.Detached);
            _engine.leadEntities.ObjectStateManager.ChangeObjectState(entity.Changed, EntityState.Detached);
            _engine.leadEntities.ObjectStateManager.ChangeObjectState(entity, EntityState.Detached);
            _engine.leadEntities.EventCalendars1.Attach(c);
            _engine.leadEntities.ObjectStateManager.ChangeObjectState(c, EntityState.Modified);
            return c;
        }
        public void Delete(long key)
        {
            var entity = this.Get(key);

            if (entity == null)
            {
                return;
            }

            entity.IsDeleted = true;
            var c = ApplyChangeOnEventView(entity);
            _engine.Save();
        }

        public void Activate(long key, bool bActivate = true)
        {
            var entity = this.Get(key);

            if (entity == null)
            {
                return;
            }

            entity.IsActive = bActivate;
            var c = ApplyChangeOnEventView(entity);
            _engine.Save();
        }

        public DateTime? GetMostRecentToHappenEventDateTimeByAccountId(long id)
        {
            var entities = this.GetByAccountID(id);

            if (entities == null)
            {
                return null;
            }

            if (entities.Count() == 0)
            {
                return null;
            }

            entities = entities.Where(x => !x.Dismissed && !x.Completed && x.SpecificDateTimeFromNow >= DateTime.Now);

            if (entities == null)
            {
                return null;
            }

            if (entities.Count() == 0)
            {
                return null;
            }

            return entities.Min(x => x.SpecificDateTimeFromNow);
        }

        //public EventCalendar Get(long id)
        //{

        //    return this.GetAll().Where(x => x.ID == id).FirstOrDefault();


        //}
        public vw_AccountEventCalendar Get(long id)
        {

            return this.GetAll().Where(x => x.ID == id).FirstOrDefault();
        }

        //public IQueryable<EventCalendar> GetByAccountID(long id)
        //{
        //    return this.GetAll().Where(x => x.AccountID == id);
        //}
        public IQueryable<vw_AccountEventCalendar> GetByAccountID(long id)
        {
            return this.GetAll().Where(x => x.AccountID == id);
        }

        //public IQueryable<EventCalendar> GetByUserID(Guid id)
        //{
        //    return this.GetAll().Where(x => x.UserID == id);
        //}
        public IQueryable<vw_AccountEventCalendar> GetByUserID(Guid id)
        {
            return this.GetAll().Where(x => x.UserID == id);
        }
        /// <summary>
        /// Mehross S -Added new method to get the event calander by using view
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IQueryable<vw_EventCalendar> GetEventsByUserID(Guid id)
        {
            return this.GetCalendarData().Where(x => x.UserID == id);
        }

        //public IQueryable<EventCalendar> GetNextHourByUserID(Guid id, double hours)
        //{
        //    return this.GetByUserID(id)/*.AsEnumerable()*/
        //        .Where(x => !x.Dismissed && !x.Completed && x.SpecificDateTimeFromNow <= SqlFunctions.DateAdd("HOUR",hours,DateTime.Now)/*DateTime.Now.AddHours(hours).AsQurable()*/);
        //}

        public IQueryable<vw_AccountEventCalendar> GetNextHourByUserID(Guid id, double hours)
        {
            return this.GetByUserID(id)/*.AsEnumerable()*/
                .Where(x => !x.Dismissed && !x.Completed && x.SpecificDateTimeFromNow <= SqlFunctions.DateAdd("HOUR", hours, DateTime.Now)/*DateTime.Now.AddHours(hours).AsQurable()*/);
        }

        public IQueryable<vw_AccountEventCalendar> GetAll()
        {
            //IH 27.09.13--here check the lead table as well should be IsActive and not deleted 
            //var qry = (from x in _engine.Lead.Leads
            //           join y in _engine.Lead.EventCalendars1
            //               on x.AccountId equals y.AccountID
            //           where (x.IsActive ?? false) && !(x.IsDeleted ?? false) && y.IsActive && !y.IsDeleted
            //           select y);

            //IH 12.10.13--here check the lead table as well should be IsActive and not deleted  
            //Added Accounts table join 

            //var qry = (from ac in _engine.Lead.Accounts
            //           join evc in _engine.Lead.EventCalendars1
            //               on ac.Key equals evc.AccountID
            //           join l in _engine.Lead.Leads on ac.PrimaryLeadKey equals l.Key
            //           where (l.IsActive ?? false) && !(l.IsDeleted ?? false) && evc.IsActive && !evc.IsDeleted && (ac.IsActive ?? false) && !(ac.IsDeleted ?? false)
            //           select evc);


            ////       return _engine.Lead.EventCalendars1.Where(x => x.IsActive && !x.IsDeleted);
            //return qry;
            var query = _engine.leadEntities.vw_AccountEventCalendar;
            return query;
        }

        public IQueryable<vw_EventCalendar> GetCalendarData()
        {
            return _engine.leadEntities.vw_EventCalendar.AsQueryable();
        }

        public Int32? GetEventTimeZone(Int64? accountId, Guid? loggedInUserId)
        {
            Int32? result = null;
            try
            {
                result = _engine.adminEntities.GetEventTimeZone(accountId, loggedInUserId).AsEnumerable().FirstOrDefault();
            }
            catch (Exception e)
            {
            }
            return result;
        }

        public IQueryable<Models.TimeZone> GetTimeZones()
        {
            return _engine.adminEntities.TimeZones;
        }
    }
}