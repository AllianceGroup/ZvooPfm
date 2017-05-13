using System;
using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Finance.BudgetController.Filters;
using Default.ViewModel.Areas.Finance.DashboardController;
using Default.ViewModel.Areas.Finance.DebtToIncomeRatioController;
using Default.ViewModel.Areas.Shared;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Domain.Accounting;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.Framework.Mvc;
using mPower.Framework.Services;
using mPower.TempDocuments.Server.Notifications.Documents.DashboardAlerts;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using mPower.TempDocuments.Server.Notifications.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DashboardViewModel = mPower.WebApi.Tenants.ViewModels.Business.DashboardViewModel;

namespace mPower.WebApi.Tenants.Controllers.Business
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class BusinessDashboardController : BaseController
    {
        private readonly IObjectRepository _objectRepository;
        private readonly DashboardAlertTempService _dashboardAlertService;
        private readonly TransactionsStatisticDocumentService _statisticService;
        private readonly IEventService _eventService;

        public BusinessDashboardController(IObjectRepository objectRepository, DashboardAlertTempService dashboardAlertService, 
            TransactionsStatisticDocumentService statisticService, IEventService eventService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _eventService = eventService;
            _objectRepository = objectRepository;
            _dashboardAlertService = dashboardAlertService;
            _statisticService = statisticService;
        }

        [HttpGet]
        public DashboardViewModel GetDashboardModel()
        {
            var model = new DashboardViewModel
            {
                Entries = _objectRepository.Load<BuildAllEntriesFilter, List<Entry>>(new BuildAllEntriesFilter
                {
                    AffiliateId = Tenant.ApplicationId,
                    UserId = GetUserId(),
                    LedgerId = GetLedgerId(),
                    Paging = new PagingInfo {Take = 5, CurrentPage = 1}
                }),
                Alerts = GetLatestAlerts(),
                Graph = CalcProfitLostStatistic(GetLedgerId())
            };

            return model;
        }

        [HttpGet("deleteAlert/{id}")]
        public IActionResult DeleteAlert(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                var deleteAlertEvent = new AlertsRemovedMessage { Ids = new List<string> { id } };
                _eventService.Send(deleteAlertEvent);

                return new OkResult();
            }

            return new BadRequestResult();
        }

        private List<DashboardAlertModel> GetLatestAlerts()
        {
            const int alertsLimit = 5;

            var list = _dashboardAlertService.GetLatestAlerts(GetUserId(), GetLedgerId(), alertsLimit);

            return list.Select(Map).ToList();
        }

        private static DashboardAlertModel Map(DashboardAlertDocument alert)
        {
            return new DashboardAlertModel
            {
                Id = alert.Id,
                Date = alert.CreatedDate,
                Text = alert.Text,
            };
        }

        private List<ChartItem> CalcProfitLostStatistic(string ledgerId)
        {
            var date = DateTime.Now;
            var statistic = _statisticService.GetProfitLostStatistic(ledgerId, date);

            var result = new List<ChartItem>();
            for (var i = 1; i <= date.Month; i++)
            {
                var income = statistic
                    .Where(s => s.Month == i && s.AccountType == AccountTypeEnum.Income)
                    .Sum(s => AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(s.DebitAmountInCents, s.CreditAmountInCents, AccountTypeEnum.Income));

                var expense = statistic
                    .Where(s => s.Month == i && s.AccountType == AccountTypeEnum.Expense)
                    .Sum(s => AccountingFormatter.FormatDebitCreditToPositiveOrNegativeNumberByAccountType(s.DebitAmountInCents, s.CreditAmountInCents, AccountTypeEnum.Expense));

                var netIncome = AccountingFormatter.CentsToDollars(income) - AccountingFormatter.CentsToDollars(expense);

                result.Add(new ChartItem(Convert.ToInt64(netIncome), $"{i:00}/{date:yy}"));
            }

            return result;
        }
    }
}
