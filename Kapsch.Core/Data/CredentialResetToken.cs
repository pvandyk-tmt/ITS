using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("CREDENTIAL_RESET_TOKEN", Schema = "CREDENTIALS")]
    public class CredentialResetToken
    {
        [Key]
        [Column("ID")]
        [DatabaseGenerated(System.ComponentModel.DataAnnotations.Schema.DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Column("CREDENTIAL_ID")]
        public long CredentialID { get; set; }

        [Column("EXPIRY_TIMESTAMP")]
        public DateTime ExpiryTimestamp { get; set; }

        [Column("TOKEN")]
        public string Token { get; set; }

        [ForeignKey("CredentialID")]
        public virtual Credential Credential { get; set; }
    }
}
