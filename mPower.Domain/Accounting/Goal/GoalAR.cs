using System;
using Paralect.Domain;
using mPower.Domain.Accounting.Goal.Data;
using mPower.Domain.Accounting.Goal.Events;
using mPower.Framework;

namespace mPower.Domain.Accounting.Goal
{
    public class GoalAR : MpowerAggregateRoot
    {
        /// <summary>
        /// For object reconstraction
        /// </summary>
        public GoalAR() { }

        public GoalAR(string goalId, GoalData data, ICommandMetadata metadata)
        {
            SetCommandMetadata(metadata);

            Apply(new Goal_CreatedEvent
            {
                GoalId = goalId,
                Type = data.Type,
                Title = data.Title,
                TotalAmountInCents = data.TotalAmountInCents,
                MonthlyPlanAmountInCents = data.MonthlyPlanAmountInCents,
                StartDate = data.StartDate,
                PlannedDate = data.PlannedDate,
                ProjectedDate = data.ProjectedDate,
                UserId = data.UserId,
                Image = data.Image,
                StartingBalanceInCents = data.StartingBalanceInCents
            });
        }

        public void Update(GoalData data)
        {
            Apply(new Goal_UpdatedEvent
            {
                GoalId = _id,
                Title = data.Title,
                TotalAmountInCents = data.TotalAmountInCents,
                MonthlyPlanAmountInCents = data.MonthlyPlanAmountInCents,
                PlannedDate = data.PlannedDate,
                ProjectedDate = data.ProjectedDate,
                Image = data.Image,
                StartingBalanceInCents = data.StartingBalanceInCents
            });
        }

        public void MarkAsCompleted()
        {
            Apply(new Goal_MarkedAsCompletedEvent
            {
                GoalId = _id
            });
        }

        public void Archive()
        {
            Apply(new Goal_ArchivedEvent
            {
                GoalId = _id
            });
        }

        public void AdjustCurrentAmount(long valueInCents, DateTime date)
        {
            Apply(new Goal_AdjustCurrentAmountEvent
            {
                GoalId = _id,
                ValueInCents = valueInCents,
                Date = date,
            });
        }

        public void Delete()
        {
            Apply(new Goal_DeletedEvent
            {
                GoalId = _id
            });
        }

        #region Object Reconstruction

        protected void On(Goal_CreatedEvent created)
        {
            _id = created.GoalId;
        }

        #endregion
    }
}