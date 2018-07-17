using Kapsch.Core.Gateway.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.Core.Gateway.Models.Payment
{
    public class PaymentTransactionModel
    {
        public long ID { get; set; }
        public string Receipt { get; set; }
        public DateTime? ReceiptTimestamp { get; set; }
        public string TransactionToken { get; set; }
        public TerminalType TerminalType { get; set; }
        public string TerminalUUID { get; set; } 
        public long? CourtID { get; set; }
        public PaymentMethod PaymentSource { get; set; }
        public string CustomerFirstName { get; set; }
        public string CustomerLastName { get; set; }
        public string CustomerIDNumber { get; set; }
        public string CustomerContactNumber { get; set; }
        public decimal Amount { get; set; }
        public long? CredentialID { get; set; }      
        public DateTime CreatedTimestamp { get; set; }
        public DateTime? ModifiedTimestamp { get; set; }
        public PaymentTransactionStatus Status { get; set; }
        public IList<PaymentTransactionItemModel> PaymentTransactionItems { get; set; }
    }
}
