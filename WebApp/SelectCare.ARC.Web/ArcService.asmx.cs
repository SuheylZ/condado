using System;
using System.Collections.Generic;
using System.Data.EntityClient;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using SalesTool.DataAccess;
using SelectCare.ARC;
using SelectCare.ARC.ArcRequest;

/// <summary>
/// Summary description for ArcService
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class ArcService : System.Web.Services.WebService
{

    public ArcService()
    {
        //Uncomment the following line if using designed components 
        //InitializeComponent(); 
    }
    /// <summary>
    /// 4.1
    /// </summary>
    /// <param name="leadRequest"></param>
    /// <returns></returns>
    [WebMethod]
    public ArcResponse InsertUpdateLead(ArcLeadRequest leadRequest)
    {
        if (!IsTermLife)
        {
            SoapException se = new SoapException("Function is only available at life insurance", SoapException.ServerFaultCode);
            throw se;
        }
        ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
        var result = manager.ProcessInsertAndUpdate(leadRequest);
        manager.Dispose();
        return result;
    }

    /// <summary>
    /// 4.2
    /// </summary>
    /// <param name="campaignRequest"></param>
    /// <returns></returns>
    [WebMethod]
    public ArcResponse UpdateCampaign(ArcCampaignRequest campaignRequest)
    {
        if (!IsTermLife)
        {
            SoapException se = new SoapException("Function is only available at life insurance", SoapException.ServerFaultCode);
            throw se;
        }
        ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
        var result = manager.UpdateCampaign(campaignRequest);
        manager.Dispose();
        return result;
    }

    /// <summary>
    /// 4.3
    /// </summary>
    /// <param name="arcStatusRequest"></param>
    /// <returns></returns>
    [WebMethod]
    public ArcResponse UpdateStatus(ArcStatusRequest arcStatusRequest)
    {
        if (!IsTermLife)
        {
            SoapException se = new SoapException("Function is only available at life insurance", SoapException.ServerFaultCode);
            throw se;
        }
        ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
        var result = manager.UpdateStatus(arcStatusRequest);
        manager.Dispose();
        return result;
    }

    /// <summary>
    /// 4.4
    /// </summary>
    /// <param name="arcStopCommunicationRequest"></param>
    /// <returns></returns>
    [WebMethod]
    public ArcResponse StopCommunication(ArcStopCommunicationRequest arcStopCommunicationRequest)
    {
        if (!IsTermLife)
        {
            SoapException se = new SoapException("Function is only available at life insurance", SoapException.ServerFaultCode);
            throw se;
        }
        ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
        var result = manager.UpdateStopCommunication(arcStopCommunicationRequest);
        manager.Dispose();
        return result;
    }

    /// <summary>
    /// 4.5
    /// </summary>
    /// <param name="arcAcdCapUpdateRequest"></param>
    /// <returns></returns>
    [WebMethod]
    public ArcAgentResponse AcdCapUpdate(ArcAcdCapUpdateRequest arcAcdCapUpdateRequest)
    {
        if (!IsTermLife)
        {
            SoapException se = new SoapException("Function is only available at life insurance", SoapException.ServerFaultCode);
            throw se;
        }
        ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
        var result = manager.AcdCapUpdate(arcAcdCapUpdateRequest);
        manager.Dispose();
        return result;
    }

    /// <summary>
    /// 4.6
    /// </summary>
    /// <param name="arcConsentUpdateRequest"></param>
    /// <returns></returns>
    [WebMethod]
    public ArcResponse ConsentUpdate(ArcConsentUpdateRequest arcConsentUpdateRequest)
    {
        if (!IsTermLife)
        {
            SoapException se = new SoapException("Function is only available at life insurance", SoapException.ServerFaultCode);
            throw se;
        }
        ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
        var result = manager.ConsentUpdate(arcConsentUpdateRequest);
        manager.Dispose();
        return result;
    }

    /// <summary>
    /// 4.7
    /// </summary>
    /// <returns></returns>
    /// <author>MH:10 June 2014</author>
    [WebMethod]
    public ServiceResponse ServerStatus()
    {
        if (!IsTermLife)
        {
            SoapException se = new SoapException("Function is only available at life insurance", SoapException.ServerFaultCode);
            throw se;
        }
        ServerStatusManger manger = new ServerStatusManger(ADOConnectionString);
        var response = manger.GetServerApplicationStatuses();
        manger.Dispose();
        return response;
    }



    #region Testing purpose
    ////testing purpose only
    //private T GetRequestFromFile<T>(string path)
    //{
    //    FileStream reader = null;
    //    try
    //    {
    //        string filePath = Server.MapPath(path);
    //        reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    //        var serializer = new XmlSerializer(typeof(T));
    //        var obj = (T)serializer.Deserialize(reader);
    //        reader.Close();
    //        return obj;
    //    }
    //    catch (Exception e)
    //    {
    //        if (reader != null) reader.Close();
    //    }
    //    return default(T);
    //}
    //[WebMethod]
    //public ArcResponse Test()
    //{
    //    const string leadPath = "ArcRequestSample/LeadRequest.xml";
    //    const string campaignPath = "ArcRequestSample/CampaignRequest.xml";
    //    const string statusPath = "ArcRequestSample/ArcStatus.xml";
    //    const string stopCommunicationPath = "ArcRequestSample/StopCommunication.xml";
    //    const string AcdAgentPath = "ArcRequestSample/AcdCapUpdate.xml";
    //    const string consentPath = "ArcRequestSample/ConsentUpdate.xml";

    //    //ArcLeadRequest request = GetRequestFromFile<ArcLeadRequest>(leadPath);
    //    //ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
    //    //var result = manager.ProcessInsertAndUpdate(request);

    //    // CampignRequest
    //    CampaignRequest request = GetRequestFromFile<CampaignRequest>(campaignPath);
    //    ArcSystemManager manager = new ArcSystemManager(ApplicationSettings.ADOConnectionString);
    //    var result = manager.UpdateCampaign(request);

    //    ////Staus
    //    //ArcStatusRequest request = GetRequestFromFile<ArcStatusRequest>(statusPath);
    //    //ArcSystemManager manager = new ArcSystemManager(ApplicationSettings.ADOConnectionString);
    //    //var result = manager.UpdateStatus(request);

    //    ////Stop Communication
    //    //ArcStopCommunicationRequest request = GetRequestFromFile<ArcStopCommunicationRequest>(stopCommunicationPath);
    //    //ArcSystemManager manager = new ArcSystemManager(ApplicationSettings.ADOConnectionString);
    //    //var result = manager.UpdateStopCommunication(request);

    //    ////AcdCap
    //    //ArcAcdCapUpdateRequest request = GetRequestFromFile<ArcAcdCapUpdateRequest>(AcdAgentPath);
    //    //ArcSystemManager manager = new ArcSystemManager(ApplicationSettings.ADOConnectionString);
    //    //var result = manager.AcdCapUpdate(request);

    //    ////AcdCap
    //    //ArcConsentUpdateRequest request = GetRequestFromFile<ArcConsentUpdateRequest>(consentPath);
    //    //ArcSystemManager manager = new ArcSystemManager(ApplicationSettings.ADOConnectionString);
    //    //var result = manager.ConsentUpdate(request);
    //    return result;
    //}
    #endregion


    //This is a temporary area for some experimentation
    // DO NOT uncomment it
    //[WebMethod]
    //public bool AddIndividual(XState arg)
    //{
    //    System.Diagnostics.Debug.WriteLine(arg.Login.UserId);
    //    return true;
    //}
    public const string K_ADO_CONNECTION = "ApplicationServices";
    public static string ADOConnectionString
    {
        get
        {

            return System.Configuration.ConfigurationManager.ConnectionStrings[K_ADO_CONNECTION].ConnectionString;
        }
    }
    public static bool IsTermLife { get { return InsuranceType == 2; } }

    public static int InsuranceType
    {
        get
        {
            //int Ans = 0;
            //int.TryParse(System.Configuration.ConfigurationManager.AppSettings[Konstants.K_INSURANCE_TYPE] ?? "", out Ans);
            //return Ans;
            //return ApplicationType;
            if (!_instanceType.HasValue)
            {
                lock (rootSync)
                {
                    if (!_instanceType.HasValue)
                    {
                        const string K_Application_Type = "APPLICATION_TYPE";
                        _instanceType = SalesTool.Common.CGlobalStorage.Instance.Get<int>(K_Application_Type);
                    }
                }
            }
            return _instanceType.Value;

        }
    }
    private static object rootSync = new object();
    private static int? _instanceType;
}
