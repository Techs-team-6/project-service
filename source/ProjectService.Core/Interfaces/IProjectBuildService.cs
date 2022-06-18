using ProjectService.Shared.Entities;

namespace ProjectService.Core.Interfaces;

public interface IProjectBuildService
{
    Task<ProjectBuild> CreateBuildAsync(Project project);
    Stream GetBuild(ProjectBuild build);
}