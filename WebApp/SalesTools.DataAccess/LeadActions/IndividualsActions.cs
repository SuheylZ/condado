using System;
using DBG = System.Diagnostics.Debug;
using System.Linq;
using System.Data.SqlClient;
using System.Collections.Generic;


namespace SalesTool.DataAccess
{
    public class IndividualsActions : BaseActions
    {
        //private DBEngine _engine = null;
        public event EventHandler<ItemEventArgs<long>> StopLetterChanged;

        protected virtual void OnStopLetterChanged(long individualId)
        {
            var handler = StopLetterChanged;
            if (handler != null) handler(this, new ItemEventArgs<long>(individualId));
        }

        internal IndividualsActions(DBEngine engine)
            : base(engine)
        { }


        public Models.Individual Add(Models.Individual entity, string by="")
        {
            entity.IsActive = true;
            entity.IsDeleted = false;
            entity.AddedBy = by;
            entity.AddedOn = DateTime.Now;

            E.Lead.Individuals.AddObject(entity);

            E.Save();
            return entity;
        }


        public void Change(Models.Individual entity, string by)
        {
            entity.Account.ChangedBy = by;
            entity.Account.ChangedOn = DateTime.Now;
            entity.ChangedBy = by;
            entity.ChangedOn = DateTime.Now;

            E.Save();
            if (entity.IndividualEmailOptOutQueuedChange && entity.arc_cases.Any())
            {
                OnStopLetterChanged(entity.Key);
            }
        }
        public void Delete(long key, string by="", bool bPermanent = false)
        {
            var x = this.Get(key);
            if (x != null)
            {
                if (!bPermanent)
                {
                    x.Account.ChangedBy = by;
                    x.Account.ChangedOn = DateTime.Now;
                    x.ChangedBy = by;
                    x.ChangedOn = DateTime.Now;
                    x.IsDeleted = true;
                }
                else
                    E.leadEntities.Individuals.DeleteObject(x);
            }
            E.Save();
        }

        public void Activate(long key, string by="", bool bActivate = true)
        {
            var Tmp = Get(key);
            if(Tmp!=null){
                Tmp.IsActive = bActivate;
                Tmp.ChangedOn = DateTime.Now;
                Tmp.ChangedBy = by;
            }


            E.Save();
        }

        public IQueryable<Models.ViewIndividuals> GetByAccountID(long id, Guid userkey)
        {
            return E.Lead.GetIndividualsByAccountID(id, userkey).AsQueryable();
            //return E.Lead.GetIndivs(id, userkey).AsQueryable();
            //return E.Lead.ViewIndividuals.Where(x=>x.AccountId==id).OrderBy(x=>x.SortOrder);
            // return this.GetAllOrderByPrimaryThenSecondary().Where(x => x.AccountId == id);
        }

        //public IQueryable<Models.Individual> GetAllOrderByPrimaryThenSecondary()
        //{
        //    return this.GetAll()
        //        .OrderBy(T => (T.Account.PrimaryIndividualId == T.Key) ? -2 : (T.Account.SecondaryIndividualId == T.Key) ? -1 : 0);
        //}

        public IQueryable<Models.Individual> GetAllAccountID(long id)
        {
            return this.GetAll().Where(x => x.AccountId == id);
        }

        public IQueryable<Models.Individual> GetAllAccountID(List<Int64> accountIds)
        {
            var foo = E.leadEntities.Individuals.AsQueryable<Models.Individual>()
               .Where(codeData => accountIds.Contains(codeData.AccountId.Value));
            return foo;
            //return this.GetAll().Where(x => x.AccountId == id);
        }

        public IQueryable<Models.Individual> GetAll()
        {
            //E.Lead.Individuals.MergeOption = System.Data.Objects.MergeOption.NoTracking;
            return E.Lead.Individuals.Where(x => (x.IsActive ?? true) && !(x.IsDeleted ?? false));
        }

        public Models.Individual Get(long id)
        {
            return E.leadEntities.Individuals.Where(x => x.Key == id && (x.IsActive ?? true) && !(x.IsDeleted ?? false)).FirstOrDefault();
        }

        public object FindByPhone(string phone1, string phone2 = "", string phone3 = "")
        {
            phone1 = phone1 == "0" ? string.Empty : phone1;
            phone2 = phone2 == "0" ? string.Empty : phone2;
            phone3 = phone3 == "0" ? string.Empty : phone3;

            var Result = E.leadEntities.FindIndividualByPhone(phone1, phone2, phone3);
            return Result.FirstOrDefault();
        }

        //SZ [Sep 27, 2013] Sets the Consent. Range => {null, y, n, a}
        public void SetConsent(long personid, TCPAConsentType val)
        {
            char? value = null;

            if (val != TCPAConsentType.Blank)
                value = val == TCPAConsentType.Yes ? 'y' : val == TCPAConsentType.No ? 'n' : 'a';

            const string K_SQL = "UPDATE [individuals] SET [indv_tcpa_consent] = @value WHERE [indv_key] = @id";
            E.leadEntities.ExecuteStoreCommand(K_SQL, new SqlParameter[] { new SqlParameter("@value", val == TCPAConsentType.Blank ? (object)DBNull.Value : (object)value), new SqlParameter("@id", personid) });
        }
        public TCPAConsentType GetConsent(long personid)
        {
            TCPAConsentType Ans = TCPAConsentType.Blank;
            const string K_SQL = "Select ISNULL([indv_tcpa_consent],'b') from [individuals] where [indv_key] = @id";
            string lret = E.leadEntities.ExecuteStoreQuery<string>(K_SQL, new SqlParameter[] { new SqlParameter("@id", personid) }).FirstOrDefault<string>();
            if (!string.IsNullOrEmpty(lret))
            {
                lret = lret.Trim();
                Ans = lret == "y" ? TCPAConsentType.Yes : lret == "n" ? TCPAConsentType.No : lret == "a" ? TCPAConsentType.Undefined : TCPAConsentType.Blank;
            }


            return Ans;
        }
    }
}
