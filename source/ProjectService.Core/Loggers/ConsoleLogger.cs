using System.Globalization;
using ProjectService.Core.Interfaces;

namespace ProjectService.Core.Loggers;

public class ConsoleLogger : ILogger
{
    public void Log(string msg, DateTime dateTime = default, string pattern = "[%dt]: %msg")
    {
        if (dateTime == default)
            dateTime = DateTime.Now;
        
        Console.WriteLine(ILogger.GetLogString(msg, dateTime, pattern));
    }
}