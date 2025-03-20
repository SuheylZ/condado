using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class RequestLeadStatus
    {

        public string Code { get; set; }

        /// <remarks/>
        public string LastUpdate { get; set; }

        /// <remarks/>
        public string AgentInitials { get; set; }
    }
}