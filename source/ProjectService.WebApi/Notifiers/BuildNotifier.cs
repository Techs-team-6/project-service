using ProjectService.WebApi.Entities;
using ProjectService.WebApi.Interfaces;
using Server.API.Client;

namespace ProjectService.WebApi.Notifiers;

public class BuildNotifier : IBuildNotifier
{
    private readonly ProjectClient _client;

    public BuildNotifier(IConfigurationWrapper configuration)
    {
        _client = new ProjectClient(configuration.ServerAddress, new HttpClient());
    }

    public async Task NotifyOnBuildAsync(ProjectBuild build)
    {
        //FIXME: remove
        if(build.Id == 1)
            return;
        
        await _client.AddBuildAsync(build.ProjectId, build.Id.ToString(), build.StorageId);
    }
}