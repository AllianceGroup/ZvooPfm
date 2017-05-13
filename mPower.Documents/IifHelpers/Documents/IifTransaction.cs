using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.IifHelpers.Documents
{
    public class IifTransaction
    {
        [BsonId]
        public string Id { get; set; }

        public bool Include { get; set; }

        public TransactionType Type { get; set; }

        public List<IifEntry> Entries { get; set; }

        public DateTime BookedDate { get; set; }

        public IifTransaction()
        {
            Include = true;
            Entries = new List<IifEntry>();
        }
    }
}
