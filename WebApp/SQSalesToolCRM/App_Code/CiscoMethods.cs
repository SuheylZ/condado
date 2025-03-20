using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Net;
using MODELS = SalesTool.DataAccess.Models;
using System.Web.Script.Serialization;
/// <summary>
/// Summary description for CiscoMethods
/// </summary>
public class CiscoMethods
{
    public CiscoMethods()
    {

    }

    public string CiscoAuthorization(string agentId, string extention, string authorizationCode, ref string exceptionMessage)
    {
        string responseText = string.Empty;
        try
        {
            string _url = @"http://fnse-a2.selectquote.com/finesse/api/User/" + agentId;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "PUT";
            request.Headers.Add("Authorization", "Basic " + authorizationCode);
            request.Headers.Add("contenttype", "application/xml");
            request.ContentType = "Application/XML";
            using (var s = request.GetRequestStream())
            using (var sw = new StreamWriter(s))
            {
                sw.Write("<User><state>LOGIN</state><extension>" + extention + "</extension></User>");
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode.ToString() == "Accepted")
                {
                    string cookieValue = response.Headers.GetValues("Set-Cookie")[1];
                    JavaScriptSerializer jc = new JavaScriptSerializer();

                    var encoding = ASCIIEncoding.ASCII;
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
                    {
                        responseText = reader.ReadToEnd();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            while (ex.Message.Contains("inner exception for details"))
                ex = ex.InnerException;
            exceptionMessage = "Unable to authenticate with Softphone.";
            return null;
        }
        return responseText;
    }

    public void CiscoDialNumber(string userName, string authorizationCode, string phoneNumber, string extention,ref string exceptionMessage)
    {
        try
        {
            string _url = @"http://fnse-a2.selectquote.com/finesse/api/User/" + userName + "/Dialogs";
            phoneNumber = phoneNumber.StartsWith("91") ? phoneNumber : "91" + phoneNumber;
            HttpWebRequest request = null;
            request = (HttpWebRequest)WebRequest.Create(_url);
            request.Method = "POST";
            request.Headers.Add("Authorization", "Basic " + authorizationCode);
            request.Headers.Add("contenttype", "application/xml");
            request.ContentType = "Application/XML";
            using (var s = request.GetRequestStream())
            using (var sw = new StreamWriter(s))
            {
                sw.Write("<Dialog><requestedAction>MAKE_CALL</requestedAction><fromAddress>" + extention + "</fromAddress><toAddress>" + phoneNumber + "</toAddress></Dialog>");
            }
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                if (response.StatusCode.ToString() == "Accepted")
                {
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            while (ex.Message.Contains("inner exception for details"))
                ex = ex.InnerException;
            exceptionMessage = "Unable to authenticate with Softphone.";
        }
    }
}