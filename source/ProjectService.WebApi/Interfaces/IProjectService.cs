using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectService
{
    public Task<Uri> AddProjectAsync(ProjectCreateDto project);
    public Task<ProjectBuild> CreateVersionAsync(Guid projectId);
    public Stream GetProjectVersionArchive(Guid projectId, int buildId);
    public string? UpdateBuildString(Guid projectId, string newBuildString);
    public GitInfo GetGitInfo();
}