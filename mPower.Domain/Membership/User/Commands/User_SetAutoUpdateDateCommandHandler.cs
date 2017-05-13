using Paralect.Domain;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_SetAutoUpdateDateCommandHandler
    {
        private readonly IRepository _repository;

        public User_SetAutoUpdateDateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_SetAutoUpdateDateCommand message)
        {
            var ar = _repository.GetById<UserAR>(message.UserId);
            ar.SetCommandMetadata(message.Metadata);
            ar.SetAutoUpdateDate(message.Date);
            _repository.Save(ar);
        }
    }
}