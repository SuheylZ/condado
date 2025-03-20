namespace SelectCare.ARC.ArcRequest
{
    /// <summary>
    /// Class for Request Method 4.5
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcAcdCapUpdateRequest")]
    [System.Xml.Serialization.XmlRootAttribute(IsNullable = false, ElementName = "Request")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://selectcare.com/arc/AcdCapUpdate", IsNullable = false, ElementName = "Request")]
    public partial class ArcAcdCapUpdateRequest
    {

        private ArcRequestLogin loginField;

        private ArcRequestAgent[] agentsField;

        /// <remarks/>
        public ArcRequestLogin Login
        {
            get
            {
                return this.loginField;
            }
            set
            {
                this.loginField = value;
            }
        }

        /// <remarks/>
        //[System.Xml.Serialization.XmlArrayItemAttribute("Agent", IsNullable = false)]
        [System.Xml.Serialization.XmlArray("Agents")]
        [System.Xml.Serialization.XmlArrayItem(typeof(ArcRequestAgent), ElementName = "Agent")]
        public ArcRequestAgent[] Agents
        {
            get
            {
                return this.agentsField;
            }
            set
            {
                this.agentsField = value;
            }
        }
    }

    /// <summary>
    /// Agent Item within Request method 4.5
    /// Causes to update User record.
    /// AgentInitial is used as lookup to update user's acdCap
    /// </summary>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcRequestAgent")]
    public partial class ArcRequestAgent
    {

        private string agentInitialsField;

        private int? acdCapField;

        /// <remarks/>
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

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public int? AcdCap
        {
            get
            {
                return this.acdCapField;
            }
            set
            {
                this.acdCapField = value;
            }
        }
    }

}