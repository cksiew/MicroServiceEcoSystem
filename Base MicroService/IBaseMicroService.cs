using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;

namespace Base_MicroService
{
    public interface IBaseMicroService
    {
        bool Start(HostControl hc);
        bool Stop();
        bool Pause();
        bool Continue();
        bool Shutdown();
        bool Resume();
        void TryRequest(Action action, int maxFailures, int startTimeoutMS = 100, int resetTimeout = 10000, Action<Exception> OnError = null);
        Task TryRequestAsync([NotNull] Func<Task> action, int maxFailure, int startTimeoutMS = 100, int resetTimeout = 10000, [CanBeNull] Action<Exception> OnError = null);
        void PublishMessage(object message, string connStr = "host=localhost", string topic = "");
        Task PublishMessageAsync(object message, string connStr = "host=localhost", string topic = "");
    }
}
