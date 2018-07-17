using Kapsch.Core.Data.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("MOBILE_DEVICE_DB_SCRIPT", Schema = "ITS")]
    public class MobileDeviceDbScript
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("SCRIPT")]
        public string Script { get; set; }
    }
}
