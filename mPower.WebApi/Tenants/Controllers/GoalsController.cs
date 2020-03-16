using System;
using System.Collections.Generic;
using System.Linq;
using Default.Areas.Finance.Models;
using Default.ViewModel.Areas.Finance.GoalsController;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.Documents.Goal;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Goal;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.Enums;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Goal.Commands;
using mPower.Domain.Membership.User.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class GoalsController : BaseController
    {
        private readonly List<AccountLabelEnum> _allowedLinkedAccountLabels = new List<AccountLabelEnum>
        {
            AccountLabelEnum.Bank,
            AccountLabelEnum.Investment
        };
        private readonly IIdGenerator _idGenerator;
        private readonly UserDocumentService _userService;
        private readonly GoalDocumentService _goalService;
        private readonly LedgerDocumentService _ledgerService;
        private AccountDocument _linkedAccount;

        public GoalsController(IIdGenerator idGenerator, UserDocumentService userService, 
            GoalDocumentService goalService, LedgerDocumentService ledgerService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _idGenerator = idGenerator;
            _userService = userService;
            _goalService = goalService;
            _ledgerService = ledgerService;
        }

        private AccountDocument LinkedAccount
        {
            get
            {
                if (_linkedAccount == null && !string.IsNullOrEmpty(GetUserId()))
                {
                    var user = _userService.GetById(GetUserId());
                    var linkedAccountId = user.GoalsLinkedAccount.AccountId;
                    _linkedAccount = _ledgerService.GetById(GetLedgerId()).Accounts.Find(a => a.Id == linkedAccountId);
                }
                return _linkedAccount;
            }
        }

        [HttpPost("create")]
        public void CreateGoal([FromBody]GoalViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Id))
            {
                // update existing goal
                var filter = new GoalFilter { UserId = GetUserId() };
                var goals = _goalService.GetByFilter(filter);
                var goal = goals.Find(g => g.GoalId == model.Id && g.Status == GoalStatusEnum.Projected);
                if (goal != null)
                {
                    var command = new Goal_UpdateCommand
                    {
                        GoalId = goal.GoalId,
                        MonthlyPlanAmountInCents = AccountingFormatter.DollarsToCents(model.MonthlyContribution),
                        PlannedDate = model.PlannedDate,
                        ProjectedDate = model.ProjectedDate ?? model.PlannedDate,
                        Title = model.Title,
                        TotalAmountInCents = AccountingFormatter.DollarsToCents(model.Amount),
                        StartingBalanceInCents = AccountingFormatter.DollarsToCents(model.StartingBalance)
                    };
                    Send(command);
                }
            }
            else
            {
                // create new goal
                var command = new Goal_CreateCommand
                {
                    GoalId = _idGenerator.Generate(),
                    MonthlyPlanAmountInCents = AccountingFormatter.DollarsToCents(model.MonthlyContribution),
                    PlannedDate = model.PlannedDate,
                    StartDate = model.StartDate,
                    ProjectedDate = model.ProjectedDate ?? model.PlannedDate,
                    Title = model.Title,
                    Type = model.Type,
                    TotalAmountInCents = AccountingFormatter.DollarsToCents(model.Amount),
                    UserId = GetUserId(),
                    StartingBalanceInCents = AccountingFormatter.DollarsToCents(model.StartingBalance)
                };
                Send(command);
            }
        }

        [HttpDelete("delete/{goalId}")]
        public IActionResult Delete(string goalId)
        {
            var filter = new GoalFilter { UserId = GetUserId() };
            var goals = _goalService.GetByFilter(filter);
            if (goals.Any(g => g.GoalId == goalId))
            {
                Send(new Goal_DeleteCommand { GoalId = goalId });
                return new OkResult();
            }
            ModelState.AddModelError("goalId", "Can't find specified goal.");
            return new BadRequestObjectResult(ModelState);
        }

        [HttpPost("complete/{goalId}")]
        public IActionResult Complete(string goalId)
        {
            var filter = new GoalFilter { UserId = GetUserId() };
            var goals = _goalService.GetByFilter(filter);
            if (goals.Any(g => g.GoalId == goalId))
            {
                Send(new Goal_MarkAsCompletedCommand { GoalId = goalId });
                return GetById(goalId);
            }
            ModelState.AddModelError("goalId", "Can't find specified goal.");
            return new BadRequestObjectResult(ModelState);
        }


        [HttpPost("archive/{goalId}")]
        public IActionResult Archive(string goalId)
        {
            var filter = new GoalFilter { UserId = GetUserId() };
            var goals = _goalService.GetByFilter(filter);
            if (goals.Any(g => g.GoalId == goalId))
            {
                Send(new Goal_ArchiveCommand { GoalId = goalId });
                return new OkResult();
            }
            ModelState.AddModelError("goalId", "Can't find specified goal.");
            return new BadRequestObjectResult(ModelState);
        }

        [HttpGet("editmodel/{goalId}")]
        public IActionResult GetEditModel(string goalId)
        {
            var filter = new GoalFilter { UserId = GetUserId() };
            var goals = _goalService.GetByFilter(filter);
            var goal = goals.Find(g => g.GoalId == goalId && g.Status == GoalStatusEnum.Projected);
            if (goal == null)
            {
                ModelState.AddModelError("goalId", "Can't find specified goal.");
                return new BadRequestObjectResult(ModelState);
            }

            return new OkObjectResult(new GoalViewModel
            {
                Id = goal.GoalId,
                Title = goal.Title,
                Amount = AccountingFormatter.CentsToDollars(goal.TotalAmountInCents),
                Contributed = AccountingFormatter.CentsToDollars(goal.CurrentAmountInCents),
                Type = goal.Type,
                MonthlyContribution = AccountingFormatter.CentsToDollars(goal.MonthlyPlanAmountInCents),
                StartDate = goal.StartDate,
                PlannedDate = goal.PlannedDate,
                BackLinkAction = string.Empty,
                StartingBalance = AccountingFormatter.CentsToDollars(goal.StartingBalanceInCents)
            });
        }


        [HttpGet("all")]
        public GoalsListingModel GetGoals(GoalStatusEnum type = GoalStatusEnum.Projected)
        {
            var filter = new GoalFilter { UserId = GetUserId() };
            var goals = _goalService.GetByFilter(filter);
            return new GoalsListingModel
            {
                ActiveGoalsNumber = goals.Count(g => g.Status == GoalStatusEnum.Projected),
                AvailableAmountInDollars = GetAvailableAmountInDollars(goals),
                CompletedItems =
                    type == GoalStatusEnum.Projected
                        ? goals.Where(g => g.Status == GoalStatusEnum.Completed).Select(MapToListingItem).ToList()
                        : null,
                ActiveItems =
                    type == GoalStatusEnum.Projected
                        ? goals.Where(g => g.Status == GoalStatusEnum.Projected).Select(MapToListingItem).ToList()
                        : null,
                ArchivedItems =
                    type == GoalStatusEnum.Archived
                        ? goals.Where(g => g.Status == GoalStatusEnum.Archived).Select(MapToListingItem).ToList()
                        : null,
            };
        }

        [HttpGet("get/{id}")]
        public IActionResult GetById(string id)
        {
            var filter = new GoalFilter { UserId = GetUserId() };
            var goals = _goalService.GetByFilter(filter);
            var selectedGoal = goals.Find(g => g.GoalId == id);

            if (selectedGoal == null)
            {
                ModelState.AddModelError("", "Specified goal cannot be found.");
                return new BadRequestObjectResult(ModelState);
            }

            CheckCalcData(selectedGoal);
            return new OkObjectResult(new GoalDetailedModel
            {
                AvailableAmountInDollars = GetAvailableAmountInDollars(goals),
                Id = selectedGoal.GoalId,
                Title = selectedGoal.Title,
                Status = selectedGoal.Status,
                PlannedDate = selectedGoal.PlannedDate,
                ProjectedDate = selectedGoal.ProjectedDate,
                TotalAmountInDollars = CentsToRoundedDollars(selectedGoal.TotalAmountInCents),
                CurrentAmountInDollars = CentsToRoundedDollars(selectedGoal.CurrentAmountInCents),
                MonthlyActualAmountInDollars = GetCurrentMonthAmountInDollars(selectedGoal),
                MonthsAheadNumber = selectedGoal.MonthsAheadNumber,
                StartingBalanceInDollars = CentsToRoundedDollars(selectedGoal.StartingBalanceInCents),
            });
        }

        [HttpPost("setup/linkedaccount")]
        public IActionResult SetupLinkedAccount(string linkedAccountId)
        {
            if (string.IsNullOrEmpty(linkedAccountId))
            {
                ModelState.AddModelError("linkedAccountId", "Can't find account with specified id");
                return new BadRequestObjectResult(ModelState);
            }

            var ledger = _ledgerService.GetById(GetLedgerId());
            var account = ledger.Accounts.Find(a => a.Id == linkedAccountId);
            if (account != null && _allowedLinkedAccountLabels.Contains(account.LabelEnum))
            {
                var cmd = new User_GoalsLinkedAccount_SetCommand
                {
                    UserId = GetUserId(),
                    LedgerId = ledger.Id,
                    AccountId = account.Id,
                };

                Send(cmd);
                return new OkResult();
            }

            return new BadRequestObjectResult(ModelState);
        }

        [HttpPost("adjustamount")]
        public IActionResult AdjustCurrentAmount(string goalId, long adjustment)
        {
            var filter = new GoalFilter { UserId = GetUserId()};
            var goals = _goalService.GetByFilter(filter);
            var availableAmountInDollars = GetAvailableAmountInDollars(goals);

            if (LinkedAccount != null && goals.Any(g => g.GoalId == goalId && g.Status == GoalStatusEnum.Projected) && (adjustment < 0 || adjustment <= availableAmountInDollars))
            {
                var cmd = new Goal_AdjustCurrentAmountCommand
                {
                    GoalId = goalId,
                    ValueInCents = adjustment * 100,
                    Date = DateTime.Now,
                };
                Send(cmd);
            }
            else
                return new BadRequestResult();

            return new OkResult();
        }

        #region Helper Methods

        private GoalsListItemModel MapToListingItem(GoalDocument doc)
        {
            CheckCalcData(doc);
            return new GoalsListItemModel
            {
                Id = doc.GoalId,
                Title = doc.Title,
                Status = doc.Status,
                PlannedDate = doc.PlannedDate,
                TotalAmountInDollars = CentsToRoundedDollars(doc.TotalAmountInCents),
                CurrentAmountInDollars = CentsToRoundedDollars(doc.CurrentAmountInCents),
                MonthlyPlanAmountInDollars = CentsToRoundedDollars(doc.MonthlyPlanAmountInCents),
                MonthlyActualAmountInDollars = GetCurrentMonthAmountInDollars(doc),
                MonthsAheadNumber = doc.MonthsAheadNumber,
                ImageName = GetListingImageName(doc),
                StartingBalanceInDollars = CentsToRoundedDollars(doc.StartingBalanceInCents)
            };
        }

        private GoalSideBarItemModel MapToSideBarItem(GoalDocument doc, string activeGoalId)
        {
            return new GoalSideBarItemModel
            {
                Id = doc.GoalId,
                Title = doc.Title,
                Status = doc.Status,
                PlannedDate = doc.PlannedDate,
                ImageName = GetListingImageName(doc),
                IsActive = doc.GoalId == activeGoalId,
            };
        }

        private long? GetAvailableAmountInDollars(IEnumerable<GoalDocument> goals)
        {
            if (LinkedAccount == null) return null;

            var balance = LinkedAccount.IsAggregated ? LinkedAccount.AggregatedBalance : LinkedAccount.ActualBalance;

            return CentsToRoundedDollars(balance - goals.Where(g => g.Status == GoalStatusEnum.Projected).Sum(g => g.CurrentAmountInCents));
        }

        private void CheckCalcData(GoalDocument doc)
        {
            var now = DateTime.Now;
            var currMonth = new DateTime(now.Year, now.Month, 1);
            if (doc.Status == GoalStatusEnum.Projected && (!doc.CalcDate.HasValue || doc.CalcDate.Value < currMonth))
            {
                UpdateCalcData(doc);
            }
        }

        private void UpdateCalcData(GoalDocument doc)
        {
            var now = DateTime.Now;
            var currMonth = new DateTime(now.Year, now.Month, 1);
            var startMonth = new DateTime(doc.StartDate.Year, doc.StartDate.Month, 1);
            var endMonth = new DateTime(doc.PlannedDate.Year, doc.PlannedDate.Month, 1);

            long plannedAmount = 0;
            for (var month = startMonth; month < currMonth; month = month.AddMonths(1))
            {
                if (month == startMonth)
                {
                    var endDate = month == endMonth ? doc.PlannedDate : month.AddMonths(1);
                    double startMonthAmount = (endDate - doc.StartDate).TotalDays / DateTime.DaysInMonth(month.Year, month.Month) * doc.MonthlyPlanAmountInCents;
                    plannedAmount += (long)Math.Round(startMonthAmount, 0);
                }
                else if (month == endMonth)
                {
                    double endMonthAmount = (doc.PlannedDate - month).TotalDays / DateTime.DaysInMonth(month.Year, month.Month) * doc.MonthlyPlanAmountInCents;
                    plannedAmount += (long)Math.Round(endMonthAmount, 0);
                }
                else
                {
                    plannedAmount += doc.MonthlyPlanAmountInCents;
                }
            }

            var actualAmount = doc.CurrentAmountInCents;
            var currentMonthAmountActual = GetCurrentMonthAmountInDollars(doc);
            var currentMonthPart = ((currMonth == endMonth ? doc.PlannedDate : currMonth.AddMonths(1)) -
                                    (currMonth == startMonth ? doc.StartDate : currMonth)).TotalDays
                                    / DateTime.DaysInMonth(currMonth.Year, currMonth.Month);
            var currentMonthAmountPlanned = (long)Math.Round(doc.MonthlyPlanAmountInCents * currentMonthPart, 0);
            actualAmount -= Math.Min(currentMonthAmountActual, currentMonthAmountPlanned);

            doc.MonthsAheadNumber = doc.MonthlyPlanAmountInCents == 0 ? 0 :(int)((actualAmount - plannedAmount) / doc.MonthlyPlanAmountInCents);
            doc.CalcDate = now;

            _goalService.UpdateCalculatedData(doc.UserId, doc.GoalId, doc.MonthsAheadNumber, doc.CalcDate.Value);
        }

        private static long GetCurrentMonthAmountInDollars(GoalDocument doc)
        {
            var now = DateTime.Now;
            var currMonth = new DateTime(now.Year, now.Month, 1);
            return currMonth <= doc.LatestAdjustmentDate ? CentsToRoundedDollars(doc.LatestMonthAdjustmentInCents) : 0;
        }

        private static string GetListingImageName(GoalDocument doc)
        {
            switch (doc.Type)
            {
                case GoalTypeEnum.Emergency:
                    return "goal-emergency.png";
                case GoalTypeEnum.Retirement:
                    return "goal-retirement.png";
                case GoalTypeEnum.BuyHome:
                    return "goal-house.png";
                case GoalTypeEnum.BuyCar:
                    return "goal-car.png";
                case GoalTypeEnum.College:
                    return "goal-college.png";
                case GoalTypeEnum.Trip:
                    return "goal-trip.png";
                case GoalTypeEnum.ImproveHome:
                    return "goal-home-improvement.png";

                default:
                    return "goal-avatar-example.png";
            }
        }

        private static long CentsToRoundedDollars(long cents)
        {
            return (long)Math.Round(cents / 100M, 0);
        }

        private SetupLinkedAccountModel GetLinkedAccountSetupModel(bool fromGoalCreationWizard = false)
        {
            var model = new SetupLinkedAccountModel { FromGoalCreationWizard = fromGoalCreationWizard };

            var ledger = _ledgerService.GetById(GetLedgerId());
            if (ledger != null)
            {
                model.Accounts = ledger.Accounts
                    .Where(a => _allowedLinkedAccountLabels.Contains(a.LabelEnum) &&
                        a.Id != BaseAccounts.UnknownCash)
                    .Select(a => new GoalsLinkedAccount
                    {
                        Id = a.Id,
                        Name = a.Name,
                        Description = a.Description,
                        BalanceInCents = a.AggregatedBalance,
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

        #endregion
    }
}