using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class IndividualDesiredInsurance
    {
        /// <remarks/>
        public decimal? Amount { get; set; }

        /// <remarks/>
        public decimal? AlternateAmount { get; set; }

        /// <remarks/>
        public string TermDuration { get; set; }
    }
}