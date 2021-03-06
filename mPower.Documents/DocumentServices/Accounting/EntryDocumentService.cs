using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Framework;
using mPower.Framework.Services;
using System;
using MongoDB.Bson;

namespace mPower.Documents.DocumentServices.Accounting
{
    public class EntryDocumentService : BaseDocumentService<EntryDocument, EntryFilter>
    {
        private const double RANGE = 0.2;
        public EntryDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.Entries; }
        }

        protected override IMongoSortBy BuildSortExpression(EntryFilter filter)
        {
            var sortExpression = SortBy.Null;

            switch (filter.SortByFiled)
            {
                case EntrySortFieldEnum.BookedDate:
                    switch (filter.SortDirection)
                    {
                        case SortDirection.Descending:
                            sortExpression = SortBy.Descending("BookedDate");
                            break;
                        case SortDirection.Ascending:
                            sortExpression = SortBy.Ascending("BookedDate");
                            break;

                    }
                    break;
            }

            return sortExpression;
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(EntryFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerId", filter.LedgerId);
            }

            if (!string.IsNullOrEmpty(filter.AccountId))
            {
                yield return Query.EQ("AccountId", filter.AccountId);
            }

            if (!string.IsNullOrEmpty(filter.OffsetAccountId))
            {
                yield return Query.EQ("OffsetAccountId", filter.OffsetAccountId);
            }

            if (filter.AccountIds != null && filter.AccountIds.Count > 0)
            {
                yield return Query.In("AccountId", BsonArray.Create(filter.AccountIds));
            }

            if (!string.IsNullOrEmpty(filter.TransactionId))
            {
                yield return Query.EQ("TransactionId", filter.TransactionId);
            }
            if (filter.AccountLabel != null)
            {
                yield return Query.Or(filter.AccountLabel.Select(enums => Query.EQ("AccountLabel", enums)).ToArray());
            }
        }

        public DateTime GetLedgerMinTransactionDate(string ledgerId)
        {
            var item = GetByFilter(new EntryFilter
                                       {
                                           LedgerId = ledgerId,
                                           SortByFiled = EntrySortFieldEnum.BookedDate,
                                           SortDirection = SortDirection.Ascending,
                                           PagingInfo = new PagingInfo {Take = 1}
                                       }).FirstOrDefault();

            return item == null ? DateTime.MinValue : item.BookedDate;

        }

        public List<EntryDocument> GetFinantialInstituionAndNotAutogeneratedByFilter(EntryFilter filter)
        {
            return GetByFilter(filter).Where(TransactionGenerator.IsValidEntry).ToList();
        }

        public List<EntryDocument> GetAutogeneratedEntriesByFilter(EntryFilter filter)
        {
            return GetByFilter(filter).Where(e => !TransactionGenerator.IsValidEntry(e)).ToList();
        }
    }
}