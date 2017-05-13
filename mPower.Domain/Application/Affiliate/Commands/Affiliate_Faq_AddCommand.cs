using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paralect.Domain;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Faq_AddCommand : Command
    {
        public string Id { get; set; }

        public string AffiliateId { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationDate { get; set; }

        public Affiliate_Faq_AddCommand()
        {
            CreationDate = DateTime.Now;
        }
    }
}
