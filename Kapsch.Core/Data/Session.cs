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
    [Table("CREDENTIAL_SESSION", Schema = "CREDENTIALS")]
    [Serializable]
    public class Session
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [Column("TOKEN")]
        public string Token { get; set; }

        [Column("CREDENTIAL_ID")]
        public long CredentialID { get; set; }

        [Column("CREATED_TIMESTAMP")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("EXPIRY_TIMESTAMP")]
        public DateTime ExpiryTimestamp { get; set; }

        [ForeignKey("CredentialID")]
        public virtual Credential Credential { get; set; }
    }
}
