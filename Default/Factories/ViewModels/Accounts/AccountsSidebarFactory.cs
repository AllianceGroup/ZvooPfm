using System;
using System.Linq;
using Default.ViewModel.Areas.Business.BusinessController;
using Default.ViewModel.Areas.Shared;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Yodlee.ContentServiceItem.Commands;
using mPower.Domain.Yodlee.Enums;
using mPower.Domain.Yodlee.Storage.Documents;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;

namespace Default.Factories.ViewModels
{
    public class AccountsSidebarFactory : IObjectFactory<string, AccountsSidebarModel>
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IObjectRepository _objectRepository;

        public AccountsSidebarFactory(LedgerDocumentService ledgerService,
            IObjectRepository objectRepository)
        {
            _ledgerService = ledgerService;
            _objectRepository = objectRepository;
        }

        public AccountsSidebarModel Load(string ledgerId)
        {
            var model = new AccountsSidebarModel();

            var accounts = _ledgerService.GetById(ledgerId).Accounts;

            var loanSubtotal = 0L;
            var accountSubtotal = 0L;
            var propertySubtotal = 0L;
            var ccSubtotal = 0L;
            var investmentSubtotal = 0L;

            var aggregatedLoanSubtotal = 0L;
            var aggregatedAccountSubtotal = 0L;
            var aggregatedPropertySubtotal = 0L;
            var aggregatedccSubtotal = 0L;
            var aggregatedInvestmentSubtotal = 0L;

            var manualLoanSubtotal = 0L;
            var manualAccountSubtotal = 0L;
            var manualPropertySubtotal = 0L;
            var manualCcSubtotal = 0L;
            var manualInvestmentSubtotal = 0L;


            foreach (var account in accounts)
            {
                var newAccount = _objectRepository.Load<AccountDocument, BusinessAccount>(account);

                switch (account.LabelEnum)
                {
                    case AccountLabelEnum.Bank:
                        if (account.IsUnknownCash) // we are skipping unckonown cash now at all
                            continue;

                        model.Accounts.Add(newAccount);
                        accountSubtotal += account.Denormalized.Balance;
                        aggregatedAccountSubtotal += account.AggregatedBalance;
                        if (!account.IsAggregated)
                            manualAccountSubtotal += account.Denormalized.Balance;
                        break;

                    case AccountLabelEnum.Loan:
                        model.Loans.Add(newAccount);
                        loanSubtotal += account.Denormalized.Balance;
                        aggregatedLoanSubtotal += account.AggregatedBalance;
                        if (!account.IsAggregated)
                            manualLoanSubtotal += account.Denormalized.Balance;
                        break;

                    case AccountLabelEnum.CreditCard:
                        model.CreditCards.Add(newAccount);
                        ccSubtotal += account.Denormalized.Balance;
                        aggregatedccSubtotal += account.AggregatedBalance;
                        if (!account.IsAggregated)
                            manualCcSubtotal += account.Denormalized.Balance;
                        break;

                    case AccountLabelEnum.Investment:
                        model.Investments.Add(newAccount);
                        investmentSubtotal += account.Denormalized.Balance;
                        aggregatedInvestmentSubtotal += account.AggregatedBalance;
                        if (!account.IsAggregated)
                            manualInvestmentSubtotal += account.Denormalized.Balance;
                        break;
                }
            }

            model.AccountsTotalInDollars = AccountingFormatter.CentsToDollars(accountSubtotal);
            model.LoansTotalInDollars = AccountingFormatter.CentsToDollars(loanSubtotal);
            model.PropertyTotalsInDollars = AccountingFormatter.CentsToDollars(propertySubtotal);
            model.CreditCardTotalsInDollars = AccountingFormatter.CentsToDollars(ccSubtotal);
            model.InvestmentTotalsInDollars = AccountingFormatter.CentsToDollars(investmentSubtotal);

            var manualAndAggregatedAccountsTotal = aggregatedAccountSubtotal + manualAccountSubtotal;
            var manualAndAggregatedLoansTotal = aggregatedLoanSubtotal + manualLoanSubtotal;
            var manualAndAggregatedPropertyTotal = aggregatedPropertySubtotal + manualPropertySubtotal;
            var manualAndAggregatedCreditCardTotal = aggregatedccSubtotal + manualCcSubtotal;
            var manualAndAggregatedInvestmentTotal = aggregatedInvestmentSubtotal + manualInvestmentSubtotal;

            model.ManualAndAggregatedAccountsTotalInDollars = AccountingFormatter.CentsToDollars(manualAndAggregatedAccountsTotal);
            model.ManualAndAggregatedLoansTotalInDollars = AccountingFormatter.CentsToDollars(manualAndAggregatedLoansTotal);
            model.ManualAndAggregatedPropertyTotalsInDollars = AccountingFormatter.CentsToDollars(manualAndAggregatedPropertyTotal);
            model.ManualAndAggregatedCreditCardTotalsInDollars = AccountingFormatter.CentsToDollars(manualAndAggregatedCreditCardTotal);
            model.ManualAndAggregatedInvestmentTotalsInDollars = AccountingFormatter.CentsToDollars(manualAndAggregatedInvestmentTotal);

            var equityBalance = accounts.Where(x => x.TypeEnum == AccountTypeEnum.Equity).Sum(x => x.ActualBalance);
            var totalExpense = accounts.Where(x => x.TypeEnum == AccountTypeEnum.Expense).Sum(x => x.ActualBalance);
            var totalIncome = accounts.Where(x => x.TypeEnum == AccountTypeEnum.Income).Sum(x => x.ActualBalance);
            var netIncome = totalIncome - totalExpense;

            model.Equity = AccountingFormatter.ConvertToDollarsThenFormat(equityBalance + netIncome, true);
            model.AggregatedEquity = AccountingFormatter.ConvertToDollarsThenFormat(aggregatedAccountSubtotal + aggregatedInvestmentSubtotal + aggregatedPropertySubtotal - aggregatedLoanSubtotal - aggregatedccSubtotal, true);
            model.ManualAndAggregatedEquity = AccountingFormatter.CentsToDollars(manualAndAggregatedAccountsTotal + manualAndAggregatedInvestmentTotal + manualAndAggregatedPropertyTotal - manualAndAggregatedLoansTotal - manualAndAggregatedCreditCardTotal);

            return model;
        }
    }
}
