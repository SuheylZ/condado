using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class CommunicationsPreferences
    {

        /// <remarks/>
        public string OptOut { get; set; }

        /// <remarks/>
        public string Reason { get; set; }

        /// <remarks/>
        public string LastUpdate { get; set; }
    }
}