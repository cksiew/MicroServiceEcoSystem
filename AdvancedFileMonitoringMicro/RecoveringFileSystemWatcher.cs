using log4net;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace AdvancedFileMonitoringMicro
{
    public class RecoveringFileSystemWatcher : BufferingFileSystemWatcher
    {
        /// <summary>
        /// The directory monitor interval
        /// </summary>
        public TimeSpan DirectoryMonitorInterval = TimeSpan.FromMinutes(5);

        /// <summary>
        /// The directory retry interval
        /// </summary>
        public TimeSpan DirectoryRetryInterval = TimeSpan.FromSeconds(5);

        /// <summary>
        /// The monitor timer
        /// </summary>
        private System.Threading.Timer _monitorTimer;

        /// <summary>
        /// True if this object is recovering
        /// </summary>
        private bool _isRecovering;

        private static ILog _trace = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
        /// <summary>
        /// Initializes a new instance of the
        /// AdvancedFileMonitoringMicroservice.RecoveringFileSystemWatcher class
        /// </summary>
        public RecoveringFileSystemWatcher() : base(){ }

        /// <summary>
        /// Initializes a new instance of the
        /// AdvancedFileMonitoringMicroservice.RecoveringFileSystemWatcher class
        /// </summary>
        /// <param name="path">Full pathname of the file</param>
        public RecoveringFileSystemWatcher(string path) : base(path, "*.*")
        {

        }

        /// <summary>
        /// Initializes a new instance of the 
        /// AdvancedFileMonitoringMicroservice.RecoveringFileSystemWatcher class
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filter"></param>
        public RecoveringFileSystemWatcher(string path, string filter) : base(path, filter) { }

        private EventHandler<FileWatcherErrorEventArgs> _onErrorHandler = null;

        public new event EventHandler<FileWatcherErrorEventArgs> Error
        {
            add => _onErrorHandler += value;
            remove => _onErrorHandler -= value;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the raising events is enabled
        /// </summary>

        public new bool EnableRaisingEvents
        {
            get => base.EnableRaisingEvents;
            set
            {
                if (value == EnableRaisingEvents)
                    return;

                base.EnableRaisingEvents = value;
                if (EnableRaisingEvents)
                {
                    base.Error += BufferingFileSystemWatcher_Error;
                    Start();
                }
                else
                {
                    base.Error -= BufferingFileSystemWatcher_Error;
                }
            }
        }

        /// <summary>
        /// Starts this object
        /// </summary>
        private void Start()
        {
            _trace.Debug("");
            try
            {
                _monitorTimer = new System.Threading.Timer(_monitorTimer_Elapsed);

                Disposed += (_, __) =>
                {
                    _monitorTimer.Dispose();
                    _trace.Info("Obeying cancel request");
                };

                ReStartIfNecessary(TimeSpan.Zero);
            }
            catch(Exception ex)
            {
                _trace.Error($"Unexpected error:{ex}");
                throw;
            }
        }

        private void _monitorTimer_Elapsed(object state)
        {
            _trace.Debug("!!");
            _trace.Info($"Watching:{Path}");

            try
            {
                if (!Directory.Exists(Path))
                {
                    throw new DirectoryNotFoundException($"Directory not found {Path}");
                }

                _trace.Info($"Directory {Path} accessibility is OK.");
                if (!EnableRaisingEvents)
                {
                    EnableRaisingEvents = true;
                    if (_isRecovering)
                        _trace.Warn("<= Watcher recovered");
                }

                ReStartIfNecessary(DirectoryMonitorInterval);
            }
            catch(Exception ex) when (ex is FileNotFoundException || ex is DirectoryNotFoundException)
            {
                //Handles race condition too: Path loses accessibility between .Exists() and .EnableRaisingEvents
                if (ExceptionWasHandledByCaller(ex))
                    return;

                if (_isRecovering)
                {
                    _trace.Warn("...retrying");
                }
                else
                {
                    _trace.Warn($@"=> Directory {Path} Is Not accessible. - Will try to recover automatically in {DirectoryRetryInterval}!");
                    _isRecovering = true;
                }

                EnableRaisingEvents = false;
                _isRecovering = true;
                ReStartIfNecessary(DirectoryRetryInterval);
            }
            catch(Exception ex)
            {
                _trace.Error($"Unexpected error: {ex}");
                throw;
            }
        }

        /// <summary>
        /// Restart if necessary
        /// </summary>
        /// <param name="delay"></param>
        private void ReStartIfNecessary(TimeSpan delay)
        {
            _trace.Debug("");
            try
            {
                _monitorTimer.Change(delay, Timeout.InfiniteTimeSpan);
            }
            catch (ObjectDisposedException) { }
        }

        /// <summary>
        /// Event handler. Called by BufferingFileSystemWatcher for error events
        /// </summary>
        /// <param name="sender">Source of the event</param>
        /// <param name="e">Error event information</param>
        private void BufferingFileSystemWatcher_Error(object sender, ErrorEventArgs e)
        {
            // These exceptions have the same HResult
            var NetworkNameNoLongerAvailable = -2147467259; // occurs on network outage
            var AccessIsDenied = -2147467259;   // occurs after directory was deleted

            _trace.Debug("");

            var ex = e.GetException();
            if (ExceptionWasHandledByCaller(e.GetException()))
                return;

            //The base FSW does set .EnableRaisingEvents=False After raising OnError()
            EnableRaisingEvents = false;

            if (ex is InternalBufferOverflowException || ex is EventQueueOverflowException)
            {
                _trace.Warn(ex.Message);
                _trace.Error(@"This should Not happen with short event handlers! - Will recover automatically.");
                ReStartIfNecessary(DirectoryRetryInterval);
            }
            else if (ex is Win32Exception && (ex.HResult == NetworkNameNoLongerAvailable | ex.HResult == AccessIsDenied))
            {
                _trace.Warn(ex.Message);
                _trace.Warn("Will try to recover automatically!");
                ReStartIfNecessary(DirectoryRetryInterval);
            }
            else
            {
                _trace.Error($@"Unexpected error: {ex} - Watcher is disabled!");
                throw ex;
            }
        }

        /// <summary>
        /// Exception was handled by caller.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>True if it succeeds, false if it fails</returns>
        private bool ExceptionWasHandledByCaller(Exception ex)
        {
            // Allow consumer to handle error
            if (_onErrorHandler != null)
            {
                FileWatcherErrorEventArgs e = new FileWatcherErrorEventArgs(ex);
                InvokeHandler(_onErrorHandler, e);
                return e.Handled;
            }

            return false;
        }


        private void InvokeHandler(EventHandler<FileWatcherErrorEventArgs> eventHandler, FileWatcherErrorEventArgs e)
        {
            if (eventHandler != null)
            {
                if (SynchronizingObject?.InvokeRequired == true)
                    SynchronizingObject.BeginInvoke(eventHandler, new object[] { this, e });
                else
                    eventHandler(this, e);
            }
        }
    }
}
