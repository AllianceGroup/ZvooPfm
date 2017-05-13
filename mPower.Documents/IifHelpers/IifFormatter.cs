using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Framework.Utils.Extensions;

namespace mPower.Documents.IifHelpers
{
    public class IifFormatter
    {
        public static String LedgerToIifString(LedgerDocument ledger, List<TransactionDocument> transactions)
        {
            var sb = new StringBuilder();

            AccountsToIifString(sb, ledger.Accounts);
            TransactionsToIifString(sb, ledger.Accounts, transactions);

            return sb.ToString();
        }

        #region Accounts

        private static void AccountsToIifString(StringBuilder sb, List<AccountDocument> accountsList)
        {
            if (accountsList != null && accountsList.Count > 0)
            {
                const string key = IifConstants.Account.Key;
                var format = GetFormatString(4);
                sb.AppendLine(string.Format(IifConstants.HeaderSign + format, key, IifConstants.Account.Name, IifConstants.Account.Type, IifConstants.Account.Description));
                foreach (var account in accountsList)
                {
                    sb.AppendLine(string.Format(format, key, account.Name, account.LabelEnum.GetIifName(), account.Description));
                }
            }
        }

        #endregion

        #region Transactions

        private static void TransactionsToIifString(StringBuilder sb, IEnumerable<AccountDocument> accountsList, List<TransactionDocument> transactionsList)
        {
            if (transactionsList != null && transactionsList.Count > 0)
            {
                const string transKey = IifConstants.Tranaction.Key;
                const string splKey = IifConstants.Spl.Key;
                const string endKey = IifConstants.Tranaction.EndKey;


                //Building IIF Header
                var transFormat = GetFormatString(8);
                sb.AppendLine(string.Format(IifConstants.HeaderSign + transFormat, transKey, IifConstants.Tranaction.Id,
                                            IifConstants.Tranaction.Type, IifConstants.Tranaction.Date,
                                            IifConstants.Tranaction.AccountName, IifConstants.Tranaction.Amount,
                                            IifConstants.Tranaction.Name, IifConstants.Tranaction.Memo));

                var splFormat = GetFormatString(8);
                sb.AppendLine(string.Format(IifConstants.HeaderSign + splFormat, splKey, IifConstants.Spl.Id,
                                            IifConstants.Spl.Type, IifConstants.Spl.Date,
                                            IifConstants.Spl.AccountName, IifConstants.Spl.Amount,
                                            IifConstants.Spl.Name, IifConstants.Spl.Memo));

                sb.AppendLine(IifConstants.HeaderSign + endKey);


                var accounts = accountsList.ToDictionary(a => a.Id, a => a);

                var entryNum = 1;

                // entries should reference only to exported accounts
                var consistentTransactions = transactionsList.Where(t => t.Entries.All(e => accounts.ContainsKey(e.AccountId)));

                foreach (var transaction in consistentTransactions)
                {
                    var isFirst = true;

                    // transaction common info
                    var date = transaction.BookedDate.ToString(IifConstants.DateFormat);
                    var type = transaction.Type.GetIifName();

                    var orderedEntries = OrderEntries(transaction.Entries);
                    foreach (var entry in orderedEntries)
                    {
                        var accountName = accounts[entry.AccountId].Name;
                        var amountInDollars = AccountingFormatter.CentsToDollars(entry.DebitAmountInCents - entry.CreditAmountInCents);
                        sb.AppendLine(isFirst
                            // TRNS
                            ? string.Format(transFormat, transKey, entryNum++, type, date, accountName, amountInDollars, entry.Payee, entry.Memo)
                            // SPL
                            : string.Format(splFormat, splKey, entryNum++, type, date, accountName, amountInDollars, entry.Payee, entry.Memo));
                        
                        isFirst = false;
                    }
                    //ENDTRNS
                    sb.AppendLine(endKey);
                }
            }
        }

        #endregion

        #region Helper Methods

        private static string GetFormatString(int argsNum)
        {
            var result = string.Empty;
            for (var i = 0; i < argsNum; i++)
            {
                if (i != 0)
                {
                    result += IifConstants.Separator;
                }
                result += "{" + i + "}";
            }
            return result;
        }

        private static List<TransactionEntryDocument>  OrderEntries(List<TransactionEntryDocument> entries)
        {
            var orderedEntries = new List<TransactionEntryDocument>();

            var debitors = entries.Where(e => e.DebitAmountInCents > 0).ToList();
            var creditors = entries.Where(e => e.CreditAmountInCents > 0).ToList();

            if (debitors.Count == 1)
            {
                orderedEntries.AddRange(debitors);
                orderedEntries.AddRange(creditors);
            }
            else if (creditors.Count == 1)
            {
                orderedEntries.AddRange(creditors);
                orderedEntries.AddRange(debitors);
            }
            else
            {
                // should never happen
                return entries;
            }

            return orderedEntries;
        }

        #endregion
    }
}
