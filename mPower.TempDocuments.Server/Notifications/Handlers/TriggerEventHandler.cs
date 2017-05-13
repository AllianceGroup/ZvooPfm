using mPower.Domain.Accounting.CreditIdentity.Events;
using mPower.Domain.Accounting.Ledger.Events;
using mPower.Domain.Membership.User.Events;
using Paralect.ServiceBus;
using mPower.TempDocuments.Server.Notifications.Documents;
using mPower.Domain.Application.Enums;
using mPower.TempDocuments.Server.Notifications.Documents.Triggers;

namespace mPower.TempDocuments.Server.Notifications.Handlers
{
    public class TriggerEventHandler :
        IMessageHandler<User_CreatedEvent>,
        IMessageHandler<User_ActivatedEvent>,
        IMessageHandler<User_DeactivatedEvent>,
        IMessageHandler<Ledger_Account_AddedEvent>,
        IMessageHandler<CreditIdentity_CreatedEvent>
    {
        private readonly TriggerBuilder _triggerBuilder;

        public TriggerEventHandler(TriggerBuilder triggerBuilder)
        {
            _triggerBuilder = triggerBuilder;
        }

        public void Handle(User_CreatedEvent message)
        {
            _triggerBuilder.CreateAndSave<BaseNotification>(message.UserId, EmailTypeEnum.Signup);
        }

        public void Handle(User_ActivatedEvent message)
        {
            if (message.IsAdmin)
                _triggerBuilder.CreateAndSave<BaseNotification>(message.UserId, EmailTypeEnum.UserReactivation);
        }

        public void Handle(User_DeactivatedEvent message)
        {
            if (message.IsAdmin)
                _triggerBuilder.CreateAndSave<BaseNotification>(message.UserId, EmailTypeEnum.UserDeactivation);
        }

        public void Handle(Ledger_Account_AddedEvent message)
        {
            if (message.Aggregated)
            {
                _triggerBuilder.CreateAndSave<NewAggregatedAccountTriggerNotification>(message.Metadata.UserId, EmailTypeEnum.NewAccountAggregation, (n)=>
                {
                    n.AccountId = message.AccountId;
                    n.AccountName = message.Name;
                });
            }
        }

        public void Handle(CreditIdentity_CreatedEvent message)
        {
            _triggerBuilder.CreateAndSave<NewCreditIdentityTriggerNotification>(message.UserId, EmailTypeEnum.NewCreditIdentity, (c) =>
            {
                c.CreditIdentityId = message.Data.ClientKey;
                c.CreditIdentitySocial = message.Data.SocialSecurityNumber;
            });
        }



    }
}
