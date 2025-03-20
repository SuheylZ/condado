namespace SelectCare.ARC
{
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcAgentResponse")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://selectcare.com/arc/AgentResponse", IsNullable = false, ElementName = "Response")]
    [System.Xml.Serialization.XmlRootAttribute(IsNullable = false, ElementName = "Response")]
    public partial class ArcAgentResponse
    {

        private ArcAgentResponseResult[] _arcAgentResultsField;

        /// <remarks/>
        [System.Xml.Serialization.XmlArray("AgentResults")]
        [System.Xml.Serialization.XmlArrayItem(typeof(ArcAgentResponseResult), ElementName = "AgentResult")]
        public ArcAgentResponseResult[] ArcAgentResults
        {
            get
            {
                return this._arcAgentResultsField;
            }
            set
            {
                this._arcAgentResultsField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcAgentResponseResult")]
    public partial class ArcAgentResponseResult
    {

        private string acceptedField;

        private string reasonField;

        private string timestampField;

        private string agentInitialsField;

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
        public string AgentInitials
        {
            get
            {
                return this.agentInitialsField;
            }
            set
            {
                this.agentInitialsField = value;
            }
        }
    }
}