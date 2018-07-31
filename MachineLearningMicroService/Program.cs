using Autofac;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Autofac;
using Topshelf.Diagnostics;
using Topshelf.Leader;

namespace MachineLearningMicroService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            // Service itself
            builder.RegisterType<MLMicroService>().AsImplementedInterfaces()
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
                c?.UseWindowsHostEnvironmentWithDebugSupport();
                c?.Service<MLMicroService>(s =>
                {
                    s.ConstructUsingAutofacContainer<MLMicroService>();
                    s?.ConstructUsing(settings =>
                    {
                        var service = AutofacHostBuilderConfigurator.LifetimeScope.Resolve<MLMicroService>();
                        return service;
                    });
                    s?.ConstructUsing(name => new MLMicroService());
                    s?.WhenStartedAsLeader(b =>
                    {
                        b.WhenStarted(async (service, token) =>
                        {
                            await service.Start(token);
                        });
                        b.Lease(lcb => lcb.RenewLeaseEvery(TimeSpan.FromSeconds(2))
                        .AquireLeaseEvery(TimeSpan.FromSeconds(5))
                        .LeaseLength(TimeSpan.FromDays(1))
                        .WithLeaseManager(new MLMicroService()));

                        b.WithHeartBeat(TimeSpan.FromSeconds(30), (isLeader, token) => Task.CompletedTask);
                        b.Build();
                    });
                    s?.WhenStarted((MLMicroService server, HostControl host) => server.OnStart(host));
                    s?.WhenPaused(server => server.OnPause());
                    s?.WhenContinued(server => server.OnResume());
                    s?.WhenStopped(server => server.OnStop());
                    s?.WhenShutdown(server => server.OnShutdown());
                });
                c?.RunAsNetworkService();
                c?.StartAutomaticallyDelayed();
                c?.SetDescription(string.Intern("Machine Learning Microservice Sample"));
                c?.SetDisplayName(string.Intern("MachineLearningMicroservice"));
                c?.SetServiceName(string.Intern("MachineLearningMicroservice"));
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
