using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    public class MobileDeviceLocationModel
    {
        public long ID { get; set; }
        public decimal? GpsLatitude { get; set; }
        public decimal? GpsLongitude { get; set; }
        public long MobileDeviceID { get; set; }
        public DateTime LocationTimestamp { get; set; }
    }
}
