using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Default.Factories.Commands.Aggregation;
using Default.Factories.ViewModels.Aggregation;
using Default.Helpers;
using Default.Models;
using Default.ViewModel.AccountsController;
using Default.ViewModel.GettingStartedController;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Data;
using mPower.Aggregation.Contract.Exceptions;
using mPower.Domain.Accounting;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.Framework.Utils.Extensions;
using mPower.WebApi.Helpers;
using mPower.WebApi.Tenants.ViewModels;
using mPower.WebApi.Tenants.ViewModels.Aggregation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AggregationHelper = mPower.WebApi.Tenants.Helpers.AggregationHelper;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class AggregationController : BaseController
    {
        private readonly IAggregationClient _aggregationClient;
        private readonly IObjectRepository _objectRepository;

        public AggregationController(IAggregationClient aggregationClient, IObjectRepository objectRepository,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _aggregationClient = aggregationClient;
            _objectRepository = objectRepository;
        }

        [HttpGet("search/{searchText}")]
        public SearchInstitutionsResultPartialViewModel SearchIntuitions(string searchText)
        {
            return _objectRepository.Load<string, SearchInstitutionsResultPartialViewModel>(searchText);
        }

        [HttpPost("authenticate/{id}")]
        public IActionResult AuthenticateToInstitution(long id)
        {
            try
            {
                return new OkObjectResult(_objectRepository.Load<AuthenticateDto, AuthenticateToInstitutionViewModel>(new AuthenticateDto
                {
                    UserId = GetUserId(),
                    InstitutionId = id,
                    AggregationLoggingEnabled = GetAggregationLoggingEnabled(),
                }));
            }
            catch (AggregationException error)
            {
                ModelState.AddModelError(string.Empty, error.GetErrorMessage());
                return new BadRequestObjectResult(ModelState);
            }
        }
        

        [HttpPost("authenticateToBank")]
        public IActionResult AuthenticateToInstitution([FromBody]AuthenticateToInstitutVewModel model)
        {
            var aggregationHelper = new AggregationHelper(_aggregationClient, GetUserId(), GetLedgerId());

            #region Discover accounts and mfa

            DiscoverAccountsResponse discoveryResult;
            DiscoverySession discoveryKeys;

            if (model.MfaSession == null)
            {
                try
                {
                    discoveryResult = aggregationHelper.DiscoverAccounts(model.Dto.InstitutionId, model.Dto.Keys);
                }
                catch (AggregationException error)
                {
                    ModelState.AddModelError(string.Empty, error.GetErrorMessage());
                    return new BadRequestObjectResult(new {Errors = ModelState.AsEnumerable().SelectMany(x => x.Value.Errors)});
                }
                discoveryKeys = new DiscoverySession
                {
                    ContentServiceId = model.Dto.InstitutionId,
                    Keys = model.Dto.Keys,
                    Accounts = discoveryResult.Accounts
                };
            }
            else //handle mfa questions
            {
                if (!CheckForMissedAnswers(model.MfaSession.Questions.Select(q=>q.Answer).ToArray()))
                    return new BadRequestObjectResult(new { Errors = ModelState.AsEnumerable().SelectMany(x => x.Value.Errors) });

                try
                {
                    discoveryResult = aggregationHelper.DiscoverAccounts(model.Dto.InstitutionId, model.MfaSession);
                }
                catch (AggregationException error)
                {
                    ModelState.AddModelError(string.Empty, error.GetErrorMessage());
                    return new BadRequestObjectResult(new { Errors = ModelState.AsEnumerable().SelectMany(x => x.Value.Errors) });
                }

                discoveryKeys = new DiscoverySession
                {
                    ContentServiceId = model.Dto.InstitutionId,
                    Keys = model.Dto.Keys,
                    Accounts = discoveryResult.Accounts
                };
            }

            if (discoveryResult.Session != null)
            {
                model.MfaSession = discoveryResult.Session;
                return new OkObjectResult(new
                {
                    model.MfaSession
                });
            }

            #endregion

            #region Handle discover accounts errors
            #endregion

            // user is aggregating new account -- show him a list of available accounts
            //if (!model.Dto.accountId.HasValue)
            //{
                var assignDto = new AssignDto
                {
                    UserId = GetUserId(),
                    LedgerId = GetLedgerId(),
                    Accounts = discoveryKeys.Accounts,
                    InstitutionId = discoveryKeys.ContentServiceId,
                };

                var assignModel =
                    _objectRepository.Load<AssignDto, AssignAccountsToLedgerViewModel>(assignDto);

            return new OkObjectResult(new AccountsToLedgerViewModel
            {
                AssignModel = assignModel,
                DiscoverySession = discoveryKeys
            });
            //}

            #region update account

            //// user is re-authenticating account
            //var intuitAccountId = model.Dto.accountId.Value;

            //// _aggregation.UpdateAccount(intuitAccountId, DiscoveryKeys.Keys, true);

            //var result = _objectRepository.Load<RefreshAccountDto, RefreshAccountResult>(new RefreshAccountDto
            //{
            //    UserId = GetUserId(),
            //    LedgerId = GetUserId(),
            //    IntuitAccountId = intuitAccountId,
            //});

            //if (result.SetStatusCommand != null)
            //{
            //    Send(result.SetStatusCommand);
            //}
            //if (result.PullTransactions)
            //{
            //    _aggregationHelper.LaunchPullingTransactions(intuitAccountId, GetLedgerId());
            //}

            //return RedirectToAction("Completed", "Aggregation", new { aggAction = AggregationActionEnum.Reauthentication });

            #endregion
        }

        [HttpPost("aligntoledger")]
        public IActionResult AssignContentServiceAccountsToLedger([FromBody]AccountsToLedgerViewModel accountsToLedgerView)
        {
            var aggregationHelper = new AggregationHelper(_aggregationClient, GetUserId(), GetLedgerId());
            var discoverySession = accountsToLedgerView.DiscoverySession;
            var postModel = accountsToLedgerView.AssignModel;

            #region Validation

            if(!postModel.IsValid())
            {
                if(postModel.Accounts.All(x => !x.Selected))
                {
                    ModelState.AddModelError("Error", "Please choose at least one account.");
                }
                else if(postModel.Accounts.Any(x => !x.IsValid()))
                {
                    ModelState.AddModelError("Error", "Please choose an account type for all selected accounts.");
                }

                return new BadRequestObjectResult(ModelState);
            }

            #endregion

            var aggregateDto = new AggregateAccountsDto
            {
                DiscoverySession = discoverySession,
                LedgerId = GetLedgerId(),
                UserId = GetUserId(),
                AggregationLoggingEnabled = true,
                Model = postModel,
            };

            var result = _objectRepository.Load<AggregateAccountsDto, AggregateAccountsResult>(aggregateDto);

            foreach(var cmd in result.AddAccountCommands)
                Send(cmd);
            foreach(var cmd in result.UpdateBalanceCommands)
                Send(cmd);
            foreach(var cmd in result.SetStatusCommands)
                Send(cmd);

            aggregationHelper.RefreshAccountsInteractively(result.IntuitAccountsIds[0]);
            foreach(var accountId in result.IntuitAccountsIds)
                aggregationHelper.LaunchPullingTransactions(accountId, GetLedgerId(), true);

            return new OkObjectResult(result.AddAccountCommands.Select(x =>
                new ShortAccountViewModel
                {
                    AccountId = x.AccountId,
                    LedgerId = x.LedgerId,
                    Name = x.Name,
                    Type = x.AccountTypeEnum.GetIifName(),
                    Group = AccountingFormatter.GenericCategoryGroup(x.AccountLabelEnum)
                }
            ));
        }

        [HttpPost("aggregateuser")]
        public void AggregateUser()
        {
            var dto = new AggregateUserDto
            {
                LedgerId = GetLedgerId(),
                UserId = GetUserId()
            };
            var result = _objectRepository.Load<AggregateUserDto, AggregateUserResult>(dto);
            var aggregationHelper = new AggregationHelper(_aggregationClient, GetUserId(), GetLedgerId());

            foreach (var command in result.SetStatusCommands)
                Send(command);
            foreach (var accountId in result.IntuitAccountsIds)
                aggregationHelper.LaunchPullingTransactions(accountId, GetLedgerId());
        }

        [HttpPost("reathenticateGetLogonForm")]
        public IActionResult ReathenticateGetLogonForm(long intuitInstitutionId, long intuitAccountId)
        {
            try
            {
                return new OkObjectResult(_objectRepository.Load<ReauthenticateDto, ReauthenticateToInstitutionViewModel>(new ReauthenticateDto
                {
                    UserId = GetUserId(),
                    InstitutionId = intuitInstitutionId,
                    AggregationLoggingEnabled = GetAggregationLoggingEnabled(),
                    IntuitAccountId = intuitAccountId
                }));
            }
            catch (AggregationException error)
            {
                ModelState.AddModelError(string.Empty, error.GetErrorMessage());
                return new BadRequestObjectResult(ModelState);
            }
        }

        [HttpPost("reauthentication")]
        public IActionResult Reauthentication([FromBody]ReauthenticateToInstitutVewModel model)
        {
            var aggregationHelper = new AggregationHelper(_aggregationClient, GetUserId(), GetLedgerId());
            try
            {
                aggregationHelper.UpdateAccountCredentials(model.IntuitAccountId, model.Dto.Keys);
                aggregationHelper.RefreshAccountsInteractively(model.IntuitAccountId);
                aggregationHelper.LaunchPullingTransactions(model.IntuitAccountId, GetLedgerId(), true);
                return Ok();
            }
            catch (AggregationException error)
            {
                ModelState.AddModelError(string.Empty, error.GetErrorMessage());
                return new BadRequestObjectResult(new { Errors = ModelState.AsEnumerable().SelectMany(x => x.Value.Errors) });
            }

            
        }

        [HttpPost("interactiveRefresh/{intuitAccountId}")]
        public IActionResult InteractiveRefresh(long intuitAccountId)
        {
            var aggregationHelper = new AggregationHelper(_aggregationClient, GetUserId(), GetLedgerId());                      
            try
            {
                var refreshResult = aggregationHelper.RefreshAccountsInteractively(intuitAccountId);

                if (refreshResult.Session != null)
                    return new OkObjectResult(new RefreshAccountViewModel()
                    {
                        FinicityAccountId = intuitAccountId,
                        MfaSession = refreshResult.Session
                    });
                IEnumerable<ModelError> errorMessage = new List<ModelError> {new ModelError("No MfaSession") };
                return new BadRequestObjectResult(new { Errors = errorMessage });
            }
            catch (AggregationException error)
            {
                ModelState.AddModelError(string.Empty, error.GetErrorMessage());
                return new BadRequestObjectResult(new { Errors = ModelState.AsEnumerable().SelectMany(x => x.Value.Errors) });
            }
            
        }

        [HttpPost("interactiveRefreshMfa")]
        public IActionResult InteractiveRefreshMfa([FromBody]RefreshAccountViewModel model)
        {
            var aggregationHelper = new AggregationHelper(_aggregationClient, GetUserId(), GetLedgerId());
            try
            {
                var refreshResult = aggregationHelper.RefreshAccountsInteractively(model.FinicityAccountId, model.MfaSession);

                if (refreshResult.Session != null)
                    return new OkObjectResult(refreshResult.Session);

                foreach (var accountId in refreshResult.Accounts.Select(a => a.FinicityAccountId))
                {
                    aggregationHelper.LaunchPullingTransactions(accountId, GetLedgerId());
                }

                return Ok();
            }
            catch (AggregationException error)
            {
                ModelState.AddModelError(string.Empty, error.GetErrorMessage());
                return new BadRequestObjectResult(new { Errors = ModelState.AsEnumerable().SelectMany(x => x.Value.Errors) });
            }
        }

        #region helpers
        private bool CheckForMissedAnswers(string[] answers)
        {
            var indexOfEmptyAnswer = -1;
            for (var i = 0; i < answers.Length; i++)
            {
                if (answers[i].IsNullOrEmpty())
                {
                    indexOfEmptyAnswer = i;
                    break;
                }
            }

            if (indexOfEmptyAnswer >= 0)
            {
                ModelState.AddModelError($"[{indexOfEmptyAnswer}]",
                    "All answers are required. You've missed answer for question #" + (indexOfEmptyAnswer + 1));

                return false;
            }

            return true;
        }

        //private IList<AggregationQuestion> PrepareForDisplaying(IEnumerable<AggregationQuestion> questions)
        //{
        //    if (questions == null)
        //    {
        //        return null;
        //    }

        //    var result = (AggregationQuestion[])questions.ToArray().Clone();

        //    foreach (var question in result)
        //    {
        //        var items = question.Items;
        //        if (items != null)
        //        {
        //            for (var i = 0; i < items.Length; i++)
        //            {
        //                var item = items[i];
        //                if (item is byte[])
        //                {
        //                    var imageId = Guid.NewGuid();
        //                    items[i] = imageId;
        //                }
        //            }
        //        }
        //    }

        //    return result;
        //}
    #endregion 
    }
}