using System.Xml.Schema;
using System.Xml.Serialization;

namespace SelectCare.ARC.ArcRequest
{

    /// <remarks/>
    [XmlType(AnonymousType = false, TypeName = "ArcConsentUpdateRequest")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://selectcare.com/arc/ConsentUpdate", IsNullable = false, ElementName = "Request")]
    [XmlRoot( IsNullable = false, ElementName = "Request")]
    public partial class ArcConsentUpdateRequest
    {

        private ArcRequestLogin loginField;

        private ArcConsentUpdateRequestLead[] leadsField;

        /// <remarks/>
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

        /// <remarks/>
        //[System.Xml.Serialization.XmlArrayItemAttribute("Lead", IsNullable = false)]
        [System.Xml.Serialization.XmlArray(ElementName = "Leads")]
        [System.Xml.Serialization.XmlArrayItem(typeof(ArcConsentUpdateRequestLead), ElementName = "Lead")]
        public ArcConsentUpdateRequestLead[] Leads
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

    /// <remarks/>
    [XmlType(AnonymousType = false, TypeName = "ArcConsentUpdateRequestLead")]
    public partial class ArcConsentUpdateRequestLead
    {

        private string timestampField;

        private ArcLeadIndividual individualField;

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
        //[XmlElement(Form = XmlSchemaForm.Qualified, ElementName = "Individual")]
        public ArcLeadIndividual Individual
        {
            get
            {
                return this.individualField;
            }
            set
            {
                this.individualField = value;
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