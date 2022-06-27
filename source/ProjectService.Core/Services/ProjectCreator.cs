using System.Security.Cryptography.X509Certificates;
using NLog;
using ProjectService.Core.Interfaces;
using ProjectService.Shared.Exceptions;

namespace ProjectService.Core.Services;

public class ProjectCreator : IProjectCreator
{
    private ITemplateService _templateService;
    private IArchiver _archiver;
    private readonly Logger _logger;

    public ProjectCreator(ITemplateService templateService, IArchiver archiver, Logger logger)
    {
        _templateService = templateService;
        _archiver = archiver;
        _logger = logger;
    }

    public async Task<string> CreateAsync(string path, string projectName, Guid templateId = default)
    {
        if (templateId == default)
        {
            return await CreateConsoleAsync(path, projectName);
        }

        await using Stream template = _templateService.GetTemplateZip(templateId);

        _archiver.DecompressStream(path, template);

        _logger.Log(LogLevel.Info, "Project {0} created!", projectName);
        return path;
    }
    
    private async Task<string> CreateConsoleAsync(string path, string projectName)
    {
        string solutionString = $"new sln --name \"{projectName}\" --output \"{path}\"";
        var res = System.Diagnostics.Process.Start("dotnet", solutionString);
        await res.WaitForExitAsync();
        if(res.ExitCode != 0)
        {
            throw new GitException("Error creating solution");
        }
        
        string projectPath = Path.Combine(path, projectName);
        string projectString = $"new console --name \"{projectName}\" --output \"{projectPath}\"";
        Directory.CreateDirectory(projectPath);
        res = System.Diagnostics.Process.Start("dotnet", projectString);
        await res.WaitForExitAsync();
        if(res.ExitCode != 0)
        {
            throw new GitException("Error creating project");
        }

        string csprojPath = Path.Combine(projectPath, $"{projectName}.csproj");
        
        _logger.Log(LogLevel.Info, "Project {0} created!", projectName);
        return csprojPath;
    }
}