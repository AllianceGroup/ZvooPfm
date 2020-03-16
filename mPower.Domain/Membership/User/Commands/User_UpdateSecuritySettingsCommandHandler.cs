using mPower.Domain.Membership.User.Messages;
using mPower.Framework;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateSecuritySettingsCommandHandler : IMessageHandler<User_UpdateSecuritySettingsCommand>
    {
        private readonly IRepository _repository;
        private readonly IEventService _eventService;

        public User_UpdateSecuritySettingsCommandHandler(IRepository repository, IEventService eventService)
        {
            _repository = repository;
            _eventService = eventService;
        }

        public void Handle(User_UpdateSecuritySettingsCommand message)
        {
            var ar = _repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.UpdateSecuritySettings(message.EnableAdminAccess, message.EnableAggregationLogging, message.EnableAgentAccess);
            _repository.Save(ar);
            if (!message.EnableAggregationLogging)
            {
                var msg = new IntuitLogsDeletedMessage {UserId = message.UserId};
                _eventService.Send(msg);
            }
        }
    }
}