using System;
using System.Collections.Generic;
using Paralect.Domain;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using mPower.Domain.Application.Affiliate.Events;

namespace mPower.Domain.Patches
{
    public class Patch2AffiliateUpdateDate : IPatch
    {
        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            transitionRepository.EnsureIndexes();
            foreach (var transition in transitions)
            {
                foreach (var transitionEvent in transition.Events)
                {
                    bool isUpdated = false;
                    var item = transitionEvent.Data as Event;
                    if (item != null)
                    {
                        var updateEvent = item as Affiliate_UpdatedEvent;
                        if (updateEvent != null && updateEvent.UpdateDate == DateTime.MinValue)
                        {
                            var date = DateTime.Now.AddMonths(-1);
                            if (updateEvent.Metadata.StoredDate != DateTime.MinValue)
                                date = updateEvent.Metadata.StoredDate;

                            ((Affiliate_UpdatedEvent)transitionEvent.Data).UpdateDate = date;
                            isUpdated = true;
                        }
                    }

                    if (isUpdated)
                        transitionRepository.SaveTransition(transition);
                }
            }
        }

        public string Name
        {
            get { return "Set UpdateDate at Affiliate_UpdatedEvent event"; }
        }

        public int Id
        {
            get { return 2; }
        }

        public bool UseIncomeTransitions
        {
            get { return true; }
        }
    }
}