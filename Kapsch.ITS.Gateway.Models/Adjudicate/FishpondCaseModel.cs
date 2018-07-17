using System;

namespace Kapsch.ITS.Gateway.Models.Adjudicate
{
    public class FishpondCaseModel
    {
        public string TicketNo { get; set; }
        public string VehicleRegistration { get; set; }
        public DateTime TicketDate { get; set; }
        public string VehicleMake { get; set; }
        public string VehicleModel { get; set; }
        public string RejectReason { get; set; }
        public string RejectBy { get; set; }
        public DateTime VerifyDate { get; set; }
        public int TimesRejected { get; set; }
        public string LockedBy { get; set; }
    }
}
