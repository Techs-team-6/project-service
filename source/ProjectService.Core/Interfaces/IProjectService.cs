using ProjectService.Shared.Entities;
using ProjectService.Shared.Models;

namespace ProjectService.Core.Interfaces;

public interface IProjectService
{
    public Task<Uri> AddProjectAsync(ProjectCreateDto project);
    public Task<ProjectBuild> CreateVersionAsync(Guid projectId);
    Task<Project> GetProjectAsync(Guid projectId);
    public Stream GetProjectVersionArchive(Guid projectId, int buildId);
    public Stream GetProjectVersionArchive(Guid storageId);
    public string? UpdateBuildString(Guid projectId, string newBuildString);
    public GitInfo GetGitInfo();
    Task DeleteProject(Guid projectId);
}