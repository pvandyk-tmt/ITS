using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Data
{
    [Table("SYSTEM_ROLE_FUNCTION", Schema = "CREDENTIALS")]
    public class SystemRoleFunction
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("SYSTEM_ROLE_ID")]
        public long RoleID { get; set; }

        [Column("SYSTEM_FUNCTION_ID")]
        public long FunctionID { get; set; }

        [ForeignKey("RoleID")]
        public virtual SystemRole SystemRole { get; set; }

        [ForeignKey("FunctionID")]
        public virtual SystemFunction SystemFunction { get; set; }
    }
}
