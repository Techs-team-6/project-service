using ProjectService.WebApi.Entities;

namespace ProjectService.WebApi.Interfaces;

public interface IProjectBuildService
{
    ProjectBuild CreateBuild(Project project);
    Stream GetBuild(ProjectBuild build);
}