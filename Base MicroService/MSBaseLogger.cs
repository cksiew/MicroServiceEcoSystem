using ReflectSoftware.Insight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Base_MicroService
{
    public class MSBaseLogger : ILogger
    {
        private void WriteLineInColor(string message, ConsoleColor foregroundColor)
        {
            Console.ForegroundColor = foregroundColor;
            Console.WriteLine(message);
            Console.ResetColor();
        }

        public void LogDebug(string message)
        {
            RILogManager.Default?.SendDebug(message);
            WriteLineInColor(message, ConsoleColor.Blue);
        }

        public void LogError(string message)
        {
            RILogManager.Default?.SendError(message);
            WriteLineInColor(message, ConsoleColor.Red);
        }

        public void LogException(string message, Exception ex)
        {
            RILogManager.Default?.SendException(message, ex);
            WriteLineInColor(message, ConsoleColor.Red);
        }

        public void LogInformation(string message)
        {
            RILogManager.Default?.SendInformation(message);
            WriteLineInColor(message, ConsoleColor.White);
        }

        public void LogTrace(string message)
        {
            RILogManager.Default?.SendTrace(message);
            WriteLineInColor(message, ConsoleColor.Cyan);
            Trace.WriteLine(message);
        }

        public void LogWarning(string message)
        {
            RILogManager.Default?.SendWarning(message);
            WriteLineInColor(message, ConsoleColor.Yellow);
        }
    }
}
