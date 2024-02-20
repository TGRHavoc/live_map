using System;
using CitizenFX.Core;
using Microsoft.Extensions.Logging;

namespace LiveMap.Utils
{
    public class DebugLoggerProvider : ILoggerProvider
    {
        public void Dispose()
        {
            throw new System.NotImplementedException();
        }
        
        public ILogger CreateLogger(string categoryName)
        {
            return new DebugLogger(categoryName);
        }
    }

    public class DebugLogger : ILogger
    {
        private readonly string _categoryName;
        public DebugLogger(string categoryName)
        {
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return true;
        }
        
        private string GetPrefix(LogLevel logLevel)
        {
            return $"{DateTime.Now:HH:mm:ss} [{logLevel}] {_categoryName}: ";
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (formatter == null)
            {
                throw new ArgumentNullException(nameof(formatter));
            }

            var message = formatter(state, exception);
            
            if (exception != null)
            {
                message += $"\n Exception: {exception.Message}\n StackTrace: {exception.StackTrace}";
            }
            //Console.WriteLine($"Message: {message}, Exception: {exception}");
            if (!string.IsNullOrEmpty(message))
            {
                Debug.WriteLine($"{GetPrefix(logLevel)}{message}");
            }
        }
    }
}