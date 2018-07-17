using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("COMPUTER", Schema = "CREDENTIALS")]
    public class Computer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("NAME")]
        public string Name { get; set; }

        [Column("IP_ADDRESS")]
        public string IPAddress { get; set; }       

        [Column("DISTRICT_ID")]
        public long? DistrictID { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }

        public virtual IList<ComputerConfigSetting> ComputerConfigSettings { get; set; }
    }
}
