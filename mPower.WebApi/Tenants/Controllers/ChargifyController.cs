using System.Linq;
using ChargifyNET;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Credit;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.ExternalServices;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class ChargifyController : BaseController
    {
        private readonly ChargifyService _chargifyService;
        private readonly UserDocumentService _userService;
        private readonly IIdGenerator _idGenerator;
        private readonly AccountsService _accountsService;
        private readonly LedgerDocumentService _ledgerDocumentService;
        private readonly CreditIdentityDocumentService _creditIdentityService;

        public ChargifyController(ChargifyService chargifyService, UserDocumentService userService, IIdGenerator idGenerator, 
            AccountsService accountsService, LedgerDocumentService ledgerDocumentService,
            CreditIdentityDocumentService creditIdentityService, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _chargifyService = chargifyService;
            _userService = userService;
            _idGenerator = idGenerator;
            _accountsService = accountsService;
            _ledgerDocumentService = ledgerDocumentService;
            _creditIdentityService = creditIdentityService;
        }

        [HttpPost("subscribeuser")]
        public IActionResult SubscribeUser(int id)
        {
            _chargifyService.Connect(Tenant.ChargifyUrl, Tenant.ChargifySharedKey, Tenant.ChargifyApiKey);
            var subscription = _chargifyService.GetSubscription(id);

            if (subscription == null)
                return new BadRequestObjectResult($"A valid subscription was not found.  subscription_id = {id}");

            #region update user with chargify info
            var user = _userService.GetByChargifySystemId(subscription.Customer.SystemID);
            var updateUser = new User_UpdateCommand
            {
                Email = subscription.Customer.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserId = user.Id,
            };
            Send(updateUser);
            #endregion

            #region create subscription in system from chargify subscription
            var subscribeCommand = CreateSubscribeCommand(user, subscription);
            Send(subscribeCommand);
            #endregion

            #region activate user
            var activateUserCommand = new User_ActivateCommand()
            {
                UserId = user.Id
            };
            Send(activateUserCommand);
            #endregion

            #region create Personal Ledger if it not created yet
            if (_ledgerDocumentService.GetByUserId(user.Id).Count(x => x.TypeEnum == LedgerTypeEnum.Personal) == 0)
            {
                var ledgerId = _idGenerator.Generate();
                var setupPersonalLedgerCmds = _accountsService.SetupPersonalLedger(user.Id, ledgerId);

                foreach (var setupPersonalLedgerCmd in setupPersonalLedgerCmds)
                    Send(setupPersonalLedgerCmd);
            }
            #endregion

            return new OkObjectResult("Subscription Successful.  Please login.");
        }

        private User_Subscription_SubscribeCommand CreateSubscribeCommand(UserDocument user, ISubscription subscription)
        {
            var subscribeCommand = new User_Subscription_SubscribeCommand
            {
                BillingAddress = "n/a",
                BillingCity = "n/a",
                BillingCountry = "n/a",
                BillingState = "n/a",
                BillingZip = "n/a",
                CVV = "n/a",
                Email = subscription.Customer.Email,
                FirstName = subscription.Customer.FirstName,
                LastName = subscription.Customer.LastName,
                Organization = subscription.Customer.Organization,
                UserId = user.Id,
                //subscription.Customer.SystemID, // Changee to Subscription ID then find user by subscriptionId Subscriptions._id
                ChargifyCustomerSystemId = subscription.Customer.SystemID,
                //subscription.SubscriptionID.ToString(),
                ProductName = subscription.Product.Name,
                ProductPriceInCents = subscription.Product.PriceInCents,
                ProductHandle = subscription.Product.Handle,
                ChargifySuscriptionId = subscription.SubscriptionID
            };

            if (subscription.PaymentProfile != null)
            {
                subscribeCommand.FirstNameCC = subscription.PaymentProfile.FirstName;
                subscribeCommand.LastNameCC = subscription.PaymentProfile.LastName;
                subscribeCommand.FullNumber = subscription.PaymentProfile.FullNumber;
                subscribeCommand.ExpirationMonth = subscription.PaymentProfile.ExpirationMonth;
                subscribeCommand.ExpirationYear = subscription.PaymentProfile.ExpirationYear;
            }

            return subscribeCommand;
        }
    }
}