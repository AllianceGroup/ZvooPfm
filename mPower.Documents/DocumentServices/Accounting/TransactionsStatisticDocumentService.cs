using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Services;

namespace mPower.Documents.DocumentServices.Accounting
{
    public class TransactionsStatisticDocumentService : BaseDocumentService<TransactionsStatisticDocument, TransactionsStatisticFilter>
    {
        private readonly IIdGenerator _idGenerator;

        public TransactionsStatisticDocumentService(MongoRead mongo, IIdGenerator idGenerator)
            : base(mongo)
        {
            _idGenerator = idGenerator;
        }

        protected override MongoCollection Items
        {
            get { return _read.TransactionsStatistics; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(TransactionsStatisticFilter filter)
        {
           
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerId", filter.LedgerId);
            }

            if (!string.IsNullOrEmpty(filter.AccountId))
            {
                yield return Query.EQ("AccountId", filter.AccountId);
            }

            if (filter.AccountIds != null && filter.AccountIds.Count > 0)
            {
                yield return Query.In("AccountId", BsonArray.Create(filter.AccountIds));
            }

            if (filter.AccountType.HasValue)
            {
                yield return Query.EQ("AccountType", filter.AccountType);
            }

            if (filter.Year.HasValue)
            {
                yield return Query.EQ("Year", filter.Year);
            }

            if (filter.Month.HasValue)
            {
                yield return Query.EQ("Month", filter.Month);
            }
        }

        public void AddStatistic(string ledgerId, string accountId, AccountTypeEnum accountType, DateTime date, long debitInDollars, long creditInDollars)
        {
            var filter = new TransactionsStatisticFilter {LedgerId = ledgerId, AccountId = accountId, AccountType = accountType, Year = date.Year, Month = date.Month};
            var existingDocuments = GetByFilter(filter);

            if (existingDocuments.Count == 1)
            {
                var doc = existingDocuments[0];
                var query = Query.EQ("_id", doc.Id);
                var update = MongoDB.Driver.Builders.Update<TransactionsStatisticDocument>
                    .Set(x => x.DebitAmountInCents, doc.DebitAmountInCents + debitInDollars)
                    .Set(x => x.CreditAmountInCents, doc.CreditAmountInCents + creditInDollars);
                Update(query, update);
            }
            else
            {
                long prevDebit = 0, prevCredit = 0;
                if (existingDocuments.Count > 1)
                {
                    prevDebit = existingDocuments.Sum(tsd => tsd.DebitAmountInCents);
                    prevCredit = existingDocuments.Sum(tsd => tsd.CreditAmountInCents);
                    Remove(filter);
                }

                var documentForSave = new TransactionsStatisticDocument
                {
                    Id = _idGenerator.Generate(),
                    LedgerId = ledgerId,
                    AccountId = accountId,
                    AccountType = accountType,
                    Year = date.Year,
                    Month = date.Month,
                    DebitAmountInCents = prevDebit + debitInDollars,
                    CreditAmountInCents = prevCredit + creditInDollars,
                };
                Save(documentForSave);
            }
        }

        public List<TransactionsStatisticDocument> GetProfitLostStatistic(string ledgerId, DateTime date)
        {
            var query = Query.And(Query.EQ("LedgerId", ledgerId),
                                  Query.EQ("Year", date.Year),
                                  Query.LTE("Month", date.Month),
                                  Query<TransactionsStatisticDocument>.In(x => x.AccountType, new[] { AccountTypeEnum.Income, AccountTypeEnum.Expense }));
            return GetByQuery(query);
        }

        public double GetLastSixMonthsAvr(string ledgerId, string accountId, DateTime lastMonth)
        {
            var firstMonth = ((lastMonth >=  DateTime.MinValue.AddMonths(5)) ? lastMonth : DateTime.Now).AddMonths(-5);
            var dateQuery = Query.Or(
                Query.And(Query.EQ("Year", firstMonth.Year), Query.GTE("Month", firstMonth.Month)),
                Query.And(Query.EQ("Year", lastMonth.Year), Query.LTE("Month", lastMonth.Month)));
            var query = Query.And(Query.EQ("LedgerId", ledgerId), Query.EQ("AccountId", accountId), dateQuery);

            var statistics = GetByQuery(query);
            if (statistics.Count > 0)
            {
                return statistics.Average(s => AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(s.DebitAmountInCents, s.CreditAmountInCents, s.AccountType));
            }
            return 0;
        }
    }
}
