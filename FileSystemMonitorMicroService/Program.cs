using CommonMessages;
using EasyNetQ;
using EasyNetQ.Migrations;
using EasyNetQ.Topology;
using NodaTime;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Topshelf;
using Topshelf.FileSystemWatcher;

namespace FileSystemMonitorMicroService
{
    internal class Program : Migration
    {
        /// <summary>
        /// The test dir
        /// </summary>
        private static readonly string _testDir = Directory.GetCurrentDirectory() + @"\test\";

        /// <summary>
        /// True to include, false to exclude the sub directories
        /// </summary>
        private static readonly bool _includeSubDirectories = true;

        /// <summary>
        /// True to exclude, false to include the duplicate events.
        /// </summary>
        private static readonly bool _excludeDuplicateEvents = true;

        /// <summary>
        /// The connection factory
        /// </summary>
        static ConnectionFactory _connectinFactory;

        /// <summary>
        /// The connection
        /// </summary>
        static IConnection _connection;

        /// <summary>
        /// The channel
        /// </summary>
        static IModel _channel;

        /// <summary>
        /// The bus
        /// </summary>
        private static IBus _bus;


        static void Main(string[] args)
        {
            HostFactory.Run(config =>
            {
                config.Service<Program>(s =>
                {
                    s.ConstructUsing(() => new Program());
                    s.BeforeStartingService((hostStart) =>
                    {
                        _bus = RabbitHutch.CreateBus("host=localhost");
                        if (!Directory.Exists(_testDir))
                            Directory.CreateDirectory(_testDir);
                    });
                    s.WhenStarted((service, host) => true);
                    s.WhenStopped((service, host) => true);
                    s.WhenFileSystemCreated(ConfigureDirectoryWorkCreated, FileSystemCreated);
                    s.WhenFileSystemChanged(ConfigureDirectoryWorkChanged, FileSystemCreated);
                    s.WhenFileSystemRenamed(ConfigureDirectoryWorkRenamedFile, FileSystemRenamedFile);
                    s.WhenFileSystemRenamed(ConfigureDirectoryWorkRenamedDirectory, FileSystemRenamedDirectory);
                    s.WhenFileSystemDeleted(ConfigureDirectoryWorkDeleted, FileSystemCreated);
                });
            });
            Console.ReadKey();
        }

        public override void Apply()
        {
            Declare.Exchange("EvolvedAI")
                .OnVirtualHost("/")
                .AsType(EasyNetQ.Migrations.ExchangeType.Topic)
                .Durable();

            Declare.Queue("FileSystem")
                .OnVirtualHost("/")
                .Durable();

            Declare.Binding()
                .OnVirtualHost("/")
                .FromExchange("EvolvedAI")
                .ToQueue("FileSystem")
                .RoutingKey("#");
        }

        public void Subscribe()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        /// <summary>
        /// Configure directory work created
        /// </summary>
        /// <param name="obj"></param>

        private static void ConfigureDirectoryWorkCreated(FileSystemWatcherConfigurator obj)
        {
            obj.AddDirectory(dir =>
            {
                dir.Path = _testDir;
                dir.IncludeSubDirectories = _includeSubDirectories;
                dir.NotifyFilters = NotifyFilters.DirectoryName | NotifyFilters.FileName;
                dir.ExcludeDuplicateEvents = _excludeDuplicateEvents;
            });
        }

        /// <summary>
        /// Configure directory work changed
        /// </summary>
        /// <param name="obj"></param>
        private static void ConfigureDirectoryWorkChanged(FileSystemWatcherConfigurator obj)
        {
            obj.AddDirectory(dir =>
            {
                dir.Path = _testDir;
                dir.IncludeSubDirectories = _includeSubDirectories;
                dir.NotifyFilters = NotifyFilters.LastWrite;
                dir.ExcludeDuplicateEvents = _excludeDuplicateEvents;
            });
        }

        /// <summary>
        /// Configure directory work renamed file.
        /// </summary>
        /// <param name="obj"></param>
        private static void ConfigureDirectoryWorkRenamedFile(FileSystemWatcherConfigurator obj)
        {
            obj.AddDirectory(dir =>
            {
                dir.Path = _testDir;
                dir.IncludeSubDirectories = _includeSubDirectories;
                dir.NotifyFilters = NotifyFilters.FileName;
                dir.ExcludeDuplicateEvents = _excludeDuplicateEvents;
            });
        }

        /// <summary>
        /// Configure directory work renamed directory
        /// </summary>
        /// <param name="obj"></param>
        private static void ConfigureDirectoryWorkRenamedDirectory(FileSystemWatcherConfigurator obj)
        {
            obj.AddDirectory(dir =>
            {
                dir.Path = _testDir;
                dir.IncludeSubDirectories = _includeSubDirectories;
                dir.NotifyFilters = NotifyFilters.DirectoryName;
                dir.ExcludeDuplicateEvents = _excludeDuplicateEvents;
            });
        }

        /// <summary>
        /// Configure directory work deleted.
        /// </summary>
        /// <param name="obj"></param>
        private static void ConfigureDirectoryWorkDeleted(FileSystemWatcherConfigurator obj)
        {
            obj.AddDirectory(dir =>
            {
                dir.Path = _testDir;
                dir.IncludeSubDirectories = _includeSubDirectories;
                dir.NotifyFilters = NotifyFilters.DirectoryName | NotifyFilters.FileName;
                dir.ExcludeDuplicateEvents = _excludeDuplicateEvents;
            });
        }

        /// <summary>
        /// File system created.
        /// </summary>
        /// <param name="topshelfFileSystemEventArgs"></param>

        private static void FileSystemCreated(TopshelfFileSystemEventArgs topshelfFileSystemEventArgs)
        {
            FileSystemChangeMessage m = new FileSystemChangeMessage
            {
                ChangeDate = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().ToLocalTime(),
                ChangeType = (int)topshelfFileSystemEventArgs.ChangeType,
                EventType = (int)topshelfFileSystemEventArgs.FileSystemEventType,
                FullPath = topshelfFileSystemEventArgs.FullPath,
                OldPath = topshelfFileSystemEventArgs.OldFullPath,
                Name = topshelfFileSystemEventArgs.Name,
                OldName = topshelfFileSystemEventArgs.OldName
            };

            _bus.Publish(m, "FileSystemChanges");

            Console.WriteLine("*************************");
            Console.WriteLine("ChangeType = {0}", topshelfFileSystemEventArgs.ChangeType);
            Console.WriteLine("FullPath = {0}", topshelfFileSystemEventArgs.FullPath);
            Console.WriteLine("Name = {0}", topshelfFileSystemEventArgs.Name);
            Console.WriteLine("FIleSystemEventType {0}", topshelfFileSystemEventArgs.FileSystemEventType);
            Console.WriteLine("*************************");
        }

        /// <summary>
        /// File system renamed file
        /// </summary>
        /// <param name="topshelfFileSystemEventArgs"></param>
        private static void FileSystemRenamedFile(TopshelfFileSystemEventArgs topshelfFileSystemEventArgs)
        {
            FileSystemChangeMessage m = new FileSystemChangeMessage
            {
                ChangeDate = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().ToLocalTime(),
                ChangeType = (int)topshelfFileSystemEventArgs.ChangeType,
                EventType = (int)topshelfFileSystemEventArgs.FileSystemEventType,
                FullPath = topshelfFileSystemEventArgs.FullPath,
                OldPath = topshelfFileSystemEventArgs.OldFullPath,
                Name = topshelfFileSystemEventArgs.Name,
                OldName = topshelfFileSystemEventArgs.OldName
            };

            _bus.Publish(m, "FileSystemChanges");

            Console.WriteLine("*************************");
            Console.WriteLine("ChangeType = {0}", topshelfFileSystemEventArgs.ChangeType);
            Console.WriteLine("FullPath = {0}", topshelfFileSystemEventArgs.FullPath);
            Console.WriteLine("Name = {0}", topshelfFileSystemEventArgs.Name);
            Console.WriteLine("FIleSystemEventType {0}", topshelfFileSystemEventArgs.FileSystemEventType);
            Console.WriteLine("*************************");
        }


        private static void FileSystemRenamedDirectory(TopshelfFileSystemEventArgs topshelfFileSystemEventArgs)
        {
            FileSystemChangeMessage m = new FileSystemChangeMessage
            {
                ChangeDate = SystemClock.Instance.GetCurrentInstant().ToDateTimeUtc().ToLocalTime(),
                ChangeType = (int)topshelfFileSystemEventArgs.ChangeType,
                EventType = (int)topshelfFileSystemEventArgs.FileSystemEventType,
                FullPath = topshelfFileSystemEventArgs.FullPath,
                OldPath = topshelfFileSystemEventArgs.OldFullPath,
                Name = topshelfFileSystemEventArgs.Name,
                OldName = topshelfFileSystemEventArgs.OldName
            };

            _bus.Publish(m, "FileSystemChanges");

            Console.WriteLine("*************************");
            Console.WriteLine("ChangeType = {0}", topshelfFileSystemEventArgs.ChangeType);
            Console.WriteLine("FullPath = {0}", topshelfFileSystemEventArgs.FullPath);
            Console.WriteLine("Name = {0}", topshelfFileSystemEventArgs.Name);
            Console.WriteLine("FIleSystemEventType {0}", topshelfFileSystemEventArgs.FileSystemEventType);
            Console.WriteLine("*************************");
        }

        /// <summary>
        /// Create a topology
        /// </summary>
        /// <param name="exchange">The exchange</param>
        /// <param name="queue">The queue</param>
        /// <param name="routingID"></param>
        public static void CreateTopology(string exchange, string queue, string routingID = "")
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
            IExchange exch = _bus.Advanced.ExchangeDeclare(exchange, EasyNetQ.Topology.ExchangeType.Topic);
            IQueue q = _bus.Advanced.QueueDeclare(queue);
            _bus.Advanced.Bind(exch, q, "");
        }


        public static void PublishMessage(object msg, string exchange, string routingID = "")
        {
            _bus.Publish(msg, "FileSystem");
        }
    }
}
