using Kapsch.Core.Gateway.Models.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleBookingModel
    {
        public VehicleModel Vehicle = new VehicleModel();

        public List<VehicleMakeModel> VehicleMakes = new List<VehicleMakeModel>();
        public List<VehicleModelModel> VehicleModels = new List<VehicleModelModel>();
        public List<VehicleModelNumberModel> VehicleModelNumbers = new List<VehicleModelNumberModel>();
        public List<VehicleCategoryModel> VehicleCategories = new List<VehicleCategoryModel>();
        public List<VehicleTypeModel> VehicleTypes = new List<VehicleTypeModel>();
        public List<VehiclePropellerModel> VehiclePropellers = new List<VehiclePropellerModel>();
        public List<VehicleFuelTypeModel> VehicleFuelType = new List<VehicleFuelTypeModel>();
        public List<VehicleColorModel> VehicleColors = new List<VehicleColorModel>();
        public List<TestCategoryModel> TestCategories = new List<TestCategoryModel>();
        public List<DistrictModel> Districts = new List<DistrictModel>();
        public List<SiteModel> Sites = new List<SiteModel>();

        public int TestCategoryID { get; set; }
        public int CredentialID { get; set; }
        public int CapturedByCredentialId { get; set; }
        public DateTime CapturedDate { get; set; }

        [Display(Name = "Booking Reference Number")]
        [StringLength(128)]
        [Required]
        public string BookingReference { get; set; }
        public int SiteNumber { get; set; }

        public int IsSuccessfull { get; set; }        
    }
}
