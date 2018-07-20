using Base_MicroService;
using CommonMessages;
using JetBrains.Annotations;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace SchedulingMicroService
{
    public class SchedulingMicroService : BaseMicroService<SchedulingMicroService, HealthStatusMessage>
    {
        /// <summary>
        /// The job scheduler
        /// </summary>
        private readonly IScheduler _jobScheduler;

        /// <summary>
        /// The logger
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// The hc
        /// </summary>
        private HostControl _hc;

        /// <summary>
        /// Initializes a new instance of the MicroServiceEcoSystem.SchedulingMicoService class
        /// </summary>
        public SchedulingMicroService()
        {
            Name = "Scheduling Microservice_" + Environment.MachineName;
        }

        /// <summary>
        /// Initializes a new instance of the MicroServiceEcoSystem.SchedulingMicoService class.
        /// </summary>
        /// <param name="jobScheduler">The job scheduler</param>
        /// <param name="logger">The logger</param>

        public SchedulingMicroService([NotNull] IScheduler jobScheduler, [NotNull] ILogger logger) : base()
        {
            _jobScheduler = jobScheduler;
            _logger = logger;
        }

        /// <summary>
        /// Executes the start action.
        /// </summary>
        /// <param name="host">The host. This may be null.</param>
        /// <returns>True if it succeeds, false if it fails</returns>
        public new bool OnStart([CanBeNull] HostControl host)
        {
            base.Start(host);
            _hc = host;
            _jobScheduler?.Start();
            //_logger?.SendInformation(string.Intern("Job Scheduler started"));
            //construct a scheduler factory
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            // get a scheduler
            IScheduler sched = schedFact.GetScheduler().Result;
            sched.Start();
            // define the job and tie it to our HelloJob class
            IJobDetail job = JobBuilder.Create<SampleJob>()
                .WithIdentity("myJob", "group1")
                .Build();
            // Trigger the job to run now, and then every 40 seconds
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                .WithSimpleSchedule(x => x.WithIntervalInSeconds(40).RepeatForever())
                .Build();
            // Schedule the job
            sched.ScheduleJob(job, trigger);
            return true;

        }

        public new bool OnStop()
        {
            base.Stop();
            _jobScheduler?.Shutdown(true);
            return true;
        }

        public new bool Continue()
        {
            return true;
        }

        public new bool Pause()
        {
            return true;
        }

        public new bool Resume()
        {
            return true;
        }

        public new bool OnResume()
        {
            return true;
        }

        public new bool OnPause()
        {
            return true;
        }

        public new bool OnContinue()
        {
            return true;
        }

        public new bool OnShutdown()
        {
            return true;
        }
    }
}
