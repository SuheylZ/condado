using System;
using System.Collections.Generic;
using System.IO;
//using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace GalEngine
{
    public static class SignalRConnection
    {
        public static string NotifySignalR(List<string> lstUserIDs, SignalRMethod method)
        {
            try
            {
                string url = "";
                if (method == SignalRMethod.AcdGal)
                {
                    url = DataAccess.getAcdGalSignalRurl();
                }
                else if (method == SignalRMethod.WebGal)
                {
                    url = DataAccess.getWebGalSignalRurl();
                }
                Uri address = new Uri(url);
                //Create the web request
                System.Net.HttpWebRequest extrequest = System.Net.WebRequest.Create(address) as System.Net.HttpWebRequest;

                // Set type to POST 
                extrequest.Method = "POST";
                extrequest.ContentType = "application/x-www-form-urlencoded";

                
                var jsonSerialiser = new JavaScriptSerializer();
                var json = jsonSerialiser.Serialize(lstUserIDs);


                // Create the data we want to send 
                StringBuilder data = new StringBuilder();
                data.Append("serializedUsersList=" + json);
                // Create a byte array of the data we want to send 
                byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());

                // Set the content length in the request headers 
                extrequest.ContentLength = byteData.Length;

                // Write data 
                using (Stream postStream = extrequest.GetRequestStream())
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
            catch (Exception e)
            {
                return e.Message;
            }
            return string.Empty;
        }
    }
    public enum SignalRMethod
    { 
        AcdGal,
        WebGal
    };
}