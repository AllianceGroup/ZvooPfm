using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class KeywordMapDocument
    {
        public string AccountId { get; set; }
        
        public string Keyword { get; set; }
    }

    
}