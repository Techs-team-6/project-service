using NLog;
using NLog.Fluent;

namespace ProjectService.Core.Entities;

public class LoggerKeeper
{
    private static LoggerKeeper? _instance;

    private static object syncRoot = new();

    private LoggerKeeper()
    {
        Logger = LogManager.GetLogger("fileLogger");
    }

    public Logger Logger { get; }
    
    public static LoggerKeeper GetInstance()
    {
        lock (syncRoot)
        {
            if (_instance == null)
            {
                _instance = new LoggerKeeper();
            }
        }

        return _instance;
    }
}