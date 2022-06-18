using ProjectService.Shared.Entities;

namespace ProjectService.Core.Interfaces;

public interface IBuildNotifier
{ 
    Task NotifyOnBuildAsync(ProjectBuild build);
}