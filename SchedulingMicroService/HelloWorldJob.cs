using Common.Logging;
using JetBrains.Annotations;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchedulingMicroService
{
    public class HelloWorldJob : IJob
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(HelloWorldJob));

        public HelloWorldJob()
        {

        }


        [NotNull]
        public Task Execute([NotNull]IJobExecutionContext context)
        {
            try
            {
                Console.WriteLine("{0}*****{0}Job {1} fired @ {2} next scheduled for {3}{0}***{0}", Environment.NewLine, context.JobDetail?.Key,
                    context.FireTimeUtc.LocalDateTime.ToString("r"),
                    context.NextFireTimeUtc?.ToString("r"));
                Console.WriteLine("{0}***{0}Hello World!{0}***{0}", Environment.NewLine);
            }
            catch(Exception ex)
            {
                Log?.DebugFormat("{0}***{0}Failed: {1}{0}***{0}", Environment.NewLine, ex.Message);
            }
            return Task.CompletedTask;
        }
    }
}
