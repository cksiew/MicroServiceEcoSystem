using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    [Queue("Financial", ExchangeName="EvolvedAI")]
    [Serializable]
    public class BondsResponseMessage
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public long ID { get; set; }

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
        /// Gets or sets the compounding
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

        public string message { get; set; }
    }
}
