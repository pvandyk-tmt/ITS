using System;

namespace Kapsch.ITS.Reports.Financial.Models
{
    public class InfringementStatusModel
    {
        public DateTime InfringementDate { get; set; }
        public string InfringementNumber { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal CurrentAmount { get; set; }
        public string FormattedRegisterStatus { get; set; }
        public string FormattedInfringementType { get; set; }
        public string CourtName { get; set; }
        public string DistrictName { get; set; }
    }
}
