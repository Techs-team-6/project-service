using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectService.WebApi.Entities;

public class ProjectBuild
{
    public ProjectBuild(int id, Guid storageId, Guid projectId)
    {
        StorageId = storageId;
        ProjectId = projectId;
        Id = id;
    }

#pragma warning disable CS8618
    protected ProjectBuild() { }
#pragma warning restore CS8618

    [Column(Order = 0), Key]
    public int Id { get; private init; }
    [Column(Order = 1), Key, ForeignKey("ProjectId")]
    public Guid ProjectId { get; private init; }
    public Guid StorageId { get; private init;  }
}