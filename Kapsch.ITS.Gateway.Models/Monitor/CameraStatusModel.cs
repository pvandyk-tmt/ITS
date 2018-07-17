using Kapsch.Core.Gateway.Models.Enums;
using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.ITS.Gateway.Models.Monitor
{
    public class CameraStatusModel
    {
        /// <summary>
        /// The device identifier to be added to the cache.
        /// </summary>
        public long DeviceID { get; set; }

        /// <summary>
        /// The devices current monitoring status.
        /// </summary>
        [Required]
        [EnumDataType(typeof(CameraStatusType))]
        public CameraStatusType CameraStatusType { get; set; }

        /// <summary>
        /// The CreatedTimeStamp.
        /// </summary>
        [Required]
        public DateTime CreatedTimeStamp { get; set; }
    }
}
