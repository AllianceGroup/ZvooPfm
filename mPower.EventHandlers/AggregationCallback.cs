using MongoDB.Bson;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract;
using mPower.Aggregation.Contract.Data;
using mPower.Aggregation.Contract.Domain.Data;
using mPower.Aggregation.Contract.Domain.Enums;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger;
using mPower.Domain.Accounting.Ledger.Data;
using mPower.Domain.Accounting.Transaction;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Domain.Accounting.Transaction.Messages;
using mPower.EventHandlers.Eventual;
using mPower.Framework;
using Paralect.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Signals;
using ExpandedEntryData = mPower.Domain.Accounting.Transaction.Data.ExpandedEntryData;

namespace mPower.EventHandlers
{
    public class AggregationCallback : IAggregationCallback
    {
        private readonly IRepository _repository;
        private readonly IEventService _eventService;
        private readonly LedgerDocumentService _ledgerService;

        public AggregationCallback(IRepository repository, IEventService eventService, LedgerDocumentService ledgerService)
        {
            _repository = repository;
            _eventService = eventService;
            _ledgerService = ledgerService;
        }

        public void TransactionsAggregated(string ledgerId, List<AggregatedTransactionData> transactions, List<AggregatedAccountBalanceUpdateData> balanceUpdates)
        {
            var multipleTransactions = new List<CreateMultipleTransactionDto>();
            var ledger = _ledgerService.GetById(ledgerId);
            var userId = ledger.Users.First().Id;
            var metadata = GenerateMetadata();

            var aggregatedAccounts = ledger.Accounts.Where(x => x.Aggregated);
            foreach (var account in aggregatedAccounts)
            {
                #region Generate Transactions Created Messages

                var intuitAccountId = account.IntuitAccountId.Value;
                var accountTransactions = transactions.Where(x => x.IntuitAccountId == intuitAccountId);
                DateTime? latestPostedDate = null;
                var savedTransactionFinicityIds = new List<long>();
                foreach (var dto in accountTransactions)
                {
                    UpdateTransactionData(dto, account.LabelEnum);
                    var offsetAccount = GetOffestAccount(dto, ledger);
                    var entries = BuildEntries(dto.IntuitTransactionType, dto, offsetAccount);

                    var transactionType = GetTransactionType(dto.IntuitTransactionType, dto.AmountInCents);
                    dto.Id = ObjectId.GenerateNewId().ToString();
                    var transaction = new TransactionAR(dto.Id, userId, dto.LedgerId, transactionType, entries, dto.LedgerAccountId,
                        entries[0].AccountType, true, dto.Id, metadata, dto.ReferenceNumber, true);

                    _repository.Save(transaction);

                    if (latestPostedDate == null || latestPostedDate.Value < dto.PostedDate)
                    {
                        latestPostedDate = dto.PostedDate;
                    }
                    
                    multipleTransactions.Add(new CreateMultipleTransactionDto
                    {
                        Entries = entries,
                        LedgerId = dto.LedgerId,
                        TransactionId = dto.Id,
                        BaseAccountId = dto.LedgerAccountId
                    });
                    savedTransactionFinicityIds.Add(dto.FinicityTransactionId);
                }

                #endregion

                _eventService.Send(new Aggregation_TransactionSavedMessage
                {
                    UserId = userId,
                    IntuitAccountId = intuitAccountId,
                    LedgerId = ledgerId,
                    LatestPostedDate = latestPostedDate,
                    SavedTransactionFinicityIds = savedTransactionFinicityIds
                });
                

                #region Update Account Aggregated Balance

                var ledgerAccountId = account.Id;
                var accountBalanceUpdates = balanceUpdates.Where(x => x.LedgerAccountId == ledgerAccountId);
                foreach (var balanceUpdate in accountBalanceUpdates)
                {
                    var ledgerAr = _repository.GetById<LedgerAR>(balanceUpdate.LedgerId);
                    ledgerAr.SetCommandMetadata(metadata);
                    var data = new BalanceChangedData
                    {
                        AccountId = balanceUpdate.LedgerAccountId,
                        LedgerId = balanceUpdate.LedgerId,
                        BalanceInCents = AccountingFormatter.IntuitBalanceToAggregegatedBalanceInCents(balanceUpdate.NewBalance, account.LabelEnum),
                        AccountName = balanceUpdate.AccountName,
                        UserId = balanceUpdate.LogonId,
                        OldValueInCents = AccountingFormatter.IntuitBalanceToAggregegatedBalanceInCents(balanceUpdate.OldValue, account.LabelEnum),
                        Date = DateTime.Now
                    };
                    ledgerAr.UpdateAccountAggregatedBalance(data);
                    _repository.Save(ledgerAr);
                }
                #endregion
            }

            var message = new Transaction_CreateMultipleMessage
            {
                Transactions = multipleTransactions,
                Date = DateTime.Now,
            };

            _eventService.Send(message);
            _eventService.Send(new AccountsUpdateSignal { UserId = userId, LedgerId = ledgerId });
        }

        public void AccountAggregationStatusChanged(string ledgerId, long intuitAccountId, AggregationStatusEnum status, string exceptionId = null)
        {
            var ledgerAr = _repository.GetById<LedgerAR>(ledgerId);
            ledgerAr.SetCommandMetadata(GenerateMetadata());
            ledgerAr.UpdateAccountAggregationStatus(null, intuitAccountId, Map(status), DateTime.Now, exceptionId);
            _repository.Save(ledgerAr);

            var ledger = _ledgerService.GetById(ledgerId);
            _eventService.Send(new AccountsUpdateSignal { UserId = ledger.Users.First().Id, LedgerId = ledgerId });
        }

        private static void UpdateTransactionData(AggregatedTransactionData data, AccountLabelEnum label)
        {
            switch (label)
            {
                case AccountLabelEnum.Bank:
                    data.IntuitTransactionType = AggregatedTransactionType.Bank;
                    break;
                case AccountLabelEnum.CreditCard:
                    data.AmountInCents *= -1; // Intuit brings Liability transactions over in negative for so this is reversing that.
                    data.IntuitTransactionType = AggregatedTransactionType.CreditCard;
                    break;
                case AccountLabelEnum.Loan:
                    data.AmountInCents *= -1; // Intuit brings Liability transactions over in negative for so this is reversing that.
                    data.IntuitTransactionType = AggregatedTransactionType.Loan;
                    break;
            }
        }

        private static AccountDocument GetOffestAccount(AggregatedTransactionData data, LedgerDocument ledger)
        {
            try
            {
                return ledger.Accounts.First(x =>
                    data.Categories.Contains(x.Name) || 
                    x.IntuitCategoriesNames.Any(y => data.Categories.Contains(y)));
            }
            catch
            {
                string offsetAccountId;
                switch (data.IntuitTransactionType)
                {
                    case AggregatedTransactionType.Bank:
                    case AggregatedTransactionType.Investment:
                        offsetAccountId = data.AmountInCents > 0
                                              ? BaseAccounts.UnCategorizedIncome
                                              : BaseAccounts.UnCategorizedExpense;
                        break;
                    case AggregatedTransactionType.CreditCard:
                    case AggregatedTransactionType.Loan:
                        offsetAccountId = data.AmountInCents < 0
                                              ? BaseAccounts.Payments
                                              : BaseAccounts.UnCategorizedExpense;
                        break;
                    default:
                        throw new Exception("Provided intuit transaction type does not supported by system :" +
                                            data.IntuitTransactionType);
                }
                var account = ledger.Accounts.FirstOrDefault(x => x.Id == offsetAccountId);
                if (account == null)
                {
                    throw new Exception(
                        string.Format(
                            "Can't find default offset account with ID: {0} for imported transaction with ID: {1}",
                            offsetAccountId, data.Id));
                }
                return account;
            }
        }

        private static List<ExpandedEntryData> BuildEntries(AggregatedTransactionType type, AggregatedTransactionData data, AccountDocument offsetAccount)
        {
            switch (type)
            {
                #region bank transactions
                case AggregatedTransactionType.Bank:
                    {
                        var baseEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = data.LedgerAccountId,
                            DebitAmountInCents = AccountingFormatter.DebitAmount(data.AmountInCents, AccountTypeEnum.Asset),
                            CreditAmountInCents = AccountingFormatter.CreditAmount(data.AmountInCents, AccountTypeEnum.Asset),
                            AccountLabel = AccountLabelEnum.Bank,
                            AccountType = AccountTypeEnum.Asset,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = offsetAccount.Id,
                            OffsetAccountName = offsetAccount.Name,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = data.LedgerAccountName

                        };

                        var offSetEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = baseEntry.OffsetAccountId,
                            DebitAmountInCents = baseEntry.CreditAmountInCents,
                            CreditAmountInCents = baseEntry.DebitAmountInCents,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = baseEntry.AccountId,
                            OffsetAccountName = baseEntry.AccountName,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = offsetAccount.Name,
                            AccountLabel = offsetAccount.LabelEnum,
                            AccountType = offsetAccount.TypeEnum,
                        };

                        return new List<ExpandedEntryData> { baseEntry, offSetEntry };
                    }
                #endregion

                #region cc transactions

                case AggregatedTransactionType.CreditCard:
                    {
                        var baseEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = data.LedgerAccountId,
                            DebitAmountInCents = AccountingFormatter.DebitAmount(data.AmountInCents, AccountTypeEnum.Liability),
                            CreditAmountInCents = AccountingFormatter.CreditAmount(data.AmountInCents, AccountTypeEnum.Liability),
                            AccountLabel = AccountLabelEnum.CreditCard,
                            AccountType = AccountTypeEnum.Liability,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = offsetAccount.Id,
                            OffsetAccountName = offsetAccount.Name,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = data.LedgerAccountName
                        };

                        var offSetEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = baseEntry.OffsetAccountId,
                            DebitAmountInCents = baseEntry.CreditAmountInCents,
                            CreditAmountInCents = baseEntry.DebitAmountInCents,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = baseEntry.AccountId,
                            OffsetAccountName = baseEntry.AccountName,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = offsetAccount.Name,
                            AccountLabel = offsetAccount.LabelEnum,
                            AccountType = offsetAccount.TypeEnum
                        };

                        return new List<ExpandedEntryData> { baseEntry, offSetEntry };
                    }

                #endregion

                #region loan transactions

                case AggregatedTransactionType.Loan:
                    {
                        var baseEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = data.LedgerAccountId,
                            DebitAmountInCents = AccountingFormatter.DebitAmount(data.AmountInCents, AccountTypeEnum.Liability),
                            CreditAmountInCents = AccountingFormatter.CreditAmount(data.AmountInCents, AccountTypeEnum.Liability),
                            AccountLabel = AccountLabelEnum.Loan,
                            AccountType = AccountTypeEnum.Liability,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = offsetAccount.Id,
                            OffsetAccountName = offsetAccount.Name,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = data.LedgerAccountName,
                        };

                        var offSetEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = baseEntry.OffsetAccountId,
                            DebitAmountInCents = baseEntry.CreditAmountInCents,
                            CreditAmountInCents = baseEntry.DebitAmountInCents,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = baseEntry.AccountId,
                            OffsetAccountName = baseEntry.AccountName,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = offsetAccount.Name,
                            AccountLabel = offsetAccount.LabelEnum,
                            AccountType = offsetAccount.TypeEnum,
                        };

                        return new List<ExpandedEntryData> { baseEntry, offSetEntry };
                    }

                #endregion

                #region investment transactions
                case AggregatedTransactionType.Investment:
                    {
                        var baseEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = data.LedgerAccountId,
                            DebitAmountInCents = AccountingFormatter.DebitAmount(data.AmountInCents, AccountTypeEnum.Asset),
                            CreditAmountInCents = AccountingFormatter.CreditAmount(data.AmountInCents, AccountTypeEnum.Asset),
                            AccountLabel = AccountLabelEnum.Bank,
                            AccountType = AccountTypeEnum.Asset,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = offsetAccount.Id,
                            OffsetAccountName = offsetAccount.Name,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = data.LedgerAccountName
                        };

                        var offSetEntry = new ExpandedEntryData
                        {
                            Memo = data.Description,
                            BookedDate = data.PostedDate,
                            AccountId = baseEntry.OffsetAccountId,
                            DebitAmountInCents = baseEntry.CreditAmountInCents,
                            CreditAmountInCents = baseEntry.DebitAmountInCents,
                            LedgerId = data.LedgerId,
                            OffsetAccountId = baseEntry.AccountId,
                            OffsetAccountName = baseEntry.AccountName,
                            Payee = String.IsNullOrEmpty(data.NormalizedDescription) ? data.Description : data.NormalizedDescription,
                            TransactionId = data.Id,
                            TransactionImported = true,
                            AccountName = offsetAccount.Name,
                            AccountLabel = offsetAccount.LabelEnum,
                            AccountType = offsetAccount.TypeEnum,
                        };

                        return new List<ExpandedEntryData> { baseEntry, offSetEntry };
                    }

                #endregion

                default:
                    throw new Exception("Provided intuit transaction type does not supported by system :" + type);
            }
        }

        private static TransactionType GetTransactionType(AggregatedTransactionType intuitType, long amountInCents)
        {
            TransactionType type;
            switch (intuitType)
            {
                case AggregatedTransactionType.Bank:
                case AggregatedTransactionType.Investment:
                    type = amountInCents > 0 ? TransactionType.Deposit : TransactionType.Check;
                    break;
                case AggregatedTransactionType.CreditCard:
                    type = amountInCents > 0 ? TransactionType.CreditCard : TransactionType.Check;
                    break;
                case AggregatedTransactionType.Loan:
                    type = TransactionType.Check;
                    break;

                default:
                    throw new Exception("Provided intuit transaction type does not supported by system :" + intuitType);
            }

            return type;
        }

        private static CommandMetadata GenerateMetadata()
        {
            return new CommandMetadata
            {
                CommandId = ObjectId.GenerateNewId().ToString(),
                CreatedDate = DateTime.Now,
                UserId = "Intuit",
            };
        }

        private static AggregatedAccountStatusEnum Map(AggregationStatusEnum status)
        {
            switch (status)
            {
                case AggregationStatusEnum.PullingTransactions:
                    return AggregatedAccountStatusEnum.PullingTransactions;
                case AggregationStatusEnum.AccountBeingAggregated:
                    return AggregatedAccountStatusEnum.AccountBeingAggregated;
                case AggregationStatusEnum.NeedReauthentication:
                    return AggregatedAccountStatusEnum.NeedReauthentication;
                case AggregationStatusEnum.UnexpectedErrorOccurred:
                    return AggregatedAccountStatusEnum.UnexpectedErrorOccurred;
                case AggregationStatusEnum.NeedInteractiveRefresh:
                    return AggregatedAccountStatusEnum.NeedInteractiveRefresh;
                case AggregationStatusEnum.Finished:
                    return AggregatedAccountStatusEnum.Normal;
                default:
                    throw new NotSupportedException(string.Format("Aggrecation status '{0}' is not supported.", status));
            }
        }
    }
}