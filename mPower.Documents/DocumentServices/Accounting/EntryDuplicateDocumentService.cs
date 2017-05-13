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
    public class EntryDuplicateDocumentService : BaseDocumentService<EntryDuplicateDocument, DuplicateEntryFilter>
    {
        public EntryDuplicateDocumentService(MongoRead mongo)
            : base(mongo)
        {

        }

        protected override MongoCollection Items
        {
            get
            {
                return _read.EntryDuplicates;
            }
        }
        protected override IEnumerable<IMongoQuery> BuildFilterQuery(DuplicateEntryFilter filter)
        {
            throw new System.NotImplementedException();
        }
    }
}
