using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Services;
using mPower.Domain.Accounting;

namespace mPower.Documents.ExternalServices.FullTextSearch
{
    public class EntryLuceneFilter : BaseFilter
    {
        public EntryLuceneFilter()
        {
            AccountLabels = new List<AccountLabelEnum>();
            AccountIds = new List<string>();
        }

        public DateTime? BookedDateMinValue { get; set; }

        public DateTime? BookedDateMaxValue { get; set; }

        public long? MinEntryAmount { get; set; }

        public long? MaxEntryAmount { get; set; }

        public List<AccountLabelEnum> AccountLabels { get; set; }

        public string LedgerId { get; set; }

        public string AccountId { get; set; }
        public List<string> AccountIds { get; set; }
        public AccountTypeEnum? AccountType { get; set; }

        public string TransactionId { get; set; }

        public string SearchText { get; set; }

        public void ToLower()
        {
            if (!String.IsNullOrEmpty(LedgerId))
                LedgerId = LedgerId.ToLower();
            if (!String.IsNullOrEmpty(AccountId))
                AccountId = AccountId.ToLower();
            if (!String.IsNullOrEmpty(TransactionId))
                TransactionId = TransactionId.ToLower();
            if (!String.IsNullOrEmpty(SearchText))
                SearchText = SearchText.ToLower();
            if (AccountIds != null)
            {
                for (int i = 0; i < AccountIds.Count; i++)
                {
                    AccountIds[i] = AccountIds[i].ToLower();
                }
            }
        }
    }

    public class TransactionLuceneService : LuceneIndexService<EntryDocument>
    {
        public TransactionLuceneService(MPowerSettings settings)
            : base(settings)
        {
            base.SetIndexName("Transactions");
            NameOfIdField = "TransactionId";
        }

        public virtual List<EntryDocument> SearchByQuery(EntryLuceneFilter filter)
        {
            filter.ToLower();
            var filterQueries = new List<Query>();
            var queries = new List<Query>();

            if (filter.BookedDateMaxValue.HasValue || filter.BookedDateMinValue.HasValue)
            {
                queries.Add(BuildDateRangeQuery("BookedDateFilter", filter.BookedDateMinValue, filter.BookedDateMaxValue));
            }

            if (filter.MaxEntryAmount.HasValue || filter.MinEntryAmount.HasValue)
            {
                queries.Add(BuildNumericRangeQuery("EntryBalance", filter.MinEntryAmount, filter.MaxEntryAmount));
            }

            if (!String.IsNullOrEmpty(filter.SearchText))
            {
                var words = filter.SearchText.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                queries.Add(JoinQueriesOr(
                    BuildPrefixQueryAnd("Memo", words),
                    BuildPrefixQueryAnd("Payee", words),
                    BuildPrefixQueryAnd("AccountName", words),
                    BuildPrefixQueryAnd("OffsetAccountName", words),
                    BuildPrefixQuery("AccountLabel", filter.SearchText),
                    BuildPrefixQuery("AccountType", filter.SearchText),
                    BuildPrefixQuery("ReferenceNumber", filter.SearchText)));
            }

            if (!String.IsNullOrEmpty(filter.LedgerId))
            {
                filterQueries.Add(BuildMatchQuery("LedgerId", filter.LedgerId));
            }

            if (filter.AccountLabels.Count > 0)
            {
                filterQueries.Add(JoinQueriesOr(filter.AccountLabels.Select(label => BuildMatchQuery("AccountLabel", label.ToString().ToLower())).ToArray()));
            }

            if (!String.IsNullOrEmpty(filter.AccountId))
            {
                filterQueries.Add(JoinQueriesOr(BuildMatchQuery("AccountId", filter.AccountId), BuildMatchQuery("OffsetAccountId", filter.AccountId)));
            }
            else if (filter.AccountIds != null && filter.AccountIds.Count > 0)
            {
                var accountsQuries = new List<Query>();

                foreach (var id in filter.AccountIds)
                {
                    accountsQuries.Add(BuildMatchQuery("AccountId", id));
                    accountsQuries.Add(BuildMatchQuery("OffsetAccountId", id));
                }
                filterQueries.Add(JoinQueriesOr(accountsQuries.ToArray()));
            }

            if (filter.AccountType.HasValue)
            {
                filterQueries.Add(BuildMatchQuery("AccountType", filter.AccountType.Value.ToString().ToLower()));
            }

            if (!String.IsNullOrEmpty(filter.TransactionId))
            {
                filterQueries.Add(BuildMatchQuery("TransactionId", filter.TransactionId));
            }

            var filterQuery = new BooleanQuery();

            foreach (var query in filterQueries)
            {
                filterQuery.Add(query, BooleanClause.Occur.MUST);
            }

            //Exclude Opening Balance Equity transactions from results

            //very important to check if filter is empty, otherwise lucene will not search.
            Filter queryFilter = filterQuery.GetClauses().Count() == 0 ? null : new QueryWrapperFilter(filterQuery);

            return Search(queries.Count == 0 ? new MatchAllDocsQuery() : JoinQueriesAnd(queries.ToArray()),
                queryFilter,
                new Sort(new SortField("BookedDate", SortField.LONG, true)),
                filter.PagingInfo).Where(IsValidEntry).ToList();
        }

        public override void Insert(params EntryDocument[] documents)
        {
            base.Insert(documents.Where(IsValidEntry).ToArray());
        }

        public new void Delete(params string[] ids)
        {
            base.Delete(ids);
        }

        public override void Update(params EntryDocument[] documents)
        {
            base.Update(documents.Where(IsValidEntry).ToArray());
        }

        public bool IsValidEntry(EntryDocument entry)
        {
            if (entry == null)
                return false;

            //we need only these types of events
            return (entry.AccountLabel == AccountLabelEnum.Bank ||
                   entry.AccountLabel == AccountLabelEnum.CreditCard ||
                   entry.AccountLabel == AccountLabelEnum.Loan);
        }

        protected override Document MapToLucene(EntryDocument entry)
        {
            var doc = new Document();
            doc.Add(new Field("_id", entry.Id, Field.Store.YES, Field.Index.ANALYZED));
            if (entry.AccountName != null)
                doc.Add(new Field("AccountName", entry.AccountName, Field.Store.YES, Field.Index.ANALYZED));
            if (entry.Memo != null)
                doc.Add(new Field("Memo", entry.Memo, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));
            if (entry.Payee != null)
                doc.Add(new Field("Payee", entry.Payee, Field.Store.YES, Field.Index.ANALYZED, Field.TermVector.WITH_POSITIONS_OFFSETS));

            doc.Add(new Field("AccountType", entry.AccountType.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new NumericField("EntryBalance", 4, Field.Store.YES, true).SetLongValue(entry.EntryBalance));
            doc.Add(new NumericField("CreditAmountInCents", 4, Field.Store.YES, false).SetLongValue(entry.CreditAmountInCents));
            doc.Add(new NumericField("DebitAmountInCents", 4, Field.Store.YES, false).SetLongValue(entry.DebitAmountInCents));
            //field should not be analized to sort on it
            doc.Add(new Field("BookedDate", LuceneDateFormatter.ConvertToLucene(entry.BookedDate).ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new NumericField("BookedDateFilter", Field.Store.NO, true).SetLongValue(LuceneDateFormatter.ConvertToLucene(entry.BookedDate)));
            doc.Add(new Field("TransactionId", entry.TransactionId, Field.Store.YES, Field.Index.ANALYZED));
            //if (entry.FormattedAmountInDollars != null)
            //    doc.Add(new Field("FormattedAmountInDollars", entry.FormattedAmountInDollars, Field.Store.YES, Field.Index.NOT_ANALYZED));

            //Save calculated value in document, so the actual value will save
            if (entry.FormattedAmountInDollars != null)
                doc.Add(new Field("FormattedAmountInDollars", AccountingFormatter.ConvertToDollarsThenFormat((entry.DebitAmountInCents - entry.CreditAmountInCents), true), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("LedgerId", entry.LedgerId, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("AccountLabel", entry.AccountLabel.ToString(), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("AccountId", entry.AccountId, Field.Store.YES, Field.Index.ANALYZED));
            if (entry.OffsetAccountId != null)
                doc.Add(new Field("OffsetAccountId", entry.OffsetAccountId, Field.Store.YES, Field.Index.ANALYZED));
            if (entry.OffsetAccountName != null)
                doc.Add(new Field("OffsetAccountName", entry.OffsetAccountName, Field.Store.YES, Field.Index.ANALYZED));

            return doc;
        }

        protected override EntryDocument MapFromLucene(Document doc)
        {
            var entryDocument = new EntryDocument();
            entryDocument.Id = doc.Get("_id");
            entryDocument.AccountName = doc.Get("AccountName");
            entryDocument.Memo = doc.Get("Memo");
            entryDocument.Payee = doc.Get("Payee");
            entryDocument.AccountLabel = (AccountLabelEnum)Enum.Parse(typeof(AccountLabelEnum), doc.Get("AccountLabel"));
            entryDocument.AccountType = (AccountTypeEnum)Enum.Parse(typeof(AccountTypeEnum), doc.Get("AccountType"));
            entryDocument.CreditAmountInCents = long.Parse(doc.Get("CreditAmountInCents"));
            entryDocument.DebitAmountInCents = long.Parse(doc.Get("DebitAmountInCents"));
            entryDocument.BookedDate = LuceneDateFormatter.ConvertFromLucene(doc.Get("BookedDate"));
            entryDocument.TransactionId = doc.Get("TransactionId");
            //entryDocument.FormattedAmountInDollars = doc.Get("FormattedAmountInDollars");

            //Getting calculated value from document, so the actual value will display
            entryDocument.FormattedAmountInDollars = AccountingFormatter.ConvertToDollarsThenFormat(long.Parse(doc.Get("DebitAmountInCents")) - long.Parse(doc.Get("CreditAmountInCents")), true);
            entryDocument.LedgerId = doc.Get("LedgerId");
            entryDocument.AccountId = doc.Get("AccountId");
            entryDocument.OffsetAccountId = doc.Get("OffsetAccountId");
            entryDocument.OffsetAccountName = doc.Get("OffsetAccountName");

            return entryDocument;
        }
    }
}
