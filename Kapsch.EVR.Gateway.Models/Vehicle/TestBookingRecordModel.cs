using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class TestBookingRecordModel
    {
        public int ID { get; set; }
        public string VIN { get; set; }
        public string EngineNumber { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public DateTime TestDate { get; set; }
        public int IsPassed { get; set; }
        public string TestType { get; set; }
        public string BookingReference { get; set; }
        public string VLN { get; set; }
        public string YearOfMake { get; set; }
        public int NetWeight { get; set; }
        public int GVM { get; set; }
        public DateTime? LicenceExpiryDate { get; set; }
        public DateTime? RoadworthyDate { get; set; }
        public DateTime? InsuranceExpiryDate { get; set; }
        public DateTime? CapturedDate { get; set; }


        public string FormattedTestDate
        {
            get { return string.Format("{0:dd/MM/yyyy}", TestDate); }
        }

        public string IsPassedString
        {
            get { return Convert.ToString(IsPassed); }
        }

        public string FormattedLicenceExpiryDate
        {
            get { return string.Format("{0:dd/MM/yyyy}", LicenceExpiryDate); }
        }

        public string FormattedRoadworthyExpiryDate
        {
            get { return string.Format("{0:dd/MM/yyyy}", RoadworthyDate); }
        }

        public string FormattedInsuranceExpiryDate
        {
            get { return string.Format("{0:dd/MM/yyyy}", InsuranceExpiryDate); }
        }

        public string FormattedCapturedDate
        {
            get { return string.Format("{0:dd/MM/yyyy}", CapturedDate); }
        }
    }
}
