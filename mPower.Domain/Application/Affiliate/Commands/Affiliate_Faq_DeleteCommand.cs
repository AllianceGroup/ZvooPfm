using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Faq_DeleteCommand : Command
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }
    }
}
