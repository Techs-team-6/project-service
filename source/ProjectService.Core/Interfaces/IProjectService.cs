using ProjectService.Core.Entities;
using ProjectService.Core.Models;

namespace ProjectService.Core.Interfaces;

public interface IProjectService
{
    public Guid AddProject(Project project);
    public Guid SaveVersion(LocalRepository localRepository);
    public ProjectVersionArchiveDto GetProjectVersionArchive(Guid projectId, Guid versionId);
}