using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    /// <summary>
    /// The bonds request message
    /// </summary>
    [Queue("Financial", ExchangeName ="EvolvedAI")]
    [Serializable]
    public class BondsRequestMessage
    {
        /// <summary>
        /// Gets or sets the Date/Time of the issue
        /// </summary>

        public DateTime issue { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the maturity
        /// </summary>
        public DateTime maturity { get; set; }

        /// <summary>
        /// Gets or sets the coupon
        /// </summary>
        public double coupon { get; set; }

        /// <summary>
        /// Gets or sets the frequency
        /// </summary>
        public int frequency { get; set; }

        /// <summary>
        /// Gets or sets the yield
        /// </summary>
        public double yield { get; set; }

        /// <summary>
        /// Gets or sets a message indicating if this is compounding or continous
        /// </summary>
        public string compounding { get; set; }

        /// <summary>
        /// Gets or sets the price
        /// </summary>
        public double price { get; set; }

        /// <summary>
        /// Gets or sets the calculate yield
        /// </summary>

        public double calcYield { get; set; }

        /// <summary>
        /// Gets or sets the price 2
        /// </summary>
        public double price2 { get; set; }

        /// <summary>
        /// Gets or sets the message
        /// </summary>
        public string message { get; set; }
    }
}
