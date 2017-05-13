using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework.Utils.Extensions;

namespace Default.ViewModel.Areas.Business.BusinessController
{
    public class AddManualAccountViewModel
    {
        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        public AccountTypeEnum Type { get; set; }

        [Required]
        [Display(Name = "Account Type")]
        public AccountLabelEnum Label { get; set; }

        public string Number { get; set; }

        public string ParentAccountId { get; set; }

        [Range(0.1, 1000)]
        [Display(Name = "Interest Rate")]
        public float? InterestRatePercentage { get; set; }

        public decimal MinMonthPaymentInDollars { get; set; }

        public IDictionary<string, string> AccountLabels => AccountsService.PersonalAccountLabelsList
                    .ToDictionary(x => x.ToString(), x => x.GetDescription());

        public IDictionary<string, string> Accounts => new Dictionary<string, string>();


        public decimal OpeningBalance { get; set; }

        public decimal CreditLimitInDollars { get; set; }
    }
}