using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Xml.Serialization;

namespace SelectCare.ARC.Client
{
    public class ArcTestClient
    {
        public void TestAgent()
        {
            //var request=new ArcRequest();
            //request.Login = new RequestLogin() { Password = "$6%mvAdW2e", UserId = "SelectCare" };

            var request = GetRequestFromFile<ArcRequest>("Sample.xml");
            string basePath = "http://vsc01.selectquote.com/LifeApi/";

            var responseMessage = ArcClientHelper.PostChangeAgent(basePath, request).Result;// blocking
            HandleResponse(responseMessage);
            
            Console.WriteLine();
            Console.WriteLine("Calling PostLetterLog...");
            responseMessage = ArcClientHelper.PostLetterLog(basePath, request).Result;// blocking
            HandleResponse(responseMessage);

            Console.WriteLine();
            Console.WriteLine("Calling PostStopLetter...");
            responseMessage = ArcClientHelper.PostStopLetter(basePath, request).Result;// blocking
            HandleResponse(responseMessage);

            Console.WriteLine();
            Console.WriteLine("Calling PostUpdate...");
            responseMessage = ArcClientHelper.PostUpdate(basePath, request).Result;// blocking
            HandleResponse(responseMessage);


            // client.PostAsXmlAsync()

        }

        private static void HandleResponse(HttpResponseMessage responseMessage)
        {
            if (responseMessage.IsSuccessStatusCode)
            {
                var result = responseMessage.Content.ReadAsStringAsync().Result;
                Console.WriteLine("Response {0} =>", result);
            }
            else
            {
                Console.WriteLine("Ops error found {0} ,{1}", responseMessage.StatusCode, responseMessage.ReasonPhrase);
            }
        }

        private T GetRequestFromFile<T>(string filePath)
        {
            FileStream reader = null;
            //TextReader reader = new StringReader(xml);
            try
            {


                //string filePath = Server.MapPath(xml);
                reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var serializer = new XmlSerializer(typeof(T));
                var obj = (T)serializer.Deserialize(reader);
                reader.Close();
                return obj;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                if (reader != null) reader.Close();

            }
            return default(T);
        }
        private static string WriteObjectAsXml(Type type, object obj)
        {
            XmlSerializer serializer = new XmlSerializer(type);
            var builder = new StringBuilder();
            TextWriter writer = new StringWriter(builder);
            serializer.Serialize(writer, obj);
            return builder.ToString();
        }

    }
}