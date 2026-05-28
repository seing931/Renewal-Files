using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RenewalTransFiles.Models
{
    public class MotorPolicy
    {
        public string CoverNoteNo { get; set; }
        public string QuotationNo { get; set; }
        public string PolicyStatus { get; set; }
        public string JPJStatus { get; set; }
        public string AgentCode { get; set; }
        public DateTime IssueDate { get; set; }
        public string DataBatchNo { get; set; }
    }
}
