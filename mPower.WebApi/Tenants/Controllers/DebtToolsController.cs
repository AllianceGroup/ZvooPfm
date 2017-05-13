using Default.ViewModel.Areas.Finance.DebtToolsController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting.DebtElimination.Commands;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.WebApi.Tenants.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class DebtToolsController: BaseController
    {
        private readonly DebtEliminationDocumentService _debtEliminationService;
        private readonly IIdGenerator _idGenerator;
        private readonly DebtViewModelBuilder _debtBuilder;

        public DebtToolsController(DebtEliminationDocumentService debtEliminationService, IIdGenerator idGenerator,
            DebtViewModelBuilder debtBuilder, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _debtEliminationService = debtEliminationService;
            _idGenerator = idGenerator;
            _debtBuilder = debtBuilder;
        }

        [HttpGet]
        public DebtToolsModel GetDefaultModel(string ledgerId = null)
        {
            if (string.IsNullOrEmpty(ledgerId)) ledgerId = GetLedgerId();

            var userId = GetUserId();
            var model = new DebtToolsModel();

            //Precreate debt elimination Program if it was not created yet
            var debtElimination = _debtEliminationService.GetDebtEliminationByUser(ledgerId, userId);
            if (debtElimination == null)
            {
                var command = new DebtElimination_CreateCommand
                {
                    Id = _idGenerator.Generate(),
                    LedgerId = ledgerId,
                    UserId = userId
                };

                Send(command);
            }
            else
            {
                if (debtElimination.PlanId != DebtEliminationPlanEnum.NotInitialized)
                    model.DebtElimination = _debtBuilder.GetDebtEliminationShortModel(debtElimination);
                if (debtElimination.IsDebtToIncomeCalculatedBefore)
                    model.DebtToIncomeRatio = _debtBuilder.GetDebtToIncomeRatioModel(debtElimination);
                if (!string.IsNullOrEmpty(debtElimination.CurrentMortgageProgramId))
                    model.CurrentMortgageProgram = debtElimination.MortgagePrograms.Find(mp => mp.Id == debtElimination.CurrentMortgageProgramId);
            }

            return model;
        }
    }
}