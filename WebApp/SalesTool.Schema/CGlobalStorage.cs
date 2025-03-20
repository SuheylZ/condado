using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace SalesTool.Common
{

    // SZ [Sep 20, 2013] This class, a much awaited and much one, stores the values in the database in the form of name key pair i.e. GlobalStorage
    // This can be considered eqvilant to the AppSettings of Web.Config where a value is defined against a key and it remains there until teh app lifetime. 
    // Since Web.Config is getting cluttered and changing the Web.Config restarts the application in IIS, Change in the GlobalStorage does not cause app to restart.
    // and teh values can be chnaged dynamically. It can store short, int, float, double, string(254 length max), Guid and boolean values. use it whenever needed.

    // CAUTION:  TRY TO USE KEYS AS UNIQUE AS POSSIBLE TO AVOID ACCIDENTAL OVERWRITES
    // USAGE:  its a singleton class, initialize it with Init() and use through Instance property. calling Dispose() is not necessary  

    //  If the table is missing run the following SQL command
    //    CREATE TABLE [Application_Storage]([Key] nvarchar(40) not null primary key, 
    //      [iValue] int null default 0, 
    //      [fValue] numeric(5, 2) null, 
    //      [tValue] nvarchar(254) null, 
    //      [bValue] bit null, 
    //      [uValue] uniqueidentifier null, 
    //      [dValue] datetime null)

    public class CGlobalStorage
    {
        string _cnnstr = string.Empty;
        static CGlobalStorage This = null;

        const string K_Table = "Application_Storage";

        CGlobalStorage(string cnnStr)
        {
            _cnnstr = cnnStr;
            //if (_cnn == null)
            //{
            //    _cnn = new SqlConnection(_cnnstr);
            //    _cnn.Open();
            //}
        }

        public static void Init()
        {
            if (This == null)
                This = new CGlobalStorage(Konstants.ConnectionString);
        }
        public static CGlobalStorage Instance
        {
            get
            {
                if (This == null)
                    Init();

                return This;
            }
        }
        public void Dispose()
        {
            //if (_cnn != null)
            //{
            //    _cnn.Close();
            //    _cnn.Dispose();
            //    _cnn = null;
            //}
            GC.SuppressFinalize(this);
            This = null;
        }

        bool Exists(string key)
        {
            bool bAns = false;

            object obj = SqlHelper.ExecuteScalar(_cnnstr, CommandType.Text,
                string.Format("SELECT Count(*) FROM {0} WHERE [Key]=@key", K_Table), new SqlParameter[] { new SqlParameter("@key", key) });
            if (obj != null)
                if (!(obj is DBNull))
                    if ((int)obj > 0)
                        bAns = true;

            return bAns;
        }
        string GetColumn<T>()
        {
            // Consider this:  iValue int, fValue numeric(5, 2), tValue nvarchar(254), bValue bit, uValue uniqueidentifier
            T x = default(T);
            return x is short ? "iValue" : x is int ? "iValue" :
                x is float ? "fValue" : x is double ? "fValue" :
                x is char ? "tValue" : x is string ? "tValue" :
                x is bool ? "bValue" :
                x is Guid ? "uValue" :
                x is DateTime ? "dValue" : "tValue";
        }
        T CastToCorrectType<T>(object value)
        {
            object ans = default(T);

            //SZ [Sep 20, 2013] this has been added coz T:string is not correctly identified
            if (value is string) ans = string.Empty;
            if (value is char) ans = '\0';

            if (value != null && !Convert.IsDBNull(value))
            {
                if (ans is short)
                    ans = (short)Convert.ToInt16(value);
                else if (ans is int)
                    ans = (int)Convert.ToInt32(value);
                else if (ans is long)
                    ans = (long)Convert.ToInt64(value);
                else if (ans is float)
                    ans = (float)Convert.ToSingle(value);
                else if (ans is double)
                    ans = (double)Convert.ToDouble(value);
                else if (ans is bool)
                    ans = (bool)Convert.ToBoolean(value);
                else if (ans is char)
                    ans = (char)Convert.ToChar(value);
                else if (ans is string)
                    ans = (string)Convert.ToString(value);
                else if (ans is Guid)
                    ans = new Guid(value.ToString());
                else if (ans is DateTime)
                    ans = (DateTime)Convert.ToDateTime(value.ToString());
            }
            return (T)ans;
        }

        public T Get<T>(string key)
        {
            T Ans = default(T);
            string sql = string.Format("SELECT [{1}] FROM {0} WHERE [Key]=@Key", K_Table, GetColumn<T>());
            if (Exists(key))
            {
                object obj = SqlHelper.ExecuteScalar(_cnnstr, CommandType.Text, sql, new SqlParameter[] { new SqlParameter("@key", key) });
                Ans = CastToCorrectType<T>(obj);
            }
            return Ans;
        }
        public void Set<T>(string key, T Value)
        {
            string sql = Exists(key) ? string.Format("UPDATE [{0}] SET [{1}]=@Value WHERE [Key]=@key", K_Table, GetColumn<T>()) :
                string.Format("INSERT INTO [{0}]([Key], [{1}]) VALUES(@Key, @Value)", K_Table, GetColumn<T>());
            SqlHelper.ExecuteNonQuery(_cnnstr, CommandType.Text, sql,
                new SqlParameter[] { new SqlParameter("@key", key), new SqlParameter("@Value", Value) }
                );
        }
        public void Clear(string key = "")
        {
            if (string.IsNullOrEmpty(key))
                SqlHelper.ExecuteNonQuery(_cnnstr, CommandType.Text, string.Format("DELETE FROM [{0}]", K_Table));
            else
            {
                if (Exists(key))
                    SqlHelper.ExecuteNonQuery(_cnnstr, CommandType.Text,
                        string.Format("DELETE FROM [{0}] WHERE [Key]=@key", K_Table),
                        new SqlParameter[] { new SqlParameter("@key", key) });
            }
        }
    }
}
