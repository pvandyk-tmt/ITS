using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleModel
    {
        public int ID { get; set; }

        [Required]
        [Display(Name = "Chassis No. / VIN")]
        [StringLength(128)]        
        public string VIN  { get; set; }

        public int VehicleTestBookingID { get; set; }

        [Required]
        [Display(Name = "Engine Number")]
        [StringLength(128)]
        public string EngineNumber { get; set; }

        [Required]
        [Display(Name = "Vehicle Category")]
        public int? VehicleCategoryId { get; set; }

        [Required]
        [Display(Name = "Vehicle Type")]
        public int? VehicleTypeId { get; set; }

        [Required]
        [Display(Name = "Vehicle Make")]
        public int? VehicleMakeId { get; set; }

        [Required]
        [Display(Name = "Vehicle Model")]
        public int? VehicleModelId { get; set; }

        [Required]
        [Display(Name = "Vehicle Model Number")]
        public int? VehicleModelNumberId { get; set; }

        [Required]
        [Display(Name = "Color")]
        public int? VehicleColourId { get; set; }

        [Required]
        [Display(Name = "Year Of Make")]
        [MaxLength(4)]
        [Range(1908, 3000)]
        public int YearOfMake { get; set; }

        //[Required]
        [Display(Name = "VLN")]
        [StringLength(128)]
        public string VLN { get; set; }

        [Required]
        [Display(Name = "Net Weight")]
        [Range(50, 30000)]
        public int? NetWeight { get; set; }

        [Required]
        [Display(Name = "GVM")]
        [Range(50, 30000)]
        public int GVM { get; set; }

        [Required]
        [Display(Name = "Seating Capacity")]
        [Range(0, 100)]
        public int? SeatingCapacity { get; set; }

        [Required]
        [Display(Name = "Propelled By")]
        public int? PropelledById { get; set; }

        [Required]
        [Display(Name = "Fuel Type")]
        public int? FuelTypeId { get; set; }

        [Required]
        [Display(Name = "Registration Status")]
        public int? RegistrationStatusId { get; set; }

        //[Required]
        [Display(Name = "Licence Expiry Date")]
        public DateTime? LicenceExpiryDate { get; set; }

       // [Required]
        [Display(Name = "Roadworthy Date")]
       // [StringLength(128)]
        public DateTime? RoadworthyExpiryDate { get; set; }

       // [Required]
        [Display(Name = "Insurance Expiry Date")]
       // [StringLength(128)]
        public DateTime? InsuranceExpiryDate { get; set; }

        [Required]
        [Display(Name = "Captured Date")]
        public DateTime CapturedDate { get; set; }

        [Required]
        [Display(Name = "Captured By")]
      //  [StringLength(128)]
        public int CapturedCredentialId { get; set; }

        public string FormattedLicenceExpiryDate
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:dd/MM/yyyy}", LicenceExpiryDate); }
        }

        public string FormattedInsuranceExpiryDate
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:dd/MM/yyyy}", InsuranceExpiryDate); }
        }

        public string FormattedCapturedDate
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:dd/MM/yyyy}", CapturedDate); }
            set { }
        }

        public string FormattedRoadworthyExpiryDate
        {
            get { return string.Format(CultureInfo.InvariantCulture, "{0:dd/MM/yyyy}", RoadworthyExpiryDate); }
        }
        

        public IList<VehicleBookingModel> TestBookings { get; set; } 

        public string FirstBookTestReference
        {
            get
            {
                if (TestBookings == null || TestBookings.Count == 0)
                    return string.Empty;

                return TestBookings[0].BookingReference;
            }
        }
    }
}
