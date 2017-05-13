using System.Collections.Generic;
using Default.ViewModel.Areas.Finance.DebtElimninationProgramController;

namespace mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram
{
    public class Step1Model
    {
        public List<DebtModel> Debts { get; set; }

        public bool OpenAddDebtPopup { get; set; }
    }
}
