using mPower.Domain.Accounting.Enums;

namespace mPower.WebApi.Tenants.Model.Dashboard
{
    public class DashboardBudgetModel
    {
        public string AccountName { get; set; }

        public int Percentage { get; set; }

        public long SpentAmount { get; set; }

        public long BudgetAmount { get; set; }

        public AccountTypeEnum AccountType { get; set; }

        public string Color
        {
            get
            {
                string result = "";
                switch(AccountType)
                {
                    case AccountTypeEnum.Expense:
                    result = "progress-bar-success";
                    if(Percentage > 50 && Percentage < 100)
                        result = "progress-bar-warning";
                    else if(Percentage >= 100)
                        result = "progress-bar-danger";
                    break;
                    case AccountTypeEnum.Income:
                    result = "progress-bar-danger";
                    if(Percentage > 33 && Percentage <= 66)
                        result = "progress-bar-warning";
                    else if(Percentage > 66)
                        result = "progress-bar-success";
                    break;
                }

                return result;
            }
        }
    }
}
