using Kapsch.Core.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("ACCOUNT_TRANSACTION", Schema = "FINANCE")]
    public class AccountTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("ACCOUNT_TRANSACTION_TYPE_ID")]
        public AccountTransactionType TransactionType { get; set; }

        [Column("ACCOUNT_TRANSACTION_STATUS_ID")]
        public AccountTransactionStatus Status { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("ENTITY_REFERENCE_TYPE_ID")]
        public long EntityReferenceTypeID { get; set; }

        [Column("AMOUNT")]
        public decimal Amount { get; set; }

        [Column("ACCOUNT_ID")]
        public long AccountID { get; set; }
        
        [Column("REFERENCE_TRANSACTION_ID")]
        public long RefTransactionID { get; set; }

        [Column("REFERENCE_TRANSACTION_TYPE_ID")]
        public int RefTransactionType { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("REQUEST_DATE")]
        public DateTime? RequestTimestamp { get; set; }

        [Column("CREDENTIAL_ID")]
        public long CredentialID { get; set; }

        [ForeignKey("ID")]
        public virtual Account Account { get; set; }

        [ForeignKey("CredentialID")]
        public virtual Credential Credential { get; set; }
    }
}
