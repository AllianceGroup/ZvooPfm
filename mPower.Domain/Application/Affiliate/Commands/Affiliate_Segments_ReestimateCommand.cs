using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Application.Affiliate.Data;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Segments_ReestimateCommand : Command
    {
        public string AffiliateId { get; set; }

        public List<SegmentData> ReestimatedSegments { get; set; }
    }
}