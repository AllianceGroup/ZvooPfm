namespace mPower.WebApi.Tenants.Model.Budget
{
    public class BudgetUpdateModel
    {
        public string BudgetId { get; set; }

        public long Amount { get; set; }

        public int Year { get; set; }

        public int Month { get; set; }
    }
}
