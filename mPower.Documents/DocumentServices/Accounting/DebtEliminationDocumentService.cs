using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Enums;
using mPower.Framework;
using mPower.Framework.Services;
using mPower.Documents.Documents.Accounting.DebtElimination;

namespace mPower.Documents.DocumentServices.Accounting
{
    public class DebtEliminationDocumentService : BaseDocumentService<DebtEliminationDocument, DebtEliminationFilter>
    {
        public DebtEliminationDocumentService(MongoRead mongo)
            : base(mongo)
        {

        }

        protected override MongoCollection Items
        {
            get { return _read.DebtElimintations; }
        }

        protected override IMongoSortBy BuildSortExpression(DebtEliminationFilter filter)
        {
            var sortExpression = SortBy.Null;

            return sortExpression;
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(DebtEliminationFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.LedgerId))
            {
                yield return Query.EQ("LedgerId", filter.LedgerId);
            }

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                yield return Query.EQ("UserId", filter.UserId);
            }
        }

        public DebtEliminationDocument GetDebtEliminationByUser(string ledgerId, string userId)
        {
            return GetByFilter(new DebtEliminationFilter() { LedgerId = ledgerId, UserId = userId }).FirstOrDefault();
        }
    }
}
