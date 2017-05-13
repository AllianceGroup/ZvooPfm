using System;
using System.Collections.Generic;
using System.Linq;
using Paralect.Domain;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.Segments;
using mPower.Domain.Application.Affiliate.Commands;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Framework;
using mPower.Schedule.Server.Environment;

namespace mPower.Schedule.Server.Jobs
{
    public class UpdateSegmentsJob : IScheduledJob
    {
        private readonly AffiliateDocumentService _affiliateService;
        private readonly CommandService _cmdService;
        private readonly SegmentEstimationHelper _estimationHelper;

        public bool IsEnabled
        {
            get { return true; }
        }

        public UpdateSegmentsJob(AffiliateDocumentService affiliateService, CommandService cmdService, SegmentEstimationHelper estimationHelper)
        {
            _affiliateService = affiliateService;
            _cmdService = cmdService;
            _estimationHelper = estimationHelper;
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("Update all affiliates' segments", GetType());
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("UpdateSegmentsOncePerDay", new DateTimeOffset(DateTime.UtcNow.Date.AddHours(3 + 6)), null, Int32.MaxValue, TimeSpan.FromHours(24)); // every day at 3 MTD (UTC -6)
        }

        public void Execute(IJobExecutionContext context)
        {
            var affiliates = _affiliateService.GetAll();
            var commands = new List<ICommand>();
            foreach (var aff in affiliates.Where(x => x.Segments.Any()))
            {
                var reestimatedSegments = aff.Segments.Select(x => Reestimate(x, aff.ApplicationId)).ToList();
                commands.Add(new Affiliate_Segments_ReestimateCommand
                {
                    AffiliateId = aff.ApplicationId,
                    ReestimatedSegments = reestimatedSegments
                });
            }
            _cmdService.SendAsync(commands.ToArray());
        }

        private SegmentData Reestimate(SegmentDocument segment, string affiliateId)
        {
            var data = new SegmentData
            {
                AffiliateId = affiliateId,
                AgeRangeFrom = segment.AgeRangeFrom,
                AgeRangeTo = segment.AgeRangeTo,
                CustomDateRangeEnd = segment.CustomDateRangeEnd,
                CustomDateRangeStart = segment.CustomDateRangeStart,
                Name = segment.Name,
                Id = segment.Id,
                DateRange = segment.DateRange,
                Gender = segment.Gender,
                Options = segment.Options,
                State = segment.State,
                ZipCodes = segment.ZipCodes,
                MerchantSelections = segment.MerchantSelections,
                SpendingCategories = segment.SpendingCategories,
            };

            float past30, past60, past90;
            data.MatchedUsers = _estimationHelper.CalculatedSegmentGrowth(data, out past30, out past60, out past90);
            data.Past30DaysGrowthInPct = past30;
            data.Past60DaysGrowthInPct = past60;
            data.Past90DaysGrowthInPct = past90;

            return data;
        }
    }
}