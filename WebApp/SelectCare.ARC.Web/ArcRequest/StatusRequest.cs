namespace SelectCare.ARC.ArcRequest
{

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcStatusRequest")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://selectcare.com/arc/UpdateStatus", IsNullable = false, ElementName = "Request")]
    [System.Xml.Serialization.XmlRootAttribute( IsNullable = false, ElementName = "Request")]
    public partial class ArcStatusRequest
    {

        private ArcRequestLogin loginField;

        private ArcStatusRequestLead[] leadsField;

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
        //[System.Xml.Serialization.XmlArrayItemAttribute("Lead", IsNullable = false)]
        [System.Xml.Serialization.XmlArray(ElementName = "Leads",IsNullable = false)]
        [System.Xml.Serialization.XmlArrayItem(typeof(ArcStatusRequestLead),ElementName = "Lead",IsNullable = false)]
        public ArcStatusRequestLead[] Leads
        {
            get
            {
                return this.leadsField;
            }
            set
            {
                this.leadsField = value;
            }
        }
    }

   
    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcStatusRequestLead")]
    public partial class ArcStatusRequestLead
    {

        private string timestampField;

        private ArcLeadStatus statusField;

        private string referenceField;

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
        public ArcLeadStatus Status
        {
            get
            {
                return this.statusField;
            }
            set
            {
                this.statusField = value;
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

    ///// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //public partial class ArcStatusRequestLeadStatus
    //{

    //    private string codeField;

    //    private string lastUpdateField;

    //    private string agentInitialsField;

    //    /// <remarks/>
    //    public string Code
    //    {
    //        get
    //        {
    //            return this.codeField;
    //        }
    //        set
    //        {
    //            this.codeField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string LastUpdate
    //    {
    //        get
    //        {
    //            return this.lastUpdateField;
    //        }
    //        set
    //        {
    //            this.lastUpdateField = value;
    //        }
    //    }

    //    /// <remarks/>
    //    public string AgentInitials
    //    {
    //        get
    //        {
    //            return this.agentInitialsField;
    //        }
    //        set
    //        {
    //            this.agentInitialsField = value;
    //        }
    //    }
    //}


}