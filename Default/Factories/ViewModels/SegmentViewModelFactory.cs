using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Default.Areas.Administration.Models;
using Default.Helpers;
using MongoDB.Driver.Builders;
using mPower.Documents.DocumentServices;
using mPower.Documents.Documents.Affiliate;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;

namespace Default.Factories.ViewModels
{
    public class SegmentViewModelFactory : 
        IObjectFactory<SegmentDocument, SegmentsListItemModel>,
        IObjectFactory<SegmentModel, SegmentData>,
        IObjectFactory<SegmentOptionModel, SegmentOption>,
        IObjectFactory<string, SegmentModel>
    {
        private readonly IObjectRepository _objectRepository;
        private readonly AffiliateDocumentService _affiliateService;
        private readonly SegmentViewHelper _helper;

        public SegmentViewModelFactory(IObjectRepository objectRepository, AffiliateDocumentService affiliateService, SegmentViewHelper helper)
        {
            _objectRepository = objectRepository;
            _affiliateService = affiliateService;
            _helper = helper;
        }

        public SegmentsListItemModel Load(SegmentDocument document)
        {
            var model = new SegmentsListItemModel
            {
                Id = document.Id,
                Reach = document.EstimatedReach.ToString(CultureInfo.InvariantCulture),
                Name = document.Name,
                Past30DaysGrowthInPct = document.Past30DaysGrowthInPct,
                Past60DaysGrowthInPct = document.Past60DaysGrowthInPct,
                Past90DaysGrowthInPct = document.Past90DaysGrowthInPct,
                AllOptions = new SegmentModel().AllOptions,
                BasicOptions = new List<String>(),
            };

            if (document.MerchantSelections != null && document.MerchantSelections.Any())
            {
                model.BasicOptions.Add(String.Format("Shops at: <span>{0}</span>", string.Join(",", document.MerchantSelections.Select(x=> x.MerchantName))));
            }
            if (document.Gender.HasValue)
            {
                model.BasicOptions.Add(String.Format("Gender is <span>{0}</span>", document.Gender));
            }
            if (document.AgeRangeFrom.HasValue || document.AgeRangeTo.HasValue)
            {
                var from = document.AgeRangeFrom;
                var to = document.AgeRangeTo;
                model.BasicOptions.Add("Age range" + (from.HasValue ? String.Format(" from <span>{0}</span>", from) : "") + (to.HasValue ? String.Format(" to <span>{0}</span>", to) : ""));
            }
            if (document.DateRange.HasValue)
            {
                if (document.DateRange.Value == DateRangeEnum.Custom && (document.CustomDateRangeStart.HasValue || document.CustomDateRangeEnd.HasValue))
                {
                    var from = document.CustomDateRangeStart;
                    var to = document.CustomDateRangeEnd;
                    model.BasicOptions.Add("Date range" + (from.HasValue ? String.Format(" from <span>{0:M/d/yyyy}</span>", from) : "") + (to.HasValue ? String.Format(" to <span>{0:M/d/yyyy}</span>", to) : ""));
                }
                else
                {
                    model.BasicOptions.Add(String.Format("Date range is <span>{0}</span>", document.DateRange.GetDescription()));
                }
            }
            if (!string.IsNullOrEmpty(document.State))
            {
                model.BasicOptions.Add(String.Format("State is <span>{0}</span>", document.State));
            }
            if (document.SpendingCategories != null && document.SpendingCategories.Any())
            {
                model.BasicOptions.Add(String.Format("Spending categories: <span>{0}</span>", string.Join(",", document.SpendingCategories)));
            }
            if (document.Options != null)
            {
                ApplyOptions(document.Options, model.AllOptions);
            }
            return model;
        }

        public SegmentData Load(SegmentModel segment)
        {
            var data = new SegmentData
            {
                Id = segment.Id,
                AffiliateId = segment.AffiliateId,
                AgeRangeFrom = segment.AgeRangeFrom,
                AgeRangeTo = segment.AgeRangeTo,
                DateRange = segment.DateRange,
                Options = segment.AllOptions.Select(_objectRepository.Load<SegmentOptionModel, SegmentOption>).ToList(),
                CustomDateRangeEnd = segment.CustomDateRangeEnd,
                CustomDateRangeStart = segment.CustomDateRangeStart,
                Gender = segment.Gender,
                State = segment.State ?? String.Empty,
                ZipCodes = segment.ZipCodes.Where(x => (x ?? "").Trim().IsNotNullOrEmpty()).Distinct().ToList(),
                MerchantSelections = segment.MerchantSelections.Where(x => (x.MerchantName ?? "").Trim().IsNotNullOrEmpty()).ToList(),
                SpendingCategories = segment.SpendingCategories.Where(x => (x ?? "").Trim().IsNotNullOrEmpty()).Distinct().ToList(),
                Name = segment.Name,
            };

            return data;
        }

        public SegmentOption Load(SegmentOptionModel x)
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
                Frequency = x.Frequency
            };
        }

        public SegmentModel Load(string segmentId)
        {
            SegmentModel model = null;
            var affiliate = _affiliateService.GetByQuery(Query.EQ("Segments._id", segmentId)).FirstOrDefault();

            if (affiliate != null)
            {
                var document = affiliate.Segments.Find(x => x.Id == segmentId);

                if (document != null)
                {
                    model = new SegmentModel
                    {
                        Id = document.Id,
                        AffiliateId = affiliate.ApplicationId,
                        Name = document.Name,
                        Reach = document.EstimatedReach,
                        Gender = document.Gender,
                        AgeRangeFrom = document.AgeRangeFrom,
                        AgeRangeTo = document.AgeRangeTo,
                        DateRange = document.DateRange,
                        CustomDateRangeEnd = document.CustomDateRangeEnd,
                        CustomDateRangeStart = document.CustomDateRangeStart,
                        State = document.State,
                    };

                    if (document.ZipCodes != null && document.ZipCodes.Any())
                    {
                        model.ZipCodes = document.ZipCodes;
                    }

                    if (document.MerchantSelections != null && document.MerchantSelections.Count > 0)
                    {
                        model.MerchantSelections = document.MerchantSelections;
                    }

                    if (document.SpendingCategories != null && document.SpendingCategories.Count > 0)
                    {
                        model.SpendingCategories = document.SpendingCategories;
                    }
                    if (document.Options != null)
                    {
                        ApplyOptions(document.Options, model.AllOptions);
                    }
                    _helper.FormatReachNumber(model);
                }
            }

            return model;
        }

        private static void ApplyOptions(IEnumerable<SegmentOption> @from, List<SegmentOptionModel> to)
        {
            foreach (var document in @from.Where(x => x.Enabled))
            {
                var modelOption = to.Find(x => x.Name == document.Name);
                if (modelOption != null)
                {
                    modelOption.Enabled = document.Enabled;
                    modelOption.Comparer = document.Comparer;
                    modelOption.Value = document.Value;
                    modelOption.Condition = document.Condition;
                    modelOption.Trend = document.Trend;
                    modelOption.Frequency = document.Frequency;
                }
            }
        }
    }
}