using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mPower.Framework.Utils.CreditCalculator
{
    public class ReportCardIn
    {
        public double CreditLimit { get; set; }
        public double CurrentBalance { get; set; }
        public double TotalLatePayments { get; set; }
        public double TotalPayments { get; set; }
        public int TotalNumberOfOpenCards { get; set; }
        public int TotalNumberOfDaysCardsHaveBeenOpen { get; set; }
        public int OpenAccountsCount { get; set; }
        public int ClosedAccountsCount { get; set; }
        public int RevolvingAccountsCount { get; set; }
        public int InstallmentAccountsCount { get; set; }
        public int MortgageAccountsCount { get; set; }
        public int OtherAccountsCount { get; set; }
        public int InqueriesCount { get; set; }
        public int NegativeFactorsCount { get; set; }
        public int PublicRecordsCount { get; set; }
    }
}
