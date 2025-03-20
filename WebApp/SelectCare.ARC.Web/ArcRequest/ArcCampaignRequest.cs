using System.Xml.Serialization;

namespace SelectCare.ARC.ArcRequest
{
    [XmlType(AnonymousType = false, TypeName = "ArcCampaignRequest")]
    //[XmlRoot(Namespace = "http://selectcare.com/arc/CampaignRequest", IsNullable = false, ElementName = "Request")]
    [XmlRoot( IsNullable = false, ElementName = "Request")]
    public partial class ArcCampaignRequest
    {

        private ArcRequestLogin loginField;

        private ArcCampaignRequestLead[] leadsField;

        /// <remarks/>
        [XmlElement(ElementName = "Login")]
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
        [XmlArrayItem(typeof(ArcCampaignRequestLead), ElementName = "Lead", IsNullable = false)]
        [XmlArray("Leads")]
        public ArcCampaignRequestLead[] Leads
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
    [XmlType(AnonymousType = false, TypeName = "ArcCampaignRequestLead")]
    public partial class ArcCampaignRequestLead
    {

        private string timestampField;

        private ArcLeadMarketing marketingField;

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
        public ArcLeadMarketing Marketing
        {
            get
            {
                return this.marketingField;
            }
            set
            {
                this.marketingField = value;
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