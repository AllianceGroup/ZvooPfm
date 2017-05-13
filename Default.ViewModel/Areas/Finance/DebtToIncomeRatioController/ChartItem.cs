namespace Default.ViewModel.Areas.Finance.DebtToIncomeRatioController
{
    public class ChartItem
    {
        public double value { get; set; }
        public string label { get; set; }

        public ChartItem(double value, string label)
        {
            this.value = value;
            this.label = label;
        }
    }
}