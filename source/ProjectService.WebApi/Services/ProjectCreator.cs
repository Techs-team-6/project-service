﻿using ProjectService.WebApi.Interfaces;

namespace ProjectService.WebApi.Services;

public class ProjectCreator : IProjectCreator
{
    public async Task<string> Create(string path, string projectName)
    {
        string solutionString = $"new sln --name \"{projectName}\" --output \"{path}\"";
        var res = System.Diagnostics.Process.Start("dotnet", solutionString);
        await res.WaitForExitAsync();
        if(res.ExitCode != 0)
        {
            throw new Exception("Error creating solution");
        }
        
        string projectPath = Path.Combine(path, projectName);
        string projectString = $"new console --name \"{projectName}\" --output \"{projectPath}\"";
        Directory.CreateDirectory(projectPath);
        res = System.Diagnostics.Process.Start("dotnet", projectString);
        await res.WaitForExitAsync();
        if(res.ExitCode != 0)
        {
            throw new Exception("Error creating project");
        }

        string csprojPath = Path.Combine(projectPath, $"{projectName}.csproj");
        // string slnPath = Path.Combine(path, $"{projectName}.sln");
        // string addString = $"sln add \"{slnPath}\" \"{csprojPath}\"";
        //
        // res = System.Diagnostics.Process.Start("dotnet", addString);
        // await res.WaitForExitAsync();
        // if(res.ExitCode != 0)
        // {
        //     throw new Exception("Error adding project to solution");
        // }

        return csprojPath;
    }
}