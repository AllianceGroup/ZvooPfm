namespace Default.ViewModel.Areas.Finance.DebtToolsController
{
    public class DebtEliminationSavingsSummaryModel
    {
        public long Id { get; set; }

        public string Creditor { get; set; }

        public decimal CurrentTotalMonthlyPayments { get; set; }
        public decimal AcceleratedTotalMonthlyPayments { get; set; }
        public decimal CurrentDebtBalance { get; set; }
        public decimal AcceleratedDebtBalance { get; set; }
        public double CurrentInterestPaid { get; set; }
        public double AcceleratedInterestPaid { get; set; }
        public double CurrentTotalAmountPaid { get; set; }
        public double AcceleratedTotalAmountPaid { get; set; }
        public double CurrentTotalInterestSavings { get; set; }
        public double AcceleratedTotalInterestSavings { get { return CurrentInterestPaid - AcceleratedInterestPaid > 0 ? CurrentInterestPaid - AcceleratedInterestPaid: 0 ; } }
        public double CurrentRetireDebt { get; set; }
        public double AcceleratedRetireDebt { get; set; }
       
    }
}