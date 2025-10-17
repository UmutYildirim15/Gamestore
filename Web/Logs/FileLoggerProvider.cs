namespace Web.Logs
{
    public class FileLoggerProvider(string basePath, LogLevel minLogLevel) : ILoggerProvider
    {
        public ILogger CreateLogger(string categoryName) => new FileLogger(basePath, minLogLevel);

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }

    public class FileLogger(string basePath, LogLevel minLogLevel) : ILogger
    {
        IDisposable? ILogger.BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => logLevel >= minLogLevel;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception, string>? formatter)
        {
            if (!IsEnabled(logLevel) || formatter == null) return;

            var logPath = Path.Combine(basePath, $"{DateTime.UtcNow:yyyy-MM-dd}.txt");
            if (exception == null) return;
            var logRecord = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} [{logLevel}] {formatter(state, exception)}";

            logRecord += Environment.NewLine + exception;

            logRecord += Environment.NewLine;
            File.AppendAllText(logPath, logRecord);
        }
    }
}