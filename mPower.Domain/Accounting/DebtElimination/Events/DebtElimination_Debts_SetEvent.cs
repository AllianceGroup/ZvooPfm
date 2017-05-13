using System;
using System.Collections.Generic;
using Paralect.Domain;
using mPower.Domain.Accounting.DebtElimination.Data;

namespace mPower.Domain.Accounting.DebtElimination.Events
{
    public class DebtElimination_Debts_SetEvent : Event
    {
        public string DebtEliminationId { get; set; }

        [Obsolete]
        public List<string> DebtsIds { get; set; }

        public List<DebtItemData> Debts { get; set; } 
    }
}