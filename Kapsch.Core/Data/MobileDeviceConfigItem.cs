using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("MOBILE_DEVICE_CONFIG_ITEM", Schema = "ITS")]
    public class MobileDeviceConfigItem
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("VALUE")]
        public string Value { get; set; }

        [Column("MOBILE_DEVICE_ID")]
        public long MobileDeviceID { get; set; }

        [ForeignKey("MobileDeviceID")]
        public virtual MobileDevice MobileDevice { get; set; }
    }
}
