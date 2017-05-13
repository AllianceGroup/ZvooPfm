using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Email_Template_UpdateCommandHandler : IMessageHandler<Affiliate_Email_Template_UpdateCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Email_Template_UpdateCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Email_Template_UpdateCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            affiliateAr.UpdateEmailTemplate(message.Id, message.Name, message.Html, message.Status);

            _repository.Save(affiliateAr);
        }
    }
}