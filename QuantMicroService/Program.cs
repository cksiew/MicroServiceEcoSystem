using Autofac;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.Autofac;
using Topshelf.Diagnostics;

namespace QuantMicroService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            // Service itself
            builder.RegisterType<Logger>()?.SingleInstance();
            builder.RegisterType<QuantMicroService>()
                .AsImplementedInterfaces()
                .AsSelf()
                ?.InstancePerLifetimeScope();
            var container = builder.Build();
            XmlConfigurator.ConfigureAndWatch(new FileInfo(@".log4net.config"));
            HostFactory.Run(c =>
            {
                c?.UseAutofacContainer(container);
                c?.UseLog4Net();
                c?.ApplyCommandLineWithDebuggerSupport();
                c?.EnablePauseAndContinue();
                c?.OnException(ex => Console.WriteLine(ex.Message));
                c?.Service<QuantMicroService>(s =>
                {
                    s.ConstructUsingAutofacContainer<QuantMicroService>();
                    s?.ConstructUsing(settings =>
                    {
                        var service = AutofacHostBuilderConfigurator.LifetimeScope.Resolve<QuantMicroService>();
                        return service;
                    });
                    s?.ConstructUsing(name => new QuantMicroService());
                    s?.WhenStarted((QuantMicroService server, HostControl host) => server.OnStart(host));
                    s?.WhenPaused(server => server?.OnPause());
                    s?.WhenContinued(server => server?.OnResume());
                    s?.WhenStopped(server => server?.OnStop());
                    s?.WhenShutdown(server => server?.OnShutdown());
                });
                c?.RunAsNetworkService();
                c?.StartAutomaticallyDelayed();
                c?.SetDescription(string.Intern("Quantitative Finance MicroService Sample"));
                c?.SetDisplayName(string.Intern("QuantFinanceMicroService"));
                c?.SetServiceName(string.Intern("QuantFinanceMicroService"));
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
