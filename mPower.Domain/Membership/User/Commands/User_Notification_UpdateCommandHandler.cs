using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Notification_UpdateCommandHandler : IMessageHandler<User_Notification_UpdateCommand>
    {
        private readonly Repository _repository;

        public User_Notification_UpdateCommandHandler(Repository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Notification_UpdateCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            var data = new NotificationData
            {
                Type = message.Type,
                SendEmail = message.SendEmail,
                SendText = message.SendText,
                BorderValue = message.BorderValue,
            };
            user.UpdateNotification(data);

            _repository.Save(user);
        }
    }
}
