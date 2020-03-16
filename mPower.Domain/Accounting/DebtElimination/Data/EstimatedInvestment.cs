using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mPower.Domain.Accounting.DebtElimination.Data
{
    public class EstimatedInvestment
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public long MonthlyDepositInDollars { get; set; }

        public double EarningsRate  { get; set; }

        public double InvestmentTimeInYears { get; set; }
        public int CompoundingInterval { get; set; }
    }
}
