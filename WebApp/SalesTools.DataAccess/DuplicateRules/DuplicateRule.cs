using System;
using System.Collections.Generic;
using DBG = System.Diagnostics.Debug;
using System.Linq;


namespace SalesTool.DataAccess.Extentions
{
	public class DuplicateRule
	{
        DBEngine _engine = null;        
		int _ruleId=0;

		List<DuplicateColumn> _columns= new List<DuplicateColumn>();
		List<QueryFilter> _oldFilters = new List<QueryFilter>();
		List<QueryFilter> _newFilters = new List<QueryFilter>();

        QueryHolder _holder = null;

        public DuplicateRule(DBEngine engine, int ruleid)
        {
            DBG.Assert(engine != null);
            _engine = engine;
            _ruleId = ruleid;
            _holder = QueryHolder.Instance;
        }

		public void Build()
		{
            IEnumerable<Models.FilterLead> filters = _engine.DuplicateRecordActions.GetFiltersForExisitng(_ruleId);
            foreach (var filter in filters)
            {
                QueryFilter Q = new QueryFilter(_engine, filter);
                Q.Build();
                _oldFilters.Add(Q);
            }

            filters = _engine.DuplicateRecordActions.GetFiltersForIncomming(_ruleId);
            foreach (var filter in filters)
            {
                QueryFilter Q = new QueryFilter(_engine, filter);
                Q.Build();
                _newFilters.Add(Q);
            }
		}

		public bool Execute()
		{
            foreach (QueryFilter Q in _oldFilters)
            {
                Q.Execute();
            }

            foreach (QueryFilter Q in _newFilters)
                Q.Execute();



            return false;
		}

        public List<long> IncommingLeadIds
        {
            get
            {
                List<long> list = new List<long>();
                foreach (var F in _newFilters)
                {
                    if (F.LeadIds.Count > 0)
                        list.AddRange(F.LeadIds);
                }
                
                return list.Distinct().ToList();
            }
        }
        public List<long> ExistingLeadIds
        {
            get
            {
                List<long> list = new List<long>();
                foreach (var F in _oldFilters)
                {
                    if (F.LeadIds.Count > 0)
                        list.AddRange(F.LeadIds);
                }

                return list.Distinct().ToList();
            }
        }
	}
}
