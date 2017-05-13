using System;
using System.Collections.Generic;
using System.Linq;
using Default.Factories.Commands;
using Default.Factories.ViewModels;
using Default.ViewModel.AccountsController;
using Default.ViewModel.Areas.Business.AccountsController;
using Default.ViewModel.Areas.Business.BusinessController;
using Default.ViewModel.Areas.Finance.GoalsController;
using Default.ViewModel.Areas.Shared;
using Default.ViewModel.RealestateController;
using mPower.Aggregation.Client;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Goal;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.Helpers;
using mPower.WebApi.Tenants.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paralect.Domain;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class AccountsController : BaseController
    {
        private readonly IObjectRepository _objectRepository;
        private readonly AccountsService _accountService;
        private readonly LedgerDocumentService _ledgerService;
        private readonly UserDocumentService _userService;
        private readonly GoalDocumentService _goalService;
        private readonly IAggregationClient _aggregationClient;

        public AccountsController(IObjectRepository objectRepository, AccountsService accountService,
            LedgerDocumentService ledgerService, UserDocumentService userService, GoalDocumentService goalService,
            IAggregationClient aggregationClient, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _objectRepository = objectRepository;
            _accountService = accountService;
            _ledgerService = ledgerService;
            _userService = userService;
            _goalService = goalService;
            _aggregationClient = aggregationClient;
        }


        [HttpGet]
        public AccountsSidebarModel GetUserAccounts(string ledgerId = null)
        {
            if (string.IsNullOrEmpty(ledgerId)) ledgerId = GetLedgerId();
            var accountSidebarModel = _objectRepository.Load<string, AccountsSidebarModel>(ledgerId);
            GetRealEstates(accountSidebarModel);
            accountSidebarModel.ManualAndAggregatedRealEstateTotalsInDollars = accountSidebarModel.RealEstates.Items.Sum(c => c.Value);
            accountSidebarModel.ManualAndAggregatedEquity += accountSidebarModel.ManualAndAggregatedRealEstateTotalsInDollars;  
               
            return accountSidebarModel;
        }

        [HttpGet("getAccounts")]
        public ChartOfAccountsViewModel GetAccounts()
        {
            return _objectRepository.Load<string, ChartOfAccountsViewModel>(GetLedgerId());
        }

        [HttpGet("add")]
        public IDictionary<string, string> GetDataForAdding()
        {
            return new AddManualAccountViewModel().AccountLabels;
        }

        [HttpGet("edit/{id}")]
        public EditManualAccountViewModel Edit(string id)
        {
            var model = _objectRepository
                .Load<EditAccountFilter, EditManualAccountViewModel>
                (new EditAccountFilter
                {
                    LedgerId = GetLedgerId(),
                    AccountId = id
                });

            return model;
        }

        [HttpPost("edit")]
        public void Edit([FromBody]EditManualAccountViewModel model)
        {
            var cmd = new Ledger_Account_UpdateCommand
            {
                AccountId = model.Id,
                LedgerId = GetLedgerId(),
                Description = model.Description ?? String.Empty,
                Name = model.Name,
                Number = model.Number,
                ParentAccountId = model.ParentAccountId,
                InstitutionName = model.InstitutionName,
                InterestRatePerc = model.InterestRatePercentage,
                MinMonthPaymentInCents = AccountingFormatter.DollarsToCents(model.MinMonthPaymentInDollars),
                CreditLimitInCents = AccountingFormatter.DollarsToCents(model.CreditLimitInDollars),
            };

            Send(cmd);
        }

        [HttpGet("confirmDelete/{id}")]
        public ConfirmDeleteModel ConfirmDelete(string id)
        {
            var ledger = _ledgerService.GetById(GetLedgerId());
            var account = ledger.Accounts.SingleOrDefault(x => x.Id == id);
            if(account == null)
            {
                ModelState.AddModelError(id, "Account is already deleted.");
            }

            var hasTransactions = GetAccountDeleteCommands(id).Count(x => (x as Transaction_DeleteMultipleCommand) != null) == 1;
            var model = new ConfirmDeleteModel
            {
                AccountId = id,
                AccountName = account.Name,
                DisplayHasTransactionsMessage = hasTransactions,
                IsLinked = IsUsedGoalsLinkedAccount(id)
            };

            return model;
        }

        [HttpGet("delete/{id}")]
        public IActionResult Delete(string id)
        {
            var ledger = _ledgerService.GetById(GetLedgerId());
            var account = ledger.Accounts.SingleOrDefault(x => x.Id == id);
            if(account == null)
            {
                ModelState.AddModelError(id, "Account is already deleted.");
                return new BadRequestObjectResult(ModelState);
            }
            if(IsUsedGoalsLinkedAccount(id))
            {
                ModelState.AddModelError("Error", "Goals linked account can't be deleted while any goal exists.");
                return new BadRequestObjectResult(ModelState);
            }

            var commands = GetAccountDeleteCommands(id);
            Send(commands.ToArray());
            return new OkResult();
        }

        [HttpPost("add")]
        public List<ShortAccountViewModel> AddAccountManually([FromBody]AddManualAccountViewModel model)
        {
            var cmds = _accountService.CreateAccountCommands(model.Name,
                GetUserId(),
                GetLedgerId(),
                model.Label,
                model.ParentAccountId,
                model.Description,
                model.Number,
                model.InterestRatePercentage ?? 0,
                model.MinMonthPaymentInDollars,
                model.CreditLimitInDollars,
                model.OpeningBalance).ToArray();

            Send(cmds);
            var account = (Ledger_Account_CreateCommand)cmds[0];

            return new List<ShortAccountViewModel>
            {
                new ShortAccountViewModel
                {
                    AccountId = account.AccountId,
                    LedgerId = account.LedgerId,
                    Name = account.Name,
                    Type = account.AccountTypeEnum.GetIifName(),
                    Group = AccountingFormatter.GenericCategoryGroup(account.AccountLabelEnum)
                }
            };
        }

        [HttpGet("get/goals")]
        public SetupLinkedAccountModel GetLinkedAccounts()
        {
            var availableAccounts = new List<AccountLabelEnum>
            {
                AccountLabelEnum.Bank,
                AccountLabelEnum.Investment
            };
            var model = new SetupLinkedAccountModel();

            var ledger = _ledgerService.GetById(GetLedgerId());
            if (ledger != null)
            {
                model.Accounts = ledger.Accounts
                    .Where(a => availableAccounts.Contains(a.LabelEnum) &&
                                a.Id != BaseAccounts.UnknownCash)
                    .Select(a => new GoalsLinkedAccount
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        BalanceInCents = a.ActualBalance,
                        Group = a.LabelEnum.GetDescription(),
                    }).ToList();
            }

            var user = _userService.GetById(GetUserId());
            if (user != null)
            {
                model.LinkedAccountId = user.GoalsLinkedAccount.AccountId;
            }

            return model;
        }

        [HttpGet("get/financial")]
        public IEnumerable<FinancialAccountViewModel> GetFinancialAccounts()
        {
            var ledger = _ledgerService.GetByUserId(GetUserId()).Single(x => x.TypeEnum == LedgerTypeEnum.Personal);

            var aggregatedAccounts = ledger.Accounts.Where(x => x.IsAggregated);
            var manualAccounts = ledger.Accounts.Where(x =>
                !x.IsAggregated &&
                !x.IsUnknownCash &&
                (x.LabelEnum == AccountLabelEnum.Bank ||
                 x.LabelEnum == AccountLabelEnum.Loan ||
                 x.LabelEnum == AccountLabelEnum.CreditCard ||
                 x.LabelEnum == AccountLabelEnum.Investment)); // currently, show in Profile only these account types created manually 

            var accounts = aggregatedAccounts.Concat(manualAccounts); //merge aggregated and manual accounts into single collection

            return accounts.Select(account =>
            {

                var accountModel = new FinancialAccountViewModel
                {
                    AccountId = account.Id,
                    AccountName = account.Name,
                    AmountInCents = account.ActualBalance,
                    InstitutionName = account.InstitutionName,
                    InterestRate = account.InterestRatePerc / 100f,
                    AvailableBalanceInCents = account.CreditLimitInCents > 0 ? account.CreditLimitInCents - account.ActualBalance : (long?)null,
                };
                if (account.IntuitInstitutionId.HasValue)
                {
                    var aggregationHelper = new AggregationHelper(_aggregationClient, GetUserId(), GetLedgerId());
                    var institution = aggregationHelper.GetInstitution(account.IntuitInstitutionId.Value);
                    if (institution != null)
                    {
                        accountModel.InstitutionName = institution.Name;
                        accountModel.Website = institution.HomeUrl;
                        accountModel.Phone = institution.Phone;
                    }
                }
                return accountModel;
            });
        }

        [HttpGet("parents")]
        public IDictionary<string, string> GetParentAccounts(AccountLabelEnum accountLabel, string id = null)
        {
            var accounts = _ledgerService.GetById(GetLedgerId()).Accounts;
            var selectAccount = accounts.FirstOrDefault(x => x.LabelEnum == accountLabel);
            if(selectAccount == null) return new Dictionary<string, string>();

            return _objectRepository.Load<ParentAccountsFilter, IDictionary<string, string>>(new ParentAccountsFilter
            {
                Accounts = accounts,
                AccountLabel = accountLabel,
                Id = id
            });
        }

        #region Helpers

        private bool IsUsedGoalsLinkedAccount(string accountId)
        {
            var user = _userService.GetById(GetUserId());
            return user.GoalsLinkedAccount.AccountId == accountId && user.GoalsLinkedAccount.LedgerId == GetLedgerId()
                && _goalService.Count(new GoalFilter { UserId = user.Id }) > 0;
        }

        private IEnumerable<Command> GetAccountDeleteCommands(string accountId)
        {
            TempData.Keep();

            var ledger = _ledgerService.GetById(GetLedgerId());
            var account = ledger.Accounts.Find(a => a.Id == accountId);

            if(account == null || BaseAccounts.All().Select(x => x.ToLower()).Contains(accountId.ToLower()))
                throw new Exception("Some bad man trying to delete system account or not existing one: " + accountId);

            var commands = _objectRepository.Load<DeleteAccountCommadsFilter, IEnumerable<Command>>(new DeleteAccountCommadsFilter
            {
                AccountId = accountId,
                LedgerId = GetLedgerId()
            });

            return commands;
        }

        private void GetRealEstates(AccountsSidebarModel accountSidebarModel)
        {
            var user = _userService.GetById(GetUserId());
            if (user != null)
            {
                accountSidebarModel.RealEstates.Items = user.Realestates.Select(r =>
                    new PropertyModel
                    {
                        Id = r.Id,
                        Name = r.Name,
                        Value = AccountingFormatter.CentsToDollars(r.AmountInCents),
                        IsIncludedInWorth = r.IsIncludedInWorth
                    }).Where(c => c.IsIncludedInWorth).ToList();
            }
        }

        #endregion
    }
}