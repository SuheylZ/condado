using System;
using System.Collections.Generic;
using System.Linq;

// SZ [Jan 17, 2013] The file has been created by D.Tosh and has been altered for efficency, consistency and maintainbility
// The code has been reduced as well as is more terse than previous version. 
// for older version, use TFS History feature



namespace SalesTool.DataAccess
{
    public class VehiclesActions: BaseActions
    {
        internal VehiclesActions(DBEngine inputEngine)
            : base(inputEngine)
        {}
        public Models.Vehicle Add(Models.Vehicle vehicle, string byUser)
        {
            vehicle.IsDeleted = false;
            vehicle.IsActive = true;
            vehicle.AddedOn = DateTime.Now;
            vehicle.AddedBy = byUser;
            E.Lead.Vehicles.AddObject(vehicle);
            E.Save();

            return vehicle;
        }
        public void Change(Models.Vehicle vehicle, string byUser)
        {
            vehicle.IsDeleted = false;
            vehicle.ChangedBy = byUser;
            vehicle.ChangedOn = DateTime.Now;
            E.Save();
        }
        public void Activate (long key, bool bActive=true)
        {
            All.Where(x=> x.Key==key).FirstOrDefault().IsActive = bActive;
            E.Save();
        }

        public void ToggleActivation(long key)
        {
            var T=  All.Where(x => x.Key == key).FirstOrDefault();
            T.IsActive = !T.IsActive;
            E.Save();
        }
        public void Delete(long key, bool bPermanent =false)
        {
            if (!bPermanent)
                All.Where(x => x.Key == key).FirstOrDefault().IsDeleted = true;
            else
                E.leadEntities.Vehicles.DeleteObject(
                    All.Where(x => x.Key == key).FirstOrDefault()
                    );

            E.Save();
        }
        public IQueryable<Models.Vehicle> All
        {
            get{
                return E.Lead.Vehicles.Where(x=> !(x.IsDeleted??false)).OrderBy(x=>x.AddedOn);
            }
        }
        public Models.Vehicle Get(long key)
        {
            return All.Where(x => x.Key == key).FirstOrDefault(); 
        }
        public IQueryable<Models.Vehicle> GetByAccount(Int64 id)
        {
            return All.Where(x => x.AccountId == id);
        }

        public IQueryable<Models.ViewVehicle> GetVehicles(long accId)
        {
            return E.leadEntities.ViewVehicles.Where(x => x.AccountId== accId).AsQueryable();
        }

   }
}
