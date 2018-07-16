using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    [Serializable]
    [Queue("General",ExchangeName="EvolvedAI")]
    public class BasicMessage
    {
        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the random number
        /// </summary>
        public int RandomNumber { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the date
        /// </summary>
        public DateTime Date { get; set; }
    }
}
