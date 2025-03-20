<%@ WebService Language="C#" Class="SelectCare" %>

using System;
using System.Web;
using System.Web.Services;
using System.Web.Script.Services;
using System.Web.Services.Protocols;
using System.Configuration;
using System.Collections.Generic;
using MODELS=SalesTool.DataAccess.Models;
using SECURITY = System.Web.Security.FormsAuthentication;
using System.Web.Security;


[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
[System.Web.Script.Services.ScriptService]
public class SelectCare  : System.Web.Services.WebService {
    SalesTool.DataAccess.DBEngine _engine=null;

    #region Internal Methods
  
    public SalesTool.DataAccess.DBEngine Engine{
     get{
         if(_engine==null){
             _engine = new SalesTool.DataAccess.DBEngine();
             _engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
            _engine.Init(ApplicationSettings.ADOConnectionString);
         }
         return _engine;
     }   
    }

    private void Init()
    {
        if (_engine == null)
        {
            _engine = new SalesTool.DataAccess.DBEngine();
            _engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
            _engine.Init(ApplicationSettings.ADOConnectionString);
        }
    }
    private void Cleanup()
    {
        if (_engine != null)
        {
            _engine.Dispose();
            _engine = null;
        }
    }
    
    private string SerializeAsJSON(object I)
    {
        System.Web.Script.Serialization.JavaScriptSerializer js = new System.Web.Script.Serialization.JavaScriptSerializer();
        return js.Serialize(I);
    }

    private SalesTool.DataAccess.Models.Lead GetPrmaryLeadByAccountId(long id)
    {
        Init();
        SalesTool.DataAccess.Models.Lead L = Engine.LeadsActions.GetPrimaryLeadByAccountId(id);
        if (L == null)
            throw new InvalidOperationException(string.Format("Erroneous record: The account {0} has no primary lead defined.", id));
        if (L.Account.PrimaryIndividual == null)
            throw new InvalidOperationException(string.Format("Erroneous record: The account {0} has no primary individual defined.", id));
        Cleanup();
        
        return L;
    }
    

    #endregion
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetIndividualForArc(long id) 
    {
        IndividualData I = null;
        //string DateFormat = string.IsNullOrEmpty(ApplicationSettings.NewArcCallDateFormat) ? "MM/DD/YYYY" : ApplicationSettings.NewArcCallDateFormat;
        
        Init();
        string DateFormat = string.IsNullOrEmpty(Engine.ApplicationSettings.NewArcCallDateFormat) ? "MM/dd/yyyy" : Engine.ApplicationSettings.NewArcCallDateFormat;
        SalesTool.DataAccess.Models.Lead L = GetPrmaryLeadByAccountId(id);
        I = new IndividualData();
            I.SourceCode = L.SourceCode;
            I.Title = string.Empty;
            I.FirstName = L.Account.PrimaryIndividual.FirstName?? string.Empty;
            I.MiddleName = L.Account.PrimaryIndividual.MiddleName?? string.Empty;
            I.LastName = L.Account.PrimaryIndividual.LastName?? string.Empty;
            I.Suffix = string.Empty;
            I.Gender = L.Account.PrimaryIndividual.Gender?? string.Empty;
            I.State = L.Account.PrimaryIndividual.ApplicationState.HasValue ? Engine.Constants.GetStateCode(Helper.SafeConvert<byte>(L.Account.PrimaryIndividual.ApplicationState.Value.ToString())) : "";
            I.Birthdate = L.Account.PrimaryIndividual.Birthday.HasValue? L.Account.PrimaryIndividual.Birthday.Value.ToString(DateFormat): "";
            I.indv_key = L.Account.PrimaryIndividual.Key.ToString();//MH:26 March 2014
            I.AccountKey = id.ToString();
        
        Cleanup();
        return SerializeAsJSON(I);
    }

    
    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void WriteAccountLog(long id, string userId, string message)
    {
        try{
            Init();
            Guid user = Guid.Empty;

            if (!string.IsNullOrEmpty(userId))
                user = new Guid(userId);
            
            Engine.AccountHistory.Log(id, message, user); }  
        finally {
            Cleanup();
        }
    }
    
    [WebMethod]
    [ScriptMethod(ResponseFormat= ResponseFormat.Json)]
    public string IsDataSame(long id, string fname, string lname, string  gender, string state, string dob)
    {
        bool Ans = false;
        try
        {
            Init();
            SalesTool.DataAccess.Models.Lead L = GetPrmaryLeadByAccountId(id);
            SalesTool.DataAccess.Models.Individual I = L.Account.PrimaryIndividual;
            
            DateTime dtDob = Helper.SafeConvert<DateTime>(dob);
            byte? istate = Helper.NullConvert<byte>(state);//Helper.SafeConvert<short>(state);


            if (
                I.FirstName.SimilarAs(fname) && 
                I.LastName.SimilarAs(lname) && 
                I.Gender.SimilarAs(gender) &&
                //(I.StateID??-1) == istate &&
                (I.StateID) == istate &&
                I.Birthday.GetValueOrDefault().IsDateEqual(dtDob)
               )
             
                Ans = true;
        }
        catch (Exception ex)
        {
            WriteAccountLog(id, "", ex.Message);
        }
        finally
        {
            Cleanup();
        }
        
        return SerializeAsJSON(Ans);
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string IsDataValid(string id, string fname, string lname, string gender, string state, string dob)
    {
        bool Ans = false;
        long aid = Helper.SafeConvert<long>(id);
        try
        {

            Init();
            SalesTool.DataAccess.Models.Lead L = GetPrmaryLeadByAccountId(aid);
            

            //DateTime dtDob = Helper.SafeConvert<DateTime>(dob);
            //short istate = Helper.SafeConvert<short>(state);
            
            string code = L.SourceCode;

            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(fname) || string.IsNullOrEmpty(lname) ||
                string.IsNullOrEmpty(gender) || string.IsNullOrEmpty(dob))
                Ans = false;
            else
                Ans = true;
        }
        catch (Exception ex)
        {
            WriteAccountLog(aid, "", ex.Message);
        }
        finally
        {
            Cleanup();
        }

        return SerializeAsJSON(Ans);
    }
    
    [WebMethod(EnableSession = true)]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string SaveDataForArc(string id, string fname, string lname, string gender, string appState, string dob,
        string mName, string dayPhone, string evenPhone, string cellPhone, string email, bool emailOptOut, string address1, string address2, string city, string state, string zipCode, string appDate, string parentActId)
    {
        long aid = Helper.SafeConvert<long>(id);
        long? ParentActId = parentActId.ConvertOrDefault<long?>();
        bool newAccountCreated = false;
        bool Ans = false;
        long? DayPhoneParsed=default(long?);
        long? evenPhoneParsed = default(long?);
        long? cellPhoneParsed = default(long?);

        Init();       
        Guid key = new Guid(Session[Konstants.K_USERID].ToString());
        if (!string.IsNullOrEmpty(dayPhone) && dayPhone.Length == 10)
            DayPhoneParsed = dayPhone.ConvertOrDefault<long?>();

        if (!string.IsNullOrEmpty(evenPhone) && evenPhone.Length == 10)
            evenPhoneParsed = evenPhone.ConvertOrDefault<long?>();

        if (!string.IsNullOrEmpty(cellPhone) && cellPhone.Length == 10)
            cellPhoneParsed = cellPhone.ConvertOrDefault<long?>();


        try
        {
            if (aid < 1)
            {
                long leadid = 0;
                //Engine.AccountActions.QuickSave(ref aid, ref leadid, fname, lname, gender,key, Helper.NullConvert<byte>(state), ApplicationSettings.DefaultCampaignId, ApplicationSettings.DefaultStatusId,  ConvertDateIgnoreTimeZoneDifference(dob));
                Engine.AccountActions.QuickSave(ref aid, ref leadid, fname, lname, gender, key, Helper.NullConvert<byte>(appState), Engine.ApplicationSettings.DefaultCampaignId, Engine.ApplicationSettings.DefaultStatusId, ConvertDateIgnoreTimeZoneDifference(dob), mName, DayPhoneParsed, evenPhoneParsed, cellPhoneParsed, email, emailOptOut, address1, address2, city, state == "-1" ? null : state.ConvertOrDefault<Byte?>(), zipCode,ConvertDateIgnoreTimeZoneDifference( appDate),ParentActId);

                Session[Konstants.K_ACCOUNT_ID] = aid;
                Session[Konstants.K_LEAD_ID] = leadid;


                newAccountCreated = true;
                //MH:21 April Fix Object Reference exception
                WriteAccountLog(aid, key.ToString(), string.Format("An account has been created. Url is {0}", aid));
            }

            SalesTool.DataAccess.Models.Lead L = GetPrmaryLeadByAccountId(aid);
            SalesTool.DataAccess.Models.Individual I = L.Account.PrimaryIndividual;

            I.FirstName = fname;
            I.LastName = lname;
            I.Gender = gender;
            I.ApplicationState = Helper.NullConvert<byte>(appState);
            I.Birthday = ConvertDateIgnoreTimeZoneDifference(dob);
            
            //MH: 30 April 2014
            I.MiddleName = mName;
            I.DayPhone = DayPhoneParsed;
            I.EveningPhone = evenPhoneParsed;
            I.CellPhone = cellPhoneParsed;
            
            I.Email = email;
            I.IndividualEmailOptOut = emailOptOut;
            I.Address1 = address1;
            I.Address2 = address2;
            I.City = city;
            I.StateID = state == "-1" ? null: state.ConvertOrDefault<Byte?>();
            I.Zipcode = zipCode;
            I.indv_ap_date = ConvertDateIgnoreTimeZoneDifference(appDate);
            
            Engine.IndividualsActions.Change(I, "");
            Ans = true;
            if (!newAccountCreated)
                aid = -aid;
        }
        catch (Exception ex)
        {
            WriteAccountLog(aid, "", ex.Message);
        }
        finally
        {
            Cleanup();
        }

        return SerializeAsJSON(aid);
    }
    
    private DateTime? ConvertDateIgnoreTimeZoneDifference(string dob)
    {
        DateTime? Ans = null;
        
        if (!string.IsNullOrEmpty(dob)) {
            int i = dob.IndexOf('T');
            if (i != -1)
                dob = dob.Substring(0, i);
            Ans = Helper.NullConvert<DateTime>(dob);
            if (Ans.HasValue && (Ans.Value == DateTime.MinValue || Ans.Value == DateTime.MaxValue))
                Ans = null;
        }

        if (Ans != null)
        {
            //Ans = Ans.Value.AddDays(ApplicationSettings.GetDayDifference);
            Ans = Ans.Value.AddDays(Engine.ApplicationSettings.GetDayDifference);
        }

        return Ans;
    }
    
    
    //-------------------------------------------------
    // SZ [May 5, 2014] Functions for Apply Action
    // The client requested for faster response for UI so implemented this functionality.
    // The page used to perform partial postback for enabling and disabling certain UI elements. (Call Attempt required / Disable Last action)
    // for these, the functions are added. 
    //
    // Do not mess with my comment
    // لا تعبث مع تعليقي
    // Verwirren Sie nicht mit meinem Kommentar
    // Ne salissez pas avec mon commentaire
    // اپنے تبصرہ کے ساتھ پنگا نہیں کرتے
    // मेरी टिप्पणी के साथ गड़बड़ नहीं
    // 不要惹我的评论
    // 私のコメントを台無しにしないでください
    // אל תתעסקו עם ההערה שלי
    // آیا نمی ظرف غذا با نظر من
    // Non mihi nuntius comment
    //-------------------------------------------------

    [Serializable()]
    public class ActionTimer
    {
        public bool valid;
        public bool disable;
        public int duration;
    }

    //[WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    //public string getActionTimer(long accountId, int actionId, string userId)
    //{
    //    ActionTimer Ans = new ActionTimer{valid=false, disable=false, duration=0};
    //    DateTime dtNow = DateTime.Now;
    //    long lduration = default(long);

    //    try
    //    {
    //        /*
    //         * 1. Get the primary lead for the account and check the LastCallDate, LastCallAttemptDate and LastActionDate.
    //         * 2. Get the 2 properties of Action (Disable Next and Call Attempt Required.
    //         * 3. Perform the calc for the last action
    //         * 4. Perform the calc for the call attempt required.
    //         * 5. if any of 3/4 returns true fill the ActionAttributes and return, if both then return the longest duration
    //         * 6. otherwise fill the id and return false
    //         * 7. Make it so that it doesnt affect the world's thinest application suffering from anorexia 
    //        */

    //        MODELS.Lead L = Engine.LeadsActions.GetPrimaryLeadByAccountId(accountId);
    //        MODELS.Action A = Engine.LocalActions.Get(actionId);

    //        if (A.HasDisableAction ?? false)
    //        {
    //            Ans.valid = true;
    //            Ans.disable = true;
    //            Ans.duration = Engine.ApplicationSettings.ActionDisabledSeconds * 1000;
    //        }
    //        if (A.IsCallAttemptRequired?? false)
    //        {
    //            lduration = Engine.ApplicationSettings.ActionDisabledSeconds;
    //            DateTime dt = L.LastCallAttemptDate == null ? L.LastCallContactDate.Value :
    //            L.LastCallContactDate == null ? L.LastCallAttemptDate.Value :
    //            L.LastCallAttemptDate < L.LastCallContactDate ?
    //            L.LastCallContactDate.Value : L.LastCallAttemptDate.Value;

    //            var diff = DateTime.Now.Subtract(dt).TotalSeconds;
    //            if (diff < (double)Engine.ApplicationSettings.CallAttemptRequiredSeconds)
    //            {
    //                Ans.valid = true;
    //                Ans.disable = false;
    //                Ans.duration = Engine.ApplicationSettings.CallAttemptRequiredSeconds > 0 ?
    //                    Engine.ApplicationSettings.CallAttemptRequiredSeconds * 1000 :
    //                    5000;
    //            }
    //        }
    //    }
    //    catch (Exception ex)
    //    {
    //        WriteAccountLog(accountId, userId.ToString(), "Could not retrieve the action attributes. If you keep recieving this error check your database");
    //    }
    //    return SerializeAsJSON(Ans);
    //}



    [WebMethod]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public Guid AuthenticateUser(string user, string pwd)
    {
        Guid gAns = Guid.Empty;
        try
        {
            AspnetSecurity sec = new AspnetSecurity();
            gAns = sec.Login(user, pwd);
        }
        catch (Exception ex)
        {
            //WriteAccountLog(aid, "", ex.Message);
        }
        return gAns;
    }
    
    [WebMethod()]
    //[ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public string GetUserName(Guid key){
        string ans =string.Empty;

        Init();
        var U = Engine.UserActions.Get(key);
        if (U != null)
            ans = U.FullName;
        Cleanup();
        
        return ans;
    }
    
    
}