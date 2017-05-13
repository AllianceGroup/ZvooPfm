using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Business.BusinessController
{
    public class EditManualAccountViewModel
    {
        [Required]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public AccountTypeEnum Type { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public AccountLabelEnum Label { get; set; }

        public string Number { get; set; }

        public string ParentAccountId { get; set; }

        public string InstitutionName { get; set; }

        public float InterestRatePercentage { get; set; }

        public decimal MinMonthPaymentInDollars { get; set; }

        public IDictionary<string, string> AccountLabels { get; set; }

        public IDictionary<string, string> Accounts { get; set; }

        public double OpeningBalance { get; set; }

        public decimal CreditLimitInDollars { get; set; }
    }
}