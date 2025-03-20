namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcLeadEmailCommunications", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcLeadEmailCommunications")]
    public partial class ArcLeadEmailCommunications
    {

        private ArcEmailCommunicationsPreferences preferencesField;

        /// <remarks/>
        public ArcEmailCommunicationsPreferences Preferences
        {
            get
            {
                return this.preferencesField;
            }
            set
            {
                this.preferencesField = value;
            }
        }
    }

    /// <remarks/>
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcEmailCommunicationsPreferences")]
    public partial class ArcEmailCommunicationsPreferences
    {

        private string optOutField;

        private string reasonField;

        private string lastUpdateField;

        /// <remarks/>
        public string OptOut
        {
            get
            {
                return this.optOutField;
            }
            set
            {
                this.optOutField = value;
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
        public string LastUpdate
        {
            get
            {
                return this.lastUpdateField;
            }
            set
            {
                this.lastUpdateField = value;
            }
        }
    }
}