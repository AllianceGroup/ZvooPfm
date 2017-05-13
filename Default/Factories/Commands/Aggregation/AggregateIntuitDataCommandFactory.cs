using Default.Models;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Data;
using mPower.Aggregation.Contract.Documents;
using mPower.Aggregation.Contract.Domain.Enums;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Membership.User.Commands;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Default.Factories.Commands.Aggregation
{
    public class AggregateIntuitDataCommandFactory : IObjectFactory<AggregateUserDto, AggregateUserResult>, 
        IObjectFactory<AggregateAccountsDto, AggregateAccountsResult>,
        IObjectFactory<AuthenticateDto, AuthenticateToInstitutionViewModel>,
        IObjectFactory<RefreshAccountDto, RefreshAccountResult>,
        IObjectFactory<ReauthenticateDto, ReauthenticateToInstitutionViewModel>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IAggregationClient _aggregation;
        private readonly IIdGenerator _generator;
        private readonly IObjectRepository _objectRepository;

        public AggregateIntuitDataCommandFactory(LedgerDocumentService ledgerService,
            IAggregationClient aggregation, IIdGenerator generator, IObjectRepository objectRepository)
        {
            _ledgerService = ledgerService;
            _aggregation = aggregation;
            _generator = generator;
            _objectRepository = objectRepository;
        }

        public AggregateUserResult Load(AggregateUserDto dto)
        {
            var result = new AggregateUserResult();
            var ledger = dto.LedgerId == null
                ? _ledgerService.GetPersonal(dto.UserId)
                : _ledgerService.GetById(dto.LedgerId);
            var accounts = ledger.Accounts.Where(x => x.IsAggregated).ToList();

            #region Autoupdate logic
            if (dto.IsAutoUpdate)
            {
                var border = DateTime.Now.AddHours(-1);
                if (accounts.Any(x => x.DateLastAggregated > border))
                {
                    return result;
                }
                result.SetUserAutoUpdateCommand = new User_SetAutoUpdateDateCommand
                {
                    UserId = dto.UserId,
                    Date = DateTime.Now,
                };

            }
            #endregion

            foreach (var accountDoc in accounts)
            {
                var intuitAccountId = accountDoc.IntuitAccountId.Value;
                var accountResult = _objectRepository.Load<RefreshAccountDto, RefreshAccountResult>(new RefreshAccountDto
                {
                    UserId = dto.UserId,
                    LedgerId = ledger.Id,
                    IntuitAccountId = intuitAccountId,
                });
                if (accountResult.SetStatusCommand != null)
                {
                    result.SetStatusCommands.Add(accountResult.SetStatusCommand);
                }
                if (accountResult.PullTransactions)
                {
                    result.IntuitAccountsIds.Add(intuitAccountId);
                }
            }
            return result;
        }

        public AggregateAccountsResult Load(AggregateAccountsDto dto)
        {
            var result = new AggregateAccountsResult();

            var institution = _aggregation.GetInstitution(dto.DiscoverySession.ContentServiceId);
            var ledger = _ledgerService.GetById(dto.LedgerId);

            foreach (var account in dto.Model.Accounts.Where(x => x.Selected))
            {
                var acct = dto.DiscoverySession.Accounts.Single(x =>
                    x.AccountNumber == account.Number &&
                    x.Balance == account.Balance &&
                    x.Nickname == account.Nickname);

                var ledgerAccount = ledger.Accounts.FirstOrDefault(x => x.IntuitAccountNumber == acct.AccountNumber);

                //if we are not aggregated this account yet -- aggregate
                if (ledgerAccount == null)
                {
                    var accountName = String.Format("{0} ({1})",
                                                    acct.Nickname,
                                                    acct.AccountNumber == null
                                                        ? String.Empty
                                                        : acct.AccountNumber.Substring(Math.Max(acct.AccountNumber.Length - 4, 0)));

                    var cmd = new Ledger_Account_CreateCommand
                    {
                        AccountId = _generator.Generate(),
                        Name = accountName,
                        Description = acct.AccountNumber,
                        Aggregated = true,
                        LedgerId = dto.LedgerId,
                        AccountLabelEnum = account.AssignedAccountType.Value,
                        AccountTypeEnum = AccountingFormatter.AccountLabelToType(account.AssignedAccountType.Value),
                        Imported = true,
                        IntuitInstitutionId = institution.IntuitId,
                        InstitutionName = institution.Name,
                        IntuitAccountNumber = acct.AccountNumber
                    };

                    acct.LedgerId = cmd.LedgerId;
                    acct.LedgerAccountId = cmd.AccountId;
                    acct.LedgerAccountName = cmd.Name;
                    if (acct.Type == AggregatedAccountType.Unknown)
                    {
                        acct.Type = ConvertLabelToType(account.AssignedAccountType.Value);
                    }

                    var intuitAddedAccountsIds = _aggregation.AddAccounts(
                        new Metadata {LogonId = dto.UserId, IsLoggingEnabled = dto.AggregationLoggingEnabled}, 
                        new List<AccountDocument> {acct}, 
                        dto.DiscoverySession.Keys, 
                        dto.DiscoverySession.ContentServiceId).ToList();

                    if (intuitAddedAccountsIds.Count < 1)
                        continue;
                    
                    // this id is exists only after we added account on intuit
                    var intuitAddedAccountId = intuitAddedAccountsIds.First();
                    cmd.IntuitAccountId = intuitAddedAccountId.ToString(CultureInfo.InvariantCulture);
                    result.AddAccountCommands.Add(cmd);

                    var aggregatedBalanceInCents = AccountingFormatter.IntuitBalanceToAggregegatedBalanceInCents(acct.Balance, account.AssignedAccountType.Value);
                    if (aggregatedBalanceInCents != 0)
                    {
                        result.UpdateBalanceCommands.Add(new Ledger_Account_AggregatedBalanceUpdateCommand
                        {
                            UserId = dto.UserId,
                            LedgerId = ledger.Id,
                            AccountId = cmd.AccountId,
                            AccountName = cmd.Name,
                            OldValueInCents = 0,
                            BalanceInCents = aggregatedBalanceInCents,
                            Date = DateTime.Now,
                        });
                    }

                    result.SetStatusCommands.Add(new Ledger_Account_AggregationStatus_UpdateCommand
                    {
                        AccountId = cmd.AccountId,
                        LedgerId = cmd.LedgerId,
                        NewStatus = AggregatedAccountStatusEnum.BeginPullingTransactions,
                        Date = DateTime.Now,
                    });

                    result.IntuitAccountsIds.Add(intuitAddedAccountId);
                }
            }

            return result;
        }

        public AuthenticateToInstitutionViewModel Load(AuthenticateDto input)
        {
            var institution = _aggregation.GetInstitution(input.InstitutionId);
            var meta = new Metadata {LogonId = input.UserId, IsLoggingEnabled = input.AggregationLoggingEnabled};
            return new AuthenticateToInstitutionViewModel
            {
                Keys = _aggregation.GetInstitutionKeys(meta, input.InstitutionId).OrderBy(x => x.DisplayOrder).ToList(),
                InstitutionId = input.InstitutionId,
                InstitutionName = institution.Name,
            };
        }

        public ReauthenticateToInstitutionViewModel Load(ReauthenticateDto input)
        {
            var institution = _aggregation.GetInstitution(input.InstitutionId);
            var meta = new Metadata { LogonId = input.UserId, IsLoggingEnabled = input.AggregationLoggingEnabled };
            return new ReauthenticateToInstitutionViewModel
            {
                Keys = _aggregation.GetInstitutionKeys(meta, input.InstitutionId).OrderBy(x => x.DisplayOrder).ToList(),
                InstitutionId = input.InstitutionId,
                InstitutionName = institution.Name,
                IntuitAccountId = input.IntuitAccountId
            };
        }

        public RefreshAccountResult Load(RefreshAccountDto dto)
        {
            var result = new RefreshAccountResult();
            var ledger = _ledgerService.GetById(dto.LedgerId);
            if (ledger != null)
            {
                var ledgerAccount = ledger.Accounts.FirstOrDefault(x => x.IntuitAccountId == dto.IntuitAccountId);
                if (ledgerAccount != null && ledgerAccount.AggregatedAccountStatus != AggregatedAccountStatusEnum.PullingTransactions)
                {
                    result.SetStatusCommand = new Ledger_Account_AggregationStatus_UpdateCommand
                    {
                        AccountId = ledgerAccount.Id,
                        LedgerId = dto.LedgerId,
                        NewStatus = AggregatedAccountStatusEnum.BeginPullingTransactions,
                        Date = DateTime.Now,
                    };

                    result.PullTransactions = true;
                }
            }
            return result;
        }

        private static AggregatedAccountType ConvertLabelToType(AccountLabelEnum label)
        {
            switch (label)
            {
                case AccountLabelEnum.Bank:
                    return AggregatedAccountType.Checking;
                case AccountLabelEnum.CreditCard:
                    return AggregatedAccountType.CreditCard;
                case AccountLabelEnum.Loan:
                    return AggregatedAccountType.Loan;
                case AccountLabelEnum.Investment:
                    return AggregatedAccountType.Investment;
            }

            throw new Exception("Label Type not Supported");
        }
    }
}