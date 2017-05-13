using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using mPower.Domain.Membership.User.Commands;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.Subscription
{
    public class subscription_creation_test : SubscriptionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_Subscription_CreateCommand
            {
                UserId = _id,
                SubscriptionId = _subscriptionId
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Subscription_Created();
            
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);

                Assert.AreEqual(user.Subscriptions.Count, 1);
                
            });
        }
    }

    public static class MongodbExtentions
    {
        public static T FindOne<T>(this MongoCollection collection, params string[] excludedFields)
        {
            return collection.FindAllAs<T>().SetLimit(1).SetFields(excludedFields).FirstOrDefault();
        }
    }
}
