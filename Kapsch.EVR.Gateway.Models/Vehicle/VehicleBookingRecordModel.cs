using System;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleBookingRecordModel
    {
        public Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestBooking VehicleDetails = new Kapsch.EVR.Gateway.Models.Vehicle.VehicleTestBooking();
        public int TestCategoryID { get; set; }
        public int CredentialID { get; set; }
        public int CapturedByCredentialId { get; set; }
        public DateTime CapturedDate { get; set; }
        public string BookingReference { get; set; }
        public int SiteId { get; set; }
        public int DistrictId { get; set; }
        public decimal IsSuccessfull { get; set; }

        public string Message { get; set; }

        public string FormattedCapturedDate
        {
            get { return string.Format("{0:yyyy-MM-dd}", CapturedDate); }
        }
    }
}
