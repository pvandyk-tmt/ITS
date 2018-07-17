using System;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleTestBooking
    {
        public VehicleTestBooking()
        {
        }

        public int ID { get; set; }
        public string VIN { get; set; }
        public string EngineNumber { get; set; }
        public int VehicleCategoryID { get; set; }
        public int VehicleTypeID { get; set; }
        public int VehicleMakeID { get; set; }
        public int VehicleModelID { get; set; }
        public int VehicleModelNumberID { get; set; }
        public int YearOfMake { get; set; }
        public int ColourID { get; set; }
        public string VLN { get; set; }
        public int NetWeight { get; set; }
        public int GVM { get; set; }
        public int PropelledByID { get; set; }
        public int FuelTypeID { get; set; }
        public int RegistrationStatusID { get; set; }
        public string LicenceExpiryDate { get; set; }
        public string RoadworthyExpiryDate { get; set; }
        public string InsuranceExpiryDate { get; set; }
        public long SeatingCapacity { get; set; }
    }  
}
