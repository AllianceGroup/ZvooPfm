using System;
using Paralect.Domain;
using Paralect.ServiceBus;
using com.yodlee.common;
using com.yodlee.core.usermanagement;
using com.yodlee.ext.login;
using com.yodlee.soap.appext.ybase.passwordreset;
using com.yodlee.soap.core.usermanagement;
using java.util;
using mPower.Domain.Yodlee.Services;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Domain.Membership.User.Commands;

namespace mPower.Domain.Yodlee.YodleeUser.Commands
{
    public class YodleeUser_RegisterCommandHandler : BaseYodleeService, IMessageHandler<YodleeUser_RegisterCommand>
    {
        private readonly IRepository _repository;
        private readonly IIdGenerator _id;
        private readonly CommandService _command;

        public YodleeUser_RegisterCommandHandler(IRepository repository, IIdGenerator id, CommandService command)
        {
            _repository = repository;
            _id = id;
            _command = command;
        }

        public void Handle(YodleeUser_RegisterCommand message)
        {
            ConnectToYodlee();
            var passwordCredentials = new PasswordCredentials
                                          {
                                              loginName = message.Username,
                                              password = message.Password
                                          };
            var map = new Map();
            map.Add(UserProfile.EMAIL_ADDRESS, message.Email);
            var userProfile = new UserProfile { values = map };
            var nvPairs = new[] { new NVPair { name = "com.yodlee.userprofile.LOCALE", values = new[] { ((object)"en-US") } } };
            var userRegistrationService = new UserRegistrationService();

            try
            {
                var userInfo = userRegistrationService.register(_cobrandContext, passwordCredentials, userProfile, nvPairs);

                var cmd = new User_AddYodleeAccountCommand()
                              {
                                  EmailAddress = userInfo.emailAddress,
                                  LastLoginTime = userInfo.lastLoginTime,
                                  LoginCount = userInfo.loginCount,
                                  UserId = message.UserId,
                                  Password = message.Password,
                                  LoginName = userInfo.loginName,
                                  PasswordChangedOn = userInfo.pwdChangedOn,
                                  PasswordExpiryDays = userInfo.pwdExpiryDays,
                                  PasswordExpiryNotificationDays = userInfo.pwdExpiryNotificationDays,
                                  PasswordRecovered = userInfo.passwordRecovered,
                                  //UserType = userInfo.userType.userTypeName,
                              };

                _command.Send(cmd);


            }catch(Exception)
            {
                var resetService = new PasswordResetManagementService();
               
                var token = resetService.getToken(_cobrandContext, message.Username);
                
                var userContext = resetService.getPasswordResetUserContextFromToken(_cobrandContext,
                                                                                              token.token);
                resetService.setSessionForValidToken(userContext, token.token);

                resetService.changePassword(userContext, token.token, passwordCredentials);

                var cmd = new User_AddYodleeAccountCommand()
                {
                    EmailAddress = message.Email,
                    UserId = message.UserId,
                    Password = message.Password,
                    LoginName = message.Username,
                };

                _command.Send(cmd);
            }
        }
    }
}