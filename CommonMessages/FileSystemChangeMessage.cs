using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EasyNetQ;

namespace CommonMessages
{
    /// <summary>
    /// A file system change message
    /// </summary>
    [Serializable]
    [Queue("FileSystem",ExchangeName="EvolvedAI")]
    public class FileSystemChangeMessage
    {
        /// <summary>
        /// Gets or sets the identifier
        /// </summary>
        public long ID { get; set; }

        /// <summary>
        /// Gets or sets the type of the change.
        /// </summary>
        public int ChangeType { get; set; }

        /// <summary>
        /// Gets or sets the type of the event
        /// </summary>
        public int EventType { get; set; }

        /// <summary>
        /// Gets or sets the change date
        /// </summary>
        public DateTime ChangeDate { get; set; }

        /// <summary>
        /// Gets or sets the full pathname of the file
        /// </summary>
        public string FullPath { get; set; }

        /// <summary>
        /// Gets or sets the full pathname of the old file
        /// </summary>
        public string OldPath { get; set; }

        /// <summary>
        /// Gets of sets the name of the file or directory
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the old name of the file or directory
        /// </summary>
        public string OldName { get; set; }

    }
}
