using Autofac;
using log4net.Config;
using log4net.Repository.Hierarchy;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Autofac;
using Topshelf.Diagnostics;
using Topshelf.Quartz;

namespace SchedulingMicroService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            // Service itself
            builder.RegisterType<SchedulingMicroService>()
                .AsImplementedInterfaces()
                .AsSelf()
                ?.InstancePerLifetimeScope();
            builder.RegisterType<Logger>().SingleInstance();
            var container = builder.Build();
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(@".log4net.config"));
            HostFactory.Run(c =>
            {
                c?.UseAutofacContainer(container);
                c?.UseLog4Net();
                c?.ApplyCommandLineWithDebuggerSupport();
                c?.EnablePauseAndContinue();
                c?.EnableShutdown();
                c?.OnException(ex => { Console.WriteLine(ex.Message); });

                // Here is the main difference in this Microservice. We are going to run the 
                // Quartz scheduler as a microservice. We create our sample job, have it run
                // every 30 seconds until we stop the microservice.

                c.ScheduleQuartzJobAsService(q =>
                q.WithJob(() => JobBuilder.Create<SampleJob>().Build())
                .AddTrigger(() => TriggerBuilder.Create().WithSimpleSchedule(
                    build => build.WithIntervalInSeconds(30).RepeatForever()).Build())
                    ).StartAutomatically();
                c?.Service<SchedulingMicroService>(s =>
                {
                    s.ConstructUsingAutofacContainer<SchedulingMicroService>();
                    s?.ConstructUsing(settings =>
                    {
                        var service = AutofacHostBuilderConfigurator.LifetimeScope.Resolve<SchedulingMicroService>();
                        return service;
                    });
                    s?.ConstructUsing(name => new SchedulingMicroService());
                    s?.WhenStarted((SchedulingMicroService server, HostControl host) => server.OnStart(host));
                    s?.WhenPaused(server => server.OnPause());
                    s?.WhenContinued(server => server.OnResume());
                    s?.WhenStopped(server => server.OnStop());
                    s?.WhenShutdown(server => server.OnShutdown());
                });
                c?.RunAsNetworkService();
                c?.StartAutomaticallyDelayed();
                c?.SetDescription(string.Intern("Scheduling Microservice Sample"));
                c?.SetDisplayName(string.Intern("SchedulingMicroservice"));
                c?.SetServiceName(string.Intern("SchedulingMicroService"));
                c?.EnableServiceRecovery(r =>
                {
                    r?.OnCrashOnly();
                    r?.RestartService(1); // first
                    r?.RestartService(1); // second
                    r?.RestartService(1); // subsequents
                    r?.SetResetPeriod(0);
                });
            });
        }
    }
}
