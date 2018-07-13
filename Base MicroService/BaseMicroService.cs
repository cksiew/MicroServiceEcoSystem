using Autofac;
using CacheManager.Core;
using CodeContracts;
using EasyNetQ;
using JetBrains.Annotations;
using log4net;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Grumpy.ServiceBase;
using Topshelf;

namespace Base_MicroService
{
    public class BaseMicroService<T> :IBaseMicroService
        where T: class, new()
    {
        /// <summary>
        /// The timer.
        /// </summary>
        private static Timer _timer = null;

        /// <summary>
        /// The log.
        /// </summary>
        readonly static ILog _log = LogManager.GetLogger(typeof(BaseMicroService<T>));

        /// <summary>
        /// Identifier for the worker
        /// </summary>
        private string _workerId;

        /// <summary>
        /// The lifetimescope
        /// </summary>
        readonly ILifetimeScope _lifetimescope;

        /// <summary>
        /// The name
        /// </summary>
        private static string _name;

        /// <summary>
        /// The host.
        /// </summary>
        private static HostControl _host;

        /// <summary>
        /// The type
        /// </summary>
        private static T _type;

        /// <summary>
        /// The connection factory.
        /// The bus
        /// </summary>
        private IBus _bus;

        public IBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        private ICacheManager<object> _cache = null;

        public BaseMicroService()
        {
            double interval = 60000;
            _timer = new Timer(interval);
            Assumes.True(_timer != null, "_timer is null");
            //_timer.Elapsed += OnTick;
            _timer.AutoReset = true;
            _workerId = Guid.NewGuid().ToString();
            _name = nameof(T);
        }

        public bool Start(HostControl hc)
        {
            _host = hc;
            Console.WriteLine(_name + string.Intern("Service Started."));
            Assumes.True(_timer != null, string.Intern("_timer is null"));
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
            return true;
        }

        public bool Stop()
        {
            Assumes.True(_log != null, string.Intern("_log is null"));
            _log?.Info(_name + string.Intern("Service is Stopped"));
            Assumes.True(_timer != null, string.Intern("_time is null"));
            _timer.AutoReset = false;
            _timer.Enabled = false;
            return true;
        }

        public bool Pause()
        {
            throw new NotImplementedException();
        }

        public bool Continue()
        {
            throw new NotImplementedException();
        }

        public bool Shutdown()
        {
            throw new NotImplementedException();
        }

        public bool Resume()
        {
            throw new NotImplementedException();
        }

        public void TryRequest(Action action, int maxFailures, int startTimeoutMS = 100, int resetTimeout = 10000, Action<Exception> OnError = null)
        {
            throw new NotImplementedException();
        }

        public Task TryRequestAsync([NotNull] Func<Task> action, int maxFailure, int startTimeoutMS = 100, int resetTimeout = 10000, [CanBeNull] Action<Exception> OnError = null)
        {
            throw new NotImplementedException();
        }

        public void PublishMessage(object message, string connStr = "host=localhost", string topic = "")
        {
            throw new NotImplementedException();
        }

        public Task PublishMessageAsync(object message, string connStr = "host=localhost", string topic = "")
        {
            throw new NotImplementedException();
        }



        protected virtual void OnTick([NotNull] object sender, [NotNull]ElapsedEventArgs e)
        {
            Console.WriteLine(string.Intern("Heartbeat"));
            Requires.NotNull<ILog>(_log, string.Intern("log is null"));
            _log?.Debug(_name + " (" + _workerId.ToString() + string.Intern("): ") +
                SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().ToLocalTime().ToLongTimeString() + string.Intern(": Heartbeat"));
            HealthStatusMessage h = new HealthStatusMessage
            {
                ID = _workerId,
                memoryUsed = Environment.WorkingSet,
                CPU = Convert.ToDouble(getCPUCounter()),
                date = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().ToLocalTime(),
                serviceName = _name,
                Message = "OK",
                Status = (int)MSStatus.Healthy

            };
            Bus.Publish(h, "HealthStatus");


        }


    }
}
