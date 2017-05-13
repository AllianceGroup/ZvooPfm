using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Framework;

namespace mPower.Domain.Patches
{
    public class Patch5FixtAggregationStatus : IPatch
    {
        private readonly MongoWrite _write;
        private readonly MongoTransitionSerializer _serializer;

        public int Id
        {
            get { return 5; }
        }

        public string Name
        {
            get { return "Cancel account status changes for crashed aggregations"; }
        }

        public bool UseIncomeTransitions
        {
            get { return false; }
        }

        public Patch5FixtAggregationStatus(MongoWrite write, MongoTransitionSerializer serializer)
        {
            _write = write;
            _serializer = serializer;
        }

        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            transitionRepository.EnsureIndexes();

            var bsonTransitions = _write.Transitions.FindAllAs<BsonDocument>();
            var rawTransitions = Convert(bsonTransitions);
            var changeStatusTransitions = rawTransitions.Where(t => t.Events.Any(e => e.Data is Ledger_Account_AggregationStatus_UpdatedEvent)).ToList();

            var groupedByLedger = changeStatusTransitions.GroupBy(t => GetAggregationStatusUpdateEvent(t).LedgerId, t => t).ToList();
            foreach (var ledgerAccountsChanges in groupedByLedger)
            {
                var groupedByAccount = ledgerAccountsChanges.GroupBy(t => GetAggregationStatusUpdateEvent(t).IntuitAccountId, t => t);
                foreach (var accountChanges in groupedByAccount)
                {
                    // look through the list to remove all resent settings status to 'AggregatedAccountStatusEnum.PullingTransactions'
                    var changesInRevertedOrder = accountChanges.OrderByDescending(t => t.Id.Version).ToList();
                    foreach(var transition in changesInRevertedOrder)
                    {
                        var changeStatusEvent = GetAggregationStatusUpdateEvent(transition);
                        if (changeStatusEvent.NewStatus == AggregatedAccountStatusEnum.PullingTransactions)
                        {
                            // remove
                            if (transition.Events.Count == 1)
                            {
                                var query = Query.And(Query.EQ("_id.StreamId", transition.Id.StreamId), Query.EQ("_id.Version", transition.Id.Version));
                                _write.Transitions.Remove(query);
                            }
                            else
                            {
                                transition.Events.RemoveAt(transition.Events.FindIndex(e => e.Data is Ledger_Account_AggregationStatus_UpdatedEvent));
                                _write.Transitions.Save(transition);
                            }
                        }
                        else
                        {
                            // end search
                            break;
                        }
                    }
                }
            }
        }

        private IEnumerable<Transition> Convert(IEnumerable<BsonDocument> bsonTransitions)
        {
            var result = new List<Transition>();
            foreach (var bsTransition in bsonTransitions)
            {
                Transition transition;
                try
                {
                    transition = _serializer.Deserialize(bsTransition);
                }
                catch (Exception)
                {
                    // TODO: write separate patch to handle deserialization exception
                    continue;
                }
                result.Add(transition);
            }
            return result;
        }

        private static Ledger_Account_AggregationStatus_UpdatedEvent GetAggregationStatusUpdateEvent(Transition transition)
        {
            return (Ledger_Account_AggregationStatus_UpdatedEvent)transition.Events.First(e => e.Data is Ledger_Account_AggregationStatus_UpdatedEvent).Data;
        }
    }
}