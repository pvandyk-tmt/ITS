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
    [Table("ROLLING_REGISTER", Schema = "FINANCE")]
    public class RollingRegister
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("ROLLING_REGISTER_TYPE_ID")]
        public RollingRegisterType RollingRegisterType { get; set; }

        [Column("DATE_OPENED")]
        public DateTime DateOpened { get; set; }

        [Column("AMOUNT")]
        public decimal Amount { get; set; }

        [Column("CREDENTIAL_ID")]
        public long CredentialID { get; set; }

        [Column("PAYMENT_METHOD_ID")]
        public PaymentMethod PaymentMethod { get; set; }

        [Column("PAYMENT_TRANSACTION_ID")]
        public long? PaymentTransactionID { get; set; }

        [Column("REFERENCE")]
        public string Reference { get; set; }

        [Column("ROLLING_REGISTER_STATUS_ID")]
        public RollingRegisterStatus RollingRegisterStatus { get; set; }

        [Column("DATE_CLOSED")]
        public DateTime? DateClosed { get; set; }

        [ForeignKey("PaymentTransactionID")]
        public virtual PaymentTransaction PaymentTransaction { get; set; }
    }
}
