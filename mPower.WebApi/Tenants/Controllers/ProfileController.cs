using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Default;
using Default.Models;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Documents.ExternalServices;
using mPower.Domain.Application.Enums;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Domain.Yodlee.YodleeUser.Commands;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Utils;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Tenants.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class ProfileController : BaseController
    {
        private readonly UserDocumentService _userService;
        private readonly ChargifyService _chargifyService;

        public ProfileController(UserDocumentService userService, ChargifyService chargifyService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _userService = userService;
            _chargifyService = chargifyService;
        }
        #region Membership
        [HttpGet("membership")]
        public MembershipModel MemberShip()
        {
            var user = _userService.GetById(GetUserId());

            return new MembershipModel
            {
                Subscriptions = user.Subscriptions,
                Bills = user.BillingsList,
            };
        }

        [HttpPost("membership/cancel")]
        public void CancelSubscription([FromBody]string id)
        {
            var user = _userService.GetById(GetUserId());

            _chargifyService.Connect(Tenant.ChargifyUrl, Tenant.ChargifySharedKey, Tenant.ChargifyApiKey);

            var subscription = user.Subscriptions.Single(x => x.Id == id);

            var cancelMessage = "Subscription was cancelled by user: " + user.Email;
            var credirIdentityId = subscription.CreditIdentityId;

            var cancelSubscription = new User_Subscription_DeleteCommand
            {
                CancelMessage = cancelMessage,
                SubscriptionId = subscription.Id,
                UserId = user.Id,
                CreditIdentityId = credirIdentityId
            };
            Send(cancelSubscription);

            if (string.IsNullOrEmpty(credirIdentityId)) // user unsubscribe from main product
            {
                if (user.YodleeUserInfo != null)
                {
                    var cancel = new YodleeUser_CancelCommand
                    {
                        Username = user.YodleeUserInfo.LoginName,
                        Password = user.YodleeUserInfo.Password
                    };
                    Send(cancel);
                }
            }
        }
        #endregion

        #region Profile
        [HttpGet("model")]
        public ProfileModel ProfileModel()
        {
            return GetProfileModel();
        }

        [HttpPost("save/userdetails")]
        public IActionResult SaveUserDetails([FromBody]UserDetailsModel model)
        {
            var users = _userService.GetByFilter(new UserFilter { Email = model.Email });
            if (users.Any())
            {
                if (!(users.Count == 1 && users.First().Id == GetUserId()))
                {
                    ModelState.AddModelError("Error", "Email is already in use.  Please try another.");
                }
            }

            DateTime birthDate;
            if (!DateTime.TryParse(model.BirthDate, out birthDate))
                ModelState.AddModelError("", "Invalid 'Date of Birth'");

            if (ModelState.IsValid)
            {
                var user = _userService.GetById(GetUserId());
                var command = new User_UpdateCommand
                {
                    UserId = user.Id,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    ZipCode = model.ZipCode,
                    Gender = model.Gender,
                    BirthDate = birthDate,
                };
                Send(command);
                return new OkObjectResult("Contact information has been successfully saved.");
            }
            return new BadRequestObjectResult(ModelState);
        }


        [HttpPost("change/password")]
        public IActionResult ChangePassword([FromBody]ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _userService.GetById(GetUserId());
                if (user.Password == SecurityUtil.GetMD5Hash(model.OldPassword))
                {
                    var command = new User_ChangePasswordCommand
                    {
                        ChangeDate = DateTime.Now,
                        UserId = user.Id,
                        PasswordHash = SecurityUtil.GetMD5Hash(model.NewPassword),
                    };
                    Send(command);

                    return new OkObjectResult("Password has been successfully changed.");
                }
                ModelState.AddModelError("OldPassword", "Invalid current password.");
            }

            return new BadRequestObjectResult(ModelState);
        }

        [HttpPost("save/security")]
        public string SaveSecurityLevel([FromBody]SecurityLevelModel model)
        {
            var command = new User_UpdateSecurityLevelCommand
            {
                UserId = GetUserId(),
                SecurityLevel = model.SelectedLevel,
            };
            Send(command);

            return "Security level has been successfully changed.";
        }

        [HttpPost("save/security/questions")]
        public string SaveSecurityQuestion([FromBody]SecurityQuestionModel model)
        {
            var command = new User_UpdateSecurityQuestionCommand
            {
                UserId = GetUserId(),
                Question = model.SecurityQuestion,
                Answer = model.Answer,
            };
            Send(command);

            return "Security question has been successfully changed.";
        }

        [HttpPost("save/security/settings")]
        public string SaveSecuritySettings([FromBody]SecuritySettingsModel model)
        {
            var command = new User_UpdateSecuritySettingsCommand
            {
                UserId = GetUserId(),
                EnableAdminAccess = model.EnableAdminAccess,
                EnableAggregationLogging = model.EnableAggregationLogging
            };
            Send(command);

            //SessionContext.AggregationLoggingEnabled = command.EnableAggregationLogging;

            return "Security settings have been successfully changed.";
        }
        #endregion

        #region Alerts

        [HttpGet("alerts")]
        public AlertsListModel GetAlerts()
        {
            var user = _userService.GetById(GetUserId());

            var emails = new List<UserEmail> { new UserEmail(user.Email) { IsMain = true } };
            emails.AddRange(user.AdditionalEmails.Select(ae => new UserEmail(ae)));

            var notifications = user.Notifications
                .Select(n => new AlertModel
                {
                    Type = n.Type,
                    TypeData = n.Type.GetData(),
                    SendEmail = n.SendEmail,
                    SendText = n.SendText,
                    BorderValue = n.BorderValue,
                }).ToList();

            return new AlertsListModel { Emails = emails, Phones = user.Phones, Alerts = notifications };
        }

        [HttpPost("emails/add")]
        public IActionResult AddEmail(string email)
        {
            email = (email ?? "").Trim();
            if (!Regex.IsMatch(email, Constants.Validation.Regexps.Email))
                ModelState.AddModelError("Email", "Not valid format.");
            else if (EmailExists(email))
                ModelState.AddModelError("Email", "This email is already added.");

            else
            {
                var command = new User_Email_AddCommand
                {
                    UserId = GetUserId(),
                    Email = email,
                };
                Send(command);
                return new OkResult();
            }
            return new BadRequestObjectResult(ModelState);
        }

        [HttpDelete("emails/{email}")]
        public void RemoveEmail(string email)
        {
            var command = new User_Email_RemoveCommand
            {
                UserId = GetUserId(),
                Email = email,
            };
            Send(command);
        }

        [HttpPost("phones/add")]
        public IActionResult AddPhone(string phone)
        {
            phone = Regex.Replace((phone ?? ""), @"[-\+\(\)\s]", "");
            if (!Regex.IsMatch(phone, @"\d+"))
                ModelState.AddModelError("Phone", "Not valid format.");
            else if (PhoneExists(phone))
                ModelState.AddModelError("Email", "This phone is already added.");
            else
            {
                var command = new User_Phone_AddCommand
                {
                    UserId = GetUserId(),
                    Phone = phone,
                };
                Send(command);
                return new OkResult();
            }
            return new BadRequestObjectResult(ModelState);
        }

        [HttpDelete("phones/{phone}")]
        public void RemovePhone(string phone)
        {
            var command = new User_Phone_RemoveCommand
            {
                UserId = GetUserId(),
                Phone = phone,
            };
            Send(command);
        }

        [HttpPost("alerts/settings")]
        public IActionResult UpdateAlertSettings(EmailTypeEnum type, bool? sendEmail, bool? sendText, int? borderValue)
        {
            if (type == 0)
                ModelState.AddModelError("", "Cannot retrieve alert type.");
            else if (!sendEmail.HasValue && !sendText.HasValue && !borderValue.HasValue)
                ModelState.AddModelError("", "Nothing was changed.");

            else
            {
                var command = new User_Notification_UpdateCommand
                {
                    UserId = GetUserId(),
                    Type = type,
                    SendEmail = sendEmail,
                    SendText = sendText,
                    BorderValue = borderValue,
                };
                Send(command);
                return new OkResult();
            }
            return new BadRequestObjectResult(ModelState);
        }
        #endregion

        private bool PhoneExists(string phone)
        {
            var user = _userService.GetById(GetUserId());
            return user != null && user.Phones.Contains(phone);
        }

        private bool EmailExists(string email)
        {
            var user = _userService.GetById(GetUserId());
            return user != null && (user.Email.Equals(email) || user.AdditionalEmails.Contains(email));
        }

        private ProfileModel GetProfileModel()
        {
            var user = _userService.GetById(GetUserId());

            var securityLevels = ((SecurityLevelEnum[])Enum.GetValues(typeof(SecurityLevelEnum)))
                .Select(sl => new { Type = (int)sl, Title = sl.GetDescription() });

            var model = new ProfileModel
            {
                UserDetails = new UserDetailsModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email,
                    ZipCode = user.ZipCode,
                    BirthDate = user.BirthDate.HasValue ? user.BirthDate.Value.GetFormattedDate(DateTimeFormat.MM_slash_dd_slash_yyyy) : "",
                    Gender = user.Gender,
                },
                SecurityLevel = new SecurityLevelModel
                {
                    SelectedLevel = user.SecurityLevel,
                    SecurityLevels = new SelectList(securityLevels, "Type", "Title", user.SecurityLevel),
                },
                SecurityQuestion = new SecurityQuestionModel
                {
                    Questions = new SelectList(Constants.CommonData.SequrityQuestions),
                    SecurityQuestion = user.PasswordQuestion,
                    Answer = user.PasswordAnswer,
                },
                SecuritySettings = new SecuritySettingsModel
                {
                    EnableAdminAccess = user.Settings.EnableAdminAccess,
                    EnableAggregationLogging = user.Settings.EnableIntuitLogging
                },
            };

            return model;
        }
    }
}