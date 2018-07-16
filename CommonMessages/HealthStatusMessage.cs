using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    /// <summary>
    /// Values that represent Milliseconds status
    /// </summary>
    public enum MSStatus
    {
        /// <summary>
        /// An enum constant representing the healthy=1 option
        /// </summary>
        Healthy=1,

        /// <summary>
        /// An enum constant representing the unhealthy=2 option
        /// </summary>
        Unhealthy=2
    }
    [Serializable]
    [Queue("Health",ExchangeName ="EvolvedAI")]
    public class HealthStatusMessage
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the date
        /// </summary>
        public DateTime date { get; set; }

        /// <summary>
        /// Gets or sets the name of the service
        /// </summary>
        public string serviceName { get; set; }

        /// <summary>
        /// Gets or sets the status
        /// </summary>
        public int status { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// Gets or sets the memory used
        /// </summary>
        public double memoryUsed { get; set; }

        /// <summary>
        /// Gets or sets the CPU.
        /// </summary>
        public double CPU { get; set; }
    }
}
