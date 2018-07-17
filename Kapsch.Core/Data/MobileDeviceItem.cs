using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("MOBILE_DEVICE_ITEM", Schema = "ITS")]
    public class MobileDeviceItem
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("VALUE")]
        public string Value { get; set; }
    }
}
