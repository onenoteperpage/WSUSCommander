using System;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace WSUSCommander.Extensions;

public static class CLogger
{
    private static readonly object _lock = new object();
    private static string? _logDirectory;
    private static string? _logFilePrefix;
    private static string? _logFileName;
    private static LogLevel _minLogLevel;

    // Enum for log levels
    public enum LogLevel
    {
        DEBUG = 1,
        INFO = 2,
        WARN = 3,
        ERROR = 4,
        FATAL = 5
    }

    // Initialize the logger with configuration
    public static void Initialize(IConfiguration configuration)
    {
        var loggingConfig = configuration.GetSection("CLogger");
        _logDirectory = loggingConfig["Directory"] ?? "Logs";
        _logFilePrefix = loggingConfig["FilePrefix"] ?? DateTime.Now.ToString("yyyyMMdd");
        _logFileName = loggingConfig["FileName"] ?? "_wsuscommander.log";

        // Parse minimum log level
        if (!Enum.TryParse(loggingConfig["LogLevel"], true, out _minLogLevel))
        {
            _minLogLevel = LogLevel.DEBUG; // Default to DEBUG if parsing fails
        }

        // Ensure the log directory exists
        if (!Directory.Exists(_logDirectory))
        {
            Directory.CreateDirectory(_logDirectory);
        }
    }

    // Core logging method
    private static void LogMessage(LogLevel level, string message, Exception? ex = null)
    {
        if (level < _minLogLevel)
            return;

        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
        string logLevel = level.ToString();
        string logMessage = $"[{timestamp}] [{logLevel}] {message}";

        if (ex != null)
        {
            logMessage += Environment.NewLine + $"Exception: {ex}";
        }

        string datePrefix = DateTime.Now.ToString(_logFilePrefix);
        string fullLogFilePath = Path.Combine(_logDirectory ?? "Logs", $"{datePrefix}{_logFileName}");

        lock (_lock)
        {
            try
            {
                File.AppendAllText(fullLogFilePath, logMessage + Environment.NewLine);
            }
            catch
            {
                // Handle exceptions related to logging (e.g., file access issues)
                // Optionally, you can implement fallback mechanisms here
            }
        }
    }

    // Public logging methods
    public static void Debug(string message)
    {
        LogMessage(LogLevel.DEBUG, message);
    }

    public static void Info(string message)
    {
        LogMessage(LogLevel.INFO, message);
    }

    public static void Warn(string message)
    {
        LogMessage(LogLevel.WARN, message);
    }

    public static void Error(string message, Exception? ex = null)
    {
        LogMessage(LogLevel.ERROR, message, ex);
    }

    public static void Fatal(string message, Exception? ex = null)
    {
        LogMessage(LogLevel.FATAL, message, ex);
    }
}
