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
    public class TransactionDocumentService : BaseDocumentService<TransactionDocument, TransactionFilter>
    {
        public TransactionDocumentService(MongoRead mongo)
            : base(mongo)
        {

        }

        protected override MongoCollection Items
        {
            get
            {
                return _read.Transactions;
            }
        }

        protected override IMongoSortBy BuildSortExpression(TransactionFilter filter)
        {
            var sortExpression = SortBy.Null;
            switch (filter.SortByFiled)
            {
                case TransactionsSortFieldEnum.BookedDate:
                    sortExpression = SortBy.Ascending("BookedDate");
                    break;
                case TransactionsSortFieldEnum.BookedDateDescending:
                    sortExpression = SortBy.Descending("BookedDate");
                    break;
            }

            return sortExpression;
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(TransactionFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerId", filter.LedgerId);
            }
            if (!string.IsNullOrEmpty(filter.AccountId))
            {
                yield return Query.EQ("Entries.AccountId", filter.AccountId);
            }
            else if (filter.AccountIds.Count > 0)
            {
                yield return Query.In("Entries.AccountId", BsonArray.Create(filter.AccountIds));
            }
            if (filter.EntryFromDate != null && filter.EntryToDate != null)
            {
                var toDate = filter.EntryToDate.GetValueOrDefault().Date.AddDays(1).AddMilliseconds(-1);
                yield return
                    Query.And(Query.GTE("BookedDate", filter.EntryFromDate.GetValueOrDefault().Date),
                              Query.LTE("BookedDate", toDate));
            }
            else if (filter.EntryFromDate != null)
            {
                yield return Query.GTE("BookedDate", filter.EntryFromDate.GetValueOrDefault().Date);
            }
            else if (filter.EntryToDate != null)
            {
                var toDate = filter.EntryToDate.GetValueOrDefault().Date.AddDays(1).AddMilliseconds(-1);
                yield return Query.LTE("BookedDate", toDate);
            }
        }

        public List<TransactionDocument> GetPotentialDuplicates(string ledgerId = null, string userId = null)
        {
            var query = Query.And(Query.EQ("Imported", true), Query.EQ("ConfirmedNotDuplicate", false));

            if (!string.IsNullOrEmpty(ledgerId))
            {
                query = Query.And(query, Query.EQ("LedgerId", ledgerId));
            }

            if (!string.IsNullOrEmpty(userId))
            {
                query = Query.And(query, Query.EQ("UserId", userId));
            }

            return GetByQuery(query);
        }
    }
}
