using Default.Factories.Commands.Aggregation;
using mPower.Aggregation.Client;
using mPower.Aggregation.Contract.Data;
using mPower.Documents.DocumentServices.Membership;
using mPower.Framework;
using mPower.Framework.Mvc;
using mPower.Schedule.Server.Environment;
using NLog;
using Paralect.ServiceBus;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;
using System;
using System.Threading.Tasks;

namespace mPower.Schedule.Server.Jobs
{
    public class PullIntuitDataJob : IScheduledJob
    {

        private readonly IAggregationClient _aggregation;
        private readonly IServiceBus _serviceBus;
        private readonly Logger _logger = MPowerLogManager.CurrentLogger;
        private readonly IObjectRepository _objectRepository;
        private readonly UserDocumentService _userService;

        public PullIntuitDataJob(IAggregationClient aggregation, IServiceBus serviceBus, IObjectRepository objectRepository, UserDocumentService userService)
        {
            _aggregation = aggregation;
            _serviceBus = serviceBus;
            _objectRepository = objectRepository;
            _userService = userService;
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("Pull new data from Intuit", GetType());
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("PullIntuitdataTwicePerDay", new DateTimeOffset(DateTime.UtcNow.Date.AddHours(7 + 6)), null, Int32.MaxValue, TimeSpan.FromHours(24)); // every day at 7 MTD (UTC -6)
        }

        public bool IsEnabled
        {
            get { return true; }
        }

        /// <summary>
        /// Aggregated balance of any account will be updates when transaction come!
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            _logger.Info("Pull intuit data job was started");
            var users = _aggregation.GetAllIntuitUsers();
            Parallel.ForEach(users, userDocument =>
            {
                try
                {
                    var user = _userService.GetById(userDocument.LogonId);
                    var meta = new Metadata { LogonId = userDocument.LogonId, IsLoggingEnabled = user.Settings.EnableIntuitLogging };
                    var accounts = _aggregation.GetAccountsByLogonId(meta, userDocument.LogonId);
                    foreach (var acc in accounts)
                    {
                        _logger.Info("Pulling data for userId: [{0}], intuit accountId: [{1}]", userDocument.Id, acc.FinicityAccountId);
                        try
                        {
                            UpdateAccountData(meta, userDocument.LogonId, acc.LedgerId, acc.FinicityAccountId);
                        }
                        catch (Exception ex)
                        {
                            _logger.Info("Pulling data failed for userId: [{0}], intuit accountId: [{1}]. Error: {2}.", userDocument.Id, acc.FinicityAccountId, ex.Message);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Info("Pulling data failed for userId: [{0}]. Error: {1}.", userDocument.Id, ex.Message);
                }
            });
            _logger.Info("Pull intuit data job was finished");
        }

        private void UpdateAccountData(Metadata meta, string userId, string ledgerId, long intuitAccountId)
        {
            var result = _objectRepository.Load<RefreshAccountDto, RefreshAccountResult>(new RefreshAccountDto
            {
                UserId = userId,
                LedgerId = ledgerId,
                IntuitAccountId = intuitAccountId,
            });

            if (result.SetStatusCommand != null)
            {
                _serviceBus.Send(result.SetStatusCommand);
            }

            if (result.PullTransactions)
            {
                _aggregation.LaunchPullingTransactions(meta, intuitAccountId, ledgerId, userId, false);
            }
        }
    }
}
