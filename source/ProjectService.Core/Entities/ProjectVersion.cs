namespace ProjectService.Core.Entities;

public class ProjectVersion
{
    public ProjectVersion(Guid storageId, Guid projectId, string commitId)
    {
        StorageId = storageId;
        ProjectId = projectId;
        CommitId = commitId;
        Id = Guid.Empty;
    }

#pragma warning disable CS8618
    protected ProjectVersion() { }
#pragma warning restore CS8618

    public Guid Id { get; private init; }
    public Guid ProjectId { get; private init; }
    public string CommitId { get; private init; }
    public Guid StorageId { get; private init;  }
}