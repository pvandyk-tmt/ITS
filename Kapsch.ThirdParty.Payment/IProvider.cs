using Kapsch.ThirdParty.Payment.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kapsch.ThirdParty.Payment
{
    public interface IProvider
    {
        TransactionIDModel RegisterTransaction(TransactionModel model);
        void UpdateTransaction(string transactionToken, TransactionModel model);
        void CancelTransaction(string transactionToken);
        TransactionModel VerifyTransaction(string transactionToken);

        int ID { get; }
        Action<int, string, string, string, bool> Log { set; }
    }
}
