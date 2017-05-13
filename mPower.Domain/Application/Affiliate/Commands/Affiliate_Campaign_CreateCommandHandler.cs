using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Campaign_CreateCommandHandler: IMessageHandler<Affiliate_Campaign_CreateCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Campaign_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Campaign_CreateCommand message)
        {
            var ar = _repository.GetById<AffiliateAR>(message.AffiliateId);
            ar.SetCommandMetadata(message.Metadata);
            ar.CreateCampaign(message.CampaignId, message.SegmentId, message.Offer);

            _repository.Save(ar);
        }
    }
}