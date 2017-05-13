using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.DebtElimination.Data;

namespace mPower.Domain.Accounting.DebtElimination.Commands
{
    public class DebtElimination_Debts_SetCommand : Command
    {
        public string DebtEliminationId { get; set; }

        public List<DebtItemData> Debts { get; set; }
    }
}