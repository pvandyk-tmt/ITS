using System;

namespace Kapsch.ITS.Gateway.Models.Verify
{
    public class FishpondInfoModel
    {
        public string TicketNumber { get; set; }
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
