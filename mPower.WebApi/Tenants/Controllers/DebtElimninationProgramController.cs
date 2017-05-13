using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Default.ViewModel.Areas.Finance.DebtElimninationProgramController;
using mPower.Documents;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Calendar.Commands;
using mPower.Domain.Accounting.Calendar.Data;
using mPower.Domain.Accounting.DebtElimination.Commands;
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.Enums;
using mPower.EventHandlers.Immediate;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.Services;
using mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Step1Model = mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram.Step1Model;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/debtelimination")]
    public class DebtElimninationProgramController : BaseController
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly DebtViewModelBuilder _debtViewModelBuilder;
        private readonly IIdGenerator _idGenerator;
        private readonly CalendarDocumentService _calendarService;
        private readonly DebtCalculator _calculator;

        public DebtElimninationProgramController(LedgerDocumentService ledgerService, DebtEliminationDocumentService debtEliminationService,
            DebtViewModelBuilder debtViewModelBuilder, IIdGenerator idGenerator, CalendarDocumentService calendarService,
            DebtCalculator calculator, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _ledgerService = ledgerService;
            _debtEliminationService = debtEliminationService;
            _debtViewModelBuilder = debtViewModelBuilder;
            _idGenerator = idGenerator;
            _calendarService = calendarService;
            _calculator = calculator;
        }

        [HttpGet("step1")]
        public Step1Model Step1()
        {
            var debtId = CreateDebtIfNotExists();
            var debt = _debtEliminationService.GetById(debtId);
            var debtsIds = debt.Debts.Select(d => d.DebtId).ToList();
            var model = new Step1Model();
            var ledger = _ledgerService.GetById(GetLedgerId());
            model.Debts =
                ledger.Accounts.Where(
                    x => x.LabelEnum == AccountLabelEnum.CreditCard || x.LabelEnum == AccountLabelEnum.Loan).Select(x =>
                        new DebtModel
                        {
                            Id = x.Id,
                            Name = x.Name,
                            LabelEnum = x.LabelEnum,
                            Balance = x.ActualBalance,
                            InterestRatePerc = x.InterestRatePerc,
                            MinMonthPaymentInCents = x.MinMonthPaymentInCents,
                            UseInProgram = debtsIds.Contains(x.Id),
                        }).ToList();
            return model;
        }

        [HttpPost("proceedstep2")]
        public IActionResult ProceedToStep2([FromBody]DebtIdsModel model)
        {
            var meaningfulDebts = GetMeaningfulDebts(model.DebtIds);
            if (!DebtsAreValid(meaningfulDebts))
            {
                return new BadRequestObjectResult(ModelState);
            }

            var command = new DebtElimination_Debts_SetCommand
            {
                DebtEliminationId = CreateDebtIfNotExists(),
                Debts = meaningfulDebts,
            };

            Send(command);

            return new OkResult();
        }

        [HttpGet("step2")]
        public Step2Model Step2()
        {
            var debtId = CreateDebtIfNotExists();

            var debt = _debtEliminationService.GetById(debtId);
            var model = new Step2Model
            {
                MonthlyBudget = AccountingFormatter.CentsToDollars(debt.MonthlyBudgetInCents),
                RecommendedBudgetInCents = (long) Math.Round(_ledgerService.GetById(GetLedgerId()).Accounts
                    .Where(
                        x =>
                            (x.LabelEnum == AccountLabelEnum.CreditCard || x.LabelEnum == AccountLabelEnum.Loan) &&
                            x.ActualBalance > 0)
                    .Sum(x => x.MinMonthPaymentInCents)*0.2, 0),
                Plan = ((int) debt.PlanId).ToString(CultureInfo.InvariantCulture),
                Plans = new List<SelectListItem> {new SelectListItem {Text = "Please Choose...", Value = "0"}}
            };
            // + 20% over all min monthly payments 
            var plans = Enum.GetValues(typeof(DebtEliminationPlanEnum)).Cast<DebtEliminationPlanEnum>()
                .Except(new[] { DebtEliminationPlanEnum.NotInitialized })
                .Select(e => new SelectListItem { Text = e.GetDescription(), Value = ((int)e).ToString(CultureInfo.InvariantCulture) });
            model.Plans.AddRange(plans);

            return model;
        }

        [HttpPost("proceedstep3")]
        public IActionResult ProceedToStep3([FromBody]SelectPlanModel model)
        {
            if (model.Plan == DebtEliminationPlanEnum.NotInitialized || model.MonthlyBudget < 0)
            {
                ModelState.AddModelError("Budget", "Please, specify program and positive budget.");
                return new BadRequestObjectResult(ModelState);
            }

            var command = new DebtElimination_EliminationPlanUpdateCommand
            {
                Id = CreateDebtIfNotExists(),
                MonthlyBudgetInCents = AccountingFormatter.DollarsToCents(model.MonthlyBudget),
                PlanId = model.Plan
            };

            Send(command);
            return new OkResult();
        }

        [HttpPost("updatecharts")]
        public Step3Model UpdateChart([FromBody]int displayModeKey)
        {
            var displayMode = (DebtEliminationDisplayModeEnum) displayModeKey;
            var debt = _debtEliminationService.GetById(CreateDebtIfNotExists());
            debt.DisplayMode = displayMode;

            var command = new DebtElimination_DisplayMode_UpdateCommand
            {
                Id = debt.Id,
                DisplayMode = displayMode,
            };

            Send(command);

            return _debtViewModelBuilder.GetDebtEliminationStep3Model(debt);
        }

        [HttpGet("step3")]
        public Step3Model Step3()
        {
            var debtId = CreateDebtIfNotExists();
            var debt = _debtEliminationService.GetById(debtId);

            return _debtViewModelBuilder.GetDebtEliminationStep3Model(debt);
        }

        [HttpPost("addtocalendar")]
        public IActionResult AddToCalendar()
        {
            var debt = _debtEliminationService.GetById(CreateDebtIfNotExists());
            var calendar = _calendarService.GetByFilter(new CalendarFilter { LedgerId = GetLedgerId() }).OrderBy(c => c.Type).FirstOrDefault();
            if (debt != null && calendar != null)
            {
                var details = _calculator.CalcAcceleratedDetails(debt);

                var onetimeCommands = details.Select(detailsItem =>
                    new Calendar_OnetimeCalendarEvent_CreateCommand
                    {
                        CalendarEventId = _idGenerator.Generate(),
                        CalendarId = calendar.Id,
                        CreatedDate = DateTime.Now,
                        EventDate = detailsItem.Date.AddDays(-1), // warning a day before payment
                        Description = CalendarDocumentEventHandler.GetEliminationItemDescription(detailsItem),
                        IsDone = false,
                        SendAlertOptions = new SendAlertOption
                        {
                            Mode = AlertModeEnum.Email,
                        },
                        ParentId = debt.Id,
                        UserId = debt.UserId,
                    }).ToList();

                Send(onetimeCommands.ToArray());
                return new OkResult();
            }
            return new BadRequestResult();
        }
        

        #region Helpers

        private bool DebtsAreValid(ICollection<DebtItemData> meaningfulDebts)
        {
            if (meaningfulDebts.Count == 0)
            {
                ModelState.AddModelError("Debts", "There are no selected debts suitable for calculation: it should has balance more then zero.");
                return false;
            }
            if (!meaningfulDebts.All(x => x.InterestRatePerc > 0 && x.MinMonthPaymentInCents > 0))
            {
                ModelState.AddModelError("Debts", "All selected debts with a positive balance should have an interest rate and minimum monthly payment greater than zero.");
                return false;
            }
            if (!meaningfulDebts.All(HasValidMinMonthlyPayment))
            {
                ModelState.AddModelError("Debts", "Some of selected debts have minimum monthly payment less then its interest payment.");
                return false;
            }

            return true;
        }

        private string CreateDebtIfNotExists()
        {
            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            string debtId;

            if (debt == null)
            {
                debtId = _idGenerator.Generate();
                var command = new DebtElimination_CreateCommand
                {
                    Id = debtId,
                    LedgerId = GetLedgerId(),
                    UserId = GetUserId(),
                };

                Send(command);
            }
            else
                debtId = debt.Id;

            return debtId;
        }

        private List<DebtItemData> GetMeaningfulDebts(ICollection<string> debtsIds)
        {
            var ledger = _ledgerService.GetById(GetLedgerId());
            var meaningfulDebts = ledger.Accounts.Where(x => debtsIds.Contains(x.Id) && x.ActualBalance > 0).Select(x =>
                new DebtItemData
                {
                    DebtId = x.Id,
                    Name = x.Name,
                    BalanceInCents = x.ActualBalance,
                    InterestRatePerc = x.InterestRatePerc,
                    MinMonthPaymentInCents = x.MinMonthPaymentInCents
                }).ToList();
            return meaningfulDebts;
        }

        private static bool HasValidMinMonthlyPayment(DebtItemData debt)
        {
            return debt.MinMonthPaymentInCents > debt.BalanceInCents * debt.InterestRatePerc / 12 / 100;
        }

        #endregion
    }
}