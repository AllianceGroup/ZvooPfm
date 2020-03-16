using Paralect.Domain;
using Paralect.ServiceBus;
using mPower.Domain.Membership.User.Data;

namespace mPower.Domain.Membership.User.Commands
{
    public class User_CreateCommandHandler : IMessageHandler<User_CreateCommand>
    {
        private readonly IRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public User_CreateCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        public void Handle(User_CreateCommand message)
        {
            var data = new UserData
            {
                UserId = message.UserId, 
                FirstName= message.FirstName, 
                LastName = message.LastName, 
                Email = message.Email, 
                UserName = message.UserName, 
                PasswordHash = message.PasswordHash,
                CreateDate = message.CreateDate,
                IsActive  = message.IsActive, 
                ApplicationId = message.ApplicationId,
                Metadata = message.Metadata,
                ZipCode = message.ZipCode,
                BirthDate = message.BirthDate,                                
                Gender = message.Gender,                                
                ReferralCode = message.ReferralCode,
                IsAgent= message.IsAgent,
                CreatedBy=message.CreatedBy,
                IsCreatedByAgent=message.IsCreatedByAgent
            };

            var user = new UserAR(data);
            _repository.Save(user);
        }
    }
}
