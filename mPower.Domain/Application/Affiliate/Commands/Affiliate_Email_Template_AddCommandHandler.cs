using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Email_Template_AddCommandHandler : IMessageHandler<Affiliate_Email_Template_AddCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Email_Template_AddCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Email_Template_AddCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            var data = new EmailTemplateData
            {
                Id = message.Id,
                Name = message.Name,
                Html = message.Html,
                IsDefault = message.IsDefault,
                CreationDate = message.CreationDate,
                Status = message.Status,
            };
            affiliateAr.AddEmailTemplate(data);

            _repository.Save(affiliateAr);
        }
    }
}