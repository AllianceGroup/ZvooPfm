using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_NotificationTypeEmail_UpdateCommandHandler : IMessageHandler<Affiliate_NotificationTypeEmail_UpdateCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_NotificationTypeEmail_UpdateCommandHandler(IRepository repository)
         {
             _repository = repository;
         }

        public void Handle(Affiliate_NotificationTypeEmail_UpdateCommand message)
        {
            var affiliate = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliate.SetCommandMetadata(message.Metadata);
            var data = new NotificationTypeEmailData
            {
                EmailType = message.EmailType,
                EmailContentId = message.EmailContentId,
                Status = message.Status,
            };
            affiliate.UpdateNotificationTypeEmail(data);

            _repository.Save(affiliate);
        }
    }
}