using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Credit.CreditIdentity
{
    public class AccountsGroupDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string AccountTypeDescription { get; set; }
        public string AccountTypeSymbol { get; set; }
        public string AccountTypeAbbreviation { get; set; }
        public List<AccountDocument> Accounts { get; set; } 
    }
}
