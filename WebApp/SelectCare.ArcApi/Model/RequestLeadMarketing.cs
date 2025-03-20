using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = true,Namespace = "")]
    public partial class RequestLeadMarketing
    {
    
        /// <remarks/>
        public string SourceCode { get; set; }

        /// <remarks/>
        public string IpAddress { get; set; }

        /// <remarks/>
        public string TrackingCode { get; set; }

        /// <remarks/>
        public string Campaign { get; set; }
    }
}