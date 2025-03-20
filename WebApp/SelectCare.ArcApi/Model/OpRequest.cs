using System.Collections.Generic;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "OpRequest", Namespace = "", IsNullable = false)]
    public partial class OpRequest
    {

        /// <remarks/>
        public RequestLogin Login { get; set; }

        /// <remarks/>
        //[System.Xml.Serialization.XmlArrayItemAttribute("Opportunity", IsNullable = false)]
        public List<Opportunity> Opportunities { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false)]
    public class Opportunity
    {

        public string Timestamp
        {
            get;
            set;
        }

        public string DNIS
        {
            get;
            set;
        }

        public string AgentInitials
        {
            get;
            set;
        }

        public int? TalkTime
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID { get; set; }
    }

}
