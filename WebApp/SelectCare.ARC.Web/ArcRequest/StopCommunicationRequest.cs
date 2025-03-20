using System.Xml.Schema;
using System.Xml.Serialization;

namespace SelectCare.ARC.ArcRequest
{
    /// <summary>
    /// Request method 4.4
    /// </summary>
    [XmlType(AnonymousType = false, TypeName = "ArcStopCommunicationRequest")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://selectcare.com/arc/StopCommunication", IsNullable = false, ElementName = "Request")]
    [XmlRoot( IsNullable = false, ElementName = "Request")]
    public partial class ArcStopCommunicationRequest
    {

        private ArcRequestLogin loginField;

        private ArcStopCommunicationRequestLead[] leadsField;

        /// <summary>
        /// Request Credentials
        /// </summary>
        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public ArcRequestLogin Login
        {
            get
            {
                return this.loginField;
            }
            set
            {
                this.loginField = value;
            }
        }


        /// <summary>
        /// StopCommunication methods leads collections
        /// </summary>
        [System.Xml.Serialization.XmlArray("Leads")]
        [System.Xml.Serialization.XmlArrayItem(typeof(ArcStopCommunicationRequestLead), ElementName = "Lead")]
        public ArcStopCommunicationRequestLead[] Leads
        {
            get
            {
                return this.leadsField;
            }
            set
            {
                this.leadsField = value;
            }
        }
    }

    /// <summary>
    /// StopCommunications Request method's lead item
    /// </summary>
    [XmlType(AnonymousType = false, TypeName = "ArcStopCommunicationRequestLead")]
    public partial class ArcStopCommunicationRequestLead
    {

        private string timestampField;

        private ArcLeadEmailCommunications emailCommunicationsField;

        private string referenceField;

        /// <remarks/>
        public string Timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }

        /// <remarks/>
        public ArcLeadEmailCommunications EmailCommunications
        {
            get
            {
                return this.emailCommunicationsField;
            }
            set
            {
                this.emailCommunicationsField = value;
            }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }
    }

  


}