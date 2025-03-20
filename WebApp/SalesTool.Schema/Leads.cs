using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using SalesTool.Common;

namespace SalesTool.Schema
{
    public class LeadsDirect
    {
        private string _connectionString;

        public LeadsDirect(string connectionString)
        {
            _connectionString = connectionString;
        }

        public long NextPriorityAccount(long actKey, Guid usrKey)
        {
            SqlParameter[] Params = new SqlParameter[2];
            Params[0] = new SqlParameter("@actKey", SqlDbType.BigInt);
            Params[1] = new SqlParameter("@usrKey", SqlDbType.UniqueIdentifier);

            Params[0].Value = actKey;
            Params[1].Value = usrKey;

            object o =SqlHelper.ExecuteScalar(_connectionString, "proj_GetNextPriorityAccount", Params);
            return o != null ? (long)o : default(long);
        }

        public object GetOutpulseIdByLeadId(long actKey, Guid usrKey)
        {
            SqlParameter[] Params = new SqlParameter[2];
            Params[0] = new SqlParameter("@leadId", SqlDbType.BigInt);
            Params[1] = new SqlParameter("@usrKey", SqlDbType.UniqueIdentifier);

            Params[0].Value = actKey;
            Params[1].Value = usrKey;

            return SqlHelper.ExecuteScalar(_connectionString, "proj_GetOutpulseIdByLeadId", Params);
        }

        /// <summary>
        /// [QN, 16-04-2013] In order to improve performace in getting 
        /// next substatus following function is made.
        /// </summary>
        /// <param name="StatusId"></param>
        /// <param name="SubstatusId"></param>
        /// <returns></returns>
        public object GetNextSubstatus(Int64 StatusId, Int64 SubstatusId)
        {
            SqlParameter[] Params = new SqlParameter[2];
            Params[0] = new SqlParameter("@statusID", SqlDbType.BigInt);
            Params[1] = new SqlParameter("@substatusID", SqlDbType.UniqueIdentifier);

            Params[0].Value = StatusId;
            Params[1].Value = SubstatusId;

            return SqlHelper.ExecuteScalar(_connectionString, "proj_GetNextSubStatus", Params);
        }

        public object GetAllCampaigns(Int64 AccountId)
        {
            SqlParameter[] Params = new SqlParameter[1];
            Params[0] = new SqlParameter("@AccountID", SqlDbType.BigInt);

            Params[0].Value = AccountId;
            return SqlHelper.ExecuteReader(_connectionString, CommandType.StoredProcedure, "GetAllCampaigns", Params);
        }


        // SZ [Aug 27, 2013] Calls the stored procedure, based on account id.
        // If the call flag is set, it retrieves the call type and returns the colour.
        public int GetCallType(Guid userId)
        {
            int ans = default(int);
            object o = SqlHelper.ExecuteScalar(_connectionString, "proj_GetCallType", new object[]{userId});
            if(!(o is DBNull) && o!=null)
                ans = Convert.ToInt32(o);
            return ans;
        }
       

    }
}