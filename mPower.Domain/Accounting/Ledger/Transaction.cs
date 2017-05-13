using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Accounting.Ledger
{
    public class Transaction
    {
        public String Id { get; set; }

        public String BaseEntryAccountId { get; set; }

        public TransactionType Type { get; set; }

        public bool Imported { get; set; }

        private List<Entry> _entries = new List<Entry>();

        public List<Entry> Entries
        {
            get { return _entries; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public Transaction(String transactionId, TransactionType type, List<Entry> entries, string baseEntryAccountId, bool imported)
        {
            Id = transactionId;
            Modify(type, entries, baseEntryAccountId, imported);
        }

        public void Modify(TransactionType type, List<Entry> newEntries, string baseEntryAccountId, bool imported)
        {
            Type = type;
            _entries = newEntries;
            BaseEntryAccountId = baseEntryAccountId;
        }

        public Boolean IsBalanced()
        {
            return _entries.Sum(e => e.CreditAmount) == _entries.Sum(e => e.DebitAmount);
        }

        public List<AccountBalanceDelta> GetAccountBalance()
        {
            return _entries
                .GroupBy(e => e.AccountId)
                .Select(g => new AccountBalanceDelta
                {
                    CreditAmount = g.Sum(e => e.CreditAmount),
                    DebitAmount = g.Sum(e => e.DebitAmount),
                    AccountId = g.Key
                })
                .ToList();
        }
    }
}