using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false)]
    [System.Xml.Serialization.XmlRootAttribute(ElementName = "Response", Namespace = "", IsNullable = false)]
    public class OpResponse
    {
        [System.Xml.Serialization.XmlArrayItemAttribute("OpResult", IsNullable = false)]
        public OpResult[] OpResults { get; set; }
    }

    [XmlType(AnonymousType = false)]
    public class OpResult
    {
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string ID { get; set; }


        public string Accepted { get; set; }

        public string Reason { get; set; }
    }
}