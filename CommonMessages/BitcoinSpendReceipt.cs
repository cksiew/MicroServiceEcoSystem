﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonMessages
{
    using EasyNetQ;

    /// <summary>   (Serializable) the bitcoin spend receipt message. </summary>
    [Queue("Bitcoin", ExchangeName = "EvolvedAI")]
    [Serializable]
    public class BitcoinSpendReceipt
    {
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the identifier. </summary>
        ///
        /// <value> The identifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public long ID { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the amount. </summary>
        ///
        /// <value> The amount. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public decimal amount { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the success. </summary>
        ///
        /// <value> True if success, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public bool success { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Date/Time of the time. </summary>
        ///
        /// <value> The time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        public DateTime time { get; set; }
    }
}
