using System;
using System.Collections.Generic;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace SelectCare.ARC.ArcRequest
{
    /// <remarks/>
    [XmlType(AnonymousType = false,TypeName = "ArcLeadRequest")]
    //[System.Xml.Serialization.XmlRootAttribute(Namespace = "http://selectcare.com/arc/InsertUpdateLead", IsNullable = false, ElementName = "Request")]
    [XmlRoot( IsNullable = false, ElementName = "Request")]
    public partial class ArcLeadRequest
    {

        private ArcRequestLogin loginField;

        private List<ArcLeadRequestLead> leadsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Qualified)]
        public ArcRequestLogin Login
        {
            get { return this.loginField; }
            set { this.loginField = value; }
        }

        /// <remarks/>
        [XmlArrayItem(Type = typeof(ArcLeadRequestLead), ElementName = "Lead", IsNullable = false, NestingLevel = 0)]
        [System.Xml.Serialization.XmlArray(ElementName = "Leads",Form = XmlSchemaForm.Qualified,IsNullable = false)]
        //[System.Xml.Serialization.XmlAnyElement("Lead")]
        public List<ArcLeadRequestLead> Leads
        {
            get { return this.leadsField; }
            set { this.leadsField = value; }
        }
    }

    /// <remarks/>
    [XmlType(AnonymousType = false, TypeName = "ArcLeadRequestLead", IncludeInSchema = true)]
    public partial class ArcLeadRequestLead
    {

        private string timestampField;

        private ArcLeadMarketing marketingField;

        private ArcLeadIndividual individualField;

        private string companionReferenceField;

        private ArcLeadEmailCommunications emailCommunicationsField;

        private ArcLeadStatus statusField;

        private ArcLeadPolicy policyField;

        private string referenceField;

        /// <remarks/>
        public string Timestamp
        {
            get { return this.timestampField; }
            set { this.timestampField = value; }
        }
        //[XmlIgnore]
        //public DateTime? TimestampDate
        //{
        //    get { return Timestamp.ConvertUTCToCST(); }
        //}
        /// <remarks/>
        public ArcLeadMarketing Marketing
        {
            get { return this.marketingField; }
            set { this.marketingField = value; }
        }

        /// <remarks/>
        //[XmlElement(Form = XmlSchemaForm.Qualified,ElementName = "ArcLeadIndividual")]
        public ArcLeadIndividual Individual
        {
            get { return this.individualField; }
            set { this.individualField = value; }
        }

        /// <remarks/>
        public string CompanionReference
        {
            get { return this.companionReferenceField; }
            set { this.companionReferenceField = value; }
        }
        //MH:22 April 201
        public long? CompanionIndvKey { get; set; }

        /// <remarks/>
        public ArcLeadEmailCommunications EmailCommunications
        {
            get
            {
                return this.emailCommunicationsField;
            }
            set
            {
                this.emailCommunicationsField = value;
            }
        }
        /// <remarks/>
        public ArcLeadStatus Status
        {
            get { return this.statusField; }
            set { this.statusField = value; }
        }

        /// <remarks/>
        public ArcLeadPolicy Policy
        {
            get { return this.policyField; }
            set { this.policyField = value; }
        }

        /// <remarks/>
        [XmlAttribute()]
        public string Reference
        {
            get { return this.referenceField; }
            set { this.referenceField = value; }
        }
        //MH:22 April 201
        /// <remarks/>
        public long? Indv_key { get; set; }
    }

    ///// <remarks/>
    //[System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    //public partial class RequestLeadIndividualContactEveningPhone
    //{

    //    private string numberField;

    //    private string isMobileField;

    //    private string consentField;

    //    /// <remarks/>
    //    public string Number
    //    {
    //        get { return this.numberField; }
    //        set { this.numberField = value; }
    //    }

    //    /// <remarks/>
    //    public string IsMobile
    //    {
    //        get { return this.isMobileField; }
    //        set { this.isMobileField = value; }
    //    }

    //    /// <remarks/>
    //    public string Consent
    //    {
    //        get { return this.consentField; }
    //        set { this.consentField = value; }
    //    }
    //}
}