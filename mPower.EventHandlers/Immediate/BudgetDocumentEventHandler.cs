using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Accounting.Ledger;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Ledger.Events;
using Paralect.ServiceBus;

namespace mPower.EventHandlers.Immediate
{
    public class BudgetDocumentEventHandler :
        IMessageHandler<Ledger_Budget_SetEvent>,
        IMessageHandler<Ledger_Budget_UpdatedEvent>,
        IMessageHandler<Ledger_Account_RemovedEvent>
    {
        private readonly BudgetDocumentService _budgetService;

        public BudgetDocumentEventHandler(BudgetDocumentService budgetService)
        {
            _budgetService = budgetService;
        }

        public void Handle(Ledger_Budget_SetEvent message)
        {
            if (message.Budgets.Count > 0)
            {
                //remove all current budgets
                _budgetService.Remove(new BudgetFilter { LedgerId = message.LedgerId, Month = message.Budgets[0].Month, Year = message.Budgets[0].Year });

                foreach (var item in message.Budgets)
                {
                    var budget = new BudgetDocument
                    {
                        AccountId = item.AccountId,
                        Id = item.Id,
                        AccountName = item.AccountName,
                        AccountType = item.AccountType,
                        LedgerId = message.LedgerId,
                        Month = item.Month,
                        ParentId = item.ParentId,
                        Year = item.Year,
                        BudgetAmount = item.BudgetAmount,
                        SpentAmount = item.SpentAmount
                    };

                    foreach (var sub in item.SubBudgets)
                    {
                        var subBudget = new ChildBudgetDocument
                        {
                            AccountId = sub.AccountId,
                            AccountName = sub.AccountName,
                            AccountType = sub.AccountType,
                            SpentAmount = sub.SpentAmount,
                            ParentAccountId = budget.AccountId
                        };

                        budget.SubBudgets.Add(subBudget);
                    }

                    _budgetService.Insert(budget);
                }
            }
        }

        public void Handle(Ledger_Budget_UpdatedEvent message)
        {
            var query = Query.EQ("_id", message.BudgetId);

            _budgetService.Update(query, Update<BudgetDocument>.Set(x => x.BudgetAmount, message.Amount));
        }

        public void Handle(Ledger_Account_RemovedEvent message)
        {
            var query = Query.And(
                Query.EQ("LedgerId", message.LedgerId),
                Query.EQ("AccountId", message.AccountId));


            _budgetService.Remove(query);

            query = Query.And(
                Query.EQ("LedgerId", message.LedgerId),
                Query.EQ("SubBudgets.AccountId", message.AccountId));

            _budgetService.UpdateMany(query, Update.Pull("SubBudgets", Query.EQ("AccountId", message.AccountId)));

        }
    }
}
