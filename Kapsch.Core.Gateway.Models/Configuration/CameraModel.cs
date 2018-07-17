using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Configuration
{
    public class CameraModel
    {
        [Required]
        [EnumDataType(typeof(CameraAdapterType))]
        public CameraAdapterType AdapterType { get; set; }

        /// <summary>
        /// The Id which identifies the device on the iTrac core database.
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// The FriendlyName which identifies the device.
        /// </summary>
        [Required]
        public string FriendlyName { get; set; }

        /// <summary>
        /// The property that decides remote connectivity
        /// </summary>
        [Required]
        public bool? ConnectToHost { get; set; }

        /// <summary>
        /// The GpsLatitude.
        /// </summary>

        public decimal? GpsLatitude { get; set; }
        /// <summary>
        /// The GpsLongitude.
        /// </summary>

        public decimal? GpsLongitude { get; set; }

        /// <summary>
        /// The connection type the devices is linked to for example Tcp, File or Rdb
        /// </summary>
        [Required]
        [EnumDataType(typeof(CameraConnectionType))]
        public CameraConnectionType DeviceConnectionType { get; set; }

        /// <summary>
        /// The property that determines the 
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// The json connection string consist of properties of which is user defined.
        /// Tcp {"Ip" : "192.168.0.10", "Port" : "1999"}
        /// File {"Type" : "Xml", "FileWatchPath" : "C:\Watchpath"}
        /// Rdb {"SqlConnectionType" : "MsSql", "ConnectionString" : "" }
        /// </summary>
        [Required]
        public string ConfigJson { get; set; }

        /// <summary>
        /// The CreatedTimeStamp.
        /// </summary>
        [Required]
        public DateTime CreatedTimeStamp { get; set; }

        /// <summary>
        /// The UpdatedTimeStamp.
        /// </summary>
        [Required]
        public DateTime? ModifiedTimeStamp { get; set; }

        [EnumDataType(typeof(CameraStatusType))]
        public CameraStatusType DeviceStatus { get; set; }
    }
}
