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
    [Table("ACCOUNT", Schema = "FINANCE")]
    public class Account
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; }

        [Column("LAST_TRANSACTION_DATE")]
        public DateTime LastTransactionDate { get; set; }

        [Column("ACCOUNT_TYPE_ID")]
        public AccountType AccountType { get; set; }

        [Column("CURRENCY_TYPE_ID")]
        public AccountCurrencyType AccountCurrency { get; set; }

        [Column("MIN_BALANCE")]
        public decimal? MinBalance { get; set; }

        [Column("MAX_BALANCE")]
        public decimal? MaxBalance { get; set; }

        [Column("OUTSTANDING_BALANCE")]
        public decimal OutstandingBalance { get; set; }

        [Column("STATUS_ID")]
        public Status Status { get; set; }

        [Column("DESCRIPTION")]
        public string Description { get; set; }

        [Column("PERSON_INFO_ID")]
        public long PersonID { get; set; }

        [ForeignKey("PersonID")]
        public virtual Person Person { get; set; }
    }
}
