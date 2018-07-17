using Kapsch.Core.Gateway.Models.Enums;

namespace Kapsch.Core.Gateway.Models
{
    public class ComputerConfigSettingModel
    {
        public long ID { get; set; }

        public ComputerItemType ComputerItemType { get; set; }

        public string Value { get; set; }       

        public long ComputerID { get; set; }

        public string ComputerName { get; set; }
    }
}
