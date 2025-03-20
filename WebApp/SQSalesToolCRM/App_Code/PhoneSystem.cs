using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SalesTool.DataAccess.Models;

/// <summary>
/// Summary description for PhoneSystem
/// </summary>
public class PhoneSystem
{
    private SalesTool.DataAccess.DBEngine _engine;
    private readonly User _currentUser;

    public PhoneSystem(SalesTool.DataAccess.DBEngine engine, SalesTool.DataAccess.Models.User currentUser)
    {
        _engine = engine;
        _currentUser = currentUser;
    }

    public bool InContactCall(long actId, string phoneNumber, string outPulseId, Action<string, string> action)
    {
        bool status = false;
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);
        UpdateLeadCallInformation(actId, _currentUser.Key);
        inContactAuthorizationResponse authToken;
        JoinSessionResponse sessionResponse;

        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(outPulseId))
        {
            exceptionMessage = "Outpulse ID Not Found.";
            action(exceptionMessage, "inContact Dial");
            return status;
        }
        else if (string.IsNullOrEmpty(_currentUser.PhoneSystemUsername) && string.IsNullOrEmpty(_currentUser.PhoneSystemPassword))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
            action(exceptionMessage, "inContact Dial");
            return status;
        }
        inContactFunctions funcs = new inContactFunctions();
        authToken = funcs.inContactAuthorization(_engine.ApplicationSettings.PhoneSystemAPIGrantType, _engine.ApplicationSettings.PhoneSystemAPIScope, _currentUser.PhoneSystemUsername, _currentUser.PhoneSystemPassword, _engine.ApplicationSettings.PhoneSystemAPIKey, ref exceptionMessage);
        if (authToken == null)
        {
            exceptionMessage = "Unable to authenticate with Softphone.";
            action(exceptionMessage, "inContact Dial");
            return status;
        }
        else
        {
            sessionResponse = funcs.inContactJoinSession(authToken, ref exceptionMessage);
            if (sessionResponse != null)
            {
                exceptionMessage = funcs.inContactDialNumber(authToken, sessionResponse, phoneNumber.Replace("-", ""), outPulseId);
                if (!string.IsNullOrEmpty(exceptionMessage))
                    action(exceptionMessage, "inContact Dial Error");
                else
                {
                    _engine.AccountHistory.LogCall(actId, phoneNumber, _currentUser.Key, "inContact Call Successful");
                    //SZ [Apr 18, 2014] Added for updating the fields of call attempt
                    UpdateLeadCallInformation(actId, _currentUser.Key);
                    //1 check for agent assignment / AccountLog if assigened
                    //2 AddCall
                    //3 If agent_first_call is null, write the call log timestamp to the agent_first_call field
                    _engine.AccountActions.CheckAndAssignAgent(actId, _currentUser.Key);
                    _engine.AccountHistory.AddCall(actId, phoneNumber, _currentUser.Key);
                    //SZ [Apr 18, 2014] Added for updating the fields of call contact
                    UpdateLeadCallInformation(actId, _currentUser.Key, false);
                    _engine.AccountHistory.CheckAndLogAgentFirstCall(_currentUser.Key.ToString(), actId);
                    status = true;
                }
                return status;
            }
            else
            {
                action(exceptionMessage, "inContact Dial");
                return status;
            }
        }
    }

    public bool CiscoCall(long actId, string phoneNumber, string outPulseId, Action<string, string> action)
    {
        bool status = false;
        phoneNumber = Helper.ConvertMaskToPlainText(phoneNumber);
        UpdateLeadCallInformation(actId, _currentUser.Key);

        string exceptionMessage = string.Empty;
        if (string.IsNullOrEmpty(outPulseId))
        {
            exceptionMessage = "Outpulse ID Not Found.";
            action(exceptionMessage, "inContact Dial");
            return status;
        }
        else if (string.IsNullOrEmpty(_currentUser.PhoneSystemUsername) && string.IsNullOrEmpty(_currentUser.PhoneSystemPassword))
        {
            exceptionMessage = "Phone System Credentials Not Found.";
            action(exceptionMessage, "inContact Dial");
            return status;
        }
        CiscoMethods funcs = new CiscoMethods();
        string authCode = System.Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(_currentUser.Cisco_AgentId + ":" + _currentUser.Cisco_AgentPassword));
        funcs.CiscoDialNumber(_currentUser.Cisco_AgentId, authCode, phoneNumber, _currentUser.Cisco_AgentExtension1,ref exceptionMessage);
        if (!string.IsNullOrEmpty(exceptionMessage))
            action(exceptionMessage, "inContact Dial Error");
        else
        {
            _engine.AccountHistory.LogCall(actId, phoneNumber, _currentUser.Key, "inContact Call Successful");
            //SZ [Apr 18, 2014] Added for updating the fields of call attempt
            UpdateLeadCallInformation(actId, _currentUser.Key);
            //1 check for agent assignment / AccountLog if assigened
            //2 AddCall
            //3 If agent_first_call is null, write the call log timestamp to the agent_first_call field
            _engine.AccountActions.CheckAndAssignAgent(actId, _currentUser.Key);
            _engine.AccountHistory.AddCall(actId, phoneNumber, _currentUser.Key);
            //SZ [Apr 18, 2014] Added for updating the fields of call contact
            UpdateLeadCallInformation(actId, _currentUser.Key, false);
            _engine.AccountHistory.CheckAndLogAgentFirstCall(_currentUser.Key.ToString(), actId);
            status = true;
        }
        return status;
    }


    private void UpdateLeadCallInformation(long accId, Guid user, bool bIsCallAttempt = true)
    {
        Account X = _engine.AccountActions.Get(accId);
        Lead L = X.PrimaryLead;
        Action<Lead, Guid> dolog = null;

        if (bIsCallAttempt)
            dolog = (l, key) =>
            {
                l.LastCallAttemptDate = DateTime.Now;
                if (X.AssignedUserKey == key)
                    l.LastCallAttemptAssignedUserDate = DateTime.Now;
                if (X.AssignedCsrKey == key)
                    l.LastCallAttemptCSRUserDate = DateTime.Now;
                if (X.TransferUserKey == key)
                    l.LastCallAttemptTAUserDate = DateTime.Now;
                if (X.AlternateProductUser == key)
                    l.LastCallAttemptAPUserDate = DateTime.Now;
                if (X.OnBoardUser == key)
                    l.LastCallAttemptOBUserDate = DateTime.Now;
            };
        else
            dolog = (l, key) =>
            {
                l.LastCallContactDate = DateTime.Now;
                if (X.AssignedUserKey == key)
                    l.LastCallContactAssignedUserDate = DateTime.Now;
                if (X.AssignedCsrKey == key)
                    l.LastCallContactCSRUserDate = DateTime.Now;
                if (X.TransferUserKey == key)
                    l.LastCallContactTAUserDate = DateTime.Now;
                if (X.AlternateProductUser == key)
                    l.LastCallContactAPUserDate = DateTime.Now;
                if (X.OnBoardUser == key)
                    l.LastCallContactOBUserDate = DateTime.Now;
            };

        try
        {
            dolog(L, user);

            var Usr = _engine.UserActions.Get(user, true);
            string userName = "";
            if (Usr != null)
                userName = Usr.FullName;
            _engine.LeadsActions.Update(L, userName);
        }
        catch (Exception ex)
        {
            //ctlMessage.SetStatus(ex);
        }
    }


    /// <summary>
    /// Generate Screen pop redirection script.
    /// </summary>
    /// <param name="phone"></param>
    /// <param name="campaign"></param>
    /// <param name="status"></param>
    /// <param name="type"></param>
    /// <param name="source"></param>
    /// <param name="accountId"></param>
    /// <returns></returns>
    /// <author>MH:15 April 2014</author>
    public static string GenerateScreenPopScript(string phone, string campaign, string status, string type, string source, long? accountId = null)
    {
        string script;
        string args = GenerateScreenPopLink(phone, campaign, status, type, source, accountId);
        script = String.Format("window.location.href=getBaseURL()+'Leads/Leads.aspx?{0}'", args);
        return script;
    }
    public static string GenerateScreenPopLink(string phone, string campaign, string status, string type, string source, long? accountId = null)
    {
        string phoneNumber = Helper.ConvertMaskToPlainText(phone);
        string script = string.Empty;
        string args = string.Empty;
        args = String.Format("phone={0}&campaignid={1}&statusid={2}&type={3}&source={4}", phoneNumber, campaign, status, type, source);
        if (accountId != null)
            args = string.Format("{0}&account={1}", args, accountId);//account is used instead of accountId to avoid collession
        script = String.Format("Leads/Leads.aspx?{0}", args);
        return script;
    }

}