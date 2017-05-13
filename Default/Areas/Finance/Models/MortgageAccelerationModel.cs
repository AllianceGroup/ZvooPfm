using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.MortgageAcceleration;
using mPower.Documents.Documents.Accounting.DebtElimination;
using mPower.Domain.Accounting.Enums;

namespace Default.Areas.Finance.Models
{
    public class MortgageAccelerationModel
    {
        public MortgageProgramModel SelectedProgram { get; set; }

        public List<MortgageAccelerationProgramDocument> Programs { get; set; }

        public List<KeyValuePair<int, string>> PaymentPeriods { get; set; }

        public List<KeyValuePair<int, string>> DisplayResolutions { get; set; }
    }
}