using System.Collections.Generic;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using NUnit.Framework;

namespace mPower.Tests.UnitTests.Domain.Membership.Subscription
{
    public class subscription_subscribed_test : SubscriptionTest
    {
        public override IEnumerable<Paralect.Domain.IEvent> Given()
        {
            yield return User_Created();
            yield return Subscription_Created();
        }

        public override IEnumerable<Paralect.Domain.ICommand> When()
        {

            yield return new User_Subscription_SubscribeCommand
            {
                UserId = _id,
                Email = _email,
                FirstName = _firstName,
                LastName = _lastName,
                ExpirationMonth = _expirationMonth,
                ExpirationYear = _expirationYear,
                FirstNameCC = _firstName,
                FullNumber = _fullNumber,
                LastNameCC = _lastName,
                Organization = _organization,
                ProductName = _productName,
                ProductPriceInCents = _productPriceInCents,
                ProductHandle = _productHandle,
                ChargifyCustomerSystemId = _subscriptionId
            };
        }

        public override IEnumerable<Paralect.Domain.IEvent> Expected()
        {
            yield return Subscription_Subscribed();
        }

        [Test]
        public void Test()
        {
            Validate();

            DispatchEvents(() =>
            {
                var user = _userDocumentService.GetById(_id);

                Assert.AreEqual(user.Subscriptions.Count, 1);

                var subscription = user.Subscriptions[0];

                Assert.AreEqual(subscription.Email, _email);
                Assert.AreEqual(subscription.FirstName, _firstName);
                Assert.AreEqual(subscription.LastName, _lastName);
                Assert.AreEqual(subscription.ExpirationMonth, _expirationMonth);
                Assert.AreEqual(subscription.ExpirationYear, _expirationYear);
                Assert.AreEqual(subscription.FirstNameCC, _firstName);
                Assert.AreEqual(subscription.FullNumber, _fullNumber);
                Assert.AreEqual(subscription.LastNameCC, _lastName);
                Assert.AreEqual(subscription.LastNameCC, _lastName);
                Assert.AreEqual(subscription.Organization, _organization);
                Assert.AreEqual(subscription.ProductHandle, _productHandle);

                Assert.True(user.HasPermissions(UserPermissionEnum.ViewPfm), "After user succesfully subscribed to chargify he should have ViewPfm permission");
            });
        }
    }

    
}
