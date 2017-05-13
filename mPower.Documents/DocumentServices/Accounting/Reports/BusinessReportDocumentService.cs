using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Utils;

namespace mPower.Documents.DocumentServices.Accounting.Reports
{
    public class BusinessReportDocumentService
    {
        private readonly EntryDocumentService _entryService;

        public BusinessReportDocumentService(EntryDocumentService entryService)
        {
            _entryService = entryService;
        }

        #region Profit & Loss/ Balance Sheet

        private const string _accountBalanceByDayMap =
        @"function Map() {
           var key = {id: this.AccountId, date: this.BookedDateString};
           emit(key, {creditInCents: this.CreditAmountInCents, debitInCents:this.DebitAmountInCents});  
        }";

        private const string _acccountBalanceByDayReduce =
        @"function Reduce(key, values) {
            var result = {creditInCents: 0, debitInCents: 0};
              
            values.forEach(function(val) {
            result.creditInCents += val.creditInCents;
            result.debitInCents += val.debitInCents;
            });
  
            return result;
        }";

        public virtual List<LedgerAccountBalanceByDay> GetProfitLossReportData(DateTime? fromDate, DateTime? toDate, LedgerDocument ledger)
        {
            var accounts = ledger.Accounts.Where(x => x.TypeEnum == AccountTypeEnum.Expense || x.TypeEnum == AccountTypeEnum.Income);

            return GetAccountsBalanceByDay(accounts, fromDate, toDate, ledger.Id);
        }

        public virtual List<LedgerAccountBalanceByDay> GetBalanceSheetReportData(DateTime? fromDate, DateTime? toDate, LedgerDocument ledger)
        {
            var accounts = ledger.Accounts.ToList();

            return GetAccountsBalanceByDay(accounts, fromDate, toDate, ledger.Id);
        }

        public virtual long GetIncomeForLastMonth(LedgerDocument ledger)
        {
            long result = 0;

            var incomeAccounts = ledger.Accounts.Where(x => x.TypeEnum == AccountTypeEnum.Income);
            var incomeAggreagatedByDay = GetAccountsBalanceByDay(incomeAccounts, DateUtil.GetStartOfLastMonth(), DateUtil.GetEndOfLastMonth(), ledger.Id);
            CalculateBalance(incomeAggreagatedByDay, ref result);

            return result;
        }

        public virtual long GetLedgerDebtsBalanceForLastMonth(LedgerDocument ledger)
        {

            long result = 0;

            var debtAccounts = ledger.Accounts.Where(
                    x => (x.LabelEnum == AccountLabelEnum.CreditCard || x.LabelEnum == AccountLabelEnum.Loan)
                        && x.Denormalized.Balance > 0);
            var debts = GetAccountsBalanceByDay(debtAccounts, DateUtil.GetStartOfLastMonth(), DateUtil.GetEndOfLastMonth(), ledger.Id);

            CalculateBalance(debts, ref result);

            return result;
        }

        public virtual long CalculateMonthBalance(IEnumerable<AccountDocument> accounts, int month, int year)
        {


            return 0;
        }

        private void CalculateBalance(IEnumerable<LedgerAccountBalanceByDay> items, ref long amount)
        {
            foreach (var ledgerAccountBalanceByDay in items)
            {
                amount += ledgerAccountBalanceByDay.AmountPerDay.Sum(t => t.Amount);

                if (ledgerAccountBalanceByDay.SubAccounts != null && ledgerAccountBalanceByDay.SubAccounts.Count > 0)
                {
                    CalculateBalance(ledgerAccountBalanceByDay.SubAccounts, ref amount);
                }
            }
        }

        internal List<LedgerAccountBalanceByDay> GetAccountsBalanceByDay(IEnumerable<AccountDocument> accounts, DateTime? fromDate, DateTime? toDate, string ledgerId)
        {
            var accountsAggregatedByDay = new List<LedgerAccountBalanceByDay>();

            var accountsIds = new List<string>(accounts.Select(x => x.Id));

            var query = Query.And(Query.In("AccountId", BsonArray.Create(accountsIds)), Query.EQ("LedgerId", ledgerId));
            if (fromDate != null && toDate != null)
            {
                var toDateEnd = toDate.GetValueOrDefault().Date.AddDays(1).AddMilliseconds(-1);
                query = Query.And(query,
                    Query.GTE("BookedDate", fromDate.GetValueOrDefault()),
                    Query.LTE("BookedDate", toDateEnd));
            }

            var result = _entryService.MapReduce(query, BsonJavaScript.Create(_accountBalanceByDayMap),
                                                             BsonJavaScript.Create(_acccountBalanceByDayReduce), MapReduceOptions.SetOutput(MapReduceOutput.Inline));
            foreach (var item in result.InlineResults)
            {
                var group = item["_id"].AsBsonDocument;
                var accountId = group["id"].AsString;
                var dateString = group["date"].AsString;

                var date = DateTime.ParseExact(dateString, "MM-dd-yyyy", null);

                var account = accounts.FirstOrDefault(x => x.Id == accountId);
                if (account != null)
                {
                    var doc = item["value"].AsBsonDocument;
                    var debit = Int64.Parse(doc["debitInCents"].ToString());
                    var credit = Int64.Parse(doc["creditInCents"].ToString());
                    var amount = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(debit, credit, account.TypeEnum);

                    var accountItem = accountsAggregatedByDay.FirstOrDefault(x => x.AccountId == account.Id);
                    if (string.IsNullOrEmpty(account.ParentAccountId)) // parent doesn't exist
                    {
                        if (accountItem == null)
                        {
                            accountItem = CreateAndAddToList(account, accountsAggregatedByDay);
                        }
                        accountItem.AmountPerDay.Add(new DateAmount {Amount = amount, Date = date});
                    }
                    else
                    {
                        //this is parent account
                        accountItem = accountsAggregatedByDay.FirstOrDefault(x => x.AccountId == account.ParentAccountId);
                        if (accountItem == null)
                        {
                            var parentAccount = accounts.Single(x => x.Id == account.ParentAccountId);
                            accountItem = CreateAndAddToList(parentAccount.Id, parentAccount.Name, parentAccount.TypeEnum, account.Order, accountsAggregatedByDay);
                        }

                        var subAccount = accountItem.SubAccounts.FirstOrDefault(x => x.AccountId == account.Id) 
                            ?? CreateAndAddToList(account, accountItem.SubAccounts);

                        subAccount.AmountPerDay.Add(new DateAmount {Amount = amount, Date = date});
                    }
                }
            }
            return accountsAggregatedByDay;
        }

        private static LedgerAccountBalanceByDay CreateAndAddToList(AccountDocument account, List<LedgerAccountBalanceByDay> list)
        {
            return CreateAndAddToList(account.Id, account.Name, account.TypeEnum, account.Order, list);
        }

        private static LedgerAccountBalanceByDay CreateAndAddToList(string id, string name, AccountTypeEnum type, int order, List<LedgerAccountBalanceByDay> list)
        {
            var result = new LedgerAccountBalanceByDay
            {
                AccountId = id,
                AccountType = type,
                Name = name,
                Order = order,
            };
            list.Add(result);
            return result;
        }

        #endregion

        #region Transaction Detail

        private const string _accountBalanceMap =
        @"function Map() {
            emit({AccountId: this.AccountId, Type: this.AccountType}, {creditInCents: this.CreditAmountInCents, debitInCents: this.DebitAmountInCents});
        }";

        private const string _acccountBalanceReduce =
        @"function Reduce(key, values) {
            var credit = 0;
            var debit = 0;
          
            values.forEach(function(val) {
	            credit += val.creditInCents;
	            debit += val.debitInCents;
            });
          
            return {creditInCents: credit, debitInCents: debit};
        }";

        public long GetAccountBalance(string ledgerId, string accountId, DateTime? fromDate, DateTime? toDate)
        {
            long balance = 0;

            var query = Query.And(BuildAccountBalanceQuery(ledgerId, accountId, fromDate, toDate).ToArray());

            var result = _entryService.MapReduce(query, BsonJavaScript.Create(_accountBalanceMap),
                                                             BsonJavaScript.Create(_acccountBalanceReduce), MapReduceOptions.SetOutput(MapReduceOutput.Inline));

            foreach (var item in result.InlineResults)
            {
                var id = item["_id"].AsBsonDocument["AccountId"].AsString;
                var accountType = (AccountTypeEnum)((int)(item["_id"].AsBsonDocument["Type"].AsDouble));

                if (id == accountId)
                {
                    var doc = item["value"].AsBsonDocument;
                    var debit = Int64.Parse(doc["debitInCents"].ToString());
                    var credit = Int64.Parse(doc["creditInCents"].ToString());
                    balance = AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(debit, credit, accountType); 
                }
            }

            return balance;
        }

        private static IEnumerable<IMongoQuery> BuildAccountBalanceQuery(string ledgerId, string accountId, DateTime? fromDate, DateTime? toDate)
        {
            yield return Query.And(Query.EQ("AccountId", accountId), Query.EQ("LedgerId", ledgerId));
            if (fromDate != null && toDate != null)
            {
                yield return Query.And(Query.GTE("BookedDate", fromDate.GetValueOrDefault()), Query.LTE("BookedDate", toDate.GetValueOrDefault()));
            }
            else if (fromDate != null)
            {
                yield return Query.GTE("BookedDate", fromDate.GetValueOrDefault());
            }
            else if (toDate != null)
            {
                yield return Query.LTE("BookedDate", toDate.GetValueOrDefault());
            }
        }

        #endregion
    }
}
