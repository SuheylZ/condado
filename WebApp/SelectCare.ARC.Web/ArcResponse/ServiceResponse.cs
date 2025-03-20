using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace SelectCare.ARC
{
    [System.Xml.Serialization.XmlRootAttribute(IsNullable = false, ElementName = "Response")]
    public class ServiceResponse
    {
        public List<ServiceStatus> ServiceStatus { get; set; }
    }
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = false, TypeName = "Service")]
    public class ServiceStatus
    {
        public string ServiceType { get; set; }
        public string ServiceName { get; set; }
        public string Status { get; set; }
    }
   public enum EServiceStatus
   {
       Running,
       [Description("Not Running")]
       NotRunning
   }
}