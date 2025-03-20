using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class IndividualExistingInsurance
    {

        /// <remarks/>
        public string Insurance { get; set; }

        /// <remarks/>
        public decimal Amount { get; set; }

        /// <remarks/>
        public string WantsToReplace { get; set; }
    }
}