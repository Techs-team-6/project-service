using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

    [Column(Order = 0), Key, ForeignKey("BuildId")]
    public int Id { get; private init; }
    [Column(Order = 1), Key, ForeignKey("ProjectId")]
    public Guid ProjectId { get; private init; }
    public string CommitId { get; private init; }
    public Guid StorageId { get; private init;  }
}