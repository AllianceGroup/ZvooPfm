using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace mPower.TempDocuments.Server.Documents
{
    [BsonIgnoreExtraElements]
    public class ImportTemporaryDocument
    {
        [BsonId]
        public string Id { get; set; }

        public List<LedgerImportDocument> LedgerImports { get; set; }

        public ImportTemporaryDocument()
        {
            LedgerImports = new List<LedgerImportDocument>();
        }
    }
}
