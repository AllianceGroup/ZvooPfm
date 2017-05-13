using MongoDB.Bson.Serialization.Attributes;
using mPower.Documents.IifHelpers;

namespace mPower.TempDocuments.Server.Documents
{
    [BsonIgnoreExtraElements]
    public class LedgerImportDocument
    {
        [BsonId]
        public string Id { get; set; }

        public IifParsingResult ParsingResult { get; set; }
    }
}
