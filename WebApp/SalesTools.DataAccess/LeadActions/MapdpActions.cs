using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class MapdpActions: BaseActions
    {
        internal MapdpActions(DBEngine rengine):base(rengine)
        {}

        public void Add(Models.Mapdp nMapdp)
        {
            nMapdp.IsActive = true;
            nMapdp.IsDeleted = false;
            nMapdp.AddedOn = DateTime.Now;
            
            E.leadEntities.Mapdps.AddObject(nMapdp);
            E.Save();
        }


        public void Update(Models.Mapdp nMapdp)
        {
            nMapdp.ChangedOn = DateTime.Now;
            E.Save();
        }


        public void Delete(Int64 inputKey)
        {
            var U = (from T in E.Lead.Mapdps.Where(x => x.Key == inputKey) select T).FirstOrDefault();
            U.IsDeleted = true;
            E.Save();
        }

        public Models.Mapdp GetByKey(Int64? inputKey)
        {
            return E.leadEntities.Mapdps.Where(x =>
                x.Key == inputKey).FirstOrDefault();
        }

        //public void InActivate(Int64 inputKey)
        //{
        //    var U = (from T in E.Lead.Mapdps.Where(x => x.Key == inputKey) select T).FirstOrDefault();
        //    U.IsDeleted = false;
        //    E.Save();
        //}

        //public void Activate(Int64 inputKey)
        //{
        //    var U = (from T in E.Lead.Mapdps.Where(x => x.Key == inputKey) select T).FirstOrDefault();
        //    U.IsActive = true;
        //    E.Save();
        //}

        //public IEnumerable<Models.Mapdp> GetAll()
        //{
        //    return _engine.Lead.Mapdps;
        //}

        //public IQueryable<Models.Mapdp> GetAllRecordByAccountID(Int64? inputAccoutId)
        //{
        //    return _engine.leadEntities.Mapdps.Where(x =>
        //        x.IsDeleted != true && (x.AccountId == inputAccoutId));
        //}


        //public IEnumerable<Models.Mapdp> GetAll(Int64? inputIndvID)
        //{

        //    if (inputIndvID != null && inputIndvID > 0)
        //    {
        //        return _engine.Lead.Mapdps.Where(x =>
        //           (x.Key.Equals(inputIndvID)
        //            && x.IsActive == true && x.IsDeleted == true));
        //    }
        //    else
        //    {
        //        return _engine.Lead.Mapdps.Where(x =>
        //             (x.IsActive == true) && x.IsDeleted != false);
        //    }

        //}

        public IQueryable<Models.ViewMAPDP> GetAllByAccount(long accountID)
        {
            return E.leadEntities.ViewMAPDPs.Where(x => x.AccountId == accountID).AsQueryable();
        }

        public IQueryable<Models.Mapdp> GetByAccount(long accID)
        {
            return E.leadEntities.Mapdps.Where(x => x.AccountId == accID && !(x.IsDeleted??false));
        }

        public IQueryable<Models.MapdpReasonCodes> MapDPReasonCodes
        {
            get
            {
                return E.Lead.mapdp_reason_codes.AsQueryable();
            }
        }
    }
}
