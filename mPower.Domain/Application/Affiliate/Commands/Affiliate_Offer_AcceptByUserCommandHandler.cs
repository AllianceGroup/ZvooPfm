using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Offer_AcceptByUserCommandHandler : IMessageHandler<Affiliate_Offer_AcceptByUserCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Offer_AcceptByUserCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Offer_AcceptByUserCommand message)
        {
            var ar = _repository.GetById<AffiliateAR>(message.UserAffiliateId);
            ar.SetCommandMetadata(message.Metadata);
            ar.AcceptOffer(message.UserId, message.OfferAffiliateId, message.OfferId, message.Date);
            _repository.Save(ar);
        }
    }
}