namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcLeadMarketing", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcLeadMarketing")]
    public partial class ArcLeadMarketing
    {

        private string sourceCodeField;

        private string ipAddressField;

        private string trackingCodeField;

        private string campaignField;

        /// <remarks/>
        public string SourceCode
        {
            get { return this.sourceCodeField; }
            set { this.sourceCodeField = value; }
        }

        /// <remarks/>
        public string IpAddress
        {
            get { return this.ipAddressField; }
            set { this.ipAddressField = value; }
        }

        /// <remarks/>
        public string TrackingCode
        {
            get { return this.trackingCodeField; }
            set { this.trackingCodeField = value; }
        }

        /// <remarks/>
        public string Campaign
        {
            get { return this.campaignField; }
            set { this.campaignField = value; }
        }
    }
}