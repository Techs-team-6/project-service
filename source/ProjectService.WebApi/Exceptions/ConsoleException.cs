using System.Diagnostics;

namespace ProjectService.WebApi.Exceptions;

public class ConsoleException : ApplicationException
{
    public ConsoleException(Process proc)
        : base($"{proc.StartInfo.FileName} {proc.StartInfo.Arguments}\n" +
               "Failure: " + proc.ExitCode + "\n" +
               "Output: " + proc.StandardOutput.ReadToEnd() + "\n")

    {
    }
}