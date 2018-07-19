using Base_MicroService;
using CommonMessages;
using EasyNetQ;
using EasyNetQ.Management.Client.Model;
using EasyNetQ.MessageVersioning;
using EasyNetQ.Topology;
using JetBrains.Annotations;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Topshelf;

namespace DeploymentMonitorMicroService
{
    /// <summary>
    /// A deployment monitor microservice. This class cannot be inherited
    /// </summary>
    public sealed class DeploymentMonitorMicroService : BaseMicroService<DeploymentMonitorMicroService,HealthStatusMessage>
    {
        /// <summary>
        /// The bus
        /// </summary>
        private IBus _bus;

        /// <summary>
        /// True to deployment in progress
        /// </summary>
        private bool _deploymentInProgress;

        /// <summary>
        /// The deployment timer
        /// </summary>
        private Timer _deploymentTimer;

        /// <summary>
        /// The health timer
        /// </summary>
        private Timer _healthTimer;

        public DeploymentMonitorMicroService()
        {
            Name = "Deployment Monitor Microservice";
        }

        public new bool OnStart([CanBeNull] HostControl hc)
        {
            base.Start(hc);
            if (_bus == null)
                _bus = RabbitHutch.CreateBus("host=localhost");
            _deploymentInProgress = false;
            if (_deploymentTimer == null)
                _deploymentTimer = new Timer();
            _deploymentTimer.Interval = 6000 * 15; // give it 15 minutes
            _deploymentTimer.Enabled = true;
            _deploymentTimer.Elapsed += _deploymentTimer_Elapsed;
            _deploymentTimer.AutoReset = true;
            _deploymentTimer.Start();
            if (_healthTimer == null)
                _healthTimer = new Timer();
            _healthTimer.Interval = 60000;
            _healthTimer.Enabled = true;
            _healthTimer.AutoReset = true;
            _healthTimer.Elapsed += _healthTimer_Elapsed;
            _healthTimer.Start();
            Subscribe();
            return (true);
        }

        public void Subscribe()
        {
            Bus = RabbitHutch.CreateBus("host=localhost", x =>
            {
                x.Register<IConventions, AttributeBasedConventions>();
                x.EnableMessageVersioning();
            });
            IExchange exchange = Bus.Advanced.ExchangeDeclare("EvolvedAI", EasyNetQ.Topology.ExchangeType.Topic);
            IQueue queue = Bus.Advanced.QueueDeclare("Deployments");
            Bus.Advanced.Bind(exchange, queue, "");
            Bus.Subscribe<DeploymentStartMessage>("Deployment.Start", msg =>
            {
                ProcessDeploymentStartMessage(msg);
            },config=> config.WithTopic("Deployments"));
            Bus.Subscribe<DeploymentStopMessage>("Deployment.Stop", msg =>
            {
                ProcessDeploymentStopMessage(msg);
            }, config => config.WithTopic("Deployments"));
        }

        public void ProcessDeploymentStartMessage(DeploymentStartMessage msg)
        {
            Console.WriteLine("Processing Start Messages");
            _deploymentTimer.Stop();
            _deploymentTimer.Start();
        }

        public void ProcessDeploymentStopMessage(DeploymentStopMessage msg)
        {
            Console.WriteLine("Processing Stop Messages");
            _deploymentTimer.Stop();
        }

        private void _healthTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            HealthStatusMessage h = new HealthStatusMessage
            {
                ID = ID,
                memoryUsed = Environment.WorkingSet,
                CPU = Convert.ToDouble(getCPUCounter()),
                date = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().ToLocalTime(),
                serviceName = "Deployment Monitor MicroService",
                message = "OK",
                status = (int)MSStatus.Healthy
            };
            PublishMessage(h, "EvolvedAI", "");
        }

        private void _deploymentTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_deploymentInProgress)
                Console.WriteLine("ERROR: Deployment is taking too long");
        }


        public new bool OnStop()
        {
            base.Stop();
            _deploymentTimer.Stop();
            _deploymentTimer.Enabled = false;
            _deploymentTimer.Elapsed -= _deploymentTimer_Elapsed;
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
