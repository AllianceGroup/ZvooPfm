using System;
using System.Collections.Generic;
using System.Threading;
using NLog;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using mPower.Framework;

namespace mPower.Schedule.Server.Environment
{
    /// <summary>
    /// The main server logic.
    /// </summary>
    public class QuartzServer : IQuartzServer
    {
        private readonly IEnumerable<IScheduledJob> _jobs;
        private readonly IJobFactory _jobFactory;
        
        private readonly Logger _logger = MPowerLogManager.CurrentLogger;
            
        private ISchedulerFactory _schedulerFactory;
        private IScheduler _scheduler;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuartzServer"/> class.
        /// </summary>
        public QuartzServer(IEnumerable<IScheduledJob> jobs, IJobFactory jobFactory)
        {
            _jobs = jobs;
            _jobFactory = jobFactory;
        }

        /// <summary>
        /// Initializes the instance of the <see cref="QuartzServer"/> class.
        /// </summary>
        public virtual void Initialize()
        {
            try
            {
                _schedulerFactory = CreateSchedulerFactory();
                _scheduler = GetScheduler();
                _scheduler.JobFactory = _jobFactory;
                foreach (var job in _jobs)
                {
                    if (job.IsEnabled)
                        _scheduler.ScheduleJob(job.ConfigureJob(), job.ConfigureTrigger());
                }
            }
            catch (Exception e)
            {
                _logger.Error("Server initialization failed:" + e.Message, e);
                throw;
            }
        }

        /// <summary>
        /// Gets the scheduler with which this server should operate with.
        /// </summary>
        /// <returns></returns>
        protected virtual IScheduler GetScheduler()
        {
            return _schedulerFactory.GetScheduler();
        }

        /// <summary>
        /// Returns the current scheduler instance (usually created in <see cref="Initialize" />
        /// using the <see cref="GetScheduler" /> method).
        /// </summary>
        protected virtual IScheduler Scheduler
        {
            get { return _scheduler; }
        }

        /// <summary>
        /// Creates the scheduler factory that will be the factory
        /// for all schedulers on this instance.
        /// </summary>
        /// <returns></returns>
        protected virtual ISchedulerFactory CreateSchedulerFactory()
        {
            return new StdSchedulerFactory();
        }

        /// <summary>
        /// Starts this instance, delegates to scheduler.
        /// </summary>
        public virtual void Start()
        {
            _scheduler.Start();

            try
            {
                Thread.Sleep(3000);
            }
            catch (ThreadInterruptedException)
            {
            }

            _logger.Info("Scheduler started successfully");
        }

        /// <summary>
        /// Stops this instance, delegates to scheduler.
        /// </summary>
        public virtual void Stop()
        {
            _scheduler.Shutdown(true);
            _logger.Info("Scheduler shutdown complete");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            // no-op for now
        }

        /// <summary>
        /// Pauses all activity in scheudler.
        /// </summary>
        public virtual void Pause()
        {
            _scheduler.PauseAll();
        }

        /// <summary>
        /// Resumes all acitivity in server.
        /// </summary>
        public void Resume()
        {
            _scheduler.ResumeAll();
        }
    }
}
