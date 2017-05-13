using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using Paralect.ServiceBus;
using com.yodlee.common;
using com.yodlee.core.dataservice;
using com.yodlee.core.dataservice.types;
using com.yodlee.soap.core.dataservice;
using mPower.Domain.Yodlee.ContentServiceItem.Data;
using mPower.Domain.Yodlee.Enums;
using mPower.Domain.Yodlee.Services;
using mPower.Domain.Yodlee.Storage.Documents;
using mPower.Framework;
using ContentServiceItemAccount = mPower.Domain.Yodlee.ContentServiceItem.Data.ContentServiceItemAccount;
using ContentServiceItemAccountTransaction = mPower.Domain.Yodlee.ContentServiceItem.Data.ContentServiceItemAccountTransaction;
using mPower.Domain.Accounting.Enums;

namespace mPower.Domain.Yodlee.ContentServiceItem.Commands
{
    public class ContentServiceItem_DownloadCommandHandler : BaseYodleeService, IMessageHandler<ContentServiceItem_DownloadCommand>
    {
        private readonly ContentServiceItemDocumentService _docService;
        private static readonly Logger _logger = MPowerLogManager.CurrentLogger;


        public ContentServiceItem_DownloadCommandHandler(ContentServiceItemDocumentService docService)
        {
            _docService = docService;
        }

        public void Handle(ContentServiceItem_DownloadCommand message)
        {
            ConnectToYodlee();
            var user = LoginUser(message.LoginName, message.Password);

            var dataService = new DataServiceService();
            // itemSummary is all of the information for a specific account login including transactions.  This may contain multiple accounts
            var itemSummary = dataService.getItemSummaryForItem(user.userContext, message.ItemId,
                                                                new DataExtent { startLevel = 0, endLevel = int.MaxValue });
            
            if (itemSummary == null)
                return;
            
            var data = Map(itemSummary, message.UserId);
            
            var existingContentServiceItem = _docService.GetById(message.ItemId.ToString(System.Globalization.CultureInfo.InvariantCulture));

            if (existingContentServiceItem == null)
                CreateContentServiceItem(data, message.AuthenticationReferenceId);
            else
                UpdateContentServiceItem(data);

        }

        #region Mapping

        private ContentServiceItemData Map(ItemSummary itemSummary, string userId)
        {
            var document = new ContentServiceItemData
                               {
                                   ItemId = itemSummary.itemId.ToString(),
                                   ContentServiceId = itemSummary.contentServiceId.ToString(),
                                   DisplayName = itemSummary.itemDisplayName,
                                   LastSuccessfulUpdate = itemSummary.refreshInfo.lastSuccessfulDataUpdate,
                                   LastUpdateAttempt = itemSummary.refreshInfo.lastDataUpdateAttempt.date,
                                   AccessStatus = (ItemAccessStatusEnum)itemSummary.refreshInfo._itemAccessStatusEnum,
                                   DataUpdateAttemptStatus = (DataUpdateAttemptStatus)itemSummary.refreshInfo.lastDataUpdateAttempt._statusEnum,
                                   UserId = userId,
                                   ResponseCode = (ResponseCodeEnum)itemSummary.refreshInfo._responseCodeTypeEnum,
                                   ActionRequired = (UserActionRequiredEnum)itemSummary.refreshInfo._userActionRequiredTypeEnum,
                                   ErrorCode = (ContentServiceRefreshErrorCode)itemSummary.refreshInfo.statusCode
                               };

            switch (itemSummary.contentServiceInfo.containerInfo.containerName.ToLower())
            {
                case "bank":
                    document.Accounts = itemSummary.itemData == null ? null : itemSummary.itemData.accounts.Cast<BankData>().Select(Map).ToList();
                    break;
                case "credits":
                    document.Accounts = itemSummary.itemData == null
                                            ? null
                                            : itemSummary.itemData.accounts.Cast<CardData>().Select(Map).ToList();
                    break;
                case "stocks":
                    document.Accounts = itemSummary.itemData == null
                                            ? null
                                            : itemSummary.itemData.accounts.Cast<InvestmentData>().Select(Map).ToList();
                    break;
                case "mortgage":
                    document.Accounts = itemSummary.itemData == null
                                            ? null
                                            : Map(itemSummary.itemData.accounts.Cast<LoanLoginAccountData>().ToList());
                    break;
                case "loans":

                    document.Accounts = itemSummary.itemData == null
                                            ? null
                                            : Map(itemSummary.itemData.accounts.Cast<LoanLoginAccountData>().ToList());
                    break;
            }
            return document;
        }

        #region Bank

        private ContentServiceItemAccount Map(BankData data)
        {
            return new ContentServiceItemAccount
                       {
                           AccountId = data.accountId.ToString(),
                           ItemAccountId = data.itemAccountId.ToString(),
                           AccountType = data.acctType,
                           AccountName = data.accountName,
                           AccountNumber = data.accountNumber,
                           BankAccountId = data.bankAccountId.ToString(),
                           Transactions =
                               data.bankTransactions == null
                                   ? null
                                   : data.bankTransactions.Cast<BankTransactionData>().Select(Map).OrderBy(x => x.Date).ToList(),
                           AccountLabelEnum = AccountLabelEnum.Bank,
                           AvailableBalanceInCents = data.availableBalance == null ? 000 : Convert.ToInt64(data.availableBalance.amount * 100),
                           CurrentBalanceInCents = Convert.ToInt64(data.currentBalance.amount * 100),



                       };
        }

        private ContentServiceItemAccountTransaction Map(BankTransactionData data)
        {

            var validDate = data.postDate.date.Year == 0001 ? data.transactionDate.date : data.postDate.date;

            return new ContentServiceItemAccountTransaction
                       {
                           Status = data.transactionStatus,
                           Type = data.transactionBaseType,
                           BankTransactionId = data.bankTransactionId.ToString(),
                           Date = validDate,
                           Description = data.plainTextDescription,
                           Amount = data.transactionAmount.amount,
                           CurrencyCode = data.transactionAmount.currencyCode,
                           CategorizationKeyword = data.categorizationKeyword,
                           TransactionType = data.transactionBaseType == "credit" ? TransactionType.Deposit : TransactionType.Check
                       };
        }

        #endregion

        #region Credit Card
        private ContentServiceItemAccount Map(CardData data)
        {
            return new ContentServiceItemAccount
                       {
                           AccountId = data.accountId.ToString(),
                           ItemAccountId = data.itemAccountId.ToString(),
                           AccountType = data.acctType,
                           AccountName = data.accountName,
                           AccountNumber = data.accountNumber,
                           BankAccountId = data.cardAccountId.ToString(),
                           Transactions =
                               data.cardTransactions == null
                                   ? null
                                   : data.cardTransactions.Cast<CardTransactionData>().Select(Map).OrderBy(x => x.Date).ToList(),
                           AccountLabelEnum = AccountLabelEnum.CreditCard,
                           AvailableBalanceInCents = data.availableCredit == null ? 000 : Convert.ToInt64(data.availableCredit.amount * 100),
                           CurrentBalanceInCents = Convert.ToInt64(data.runningBalance.amount * 100),

                       };
        }

        private ContentServiceItemAccountTransaction Map(CardTransactionData data)
        {
            var validDate = data.postDate.date.Year == 0001 ? data.transDate.date : data.postDate.date;

            return new ContentServiceItemAccountTransaction
                       {
                           Status = data.transactionStatus,
                           Type = data.transactionBaseType,
                           BankTransactionId = data.cardTransactionId.ToString(),
                           Date = validDate,
                           Description = data.plainTextDescription,
                           Amount = data.transAmount.amount,
                           CurrencyCode = data.transAmount.currencyCode,
                           CategorizationKeyword = data.categorizationKeyword,
                           TransactionType = data.transactionBaseType == "debit" ? TransactionType.CreditCard : TransactionType.Check
                       };
        }

        #endregion

        #region Investment
        private ContentServiceItemAccount Map(InvestmentData data)
        {
            return new ContentServiceItemAccount
                       {
                           AccountId = data.accountId.ToString(),
                           ItemAccountId = data.itemAccountId.ToString(),
                           AccountType = data.acctType,
                           AccountName = data.accountName,
                           AccountNumber = data.accountNumber,
                           BankAccountId = data.investmentAccountId.ToString(),
                           Transactions =
                               data.investmentTransactions == null
                                   ? null
                                   : data.investmentTransactions.Cast<InvestmentTransactionsData>().Select(Map).ToList(),
                           AccountLabelEnum = AccountLabelEnum.Investment,
                           AvailableBalanceInCents = data.totalUnvestedBalance == null ? 000 : Convert.ToInt64(data.totalUnvestedBalance.amount * 100),
                           CurrentBalanceInCents = Convert.ToInt64(data.totalBalance.amount * 100)
                       };
        }

        private ContentServiceItemAccountTransaction Map(InvestmentTransactionsData data)
        {

            var validDate = data.transDate.date.Year == 0001 ? data.settleDate.date : data.transDate.date;

            return new ContentServiceItemAccountTransaction
                       {
                           Status = data.transactionStatus,
                           Type = data.transactionBaseType,
                           BankTransactionId = data.investmentTransactionId.ToString(),
                           Date = validDate,
                           Description = data.plainTextDescription,
                           Amount = data.amount.amount,
                           CurrencyCode = data.amount.currencyCode,
                           CategorizationKeyword = data.categorizationKeyword,
                           TransactionType = data.transactionBaseType == "credit" ? TransactionType.Check : TransactionType.Transfer
                       };
        }

        #endregion

        #region Loans & Mortgages


        private List<ContentServiceItemAccount> Map(IEnumerable<LoanLoginAccountData> data)
        {
            var accounts = new List<ContentServiceItemAccount>();

            foreach (var loan in data)
            {
                accounts.AddRange(loan.loans.Cast<Loan>().Select(Map));
            }

            return accounts;
        }

        private ContentServiceItemAccount Map(Loan data)
        {
            return new ContentServiceItemAccount
                       {
                           AccountId = data.accountId.ToString(),
                           ItemAccountId = data.itemAccountId.ToString(),
                           AccountType = data._loanTypeId.ToString(),
                           AccountName = data.accountName ?? data.description,
                           AccountNumber = data.accountNumber,
                           BankAccountId = data.loanLoginAccountId.ToString(),
                           Transactions =
                               data.loanTransactions == null
                                   ? null
                                   : data.loanTransactions.Cast<LoanTransaction>().Select(Map).OrderBy(x => x.Date).ToList(),
                           AccountLabelEnum = AccountLabelEnum.Loan,
                           AvailableBalanceInCents = data.availableCredit == null ? 000 : Convert.ToInt64(data.availableCredit.amount * 100),
                           CurrentBalanceInCents = Convert.ToInt64(data.principalBalance.amount * 100)
                       };
        }

        private static ContentServiceItemAccountTransaction Map(LoanTransaction data)
        {
            if (data.principal == null)
                Console.WriteLine("Null Principal");

            //Manually specifying debit credit when transaction type is unknown.
            if (data.transactionType == "unknown")
            {
                if (data.principal.amount == 0 && data.interest.amount == 0 && data.amount.amount > 0)
                    data.transactionType = "debit";

                else if (data.principal.amount == 0 && data.interest.amount == 0 && data.amount.amount < 0)
                    data.transactionType = "credit";

                else if (data.principal.amount > 0 || data.interest.amount > 0)
                    data.transactionType = "credit";

                else if (data.amount.amount == 0)
                    data.transactionType = "credit";
                else
                    data.transactionType = "credit";

                Console.WriteLine("Loan Transaction Type Unknown");
            }
            else
            {
                Console.WriteLine("Loan Transaction Type Known");
            }

            var validDate = data.postDate.date.Year == 0001 ? data.transDate.date : data.postDate.date;

            return new ContentServiceItemAccountTransaction
                       {
                           Status = data.transactionStatus,
                           Type = data.transactionType,
                           BankTransactionId = data.loanTransactionId.ToString(System.Globalization.CultureInfo.InvariantCulture),
                           Date = validDate,
                           Description = data.plainTextDescription,
                           Amount = Math.Abs(data.amount.amount),
                           CurrencyCode = data.amount.currencyCode,
                           CategorizationKeyword = data.categorizationKeyword ?? String.Empty,
                           PrincipalAmount = data.principal == null ? 0 : data.principal.amount,
                           InterestAmount = data.principal == null ? 0 : data.interest.amount,
                           TransactionType = data.transactionType == "credit" ? TransactionType.Transfer : TransactionType.Check
                       };
        }

        #endregion



        public Storage.Documents.ContentServiceItemAccount Map(ContentServiceItemAccount data)
        {
            return new Storage.Documents.ContentServiceItemAccount()
                       {
                           AccountId = data.AccountId,
                           AccountLabelEnum = (mPower.Domain.Accounting.Enums.AccountLabelEnum)data.AccountLabelEnum,
                           AccountName = data.AccountName,
                           AccountNumber = data.AccountNumber,
                           AccountType = data.AccountType,
                           AvailableBalanceInCents = data.AvailableBalanceInCents,
                           BankAccountId = data.BankAccountId,
                           CurrentBalanceInCents = data.CurrentBalanceInCents,
                           ImportStatus = ImportStatusEnum.New,
                           ItemAccountId = data.ItemAccountId,
                           Transactions = data.Transactions == null ? null : data.Transactions.Select(Map).ToList()

                       };
        }

        public Storage.Documents.ContentServiceItemAccountTransaction Map(ContentServiceItemAccountTransaction data)
        {
            return new Storage.Documents.ContentServiceItemAccountTransaction()
                       {
                           Amount = data.Amount,
                           BankTransactionId = data.BankTransactionId,
                           CategorizationKeyword = data.CategorizationKeyword,
                           CurrencyCode = data.CurrencyCode,
                           Date = data.Date,
                           Description = data.Description,
                           Status = data.Status,
                           Type = data.Type,
                           InterestAmount = data.InterestAmount,
                           PrincipalAmount = data.PrincipalAmount,
                           TransactionType = data.TransactionType,

                       };

        }


        #endregion

        private void CreateContentServiceItem(ContentServiceItemData data, string authenticationReferenceId)
        {
            var document = new ContentServiceItemDocument()
                               {
                                   ItemId = data.ItemId,
                                   UserId = data.UserId,
                                   AccessStatus = data.AccessStatus,
                                   ContentServiceId = data.ContentServiceId,
                                   DisplayName = data.DisplayName,
                                   LastUpdateAttempt = data.LastUpdateAttempt,
                                   LastSuccessfulUpdate = data.LastSuccessfulUpdate,
                                   RefreshStatus = data.RefreshStatus,
                                   Accounts = data.Accounts == null ? null : data.Accounts.Select(Map).ToList(),
                                   ResponseCode = data.ResponseCode,
                                   ActionRequired = data.ActionRequired,
                                   RefreshErrorCode = data.ErrorCode,
                                   AuthenticationReferenceId = authenticationReferenceId
                               };

            _docService.Insert(document);

        }

        private void UpdateContentServiceItem(ContentServiceItemData message)
        {
            var document = _docService.GetById(message.ItemId);

            document.AccessStatus = message.AccessStatus;
            document.LastSuccessfulUpdate = message.LastSuccessfulUpdate;
            document.LastUpdateAttempt = message.LastUpdateAttempt;
            document.DataUpdateAttemptStatus = message.DataUpdateAttemptStatus;
            document.ActionRequired = message.ActionRequired;
            document.RefreshErrorCode = message.ErrorCode;
            document.ResponseCode = message.ResponseCode;
            document.DisplayName = message.DisplayName;
            

            if (message.Accounts != null)
            {
                foreach (var msgAccount in message.Accounts)
                {

                    if (document.Accounts == null)
                    {
                        document.Accounts = new List<Storage.Documents.ContentServiceItemAccount>();
                        document.Accounts.Add(Map(msgAccount));
                        continue;
                    }

                    var docAccount = Enumerable.SingleOrDefault<Storage.Documents.ContentServiceItemAccount>(document.Accounts, x => x.AccountId == msgAccount.AccountId);

                    if (docAccount == null)
                    {
                        document.Accounts.Add(Map(msgAccount));
                        continue;

                    }

                    docAccount.CurrentBalanceInCents = msgAccount.CurrentBalanceInCents;
                    docAccount.AvailableBalanceInCents = msgAccount.AvailableBalanceInCents;

                    if (msgAccount.Transactions != null )
                    {
                        foreach (var transaction in msgAccount.Transactions)
                        {
                            if (transaction.BankTransactionId == null)
                                continue;

                            if(docAccount.Transactions == null)
                                docAccount.Transactions = new List<Storage.Documents.ContentServiceItemAccountTransaction>();
                            
                            var docTransaction =
                                docAccount.Transactions.SingleOrDefault(
                                    x => x.BankTransactionId == transaction.BankTransactionId);

                            if (docTransaction == null)
                            {
                                docAccount.Transactions.Add(Map(transaction));
                                continue;
                            }

                            docTransaction.Status = transaction.Status;
                        }
                    }
                }
            }

            _docService.Save(document);

        }
        
    }
}