using System;
using System.Linq;
using Default.Areas.Administration.Models;
using mPower.Documents.Segments;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Framework.Environment;
using mPower.Framework.Mvc;

namespace Default.Factories.Commands
{
    public class SegmentCommandFactory : 
        IObjectFactory<SegmentModel, Affiliate_Segment_AddCommand>,
        IObjectFactory<SegmentModel, Affiliate_Segment_UpdateCommand>
    {
        private readonly IIdGenerator _idGenerator;
        private readonly SegmentEstimationHelper _estimationHelper;
        private readonly ObjectRepository _objectRepository;

        public SegmentCommandFactory(IIdGenerator idGenerator, SegmentEstimationHelper estimationHelper, ObjectRepository objectRepository)
        {
            _idGenerator = idGenerator;
            _estimationHelper = estimationHelper;
            _objectRepository = objectRepository;
        }

        Affiliate_Segment_AddCommand IObjectFactory<SegmentModel, Affiliate_Segment_AddCommand>.Load(SegmentModel model)
        {
            float past30, past60, past90;
            var command = new Affiliate_Segment_AddCommand
            {
                Id = _idGenerator.Generate(),
                AffiliateId = model.AffiliateId,
                AgeRangeFrom = model.AgeRangeFrom,
                AgeRangeTo = model.AgeRangeTo,
                DateRange = model.DateRange,
                Options = model.AllOptions.Select(_objectRepository.Load<SegmentOptionModel, SegmentOption>),
                CustomDateRangeEnd = model.CustomDateRangeEnd,
                CustomDateRangeStart = model.CustomDateRangeStart,
                Gender = model.Gender,
                State = model.State ?? string.Empty,
                ZipCodes = model.ZipCodes.Where(x => (x ?? "").Trim().IsNotNullOrEmpty()).Distinct().ToList(),
                MerchantSelections = model.MerchantSelections.Where(x => (x.MerchantName ?? "").Trim().IsNotNullOrEmpty()).ToList(),
                SpendingCategories = model.SpendingCategories.Where(x => (x ?? "").Trim().IsNotNullOrEmpty()).Distinct().ToList(),
                Name = model.Name,
                CreatedDate = DateTime.Now,
                MatchedUsers = _estimationHelper.CalculatedSegmentGrowth(_objectRepository.Load<SegmentModel, SegmentData>(model), out past30, out past60, out past90),
                Past30DaysGrowthInPct = past30,
                Past60DaysGrowthInPct = past60,
                Past90DaysGrowthInPct = past90,
            };

            return command;
        }

        Affiliate_Segment_UpdateCommand IObjectFactory<SegmentModel, Affiliate_Segment_UpdateCommand>.Load(SegmentModel model)
        {
            float past30, past60, past90;
            var command = new Affiliate_Segment_UpdateCommand
            {
                Id = model.Id,
                AffiliateId = model.AffiliateId,
                AgeRangeFrom = model.AgeRangeFrom,
                AgeRangeTo = model.AgeRangeTo,
                DateRange = model.DateRange,
                Options = model.AllOptions.Select(Map),
                CustomDateRangeEnd = model.CustomDateRangeEnd,
                CustomDateRangeStart = model.CustomDateRangeStart,
                Gender = model.Gender,
                State = model.State ?? string.Empty,
                ZipCodes = model.ZipCodes.Where(x => (x ?? "").Trim().IsNotNullOrEmpty()).Distinct().ToList(),
                MerchantSelections = model.MerchantSelections.Where(x => (x.MerchantName ?? "").Trim().IsNotNullOrEmpty()).ToList(),
                SpendingCategories = model.SpendingCategories.Where(x => (x ?? "").Trim().IsNotNullOrEmpty()).Distinct().ToList(),
                Name = model.Name,
                UpdatedDate = DateTime.Now,
                MatchedUsers = _estimationHelper.CalculatedSegmentGrowth(_objectRepository.Load<SegmentModel, SegmentData>(model), out past30, out past60, out past90),
                Past30DaysGrowthInPct = past30,
                Past60DaysGrowthInPct = past60,
                Past90DaysGrowthInPct = past90,
            };

            return command;
        }

        private static SegmentOption Map(SegmentOptionModel x)
        {
            return new SegmentOption
            {
                Value = x.Value,
                Condition = x.Condition,
                Enabled = x.Enabled,
                Comparer = x.Comparer,
                Type = x.Type,
                Name = x.Name,
                Trend = x.Trend,
                Multiplier = x.Multiplier,
                Frequency = x.Frequency,
            };
        }
    }
}