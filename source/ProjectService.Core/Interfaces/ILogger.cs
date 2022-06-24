using System.Globalization;

namespace ProjectService.Core.Interfaces;

public interface ILogger
{
    void Log(string msg, DateTime dateTime = default, string pattern = "[%dt]: %msg");

    static string GetLogString(string msg, DateTime dateTime, string pattern)
    {
        string logString = pattern;
        int i = -1;
        while (++i < logString.Length)
        {
            if (logString[i] == '%' && (i == 0 || logString[i - 1] != '\\'))
            {
                if (logString.Substring(i, 3) == "%dt")
                {
                    logString = logString.Remove(i, 3);
                    logString = logString.Insert(i, dateTime.ToString(CultureInfo.InvariantCulture));
                    continue;
                }
                if (logString.Substring(i, 4) == "%msg")
                {
                    logString = logString.Remove(i, 4);
                    logString = logString.Insert(i, msg);
                    continue;
                }
            }
            
        }

        return logString;
    }
}