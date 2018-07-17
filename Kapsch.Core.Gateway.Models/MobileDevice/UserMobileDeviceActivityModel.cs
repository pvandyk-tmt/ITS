using System;

namespace Kapsch.Core.Gateway.Models.MobileDevice
{
    public class UserMobileDeviceActivityModel
    {
        public long ID { get; set; }
        public DateTime CreatedTimestamp { get; set; }
        public string DeviceID { get; set; }
        public long? CredentialID { get; set; }
        public string Category { get; set; }
        public string ActionDescription { get; set; }
    }
}
