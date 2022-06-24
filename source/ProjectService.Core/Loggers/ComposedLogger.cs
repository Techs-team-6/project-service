using ProjectService.Core.Interfaces;

namespace ProjectService.Core.Loggers;

public class ComposedLogger : ILogger
{
    private readonly ILogger[] _loggers;

    public ComposedLogger(ILogger[] loggers)
    {
        _loggers = loggers;
    }

    public void Log(string msg, DateTime dateTime = default, string pattern = "[%dt]: %msg")
    {
        foreach (ILogger logger in _loggers)
        {
            logger.Log(msg, dateTime, pattern);
        }
    }
}