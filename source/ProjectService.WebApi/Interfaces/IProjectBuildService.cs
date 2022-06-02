using ProjectService.WebApi.Entities;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectBuildService
{
    Task<ProjectBuild> CreateBuildAsync(Project project);
    Stream GetBuild(ProjectBuild build);
}