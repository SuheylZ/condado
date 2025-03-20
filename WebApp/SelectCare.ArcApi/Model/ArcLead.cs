using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using SelectCare.ArcApi;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false, TypeName = "Lead", Namespace = "")]
    [DataContract(Name = "Lead", Namespace = "")]
    public class ArcLead
    {

        /// <remarks/>
        [DataMember]
        public string Timestamp { get; set; }

        /// <remarks/>
        [DataMember]
        public RequestLeadMarketing Marketing { get; set; }

        /// <remarks/>
        [DataMember]
        public LeadIndividual Individual { get; set; }

        /// <remarks/>
        [DataMember]
        public string CompanionReference { get; set; }

        /// <remarks/>
        [DataMember]
        public RequestLeadStatus Status { get; set; }

        /// <remarks/>
        [DataMember]
        public EmailCommunications EmailCommunications { get; set; }

        /// <remarks/>
        [XmlAttribute()]
        [DataMember]
        public string Reference { get; set; }

        [DataMember]
        public List<LeadAction> Actions { get; set; }
    }
}