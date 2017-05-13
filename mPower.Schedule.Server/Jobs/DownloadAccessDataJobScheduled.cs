 using System;
 using Quartz;
 using Quartz.Impl;
using Quartz.Impl.Triggers;
using mPower.OfferingsSystem;
using mPower.OfferingsSystem.Sheduler;
using mPower.Schedule.Server.Environment;
using mPower.TempDocuments.Server.DocumentServices;

namespace mPower.Schedule.Server.Jobs
{
    public class DownloadAccessDataJobScheduled : IScheduledJob
    {
        private readonly DownloadAccessDataJob _job;

        public DownloadAccessDataJobScheduled(DownloadAccessDataJob job)
        {
            _job = job;
        }

        public JobDetailImpl ConfigureJob()
        {
            return new JobDetailImpl("DownloadAccessDataJobOncePerDay", GetType());
        }

        public SimpleTriggerImpl ConfigureTrigger()
        {
            return new SimpleTriggerImpl("DownloadAccessDataJobOncePerDay", new DateTimeOffset(DateTime.UtcNow.Date.AddHours(8 + 6)), null, Int32.MaxValue, TimeSpan.FromHours(24)); // every day at 7 MTD (UTC -6)
        }

        public bool IsEnabled
        {
            get { return true; }
        }

        public void Execute(IJobExecutionContext context)
        {
            _job.Execute();
        }
    }
}