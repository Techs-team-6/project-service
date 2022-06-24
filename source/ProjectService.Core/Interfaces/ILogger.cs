using System.Globalization;

namespace ProjectService.Core.Interfaces;

public interface ILogger
{
    /**
     * <param name="msg">message to log</param>
     * <param name="dateTime">Date and Time of log</param>
     * <param name="pattern">%dt - DateTime %msg - message</param>
     */
    void Log(string msg, DateTime dateTime = default, string pattern = "[%dt]: %msg");

    protected static string GetLogString(string msg, DateTime dateTime, string pattern)
    {
        string logString = pattern;
        int i = -1;
        while (++i < logString.Length)
        {
            if (logString[i] == '%' && (i == 0 || logString[i - 1] != '\\'))
            {
                try
                {
                    if (logString.Substring(i, 3) == "%dt")
                    {
                        logString = logString.Remove(i, 3);
                        logString = logString.Insert(i, dateTime.ToString(CultureInfo.InvariantCulture));
                        continue;
                    }
                }
                catch (ArgumentOutOfRangeException)
                { }

                try
                {
                    if (logString.Substring(i, 4) == "%msg")
                    {
                        logString = logString.Remove(i, 4);
                        logString = logString.Insert(i, msg);
                        continue;
                    }
                }
                catch (ArgumentOutOfRangeException)
                { }
            }
            
        }

        return logString;
    }
}