using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SalesTool.DataAccess
{
    public class leadView
    {
        [KeyAttribute()]
        public Int64 leadId { get; set; }
        public Int64 accountId { get; set; }
        public Int64 individualId { get; set; }
        public string dateCreated { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string dateOfBirth { get; set; }
        public long? dayPhone { get; set; }
        public long? eveningPhone { get; set; }
        public long? cellPhone { get; set; }
        public string leadStatus { get; set; }
        public string userAssigned { get; set; }
        public string CSR { get; set; }
        public string OutpulseId { get; set; }
    }
}
