using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Models;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectService
{
    public string AddProject(Project project);
    public Guid CreateVersion(Guid projectId);
    public MemoryStream GetProjectVersionArchive(Guid projectId, int versionId);
}