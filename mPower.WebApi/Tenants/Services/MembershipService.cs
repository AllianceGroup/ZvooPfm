using System;
using System.Linq;
using Default.Areas.Api.Models;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Domain.Membership.User.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils;
using Paralect.Domain;

namespace mPower.WebApi.Tenants.Services
{
    public class MembershipService
    {
        private readonly UserDocumentService _userService;
        private readonly MPowerSettings _settings;
        private readonly IRepository _repository;
        private readonly IIdGenerator _idGenerator;
        private readonly IApplicationTenant _tenant;
        private readonly ICommandService _commandService;

        public MembershipService(UserDocumentService userService, MPowerSettings settings, IRepository repository,
            IIdGenerator idGenerator, IApplicationTenant tenant, ICommandService commandService)
        {
            _userService = userService;
            _settings = settings;
            _repository = repository;
            _idGenerator = idGenerator;
            _tenant = tenant;
            _commandService = commandService;
        }

        public UserDocument Login(string userName, string password)
        {
            var filter = new UserFilter {UserNameOrEmail = userName};

            var user = _userService.GetByFilter(filter).FirstOrDefault();
            if (user == null || user.Password != SecurityUtil.GetMD5Hash(password))
            {
                return null;
            }
            var authToken = SecurityUtil.GetUniqueToken();
            user.AuthToken = authToken;
            var loginCommand = new User_LogInCommand
            {
                LogInDate = DateTime.Now,
                UserId = user.Id,
                AuthToken = authToken,
                AffiliateName = _tenant.ApplicationName,
                AffiliateId = _tenant.ApplicationId,
                UserEmail = user.Email,
                UserName = user.UserName,
                Metadata = {UserId = user.Id}
            };
            Send(loginCommand);
            return _userService.GetById(user.Id);
        }

        public UserDocument CreateUser(CreateUserModel model)
        {
            if (_userService.GetUserByUserName(model.UserName) == null)
            {
                var command = new User_CreateCommand
                {
                    CreateDate = DateTime.Now,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    IsActive = true,
                    LastName = model.LastName,
                    PasswordHash = SecurityUtil.GetMD5Hash(model.Password),
                    UserId = _idGenerator.Generate(),
                    UserName = model.UserName,
                    ApplicationId = _tenant.ApplicationId,
                    //ReferralCode = SessionContext.ReferralCode,
                };

                Send(command);

                return _userService.GetById(command.UserId);
            }
            return null;
        }

        public string GetAuthenticationQuestion(string userId)
        {
            var user = _userService.GetById(userId);
            if (user == null || !IsUserBelongToTheAffiliate(user))
            {
                //ApiResponse.SetErrorCode((int)MembershipApiErrorCodesEnum.UserNotFound);
                return null;
            }
            return user.PasswordQuestion;
        }

        public bool ValidateAuthenticationQuestion(string userId, string answer)
        {
            var user = _userService.GetById(userId);
            if (user == null || !IsUserBelongToTheAffiliate(user))
            {
                //ApiResponse.SetErrorCode((int)MembershipApiErrorCodesEnum.UserNotFound);
                return false;
            }
            return user.PasswordAnswer == answer;
        }

        public void Activate(string userId)
        {
            var user = _userService.GetById(userId);
            if (user == null || !IsUserBelongToTheAffiliate(user))
            {
                //ApiResponse.SetErrorCode((int)MembershipApiErrorCodesEnum.UserNotFound);
            }
            else
            {
                var activateCommand = new User_ActivateCommand
                {
                    UserId = userId
                };

                Send(activateCommand);
            }
            
        }

        public UserDocument GetUserByUsername(string userName)
        {
            var user = _userService.GetUserByUserName(userName);
            if (user == null || !IsUserBelongToTheAffiliate(user))
            {
                //ApiResponse.SetErrorCode((int)MembershipApiErrorCodesEnum.UserNotFound);
                return null;
            }
            return user;
        }


        protected void Send(params ICommand[] commands)
        {
            foreach (var command in commands.Where(command => string.IsNullOrEmpty(command.Metadata.UserId)))
            {
                command.Metadata.UserId = "Api";
            }
            _commandService.Send(commands);
        }

        private bool IsUserBelongToTheAffiliate(UserDocument user)
        {
            return user.ApplicationId == _tenant.ApplicationId;
        }
    }
}