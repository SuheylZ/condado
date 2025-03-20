using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.Script.Serialization;
using NLog;

namespace SignalR_Engine
{    
    /// <summary>
    /// Summary description for WebService1
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
    // [System.Web.Script.Services.ScriptService]
    public class SignalRWebServices : System.Web.Services.WebService
    {

        static  NLog.Logger Writer { get { return NLog.LogManager.GetLogger(Konstants.LOGGER_NAME); } } 
        [WebMethod(MessageName = "InvokeHubUpdateCounts")]
        public void InvokeHubUpdateCounts(string SkillID)
        {
            try
            {
                Writer.Info("InvokeHubUpdateCounts, SkillID: " + SkillID);                
                SelectCareHub.UpdatePersonalQueueCounts(SkillID);
            }
            catch (Exception e)
            {
                Writer.Info("InvokeHubUpdateCounts, " + e.ToString());
            }
        }

        [WebMethod(MessageName = "InvokeDialerHubUpdateCounts")]
        public void InvokeDialerHubUpdateCounts(string serializedUsersList = null)
        {
            try
            {
                Writer.Info("InvokeDialerHubUpdateCounts, serializedUsersList: " + serializedUsersList);
                if (serializedUsersList != null)
                {
                    var jsonSerialiser = new JavaScriptSerializer();
                    List<string> lstUsers = new List<string>();
                    lstUsers = jsonSerialiser.Deserialize<List<string>>(serializedUsersList);
                    SelectCareHub.UpdateGWBCounts(lstUsers);
                }
                else
                {
                    SelectCareHub.UpdateGWBCounts(null);
                }
            }
            catch (Exception e)
            {
                Writer.Info("InvokeDialerHubUpdateCounts, serializedUsersList" + serializedUsersList + "\n" + e.ToString());
            }
        }

        [WebMethod(MessageName = "InvokeACDHubUpdateCounts")]
        public void InvokeACDHubUpdateCounts(string serializedUsersList = null)
        {
            try
            {
                Writer.Info("InvokeACDHubUpdateCounts, serializedUsersList: " + serializedUsersList);
                if (serializedUsersList != null)
                {
                    var jsonSerialiser = new JavaScriptSerializer();
                    List<string> lstUsers = new List<string>();
                    lstUsers = jsonSerialiser.Deserialize<List<string>>(serializedUsersList);
                    SelectCareHub.UpdateACDCounts(lstUsers);
                }
                else
                {
                    SelectCareHub.UpdateACDCounts(null);
                }
            }
            catch (Exception e)
            {
                Writer.Info("InvokeACDHubUpdateCounts, serializedUsersList" + serializedUsersList + "\n" + e.ToString());
            }
        }
    }
}
