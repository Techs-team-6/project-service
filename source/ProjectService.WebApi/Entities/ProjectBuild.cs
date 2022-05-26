namespace ProjectService.WebApi.Entities;

public class ProjectBuild
{
    public ProjectBuild(int id, Guid storageId, Guid projectId, string commitId)
    {
        StorageId = storageId;
        ProjectId = projectId;
        CommitId = commitId;
        Id = id;
    }

#pragma warning disable CS8618
    protected ProjectBuild() { }
#pragma warning restore CS8618

    public int Id { get; private init; }
    public Guid ProjectId { get; private init; }
    public string CommitId { get; private init; }
    public Guid StorageId { get; private init;  }
}