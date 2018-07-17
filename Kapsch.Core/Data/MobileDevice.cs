using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("MOBILE_DEVICE", Schema = "ITS")]
    public class MobileDevice
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("DEVICE_ID")]
        public string DeviceID { get; set; }

        [Column("DISTRICT_ID")]
        public long? DistrictID { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("SERIAL_NUMBER")]
        public string SerialNumber { get; set; }

        [Column("MOBILE_STATUS_ID")]
        public Status Status { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

        public virtual IList<MobileDeviceConfigItem> DeviceItems { get; set; }
    }
}
