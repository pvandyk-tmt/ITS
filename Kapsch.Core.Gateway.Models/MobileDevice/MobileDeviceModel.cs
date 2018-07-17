using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Gateway.Models.MobileDevice
{
    public class MobileDeviceModel
    {
        public long ID { get; set; }
        public string DeviceID { get; set; }
        public long? DistrictID { get; set; }
        public string DistrictName { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public string SerialNumber { get; set; }
        public MobileDeviceStatus Status { get; set; }
    }
}
