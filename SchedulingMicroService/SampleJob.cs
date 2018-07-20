using NodaTime;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingMicroService
{
    public class SampleJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("The current time is: {0}", SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().ToLocalTime());
            return Task.CompletedTask;
        }
    }
}
