namespace SelectCare.ArcApi
{
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false)]
    public class LeadPolicy
    {

        public decimal? FaceAmount { get; set; }

        public int? Duration
        {
            get;
            set;
        }

        public string CompanyCode
        {
            get;
            set;
        }

        public LeadPolicyCaseSpecialist CaseSpecialist
        {
            get;
            set;
        }

        public string Note
        {
            get;
            set;
        }
    }
    public class LeadPolicyCaseSpecialist
    {

        public string Name
        {
            get;
            set;
        }

        public int? Ext
        {
            get;
            set;
        }
    }
}