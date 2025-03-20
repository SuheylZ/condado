namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcLeadIndividual", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcLeadIndividual")]
    public partial class ArcLeadIndividual
    {

        private ArcIndividualScoringInformation scoringInformationField;

        private ArcIndividualContact contactField;

        private ArcIndividualExistingInsurance existingInsuranceField;

        private ArcIndividualDesiredInsurance desiredInsuranceField;

        /// <remarks/>
        public ArcIndividualScoringInformation ScoringInformation
        {
            get { return this.scoringInformationField; }
            set { this.scoringInformationField = value; }
        }

        /// <remarks/>
        public ArcIndividualContact Contact
        {
            get { return this.contactField; }
            set { this.contactField = value; }
        }

        /// <remarks/>
        public ArcIndividualExistingInsurance ExistingInsurance
        {
            get { return this.existingInsuranceField; }
            set { this.existingInsuranceField = value; }
        }

        /// <remarks/>
        public ArcIndividualDesiredInsurance DesiredInsurance
        {
            get { return this.desiredInsuranceField; }
            set { this.desiredInsuranceField = value; }
        }
    }
}