namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualScoringInformation", Namespace = "http://selectcare.com/arc/")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "ArcIndividualScoringInformation")]
    public partial class ArcIndividualScoringInformation
    {

        private string substatusField;

        private int tierField;

        private int eScoreField;

        private int fraudScoreField;

        /// <remarks/>
        public string Substatus
        {
            get { return this.substatusField; }
            set { this.substatusField = value; }
        }

        /// <remarks/>
        public int Tier
        {
            get { return this.tierField; }
            set { this.tierField = value; }
        }

        /// <remarks/>
        public int eScore
        {
            get { return this.eScoreField; }
            set { this.eScoreField = value; }
        }

        /// <remarks/>
        public int FraudScore
        {
            get { return this.fraudScoreField; }
            set { this.fraudScoreField = value; }
        }
    }
}