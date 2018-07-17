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
    [Table("PAYMENT_TERMINAL", Schema = "FINANCE")]
    public class PaymentTerminal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("TERMINAL_TYPE_ID")]
        public TerminalType TerminalType { get; set; }

        [Column("UUID")]
        public string UUID { get; set; }

        [Column("STATUS_ID")]
        public Status Status { get; set; }

        [Column("MODIFIED_DATE")]
        public DateTime? ModifiedTimestamp { get; set; }    
    }
}
