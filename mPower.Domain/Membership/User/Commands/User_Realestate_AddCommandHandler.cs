using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_Realestate_AddCommandHandler : IMessageHandler<User_Realestate_AddCommand>
    {
         private readonly IRepository _repository;

         public User_Realestate_AddCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

         public void Handle(User_Realestate_AddCommand message)
        {
            var user = _repository.GetById<UserAR>(message.UserId);
            user.SetCommandMetadata(message.Metadata);
            user.AddRealestate(new RealestateData
                                   {
                                       Id = message.Id,
                                       Name = message.Name,
                                       AmountInCents = message.AmountInCents,
                                       RawData = message.RawData,
                                   });

            _repository.Save(user);
        }
    }
}