using System.ComponentModel.DataAnnotations;

namespace Default.ViewModel.Areas.Finance.DebtToIncomeRatioController
{
    public class UpdateDebtIncomeModel
    {
        [Required]
        public decimal MonthlyGrossIncome { get; set; }
        [Required]
        public decimal TotalMonthlyRent { get; set; }
        [Required]
        public decimal TotalMonthlyPitia { get; set; }
        [Required]
        public decimal TotalMonthlyDebt { get; set; }
    }
}