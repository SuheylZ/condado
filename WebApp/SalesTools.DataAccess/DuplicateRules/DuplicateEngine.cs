using System;
using System.Linq;
using System.Collections.Generic;

using DBG = System.Diagnostics.Debug;

namespace SalesTool.DataAccess.Extentions
{

    public class DuplicateEngine
    {
        DBEngine _engine = null;
        string _filepath = string.Empty;
        List<DuplicateRule> _rules = new List<DuplicateRule>();

        public DuplicateEngine(DBEngine engine, string file)
        {
            DBG.Assert(engine != null);
            DBG.Assert(System.IO.File.Exists(file));

            _engine = engine;
            _filepath = file;
            QueryHolder.FilePath = _filepath;
        }

        public void Build()
        {
                int[] ruleids = _engine.DuplicateRecordActions.All.Where(x=>x.IsActive??false).Select(x => x.Id).ToArray();
                for (int i = 0; i < ruleids.Length; i++)
                {
                    var rule = new DuplicateRule(_engine, ruleids[i]);
                    rule.Build();
                    _rules.Add(rule);
                }
        }

        public void Execute()
        {
            foreach (var rule in _rules)
                rule.Execute();
        }
    }


    //public class QueryHolder
    //{
    //    const string K_NAME = "name";
    //    const string K_SQL = "sql";
    //    const string K_ID = "id";
    //    const string K_ALIAS = "alias";

    //    Xml.XmlDocument _doc = null;
    //    Xml.XmlNodeList _nodes = null;

    //    public QueryHolder(string file)
    //    {
    //        _doc = new Xml.XmlDocument();
    //        _doc.Load(file);
    //        _nodes = _doc.ChildNodes[1].ChildNodes;
    //    }

    //    string GetSqlByName(string name)
    //    {
    //        string sAns = string.Empty;
    //        foreach (Xml.XmlNode N in _nodes)
    //            if (string.Compare(N.Attributes[K_NAME].Value, name, true) == 0)
    //            {
    //                sAns = N.Attributes[K_SQL].InnerText;
    //                break;
    //            }
    //        return sAns;
    //    }
    //    string GetSqlById(int id)
    //    {
    //        string ids = id.ToString();
    //        string sAns = string.Empty;
    //        foreach (Xml.XmlNode N in _nodes)
    //            if (string.Compare(N.Attributes[K_ID].Value, ids, true) == 0)
    //            {
    //                sAns = N.Attributes[K_SQL].InnerText;
    //                break;
    //            }
    //        return sAns;
    //    }
    //    string GetSqlByAlias(string alias)
    //    {
    //        string sAns = string.Empty;
    //        foreach (Xml.XmlNode N in _nodes)
    //            if (string.Compare(N.Attributes[K_ALIAS].Value, alias, true) == 0)
    //            {
    //                sAns = N.Attributes[K_SQL].InnerText;
    //                break;
    //            }
    //        return sAns;
    //    }

    //    public string GetSql(string key)
    //    {
    //        string sAns = string.Empty;

    //        sAns = GetSqlByName(key);
    //        if (sAns.Length == 0)
    //            sAns = GetSqlByAlias(key);
    //        return sAns;
    //    }
    //}

 
    //public class DuplicateRules
    //{
        //SalesTool.DataAccess.DBEngine _engine = null;
        //QueryHolder _holder;
        
        //const string K_NAME = "name";
        //const string K_SQL = "sql";
        //const string K_ID = "id";
        //const string K_ALIAS = "alias";

        //Xml.XmlDocument _doc=null;
        //Xml.XmlNodeList _nodes = null;

        //public DuplicateRules(SalesTool.DataAccess.DBEngine engine, string queriesfile)
        //{
        //    _engine = engine;
        //    _holder = new QueryHolder(queriesfile);
        //}

        //string GetSqlByName(string name)
        //{
        //    string sAns = string.Empty;
        //    foreach (Xml.XmlNode N in _nodes)
        //        if (string.Compare(N.Attributes[K_NAME].Value, name, true) == 0)
        //        {
        //            sAns = N.Attributes[K_SQL].InnerText;
        //            break;
        //        }
        //    return sAns;
        //}
        //string GetSqlById(int id)
        //{
        //    string ids = id.ToString();
        //    string sAns = string.Empty;
        //    foreach (Xml.XmlNode N in _nodes)
        //        if (string.Compare(N.Attributes[K_ID].Value, ids, true) == 0)
        //        {
        //            sAns = N.Attributes[K_SQL].InnerText;
        //            break;
        //        }
        //    return sAns;
        //}
        //string GetSqlByAlias(string alias)
        //{
        //    string sAns = string.Empty;
        //    foreach (Xml.XmlNode N in _nodes)
        //        if (string.Compare(N.Attributes[K_ALIAS].Value, alias, true) == 0)
        //        {
        //            sAns = N.Attributes[K_SQL].InnerText;
        //            break;
        //        }
        //    return sAns;
        //}
        //string GetSql(string key)
        //{
        //    string sAns = string.Empty;

        //    sAns = GetSqlByName(key);
        //    if (sAns.Length == 0)
        //        sAns = GetSqlByAlias(key);
        //    return sAns;
        //}
       
       
        //string BuildSubsetQuery(int ruleid, bool bIncomming = true)
        //{

        //    string query = _engine.BaseQueryDataActions.Get(1).BaseQuery;
        //    query = System.Text.RegularExpressions.Regex.Replace(query, @"\s+", " ");

        //    var x = BuildWhereClause(bIncomming ?
        //        _engine.DuplicateRecordActions.GetFiltersForIncomming(ruleid) :
        //        _engine.DuplicateRecordActions.GetFiltersForExisitng(ruleid),
        //        bIncomming
        //        );

        //    string select = BuildSelectClause(_engine.DuplicateRecordActions.GetDuplicateColumns(ruleid));
        //    query = query.Replace("[ColumnNames]", select);

        //    string where = string.Empty;
        //    foreach (var y in x)
        //        where = where + (where.Length > 0 ? " AND " : "") + y.Value;

        //    query += where;
        //    return query;
        //}
        //string BuildSelectClause(IEnumerable<Models.DuplicateColumn> columns)
        //{
        //    return " leads.lea_key ";
        //}
        //Dictionary<int, string> BuildWhereClause(IEnumerable<Models.FilterLead> filters, bool bIncoming)
        //{
        //    Dictionary<int, string> expressions = new Dictionary<int, string>(filters.Count());

        //    foreach (var filter in filters)
        //    {
        //        string value = (filter.DATATYPE == 9 || filter.DATATYPE == 1) ? string.Format("'{0}'", filter.VALUE) : filter.VALUE;
        //        string sexpr = string.Empty;
        //        sexpr = string.Format("({0}.{1} {2} )", filter.TABLENAME, filter.FILTERCOLUMN, filter.SQLOPERATOR);
        //        sexpr = sexpr.Replace("<X>", value);
        //        expressions[filter.QUERYORDER ?? expressions.Count + 1] = sexpr;
        //    }

        //    //expressions[expressions.Count + 1] = bIncoming ? "(leads.lea_key IN (" + K_LEADLIST + "))" : "(leads.lea_key NOT IN (" + K_LEADLIST + "))";
        //    return expressions;
        //}

        //string StripSelectColumns(string sql)
        //{
        //    string tmp = sql.Substring(sql.IndexOf("SELECT ") + 8);
        //    string cols = tmp.Substring(0, tmp.IndexOf(" FROM ") - 1);
        //    string query = tmp.Substring(tmp.IndexOf("FROM "));
        //    query = "SELECT L.lea_key " + query;
        //    return query;
        //}
        //string BuildQuery(Models.FilterLead filter)
        //{
        //    string sql = _holder.GetSql(filter.TABLENAME);
        //    sql = StripSelectColumns(sql);

        //    string value = (filter.DATATYPE == 9 || filter.DATATYPE == 1) ? string.Format("'{0}'", filter.VALUE) : filter.VALUE;
        //    sql = sql + string.Format(" WHERE ( {0}.{1} {2} )", filter.TABLENAME, filter.FILTERCOLUMN, filter.SQLOPERATOR);
        //    sql = sql.Replace("<X>", value);
        //    return sql;
        //}

        //Dictionary<int, string> BuildQueries(int ruleid)
        //{
        //    IEnumerable<Models.FilterLead> filters = _engine.DuplicateRecordActions.GetFiltersForIncomming(ruleid);
        //    Dictionary<int, string> queries = new Dictionary<int, string>(filters.Count());

        //    foreach (var filter in filters)
        //        queries[filter.QUERYORDER ?? queries.Count + 1] = BuildQuery(filter);

        //    return queries;
        //}

//        string QueryForDuplicates(string table, string column)
//        {
//            string K_SQL = @"Select Count(RankID) from 
//                       (Select RANK() over(partition by XCOLUMN order by XCOLUMN) as RANKID, XCOLUMN from XTABLE) as A
//                        Where XCOLUMN = @value group by RankID";
//            return K_SQL.Replace("XTABLE", table).Replace("XCOLUMN", column);
//        }


        //public long[] GetSubsetofLeads(bool bIncoming, long[] leadIds)
        //{
        //    int[] ruleids = _engine.DuplicateRecordActions.All.Select(x => x.Id).ToArray();
        //    HashSet<long> Leads = new HashSet<long>();

        //    string leadlist = string.Empty;
        //    foreach (int id in leadIds)
        //        leadlist = leadlist + (leadlist.Length > 0 ? ", " : "") + id.ToString();

        //    foreach (var ruleid in ruleids)
        //    {
        //        //string query = BuildSubsetQuery(ruleid, bIncoming);
        //        var queries = BuildQueries(ruleid);
        //        foreach(var query in queries)
        //        DBG.WriteLine(query.Value);
        //        //var x = _engine.adminEntities.ExecuteStoreQuery<long>(query);
        //        //if (x != null)
        //        //    foreach (long i in x)
        //        //        Leads.Add(i);
        //    }
        //    return Leads.ToArray<long>();
        //}
  

   // };
}