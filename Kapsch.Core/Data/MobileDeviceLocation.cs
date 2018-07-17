using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("MOBILE_DEVICE_LOCATION", Schema = "ITS")]
    public class MobileDeviceLocation
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("GPS_LATITUDE")]
        public decimal? GpsLatitude { get; set; }

        [Column("GPS_LONGITUDE")]
        public decimal? GpsLongitude { get; set; }

        [Column("MOBILE_DEVICE_ID")]
        public long MobileDeviceID { get; set; }

        [Column("LOCATION_TIMESTAMP")]
        public DateTime LocationTimestamp { get; set; }

        [ForeignKey("MobileDeviceID")]
        public virtual MobileDevice MobileDevice { get; set; }
    }
}
