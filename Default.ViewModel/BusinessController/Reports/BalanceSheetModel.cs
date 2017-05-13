using System.Collections.Generic;
using mPower.Framework.Utils;

namespace Default.ViewModel.BusinessController.Reports
{
    public class BalanceSheetModel : BaseBusinessReportsModel
    {
        public Dictionary<int, string> Formats { get; set; }

        public DateRangeFormatEnum Format { get; set; }

        public string TotalDatesRange { get; set; }

        public List<string> Headers { get; set; }

        public string LedgerName { get; set; }

        public Matrix LiabilityMatrix { get; set; }

        public Matrix AssetsMatrix { get; set; }

        public Matrix EquityMatrix { get; set; }

        public int ColumnsCount
        {
            get { return Headers.Count; }
        }

        public int Colspan
        {
            get { return ColumnsCount + 1; }
        }

        public Matrix LiabilityEquityMatrix { get; set; }
    }
}
