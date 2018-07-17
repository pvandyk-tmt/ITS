using System;

namespace Kapsch.ITS.Reports.Models
{
    public class OutstandingInfringerModel
    {
        public string DistrictName { get; set; }
        public DateTime OffenceDate { get; set; }
        public string ReferenceNumber { get; set; }
        public string OffenderName { get; set; }
        public string OffenceCode { get; set; }
        public string OffenceDescription { get; set; }
        public string OffenceLocation { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string FormattedRegisterStatus { get; set; }
        public string OffenderMobile { get; set; }
        public string OffenderEmail { get; set; }
    }
}
