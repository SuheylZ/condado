namespace SelectCare.ARC
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcResponse")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://selectcare.com/arc/LeadResponse", IsNullable = false, ElementName = "Response")]
    [System.Xml.Serialization.XmlRootAttribute( IsNullable = false, ElementName = "Response")]
    public partial class ArcResponse
    {

        private ArcResponseResult[] _responseResultsField;

        /// <remarks/>
        //[System.Xml.Serialization.XmlArrayItemAttribute("LeadResult", IsNullable = false)]
        [System.Xml.Serialization.XmlArray("LeadResults")]
        [System.Xml.Serialization.XmlArrayItem(typeof(ArcResponseResult), ElementName = "LeadResult")]
        public ArcResponseResult[] ResponseResults
        {
            get
            {
                return this._responseResultsField;
            }
            set
            {
                this._responseResultsField = value;
            }
        }
    }

    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcResponseResult", Namespace = "http://selectcare.com/arc/LeadResponse")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcResponseResult")]
    public partial class ArcResponseResult
    {

        private string acceptedField;

        private string reasonField;

        private long? accountIDField;

        private string timestampField;

        private string referenceField;

        /// <remarks/>
        public string Accepted
        {
            get
            {
                return this.acceptedField;
            }
            set
            {
                this.acceptedField = value;
            }
        }

        /// <remarks/>
        public string Reason
        {
            get
            {
                return this.reasonField;
            }
            set
            {
                this.reasonField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public long? AccountID
        {
            get
            {
                return this.accountIDField;
            }
            set
            {
                this.accountIDField = value;
            }
        }

        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public long? Indv_key { get; set; }
        /// <remarks/>
        public string Timestamp
        {
            get
            {
                return this.timestampField;
            }
            set
            {
                this.timestampField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Reference
        {
            get
            {
                return this.referenceField;
            }
            set
            {
                this.referenceField = value;
            }
        }
    }


}