using System;
using System.IO;

namespace Eventify.Services
{
    public static class LoggerService
    {
        private const string LogFilePath = "Data/logs.txt";

        public static void Log(string message)
        {
            try
            {
                var logEntry = $"{DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";
                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // Silent fail - logging shouldn't break the application
            }
        }

        public static void LogException(Exception ex)
        {
            Log($"ERROR: {ex.GetType().Name} - {ex.Message}");
            Log($"Stack Trace: {ex.StackTrace}");
        }
    }
}