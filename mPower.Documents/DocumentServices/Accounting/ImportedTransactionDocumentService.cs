using System.Collections.Generic;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting
{
    public class ImportedTransactionDocumentService : BaseDocumentService<ImportedTransactionDocument, ImportedTransactionDocumentFilter>
    {
        public ImportedTransactionDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.ImportedTransactions; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(ImportedTransactionDocumentFilter filter)
        {
            yield break;
        }


        public bool TransactionNotImported(string ledgerId, string transactionId)
        {
            var query = Query.And(Query.EQ("LedgerId", ledgerId), Query.EQ("ImportedTransactionId", transactionId));

            var items = GetByQuery(query);

            return items.Count == 0;
        }

    }
}
