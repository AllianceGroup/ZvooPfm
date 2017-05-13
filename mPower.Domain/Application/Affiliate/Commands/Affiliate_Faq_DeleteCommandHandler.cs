using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
   public class Affiliate_Faq_DeleteCommandHandler : IMessageHandler<Affiliate_Faq_DeleteCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Faq_DeleteCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Faq_DeleteCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            affiliateAr.DeleteFaq(message.Id);

            _repository.Save(affiliateAr);
        }
    }
}