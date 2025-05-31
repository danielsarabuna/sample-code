using System.Collections.Generic;
using Model;

namespace Proxy
{
    public class TransactionsProxy : ITransactions
    {
        private readonly ITransactions _transactions;
        public ICollection<decimal> TransactionHistory => throw new System.NotImplementedException();

        public TransactionsProxy(ITransactions transactions)
        {
            _transactions = transactions;
        }
    }
}