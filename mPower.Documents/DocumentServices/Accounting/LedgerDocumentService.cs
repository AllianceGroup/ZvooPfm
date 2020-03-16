using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;
using mPower.Framework.Services;
using System.Collections.Generic;
using System.Linq;
using MongoUpdate = MongoDB.Driver.Builders.Update;

namespace mPower.Documents.DocumentServices.Accounting
{
    /// <summary>
    /// Always returns native documents
    /// </summary>
    public class LedgerDocumentService : BaseDocumentService<LedgerDocument, LedgerFilter>
    {
        public LedgerDocumentService(MongoRead mongo)
            : base(mongo)
        {
        }

        protected override MongoCollection Items
        {
            get { return _read.Ledgers; }
        }

        protected override IEnumerable<IMongoQuery> BuildFilterQuery(LedgerFilter filter)
        {
            

            if (!string.IsNullOrEmpty(filter.AccountId))
            {
                yield return Query.EQ("Accounts._id", filter.AccountId);
            }

            if (!string.IsNullOrEmpty(filter.UserId))
            {
                yield return Query.EQ("Users._id", filter.UserId);
            }
            if (filter.AccountFilter != null)
            {
                if (filter.AccountFilter.AggregationStartedDateLessThan.HasValue)
                {
                    yield return Query.LT("Accounts.AggregationStartedDate", filter.AccountFilter.AggregationStartedDateLessThan);
                }
                if (filter.AccountFilter.AggregationsStatusIn != null)
                {
                    yield return Query.In("Accounts.AggregatedAccountStatus",new BsonArray(filter.AccountFilter.AggregationsStatusIn));
                }
            }
        }

        public List<LedgerDocument> GetByUserId(string userId)
        {
            return GetByFilter(new LedgerFilter {UserId = userId});

        }

        public LedgerDocument GetPersonal(string userId)
        {
            return GetByUserId(userId).SingleOrDefault(x => x.TypeEnum == LedgerTypeEnum.Personal);
        }

        public IEnumerable<AccountDocument> GetAccountsByFilter(AccountFilter accountFilter)
        {
            var ledgers = GetByFilter(new LedgerFilter {AccountFilter = accountFilter});
            var accounts =
                ledgers.SelectMany(x => x.Accounts).Where(
                    x =>
                    x.AggregationStartedDate < accountFilter.AggregationStartedDateLessThan.Value &&
                    accountFilter.AggregationsStatusIn.Contains(x.AggregatedAccountStatus));
            return accounts;
        }

        public void SetAccountBalance(string ledgerId, string accountId, long balanceInCents)
        {
            var query = Query.And(Query.EQ("_id", ledgerId), Query.EQ("Accounts._id", accountId));
            var update = MongoDB.Driver.Builders.Update
                .Set("Accounts.$.Denormalized.Balance", balanceInCents);
            Update(query, update);
        }

        public void ArchiveAccount(string ledgerId, string accountId, string reason)
        {
            var query = Query.And(Query.EQ("_id", ledgerId), Query.EQ("Accounts._id", accountId));
            var update = MongoDB.Driver.Builders.Update
                .Set("Accounts.$.ReasonToArchive", BsonValue.Create(reason) ?? BsonNull.Value)
                .Set("Accounts.$.Archived", true);
            Update(query, update);
        }

        public void AddAccount(string ledgerId, AccountDocument account)
        {
            var query = Query.EQ("_id", ledgerId);
            var update = MongoDB.Driver.Builders.Update
                .Push("Accounts", account.ToBsonDocument());
            Update(query, update);
        }

        public void RemoveAccount(string ledgerId, string accountId)
        {
            var query = Query.And(Query.EQ("_id", ledgerId));
            var update = MongoDB.Driver.Builders.Update.Pull("Accounts", Query.EQ("_id", accountId));
            Update(query, update);
        }

        public void UpdateAccount(Ledger_Account_UpdatedEvent message)
        {
            var query = Query.And(Query.EQ("_id", message.LedgerId), Query.EQ("Accounts._id", message.AccountId));
            var update = MongoDB.Driver.Builders.Update
                .Set("Accounts.$.ParentAccountId", BsonValue.Create(message.ParentAccountId) ?? BsonNull.Value)
                .Set("Accounts.$.Description", BsonValue.Create(message.Description) ?? BsonNull.Value)
                .Set("Accounts.$.Name", BsonValue.Create(message.Name) ?? BsonNull.Value)
                .Set("Accounts.$.Number", BsonValue.Create(message.Number) ?? BsonNull.Value)
                .Set("Accounts.$.InstitutionName", BsonValue.Create(message.InstitutionName) ?? BsonNull.Value)
                .Set("Accounts.$.InterestRatePerc", message.InterestRatePerc)
                .Set("Accounts.$.MinMonthPaymentInCents", message.MinMonthPaymentInCents)
                .Set("Accounts.$.CreditLimitInCents", message.CreditLimitInCents);
            Update(query, update);
        }

        public void AddUser(string ledgerId, LedgerUserDocument userDocument)
        {
            var query = Query.EQ("_id", ledgerId);
            Update(query, MongoDB.Driver.Builders.Update.AddToSet("Users", userDocument.ToBsonDocument())); 
        }

        public void RemoveUser(string ledgerId, string userId)
        {
            var query = Query.EQ("_id", ledgerId);
            Update(query, MongoUpdate.Pull("Users", Query.EQ("_id", userId)));
        }

        public void UpdateAccountsOrder(string ledgerId, List<AccountOrderData> orders)
        {
            foreach (var account in orders)
            {
                var query = Query.And(Query.EQ("_id", ledgerId), Query.EQ("Accounts._id", account.Id));
                var update = MongoUpdate.Set("Accounts.$.Order", account.Order);
                Update(query, update);
            }
        }

        public void SetBudget(string ledgerId, List<BudgetData> budgets)
        {
            var query = Query.EQ("_id", ledgerId);
            var update = Update<LedgerDocument>.Set(x => x.IsBudgetSet, true);
            Update(query, update);
            foreach (var item in budgets)
            {
                Update(
                    Query.And(query, Query.EQ("Accounts._id", item.AccountId)),
                    MongoUpdate.Set("Accounts.$.IsBudgetSet", true));
            }
        }

        public void SetAggregatedBalance(string ledgerId, string ledgerAccountId, long newBalanceOfAggregatedAccount)
        {
            var query = Query.And(Query.EQ("_id", ledgerId), Query.EQ("Accounts._id", ledgerAccountId));
            var update = MongoUpdate.Set("Accounts.$.AggregatedBalance", newBalanceOfAggregatedAccount);
            Update(query, update);
        }

        public void SetAccountAggregationStatus(Ledger_Account_AggregationStatus_UpdatedEvent message)
        {
            var query = Query.EQ("_id", message.LedgerId);
            if (!string.IsNullOrEmpty(message.AccountId))
            {
                query =  Query.And(query, Query.EQ("Accounts._id", message.AccountId));
            }
            else
            {
                query = Query.And(query, Query.EQ("Accounts.IntuitAccountId", (long)message.IntuitAccountId));
            }

            var update = MongoUpdate
                .Set("Accounts.$.AggregatedAccountStatus", message.NewStatus)
                .Set("Accounts.$.AggregationExceptionId", BsonValue.Create(message.AggregationExceptionId) ?? BsonNull.Value);
            if (message.NewStatus != AggregatedAccountStatusEnum.PullingTransactions &&
                message.NewStatus != AggregatedAccountStatusEnum.BeginPullingTransactions)
            {
                update.Set("Accounts.$.DateLastAggregated", message.Date);
            }
            else
            {
                update.Set("Accounts.$.AggregationStartedDate", message.Date);
            }
            Update(query, update);
        }

        public void ChangeAccountName(string ledgerId, string accountId, string name)
        {
            var query = Query.And(Query.EQ("_id", ledgerId), Query.EQ("Accounts._id", accountId));
            var update = MongoUpdate.Set("Accounts.$.Name", BsonValue.Create(name) ?? BsonNull.Value);
            Update(query, update);
        }

        public void ChangeInterestRate(string ledgerId, string accountId, float interestRate)
        {
            var query = Query.And(Query.EQ("_id", ledgerId), Query.EQ("Accounts._id", accountId));
            var update = MongoUpdate.Set("Accounts.$.InterestRatePerc", interestRate);
            Update(query, update);
        }

        public void SetAggregatedDate(string ledgerId, string ledgerAccountId, System.DateTime date)
        {
            var query = Query.And(Query.EQ("_id", ledgerId), Query.EQ("Accounts._id", ledgerAccountId));
            var update = MongoUpdate.Set("Accounts.$.DateLastAggregated", date);
            Update(query, update);
        }
    }
}
