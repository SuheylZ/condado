using System;
using System.Collections.Generic;
using SQL =System.Data.SqlClient;
using System.Linq;

namespace SalesTool.DataAccess.Extentions
{
	internal class DuplicateColumn
	{
        DBEngine _engine = null;

        const string K_REPLACER = "<XXX>";
		
        string _column= string.Empty;
        string _table = string.Empty;
        string _sql = string.Empty;
        object _ovalue= null;
        
        List<long> _leadIds = new List<long>();
        Dictionary<string, object> _values = new Dictionary<string, object>();
        List<long> _result = new List<long>();


        int _count = 0;

        internal DuplicateColumn(DBEngine engine, string column, string table, object value)
        {
            _engine = engine;
            _column = column;
            _table = table;
            _ovalue = value;
        }

		internal void Execute()
		{
            List<SQL.SqlParameter> parValues = new List<SQL.SqlParameter>(_values.Count);

            if(_values.Count>0){
                foreach(var K in _values)
                    parValues.Add(new SQL.SqlParameter(K.Key, Convert.ToInt64(K.Value)));
            }

            System.Data.Objects.ObjectResult<long> x = parValues.Count>0?
                _engine.Admin.ExecuteStoreQuery<long>(_sql, parValues):
                _engine.Admin.ExecuteStoreQuery<long>(_sql);

            if (x != null)
                _result = x.ToList<long>();
		}

		internal List<long> LeadIds
		{
			set
			{
                _leadIds = value.Distinct().ToList();
			}
		}
        string StripSelectColumns(string sql)
        {
            string tmp = sql.Substring(sql.IndexOf("SELECT ") + 8);
            string cols = tmp.Substring(0, tmp.IndexOf(" FROM ") - 1);
            string query = tmp.Substring(tmp.IndexOf("FROM "));
            query = string.Format("SELECT lea_key {0}", query);
            return query;
        }
        void Build()
        {
            _sql = QueryHolder.Instance.GetSql(_table);
            _sql = StripSelectColumns(_sql);
            _sql = WhereClause(_sql, _leadIds);
        }
  
        private string WhereClause(string _sql, List<long> _leadIds)
        {
            if (_leadIds.Count > 0)
            {
                string where = string.Empty;

                foreach (var l in _leadIds)
                {
                    string pname = string.Format("@Param{0}",  DateTime.Now.Ticks.ToString());
                    _values[pname] = l;
                    where = where + (where.Length > 0 ? ", " : "") + pname;
                }
                _sql = string.Format("{0} WHERE ( lea_key IN ( {1} ) )", _sql, where);
            }
            return _sql;
        }

        internal int DuplicateCount
        {
            get
            {
                return _count;
            }
        }
	}
}
