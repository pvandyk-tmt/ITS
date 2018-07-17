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
    [Table("CAMERA_STATUS_HISTORY", Schema = "ITS")]
    public class CameraStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("CAMERA_ID")]
        public long CameraID { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimeStamp { get; set; }

        [Column("CAMERA_STATUS_TYPE_ID")]
        public CameraStatusType CameraStatusType { get; set; }

        [ForeignKey("CameraID")]
        public virtual Camera Camera { get; set; }
    }
}
