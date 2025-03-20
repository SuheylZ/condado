namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcLeadPolicy", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcLeadPolicy")]
    public partial class ArcLeadPolicy
    {

        private decimal faceAmountField;

        private int durationField;

        private string companyCodeField;

        private ArcLeadPolicyCaseSpecialist caseSpecialistField;

        private string noteField;

        /// <remarks/>
        public decimal FaceAmount
        {
            get { return this.faceAmountField; }
            set { this.faceAmountField = value; }
        }

        /// <remarks/>
        public int Duration
        {
            get { return this.durationField; }
            set { this.durationField = value; }
        }

        /// <remarks/>
        public string CompanyCode
        {
            get { return this.companyCodeField; }
            set { this.companyCodeField = value; }
        }

        /// <remarks/>
        public ArcLeadPolicyCaseSpecialist CaseSpecialist
        {
            get { return this.caseSpecialistField; }
            set { this.caseSpecialistField = value; }
        }

        /// <remarks/>
        public string Note
        {
            get { return this.noteField; }
            set { this.noteField = value; }
        }
    }
}