using System.Diagnostics;
using NLog;
using ProjectService.Core.Entities;
using ProjectService.Core.Interfaces;
using ProjectService.Shared.Exceptions;

namespace ProjectService.Core.Services;

public class ProjectBuilder : IBuilder
{
    private readonly Logger _logger;

    public ProjectBuilder()
    {
        _logger = LoggerKeeper.GetInstance().Logger;
    }

    public async Task<string> Build(string path, string buildString)
    {
        Guid id = Guid.Empty;
        do
        {
            id = Guid.NewGuid();
        } while (Directory.Exists(Path.Combine(path, id.ToString())));
        
        string buildpath = Path.Combine(path, id.ToString());
        
        Directory.CreateDirectory(buildpath);
        buildString = $"{buildString} -o \"{buildpath}\"";
        string cmd = buildString.Split(' ')[0];
        string args = buildString.Split(' ').Skip(1).Aggregate((a, b) => $"{a} {b}");
        var startInfo = new ProcessStartInfo()
        {
            FileName = cmd,
            Arguments = args,
            WorkingDirectory = path,
            UseShellExecute = true,
            CreateNoWindow = true
        };
        var proc = Process.Start(startInfo); 
        await proc.WaitForExitAsync();
        if(proc.ExitCode != 0)
        {
            throw new ConsoleException(proc);
        }
        
        return buildpath;
    }
}