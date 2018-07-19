using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CommonMessages
{
    using EasyNetQ;

    [Serializable]
    [Queue("Memory",ExchangeName ="EvolvedAI")]
    public class MemoryUpdateMessage
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the text
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets the number of generate 1 collections
        /// </summary>
        public int Gen1CollectionCount { get; set; }

        /// <summary>
        /// Gets or sets the number of generate 2 collections
        /// </summary>
        public int Gen2CollectionCount { get; set; }

        /// <summary>
        /// Gets or sets the time spent percent
        /// </summary>
        public float TimeSpentPercent { get; set; }

        /// <summary>
        /// Gets or sets a collection of memory befores
        /// </summary>
        public string MemoryBeforeCollection { get; set; }

        /// <summary>
        /// Gets or sets a collection of memory afters
        /// </summary>
        public string MemoryAfterCollection { get; set; }

        /// <summary>
        /// Gets or sets the Date/Time of the date
        /// </summary>
        public DateTime Date { get; set; }
    }
}
