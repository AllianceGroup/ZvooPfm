using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Paralect.Domain;
using Paralect.ServiceBus;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Realestate_IncludeInWorthCommandHandler : IMessageHandler<User_Realestate_IncludeInWorthCommand>
    {
        private readonly IRepository _repository;

        public User_Realestate_IncludeInWorthCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_Realestate_IncludeInWorthCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.IncludeInWorthRealestate(message.UserId, message.Id, message.IsIncludedInWorth);

            _repository.Save(user);
        }
    }
}