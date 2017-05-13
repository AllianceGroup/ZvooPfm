using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Email_Content_AddCommandHandler : IMessageHandler<Affiliate_Email_Content_AddCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Email_Content_AddCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Email_Content_AddCommand message)
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
                IsDefaultForEmailType = message.IsDefaultForEmailType,
                CreationDate = message.CreationDate,
                Status = message.Status,
            };
            affiliateAr.AddEmailContent(data);

            _repository.Save(affiliateAr);
        }
    }
}