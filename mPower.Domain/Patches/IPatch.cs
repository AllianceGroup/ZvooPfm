using System.Collections.Generic;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;

namespace mPower.Domain.Patches
{
    public interface IPatch
    {
        int Id { get; }

        string Name { get; }

        bool UseIncomeTransitions { get; }

        void Apply(List<Transition> transitions, Dispatcher dispatcher,ITransitionRepository transitionRepository);
    }
}