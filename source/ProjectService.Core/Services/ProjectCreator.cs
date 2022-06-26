using System.Security.Cryptography.X509Certificates;
using ProjectService.Core.Interfaces;
using ProjectService.Shared.Exceptions;

namespace ProjectService.Core.Services;

public class ProjectCreator : IProjectCreator
{
    private ITemplateService _templateService;
    private IArchiver _archiver;

    public ProjectCreator(ITemplateService templateService, IArchiver archiver)
    {
        _templateService = templateService;
        _archiver = archiver;
    }

    public async Task<string> CreateAsync(string path, string projectName, Guid templateId = default)
    {
        if (templateId == default)
        {
            return await CreateConsoleAsync(path, projectName);
        }

        await using Stream template = _templateService.GetTemplateZip(templateId);

        _archiver.DecompressStream(path, template);

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
        return csprojPath;
    }
}