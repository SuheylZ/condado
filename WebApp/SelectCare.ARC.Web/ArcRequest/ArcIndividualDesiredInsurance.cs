namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualDesiredInsurance", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualDesiredInsurance")]
    public partial class ArcIndividualDesiredInsurance
    {

        private decimal amountField;

        private decimal alternateAmountField;

        private string termDurationField;

        /// <remarks/>
        public decimal Amount
        {
            get { return this.amountField; }
            set { this.amountField = value; }
        }

        /// <remarks/>
        public decimal AlternateAmount
        {
            get { return this.alternateAmountField; }
            set { this.alternateAmountField = value; }
        }

        /// <remarks/>
        public string TermDuration
        {
            get { return this.termDurationField; }
            set { this.termDurationField = value; }
        }
    }
}