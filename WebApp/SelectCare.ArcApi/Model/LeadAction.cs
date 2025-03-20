using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace SelectCare.ArcApi
{
    [XmlType(AnonymousType = false, TypeName = "Action", Namespace = "")]
    //[DataContract(Name = "Action", Namespace = "")]
    public class LeadAction
    {
        //[XmlType(AnonymousType = false)]
        //public class ArcAction
        //{
            public string Timestamp { get; set; }

            public string PerformedBy { get; set; }

            public long? Code { get; set; }

            public string Description { get; set; }

            public string Notes { get; set; }

            [XmlAttribute()]
            public long ID{ get; set; }
        //}

    }
}
