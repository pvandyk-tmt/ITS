using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("CAMERA", Schema = "ITS")]
    public class Camera
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("FRIENDLY_NAME")]
        public string FriendlyName { get; set; }

        [Column("CONNECT_TO_HOST")]
        public string ConnectToHost { get; set; }

        [Column("INFRINGEMENT_LOCATION_ID")]
        public long? InfringementLocationID { get; set; }

        [Column("GPS_LATITUDE")]
        public decimal? GpsLatitude { get; set; }

        [Column("GPS_LONGITUDE")]
        public decimal? GpsLongitude { get; set; }

        [Column("CAMERA_ADAPTER_TYPE_ID")]
        public CameraAdapterType CameraAdapterType { get; set; }

        [Column("CAMERA_CONNECTION_TYPE_ID")]
        public CameraConnectionType CameraConnectionType { get; set; }

        [Column("CAMERA_STATUS_TYPE_ID")]
        public CameraStatusType CameraStatusType { get; set; }

        [Column("IS_ENABLED")]
        public string IsEnabled { get; set; }

        [Column("CONFIG_JSON")]
        public string ConfigJson { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimeStamp { get; set; }

        [Column("MODIFIED_TIMESTAMP")]
        public DateTime? ModifiedTimeStamp { get; set; }

        [ForeignKey("InfringementLocationID")]
        public virtual InfringementLocation InfringementLocation { get; set; }
    }
}
