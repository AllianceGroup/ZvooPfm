using Quartz;
using Quartz.Impl.Triggers;
using Quartz.Impl;

namespace mPower.Schedule.Server.Environment
{
    public interface IScheduledJob : IJob
    {
        JobDetailImpl ConfigureJob();

        SimpleTriggerImpl ConfigureTrigger();

        bool IsEnabled { get; }
    }
}
