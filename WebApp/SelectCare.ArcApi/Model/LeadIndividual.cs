using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = true,Namespace = "")]
    public partial class LeadIndividual
    {

        /// <remarks/>
        public IndividualScoringInformation ScoringInformation { get; set; }

        /// <remarks/>
        public IndividualContact Contact { get; set; }

        /// <remarks/>
        public IndividualExistingInsurance ExistingInsurance { get; set; }

        /// <remarks/>
        public IndividualDesiredInsurance DesiredInsurance { get; set; }
    }
}