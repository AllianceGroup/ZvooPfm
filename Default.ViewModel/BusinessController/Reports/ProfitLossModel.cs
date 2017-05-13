using System.Collections.Generic;
using mPower.Framework.Utils;

namespace Default.ViewModel.BusinessController.Reports
{
    public class ProfitLossModel : BaseBusinessReportsModel
    {
        public Dictionary<int, string> Formats { get; set; }

        public DateRangeFormatEnum Format { get; set; }

        public string LedgerName { get; set; }

        public Matrix IncomeMatrix { get; set; }

        public Matrix ExpenseMatrix { get; set; }

        public MatrixRow NetIncome { get; set; }

        public string TotalDatesRange { get; set; }

        public List<string> Headers { get; set; }

        public int ColumnsCount
        {
            get { return Headers.Count; }
        }

        public int Colspan
        {
            get { return ColumnsCount + 1; }
        }
    }
}
