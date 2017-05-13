using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices.Membership;
using mPower.Domain.Yodlee;
using mPower.Domain.Yodlee.YodleeUser.Commands;
using mPower.Framework;
using mPower.Framework.Mvc;

namespace Default.Factories
{
    public class YodleeUserInfoFactory : IObjectFactory<string, YodleeUserInfoDocument>
    {
        private readonly UserDocumentService _userDocumentService;
        private readonly ICommandService _commandService;

        public YodleeUserInfoFactory(UserDocumentService userDocumentService, ICommandService commandService)
        {
            _userDocumentService = userDocumentService;
            _commandService = commandService;
        }

        public YodleeUserInfoDocument Load(string userId)
        {
            var user = _userDocumentService.GetById(userId);

            if(user.YodleeUserInfo == null)
            {
                //var charArray = user.Email.ToCharArray();
                //Array.Reverse(charArray);
                
                var cmd = new YodleeUser_RegisterCommand
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Password = YodleePasswordGenerator.Generate(),
                    Username = "_" + user.UserName
                };

                _commandService.Send(cmd);

                user.YodleeUserInfo = new YodleeUserInfoDocument()
                                          {
                                              EmailAddress = user.Email,
                                              Password = cmd.Password,
                                              LoginName = cmd.Username
                                          };
            }

            return user.YodleeUserInfo;
        }
    }
}