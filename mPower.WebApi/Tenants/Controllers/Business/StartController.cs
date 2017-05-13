using System;
using System.Linq;
using Default.ViewModel.Areas.Business.StartController;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers.Business
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class StartController : BaseController
    {
        private readonly LedgerDocumentService _ledgers;
        private readonly IIdGenerator _idGenerator;
        private readonly AccountsService _accountsService;

        public StartController(LedgerDocumentService ledgers, IIdGenerator idGenerator, AccountsService accountsService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _ledgers = ledgers;
            _idGenerator = idGenerator;
            _accountsService = accountsService;
        }

        [HttpGet]
        public IndexViewModel GetListBusiness()
        {
            var model = new IndexViewModel
            {
                Companies = (from l in _ledgers.GetAll()
                    where l.Users.Count(x => x.Id == GetUserId()) > 0
                          && l.TypeEnum == LedgerTypeEnum.Business
                    select new Company()
                    {
                        LedgerId = l.Id,
                        Name = l.Name,
                        DateCreated = l.CreatedDate,
                    }).ToList()
            };

            return model;
        }

        [HttpGet("addLedger")]
        public AddLedgerViewModel Addledger()
        {
            var model = new AddLedgerViewModel{ AutoOrManual = "Auto" };

            return model;
        }

        [HttpPost("addLedger")]
        public IActionResult Addledger([FromBody]AddLedgerViewModel model)
        {
            if(!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                var ledgerCreateCommand = new Ledger_CreateCommand
                {
                    Name = model.Name,
                    Address = model.Address,
                    Address2 = model.Address2,
                    City = model.City,
                    FiscalYearStart = model.FiscalYearStart,
                    LedgerId = _idGenerator.Generate(),
                    State = model.State,
                    TaxId = model.EIN,
                    Zip = model.Zip,
                    CreatedDate = DateTime.Now,
                    TypeEnum = LedgerTypeEnum.Business

                };

                var ledgerUserAddCommand = new Ledger_User_AddCommand()
                {
                    LedgerId = ledgerCreateCommand.LedgerId,
                    UserId = GetUserId()
                };

                Send(ledgerCreateCommand, ledgerUserAddCommand);

                var accountCommands = _accountsService.
                    CreateBusinessBaseAccounts(ledgerCreateCommand.LedgerId).ToList();

                if (model.AutoOrManual == "Auto")
                {
                    accountCommands.AddRange(_accountsService.CreateBusinessCommonAccounts(ledgerCreateCommand.LedgerId));

                    if (model.Based == "Product")
                        accountCommands.AddRange(
                            _accountsService.CreateProductBasedBusinessAccounts(ledgerCreateCommand.LedgerId));

                    if (model.Based == "Service")
                        accountCommands.AddRange(
                            _accountsService.CreateServiceBasedBusinessAccounts(ledgerCreateCommand.LedgerId));
                }

                foreach (var accountCommand in accountCommands)
                {
                    Send(accountCommand);
                }

                var addedLedger = _ledgers.GetById(ledgerCreateCommand.LedgerId);
                var business = new Company
                {
                    LedgerId = addedLedger.Id,
                    Name = addedLedger.Name,
                    DateCreated = addedLedger.CreatedDate
                };

                return new OkObjectResult(business);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("Error", e.Message);
                return new BadRequestObjectResult(ModelState);
            }
        }
    }
}
