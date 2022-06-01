using ProjectService.WebApi.Enums;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectCreator
{
    public Task<string> Create(string path, string projectName);
}