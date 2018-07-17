using System;

namespace Kapsch.ThirdParty.Payment.Mock
{
    public class Provider : IProvider
    {
        private Action<int, string, string, string, bool> _log;

        public Models.TransactionIDModel RegisterTransaction(Models.TransactionModel model)
        {
            return new Models.TransactionIDModel { TransactionReference = Guid.NewGuid().ToString(), TransactionToken = Guid.NewGuid().ToString() };
        }

        public void UpdateTransaction(string transactionToken, Models.TransactionModel model)
        {
            
        }

        public void CancelTransaction(string transactionToken)
        {
            
        }

        public Models.TransactionModel VerifyTransaction(string transactionToken)
        {
            return new Models.TransactionModel {  };
        }

        public Action<int, string, string, string, bool> Log
        {
            set
            {
                _log = value;
            }
        }

        public int ID
        {
            get { return 0; }
        }
    }
}
