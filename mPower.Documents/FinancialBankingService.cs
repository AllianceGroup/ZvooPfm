using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;
using mPower.Framework.Services;
using System.Collections.Generic;
using System.Linq;
using MongoUpdate = MongoDB.Driver.Builders.Update;

namespace mPower.Documents
{
    /// <summary>
    /// Always returns native documents
    /// </summary>
    public class FinancialBankingService : BaseDocumentService<MarketHistoryDocument, CalculatorFilters>
    {
        public FinancialBankingService(MongoRead mongo)
            : base(mongo)
        {
        }
      
        protected override MongoCollection Items
        {
            get { return _read.MarketHistoryData; }
        }

        protected override IMongoSortBy BuildSortExpression(CalculatorFilters filter)
        {
            var sortExpression = SortBy.Null;

            return sortExpression;
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(CalculatorFilters filter)
        {
            if (!string.IsNullOrEmpty(filter._Id))
            {
                yield return Query.EQ("_Id", filter._Id);
            }

            if (!string.IsNullOrEmpty(filter.Year))
            {
                yield return Query.EQ("Year", filter.Year);
            }
        }

        


    }
}
