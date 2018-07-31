using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdvancedFileMonitoringMicro
{
    public class FileWatcherErrorEventArgs : HandledEventArgs
    {
        /// <summary>
        /// The error
        /// </summary>
        public readonly Exception Error;

        /// <summary>
        /// Initializes a new instance of the FileWatcherErrorEventArgs class.
        /// </summary>
        /// <param name="exception">The exception</param>
        public FileWatcherErrorEventArgs(Exception exception)
        {
            this.Error = exception;
        }
    }
}
