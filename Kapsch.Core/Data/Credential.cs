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
    [Table("CREDENTIAL", Schema = "CREDENTIALS")]
    public class Credential
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("USER_NAME")]
        public string UserName { get; set; }

        [Column("PASSWORD")]
        public string Password { get; set; }

        [Column("ENTITY_TYPE_ID")]
        public EntityType EntityType { get; set; }

        [Column("ENTITY_ID")]
        public long EntityID { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimeStamp { get; set; }

        [Column("EXPIRY_TIMESTAMP")]
        public DateTime ExpiryTimeStamp { get; set; }

        [Column("CREDENTIAL_STATUS_ID")]
        public Status Status { get; set; }

        [ForeignKey("EntityID")]
        public virtual User User { get; set; }

        public virtual IList<CredentialSystemFunction> CredentialSystemFunctions { get; set; }
    }
}
