using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using mPower.Documents.Segments;
using mPower.Domain.Application.Affiliate.Data;
using mPower.Domain.Application.Enums;

namespace Default.Areas.Administration.Models
{
    public class SegmentModel
    {
        public const int DynamicItemsLimit = 5;

        public string Id { get; set; }
        public string AffiliateId { get; set; }

        [Required]
        public string Name { get; set; }
        public int? Reach { get; set; }
        public string ReachFormatted { get; set; }
        public bool IsNew { get; set; }

        public List<SegmentOptionModel> ApplicationOptions { get; set; }
        
        public GenderEnum? Gender { get; set; }
        public int? AgeRangeFrom { get; set; }
        public int? AgeRangeTo { get; set; }
        public string State { get; set; }
        public List<string> ZipCodes { get; set; }
        
        public DateRangeEnum? DateRange { get; set; }
        public DateTime? CustomDateRangeStart { get; set; }
        public DateTime? CustomDateRangeEnd { get; set; }
        
        public List<SegmentOptionModel> FinancesOptions { get; set; }
        
        public List<MerchantSelectItem> MerchantSelections { get; set; }
        public List<SegmentOptionModel> MerchantOptions { get; set; }
        
        public List<string> SpendingCategories { get; set; }
        public List<SegmentOptionModel> SpendingCategoryOptions { get; set; }

        public List<SegmentOptionModel> AllOptions
        {
            get
            {
                return ApplicationOptions.Union(FinancesOptions).Union(MerchantOptions).Union(SpendingCategoryOptions).ToList();
            }
        }


        public SegmentModel()
        {
            ApplicationOptions = new List<SegmentOptionModel>
            {
                CreateOption("Number of logins", "Logins"),
                CreateOption("Number of Aggregated Accounts", "AggregatedAccounts"),
                CreateOption("Number of Bank Accounts", "Banks"),
                CreateOption("Number of Credit Card Accounts", "CreditCards"),
                CreateOption("Number of Loan Accounts", "Loans"),
                CreateOption("Number of goals set up", "Goals"),
                CreateOption("Number of Investment Accounts", "Investments"),
                CreateOption("Number of Real Estate Properties", "Zillows"),
                CreateOption("Have set up debt elimination program", "HasDeptEliminationProgramm", OptionTypeEnum.Flag),
                CreateOption("Have set up mortgage acceleration program", "HasMortgageAccelerationProgramm", OptionTypeEnum.Flag),
                CreateOption("Have authenticated a credit identity", "HasAuthenticatedCreditIdentity", OptionTypeEnum.Flag),
                CreateOption("Have an emergency fund", "HasEmergencyFund", OptionTypeEnum.Flag),
                CreateOption("Have an retirement plan", "HasRetirementPlan", OptionTypeEnum.Flag),
                CreateOption("Include deactivated users", SegmentOptionsHelper.ShowDeactivated, OptionTypeEnum.Flag),
                CreateOption("Include all network users", SegmentOptionsHelper.ShowNetworkUsers, OptionTypeEnum.Flag),
            };

            FinancesOptions = new List<SegmentOptionModel>
            {
                CreateOption("Available cash", "AvailableCashInCents"),
                CreateOption("Amount of debt", "TotalDebtInCents"),
                CreateOption("Amount in investment accounts", "InvestmentAccountsBalanceInCents"),
                CreateOption("Amount of credit card debt", "CreditCardsDebtInCents"),
                CreateOption("Debt trend", "MonthlyDebtInCents", OptionTypeEnum.Trend),
                CreateOption("Has setup a budget", "SetupBudget", OptionTypeEnum.Flag),
                CreateOption("Over budget in a category", "AccountsWithExceededBudget", OptionTypeEnum.Custom),
                CreateOption("Under budget in a category", "AccountsWithoutExceededBudget", OptionTypeEnum.Custom),
                CreateOption("Available credit", "AvailableCreditInCents"),
                CreateOption("Mortgage amount", "Mortgages.AmountInCents"),
                CreateOption("Mortgage interest rate (% per year)", "Mortgages.InterestRate"),
                CreateOption("Monthly income", "MonthlyIncomeInCents"),
            };

            MerchantSelections = new List<MerchantSelectItem> {new MerchantSelectItem()};
            MerchantOptions = new List<SegmentOptionModel>
            {
                CreateOption("Number of purchases", "Merchants.ShopVisitsNumber", OptionTypeEnum.Frequency),
                CreateOption("Dollar volume", "Merchants.SpentAmountInCents", OptionTypeEnum.Full),
            };

            SpendingCategories = new List<string> {""};
            SpendingCategoryOptions = new List<SegmentOptionModel>
            {
                CreateOption("Number of purchases", "CategoryPurchasesNumber", OptionTypeEnum.Frequency),
                CreateOption("Dollar volume", "CategorySpentAmountInCents", OptionTypeEnum.Full),
            };

            ZipCodes = new List<string> {""};
        }

        private static SegmentOptionModel CreateOption(string title, string name, OptionTypeEnum type = OptionTypeEnum.Default, int? multiplier = null)
        {
            return new SegmentOptionModel
            {
                Title = title,
                Name = name,
                Type = type,
                Multiplier = multiplier.HasValue 
                    ? multiplier.Value 
                    : name.EndsWith("InCents") ? 100 : 1,
            };
        }
    }
}