using System.Collections.Generic;
using mPower.Documents.Documents.Accounting.Ledger;

namespace Default.ViewModel.BusinessController.Reports
{
    public class TransactionDetailModel : BaseBusinessReportsModel
    {
        public string LedgerName { get; set; }

        public string ParentAccountName { get; set; }

        public string ParentAccountId { get; set; }

        public int p { get; set; }

        public List<string> AccountIds { get; set; }

        public List<TransactionDocument> Transactions { get; set; }

        public long InitialBalance { get; set; }

        public string TotalDatesFormatted { get; set; }
    }
}
