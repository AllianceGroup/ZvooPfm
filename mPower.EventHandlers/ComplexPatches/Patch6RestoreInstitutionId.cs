using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using mPower.Documents.ExternalServices.FullTextSearch;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Patches;
using mPower.Framework;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;

namespace mPower.EventHandlers.ComplexPatches
{
    public class Patch6RestoreInstitutionId : IPatch
    {
        private readonly IntutitInstitutionLuceneService _luceneService;
        private readonly MongoWrite _write;
        private readonly MongoTransitionSerializer _serializer;

        public int Id
        {
            get { return 6; }
        }

        public string Name
        {
            get { return "Try to restore missed Intuit institutions IDs"; }
        }

        public bool UseIncomeTransitions
        {
            get { return true; }
        }

        public Patch6RestoreInstitutionId(IntutitInstitutionLuceneService luceneService, MongoWrite write, MongoTransitionSerializer serializer)
        {
            _luceneService = luceneService;
            _write = write;
            _serializer = serializer;
        }

        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            transitionRepository.EnsureIndexes();
            foreach (var transition in transitions)
            {
                var isUpdated = false;
                foreach (var transitionEvent in transition.Events)
                {
                    var accountAddedEvent = transitionEvent.Data as Ledger_Account_AddedEvent;
                    if (accountAddedEvent != null && accountAddedEvent.Aggregated && string.IsNullOrEmpty(accountAddedEvent.YodleeContentServiceId))
                    {
                        string institutionId = null;
                        if (!string.IsNullOrEmpty(accountAddedEvent.InstitutionName))
                        {
                            institutionId = GetInstitutionIdByName(accountAddedEvent.InstitutionName);
                        }
                        if (string.IsNullOrEmpty(institutionId) && !string.IsNullOrEmpty(accountAddedEvent.IntuitAccountNumber))
                        {
                            institutionId = GetInstitutionIdByAccountNumber(accountAddedEvent.IntuitAccountNumber);
                        }

                        if (!string.IsNullOrEmpty(institutionId))
                        {
                            accountAddedEvent.YodleeContentServiceId = institutionId;
                            isUpdated = true;
                        }
                    }
                }

                if (isUpdated)
                {
                    transitionRepository.RemoveTransition(transition.Id.StreamId, transition.Id.Version);
                    transitionRepository.SaveTransition(transition); // it's not updating but inserting transition
                }
            }
        }

        private string GetInstitutionIdByName(string institutionName)
        {
            string id = null;

            var searchResults = _luceneService
                .SearchByQuery(institutionName)
                .Where(x => x.Name == institutionName)
                .ToList();

            if (searchResults.Count == 1)
            {
                id = searchResults.First().IntuitId;
            }

            return id;
        }

        private string GetInstitutionIdByAccountNumber(string accountNumber)
        {
            var query = Query.And(Query.EQ("Events.Data.IntuitAccountNumber", accountNumber), Query.GT("Events.Data.YodleeContentServiceId", ""));
            var bsonTransitions = _write.Transitions.FindAs<BsonDocument>(query);

            foreach (var bsTransition in bsonTransitions)
            {
                try
                {
                    var transition = _serializer.Deserialize(bsTransition);
                    foreach (var transitionEvent in transition.Events)
                    {
                        var accountAddedEvent = transitionEvent.Data as Ledger_Account_AddedEvent;
                        if (accountAddedEvent != null && !string.IsNullOrEmpty(accountAddedEvent.YodleeContentServiceId))
                        {
                            return accountAddedEvent.YodleeContentServiceId;
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }
    }
}