using System;
using MongoDB.Bson;
using MongoDB.Driver.Builders;
using mPower.Documents.Documents.Goal;
using mPower.Documents.DocumentServices.Goal;
using mPower.Documents.Enums;
using mPower.Domain.Accounting.Goal.Events;
using Paralect.ServiceBus;
using mPower.Domain.Accounting.Goal.Messages;
using mPower.Framework;

namespace mPower.EventHandlers.Immediate
{
    public class GoalDocumentEventHandler :
        IMessageHandler<Goal_CreatedEvent>,
        IMessageHandler<Goal_AdjustCurrentAmountEvent>,
        IMessageHandler<Goal_MarkedAsCompletedEvent>,
        IMessageHandler<Goal_ArchivedEvent>,
        IMessageHandler<Goal_DeletedEvent>,
        IMessageHandler<Goal_UpdatedEvent>
    {
        private readonly GoalDocumentService _goalService;
        private readonly IEventService _eventService;

        public GoalDocumentEventHandler(GoalDocumentService goalService, IEventService eventService)
        {
            _goalService = goalService;
            _eventService = eventService;
        }

        public void Handle(Goal_CreatedEvent message)
        {
            var document = new GoalDocument
            {
                GoalId = message.GoalId,
                Type = message.Type,
                Title = message.Title,
                TotalAmountInCents = message.TotalAmountInCents,
                MonthlyPlanAmountInCents = message.MonthlyPlanAmountInCents,
                StartDate = message.StartDate.Date, // time part isn't required
                PlannedDate = message.PlannedDate.Date,
                UserId = message.UserId,
                Image = message.Image,
                Status = GoalStatusEnum.Projected,
                ProjectedDate = (message.ProjectedDate > DateTime.MinValue ? message.ProjectedDate : message.PlannedDate).Date,
                MonthsAheadNumber = 0,
                CalcDate = null, 
                StartingBalanceInCents = message.StartingBalanceInCents              
            };
            _goalService.Save(document);
        }

        public void Handle(Goal_MarkedAsCompletedEvent message)
        {
            var query = Query.EQ("_id", message.GoalId);
            var update = Update<GoalDocument>.Set(x => x.Status, GoalStatusEnum.Completed);
            _goalService.Update(query, update);
        }

        public void Handle(Goal_ArchivedEvent message)
        {
            var query = Query.EQ("_id", message.GoalId);
            var update = Update<GoalDocument>.Set(x => x.Status, GoalStatusEnum.Archived);
            _goalService.Update(query, update);
        }

        public void Handle(Goal_AdjustCurrentAmountEvent message)
        {
            var goal = _goalService.GetById(message.GoalId);

            var query = Query.EQ("_id", message.GoalId);
            var update = Update<GoalDocument>.Inc(x=> x.CurrentAmountInCents, message.ValueInCents)
                .Set(x => x.LatestAdjustmentDate, message.Date)
                .Set(x => x.CalcDate, null);
            update = IsSameMonth(goal.LatestAdjustmentDate, message.Date)
                ? update.Inc(x => x.LatestMonthAdjustmentInCents, message.ValueInCents)
                : update.Set(x => x.LatestMonthAdjustmentInCents, message.ValueInCents);

            _goalService.Update(query, update);
        }

        public void Handle(Goal_UpdatedEvent message)
        {
            var query = Query.EQ("_id", message.GoalId);
            var update = Update<GoalDocument>
                .Set(x => x.Title, message.Title)
                .Set(x => x.TotalAmountInCents, message.TotalAmountInCents)
                .Set(x => x.MonthlyPlanAmountInCents, message.MonthlyPlanAmountInCents)
                .Set(x => x.PlannedDate, message.PlannedDate)
                .Set(x => x.ProjectedDate, (message.ProjectedDate > DateTime.MinValue ? message.ProjectedDate : message.PlannedDate).Date)
                .Set(x => x.Image, message.Image)
                .Set(x => x.CalcDate, null)
                .Set(x => x.StartingBalanceInCents, message.StartingBalanceInCents);

            _goalService.Update(query, update);
        }

        public void Handle(Goal_DeletedEvent message)
        {
            var goal = _goalService.GetById(message.GoalId);

            _goalService.RemoveById(message.GoalId);

            _eventService.Send(new Goal_DeletedMessage
            {
                GoalId = goal.GoalId,
                Type = goal.Type,
                UserId = goal.UserId,
                StartDate = goal.StartDate,
            });
        }

        private bool IsSameMonth(DateTime date1, DateTime date2)
        {
            return date1.Year == date2.Year && date1.Month == date2.Month;
        }
    }
}