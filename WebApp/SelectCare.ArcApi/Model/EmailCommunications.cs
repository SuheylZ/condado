using System.Collections.Generic;
using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class EmailCommunications
    {
        /// <remarks/>
        public CommunicationsPreferences Preferences { get; set; }

        public List<ArcMessage> Messages { get; set; }
    }
}