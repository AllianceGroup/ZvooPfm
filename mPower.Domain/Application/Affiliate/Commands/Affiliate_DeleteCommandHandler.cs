using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_DeleteCommandHandler : IMessageHandler<Affiliate_DeleteCommand>
    {
        private readonly Repository _repository;

        public Affiliate_DeleteCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_DeleteCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.Id);
            affiliateAr.SetCommandMetadata(message.Metadata);
            affiliateAr.Delete();

            _repository.Save(affiliateAr);
        }
    }
}
