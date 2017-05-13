using System.Collections.Generic;

namespace mPower.Documents.Segments
{
    public class UserSegmentAggregateData
    {
        #region Day data

        public int Logins { get; set; }

        #endregion

        #region Month data

        public List<string> AccountsWithExceededBudget { get; set; }

        public List<string> AccountsWithoutExceededBudget { get; set; }

        public long MonthlyDebtInCents { get; set; }

        public long MonthlyIncomeInCents { get; set; }

        #endregion

        #region Global data

        public bool HasDeptEliminationProgramm { get; set; }

        public int AuthenticatedCreditIdentitiesCount { get; set; }

        public bool SetupBudget { get; set; }

        public int EmergencyFundsCount { get; set; }

        public int RetirementPlansCount { get; set; }

        public int AggregatedAccounts { get; set; }

        public bool PulledCreditScore { get; set; }

        public int Goals { get; set; }

        public int Banks { get; set; }

        public int CreditCards { get; set; }

        public int Loans { get; set; }

        public int Investments { get; set; }

        public int Zillows { get; set; }

        public bool IsActive { get; set; }


        public long AvailableCashInCents { get; set; }

        public long TotalDebtInCents { get; set; }

        public long InvestmentAccountsBalanceInCents { get; set; }

        public long CreditCardsDebtInCents { get; set; }

        public long AvailableCreditInCents { get; set; }

        public List<MortgageData> Mortgages { get; set; }

        public List<string> RemovedMortgagesIds { get; set; }

        public int UserAddedCounter { get; set; }

        #endregion

        #region Expense data

        public string Merchant { get; set; }

        public string SpendingCategory { get; set; }

        public long SpentAmountInCents { get; set; }

        public long ShopVisitsNumber { get; set; }

        #endregion


        public UserSegmentAggregateData()
        {
            AccountsWithExceededBudget = new List<string>();
            AccountsWithoutExceededBudget = new List<string>();
            Mortgages = new List<MortgageData>();
            RemovedMortgagesIds = new List<string>();
            Merchant = "";
            IsActive = false;
        }
    }
}