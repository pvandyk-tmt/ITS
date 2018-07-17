using System;
using TMT.Build.OracleTableTypeClasses;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleTestBookingModel
    {
        //public BookingSearchTypeModel bookingSearchTypeModel = new BookingSearchTypeModel();
        public int ID { get; set; }
        public int VehicleDetailID { get; set; }
        public int InspectorCredentialID { get; set; }
        public string BookingReference { get; set; }
        public DateTime TestDate { get; set; }
        public Nullable<int> IsPassed { get; set; }
        public int TestTypeID { get; set; }
        public long CapturedCredentialID { get; set; }
        public DateTime CapturedDate { get; set; }
        public long SiteID { get; set; }
    }
}
