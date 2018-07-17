using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.RTE.Data
{
    [Table("CAMERA_POINT_DATA", Schema = "ITS")]
    public class CameraPointData
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("CAMERA_POINT_DATA_JSON")]
        public string Json { get; set; }

        [Column("CREATED_DATE")]
        public DateTime TimeStamp { get; set; }
    }
}
