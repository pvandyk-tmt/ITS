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
    [Table("USER_DETAIL_DISTRICTS_REL", Schema = "CREDENTIALS")]
    public class UserDistrict
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("USER_DETAIL_ID")]
        public long UserID { get; set; }

        [Column("DISTRICT_ID")]
        public long DistrictID { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("DistrictID")]
        public virtual District District { get; set; }
    }
}
