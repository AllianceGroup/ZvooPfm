using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_SynchronizeChargifyProductsCommandHandler : IMessageHandler<Affiliate_SynchronizeChargifyProductsCommand>
    {
        private readonly Repository _repository;

        public Affiliate_SynchronizeChargifyProductsCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_SynchronizeChargifyProductsCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            affiliateAr.SynchronizeChargifyProducts(message.Products);

            _repository.Save(affiliateAr);
        }
    }
}
