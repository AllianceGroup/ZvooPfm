using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mPower.Domain.Application.Affiliate.Data
{
    public class FaqData
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Html { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreationDate { get; set; }
    }
}
