using System;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleModelModel
    {
        public int ID { get; set; }
        public int VehicleMakeID { get; set; }

        [Display(Name = "Vehicle Make")]
        public string VehicleMakeDescription { get; set; }

        [Required()]
        [StringLength(100)]
        public string Description { get; set; }

        [Display(Name = "External Code")]
        [StringLength(20)]
        public string ExternalCode { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}
