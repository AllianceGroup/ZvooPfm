namespace Default.ViewModel.Areas.Finance.DebtToolsController
{
    public class DebtEliminationShortModel
    {
        public bool WrongProgramParams { get; set; }

        public long MonthlyBudget { get; set; }

        public double PayoffTime { get; set; }
        public double PayoffTimeViaPlan { get; set; }
        public double TimeSaved
        {
            get { return PayoffTime - PayoffTimeViaPlan; }
        }

        public long TotalPayed { get; set; }
        public long TotalPayedViaPlan { get; set; }
        public long MoneySaved
        {
            get { return TotalPayed - TotalPayedViaPlan; }
        }
    }
}