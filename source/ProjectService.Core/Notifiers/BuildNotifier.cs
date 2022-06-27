using ProjectService.Core.Interfaces;
using ProjectService.Shared.Entities;
using Server.API.Client;

namespace ProjectService.Core.Notifiers;

public class BuildNotifier : IBuildNotifier
{
    private readonly ProjectClient _client;

    public BuildNotifier(IConfigurationWrapper configuration)
    {
        _client = new ProjectClient(configuration.ServerAddress, new HttpClient());
    }

    public async Task NotifyOnBuildAsync(ProjectBuild build)
    {
        await _client.AddBuildAsync(build.ProjectId, build.Id.ToString(), build.StorageId);
    }
}