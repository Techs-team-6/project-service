using System.Text;
using Octokit;
using ProjectService.Core.Interfaces;

namespace ProjectService.Core.Loggers;

public class StreamLogger : ILogger
{
    private readonly Stream _stream;
    private readonly bool _leaveOpen;

    public StreamLogger(Stream stream, bool leaveOpen)
    {
        _stream = stream ?? throw new ArgumentNullException(nameof(stream),"stream can't be null");
        _leaveOpen = leaveOpen;
    }

    public void Log(string msg, DateTime dateTime = default, string pattern = "[%dt]: %msg")
    {
        if (dateTime == default)
            dateTime = DateTime.Now;

        string logString = ILogger.GetLogString(msg, dateTime, pattern);
        
        Encoding encoding = Encoding.Default;
        var streamWriter = new StreamWriter(_stream, encoding, encoding.GetByteCount(logString), _leaveOpen);
        
        streamWriter.WriteLine(logString);
        streamWriter.Dispose();
    }
}