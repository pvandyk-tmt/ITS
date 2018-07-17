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
    [Table("CREDENTIAL_SYSTEM_FUNCT_REL", Schema = "CREDENTIALS")]
    public class CredentialSystemFunction
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("CREDENTIAL_ID")]
        public long CredentialID { get; set; }

        [Column("SYSTEM_FUNCTION_ID")]
        public long SystemFunctionID { get; set; }

        [Column("INDICATOR_STATUS_ID")]
        public Status Status { get; set; }

        [ForeignKey("CredentialID")]
        public virtual Credential Credential { get; set; }

        [ForeignKey("SystemFunctionID")]
        public virtual SystemFunction SystemFunction { get; set; }
    }
}
