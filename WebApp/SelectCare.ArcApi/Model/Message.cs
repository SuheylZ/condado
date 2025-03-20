namespace SelectCare.ArcApi
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false,TypeName = "Message")]
    public class ArcMessage
    {
        //public int Code { get; set; }
        //we need to send template title for code and title for name
        // previously it was int mapped to template id
        public string Code { get; set; }

        public string Date { get; set; }

        public string Name { get; set; }

        public string Status { get; set; }

        [System.Xml.Serialization.XmlAttributeAttribute()]
        public long ID { get; set; }
    }
}