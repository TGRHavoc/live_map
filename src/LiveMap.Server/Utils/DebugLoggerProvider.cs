using CitizenFX.Core;
using Microsoft.Extensions.Logging;

namespace LiveMap.Server.Utils;

public class DebugLoggerProvider : ILoggerProvider
{
    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public ILogger CreateLogger(string categoryName)
    {
        // Category name is a fully qualified class name but we don't need that, only the class name
        var lastDot = categoryName.LastIndexOf('.');
        if (lastDot > 0) categoryName = categoryName.Substring(lastDot + 1);

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

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (formatter == null) throw new ArgumentNullException(nameof(formatter));

        var message = formatter(state, exception);

        if (exception != null) message += $"\n Exception: {exception.Message}\n StackTrace: {exception.StackTrace}";
        //Console.WriteLine($"Message: {message}, Exception: {exception}");
        if (!string.IsNullOrEmpty(message)) Debug.WriteLine($"{GetPrefix(logLevel)}{message}");
    }

    private string GetPrefix(LogLevel logLevel)
    {
        var logLevelString = logLevel.ToString();
        if (logLevel is LogLevel.Information or LogLevel.Critical or LogLevel.Warning)
            logLevelString = logLevel.ToString().Substring(0, 4);

        if (logLevelString.Length < 5)
        {
            // Pad with space to align log levels
            var padAmount = 5 - logLevelString.Length;
            logLevelString = logLevelString.PadRight(padAmount + logLevelString.Length);
        }
        
        return $"{DateTime.Now:HH:mm:ss} [{logLevelString}] {_categoryName}: ";
    }
}