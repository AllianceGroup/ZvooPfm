using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Services;
using System;

namespace mPower.Documents.DocumentServices.Accounting
{
    public class BudgetDocumentService : BaseDocumentService<BudgetDocument, BudgetFilter>
    {
        public BudgetDocumentService(MongoRead mongo)
            : base(mongo)
        {

        }

        protected override MongoCollection Items
        {
            get { return _read.Budgets; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(BudgetFilter filter)
        {         
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerId", filter.LedgerId);
            }
            if (!string.IsNullOrEmpty(filter.AccountId))
            {
                yield return Query.EQ("AccountId", filter.AccountId);
            }
            if (filter.Year.HasValue)
            {
                yield return Query.EQ("Year", filter.Year);
            }
            if (filter.Month.HasValue)
            {
                yield return Query.EQ("Month", filter.Month);
            }
            if (filter.YearPlusMonthFrom.HasValue && filter.YearPlusMonthTo.HasValue)
            {
                yield return Query.And(Query.GTE("YearPlusMonth", filter.YearPlusMonthFrom),Query.LTE("YearPlusMonth",filter.YearPlusMonthTo));
            }
            else if (filter.YearPlusMonthFrom.HasValue)
            {
                yield return Query.GTE("YearPlusMonth", filter.YearPlusMonthFrom);
            }
            else if (filter.YearPlusMonthTo.HasValue)
            {
                yield return Query.LTE("YearPlusMonth", filter.YearPlusMonthTo);
            }
            if (filter.AccountType.HasValue)
            {
                yield return Query.EQ("AccountType", (int)filter.AccountType);
            }
        }

        public virtual List<BudgetDocument> GetLedgetBudgetsByMonthAndYear(int month, int year, string ledgerId)
        {
            return GetByFilter(new BudgetFilter {Year = year, Month = month, LedgerId = ledgerId}).ToList();
        }

        public virtual List<BudgetDocument> GetNearestBudgets(int month, int year, string ledgerId)
        {
            var isPreviousMonth = ((year * 12 + month) < (DateTime.Now.Year * 12 + DateTime.Now.Month));

            var query = Query.And(Query.EQ("LedgerId", ledgerId));

            var sortBy = isPreviousMonth 
                ? SortBy.Ascending("Year", "Month") 
                : SortBy.Descending("Year", "Month");

            //for now just taking budget with min date if date/month < current
            //or budget with max date if date/month > current
            var budget = Items.FindAs<BudgetDocument>(query).SetSortOrder(sortBy).SetLimit(1).Single();

            return GetLedgetBudgetsByMonthAndYear(budget.Month, budget.Year, ledgerId);
        }

        public virtual List<BudgetDocument> GetHalfYearToDateBudgets(string ledgerId, DateTime date)
        {
            var yearPlusMonth = date.Year*12 + date.Month;

            var query = Query.And(Query.EQ("LedgerId", ledgerId),
                                  Query.GTE("YearPlusMonth", yearPlusMonth-5),
                                  Query.LTE("YearPlusMonth", yearPlusMonth),
                                  Query.In("AccountType", new[] { AccountTypeEnum.Income, AccountTypeEnum.Expense }.Select(x=> BsonValue.Create(x))));

            return GetByQuery(query);
        }
    }
}
