using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using SalesTool.DataAccess;
using SelectCare.ARC.ArcRequest;
using System.Reflection;

namespace SelectCare.ARC.Web
{
    public partial class ArcTestPage : System.Web.UI.Page
    {
        
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void InserUpdateLead(object sender, EventArgs e)
        {

            if (!IsTermLife)
            {
                txtResponse.InnerText = "Function is only available at life insurance";
                return;
            }
            try
            {
                string text = txtRequestObject.InnerText;
                var obj = GetRequestFromFile<ArcLeadRequest>(text);
                ArcService svc = new ArcService();
                var result = svc.InsertUpdateLead(obj);
                txtResponse.InnerText = WriteObjectAsXml(typeof(ArcResponse), result);
            }
            catch (Exception exception)
            {
                txtResponse.InnerText = exception.ToString();
                //txtResponse.InnerText = "Invalid Request or method";
            }

        }
        private T GetRequestFromFile<T>(string xml)
        {
            //FileStream reader = null;
            TextReader reader = new StringReader(xml);
            try
            {


                //string filePath = Server.MapPath(xml);
                //reader = new FileStream(filePath, FileMode.Open, FileAccess.Read);
                var serializer = new XmlSerializer(typeof(T));
                var obj = (T)serializer.Deserialize(reader);
                reader.Close();
                return obj;
            }
            catch (Exception e)
            {
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

        protected void UpdateCampaign(object sender, EventArgs e)
        {
            if (!IsTermLife)
            {
                txtResponse.InnerText = "Function is only available at life insurance";
                return;
            }
            try
            {
                string text = txtRequestObject.InnerText;
                var obj = GetRequestFromFile<ArcCampaignRequest>(text);
                ArcService svc = new ArcService();
                var result = svc.UpdateCampaign(obj);
                txtResponse.InnerText = WriteObjectAsXml(typeof(ArcResponse), result);
            }
            catch (Exception exception)
            {
                txtResponse.InnerText = exception.ToString();
            }
        }

        protected void UpdateStatus(object sender, EventArgs e)
        {
            if (!IsTermLife)
            {
                txtResponse.InnerText = "Function is only available at life insurance";
                return;
            }
            try
            {
                string text = txtRequestObject.InnerText;
                var obj = GetRequestFromFile<ArcStatusRequest>(text);
                ArcService svc = new ArcService();
                var result = svc.UpdateStatus(obj);
                txtResponse.InnerText = WriteObjectAsXml(typeof(ArcResponse), result);
            }
            catch (Exception exception)
            {
                txtResponse.InnerText = exception.ToString();
            }
        }

        protected void UpdateCommunication(object sender, EventArgs e)
        {
            if (!IsTermLife)
            {
                txtResponse.InnerText = "Function is only available at life insurance";
                return;
            }
            try
            {
                string text = txtRequestObject.InnerText;
                var obj = GetRequestFromFile<ArcStopCommunicationRequest>(text);
                ArcService svc = new ArcService();
                var result = svc.StopCommunication(obj);
                txtResponse.InnerText = WriteObjectAsXml(typeof(ArcResponse), result);
            }
            catch (Exception exception)
            {
                txtResponse.InnerText = exception.ToString();
            }
        }

        protected void UpdateAcd(object sender, EventArgs e)
        {
            if (!IsTermLife)
            {
                txtResponse.InnerText = "Function is only available at life insurance";
                return;
            }
            try
            {
                string text = txtRequestObject.InnerText;
                var obj = GetRequestFromFile<ArcAcdCapUpdateRequest>(text);
                ArcService svc = new ArcService();
                var result = svc.AcdCapUpdate(obj);
                txtResponse.InnerText = WriteObjectAsXml(typeof(ArcAgentResponse), result);
            }
            catch (Exception exception)
            {
                txtResponse.InnerText = exception.ToString();
            }
        }

        protected void UpdateConsent(object sender, EventArgs e)
        {
            if (!IsTermLife)
            {
                txtResponse.InnerText = "Function is only available at life insurance";
                return;
            }
            try
            {
                string text = txtRequestObject.InnerText;
                ArcConsentUpdateRequest request = GetRequestFromFile<ArcConsentUpdateRequest>(text);
                ArcSystemManager manager = new ArcSystemManager(ADOConnectionString);
                var result = manager.ConsentUpdate(request);
                txtResponse.InnerText = WriteObjectAsXml(typeof(ArcResponse), result);
            }
            catch (Exception exception)
            {
                txtResponse.InnerText = exception.ToString();
            }
        }

        protected void Btn_ClearClick(object sender, EventArgs e)
        {
            txtRequestObject.InnerText = "";
            txtResponse.InnerText = "";
        }
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
                const string K_Application_Type = "APPLICATION_TYPE";
                return SalesTool.Common.CGlobalStorage.Instance.Get<int>(K_Application_Type);
            }
        }

        public string Version
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(64);
                sb.AppendFormat("{0}.{1}",
                    DBEngine.ApplicationSettingsInstance.MajorVersion,
                    DBEngine.ApplicationSettingsInstance.MinorVersion
                    );
                foreach (System.Reflection.Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                    if (a.FullName.Contains("SalesTool.DataAccess"))
                    {
                        AssemblyName webName = a.GetName();
                        sb.AppendFormat(".{0}.{1}", webName.Version.Build.ToString(), webName.Version.Revision.ToString());
                        break;
                    }
                return sb.ToString();
            }
        }
    }
}