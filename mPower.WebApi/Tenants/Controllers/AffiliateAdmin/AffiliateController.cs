using System;
using System.Linq;
using System.Text;
using Default.Areas.Administration.Models;
using Default.Helpers;
using Default.ViewModel.Areas.Administration;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Domain.Accounting;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Yodlee.YodleeUser.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Services;
using mPower.Framework.Utils;
using mPower.WebApi.Tenants.ViewModels.Filters;
using Microsoft.AspNetCore.Mvc;
using mPower.WebApi.Authorization;

namespace mPower.WebApi.Tenants.Controllers.AffiliateAdmin
{
    [AllianceAuthorize(UserPermissionEnum.AffiliateAdminView)]
    [Route("api/[controller]")]
    public class AffiliateController : BaseController
    {
        private readonly UserDocumentService _userService;
        private readonly EventLogDocumentService _eventLogervice;
        private readonly IIdGenerator _idGenerator;
        private readonly AccountsService _accountsService;

        public AffiliateController(UserDocumentService userService, EventLogDocumentService eventLogervice,
            IIdGenerator idGenerator, AccountsService accountsService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _userService = userService;
            _eventLogervice = eventLogervice;
            _idGenerator = idGenerator;
            _accountsService = accountsService;
        }

        #region Users
        [HttpGet("GetUsers")]
        public UsersListModel GetUsers()
        {
            return GetUsersModel(1);
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("addUser")]
        public IActionResult AddUser([FromBody] UserModel model)
        {
            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            var filter = new UserFilter { Email = model.Email, AffiliateId = Tenant.ApplicationId };
            if (_userService.GetByFilter(filter).Any())
            {
                ModelState.AddModelError("Error", "Email is already in use. Please try another.");
                return new BadRequestObjectResult(ModelState);
            }

            var command = new User_CreateCommand
            {
                ApplicationId = Tenant.ApplicationId,
                UserName = model.UserName,
                LastName = model.LastName,
                FirstName = model.FirstName,
                IsActive = true,
                CreateDate = DateTime.Now,
                Email = model.Email,
                UserId = _idGenerator.Generate(),
                PasswordHash = SecurityUtil.GetMD5Hash(model.NewPassword)
            };

            Send(command);
            
            var user = _userService.GetById(command.UserId);

            AddRemovePermission(UserPermissionEnum.AffiliateAdminDelete, user, model.AffiliateAdminDelete);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminEdit, user, model.AffiliateAdminEdit);
            AddRemovePermission(UserPermissionEnum.AffiliateAdminView, user, model.AffiliateAdminView);

            var ledgerId = _idGenerator.Generate();
            var setupPersonalLedgerCommands = _accountsService.SetupPersonalLedger(command.UserId, ledgerId).ToList();

            foreach(var setupPersonalCommand in setupPersonalLedgerCommands)
            {
                Send(setupPersonalCommand);
            }

            return new OkObjectResult(command);
        }

        [HttpGet("userInfo/{id}")]
        public IActionResult ExtendedUserPopup(string id)
        {
            var user = _userService.GetById(id);
            if (user == null || user.ApplicationId != Tenant.ApplicationId)
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

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("profile/{id}")]
        public IActionResult Profile(string id)
        {
            var user = _userService.GetById(id);
            if (user == null || user.ApplicationId != Tenant.ApplicationId)
            {
                ModelState.AddModelError("UserId", "User doesn't exist");
                return new BadRequestObjectResult(ModelState);
            }

            var model = new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                IsActive = user.IsActive,
                UserName = user.UserName,
                Email = user.Email,
                UserId = user.Id,
                AffiliateAdminDelete = user.HasPermissions(UserPermissionEnum.AffiliateAdminDelete),
                AffiliateAdminEdit = user.HasPermissions(UserPermissionEnum.AffiliateAdminEdit),
                AffiliateAdminView = user.HasPermissions(UserPermissionEnum.AffiliateAdminView)
            };

            return new OkObjectResult(model);
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpPost("profile")]
        public IActionResult Profile([FromBody]UserModel model)
        {
            ModelState.Remove(string.Empty);
            if (ModelState.ContainsKey(nameof(model.NewPassword)) && string.IsNullOrEmpty(model.NewPassword))
            {
                ModelState.Remove(nameof(model.NewPassword));
            }

            var user = _userService.GetById(model.UserId);
            if (user == null || user.ApplicationId != Tenant.ApplicationId)
                ModelState.AddModelError("UserId", "User doesn't exist");

            if (user != null && user.Email != model.Email)
            {
                var filter = new UserFilter { Email = model.Email, AffiliateId = Tenant.ApplicationId };
                if (_userService.GetByFilter(filter).Any())
                    ModelState.AddModelError("Error", "Email is already in use. Please try another.");
            }

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

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

            return new OkObjectResult(GetUsers());
        }

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminDelete)]
        [HttpDelete("deleteUser/{id}")]
        public IActionResult DeleteUser(string id)
        {
            var user = _userService.GetById(id);
            if (user == null || user.ApplicationId != Tenant.ApplicationId)
            {
                ModelState.AddModelError("id", "User is not exists.");
                return new BadRequestObjectResult(ModelState);
            }
            if (user.Id == GetUserId())
            {
                ModelState.AddModelError("id", "Can't remove current user.");
                return new BadRequestObjectResult(ModelState);
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

        [AllianceAuthorize(UserPermissionEnum.AffiliateAdminEdit)]
        [HttpGet("toggleUserIsActive/{id}")]
        public IActionResult ToggleUserIsActive(string id)
        {
            var user = _userService.GetById(id);
            if (user == null || user.ApplicationId != Tenant.ApplicationId)
            {
                ModelState.AddModelError("UserId", "User doesn't exist");
                return new BadRequestObjectResult(ModelState);
            }

            if (user.IsActive){
                var command = new User_DeactivateCommand {UserId = user.Id, IsAdmin = true};
                Send(command);
            }
            else{
                var command = new User_ActivateCommand {UserId = user.Id, IsAdmin = true};
                Send(command);
            }

            return new OkResult();
        }

        [HttpGet("activity")]
        public IActionResult Activity(ActivityFilter filter)
        {
            var user = _userService.GetById(filter.Id);
            if (user == null || user.ApplicationId != Tenant.ApplicationId)
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

        [HttpGet("exportUsersToCsv")]
        public IActionResult ExportUsersToCsv()
        {
            var currentUser = _userService.GetById(GetUserId());

            var users =
                _userService.GetByFilter(new UserFilter { AffiliateId = currentUser.ApplicationId });
            var encoding = new UTF8Encoding();

            return File(encoding.GetBytes(CsvHelper.CreateUsersCsv(users)), "text/csv", "UserReport.csv");
        }

        [HttpGet("refreshUsers")]
        public UsersListModel RefreshUsers(UsersViewModelFilter filter)
        {
            return GetUsersModel(filter.PageNumber, filter.SearchKey);
        }

        private UsersListModel GetUsersModel(int pageNumber, string searchKey = null)
        {
            var currentUser = _userService.GetById(GetUserId());
            var paginInfo = new PagingInfo { ItemsPerPage = 50, CurrentPage = pageNumber };
            var filter = new UserFilter
            {
                PagingInfo = paginInfo,
                AffiliateId = currentUser.ApplicationId,
                SearchKey = searchKey
            };

            var users = _userService.GetByFilter(filter);
            var model = new UsersListModel{
                Paging = paginInfo,
                Users = users,
                SearchKey = searchKey,
                CanEdit = currentUser.HasPermissions(UserPermissionEnum.AffiliateAdminEdit),
                CanDelete = currentUser.HasPermissions(UserPermissionEnum.AffiliateAdminDelete)
            };

            return model;
        }

        private void AddRemovePermission(UserPermissionEnum permission, UserDocument user, bool modelValue)
        {
            if (user.HasPermissions(permission) != modelValue)
            {
                if (modelValue)
                {
                    var addPermissionsCommand = new User_AddPermissionCommand {Permission = permission, UserId = user.Id};
                    Send(addPermissionsCommand);
                }
                else
                {
                    var removePermissionCommand = new User_RemovePermissionCommand {Permission = permission, UserId = user.Id};
                    Send(removePermissionCommand);
                }
            }
        }
        #endregion

    }
}
