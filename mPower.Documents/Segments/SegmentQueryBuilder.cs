using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;

namespace mPower.Documents.Segments
{
    public static class SegmentOptionsHelper
    {
        public const string ShowDeactivated = "ShowDeactivated";
        public const string ShowNetworkUsers = "ShowNetworkUsers";

        public static bool IsShowNetworkUsersEnabled(this IEnumerable<SegmentOption> options)
        {
            return options.Any(x => x.Name == ShowNetworkUsers && x.Enabled);
        }
    }

    public class SegmentQueryBuilder
    {
        #region map/reduce

        public string GetMapFunction(SegmentData segment)
        {
            return @"function Map() {
                var key = {userId: this.UserId};
                emit(key, {
                    AffiliateId: this.AffiliateId,
                    ZipCode: this.ZipCode,
                    Logins: this.AggregateData.Logins, 
			        HasDeptEliminationProgramm: this.AggregateData.HasDeptEliminationProgramm, 
			        HasMortgageAccelerationProgramm: false, 
			        HasAuthenticatedCreditIdentity: this.AggregateData.AuthenticatedCreditIdentitiesCount,
			        SetupBudget: this.AggregateData.SetupBudget,
			        HasEmergencyFund: this.AggregateData.EmergencyFundsCount, 
			        HasRetirementPlan: this.AggregateData.RetirementPlansCount, 
			        AggregatedAccounts: this.AggregateData.AggregatedAccounts, 
			        Goals: this.AggregateData.Goals, 
			        CreditCards: this.AggregateData.CreditCards, 
			        Banks: this.AggregateData.Banks, 
			        Loans: this.AggregateData.Loans, 
			        Investments: this.AggregateData.Investments, 
			        Zillows: this.AggregateData.Zillows,
                    IsActive: this.AggregateData.IsActive,
                    AvailableCashInCents: this.AggregateData.AvailableCashInCents,
                    TotalDebtInCents: this.AggregateData.TotalDebtInCents,
                    InvestmentAccountsBalanceInCents: this.AggregateData.InvestmentAccountsBalanceInCents,
                    CreditCardsDebtInCents: this.AggregateData.CreditCardsDebtInCents,
                    MonthlyDebtInCents: this.AggregateData.MonthlyDebtInCents,
                    AvailableCreditInCents: this.AggregateData.AvailableCreditInCents,
                    MonthlyIncomeInCents: this.AggregateData.MonthlyIncomeInCents,
                    AccountsWithExceededBudget: this.AggregateData.AccountsWithExceededBudget,
                    AccountsWithoutExceededBudget: this.AggregateData.AccountsWithoutExceededBudget,
                    Mortgages: this.AggregateData.Mortgages,
                    RemovedMortgagesIds: this.AggregateData.RemovedMortgagesIds,
                    Merchants: [{
			            Name: this.AggregateData.Merchant,
                        UseExcludeCondition: false,
			            SpentAmountInCents: this.AggregateData.SpentAmountInCents,
			            ShopVisitsNumber: this.AggregateData.ShopVisitsNumber,
			            FirstVisitDate: this.Date,
			            SpentAmountInCentsAverage: 0,
			            ShopVisitsNumberPerDay: 0,
                        FoundTags: []
		            }],
		            SpendingCategory: this.AggregateData.SpendingCategory,
		            SpendingCategoryCriterionSatisfied: " + (segment.SpendingCategories != null && segment.SpendingCategories.Any() ? "false" : "true") + @",
		            CategorySpentAmountInCents: this.AggregateData.SpentAmountInCents,
		            CategoryPurchasesNumber: this.AggregateData.ShopVisitsNumber,
		            CategoryFirstVisitDate: this.Date,
		            CategorySpentAmountInCentsAverage: 0,
		            CategoryPurchasesNumberPerDay: 0,
                    UserExists: this.AggregateData.UserAddedCounter,
                    MerchantsCriterionSatisfied: false
                });  
            }";
        }

        public string GetReduceFunction(SegmentData segment)
        {
            return @" function Reduce(key, values) {
                var result = {
                    AffiliateId: null,
                    ZipCode: null,
                    Logins: 0, 
			        HasDeptEliminationProgramm: false, 
			        HasMortgageAccelerationProgramm: false, 
			        HasAuthenticatedCreditIdentity: 0, 
			        SetupBudget: false, 
			        HasEmergencyFund: 0, 
			        HasRetirementPlan: 0, 
			        AggregatedAccounts: 0, 
			        Goals: 0, 
			        CreditCards: 0,
			        Banks: 0, 
			        Loans: 0, 
			        Investments: 0, 
			        Zillows: 0,
                    IsActive: false,
                    AvailableCashInCents: 0,
                    TotalDebtInCents: 0,
                    InvestmentAccountsBalanceInCents: 0,
                    CreditCardsDebtInCents: 0,
                    MonthlyDebtInCents: 0,
                    AvailableCreditInCents: 0,
                    MonthlyIncomeInCents: 0,
                    AccountsWithExceededBudget: [],
                    AccountsWithoutExceededBudget: [],
                    Mortgages: [],
                    RemovedMortgagesIds: [],
                    Merchants: [" + MerchantsToJsonString(segment) + @"],
                    SpendingCategory: '" + string.Join("|", segment.SpendingCategories ?? new List<string>()) + @"',
		            SpendingCategoryCriterionSatisfied: " + (segment.SpendingCategories != null && segment.SpendingCategories.Any() ? "false" : "true") + @",
                    CategorySpentAmountInCents: 0,
                    CategoryPurchasesNumber: 0,
                    CategoryFirstVisitDate: new Date(),
                    CategorySpentAmountInCentsAverage: 0,
                    CategoryPurchasesNumberPerDay: 0,
                    UserExists: 0,
                    MerchantsCriterionSatisfied: false
                };
      
                values.forEach(function(val) {
                    if (val.AffiliateId > '') {
                        result.AffiliateId = val.AffiliateId;
                    }
                    if (val.ZipCode > '') {
                        result.ZipCode = val.ZipCode;
                    }
  	  	            result.Logins += val.Logins;
                    if(val.HasDeptEliminationProgramm)
		            {
    		            result.HasDeptEliminationProgramm = true;
		            }
                    result.HasAuthenticatedCreditIdentity += val.HasAuthenticatedCreditIdentity;
                    if(val.SetupBudget)
		            {
    		            result.SetupBudget = true;
		            }
                    if(val.IsActive)
                    {
                        result.IsActive = true;
                    }

                    result.HasEmergencyFund += val.HasEmergencyFund;
                    result.HasRetirementPlan += val.HasRetirementPlan;
		            result.AggregatedAccounts += val.AggregatedAccounts;
		            result.Goals += val.Goals;
		            result.CreditCards += val.CreditCards;
		            result.Banks += val.Banks;
		            result.Loans += val.Loans;
		            result.Investments += val.Investments;
		            result.Zillows += val.Zillows;
		            result.AvailableCashInCents += val.AvailableCashInCents;
		            result.TotalDebtInCents += val.TotalDebtInCents;
		            result.InvestmentAccountsBalanceInCents += val.InvestmentAccountsBalanceInCents;
		            result.CreditCardsDebtInCents += val.CreditCardsDebtInCents;
		            result.MonthlyDebtInCents += val.MonthlyDebtInCents;
		            result.AvailableCreditInCents += val.AvailableCreditInCents;
		            result.MonthlyIncomeInCents += val.MonthlyIncomeInCents;
		            result.AccountsWithExceededBudget = result.AccountsWithExceededBudget.concat(val.AccountsWithExceededBudget);
		            result.AccountsWithoutExceededBudget = result.AccountsWithoutExceededBudget.concat(val.AccountsWithoutExceededBudget);
		            result.Mortgages = result.Mortgages.concat(val.Mortgages);
		            result.RemovedMortgagesIds = result.RemovedMortgagesIds.concat(val.RemovedMortgagesIds);
                    result.UserExists += val.UserExists;

                    // merchant statistic
                    var needToFilterMerchants = result.Merchants.length > 1 || result.Merchants[0].Name > '';
                    if (!needToFilterMerchants) {
                        result.Merchants[0].SpentAmountInCents += val.Merchants[0].SpentAmountInCents;
                        result.Merchants[0].ShopVisitsNumber += val.Merchants[0].ShopVisitsNumber;
                        if(val.Merchants[0].FirstVisitDate.getTime() < result.Merchants[0].FirstVisitDate.getTime())
                        {
                            result.Merchants[0].FirstVisitDate = val.Merchants[0].FirstVisitDate;
                        }
                    } else {
                        for(var i = 0; i < val.Merchants.length; i++) {

                            if (val.Merchants[i].ShopVisitsNumber > 0) {
                                var isResultOfPreviousReduce = val.Merchants[i].Name.indexOf('|') != -1;
                                for(var j = 0; j < result.Merchants.length; j++) {
                                    var merchantBelongToThisGroup = false;
                                    if (isResultOfPreviousReduce) {
                                        merchantBelongToThisGroup = val.Merchants[i].Name == result.Merchants[j].Name;
                                    } else {
                                        var merchantPatterns = result.Merchants[j].Name.split('|');
                                        for(var k = 0; k < merchantPatterns.length; k++) {
                                            merchantBelongToThisGroup |= val.Merchants[i].Name.indexOf(merchantPatterns[k]) != -1;
                                        }
                                    }
                            
                                    if (merchantBelongToThisGroup) {
                                        if (val.Merchants[i].UseExcludeCondition) {
                                            result.Merchants[j].UseExcludeCondition = true;
                                        }
                                        result.Merchants[j].SpentAmountInCents += val.Merchants[i].SpentAmountInCents;
                                        result.Merchants[j].ShopVisitsNumber += val.Merchants[i].ShopVisitsNumber;
                                        if(val.Merchants[i].FirstVisitDate.getTime() < result.Merchants[j].FirstVisitDate.getTime())
                                        {
                                            result.Merchants[j].FirstVisitDate = val.Merchants[i].FirstVisitDate;
                                        }
                                        if (isResultOfPreviousReduce) {
                                            result.Merchants[j].FoundTags = result.Merchants[j].FoundTags.concat(val.Merchants[i].FoundTags);
                                        } else if (result.Merchants[j].FoundTags.indexOf(val.Merchants[i].Name) == -1)
                                        {
                                            result.Merchants[j].FoundTags.push(val.Merchants[i].Name);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // category spending statistic
                    if (val.CategoryPurchasesNumber > 0) {
                        var needToFilterCategories = result.SpendingCategory > '';
                        var isResultOfPreviousReduce = val.SpendingCategory.indexOf('|') != -1;
                        var isMatchedCategory = false;
                        if (needToFilterCategories && !isResultOfPreviousReduce) {
                            var categories = result.SpendingCategory.split('|');
                            for(var i = 0; i < categories.length && !isMatchedCategory; i++) {
                                if (categories[i].toLowerCase() == val.SpendingCategory.toLowerCase()) {
                                    isMatchedCategory = true;
                                }
                            }
                        }
                        
                        if (!needToFilterCategories || isResultOfPreviousReduce || isMatchedCategory) {
                            result.SpendingCategoryCriterionSatisfied = true;
                            result.CategorySpentAmountInCents += val.CategorySpentAmountInCents;
                            result.CategoryPurchasesNumber += val.CategoryPurchasesNumber;
                            if(val.CategoryFirstVisitDate.getTime() < result.CategoryFirstVisitDate.getTime())
                            {
                                result.CategoryFirstVisitDate = val.CategoryFirstVisitDate;
                            }
                        }
                    }
                });

                return result;
            }";
        }

        public readonly string Finalize =
            @"function Finalize(key, value)
            {
                value.HasAuthenticatedCreditIdentity = value.HasAuthenticatedCreditIdentity > 0;
                value.HasEmergencyFund = value.HasEmergencyFund > 0;
                value.HasRetirementPlan = value.HasRetirementPlan > 0;
                value.UserExists = value.UserExists > 0;
                var anyMerchantMatched = false;
                var excludeConditionSatisfied = true;
                for (var i = 0; i < value.Merchants.length && !anyMerchantMatched; i++) {
                    if (!value.Merchants[i].UseExcludeCondition) {
                        var merchantPatterns = value.Merchants[i].Name.split('|');
                        anyMerchantMatched = true;
                        for (var k = 0; k < merchantPatterns.length && anyMerchantMatched; k++) {
                            if(value.Merchants[i].FoundTags.join('|').indexOf(merchantPatterns[k]) == -1) {
                                anyMerchantMatched = false;
                            }
                        }
                    }
                }
                if (anyMerchantMatched) {
                    for (var i = 0; i < value.Merchants.length && excludeConditionSatisfied; i++) {
                        if (value.Merchants[i].UseExcludeCondition) {
                            var merchantPatterns = value.Merchants[i].Name.split('|');
                            for (var k = 0; k < merchantPatterns.length && excludeConditionSatisfied; k++) {
                                if(value.Merchants[i].FoundTags.join('|').indexOf(merchantPatterns[k]) != -1) {
                                    excludeConditionSatisfied = false;
                                }
                            }
                        }
                    }
                }
                value.MerchantsCriterionSatisfied = value.Merchants.length == 0 || (anyMerchantMatched && excludeConditionSatisfied);

                // mortgages
                var existingMortgages = [];
                for (var i = 0; i < value.Mortgages.length; i++){
                    var wasRemoved = false;
                    for (var j = 0; j < value.RemovedMortgagesIds.length; j++) {
                        if (value.Mortgages[i].Id == value.RemovedMortgagesIds[j]) {
                            wasRemoved = true;
                            break;
                        }
                    }
                    if (!wasRemoved) {
                        existingMortgages.push(value.Mortgages[i])
                    }
                }
                value.Mortgages = existingMortgages;
                
                // find out whether user Has Mortgage Acceleration Programm
                var hasMortgage = false;
	            for (var i=0; i<value.Mortgages.length; i++) {
		            if (value.Mortgages[i].Id.indexOf('acc') == -1) { // 1) when Mortgages.Id looks like 'acc_XXX', then it is connected with Ledger (XXX - account Id from Ledger accounts) 
                        hasMortgage = true;                           //    but it does NOT mean that user has Mortgage
			            break;                                        // 2) when Mortgages.Id looks like 'YYY' (YYY - Id of Mortgage Program from 'dept_eliminations' collection)     
			        }                                                 //    then it DOES mean user has Mortgage
	            }

                value.HasMortgageAccelerationProgramm = hasMortgage;
                // merchants average values
                for(var i = 0; i < value.Merchants.length; i++) {
                    value.Merchants[i].SpentAmountInCentsAverage = value.Merchants[i].SpentAmountInCents / Math.max(value.Merchants[i].ShopVisitsNumber, 1);
                    value.Merchants[i].ShopVisitsNumberPerDay = value.Merchants[i].ShopVisitsNumber / Math.max(Math.round(((new Date()).getTime() - value.Merchants[i].FirstVisitDate.getTime()) / (1000 * 60 * 60 * 24)), 1);
                }

                // category spending average values
                value.CategorySpentAmountInCentsAverage = value.CategorySpentAmountInCents / Math.max(value.CategoryPurchasesNumber, 1);
                value.CategoryPurchasesNumberPerDay = value.CategoryPurchasesNumber / Math.max(Math.round(((new Date()).getTime() - value.CategoryFirstVisitDate.getTime()) / (1000 * 60 * 60 * 24)), 1);

                return value;
            }";

        private static string MerchantsToJsonString(SegmentData segment)
        {
            const string merchantJsonFormat =
            @"{{
			    Name: '{0}',
                UseExcludeCondition: {1},
			    SpentAmountInCents: 0,
			    ShopVisitsNumber: 0,
			    FirstVisitDate: new Date(),
			    SpentAmountInCentsAverage: 0,
			    ShopVisitsNumberPerDay: 0,
                FoundTags: [{2}]
		    }}";

            if (segment.MerchantSelections == null || !segment.MerchantSelections.Any())
            {
                // at least one merchant required to apply merchants' filters
                return string.Format(merchantJsonFormat, "", "false", "''");
            }

            var merchantGroups = new List<MerchantGroup>();
            foreach (var merchant in segment.MerchantSelections.OrderBy(x => x.Index))
            {
                var name = merchant.MerchantName.ToLowerInvariant();
                if (!merchantGroups.Any())
                {
                    merchantGroups.Add(new MerchantGroup {Name = name});
                }
                else
                {
                    switch (merchant.Operation)
                    {
                        case LogicalOperationEnum.And:
                            // business rule: merchants group for exclusion should always contain only one merchant
                            // it should also be provided at UI: NOT operation can be used only once and only for last merchant
                            if (merchantGroups[merchantGroups.Count - 1].UseExcludeCondition)
                            {
                                merchantGroups.Add(new MerchantGroup {Name = name});
                            }
                            else
                            {
                                merchantGroups[merchantGroups.Count - 1].Name += ("|" + name);
                            }
                            break;
                        case LogicalOperationEnum.Or:
                            merchantGroups.Add(new MerchantGroup {Name = name});
                            break;
                        case LogicalOperationEnum.Not:
                            merchantGroups.Add(new MerchantGroup {Name = name, UseExcludeCondition   = true});
                            break;

                        default:
                            throw new NotSupportedException();
                    }
                }
            }

            return string.Join(",", merchantGroups.Select(x => string.Format(merchantJsonFormat, x.Name, x.UseExcludeCondition ? "true" : "false", "")));
        }

        #endregion

        public string FormatSegmentDate(UserSegmentTypeEnum segmentType, DateTime date)
        {
            string format;
            switch (segmentType)
            {
                case UserSegmentTypeEnum.Month:
                    format = "MM-yyyy";
                    break;

                case UserSegmentTypeEnum.Global:
                case UserSegmentTypeEnum.Day:
                case UserSegmentTypeEnum.Expense:
                    format = "dd-MM-yyyy";
                    break;

                default:
                    throw new NotSupportedException();
            }

            return date.ToUniversalTime().ToString(format);
        }

        public IMongoQuery GetSegmentQuery(UserSegmentTypeEnum segmentType, DateTime date, string userId, string merchant, string spendingCategory)
        {
            var searchQuery = Query.And(
                Query.EQ("UserSegmentType", (int)segmentType),
                Query.EQ("UserId", userId),
                Query.EQ("FormattedDate", FormatSegmentDate(segmentType, date)));

            if (segmentType == UserSegmentTypeEnum.Expense)
            {
                if (!string.IsNullOrEmpty(merchant))
                {
                    searchQuery = Query.And(searchQuery, Query.EQ("AggregateData.Merchant", merchant.ToLowerInvariant()));
                }
                if (!string.IsNullOrEmpty(spendingCategory))
                {
                    searchQuery = Query.And(searchQuery, Query.EQ("AggregateData.SpendingCategory", spendingCategory.ToLowerInvariant()));
                }
            }

            return searchQuery;
        }

        public IMongoQuery BuildInitialFilterQuery(SegmentData segment, DateTime? onDate = null)
        {
            #region filter for periods

            var filterForGlobalSegments = Query.EQ("UserSegmentType", (int)UserSegmentTypeEnum.Global);
            var filterForMonthSegments = Query.And(Query.EQ("UserSegmentType", (int)UserSegmentTypeEnum.Month), Query.EQ("FormattedDate", FormatSegmentDate(UserSegmentTypeEnum.Month, onDate ?? DateTime.Now)));

            DateTime? from = null;
            DateTime? to = null;

            if (segment.DateRange != null)
            {
                switch (segment.DateRange.Value)
                {
                    case DateRangeEnum.Last30Days:
                        from = DateTime.Now.AddDays(-30);
                        to = DateTime.Now;
                        break;
                    case DateRangeEnum.Last60Days:
                        from = DateTime.Now.AddDays(-60);
                        to = DateTime.Now;
                        break;
                    case DateRangeEnum.Last90Days:
                        from = DateTime.Now.AddDays(-90);
                        to = DateTime.Now;
                        break;
                    case DateRangeEnum.Custom:
                        from = segment.CustomDateRangeStart;
                        to = segment.CustomDateRangeEnd;
                        break;
                }
            }
            var filterForDaySegments = Query.EQ("UserSegmentType", (int)UserSegmentTypeEnum.Day);
            if (from != null)
            {
                filterForDaySegments = Query.And(filterForDaySegments, Query.GTE("Date", from.Value.ToLocalTime().ToUniversalTime()));
            }
            if (to != null)
            {
                filterForDaySegments = Query.And(filterForDaySegments, Query.LTE("Date", to.Value.ToLocalTime().ToUniversalTime()));
            }

            var filterForExpenseSegments = Query.EQ("UserSegmentType", (int)UserSegmentTypeEnum.Expense);

            var filterForPeriods = Query.Or(filterForGlobalSegments, filterForMonthSegments, filterForDaySegments, filterForExpenseSegments);

            if (onDate.HasValue)
            {
                filterForPeriods = Query.And(filterForPeriods, Query.LT("Date", onDate.Value.Date.AddDays(1)));
            }

            #endregion

            if (!segment.Options.IsShowNetworkUsersEnabled())
            {
                var filterForAffiliate = Query.EQ("AffiliateId", segment.AffiliateId);
                return Query.And(filterForAffiliate, filterForPeriods);
            }

            return filterForPeriods;
        }

        public IMongoQuery BuildSegmentSearchQuery(SegmentData segment)
        {
            var queries = new List<IMongoQuery>
            {
                Query.EQ("value.UserExists", true),
                Query.EQ("value.MerchantsCriterionSatisfied", true),
                Query.EQ("value.SpendingCategoryCriterionSatisfied", true)
            };
            var merchantQueries = new List<IMongoQuery>();
            var showDeactivated = false;

            foreach (var opt in segment.Options.Where(x => x.Enabled && x.Name != SegmentOptionsHelper.ShowNetworkUsers))
            {
                if (opt.Name == SegmentOptionsHelper.ShowDeactivated)
                {
                    showDeactivated = true;
                    continue;
                }

                var isMerchantOpt = opt.Name.StartsWith("Merchants.");
                var name = isMerchantOpt ? opt.Name.Substring("Merchants.".Length) : "value." + opt.Name;


                var query = BuildOptionSearchQuery(opt, name);

                if (query != null)
                {
                    if (isMerchantOpt)
                    {
                        merchantQueries.Add(query);
                    }
                    else
                    {
                        queries.Add(query);
                    }
                }
            }

            if (!showDeactivated)
            {
                queries.Add(Query.EQ("value.IsActive", true));
            }

            if (merchantQueries.Any())
            {
                queries.Add(Query.ElemMatch("value.Merchants", Query.And(merchantQueries.ToArray())));
            }

            if (segment.ZipCodes != null && segment.ZipCodes.Any())
            {
                queries.Add(Query.In("value.ZipCode", new BsonArray(segment.ZipCodes)));
            }


            return Query.And(queries.ToArray());
        }

        public IMongoQuery BuildOptionSearchQuery(SegmentOption opt, string name)
        {
            IMongoQuery query = null;

            switch (opt.Type)
            {
                case OptionTypeEnum.Flag:
                    query = Query.EQ(name, true);
                    break;

                case OptionTypeEnum.Trend:
                    query = GetTrendOptionQuery(opt, name);
                    break;

                case OptionTypeEnum.Frequency:
                    query = GetFrequencyOptionQuery(opt, name);
                    break;

                case OptionTypeEnum.Default:
                case OptionTypeEnum.Full:
                    query = GetDefaultOptionQuery(opt, name);
                    break;

                case OptionTypeEnum.Custom:
                    query = Query.EQ(name, opt.Value);
                    break;
            }

            return query;
        }

        private static IMongoQuery GetTrendOptionQuery(SegmentOption opt, string name)
        {
            switch (opt.Trend)
            {
                case TrendEnum.Increasing:
                    return Query.GT(name, 0);
                case TrendEnum.Decreasing:
                    return Query.LT(name, 0);
            }

            return null;
        }

        private static IMongoQuery GetFrequencyOptionQuery(SegmentOption opt, string name)
        {
            int intValue;
            int.TryParse(opt.Value, out intValue);
            if (opt.Multiplier != 0)
            {
                intValue *= opt.Multiplier;
            }

            name += "PerDay";

            var doubleValue = (double)intValue;
            switch (opt.Frequency)
            {
                case FrequencyEnum.Week:
                    doubleValue /= 7;
                    break;
                case FrequencyEnum.Month:
                    doubleValue /= (365 / 12.0);
                    break;
                case FrequencyEnum.Year:
                    doubleValue /= 365;
                    break;
            }

            switch (opt.Comparer)
            {
                case ComparerEnum.Equal:
                    return Query.EQ(name, doubleValue);
                case ComparerEnum.Greater:
                    return Query.GT(name, doubleValue);
                case ComparerEnum.Less:
                    return Query.LT(name, doubleValue);
            }

            return null;
        }

        private static IMongoQuery GetDefaultOptionQuery(SegmentOption opt, string name)
        {
            decimal decValue;
            decimal.TryParse(opt.Value, out decValue);
            if (opt.Multiplier != 0)
            {
                decValue *= opt.Multiplier;
            }

            if (opt.Type == OptionTypeEnum.Full && opt.Condition == ConditionEnum.Average)
            {
                name += "Average";
            }

            var intValue = (int)Math.Truncate(decValue);
            switch (opt.Comparer)
            {
                case ComparerEnum.Equal:
                    return Query.EQ(name, intValue);
                case ComparerEnum.Greater:
                    return Query.GT(name, intValue);
                case ComparerEnum.Less:
                    return Query.LT(name, intValue);
            }

            return null;
        }
    }
}