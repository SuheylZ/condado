namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualExistingInsurance", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualExistingInsurance")]
    public partial class ArcIndividualExistingInsurance
    {

        private string insuranceField;

        private decimal amountField;

        private string wantsToReplaceField;

        /// <remarks/>
        public string Insurance
        {
            get { return this.insuranceField; }
            set { this.insuranceField = value; }
        }

        /// <remarks/>
        public decimal Amount
        {
            get { return this.amountField; }
            set { this.amountField = value; }
        }

        /// <remarks/>
        public string WantsToReplace
        {
            get { return this.wantsToReplaceField; }
            set { this.wantsToReplaceField = value; }
        }
    }
}