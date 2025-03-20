// --------------------------------------------------------------------------
// Application:  SQ Sales Tool
// 
// Project:      SalesTool.Direct
// 
// Description: This application is created for Condado Group. the application 
//              is accessible from the condado-02 (QA site)
//              
// 
// Created By:   suheyl.zafar
// Created On:   12/12/2012
// 
// --------------------------------------------------------------------------
// 
  
using System;
using System.Data;
using System.Linq;
using System.Data.SqlClient;

namespace SalesTool.Logging
{
    public enum AuditEvent
    {
        Login = 1,
        Logout = 2,
        SessionTerminate = 3,
        Other = 4
    }

    public sealed class Logging: IDisposable
    {
        private IDbConnection _cnn = null;
        
        private static int icount = 0;
        private static Logging This = null;

        //SZ [Jan 15, 2013] Moved to the K class due to sharing between different classes
        //private const string K_APPLICATION_SERVICES = "ApplicationServices";

        private Logging()
        {
            //string cnnstr = System.Configuration.ConfigurationManager.ConnectionStrings[K_APPLICATION_SERVICES].ConnectionString.Trim();
            _cnn = new SqlConnection(Common.Konstants.ConnectionString);
            _cnn.Open();
        }

        public static Logging Instance
        {
            get{
                if(This==null)
                    This = new Logging();
                icount++;
                return This;
            }
        }
        public void Dispose()
        {
            icount -= 1;
            if (icount <= 0)
            {
                if (This != null)
                {
                    try
                    {
                        _cnn.Close();
                        _cnn.Dispose();
                    }
                    catch { }
                    _cnn = null;
                }
                icount = 0;
                This = null;
            }
        }

        public void Write(AuditEvent auditEvent, string message, Guid userKey = new Guid())
        {
            SqlParameter param = null;

            SqlCommand cmd = (SqlCommand)_cnn.CreateCommand();
            cmd.CommandText = "proj_Log";

            param = cmd.CreateParameter();
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "@type";
            param.SqlDbType = SqlDbType.TinyInt;
            param.Value = (int) auditEvent;
            cmd.Parameters.Add(param);


            param = cmd.CreateParameter();
            param.Direction = ParameterDirection.Input;
            param.ParameterName = "@user";
            param.SqlDbType = SqlDbType.UniqueIdentifier;
            param.Value = userKey;
            cmd.Parameters.Add(param);

            param = cmd.CreateParameter();
            param.Direction = ParameterDirection.Input;
            param.SqlDbType = SqlDbType.NVarChar;
            param.ParameterName = "@notes";
            param.Value = message; 
            cmd.Parameters.Add(param);

            cmd.CommandType = CommandType.StoredProcedure;
            cmd.ExecuteNonQuery();
            cmd.Dispose();
        }
    }
}
