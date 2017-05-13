using Default.ViewModel.GettingStartedController;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Documents;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Default.Factories.ViewModels.Aggregation
{
    public class AssignAccountsToLedgerViewModelFactory : IObjectFactory<AssignDto, AssignAccountsToLedgerViewModel>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IAggregationClient _aggregation;

        public AssignAccountsToLedgerViewModelFactory(LedgerDocumentService ledgerService, IAggregationClient aggregation)
        {
            _ledgerService = ledgerService;
            _aggregation = aggregation;
        }

        public AssignAccountsToLedgerViewModel Load(AssignDto input)
        {
            #region Exclude authenticated accounts

            var accounts = new List<AccountDocument>();

            if (input.Accounts != null)
            {
                //exlude already authenticated accounts
                var ledger = _ledgerService.GetById(input.LedgerId);
                if (ledger == null)
                {
                    accounts.AddRange(input.Accounts);
                }
                else
                {
                    foreach (var intuitAccount in input.Accounts)
                    {
                        if (ledger.Accounts.All(x => x.IntuitAccountNumber != intuitAccount.AccountNumber))
                            accounts.Add(intuitAccount);
                    }
                }
            }

            input.Accounts = accounts;

            #endregion

            var ledgers = _ledgerService.GetByUserId(input.UserId);
            var ledgerChoices = new List<SelectListItem>();

            ledgerChoices.AddRange(
                ledgers.Where(t => t.Id == input.LedgerId).Select(
                    x => new SelectListItem { Text = x.Name, Value = x.Id }));
            ledgerChoices.AddRange(
                ledgers.Where(t => t.Id != input.LedgerId).Select(
                    x => new SelectListItem { Text = x.Name, Value = x.Id }));
            ledgerChoices.Add(new SelectListItem { Text = "Hide", Value = "Hide" });


            var institution = _aggregation.GetInstitution(input.InstitutionId);

            var model = new AssignAccountsToLedgerViewModel
            {
                LedgerChoices = ledgerChoices.AsEnumerable(),
                ContentServiceDisplayName = institution.Name,
                Accounts =
                    input.Accounts == null ? null : input.Accounts
                    .Select(s => new UnAssignedAccount
                    {
                        Number = s.AccountNumber,
                        Name =
                            String.Format("{0} {1}", s.Nickname, s.Balance),
                        Balance = s.Balance,
                        Nickname = s.Nickname
                    }).ToList(),
            };

            return model;
        }
    }
}