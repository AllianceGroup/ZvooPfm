using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_NotificationTypeEmail_CreateCommandHandler : IMessageHandler<Affiliate_NotificationTypeEmail_CreateCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_NotificationTypeEmail_CreateCommandHandler(IRepository repository)
         {
             _repository = repository;
         }

        public void Handle(Affiliate_NotificationTypeEmail_CreateCommand message)
        {
            var affiliate = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliate.SetCommandMetadata(message.Metadata);
            var data = new NotificationTypeEmailData
            {
                Name = message.Name,
                EmailType = message.EmailType,
                EmailContentId = message.EmailContentId,
                Status = message.Status,
            };
            affiliate.AddNotificationTypeEmail(data);

            _repository.Save(affiliate);
        }
    }
}