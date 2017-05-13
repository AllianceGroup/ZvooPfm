using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class TransactionsStatisticDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string LedgerId { get; set; }

        public string AccountId { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }

        public long DebitAmountInCents { get; set; }

        public long CreditAmountInCents { get; set; }

        public string AccountName { get; set; }
    }
}
