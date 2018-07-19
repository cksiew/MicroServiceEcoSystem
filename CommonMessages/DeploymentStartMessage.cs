using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    /// <summary>
    /// A deployment start message
    /// </summary>
    [Serializable]
    [Queue("Deployments", ExchangeName = "EvolvedAI")]
    public class DeploymentStartMessage
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the date.
        /// </summary>
        public DateTime Date { get; set; }
    }
}
