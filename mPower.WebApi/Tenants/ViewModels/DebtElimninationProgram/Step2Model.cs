using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.ViewModels.DebtElimninationProgram
{
    public class Step2Model
    {
        public decimal MonthlyBudget { get; set; }

        public long RecommendedBudgetInCents { get; set; }

        public string Plan { get; set; }

        public List<SelectListItem> Plans { get; set; }
    }
}
