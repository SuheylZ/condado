namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcLeadPolicyCaseSpecialist", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcLeadPolicyCaseSpecialist")]
    public partial class ArcLeadPolicyCaseSpecialist
    {

        private string nameField;

        private int extField;

        /// <remarks/>
        public string Name
        {
            get { return this.nameField; }
            set { this.nameField = value; }
        }

        /// <remarks/>
        public int Ext
        {
            get { return this.extField; }
            set { this.extField = value; }
        }
    }
}