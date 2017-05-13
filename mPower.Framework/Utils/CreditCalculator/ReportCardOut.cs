using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace mPower.Framework.Utils.CreditCalculator
{
    public class ReportCardOut
    {
        public double OpenCreditCardUtilizationRatio { get; set; }
        public string OpenCreditCardUtilizationGrade { get; set; }
        public double PercentOfPaymentsOnTimeRatio { get; set; }
        public string PercentPaymentsOnTimeGrade { get; set; }
        public string AvgAge { get; set; }
        public string AvgAgeGrade { get; set; }
        public int TotalAccounts { get; set; }
        public string TotalAccountsGrade { get; set; }
        public int InqueriesCount { get; set; }
        public string InqueriesGrade { get; set; }
        public int OtherNegativeFactorsCount { get; set; }
        public string OtherNegativeFactorsGrade { get; set; }
    }
}
