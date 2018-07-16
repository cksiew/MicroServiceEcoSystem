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
using IContainer = Autofac.IContainer;
using Console = Colorful.Console;
using log4net.Core;
using CircuitBreaker.Net;
using CircuitBreaker.Net.Exceptions;
using System.Drawing;
using CommonMessages;
using System.Diagnostics;
using System.Threading;

namespace Base_MicroService
{
    public class BaseMicroService<T> :IBaseMicroService
        where T: class, new()
    {
        /// <summary>
        /// The timer.
        /// </summary>
        private static System.Timers.Timer _timer = null;

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
        private ILifetimeScope _lifetimescope;

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

        /// <summary>
        /// The logger.
        /// </summary>
        private static ILogger _logger;

        /// <summary>
        /// The di container
        /// </summary>
        private IContainer _diContainer;
       

        private ICacheManager<object> _cache = null;

        /// <summary>
        /// Gets or sets the bus
        /// </summary>
        public IBus Bus
        {
            get { return _bus; }
            set { _bus = value; }
        }

        /// <summary>
        /// Gets or sets the cache
        /// </summary>
        public ICacheManager<object> Cache
        {
            get { return _cache; }
            set { _cache = value; }
        }

        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public string ID
        {
            get { return _workerId.ToString(); }
            set { _workerId = value; }
        }

        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        /// <summary>
        /// Gets or sets the host
        /// </summary>
        public HostControl Host
        {
            get { return _host; }
            set { _host = value; }
        }

        /// <summary>
        /// Gets or sets the IOC container
        /// </summary>
        public IContainer IOCContainer
        {
            get { return _diContainer; }
            set { _diContainer = value; }
        }

        /// <summary>
        /// Gets or sets the lifetime scope
        /// </summary>
        public ILifetimeScope LifetimeScope
        {
            get { return _lifetimescope; }
            set { _lifetimescope = value; }
        }


        public BaseMicroService()
        {
            double interval = 60000;
            _timer = new System.Timers.Timer(interval);
            Assumes.True(_timer != null, "_timer is null");
            _timer.Elapsed += OnTick;
            _timer.AutoReset = true;
            _workerId = Guid.NewGuid().ToString();
            _name = nameof(T);
        }

        public virtual bool Start(HostControl hc)
        {
            _host = hc;
            Console.WriteLine(_name + string.Intern("Service Started."));
            Assumes.True(_timer != null, string.Intern("_timer is null"));
            _timer.AutoReset = true;
            _timer.Enabled = true;
            _timer.Start();
            return true;
        }

        public virtual bool Stop()
        {
            using (var scope = IOCContainer?.BeginLifetimeScope())
            {
                var logger = scope?.Resolve<MSBaseLogger>();
                logger?.LogInformation(Name + " Microservice Stopping");
            }
            Assumes.True(_log != null, string.Intern("_log is null"));
            _log?.Info(_name + string.Intern("Service is Stopped"));
            Assumes.True(_timer != null, string.Intern("_time is null"));
            _timer.AutoReset = false;
            _timer.Enabled = false;
            _timer.Stop();
            return true;
        }

        public bool Pause()
        {
            using (var scope = IOCContainer?.BeginLifetimeScope())
            {
                var logger = scope?.Resolve<MSBaseLogger>();
                logger?.LogInformation(Name + "Microservice Pausing");
            }
            return true;
        }

        public bool Continue()
        {
            using (var scope = IOCContainer?.BeginLifetimeScope())
            {
                var logger = scope?.Resolve<MSBaseLogger>();
                logger?.LogInformation(Name + "Microservice Continuing");
            }
            return true;
        }

        public bool Shutdown()
        {
            using (var scope = IOCContainer.BeginLifetimeScope())
            {
                var logger = scope?.Resolve<MSBaseLogger>();
                logger?.LogInformation(Name + "Microservice Shutting Down");
            }
            return true;
        }

        public virtual bool Resume()
        {
            using (var scope = IOCContainer?.BeginLifetimeScope())
            {
                var logger = scope?.Resolve<MSBaseLogger>();
                logger?.LogInformation(Name + " Microservice Running");
            }
            return true;
        }

        public void TryRequest(Action action, int maxFailures, int startTimeoutMS = 100, int resetTimeout = 10000, Action<Exception> OnError = null)
        {
            try
            {
                Requires.True(maxFailures >= 1, "maxFailures must be >= 1");
                Requires.True(startTimeoutMS >= 1, "startTimeoutMS must be >= 1");
                Requires.True(resetTimeout >= 1, "resetTimeout must be >= 1");
                // Initialize the circuit breaker
                var circuitBreaker = new CircuitBreaker.Net.CircuitBreaker(TaskScheduler.Default,
                    maxFailures: maxFailures,
                    invocationTimeout: TimeSpan.FromMilliseconds(startTimeoutMS),
                    circuitResetTimeout: TimeSpan.FromMilliseconds(resetTimeout));
                circuitBreaker.Execute(() => action);
               
            }
            catch(CircuitBreakerOpenException e1)
            {
                OnError?.Invoke(e1);
                Console.WriteLine(e1.Message,Color.Red);
            }
            catch(CircuitBreakerTimeoutException e2)
            {
                OnError?.Invoke(e2);
                Console.WriteLine(e2.Message, Color.Red);
            }
            catch(Exception e3)
            {
                OnError?.Invoke(e3);
                Console.WriteLine(e3.Message, Color.Red);
            }
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

        /// <summary>
        /// Publish memory update message.
        /// </summary>
        /// <param name="gen1"> The first generate. </param>
        /// <param name="gen2"> The second generate. </param>
        /// <param name="timeSpent"> The time spent. </param>
        /// <param name="MemoryBefore"> The memory before. </param>
        /// <param name="MemoryAfter"> The memory after. </param>
        public void PublishMemoryUpdateMessage(int gen1, int gen2, float timeSpent, string MemoryBefore, string MemoryAfter)
        {
            // publish a message
            MemoryUpdateMessage msg = new MemoryUpdateMessage
            {
                Text = "Memory MicroService Ran",
                Date = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc(),
                Gen1CollectionCount = gen1,
                Gen2CollectionCount = gen2,
                TimeSpentPercent = timeSpent,
                MemoryBeforeCollection = MemoryBefore,
                MemoryAfterCollection = MemoryAfter
            };
            Bus.Publish(msg, "MemoryStatus");
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
                message = "OK",
                status = (int)MSStatus.Healthy

            };
            Bus.Publish(h, "HealthStatus");


        }

        /// <summary>
        /// Gets CPU counter
        /// </summary>
        /// <returns>The CPU counter</returns>
        public float getCPUCounter()
        {
            PerformanceCounter cpuCounter = new PerformanceCounter
            {
                CategoryName = string.Intern("Processor"),
                CounterName = string.Intern("%Processor Time"),
                InstanceName = string.Intern("_Total")
            };

            // will always start at 0
            dynamic firstValue = cpuCounter.NextValue();
            Thread.Sleep(1000);
            // now matches task manager reading
            float secondValue = cpuCounter.NextValue();
            return secondValue;
        }


    }
}
