using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectService
{
    public Uri AddProject(Project project);
    public ProjectBuild CreateVersion(Guid projectId);
    public MemoryStream GetProjectVersionArchive(Guid projectId, int buildId);
}