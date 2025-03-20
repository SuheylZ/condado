using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Text;


    public enum PostType
    {
        Unknown = 0,
        Post = 1,
        Get = 2,
        XML = 3
    };

    public class PostHelper
    {
        /// <summary>
        /// Posts the data to specific url and returns the status aqnd server response
        /// </summary>
        /// <param name="url">The web address where data should be posted</param>
        /// <param name="type">type of request to create, affects the data size of data to be sent</param>
        /// <param name="Header">Get/Post Header</param>
        /// <param name="postData">Get/Post data</param>
        /// <param name="statusDescription">Returned status description</param>
        /// <param name="responseData">data recieved from the server</param>
        /// <returns>status code returned from the server</returns>
        public static HttpStatusCode Post(string url, PostType type, string Header, string postData, ref string statusDescription, ref string responseData)
        {
            try
            {
                HttpStatusCode Ans = HttpStatusCode.Ambiguous;

                //Step 1: prepare request object
                WebRequest request = WebRequest.Create(url);
                request.Method = type == PostType.Get ? "GET" : (type == PostType.Post || type == PostType.XML) ? "POST" : "GET";

                if (postData != null)
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(postData);
                    if (type == PostType.XML)
                        request.ContentType = "text/xml";
                    else
                        request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = byteArray.Length;
                    //request.Headers.Add(Header);
                    using (Stream mystream = request.GetRequestStream())
                    {
                        mystream.Write(byteArray, 0, byteArray.Length);
                        mystream.Close();
                    }
                }

                // Step 2: Post and await the response
                WebResponse response = request.GetResponse();
                statusDescription = ((HttpWebResponse)response).StatusDescription;
                Ans = ((HttpWebResponse)response).StatusCode;
                using (Stream mystream = response.GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(mystream))
                    {
                        responseData = reader.ReadToEnd();
                        reader.Close();
                    }
                    mystream.Close();
                }
                response.Close();
                return Ans;
            }
            catch (Exception ex)
            {
                while (ex.Message.Contains("inner exception for details"))
                    ex = ex.InnerException;
                statusDescription = ex.Message;
                return HttpStatusCode.BadRequest;
            }
        }


    }
