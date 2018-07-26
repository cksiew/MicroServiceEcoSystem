using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    [Queue("Email",ExchangeName ="EvolvedAI")]
    [Serializable]
    public class EmailSendRequest
    {
        /// <summary>
        /// Source of the email
        /// </summary>
        public string From;

        /// <summary>
        /// to
        /// </summary>
        public string To;

        /// <summary>
        /// The subject
        /// </summary>
        public string Subject;

        /// <summary>
        /// The body
        /// </summary>
        public string Body;

    }
}
