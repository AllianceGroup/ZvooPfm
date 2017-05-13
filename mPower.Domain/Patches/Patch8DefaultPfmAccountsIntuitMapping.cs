using System.Collections.Generic;
using System.Linq;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Ledger.Events;

namespace mPower.Domain.Patches
{
    public class Patch8DefaultPfmAccountsIntuitMapping : IPatch
    {
        private readonly Dictionary<string, List<string>> _mapping = new Dictionary<string, List<string>>();

        public int Id
        {
            get { return 8; }
        }

        public string Name
        {
            get { return "Add Intuit categories mapping for default PFM accounts"; }
        }

        public bool UseIncomeTransitions
        {
            get { return true; }
        }

        public Patch8DefaultPfmAccountsIntuitMapping(AccountsService accountsService)
        {
            // generate mapping
            var accountsWithMapping = new List<ExpenseAccount>();
            foreach (var account in accountsService.CommonPersonalExpenseAccounts())
            {
                if (account.IntuitCategoriesNames != null && account.IntuitCategoriesNames.Any())
                {
                    accountsWithMapping.Add(account);
                }
                if (account.SubAccounts != null && account.SubAccounts.Any())
                {
                    accountsWithMapping.AddRange(account.SubAccounts.Where(x => x.IntuitCategoriesNames != null && x.IntuitCategoriesNames.Any()));
                }
            }

            _mapping = accountsWithMapping.ToDictionary(x => x.Name, x => x.IntuitCategoriesNames);
        }

        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            foreach (var transition in transitions)
            {
                var transitionNeedToBeSaved = false;
                foreach (var rawEvent in transition.Events)
                {
                    var @event = rawEvent.Data as Ledger_Account_AddedEvent;
                    if (@event != null && (@event.IntuitCategoriesNames == null || @event.IntuitCategoriesNames.Count == 0) && _mapping.ContainsKey(@event.Name))
                    {
                        @event.IntuitCategoriesNames = _mapping[@event.Name];
                        transitionNeedToBeSaved = true;
                    }
                }
                if (transitionNeedToBeSaved)
                {
                    UpdateTransition(transitionRepository, transition);
                }
            }
        }

        private static void UpdateTransition(ITransitionRepository transitionRepository, Transition transition)
        {
            transitionRepository.RemoveTransition(transition.Id.StreamId, transition.Id.Version);
            transitionRepository.SaveTransition(transition);
        }
    }
}