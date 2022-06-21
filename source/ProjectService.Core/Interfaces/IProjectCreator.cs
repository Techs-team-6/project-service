namespace ProjectService.Core.Interfaces;

public interface IProjectCreator
{
    Task<string> CreateAsync(string path, string projectName);
}