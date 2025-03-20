using System;
using System.Collections.Generic;

using System.Web.Services;
using SalesTool.DataAccess.Models;
using SalesTool.DataAccess;
using X = System.Configuration.ConfigurationManager;
using System.Xml;
using System.Linq;
using System.ComponentModel.DataAnnotations;



// SZ [Aug 30, 2013] This has been added f
//internal enum RecordType{    Account = 1, Lead = 2, Individual = 3, Home = 5, Driver = 6, Vehicle = 7 };
//internal class OperationStatus
//{
//    public long lRecordId = 0L;
//    public RecordType eType;
//    public bool bExisted = false;

//    internal OperationStatus(long id, RecordType etype, bool existed=false)
//    {
//        lRecordId = id;
//        eType = etype;
//        bExisted = existed;
//    }
//};


[WebService(Namespace = "http://sqsst.condadogroup.com/")]
[WebServiceBinding(ConformsTo = WsiProfiles.None)]
public class Service : System.Web.Services.WebService
{
    protected SalesTool.DataAccess.DBEngine _engine = null;

    DBOperationJournal _journal = null;
    //Stack<OperationStatus> _Stk = new Stack<OperationStatus>();
    ////SZ [Aug 28, 2013] Perform a delete of whatever has been inserted into teh DB so far
    //void Rollback()
    //{
    //    Stack<OperationStatus> stk = _Stk;
    //    while (stk.Count > 0) //SZ [Aug 28, 2013] Pop every item and perform delete. 
    //    {
    //        OperationStatus item = stk.Pop();
    //        try
    //        {
    //            switch (item.eType)
    //            {
    //                case RecordType.Vehicle:
    //                    _engine.VehiclesActions.Delete(item.lRecordId, true);
    //                    break;
    //                case RecordType.Driver:
    //                    _engine.DriverActions.Delete(item.lRecordId, true);
    //                    break;
    //                case RecordType.Home:
    //                    _engine.HomeActions.Delete(item.lRecordId, true);
    //                    break;
    //                case RecordType.Individual:
    //                    _engine.IndividualsActions.Delete(item.lRecordId, true);
    //                    break;
    //                case RecordType.Lead:
    //                    // SZ [Nov 5, 2013] when this lead is removed and it was not pre-existed, set the primary lead to previous primary lead.
    //                    if (!item.bExisted)
    //                    {
    //                        long accid = _engine.LeadsActions.Get(item.lRecordId).AccountId;
    //                        _engine.LeadsActions.Delete(item.lRecordId);
    //                        Lead L = _engine.LeadsActions.All.Where(x => x.AccountId == accid).OrderByDescending(x => x.ChangedOn).FirstOrDefault();
    //                        if (L != null)
    //                        {
    //                            long leadid = L.Key;
    //                            _engine.AccountActions.SetPrimaryLead(accid, leadid);
    //                        }
    //                    }

    //                    break;
    //                case RecordType.Account:
    //                    if(!item.bExisted)
    //                        _engine.AccountActions.Delete(item.lRecordId, true);
    //                    break;
    //            }
    //        }
    //        catch //(Exception ex)
    //        {
    //            // SZ do something useful here. perhaps report hte issue
    //        }
    //    }
    //}
    //void Mark(RecordType type, long Id, bool bExisted = false)
    //{
    //    //SZ [Aug 28, 2013] Code to handle error
    //    _Stk.Push(new OperationStatus(Id, type, bExisted));
    //}
    //SZ [Aug 28, 2013] Checks if all the arguments are blank
    ushort ParamtersBlank(params string[] Args)
    {
        bool bAns = true;
        foreach (string s in Args)
            bAns &= string.IsNullOrEmpty(s);
        return (ushort)(bAns ? 1 : 0);
    }
    ushort _ParamsBlank = 0; // SZ [Aug 30, 2013] Implemented to avoid the blank record insertion
    // Schema [Account|Lead|Primary|Secondary|Driver1|Driver2|Driver3|Vehicle1|Vehicle2|Vehicle3|Home1|Home2|Home3]


    #region Variables
    private long AccountID;
    private string _Account_External_Agent = "";
    private string _Account_Life_Information = "";

    private long IndividualID;
    private long IndividualSecondaryID;
    private long LeadID;
    private string _Lead_Pub_ID = "";
    private string _Lead_Ad_Variation = "";
    private string _Lead_IP_Address = "";
    private string _Lead_DTE_Company_Name = "";
    private string _Lead_Group = "";
    private DateTime _Leads_First_Contact_Appointment;

    //WM - 03.07.2013
    //----------------
    private string _Lead_Tracking_Code = "";
    private string _Lead_Tracking_Information = "";
    private string _Lead_Source_Code = "";
    private string _Lead_Pub_Sub_ID = "";
    private string _Lead_Email_Tracking_Code = "";

    private enum ErrorCodes { InsertSuccess = 1, StateAbbreviation = 2, IndividualRequiredFields = 3, IntDataType = 4, BoolDataType = 5, DateTime = 6, GeneralError = 7, DecimalDataType = 8 };

    //For Primary
    private string _Primary_FirstName = "";
    private string _Primary_LastName = "";
    private string _Primary_Gender;
    private long? _Primary_DayPhone;
    private long? _Primary_EveningPhone;
    private long? _Primary_MobilePhone;
    private long? _Primary_Fax;
    private string _Primary_Address1 = "";
    private string _Primary_Address2 = "";
    private string _Primary_City = "";
    private string _Primary_State = "";
    //private int _Primary_Zip = 0;
    private string _Primary_Zip = "";
    private DateTime _Primary_BirthDate;
    private bool _Primary_Tobacco = false;
    private string _Primary_Email = "";
    private string _Primary_Reference_ID = "";
    private string _Primary_Notes = "";
    private decimal _Primary_HRASubsidyAmount;

    // Attiq: 27-03-2014 : New Field Added. Values: "a/y/n" 
    // "a" = "Not Applicable". null If no value entered.
    private string _primary_tcpa_consent = "";

    //For Secondary
    private string _Secondary_FirstName = "";
    private string _Secondary_LastName = "";
    private string _Secondary_Gender = "";
    private long? _Secondary_DayPhone;
    private long? _Secondary_EveningPhone;
    private long? _Secondary_MobilePhone;
    private long? _Secondary_Fax;
    private string _Secondary_Address1 = "";
    private string _Secondary_Address2 = "";
    private string _Secondary_City = "";
    private string _Secondary_State = "";

    //private int _Secondary_Zip = 0;
    private string _Secondary_Zip = "";

    private DateTime _Secondary_BirthDate;
    private bool _Secondary_Tobacco = false;
    private string _Secondary_Email = "";
    private string _Secondary_Reference_ID = "";
    private string _Secondary_Notes = "";
    private decimal _Secondary_HRASubsidyAmount;

    // Attiq: 27-03-2014 : New Field Added. Values: "a/y/n" 
    // "a" = "Not Applicable". null If no value entered.
    private string _secondary_tcpa_consent = "";

    //For DriverInfo 1
    string _Driver1_DlState { get; set; }
    string _Driver1_MaritalStatus { get; set; }
    string _Driver1_LicenseStatus { get; set; }
    long? _Driver1_AgeLicensed { get; set; }
    long? _Driver1_YearsAtResidence { get; set; }
    string _Driver1_Occupation { get; set; }
    long? _Driver1_YearsWithCompany { get; set; }
    long? _Driver1_YrsInField { get; set; }
    string _Driver1_Education { get; set; }
    long? _Driver1_NmbrIncidents { get; set; }
    string _Driver1_Sr22 { get; set; }
    long? _Driver1_PolicyYears { get; set; }
    string _Driver1_LicenseNumber { get; set; }
    string _Driver1_CurrentCarrier { get; set; }
    string _Driver1_LiabilityLimit { get; set; }
    DateTime? _Driver1_CurrentAutoXDate { get; set; }
    string _Driver1_MedicalPayment { get; set; }
    string _Driver1_TicketsAccidentsClaims { get; set; }
    string _Driver1_IncidentType { get; set; }
    string _Driver1_IncidentDescription { get; set; }
    DateTime? _Driver1_IncidentDate { get; set; }
    decimal? _Driver1_ClaimPaidAmount { get; set; }

    //For DriverInfo 2
    string _Driver2_DlState { get; set; }
    string _Driver2_MaritalStatus { get; set; }
    string _Driver2_LicenseStatus { get; set; }
    long? _Driver2_AgeLicensed { get; set; }
    long? _Driver2_YearsAtResidence { get; set; }
    string _Driver2_Occupation { get; set; }
    long? _Driver2_YearsWithCompany { get; set; }
    long? _Driver2_YrsInField { get; set; }
    string _Driver2_Education { get; set; }
    long? _Driver2_NmbrIncidents { get; set; }
    string _Driver2_Sr22 { get; set; }
    long? _Driver2_PolicyYears { get; set; }
    string _Driver2_LicenseNumber { get; set; }
    string _Driver2_CurrentCarrier { get; set; }
    string _Driver2_LiabilityLimit { get; set; }
    DateTime? _Driver2_CurrentAutoXDate { get; set; }
    string _Driver2_MedicalPayment { get; set; }
    string _Driver2_TicketsAccidentsClaims { get; set; }
    string _Driver2_IncidentType { get; set; }
    string _Driver2_IncidentDescription { get; set; }
    DateTime? _Driver2_IncidentDate { get; set; }
    decimal? _Driver2_ClaimPaidAmount { get; set; }

    //For DriverInfo 3
    string _Driver3_DlState { get; set; }
    string _Driver3_MaritalStatus { get; set; }
    string _Driver3_LicenseStatus { get; set; }
    long? _Driver3_AgeLicensed { get; set; }
    long? _Driver3_YearsAtResidence { get; set; }
    string _Driver3_Occupation { get; set; }
    long? _Driver3_YearsWithCompany { get; set; }
    long? _Driver3_YrsInField { get; set; }
    string _Driver3_Education { get; set; }
    long? _Driver3_NmbrIncidents { get; set; }
    string _Driver3_Sr22 { get; set; }
    long? _Driver3_PolicyYears { get; set; }
    string _Driver3_LicenseNumber { get; set; }
    string _Driver3_CurrentCarrier { get; set; }
    string _Driver3_LiabilityLimit { get; set; }
    DateTime? _Driver3_CurrentAutoXDate { get; set; }
    string _Driver3_MedicalPayment { get; set; }
    string _Driver3_TicketsAccidentsClaims { get; set; }
    string _Driver3_IncidentType { get; set; }
    string _Driver3_IncidentDescription { get; set; }
    DateTime? _Driver3_IncidentDate { get; set; }
    decimal? _Driver3_ClaimPaidAmount { get; set; }

    //For Vehicle Info 1
    long? _Vehicle1_Year { get; set; }
    string _Vehicle1_Make { get; set; }
    string _Vehicle1_Model { get; set; }
    string _Vehicle1_Submodel { get; set; }
    long? _Vehicle1_AnnualMileage { get; set; }
    long? _Vehicle1_WeeklyCommuteDays { get; set; }
    string _Vehicle1_PrimaryUse { get; set; }
    string _Vehicle1_ComprehensiveDeductable { get; set; }
    string _Vehicle1_CollisionDeductable { get; set; }
    string _Vehicle1_SecuritySystem { get; set; }
    string _Vehicle1_WhereParked { get; set; }

    //For Vehicle Info 2
    long? _Vehicle2_Year { get; set; }
    string _Vehicle2_Make { get; set; }
    string _Vehicle2_Model { get; set; }
    string _Vehicle2_Submodel { get; set; }
    long? _Vehicle2_AnnualMileage { get; set; }
    long? _Vehicle2_WeeklyCommuteDays { get; set; }
    string _Vehicle2_PrimaryUse { get; set; }
    string _Vehicle2_ComprehensiveDeductable { get; set; }
    string _Vehicle2_CollisionDeductable { get; set; }
    string _Vehicle2_SecuritySystem { get; set; }
    string _Vehicle2_WhereParked { get; set; }

    //For Vehicle Info 3
    long? _Vehicle3_Year { get; set; }
    string _Vehicle3_Make { get; set; }
    string _Vehicle3_Model { get; set; }
    string _Vehicle3_Submodel { get; set; }
    long? _Vehicle3_AnnualMileage { get; set; }
    long? _Vehicle3_WeeklyCommuteDays { get; set; }
    string _Vehicle3_PrimaryUse { get; set; }
    string _Vehicle3_ComprehensiveDeductable { get; set; }
    string _Vehicle3_CollisionDeductable { get; set; }
    string _Vehicle3_SecuritySystem { get; set; }
    string _Vehicle3_WhereParked { get; set; }

    //For Home 1
    string _Home1_CurrentCarrier { get; set; }
    string _Home1_CurrentXdateLeadInfo { get; set; }
    string _Home1_YearBuilt { get; set; }
    string _Home1_DwellingType { get; set; }
    string _Home1_DesignType { get; set; }
    long? _Home1_RoofAge { get; set; }
    string _Home1_RoofType { get; set; }
    string _Home1_FoundationType { get; set; }
    string _Home1_HeatingType { get; set; }
    string _Home1_ExteriorWallType { get; set; }
    long? _Home1_NumberOfClaims { get; set; }
    long _Home1_NumberOfBedrooms { get; set; }
    long? _Home1_SqFootage { get; set; }
    string _Home1_ReqCoverage { get; set; }
    long? _Home1_NumberOfBathrooms { get; set; }

    //For Home 2
    string _Home2_CurrentCarrier { get; set; }
    string _Home2_CurrentXdateLeadInfo { get; set; }
    string _Home2_YearBuilt { get; set; }
    string _Home2_DwellingType { get; set; }
    string _Home2_DesignType { get; set; }
    long? _Home2_RoofAge { get; set; }
    string _Home2_RoofType { get; set; }
    string _Home2_FoundationType { get; set; }
    string _Home2_HeatingType { get; set; }
    string _Home2_ExteriorWallType { get; set; }
    long? _Home2_NumberOfClaims { get; set; }
    long _Home2_NumberOfBedrooms { get; set; }
    long? _Home2_SqFootage { get; set; }
    string _Home2_ReqCoverage { get; set; }
    long? _Home2_NumberOfBathrooms { get; set; }

    //For Home 3
    string _Home3_CurrentCarrier { get; set; }
    string _Home3_CurrentXdateLeadInfo { get; set; }
    string _Home3_YearBuilt { get; set; }
    string _Home3_DwellingType { get; set; }
    string _Home3_DesignType { get; set; }
    long? _Home3_RoofAge { get; set; }
    string _Home3_RoofType { get; set; }
    string _Home3_FoundationType { get; set; }
    string _Home3_HeatingType { get; set; }
    string _Home3_ExteriorWallType { get; set; }
    long? _Home3_NumberOfClaims { get; set; }
    long _Home3_NumberOfBedrooms { get; set; }
    long? _Home3_SqFootage { get; set; }
    string _Home3_ReqCoverage { get; set; }
    long? _Home3_NumberOfBathrooms { get; set; }


    bool HasSpouce = true;
    private int _CampaignId;
    private int _StatusId;
    private bool ErrorFlag = false;
    private System.Xml.XmlDocument docMain = new System.Xml.XmlDocument();
    System.Xml.XmlNode nodeAccounts;
    #endregion

    Dictionary<string, byte> _States = new Dictionary<string, byte>();
    private const string K_WEBSERVICE = "Web Service";

    public Service()
    {

        //Uncomment the following line if using designed components 
        //InitializeComponent(); 

        if (_engine == null)
        {
            _engine = new SalesTool.DataAccess.DBEngine();
            _engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
            _engine.Init(ApplicationSettings.ADOConnectionString);
            _journal = new DBOperationJournal(_engine);
        }

        foreach (var S in Engine.Constants.States)
            _States[S.Abbreviation] = S.Id;
    }



    [WebMethod(MessageName = "InsertAccountAndDetailsWithAllParams")]
    public XmlDocument InsertAccountAndDetails(string CampaignId, string StatusId,
        string Lead_Pub_ID,
        string Lead_Ad_Variation,
        string Lead_IP_Address,
        string Lead_DTE_Company_Name,
        string Lead_Group,
        string Leads_First_Contact_Appointment,
        //WM - 03.07.2013
        string Lead_Tracking_Code,
        string Lead_Tracking_Information,
        string Lead_Source_Code,
        string Lead_Pub_Sub_ID,
        string Lead_Email_Tracking_Code,

        string Account_External_Agent,
        string Account_Life_Information,

        string Primary_FirstName,
        string Primary_LastName,
        string Primary_Gender,
        string Primary_DayPhone,
        string Primary_EveningPhone,
        string Primary_MobilePhone,
        string Primary_Fax,
        string Primary_Address1,
        string Primary_Address2,
        string Primary_City,
        string PrimaryState,
        string Primary_Zip,
        string Primary_BirthDate,
        string Primary_Tobacco,
        string Primary_Email,
        string Primary_Reference_ID,
        string Primary_Notes,
        string Primary_HRASubsidyAmount,

        string primary_tcpa_consent, // Attiq: 27-03-2014

        string Secondary_FirstName,
        string Secondary_LastName,
        string Secondary_Gender,
        string Secondary_DayPhone,
        string Secondary_EveningPhone,
        string Secondary_MobilePhone,
        string Secondary_Fax,
        string Secondary_Address1,
        string Secondary_Address2,
        string Secondary_City,
        string SecondaryState,
        string Secondary_Zip,
        string Secondary_BirthDate,
        string Secondary_Tobacco,
        string Secondary_Email,
        string Secondary_Reference_ID,
        string Secondary_Notes,
        string Secondary_HRASubsidyAmount,

        string secondary_tcpa_consent, // Attiq: 27-03-2014

        string Driver1_DlState,
        string Driver1_MaritalStatus,
        string Driver1_LicenseStatus,
        string Driver1_AgeLicensed,
        string Driver1_YearsAtResidence,
        string Driver1_Occupation,
        string Driver1_YearsWithCompany,
        string Driver1_YrsInField,
        string Driver1_Education,
        string Driver1_NmbrIncidents,
        string Driver1_Sr22,
        string Driver1_PolicyYears,
        string Driver1_LicenseNumber,
        string Driver1_CurrentCarrier,
        string Driver1_LiabilityLimit,
        string Driver1_CurrentAutoXDate,
        string Driver1_MedicalPayment,
        string Driver1_TicketsAccidentsClaims,
        string Driver1_IncidentType,
        string Driver1_IncidentDescription,
        string Driver1_IncidentDate,
        string Driver1_ClaimPaidAmount,

        string Driver2_DlState,
        string Driver2_MaritalStatus,
        string Driver2_LicenseStatus,
        string Driver2_AgeLicensed,
        string Driver2_YearsAtResidence,
        string Driver2_Occupation,
        string Driver2_YearsWithCompany,
        string Driver2_YrsInField,
        string Driver2_Education,
        string Driver2_NmbrIncidents,
        string Driver2_Sr22,
        string Driver2_PolicyYears,
        string Driver2_LicenseNumber,
        string Driver2_CurrentCarrier,
        string Driver2_LiabilityLimit,
        string Driver2_CurrentAutoXDate,
        string Driver2_MedicalPayment,
        string Driver2_TicketsAccidentsClaims,
        string Driver2_IncidentType,
        string Driver2_IncidentDescription,
        string Driver2_IncidentDate,
        string Driver2_ClaimPaidAmount,

        string Driver3_DlState,
        string Driver3_MaritalStatus,
        string Driver3_LicenseStatus,
        string Driver3_AgeLicensed,
        string Driver3_YearsAtResidence,
        string Driver3_Occupation,
        string Driver3_YearsWithCompany,
        string Driver3_YrsInField,
        string Driver3_Education,
        string Driver3_NmbrIncidents,
        string Driver3_Sr22,
        string Driver3_PolicyYears,
        string Driver3_LicenseNumber,
        string Driver3_CurrentCarrier,
        string Driver3_LiabilityLimit,
        string Driver3_CurrentAutoXDate,
        string Driver3_MedicalPayment,
        string Driver3_TicketsAccidentsClaims,
        string Driver3_IncidentType,
        string Driver3_IncidentDescription,
        string Driver3_IncidentDate,
        string Driver3_ClaimPaidAmount,

        string Vehicle1_Year,
        string Vehicle1_Make,
        string Vehicle1_Model,
        string Vehicle1_Submodel,
        string Vehicle1_AnnualMileage,
        string Vehicle1_WeeklyCommuteDays,
        string Vehicle1_PrimaryUse,
        string Vehicle1_ComprehensiveDeductable,
        string Vehicle1_CollisionDeductable,
        string Vehicle1_SecuritySystem,
        string Vehicle1_WhereParked,

        string Vehicle2_Year,
        string Vehicle2_Make,
        string Vehicle2_Model,
        string Vehicle2_Submodel,
        string Vehicle2_AnnualMileage,
        string Vehicle2_WeeklyCommuteDays,
        string Vehicle2_PrimaryUse,
        string Vehicle2_ComprehensiveDeductable,
        string Vehicle2_CollisionDeductable,
        string Vehicle2_SecuritySystem,
        string Vehicle2_WhereParked,

        string Vehicle3_Year,
        string Vehicle3_Make,
        string Vehicle3_Model,
        string Vehicle3_Submodel,
        string Vehicle3_AnnualMileage,
        string Vehicle3_WeeklyCommuteDays,
        string Vehicle3_PrimaryUse,
        string Vehicle3_ComprehensiveDeductable,
        string Vehicle3_CollisionDeductable,
        string Vehicle3_SecuritySystem,
        string Vehicle3_WhereParked,


        string Home1_CurrentCarrier,
        string Home1_CurrentXdateLeadInfo,
        string Home1_YearBuilt,
        string Home1_DwellingType,
        string Home1_DesignType,
        string Home1_RoofAge,
        string Home1_RoofType,
        string Home1_FoundationType,
        string Home1_HeatingType,
        string Home1_ExteriorWallType,
        string Home1_NumberOfClaims,
        string Home1_NumberOfBedrooms,
        string Home1_SqFootage,
        string Home1_ReqCoverage,
        string Home1_NumberOfBathrooms,

        string Home2_CurrentCarrier,
        string Home2_CurrentXdateLeadInfo,
        string Home2_YearBuilt,
        string Home2_DwellingType,
        string Home2_DesignType,
        string Home2_RoofAge,
        string Home2_RoofType,
        string Home2_FoundationType,
        string Home2_HeatingType,
        string Home2_ExteriorWallType,
        string Home2_NumberOfClaims,
        string Home2_NumberOfBedrooms,
        string Home2_SqFootage,
        string Home2_ReqCoverage,
        string Home2_NumberOfBathrooms,

        string Home3_CurrentCarrier,
        string Home3_CurrentXdateLeadInfo,
        string Home3_YearBuilt,
        string Home3_DwellingType,
        string Home3_DesignType,
        string Home3_RoofAge,
        string Home3_RoofType,
        string Home3_FoundationType,
        string Home3_HeatingType,
        string Home3_ExteriorWallType,
        string Home3_NumberOfClaims,
        string Home3_NumberOfBedrooms,
        string Home3_SqFootage,
        string Home3_ReqCoverage,
        string Home3_NumberOfBathrooms

        )
    {
        try
        {

            //Lead_Pub_ID = "852255";
            //Primary_FirstName = "FirstName";
            //Primary_LastName = "LastName";
            //Primary_HRASubsidyAmount = "10";
            //CampaignId = "2";
            //StatusId = "2";
            //Secondary_FirstName = "2nd First Name";
            //Secondary_LastName = "2nd Last Name";
            //Secondary_HRASubsidyAmount = "10.2";

            docMain = new System.Xml.XmlDocument();
            nodeAccounts = docMain.AppendChild(docMain.CreateElement("Accounts"));

            _ParamsBlank |= (ushort)(ParamtersBlank(Lead_Pub_ID, Lead_Ad_Variation, Lead_IP_Address, Lead_DTE_Company_Name, Lead_Group,
                                                Leads_First_Contact_Appointment, Lead_Tracking_Code, Lead_Tracking_Information,
                                                Lead_Source_Code, Lead_Pub_Sub_ID, Lead_Email_Tracking_Code, Account_External_Agent,
                                                Account_Life_Information) << 2);

            _ParamsBlank |= (ushort)(ParamtersBlank(Primary_FirstName, Primary_LastName, Primary_Gender,
                                                Primary_DayPhone, Primary_EveningPhone, Primary_MobilePhone, Primary_Fax,
                                                Primary_Address1, Primary_Address2, Primary_City, PrimaryState, Primary_Zip,
                                                Primary_BirthDate, Primary_Tobacco, Primary_Email, Primary_Reference_ID, Primary_Notes, Primary_HRASubsidyAmount, primary_tcpa_consent) << 3);

            _ParamsBlank |= (ushort)(ParamtersBlank(Secondary_FirstName, Secondary_LastName, Secondary_Gender,
                                                Secondary_DayPhone, Secondary_EveningPhone, Secondary_MobilePhone, Secondary_Fax,
                                                Secondary_Address1, Secondary_Address2, Secondary_City, SecondaryState, Secondary_Zip,
                                                Secondary_BirthDate, Secondary_Tobacco, Secondary_Email, Secondary_Reference_ID, Secondary_Notes, Secondary_HRASubsidyAmount, secondary_tcpa_consent) << 4);

            _ParamsBlank |= (ushort)(ParamtersBlank(Driver1_DlState, Driver1_MaritalStatus, Driver1_LicenseStatus, Driver1_AgeLicensed,
                                                Driver1_YearsAtResidence, Driver1_Occupation, Driver1_YearsWithCompany,
                                                Driver1_YrsInField, Driver1_Education, Driver1_NmbrIncidents,
                                                Driver1_Sr22, Driver1_PolicyYears, Driver1_LicenseNumber,
                                                Driver1_CurrentCarrier, Driver1_LiabilityLimit, Driver1_CurrentAutoXDate,
                                                Driver1_MedicalPayment, Driver1_TicketsAccidentsClaims, Driver1_IncidentType, Driver1_IncidentDescription,
                                                Driver1_IncidentDate, Driver1_ClaimPaidAmount) << 5);
            _ParamsBlank |= (ushort)(ParamtersBlank(Driver2_DlState, Driver2_MaritalStatus, Driver2_LicenseStatus, Driver2_AgeLicensed,
                                                Driver2_YearsAtResidence, Driver2_Occupation, Driver2_YearsWithCompany,
                                                Driver2_YrsInField, Driver2_Education, Driver2_NmbrIncidents,
                                                Driver2_Sr22, Driver2_PolicyYears, Driver2_LicenseNumber,
                                                Driver2_CurrentCarrier, Driver2_LiabilityLimit, Driver2_CurrentAutoXDate,
                                                Driver2_MedicalPayment, Driver2_TicketsAccidentsClaims, Driver2_IncidentType, Driver2_IncidentDescription,
                                                Driver2_IncidentDate, Driver2_ClaimPaidAmount) << 6);
            _ParamsBlank |= (ushort)(ParamtersBlank(Driver3_DlState, Driver3_MaritalStatus, Driver3_LicenseStatus, Driver3_AgeLicensed,
                                                Driver3_YearsAtResidence, Driver3_Occupation, Driver3_YearsWithCompany,
                                                Driver3_YrsInField, Driver3_Education, Driver3_NmbrIncidents,
                                                Driver3_Sr22, Driver3_PolicyYears, Driver3_LicenseNumber,
                                                Driver3_CurrentCarrier, Driver3_LiabilityLimit, Driver3_CurrentAutoXDate,
                                                Driver3_MedicalPayment, Driver3_TicketsAccidentsClaims, Driver3_IncidentType, Driver3_IncidentDescription,
                                                Driver3_IncidentDate, Driver3_ClaimPaidAmount) << 7);

            _ParamsBlank |= (ushort)(ParamtersBlank(Vehicle1_Year, Vehicle1_Make, Vehicle1_Model, Vehicle1_Submodel,
                                                Vehicle1_AnnualMileage, Vehicle1_WeeklyCommuteDays, Vehicle1_PrimaryUse,
                                                Vehicle1_ComprehensiveDeductable, Vehicle1_CollisionDeductable, Vehicle1_SecuritySystem,
                                                Vehicle1_WhereParked) << 8);
            _ParamsBlank |= (ushort)(ParamtersBlank(Vehicle2_Year, Vehicle2_Make, Vehicle2_Model, Vehicle2_Submodel,
                                                 Vehicle2_AnnualMileage, Vehicle2_WeeklyCommuteDays, Vehicle2_PrimaryUse,
                                                 Vehicle2_ComprehensiveDeductable, Vehicle2_CollisionDeductable, Vehicle2_SecuritySystem,
                                                 Vehicle2_WhereParked) << 9);
            _ParamsBlank |= (ushort)(ParamtersBlank(Vehicle3_Year, Vehicle3_Make, Vehicle3_Model, Vehicle3_Submodel,
                                                 Vehicle3_AnnualMileage, Vehicle3_WeeklyCommuteDays, Vehicle3_PrimaryUse,
                                                 Vehicle3_ComprehensiveDeductable, Vehicle3_CollisionDeductable, Vehicle3_SecuritySystem,
                                                 Vehicle3_WhereParked) << 10);

            _ParamsBlank |= (ushort)(ParamtersBlank(Home1_CurrentCarrier, Home1_CurrentXdateLeadInfo, Home1_YearBuilt, Home1_DwellingType,
                                                 Home1_DesignType, Home1_RoofAge, Home1_RoofType, Home1_FoundationType, Home1_HeatingType,
                                                 Home1_ExteriorWallType, Home1_NumberOfClaims, Home1_NumberOfBedrooms, Home1_SqFootage,
                                                 Home1_ReqCoverage, Home1_NumberOfBathrooms) << 11);
            _ParamsBlank |= (ushort)(ParamtersBlank(Home2_CurrentCarrier, Home2_CurrentXdateLeadInfo, Home2_YearBuilt, Home2_DwellingType,
                                                 Home2_DesignType, Home2_RoofAge, Home2_RoofType, Home2_FoundationType, Home2_HeatingType,
                                                 Home2_ExteriorWallType, Home2_NumberOfClaims, Home2_NumberOfBedrooms, Home2_SqFootage,
                                                 Home2_ReqCoverage, Home2_NumberOfBathrooms) << 12);
            _ParamsBlank |= (ushort)(ParamtersBlank(Home3_CurrentCarrier, Home3_CurrentXdateLeadInfo, Home3_YearBuilt, Home3_DwellingType,
                                                 Home3_DesignType, Home3_RoofAge, Home3_RoofType, Home3_FoundationType, Home3_HeatingType,
                                                 Home3_ExteriorWallType, Home3_NumberOfClaims, Home3_NumberOfBedrooms, Home3_SqFootage,
                                                 Home3_ReqCoverage, Home3_NumberOfBathrooms) << 13);

            //Set driver1 params
            InitializeDriver1_InfoParams(
                Driver1_DlState,
                Driver1_MaritalStatus,
                Driver1_LicenseStatus,
                Driver1_AgeLicensed,
                Driver1_YearsAtResidence,
                Driver1_Occupation,
                Driver1_YearsWithCompany,
                Driver1_YrsInField,
                Driver1_Education,
                Driver1_NmbrIncidents,
                Driver1_Sr22,
                Driver1_PolicyYears,
                Driver1_LicenseNumber,
                Driver1_CurrentCarrier,
                Driver1_LiabilityLimit,
                Driver1_CurrentAutoXDate,
                Driver1_MedicalPayment,
                Driver1_TicketsAccidentsClaims,
                Driver1_IncidentType,
                Driver1_IncidentDescription,
                Driver1_IncidentDate,
                Driver1_ClaimPaidAmount);
            //Set driver2 params
            InitializeDriver2_InfoParams(
                Driver2_DlState,
                Driver2_MaritalStatus,
                Driver2_LicenseStatus,
                Driver2_AgeLicensed,
                Driver2_YearsAtResidence,
                Driver2_Occupation,
                Driver2_YearsWithCompany,
                Driver2_YrsInField,
                Driver2_Education,
                Driver2_NmbrIncidents,
                Driver2_Sr22,
                Driver2_PolicyYears,
                Driver2_LicenseNumber,
                Driver2_CurrentCarrier,
                Driver2_LiabilityLimit,
                Driver2_CurrentAutoXDate,
                Driver2_MedicalPayment,
                Driver2_TicketsAccidentsClaims,
                Driver2_IncidentType,
                Driver2_IncidentDescription,
                Driver2_IncidentDate,
                Driver2_ClaimPaidAmount);
            //Set driver3 params
            InitializeDriver3_InfoParams(
                Driver3_DlState,
                Driver3_MaritalStatus,
                Driver3_LicenseStatus,
                Driver3_AgeLicensed,
                Driver3_YearsAtResidence,
                Driver3_Occupation,
                Driver3_YearsWithCompany,
                Driver3_YrsInField,
                Driver3_Education,
                Driver3_NmbrIncidents,
                Driver3_Sr22,
                Driver3_PolicyYears,
                Driver3_LicenseNumber,
                Driver3_CurrentCarrier,
                Driver3_LiabilityLimit,
                Driver3_CurrentAutoXDate,
                Driver3_MedicalPayment,
                Driver3_TicketsAccidentsClaims,
                Driver3_IncidentType,
                Driver3_IncidentDescription,
                Driver3_IncidentDate,
                Driver3_ClaimPaidAmount);
            //Set vehicle1 params
            InitializeVehicle1_InfoParams(
                Vehicle1_Year,
                Vehicle1_Make,
                Vehicle1_Model,
                Vehicle1_Submodel,
                Vehicle1_AnnualMileage,
                Vehicle1_WeeklyCommuteDays,
                Vehicle1_PrimaryUse,
                Vehicle1_ComprehensiveDeductable,
                Vehicle1_CollisionDeductable,
                Vehicle1_SecuritySystem,
                Vehicle1_WhereParked);
            //Set vehicle2 params
            InitializeVehicle2_InfoParams(
                Vehicle2_Year,
                Vehicle2_Make,
                Vehicle2_Model,
                Vehicle2_Submodel,
                Vehicle2_AnnualMileage,
                Vehicle2_WeeklyCommuteDays,
                Vehicle2_PrimaryUse,
                Vehicle2_ComprehensiveDeductable,
                Vehicle2_CollisionDeductable,
                Vehicle2_SecuritySystem,
                Vehicle2_WhereParked);
            //Set vehicle3 params
            InitializeVehicle3_InfoParams(
                Vehicle3_Year,
                Vehicle3_Make,
                Vehicle3_Model,
                Vehicle3_Submodel,
                Vehicle3_AnnualMileage,
                Vehicle3_WeeklyCommuteDays,
                Vehicle3_PrimaryUse,
                Vehicle3_ComprehensiveDeductable,
                Vehicle3_CollisionDeductable,
                Vehicle3_SecuritySystem,
                Vehicle3_WhereParked);
            //Set home1 params
            InitializeHome1_InfoParams(
                Home1_CurrentCarrier,
                Home1_CurrentXdateLeadInfo,
                Home1_YearBuilt,
                Home1_DwellingType,
                Home1_DesignType,
                Home1_RoofAge,
                Home1_RoofType,
                Home1_FoundationType,
                Home1_HeatingType,
                Home1_ExteriorWallType,
                Home1_NumberOfClaims,
                Home1_NumberOfBedrooms,
                Home1_SqFootage,
                Home1_ReqCoverage,
                Home1_NumberOfBathrooms);
            //Set home2 params
            InitializeHome2_InfoParams(
                Home2_CurrentCarrier,
                Home2_CurrentXdateLeadInfo,
                Home2_YearBuilt,
                Home2_DwellingType,
                Home2_DesignType,
                Home2_RoofAge,
                Home2_RoofType,
                Home2_FoundationType,
                Home2_HeatingType,
                Home2_ExteriorWallType,
                Home2_NumberOfClaims,
                Home2_NumberOfBedrooms,
                Home2_SqFootage,
                Home2_ReqCoverage,
                Home2_NumberOfBathrooms);
            //Set home3 params
            InitializeHome3_InfoParams(
                Home3_CurrentCarrier,
                Home3_CurrentXdateLeadInfo,
                Home3_YearBuilt,
                Home3_DwellingType,
                Home3_DesignType,
                Home3_RoofAge,
                Home3_RoofType,
                Home3_FoundationType,
                Home3_HeatingType,
                Home3_ExteriorWallType,
                Home3_NumberOfClaims,
                Home3_NumberOfBedrooms,
                Home3_SqFootage,
                Home3_ReqCoverage,
                Home3_NumberOfBathrooms);
            //Logger.Logfile("**********************", ApplicationSettings.LogFilePath);
            Logger.Logfile("**********************", Engine.ApplicationSettings.LogFilePath);
            //Logger.Logfile("Initializing Params.....", ApplicationSettings.LogFilePath);
            Logger.Logfile("Initializing Params.....", Engine.ApplicationSettings.LogFilePath);
            //Set CampaginID and StatusID, Lead, Account, Primary and Secondary Params
            InitilizeParamsAndDetails(CampaignId, StatusId, Lead_Pub_ID, Lead_Ad_Variation, Lead_IP_Address, Lead_DTE_Company_Name, Lead_Group, Leads_First_Contact_Appointment
                //WM - 03.07.2013
                , Lead_Tracking_Code, Lead_Tracking_Information, Lead_Source_Code, Lead_Pub_Sub_ID, Lead_Email_Tracking_Code
                , Account_External_Agent, Account_Life_Information, Primary_FirstName, Primary_LastName, Primary_Gender, Primary_DayPhone, Primary_EveningPhone, Primary_MobilePhone, Primary_Fax, Primary_Address1, Primary_Address2, Primary_City, PrimaryState, Primary_Zip, Primary_BirthDate, Primary_Tobacco, Primary_Email, Primary_Reference_ID, Primary_Notes, Secondary_FirstName, Secondary_LastName, Secondary_Gender, Secondary_DayPhone, Secondary_EveningPhone, Secondary_MobilePhone, Secondary_Fax, Secondary_Address1, Secondary_Address2, Secondary_City, SecondaryState, Secondary_Zip, Secondary_BirthDate, Secondary_Tobacco, Secondary_Email, Secondary_Reference_ID, Secondary_Notes, Primary_HRASubsidyAmount, Secondary_HRASubsidyAmount
                , primary_tcpa_consent, secondary_tcpa_consent);
            if (!ErrorFlag)
            {
                //Insert records for Account, Primary, Secondary, Home, vehicle and driver info.
                InsertAllRecords(true);
            }
            return docMain;
        }
        catch (Exception ex)
        {
            ErrorXML(AccountID.ToString(), ErrorCodes.GeneralError.ToString(), ShowErrorDetails(ex), "");
            return docMain;
        }
    }


    private void InitializeDriver1_InfoParams(string Driver1_DlState = "",
                                            string Driver1_MaritalStatus = "",
                                            string Driver1_LicenseStatus = "",
                                            string Driver1_AgeLicensed = "",
                                            string Driver1_YearsAtResidence = "",
                                            string Driver1_Occupation = "",
                                            string Driver1_YearsWithCompany = "",
                                            string Driver1_YrsInField = "",
                                            string Driver1_Education = "",
                                            string Driver1_NmbrIncidents = "",
                                            string Driver1_Sr22 = "",
                                            string Driver1_PolicyYears = "",
                                            string Driver1_LicenseNumber = "",
                                            string Driver1_CurrentCarrier = "",
                                            string Driver1_LiabilityLimit = "",
                                            string Driver1_CurrentAutoXDate = "",
                                            string Driver1_MedicalPayment = "",
                                            string Driver1_TicketsAccidentsClaims = "",
                                            string Driver1_IncidentType = "",
                                            string Driver1_IncidentDescription = "",
                                            string Driver1_IncidentDate = "",
                                            string Driver1_ClaimPaidAmount = "")
    {
        _Driver1_DlState = Driver1_DlState;
        _Driver1_MaritalStatus = Driver1_MaritalStatus;
        _Driver1_LicenseStatus = Driver1_LicenseStatus;
        _Driver1_AgeLicensed = ExtractLong(Driver1_AgeLicensed, "Driver1_AgeLicensed");
        _Driver1_YearsAtResidence = ExtractLong(Driver1_YearsAtResidence, "Driver1_YearsAtResidence");
        _Driver1_Occupation = Driver1_Occupation;
        _Driver1_YearsWithCompany = ExtractLong(Driver1_YearsWithCompany, "Driver1_YearsWithCompany");
        _Driver1_YrsInField = ExtractLong(Driver1_YrsInField, "Driver1_YrsInField");
        _Driver1_Education = Driver1_Education;
        _Driver1_NmbrIncidents = ExtractLong(Driver1_NmbrIncidents, "Driver1_NmbrIncidents");
        _Driver1_Sr22 = Driver1_Sr22;
        _Driver1_PolicyYears = ExtractLong(Driver1_PolicyYears, "Driver1_PolicyYears");
        _Driver1_LicenseNumber = Driver1_LicenseNumber;
        _Driver1_CurrentCarrier = Driver1_CurrentCarrier;
        _Driver1_LiabilityLimit = Driver1_LiabilityLimit;
        _Driver1_CurrentAutoXDate = ExtractDateTime(Driver1_CurrentAutoXDate, "Driver1_CurrentAutoXDate");
        _Driver1_MedicalPayment = Driver1_MedicalPayment;
        _Driver1_TicketsAccidentsClaims = Driver1_TicketsAccidentsClaims;
        _Driver1_IncidentType = Driver1_IncidentType;
        _Driver1_IncidentDescription = Driver1_IncidentDescription;
        _Driver1_IncidentDate = ExtractDateTime(Driver1_IncidentDate, "Driver1_IncidentDate");
        _Driver1_ClaimPaidAmount = ExtractInteger(Driver1_ClaimPaidAmount, "Driver1_ClaimPaidAmount");
    }

    private void InitializeDriver2_InfoParams(string Driver2_DlState = "",
                                            string Driver2_MaritalStatus = "",
                                            string Driver2_LicenseStatus = "",
                                            string Driver2_AgeLicensed = "",
                                            string Driver2_YearsAtResidence = "",
                                            string Driver2_Occupation = "",
                                            string Driver2_YearsWithCompany = "",
                                            string Driver2_YrsInField = "",
                                            string Driver2_Education = "",
                                            string Driver2_NmbrIncidents = "",
                                            string Driver2_Sr22 = "",
                                            string Driver2_PolicyYears = "",
                                            string Driver2_LicenseNumber = "",
                                            string Driver2_CurrentCarrier = "",
                                            string Driver2_LiabilityLimit = "",
                                            string Driver2_CurrentAutoXDate = "",
                                            string Driver2_MedicalPayment = "",
                                            string Driver2_TicketsAccidentsClaims = "",
                                            string Driver2_IncidentType = "",
                                            string Driver2_IncidentDescription = "",
                                            string Driver2_IncidentDate = "",
                                            string Driver2_ClaimPaidAmount = "")
    {
        _Driver2_DlState = Driver2_DlState;
        _Driver2_MaritalStatus = Driver2_MaritalStatus;
        _Driver2_LicenseStatus = Driver2_LicenseStatus;
        _Driver2_AgeLicensed = ExtractLong(Driver2_AgeLicensed, "Driver2_AgeLicensed");
        _Driver2_YearsAtResidence = ExtractLong(Driver2_YearsAtResidence, "Driver2_YearsAtResidence");
        _Driver2_Occupation = Driver2_Occupation;
        _Driver2_YearsWithCompany = ExtractLong(Driver2_YearsWithCompany, "Driver2_YearsWithCompany");
        _Driver2_YrsInField = ExtractLong(Driver2_YrsInField, "Driver2_YrsInField");
        _Driver2_Education = Driver2_Education;
        _Driver2_NmbrIncidents = ExtractLong(Driver2_NmbrIncidents, "Driver2_NmbrIncidents");
        _Driver2_Sr22 = Driver2_Sr22;
        _Driver2_PolicyYears = ExtractLong(Driver2_PolicyYears, "Driver2_PolicyYears");
        _Driver2_LicenseNumber = Driver2_LicenseNumber;
        _Driver2_CurrentCarrier = Driver2_CurrentCarrier;
        _Driver2_LiabilityLimit = Driver2_LiabilityLimit;
        _Driver2_CurrentAutoXDate = ExtractDateTime(Driver2_CurrentAutoXDate, "Driver2_CurrentAutoXDate");
        _Driver2_MedicalPayment = Driver2_MedicalPayment;
        _Driver2_TicketsAccidentsClaims = Driver2_TicketsAccidentsClaims;
        _Driver2_IncidentType = Driver2_IncidentType;
        _Driver2_IncidentDescription = Driver2_IncidentDescription;
        _Driver2_IncidentDate = ExtractDateTime(Driver2_IncidentDate, "Driver2_IncidentDate");
        _Driver2_ClaimPaidAmount = ExtractInteger(Driver2_ClaimPaidAmount, "Driver2_ClaimPaidAmount");
    }

    private void InitializeDriver3_InfoParams(string Driver3_DlState = "",
                                            string Driver3_MaritalStatus = "",
                                            string Driver3_LicenseStatus = "",
                                            string Driver3_AgeLicensed = "",
                                            string Driver3_YearsAtResidence = "",
                                            string Driver3_Occupation = "",
                                            string Driver3_YearsWithCompany = "",
                                            string Driver3_YrsInField = "",
                                            string Driver3_Education = "",
                                            string Driver3_NmbrIncidents = "",
                                            string Driver3_Sr22 = "",
                                            string Driver3_PolicyYears = "",
                                            string Driver3_LicenseNumber = "",
                                            string Driver3_CurrentCarrier = "",
                                            string Driver3_LiabilityLimit = "",
                                            string Driver3_CurrentAutoXDate = "",
                                            string Driver3_MedicalPayment = "",
                                            string Driver3_TicketsAccidentsClaims = "",
                                            string Driver3_IncidentType = "",
                                            string Driver3_IncidentDescription = "",
                                            string Driver3_IncidentDate = "",
                                            string Driver3_ClaimPaidAmount = "")
    {
        _Driver3_DlState = Driver3_DlState;
        _Driver3_MaritalStatus = Driver3_MaritalStatus;
        _Driver3_LicenseStatus = Driver3_LicenseStatus;
        _Driver3_AgeLicensed = ExtractLong(Driver3_AgeLicensed, "Driver3_AgeLicensed");
        _Driver3_YearsAtResidence = ExtractLong(Driver3_YearsAtResidence, "Driver3_YearsAtResidence");
        _Driver3_Occupation = Driver3_Occupation;
        _Driver3_YearsWithCompany = ExtractLong(Driver3_YearsWithCompany, "Driver3_YearsWithCompany");
        _Driver3_YrsInField = ExtractLong(Driver3_YrsInField, "Driver3_YrsInField");
        _Driver3_Education = Driver3_Education;
        _Driver3_NmbrIncidents = ExtractLong(Driver3_NmbrIncidents, "Driver3_NmbrIncidents");
        _Driver3_Sr22 = Driver3_Sr22;
        _Driver3_PolicyYears = ExtractLong(Driver3_PolicyYears, "Driver3_PolicyYears");
        _Driver3_LicenseNumber = Driver3_LicenseNumber;
        _Driver3_CurrentCarrier = Driver3_CurrentCarrier;
        _Driver3_LiabilityLimit = Driver3_LiabilityLimit;
        _Driver3_CurrentAutoXDate = ExtractDateTime(Driver3_CurrentAutoXDate, "Driver3_CurrentAutoXDate");
        _Driver3_MedicalPayment = Driver3_MedicalPayment;
        _Driver3_TicketsAccidentsClaims = Driver3_TicketsAccidentsClaims;
        _Driver3_IncidentType = Driver3_IncidentType;
        _Driver3_IncidentDescription = Driver3_IncidentDescription;
        _Driver3_IncidentDate = ExtractDateTime(Driver3_IncidentDate, "Driver3_IncidentDate");
        _Driver3_ClaimPaidAmount = ExtractInteger(Driver3_ClaimPaidAmount, "Driver3_ClaimPaidAmount");
    }

    private void InitializeVehicle1_InfoParams(
                                            string Vehicle1_Year = "",
                                            string Vehicle1_Make = "",
                                            string Vehicle1_Model = "",
                                            string Vehicle1_Submodel = "",
                                            string Vehicle1_AnnualMileage = "",
                                            string Vehicle1_WeeklyCommuteDays = "",
                                            string Vehicle1_PrimaryUse = "",
                                            string Vehicle1_ComprehensiveDeductable = "",
                                            string Vehicle1_CollisionDeductable = "",
                                            string Vehicle1_SecuritySystem = "",
                                            string Vehicle1_WhereParked = "")
    {
        _Vehicle1_Year = ExtractLong(Vehicle1_Year, "Vehicle1_Year");
        _Vehicle1_Make = Vehicle1_Make;
        _Vehicle1_Model = Vehicle1_Model;
        _Vehicle1_Submodel = Vehicle1_Submodel;
        _Vehicle1_AnnualMileage = ExtractLong(Vehicle1_AnnualMileage, "Vehicle1_AnnualMileage");
        _Vehicle1_WeeklyCommuteDays = ExtractLong(Vehicle1_WeeklyCommuteDays, "Vehicle1_WeeklyCommuteDays");
        _Vehicle1_PrimaryUse = Vehicle1_PrimaryUse;
        _Vehicle1_ComprehensiveDeductable = Vehicle1_ComprehensiveDeductable;
        _Vehicle1_CollisionDeductable = Vehicle1_CollisionDeductable;
        _Vehicle1_SecuritySystem = Vehicle1_SecuritySystem;
        _Vehicle1_WhereParked = Vehicle1_WhereParked;
    }

    private void InitializeVehicle2_InfoParams(
                                            string Vehicle2_Year = "",
                                            string Vehicle2_Make = "",
                                            string Vehicle2_Model = "",
                                            string Vehicle2_Submodel = "",
                                            string Vehicle2_AnnualMileage = "",
                                            string Vehicle2_WeeklyCommuteDays = "",
                                            string Vehicle2_PrimaryUse = "",
                                            string Vehicle2_ComprehensiveDeductable = "",
                                            string Vehicle2_CollisionDeductable = "",
                                            string Vehicle2_SecuritySystem = "",
                                            string Vehicle2_WhereParked = "")
    {
        _Vehicle2_Year = ExtractLong(Vehicle2_Year, "Vehicle2_Year");
        _Vehicle2_Make = Vehicle2_Make;
        _Vehicle2_Model = Vehicle2_Model;
        _Vehicle2_Submodel = Vehicle2_Submodel;
        _Vehicle2_AnnualMileage = ExtractLong(Vehicle2_AnnualMileage, "Vehicle2_AnnualMileage");
        _Vehicle2_WeeklyCommuteDays = ExtractLong(Vehicle2_WeeklyCommuteDays, "Vehicle2_WeeklyCommuteDays");
        _Vehicle2_PrimaryUse = Vehicle2_PrimaryUse;
        _Vehicle2_ComprehensiveDeductable = Vehicle2_ComprehensiveDeductable;
        _Vehicle2_CollisionDeductable = Vehicle2_CollisionDeductable;
        _Vehicle2_SecuritySystem = Vehicle2_SecuritySystem;
        _Vehicle2_WhereParked = Vehicle2_WhereParked;
    }

    private void InitializeVehicle3_InfoParams(
                                            string Vehicle3_Year = "",
                                            string Vehicle3_Make = "",
                                            string Vehicle3_Model = "",
                                            string Vehicle3_Submodel = "",
                                            string Vehicle3_AnnualMileage = "",
                                            string Vehicle3_WeeklyCommuteDays = "",
                                            string Vehicle3_PrimaryUse = "",
                                            string Vehicle3_ComprehensiveDeductable = "",
                                            string Vehicle3_CollisionDeductable = "",
                                            string Vehicle3_SecuritySystem = "",
                                            string Vehicle3_WhereParked = "")
    {
        _Vehicle3_Year = ExtractLong(Vehicle3_Year, "Vehicle3_Year");
        _Vehicle3_Make = Vehicle3_Make;
        _Vehicle3_Model = Vehicle3_Model;
        _Vehicle3_Submodel = Vehicle3_Submodel;
        _Vehicle3_AnnualMileage = ExtractLong(Vehicle3_AnnualMileage, "Vehicle3_AnnualMileage");
        _Vehicle3_WeeklyCommuteDays = ExtractLong(Vehicle3_WeeklyCommuteDays, "Vehicle3_WeeklyCommuteDays");
        _Vehicle3_PrimaryUse = Vehicle3_PrimaryUse;
        _Vehicle3_ComprehensiveDeductable = Vehicle3_ComprehensiveDeductable;
        _Vehicle3_CollisionDeductable = Vehicle3_CollisionDeductable;
        _Vehicle3_SecuritySystem = Vehicle3_SecuritySystem;
        _Vehicle3_WhereParked = Vehicle3_WhereParked;
    }

    private void InitializeHome1_InfoParams(
        string Home1_CurrentCarrier = "",
        string Home1_CurrentXdateLeadInfo = "",
        string Home1_YearBuilt = "",
        string Home1_DwellingType = "",
        string Home1_DesignType = "",
        string Home1_RoofAge = "",
        string Home1_RoofType = "",
        string Home1_FoundationType = "",
        string Home1_HeatingType = "",
        string Home1_ExteriorWallType = "",
        string Home1_NumberOfClaims = "",
        string Home1_NumberOfBedrooms = "",
        string Home1_SqFootage = "",
        string Home1_ReqCoverage = "",
        string Home1_NumberOfBathrooms = "")
    {
        _Home1_CurrentCarrier = Home1_CurrentCarrier;
        _Home1_CurrentXdateLeadInfo = Home1_CurrentXdateLeadInfo;
        _Home1_YearBuilt = Home1_YearBuilt;
        _Home1_DwellingType = Home1_DwellingType;
        _Home1_DesignType = Home1_DesignType;
        _Home1_RoofAge = ExtractLong(Home1_RoofAge, "Home1_RoofAge");
        _Home1_RoofType = Home1_RoofType;
        _Home1_FoundationType = Home1_FoundationType;
        _Home1_HeatingType = Home1_HeatingType;
        _Home1_ExteriorWallType = Home1_ExteriorWallType;
        _Home1_NumberOfClaims = ExtractLong(Home1_NumberOfClaims, "Home1_NumberOfClaims");
        _Home1_NumberOfBedrooms = ExtractLong(Home1_NumberOfBedrooms, "Home1_NumberOfBedrooms");
        _Home1_SqFootage = ExtractLong(Home1_SqFootage, "Home1_SqFootage");
        _Home1_ReqCoverage = Home1_ReqCoverage;
        _Home1_NumberOfBathrooms = ExtractLong(Home1_NumberOfBathrooms, "Home1_NumberOfBathrooms");
    }

    private void InitializeHome2_InfoParams(
        string Home2_CurrentCarrier = "",
        string Home2_CurrentXdateLeadInfo = "",
        string Home2_YearBuilt = "",
        string Home2_DwellingType = "",
        string Home2_DesignType = "",
        string Home2_RoofAge = "",
        string Home2_RoofType = "",
        string Home2_FoundationType = "",
        string Home2_HeatingType = "",
        string Home2_ExteriorWallType = "",
        string Home2_NumberOfClaims = "",
        string Home2_NumberOfBedrooms = "",
        string Home2_SqFootage = "",
        string Home2_ReqCoverage = "",
        string Home2_NumberOfBathrooms = "")
    {
        _Home2_CurrentCarrier = Home2_CurrentCarrier;
        _Home2_CurrentXdateLeadInfo = Home2_CurrentXdateLeadInfo;
        _Home2_YearBuilt = Home2_YearBuilt;
        _Home2_DwellingType = Home2_DwellingType;
        _Home2_DesignType = Home2_DesignType;
        _Home2_RoofAge = ExtractLong(Home2_RoofAge, "Home2_RoofAge");
        _Home2_RoofType = Home2_RoofType;
        _Home2_FoundationType = Home2_FoundationType;
        _Home2_HeatingType = Home2_HeatingType;
        _Home2_ExteriorWallType = Home2_ExteriorWallType;
        _Home2_NumberOfClaims = ExtractLong(Home2_NumberOfClaims, "Home2_NumberOfClaims");
        _Home2_NumberOfBedrooms = ExtractLong(Home2_NumberOfBedrooms, "Home2_NumberOfBedrooms");
        _Home2_SqFootage = ExtractLong(Home2_SqFootage, "Home2_SqFootage");
        _Home2_ReqCoverage = Home2_ReqCoverage;
        _Home2_NumberOfBathrooms = ExtractLong(Home2_NumberOfBathrooms, "Home2_NumberOfBathrooms");
    }

    private void InitializeHome3_InfoParams(
        string Home3_CurrentCarrier = "",
        string Home3_CurrentXdateLeadInfo = "",
        string Home3_YearBuilt = "",
        string Home3_DwellingType = "",
        string Home3_DesignType = "",
        string Home3_RoofAge = "",
        string Home3_RoofType = "",
        string Home3_FoundationType = "",
        string Home3_HeatingType = "",
        string Home3_ExteriorWallType = "",
        string Home3_NumberOfClaims = "",
        string Home3_NumberOfBedrooms = "",
        string Home3_SqFootage = "",
        string Home3_ReqCoverage = "",
        string Home3_NumberOfBathrooms = "")
    {
        _Home3_CurrentCarrier = Home3_CurrentCarrier;
        _Home3_CurrentXdateLeadInfo = Home3_CurrentXdateLeadInfo;
        _Home3_YearBuilt = Home3_YearBuilt;
        _Home3_DwellingType = Home3_DwellingType;
        _Home3_DesignType = Home3_DesignType;
        _Home3_RoofAge = ExtractLong(Home3_RoofAge, "Home3_RoofAge");
        _Home3_RoofType = Home3_RoofType;
        _Home3_FoundationType = Home3_FoundationType;
        _Home3_HeatingType = Home3_HeatingType;
        _Home3_ExteriorWallType = Home3_ExteriorWallType;
        _Home3_NumberOfClaims = ExtractLong(Home3_NumberOfClaims, "Home3_NumberOfClaims");
        _Home3_NumberOfBedrooms = ExtractLong(Home3_NumberOfBedrooms, "Home3_NumberOfBedrooms");
        _Home3_SqFootage = ExtractLong(Home3_SqFootage, "Home3_SqFootage");
        _Home3_ReqCoverage = Home3_ReqCoverage;
        _Home3_NumberOfBathrooms = ExtractLong(Home3_NumberOfBathrooms, "Home3_NumberOfBathrooms");
    }

    private void InitilizeParamsAndDetails(string CampaignId, string StatusId, string Lead_Pub_ID, string Lead_Ad_Variation, string Lead_IP_Address, string Lead_DTE_Company_Name, string Lead_Group, string Leads_First_Contact_Appointment
        //WM - 03.07.2013
        , string Lead_Tracking_Code, string Lead_Tracking_Information, string Lead_Source_Code, string Lead_Pub_Sub_ID, string Lead_Email_Tracking_Code
        , string Account_External_Agent, string Account_Life_Information, string Primary_FirstName = "", string Primary_LastName = "", string Primary_Gender = "", string Primary_DayPhone = "", string Primary_EveningPhone = "", string Primary_MobilePhone = "", string Primary_Fax = "", string Primary_Address1 = "", string Primary_Address2 = "", string Primary_City = "", string PrimaryState = "", string Primary_Zip = "", string Primary_BirthDate = "", string Primary_Tobacco = "", string Primary_Email = "", string Primary_Reference_ID = "", string Primary_Notes = "", string Secondary_FirstName = "", string Secondary_LastName = "", string Secondary_Gender = "", string Secondary_DayPhone = "", string Secondary_EveningPhone = "", string Secondary_MobilePhone = "", string Secondary_Fax = "", string Secondary_Address1 = "", string Secondary_Address2 = "", string Secondary_City = "", string SecondaryState = "", string Secondary_Zip = "", string Secondary_BirthDate = "", string Secondary_Tobacco = "", string Secondary_Email = "", string Secondary_Reference_ID = "", string Secondary_Notes = "", string Primary_HRASubsidyAmount = "", string Secondary_HRASubsidyAmount = ""
        , string primary_tcpa_consent = "", string secondary_tcpa_consent = "")
    {
        SetCampaignAndStatus(CampaignId, StatusId);

        SetAccountAndLeadsParams(Lead_Pub_ID, Lead_Ad_Variation, Lead_IP_Address, Lead_DTE_Company_Name, Lead_Group, Leads_First_Contact_Appointment
            //WM - 03.07.2013
            , Lead_Tracking_Code, Lead_Tracking_Information, Lead_Source_Code, Lead_Pub_Sub_ID, Lead_Email_Tracking_Code
            , Account_External_Agent, Account_Life_Information);

        SetPrimaryIndividualParams(Primary_FirstName, Primary_LastName, Primary_Gender, Primary_DayPhone, Primary_EveningPhone, Primary_MobilePhone, Primary_Fax, Primary_Address1, Primary_Address2, Primary_City, PrimaryState, Primary_Zip, Primary_BirthDate, Primary_Tobacco, Primary_Email, Primary_Reference_ID, Primary_Notes, Primary_HRASubsidyAmount, primary_tcpa_consent);

        SetSecondaryIndividualParams(PrimaryState, Secondary_FirstName, Secondary_LastName, Secondary_Gender, Secondary_DayPhone, Secondary_EveningPhone, Secondary_MobilePhone, Secondary_Fax, Secondary_Address1, Secondary_Address2, Secondary_City, SecondaryState, Secondary_Zip, Secondary_BirthDate, Secondary_Tobacco, Secondary_Email, Secondary_Reference_ID, Secondary_Notes, Secondary_HRASubsidyAmount, secondary_tcpa_consent);

    }

    private void SetCampaignAndStatus(string CampaignId, string StatusId)
    {
        _CampaignId = ExtractInteger(CampaignId, "CampaignId", true);
        _StatusId = ExtractInteger(StatusId, "StatusId", true);
    }

    private void SetAccountAndLeadsParams(string Lead_Pub_ID, string Lead_Ad_Variation, string Lead_IP_Address, string Lead_DTE_Company_Name, string Lead_Group, string Leads_First_Contact_Appointment
        //WM - 03.07.2013
            , string Lead_Tracking_Code, string Lead_Tracking_Information, string Lead_Source_Code, string Lead_Pub_Sub_ID, string Lead_Email_Tracking_Code
            , string Account_External_Agent, string Account_Life_Information)
    {
        _Lead_Pub_ID = Lead_Pub_ID;
        _Lead_Ad_Variation = Lead_Ad_Variation;
        _Lead_IP_Address = Lead_IP_Address;
        _Lead_DTE_Company_Name = Lead_DTE_Company_Name;
        _Lead_Group = Lead_Group;
        _Leads_First_Contact_Appointment = ExtractDateTime(Leads_First_Contact_Appointment, "Leads_First_Contact_Appointment");

        //WM - 03.07.2013
        _Lead_Tracking_Code = Lead_Tracking_Code;
        _Lead_Tracking_Information = Lead_Tracking_Information;
        _Lead_Source_Code = Lead_Source_Code;
        _Lead_Pub_Sub_ID = Lead_Pub_Sub_ID;
        _Lead_Email_Tracking_Code = Lead_Email_Tracking_Code;

        _Account_External_Agent = Account_External_Agent;
        _Account_Life_Information = Account_Life_Information;

    }

    private void SetSecondaryIndividualParams(string PrimaryState, string Secondary_FirstName, string Secondary_LastName, string Secondary_Gender, string Secondary_DayPhone, string Secondary_EveningPhone, string Secondary_MobilePhone, string Secondary_Fax, string Secondary_Address1, string Secondary_Address2, string Secondary_City, string SecondaryState, string Secondary_Zip, string Secondary_BirthDate, string Secondary_Tobacco, string Secondary_Email = "", string Secondary_Reference_ID = "", string Secondary_Notes = "", string Secondary_HRASubsidyAmount = "", string secondary_tcpa_consent = "")
    {
        //if (String.IsNullOrEmpty(Secondary_FirstName))
        //{
        //    ErrorFlag = true;
        //    ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: Secondary_FirstName string parameter required.");
        //}
        _Secondary_FirstName = Secondary_FirstName;
        //if (String.IsNullOrEmpty(Secondary_LastName))
        //{
        //    ErrorFlag = true;
        //    ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: Secondary_LastName string parameter required.");
        //}
        _Secondary_LastName = Secondary_LastName;
        _Secondary_Gender = Secondary_Gender;
        _Secondary_DayPhone = ExtractLong(Secondary_DayPhone, "Secondary_DayPhone");
        _Secondary_EveningPhone = ExtractLong(Secondary_EveningPhone, "Secondary_EveningPhone");
        _Secondary_MobilePhone = ExtractLong(Secondary_MobilePhone, "Secondary_MobilePhone");
        _Secondary_Fax = ExtractLong(Secondary_Fax, "Secondary_Fax");
        _Secondary_Address1 = Secondary_Address1;
        _Secondary_Address2 = Secondary_Address2;
        _Secondary_City = Secondary_City;
        if (PrimaryState.Length == 2)
        {
            _Secondary_State = SecondaryState;
        }
        else if (SecondaryState.Length != 0)
            ErrorXML("", ErrorCodes.StateAbbreviation.ToString(), "Secondary State abbreviation is not valid");
        _Secondary_Zip = Secondary_Zip == "" ? "" : Secondary_Zip;//ExtractInteger(Secondary_Zip, "Secondary_Zip");
        if (Secondary_BirthDate != "")
            _Secondary_BirthDate = ExtractDateTime(Secondary_BirthDate, "Secondary_BirthDate");
        _Secondary_Tobacco = Secondary_Tobacco == "" ? false : ExtractBoolean(Secondary_Tobacco, "Secondary_Tobacco");
        _Secondary_Email = Secondary_Email;
        _Secondary_Reference_ID = Secondary_Reference_ID;
        _Secondary_Notes = Secondary_Notes;
        _Secondary_HRASubsidyAmount = ExtractDecimal(Secondary_HRASubsidyAmount, "Secondary_HRASubsidyAmount");


        // Attiq - April-03-2014 - Show the user with message if the value entered is invald
        // Valid Values are ('a' : Not Applicable, 'y' : Yes, 'n' : No)
        if (secondary_tcpa_consent.Length > 1)
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: secondary_tcpa_consent valid values are: 'n' or 'y' or 'a'.");
        }
        else if (secondary_tcpa_consent.Length == 1)
        {
            if (!secondary_tcpa_consent.ToLower().Equals("n") && !(secondary_tcpa_consent.ToLower().Equals("y")) &&
                !(secondary_tcpa_consent.ToLower().Equals("a")))
            {
                ErrorFlag = true;
                ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: secondary_tcpa_consent valid values are: 'n' or 'y' or 'a'.");
            }
            else
            {
                _secondary_tcpa_consent = string.IsNullOrEmpty(secondary_tcpa_consent) ? null : secondary_tcpa_consent.ToLower();
            }
        }
        else
        {
            _secondary_tcpa_consent = string.IsNullOrEmpty(secondary_tcpa_consent) ? null : secondary_tcpa_consent.ToLower();
        }


    }

    private void SetPrimaryIndividualParams(string Primary_FirstName, string Primary_LastName, string Primary_Gender, string Primary_DayPhone, string Primary_EveningPhone, string Primary_MobilePhone, string Primary_Fax, string Primary_Address1, string Primary_Address2, string Primary_City, string PrimaryState, string Primary_Zip, string Primary_BirthDate, string Primary_Tobacco, string Primary_Email = "", string Primary_Reference_ID = "", string Primary_Notes = "", string Primary_HRASubsidyAmount = "", string primary_tcpa_consent = "")
    {
        if (String.IsNullOrEmpty(Primary_FirstName))
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: Primary_FirstName string parameter required.");
        }
        _Primary_FirstName = Primary_FirstName;
        if (String.IsNullOrEmpty(Primary_LastName))
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: Primary_LastName string parameter required.");
        }
        _Primary_LastName = Primary_LastName;
        _Primary_Gender = Primary_Gender;
        _Primary_DayPhone = ExtractLong(Primary_DayPhone, "Primary_DayPhone");
        _Primary_EveningPhone = ExtractLong(Primary_EveningPhone, "Primary_EveningPhone");
        _Primary_MobilePhone = ExtractLong(Primary_MobilePhone, "Primary_MobilePhone");
        _Primary_Fax = ExtractLong(Primary_Fax, "Primary_Fax");
        _Primary_Address1 = Primary_Address1;
        _Primary_Address2 = Primary_Address2;
        _Primary_City = Primary_City;
        _Primary_HRASubsidyAmount = ExtractDecimal(Primary_HRASubsidyAmount, "Primary_HRASubsidyAmount");
        if (PrimaryState.Length == 2)
        {
            _Primary_State = PrimaryState;
        }
        else if (PrimaryState.Length != 0)
            ErrorXML("", ErrorCodes.StateAbbreviation.ToString(), "Primary State abbreviation is not valid");

        _Primary_Zip = Primary_Zip == "" ? "" : Primary_Zip;
        if (Primary_BirthDate != "")
            _Primary_BirthDate = ExtractDateTime(Primary_BirthDate, "Primary_BirthDate");
        _Primary_Tobacco = Primary_Tobacco == "" ? false : ExtractBoolean(Primary_Tobacco, "Primary_Tobacco");
        _Primary_Email = Primary_Email;
        _Primary_Reference_ID = Primary_Reference_ID;
        _Primary_Notes = Primary_Notes;

        // Attiq - April-03-2014 - Show the user with message if the value entered is invald
        // Valid Values are ('a' : Not Applicable, 'y' : Yes, 'n' : No)
        if (primary_tcpa_consent.Length > 1)
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: primary_tcpa_consent valid values are: 'n' or 'y' or 'a'.");
        }
        else if (primary_tcpa_consent.Length == 1)
        {
            if (!primary_tcpa_consent.ToLower().Equals("n") && !(primary_tcpa_consent.ToLower().Equals("y")) &&
                !(primary_tcpa_consent.ToLower().Equals("a")))
            {
                ErrorFlag = true;
                ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: primary_tcpa_consent valid values are: 'n' or 'y' or 'a'.");
            }
            else
            {
                _primary_tcpa_consent = string.IsNullOrEmpty(primary_tcpa_consent) ? null : primary_tcpa_consent.ToLower();
            }
        }
        else
        {
            _primary_tcpa_consent = string.IsNullOrEmpty(primary_tcpa_consent) ? null : primary_tcpa_consent.ToLower();
        }
    }

    public SalesTool.DataAccess.DBEngine Engine
    {
        get
        {
            if (_engine == null)
            {
                _engine = new SalesTool.DataAccess.DBEngine();
                _engine.SetSettings(System.Web.HttpContext.Current.IfNotNull(p => p.Application).ApplicationSettings());
                //string key = System.Configuration.ConfigurationManager.AppSettings[Konstants.K_CURRENT_MODEL];
                //_engine.Init(ApplicationSettings.AdminEF, ApplicationSettings.LeadEF, ApplicationSettings.DashboardEF);
                _engine.Init(ApplicationSettings.ADOConnectionString);
            }
            return _engine;
        }
    }


    private void InsertAllRecords(bool bInsertAdditionalInfo = false)
    {
        try
        {
            Account A = InsertAccountDetails();
            if (A != null && CanContinue)
            {
                int countIndividual = 2;

                if (CanContinue) InsertPrimaryIndividual(A); else throw new Exception();
                if (CanContinue) InsertSecondaryIndividual(A); else throw new Exception();


                if (AccountID > 0 && bInsertAdditionalInfo && !ErrorFlag)
                {
                    //Insert Drivers Info
                    if (CanContinue) InsertDriver1_Info(); else throw new Exception();
                    if (CanContinue) InsertDriver2_Info(); else throw new Exception();
                    if (CanContinue) InsertDriver3_Info(); else throw new Exception();

                    //Insert Vehicle Info
                    if (CanContinue) InsertVehicle1_Info(); else throw new Exception();
                    if (CanContinue) InsertVehicel2_Info(); else throw new Exception();
                    if (CanContinue) InsertVehicel3_Info(); else throw new Exception();
                    //Insert Homes Info

                    if (CanContinue) InsertHome1_Info(); else throw new Exception();
                    if (CanContinue) InsertHome2_Info(); else throw new Exception();
                    if (CanContinue) InsertHome3_Info(); else throw new Exception();
                }

                Engine.AccountActions.Update(A, K_WEBSERVICE);

                if (!ErrorFlag)
                    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Insert Success", countIndividual.ToString());
                else throw new Exception();

            }
            else throw new Exception();

            //Logger.Logfile("Checking duplicate settings...." + ApplicationSettings.CanUseDuplicateManagementFeature.ToString(), ApplicationSettings.LogFilePath);
            Logger.Logfile("Checking duplicate settings...." + ApplicationSettings.CanUseDuplicateManagementFeature.ToString(), Engine.ApplicationSettings.LogFilePath);
            if (ApplicationSettings.CanUseDuplicateManagementFeature == Konstants.UseDuplicateManagementFeature.Both || ApplicationSettings.CanUseDuplicateManagementFeature == Konstants.UseDuplicateManagementFeature.Posted)
            {
                //Logger.Logfile("Entering in the duplicate process....", ApplicationSettings.LogFilePath);
                Logger.Logfile("Entering in the duplicate process....", Engine.ApplicationSettings.LogFilePath);
                //SR 4.9.2014 CheckDuplicateLead.Execute(LeadID, Konstants.UseDuplicateManagementFeature.Posted);
                string errorMessage = new CheckDuplicateLead(K_WEBSERVICE).CheckDuplicateFromService(Konstants.UseDuplicateManagementFeature.Posted, LeadID);
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    if (errorMessage.Contains("Any Phone Number"))
                        throw new Exception("Error: Duplicate phone number");
                    else
                        throw new Exception("Error: Duplicates found in following rules: " + errorMessage);
                }
                //Logger.Logfile("Exited from the duplicate process....", ApplicationSettings.LogFilePath);
                Logger.Logfile("Exited from the duplicate process....", Engine.ApplicationSettings.LogFilePath);
            }
            var nleadCheck = Engine.LeadsActions.Get(LeadID);
            if (nleadCheck != null)
            {
                //YA[April 2, 2013] For setting the Email Queue records with Queued status
                //if (ApplicationSettings.CanRunEmailUpdater)
                if (Engine.ApplicationSettings.CanRunEmailUpdater)
                {
                    EmailQueueUpdater.Execute(AccountID, nleadCheck.ActionId, nleadCheck.StatusId, nleadCheck.SubStatusId);
                }
                //if (ApplicationSettings.CanRunPostQueueUpdater)
                if (Engine.ApplicationSettings.CanRunPostQueueUpdater)
                {
                    PostQueueUpdater.Execute(AccountID, nleadCheck.ActionId, nleadCheck.StatusId, nleadCheck.SubStatusId);
                }
            }
        }
        catch (Exception ex)
        {
            //SZ [Aug 30, 2013] unwind stack
            string sMessage = ex.Message != string.Empty ? ex.Message :
                "An error occured while processing, Please see other messages ofr details and retry";
            ErrorXML("", ErrorCodes.GeneralError.ToString(), sMessage + "./n Rolling back all the changes and restoring database to previous state");
            _journal.Rollback();
        }
    }

    bool PhoneExists(string phone1, string phone2, string phone3, ref List<long> accIds)
    {
        bool bAns = false;

        phone1 = phone1 == "0" ? string.Empty : phone1;
        phone2 = phone2 == "0" ? string.Empty : phone2;
        phone3 = phone3 == "0" ? string.Empty : phone3;

        using (SalesTool.Schema.ProxySearch search = new SalesTool.Schema.ProxySearch(ApplicationSettings.ADOConnectionString))
        {
            search.Init("");
            if (phone1 != string.Empty)
                accIds.AddRange(search.SearchByPhone(phone1));
            if (phone2 != string.Empty)
                accIds.AddRange(search.SearchByPhone(phone2));
            if (phone3 != string.Empty)
                accIds.AddRange(search.SearchByPhone(phone3));
            bAns = accIds.Count > 0;
        }
        return bAns;
    }

    #region Basic Functions for Insertion
    private Account InsertAccountDetails()
    {
        Account nAccount = null;
        Lead L = null;

        try
        {
            if (ApplicationSettings.CanUseDuplicateManagementFeature == Konstants.UseDuplicateManagementFeature.Basic)
            {
                //SZ| [Apr 16, 2013] slight optimization
                List<long> accIds = new List<long>();
                bool bAccountExists = PhoneExists(_Primary_DayPhone.ToString(), _Primary_EveningPhone.ToString(), _Primary_MobilePhone.ToString(), ref accIds);

                if (bAccountExists && ApplicationSettings.CanUseDuplicateManagementFeature == Konstants.UseDuplicateManagementFeature.Basic)
                    throw new Exception("Error: Duplicate phone number");

                if (bAccountExists)
                {
                    //SZ [Aug 28, 2013] Code to handle error
                    //SZ [Nov 1, 2013] marking not required for exiting records as they are not deleted
                    //_journal.Mark(RecordType.Account, accIds[0], true); 

                    // Attiq - April 04-2014
                    // passing bWebServiceCall = true so that the E.Lead.Refresh(RefreshMode.StoreWins, R); 
                    // is not executed inside the Get function.
                    nAccount = Engine.AccountActions.Get(accIds[0], true, true);
                    AccountID = nAccount.Key;
                    Lead nLead = Engine.LeadsActions.Get(nAccount.PrimaryLeadKey ?? -1);
                    if (nLead != null)
                    {
                        if (nLead.CampaignId == _CampaignId)
                        {
                            //ErrorFlag = true;
                            //ErrorXML("", ErrorCodes.GeneralError.ToString(), "Error: Duplicate phone number.");
                            //return null;
                            throw new Exception("Error: Duplicate phone number");
                        }
                        else
                        {
                            //CloneLead(L, nLead);
                            L = nLead.Duplicate();
                            L.CampaignId = _CampaignId;
                            L.AccountId = AccountID;
                            L.AddedBy = "Service";
                            L.AddedOn = DateTime.Now;

                            if (_Leads_First_Contact_Appointment != DateTime.MinValue)
                                L.FirstContactAppointment = _Leads_First_Contact_Appointment;

                            LeadID = Engine.LeadsActions.Add(L, K_WEBSERVICE).Key;
                            //_journal.Mark(RecordType.Lead, LeadID);
                            _journal.Mark(L);

                        }
                    }
                    else
                    {
                        L = InsertNewLead();
                    }
                }
                else
                {
                    nAccount = new Account();

                    //SZ Aug 21, 2014 Added for updating the account history
                    Engine.AccountActions.Add(nAccount, K_WEBSERVICE);
                    AccountID = nAccount.Key;
                    //_journal.Mark(RecordType.Account, AccountID);
                    _journal.Mark(nAccount);
                    L = InsertNewLead();
                }
            }
            else
            {
                nAccount = new Account();
                Engine.AccountActions.Add(nAccount, K_WEBSERVICE);
                AccountID = nAccount.Key;
                //_journal.Mark(RecordType.Account, AccountID);
                _journal.Mark(nAccount);
                L = InsertNewLead();
            }
            //}
            //else
            //{
            //    nAccount = new Account();
            //    Engine.AccountActions.Add(nAccount);
            //    AccountID = nAccount.Key;
            //    //_journal.Mark(RecordType.Account, AccountID);
            //    _journal.Mark(nAccount);
            //    L=InsertNewLead();
            //}
            nAccount.PrimaryLeadKey = LeadID;
            nAccount.ExternalAgent = _Account_External_Agent;
            nAccount.LifeInfo = _Account_Life_Information;
            Engine.AccountActions.Update(nAccount, K_WEBSERVICE);

        }
        catch (Exception ex)
        {
            nAccount = null;
            Capture(ex);
            //ErrorFlag = true;
            //ErrorXML("", ErrorCodes.GeneralError.ToString(), ex.Message);
        }
        return nAccount;
    }
    private Lead InsertNewLead()
    {
        Lead L = new Lead
        {
            AccountId = AccountID,
            CampaignId = _CampaignId,
            StatusId = _StatusId,
            PublisherID = _Lead_Pub_ID,
            AdVariation = _Lead_Ad_Variation,
            IPAddress = _Lead_IP_Address,
            Company = _Lead_DTE_Company_Name,
            Group = _Lead_Group,

            //WM - 03.07.2013
            //----------------
            TrackingCode = _Lead_Tracking_Code,
            TrackingInformation = _Lead_Tracking_Information,
            SourceCode = _Lead_Source_Code,
            PubSubId = _Lead_Pub_Sub_ID,
            EmailTrackingCode = _Lead_Email_Tracking_Code,
        };

        if (_Leads_First_Contact_Appointment != DateTime.MinValue)
            L.FirstContactAppointment = _Leads_First_Contact_Appointment;

        LeadID = Engine.LeadsActions.Add(L, K_WEBSERVICE).Key;

        //_journal.Mark(RecordType.Lead, LeadID);
        _journal.Mark(L);
        return L;
    }

    long AddDriver(long accId, long? IndvId, string sLicenseNumber, string DlState, string LicenseStatus, string MaritalStatus, long? AgeLicensed, long? YearsAtResidence,
    string Occupation, long? YearsWithCompany, long? YrsInField, string Education, string TicketsAccidentsClaims,
    long? NmbrIncidents, string IncidentType, string IncidentDescription, DateTime? IncidentDate, DateTime? CurrentAutoXDate,
    decimal? ClaimPaidAmount, string Sr22, long? PolicyYears)
    {
        long Id = 0;

        DriverInfo nDriver = new DriverInfo
        {
            AccountId = accId,
            IndividualId = IndvId,
            LisenceNumber = sLicenseNumber,
            DlState = DlState,
            LicenseStatus = LicenseStatus,
            MaritalStatus = MaritalStatus,
            AgeLicensed = AgeLicensed,
            YearsAtResidence = YearsAtResidence,
            Occupation = Occupation,
            YearsWithCompany = YearsWithCompany,
            YrsInField = YrsInField,
            Education = Education,
            TicketsAccidentsClaims = TicketsAccidentsClaims,
            NmbrIncidents = NmbrIncidents,
            IncidentType = IncidentType,
            IncidentDescription = IncidentDescription,
            ClaimPaidAmount = ClaimPaidAmount,
            St22 = Sr22,
            PolicyYears = PolicyYears
        };

        if (IncidentDate != DateTime.MinValue)
            nDriver.IncidentDate = IncidentDate;
        if (CurrentAutoXDate != DateTime.MinValue)
            nDriver.CurrentAutoXDate = CurrentAutoXDate;

        Engine.DriverActions.Add(nDriver);
        _journal.Mark(nDriver);

        Id = nDriver.Key;

        return Id;
    }

    long AddIndividual(long accId, string _FirstName, string _LastName, string _Gender,
        long? _DayPhone, long? _EveningPhone, long? _MobilePhone, long? _Fax,
        DateTime _BirthDate, string _Address1, string _Address2, string _City, string _Email,
        string _Reference_ID, bool _Tobacco, string _Zip,
        string _State, string _Notes, decimal? _HRASubsidyAmount = null, string tcpaConsent = "")
    {
        long Ans = 0;


        Individual P =
            new Individual
            {
                AccountId = accId,
                FirstName = _FirstName,
                LastName = _LastName,
                Gender = _Gender,
                DayPhone = _DayPhone == 0 ? (long?)null : _DayPhone,
                EveningPhone = _EveningPhone == 0 ? (long?)null : _EveningPhone,
                CellPhone = _MobilePhone == 0 ? (long?)null : _MobilePhone,
                FaxNmbr = _Fax == 0 ? (long?)null : _Fax,
                Address1 = _Address1,
                Address2 = _Address2,
                City = _City,
                Email = _Email,
                ExternalReferenceID = _Reference_ID,
                Notes = _Notes,
                Smoking = _Tobacco,
                Zipcode = _Zip,
                HRASubsidyAmount = _HRASubsidyAmount,
                HasConsent = string.IsNullOrEmpty(tcpaConsent) ? TCPAConsentType.Blank : tcpaConsent == "y" ? TCPAConsentType.Yes : tcpaConsent == "n" ? TCPAConsentType.No : TCPAConsentType.Undefined

            };
        if (_BirthDate != DateTime.MinValue)
            P.Birthday = _BirthDate;

        if (_States.ContainsKey(_State))
            P.StateID = _States[_State];

        Ans = Engine.IndividualsActions.Add(P, K_WEBSERVICE).Key;

        _journal.Mark(P);
        return Ans;
    }

    long AddVehicle(long accId, long? indvId, long? _Year, string _Make, string _Model, string _Submodel, long? _AnnualMileage,
      string _CollisionDeductable, string _ComprehensiveDeductable, string _WhereParked, string _PrimaryUse, string _SecuritySystem, long? _WeeklyCommuteDays)
    {
        long id = 0;
        Vehicle V = new Vehicle
        {
            Year = _Year,
            Make = _Make,
            Model = _Model,
            Submodel = _Submodel,
            AnnualMileage = _AnnualMileage,
            CollisionDeductable = _CollisionDeductable,
            ComprehensiveDeductable = _ComprehensiveDeductable,
            WhereParked = _WhereParked,
            PrimaryUse = _PrimaryUse,
            SecuritySystem = _SecuritySystem,
            WeeklyCommuteDays = _WeeklyCommuteDays,
            AccountId = accId
        };
        if (indvId > 0)
            V.IndividualId = indvId;

        id = Engine.VehiclesActions.Add(V, "Web Service").Key;
        _journal.Mark(V);

        return id;
    }

    long AddHome(long accId, long? IndvId,
        string _Home1_YearBuilt, long? _Home1_SqFootage, string _Home1_DwellingType, string _Home1_DesignType, long? _Home1_RoofAge,
        string _Home1_RoofType, string _Home1_FoundationType, string _Home1_HeatingType, string _Home1_ExteriorWallType, long? _Home1_NumberOfClaims,
        long _Home1_NumberOfBedrooms, long? _Home1_NumberOfBathrooms, string _Home1_ReqCoverage)
    {
        long id = 0;
        Home nHome = new Home
        {
            AccountId = accId,
            Individualkey = IndvId ?? default(long),
            YearBuilt = _Home1_YearBuilt,
            SqFootage = _Home1_SqFootage,
            DwellingType = _Home1_DwellingType,
            DesignType = _Home1_DesignType,
            RoofAge = _Home1_RoofAge,
            RoofType = _Home1_RoofType,
            FoundationType = _Home1_FoundationType,
            HeatingType = _Home1_HeatingType,
            ExteriorWallType = _Home1_ExteriorWallType,
            NumberOfClaims = _Home1_NumberOfClaims,
            NumberOfBedrooms = _Home1_NumberOfBedrooms,
            NumberOfBathrooms = _Home1_NumberOfBathrooms,
            ReqCoverage = _Home1_ReqCoverage
        };

        Engine.HomeActions.Add(nHome);
        id = nHome.Id;

        _journal.Mark(nHome);
        return id;
    }
    #endregion


    private void InsertPrimaryIndividual(Account A)
    {
        bool bInsert = false;
        long Id = 0;
        long accId = A.Key;

        //int countIndividual = 0;
        //bool bNewRecord = false;

        Individual person = Engine.AccountActions.GetIndividual(accId, SalesTool.DataAccess.IndividualType.Primary);
        if ((_ParamsBlank & 1 << 3) == 0)
            try
            {
                if (person == null)
                    bInsert = true;
                else if (
                        person.LastName != _Primary_LastName &&
                        person.Zipcode != _Primary_Zip &&
                        person.Birthday != _Primary_BirthDate
                        )
                    bInsert = true;

                if (bInsert)
                {
                    if (!(_Primary_DayPhone.HasValue || _Primary_EveningPhone.HasValue || _Primary_MobilePhone.HasValue))
                        throw new Exception("Error: At least one phone number (Day,Evening,Mobile) is required for primary individual.");
                    else if (!(_Primary_FirstName != string.Empty || _Primary_LastName != string.Empty))
                        throw new Exception("Error: FirstName & LastName must be provided for primary individual.");
                    else
                    {
                        Id = AddIndividual(
                              accId,
                             _Primary_FirstName,
                             _Primary_LastName,
                             _Primary_Gender,
                             _Primary_DayPhone,
                             _Primary_EveningPhone,
                             _Primary_MobilePhone,
                             _Primary_Fax,
                             _Primary_BirthDate,
                             _Primary_Address1,
                             _Primary_Address2,
                             _Primary_City,
                             _Primary_Email,
                             _Primary_Reference_ID,
                             _Primary_Tobacco,
                             _Primary_Zip,
                             _Primary_State,
                             _Primary_Notes,
                             _Primary_HRASubsidyAmount,
                             _primary_tcpa_consent);

                        ErrorXML(accId.ToString(), "3", "Primary Individual Insert Success");
                    }
                }
                else
                    Id = person.Key;
                A.PrimaryIndividualId = IndividualID = Id;
            }
            catch (Exception ex)
            {
                Capture(ex);
                //ErrorXML("", "3", ex.Message);
                //ErrorFlag = true;
            }
    }
    private void InsertSecondaryIndividual(Account A)
    {
        bool bInsert = false;
        long Id = 0;
        long accId = A.Key;

        if ((_ParamsBlank & 1 << 4) == 0) try
            {
                Individual person = Engine.AccountActions.GetIndividual(accId, SalesTool.DataAccess.IndividualType.Secondary);

                if (person == null)
                    bInsert = true;
                else if (HasSpouce)
                    bInsert = true;

                if (bInsert)
                {
                    if (
                        (_Secondary_FirstName != string.Empty || _Secondary_LastName != string.Empty) &&
                        (_Secondary_DayPhone.HasValue || _Secondary_EveningPhone.HasValue || _Secondary_MobilePhone.HasValue)
                      )
                    {
                        Id = AddIndividual(
                            accId,
                            _Secondary_FirstName,
                            _Secondary_LastName,
                            _Secondary_Gender,
                            _Secondary_DayPhone,
                            _Secondary_EveningPhone,
                            _Secondary_MobilePhone,
                            _Secondary_Fax,
                            _Secondary_BirthDate,
                            _Secondary_Address1,
                            _Secondary_Address2,
                            _Secondary_City,
                            _Secondary_Email,
                            _Secondary_Reference_ID,
                            _Secondary_Tobacco,
                            _Secondary_Zip,
                            _Primary_State,
                            _Secondary_Notes,
                            _Secondary_HRASubsidyAmount,
                            _secondary_tcpa_consent);

                        //Success(Id, RecordType.Individual, "Secondary Individual Insert Success");
                        //_journal.Mark(RecordType.Individual, Id);
                        ErrorXML(accId.ToString(), "3", "Secondary Individual Insert Success");
                    }
                }
                else
                    Id = person.Key;
                A.SecondaryIndividualId = IndividualSecondaryID = Id;
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }

    private void InsertDriver1_Info()
    {
        long Id = 0;

        if ((_ParamsBlank & 1 << 5) == 0)
            try
            {
                Id = AddDriver(
                AccountID,
                IndividualID,
                _Driver1_LicenseNumber,
                _Driver1_DlState,
                _Driver1_LicenseStatus,
                _Driver1_MaritalStatus,
                _Driver1_AgeLicensed,
                _Driver1_YearsAtResidence,
                _Driver1_Occupation,
                _Driver1_YearsWithCompany,
                _Driver1_YrsInField,
                _Driver1_Education,
                _Driver1_TicketsAccidentsClaims,
                _Driver1_NmbrIncidents,
                _Driver1_IncidentType,
                _Driver1_IncidentDescription,
                (_Driver1_IncidentDate == DateTime.MinValue) ? null : _Driver1_IncidentDate,
                (_Driver1_CurrentAutoXDate == DateTime.MinValue) ? null : _Driver1_CurrentAutoXDate,
                _Driver1_ClaimPaidAmount,
                _Driver1_Sr22,
                _Driver1_PolicyYears);

                Success(Id, RecordType.Driver, "Driver1_Info Insert Success");
                //if (Id != 0)
                //{
                //    Push(Id, RecordType.Driver);
                //    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Driver1_Info Insert Success", "");
                //}
                //else
                //    throw new Exception("No record Inserted for Driver1");
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }
    private void InsertDriver2_Info()
    {

        long Id = 0;
        if ((_ParamsBlank & 1 << 6) == 0) try
            {
                Id = AddDriver(
            AccountID,
            IndividualSecondaryID == 0 ? (long?)null : IndividualSecondaryID,
            _Driver2_LicenseNumber,
            _Driver2_DlState,
            _Driver2_LicenseStatus,
            _Driver2_MaritalStatus,
            _Driver2_AgeLicensed,
            _Driver2_YearsAtResidence,
            _Driver2_Occupation,
            _Driver2_YearsWithCompany,
            _Driver2_YrsInField,
            _Driver2_Education,
            _Driver2_TicketsAccidentsClaims,
            _Driver2_NmbrIncidents,
            _Driver2_IncidentType,
            _Driver2_IncidentDescription,
            (_Driver2_IncidentDate == DateTime.MinValue) ? null : _Driver2_IncidentDate,
            (_Driver2_CurrentAutoXDate == DateTime.MinValue) ? null : _Driver2_CurrentAutoXDate,
            _Driver2_ClaimPaidAmount,
            _Driver2_Sr22,
            _Driver2_PolicyYears);

                Success(Id, RecordType.Driver, "Driver2_Info Insert Success");
                //if (Id != 0)
                //{
                //    Push(Id, RecordType.Driver);
                //    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Driver2_Info Insert Success", "");
                //}
                //else
                //    throw new Exception("No record inserted for Driver2");
            }
            catch (Exception ex)
            {
                Capture(ex);
                //ErrorXML(AccountID.ToString(), ErrorCodes.GeneralError.ToString(), ex.Message);
                //ErrorFlag = true;
            }
    }
    private void InsertDriver3_Info()
    {
        long Id = 0L;

        if ((_ParamsBlank & 1 << 7) == 0) try
            {
                Id = AddDriver(
                    AccountID,
                    (long?)null,
                    _Driver3_LicenseNumber,
                    _Driver3_DlState,
                    _Driver3_LicenseStatus,
                    _Driver3_MaritalStatus,
                    _Driver3_AgeLicensed,
                    _Driver3_YearsAtResidence,
                    _Driver3_Occupation,
                    _Driver3_YearsWithCompany,
                    _Driver3_YrsInField,
                    _Driver3_Education,
                    _Driver3_TicketsAccidentsClaims,
                    _Driver3_NmbrIncidents,
                    _Driver3_IncidentType,
                    _Driver3_IncidentDescription,
                    (_Driver3_IncidentDate == DateTime.MinValue) ? null : _Driver3_IncidentDate,
                    (_Driver3_CurrentAutoXDate == DateTime.MinValue) ? null : _Driver3_CurrentAutoXDate,
                    _Driver3_ClaimPaidAmount,
                    _Driver3_Sr22,
                    _Driver3_PolicyYears);

                Success(Id, RecordType.Driver, "Driver3 Inserted Success");
                //if (Id != 0)
                //{
                //    Push(Id, RecordType.Driver);
                //    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Driver3 Inserted Success", "");
                //}
                //else
                //    throw new Exception("No record inserted for driver 3");
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }


    private void InsertVehicle1_Info()
    {
        long Id = default(long);

        if ((_ParamsBlank & 1 << 8) == 0)
            try
            {
                Id = AddVehicle(AccountID, IndividualID, _Vehicle1_Year,
                    _Vehicle1_Make,
                    _Vehicle1_Model,
                    _Vehicle1_Submodel,
                    _Vehicle1_AnnualMileage,
                    _Vehicle1_CollisionDeductable,
                    _Vehicle1_ComprehensiveDeductable,
                    _Vehicle1_WhereParked,
                    _Vehicle1_PrimaryUse,
                    _Vehicle1_SecuritySystem,
                    _Vehicle1_WeeklyCommuteDays);

                Success(Id, RecordType.Vehicle, "Vehicle1_Info Insert Success");
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }
    private void InsertVehicel2_Info()
    {
        long Id = 0;
        if ((_ParamsBlank & 1 << 9) == 0) try
            {
                if (IndividualSecondaryID != 0)
                {
                    Id = AddVehicle(AccountID, IndividualSecondaryID == 0 ? (long?)null : IndividualSecondaryID,
                        _Vehicle2_Year,
                        _Vehicle2_Make,
                        _Vehicle2_Model,
                        _Vehicle2_Submodel,
                        _Vehicle2_AnnualMileage,
                        _Vehicle2_CollisionDeductable,
                        _Vehicle2_ComprehensiveDeductable,
                        _Vehicle2_WhereParked,
                        _Vehicle2_PrimaryUse,
                        _Vehicle2_SecuritySystem,
                        _Vehicle2_WeeklyCommuteDays);
                    Success(Id, RecordType.Vehicle, "Vehicle2_Info Insert Success");
                    //if (Id != 0)
                    //{
                    //    Push(Id, RecordType.Vehicle);
                    //    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Vehicle2_Info Insert Success", "");
                    //}
                    //else
                    //    throw new Exception("No record inserted");
                }
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }
    private void InsertVehicel3_Info()
    {
        long Id = 0;

        if ((_ParamsBlank & 1 << 10) == 0) try
            {
                if (IndividualSecondaryID != 0)
                {
                    Id = AddVehicle(AccountID,
                        (long?)null,
                        _Vehicle3_Year,
                        _Vehicle3_Make,
                        _Vehicle3_Model,
                        _Vehicle3_Submodel,
                        _Vehicle3_AnnualMileage,
                        _Vehicle3_CollisionDeductable,
                        _Vehicle3_ComprehensiveDeductable,
                        _Vehicle3_WhereParked,
                        _Vehicle3_PrimaryUse,
                        _Vehicle3_SecuritySystem,
                        _Vehicle3_WeeklyCommuteDays);
                    Success(Id, RecordType.Vehicle, "Vehicle3_Info Insert Success");
                    //Push(Id, RecordType.Vehicle);
                    //if (Id != 0)
                    //    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Vehicle3_Info Insert Success", "");
                }
            }
            catch (Exception ex)
            {
                Capture(ex);
                //ErrorFlag = true;
                //ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), ex.Message);
            }
    }


    private void InsertHome1_Info()
    {
        long Id = 0;
        if ((_ParamsBlank & 1 << 11) == 0) try
            {
                Id = AddHome(
                    AccountID,
                    IndividualID,
                    _Home1_YearBuilt,
                    _Home1_SqFootage,
                    _Home1_DwellingType,
                    _Home1_DesignType,
                    _Home1_RoofAge,
                    _Home1_RoofType,
                    _Home1_FoundationType,
                    _Home1_HeatingType,
                    _Home1_ExteriorWallType,
                    _Home1_NumberOfClaims,
                    _Home1_NumberOfBedrooms,
                    _Home1_NumberOfBathrooms,
                    _Home1_ReqCoverage);

                Success(Id, RecordType.Home, "Home1_Info Insert Success");
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }
    private void InsertHome2_Info()
    {
        long Id = 0;
        if ((_ParamsBlank & 1 << 12) == 0) try
            {
                if (IndividualSecondaryID > 0)
                {
                    Id = AddHome(AccountID, IndividualSecondaryID == 0 ? (long?)null : IndividualSecondaryID,
                         _Home2_YearBuilt,
                         _Home2_SqFootage,
                         _Home2_DwellingType,
                         _Home2_DesignType,
                         _Home2_RoofAge,
                         _Home2_RoofType,
                      _Home2_FoundationType,
                          _Home2_HeatingType,
                     _Home2_ExteriorWallType,
                     _Home2_NumberOfClaims,
                     _Home2_NumberOfBedrooms,
                     _Home2_NumberOfBathrooms,
                     _Home2_ReqCoverage);
                    Success(Id, RecordType.Home, "Home2_Info Insert Success");
                    //ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), , "");
                }
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }
    private void InsertHome3_Info()
    {
        long Id = 0;
        if ((_ParamsBlank & 1 << 13) == 0) try
            {
                if (IndividualSecondaryID > 0)
                {
                    Id = AddHome(AccountID, (long?)null,
                        _Home3_YearBuilt,
                        _Home3_SqFootage,
                        _Home3_DwellingType,
                        _Home3_DesignType,
                        _Home3_RoofAge,
                        _Home3_RoofType,
                        _Home3_FoundationType,
                        _Home3_HeatingType,
                        _Home3_ExteriorWallType,
                        _Home3_NumberOfClaims,
                        _Home3_NumberOfBedrooms,
                        _Home3_NumberOfBathrooms,
                        _Home3_ReqCoverage);
                    Success(Id, RecordType.Home, "Home3_Info Insert Success");
                    //ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Home3_Info Insert Success", "");
                }
            }
            catch (Exception ex)
            {
                Capture(ex);
            }
    }

    private XmlDocument ErrorXML(string paramAccountID = "", string errorCode = "1", string errorDesc = "Error", string individualCount = "0")
    {
        System.Xml.XmlNode nodeAccount = nodeAccounts.AppendChild(docMain.CreateElement("Account"));

        nodeAccount.Attributes.Append(docMain.CreateAttribute("id")).InnerText = paramAccountID;
        nodeAccount.Attributes.Append(docMain.CreateAttribute("error")).InnerText = errorCode;
        nodeAccount.Attributes.Append(docMain.CreateAttribute("error_desc")).InnerText = errorDesc;

        System.Xml.XmlNode nodeAccountChild = nodeAccount.AppendChild(docMain.CreateElement("individuals"));

        nodeAccountChild.Attributes.Append(docMain.CreateAttribute("count")).InnerText = individualCount;

        return docMain;
    }
    // SZ [Aug 29, 2013] This functions captures the exception and prepares the response appropiately
    void Capture(Exception ex)
    {
        ErrorFlag = true;
        while (ex.Message.Contains("inner exception for details"))
            ex = ex.InnerException;

        ErrorXML(AccountID.ToString(), ErrorCodes.GeneralError.ToString(), ex.Message);
    }
    // SZ [Aug 29, 2013] This functions checks for the successful insert. If not throws the error
    void Success(long Id, RecordType eType, string message)
    {
        string stype =
            eType == RecordType.Individual ? "Individuals" :
            eType == RecordType.Home ? "Homes" :
            eType == RecordType.Driver ? "Drivers" :
            eType == RecordType.Vehicle ? "Vehicles" :
            eType == RecordType.Lead ? "Leads" :
            eType == RecordType.Account ? "Accounts" :
            "Unknown";

        if (Id > 0)
        {
            //Call method to process leads and invoke SignalR
            NotifyDialerSignalR();

            //_journal.Mark(eType, Id);
            ErrorXML(Id.ToString(), ErrorCodes.InsertSuccess.ToString(), message);
        }
        else
            throw new Exception(string.Format("No record inserted for {0}", stype));

    }
    bool CanContinue { get { return !ErrorFlag; } }

    //TM [11 July 2014] temporarily placed as is till further action
    private static string NotifyDialerSignalR()
    {
        try
        {
            //var parm = new System.Data.SqlClient.SqlParameterFluent().ToObjectArray();
            ////Loader.ExecuteStoreProcedure("spGalUpdate", parm);

            //Loader.Database.SqlQuery<bool>("exec spGalUpdate", parm).FirstOrDefault();

            {
                string url = System.Configuration.ConfigurationManager.AppSettings["SignalRDialerWebServiceURL"].ToString();
                Uri address = new Uri(url);
                //Create the web request
                System.Net.HttpWebRequest extrequest = System.Net.WebRequest.Create(address) as System.Net.HttpWebRequest;

                // Set type to POST 
                extrequest.Method = "POST";
                extrequest.ContentType = "application/x-www-form-urlencoded";

                // Create the data we want to send 
                byte[] byteData = System.Text.UTF8Encoding.UTF8.GetBytes(string.Empty);

                // Set the content length in the request headers 
                extrequest.ContentLength = byteData.Length;

                // Write data 
                using (System.IO.Stream postStream = extrequest.GetRequestStream())
                {
                    postStream.Write(byteData, 0, byteData.Length);
                }
                // Get response 
                //extrequest.GetResponse();
                using (System.Net.HttpWebResponse response1 = extrequest.GetResponse() as System.Net.HttpWebResponse)
                {
                    //response1.Close();
                }
            }
        }
        catch (Exception e)
        {
            return e.Message;
        }
        return string.Empty;
    }

    #region Extraction Functions
    private bool ExtractBoolean(string paramValue, string paramVariable = "", bool IsRequired = false)
    {
        bool valBoolean = false;
        if (String.IsNullOrEmpty(paramValue) && IsRequired == false) return valBoolean;
        bool flag = bool.TryParse(paramValue, out valBoolean);
        if (flag == false)
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.BoolDataType.ToString(), "Error: " + paramVariable + " boolean parameter not in a correct form. Use true/1 or false/0.");
        }
        return valBoolean;
    }
    private int ExtractInteger(string paramValue, string paramVariable = "", bool IsRequired = false)
    {
        int valueInner = 0;
        if (String.IsNullOrEmpty(paramValue) && IsRequired == false) return valueInner;
        bool flag = int.TryParse(paramValue, out valueInner);
        if (flag == false)
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.IntDataType.ToString(), "Error: " + paramVariable + " integer parameter not in a correct form.");
        }
        return valueInner;
    }
    private DateTime ExtractDateTime(string paramValue, string paramVariable = "", bool IsRequired = false)
    {
        DateTime valueInner = DateTime.MinValue;
        if (String.IsNullOrEmpty(paramValue) && IsRequired == false) return valueInner;
        bool flag = DateTime.TryParse(paramValue, out valueInner);
        if (flag == false)
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.DateTime.ToString(), "Error: " + paramVariable + " datetime parameter not in a correct form.");
        }
        return valueInner;
    }
    private long ExtractLong(string paramValue, string paramVariable = "", bool IsRequired = false)
    {
        long valueInner = 0;
        if (String.IsNullOrEmpty(paramValue) && IsRequired == false) return valueInner;
        bool flag = long.TryParse(paramValue, out valueInner);
        if (flag == false)
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.IntDataType.ToString(), "Error: " + paramVariable + " integer parameter not in a correct form.");
        }
        return valueInner;
    }
    private decimal ExtractDecimal(string paramValue, string paramVariable = "", bool IsRequired = false)
    {
        decimal valueInner = 0;
        if (String.IsNullOrEmpty(paramValue) && IsRequired == false) return valueInner;
        bool flag = decimal.TryParse(paramValue, out valueInner);
        if (flag == false)
        {
            ErrorFlag = true;
            ErrorXML("", ErrorCodes.DecimalDataType.ToString(), "Error: " + paramVariable + " decimal parameter not in a correct form.");
        }
        return valueInner;
    }
    #endregion

    #region Others
    public XmlDocument InsertAccountsAndDetails(string pAccountID, string pIndividualID, DriverInfo_Record[] nDriverInfo, Vehicle_Record[] nVehicleInfo, Home_Record[] nHomeInfo)
    {
        try
        {
            docMain = new System.Xml.XmlDocument();
            nodeAccounts = docMain.AppendChild(docMain.CreateElement("Accounts"));
            AccountID = ExtractLong(pAccountID, "pAccountID");
            IndividualID = ExtractLong(pIndividualID, "pIndividualID");
            int countDriverInserted = 0, countVehicleInserted = 0, countHomeInserted = 0;
            if (AccountID != 0)
            {
                if (nDriverInfo.Length <= 10 && nDriverInfo != null)
                {
                    foreach (DriverInfo_Record nDriverInfoItem in nDriverInfo)
                    {
                        DriverInfo nDriver = new DriverInfo();
                        nDriver = SetDriverValues(nDriver, nDriverInfoItem);
                        Engine.DriverActions.Add(nDriver);
                        countDriverInserted++;
                    }
                    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), countDriverInserted.ToString() + " Drivers Insert Success");
                }
                if (nVehicleInfo.Length <= 10 && nVehicleInfo != null)
                {
                    foreach (Vehicle_Record vehicleItem in nVehicleInfo)
                    {
                        Vehicle nVehicle = new Vehicle();
                        nVehicle = SetVehicleValues(nVehicle, vehicleItem);
                        var recordAdded = Engine.VehiclesActions.Add(nVehicle, "Web Service");
                        countVehicleInserted++;
                    }
                    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), countVehicleInserted.ToString() + " Vehicles Insert Success");
                }
                if (nHomeInfo.Length <= 10 && nHomeInfo != null)
                {
                    foreach (Home_Record homeItem in nHomeInfo)
                    {
                        Home nHome = new Home();
                        nHome = SetHomeValues(nHome, homeItem);
                        Engine.HomeActions.Add(nHome);
                        countHomeInserted++;
                    }
                    ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), countHomeInserted.ToString() + " Vehicles Insert Success");
                }
            }

            return docMain;
        }
        catch (Exception ex)
        {
            ErrorXML(AccountID.ToString(), ErrorCodes.GeneralError.ToString(), ShowErrorDetails(ex), "");
            return docMain;
        }
    }

    [WebMethod]
    public XmlDocument OptimizedCallInsertAccountAndDetails(string CampaignId, string StatusId, Individual_Record paramPrimary, Individual_Record paramSecondary, DriverInfo_Record[] nDriverInfo, Vehicle_Record[] nVehicleInfo, Home_Record[] nHomeInfo)
    {
        docMain = new System.Xml.XmlDocument();
        nodeAccounts = docMain.AppendChild(docMain.CreateElement("Accounts"));

        _CampaignId = ExtractInteger(CampaignId, "CampaignId");
        _StatusId = ExtractInteger(StatusId, "StatusId");

        Account A = Engine.AccountActions.Add(new Account(),K_WEBSERVICE);
        AccountID = A.Key;
        //saving Lead
        Lead L = new Lead();
        L.AccountId = AccountID;
        L.CampaignId = _CampaignId;
        //L.ActionId = Convert.ToInt32(ddActions.SelectedValue);
        L.StatusId = _StatusId;
        //L.Agent = ddUsers.SelectedValue;
        //L.Csr = ddCSR.SelectedValue;

        LeadID = Engine.LeadsActions.Add(L, K_WEBSERVICE).Key;
        A.PrimaryLeadKey = LeadID;
        int countIndividual = 0;
        bool bNewRecord = false;
        Individual IPrimary = Engine.AccountActions.GetIndividual(AccountID, SalesTool.DataAccess.IndividualType.Primary);
        if (IPrimary == null)
        {
            IPrimary = new Individual();
            bNewRecord = true;
        }
        if (!(IPrimary.LastName == paramPrimary.LastName && IPrimary.Zipcode == paramPrimary.Zipcode && IPrimary.Birthday == paramPrimary.Birthday))
        {
            if (paramPrimary.FirstName == null ? false : paramPrimary.FirstName.Length > 0 && paramPrimary.LastName == null ? false : paramPrimary.LastName.Length > 0 && (paramPrimary.DayPhone > 0 || paramPrimary.EveningPhone > 0 || paramPrimary.CellPhone > 0))
            {
                IPrimary = paramPrimary.ToIndividual(_States); //SetIndividualValues(IPrimary, paramPrimary);
                if (bNewRecord)
                {
                    IPrimary = Engine.IndividualsActions.Add(IPrimary, K_WEBSERVICE);
                    A.PrimaryIndividualId = IPrimary.Key;
                    countIndividual++;
                }
                else
                    Engine.IndividualsActions.Change(IPrimary, K_WEBSERVICE);
            }
            else
            {
                return ErrorXML("", "3", "Error: FirstName, LastName and at least one phone number (Day,Evening,Mobile) is required in primary individual.");
            }
            bNewRecord = false;
            Individual ISecondary = Engine.AccountActions.GetIndividual(AccountID, SalesTool.DataAccess.IndividualType.Secondary);
            if (ISecondary == null)
            {
                bNewRecord = true;
                ISecondary = new Individual();
            }
            if (HasSpouce)
            {
                if (paramSecondary.FirstName == null ? false : paramSecondary.FirstName.Length > 0 && paramSecondary.LastName == null ? false : paramSecondary.LastName.Length > 0 && (paramSecondary.DayPhone > 0 || paramSecondary.EveningPhone > 0 || paramSecondary.CellPhone > 0))
                {
                    ISecondary = paramSecondary.ToIndividual(_States); //SetIndividualValues(ISecondary, paramSecondary);
                    if (bNewRecord)
                    {
                        ISecondary = Engine.IndividualsActions.Add(ISecondary, K_WEBSERVICE);
                        A.SecondaryIndividualId = ISecondary.Key;
                        countIndividual++;
                    }
                    else
                        Engine.IndividualsActions.Change(ISecondary, K_WEBSERVICE);
                }
                else
                {
                    return ErrorXML("", "3", "Error: FirstName, LastName and at least one phone number (Day,Evening,Mobile) is required in secondary individual.");
                }
            }
            else
            {
                Engine.IndividualsActions.Delete(A.SecondaryIndividualId ?? 0, K_WEBSERVICE);
                A.SecondaryIndividualId = null;
            }
            Engine.AccountActions.Update(A, K_WEBSERVICE);
        }

        ErrorXML(AccountID.ToString(), ErrorCodes.InsertSuccess.ToString(), "Insert Success", countIndividual.ToString());
        return docMain;

    }

    private Individual SetIndividualValues(Individual IIndividual, Individual_Record paramPrimary)
    {
        IIndividual.AccountId = AccountID;
        IIndividual.FirstName = paramPrimary.FirstName;
        IIndividual.LastName = paramPrimary.LastName;
        IIndividual.Gender = paramPrimary.Gender;
        IIndividual.DayPhone = paramPrimary.DayPhone;
        IIndividual.EveningPhone = paramPrimary.EveningPhone;
        IIndividual.CellPhone = paramPrimary.CellPhone;
        IIndividual.FaxNmbr = paramPrimary.FaxNumber;
        IIndividual.Birthday = paramPrimary.Birthday;
        IIndividual.Address1 = paramPrimary.Address1;
        IIndividual.Address2 = paramPrimary.Address2;
        IIndividual.City = paramPrimary.City;
        var statePrimary = Engine.Constants.States.Where(x => x.Abbreviation == paramPrimary.StateName).Select(y => new { y.Id, y.Abbreviation }).SingleOrDefault();
        if (statePrimary != null)
        {
            IIndividual.StateID = statePrimary.Id;
        }
        IIndividual.Smoking = paramPrimary.Smoking;
        IIndividual.Zipcode = paramPrimary.Zipcode;
        return IIndividual;
    }

    private Home SetHomeValues(Home nHome, Home_Record nHome_WebService)
    {

        nHome.AccountId = AccountID;
        nHome.Individualkey = nHome_WebService.Individualkey;

        nHome.YearBuilt = nHome_WebService.YearBuilt;
        nHome.SqFootage = nHome.SqFootage;
        nHome.DwellingType = nHome.DwellingType;
        nHome.DesignType = nHome_WebService.DesignType;
        nHome.RoofAge = nHome_WebService.RoofAge;
        nHome.RoofType = nHome_WebService.RoofType;
        nHome.FoundationType = nHome.FoundationType;
        nHome.HeatingType = nHome_WebService.HeatingType;
        nHome.ExteriorWallType = nHome_WebService.ExteriorWallType;
        nHome.NumberOfClaims = nHome_WebService.NumberOfClaims;
        nHome.NumberOfBedrooms = nHome_WebService.NumberOfBedrooms;
        nHome.NumberOfBathrooms = nHome_WebService.NumberOfBathrooms;
        nHome.ReqCoverage = nHome_WebService.ReqCoverage;
        return nHome;

    }
    private DriverInfo SetDriverValues(DriverInfo nDriver, DriverInfo_Record nDriver_WebService)
    {
        nDriver.AccountId = AccountID;
        nDriver.IndividualId = IndividualID;

        nDriver.LisenceNumber = nDriver_WebService.LicenseNumber;
        nDriver.DlState = nDriver_WebService.DlState;
        nDriver.LicenseStatus = nDriver_WebService.LicenseStatus;
        nDriver.MaritalStatus = nDriver_WebService.MaritalStatus;
        nDriver.AgeLicensed = nDriver_WebService.AgeLicensed;
        nDriver.YearsAtResidence = nDriver_WebService.YearsAtResidence;
        nDriver.Occupation = nDriver_WebService.Occupation;
        nDriver.YearsWithCompany = nDriver_WebService.YearsWithCompany;
        nDriver.YrsInField = nDriver_WebService.YrsInField;
        nDriver.Education = nDriver_WebService.Education;
        nDriver.TicketsAccidentsClaims = nDriver_WebService.TicketsAccidentsClaims;
        nDriver.NmbrIncidents = nDriver_WebService.NmbrIncidents;
        nDriver.IncidentType = nDriver_WebService.IncidentType;
        nDriver.IncidentDescription = nDriver_WebService.IncidentDescription;
        nDriver.IncidentDate = nDriver_WebService.IncidentDate;
        nDriver.ClaimPaidAmount = nDriver_WebService.ClaimPaidAmount;
        nDriver.St22 = nDriver_WebService.Sr22;
        nDriver.PolicyYears = nDriver_WebService.PolicyYears;
        return nDriver;
    }
    private Vehicle SetVehicleValues(Vehicle nVehicle, Vehicle_Record nVehicle_WebService)
    {
        nVehicle.Year = nVehicle_WebService.Year;
        nVehicle.Make = nVehicle_WebService.Make;
        nVehicle.Model = nVehicle_WebService.Model;
        nVehicle.Submodel = nVehicle_WebService.Submodel;

        nVehicle.AnnualMileage = nVehicle_WebService.AnnualMileage;
        nVehicle.CollisionDeductable = nVehicle_WebService.CollisionDeductable;
        nVehicle.ComprehensiveDeductable = nVehicle_WebService.ComprehensiveDeductable;
        nVehicle.WhereParked = nVehicle_WebService.WhereParked;
        nVehicle.PrimaryUse = nVehicle_WebService.PrimaryUse;
        nVehicle.SecuritySystem = nVehicle_WebService.SecuritySystem;

        nVehicle.WeeklyCommuteDays = nVehicle_WebService.WeeklyCommuteDays;
        nVehicle.Individualkey = IndividualID;
        nVehicle.AccountId = AccountID;
        return nVehicle;
    }
     
    private string ShowErrorDetails(Exception ex)
    {
        while (ex.Message.Contains("inner exception for details"))
            ex = ex.InnerException;
        return ex.Message;
    }

    [WebMethod]
    public XmlDocument InsertByXML(XmlDocument receivedXML)
    {
        //XmlDocument saveXml = new XmlDocument();
        //saveXml.PreserveWhitespace = false;
        //saveXml.LoadXml(receivedXML);

        return receivedXML;
    }

    [Serializable()]
    public class Individual_Record
    {

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Gender { get; set; }

        public bool? Smoking { get; set; }

        public long? Age { get; set; }

        public string Address1 { get; set; }

        public string Address2 { get; set; }

        public string City { get; set; }

        public DateTime? Birthday { get; set; }

        public long? DayPhone { get; set; }

        public long? EveningPhone { get; set; }

        public long? FaxNumber { get; set; }

        //public int? Zipcode { get; set; }

        public string Zipcode { get; set; }

        public long? CellPhone { get; set; }

        public string Notes { get; set; }

        public string Relation { get; set; }

        public string StateName { get; set; }

        public Individual ToIndividual(Dictionary<string, byte> Map)
        {
            Individual I = new Individual
            {
                FirstName = FirstName,
                LastName = LastName,
                Gender = Gender,
                Smoking = Smoking,
                Age = Age,
                Address1 = Address1,
                Address2 = Address2,
                City = City,
                Birthday = Birthday,
                DayPhone = DayPhone,
                EveningPhone = EveningPhone,
                FaxNmbr = FaxNumber,
                Zipcode = Zipcode,
                CellPhone = CellPhone,
                Notes = Notes,
                Relation = Relation
            };

            if (Map.ContainsKey(StateName))
                I.StateID = Map[StateName];

            return I;
        }
    }

    [Serializable()]
    public class DriverInfo_Record
    {
        public string DlState { get; set; }

        public string MaritalStatus { get; set; }

        public string LicenseStatus { get; set; }

        public long? AgeLicensed { get; set; }

        public long? YearsAtResidence { get; set; }

        public string Occupation { get; set; }

        public long? YearsWithCompany { get; set; }

        public long? YrsInField { get; set; }

        public string Education { get; set; }

        public long? NmbrIncidents { get; set; }

        public string Sr22 { get; set; }

        public long? PolicyYears { get; set; }

        public long? IndividualId { get; set; }

        public string LicenseNumber { get; set; }

        public string CurrentCarrier { get; set; }

        public string LiabilityLimit { get; set; }

        public DateTime? CurrentAutoXDate { get; set; }

        public string MedicalPayment { get; set; }

        public long? AccountId { get; set; }

        public string TicketsAccidentsClaims { get; set; }

        public string IncidentType { get; set; }

        public string IncidentDescription { get; set; }

        public DateTime? IncidentDate { get; set; }

        public decimal? ClaimPaidAmount { get; set; }
    }

    [Serializable()]
    public class Vehicle_Record
    {

        public long? Year { get; set; }

        public string Make { get; set; }

        public string Model { get; set; }

        public string Submodel { get; set; }

        public long? AnnualMileage { get; set; }

        public long? WeeklyCommuteDays { get; set; }

        public string PrimaryUse { get; set; }

        public string ComprehensiveDeductable { get; set; }

        public string CollisionDeductable { get; set; }

        public string SecuritySystem { get; set; }

        public string WhereParked { get; set; }

        public long? IndividualId { get; set; }

        public long? PolicyId { get; set; }

        public long Individualkey { get; set; }

        public long? AccountId { get; set; }
    }

    [Serializable()]
    public class Home_Record
    {
        public string CurrentCarrier { get; set; }

        public string CurrentXdateLeadInfo { get; set; }

        public string YearBuilt { get; set; }

        public string DwellingType { get; set; }

        public string DesignType { get; set; }

        public long? RoofAge { get; set; }

        public string RoofType { get; set; }

        public string FoundationType { get; set; }

        public string HeatingType { get; set; }

        public string ExteriorWallType { get; set; }

        public long? NumberOfClaims { get; set; }

        public long NumberOfBedrooms { get; set; }

        public long? SqFootage { get; set; }

        public string ReqCoverage { get; set; }

        public long? AccountId { get; set; }

        public long Individualkey { get; set; }

        public long? NumberOfBathrooms { get; set; }
    }
    #endregion
}

internal enum RecordType { Account = 1, Lead = 2, Individual = 3, Home = 5, Driver = 6, Vehicle = 7, DuplicateManagement };
internal class DBOperationJournal
{
    // SZ [Aug 28, 2013] this class provides a limited rollback journal fucntionality for record insertions of 6 entities only
    // for every insertion, call the mark with the entity inserted. The journal logs it in proper order.
    // if the rolback is called. it performs deletions in exact reverse order. thus maintainng the integrity of the database

    private const string K_WEBSERVICE = "Web Service";
    class OperationStatus
    {
        public long lRecordId = 0L;
        public RecordType eType;
        public bool bExisted = false;

        internal OperationStatus(long id, RecordType etype, bool existed = false)
        {
            lRecordId = id;
            eType = etype;
            bExisted = existed;
        }
    };


    DBEngine _engine = null;
    Stack<OperationStatus> _Stk = new Stack<OperationStatus>();

    internal DBOperationJournal(DBEngine reng)
    {
        _engine = reng;
    }

    //SZ [Aug 28, 2013] Perform a delete of whatever has been inserted into teh DB so far
    internal void Rollback()
    {
        Stack<OperationStatus> stk = _Stk;
        while (stk.Count > 0) //SZ [Aug 28, 2013] Pop every item and perform delete. 
        {
            OperationStatus item = stk.Pop();
            try
            {
                switch (item.eType)
                {
                    case RecordType.Vehicle:
                        _engine.VehiclesActions.Delete(item.lRecordId, true);
                        break;
                    case RecordType.Driver:
                        _engine.DriverActions.Delete(item.lRecordId, true);
                        break;
                    case RecordType.Home:
                        _engine.HomeActions.Delete(item.lRecordId, true);
                        break;
                    case RecordType.Individual:
                        _engine.IndividualsActions.Delete(item.lRecordId, K_WEBSERVICE, true);
                        break;
                    case RecordType.Lead:
                        // SZ [Nov 5, 2013] when this lead is removed and it was not pre-existed, set the primary lead to previous primary lead.
                        if (!item.bExisted)
                        {
                            long accid = _engine.LeadsActions.Get(item.lRecordId).AccountId;
                            _engine.LeadsActions.Delete(item.lRecordId, K_WEBSERVICE);
                            Lead L = _engine.LeadsActions.All.Where(x => x.AccountId == accid).OrderByDescending(x => x.ChangedOn).FirstOrDefault();
                            if (L != null)
                            {
                                long leadid = L.Key;
                                _engine.AccountActions.SetPrimaryLead(accid, leadid, K_WEBSERVICE);
                            }
                        }

                        break;
                    case RecordType.Account:
                        if (!item.bExisted)
                            _engine.AccountActions.Delete(item.lRecordId, K_WEBSERVICE , true);
                        break;
                }
            }
            catch //(Exception ex)
            {
                // SZ do something useful here. perhaps report hte issue
            }
        }
    }
    //SZ [Aug 28, 2013] Perform a markign of the records accoridng to the insertions made.
    void Mark(RecordType type, long Id, bool bExisted = false)
    {
        //SZ [Aug 28, 2013] Code to handle error
        _Stk.Push(new OperationStatus(Id, type, bExisted));
    }

    internal void Mark(Account arg) { Mark(RecordType.Account, arg.Key); }
    internal void Mark(Lead arg) { Mark(RecordType.Lead, arg.Key); }
    internal void Mark(Individual arg) { Mark(RecordType.Individual, arg.Key); }
    internal void Mark(Home arg) { Mark(RecordType.Home, arg.Id); }
    internal void Mark(DriverInfo arg) { Mark(RecordType.Driver, arg.Key); }
    internal void Mark(Vehicle arg) { Mark(RecordType.Vehicle, arg.Key); }

}