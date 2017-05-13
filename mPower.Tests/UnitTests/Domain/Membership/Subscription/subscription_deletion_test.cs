using System;
using System.Collections.Generic;
using System.Linq;
using mPower.Documents.Enums;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Membership.User.Events;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.Subscription
{
    public class subscription_deletion_test : SubscriptionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();

            yield return Subscription_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {
            yield return new User_Subscription_DeleteCommand()
            {
                UserId = _id,
                SubscriptionId = _subscriptionId,
                CancelMessage = String.Empty
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return new User_Subscription_DeletedEvent()
                             {
                                 CancelMessage = String.Empty,
                                 SubscriptionId = _subscriptionId,
                                 UserId = _id
                             };
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);
                Assert.True(user.Subscriptions.Single(x=> x.Id == _subscriptionId).Status == SubscriptionStatusEnum.Canceled);

                Assert.True(!user.HasPermissions(UserPermissionEnum.ViewPfm), "After user has deleted subscription to chargify we remove permission to access PFM");
            });
        }
    }
}
