using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class EntryDuplicateDocument
    {
        [BsonId]
        public string Id { get; set; }

        public EntryDocument ManualEntry { get; set; }
        public IEnumerable<EntryDocument> PotentialDuplicates { get; set; } 
    }

    
}
