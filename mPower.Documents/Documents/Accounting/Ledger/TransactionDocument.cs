using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class TransactionDocument
    {
        
        public AccountTypeEnum BaseEntryAccountType { get; set; }
        public string BaseEntryAccountId { get; set; }

        public TransactionDocument()
        {
            Entries = new List<TransactionEntryDocument>();
        }

        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public TransactionType Type { get; set; }

        public List<TransactionEntryDocument> Entries { get; set; }

        public string Memo { get; set; }

        public DateTime BookedDate { get; set; }

        public bool Imported { get; set; }

        public string ReferenceNumber { get; set; }

        public bool ConfirmedNotDuplicate { get; set; }
    }
}
