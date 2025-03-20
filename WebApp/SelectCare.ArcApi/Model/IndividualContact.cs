using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    public partial class IndividualContact
    {

        /// <remarks/>
        public string Title { get; set; }

        /// <remarks/>
        public string FirstName { get; set; }

        /// <remarks/>
        public string LastName { get; set; }

        /// <remarks/>
        public string Suffix { get; set; }

        /// <remarks/>
        public string AppState { get; set; }

        /// <remarks/>
        public string BirthDate { get; set; }

        /// <remarks/>
        public string Address { get; set; }

        /// <remarks/>
        public string City { get; set; }

        /// <remarks/>
        public string State { get; set; }

        /// <remarks/>
        public string Zip { get; set; }

        /// <remarks/>
        public ContactPhone DayPhone { get; set; }

        /// <remarks/>
        public ContactPhone EveningPhone { get; set; }
        
        public ContactPhone MobilePhone { get; set; }

        /// <remarks/>
        public string Email { get; set; }

        /// <remarks/>
        public string Gender { get; set; }
    }
}