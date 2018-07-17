using System;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.EVR.Gateway.Models.Vehicle
{
    public class VehicleModelNumberModel
    {
        public int ID { get; set; }
        public int VehicleModelID { get; set; }

        [Display(Name = "Vehicle Model")]
        public string VehicleModelDescription { get; set; }

        [Required()]
        [StringLength(100)]
        public string Description { get; set; }

        [Display(Name = "External Code")]
        [StringLength(20)]
        public string ExternalCode { get; set; }

        public string ExternalModelCode { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}




