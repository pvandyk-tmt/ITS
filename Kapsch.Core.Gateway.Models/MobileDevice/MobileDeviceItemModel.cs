using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.MobileDevice
{
    public class MobileDeviceItemModel
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public MobileDeviceItemType MobileDeviceItemType { get; set; }
    }
}
