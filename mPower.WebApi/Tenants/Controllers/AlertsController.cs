using System.Collections.Generic;
using System.Linq;
using Default.ViewModel.Areas.Finance.DashboardController;
using mPower.Framework;
using mPower.Framework.Environment.MultiTenancy;
using mPower.TempDocuments.Server.Notifications.DocumentServices;
using mPower.TempDocuments.Server.Notifications.Messages;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace mPower.WebApi.Tenants.Controllers
{
    [Authorize("Pfm")]
    [Route("api/[controller]")]
    public class AlertsController : BaseController
    {
        private readonly DashboardAlertTempService _dashboardAlertService;
        private readonly IEventService _eventService;

        public AlertsController(DashboardAlertTempService dashboardAlertService, IEventService eventService,
            ICommandService command, IApplicationTenant tenant) :base(command, tenant)
        {
            _dashboardAlertService = dashboardAlertService;
            _eventService = eventService;
        }

        [HttpGet("latest")]
        public List<DashboardAlertModel> GetLatestAlerts(string ledgerId = null)
        {
            if (string.IsNullOrEmpty(ledgerId)) ledgerId = GetLedgerId();

            const int alertsLimit = 5;

            var list = _dashboardAlertService.GetLatestAlerts(GetUserId(), ledgerId, alertsLimit);

            return list.Select(alert => new DashboardAlertModel
            {
                Id = alert.Id,
                Date = alert.CreatedDate,
                Text = alert.Text,
            }).ToList();
        }

        [HttpDelete("delete/{id}")]
        public void DeleteAlert(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                ModelState.AddModelError("alertId", "Value of 'alertId' should be specified.");
            }
            else
            {
                var deleteAlertEvent = new AlertsRemovedMessage { Ids = new List<string> { id } };
                _eventService.Send(deleteAlertEvent);
            }
        }
    }
}