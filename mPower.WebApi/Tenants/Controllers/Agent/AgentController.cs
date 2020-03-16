using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Default.Areas.Administration.Models;
using Default.Helpers;
using Default.ViewModel.Areas.Administration;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Credit;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Documents.ExternalServices;
using mPower.Domain.Accounting;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Yodlee.YodleeUser.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.Framework.Services;
using mPower.Framework.Utils;
using mPower.Framework.Utils.CreditCalculator;
using mPower.Framework.Utils.Security;
using mPower.WebApi.Tenants.ViewModels.Filters;
using Microsoft.AspNetCore.Mvc;
using mPower.WebApi.Authorization;

namespace mPower.WebApi.Tenants.Controllers.Agent
{
    [AllianceAuthorize(UserPermissionEnum.Agent)]
    [Route("api/[controller]")]
    public class AgentController : BaseController
    {
        private delegate IActionResult BulkActionDelegate(string userId);

        private readonly UserDocumentService _userService;
        private readonly AffiliateDocumentService _affiliateService;
        private readonly IIdGenerator _idGenerator;
        private readonly IEncryptionService _encrypter;
        private readonly ChargifyService _chargifyService;
        private readonly CreditIdentityDocumentService _creditIdentityService;
        private readonly AccountsService _accountsService;
        private readonly IObjectRepository _objectRepository;
        private readonly EventLogDocumentService _eventLogervice;

        public AgentController(UserDocumentService userService, AffiliateDocumentService affiliateService, EventLogDocumentService eventLogervice,
            IIdGenerator idGenerator, ChargifyService chargifyService, CreditIdentityDocumentService creditIdentityService, 
            IObjectRepository objectRepository, IEncryptionService encrypter, 
            AccountsService accountsService, ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _userService = userService;
            _affiliateService = affiliateService;
            _idGenerator = idGenerator;
            _chargifyService = chargifyService;
            _creditIdentityService = creditIdentityService;
            _objectRepository = objectRepository;
            _encrypter = encrypter;
            _accountsService = accountsService;
            _eventLogervice = eventLogervice;
        }

        #region private
        private UsersListModel GetUsersModel(int pageNumber, string searchKey = null)
        {
            var currentUser = _userService.GetById(GetUserId());
            var paginInfo = new PagingInfo { ItemsPerPage = 50, CurrentPage = pageNumber };
            var filter = new UserFilter
            {
                PagingInfo = paginInfo,
                SearchKey = searchKey,
                IsCreatedByAgent = true,
                CreatedBy= currentUser.Id
            };

            var users = _userService.GetByFilter(filter);
            var model = new UsersListModel
            {
                Paging = paginInfo,
                Users = users,
                SearchKey = searchKey,
                CanEdit = currentUser.HasPermissions(UserPermissionEnum.Agent),
                CanDelete = currentUser.HasPermissions(UserPermissionEnum.Agent)
            };

            return model;
        }

        private Dictionary<string, string> MapAffiliates()
        {
            var affiliates = _affiliateService.GetAll();
            return affiliates.ToDictionary(affiliate => affiliate.ApplicationId, affiliate => affiliate.ApplicationName);
        }

        private void AddRemovePermission(UserPermissionEnum permission, UserDocument user, bool modelValue)
        {
            if (user.HasPermissions(permission) != modelValue)
            {
                if (modelValue)
                {
                    var addPermissionsCommand = new User_AddPermissionCommand
                    {
                        Permission = permission,
                        UserId = user.Id
                    };
                    Send(addPermissionsCommand);
                }
                else
                {
                    var removePermissionCommand = new User_RemovePermissionCommand { Permission = permission, UserId = user.Id };
                    Send(removePermissionCommand);
                }
            }
        }
        #endregion

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpGet("GetUsers")]
        public UsersListModel GetUsers()
        {
            return GetUsersModel(1);
        }

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpGet("addUser")]
        public UserModel AddUser()
        {
            var affiliate = MapAffiliates();

            return new UserModel {Affiliates = affiliate};
        }

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpPost("addUser")]
        public IActionResult AddUser([FromBody] UserModel model)
        {
            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var filter = new UserFilter { Email = model.Email, AffiliateId = model.AffiliateId };
            if (_userService.GetByFilter(filter).Any())
            {
                ModelState.AddModelError("Error", "Email is already in use.  Please try another.");
                return new BadRequestObjectResult(ModelState);
            }
            var currentUser = _userService.GetById(GetUserId());
            var command = new User_CreateCommand
            {
                ApplicationId = model.AffiliateId,
                UserName = model.UserName,
                FirstName = model.FirstName,
                LastName = model.LastName,
                IsActive = true,
                CreateDate = DateTime.Now,
                Email = model.Email,
                UserId = _idGenerator.Generate(),
                PasswordHash = SecurityUtil.GetMD5Hash(model.NewPassword),
                IsAgent=model.Agent,
                CreatedBy = currentUser.Id,
                IsCreatedByAgent = true
            };
            Send(command);

            var userCreated = _userService.GetById(command.UserId);
            AddRemovePermission(UserPermissionEnum.Agent, userCreated, model.Agent);
            //AddRemovePermission(UserPermissionEnum.AgentEdit, userCreated, model.AgentEdit);
            //AddRemovePermission(UserPermissionEnum.AgentView, userCreated, model.AgentView);

            var ledgerId = _idGenerator.Generate();
            var setupPersonalLedgerCommands = _accountsService.SetupPersonalLedger(command.UserId, ledgerId).ToList();

            foreach (var setupPersonalCommand in setupPersonalLedgerCommands)
            {
                Send(setupPersonalCommand);
            }

            var SecurityCommand = new User_UpdateSecuritySettingsCommand
            {
                UserId = command.UserId,
                EnableAdminAccess = true,
                EnableAgentAccess = true
            };
            Send(SecurityCommand);

            return new OkObjectResult(userCreated);
        }

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpGet("getProfile/{id}")]
        public UserModel GetProfile(string id)
        {
            var user = _userService.GetById(id);
            var creditIdentities = _creditIdentityService.GetCreditIdentitiesByUserId(id);
            var creditIdentitiesModel = new List<CreditIndentityModel>();

            foreach (var item in creditIdentities)
            {
                creditIdentitiesModel.Add(new CreditIndentityModel()
                {
                    Ssn = CreditCalculator.MaskSocialSecurityNumber(_encrypter.Decode(item.SocialSecurityNumber)),
                    ClientKeyEncrypted = item.ClientKey,
                    IsEnrolled = item.IsEnrolled
                });
            }

            var model = new UserModel()
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                UserName = user.UserName,
                Email = user.Email,
                UserId = user.Id,
                Agent = user.HasPermissions(UserPermissionEnum.Agent),
                CreditIdentityDocuments = creditIdentitiesModel
            };

            return model;
        }

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpPost("updateProfile")]
        public IActionResult UpdateProfile([FromBody] UserModel model)
        {
            ModelState.Remove(string.Empty);
            if (ModelState.ContainsKey(nameof(model.NewPassword)) && string.IsNullOrEmpty(model.NewPassword))
                ModelState.Remove(nameof(model.NewPassword));

            if (GetUserId() == model.UserId)
            {
                ModelState.AddModelError("", "You cannot edit this profile");
                return new BadRequestObjectResult(ModelState);
            }

            var user = _userService.GetById(model.UserId);
            if (user.Email != model.Email)
            {
                var filter = new UserFilter { Email = model.Email, AffiliateId = model.AffiliateId };
                if (_userService.GetByFilter(filter).Any())
                {
                    ModelState.AddModelError("Error", "Email is already in use. Please try another.");
                }
            }

            if (!ModelState.IsValid)
                return new BadRequestObjectResult(ModelState);

            var command = new User_UpdateCommand
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserId = user.Id,
            };

            Send(command);

            AddRemovePermission(UserPermissionEnum.Agent, user, model.Agent);

            if (!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.ConfirmNewPassword))
            {
                var changePasswordCommand = new User_ChangePasswordCommand
                {
                    ChangeDate = DateTime.Now,
                    UserId = user.Id,
                    PasswordHash = SecurityUtil.GetMD5Hash(model.NewPassword)
                };

                Send(changePasswordCommand);
            }

            var userUpdated = _userService.GetById(command.UserId);
            AddRemovePermission(UserPermissionEnum.Agent, user, model.Agent);
            return new OkObjectResult(userUpdated);
        }

        [HttpGet("refreshUsers")]
        public UsersListModel RefreshUsers(UsersViewModelFilter filter)
        {
            return GetUsersModel(filter.PageNumber, filter.SearchKey);
        }

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpDelete("deleteUser/{id}")]
        public IActionResult DeleteUser(string id)
        {
            _chargifyService.Connect(Tenant.ChargifyUrl, Tenant.ChargifySharedKey, Tenant.ChargifyApiKey);
            var user = _userService.GetById(id);
            var subscriptions = user.Subscriptions;

            var currentUser = _userService.GetById(GetUserId());
            foreach (var subscriptionDocument in subscriptions)
            {
                if (subscriptionDocument.ChargifySubscriptionId != 0)
                    _chargifyService.CancelSubscription(subscriptionDocument.ChargifySubscriptionId,
                        "Subscription was cancelled by global admin: " + currentUser.Email);
            }
            var command = new User_DeleteCommand {UserId = id};

            if (user.YodleeUserInfo != null)
            {
                var command2 = new YodleeUser_CancelCommand
                {
                    Username = user.YodleeUserInfo.LoginName,
                    Password = user.YodleeUserInfo.Password
                };
                Send(command2);
            }

            Send(command);

            return new OkResult();
        }

        [HttpGet("getUserDetails/{id}")]
        public UserExtendedInfoModel GetUserDetails(string id)
        {
            var user = _userService.GetById(id);

            var doc = new UserExtendedInfoModel
            {
                Id = user.Id,
                ZipCode = user.ZipCode,
                ApplicationId = user.ApplicationId,
                AuthToken = user.AuthToken,
                CreateDate = user.CreateDate,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive ? "active" : "inactive",
                IsEnrolled = user.IsEnrolled ? "enrolled" : "not enrolled",
                LastLoginDate = user.LastLoginDate,
                LastPasswordChangedDate = user.LastPasswordChangedDate,
                MobileAccessToken = user.MobileAccessToken,
                PasswordAnswer = user.PasswordAnswer,
                PasswordQuestion = user.PasswordQuestion,
                Permissions = string.Join(",", user.Permissions.Select(x => x.ToString())),
                Phones = string.Join(",", user.Phones),
                ResetPasswordToken = user.ResetPasswordToken,
                SecurityLevel = user.SecurityLevel.ToString(),
                UserName = user.UserName,
                ReferralCode = user.ReferralCode,
                YodleeUserInfo = user.YodleeUserInfo
            };

            return doc;
        }

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpGet("activate/{id}")]
        public IActionResult ActivateUser(string id)
        {
            var user = _userService.GetById(id);
            var activateUserCommand = new User_ActivateCommand { UserId = user.Id, IsAdmin = true };

            Send(activateUserCommand);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.Agent)]
        [HttpGet("deactivate/{id}")]
        public IActionResult DeactivateUser(string id)
        {
            var user = _userService.GetById(id);
            var deactivateUserCommand = new User_DeactivateCommand {UserId = user.Id, IsAdmin = true};

            Send(deactivateUserCommand);

            return new OkResult();
        }

        [HttpGet("exportUsersToCsv")]
        public IActionResult ExportUsersToCsv()
        {
            var currentUser = _userService.GetById(GetUserId());

            var filter = new UserFilter
            {
                IsAgent = true,
                CreatedBy = currentUser.Id
            };
            var users =
                _userService.GetByFilter(filter);

            var usersString = CsvHelper.CreateUsersCsv(users);

            var encoding = new UTF8Encoding();
            return File(encoding.GetBytes(usersString), "text/csv", "UserReport.csv");
        }

        [HttpGet("userInfo/{id}")]
        public IActionResult ExtendedUserPopup(string id)
        {
            var user = _userService.GetById(id);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "User doesn't exist");
                return new BadRequestObjectResult(ModelState);
            }

            return new OkObjectResult(new UserExtendedInfoModel
            {
                Id = user.Id,
                ZipCode = user.ZipCode,
                ApplicationId = user.ApplicationId,
                AuthToken = user.AuthToken,
                CreateDate = user.CreateDate,
                Email = user.Email,
                FullName = user.FullName,
                IsActive = user.IsActive ? "active" : "inactive",
                IsEnrolled = user.IsEnrolled ? "enrolled" : "not enrolled",
                LastLoginDate = user.LastLoginDate,
                LastPasswordChangedDate = user.LastPasswordChangedDate,
                MobileAccessToken = user.MobileAccessToken,
                PasswordAnswer = user.PasswordAnswer,
                PasswordQuestion = user.PasswordQuestion,
                Permissions = string.Join(",", user.Permissions.Select(x => x.ToString())),
                Phones = string.Join(",", user.Phones),
                ResetPasswordToken = user.ResetPasswordToken,
                SecurityLevel = user.SecurityLevel.ToString(),
                UserName = user.UserName,
                ReferralCode = user.ReferralCode,
                YodleeUserInfo = user.YodleeUserInfo
            });
        }

        [HttpGet("activity")]
        public IActionResult Activity(ActivityFilter filter)
        {
            var user = _userService.GetById(filter.Id);
            if (user == null)
            {
                ModelState.AddModelError("UserId", "User doesn't exist.");
                return new BadRequestObjectResult(ModelState);
            }

            var model = new ActivityListModel
            {
                UserId = user.Id,
                UserFullName = user.FullName
            };

            var paging = new PagingInfo { Take = 10, CurrentPage = filter.PageNumber };
            var activities = _eventLogervice.GetUserActivity(filter.Id, paging);

            model.Activities = activities;
            model.Paging = paging;

            return new OkObjectResult(model);
        }

        #region Bulk Actions
        private static IActionResult BulkAction(List<string> userIds, BulkActionDelegate bulkActionDelegate)
        {
            if(userIds != null)
            {
                foreach(var userId in userIds)
                {
                    bulkActionDelegate.Invoke(userId);
                }
            }

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminDelete)]
        [HttpGet("bulkDeleteUser")]
        public IActionResult BulkDeleteUser(List<string> ids)
        {
            return BulkAction(ids, DeleteUser);
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
        [HttpGet("bulkDeactivateUser")]
        public IActionResult BulkDeactivateUser(List<string> ids)
        {
            return BulkAction(ids, DeactivateUser);
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
        [HttpGet("bulkActivateUser")]
        public IActionResult BulkActivateUser(List<string> ids)
        {
            return BulkAction(ids, ActivateUser);
        }
        #endregion
    }
}
