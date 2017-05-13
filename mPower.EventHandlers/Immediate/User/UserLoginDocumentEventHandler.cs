using MongoDB.Driver.Builders;
using Paralect.ServiceBus;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.Documents.Membership;
using mPower.Domain.Application.Affiliate.Events;
using mPower.Domain.Membership.User.Events;
using mPower.Framework.Environment;

namespace mPower.EventHandlers.Immediate.User
{
    public class UserLoginDocumentEventHandler :
        IMessageHandler<User_LoggedInEvent>,
        IMessageHandler<User_MobileLoggedInEvent>,
        IMessageHandler<Affiliate_UpdatedEvent>
    {
        private readonly UserLoginsDocumentService _userLoginsService;
        private readonly IIdGenerator _idGenerator;

        public UserLoginDocumentEventHandler(UserLoginsDocumentService userLoginsService, IIdGenerator idGenerator)
        {
            _userLoginsService = userLoginsService;
            _idGenerator = idGenerator;
        }

        public void Handle(User_LoggedInEvent message)
        {
            if (!string.IsNullOrEmpty(message.AffiliateName) && !string.IsNullOrEmpty(message.UserName))
            {
                var doc = new UserLoginDocument
                {
                    AffiliateName = message.AffiliateName.ToLower(),
                    AffiliateId = message.AffiliateId,
                    Id = _idGenerator.Generate(),
                    IsFromMobile = false,
                    LoginDate = message.Date,
                    UserId = message.UserId,
                    UserEmail = message.UserEmail.ToLower(),
                    UserName = message.UserName.ToLower()
                };

                _userLoginsService.Insert(doc);
            }
        }

        public void Handle(User_MobileLoggedInEvent message)
        {
            if (!string.IsNullOrEmpty(message.AffiliateName) && !string.IsNullOrEmpty(message.UserName))
            {
                var doc = new UserLoginDocument
                {
                    AffiliateName = message.AffiliateName.ToLower(),
                    AffiliateId = message.AffiliateId,
                    Id = _idGenerator.Generate(),
                    IsFromMobile = true,
                    LoginDate = message.Date,
                    UserId = message.UserId,
                    UserEmail = message.UserEmail.ToLower(),
                    UserName = message.UserName.ToLower()
                };

                _userLoginsService.Insert(doc);
            }
        }

        public void Handle(Affiliate_UpdatedEvent message)
        {
            var query = Query.EQ("AffiliateId", message.ApplicationId);
            var update = Update<UserLoginDocument>.Set(x => x.AffiliateName, message.ApplicationName.ToLower());

            _userLoginsService.UpdateMany(query, update);
        }
    }
}
