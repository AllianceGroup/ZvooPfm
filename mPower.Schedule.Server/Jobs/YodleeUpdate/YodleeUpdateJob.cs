using System;
using mPower.Documents;
using mPower.Schedule.Server.Environment;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Triggers;

namespace mPower.Schedule.Server.Jobs.YodleeUpdate
{
    public class YodleeUpdateJob : IScheduledJob
    {
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();
        private static void LogInfo(string message)
        {
            _logger.Log(LogLevel.Info, "YodleeUpdateJob: " + message);
        }

        private readonly YodleeBulkOperations _yodleeBulkOperations;

        public YodleeUpdateJob(YodleeBulkOperations yodleeBulkOperations)
        {
            _yodleeBulkOperations = yodleeBulkOperations;
        }

        public void Execute(IJobExecutionContext context)
        {
            LogInfo("Yodlee Update Job Starting via Quartz");

            _yodleeBulkOperations.StartRefreshForAllContentServiceItems();
            _yodleeBulkOperations.DownloadAllContentServiceItemData();
            _yodleeBulkOperations.UpdateAllLedgers();

            LogInfo("Yodlee Update Job Completed via Quartz");
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("Yodlee updater job", GetType());
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("Yodlee Each 8 hours", Int32.MaxValue, TimeSpan.FromHours(12));
        }

        public bool IsEnabled
        {
            get { return true; }
        }
    }
}
