using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
   public class Affiliate_Faq_UpdateCommandHandler : IMessageHandler<Affiliate_Faq_UpdateCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Faq_UpdateCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Faq_UpdateCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            affiliateAr.UpdateFaq(message.Id, message.Name, message.Html, message.IsActive);

            _repository.Save(affiliateAr);
        }
    }
}
