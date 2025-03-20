using System;
using System.Linq;
using System.Data.SqlClient;


namespace SalesTool.Schema
{
    [Serializable]
    public class MiniUser
    {
        private Guid _key = Guid.Empty;
        private string _fname;
        private string _lname;
        private string _email;

        public string Email
        {
            get
            {
                return this._email;
            }
        }
        public string LastName
        {
            get
            {
                return this._lname;
            }
        }
        public string FirstName
        {
            get
            {
                return this._fname;
            }
        }
        public string FullName
        {
            get
            {
                return string.Format("{1}, {0}", _fname, _lname);
            }
        }

        public MiniUser(Guid rkey, string rfname, string rlname, string remail)
        {
            _key = rkey;
            _fname = rfname;
            _lname = rlname;
            _email = remail;
        }
        internal MiniUser()
        {
            _key=Guid.Empty;
        }

        public static MiniUser Get(Guid rkey)
        {
            const string SPNAME = "proj_GetUserById";
            MiniUser user = new MiniUser();

            using (SqlConnection cnn = new SqlConnection(Common.Konstants.ConnectionString))
            {
                cnn.Open();
                using (SqlCommand cmd = new SqlCommand(SPNAME, cnn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter("@userId", System.Data.SqlDbType.UniqueIdentifier);
                    param.Value = rkey;
                    cmd.Parameters.Add(param);

                    using (System.Data.IDataReader reader = cmd.ExecuteReader(System.Data.CommandBehavior.SingleRow))
                    {
                        while (reader.Read())
                            user = new MiniUser(reader.GetGuid(0), reader.GetString(1), reader.GetString(2), reader.GetString(3));
                        reader.Close();
                    }
                }
            }
            return user;
        }
    }
}
