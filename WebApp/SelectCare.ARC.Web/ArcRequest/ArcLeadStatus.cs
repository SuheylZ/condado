namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcLeadStatus", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcLeadStatus")]
    public partial class ArcLeadStatus
    {

        private string codeField;

        private string lastUpdateField;

        private string agentInitialsField;

        /// <remarks/>
        public string Code
        {
            get { return this.codeField; }
            set { this.codeField = value; }
        }

        /// <remarks/>
        public string LastUpdate
        {
            get { return this.lastUpdateField; }
            set { this.lastUpdateField = value; }
        }

        /// <remarks/>
        public string AgentInitials
        {
            get { return this.agentInitialsField; }
            set { this.agentInitialsField = value; }
        }

        public string SubStatus { get; set; }

        //Added at 24 June 2014
        public string ExamVendor { get; set; }

        public string AppointmentType { get; set; }

        public string AppointmentDateTime { get; set; }
       
    }
}