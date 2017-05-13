using System;
using MongoDB.Bson.Serialization.Attributes;
using mPower.Documents.Enums;
using mPower.Domain.Accounting.Enums;

namespace mPower.Documents.Documents.Goal
{
    public class GoalDocument
    {
        [BsonId]
        public string GoalId { get; set; }

        public GoalTypeEnum Type { get; set; }

        public long TotalAmountInCents { get; set; }

        public long MonthlyPlanAmountInCents { get; set; }

        public long CurrentAmountInCents { get; set; }

        public long LatestMonthAdjustmentInCents { get; set; }

        public long StartingBalanceInCents { get; set; }

        public string Title { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime PlannedDate { get; set; }

        public GoalStatusEnum Status { get; set; }

        public string UserId { get; set; }

        public string Image { get; set; }

        public DateTime LatestAdjustmentDate { get; set; }

        #region Projected Data

        /// <summary>
        /// Before using this value check if it is actual (with CalcDate property)
        /// </summary>
        public DateTime ProjectedDate { get; set; }

        /// <summary>
        /// Before using this value check if it is actual (with CalcDate property)
        /// </summary>
        public int MonthsAheadNumber { get; set; }

        public DateTime? CalcDate { get; set; }

        #endregion
    }
}