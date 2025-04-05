using System;
using System.IO;

public class LoggerService
{
    private const string LogFile = "Data/logs.txt";

    public void Log(string message)
    {
        var line = $"[{DateTime.Now}] {message}\n";
        File.AppendAllText(LogFile, line);
    }
}
