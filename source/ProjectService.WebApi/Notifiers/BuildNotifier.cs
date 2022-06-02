using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Interfaces;
using Server.API.Client;

namespace ProjectService.WebApi.Notifiers;

public class BuildNotifier : IBuildNotifier
{
    private readonly ProjectClient _client;

    public BuildNotifier(ProjectClient client)
    {
        _client = client;
    }

    public async Task NotifyOnBuildAsync(ProjectBuild build)
    {
        await _client.AddBuildAsync(build.ProjectId, build.Id.ToString(), build.StorageId);
    }
}