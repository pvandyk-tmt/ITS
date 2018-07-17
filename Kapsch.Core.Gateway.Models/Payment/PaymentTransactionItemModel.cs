using Kapsch.Core.Gateway.Models.Enums;

namespace Kapsch.Core.Gateway.Models.Payment
{
    public class PaymentTransactionItemModel
    {
        public string ReferenceNumber { get; set; }
        public long EntityReferenceTypeID { get; set; }
        public string TransactionToken { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public PaymentTransactionStatus Status { get; set; }
    }
}
