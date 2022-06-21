using ProjectService.Shared.Entities;
using ProjectService.Shared.Models;

namespace ProjectService.Core.Interfaces;

public interface IProjectService
{
    Task<Uri> AddProjectAsync(ProjectCreateDto project);
    Task<ProjectBuild> CreateVersionAsync(Guid projectId);
    Task<Project> GetProjectAsync(Guid projectId);
    Stream GetProjectVersionArchive(Guid projectId, int buildId);
    Stream GetProjectVersionArchive(Guid storageId);
    string? UpdateBuildString(Guid projectId, string newBuildString);
    GitInfo GetGitInfo();
    Task DeleteProjectAsync(Guid projectId);
}