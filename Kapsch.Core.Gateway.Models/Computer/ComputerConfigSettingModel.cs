using Kapsch.Core.Gateway.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.Core.Gateway.Models.Computer
{
    public class ComputerConfigSettingModel
    {
        public long ID { get; set; }

        [Display(Name = "Configuration Type")]
        public ComputerItemType ComputerItemType { get; set; }

        [Required]
        [Display(Name = "Value")]
        [StringLength(256)]
        public string Value { get; set; }

        [Display(Name = "Computer")]
        [Required]
        public long ComputerID { get; set; }

        [Display(Name = "Computer")]
        public string ComputerName { get; set; }
    }
}
