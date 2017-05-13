using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Email_Content_UpdateCommandHandler : IMessageHandler<Affiliate_Email_Content_UpdateCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Email_Content_UpdateCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Email_Content_UpdateCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            var data = new EmailContentData
            {
                Id = message.Id,
                TemplateId = message.TemplateId,
                Name = message.Name,
                Subject = message.Subject,
                Html = message.Html,
                Status = message.Status,
            };
            affiliateAr.UpdateEmailContent(data);

            _repository.Save(affiliateAr);
        }
    }
}