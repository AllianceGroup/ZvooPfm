using MongoDB.Bson.Serialization.Attributes;

namespace mPower.Documents.Documents.Accounting.Ledger
{
    public class ImportedTransactionDocument
    {
        [BsonId]
        public string TransactionId { get; set; }
        public string LedgerId { get; set; }
        public string ImportedTransactionId { get; set; }
    }
}