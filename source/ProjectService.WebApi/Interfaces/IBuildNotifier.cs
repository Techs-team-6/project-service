using ProjectService.WebApi.Entities;

namespace ProjectService.WebApi.Interfaces;

public interface IBuildNotifier
{ 
    Task NotifyOnBuildAsync(ProjectBuild build);
}