using System.Collections.Generic;
using System.Linq;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using mPower.Domain.Accounting.Transaction.Events;

namespace mPower.Domain.Patches
{
    public class Patch7FixTransactionsAccountsReferences: IPatch
    {
        public int Id
        {
            get { return 7; }
        }

        public string Name
        {
            get { return "Remove Transaction_Modified events, where offset account id is the same as base account Id"; }
        }

        public bool UseIncomeTransitions
        {
            get { return true; }
        }

        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            var transitionsToRemove = new List<Transition>();
            foreach (var transition in transitions)
            {
                foreach (var data in transition.Events)
                {
                    var @event = data.Data as Transaction_ModifiedEvent;
                    if (@event != null &&  @event.BaseEntryAccountId == @event.Entries[0].AccountId && @event.Entries[1].AccountId == @event.Entries[0].AccountId)
                    {
                        transitionsToRemove.Add(transition);
                    }
                }
            }
            foreach (var transition in transitionsToRemove)
            {
                transitionRepository.RemoveTransition(transition.Id.StreamId,transition.Id.Version);
            }
        }
    }
}