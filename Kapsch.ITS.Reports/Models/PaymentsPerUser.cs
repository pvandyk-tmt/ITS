using System;

namespace Kapsch.ITS.Reports.Example.Models
{
    public class PaymentsPerUser
    {
        public DateTime PaymentDate { get; set; }
        public DateTime CapturedDate { get; set; }
        public string District { get; set; }
        public string Court { get; set; }
        public string User { get; set; }
        public string PaymentType { get; set; }
        public string OffenceNumber { get; set; }
        public decimal OffenceAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public string Reference { get; set; }
    }
}
