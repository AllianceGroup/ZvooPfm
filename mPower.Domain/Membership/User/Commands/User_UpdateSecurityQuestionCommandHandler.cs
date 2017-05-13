using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_UpdateSecurityQuestionCommandHandler : IMessageHandler<User_UpdateSecurityQuestionCommand>
    {
        private readonly IRepository _repository;

        public User_UpdateSecurityQuestionCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_UpdateSecurityQuestionCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.UpdateSecurityQuestion(message.Question, message.Answer);

            _repository.Save(user);
        }
    }
}
