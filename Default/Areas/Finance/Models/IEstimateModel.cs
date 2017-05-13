using mPower.Domain.Accounting.Enums;

namespace Default.Areas.Finance.Models
{
    public interface IEstimateModel
    {
        decimal EstimatedValue { get; }

        string Title { get; }

        GoalTypeEnum GoalType { get; }
    }
}