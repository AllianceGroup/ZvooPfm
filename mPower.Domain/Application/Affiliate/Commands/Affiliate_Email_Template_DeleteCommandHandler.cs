using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Email_Template_DeleteCommandHandler : IMessageHandler<Affiliate_Email_Template_DeleteCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Email_Template_DeleteCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Email_Template_DeleteCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            affiliateAr.DeleteEmailTemplate(message.Id);

            _repository.Save(affiliateAr);
        }
    }
}