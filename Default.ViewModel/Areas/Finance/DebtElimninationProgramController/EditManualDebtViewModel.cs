using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using mPower.Domain.Accounting.Enums;

namespace Default.ViewModel.Areas.Finance.DebtElimninationProgramController
{
    public class EditManualDebtViewModel
    {
        private const double MaxAmountValue = 1000000000;

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

        [Range(0.1, 1000)]
        [Display(Name = "Interest Rate")]
        public float InterestRatePercentage { get; set; }

        [Range(0.01, MaxAmountValue)]
        [Display(Name = "Minimum Monthly Payment")]
        public decimal MinMonthPaymentInDollars { get; set; }

        public IEnumerable<SelectListItem> AccountLabels { get; set; }

        public IEnumerable<SelectListItem> Accounts { get; set; }

        public double OpeningBalance { get; set; }

        public decimal CreditLimitInDollars { get; set; }
    }
}