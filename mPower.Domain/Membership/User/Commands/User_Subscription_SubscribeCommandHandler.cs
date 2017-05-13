using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Subscription_SubscribeCommandHandler : IMessageHandler<User_Subscription_SubscribeCommand>
    {
        private readonly Repository _repository;

        public User_Subscription_SubscribeCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Subscription_SubscribeCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);

            var creditCardData = new CreditCardData()
            {
                BillingAddress = message.BillingAddress,
                BillingCity = message.BillingCity,
                BillingCountry = message.BillingCountry,
                BillingState = message.BillingState,
                BillingZip = message.BillingZip,
                CVV = message.CVV,
                ExpirationMonth = message.ExpirationMonth,
                ExpirationYear = message.ExpirationYear,
                FirstName = message.FirstNameCC,
                FullNumber = message.FullNumber,
                LastName = message.LastNameCC
            };

            var customerData = new CustomerData()
            {
                Email = message.Email,
                FirstName = message.FirstName,
                LastName = message.LastName,
                Organization = message.Organization,
                UserId = message.UserId
            };

            var productData = new ProductData
                                  {
                                      Name = message.ProductName,
                                      PriceInCents = message.ProductPriceInCents,
                                      Handle = message.ProductHandle,
                                  };
            user.SetCommandMetadata(message.Metadata);
            user.ActivateSubscription(productData, creditCardData, customerData, message.ChargifyCustomerSystemId, message.ChargifySuscriptionId);

            _repository.Save(user);
        }
    }
}
