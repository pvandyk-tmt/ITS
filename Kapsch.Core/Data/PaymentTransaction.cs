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
    [Table("PAYMENT_TRANSACTION", Schema = "FINANCE")]
    public class PaymentTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("TRANSACTION_TOKEN")]
        public string TransactionToken { get; set; }

        [Column("TRANSACTION_REFERENCE")]
        public string TransactionReference { get; set; }

        [Column("RECEIPT_NUMBER")]
        public string Receipt { get; set; }

        [Column("RECEIPT_DATE")]
        public DateTime? ReceiptTimestamp { get; set; }

        [Column("COURT_DETAIL_ID")]
        public long? CourtID { get; set; }

        [Column("TERMINAL_ID")]
        public long? TerminalID { get; set; }

        [Column("PAYMENT_METHOD_ID")]
        public PaymentMethod PaymentMethod { get; set; }

        [Column("CUSTOMER_FIRST_NAME")]
        public string CustomerFirstName { get; set; }

        [Column("CUSTOMER_LAST_NAME")]
        public string CustomerLastName { get; set; }

        [Column("CUSTOMER_ID_NUMBER")]
        public string CustomerIDNumber { get; set; }

        [Column("CUSTOMER_CONTACT_NUMBER")]
        public string CustomerContactNumber { get; set; }

        [Column("AMOUNT")]
        public decimal Amount { get; set; }

        [Column("CREDENTIAL_ID")]
        public long? CredentialID { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime? ModifiedTimestamp { get; set; }

        [Column("TRANSACTION_STATUS_ID")]
        public PaymentTransactionStatus Status { get; set; }

        [ForeignKey("CredentialID")]
        public virtual Credential Credential { get; set; }

        [ForeignKey("TerminalID")]
        public virtual PaymentTerminal PaymentTerminal { get; set; }

        [ForeignKey("CourtID")]
        public virtual Court Court { get; set; }
         
        public virtual IList<PaymentTransactionItem> TransactionItems { get; set; }
    }
}
