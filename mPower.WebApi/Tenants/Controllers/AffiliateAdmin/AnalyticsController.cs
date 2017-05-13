using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Default.Areas.Administration.Models;
using Default.Factories.ViewModels;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using mPower.Documents.Documents.Affiliate;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.ViewModels.AffiliateAdmin;
using Microsoft.AspNetCore.Mvc;
using mPower.WebApi.Authorization;
using mPower.Domain.Membership.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.WebApi.Tenants.ViewModels;

namespace mPower.WebApi.Tenants.Controllers.AffiliateAdmin
{
    [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
    [Route("api/[controller]")]
    public class AnalyticsController : BaseController
    {
        private readonly AffiliateAnalyticsDocumentService _affiliateAnalyticsDocumentService;
        private readonly IObjectRepository _objectRepository;
        private readonly AffiliateDocumentService _affiliateDocumentService;

        public AnalyticsController(AffiliateAnalyticsDocumentService affiliateAnalyticsDocumentService, 
            IObjectRepository objectRepository, AffiliateDocumentService affiliateDocumentService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _affiliateAnalyticsDocumentService = affiliateAnalyticsDocumentService;
            _objectRepository = objectRepository;
            _affiliateDocumentService = affiliateDocumentService;
        }

        #region Analytics Dashboard
    
        [HttpGet]
        public AnalyticsDashboardViewModel Get(Statistic statistic)
        {
            var model = new AnalyticsDashboardViewModel { Statistic = statistic, Chart = new List<ChartItem>() };

            var now = DateTime.Now;
            //Fix: need statistic just for current affiliate
            var stats = _affiliateAnalyticsDocumentService.GetById(Tenant.ApplicationId);

            foreach (var x in Enumerable.Range(-5, 6))
            {
                var currMonth = now.AddMonths(x);
                var spent = stats.BalanceAdjustmentStatisctics.GetSpentForMonths(new MonthYear(currMonth.Month, currMonth.Year));
                model.Chart.Add(new ChartItem((spent / 100), currMonth.ToString("MM/yyyy", CultureInfo.InvariantCulture)));
            }
            model.TotalMoneyManaged = _objectRepository.Load<SpentStatiscticData, AnalyticsModel>(stats.BalanceAdjustmentStatisctics);
            model.AvailableCash = _objectRepository.Load<AnalyticsViewModelDto, AnalyticsModel>(new AnalyticsViewModelDto(stats.AvailableCashStatisctics, model.Statistic.AvailableCashType));
            model.AvailableCredit = _objectRepository.Load<SpentStatiscticData, AnalyticsModel>(stats.AvailableCreditStatisctics);
            model.TotalUserDebt = _objectRepository.Load<AnalyticsViewModelDto, AnalyticsModel>(new AnalyticsViewModelDto(stats.UserDebtStatisctics, model.Statistic.TotalDebtType));
            model.AvgUserAnnualIncome = _objectRepository.Load<AnalyticsViewModelDto, AnalyticsModel>(new AnalyticsViewModelDto(stats.UserIncomeStatisctics, StatisticTypeEnum.AverageAnnual));

            return model;
        }

        #endregion

        #region Reports
        [HttpGet("getReports")]
        public SegmentsListModel GetReports()
        {
            var affiliateId = Tenant.ApplicationId;
            var affiliate = _affiliateDocumentService.GetById(affiliateId);

            var segments = affiliate.Segments.Select(Load).OrderBy(x => x.Name).ToList();

            var model = new SegmentsListModel { Segments = segments };
            return model;
        }

        private SegmentsListItemModel Load(SegmentDocument document)
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
                BasicOptions = new List<string>(),
            };

            if(document.MerchantSelections != null && document.MerchantSelections.Any())
                model.BasicOptions.Add($"Shops at: {string.Join(",", document.MerchantSelections.Select(x => x.MerchantName))}");

            if(document.Gender.HasValue)
                model.BasicOptions.Add($"Gender is {document.Gender}");

            if(document.AgeRangeFrom.HasValue || document.AgeRangeTo.HasValue)
            {
                var from = document.AgeRangeFrom;
                var to = document.AgeRangeTo;
                model.BasicOptions.Add("Age range" + (from.HasValue ? $" from {@from}" : "") + (to.HasValue ? $" to {to}" : ""));
            }
            if(document.DateRange.HasValue)
            {
                if(document.DateRange.Value == DateRangeEnum.Custom && (document.CustomDateRangeStart.HasValue || document.CustomDateRangeEnd.HasValue))
                {
                    var from = document.CustomDateRangeStart;
                    var to = document.CustomDateRangeEnd;
                    model.BasicOptions.Add("Date range" + (from.HasValue ? $" from {@from:M/d/yyyy}" : "") + (to.HasValue ? $" to {to:M/d/yyyy}" : ""));
                }
                else
                    model.BasicOptions.Add($"Date range is {document.DateRange.GetDescription()}");
            }
            if(!string.IsNullOrEmpty(document.State))
                model.BasicOptions.Add($"State is {document.State}");

            if(document.SpendingCategories != null && document.SpendingCategories.Any())
                model.BasicOptions.Add($"Spending categories: {string.Join(",", document.SpendingCategories)}");

            if(document.Options != null)
                ApplyOptions(document.Options, model.AllOptions);

            return model;
        }

        private static void ApplyOptions(IEnumerable<SegmentOption> @from, List<SegmentOptionModel> to)
        {
            foreach(var document in @from.Where(x => x.Enabled))
            {
                var modelOption = to.Find(x => x.Name == document.Name);
                if(modelOption != null)
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
        #endregion
    }
}
