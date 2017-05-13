using mPower.Domain.Accounting.Enums;

namespace mPower.WebApi.Tenants.Model.Budget
{
    public class AddBudgetModel
    {
        public string AccountName { get; set; }

        public decimal BudgetAmountInDollars { get; set; }

        public AccountTypeEnum Type { get; set; }
    }
}
