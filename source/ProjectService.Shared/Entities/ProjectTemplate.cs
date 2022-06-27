namespace ProjectService.Shared.Entities;

public class ProjectTemplate
{
    public ProjectTemplate(Guid storageId, string name, string buildString)
    {
        Id = Guid.Empty;
        StorageId = storageId;
        Name = name;
        BuildString = buildString;
    }

    public Guid Id { get; private set; }
    public Guid StorageId { get; private set; }
    public string Name { get; private set; }
    public string BuildString { get; set; }
}