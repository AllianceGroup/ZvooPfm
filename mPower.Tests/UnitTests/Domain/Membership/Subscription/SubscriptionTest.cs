using System;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Membership.User;
using mPower.Domain.Membership.User.Events;
using Paralect.Domain;
using mPower.Tests.Environment;

namespace mPower.Tests.UnitTests.Domain.Membership.Subscription
{
    public abstract class SubscriptionTest : AggregateTest<UserAR>
    {
        protected SubscriptionTest()
        {
            _currentDate = DateTime.Now;
        }

        protected string _email = "an.orsich@gmail.com";
        protected string _firstName = "Andrew";
        protected string _lastName = "Orsich";
        protected string _password = "asd123";
        protected string _userName = "anorsich";
        protected string _subscriptionId = "15asd15";

        protected string _fullNumber = "55555";
        protected string _organization = "Mpowering";
        protected string _productName = "PFM Suite";
        protected int _productPriceInCents = 2499;
        protected string _productHandle = "1";
        protected int _expirationMonth = 1;
        protected int _expirationYear = 2015;

        protected DateTime _currentDate;

        public IEvent User_Created()
        {
            return new User_CreatedEvent
            {
                UserId = _id,
                Email = _email,
                FirstName = _firstName,
                LastName = _lastName,
                Password = _password,
                UserName = _userName,
                CreateDate = _currentDate,
                IsActive = true,
                ApplicationId = "123"
            };
        }



        public IEvent Subscription_Created()
        {
            return new User_Subscription_CreatedEvent
            {
                UserId = _id,
               
                SubscriptionId = _subscriptionId
            };
        }

        public IEvent Subscription_Subscribed()
        {
            return new User_Subscription_SubscribedEvent
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
                SubscriptionId = _subscriptionId
            };
        }

        protected UserDocumentService _userDocumentService
        {
            get
            {
                return GetInstance<UserDocumentService>();
            }
        }
    }
}
