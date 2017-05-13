using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_CreateCommandHandler : IMessageHandler<Affiliate_CreateCommand>
    {
        private readonly Repository _repository;

        public Affiliate_CreateCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_CreateCommand message)
        {
            var affiliateAr = new AffiliateAR(message.Id, message.Name, message.Metadata);

            _repository.Save(affiliateAr);
        }
    }
}
