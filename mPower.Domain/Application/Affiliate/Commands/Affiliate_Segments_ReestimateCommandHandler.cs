using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Segments_ReestimateCommandHandler : IMessageHandler<Affiliate_Segments_ReestimateCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Segments_ReestimateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Segments_ReestimateCommand message)
        {
            var affiliate = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliate.SetCommandMetadata(message.Metadata);
            affiliate.ReestimateSegments(message.ReestimatedSegments);
            _repository.Save(affiliate);
        }
    }
}