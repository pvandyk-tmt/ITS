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
    [Table("MOBILE_DEVICE_APPLICATION", Schema = "ITS")]
    public class MobileDeviceApplication
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("SOFTWARE_VERSION")]
        public string SoftwareVersion { get; set; }

        [Column("APPLICATION_TYPE_ID")]
        public ApplicationType ApplicationType { get; set; }

        [Column("STATUS_ID")]
        public Status Status { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime? ModifiedTimestamp { get; set; }       
    }
}
