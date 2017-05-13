using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Campaign_Settings_UpdateCommandHandler: IMessageHandler<Affiliate_Campaign_Settings_UpdateCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Campaign_Settings_UpdateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Campaign_Settings_UpdateCommand message)
        {
            var ar = _repository.GetById<AffiliateAR>(message.AffiliateId);
            ar.SetCommandMetadata(message.Metadata);
            ar.UpdateCampaignSettings(message.CampaignId, message.Settings);

            _repository.Save(ar);
        }
    }
}