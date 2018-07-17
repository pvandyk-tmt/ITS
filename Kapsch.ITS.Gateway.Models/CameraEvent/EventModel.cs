using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ITS.Gateway.Models.CameraEvent
{
    /// <summary>
    /// The model gets exposed to add device events to the iTrac Core.
    /// </summary>
    public class EventModel
    {
        /// <summary>
        /// The Id which identifies the event on the iTrac core database.
        /// </summary>
        public Guid EventID { get; set; }

        /// <summary>
        /// The Solution identifier.
        /// </summary>
        public Guid SolutionID { get; set; }

        /// <summary>
        /// The EventType.
        /// </summary>
        [Required]
        [EnumDataType(typeof(EventType))]
        public EventType EventType { get; set; }

        /// <summary>
        /// The switch that identifies a remote connetion or client connection.
        /// </summary>
        [Required]
        public bool RemoteAccess { get; set; }

        /// <summary>
        /// The Vlns.
        /// </summary>
        public IList<EventVLNModel> VLNs { get; set; }

        /// <summary>
        /// The GpsLatitude.
        /// </summary>
        public decimal? GPSLatitude { get; set; }

        /// <summary>
        /// The GpsLongitude.
        /// </summary>
        public decimal? GPSLongitude { get; set; }

        /// <summary>
        /// The Timestamp.
        /// </summary>
        [Required]
        public DateTime Timestamp { get; set; }

        /// <summary>
        /// The VlnXRectangleCoordinates.
        /// </summary>
        public decimal? VLNXRectangleCoordinates { get; set; }

        /// <summary>
        /// The VlnYRectangleCoordinates.
        /// </summary>
        public decimal? VLNYRectangleCoordinates { get; set; }

        /// <summary>
        /// The VlnRectangleWidth.
        /// </summary>
        public int? VLNRectangleWidth { get; set; }

        /// <summary>
        /// The VlnRectangleHeight.
        /// </summary>
        public int? VLNRectangleHeight { get; set; }

        /// <summary>
        /// The InstanceId.
        /// </summary>
        public string InstanceID { get; set; }

        /// <summary>
        /// The Direction.
        /// </summary>
        public string Direction { get; set; }

        /// <summary>
        /// The DeviceIdentifier.
        /// </summary>
        [Required]
        public Guid DeviceID { get; set; }

        /// <summary>
        /// The CreatedTimeStamp.
        /// </summary>
        [Required]
        public DateTime CreatedTimeStamp { get; set; }

        /// <summary>
        /// The Blob.
        /// </summary>
        [Required]
        public string Blob { get; set; }
    }
}
