using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false)]
    [XmlRoot(Namespace = "",ElementName = "Request", IsNullable = false)]
    [DataContract(Namespace = "",Name = "Request")]
    public class ArcRequest
    {
        public ArcRequest()
        {
            Leads=new List<ArcLead>();
        }
        /// <remarks/>
        [DataMember]
        public RequestLogin Login { get; set; }

        /// <remarks/>
        [XmlArrayItem(ElementName = "Lead", Type = typeof(ArcLead))]
        [XmlArray(ElementName = "Leads")]
        [DataMember]
        //public ArcLead[] Leads { get; set; }
        public List<ArcLead> Leads { get; set; }
        
    }
}