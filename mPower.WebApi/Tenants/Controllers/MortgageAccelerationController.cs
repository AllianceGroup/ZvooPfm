using System;
using System.Collections.Generic;
using System.Linq;
using Default.Areas.Finance.Models;
using Default.ViewModel.Areas.Finance.MortgageAcceleration;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Calendar;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.DebtElimination.Commands;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.Attributes;
using mPower.WebApi.Tenants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Paralect.Domain;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class MortgageAccelerationController : BaseController
    {
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly IIdGenerator _idGenerator;
        private readonly DebtViewModelBuilder _debtBuilder;
        private readonly CalendarDocumentService _calendarService;

        public MortgageAccelerationController(DebtEliminationDocumentService debtEliminationService, 
            IIdGenerator idGenerator, DebtViewModelBuilder debtBuilder, CalendarDocumentService calendarService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _debtEliminationService = debtEliminationService;
            _idGenerator = idGenerator;
            _debtBuilder = debtBuilder;
            _calendarService = calendarService;
        }

        [HttpGet]
        public MortgageAccelerationModel GetDefaultModel(string programId = null)
        {
            if (string.IsNullOrEmpty(programId))
            {
                var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
                programId = debt.CurrentMortgageProgramId;
            }
            return GetMortgageAccelerationModel(programId);
        }

        [HttpPost("saveorupdate")]
        public MortgageProgramModel SaveOrUpdateProgram([FromBody] UpdateProgramModel program)
        {
            var programId = program.Id;
            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            if (!string.IsNullOrEmpty(debt.Id))
            {
                Command command;
                var existingProgram = debt.MortgagePrograms.Find(mp => mp.Id == programId);
                var selectedProgram = program;
                if (existingProgram == null)
                {
                    programId = _idGenerator.Generate();
                    command = new DebtElimination_MortgageProgram_AddCommand
                    {
                        Id = programId,
                        DebtEliminationId = debt.Id,
                        Title = selectedProgram.Title,
                        LoanAmountInCents = AccountingFormatter.DollarsToCents(selectedProgram.LoanAmountInDollars),
                        InterestRatePerYear = selectedProgram.InterestRatePerYear,
                        LoanTermInYears = (float) Math.Round(selectedProgram.LoanTermInYears, 2),
                        PaymentPeriod = selectedProgram.PaymentPeriod,
                        ExtraPaymentInCentsPerPeriod =
                            AccountingFormatter.DollarsToCents(selectedProgram.ExtraPaymentInDollarsPerPeriod),
                        DisplayResolution = selectedProgram.DisplayResolution,
                    };
                }
                else
                {
                    command = new DebtElimination_MortgageProgram_UpdateCommand
                    {
                        Id = programId,
                        DebtEliminationId = debt.Id,
                        Title = selectedProgram.Title,
                        LoanAmountInCents = AccountingFormatter.DollarsToCents(selectedProgram.LoanAmountInDollars),
                        InterestRatePerYear = selectedProgram.InterestRatePerYear,
                        LoanTermInYears = (float) Math.Round(selectedProgram.LoanTermInYears, 2),
                        PaymentPeriod = selectedProgram.PaymentPeriod,
                        ExtraPaymentInCentsPerPeriod =
                            AccountingFormatter.DollarsToCents(selectedProgram.ExtraPaymentInDollarsPerPeriod),
                        DisplayResolution = selectedProgram.DisplayResolution,
                        AddedToCalendar = existingProgram.AddedToCalendar,
                    };
                }

                Send(command);
            }
            var updatedDebt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());

            return _debtBuilder.GetMortgageProgramModel(updatedDebt, programId);
        }

        [HttpDelete("{programId}")]
        public void Delete(string programId)
        {
            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            if (debt == null) return;
            var command = new DebtElimination_MortgageProgram_DeleteCommand
            {
                Id = programId,
                DebtEliminationId = debt.Id,
                CalendarId = _calendarService.GetByFilter(new CalendarFilter { LedgerId = GetLedgerId() }).OrderBy(c => c.Type).Select(c => c.Id).FirstOrDefault(),
            };

            Send(command);
        }

        [HttpPost("addtocalendar")]
        public IActionResult AddToCalendar(string programId)
        {
            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            var calendar = _calendarService.GetByFilter(new CalendarFilter { LedgerId = GetLedgerId() }).OrderBy(c => c.Type).FirstOrDefault();
            if (debt != null && calendar != null)
            {
                var mortgageProgram = debt.MortgagePrograms.Find(mp => mp.Id == programId);
                if (mortgageProgram != null)
                {
                    var command = new DebtElimination_MortgageProgram_AddToCalendarCommand
                    {
                        DebtEliminationId = debt.Id,
                        MortgageProgramId = mortgageProgram.Id,
                        CalendarId = calendar.Id,
                    };

                    Send(command);
                    return new OkResult();
                }
            }

            return new BadRequestResult();
        }

        private MortgageAccelerationModel GetMortgageAccelerationModel(string programId = null)
        {
            var model = new MortgageAccelerationModel();

            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            model.Programs = debt.MortgagePrograms;
            model.Programs.Insert(0, new MortgageAccelerationProgramDocument { Id = "-1", Title = "Add new..." });

            model.SelectedProgram = _debtBuilder.GetMortgageProgramModel(debt, programId);
            model.PaymentPeriods = ((PaymentPeriodEnum[])Enum.GetValues(typeof(PaymentPeriodEnum))).ToDictionary(pp => (int)pp, pp => pp.GetDescription()).ToList();
            model.DisplayResolutions =
                ((DisplayResolutionEnum[]) Enum.GetValues(typeof (DisplayResolutionEnum))).ToDictionary(pp => (int) pp,
                    pp => pp.GetDescription()).ToList();

            return model;
        }

        private List<MortgageAccelerationProgramDocument> GetPrograms(DebtEliminationDocument debt)
        {
            var programs = debt.MortgagePrograms;
            programs.Insert(0, new MortgageAccelerationProgramDocument { Id = "-1", Title = "Add new..." });
            return programs;
        }
    }
}