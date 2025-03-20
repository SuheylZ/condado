namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true, TypeName = "ArcRequestLogin", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcRequestLogin")]
    public partial class ArcRequestLogin
    {

        private string userIdField;

        private string passwordField;

        /// <remarks/>
        [System.Xml.Serialization.XmlElement(ElementName = "UserId")]
        public string UserId
        {
            get { return this.userIdField; }
            set { this.userIdField = value; }
        }

        /// <remarks/>
        public string Password
        {
            get { return this.passwordField; }
            set { this.passwordField = value; }
        }
    }
}