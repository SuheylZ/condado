using System;
using System.Collections.Generic;
using System.Linq;
using SQL=System.Data.SqlClient;
using DBG = System.Diagnostics.Debug;

namespace SalesTool.DataAccess.Extentions
{
	public class QueryFilter
	{
        DBEngine _engine = null;
		string _sql = string.Empty;
		int _order = 0;
		List<long> _leadIds = new List<long>();
        Models.FilterLead _filter = null;
        Dictionary<string, object> _values = new Dictionary<string, object>();

        
        
        internal QueryFilter(DBEngine engine, Models.FilterLead filter)
        {
            DBG.Assert(engine != null);
            _engine = engine;
            _order = filter.QUERYORDER ?? 1;
            _filter = filter;
        }

        string StripSelectColumns(string sql)
        {
            string tmp = sql.Substring(sql.IndexOf("SELECT ") + 8);
            string cols = tmp.Substring(0, tmp.IndexOf(" FROM ") - 1);
            string query = tmp.Substring(tmp.IndexOf("FROM "));
            query = "SELECT leads.lea_key " + query;
            return query;
        }

        public void Build()
        {
            _sql = QueryHolder.Instance.GetSql(_filter.TABLENAME);
            _sql = StripSelectColumns(_sql);
            
            ExtractValues(_filter.DATATYPE??0, _filter.VALUE);
            _sql = WhereClause(_sql, _filter.FILTERCOLUMN, _filter.SQLOPERATOR, _values);
        }
  
        private string WhereClause(string sql, string column, string expr, Dictionary<string, object> values)
        {
            sql = sql + string.Format(" WHERE ( {0} {1} )", column, expr);
            string where = string.Empty;
            
            foreach (var x in values)
                where = where + (where.Length>0? ",": " ") + x.Key;

            sql = sql.Replace("<X>", where);
            return sql;
        }

        void ExtractValues(byte type, string value)
        {
            
            string[] pvalues = value.Split(new char[] { ',' });
            object obj = null;

            //Numeric = 0, Text = 1, Date = 2, Table = 3, Checkbox = 4, DateTime = 5, Guid = 9

            foreach (var pvalue in pvalues)
            {
                switch (type)
                {
                    case 0: obj = Convert.ToInt64(pvalue); break;
                    case 1: obj = pvalue; break;
                    case 2: obj = Convert.ToDateTime(pvalue); break;
                    case 5: obj = Convert.ToDateTime(pvalue); break;
                    case 9: obj = new Guid(pvalue); break;
                    default: obj = pvalue; break;
                }
                string pname = string.Format("@p{0}QryFltr{1}",  _order, DateTime.Now.Ticks.ToString().Substring(0, 7));
                _values[pname] = obj;
            }
        }


		public void Execute()
		{
            List<SQL.SqlParameter> param = new List<SQL.SqlParameter>();

            foreach (var val in _values)
                param.Add(new SQL.SqlParameter(val.Key, val.Value));

            try // SZ [Jun 17, 2013] exception handling for debug purpose only. not for the production code
            {
                var obj = _engine.Admin.ExecuteStoreQuery<long>(_sql, param.ToArray());
                if (obj != null)
                    _leadIds = obj.ToList();
            }
            catch(Exception ex)
            {
                DBG.WriteLine(ex);
            }
		}

		public List<long> LeadIds
		{
			get
			{
                return _leadIds;
            }
		}

		internal int Order
		{
			get
			{
                return _order;
			}
		}

		internal string Sql
		{
			get
			{
                return _sql;
			}
		}
	}
}
