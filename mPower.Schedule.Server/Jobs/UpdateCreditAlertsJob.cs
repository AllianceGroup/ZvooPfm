using System;
using System.Linq;
using mPower.Documents.DocumentServices.Credit;
using mPower.Domain.Accounting.CreditIdentity.Commands;
using mPower.Domain.Accounting.CreditIdentity.Data;
using mPower.Domain.Accounting.Enums;
using mPower.Framework;
using mPower.Schedule.Server.Environment;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using TransUnionWrapper;
using TransUnionWrapper.Enums;
using TransUnionWrapper.Model;

namespace mPower.Schedule.Server.Jobs
{
    public class UpdateCreditAlertsJob : IScheduledJob
    {
        private readonly ITransUnionService _transUnionService;
        private readonly CommandService _commandService;
        private readonly CreditIdentityDocumentService _creditIdentityDocumentService;

        public UpdateCreditAlertsJob(ITransUnionService transUnionService, 
                                    CommandService commandService, 
                                    CreditIdentityDocumentService creditIdentityDocumentService)
        {
            _transUnionService = transUnionService;
            _commandService = commandService;
            _creditIdentityDocumentService = creditIdentityDocumentService;
        }

        public void Execute(IJobExecutionContext context)
        {
            var alertNotifications = _transUnionService.GetSubscribedUsersAlerts();

            foreach (AlertNotification alertNotification in alertNotifications)
            {
                var creditIdentity = _creditIdentityDocumentService.GetById(alertNotification.ClientKey);
                if (creditIdentity != null)
                {
                    var latestAlert = creditIdentity.Alerts.OrderByDescending(x => x.Date).FirstOrDefault();
                    var lastDate = latestAlert != null ? latestAlert.Date : DateTime.MinValue;
                    var alerts = _transUnionService.GetAlerts(lastDate, DateTime.Now, alertNotification.ClientKey);
                    var command = new CreditIdentity_Alerts_AddCommand();
                    command.ClientKey = alertNotification.ClientKey;
                    command.Alerts = alerts.Select(x => new AlertData() { Date = x.Date, Type = MapAlertType(x.Type), Message = x.Message }).ToList();
                    command.Metadata.UserId = "CreditAlertsUpdater";
                    _commandService.Send(command);
                }
            }
        }

        private AlertTypeEnum MapAlertType(AlertTypes type)
        {
            var result = AlertTypeEnum.Account;

            Enum.TryParse(type.ToString(), out result);

            return result;
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("Update credit alerts", GetType());
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("CreditAlertsUpdateEach8Hours", Int32.MaxValue, TimeSpan.FromHours(8));
        }

        public bool IsEnabled
        {
            get { return true; }
        }
    }
}
