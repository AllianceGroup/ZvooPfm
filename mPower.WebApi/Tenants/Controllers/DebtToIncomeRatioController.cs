using System;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.DebtElimination.Commands;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DebtViewModelBuilder = mPower.WebApi.Tenants.Services.DebtViewModelBuilder;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/debttoincome")]
    public class DebtToIncomeRatioController : BaseController
    {
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly DebtViewModelBuilder _debtBuilder;

        public DebtToIncomeRatioController(DebtEliminationDocumentService debtEliminationService, 
            DebtViewModelBuilder debtBuilder, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _debtEliminationService = debtEliminationService;
            _debtBuilder = debtBuilder;
        }

        [HttpGet]
        public DebtToIncomeRatioModel GetDefaultModel()
        {
            return GetModel();
        }

        [HttpPost("saveorupdate")]
        public IActionResult SaveOrUpdate([FromBody]UpdateDebtIncomeModel model)
        {
            if (model.MonthlyGrossIncome == 0)
            {
                ModelState.AddModelError(nameof(model.MonthlyGrossIncome),"Please, specify Monthly Gross Income not equal to zero.");
                return new BadRequestObjectResult(ModelState);
            }

            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            var dti = CalculateDebtToIncomeRatio(model);
            var command = new DebtElimination_DebtToIncomeRatio_UpdateCommand
            {
                DebtToIncomeRatio = 1,
                DebtToIncomeRatioString = dti,
                Id = debt.Id,
                MonthlyGrossIncomeInCents = AccountingFormatter.DollarsToCents(model.MonthlyGrossIncome),
                TotalMonthlyDebtInCents = AccountingFormatter.DollarsToCents(model.TotalMonthlyDebt),
                TotalMonthlyRentInCents = AccountingFormatter.DollarsToCents(model.TotalMonthlyRent),
                TotalMonthlyPitiaInCents = AccountingFormatter.DollarsToCents(model.TotalMonthlyPitia)
            };

            Send(command);
            return new OkObjectResult(GetModel());
        }

        [HttpDelete]
        public void Delete()
        {
            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            var command = new DebtElimination_DebtToIncomeRatio_UpdateCommand
            {
                Id = debt.Id,
                DebtToIncomeRatio = 0,
                DebtToIncomeRatioString = "",
                MonthlyGrossIncomeInCents = 0,
                TotalMonthlyDebtInCents = 0,
                TotalMonthlyRentInCents = 0,
                TotalMonthlyPitiaInCents = 0,
            };

            Send(command);
        }

        private DebtToIncomeRatioModel GetModel()
        {
            var debt = _debtEliminationService.GetDebtEliminationByUser(GetLedgerId(), GetUserId());
            return _debtBuilder.GetDebtToIncomeRatioModel(debt);
        }

        private static string CalculateDebtToIncomeRatio(UpdateDebtIncomeModel args)
        {
            var part1 = (args.TotalMonthlyPitia + args.TotalMonthlyRent)/args.MonthlyGrossIncome;
            var part2 = (args.TotalMonthlyPitia + args.TotalMonthlyDebt + args.TotalMonthlyRent)/args.MonthlyGrossIncome;

            return $"{Math.Round(part1, 3)*100:N0} / {Math.Round(part2, 3)*100:N0}";
        }
    }
}