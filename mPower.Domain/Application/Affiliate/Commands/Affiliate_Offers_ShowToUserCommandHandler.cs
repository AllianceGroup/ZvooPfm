using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Offers_ShowToUserCommandHandler : IMessageHandler<Affiliate_Offers_ShowToUserCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Offers_ShowToUserCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Offers_ShowToUserCommand message)
        {
            var ar = _repository.GetById<AffiliateAR>(message.UserAffiliateId);
            ar.SetCommandMetadata(message.Metadata);
            ar.ShowOffers(message.UserId, message.ShownAffiliateOffers);
            _repository.Save(ar);
        }
    }
}