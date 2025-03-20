using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    /// <remarks/>
    [XmlType(AnonymousType = false,Namespace = "")]
    [DataContract(Namespace = "",Name = "Login")]
    public class RequestLogin
    {

        /// <remarks/>
        [DataMember]
        public string UserId { get; set; }

        /// <remarks/>
        [DataMember]
        public string Password { get; set; }
    }
}