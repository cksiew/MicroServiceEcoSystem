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

namespace EmailMicroService
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Logger>()?.SingleInstance();
            builder.RegisterType<EmailMS>()
            .AsImplementedInterfaces()
            .AsSelf()
            ?.InstancePerLifetimeScope();
            var container = builder.Build();
            XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(@".log4net.config"));
            HostFactory.Run(c =>
            {
                c?.UseAutofacContainer(container);
                c?.UseLog4Net();
                c?.ApplyCommandLineWithDebuggerSupport();
                c?.EnablePauseAndContinue();
                c?.EnableShutdown();
                c?.OnException(ex => Console.WriteLine(ex.Message));
                c?.Service<EmailMS>(s =>
                {
                    s.ConstructUsingAutofacContainer<EmailMS>();
                    s?.ConstructUsing(settings =>
                    {
                        var service = AutofacHostBuilderConfigurator.LifetimeScope.Resolve<EmailMS>();
                        return service;
                    });
                    s?.ConstructUsing(name => new EmailMS());
                    s?.WhenStarted((EamilMS server, HostControl host) => server.OnStart(host));
                    s?.WhenPaused(server => server?.OnPause());
                    s?.WhenContinued(server => server?.OnResume());
                    s?.WhenStopped(server => server?.OnStop());
                    s?.WhenShutdown(server => server?.OnShutdown());

                });
            })
        }
    }
}
