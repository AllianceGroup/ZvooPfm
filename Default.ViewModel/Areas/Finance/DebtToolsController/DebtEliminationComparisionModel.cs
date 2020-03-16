using System;

namespace Default.ViewModel.Areas.Finance.DebtToolsController
{
    public class DebtEliminationComparisionModel
    {
        public long Id { get; set; }

        public string Creditor { get; set; }

        public double PayoffTime { get; set; }
        public double PayoffTimeViaPlan { get; set; }
        public double TimeSaved
        {
            get { return PayoffTime - PayoffTimeViaPlan; }
        }

        public double TotalPayed { get; set; }
        public double TotalPayedViaPlan { get; set; }
        public double MoneySaved
        {
            get { return TotalPayed - TotalPayedViaPlan; }
        }

        public string CurrentPayOffDate { get; set; }
        public string AcceleratedPayOffDate { get; set; }

        public double InterestPaid { get; set; }// Debt Elimination Changes
        public double InterestPaidViaPlan { get; set; }// Debt Elimination Changes
        public double InterestSaved => InterestPaid - InterestPaidViaPlan;// Debt Elimination Changes
        public double MinMonthPayment { get; set; }
    }
}