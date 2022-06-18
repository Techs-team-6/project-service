namespace ProjectService.Core.Interfaces;

public interface IProjectCreator
{
    public Task<string> CreateAsync(string path, string projectName);
}