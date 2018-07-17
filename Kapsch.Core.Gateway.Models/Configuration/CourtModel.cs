using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Configuration
{
    public class CourtModel
    {
        
        public long ID { get; set; }
        public string CourtName { get; set; }
        public AddressInfoModel Address { get; set; }
        public long? PersonInfoID { get; set; }
        public long? ContemptAmount { get; set; }
        public long? ContemptDays { get; set; }
        public long? BankingInfoID { get; set; }
        public long DistrictID { get; set; }
        public string CasePre { get; set; }
        public string CasePost { get; set; }
        public string SequenceName { get; set; }
        public Status Status { get; set; }
        public string WarrantPre { get; set; }
        public string WarrantPost { get; set; }
        public DateTime CaptureDate { get; set; }
        public long? TypeOfServiceAllowed { get; set; }
        public long? WarrantLetterGrace { get; set; }
        public long? WarrantExpireDays { get; set; }
        public long? SummonsExpireDays { get; set; }
        public long? UserId { get; set; }
        public string CourtTime { get; set; }
        public long? DaysToCourtDate { get; set; }
        public long? OverAllocation { get; set; }
        public long ReIssueInvalidServing { get; set; }
    }
}
