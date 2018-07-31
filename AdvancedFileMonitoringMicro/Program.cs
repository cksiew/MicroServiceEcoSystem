using Autofac;
using Base_MicroService;
using Grumpy.ServiceBase;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Autofac;

namespace AdvancedFileMonitoringMicro
{
    class Program
    {
        static void Main(string[] args)
        {
            TopshelfUtility.Run<Microservice>();
        }
        //static void Main(string[] args)
        //{
        //    Console.WindowWidth = 130;
        //    var builder = new ContainerBuilder();
        //    // Service itself
        //    builder.RegisterType<MSBaseLogger>()?.SingleInstance();
        //    builder.RegisterType<Microservice>().AsImplementedInterfaces().AsSelf()
        //        ?.InstancePerLifetimeScope();
        //    _container = builder.Build();
        //    XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(@".log4net.config"));
        //    HostFactory.Run(c =>
        //    {
        //        c?.UseAutofacContainer(_container);
        //        c?.UseLog4Net();
        //        c?.EnablePauseAndContinue();
        //        c?.EnableShutdown();
        //        c?.Service<Microservice>(s =>
        //        {
        //            s.ConstructUsingAutofacContainer<Microservice>();
        //            s?.ConstructUsing(settings =>
        //            {
        //                var service = AutofacHostBuilderConfigurator.LifetimeScope.Resolve<Microservice>();
        //                return service;
        //            });
        //            s?.ConstructUsing(name => new microservice(_container,
        //                Guid.NewGuid().ToString()));
        //            s?.WhenStartedAsLeader(b =>
        //            {
        //                b.WhenStarted(async (service, token) =>
        //                {
        //                    await service.Start(token);
        //                });
        //                b.Lease(lcb => lcb.RenewLeaseEvery(TimeSpan.FromSeconds(2))
        //                .AquireLeaseEvery(TimeSpan.FromSeconds(5))
        //                .LeaseLength(TimeSpan.FromDays(1))
        //                .WithLeaseManager(new Microservice()));
        //                b.WithHeartBeat(TimeSpan.FromSeconds(30), (isLeader, token) =>
        //                 Task.CompletedTask);
        //                b.Build();
        //            });
        //            s?.WhenStarted((Microservice server, HostControl host) => server.OnStart(host));
        //            s?.WhenPaused(server => server?.OnPause());
        //            s?.WhenContinued(server => server?.OnResume());
        //            s?.WhenStopped(server => server?.OnStop());
        //            s?.WhenShutdown(server => server?.OnShutdown());
        //            s?.WhenCustomCommandReceived((server, host, code) => { });
        //            s?.AfterStartingService(() => { });
        //            s?.AfterStoppingService(() => { });
        //            s?.BeforeStartingService(() => { });
        //            s?.BeforeStoppingService(() => { });
        //        });
        //        c?.RunAsNetworkService();
        //        c?.StartAutomaticallyDelayed();
        //        c?.SetDescription(string.Intern("Advanced File Watching Smaple"));
        //        c?.SetDisplayName(string.Intern("AdvancedFileWatchingMicroservice"));
        //        c?.SetServiceName(string.Intern("AdvancedFileWatchingMicroservice"));
        //        c?.EnableServiceRecovery(r =>
        //        {
        //            r?.OnCrashOnly();
        //            r?.RestartService(1); //first
        //            r?.RestartService(1); //second
        //            r?.RestartService(1); // subsequents
        //            r?.SetResetPeriod(0);
        //        });
        //    });
        //}
    }
}
