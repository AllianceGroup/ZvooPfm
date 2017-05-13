using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Yodlee.ContentServiceItem.Commands;
using mPower.Domain.Yodlee.Enums;
using mPower.Domain.Yodlee.Storage.Documents;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using mPower.Framework.Services;
using NLog;
using Paralect.Domain;
using AccountLabelEnum = mPower.Domain.Accounting.Enums.AccountLabelEnum;

namespace mPower.Documents
{
    public class YodleeBulkOperations
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private Dictionary<String, String> _keywords;


        private static void LogInfo(string message)
        {
            _logger.Log(LogLevel.Info, "YodleeUpdateJob: " + message);
        }

        private readonly IIdGenerator _generator;
        private readonly LedgerDocumentService _ledger;
        private readonly UserDocumentService _userDocumentService;
        private readonly CommandService _commandService;
        private readonly IObjectRepository _objectRepository;

        private readonly ImportedTransactionDocumentService _transactionDocumentService;
        private readonly ContentServiceItemDocumentService _contentServiceItemDocService;

        public YodleeBulkOperations(
                                  IIdGenerator generator,
                                  ContentServiceItemDocumentService contentServiceItemDocService,
                                  LedgerDocumentService ledger,
                                  UserDocumentService userDocumentService,
                                  CommandService commandService,
                                  IObjectRepository objectRepository,
                                  ImportedTransactionDocumentService transactionDocumentService)
        {
            _generator = generator;
            _contentServiceItemDocService = contentServiceItemDocService;
            _ledger = ledger;
            _userDocumentService = userDocumentService;
            _commandService = commandService;
            _objectRepository = objectRepository;
            _transactionDocumentService = transactionDocumentService;
        }

        /// <summary>
        /// Loops through all users and tells yodlee to start pulling their data from the institutions
        /// </summary>
        public void StartRefreshForAllContentServiceItems()
        {

            var contentServiceItemDocuments = _contentServiceItemDocService.GetAll();

            LogInfo(String.Format("----- Starting Content Service Refresh for {0} items -----", contentServiceItemDocuments.Count));

            // Start the Yodlee Refresh Content Service Item Process for every content service item in the db.
            foreach (var document in contentServiceItemDocuments)
            {
                try
                {
                    var user = _userDocumentService.GetById(document.UserId);

                    if (user == null)
                    {
                        _logger.Error(String.Format("User not found for ItemID: {0}, Looking for UserId: {1}",
                                              document.ItemId, document.UserId));
                        continue;
                    }

                    _commandService.Send(new ContentServiceItem_StartRefreshCommand()
                                             {
                                                 ItemId = Convert.ToInt64(document.ItemId),
                                                 Username = user.YodleeUserInfo.LoginName,
                                                 Password = user.YodleeUserInfo.Password

                                             });

                    //LogInfo(String.Format("Refreshing ItemId: {0}, For User: {1}", item.ItemId, user.UserName);
                }
                catch (Exception e)
                {
                    _logger.FatalException("Refresh Content Service Exception", e);
                }
            }

            LogInfo("----- Content Service Refresh Completed -----");
        }

        public void StartRefreshForUser(string userId)
        {
            var user = _userDocumentService.GetById(userId);

            var contentServiceItemDocuments = _contentServiceItemDocService.GetByUserId(userId);

            foreach (var document in contentServiceItemDocuments.Where(x => x.RefreshStatus != RefreshStatus.InvalidItem))
            {
                _commandService.Send(new ContentServiceItem_StartRefreshCommand()
                {
                    ItemId = Convert.ToInt64(document.ItemId),
                    Username = user.YodleeUserInfo.LoginName,
                    Password = user.YodleeUserInfo.Password

                });
            }
            

        }

        /// <summary>
        /// Loops through all users and pull data from yodlee for each account with a content service item id
        /// </summary>
        public void DownloadAllContentServiceItemData()
        {
            var items = _contentServiceItemDocService.GetAll();
            LogInfo("----- Starting Content Service Item Update -----");

            // Start pull the Data for Each Item and Update It
            foreach (var item in items)
            {
                try
                {
                    var user = _userDocumentService.GetById(item.UserId);

                    if (user == null)
                    {
                        LogInfo(String.Format("User not found for ItemID: {0}, Looking for UserId: {1}",
                                              item.ItemId, item.UserId));
                        continue;
                    }

                    _commandService.Send(new ContentServiceItem_DownloadCommand()
                                             {
                                                 LoginName = user.YodleeUserInfo.LoginName,
                                                 Password = user.YodleeUserInfo.Password,
                                                 UserId = user.Id,
                                                 ItemId = Convert.ToInt64(item.ItemId)
                                             });

                }
                catch (Exception e)
                {
                    _logger.FatalException("ContentServiceItemRefreshException", e);
                }
            }

            LogInfo("----- Content Service Update Completed -----");

        }

        public void DownloadContentServiceItemDateForUser(string userId)
        {
            var items = _contentServiceItemDocService.GetByUserId(userId).Where(x => x.RefreshStatus != RefreshStatus.InvalidItem);
            var user = _userDocumentService.GetById(userId);

            foreach (var item in items)
            {
                _commandService.Send(new ContentServiceItem_DownloadCommand()
                {
                    LoginName = user.YodleeUserInfo.LoginName,
                    Password = user.YodleeUserInfo.Password,
                    UserId = user.Id,
                    ItemId = Convert.ToInt64(item.ItemId)
                });
            }

        }

        /// <summary>
        /// Pulls the data from the database and converts it into a format for the ledger
        /// </summary>
        public void UpdateAllLedgers()
        {
            var ledgers = _ledger.GetAll();
            
            var transactionCommandWrappers = (from ledger in ledgers
                                              let accounts = ledger.Accounts.Where(x => !String.IsNullOrEmpty(x.YodleeContentServiceItemId))
                                              from account in accounts
                                              select GetAggregatedDataForContentServiceItemAccount(account.YodleeContentServiceItemId,
                                                                                        account.YodleeItemAccountId, ledger.Id,
                                                                                        account.Id, account.TypeEnum)).ToList();

            Process(transactionCommandWrappers);
        }

        public void UpdateLedger(string ledgerId)
        {
            var ledger = _ledger.GetById(ledgerId);

            var transactionCommandWrappers = (from account in ledger.Accounts.Where(x => !String.IsNullOrEmpty(x.YodleeContentServiceItemId))
                                              select GetAggregatedDataForContentServiceItemAccount(account.YodleeContentServiceItemId,
                                                                                        account.YodleeItemAccountId, ledger.Id,
                                                                                        account.Id, account.TypeEnum)).ToList();

            Process(transactionCommandWrappers);

        }


        public List<ICommand> GetTransactionCommandsForNewAccount(ContentServiceItemDocument contentServiceItemDocument,
                                                     Ledger_Account_CreateCommand[] createAccountCommands)
        {

            var pendingCommands = new List<ICommand>();

            foreach (var cmd in createAccountCommands)
            {
                var aggregatedData = GetAggregatedDataForContentServiceItemAccount(cmd.YodleeContentServiceItemId,
                                                                            cmd.YodleeItemAccountId, cmd.LedgerId,
                                                                            cmd.AccountId, cmd.AccountTypeEnum);

                if (aggregatedData.TransactionCreateCommands != null)
                    pendingCommands.AddRange(aggregatedData.TransactionCreateCommands);


                #region Adjusting Entry Section
                var contentServiceItemAccount =
                    (from acct in contentServiceItemDocument.Accounts
                     where acct.ItemAccountId == cmd.YodleeItemAccountId
                     select acct).Single();

                if (aggregatedData.TransactionBalanceInCents == contentServiceItemAccount.CurrentBalanceInCents) continue;

                var ledger = _ledger.GetById(cmd.LedgerId);

                var adjustinEntryCommand = GetAdjustingEntryCommand(aggregatedData.TransactionBalanceInCents,
                                                                    contentServiceItemAccount.CurrentBalanceInCents,
                                                                    cmd, aggregatedData.FirstTransactionDate, ledger);
                pendingCommands.Add(adjustinEntryCommand);
                #endregion

                var updateContentServiceItemCommand = new ContentServiceItem_Account_UpdateImportStatusCommand
                {
                    ContentServiceItemAccountId =
                        contentServiceItemAccount.AccountId,
                    ImportStatus = ImportStatusEnum.Mapped,
                    ItemId = cmd.YodleeContentServiceItemId,
                    LedgerId = cmd.LedgerId,
                    LedgerAccountId = cmd.AccountId
                };

                pendingCommands.Add(updateContentServiceItemCommand);
            }

            return pendingCommands;
        }





        #region Private Methods

        private void Process(List<AggregatedDataWrapper> transactionCommandWrappers)
        {
            foreach (var dataWrapper in transactionCommandWrappers)
            {

                if (dataWrapper != null)
                {
                    _commandService.Send(dataWrapper.AccountAggregatedBalanceUpdateCommand);

                    LogInfo(String.Format("Message send for ledgerId: {0} accountId:{1} newBalance{2}:",
                                          dataWrapper.AccountAggregatedBalanceUpdateCommand.LedgerId,
                                          dataWrapper.AccountAggregatedBalanceUpdateCommand.AccountId,
                                          dataWrapper.AccountAggregatedBalanceUpdateCommand.NewBalance));
                }
            }

            var transactionCommands = (from wrapper in transactionCommandWrappers.Where(x => x != null && x.TransactionCreateCommands != null)
                                       let createCommands = wrapper.TransactionCreateCommands
                                       from command in createCommands
                                       select command).ToList();

            foreach (var transaction in transactionCommands)
            {
                if (_transactionDocumentService.TransactionNotImported(transaction.LedgerId, transaction.ImportedTransactionId))
                {
                    _commandService.Send(transaction);
                }
            }

        }

        private void LoadKeywords(LedgerDocument ledger)
        {
            _keywords = new Dictionary<String, String>();


            //Load the keywords from the Ledger
            //And allow user keywords to override global keywords.

            if (ledger.KeywordMap != null)
            {
                foreach (var keyword in ledger.KeywordMap)
                {
                    string value;
                    if (!_keywords.TryGetValue(keyword.Keyword, out value))
                        _keywords.Add(keyword.Keyword, ledger.Accounts.Single(x => x.Id == keyword.AccountId).Name);
                }
            }



            // Read sample data from CSV file
            // This loads the Global KeyWords
            using (var reader = new CsvFileReader(@"c:\temp\coa.csv"))
            {
                var row = new CsvRow();
                while (reader.ReadRow(row))
                {
                    string value;
                    if (!_keywords.TryGetValue(row[0], out value))
                        _keywords.Add(row[0], row[1]);
                }
            }



        }





        private Transaction_CreateCommand GetAdjustingEntryCommand(long transactionBalanceInCents,
                                                                          long accountBalanceInCents,
                                                                          Ledger_Account_CreateCommand cmd,
                                                                          DateTime firstTransactionDate,
                                                                          LedgerDocument ledger)
        {
            TransactionType transactionType;

            switch (cmd.AccountLabelEnum)
            {
                case AccountLabelEnum.Bank:
                    transactionType = TransactionType.Deposit;
                    break;

                case AccountLabelEnum.CreditCard:
                    transactionType = TransactionType.CreditCard;
                    break;

                case AccountLabelEnum.Loan:
                    transactionType = TransactionType.Check;
                    break;

                default:
                    transactionType = TransactionType.Deposit;
                    break;
            }


            var differenceInBalance = accountBalanceInCents - transactionBalanceInCents;

            var accountId = String.Empty;

            switch (ledger.TypeEnum)
            {
                case LedgerTypeEnum.Business:
                    accountId = AccountingFormatter.DebitOrCredit(differenceInBalance, AccountTypeEnum.Equity) ==
                                AmountTypeEnum.Credit
                                    ? BaseAccounts.OwnerContribution
                                    : BaseAccounts.OwnerDistribution;
                    break;

                case LedgerTypeEnum.Personal:
                    accountId = BaseAccounts.OpeningBalanceEquity;
                    break;
            }


            var command = _objectRepository.Load<TransactionDto, Transaction_CreateCommand>(new TransactionDto
            {
                TransactionId = _generator.Generate(),
                Type = transactionType,
                LedgerId = cmd.LedgerId,
                BaseEntryAccountId = cmd.AccountId,
                Entries = new List<EntryData>
                              {
                                                    new EntryData
                                                        {
                                                            AccountId = cmd.AccountId,
                                                            BookedDate = firstTransactionDate.AddDays(-1),
                                                            CreditAmountInCents =
                                                                AccountingFormatter.CreditAmount(
                                                                    differenceInBalance, cmd.AccountTypeEnum),
                                                            DebitAmountInCents =
                                                                AccountingFormatter.DebitAmount(
                                                                    differenceInBalance, cmd.AccountTypeEnum),
                                                            Memo = "Beginning Balance Adjustment"
                                                        },
                                                    new EntryData
                                                        {
                                                            AccountId = accountId,
                                                            BookedDate = firstTransactionDate.AddDays(-1),
                                                            CreditAmountInCents =
                                                                AccountingFormatter.DebitAmount(
                                                                    differenceInBalance, cmd.AccountTypeEnum),
                                                            DebitAmountInCents = AccountingFormatter.CreditAmount(
                                                                differenceInBalance, cmd.AccountTypeEnum),
                                                            Memo = "Beginning Balance Adjustment"
                                                        }
                                                },
                BaseEntryAccountType = cmd.AccountTypeEnum

            });

            return command;
        }


        private AggregatedDataWrapper GetAggregatedDataForContentServiceItemAccount(
            string contentServiceItemId, string itemAccountId, string ledgerId, string ledgerAccountId, AccountTypeEnum accountTypeEnum)
        {
            try
            {
                if (String.IsNullOrEmpty(contentServiceItemId))
                    throw new Exception("ContentServiceIdEmpty");


                var contentServiceItemDocument =
                    _contentServiceItemDocService.GetById(contentServiceItemId);

                if (contentServiceItemDocument == null)
                    return null;


                var contentServiceItemDocument_Account =
                    contentServiceItemDocument.Accounts.Single(x => x.ItemAccountId == itemAccountId);


                var aggregatedData = CreateAggregatedData(contentServiceItemDocument_Account, ledgerId, ledgerAccountId,
                                                          accountTypeEnum);

                aggregatedData.AccountAggregatedBalanceUpdateCommand = new Ledger_Account_AggregatedBalanceUpdateCommand
                                                                           {

                                                                               AccountId = ledgerAccountId,
                                                                               NewBalance =
                                                                                   contentServiceItemDocument_Account.
                                                                                       CurrentBalanceInCents == 0
                                                                                       ? contentServiceItemDocument_Account
                                                                                             .AvailableBalanceInCents
                                                                                       : contentServiceItemDocument_Account
                                                                                             .CurrentBalanceInCents,
                                                                               LedgerId = ledgerId

                                                                           };

                return aggregatedData;
            }

            catch (Exception e)
            {

                _logger.Error(e.Message);
                return null;
            }
        }

        private AggregatedDataWrapper CreateAggregatedData(ContentServiceItemAccount account,
                                                                            string ledgerId,
                                                                            string accountId,
                                                                            AccountTypeEnum accountTypeEnum)
        {

            if (account.Transactions == null)
            {
                return new AggregatedDataWrapper()
                         {
                             FirstTransactionDate = DateTime.Now,
                             TransactionBalanceInCents = 0
                         };

            }


            // This section is pretty confusing because the credits and debits are sent over in terms of the bank not the customer
            // so the credit and debits are reversed.
            var newTransactionCommands = new List<Transaction_CreateCommand>();

            var ledger = _ledger.GetById(ledgerId);

            if (account.AccountLabelEnum == AccountLabelEnum.Loan)
                newTransactionCommands.AddRange(
                    account.Transactions.Select(
                        x => MapSplit(x, accountId, accountTypeEnum, ledger)));
            else
                newTransactionCommands.AddRange(
                    account.Transactions.Select(
                        x => MapStandard(x, accountId, accountTypeEnum, ledger)));


            var totalDebits =
                newTransactionCommands.Sum(
                    trans =>
                    trans.Entries.Where(e => e.AccountId == accountId).Sum(
                        x => x.DebitAmountInCents));
            var totalCredits =
                newTransactionCommands.Sum(
                    trans =>
                    trans.Entries.Where(e => e.AccountId == accountId).Sum(
                        x => x.CreditAmountInCents));

            var result = new AggregatedDataWrapper()
                         {
                             TransactionCreateCommands = newTransactionCommands.ToArray(),
                             FirstTransactionDate = account.Transactions.OrderBy(x => x.Date).First().Date,
                             TransactionBalanceInCents =
                                 AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(
                                     totalDebits,
                                     totalCredits,
                                     accountTypeEnum)

                         };



            return result;

        }

        
        private Transaction_CreateCommand MapStandard(ContentServiceItemAccountTransaction data, string accountId, AccountTypeEnum accountType,
                                                             LedgerDocument ledger)
        {


            return _objectRepository.Load<TransactionDto, Transaction_CreateCommand>(new TransactionDto()
            {
                Entries = new List<EntryData>
                              {
                                             new EntryData
                                                 {
                                                     Memo = data.Description,
                                                     BookedDate = data.Date,
                                                     AccountId = accountId,
                                                     DebitAmountInCents =
                                                         data.Type == "credit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                     //Reversed because the bank sends them in relation to their accounting to it is opposite
                                                     CreditAmountInCents =
                                                         data.Type == "debit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                 },
                                             new EntryData
                                                 {
                                                     Memo = data.Description,
                                                     BookedDate = data.Date,
                                                     AccountId = GetAccountIdFromDescription(data.Description, data.Type, ledger, accountId),
                                                         
                                                     DebitAmountInCents =
                                                         data.Type == "debit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                     CreditAmountInCents =
                                                         data.Type == "credit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                     //reveresed debit on credit on purpose to balance transaction
                                                 }
                                         },
                TransactionId = _generator.Generate(),
                ImportedTransactionId = data.BankTransactionId,
                Type = data.TransactionType,
                BaseEntryAccountId = accountId,
                //Reversed because credit is in terms of the bank not the user
                LedgerId = ledger.Id,
                Imported = true,
                BaseEntryAccountType = accountType
            });



        }

        private string GetAccountIdFromDescription(string description, string transactionType, LedgerDocument ledger, string accountId)
        {
            //if (type == "credit")
            //    return BaseAccounts.UnCategorizedIncome;

            if (_keywords == null)
                LoadKeywords(ledger);

            var accountName = _keywords.DeriveAccountFromDescription(description);

            if (accountName == null)
                return GetAccountIdFromTransactionData(accountId, transactionType, ledger);

            var account = ledger.Accounts.FirstOrDefault(x => x.Name.Equals(accountName));

            return account == null ? GetAccountIdFromTransactionData(accountId, transactionType, ledger) : account.Id;

        }

        private string GetAccountIdFromTransactionData(string accountLId, string transactionType, LedgerDocument ledgerDocument)
        {
            var account = ledgerDocument.Accounts.Single(x => x.Id == accountLId);

            switch (account.LabelEnum)
            {
                case AccountLabelEnum.CreditCard:
                    if (transactionType == "credit")
                        return BaseAccounts.UnknownCash;
                    else
                        return BaseAccounts.UnCategorizedExpense;

                case AccountLabelEnum.Loan:
                    if (transactionType == "credit")
                        return BaseAccounts.UnknownCash;
                    else
                        return BaseAccounts.UnCategorizedExpense;

                case AccountLabelEnum.Bank:
                    if (transactionType == "credit")
                        return BaseAccounts.UnCategorizedIncome;
                    else
                        return BaseAccounts.UnCategorizedExpense;

                case AccountLabelEnum.Investment:
                    if (transactionType == "credit")
                        return BaseAccounts.UnCategorizedIncome;
                    else
                        return BaseAccounts.UnCategorizedExpense;
            }


            throw new NotImplementedException();
        }


        private Transaction_CreateCommand MapSplit(ContentServiceItemAccountTransaction data, string accountId, AccountTypeEnum accountType,
                                                          LedgerDocument ledger)
        {

            if (data.PrincipalAmount == 0 && data.InterestAmount == 0)
                return MapStandardLoan(data, accountId, accountType, ledger);

            var entries = new List<EntryData>();

            //Base Entry for Unknown Cash On Split Loans
            entries.Add(new EntryData
                            {
                                Memo = data.Description,
                                BookedDate = data.Date,
                                AccountId = BaseAccounts.UnknownCash,
                                DebitAmountInCents = 0,
                                CreditAmountInCents = Convert.ToInt64(data.Amount * 100)
                            });


            if (data.InterestAmount != 0)
            {
                entries.Add(new EntryData
                {
                    Memo = data.Description,
                    BookedDate = data.Date,
                    AccountId = BaseAccounts.Interest,
                    DebitAmountInCents = Convert.ToInt64(data.InterestAmount * 100),
                    CreditAmountInCents = 0
                });
            }

            if (data.PrincipalAmount != 0)
            {
                entries.Add(new EntryData
                {
                    Memo = data.Description,
                    BookedDate = data.Date,
                    AccountId = accountId,
                    DebitAmountInCents = Convert.ToInt64(data.PrincipalAmount * 100),
                    CreditAmountInCents = 0
                });
            }

            // Often times additional amount will go to fees or loans escrow
            if (data.PrincipalAmount + data.InterestAmount != data.Amount)
            {
                var difference = data.Amount - (data.PrincipalAmount + data.InterestAmount);

                var entry = new EntryData
                {
                    AccountId = accountId,
                    BookedDate = data.Date,
                    Memo = data.Description,
                    DebitAmountInCents = Convert.ToInt64(difference * 100),
                    CreditAmountInCents = 0
                };

                entries.Add(entry);

            }

            var cmd = _objectRepository.Load<TransactionDto, Transaction_CreateCommand>(new TransactionDto()
            {
                Entries = entries,
                TransactionId = _generator.Generate(),
                ImportedTransactionId = data.BankTransactionId,
                Type = data.TransactionType,
                LedgerId = ledger.Id,
                BaseEntryAccountId = accountId,
                Imported = true,
                BaseEntryAccountType = accountType
            });


            cmd.Entries.AddRange(TransactionGenerator.ExpandEntryData(ledger, entries.ToArray()));

            return cmd;
        }


        private Transaction_CreateCommand MapStandardLoan(ContentServiceItemAccountTransaction data,
                                                                 string accountId, AccountTypeEnum accountType, LedgerDocument ledger)
        {
            return _objectRepository.Load<TransactionDto, Transaction_CreateCommand>(new TransactionDto()
            {
                Entries = new List<EntryData>
                              {
                                             new EntryData
                                                 {
                                                     Memo = data.Description,
                                                     BookedDate = data.Date,
                                                     AccountId = accountId,
                                                     DebitAmountInCents =
                                                         data.Type == "credit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                     //Reversed because the bank sends them in relation to their accounting to it is opposite
                                                     CreditAmountInCents =
                                                         data.Type == "debit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                 },
                                             new EntryData
                                                 {
                                                     Memo = data.Description,
                                                     BookedDate = data.Date,
                                                     AccountId = BaseAccounts.UnCategorizedExpense,
                                                     DebitAmountInCents =
                                                         data.Type == "debit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                     CreditAmountInCents =
                                                         data.Type == "credit" ? Convert.ToInt64(data.Amount*100) : 0,
                                                     //reveresed debit on credit on purpose to balance transaction
                                                 }
                                         },
                TransactionId = _generator.Generate(),
                ImportedTransactionId = data.BankTransactionId,
                Type = data.TransactionType,
                //Reversed because credit is in terms of the bank not the user
                LedgerId = ledger.Id,
                Imported = true,
                BaseEntryAccountId = accountId,
                BaseEntryAccountType = accountType
            });
        }

        #endregion
    }


    public class AggregatedDataWrapper
    {
        public Ledger_Account_AggregatedBalanceUpdateCommand AccountAggregatedBalanceUpdateCommand { get; set; }
        public long TransactionBalanceInCents { get; set; }
        public DateTime FirstTransactionDate { get; set; }
        public Transaction_CreateCommand[] TransactionCreateCommands { get; set; }
    }
}
