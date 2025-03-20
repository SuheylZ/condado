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


[DataContract]
public class inContactAuthorizationResponse
{
    [DataMember]
    public string access_token { get; set; }
    [DataMember]
    public string token_type { get; set; }
    [DataMember]
    public int expires_in { get; set; }
    [DataMember]
    public string refresh_token { get; set; }
    [DataMember]
    public string scope { get; set; }
    [DataMember]
    public string resource_server_base_uri { get; set; }
    [DataMember]
    public string refresh_token_server_uri { get; set; }
    [DataMember]
    public int agent_id { get; set; }
    [DataMember]
    public int team_id { get; set; }
}

[DataContract]
public class JoinSessionResponse
{
    [DataMember]
    public string sessionId { get; set; }
}

public class inContactFunctions
{

    public inContactAuthorizationResponse inContactAuthorization(string api_grant_type, string api_scope, string api_agt_usr_name, string api_agt_usr_pass, string api_authorization_code, ref string exceptionMessage)
    {
        try
        {
            //JSON URL
            var url = "https://api.incontact.com/InContactAuthorizationServer/Token";

            //Setup Post String
            string postData = "{\"grant_type\":\"" + api_grant_type + "\",\"username\":\"" + api_agt_usr_name + "\",\"password\":\"" + api_agt_usr_pass + "\",\"scope\":\"" + api_scope + "\"}";


            // corrected to WebRequest from HttpWebRequest
            WebRequest request = WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "basic " + api_authorization_code);

            //using the javascript serializer

            //get a reference to the request-stream, and write the postData to it
            using (Stream s = request.GetRequestStream())
            {
                using (StreamWriter sw = new StreamWriter(s))
                    sw.Write(postData);
            }

            //get response-stream, and use a streamReader to read the content
            using (Stream s = request.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    var jsonData = sr.ReadToEnd();

                    //decode jsonData with javascript serializer

                    return JsonConvert.DeserializeObject<inContactAuthorizationResponse>(jsonData);
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
    }

    public inContactAuthorizationResponse inContactAuthorizationRefresh(inContactAuthorizationResponse authToken, string api_authorization_code)
    {

        //JSON URL
        var url = authToken.refresh_token_server_uri;

        //Setup Post String
        string postData = "{\"grant_type\":\"refresh_token\",\"refresh_token\":\"" + authToken.refresh_token + "\"}";


        // corrected to WebRequest from HttpWebRequest
        WebRequest request = WebRequest.Create(url);

        request.Method = "POST";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "basic " + api_authorization_code);

        //using the javascript serializer

        //get a reference to the request-stream, and write the postData to it
        using (Stream s = request.GetRequestStream())
        {
            using (StreamWriter sw = new StreamWriter(s))
                sw.Write(postData);
        }

        //get response-stream, and use a streamReader to read the content
        using (Stream s = request.GetResponse().GetResponseStream())
        {
            using (StreamReader sr = new StreamReader(s))
            {
                var jsonData = sr.ReadToEnd();

                //decode jsonData with javascript serializer

                return JsonConvert.DeserializeObject<inContactAuthorizationResponse>(jsonData);
            }
        }
    }

    public JoinSessionResponse inContactJoinSession(inContactAuthorizationResponse authToken, ref string exceptionMessage)
    {
        try
        {
            //JSON URL
            var url = authToken.resource_server_base_uri + "/services/v2.0/agent-sessions/join";

            //Setup Post String
            string postData = "";

            // Define Request
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "bearer " + authToken.access_token);
            request.Accept = "application/json";

            //Build Request Stream
            using (Stream s = request.GetRequestStream())
            {
                using (StreamWriter sw = new StreamWriter(s))
                    sw.Write(postData);
            }

            //Get Response Stream
            using (Stream s = request.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    var jsonData = sr.ReadToEnd();

                    //decode jsonData with javascript serializer
                    return JsonConvert.DeserializeObject<JoinSessionResponse>(jsonData);
                }
            }
        }
        catch (Exception ex)
        {
            while (ex.Message.Contains("inner exception for details"))
                ex = ex.InnerException;
            exceptionMessage = "Your Softphone is not available.";
            return null;
        }
    }

    public string inContactDialNumber(inContactAuthorizationResponse authToken, JoinSessionResponse sessionResponse, string phoneNumber, string skillName)
    {
        try
        {
            //JSON URL
            var url = authToken.resource_server_base_uri + "/services/v2.0/agent-sessions/" + sessionResponse.sessionId.ToString() + "/dial-phone";

            //Setup Post String
            string postData = "{\"phoneNumber\":\"" + phoneNumber + "\",\"skillName\":\"" + skillName + "\"}";

            // Define Request
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

            request.Method = "POST";
            request.ContentType = "application/json; charset=utf-8";
            request.Headers.Add("Authorization", "bearer " + authToken.access_token);
            request.Accept = "application/json";

            //Build Request Stream
            using (Stream s = request.GetRequestStream())
            {
                using (StreamWriter sw = new StreamWriter(s))
                    sw.Write(postData);
            }

            //Get Response Stream
            using (Stream s = request.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    var jsonData = sr.ReadToEnd();
                }
            }
            return null;
        }
        catch (Exception ex)
        {
            while (ex.Message.Contains("inner exception for details"))
                ex = ex.InnerException;
            return "Unable to dial, please contact your system administrator.";
        }
    }

    public void inContactSkillSwap(inContactAuthorizationResponse authToken, string skillId, string agentId, string active)
    {
        //JSON URL
        var url = authToken.resource_server_base_uri + "/services/v2.0/skills/" + skillId.ToString() + "/agents/" + agentId.ToString();

        //Setup Post String
        string postData = "{\"agentProficiency\":\"3\",\"active\":\"" + active.ToString() + "\"}";

        // Define Request
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

        request.Method = "PUT";
        request.ContentType = "application/json; charset=utf-8";
        request.Headers.Add("Authorization", "bearer " + authToken.access_token);
        request.Accept = "application/json";

        //Build Request Stream
        using (Stream s = request.GetRequestStream())
        {
            using (StreamWriter sw = new StreamWriter(s))
                sw.Write(postData);
        }

        try
        {
            //Get Response Stream
            using (Stream s = request.GetResponse().GetResponseStream())
            {
                using (StreamReader sr = new StreamReader(s))
                {
                    var jsonData = sr.ReadToEnd();
                }
            }
        }
        catch (Exception ex)
        {
            var x = 1;
        }
    }
 
}
