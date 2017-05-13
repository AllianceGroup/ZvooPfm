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

namespace mPower.WebApi.Tenants.Controllers.GlobalAdmin
{
    [AllianceAuthorize(UserPermissionEnum.GlobalAdminView)]
    [Route("api/[controller]")]
    public class GlobalController : BaseController
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

        public GlobalController(UserDocumentService userService, AffiliateDocumentService affiliateService, 
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
        }

        #region private
        private UsersListModel GetUsersModel(int pageNumber, string searchKey = null, string affiliate = null)
        {
            var currentUser = _userService.GetById(GetUserId());
            var paginInfo = new PagingInfo {ItemsPerPage = 100, CurrentPage = pageNumber};

            var filter = new UserFilter { PagingInfo = paginInfo, AffiliateId = affiliate, SearchKey = searchKey };
            var users = _userService.GetByFilter(filter);

            var affiliates = MapAffiliates();
            var model = new UsersListModel
            {
                Paging = paginInfo,
                Users = users,
                AffiliateName = affiliates,
                SearchKey = searchKey,
                CanEdit = currentUser.HasPermissions(UserPermissionEnum.GlobalAdminEdit),
                CanDelete = currentUser.HasPermissions(UserPermissionEnum.GlobalAdminDelete),
                Affiliate = affiliate
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

        [HttpGet]
        public UsersListModel GetUsers()
        {
            return GetUsersModel(1);
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
        [HttpGet("addUser")]
        public UserModel AddUser()
        {
            var affiliate = MapAffiliates();

            return new UserModel {Affiliates = affiliate};
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
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
                PasswordHash = SecurityUtil.GetMD5Hash(model.NewPassword)
            };
            Send(command);

            var userCreated = _userService.GetById(command.UserId);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminDelete, userCreated, model.AffiliateAdminDelete);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminEdit, userCreated, model.AffiliateAdminEdit);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminView, userCreated, model.AffiliateAdminView);
            AddRemovePermission(UserPermissionEnum.GlobalAdminDelete, userCreated, model.GlobalAdminDelete);
            AddRemovePermission(UserPermissionEnum.GlobalAdminEdit, userCreated, model.GlobalAdminEdit);
            AddRemovePermission(UserPermissionEnum.GlobalAdminView, userCreated, model.GlobalAdminView);

            var ledgerId = _idGenerator.Generate();
            var setupPersonalLedgerCommands = _accountsService.SetupPersonalLedger(command.UserId, ledgerId).ToList();

            foreach (var setupPersonalCommand in setupPersonalLedgerCommands)
            {
                Send(setupPersonalCommand);
            }

            return new OkObjectResult(userCreated);
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
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
                AffiliateAdminDelete = user.HasPermissions(UserPermissionEnum.AffiliateAdminDelete),
                AffiliateAdminView = user.HasPermissions(UserPermissionEnum.AffiliateAdminView),
                AffiliateAdminEdit = user.HasPermissions(UserPermissionEnum.AffiliateAdminEdit),
                GlobalAdminEdit = user.HasPermissions(UserPermissionEnum.GlobalAdminEdit),
                GlobalAdminView = user.HasPermissions(UserPermissionEnum.GlobalAdminView),
                GlobalAdminDelete = user.HasPermissions(UserPermissionEnum.GlobalAdminDelete),
                CreditIdentityDocuments = creditIdentitiesModel
            };

            return model;
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
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
                UserId = user.Id
            };

            Send(command);

            AddRemovePermission(UserPermissionEnum.AffiliateAdminDelete, user, model.AffiliateAdminDelete);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminEdit, user, model.AffiliateAdminEdit);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminView, user, model.AffiliateAdminView);
            AddRemovePermission(UserPermissionEnum.GlobalAdminDelete, user, model.GlobalAdminDelete);
            AddRemovePermission(UserPermissionEnum.GlobalAdminEdit, user, model.GlobalAdminEdit);
            AddRemovePermission(UserPermissionEnum.GlobalAdminView, user, model.GlobalAdminView);

            if(!string.IsNullOrEmpty(model.NewPassword) && !string.IsNullOrEmpty(model.ConfirmNewPassword))
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
            AddRemovePermission(UserPermissionEnum.AffiliateAdminDelete, userUpdated, model.AffiliateAdminDelete);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminEdit, userUpdated, model.AffiliateAdminEdit);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminView, userUpdated, model.AffiliateAdminView);
            AddRemovePermission(UserPermissionEnum.GlobalAdminDelete, userUpdated, model.GlobalAdminDelete);
            AddRemovePermission(UserPermissionEnum.GlobalAdminEdit, userUpdated, model.GlobalAdminEdit);
            AddRemovePermission(UserPermissionEnum.GlobalAdminView, userUpdated, model.GlobalAdminView);

            return new OkObjectResult(userUpdated);
        }

        [HttpGet("refreshUsers")]
        public UsersListModel RefreshUsers(UsersViewModelFilter filter)
        {
            return GetUsersModel(filter.PageNumber, filter.SearchKey, filter.Affiliate);
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminDelete)]
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

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
        [HttpGet("activate/{id}")]
        public IActionResult ActivateUser(string id)
        {
            var user = _userService.GetById(id);
            var activateUserCommand = new User_ActivateCommand { UserId = user.Id, IsAdmin = true };

            Send(activateUserCommand);

            return new OkResult();
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminEdit)]
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
            var users =
                _userService.GetAll();

            var usersString = CsvHelper.CreateUsersCsv(users);

            var encoding = new UTF8Encoding();
            return File(encoding.GetBytes(usersString), "text/csv", "UserReport.csv");
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
