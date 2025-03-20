using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class ContactPhone
    {

        /// <remarks/>
        public string Number { get; set; }

        /// <remarks/>
        public string IsMobile { get; set; }

        /// <remarks/>
        public string Consent { get; set; }
    }
}