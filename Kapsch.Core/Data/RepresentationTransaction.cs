using Kapsch.Core.Data.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kapsch.Core.Data
{
    [Table("REPRESENTATION_TRANSACTION", Schema = "FINANCE")]
    public class RepresentationTransaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ID")]
        public long ID { get; set; } 
  
        [Column("REGISTER_ID")]
        public long RegisterID { get; set; }

        [Column("REFERENCE_NUMBER")]
        public string ReferenceNumber { get; set; }

        [Column("AMOUNT")]
        public decimal Amount { get; set; }

        [Column("REASON")]
        public string Reason { get; set; }

        [Column("ACCOUNT_TRANSACTION_TYPE_ID")]
        public AccountTransactionType AccountTransactionType { get; set; }

        [Column("CURRENCY_TYPE_ID")]
        public AccountCurrencyType? AccountCurrencyType { get; set; }

        [Column("CAPTURED_CREDENTIAL_ID")]
        public long CapturedCredentialID { get; set; }

        [Column("CAPTURED_DATE")]
        public DateTime CapturedDate { get; set; }

        [Column("RESULT_TYPE_ID")]
        public ResultType ResultType { get; set; }

        [Column("EVALUATED_DATE")]
        public DateTime? EvaluatedDate { get; set; }

        [Column("EVALUATED_BY")]
        public string EvaluatedBy { get; set; }

        [Column("CHARGE_NO")]
        public long ChargeNumber { get; set; }

        [Column("CHARGE_CODE")]
        public string ChargeCode { get; set; }

        [Column("PROCESSED_DATE")]
        public DateTime? ProcessedDate { get; set; }

        [Column("PROCESSED_TERMINAL_NAME")]
        public string ProcessedTerminalName { get; set; }

        [Column("PROCESSED_CREDENTIAL_ID")]
        public long? ProcessedCredentialID { get; set; }
    }
}
