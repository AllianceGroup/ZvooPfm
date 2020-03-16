using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using Default;
using Default.Factories.Commands.Aggregation;
using Default.ViewModel.Areas.Shared;
using Default.ViewModel.AuthenticationController;
using mPower.Aggregation.Client;
using mPower.Documents.Documents.Membership;
using mPower.Documents.DocumentServices;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Membership;
using mPower.Documents.DocumentServices.Membership.Filters;
using mPower.Documents.Enums;
using mPower.Domain.Membership.Enums;
using mPower.Domain.Membership.User.Commands;
using mPower.Framework;
using mPower.Framework.Environment;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.Framework.Utils;
using mPower.WebApi.Tenants.Helpers;
using mPower.WebApi.Tenants.Services;
using mPower.WebApi.Utilities;
using Microsoft.AspNetCore.Mvc;
using Paralect.Domain;
using mPower.WebApi.Authorization;
using mPower.WebApi.Tenants.ViewModels;
using Microsoft.IdentityModel.Tokens;
using mPower.TempDocuments.Server.Notifications;
using System.Net.Mail;
using mPower.WebApi.Tenants.ViewModels.AffiliateAdmin;
using mPower.TempDocuments.Server.Notifications.Messages;
using System.Web.Configuration;


namespace mPower.WebApi.Tenants.Controllers
{
    [Route("api/[controller]")]
    public class AuthenticationController : Controller
    {
        private readonly TokenAuthOptions _tokenOptions;
        private readonly UserDocumentService _userService;
        private readonly LedgerDocumentService _ledgerService;
        private readonly MembershipService _membershipService;
        private readonly AffiliateDocumentService _affiliateDocumentService;
        private readonly IIdGenerator _idGenerator;
        private readonly IObjectRepository _objectRepository;
        private readonly IAggregationClient _aggregationClient;

        public ICommandService CommandService { get; set; }
        public IApplicationTenant Tenant { get; set; }
        private readonly DashboardAlertBuilder _dashboardAlertBuilder;

        public AuthenticationController(TokenAuthOptions tokenOptions, UserDocumentService userService,
            LedgerDocumentService ledgerService, MembershipService membershipService,
            AffiliateDocumentService affiliateDocumentService, IIdGenerator idGenerator,
            IObjectRepository objectRepository, IAggregationClient aggregationClient,
            ICommandService command, IApplicationTenant tenant)
        {
            _tokenOptions = tokenOptions;
            _userService = userService;
            _ledgerService = ledgerService;
            _membershipService = membershipService;
            _affiliateDocumentService = affiliateDocumentService;
            _idGenerator = idGenerator;
            _objectRepository = objectRepository;
            _aggregationClient = aggregationClient;
            CommandService = command;
            Tenant = tenant;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody]ExtendedLogin model)
        {
            model.JanrainAppUrl = Tenant.JanrainAppUrl;

            var user = _membershipService.Login(model.Email, model.Password);

            if (user == null)
            {
                ModelState.AddModelError("Invalid Credentials", ErrorMessage.InvalidLogin);
                return new BadRequestObjectResult(ModelState);
            }

            if (!user.IsActive)
            {
                var url = PrepareChargifyRedirect(user);
                ModelState.AddModelError("",
                    $@"Your account is inactive. This may be caused by one of following issues: </br>
                                      1. You manually cancelled your subscription in the 'My Profile' section. <br/>
                                      2. Your payment was unsuccessful and you need to renew your subscription. <br/>
                                      3. An administrator has de-activated your account. <br/>
                                     To continue use your account, please create a new subscription <a href='{
                        url}'>here</a> or contact support.");
                return new BadRequestObjectResult(ModelState);
            }

            if (user.SecurityLevel != SecurityLevelEnum.LoginPasswordAndQuestion) return PerformLogin(user);


            if (model.SecurityQuestion.IsNullOrEmpty())
            {
                return new OkObjectResult(new
                {
                    authenticated = false,
                    SecurityQuestion = _membershipService.GetAuthenticationQuestion(user.Id)
                });
            }
            if (!_membershipService.ValidateAuthenticationQuestion(user.Id, model.SecurityQuestionAnswer))
            {
                model.SecurityLevel = SecurityLevelEnum.LoginPasswordAndQuestion;
                model.SecurityQuestion = _membershipService.GetAuthenticationQuestion(user.Id);
                ModelState.AddModelError("Invalid Credentials", ErrorMessage.InvalidAnswer);
                return new BadRequestObjectResult(ModelState);
            }
            return PerformLogin(user);
        }

        [AllianceAuthorize(UserPermissionEnum.GlobalAdminDelete)]
        [HttpGet("loginAsUser/{id}")]
        public IActionResult LoginAsUser(string id)
        {
            var user = _userService.GetById(id);
            if(user == null)
            {
                ModelState.AddModelError("Invalid Credentials", ErrorMessage.InvalidLogin);
                return new BadRequestObjectResult(ModelState);
            }
            if(!user.Settings.EnableAdminAccess)
            {
                ModelState.AddModelError("Invalid operation", ErrorMessage.InvalidOperation);
                return new BadRequestObjectResult(ModelState);
            }

            return PerformLogin(user);
        }

        [HttpGet("producthandle")]
        public string GetSignUpProductNumber()
        {
            var dbHandle = string.Empty;
            try
            {
                dbHandle = _affiliateDocumentService.GetSignUpProduct(Tenant.ApplicationId, true).Handle;
            }
            catch (Exception)
            {
                // ignored
            }
            return dbHandle;
        }

        [HttpGet("GetApiTesting")]
        public string GetApiTesting()
        {
            var dbHandle = "Successfully Called";
            try
            {
                
            }
            catch (Exception)
            {
                // ignored
            }
            return dbHandle;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody]RegisterViewModel model)
        {
            if (_membershipService.GetUserByUsername(model.Email) != null)
            {
                ModelState.AddModelError("Error", "Username is already in use.  Please try another.");
                return new BadRequestObjectResult(ModelState);
            }

            var affiliate = _affiliateDocumentService.GetById(Tenant.ApplicationId);

            var chargifyProductDocument = affiliate.Products.SingleOrDefault(x => x.Handle.Equals(model.ProductHandle));

            if (chargifyProductDocument == null)
            {
                ModelState.AddModelError("Error", "Product not found.");
                return new BadRequestObjectResult(ModelState);
            }

            var createUserCmd = new User_CreateCommand
            {
                ApplicationId = Tenant.ApplicationId,
                UserName = model.Email,
                IsActive = string.IsNullOrEmpty(affiliate.ChargifyUrl),
                CreateDate = DateTime.Now,
                Email = model.Email,
                UserId = _idGenerator.Generate(),
                PasswordHash = SecurityUtil.GetMD5Hash(model.Password),
                ZipCode = model.ZipCode,
                BirthDate = model.BirthDate,
                Gender = model.Gender,
                //ReferralCode = SessionContext.ReferralCode,
                //TODO What is referal code
            };
            Send(createUserCmd.UserId, createUserCmd);

            var createSubscriptionCmd = new User_Subscription_CreateCommand
            {
                UserId = createUserCmd.UserId,
                SubscriptionId = _idGenerator.Generate()
            };
            Send(createUserCmd.UserId, createSubscriptionCmd);

            var user = _userService.GetUserByEmail(createUserCmd.Email);
            return new OkObjectResult(PrepareChargifyRedirect(user));
        }

        [HttpPost("forgotPassword")]
        public IActionResult ForgotPassword(string email)
        {
            string UserToken = "";
            if (string.IsNullOrEmpty(email))
            {
                ModelState.AddModelError("Error", "An email address is required to retrieve a password");
                return new BadRequestObjectResult(ModelState);
            }
            //var user = _userService.GetByFilter(new UserFilter { Email = email, AffiliateId = Tenant.ApplicationId }).SingleOrDefault();
            // While adding users, there is no mandatory field to select Accelerated Family Wealth. So if dropdown is not select the user will not abale to change his password.

            var user = _userService.GetByFilter(new UserFilter { Email = email}).SingleOrDefault();
            if (user == null)
            {
                ModelState.AddModelError("Error", "There are no users with such email!");
                return new BadRequestObjectResult(ModelState);
            }
     
            Send(user.Id, new User_UpdateResetPasswordTokenCommand
            {
                UserId = user.Id,
                Token = UserToken= SecurityUtil.GetUniqueToken()
            });

            sendMail(email, UserToken);
            return new OkResult();
        }

        [HttpPost("resetPassword")]
        public IActionResult ResetPassword([FromBody] ResetPasswordViewModel model)
        {
            if (model.Password != model.ConfirmPassword)
                ModelState.AddModelError("Error", Constants.Validation.Messages.InvalidPasswordConfirmation);

            var user = _userService.GetUserByResetPasswordToken(model.Token);
            if (user == null)
                ModelState.AddModelError("Error", "There is no valid security token");

            if (!ModelState.IsValid) return new BadRequestObjectResult(ModelState);

            Send(user.Id, new User_ResetPasswordCommand
            {
                UserId = user.Id,
                ChangeDate = DateTime.Now,
                PasswordHash = SecurityUtil.GetMD5Hash(model.Password)
            });
            return new OkResult();
        }

        #region Helpers
        private string PrepareChargifyRedirect(UserDocument user)
        {
            var trial = user.Subscriptions.All(x => x.Status == SubscriptionStatusEnum.None);// trial only if no subscriptions
            var chargifyProductDocument = _affiliateDocumentService.GetSignUpProduct(Tenant.ApplicationId, trial);

            string subscriptionId;
            var subscribtions = user.Subscriptions.Where(x => x.Status == SubscriptionStatusEnum.None).ToList();
            //in case if there is precreated empty subscriptions
            if (subscribtions.Count > 0)
                subscriptionId = subscribtions.First().Id;
            else
            {
                var createSubscriptionCmd = new User_Subscription_CreateCommand
                {
                    UserId = user.Id,
                    SubscriptionId = _idGenerator.Generate()
                };
                Send(user.Id, createSubscriptionCmd);

                subscriptionId = createSubscriptionCmd.SubscriptionId;
            }
            return $"{chargifyProductDocument.SignUpPage}?reference={subscriptionId}";
        }

        private IActionResult PerformLogin(UserDocument user)
        {
            if (user.LastAutoUpdateDate.Date != DateTime.Now.Date)
            {
                var ledgers = _ledgerService.GetByUserId(user.Id);
                foreach (var ledger in ledgers)
                {
                    try
                    {
                        var result = _objectRepository.Load<AggregateUserDto, AggregateUserResult>(new AggregateUserDto
                        {
                            UserId = user.Id,
                            LedgerId = ledger.Id,
                            IsAutoUpdate = true,
                        });
                        var aggregationHelper = new AggregationHelper(_aggregationClient, user.Id, ledger.Id);

                        foreach (var command in result.SetStatusCommands)
                            Send(user.Id, command);
                        foreach (var accountId in result.IntuitAccountsIds)
                            aggregationHelper.LaunchPullingTransactions(accountId, ledger.Id);
                        if (result.SetUserAutoUpdateCommand != null)
                        {
                            Send(user.Id, result.SetUserAutoUpdateCommand);
                        }
                    }
                    catch (Exception)
                    {
                        // ignored
                    }
                }
            }

            var ledgerId = _ledgerService.GetPersonal(user.Id).Id;
            var accounts = _objectRepository.Load<string, AccountsSidebarModel>(ledgerId);
            var hasAccounts = accounts.Accounts.Any(e => e.IsAggregatedAccount) ||
                              accounts.Loans.Any(e => e.IsAggregatedAccount) ||
                              accounts.CreditCards.Any(e => e.IsAggregatedAccount) ||
                              accounts.Investments.Any(e => e.IsAggregatedAccount);
            var identity = new ClaimsIdentity(new GenericIdentity(user.Id, "TokenAuth"), new[] {
                    new Claim("LedgerId", ledgerId)
                });

            identity.AddClaims(user.Permissions.Select(x => new Claim(ClaimTypes.Role, ((int)x).ToString())));

            DateTime? expires = DateTime.UtcNow.AddHours(2);
            var token = GetToken(identity, expires);
            return
                new OkObjectResult(
                    new { token, tokenExpires = expires, authenticated = true, ledgerId, hasAccounts, IsUserCreatedByAgent= user.IsCreatedByAgent,
                        FullName = user.FullName
                    });
        }

        private string GetToken(ClaimsIdentity identity, DateTime? expires)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenData = new SecurityTokenDescriptor
            {
                Issuer = _tokenOptions.Issuer,
                Audience = _tokenOptions.Audience,
                SigningCredentials = _tokenOptions.SigningCredentials,
                Subject = identity,
                Expires = expires
            };
            var securityToken = handler.CreateToken(tokenData);

            return handler.WriteToken(securityToken);
        }

        private void Send(string userId, params ICommand[] commands)
        {
            foreach (var command in commands)
            {
                command.Metadata.UserId = userId;
            }
            CommandService.Send(commands);
        }

        private void sendMail(string Email, string Token)
        {
            SendMailViewModel model = new SendMailViewModel();
            string refURL= "";
            var affiliate = _affiliateDocumentService.GetById(Tenant.ApplicationId);
            var subj = affiliate.EmailContents.Where(x => x.IsDefaultForEmailType == Domain.Application.Enums.EmailTypeEnum.ForgotPassword).FirstOrDefault();
            var smtp = affiliate.CreateSmptClient();
            SendMailMessage message = new SendMailMessage();
            message.IsBodyHtml = true;
            
            message.Subject = subj.Name;

            string url = HttpContext.Request.Host.Host;

            if(url.Contains("localhost"))
            {
                refURL = System.Configuration.ConfigurationManager.AppSettings["APIURLLocal"] + "resetPassword?token="+Token+"";
            }
            else if(url.Contains("staging"))
            {
                refURL = System.Configuration.ConfigurationManager.AppSettings["APIURLStaging"] + "resetPassword?token=" + Token + "";
            }
            else if(url.Contains("acceleratedfamilywealth"))
            {
                refURL = System.Configuration.ConfigurationManager.AppSettings["APIURLProduction"] + "resetPassword?token=" + Token + "";
            }
            message.Body = subj.Html.Replace("{{reset_password_link}}", "<a href='"+ refURL + "'>Reset Password</a>");
            using (var mailMessage = new MailMessage { Subject = message.Subject, Body = message.Body, IsBodyHtml = message.IsBodyHtml })
            {
                mailMessage.From = new MailAddress(affiliate.Smtp.CredentialsEmail, affiliate.DisplayName);

                    mailMessage.To.Add(Email);
                    smtp.Send(mailMessage);
            }
        }
        #endregion
    }
}