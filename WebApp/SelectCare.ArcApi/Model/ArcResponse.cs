namespace SelectCare.ArcApi
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "Response", Namespace = "", IsNullable = false)]
    public class ArcResponse
    {

        [System.Xml.Serialization.XmlArrayItemAttribute("LeadResult", IsNullable = false)]
        public LeadResult[] LeadResults { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false)]
    public class LeadResult
    {

        /// <remarks/>
        public string Accepted { get; set; }

        /// <remarks/>
        public string Reason { get; set; }

        /// <remarks/>
        public string AccountID { get; set; }

        /// <remarks/>
        public string Timestamp { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("SubResult", IsNullable = false)]
        public SubResult[] SubResults { get; set; }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Reference { get; set; }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false)]
    public class SubResult
    {


        /// <remarks/>
        public string Accepted
        {
            get;
            set;
        }

        /// <remarks/>
        public string Reason
        {
            get;
            set;
        }

        /// <remarks/>
        public string Timestamp
        {
            get;
            set;
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID
        {
            get;
            set;
        }
    }
}
