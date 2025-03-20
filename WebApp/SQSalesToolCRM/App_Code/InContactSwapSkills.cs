using System;
using System.Web.UI;
using System.Linq;
//using System.ServiceProcess;
using System.Text;
using System.IO;
using System.Timers;
using SalesTool.DataAccess.Models;
using SalesTool.DataAccess;
using System.Data.SqlClient;
using SalesTool.Schema;
using System.Data;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using DBG = System.Diagnostics.Debug;

/// <summary>
/// This class will be responsible for updating for inContact Skill Swaps
/// </summary>
public class InContactSwapSkills : IDisposable
{
    DBEngine _E = null;

    #region Members/Properties



    #endregion

    private InContactSwapSkills()
    {
        _E = new DBEngine();
        _E.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
        //_E.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
        _E.Init(ApplicationSettings.ADOConnectionString);
    }
    public void Dispose()
    {
        if (_E != null)
        {
            _E.Dispose();
            _E = null;
        }
    }


    #region Methods
    public static void Execute(SalesTool.DataAccess.Models.User user, bool btnActiveStatus)
    {
        System.Threading.Thread th = new System.Threading.Thread(() => _InnerExecute(user, btnActiveStatus));
        th.Name = string.Format("InContactSwapSkills Update{0}", DateTime.Now.Ticks.ToString());
        th.IsBackground = true;
        th.Start();
    }
    private static void _InnerExecute(SalesTool.DataAccess.Models.User user, bool btnActiveStatus)
    {

        using (InContactSwapSkills tmp = new InContactSwapSkills())
        {
            DBG.WriteLine("Entering the Run Function at :" + DateTime.Now.ToString());
            tmp.Run(user, btnActiveStatus);
            DBG.WriteLine("Exiting the Run Function at :" + DateTime.Now.ToString());
        }
    }
    /// <summary>
    /// Starting point of setting the inContact Skill Swap
    /// </summary>
    private void Run(SalesTool.DataAccess.Models.User user, bool btnActiveStatus)
    {

        if (user == null) return;
        TableStructure nTable = new TableStructure();
        //string query = ApplicationSettings.PhoneQuerySkillSwap;
        string query = _E.ApplicationSettings.PhoneQuerySkillSwap;

        string valueToReplace = "[LOGGED IN USER]";
        //var user = (Page as SalesBasePage).CurrentUser;
        query = query.Replace(valueToReplace, user.Key.ToString());
        DataTable dtRecords = nTable.GetDatatable(ApplicationSettings.ADOConnectionString, query);

        bool HasError = false;
        if (dtRecords.Rows.Count > 0) SetACDStatus("1", user.Key.ToString());
        for (int i = 0; i < dtRecords.Rows.Count; i++)
        {
            DataRow itemDataRow = dtRecords.Rows[i];
            string strSkillID = itemDataRow["skillId"] == null ? "" : itemDataRow["skillId"].ToString();
            string strAgentID = itemDataRow["agentId"] == null ? "" : itemDataRow["agentId"].ToString();

            if (CallinContactSwap(user, strSkillID, strAgentID, btnActiveStatus))
            {
                HasError = true;
                break;
            }
        }
        if (!HasError)
        {
            SetACDflag(btnActiveStatus, user.Key);
        }
        SetACDStatus("0", user.Key.ToString());
    }

    private void SetACDflag(bool btnActiveStatus, Guid userkey)
    {
        string acdFlag = btnActiveStatus ? "1" : "0";
        //Update ACD button status on successful process completion.
        string query = @"Update users set usr_acd_flag = " + acdFlag + " where usr_key = '" + userkey + "'";
        SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, query);
    }

    private void SetACDStatus(string status, string userKey)
    {
        string query = @"Update users set usr_acd_status_flag = " + status + " where usr_key = '" + userKey + "'";
        SalesTool.Common.SqlHelper.ExecuteNonQuery(ApplicationSettings.ADOConnectionString, CommandType.Text, query);
    }

    private bool CallinContactSwap(SalesTool.DataAccess.Models.User user, string skillID, string agentID, bool IsActive)
    {
        inContactAuthorizationResponse authToken;
        string isActiveString = IsActive ? "true" : "false";

        string exceptionMessage = string.Empty;
        //if (string.IsNullOrEmpty(ApplicationSettings.PhoneSystemAPIUsername) && string.IsNullOrEmpty(ApplicationSettings.PhoneSystemAPIPassword))
        if (string.IsNullOrEmpty(_E.ApplicationSettings.PhoneSystemAPIUsername) && string.IsNullOrEmpty(_E.ApplicationSettings.PhoneSystemAPIPassword))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
            return true;
        }
        inContactFunctions funcs = new inContactFunctions();
        //authToken = funcs.inContactAuthorization(ApplicationSettings.PhoneSystemAPIGrantType, ApplicationSettings.PhoneSystemAPIScope, ApplicationSettings.PhoneSystemAPIUsername, ApplicationSettings.PhoneSystemAPIPassword, ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        authToken = funcs.inContactAuthorization(_E.ApplicationSettings.PhoneSystemAPIGrantType,_E. ApplicationSettings.PhoneSystemAPIScope,_E. ApplicationSettings.PhoneSystemAPIUsername,_E. ApplicationSettings.PhoneSystemAPIPassword, _E.ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        if (authToken == null)
        {
            exceptionMessage = "Unable to authenticate with Softphone.";
        }
        else
        {
            funcs.inContactSkillSwap(authToken, skillID, agentID, isActiveString);
        }
        return false;
    }

    #endregion

}