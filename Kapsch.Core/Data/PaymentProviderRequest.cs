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
    [Table("PAYMENT_PROVIDER_REQUESTS", Schema = "FINANCE")]
    public class PaymentProviderRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("PAYMENT_PROVIDER_ID")]
        public PaymentProvider PaymentProvider { get; set; }

        [Column("CREATED_DATE")]
        public DateTime CreatedTimestamp { get; set; }

        [Column("REQUEST_DETAIL")]
        public string RequestDetail { get; set; }

        [Column("RESPONSE_DETAIL")]
        public string ResponseDetail { get; set; }

        [Column("EXCEPTION_MESSAGE")]
        public string ExceptionMessage { get; set; }

        [Column("REQUEST_STATUS_ID")]
        public RequestStatus Status { get; set; }
    }
}
