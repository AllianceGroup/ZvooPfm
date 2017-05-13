using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Domain.Accounting.DebtElimination.Data;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Accounting.DebtElimination
{
    public class DebtEliminationDocument
    {
        [BsonId]
        public string Id { get; set; }

        public string UserId { get; set; }

        public string LedgerId { get; set; }

        public DebtEliminationPlanEnum PlanId { get; set; }

        public long MonthlyBudgetInCents { get; set; }

        public DebtEliminationDisplayModeEnum DisplayMode { get; set; }

        public bool AddedToCalendar { get; set; }

        public List<MortgageAccelerationProgramDocument> MortgagePrograms { get; set; }

        public string CurrentMortgageProgramId { get; set; }

        public List<DebtItemData> Debts { get; set; }

        public DebtEliminationDocument()
        {
            DisplayMode = DebtEliminationDisplayModeEnum.ProgramAndMinimums;
            MortgagePrograms = new List<MortgageAccelerationProgramDocument>();
            DebtToIncomeRatio = new DebtToIncomeRatioDocument();
            Debts = new List<DebtItemData>();
        }

        public DebtToIncomeRatioDocument DebtToIncomeRatio { get; set; }

        public bool IsDebtToIncomeCalculatedBefore
        {
            get { return DebtToIncomeRatio.DebtToIncomeRatio > 0; }
        }
    }
}
