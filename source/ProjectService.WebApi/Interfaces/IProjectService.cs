using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectService
{
    public Task<Uri> AddProject(ProjectCreateDto project);
    public ProjectBuild CreateVersion(Guid projectId);
    public Stream GetProjectVersionArchive(Guid projectId, int buildId);
    public string? UpdateBuildString(Guid projectId, string newBuildString);
    public GitInfo GetGitInfo();
}