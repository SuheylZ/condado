namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcContactPhone", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcContactPhone")]
    public partial class ArcContactPhone
    {

        private string numberField;

        private string isMobileField;

        private string consentField;

        /// <remarks/>
        public string Number
        {
            get { return this.numberField; }
            set { this.numberField = value; }
        }

        /// <remarks/>
        public string IsMobile
        {
            get { return this.isMobileField; }
            set { this.isMobileField = value; }
        }

        /// <remarks/>
        public string Consent
        {
            get { return this.consentField; }
            set { this.consentField = value; }
        }
    }
}