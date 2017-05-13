using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Framework;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting
{
    public class TransactionDuplicateDocumentService : BaseDocumentService<TransactionDuplicateDocument, DuplicateTransactionFilter>
    {
        public TransactionDuplicateDocumentService(MongoRead mongo)
            : base(mongo)
        {

        }

        protected override MongoCollection Items
        {
            get
            {
                return _read.TransactionDuplicates;
            }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(DuplicateTransactionFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerId", filter.LedgerId);
            }
            if (!string.IsNullOrEmpty(filter.IdIsNot))
            {
                yield return Query.NE("_id", filter.IdIsNot);
            }
        }
    }
}
