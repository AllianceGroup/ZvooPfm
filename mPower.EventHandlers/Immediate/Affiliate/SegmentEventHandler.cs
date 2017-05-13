using MongoDB.Bson;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.DocumentServices;
using mPower.Documents.Segments;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Affiliate.Events;
using Paralect.ServiceBus;
using System;
using System.Collections.Generic;

namespace mPower.EventHandlers.Immediate.Affiliate
{
    public class SegmentEventHandler :
        IMessageHandler<Affiliate_Segment_AddedEvent>,
        IMessageHandler<Affiliate_Segment_ChangedEvent>,
        IMessageHandler<Affiliate_Segment_DeletedEvent>,
        IMessageHandler<Affiliate_Segments_ReestimatedEvent>
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly SegmentEstimationHelper _estimationHelper;

        public SegmentEventHandler(AffiliateDocumentService affiliateService, SegmentEstimationHelper estimationHelper)
        {
            _affiliateService = affiliateService;
            _estimationHelper = estimationHelper;
        }

        public void Handle(Affiliate_Segment_AddedEvent message)
        {
            var createdDate = message.CreatedDate;
            var matchedUsers = message.MatchedUsers;
            var past30 = message.Past30DaysGrowthInPct;
            var past60 = message.Past60DaysGrowthInPct;
            var past90 = message.Past90DaysGrowthInPct;
            if (matchedUsers == null)
            {
                matchedUsers = _estimationHelper.CalculatedSegmentGrowth(Map(message), out past30, out past60, out past90);
                if (message.CreatedDate == default(DateTime))
                {
                    createdDate = message.Metadata.StoredDate;
                }
            }
            var doc = new SegmentDocument
            {
                Id = message.Id,
                Name = message.Name,
                AgeRangeTo = message.AgeRangeTo,
                AgeRangeFrom = message.AgeRangeFrom,
                DateRange = message.DateRange,
                Gender = message.Gender,
                Options = message.Options,
                CustomDateRangeEnd = message.CustomDateRangeEnd,
                CustomDateRangeStart = message.CustomDateRangeStart,
                MerchantSelections = message.MerchantSelections,
                SpendingCategories = message.SpendingCategories,
                State = message.State,
                ZipCodes = message.ZipCodes ?? new List<string>(),
                LastModifiedDate = createdDate,
                MatchedUsers = matchedUsers,
                Past30DaysGrowthInPct = past30,
                Past60DaysGrowthInPct = past60,
                Past90DaysGrowthInPct = past90,
            };

            var query = Query.EQ("_id", message.AffiliateId);
            var update = Update.Push("Segments", doc.ToBsonDocument());
            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Segment_ChangedEvent message)
        {
            var updatedDate = message.UpdatedDate;
            var matchedUsers = message.MatchedUsers;
            var past30 = message.Past30DaysGrowthInPct;
            var past60 = message.Past60DaysGrowthInPct;
            var past90 = message.Past90DaysGrowthInPct;
            if (matchedUsers == null)
            {
                matchedUsers = _estimationHelper.CalculatedSegmentGrowth(Map(message), out past30, out past60, out past90);
                if (message.UpdatedDate == default(DateTime))
                {
                    updatedDate = message.Metadata.StoredDate;
                }
            }

            var affiliate = _affiliateService.GetById(message.AffiliateId);
            if (affiliate != null)
            {
                var segment = affiliate.Segments.Find(x => x.Id == message.Id);
                if (segment != null)
                {
                    segment.AgeRangeFrom = message.AgeRangeFrom;
                    segment.AgeRangeTo = message.AgeRangeTo;
                    segment.Name = message.Name;
                    segment.State = message.State;
                    segment.DateRange = message.DateRange;
                    segment.CustomDateRangeEnd = message.CustomDateRangeEnd;
                    segment.CustomDateRangeStart = message.CustomDateRangeStart;
                    segment.Gender = message.Gender;
                    segment.ZipCodes = message.ZipCodes ?? new List<string>();
                    segment.Options = message.Options;
                    segment.MerchantSelections = message.MerchantSelections ?? new List<MerchantSelectItem>();
                    segment.SpendingCategories = message.SpendingCategories ?? new List<string>();
                    segment.LastModifiedDate = updatedDate;
                    segment.MatchedUsers = matchedUsers ?? new List<SegmentUserData>();
                    segment.Past30DaysGrowthInPct = past30;
                    segment.Past60DaysGrowthInPct = past60;
                    segment.Past90DaysGrowthInPct = past90;

                    _affiliateService.Save(affiliate);
                }
            }
        }

        public void Handle(Affiliate_Segment_DeletedEvent message)
        {
            var query = Query.EQ("_id", message.AffiliateId);
            var innerQuery = Query.EQ("_id", message.Id);
            var update = Update.Pull("Segments", innerQuery);
            _affiliateService.Update(query, update);
        }

        public void Handle(Affiliate_Segments_ReestimatedEvent message)
        {
            var affiliate = _affiliateService.GetById(message.AffiliateId);
            if (affiliate != null && affiliate.Segments.Count > 0)
            {
                foreach (var segment in affiliate.Segments)
                {
                    var data = message.ReestimatedSegments.Find(x => x.Id == segment.Id);
                    if (data != null)
                    {
                        var matchedUsers = data.MatchedUsers;
                        var past30 = data.Past30DaysGrowthInPct;
                        var past60 = data.Past60DaysGrowthInPct;
                        var past90 = data.Past90DaysGrowthInPct;
                        if (matchedUsers == null)
                        {
                            matchedUsers = _estimationHelper.CalculatedSegmentGrowth(data, out past30, out past60, out past90);
                        }
                        segment.MatchedUsers = matchedUsers;
                        segment.Past30DaysGrowthInPct = past30;
                        segment.Past60DaysGrowthInPct = past60;
                        segment.Past90DaysGrowthInPct = past90;
                    }
                }

                _affiliateService.Save(affiliate);
            }
        }

        private static SegmentData Map(Affiliate_Segment_AddedEvent message)
        {
            return new SegmentData
            {
                AffiliateId = message.AffiliateId,
                AgeRangeFrom = message.AgeRangeFrom,
                AgeRangeTo = message.AgeRangeTo,
                CustomDateRangeEnd = message.CustomDateRangeEnd,
                CustomDateRangeStart = message.CustomDateRangeStart,
                Name = message.Name,
                Id = message.Id,
                DateRange = message.DateRange,
                Gender = message.Gender,
                Options = message.Options,
                State = message.State,
                ZipCodes = message.ZipCodes,
                MerchantSelections = message.MerchantSelections,
                SpendingCategories = message.SpendingCategories,
            };
        }

        private static SegmentData Map(Affiliate_Segment_ChangedEvent message)
        {
            return new SegmentData
            {
                AffiliateId = message.AffiliateId,
                AgeRangeFrom = message.AgeRangeFrom,
                AgeRangeTo = message.AgeRangeTo,
                CustomDateRangeEnd = message.CustomDateRangeEnd,
                CustomDateRangeStart = message.CustomDateRangeStart,
                Name = message.Name,
                Id = message.Id,
                DateRange = message.DateRange,
                Gender = message.Gender,
                Options = message.Options,
                State = message.State,
                ZipCodes = message.ZipCodes,
                MerchantSelections = message.MerchantSelections,
                SpendingCategories = message.SpendingCategories,
            };
        }
    }
}