namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualContact", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualContact")]
    public partial class ArcIndividualContact
    {

        private string titleField;

        private string firstNameField;

        private string lastNameField;

        private string suffixField;

        private string appStateField;

        private System.DateTime birthDateField;

        private string addressField;

        private string cityField;

        private string stateField;

        private string zipField;

        private ArcContactPhone _dayPhoneField;

        private ArcContactPhone eveningPhoneField;

        private ArcContactPhone mobilePhoneField;

        private string emailField;

        private string genderField;

        /// <remarks/>
        public string Title
        {
            get { return this.titleField; }
            set { this.titleField = value; }
        }

        /// <remarks/>
        public string FirstName
        {
            get { return this.firstNameField; }
            set { this.firstNameField = value; }
        }

        /// <remarks/>
        public string LastName
        {
            get { return this.lastNameField; }
            set { this.lastNameField = value; }
        }

        /// <remarks/>
        public string Suffix
        {
            get { return this.suffixField; }
            set { this.suffixField = value; }
        }

        /// <remarks/>
        public string AppState
        {
            get { return this.appStateField; }
            set { this.appStateField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType = "date")]
        public System.DateTime BirthDate
        {
            get { return this.birthDateField; }
            set { this.birthDateField = value; }
        }

        /// <remarks/>
        public string Address
        {
            get { return this.addressField; }
            set { this.addressField = value; }
        }

        /// <remarks/>
        public string City
        {
            get { return this.cityField; }
            set { this.cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return this.stateField; }
            set { this.stateField = value; }
        }

        /// <remarks/>
        public string Zip
        {
            get { return this.zipField; }
            set { this.zipField = value; }
        }

        /// <remarks/>
        public ArcContactPhone DayPhone
        {
            get { return this._dayPhoneField; }
            set { this._dayPhoneField = value; }
        }

        /// <remarks/>
        public ArcContactPhone EveningPhone
        {
            get { return this.eveningPhoneField; }
            set { this.eveningPhoneField = value; }
        } 

        public ArcContactPhone MobilePhone
        {
            get { return this.mobilePhoneField; }
            set { this.mobilePhoneField = value; }
        }

        /// <remarks/>
        public string Email
        {
            get { return this.emailField; }
            set { this.emailField = value; }
        }

        /// <remarks/>
        public string Gender
        {
            get { return this.genderField; }
            set { this.genderField = value; }
        }
    }
}