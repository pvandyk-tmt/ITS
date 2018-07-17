using Foolproof;
using Kapsch.ITS.Gateway.Models.Enums;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Kapsch.ITS.Gateway.Models.Fine
{
    public class ChangeAmountModel
    {
        [Required]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        [DisplayName("Password")]
        public string Password { get; set; }

        [Required]
        [DisplayName("Reference Number")]
        public string ReferenceNumber { get; set; }

        [Required]
        [DisplayName("Current Amount")]
        public decimal CurrentAmount { get; set; }

        [Required]
        [DisplayName("New Amount")]
        [NotEqualTo("CurrentAmount")]
        [Range(typeof(Decimal),"0.0", "1000000000000000000")]
        public decimal NewAmount { get; set; }

        [Required]
        [DisplayName("Reason")]
        public AccountTransactionType AccountTransactionType { get; set; }

        [DisplayName("Applicant Reason")]
        public string ApplicantReason { get; set; }

        [DisplayName("Approved By")]
        public string ApprovedBy { get; set; }

        [DisplayName("Approved Date")]
        public DateTime? ApprovedDate { get; set; }

        [Required]
        [DisplayName("Currency")]
        public AccountCurrencyType AccountCurrencyType { get; set; }

        [DisplayName("Terminal Name")]
        public string TerminalName { get; set;  }

        public ReferenceTransactionType ReferenceTransactionType { get; set; }
    }
}
