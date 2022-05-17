using ProjectService.Core.Entities;
using ProjectService.Core.Models;

namespace ProjectService.Core.Interfaces;

public interface IGithubService
{
    Project SetupProject(ProjectCreateDto dto);
    void AddVersion(string branch = "master");
}