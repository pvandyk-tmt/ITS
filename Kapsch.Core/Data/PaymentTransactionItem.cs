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
    [Table("PAYMENT_TRANSACTION_ITEM", Schema = "FINANCE")]
    public class PaymentTransactionItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("PAYMENT_TRANSACTION_ID")]
        public long PaymentTransactionID { get; set; }

        [Column("TRANSACTION_TOKEN")]
        public string TransactionToken { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("ENTITY_REFERENCE_TYPE_ID")]
        public long EntityReferenceTypeID { get; set; }

        [Column("AMOUNT")]
        public decimal Amount { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("TRANSACTION_STATUS_ID")]
        public PaymentTransactionStatus Status { get; set; }

        public virtual PaymentTransaction PaymentTransaction { get; set; }
    }
}
