using System;
using System.Collections.Generic;

namespace mPower.Domain.Accounting.Ledger
{
    public class TransactionCollection
    {
        private readonly Dictionary<string, Transaction> _transactionsById = new Dictionary<string, Transaction>();

        public Int32 Count
        {
            get { return _transactionsById.Count; }
        }

        public void Add(Transaction transaction)
        {
            _transactionsById[transaction.Id] = transaction;
        }

        public Transaction Get(String id)
        {
            return _transactionsById[id];
        }

        public void Remove(String transactionId)
        {
            _transactionsById.Remove(transactionId);
        }

        /// <summary>
        /// Collection of transactions is balanced when and only when 
        /// all transactions are balanced.
        /// </summary>
        public Boolean IsBalanced()
        {
            foreach (var transaction in _transactionsById)
            {
                if (!transaction.Value.IsBalanced())
                    return false;
            }

            return true;
        }

        public bool Exists(string transactionId)
        {
            return _transactionsById.ContainsKey(transactionId);
        }
    }
}