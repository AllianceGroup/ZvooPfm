using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Segment_DeleteCommandHandler : IMessageHandler<Affiliate_Segment_DeleteCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Segment_DeleteCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Segment_DeleteCommand message)
        {
            var affiliate = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliate.SetCommandMetadata(message.Metadata);
            affiliate.DeleteSegment(message.Id, message.AffiliateId);
            _repository.Save(affiliate);
        }
    }
}