using System.Collections.Generic;

namespace SelectCare.ArcApi
{
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcAcdCapUpdateRequest")]
    [System.Xml.Serialization.XmlRootAttribute(IsNullable = false, ElementName = "Request")]
    public class ArcAcdCapUpdateRequest
    {
        public RequestLogin Login { get; set; }

        //[System.Xml.Serialization.XmlArray("Agents")]
        //[System.Xml.Serialization.XmlArrayItem(typeof(ArcRequestAgent), ElementName = "Agent")]
        public List<ArcRequestAgent> Agents { get; set; }
    }

    /// <summary>
    /// Agent Item within Request method 4.5
    /// Causes to update User record.
    /// AgentInitial is used as lookup to update user's acdCap
    /// </summary>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcRequestAgent")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false)]
    public class ArcRequestAgent
    {

        /// <remarks/>
        public string AgentInitials { get; set; }

        [System.Xml.Serialization.XmlElement(IsNullable = true)]
        public int? AcdCap { get; set; }
    }
}