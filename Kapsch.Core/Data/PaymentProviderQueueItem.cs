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
    [Table("PAYMENT_PROVIDER_QUEUE", Schema = "FINANCE")]
    public class PaymentProviderQueueItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("QUEUE_STATUS_ID")]
        public QueueStatus QueueStatus { get; set; }

        [Column("ARGUMENTS")]
        public string Arguments { get; set; }

        [Column("TRANSACTION_TOKEN")]
        public string TransactionToken { get; set; }

        [Column("OPERATION_NAME")]
        public string OperationName { get; set; }

        [Column("PAYMENT_PROVIDER_ID")]
        public PaymentProvider PaymentProvider { get; set; }
    }
}
