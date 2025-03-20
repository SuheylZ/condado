using System;
using System.Linq;
using Xml = System.Xml;
using DBG = System.Diagnostics.Debug;

namespace SalesTool.DataAccess.Extentions
{

    internal class QueryHolder
    {
        const string K_NAME = "name";
        const string K_SQL = "sql";
        const string K_ID = "id";
        const string K_ALIAS = "alias";

        Xml.XmlDocument _doc = null;
        Xml.XmlNodeList _nodes = null;

        //System.Collections.Generic.Dictionary<string, string> _aliases = new System.Collections.Generic.Dictionary<string,string>();

        static string _file = string.Empty;
        static QueryHolder _Instance = null;

        private QueryHolder(string file)
        {
            _doc = new Xml.XmlDocument();
            _doc.Load(file);
            _nodes = _doc.ChildNodes[1].ChildNodes;
            //foreach (Xml.XmlNode N in _nodes) 
            //{ 
            //    string sql = N.Attributes[K_SQL].Value;
            //    AliasExtracter(sql);
            //}
        }

        internal static QueryHolder Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new QueryHolder(_file);
                return _Instance;
            }
        }
        internal static string FilePath
        {
            set
            {
                DBG.Assert(System.IO.File.Exists(value));
                _file = value;
            }
        }

        string GetSqlByName(string name)
        {
            string sAns = string.Empty;
            foreach (Xml.XmlNode N in _nodes)
                if (string.Compare(N.Attributes[K_NAME].Value, name, true) == 0)
                {
                    sAns = N.Attributes[K_SQL].InnerText;
                    break;
                }
            return sAns;
        }
        string GetSqlById(int id)
        {
            string ids = id.ToString();
            string sAns = string.Empty;
            foreach (Xml.XmlNode N in _nodes)
                if (string.Compare(N.Attributes[K_ID].Value, ids, true) == 0)
                {
                    sAns = N.Attributes[K_SQL].InnerText;
                    break;
                }
            return sAns;
        }
        string GetSqlByAlias(string alias)
        {
            string sAns = string.Empty;
            foreach (Xml.XmlNode N in _nodes)
                if (string.Compare(N.Attributes[K_ALIAS].Value, alias, true) == 0)
                {
                    sAns = N.Attributes[K_SQL].InnerText;
                    break;
                }
            return sAns;
        }

/*        internal string this[string table]
        {
            get
            {
                return _aliases[table];
            }
        }*/

        internal string GetSql(string key)
        {
            string sAns = string.Empty;

            sAns = GetSqlByName(key);
            if (sAns.Length == 0)
                sAns = GetSqlByAlias(key);
            return sAns;
        }
        internal string GetSql(int id)
        {
            return GetSqlById(id);
        }

        //void AliasExtracter(string sql)
        //{
        //    string frmClause = string.Empty;

        //    sql = sql.ToLower();

        //    int idx = sql.IndexOf("from ");
            
        //    int len = sql.IndexOf("where");
        //    if (len == -1) len = sql.IndexOf("group by");
        //    else if (len == -1) len = sql.IndexOf("having");
        //    else if (len == -1) len = sql.IndexOf("order by");
        //    else len = sql.Length;

        //    frmClause = sql.Substring(idx, len - idx);
        //    var ary = frmClause.Split(new char[] { ',' });
        //    foreach (string element in ary)
        //    {
        //        int sep = element.IndexOf(" as ");
        //        if(sep>0){
        //            string table = element.Substring(0, sep-1);
        //            string alias = element.Substring(sep +5);
        //            _aliases[table] = alias;
        //        }
        //        else{
        //         sep = element.IndexOf(' ');
        //         if(sep>0){
        //            string table = element.Substring(0, sep-1);
        //            string alias = element.Substring(sep +1);
        //            _aliases[table] = alias;
        //         }
        //         else
        //             _aliases[element] = element;
        //        }
        //    }
        //}
        //internal string GetAlias(string table)
        //{
        //    return _aliases[table];
        //}
    }
}