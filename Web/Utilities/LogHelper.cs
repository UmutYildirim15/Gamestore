namespace Web.Utilities;

public static class LogHelper
{
    private static readonly string ExceptionLogFolder = Path.Combine("Logs", "ExceptionLogs");
    private static readonly string RequestLogFolder = Path.Combine("Logs", "RequestLogs");

    static LogHelper()
    {
        Directory.CreateDirectory(ExceptionLogFolder);
        Directory.CreateDirectory(RequestLogFolder);
    }

    public static void WriteExceptionLog(string logMessage)
    {
        WriteLog(ExceptionLogFolder, "exception-log", logMessage);
    }

    public static void WriteRequestLog(string logMessage)
    {
        WriteLog(RequestLogFolder, "request-log", logMessage);
    }

    private static void WriteLog(string folderPath, string logPrefix, string logMessage)
    {
        var logFileName = $"{logPrefix}-{DateTime.UtcNow:yyyy-MM-dd}.txt";
        var logFilePath = Path.Combine(folderPath, logFileName);

        using var writer = File.AppendText(logFilePath);
        writer.WriteLine($"[{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss}] {logMessage}");
    }
}
