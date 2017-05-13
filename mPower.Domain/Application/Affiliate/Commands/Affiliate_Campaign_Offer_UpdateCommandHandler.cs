using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Campaign_Offer_UpdateCommandHandler: IMessageHandler<Affiliate_Campaign_Offer_UpdateCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Campaign_Offer_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Campaign_Offer_UpdateCommand message)
        {
            var ar = _repository.GetById<AffiliateAR>(message.AffiliateId);
            ar.SetCommandMetadata(message.Metadata);
            ar.UpdateCampaignOffer(message.CampaignId, message.Offer);

            _repository.Save(ar);
        }
    }
}