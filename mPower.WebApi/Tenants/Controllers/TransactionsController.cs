using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel;
using Default.ViewModel.Areas.Shared;
using Default.ViewModel.TransactionsController;
using mPower.Documents;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Domain.Accounting.Transaction.Commands;
using mPower.Domain.Accounting.Transaction.Data;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.Framework.Services;
using mPower.WebApi.Tenants.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class TransactionsController : BaseController
    {
        private readonly IObjectRepository _objectRepository;
        private readonly TransactionDocumentService _transactionDocumentService;

        public TransactionsController(IObjectRepository objectRepository, TransactionDocumentService transactionDocumentService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _objectRepository = objectRepository;
            _transactionDocumentService = transactionDocumentService;
        }

        [HttpGet("filter")]
        public TransactionsViewModel GetFilteredTransaction(TransactionClientFilter filter)
        {
            if (filter == null) filter = new TransactionClientFilter();
            filter.UserId = GetUserId();
            filter.ledgerId = GetLedgerId();
            filter.affiliateId = Tenant.ApplicationId;
            filter.applicationName = AppName;

            return GetTransactionsList(filter);
        }

        [HttpPost("assign")]
        public IActionResult AssignEntryAccount([FromBody]AlignTransactionViewModel model)
        {
            var transaction = _transactionDocumentService.GetById(model.TransactionId);
            if (model.NewAccountId == transaction.BaseEntryAccountId)
            {
                ModelState.AddModelError("NewAccountId", "Transaction category cannot be the same as the selected account");
            }
            if (ModelState.IsValid)
            {
                var cmd = _objectRepository.Load<TransactionEntryChangeAccountDto, Transaction_ModifyCommand>(new TransactionEntryChangeAccountDto
                {
                    LedgerId = GetLedgerId(),
                    TransactionId = model.TransactionId,
                    PreviousAccountId = model.PreviousAccountId,
                    NewAccountId = model.NewAccountId,
                });

                Send(cmd);
                return new OkResult();
            }
            return BadRequest(ModelState);
        }

        [HttpPost("editmultiple")]
        public void EditMultiple([FromBody]MultipleEditTransactionViewModel model)
        {
            var cmd = _objectRepository.Load<EditMultipleEntriesViewModel, IEnumerable<Transaction_ModifyCommand>>(new EditMultipleEntriesViewModel
            {
                Memo = model.Memo,
                AccountId = model.AccountId,
                TransactionsIds = string.Join(";", model.Transactions),
                LedgerId = GetLedgerId()
            });
            Send(cmd.ToArray());
        }

        [HttpPost("deletemultiple")]
        public void DeleteMultiple([FromBody]MultipleDeleteTransactionViewModel model)
        {
            Send(new Transaction_DeleteMultipleCommand { LedgerId = GetLedgerId(), TransactionIds = model.TransactionIds });
        }

        [HttpDelete("delete/{transactionId}")]
        public void Delete(string transactionId)
        {
            var cmd = new Transaction_DeleteCommand
            {
                TransactionId = transactionId,
                LedgerId = GetLedgerId()
            };
            Send(cmd);
        }

        [HttpGet("add")]
        public AddStandardTransactionViewModel Add(TransactionType type, string accountId = null)
        {
            return _objectRepository.Load<AddStandardTransactionViewModel, AddStandardTransactionViewModel>(
                new AddStandardTransactionViewModel
                {
                    AccountId = accountId,
                    TransactionType = type,
                    LedgerId = GetLedgerId()
                });
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody]AddStandardTransactionViewModel model)
        {
            //need to use NotEqualToProperty attribute 
            if (model.AccountId.Equals(model.OffSetAccountId))
            {
                ModelState.AddModelError("AccountId", "Transaction category cannot be the same as the selected account");
            }
            model.LedgerId = GetLedgerId();

            var cmd = _objectRepository.Load<AddStandardTransactionViewModel, Transaction_CreateCommand>(model);
            Send(cmd);

            return new OkResult();
        }

        [HttpGet("edit/{transactionId}")]
        public EditStandardTransactionViewModel Edit(string transactionId)
        {
            return _objectRepository.Load<string, EditStandardTransactionViewModel>(transactionId);
        }

        [HttpPost("edit")]
        public IActionResult Edit([FromBody]EditStandardTransactionViewModel model)
        {
            if (model.MemorizeCategorization && string.IsNullOrEmpty(model.Memo))
                ModelState.AddModelError("Memo", "Field 'Memo' is required for categorization remembering.");
            //need to use NotEqualToProperty attribute 
            if (model.AccountId.Equals(model.OffSetAccountId))
                ModelState.AddModelError("AccountId", "Transaction category cannot be the same as the selected account");
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            try
            {
                if (Math.Abs(model.AmountInDollars.ToDouble()) < double.Epsilon)
                    throw new Exception("Cannot create a transaction with an amount of 0.00");

                DateTime bookedDate;
                DateTime.TryParse(model.BookedDate, out bookedDate);
                bookedDate = bookedDate == DateTime.MinValue 
                    ? DateTime.Now 
                    : bookedDate.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute).AddSeconds(DateTime.Now.Second);

                var baseEntry = new EntryData
                {
                    AccountId = model.AccountId,
                    BookedDate = bookedDate,
                    Memo = model.Memo,
                    Payee = model.Payee,
                    CreditAmountInCents = model.TransactionType == TransactionType.Deposit ? 0 : AccountingFormatter.DollarsToCents(model.AmountInDollars.ToDouble()),
                    DebitAmountInCents = model.TransactionType == TransactionType.Deposit ? AccountingFormatter.DollarsToCents(model.AmountInDollars.ToDouble()) : 0,
                };

                var offsetEntry = new EntryData
                {
                    AccountId = model.OffSetAccountId,
                    BookedDate = bookedDate,
                    Memo = model.Memo,
                    Payee = model.Payee,
                    CreditAmountInCents = baseEntry.DebitAmountInCents,
                    DebitAmountInCents = baseEntry.CreditAmountInCents
                };


                var cmd = _objectRepository.Load<TransactionModifyDto, Transaction_ModifyCommand>(new TransactionModifyDto
                {
                    LedgerId = GetLedgerId(),
                    TransactionId = model.TransactionId,
                    Entries = new List<EntryData> { baseEntry, offsetEntry },
                    Type = model.TransactionType,
                    ReferenceNumber = model.ReferenceNumber,
                    BaseEntryAccountId = model.AccountId,
                    Imported = model.Imported,
                });

                if (model.MemorizeCategorization)
                {
                    var keywordFromDescription = model.Memo.DeriveKeywordFromDescription();

                    var command = new Ledger_TransactionMap_AddItemCommand
                    {
                        Keyword = keywordFromDescription,
                        AccountId = model.OffSetAccountId,
                        LedgerId = GetLedgerId()
                    };

                    Send(command);
                }

                Send(cmd);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return new BadRequestObjectResult(ModelState);
            }

            return new OkResult();
        }

        private TransactionsViewModel GetTransactionsList(TransactionClientFilter filter)
        {
            filter.UserId = GetUserId();
            filter.affiliateId = Tenant.ApplicationId;

            var paging = new PagingInfo { CurrentPage = filter.page, ItemsPerPage = filter.itemsPerPage };
            filter.Paging = paging;

            var categories = _objectRepository.Load<string, IEnumerable<GroupedSelectListItem>>(filter.ledgerId);
            var groupedSelectListItems = categories as IList<GroupedSelectListItem> ?? categories.ToList();
            foreach (var g in groupedSelectListItems)
                g.Text = g.Text.Split('|').First();

            return new TransactionsViewModel
            {
                Entries = _objectRepository.Load<TransactionClientFilter, List<Entry>>(filter),
                CategorySelectList = groupedSelectListItems,
                Paging = paging
            };
        }
    }
}