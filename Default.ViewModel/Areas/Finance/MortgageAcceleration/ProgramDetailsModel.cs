using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.DebtToolsController;
using mPower.Documents.Documents.Accounting.DebtElimination;

namespace Default.ViewModel.Areas.Finance.MortgageAcceleration
{
    public class ProgramDetailsModel
    {
        public ChartViewModel Chart { get; set; }

        public List<ProgramDetailsItem> Items { get; set; }

        public ProgramDetailsModel()
        {
            Items = new List<ProgramDetailsItem>();
        }
    }
}
