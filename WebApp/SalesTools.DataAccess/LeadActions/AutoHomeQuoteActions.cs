using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SalesTool.DataAccess
{
    public class AutoHomeQuoteActions: BaseActions
    {
        internal AutoHomeQuoteActions(DBEngine reng):base(reng)
        {}

        public Models.AutoHomeQuote Add(Models.AutoHomeQuote nAutoHomeQuote)
        {
            long id = 0;
            if (E.Lead.AutoHomeQuotes.Count() > 0)
                id = E.Lead.AutoHomeQuotes.Max(x => x.Id);
            nAutoHomeQuote.Id = id + 1;
            E.Lead.AutoHomeQuotes.AddObject(nAutoHomeQuote);
            E.Save();
            return nAutoHomeQuote;
        }
        public long GetMaxID(long id)
        {
            if (E.Lead.AutoHomeQuotes.Count() > 0)
                id = E.Lead.AutoHomeQuotes.Max(x => x.Id);
            return id;
        }
        public void Change(Models.AutoHomeQuote nAutoHomeQuote)
        {
            E.Save();
        }

        public void Delete(long nAutoHomeQuoteID)
        {            
            E.Lead.AutoHomeQuotes.DeleteObject(Get(nAutoHomeQuoteID));
            E.Save();
        }

        public IQueryable<Models.AutoHomeQuote> All
        {
            get
            {
                return E.Lead.AutoHomeQuotes;
            }
        }

        public Models.AutoHomeQuote Get(long nAutoHomeQuoteKey)
        {
            return All.Where(x => x.Id == nAutoHomeQuoteKey).FirstOrDefault();
        }

        public Models.AutoHomeQuote Get(long nAutoHomeQuoteKey, long nAccountID)
        {
            return All.Where(x => x.Id == nAutoHomeQuoteKey && x.AccountKey== nAccountID).FirstOrDefault();
        }

        public IQueryable<Models.AutoHomeQuote> GetAllByAccountID(long nAccountID)
        {
            return All.Where(y=> y.AccountKey == nAccountID);            
        }

        public IQueryable<Models.ViewAutoHomeQuote> GetQuotes(long accId)
        {
            return E.Lead.ViewAutoHomeQuote.Where(x => x.AccountId == accId);
        }

    }
}
