using System.Collections.Generic;
using MongoDB.Bson;
using Paralect.ServiceBus.Dispatching;
using Paralect.Transitions;
using mPower.Framework;

namespace mPower.Domain.Patches
{
    public class Patch3AdjustNamespaces : IPatch
    {
        private readonly MongoWrite _write;

        public Patch3AdjustNamespaces(MongoWrite write)
        {
            _write = write;
        }

        public int Id
        {
            get { return 3; }
        }

        public string Name
        {
            get {  return "Adjust namespaces according to domain refactoring"; }
        }

        public bool UseIncomeTransitions
        {
            get { return false; }
        }

        public void Apply(List<Transition> transitions, Dispatcher dispatcher, ITransitionRepository transitionRepository)
        {
            transitionRepository.EnsureIndexes();

            var bsonTransitions = _write.Transitions.FindAllAs<BsonDocument>();
            var count = 0;
            var updatedCount = 0;

            foreach (var transition in bsonTransitions)
            {
                var events = transition.GetElement("Events").Value.AsBsonArray;
                foreach (var @event in events)
                {
                    count++;
                    bool isUpdated = false;
                    var item = @event as BsonDocument;
                    var type = item.GetElement("TypeId").Value.AsString;
                    if (type.Contains("mPower.Membership.Domain.User.Commands"))
                    {
                        type = type.Replace("mPower.Membership.Domain.User.Commands", "mPower.Membership.Domain.User.Events");
                    }

                    if (type.Contains("mPower.Membership.Domain") || type.Contains("mpower.Membership.Domain"))
                    {
                        type = type.Replace("mPower.Membership.Domain", "mPower.Domain.Membership");
                        type = type.Replace("mpower.Membership.Domain", "mPower.Domain.Membership");
                        type = type.Replace("mpower.Membership,", "mPower.Domain,");
                        isUpdated = true;
                    }

                    if (type.Contains("mPower.Domain.Affiliate"))
                    {
                        type = type.Replace("mPower.Domain.Affiliate", "mPower.Domain.Application.Affiliate");
                        type = type.Replace("mPower,", "mPower.Domain,");
                        isUpdated = true;
                    }

                    if (type.Contains("mPower.Yodlee.Domain"))
                    {
                        type = type.Replace("mPower.Yodlee.Domain", "mPower.Domain.Yodlee");
                        type = type.Replace("mPower.Yodlee,", "mPower.Domain,");
                        isUpdated = true;
                    }

                    if (type.Contains("mPower.Accounting.Domain"))
                    {
                        type = type.Replace("mPower.Accounting.Domain", "mPower.Domain.Accounting");
                        type = type.Replace("mPower.Accounting,", "mPower.Domain,");
                        isUpdated = true;
                    }

                    if (isUpdated)
                    {
                        updatedCount++;
                    }
                    //throw new Exception("Event: " + type + " was not updated with new namespace");
                    item.GetElement("TypeId").Value = type;

                    if (!isUpdated)
                    {
                        var i = 1;
                    }
                }

                _write.Transitions.Save(transition);
            }
        }
    }
}