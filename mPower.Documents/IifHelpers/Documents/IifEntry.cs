using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.IifHelpers.Documents
{
    public class IifEntry
    {
        public string AccountName { get; set; }

        public long Amount { get; set; }

        public long Debit { get; set; }

        public long Credit { get; set; }

        public string Memo { get; set; }

        public string Payee { get; set; }
    }
}
