using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class IndividualScoringInformation
    {

        /// <remarks/>
        public string Substatus { get; set; }

        /// <remarks/>
        public int? Tier { get; set; }

        /// <remarks/>
        public int? eScore { get; set; }

        /// <remarks/>
        public int? FraudScore { get; set; }
    }
}