using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SalesTool.DataAccess.Models;

namespace SalesTool.DataAccess
{
    public class LeadMetricActions : BaseActions
    {
        internal LeadMetricActions(DBEngine engine) : base(engine) { }

        public IQueryable<LeadMetric> All
        {
            get
            {
                return E.leadEntities.LeadMetrics.AsQueryable();
            }
        }
    }
}
