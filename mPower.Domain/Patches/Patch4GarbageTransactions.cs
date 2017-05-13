using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using mPower.Domain.Accounting.Transaction.Events;
using mPower.Framework;

namespace mPower.Domain.Patches
{
    public class Patch4GarbageTransactions : IPatch
    {
        private readonly MongoWrite _write;
        private readonly MongoTransitionSerializer _serializer;


        public int Id
        {
            get { return 4; }
        }

        public string Name
        {
            get { return "Clean up write model from data related with deleted transactions."; }
        }

        public bool UseIncomeTransitions
        {
            get { return false; }
        }

        public Patch4GarbageTransactions(MongoWrite write, MongoTransitionSerializer serializer)
        {
            _write = write;
            _serializer = serializer;
        }

        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            transitionRepository.EnsureIndexes();

            var bsonTransitions = _write.Transitions.FindAllAs<BsonDocument>();
            var streamIdsToDelete = new List<string>();

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

                if (transition.Events.Any(e => e.Data is Transaction_DeletedEvent))
                {
                    var streamId = transition.Id.StreamId;
                    if (!streamIdsToDelete.Contains(streamId))
                    {
                        streamIdsToDelete.Add(streamId);
                    }
                }
            }

            if (streamIdsToDelete.Count > 0)
            {
                var query = Query.In("_id.StreamId", BsonArray.Create(streamIdsToDelete));
                _write.Transitions.Remove(query);
            }
        }
    }
}