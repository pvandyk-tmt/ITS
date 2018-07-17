using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.Core.Gateway.Models.Computer
{
    public class ComputerModel
    {
        public long ID { get; set; }

        [Required]
        [Display(Name = "Name")]
        [StringLength(50)]
        public string Name { get; set; }

        [Required]
        [Display(Name = "IP Address")]
        [StringLength(30)]
        public string IPAddress { get; set; }

        [Display(Name = "District")]
        public long? DistrictID { get; set; }

        [Display(Name = "District")]
        public string DistrictName { get; set; }

        public IList<ComputerConfigSettingModel> ComputerConfigSettings { get; set; }
    }
}
