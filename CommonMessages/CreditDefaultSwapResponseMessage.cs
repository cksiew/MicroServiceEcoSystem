using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    /// <summary>
    /// A credit default swap message
    /// </summary>
    [Queue("Financial",ExchangeName="EvolvedAI")]
    [Serializable]
    public class CreditDefaultSwapResponseMessage
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the fixed rate.
        /// </summary>
        public double fixedRate { get; set; }

        /// <summary>
        /// Gets or sets the notional
        /// </summary>
        public double notional { get; set; }

        /// <summary>
        /// Gets or sets the recovery rate
        /// </summary>
        public double recoveryRate { get; set; }

        /// <summary>
        /// Gets or sets the fair rate
        /// </summary>
        public double fairRate { get; set; }

        /// <summary>
        /// Gets or sets the fair npv.
        /// </summary>
        public double fairNPV { get; set; }

    }
}
