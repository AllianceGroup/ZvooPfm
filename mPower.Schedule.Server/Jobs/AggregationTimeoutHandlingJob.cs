using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NLog;
using Paralect.ServiceBus;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract;
using mPower.Documents.DocumentServices.Accounting;
using mPower.Documents.DocumentServices.Accounting.Filters;
using mPower.Domain.Accounting.Enums;
using mPower.Domain.Accounting.Ledger.Commands;
using mPower.Framework;
using mPower.Schedule.Server.Environment;

namespace mPower.Schedule.Server.Jobs
{
    public class AggregationTimeoutHandlingJob : IScheduledJob
    {
        private readonly LedgerDocumentService _ledgerService;
        private readonly IServiceBus _serviceBus;
        private readonly Logger _logger = MPowerLogManager.CurrentLogger;

        public AggregationTimeoutHandlingJob(LedgerDocumentService ledgerService,
            IServiceBus serviceBus)
        {
            _ledgerService = ledgerService;
            _serviceBus = serviceBus;
        }

        public void Execute(IJobExecutionContext context)
        {
            var borderDate = DateTime.Now.AddMinutes(-30);
            var accountFilter = new AccountFilter{
                                                       AggregationStartedDateLessThan = borderDate,
                                                       AggregationsStatusIn =
                                                           new List<AggregatedAccountStatusEnum>(new[]
                                                                        {
                                                                            AggregatedAccountStatusEnum.BeginPullingTransactions,
                                                                            AggregatedAccountStatusEnum.PullingTransactions
                                                                        })
                                                   };
            var ledgers =  _ledgerService.GetByFilter(new LedgerFilter{AccountFilter = accountFilter});

            _logger.Info("COMMAND START COUNT");
            var commands = ledgers.Select(led =>
                {
                    var ledger = led;
                    var accounts = ledger.Accounts.Where(
                    x =>
                    x.AggregationStartedDate < accountFilter.AggregationStartedDateLessThan.Value &&
                    accountFilter.AggregationsStatusIn.Contains(x.AggregatedAccountStatus));
                    return accounts.Select(account =>
                                           new Ledger_Account_AggregationStatus_UpdateCommand
                                               {
                                                   NewStatus = AggregatedAccountStatusEnum.TimeoutTerminated,
                                                   Date = DateTime.Now,
                                                   AccountId = account.Id,
                                                   LedgerId = ledger.Id
                                               });
                }).SelectMany(x=> x);
            _logger.Info("COMMAND COUNT:" + commands.ToArray().Length);
            _serviceBus.Send(commands.ToArray());
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("Reset statuses for aggregated accounts by timeout", GetType()); 
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("AggregationTimeoutHandlingJobTrigger", Int32.MaxValue, TimeSpan.FromMinutes(30)); ;
        }

        public bool IsEnabled
        {
            get { return true; }
        }
    }
}