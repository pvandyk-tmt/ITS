using Kapsch.Core.Data.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("COMPUTER_CONFIG_ITEMS", Schema = "CREDENTIALS")]
    public class ComputerConfigSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("COMPUTER_ITEM_TYPE_ID")]
        public ComputerItemType ComputerItemType { get; set; }

        [Column("VALUE")]
        public string Value { get; set; }       

        [Column("COMPUTER_ID")]
        public long ComputerID { get; set; }

        [ForeignKey("ComputerID")]
        public virtual Computer Computer { get; set; }
    }
}
