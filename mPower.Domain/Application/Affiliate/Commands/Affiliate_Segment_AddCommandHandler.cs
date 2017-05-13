using System.Collections.Generic;
using mPower.Domain.Application.Affiliate.Data;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Application.Affiliate.Commands
{
    public class Affiliate_Segment_AddCommandHandler : IMessageHandler<Affiliate_Segment_AddCommand>
    {
        private readonly IRepository _repository;

        public Affiliate_Segment_AddCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(Affiliate_Segment_AddCommand message)
        {
            var affiliate = _repository.GetById<AffiliateAR>(message.AffiliateId);
            affiliate.SetCommandMetadata(message.Metadata);
            var data = new SegmentData
            {
                Id = message.Id,
                AffiliateId = message.AffiliateId,
                AgeRangeTo = message.AgeRangeTo,
                AgeRangeFrom = message.AgeRangeFrom,
                Name = message.Name,
                CustomDateRangeEnd = message.CustomDateRangeEnd,
                CustomDateRangeStart = message.CustomDateRangeStart,
                DateRange = message.DateRange,
                Gender = message.Gender,
                Options = new List<SegmentOption>(message.Options),
                MerchantSelections = message.MerchantSelections,
                SpendingCategories = message.SpendingCategories,
                State = message.State,
                ZipCodes = message.ZipCodes,
                LastModifiedDate = message.CreatedDate,
                MatchedUsers = message.MatchedUsers,
                Past30DaysGrowthInPct = message.Past30DaysGrowthInPct,
                Past60DaysGrowthInPct = message.Past60DaysGrowthInPct,
                Past90DaysGrowthInPct = message.Past90DaysGrowthInPct,
            };
            affiliate.AddSegment(data);
            _repository.Save(affiliate);
        }
    }
}