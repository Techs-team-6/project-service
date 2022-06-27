using ProjectService.Shared.Entities;
using ProjectService.Shared.Models;

namespace ProjectService.Core.Interfaces;

public interface IGithubService
{
    Task<Project> CreateProjectAsync(ProjectCreateDto dto, Guid templateId = default);
    void CloneRepository(string path, Project project);
}