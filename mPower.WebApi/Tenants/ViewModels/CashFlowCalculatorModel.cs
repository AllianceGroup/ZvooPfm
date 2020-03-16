using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mPower.WebApi.Tenants.ViewModels
{
    public class CashFlowCalculatorModel
    {
        public CashFlowCalculatorModel()
        {
            this.CalculatedData = new List<GridData>();
        }

        public int Year { get; set; }
        public long PresentValue { get; set; }
        public float EarningRates { get; set; }
        public long AnnualCash1 { get; set; }
        public long AnnualCash2 { get; set; }
        public long AnnualCash3 { get; set; }
        public long MiscFees { get; set; }
        public long TaxPayment { get; set; }
        public int CFApplicable1 { get; set; }
        public int CFApplicable2 { get; set; }
        public int CFApplicable3 { get; set; }
        public int MFApplicable { get; set; }
        public bool IsCF1Applied { get; set; }
        public bool IsCF2Applied { get; set; }
        public bool IsCF3Applied { get; set; }
        public bool IsMFApplied { get; set; }
        public bool IsTPApplied { get; set; }

        public List<GridData> CalculatedData { get; set; }

       
    }

    public class GridData
    {
        public int year { get; set; }
        public long BegValue { get; set; }
        public long AnnualCash1 { get; set; }
        public long AnnualCash2 { get; set; }
        public long AnnualCash3 { get; set; }
        public float EarningRate { get; set; }
        public long EndValue { get; set; }
        public long InterestEarning { get; set; }
        public long MiscFees { get; set; }
        public long TaxPayment { get; set; }
    }
    
}
