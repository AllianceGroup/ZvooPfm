using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mPower.Domain.Application.Affiliate.Data;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
  public class Affiliate_Faq_AddCommandHandler : IMessageHandler<Affiliate_Faq_AddCommand>
    {
        private readonly Repository _repository;

        public Affiliate_Faq_AddCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Faq_AddCommand message)
        {
            var affiliateAr = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliateAr.SetCommandMetadata(message.Metadata);
            var data = new FaqData
            {
                Id = message.Id,
                Name = message.Name,
                Html = message.Html,
                IsActive = message.IsActive,
                CreationDate = message.CreationDate,               
            };
            affiliateAr.AddFaq(data);

            _repository.Save(affiliateAr);
        }
    }
}