using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;


namespace SalesTool.Schema
{
    public class ProxySearch: IDisposable
    {
        string _cnnstr = string.Empty;
        IDbConnection _cnn = null;

        public ProxySearch(string cnnStr)
        {
            _cnnstr = cnnStr;
            if (_cnn == null)
            {
                _cnn = new SqlConnection(_cnnstr);
                _cnn.Open();
            }
        }
        public void Init(string configurationfile)
        {
        }
        public void Dispose()
        {
            if (_cnn != null)
            {
                _cnn.Close();
                _cnn.Dispose();
                _cnn = null;
            }
        }

        List<long> ExecuteProcedure(string spName, string phrase)
        {
            List<long> Ans = new List<long>();
            
            using (IDbCommand cmd = _cnn.CreateCommand())
            {
                cmd.CommandText = spName;
                cmd.CommandType = CommandType.StoredProcedure;
                
                IDbDataParameter par = cmd.CreateParameter();
                par.Direction = ParameterDirection.Input;
                par.DbType = DbType.String;
                par.ParameterName = "@phrase";
                par.Value = phrase;
                cmd.Parameters.Add(par);

                try{
                    using (IDataReader rd = cmd.ExecuteReader(CommandBehavior.Default))
                    {
                        while (rd.Read())
                            Ans.Add(rd.GetInt64(0));
                        rd.Close();
                    }
                }
                catch
                {
                    Ans.Clear();
                }
            }
            return Ans;
        }

        public List<long> SearchByPhone(string phrase)
        {
            return ExecuteProcedure("[proj_SearchAccountByPhone]", phrase);
        }

        public List<long> SearchByName(string phrase)
        {
            return ExecuteProcedure("[proj_SearchAccountByName]", phrase);
        }
        public List<long> SearchByFirstName(string phrase)
        {
            return ExecuteProcedure("[proj_SearchAccountByFirstName]", phrase);
        }
        public List<long> SearchByLastName(string phrase)
        {
            return ExecuteProcedure("[proj_SearchAccountByLastName]", phrase);
        }

        public List<long> SearchByAccountId(string phrase)
        {
            return ExecuteProcedure("[proj_SearchAccountById]", phrase);
        }
        public List<long> SearchByAnyField(string phrase)
        {
            return ExecuteProcedure("[proj_SearchAccountByAnyField]", phrase);
        }
        public List<long> SearchByArcReference(string phrase)
        {
            return ExecuteProcedure("[proj_SearchAccountByArcReference]", phrase);
        }
    }

    public class WebServiceHelper : IDisposable
    {
        string _cnnstr = string.Empty;
        IDbConnection _cnn = null;
        
        static WebServiceHelper _THIS = null;
        static int _icount = 0;
        
        public static WebServiceHelper GetInstance(string str)
        {
            if (_THIS == null)
                _THIS = new WebServiceHelper(str);
            _icount++;
            return _THIS;
        }

        private WebServiceHelper(string connectionstr)
        {
            _cnnstr = connectionstr;
            _cnn = new SqlConnection(_cnnstr);
            _cnn.Open();
        }
        public void Dispose()
        {
            _icount--;
            _icount = _icount < 0 ? 0 : _icount;
            if (_cnn != null && _icount==0)
            {
                _cnn.Close();
                _cnn.Dispose();
                _cnn = null;
            }
        }



    }
}
